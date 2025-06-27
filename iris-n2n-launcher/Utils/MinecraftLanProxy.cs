using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using iris_n2n_launcher.TAP;

namespace iris_n2n_launcher.Utils;

public sealed class MinecraftLanProxy : IDisposable
{
    private static readonly Lazy<MinecraftLanProxy> _instance =
        new(() => new MinecraftLanProxy());
    public static MinecraftLanProxy Instance => _instance.Value;

    private readonly ConcurrentDictionary<string, DateTime> _servers = new();
    private readonly int _port = 4445;
    private readonly int _timeout = 10;
    private UdpClient? _listener;
    private UdpClient? _broadcaster;

    private readonly object _stateLock = new();
    private CancellationTokenSource? _cts;
    private Task? _listenTask;
    private Task? _broadcastTask;

    private readonly object _ipLock = new();
    public string? _broadcastIp;
    private List<string> _localIps = new();

    private MinecraftLanProxy() { }

    private void RefreshLocalIPs() // 不负责本地 ip 变更但是 N2N 不变更的情况
    {
        _localIps = TapNetworkManager.GetAllLocalIPs();
        _localIps.Add("127.0.0.1"); // 确保包含回环地址
    }
    private bool IsLocalMachineIp(IPAddress address)
    {
        return _localIps.Contains(address.ToString());
    }

    public void SetBroadcastIp(string ip, string subnetMask = "255.255.255.0")
    {
        if (ip == "")
        {
            return;
        }

        var ipBytes = IPAddress.Parse(ip).GetAddressBytes();
        var maskBytes = IPAddress.Parse(subnetMask).GetAddressBytes();

        if (ipBytes.Length != 4 || maskBytes.Length != 4)
            throw new ArgumentException("Invalid IPv4 address format");

        var broadcastBytes = new byte[4];
        for (int i = 0; i < 4; i++)
            broadcastBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);

        var newIp = new IPAddress(broadcastBytes).ToString();

        lock (_ipLock)
        {
            if (_broadcastIp == newIp) return;
            _broadcastIp = newIp;
        }

        RestartService();
    }

    public void Start()
    {
        lock (_stateLock)
        {
            if (_cts != null) return;

            RefreshLocalIPs();
            ValidateBroadcastIp();
            InitializeNetworkComponents();

            _cts = new CancellationTokenSource();
            _listenTask = Task.Run(() => ListenLoopAsync(_cts.Token));
            _broadcastTask = Task.Run(() => BroadcastLoopAsync(_cts.Token));
        }
    }

    public void Stop()
    {
        lock (_stateLock)
        {
            if (_cts == null) return;

            _cts.Cancel();
            CleanupNetworkComponents();

            _cts.Dispose();
            _cts = null;
        }
    }

    private void RestartService()
    {
        lock (_stateLock)
        {
            if (_cts == null) return;
            Stop();
            Start();
        }
    }

    private void ValidateBroadcastIp()
    {
        lock (_ipLock)
        {
            if (string.IsNullOrEmpty(_broadcastIp))
                throw new InvalidOperationException(
                    "Broadcast IP must be set before starting service");
        }
    }

    private void InitializeNetworkComponents()
    {
        _listener = new UdpClient(_port);
        _listener.Client.SetSocketOption(
            SocketOptionLevel.Socket,
            SocketOptionName.ReuseAddress,
            true);
        _listener.JoinMulticastGroup(IPAddress.Parse("224.0.2.60"));

        _broadcaster = new UdpClient { EnableBroadcast = true };
    }

    private void CleanupNetworkComponents()
    {
        _listener?.Close();
        _listener?.Dispose();
        _broadcaster?.Close();
        _broadcaster?.Dispose();

        _listener = null;
        _broadcaster = null;
    }

    private async Task ListenLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (_listener == null) break;

                var result = await _listener.ReceiveAsync(token);
                ProcessIncomingMessage(result);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch { }
        }
    }

    private void ProcessIncomingMessage(UdpReceiveResult result)
    {
        var message = Encoding.UTF8.GetString(result.Buffer);

        if (!IsLocalMachineIp(result.RemoteEndPoint.Address))
        {
            return; // 非本机IP的广播直接丢弃
        }

        if (!message.StartsWith("[MOTD]") || !message.Contains("[AD]"))
            return;

        var portSection = message.IndexOf("[AD]", StringComparison.Ordinal);
        var portStr = message.Substring(
            portSection + 4,
            message.IndexOf("[/AD]", portSection) - (portSection + 4));

        if (int.TryParse(portStr, out var port))
        {
            var key = message;
            _servers[key] = DateTime.Now;
        }
    }

    private async Task BroadcastLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var currentIp = GetCurrentBroadcastIp();
                await ProcessServerBroadcastAsync(currentIp);
                await Task.Delay(2000, token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch { }
        }
    }

    private string GetCurrentBroadcastIp()
    {
        lock (_ipLock)
        {
            return _broadcastIp ?? throw new InvalidOperationException(
                "Broadcast IP not initialized");
        }
    }

    private async Task ProcessServerBroadcastAsync(string broadcastIp)
    {
        var now = DateTime.Now;
        var expired = _servers.Where(kvp =>
            (now - kvp.Value).TotalSeconds > _timeout).ToList();

        foreach (var kvp in expired)
        {
            _servers.TryRemove(kvp.Key, out _);
        }

        foreach (var kvp in _servers)
        {
            var message = kvp.Key;
            if (message == null) continue;

            var data = Encoding.UTF8.GetBytes(message);

            await (_broadcaster?.SendAsync(
                data,
                data.Length,
                new IPEndPoint(IPAddress.Parse(broadcastIp), _port))
               ?? Task.CompletedTask);
        }
    }

    public void Dispose()
    {
        Stop();
        _servers.Clear();
        _listener?.Dispose();
        _broadcaster?.Dispose();
        _cts?.Dispose();
    }
}