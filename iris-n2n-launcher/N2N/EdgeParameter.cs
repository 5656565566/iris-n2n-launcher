using iris_n2n_launcher.Utils;
using System.Text;

namespace iris_n2n_launcher.N2N;

public sealed class N2NConfigurationValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = [];
}

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

    public N2NConfigurationValidationResult Validate()
    {
        var result = new N2NConfigurationValidationResult();

        if (string.IsNullOrWhiteSpace(Community))
        {
            result.Errors.Add("参数非法-请填写房间名");
        }

        if (string.IsNullOrWhiteSpace(SuperNodeHostAndPort))
        {
            result.Errors.Add("参数非法-请检查服务器选项");
        }

        return result;
    }

    public bool TryBuildArguments(out string arguments, out IReadOnlyList<string> errors)
    {
        var validationResult = Validate();
        if (!validationResult.IsValid)
        {
            arguments = string.Empty;
            errors = validationResult.Errors;
            return false;
        }

        var args = new List<string>();

        AppendOptionWithValue(args, "-c", Community);
        AppendOptionWithValue(args, "-l", NormalizeSuperNodeHostAndPort(SuperNodeHostAndPort));

        if (!string.IsNullOrWhiteSpace(LocalUDPPort))
        {
            AppendOptionWithValue(args, "-p", LocalUDPPort);
        }

        if (EnablePMTUDiscovery)
        {
            args.Add("-D");
        }

        if (!string.IsNullOrWhiteSpace(LocalIPAdvertisement))
        {
            AppendOptionWithValue(args, "-e", LocalIPAdvertisement);
        }

        if (!string.IsNullOrWhiteSpace(SuperNodeConnectionType) && EnablePacketForwarding)
        {
            args.Add($"-{SuperNodeConnectionType}");
        }

        if (RegistrationInterval != 0)
        {
            AppendOptionWithValue(args, "-i", RegistrationInterval.ToString());
        }

        if (RegistrationTTL != 0)
        {
            AppendOptionWithValue(args, "-L", RegistrationTTL.ToString());
        }

        if (!string.IsNullOrWhiteSpace(EncryptionKey))
        {
            AppendOptionWithValue(args, "-k", EncryptionKey);
        }

        if (DisablePayloadEncryption)
        {
            args.Add("-A1");
        }

        if (!string.IsNullOrWhiteSpace(PayloadEncryptionAlgorithm))
        {
            args.Add($"-{PayloadEncryptionAlgorithm}");
        }

        if (UseHeaderEncryption)
        {
            args.Add("-H");
        }

        if (!string.IsNullOrWhiteSpace(OutgoingDataCompression))
        {
            args.Add($"-{OutgoingDataCompression}");
        }

        if (SelectSuperNodeByRTT)
        {
            args.Add("--select-rtt");
        }

        if (SelectSuperNodeByMAC)
        {
            args.Add("--select-mac");
        }

        if (!string.IsNullOrWhiteSpace(InterfaceAddress) && STATICAddress)
        {
            AppendOptionWithValue(args, "-a", InterfaceAddress);
        }

        if (!string.IsNullOrWhiteSpace(MacAddress))
        {
            AppendOptionWithValue(args, "-m", MacAddress);
        }

        if (!string.IsNullOrWhiteSpace(DeviceName))
        {
            AppendOptionWithValue(args, "-d", DeviceName);
        }

        if (MTU != 0)
        {
            AppendOptionWithValue(args, "-M", MTU.ToString());
        }

        if (EnablePacketForwarding)
        {
            args.Add("-r");
        }

        if (AcceptMulticastMAC)
        {
            args.Add("-E");
        }

        if (!string.IsNullOrWhiteSpace(UserPassword))
        {
            AppendOptionWithValue(args, "-J", UserPassword);
        }

        if (!string.IsNullOrWhiteSpace(UserPublicKey))
        {
            AppendOptionWithValue(args, "-P", UserPublicKey);
        }

        if (!string.IsNullOrWhiteSpace(PacketFilterRules) && UsePacketFilterRules)
        {
            AppendOptionWithValue(args, "-R", PacketFilterRules.Replace('\n', ' '));
        }

        if (InterfaceMetric != 0)
        {
            AppendOptionWithValue(args, "-x", InterfaceMetric.ToString());
        }

        AppendOptionWithValue(args, "-I", BuildDescriptionValue());

        arguments = string.Join(" ", args.Select(EscapeCommandLineArgument));
        errors = Array.Empty<string>();
        return true;
    }

    public override string ToString()
    {
        if (!TryBuildArguments(out var arguments, out var errors))
        {
            return string.Join(Environment.NewLine, errors);
        }

        return arguments;
    }

    private static void AppendOptionWithValue(ICollection<string> args, string option, string value)
    {
        args.Add(option);
        args.Add(value);
    }

    private static string NormalizeSuperNodeHostAndPort(string value)
    {
        return value.Contains(':') ? value : $"{value}:7654";
    }

    private string BuildDescriptionValue()
    {
        if (!string.IsNullOrWhiteSpace(Description))
        {
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(Description);
            string unicodeString = Encoding.Unicode.GetString(unicodeBytes);
            return $"{ConvertNonAsciiToUnicode(unicodeString)}-{MachineCode.Generate()}";
        }

        return MachineCode.Generate();
    }

    private static string EscapeCommandLineArgument(string argument)
    {
        if (string.IsNullOrEmpty(argument))
        {
            return "\"\"";
        }

        if (!argument.Any(ch => char.IsWhiteSpace(ch) || ch == '"'))
        {
            return argument;
        }

        var builder = new StringBuilder();
        builder.Append('"');

        int backslashCount = 0;
        foreach (char ch in argument)
        {
            if (ch == '\\')
            {
                backslashCount++;
                continue;
            }

            if (ch == '"')
            {
                builder.Append(new string('\\', backslashCount * 2 + 1));
                builder.Append('"');
                backslashCount = 0;
                continue;
            }

            if (backslashCount > 0)
            {
                builder.Append(new string('\\', backslashCount));
                backslashCount = 0;
            }

            builder.Append(ch);
        }

        if (backslashCount > 0)
        {
            builder.Append(new string('\\', backslashCount * 2));
        }

        builder.Append('"');
        return builder.ToString();
    }
    public static string ConvertNonAsciiToUnicode(string input)
    {
        StringBuilder output = new();

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