using NetFwTypeLib;
using System.Diagnostics;

namespace iris_n2n_launcher.Utils;

internal class FirewallHelper
{
    public class FirewallManager
    {
        /// <summary>
        /// 允许远程IP地址通过防火墙。
        /// </summary>
        /// <param name="ruleName">规则名称。</param>
        /// <param name="ipAddress">本地IP地址。</param>
        public static async Task AllowIPAsync(string ruleName, string ipAddress)
        {
            string[] octets = ipAddress.Split('.');
            if (octets.Length != 4)
            {
                throw new ArgumentException("Invalid IP address format", nameof(ipAddress));
            }

            string subnet = $"{octets[0]}.{octets[1]}.{octets[2]}.0/24";
            string commandRemoteIn = $"netsh advfirewall firewall add rule name=\"{ruleName}\" dir=in action=allow remoteip={subnet}";
            string commandRemoteOut = $"netsh advfirewall firewall add rule name=\"{ruleName}\" dir=out action=allow remoteip={subnet}";
            string commandLocalIn = $"netsh advfirewall firewall add rule name=\"{ruleName}\" dir=in action=allow localip={ipAddress}";
            string commandLocalOut = $"netsh advfirewall firewall add rule name=\"{ruleName}\" dir=out action=allow localip={ipAddress}";

            await ExecuteCommandAsync(commandRemoteIn);
            await ExecuteCommandAsync(commandRemoteOut);
            await ExecuteCommandAsync(commandLocalIn);
            await ExecuteCommandAsync(commandLocalOut);
        }

        /// <summary>
        /// 允许ICMPv4回显请求（Ping）通过防火墙。
        /// </summary>
        /// <param name="ruleName">规则名称。</param>
        public static async Task AllowPingAsync(string ruleName)
        {
            string command = $"netsh advfirewall firewall add rule name=\"{ruleName}\" protocol=icmpv4:8,any dir=in action=allow";
            await ExecuteCommandAsync(command);
        }

        /// <summary>
        /// 允许指定程序的出入站连接。
        /// </summary>
        /// <param name="ruleName">规则名称。</param>
        /// <param name="programPath">程序路径。</param>
        public static async Task AllowProgramAsync(string ruleName, string programPath)
        {
            await DeleteRuleAsync(ruleName + " (Inbound)");
            await DeleteRuleAsync(ruleName + " (Outbound)");

            string commandIn = $"netsh advfirewall firewall add rule name=\"{ruleName} (Inbound)\" dir=in action=allow program=\"{programPath}\" enable=yes";
            string commandOut = $"netsh advfirewall firewall add rule name=\"{ruleName} (Outbound)\" dir=out action=allow program=\"{programPath}\" enable=yes";

            await ExecuteCommandAsync(commandIn);
            await ExecuteCommandAsync(commandOut);
        }

        /// <summary>
        /// 删除指定名称的防火墙规则。
        /// </summary>
        /// <param name="ruleName">规则名称。</param>
        public static async Task DeleteRuleAsync(string ruleName)
        {
            string command = $"netsh advfirewall firewall delete rule name=\"{ruleName}\"";
            await ExecuteCommandAsync(command);
        }

        /// <summary>
        /// 执行命令行命令。
        /// </summary>
        /// <param name="command">要执行的命令。</param>
        private static async Task ExecuteCommandAsync(string command)
        {
            ExeHelper exeHelper = ExeHelper.Instance;
            await exeHelper.RunCommandAsync("cmd.exe", $"/c {command}");
        }
    }

    /// <summary>
    /// 通过对象防火墙操作
    /// </summary>
    /// <param name="isOpenDomain">域网络防火墙（禁用：false；启用（默认）：true）</param>
    /// <param name="isOpenPublicState">公共网络防火墙（禁用：false；启用（默认）：true）</param>
    /// <param name="isOpenStandard">专用网络防火墙（禁用: false；启用（默认）：true）</param>
    /// <returns></returns>
    public static bool FirewallOperateByObject(bool isOpenDomain = true, bool isOpenPublicState = true, bool isOpenStandard = true)
    {
        try
        {
            INetFwPolicy2? firewallPolicy = (INetFwPolicy2?)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2")!);

            if (firewallPolicy == null)
            {
                return false;
            }

            // 启用<高级安全Windows防火墙> - 专有配置文件的防火墙
            firewallPolicy.set_FirewallEnabled(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE, isOpenStandard);

            // 启用<高级安全Windows防火墙> - 公用配置文件的防火墙
            firewallPolicy.set_FirewallEnabled(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC, isOpenPublicState);

            // 启用<高级安全Windows防火墙> - 域配置文件的防火墙
            firewallPolicy.set_FirewallEnabled(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN, isOpenDomain);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 获取防火墙状态
    /// </summary>
    /// <returns>专用、公用、域</returns>
    public static bool[] GetFirewallStatus()
    {
        bool[] firewallStates = new bool[3];

        try
        {
            if (Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2")!) is INetFwPolicy2 firewallPolicy)
            {
                // 获取专用防火墙状态
                firewallStates[0] = firewallPolicy.get_FirewallEnabled(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE);

                // 获取公用防火墙状态
                firewallStates[1] = firewallPolicy.get_FirewallEnabled(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC);

                // 获取域防火墙状态
                firewallStates[2] = firewallPolicy.get_FirewallEnabled(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN);
            }
        }
        catch
        {
            for (int i = 0; i < 3; i++)
            {
                firewallStates[i] = false;
            }
        }

        return firewallStates;
    }

    public static INetFwMgr? GetFirewallManager()
    {
        try
        {
            const string CLSID_FIREWALL_MANAGER = "{304CE942-6E39-40D8-943A-B913C40C9CD4}";
            Type? objType = Type.GetTypeFromCLSID(new Guid(CLSID_FIREWALL_MANAGER));
            if (objType != null)
            {
                return Activator.CreateInstance(objType) as INetFwMgr;
            }
        }
        catch { }
        return null;
    }
}