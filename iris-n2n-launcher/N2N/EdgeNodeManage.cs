using iris_n2n_launcher.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;
using static iris_n2n_launcher.Utils.FirewallHelper;
using iris_n2n_launcher.TAP;
using iris_n2n_launcher.Config;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace iris_n2n_launcher.N2N;

public enum NodeState
{
    Created,
    ProcessStarted,
    TapReady,
    Connecting,
    Running,
    RunningWithWarning,
    AuthFailed,
    ConfigError,
    TapError,
    NetworkError,
    Timeout,
    Exited,
    StartFailed
}

public enum NodeStartError
{
    None,
    DuplicateNode,
    /// <summary>
    /// 找不到可用的 TAP 虚拟网卡
    /// </summary>
    NoAvailableTapAdapter,
    /// <summary>
    /// 配置参数无效
    /// </summary>
    InvalidConfiguration,
    /// <summary>
    /// 进程创建失败
    /// </summary>
    ProcessStartFailed,
    /// <summary>
    /// 节点注册到内部管理列表失败
    /// </summary>
    NodeRegistrationFailed,
    /// <summary>
    /// 未预期的异常
    /// </summary>
    UnexpectedException
}

public sealed class NodeStartResult
{
    public bool Success => Error == NodeStartError.None;
    public NodeStartError Error { get; init; }
    public string? Message { get; init; }
    public EdgeNodeInfo? Node { get; init; }

    public static NodeStartResult Ok(EdgeNodeInfo node) => new()
    {
        Error = NodeStartError.None,
        Node = node
    };

    public static NodeStartResult Fail(NodeStartError error, string message) => new()
    {
        Error = error,
        Message = message
    };
}

public enum NodeLogSeverity
{
    Info,
    Warning,
    Error
}

public enum NodeLogCode
{
    Unknown,
    SupernodeNotResponding,
    AuthAddressConflict,
    TapDeviceNotFound,
    TapDeviceReopenFailed,
    TapDriverNotInstalled,
    WinsockInitFailed,
    SocketCreateFailed,
    RecvFromFailed,
    SocketSetupFailed
}

public sealed class NodeLogMatchResult
{
    public NodeLogSeverity Severity { get; init; }
    public NodeLogCode Code { get; init; }
    public string Message { get; init; } = string.Empty;
    public string RawLog { get; init; } = string.Empty;
    public NodeState? SuggestedState { get; init; }
}

public sealed class EdgeRuntimeStatus
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement> RawData { get; set; } = [];

    [JsonIgnore]
    public string? Ip4Addr => GetString("ip4addr");

    [JsonIgnore]
    public string? MacAddr => GetString("macaddr");

    [JsonIgnore]
    public string? Ip4Netmask => GetString("ip4netmask");

    [JsonIgnore]
    public string? Version => GetString("version");

    public string? GetString(string propertyName)
    {
        if (!RawData.TryGetValue(propertyName, out var value))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            _ => value.ToString()
        };
    }

    public static EdgeRuntimeStatus? FromObject(object status)
    {
        try
        {
            if (status is EdgeRuntimeStatus typedStatus)
            {
                return typedStatus;
            }

            var json = JsonSerializer.Serialize(status);
            return JsonSerializer.Deserialize<EdgeRuntimeStatus>(json);
        }
        catch
        {
            return null;
        }
    }
}

public class EdgeNodeManage : IDisposable
{
    private readonly ExeHelper _exeHelper = ExeHelper.Instance;
    private readonly ConcurrentDictionary<string, EdgeNodeInfo> _activeNodes = new();
    private readonly CancellationTokenSource _monitorCts = new();
    private readonly TapNetworkManager TapNetworkManager = new();
    private readonly ConfigManager config = ConfigManager.Instance;
    private readonly SemaphoreSlim _nodeLifecycleLock = new(1, 1);
    public readonly List<string> usedAdapters = [];
    private static EdgeNodeManage? _instance;
    private static readonly object _lock = new();

    readonly static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "N2N");
    readonly static string EXE = Path.Combine(path, "edge.exe");
    private Task? _monitorTask;

    public Process? _broadcastRepair = null;
    private static readonly IReadOnlyList<(Regex Pattern, NodeLogSeverity Severity, NodeLogCode Code, string Message, NodeState? SuggestedState)> _logRules =
    [
        (new Regex("supernode not responding, now trying", RegexOptions.IgnoreCase | RegexOptions.Compiled), NodeLogSeverity.Warning, NodeLogCode.SupernodeNotResponding, "服务器无响应 正在重试", NodeState.RunningWithWarning),
        (new Regex("authentication error, MAC or IP address already in use", RegexOptions.IgnoreCase | RegexOptions.Compiled), NodeLogSeverity.Error, NodeLogCode.AuthAddressConflict, "IP 或 MAC 地址冲突", NodeState.AuthFailed),
        (new Regex("Cannot find tap device", RegexOptions.IgnoreCase | RegexOptions.Compiled), NodeLogSeverity.Error, NodeLogCode.TapDeviceNotFound, "找不到 TAP 虚拟网卡", NodeState.TapError),
        (new Regex("Reopening TAP device .* failed", RegexOptions.IgnoreCase | RegexOptions.Compiled), NodeLogSeverity.Error, NodeLogCode.TapDeviceReopenFailed, "TAP 网卡被占用或异常", NodeState.TapError),
        (new Regex("Unable to read registry|Could not determine adapter MAC", RegexOptions.IgnoreCase | RegexOptions.Compiled), NodeLogSeverity.Error, NodeLogCode.TapDriverNotInstalled, "TAP 驱动未正确安装", NodeState.TapError),
        (new Regex("FATAL ERROR: unable to initialise Winsock", RegexOptions.IgnoreCase | RegexOptions.Compiled), NodeLogSeverity.Error, NodeLogCode.WinsockInitFailed, "系统网络组件异常 (Winsock)", NodeState.NetworkError),
        (new Regex("Unable to create socket|failed to open main socket", RegexOptions.IgnoreCase | RegexOptions.Compiled), NodeLogSeverity.Error, NodeLogCode.SocketCreateFailed, "无法创建网络 Socket", NodeState.NetworkError),
        (new Regex(@"recvfrom\(\) failed", RegexOptions.IgnoreCase | RegexOptions.Compiled), NodeLogSeverity.Warning, NodeLogCode.RecvFromFailed, "本地网络连接异常断开", NodeState.RunningWithWarning),
        (new Regex("socket setup failed", RegexOptions.IgnoreCase | RegexOptions.Compiled), NodeLogSeverity.Error, NodeLogCode.SocketSetupFailed, "网络初始化失败", NodeState.NetworkError)
    ];

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

    private EdgeNodeManage() => StartMonitor();

    /// <summary>
    /// 启动一个新的n2n节点
    /// </summary>
    /// <param name="id">唯一标识符</param>
    /// <param name="parameters">启动参数</param>
    /// <returns>是否启动成功</returns>
    public async Task<NodeStartResult> StartNodeAsync(string id, N2NConfiguration parameters)
    {
        await _nodeLifecycleLock.WaitAsync();
        try
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
                return NodeStartResult.Fail(NodeStartError.DuplicateNode, "检测到重复启动...");
            }

            var tapAdapters = TapNetworkManager.ShowNetworkInterfaceMessage(AdapterType.TAP);
            var availableAdapter = tapAdapters.FirstOrDefault(ad => !usedAdapters.Contains(ad.Id));

            if (availableAdapter == null)
            {
                return NodeStartResult.Fail(NodeStartError.NoAvailableTapAdapter, "未能自动发现可用网卡...");
            }

            string originalDeviceName = parameters.DeviceName;
            List<string> deviceNameCandidates = [];

            if (string.IsNullOrWhiteSpace(originalDeviceName))
            {
                deviceNameCandidates.Add(availableAdapter.Id);
            }
            else
            {
                // 1. 尝试原样使用（可能是正确的 UUID 或名称）
                deviceNameCandidates.Add(originalDeviceName);

                // 2. 尝试转换为 UUID（如果用户填的是名称或描述）
                var matchedAdapter = tapAdapters.FirstOrDefault(ad =>
                    ad.Name.Equals(originalDeviceName, StringComparison.OrdinalIgnoreCase) ||
                    ad.Description.Equals(originalDeviceName, StringComparison.OrdinalIgnoreCase));

                if (matchedAdapter != null && matchedAdapter.Id != originalDeviceName)
                {
                    deviceNameCandidates.Add(matchedAdapter.Id);
                }

                // 3. 尝试转换为名称（如果用户填的是 UUID 或描述）
                if (matchedAdapter != null && matchedAdapter.Name != originalDeviceName)
                {
                    deviceNameCandidates.Add(matchedAdapter.Name);
                }
                else
                {
                    var matchedById = tapAdapters.FirstOrDefault(ad => ad.Id.Equals(originalDeviceName, StringComparison.OrdinalIgnoreCase));
                    if (matchedById != null && matchedById.Name != originalDeviceName)
                    {
                        deviceNameCandidates.Add(matchedById.Name);
                    }
                }
            }

            // 去重
            deviceNameCandidates = [.. deviceNameCandidates.Distinct()];

            NodeStartResult? lastResult = null;

            foreach (var candidate in deviceNameCandidates)
            {
                parameters.DeviceName = candidate;
                lastResult = StartNodeInternal(id, parameters);

                if (lastResult.Success)
                {
                    // 启动成功，等待一小段时间检查是否会因为找不到网卡而退出
                    await Task.Delay(1000);
                    var nodeInfo = GetNodeInfo(id);
                    if (nodeInfo != null && nodeInfo.State != NodeState.TapError)
                    {
                        return lastResult; // 真正成功
                    }
                    else
                    {
                        // 发生了 TapError，停止节点，准备下一次重试
                        await StopNodeCoreAsync(id, true);
                    }
                }
                else
                {
                    // 启动失败（比如参数错误、进程拉起失败），直接返回，不重试网卡
                    return lastResult;
                }
            }

            // 所有候选名称都试过了，还是失败
            parameters.DeviceName = originalDeviceName; // 恢复原值
            return lastResult ?? NodeStartResult.Fail(NodeStartError.UnexpectedException, "所有网卡标识符尝试均失败");
        }
        catch (Exception ex)
        {
            return NodeStartResult.Fail(NodeStartError.UnexpectedException, ex.Message);
        }
        finally
        {
            _nodeLifecycleLock.Release();
        }
    }

    private NodeStartResult StartNodeInternal(string id, N2NConfiguration parameters)
    {
        int managementPort = PortUtility.GetRandomUnusedPort(5000, 8000);

        usedAdapters.Add(parameters.DeviceName);

        if (!parameters.TryBuildArguments(out var edgeArguments, out var validationErrors))
        {
            usedAdapters.Remove(parameters.DeviceName);
            return NodeStartResult.Fail(
                NodeStartError.InvalidConfiguration,
                string.Join(Environment.NewLine, validationErrors));
        }

        var fullParameters = $"-t {managementPort} {edgeArguments}";

        var pendingLogs = new List<(NodeLogSeverity Severity, string Line)>();
        Process? pendingExitedProcess = null;
        object pendingLock = new();

        EdgeNodeInfo? nodeInfo = null;
        var process = _exeHelper.CreateProcess(
            EXE,
            fullParameters,
            encoding: Encoding.UTF8,
            handlers: new ProcessOutputHandlers
            {
                OnOutput = line =>
                {
                    lock (pendingLock)
                    {
                        if (nodeInfo != null)
                        {
                            HandleProcessLog(nodeInfo, line, NodeLogSeverity.Info);
                        }
                        else
                        {
                            pendingLogs.Add((NodeLogSeverity.Info, line));
                        }
                    }
                },
                OnError = line =>
                {
                    lock (pendingLock)
                    {
                        if (nodeInfo != null)
                        {
                            HandleProcessLog(nodeInfo, line, NodeLogSeverity.Error);
                        }
                        else
                        {
                            pendingLogs.Add((NodeLogSeverity.Error, line));
                        }
                    }
                },
                OnExit = exitedProcess =>
                {
                    lock (pendingLock)
                    {
                        if (nodeInfo != null)
                        {
                            HandleProcessExit(nodeInfo, exitedProcess);
                        }
                        else
                        {
                            pendingExitedProcess = exitedProcess;
                        }
                    }
                }
            });

        if (process == null)
        {
            usedAdapters.Remove(parameters.DeviceName);
            return NodeStartResult.Fail(NodeStartError.ProcessStartFailed, "进程创建失败...");
        }

        var udpManager = new EdgeUdpManage(managementPort);

        nodeInfo = new EdgeNodeInfo
        {
            Id = id,
            Process = process,
            UdpManager = udpManager,
            ManagementPort = managementPort,
            Parameters = parameters,
            StartTime = DateTime.Now,
            LastUpdateTime = DateTime.Now,
            LastStatusAt = DateTime.Now,
            State = NodeState.ProcessStarted,
            LastStatusText = "进程已启动",
            IsHealthy = true
        };

        lock (pendingLock)
        {
            foreach (var (Severity, Line) in pendingLogs)
            {
                HandleProcessLog(nodeInfo, Line, Severity);
            }

            if (pendingExitedProcess != null)
            {
                HandleProcessExit(nodeInfo, pendingExitedProcess);
            }
        }

        FetchNodeInfo(nodeInfo); // 立即尝试获取信息

        if (!_activeNodes.TryAdd(id, nodeInfo))
        {
            usedAdapters.Remove(parameters.DeviceName);
            return NodeStartResult.Fail(NodeStartError.NodeRegistrationFailed, "添加到节点列表失败...");
        }

        return NodeStartResult.Ok(nodeInfo);
    }

    /// <summary>
    /// 停止指定节点
    /// </summary>
    public bool StopNode(string id)
    {
        _nodeLifecycleLock.Wait();
        try
        {
            return StopNodeCoreAsync(id, false).GetAwaiter().GetResult();
        }
        finally
        {
            _nodeLifecycleLock.Release();
        }
    }

    /// <summary>
    /// 停止指定节点，并等待进程完全退出
    /// </summary>
    public async Task<bool> StopNodeAsync(string id)
    {
        await _nodeLifecycleLock.WaitAsync();
        try
        {
            return await StopNodeCoreAsync(id, true);
        }
        finally
        {
            _nodeLifecycleLock.Release();
        }
    }

    private async Task<bool> StopNodeCoreAsync(string id, bool waitForExit)
    {
        if (!_activeNodes.TryGetValue(id, out var nodeInfo))
        {
            return false;
        }

        try
        {
            nodeInfo.UdpManager.Dispose();

            if (!nodeInfo.Process.HasExited)
            {
                nodeInfo.Process.Kill();
            }

            if (waitForExit)
            {
                await nodeInfo.Process.WaitForExitAsync();
            }

            if (nodeInfo.Process.HasExited)
            {
                MarkNodeExited(nodeInfo);
            }

            _activeNodes.TryRemove(id, out _);
            usedAdapters.Remove(nodeInfo.Parameters.DeviceName);

            if (_activeNodes.IsEmpty)
            {
                await StopBroadcastRepairAsync(waitForExit);
            }

            return true;
        }
        catch
        {
            if (nodeInfo.Process.HasExited)
            {
                _activeNodes.TryRemove(id, out _);
                usedAdapters.Remove(nodeInfo.Parameters.DeviceName);
            }

            return false;
        }
    }

    private async Task StopBroadcastRepairAsync(bool waitForExit)
    {
        var broadcastRepair = _broadcastRepair;
        if (broadcastRepair == null)
        {
            return;
        }

        _broadcastRepair = null;

        try
        {
            if (!broadcastRepair.HasExited)
            {
                broadcastRepair.Kill();
            }

            if (waitForExit)
            {
                await broadcastRepair.WaitForExitAsync();
            }
        }
        catch
        {
            
        }
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
                            MarkNodeExited(node.Value);
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
    private static void FetchNodeInfo(EdgeNodeInfo nodeInfo)
    {
        try
        {
            // 如果已经处于致命错误状态，不再推进状态机
            if (nodeInfo.State is NodeState.AuthFailed or NodeState.TapError or NodeState.ConfigError or NodeState.NetworkError or NodeState.StartFailed or NodeState.Exited)
            {
                return;
            }

            var statusRead = nodeInfo.UdpManager.Read("info");

            if (!statusRead)
            {
                nodeInfo.LastSeenUtc = DateTime.UtcNow;
                if (nodeInfo.State == NodeState.ProcessStarted)
                {
                    TransitionNodeState(nodeInfo, NodeState.Connecting, "等待管理口响应");
                }
                return;
            }

            foreach (var info in nodeInfo.UdpManager.GetReceivedData())
            {
                var runtimeStatus = EdgeRuntimeStatus.FromObject(info);
                if (runtimeStatus?.Version == "iris")
                {
                    nodeInfo.Status = runtimeStatus;
                    nodeInfo.ManagementReady = true;
                    nodeInfo.LastSeenUtc = DateTime.UtcNow;
                    nodeInfo.LastUpdateTime = DateTime.Now;
                    nodeInfo.StableInfoReadCount += 1;

                    // 多信号联合判定是否真正 Running
                    if (!string.IsNullOrWhiteSpace(runtimeStatus.Ip4Addr))
                    {
                        nodeInfo.NetworkReady = true;

                        // 必须连续读到 3 次 info 才算稳定
                        if (nodeInfo.StableInfoReadCount >= 3)
                        {
                            // 如果有警告（比如 supernode not responding）降级为 RunningWithWarning
                            if (nodeInfo.State == NodeState.RunningWithWarning || !string.IsNullOrEmpty(nodeInfo.LastWarningMessage))
                            {
                                TransitionNodeState(nodeInfo, NodeState.RunningWithWarning, nodeInfo.LastWarningMessage ?? "运行中，但存在警告");
                            }
                            else
                            {
                                TransitionNodeState(nodeInfo, NodeState.Running, "节点运行中");
                            }
                        }
                        else
                        {
                            // 次数不够，保持在 TapReady
                            TransitionNodeState(nodeInfo, NodeState.TapReady, "正在建立连接...");
                        }
                    }
                    else
                    {
                        TransitionNodeState(nodeInfo, NodeState.TapReady, "管理口已就绪");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            nodeInfo.LastRawLog = ex.Message;
            nodeInfo.LastErrorMessage = ex.Message;
            nodeInfo.LastSeenUtc = DateTime.UtcNow;
            if (nodeInfo.State is NodeState.ProcessStarted or NodeState.TapReady or NodeState.Connecting)
            {
                TransitionNodeState(nodeInfo, NodeState.Connecting, "管理口读取异常");
            }
            Debug.WriteLine($"Fetch node info error: {ex.Message}");
        }
    }

    private static void TransitionNodeState(EdgeNodeInfo nodeInfo, NodeState newState, string? statusText = null)
    {
        nodeInfo.State = newState;
        nodeInfo.LastStatusAt = DateTime.Now;
        nodeInfo.LastUpdateTime = DateTime.Now;
        nodeInfo.LastSeenUtc = DateTime.UtcNow;
        nodeInfo.LastStatusText = statusText;
        nodeInfo.IsHealthy = newState is NodeState.Running or NodeState.TapReady or NodeState.Connecting or NodeState.ProcessStarted;
    }

    private static void MarkNodeExited(EdgeNodeInfo nodeInfo)
    {
        nodeInfo.ExitCode = nodeInfo.Process.ExitCode;
        nodeInfo.IsHealthy = false;
        nodeInfo.LastSeenUtc = DateTime.UtcNow;
        nodeInfo.LastUpdateTime = DateTime.Now;
        nodeInfo.LastStatusAt = DateTime.Now;
        nodeInfo.LastStatusText = "进程已退出";
        nodeInfo.State = nodeInfo.Process.ExitCode == 0 ? NodeState.Exited : NodeState.StartFailed;
        nodeInfo.LastErrorMessage = nodeInfo.Process.ExitCode == 0 ? null : $"进程退出，退出码 {nodeInfo.Process.ExitCode}";
    }

    private static void HandleProcessExit(EdgeNodeInfo nodeInfo, Process exitedProcess)
    {
        nodeInfo.ExitCode = exitedProcess.ExitCode;
        nodeInfo.LastRawLog = $"进程退出，退出码 {exitedProcess.ExitCode}";
        MarkNodeExited(nodeInfo);
    }

    private static void HandleProcessLog(EdgeNodeInfo nodeInfo, string rawLog, NodeLogSeverity fallbackSeverity)
    {
        nodeInfo.LastRawLog = rawLog;
        nodeInfo.LastSeenUtc = DateTime.UtcNow;
        nodeInfo.LastUpdateTime = DateTime.Now;

        var match = MatchNodeLog(rawLog, fallbackSeverity);
        if (match == null)
        {
            return;
        }

        if (match.Severity == NodeLogSeverity.Warning)
        {
            nodeInfo.LastWarningMessage = match.Message;
        }
        else if (match.Severity == NodeLogSeverity.Error)
        {
            nodeInfo.LastErrorMessage = match.Message;
        }

        nodeInfo.LastStatusText = match.Message;
        if (match.SuggestedState.HasValue)
        {
            TransitionNodeState(nodeInfo, match.SuggestedState.Value, match.Message);
        }
    }

    private static NodeLogMatchResult? MatchNodeLog(string rawLog, NodeLogSeverity fallbackSeverity)
    {
        foreach (var rule in _logRules)
        {
            if (rule.Pattern.IsMatch(rawLog))
            {
                return new NodeLogMatchResult
                {
                    Severity = rule.Severity,
                    Code = rule.Code,
                    Message = rule.Message,
                    RawLog = rawLog,
                    SuggestedState = rule.SuggestedState
                };
            }
        }

        return fallbackSeverity == NodeLogSeverity.Error
            ? new NodeLogMatchResult
            {
                Severity = fallbackSeverity,
                Code = NodeLogCode.Unknown,
                Message = "运行日志出现错误",
                RawLog = rawLog,
                SuggestedState = null
            }
            : null;
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
        _nodeLifecycleLock.Dispose();

        GC.SuppressFinalize(this);
    }
}

public class EdgeNodeInfo
{
    public string Id { get; set; } = string.Empty;
    public Process Process { get; set; } = null!;
    public EdgeUdpManage UdpManager { get; set; } = null!;
    public int ManagementPort { get; set; }
    public N2NConfiguration Parameters { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime LastUpdateTime { get; set; }
    public EdgeRuntimeStatus? Status { get; set; }
    public NodeState State { get; set; } = NodeState.Created;
    public string? LastErrorMessage { get; set; }
    public string? LastWarningMessage { get; set; }
    public string? LastStatusText { get; set; }
    public string? LastRawLog { get; set; }
    public int? ExitCode { get; set; }
    public bool ManagementReady { get; set; }
    public bool NetworkReady { get; set; }
    public DateTime LastStatusAt { get; set; }
    public int StableInfoReadCount { get; set; }
    public DateTime LastSeenUtc { get; set; }
    public bool IsHealthy { get; set; }
}
