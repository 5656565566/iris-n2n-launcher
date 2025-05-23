using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace iris_n2n_launcher.Utils;

internal class NetworkTool
{
    /// <summary>
    /// IP校验
    /// </summary>
    /// <param name="ipAddress">ip</param>
    /// <returns></returns>
    public static bool ValidateIPAddress(string? ipAddress)
    {
        if (ipAddress == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(ipAddress))
            return false;

        string[] parts = ipAddress.Split(':');

        if (parts.Length == 2)
        {
            if (!int.TryParse(parts[1], out int port) || port < 0 || port > 65535)
                return false;
        }
        else if (parts.Length > 2)
        {
            return false;
        }

        return IPAddress.TryParse(parts[0], out _);
    }

    /// <summary>
    /// 地址校验
    /// </summary>
    /// <param name="address">地址</param>
    /// <returns></returns>
    public static bool ValidateUrlAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return false;

        string[] parts = address.Split(':');

        if (parts.Length == 2)
        {
            if (!int.TryParse(parts[1], out int port) || port < 0 || port > 65535)
                return false;
        }
        else if (parts.Length > 2)
        {
            return false;
        }

        string host = parts[0];

        if (host.Length < 3)
            return false;

        if (IPAddress.TryParse(host, out _))
            return true;

        return Uri.CheckHostName(host) != UriHostNameType.Unknown;
    }

    /// <summary>
    /// Ping 主机
    /// </summary>
    /// <param name="host">主机地址</param>
    /// <param name="count">次数</param>
    /// <param name="timeout">超时时间</param>
    /// <returns></returns>
    public static async Task<List<int>> PingHostAsync(string host, int count = 4, int timeout = 1000)
    {
        var results = new List<int>();

        using (var ping = new Ping())
        {
            for (int i = 0; i < count; i++)
            {
                try
                {
                    var reply = await ping.SendPingAsync(host, timeout);
                    results.Add((int)reply.RoundtripTime);
                }
                catch (PingException)
                {
                    results.Add(-1);
                }
            }
        }

        return results;
    }

    /// <summary>
    /// 带回调函数的 PING 用于 UI，返回 [平均延迟, 丢包率]
    /// </summary>
    /// <param name="host">地址</param>
    /// <param name="callback">回调函数（可选）</param>
    /// <param name="count">Ping次数</param>
    /// <param name="timeout">超时时间</param>
    /// <returns>[平均延迟, 丢包率]</returns>
    public static async Task<double[]> PingHostWithCallbackAsync(
        string host,
        Action<int> callback,
        int count = 4,
        int timeout = 1000)
    {
        int successCount = 0;
        long totalRoundtripTime = 0;
        int lostCount = 0;

        using (var ping = new Ping())
        {
            for (int i = 0; i < count; i++)
            {
                try
                {
                    var reply = await ping.SendPingAsync(host, timeout);
                    if (reply.Status == IPStatus.Success)
                    {
                        successCount++;
                        totalRoundtripTime += reply.RoundtripTime;
                        callback?.Invoke((int)reply.RoundtripTime);
                    }
                    else
                    {
                        lostCount++;
                        callback?.Invoke(-1);
                    }
                }
                catch (PingException)
                {
                    lostCount++;
                    callback?.Invoke(-1);
                }
            }
        }

        // 计算平均延迟（仅成功请求）
        double avgLatency = successCount > 0
            ? Math.Round((double)totalRoundtripTime / successCount, 2)
            : -1;

        // 计算丢包率（0.00 格式）
        double lossRate = Math.Round((double)lostCount / count * 100, 2);

        return [avgLatency, lossRate];
    }

    /// <summary>
    /// 检测服务器是否是有效的n2n v3服务器
    /// </summary>
    /// <param name="host">主机地址</param>
    /// <param name="port">端口号</param>
    /// <param name="timeout">超时时间(毫秒)</param>
    /// <returns>包含是否有效和延迟的元组</returns>
    public static async Task<(bool IsValid, int Latency)> CheckN2Nv3ServerUdpAsync(string host, int port = 5644, int timeout = 8000)
    {
        try
        {
            using var udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = timeout;

            if (!IPAddress.TryParse(host, out IPAddress? ipAddress))
            {
                var hostEntry = await Dns.GetHostEntryAsync(host);
                ipAddress = hostEntry.AddressList[0];
            }
            var endPoint = new IPEndPoint(ipAddress, port);

            byte[] probePacket = ConstructSupernodeProbePacket();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            await udpClient.SendAsync(probePacket, probePacket.Length, endPoint);

            var receiveTask = udpClient.ReceiveAsync();
            var timeoutTask = Task.Delay(timeout);
            var completedTask = await Task.WhenAny(receiveTask, timeoutTask);

            stopwatch.Stop();
            int latency = (int)stopwatch.ElapsedMilliseconds;

            if (completedTask == timeoutTask)
            {
                return (false, -1); // 超时
            }

            var result = await receiveTask;
            byte[] response = result.Buffer;

            if (response.Length > 0)
            {
                return (true, latency);
            }
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
        {
            return (false, -1);
        }
        catch (Exception)
        {
            
        }

        return (false, -1);
    }

    /// <summary>
    /// Construct n2n v3 REGISTER_SUPER probe packet
    /// </summary>
    /// <summary>
    /// 构造n2n v3超级节点探测包 (REGISTER_SUPER)
    /// </summary>
    private static byte[] ConstructSupernodeProbePacket()
    {
        string base64String = "AwIABXRlc3QAAAAAAAAAAAAAAAAAAAAAP0dcEwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABABByW5GmcX3Ca4DZunL/XqH/AAAAAA=="; // 固定测试数据包

        return Convert.FromBase64String(base64String);
    }
}