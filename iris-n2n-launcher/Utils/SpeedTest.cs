using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace iris_n2n_launcher.Utils
{
    /// <summary>
    /// 网络测试结果模型
    /// </summary>
    public class NetworkQualityResult
    {
        public bool IsSuccess { get; set; }
        public string? Protocol { get; set; }
        public double AvgLatencyMs { get; set; } // 平均延迟
        public double JitterMs { get; set; }     // 抖动 (标准差)
        public double PacketLossRate { get; set; } // 丢包率 (0.0 - 1.0)
        public int PacketsSent { get; set; }
        public int PacketsReceived { get; set; }
        public string? Message { get; set; }

        public string QualityRating
        {
            get
            {
                if (!IsSuccess) return "测试失败";
                if (PacketLossRate > 0.10) return "极差 (丢包严重)";
                if (PacketLossRate > 0.01) return "差 (有丢包)";
                if (JitterMs > 30) return "不稳定 (抖动大)";
                if (AvgLatencyMs < 50) return "极好";
                if (AvgLatencyMs < 100) return "良好";
                if (AvgLatencyMs < 200) return "一般";
                return "高延迟";
            }
        }
    }

    internal class SpeedTest
    {
        private static TcpListener? _serverTcpListener;
        private static UdpClient? _serverUdpListener;
        private static CancellationTokenSource? _serverCts;
        private static int _serverPort = -1;
        private static bool _isServerRunning = false;

        /// <summary>
        /// 开启 Echo Server 
        /// </summary>
        /// <param name="port">指定端口 传入 0 则由系统自动分配空闲端口</param>
        /// <returns>实际监听的端口号</returns>
        public static int StartServer(int port = 0)
        {
            if (_isServerRunning) return _serverPort;

            try
            {
                _serverCts = new CancellationTokenSource();

                _serverTcpListener = new TcpListener(IPAddress.Any, port);
                _serverTcpListener.Start();

                _serverPort = ((IPEndPoint)_serverTcpListener.LocalEndpoint).Port;

                _serverUdpListener = new UdpClient(_serverPort);

                _isServerRunning = true;

                Task.Run(() => RunTcpServerLoop(_serverCts.Token));
                Task.Run(() => RunUdpServerLoop(_serverCts.Token));

                return _serverPort;
            }
            catch (Exception)
            {
                StopServer(); // 启动失败则清理
                throw;
            }
        }

        /// <summary>
        /// 关闭 Echo Server
        /// </summary>
        public static void StopServer()
        {
            _isServerRunning = false;
            _serverCts?.Cancel();

            try { _serverTcpListener?.Stop(); } catch { }
            try { _serverUdpListener?.Close(); } catch { }

            _serverTcpListener = null;
            _serverUdpListener = null;
            _serverCts?.Dispose();
            _serverCts = null;
            _serverPort = -1;
        }

        /// <summary>
        /// 获取服务器状态
        /// </summary>
        /// <returns>是否运行中 端口号</returns>
        public static (bool IsRunning, int Port) GetServerStatus()
        {
            return (_isServerRunning, _serverPort);
        }


        private static async Task RunUdpServerLoop(CancellationToken token)
        {
            if (_serverUdpListener == null) return;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var result = await _serverUdpListener.ReceiveAsync();
                    _ = _serverUdpListener.SendAsync(result.Buffer, result.Buffer.Length, result.RemoteEndPoint);
                }
            }
            catch {  }
        }

        private static async Task RunTcpServerLoop(CancellationToken token)
        {
            if (_serverTcpListener == null) return;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // 接受连接时也检查 token
                    var clientTask = _serverTcpListener.AcceptTcpClientAsync();
                    var completedTask = await Task.WhenAny(clientTask, Task.Delay(-1, token));

                    if (completedTask == clientTask)
                    {
                        var client = await clientTask;
                        _ = HandleTcpClient(client, token);
                    }
                }
            }
            catch { }
        }

        private static async Task HandleTcpClient(TcpClient client, CancellationToken token)
        {
            using (client)
            using (var stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                try
                {
                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token)) != 0)
                    {
                        await stream.WriteAsync(buffer, 0, bytesRead, token);
                    }
                }
                catch { }
            }
        }


        private const int PayloadSize = 32;
        private const int DefaultTestCount = 20;
        private const int PacketIntervalMs = 50;
        private const int RecvTimeoutMs = 1000;

        /// <summary>
        /// 执行 UDP 稳定性测试
        /// </summary>
        public static async Task<NetworkQualityResult> RunUdpTestAsync(string host, int port, int count = DefaultTestCount)
        {
            var result = new NetworkQualityResult { Protocol = "UDP", PacketsSent = count };
            var latencies = new List<double>();
            byte[] payload = new byte[PayloadSize];
            new Random().NextBytes(payload);

            using (var client = new UdpClient())
            {
                try
                {
                    var addresses = await Dns.GetHostAddressesAsync(host);
                    var targetEp = new IPEndPoint(addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork) ?? addresses.First(), port);

                    client.Client.ReceiveTimeout = RecvTimeoutMs;
                    client.Connect(targetEp);

                    for (int i = 0; i < count; i++)
                    {
                        var stopwatch = Stopwatch.StartNew();
                        try
                        {
                            await client.SendAsync(payload, payload.Length);

                            var asyncResult = client.BeginReceive(null, null);
                            bool received = asyncResult.AsyncWaitHandle.WaitOne(RecvTimeoutMs);

                            if (received)
                            {
                                IPEndPoint? remote = null;
                                byte[] data = client.EndReceive(asyncResult, ref remote);
                                stopwatch.Stop();

                                if (data.Length == payload.Length)
                                {
                                    latencies.Add(stopwatch.Elapsed.TotalMilliseconds);
                                    result.PacketsReceived++;
                                }
                            }
                        }
                        catch { }

                        await Task.Delay(PacketIntervalMs);
                    }

                    CalculateMetrics(result, latencies);
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = ex.Message;
                }
            }

            return result;
        }

        /// <summary>
        /// 执行 TCP 稳定性测试
        /// </summary>
        public static async Task<NetworkQualityResult> RunTcpTestAsync(string host, int port, int count = 10)
        {
            var result = new NetworkQualityResult { Protocol = "TCP", PacketsSent = count };
            var latencies = new List<double>();
            byte[] payload = new byte[PayloadSize];
            new Random().NextBytes(payload);

            IPAddress ip;
            try
            {
                var addresses = await Dns.GetHostAddressesAsync(host);
                ip = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork) ?? addresses.First();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "DNS Error: " + ex.Message;
                return result;
            }

            try
            {
                using (var client = new TcpClient())
                {
                    // 设置连接超时
                    var connectTask = client.ConnectAsync(ip, port);
                    if (await Task.WhenAny(connectTask, Task.Delay(2000)) != connectTask)
                    {
                        throw new SocketException((int)SocketError.TimedOut);
                    }
                    await connectTask; // 确保异常被抛出

                    var stream = client.GetStream();
                    stream.ReadTimeout = RecvTimeoutMs;
                    stream.WriteTimeout = RecvTimeoutMs;

                    byte[] buffer = new byte[PayloadSize];

                    for (int i = 0; i < count; i++)
                    {
                        var sw = Stopwatch.StartNew();
                        try
                        {
                            await stream.WriteAsync(payload);

                            int totalRead = 0;
                            while (totalRead < PayloadSize)
                            {
                                int read = await stream.ReadAsync(buffer, totalRead, PayloadSize - totalRead);
                                if (read == 0) break;
                                totalRead += read;
                            }

                            sw.Stop();

                            if (totalRead == PayloadSize)
                            {
                                latencies.Add(sw.Elapsed.TotalMilliseconds);
                                result.PacketsReceived++;
                            }
                        }
                        catch { break; }

                        await Task.Delay(PacketIntervalMs);
                    }
                }

                CalculateMetrics(result, latencies);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return result;
        }

        private static void CalculateMetrics(NetworkQualityResult result, List<double> latencies)
        {
            if (latencies.Count == 0)
            {
                result.AvgLatencyMs = 0;
                result.JitterMs = 0;
                result.PacketLossRate = 1.0;
                return;
            }

            double avg = latencies.Average();
            result.AvgLatencyMs = Math.Round(avg, 2);

            double sumSquares = latencies.Sum(d => Math.Pow(d - avg, 2));
            double stdDev = Math.Sqrt(sumSquares / (latencies.Count - 1));
            result.JitterMs = latencies.Count > 1 ? Math.Round(stdDev, 2) : 0;

            result.PacketLossRate = 1.0 - ((double)result.PacketsReceived / result.PacketsSent);
            result.PacketLossRate = Math.Round(result.PacketLossRate, 4);
        }
    }
}