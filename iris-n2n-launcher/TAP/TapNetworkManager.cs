using iris_n2n_launcher.Utils;
using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace iris_n2n_launcher.TAP;

/// <summary>
/// 表示系统中的网络适配器信息
/// </summary>
public class NetworkAdapterInfo
{
    /// <summary>
    /// 网络适配器的唯一标识符（Adapter.Id）
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 网络适配器名称（Adapter.Name）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 网络适配器的描述信息（Adapter.Description）
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 网卡类型（如“物理网卡”、“虚拟网卡”、“无线网卡”等）
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// MAC 地址（物理地址）
    /// </summary>
    public string MacAddress { get; set; } = string.Empty;

    /// <summary>
    /// 网络适配器的速度（单位：bps）
    /// </summary>
    public long Speed { get; set; }

    /// <summary>
    /// 当前网络适配器的运行状态（如“Up”、“Down”等）
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 与该网络适配器关联的 IPv4 地址或 IPv6 地址列表
    /// </summary>
    public List<string> IpAddresses { get; set; } = [];
}

public enum AdapterType
{
    All,
    Physical,
    Virtual,
    Wireless,
    TAP
}

internal class TapNetworkManager
{
    readonly static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TAP");
    readonly static string tapinstall = Path.Combine(path, "devcon.exe");
    readonly ExeHelper exeHelper = ExeHelper.Instance;
    /// <summary>
    /// 系统网络接口
    /// </summary>
    /// <returns>要使用的网卡或空字符</returns>
    public static List<NetworkAdapterInfo> ShowNetworkInterfaceMessage(AdapterType filter = AdapterType.All)
    {
        List<NetworkAdapterInfo> result = [];
        NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface adapter in adapters)
        {
            string type = "未知网卡";
            string registryKey = $"SYSTEM\\CurrentControlSet\\Control\\Network\\{{4D36E972-E325-11CE-BFC1-08002BE10318}}\\{adapter.Id}\\Connection";
            RegistryKey? rk = Registry.LocalMachine.OpenSubKey(registryKey, false);

            string pnp = rk?.GetValue("PnpInstanceID", "").ToString() ?? "";
            int mediaSubType = Convert.ToInt32(rk?.GetValue("MediaSubType", 0));

            if (pnp.StartsWith("PCI")) type = "物理网卡";
            else if (mediaSubType == 1) type = "虚拟网卡";
            else if (mediaSubType == 2) type = "无线网卡";

            if (adapter.Description.Contains("TAP-Windows Adapter V9")) type = "TAP";

            if (filter != AdapterType.All && !type.Equals(filter.ToString(), StringComparison.OrdinalIgnoreCase))
                continue;

            NetworkAdapterInfo info = new()
            {
                Id = adapter.Id,
                Name = adapter.Name,
                Description = adapter.Description,
                Type = type,
                MacAddress = adapter.GetPhysicalAddress().ToString(),
                Speed = adapter.Speed,
                Status = adapter.OperationalStatus.ToString(),
                IpAddresses = [.. adapter.GetIPProperties()
                                      .UnicastAddresses
                                      .Where(u => u.Address.AddressFamily == AddressFamily.InterNetwork)
                                      .Select(u => u.Address.ToString())]
            };

            result.Add(info);
        }

        return result;
    }

    /// <summary>
    /// 检查并确保至少存在一张本程序可用的TAP网卡，如果不存在则自动创建一张
    /// </summary>
    /// <returns>是否成功确保TAP网卡存在</returns>
    public async Task<bool> EnsureTapAdapterExistsAsync()
    {
        try
        {
            var tapAdapters = ShowNetworkInterfaceMessage(AdapterType.TAP);

            if (tapAdapters.Any(adapter => adapter.Description.Contains("TAP-Windows Adapter V9")))
            {
                return true;
            }

            bool installResult = await InstallTapAsync();

            if (!installResult)
            {
                return false;
            }

            tapAdapters = ShowNetworkInterfaceMessage(AdapterType.TAP);
            if (tapAdapters.Any(adapter => adapter.Description.Contains("TAP-Windows Adapter V9")))
            {
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取所有的 TAP 网卡（描述 -> 设备 ID）
    /// </summary>
    public async Task<Dictionary<string, string>> GetAllTapAdaptersAsync()
    {
        var output = await exeHelper.RunCommandWithOutputAsync(
            tapinstall,
            "find tap0901",
            path
        );

        return Regex.Matches(output.StdOut, @"^(ROOT\\NET\\\d+)\s+:\s+(.+)$", RegexOptions.Multiline)
            .Cast<Match>()
            .Where(m => m.Groups.Count == 3)
            .ToDictionary(
                m => m.Groups[2].Value.Trim(), // 描述
                m => m.Groups[1].Value.Trim()  // 设备 ID
            );
    }

    /// <summary>
    /// 安装一个 TAP 网卡
    /// </summary>
    public Task<bool> InstallTapAsync() =>
        exeHelper.RunCommandAsync(tapinstall, "install OemVista.inf tap0901", path, "successfully");

    /// <summary>
    /// 卸载指定编号的 TAP 网卡（默认卸载全部）
    /// </summary>
    public async Task<bool> UninstallTapAdapterAsync(int index = -1)
    {
        var adapters = await GetAllTapAdaptersAsync();

        if (adapters.Count == 0) return false;

        string? description = index switch
        {
            < 0 => adapters.Keys.FirstOrDefault(),
            0 => "TAP-Windows Adapter V9",
            _ => $"TAP-Windows Adapter V9 #{index + 1}"
        };

        if (description == null || !adapters.TryGetValue(description, out var id))
            return false;

        return await exeHelper.RunCommandAsync(tapinstall, $"remove @{id}", path, "Removed");
    }
    /// <summary>
    /// 卸载指定描述的 TAP 网卡（如果设备 ID 存在于使用中列表中则拒绝）
    /// </summary>
    /// <param name="description">网卡描述（如 TAP-Windows Adapter V9）</param>
    /// <param name="usedAdapterIds">当前正在使用的网卡 ID 列表</param>
    /// <returns>是否成功卸载</returns>
    public async Task<bool> UninstallTapAdapterAsync(string description, List<string> usedAdapterIds)
    {
        var adapters = await GetAllTapAdaptersAsync();

        if (adapters.Count == 0)
            return false;

        if (string.IsNullOrEmpty(description) || !adapters.TryGetValue(description, out var deviceId))
            return false;

        // 获取当前所有网卡，查找指定描述对应的 ID
        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        var targetAdapter = networkInterfaces.FirstOrDefault(nic => nic.Description == description);

        if (targetAdapter != null && usedAdapterIds.Contains(targetAdapter.Id))
        {
            // 如果网卡 ID 在使用列表中，拒绝卸载
            return false;
        }

        return await exeHelper.RunCommandAsync(tapinstall, $"remove @{deviceId}", path, "Removed");
    }
    /// <summary>
    /// 获取本机所有的IP地址（包括IPv4和IPv6）
    /// </summary>
    /// <returns>包含所有IP地址的列表</returns>
    public static List<string> GetAllLocalIPs()
    {
        List<string> ips = new List<string>();
        NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface adapter in adapters)
        {
            // 跳过非活动网卡和回环接口
            if (adapter.OperationalStatus != OperationalStatus.Up ||
                adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
            {
                continue;
            }

            IPInterfaceProperties properties = adapter.GetIPProperties();
            foreach (UnicastIPAddressInformation ip in properties.UnicastAddresses)
            {
                // 添加所有有效的IP地址（包括IPv4和IPv6）
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork ||
                    ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    ips.Add(ip.Address.ToString());
                }
            }
        }

        return ips;
    }
}
