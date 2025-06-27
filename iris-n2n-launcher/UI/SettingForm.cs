using iris_n2n_launcher.Utils;
using iris_n2n_launcher.Config;
using iris_n2n_launcher.N2N;
using iris_n2n_launcher.TAP;
using static iris_n2n_launcher.Utils.FirewallHelper;
using System.Reflection;
using iris_n2n_launcher.Utils.FileTransfer;

namespace iris_n2n_launcher.UI;

public partial class SettingForm : Form
{

    private static readonly ConfigManager configManager = ConfigManager.Instance;
    private static Configuration config = configManager.LoadConfig<Configuration>("config");
    private static N2NConfiguration _originalN2Nconfig = configManager.LoadConfig<N2NConfiguration>(config.ConfigName);
    private static N2NConfiguration N2Nconfig = configManager.LoadConfig<N2NConfiguration>(config.ConfigName);
    private static Dictionary<string, int> serverPing = [];
    private static readonly TapNetworkManager tapNetworkManager = new();
    private static readonly EdgeNodeManage edgeNodeManage = EdgeNodeManage.Instance;
    private static List<NetworkAdapterInfo> networkAdapters = [];
    private static readonly TcpUdpForw tcpUdpForw = TcpUdpForw.Instance;
    private static string[] version = ["0", "0", "0", "0"];
    private static int configurationChangeStatus = 0;

    public void BindEvents()
    {
        UserNameTextBox.TextChanged += UserNameTextBox_TextChanged;
        SeverListComboBox.TextChanged += SeverListComboBox_TextChanged;
        MetricUpDown.ValueChanged += MetricUpDown_ValueChanged;

        FirewallSettingAll.Click += FirewallAllSetting;
        FirewallSetting1.Click += FirewallSetting;
        FirewallSetting2.Click += FirewallSetting;
        FirewallSetting3.Click += FirewallSetting;

        MetricAutoCheckBox.Click += MetricAutoCheckBox_Click;
        MetricCheckBox.Click += MetricCheckBox_Click;
        MetricUserCheckBox.Click += MetricUserCheckBox_Click;

        StaticIPCheckBox.Click += StaticIPCheckBox_Click;
        LocalIPAdvertisementCheckBox.Click += LocalIPAdvertisementCheckBox_Click;
        MTUCheckBox.Click += MTUCheckBox_Click;
        RegistrationIntervalCheckBox.Click += RegistrationIntervalCheckBox_Click;
        RegistrationTTLCheckBox.Click += RegistrationTTLCheckBox_Click;
        UsePacketFilterRulesCheckBox.Click += UsePacketFilterRulesCheckBox_Click;
        EnablePacketForwardingCheckBox.Click += EnablePacketForwardingCheckBox_Click;
        SuperNodeConnectionTypeUDPCheckBox.Click += SuperNodeConnectionTypeUDPCheckBox_Clink;
    }

    private void TapListBoxView()
    {
        networkAdapters = TapNetworkManager.ShowNetworkInterfaceMessage(AdapterType.TAP);

        TapListBox.Items.Clear();

        foreach (var item in networkAdapters)
        {
            TapListBox.Items.Add(item.Description);
        }
    }

    private void DataView()
    {
        FirewallEnabledOnExitCheckBox.Checked = config.FirewallEnabledOnExit;
        DisableFirewallOnRunCheckBox.Checked = config.DisableFirewallOnRun;

        BroadcastRepairCheckBox.Checked = config.BroadcastRepair;
        MinecraftCheckBox.Checked = config.Minecraft;
        UrlRegisterCheckBox.Checked = config.UrlRegister;
        TcpUdpForwCheckBox.Checked = config.TcpUdpForw;
        AirportalCheckBox.Checked = config.Airportal;

        BootStartCheckBox.Checked = false;

        if (BootStart.TaskExists("Iris"))
        {
            BootStartCheckBox.Checked = true;
        }

        VersionUpdateCheckBox.Checked = config.VersionUpdate;

        UserNameTextBox.Text = N2Nconfig.Description;
        SeverListComboBox.Text = N2Nconfig.SuperNodeHostAndPort;

        if (N2Nconfig.InterfaceMetric == 0)
        {
            MetricAutoCheckBox.Checked = true;
            MetricUpDown.Enabled = false;
        }
        else if (N2Nconfig.InterfaceMetric == 1)
        {
            MetricCheckBox.Checked = true;
            MetricUpDown.Enabled = false;
        }
        else
        {
            MetricUserCheckBox.Checked = true;
            MetricUpDown.Enabled = true;
            MetricUpDown.Value = N2Nconfig.InterfaceMetric;
        }

        InterfaceAddressTextBox.Text = N2Nconfig.InterfaceAddress;
        InterfaceAddressTextBox.Enabled = N2Nconfig.STATICAddress;
        StaticIPCheckBox.Checked = N2Nconfig.STATICAddress;

        LocalIPAdvertisementTextBox.Text = N2Nconfig.LocalIPAdvertisement;
        if (LocalIPAdvertisementTextBox.Text != "auto")
        {
            LocalIPAdvertisementCheckBox.Checked = true;
            LocalIPAdvertisementTextBox.Enabled = true;
        }
        else
        {
            LocalIPAdvertisementCheckBox.Checked = false;
            LocalIPAdvertisementTextBox.Enabled = false;
        }

        EnablePMTUDiscoveryCheckBox.Checked = N2Nconfig.EnablePMTUDiscovery;

        MTUTextBox.Text = N2Nconfig.MTU.ToString();
        if (MTUTextBox.Text != "0")
        {
            MTUCheckBox.Checked = true;
            MTUTextBox.Enabled = true;
        }
        else
        {
            MTUCheckBox.Checked = false;
            MTUTextBox.Enabled = false;
        }

        EnablePacketForwardingCheckBox.Checked = N2Nconfig.EnablePacketForwarding;

        if (N2Nconfig.SuperNodeConnectionType == "S1")
        {
            SuperNodeConnectionTypeUDPCheckBox.Checked = true;
        }
        if (N2Nconfig.SuperNodeConnectionType == "S2")
        {
            SuperNodeConnectionTypeUDPCheckBox.Checked = true;
            SuperNodeConnectionTypeTCPCheckBox.Checked = true;
        }
        if (N2Nconfig.SuperNodeConnectionType == "")
        {
            SuperNodeConnectionTypeUDPCheckBox.Checked = false;
            SuperNodeConnectionTypeTCPCheckBox.Checked = false;
        }

        SuperNodeConnectionTypeUDPCheckBox.Enabled = EnablePacketForwardingCheckBox.Checked;
        SuperNodeConnectionTypeTCPCheckBox.Enabled = SuperNodeConnectionTypeUDPCheckBox.Enabled;
        SuperNodeConnectionTypeTCPCheckBox.Enabled = SuperNodeConnectionTypeUDPCheckBox.Checked;

        AcceptMulticastMACCheckBox.Checked = N2Nconfig.AcceptMulticastMAC;

        RegistrationIntervalTextBox.Text = N2Nconfig.RegistrationInterval.ToString();
        if (RegistrationIntervalTextBox.Text != "0")
        {
            RegistrationIntervalCheckBox.Checked = true;
            RegistrationIntervalTextBox.Enabled = true;
        }
        else
        {
            RegistrationIntervalCheckBox.Checked = false;
            RegistrationIntervalTextBox.Enabled = false;
        }

        RegistrationTTLTextBox.Text = N2Nconfig.RegistrationTTL.ToString();
        if (RegistrationTTLTextBox.Text != "0")
        {
            RegistrationTTLCheckBox.Checked = true;
            RegistrationTTLTextBox.Enabled = true;
        }
        else
        {
            RegistrationTTLCheckBox.Checked = false;
            RegistrationTTLTextBox.Enabled = false;
        }

        OutgoingDataCompressionComboBox.Text = N2Nconfig.OutgoingDataCompression;

        UsePacketFilterRulesCheckBox.Checked = N2Nconfig.UsePacketFilterRules;
        PacketFilterRulesRichTextBox.Text = N2Nconfig.PacketFilterRules;

        if (UsePacketFilterRulesCheckBox.Checked)
        {
            PacketFilterRulesRichTextBox.Enabled = true;
        }
        else
        {
            PacketFilterRulesRichTextBox.Enabled = false;
        }

        TapListBoxView();
        UpdataConfigList();
        UpdataServerList();

        version = Assembly.GetExecutingAssembly().GetName().Version!.ToString().Split('.');
        VersionLabel.Text = $"Version:{version[0]}.{version[1]}.{version[2]}-{version[3]}";
    }

    private void UpdataServerList()
    {
        SeverListComboBox.Items.Clear();

        foreach (string server in config.UserServerList)
        {
            SeverListComboBox.Items.Add(server);
        }

        foreach (string server in config.OnlineServerList)
        {
            SeverListComboBox.Items.Add(server);
        }
    }

    private void UpdataConfigList()
    {
        ConfigListBox.Items.Clear();

        var configList = configManager.ListConfigs();

        foreach (var config in configList)
        {
            if (config != null && config != "config")
            {
                ConfigListBox.Items.Add(config);
            }
        }

        ConfigListBox.SelectedItem = config.ConfigName;
    }

    public SettingForm()
    {
        InitializeComponent();
        DataView();
        BindEvents();

        ConfigListBox.SelectedIndexChanged += ConfigListBox_SelectedIndexChanged;
    }

    public int SettingChanging()
    {
        return configurationChangeStatus;
    }

    private void ReadConfig()
    {
        ConfigListBox.SelectedIndexChanged -= ConfigListBox_SelectedIndexChanged;

        config = configManager.LoadConfig<Configuration>("config");
        N2Nconfig = configManager.LoadConfig<N2NConfiguration>(config.ConfigName);

        DataView();

        ConfigListBox.SelectedIndexChanged += ConfigListBox_SelectedIndexChanged;
    }
    private void WriteConfig()
    {
        string errorMessage = "";

        config.FirewallEnabledOnExit = FirewallEnabledOnExitCheckBox.Checked;
        config.DisableFirewallOnRun = DisableFirewallOnRunCheckBox.Checked;

        N2Nconfig.Description = UserNameTextBox.Text;
        N2Nconfig.SuperNodeHostAndPort = SeverListComboBox.Text;

        N2Nconfig.STATICAddress = StaticIPCheckBox.Checked;
        N2Nconfig.InterfaceAddress = InterfaceAddressTextBox.Text;
        N2Nconfig.LocalIPAdvertisement = LocalIPAdvertisementTextBox.Text;
        N2Nconfig.EnablePMTUDiscovery = EnablePMTUDiscoveryCheckBox.Checked;

        if (int.TryParse(MTUTextBox.Text, out int mtu) && (mtu >= 576 && mtu <= 1500 || mtu == 0))
        {
            N2Nconfig.MTU = mtu;
        }
        else
        {
            errorMessage += "请输入有效的 MTU 数值（范围：576 - 1500）\n";
        }

        N2Nconfig.EnablePacketForwarding = EnablePacketForwardingCheckBox.Checked;

        if (EnablePacketForwardingCheckBox.Checked)
        {
            N2Nconfig.EnablePacketForwarding = true;
            N2Nconfig.SuperNodeConnectionType = "";

            if (SuperNodeConnectionTypeUDPCheckBox.Checked)
            {
                N2Nconfig.SuperNodeConnectionType = "S1";
            }

            if (SuperNodeConnectionTypeTCPCheckBox.Checked)
            {
                N2Nconfig.SuperNodeConnectionType = "S2"; // 覆盖设置
            }
        }

        N2Nconfig.AcceptMulticastMAC = AcceptMulticastMACCheckBox.Checked;
        if (int.TryParse(RegistrationIntervalTextBox.Text, out int RegistrationInterval))
        {
            N2Nconfig.RegistrationInterval = RegistrationInterval;
        }
        else
        {
            errorMessage += "请输入有效的 RegistrationInterval 数值\n";
        }

        if (int.TryParse(RegistrationTTLTextBox.Text, out int ttl) && (ttl >= 30 && ttl <= 3600 || ttl == 0))
        {
            N2Nconfig.RegistrationTTL = ttl;
        }
        else
        {
            errorMessage += "请输入有效的 TTL 数值（范围：30 - 3600）\n";
        }
        N2Nconfig.OutgoingDataCompression = OutgoingDataCompressionComboBox.Text;

        N2Nconfig.UsePacketFilterRules = UsePacketFilterRulesCheckBox.Checked;
        N2Nconfig.PacketFilterRules = PacketFilterRulesRichTextBox.Text;

        configManager.SaveConfig("config", config);
        N2Nconfig.Community = configManager.LoadConfig<N2NConfiguration>(config.ConfigName).Community;
        configManager.SaveConfig(config.ConfigName, N2Nconfig);
    }

    private void checkBox15_CheckedChanged(object sender, EventArgs e)
    {
        label2.Visible = !UsePacketFilterRulesCheckBox.Checked;
        label4.Visible = !UsePacketFilterRulesCheckBox.Checked;
        label6.Visible = !UsePacketFilterRulesCheckBox.Checked;
        label8.Visible = !UsePacketFilterRulesCheckBox.Checked;
        PacketFilterRulesRichTextBox.Visible = UsePacketFilterRulesCheckBox.Checked;
    }

    private void FirewallAllSetting(object? sender, EventArgs e)
    {
        FirewallSetting1.Checked = FirewallSettingAll.Checked;
        FirewallSetting2.Checked = FirewallSettingAll.Checked;
        FirewallSetting3.Checked = FirewallSettingAll.Checked;
        FirewallSetting(sender, e);
    }

    private void FirewallSetting(object? sender, EventArgs e)
    {
        bool[] firewall = { FirewallSetting1.Checked, FirewallSetting2.Checked, FirewallSetting3.Checked };

        if (firewall.Contains(true))
        {
            FirewallSettingAll.Checked = true;
        }
        else
        {
            MessageBox.Show("请在注意网络安全的情况下关闭 !");

            FirewallSettingAll.Checked = false;
        }

        FirewallOperateByObject(firewall[0], firewall[1], firewall[2]);
    }

    private async void SelectedIndexChanged(object sender, EventArgs e)
    {
        int selectedIndex = tabControl1.SelectedIndex;

        if (selectedIndex == 1)
        {
            bool[] firewall = GetFirewallStatus();
            if (firewall.Contains(true))
            {
                FirewallSettingAll.Checked = true;
                FirewallSetting1.Checked = firewall[0];
                FirewallSetting2.Checked = firewall[1];
                FirewallSetting3.Checked = firewall[2];
            }
        }

        if (selectedIndex == 4)
        {
            richTextBox3.Text = "更新 && 公告获取中";

            var changelog = await IrisServerApi.GetChangelogAsync();

            var latestVersion = await IrisServerApi.GetAppVersionAsync();

            var notice = await IrisServerApi.GetNoticeAsync();

            if (latestVersion == null)
            {
                richTextBox3.Text = "服务器开小差了，稍后再来看看吧?";
                return;
            }

            if (latestVersion.Version == $"{version[0]}.{version[1]}.{version[2]}.{version[3]}")
            {
                if (notice == null)
                {
                    richTextBox3.Text = "服务器开小差了，稍后再来看看吧?\n不过你至少是更新到了最新版";
                    return;
                }
                richTextBox3.Text = notice.Notice;
            }
            else
            {
                if (changelog == null)
                {
                    richTextBox3.Text = $"服务器开小差了，稍后再来看看吧?\n软件最新版 {latestVersion.Version}";
                    return;
                }
                richTextBox3.Text = changelog.Changelog;
            }
        }

    }

    private void UserNameTextBox_TextChanged(object? sender, EventArgs e)
    {
        N2Nconfig.Description = UserNameTextBox.Text;
    }

    private void SettingForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        WriteConfig();

        if (configManager.HasChanges<N2NConfiguration>(_originalN2Nconfig, N2Nconfig))
        {
            configurationChangeStatus = 1;
        }

    }

    private void SeverListComboBox_TextChanged(object? sender, EventArgs e)
    {
        N2Nconfig.SuperNodeHostAndPort = SeverListComboBox.Text;
    }

    private async void OnlineServerButton_Click(object sender, EventArgs e)
    {
        OnlineServerButton.Enabled = false;

        var server = await IrisServerApi.GetServerListAsync();

        if (server == null)
        {
            MessageBox.Show("服务器开小差了，稍后再试试吧...");
            return;
        }

        if (server.Count > 0)
        {
            config.OnlineServerList.Clear();
        }

        foreach (var item in server)
        {
            if (item.Port == 7654) { config.OnlineServerList.Add(item.Server!); }
            else { config.OnlineServerList.Add($"{item.Server}:{item.Port}"); }
        }

        configManager.SaveConfig("config", config);
        UpdataServerList();

        MessageBox.Show("更新完成");

        OnlineServerButton.Enabled = true;
    }

    private void AddServerButton_Click(object sender, EventArgs e)
    {
        string? userInput = InputForm.ShowInput("输入服务器地址", "");

        if (userInput == null)
        {
            return;
        }

        if (config.OnlineServerList.Contains(userInput))
        {
            MessageBox.Show("无法添加一个已有的在线服务器");
        }
        if (config.UserServerList.Contains(userInput))
        {
            MessageBox.Show("服务器已经存在");
        }
        else
        {
            config.UserServerList.Add(userInput);
            UpdataServerList();

            MessageBox.Show("添加成功");
            configManager.SaveConfig("config", config);
        }
    }

    private void DelServerButton_Click(object sender, EventArgs e)
    {
        if (config.OnlineServerList.Contains(SeverListComboBox.Text))
        {
            MessageBox.Show("无法删除通过在线服务获取的服务器");
        }
        if (config.UserServerList.Contains(SeverListComboBox.Text))
        {
            config.UserServerList.Remove(SeverListComboBox.Text);
            configManager.SaveConfig("config", config);
            MessageBox.Show("删除成功");
        }
    }

    private async void ServerSpeedTestButton_Click(object sender, EventArgs e)
    {
        ServerSpeedTestButton.Enabled = false;

        MessageBox.Show("点击确定，开始测试服务器，这可能需要一点时间...");

        List<string> combinedList = [.. config.OnlineServerList, .. config.UserServerList];

        foreach (string combined in combinedList)
        {

            string _combined = combined;
            int _port = 7654;

            if (combined.Contains(';'))
            {
                _combined = combined.Split(';')[0];
            }

            var replies = await NetworkTool.PingHostAsync(_combined);
            int average = replies.Count != 0 ? (int)replies.Average() : 999;

            if (average == 999 || average == 0)
            {
                (bool stauts, average) = await NetworkTool.CheckN2Nv3ServerUdpAsync(combined, _port);

                if (!stauts)
                {
                    average = 999;
                }
            }

            serverPing[combined] = average;
        }

        ServerSpeedTestButton.Enabled = true;

        SeverListComboBox_SelectedIndexChanged(sender, EventArgs.Empty);
    }

    private void SeverListComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (serverPing.TryGetValue(SeverListComboBox.Text, out int value))
        {

            label7.Visible = false;

            if (value > 999)
            {
                value = 999;
            }

            if (value == 999)
            {
                label7.Text = "不可用";
            }

            else
            {
                label7.Text = $"{value} ms";
            }

            label7.Visible = true;

        }
        else
        {
            label7.Visible = false;
        }
    }

    private void MetricUpDown_ValueChanged(object? sender, EventArgs e)
    {
        if (MetricUpDown.Value < 0)
        {
            MetricUpDown.Value = 0;
        }

        if (MetricUserCheckBox.Checked)
        {
            N2Nconfig.InterfaceMetric = (int)MetricUpDown.Value;
        }
    }

    private void MetricDefault()
    {
        if (!MetricUserCheckBox.Checked && !MetricCheckBox.Checked && !MetricAutoCheckBox.Checked)
        {
            MetricAutoCheckBox.Checked = true;
            N2Nconfig.InterfaceMetric = 0;
        }
    }

    private void MetricAutoCheckBox_Click(object? sender, EventArgs e)
    {
        if (MetricAutoCheckBox.Checked)
        {
            MetricCheckBox.Checked = false;
            MetricUserCheckBox.Checked = false;
            MetricUpDown.Enabled = false;
            N2Nconfig.InterfaceMetric = 0;
        }
        MetricDefault();
    }

    private void MetricCheckBox_Click(object? sender, EventArgs e)
    {
        if (MetricCheckBox.Checked)
        {
            MetricAutoCheckBox.Checked = false;
            MetricUserCheckBox.Checked = false;
            MetricUpDown.Enabled = false;
            N2Nconfig.InterfaceMetric = 1;
        }
        MetricDefault();
    }

    private void MetricUserCheckBox_Click(object? sender, EventArgs e)
    {
        if (MetricUserCheckBox.Checked)
        {
            MetricUpDown.Enabled = true;
            MetricAutoCheckBox.Checked = false;
            MetricCheckBox.Checked = false;
        }
        else
        {
            MetricUpDown.Enabled = false;
        }
        MetricDefault();
    }

    private async void UninstallTapButton_Click(object sender, EventArgs e)
    {

        if (edgeNodeManage.usedAdapters.Count > 0)
        {
            MessageBox.Show("请先关闭所有开启的节点");
            return;
        }

        await tapNetworkManager.UninstallTapAdapterAsync();
        Application.Exit();
    }

    private async void ClearFirewallRule_Click(object sender, EventArgs e)
    {
        await FirewallManager.DeleteRuleAsync("64_edge.exe");
        await FirewallManager.DeleteRuleAsync("64edge.exe");
        await FirewallManager.DeleteRuleAsync("64_edge");
        await FirewallManager.DeleteRuleAsync("64edge");
        await FirewallManager.DeleteRuleAsync("32_edge.exe");
        await FirewallManager.DeleteRuleAsync("32edge.exe");
        await FirewallManager.DeleteRuleAsync("32_edge");
        await FirewallManager.DeleteRuleAsync("32edge");
        await FirewallManager.DeleteRuleAsync("winipbroadcast.exe");
        await FirewallManager.DeleteRuleAsync("winipbroadcast");
        await FirewallManager.DeleteRuleAsync("N2N启动器.exe");
        await FirewallManager.DeleteRuleAsync("N2N启动器");

        MessageBox.Show("旧版本防火墙规则清理完成 !");
    }

    private async void TapAddButton_Click(object sender, EventArgs e)
    {
        TapAddButton.Enabled = false;

        await tapNetworkManager.InstallTapAsync();

        MessageBox.Show("添加完成");

        TapListBoxView();
        TapAddButton.Enabled = true;
    }

    private async void TapDelButton_Click(object sender, EventArgs e)
    {
        TapAddButton.Enabled = false;

        if (TapListBox.Items.Count == 1)
        {
            MessageBox.Show("无法在此拆卸最后的网卡");
            TapAddButton.Enabled = true;
            return;
        }

        if (TapListBox.SelectedItems.Count == 0)
        {
            MessageBox.Show("选择一张TAP网卡");
            TapAddButton.Enabled = true;
            return;
        }

        bool result = true;

        foreach (var item in TapListBox.SelectedItems)
        {
            result = await tapNetworkManager.UninstallTapAdapterAsync(item.ToString()!, edgeNodeManage.usedAdapters);
        }

        if (!result)
        {
            MessageBox.Show("不能拆卸使用中的网卡...");
            return;
        }

        TapListBoxView();

        UseTapRichTextBox.Text = "网卡已拆卸";

        MessageBox.Show("拆卸完成");

        TapAddButton.Enabled = true;
    }

    private void TapListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (TapListBox.SelectedItem == null)
        {
            return;
        }

        foreach (var networkAdapter in networkAdapters)
        {
            if (networkAdapter.Description == TapListBox.SelectedItem!.ToString())
            {
                string status = networkAdapter.Status switch
                {
                    "Up" => "已连接",
                    "Down" => "未连接",
                    _ => "未知"
                };

                string formattedMac = string.Join(":", Enumerable.Range(0, networkAdapter.MacAddress.Length / 2)
                    .Select(i => networkAdapter.MacAddress.Substring(i * 2, 2)));

                string formattedSpeed = networkAdapter.Speed switch
                {
                    >= 1_000_000_000 => $"{networkAdapter.Speed / 1_000_000_000.0:F1} Gbps",
                    >= 1_000_000 => $"{networkAdapter.Speed / 1_000_000.0:F1} Mbps",
                    _ => $"{networkAdapter.Speed} bps"
                };

                string ipAddress = networkAdapter.IpAddresses.FirstOrDefault() ?? "无";

                UseTapRichTextBox.Text =
                    $"网卡名称: {networkAdapter.Name}\n" +
                    $"网卡速率: {formattedSpeed}\n" +
                    $"网卡MAC地址: {formattedMac}\n" +
                    $"网卡IP地址: {ipAddress}\n" +
                    $"网卡状态: {status}";

            }
        }
    }

    private void StaticIPCheckBox_Click(object? sender, EventArgs e)
    {
        InterfaceAddressTextBox.Enabled = StaticIPCheckBox.Checked;
    }

    private void LocalIPAdvertisementCheckBox_Click(object? sender, EventArgs e)
    {
        LocalIPAdvertisementTextBox.Enabled = LocalIPAdvertisementCheckBox.Checked;
        if (!LocalIPAdvertisementCheckBox.Checked)
        {
            LocalIPAdvertisementTextBox.Text = "auto";
        }
    }

    private void MTUCheckBox_Click(object? sender, EventArgs e)
    {
        MTUTextBox.Enabled = MTUCheckBox.Checked;
        if (!MTUCheckBox.Checked)
        {
            MTUTextBox.Text = "0";
        }
    }

    private void EnablePacketForwardingCheckBox_Click(object? sender, EventArgs e)
    {
        SuperNodeConnectionTypeUDPCheckBox.Enabled = EnablePacketForwardingCheckBox.Checked;
        SuperNodeConnectionTypeTCPCheckBox.Enabled = EnablePacketForwardingCheckBox.Checked;
    }

    private void SuperNodeConnectionTypeUDPCheckBox_Clink(object? sender, EventArgs e)
    {
        SuperNodeConnectionTypeTCPCheckBox.Enabled = SuperNodeConnectionTypeUDPCheckBox.Checked;
    }

    private void RegistrationIntervalCheckBox_Click(object? sender, EventArgs e)
    {
        RegistrationIntervalTextBox.Enabled = RegistrationIntervalCheckBox.Checked;
        if (!RegistrationIntervalCheckBox.Checked)
        {
            RegistrationIntervalTextBox.Text = "0";
        }
    }

    private void RegistrationTTLCheckBox_Click(object? sender, EventArgs e)
    {
        RegistrationTTLTextBox.Enabled = RegistrationTTLCheckBox.Checked;
        if (!RegistrationTTLCheckBox.Checked)
        {
            RegistrationTTLTextBox.Text = "0";
        }
    }

    private void UsePacketFilterRulesCheckBox_Click(object? sender, EventArgs e)
    {
        PacketFilterRulesRichTextBox.Enabled = UsePacketFilterRulesCheckBox.Checked;
    }

    private void AddConfigbutton_Click(object sender, EventArgs e)
    {
        string? userInput = InputForm.ShowInput("给配置起个名字", "");

        if (userInput == null)
        {
            return;
        }

        if (ConfigListBox.Items.Contains(userInput))
        {
            MessageBox.Show("无法添加一个同名配置文件");
            return;
        }
        else
        {
            configManager.SaveConfig(userInput, N2Nconfig);

            ConfigListBox.Items.Add(userInput);
            UpdataConfigList();
            MessageBox.Show("添加成功");
        }
    }

    private void DelConfigbutton_Click(object sender, EventArgs e)
    {

        if (ConfigListBox.SelectedItems == null)
        {
            return;
        }

        if (ConfigListBox.SelectedItem!.ToString() == "default")
        {
            MessageBox.Show("你没法删除默认配置文件");
            return;
        }

        if (configManager.DeleteConfig(ConfigListBox.SelectedItem.ToString()!))
        {
            config.ConfigName = "default";
            configManager.SaveConfig("config", config);
            ReadConfig();

            MessageBox.Show("删除成功");
        }
        else
        {
            MessageBox.Show("删除失败");
        }
    }

    private void ConfigListBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (ConfigListBox.SelectedItems == null)
        {
            return;
        }

        WriteConfig();

        configurationChangeStatus = 1; // 通知配置更改

        config.ConfigName = ConfigListBox.SelectedItem!.ToString()!;
        configManager.SaveConfig("config", config);

        ReadConfig();
    }

    private void BroadcastRepairCheckBox_Click(object sender, EventArgs e)
    {
        config.BroadcastRepair = BroadcastRepairCheckBox.Checked;
    }

    private void MinecraftCheckBox_Click(object sender, EventArgs e)
    {
        config.Minecraft = MinecraftCheckBox.Checked;
    }

    private void UrlRegisterCheckBox_Click(object sender, EventArgs e)
    {
        config.UrlRegister = UrlRegisterCheckBox.Checked;
        if (UrlRegisterCheckBox.Checked)
        {
            Register.RegisterCustomProtocol();
        }
        else
        {
            Register.UnregisterCustomProtocol();
        }
    }

    private void TcpUdpForwCheckBox_Click(object sender, EventArgs e)
    {
        config.TcpUdpForw = TcpUdpForwCheckBox.Checked;
        if (config.TcpUdpForw)
        {
            tcpUdpForw.Start();
        }
        else
        {
            tcpUdpForw.Stop();
        }
    }

    private void AirportalCheckBox_Click(object sender, EventArgs e)
    {
        config.Airportal = AirportalCheckBox.Checked;
        FileTransferService fileTransferService = FileTransferService.Instance;

        if (config.Airportal)
        {
            fileTransferService.Start();
        }
        else
        {
            fileTransferService.Stop();
        }
    }

    private void BootStartCheckBox_Click(object sender, EventArgs e)
    {
        try
        {
            if (BootStartCheckBox.Checked)
            {
                string? exePath = Environment.ProcessPath;

                BootStart.CreateTask("Iris", exePath!);
            }
            else
            {
                BootStart.DeleteTask("Iris");
            }
        }
        catch (Exception ex)
        {
            BootStartCheckBox.Checked = false;
            MessageBox.Show(ex.ToString());
        }
    }

    private void VersionUpdateCheckBox_Click(object sender, EventArgs e)
    {
        config.VersionUpdate = VersionUpdateCheckBox.Checked;
    }
}
