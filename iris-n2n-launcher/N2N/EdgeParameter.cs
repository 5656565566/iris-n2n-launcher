using iris_n2n_launcher.Utils;
using System.Text;

namespace iris_n2n_launcher.N2N;
public class N2NConfiguration
{
    /// <summary>
    /// 指定房间名称。
    /// c <community>
    /// </summary>
    public string Community { get; set; } = string.Empty;

    /// <summary>
    /// 超级节点的 IP 地址或名称以及端口。
    /// l <host:port>
    /// </summary>
    public string SuperNodeHostAndPort { get; set; } = "";

    /// <summary>
    /// 固定本地 UDP 端口，可选择性地绑定到指定的本地 IP 地址。
    /// 默认值为任意地址。
    /// p [<ip>:]<port>
    /// </summary>
    public string LocalUDPPort { get; set; } = string.Empty;

    /// <summary>
    /// 启用 PMTU 发现，可以减少分段，但如果不正确支持可能会导致连接中断。
    /// D
    /// </summary>
    public bool EnablePMTUDiscovery { get; set; } = false;

    /// <summary>
    /// 将提供的本地 IP 地址作为首选进行广告，如果组播对等点检测不可用，则非常有用。
    /// e
    /// 'auto' 尝试 IP 地址自动检测。
    /// </summary>
    public string LocalIPAdvertisement { get; set; } = "auto";

    /// <summary>
    /// 不进行 P2P 连接，始终使用超级节点。
    /// 默认通过 UDP 连接。
    /// S1 （使用UDP转发） S2（使用TCP转发）
    /// </summary>
    public string SuperNodeConnectionType { get; set; } = string.Empty;

    /// <summary>
    /// 尝试 NAT 间隔，用于 NAT 打洞（默认20秒）。
    /// i
    /// </summary>
    public int RegistrationInterval { get; set; } = 0;

    /// <summary>
    /// 用于 NAT 打洞的超级节点注册数据包的 TTL（默认为0表示未设置）。
    /// L
    /// </summary>
    public int RegistrationTTL { get; set; } = 0;

    /// <summary>
    /// 加密密钥（ASCII）- 也可以使用 N2N_KEY=<key>。
    /// k
    /// </summary>
    public string EncryptionKey { get; set; } = string.Empty;

    /// <summary>
    /// 禁用有效载荷加密，不要与密钥一起使用，否则默认使用 AES。
    /// A1
    /// </summary>
    public bool DisablePayloadEncryption { get; set; } = false;

    /// <summary>
    /// 选择用于有效载荷加密的密码，需要一个密钥。
    /// A2 = Twofish，A3 = AES（如果提供了密钥，则默认使用），
    /// A4 = ChaCha20，A5 = Speck-CTR。
    /// </summary>
    public string PayloadEncryptionAlgorithm { get; set; } = string.Empty;

    /// <summary>
    /// 使用头部加密，超级节点需要固定社区。
    /// H
    /// </summary>
    public bool UseHeaderEncryption { get; set; }

    /// <summary>
    /// 压缩传出的数据包。
    /// z1 = lzo1x，z2 = zstd，默认情况下禁用。
    /// </summary>
    public string OutgoingDataCompression { get; set; } = string.Empty;

    /// <summary>
    /// 基于往返时间选择超级节点。
    /// --select-rtt
    /// </summary>
    public bool SelectSuperNodeByRTT { get; set; } = false;

    /// <summary>
    /// 基于 MAC 地址选择超级节点（默认为基于负载）。
    /// --select-mac
    /// </summary>
    public bool SelectSuperNodeByMAC { get; set; } = false;

    /// <summary>
    /// 接口地址和可选的 CIDR 子网，默认为 '/24'。
    ///  a [mode]<ip>[/n]
    /// </summary>
    public string InterfaceAddress { get; set; } = string.Empty;
    /// <summary>
    /// 使用 DHCP 分配地址
    /// </summary>
    public bool STATICAddress { get; set; } = false;
    /// <summary>
    /// TAP 接口的固定 MAC 地址。
    /// m <mac>
    /// </summary>
    public string MacAddress { get; set; } = string.Empty;

    /// <summary>
    /// TAP 设备名称。
    /// d <device>
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// 指定 TAP 接口的 n2n MTU，默认为 1290。
    /// M <mtu>
    /// </summary>
    public int MTU { get; set; } = 0;

    /// <summary>
    /// 启用通过 n2n 社区的数据包转发。
    /// r
    /// </summary>
    public bool EnablePacketForwarding { get; set; } = false;

    /// <summary>
    /// 接受组播 MAC 地址，默认情况下丢弃。
    /// E
    /// </summary>
    public bool AcceptMulticastMAC { get; set; } = true;

    /// <summary>
    /// 注释边缘的描述，用于在管理端口输出或用户名中更容易识别。
    /// I
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 用户密码边缘身份验证的密码。
    /// J <password>
    /// </summary>
    public string UserPassword { get; set; } = string.Empty;

    /// <summary>
    /// 用于用户密码身份验证的公钥。
    /// P <public key>
    /// </summary>
    public string UserPublicKey { get; set; } = string.Empty;

    /// <summary>
    /// 按规则丢弃或接受数据包，可以设置多次，规则格式：
    /// 'src_ip/n:[s_port,e_port],...dst_ip/n:[s_port,e_port],...TCP+/-,UDP+/-,ICMP+/-'
    /// R <rule>
    /// 示范: -R 0.0.0.0/0,0.0.0.0/0,TCP-,UDP-,ICMP- -R 192.168.100.0/24,192.168.100.0/24,ICMP+
    /// </summary>
    public string PacketFilterRules { get; set; } = string.Empty;

    /// <summary>
    /// 使用数据包转发规则配置
    /// </summary>
    public bool UsePacketFilterRules { get; set; } = false;

    /// <summary>
    /// 设置 网卡折跃点数 ，默认为 0（自动）。
    /// 例如设置为 1 以更好地检测多人游戏。
    /// -x <metric>
    /// </summary>
    public int InterfaceMetric { get; set; } = 0;

    public override string ToString()
    {
        string parameter = "";

        if (Community != "")
        {
            parameter = $"-c {Community} {parameter}";
        }
        else
        {
            return "参数非法-请填写房间名";
        }

        if (SuperNodeHostAndPort != "")
        {
            if (SuperNodeHostAndPort.Contains(":"))
            {
                parameter = $"-l {SuperNodeHostAndPort} {parameter}";
            }
            else
            {
                parameter = $"-l {SuperNodeHostAndPort}:7654 {parameter}";
            }
        }
        else
        {
            return "参数非法-请检查服务器选项";
        }

        if (LocalUDPPort != "")
        {
            parameter = $"-p {SuperNodeHostAndPort} {parameter}";
        }

        if (EnablePMTUDiscovery)
        {
            parameter = $"-D {parameter}";
        }

        if (LocalIPAdvertisement != "")
        {
            parameter = $"-e {LocalIPAdvertisement} {parameter}";
        }

        if (SuperNodeConnectionType != "" && EnablePacketForwarding)
        {
            parameter = $"-{SuperNodeConnectionType} {parameter}";
        }

        if (RegistrationInterval != 0)
        {
            parameter = $"-i {SuperNodeConnectionType} {parameter}";
        }

        if (RegistrationTTL != 0)
        {
            parameter = $"-L {RegistrationTTL} {parameter}";
        }

        if (EncryptionKey != "")
        {
            parameter = $"-k {EncryptionKey} {parameter}";
        }

        if (DisablePayloadEncryption)
        {
            parameter = $"-A1 {parameter}";
        }

        if (PayloadEncryptionAlgorithm != "")
        {
            parameter = $"-{PayloadEncryptionAlgorithm} {parameter}";
        }

        if (UseHeaderEncryption)
        {
            parameter = $"-H {parameter}";
        }

        if (OutgoingDataCompression != "")
        {
            parameter = $"-{OutgoingDataCompression} {parameter}";
        }

        if (SelectSuperNodeByRTT)
        {
            parameter = $"--select-rtt {parameter}";
        }

        if (SelectSuperNodeByMAC)
        {
            parameter = $"--select-mac {parameter}";
        }

        if (InterfaceAddress != "" && STATICAddress)
        {
            parameter = $"-a {InterfaceAddress} {parameter}";
        }

        if (MacAddress != "")
        {
            parameter = $"-m {MacAddress} {parameter}";
        }

        if (DeviceName != "")
        {
            parameter = $"-d {DeviceName} {parameter}";
        }

        if (MTU != 0)
        {
            parameter = $"-M {MTU} {parameter}";
        }

        if (EnablePacketForwarding)
        {
            parameter = $"-r {parameter}";
        }

        if (AcceptMulticastMAC)
        {
            parameter = $"-E {parameter}";
        }

        if (UserPassword != "")
        {
            parameter = $"-J {UserPassword} {parameter}";
        }

        if (UserPublicKey != "")
        {
            parameter = $"-P {UserPublicKey} {parameter}";
        }

        if (PacketFilterRules != "" && UsePacketFilterRules)
        {
            parameter = $"-R {PacketFilterRules.Replace('\n', ' ')} {parameter}";
        }

        if (InterfaceMetric != 0)
        {
            parameter = $"-x {InterfaceMetric} {parameter}";
        }
        if (Description != "")
        {
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(Description);
            string unicodeString = Encoding.Unicode.GetString(unicodeBytes);
            parameter = $"-I {ConvertNonAsciiToUnicode(unicodeString)}-{MachineCode.Generate()} {parameter}";

        }
        else
        {
            parameter = $"-I {MachineCode.Generate()} {parameter}";
        }

        return parameter;
    }
    public static string ConvertNonAsciiToUnicode(string input)
    {
        StringBuilder output = new StringBuilder();

        foreach (char c in input)
        {
            if (c > 127) // 非 ASCII 字符
            {
                output.AppendFormat("\\u{0:X4}", (int)c);
            }
            else
            {
                output.Append(c);
            }
        }

        return output.ToString();
    }
}