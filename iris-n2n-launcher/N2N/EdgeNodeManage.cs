using iris_n2n_launcher.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;
using static iris_n2n_launcher.Utils.FirewallHelper;
using iris_n2n_launcher.TAP;
using iris_n2n_launcher.Config;
using System.Text;

namespace iris_n2n_launcher.N2N;

public class EdgeNodeManage : IDisposable
{
    private readonly ExeHelper _exeHelper = ExeHelper.Instance;
    private readonly ConcurrentDictionary<string, EdgeNodeInfo> _activeNodes = new();
    private readonly CancellationTokenSource _monitorCts = new();
    private readonly TapNetworkManager TapNetworkManager = new();
    private readonly ConfigManager config = ConfigManager.Instance;
    public readonly List<string> usedAdapters = [];
    private static EdgeNodeManage? _instance;
    private static readonly object _lock = new();

    readonly static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "N2N");
    readonly static string EXE = Path.Combine(path, "edge.exe");
    private Task _monitorTask;

    public Process? _broadcastRepair = null;

    public static EdgeNodeManage Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new EdgeNodeManage();
            }
        }
    }

    private EdgeNodeManage()
    {
        StartMonitor();
    }

    /// <summary>
    /// 启动一个新的n2n节点
    /// </summary>
    /// <param name="id">唯一标识符</param>
    /// <param name="parameters">启动参数</param>
    /// <returns>是否启动成功</returns>
    public async Task<int> StartNodeAsync(string id, N2NConfiguration parameters)
    {
        await TapNetworkManager.EnsureTapAdapterExistsAsync();

        if (_activeNodes.IsEmpty)
        {
            await FirewallManager.AllowProgramAsync("n2n-edge", EXE);

            if (config.LoadConfig<Configuration>("config").BroadcastRepair)
            {
                var WinIPBroadcastEXE = Path.Combine(path, "WinIPBroadcast.exe");
                await FirewallManager.AllowProgramAsync("n2n-WinIPBroadcast", WinIPBroadcastEXE);
                _broadcastRepair = _exeHelper.CreateProcess(WinIPBroadcastEXE, "run");
            }
        }

        if (_activeNodes.ContainsKey(id))
        {
            return 10; // 已存在相同ID的节点
        }

        int managementPort = PortUtility.GetRandomUnusedPort(5000, 8000);

        var tapAdapters = TapNetworkManager.ShowNetworkInterfaceMessage(AdapterType.TAP);

        var availableAdapter = tapAdapters.FirstOrDefault(ad => !usedAdapters.Contains(ad.Id));

        if (availableAdapter == null)
        {
            return 11; // 没有可用的未使用网卡
        }

        if (parameters.DeviceName == "" || parameters.DeviceName == null) // 自动分配
        {
            parameters.DeviceName = availableAdapter.Id;
        }

        usedAdapters.Add(parameters.DeviceName);

        var fullParameters = $"-t {managementPort} {parameters}";

        var process = _exeHelper.CreateProcess(EXE, fullParameters);

        if (process == null)
        {
            usedAdapters.Remove(parameters.DeviceName);
            return 12; // 进程创建失败
        }

        var udpManager = new EdgeUdpManage(managementPort);

        var nodeInfo = new EdgeNodeInfo
        {
            Id = id,
            Process = process,
            UdpManager = udpManager,
            ManagementPort = managementPort,
            Parameters = parameters,
            StartTime = DateTime.Now
        };

        FetchNodeInfo(nodeInfo); // 立即尝试获取信息

        if (!_activeNodes.TryAdd(id, nodeInfo))
        {
            usedAdapters.Remove(parameters.DeviceName);
            return 13; // 信息添加失败
        }

        return 0;
    }

    /// <summary>
    /// 停止指定节点
    /// </summary>
    public bool StopNode(string id)
    {
        if (_activeNodes.TryRemove(id, out var nodeInfo))
        {

            if (_activeNodes.IsEmpty)
            {
                if (_broadcastRepair != null)
                {
                    _broadcastRepair.Kill();
                    _broadcastRepair = null;
                }
            }

            try
            {
                usedAdapters.Remove(nodeInfo.Parameters.DeviceName);
                nodeInfo.UdpManager.Dispose();
                nodeInfo.Process.Kill();
                return true;
            }
            catch
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>
    /// 获取所有活动节点
    /// </summary>
    public ConcurrentDictionary<string, EdgeNodeInfo> GetActiveNodes()
    {
        return _activeNodes;
    }

    /// <summary>
    /// 获取指定节点信息
    /// </summary>
    public EdgeNodeInfo? GetNodeInfo(string id)
    {
        _activeNodes.TryGetValue(id, out var nodeInfo);
        return nodeInfo;
    }

    /// <summary>
    /// 启动节点监控
    /// </summary>
    private void StartMonitor()
    {
        _monitorTask = Task.Run(async () =>
        {

            int delay = 1000;
            await Task.Delay(delay);

            while (!_monitorCts.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(delay, _monitorCts.Token);

                    foreach (var node in _activeNodes)
                    {
                        if (node.Value.Process.HasExited)
                        {
                            _activeNodes.TryRemove(node.Key, out _);
                            continue;
                        }

                        FetchNodeInfo(node.Value);
                    }

                    if (delay < 10000)
                    {
                        delay += 1000;
                    }
                }
                catch (TaskCanceledException)
                {

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Monitor error: {ex.Message}");
                }
            }
        });
    }

    /// <summary>
    /// 获取节点信息
    /// </summary>
    private void FetchNodeInfo(EdgeNodeInfo nodeInfo)
    {
        try
        {
            var stauts = nodeInfo.UdpManager.Read("info");

            if (!stauts)
            {
                return;
            }

            foreach (var info in nodeInfo.UdpManager.GetReceivedData()) {
                if (info.version == "iris")
                {
                    nodeInfo.Status = info;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Fetch node info error: {ex.Message}");
        }
    }

    public List<EdgeNodeInfo> FetchNodesInfo()
    {
        return [.. _activeNodes.Values];
    }

    /// <summary>
    /// 验证指定的网卡标识符是否已经在使用
    /// </summary>
    /// <param name="adapterId">网卡标识符</param>
    /// <returns>是否已经在使用</returns>
    public bool IsAdapterUsed(string adapterId)
    {
        return usedAdapters.Contains(adapterId);
    }

    public void Dispose()
    {
        _monitorCts.Cancel();
        _monitorTask?.Wait();

        foreach (var node in _activeNodes)
        {
            try
            {
                node.Value.UdpManager.Dispose();
                node.Value.Process.Kill();
            }
            catch
            {

            }
        }
        _activeNodes.Clear();
        _monitorCts.Dispose();
    }
}

public class EdgeNodeInfo
{
    public string Id { get; set; }
    public Process Process { get; set; }
    public EdgeUdpManage UdpManager { get; set; }
    public int ManagementPort { get; set; }
    public N2NConfiguration Parameters { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime LastUpdateTime { get; set; }
    public dynamic Status { get; set; }
}