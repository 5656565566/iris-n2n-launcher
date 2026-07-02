
using iris_n2n_launcher.Config;
using iris_n2n_launcher.N2N;
using iris_n2n_launcher.TAP;
using iris_n2n_launcher.Utils;
using iris_n2n_launcher.Utils.FileTransfer;
using System.Text.RegularExpressions;

using static iris_n2n_launcher.Utils.FirewallHelper;

namespace iris_n2n_launcher.UI;

public partial class MainForm : Form
{
    private readonly EdgeNodeManage edgeNodeManage = EdgeNodeManage.Instance;
    private readonly string nodeName = "n2n";
    private readonly ConfigManager configManager = ConfigManager.Instance;
    private static readonly MinecraftLanProxy minecraftLanProxy = MinecraftLanProxy.Instance;
    private static readonly BackgroundEventManager eventManager = new();
    private static int seletMode = 0;
    public string? share;
    public bool exitN2N = false; // 正常退出标志位

    public MainForm(int SeletMode)
    {
        seletMode = SeletMode;
        InitializeComponent();
        ReadConfig();

        Shown += MainForm_Shown;
        N2NNotifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
    }

    private void MainForm_Shown(object? sender, EventArgs e)
    {
        eventManager.AddEvent("MianInfo", MianInfo, 1000);
    }

    private void MianInfo()
    {
        IPtextBox.Invoke(new Action(MianEdge));
    }

    private void MianEdge()
    {
        var nodes = edgeNodeManage.GetActiveNodes();
        var mainNode = nodes.TryGetValue("n2n", out var nodeInfo) ? nodeInfo : null;
        int otherNodeCount = nodes.Keys.Count(key => key != "n2n");

        if (mainNode != null)
        {
            IPtextBox.Text = mainNode.Status?.Ip4Addr ?? string.Empty;
            SwitchButton.Text = mainNode.LastStatusText ?? "正在运行";
            SwitchButton.Enabled = false;
            StopButton.Enabled = true;
            return;
        }

        if (otherNodeCount > 0)
        {
            IPtextBox.Text = "其他节点运行中...";
            return;
        }

        if (!string.IsNullOrEmpty(IPtextBox.Text))
        {
            MessageBox.Show("所有节点已关闭...");
            StopMainEdge();
        }
    }


    private async Task ProcessSharingLinkAsync(string link)
    {

        string pattern = @"iris:\/\/[^\s]+";
        Regex regex = new Regex(pattern);

        Match match = regex.Match(link.TrimEnd('/'));

        if (match.Success)
        {
            string irisLink = match.Value;
            string[] share = ShareForm.ReadUrl(irisLink.Replace("iris://", "")).Split('#');

            if (share.Length < 2)
            {
                MessageBox.Show("分享链接格式无效");
                return;
            }

            var config = configManager.LoadConfig<Configuration>("config");
            var n2nconfig = configManager.LoadConfig<N2NConfiguration>(config.ConfigName);

            n2nconfig.SuperNodeHostAndPort = share[0];
            n2nconfig.Community = share[1];

            await UrlJoin(n2nconfig);
        }
    }
    public void ReadConfig()
    {
        RoomTextBox.TextChanged -= RoomTextBox_TextChanged;

        string configMame = configManager.LoadConfig<Configuration>("config").ConfigName;
        N2NConfiguration n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(configMame);
        RoomTextBox.Text = n2NConfiguration.Community;

        RoomTextBox.TextChanged += RoomTextBox_TextChanged;
    }

    private void MainForm_SizeChanged(object sender, EventArgs e)
    {
        if (WindowState == FormWindowState.Minimized)
        {
            int tipShowMilliseconds = 1000;
            string tipTitle = "N2N启动器已经最小化";
            string tipContent = "双击托盘图标显示窗口^w^";
            ToolTipIcon tipType = ToolTipIcon.Info;
            N2NNotifyIcon.ShowBalloonTip(tipShowMilliseconds, tipTitle, tipContent, tipType);

            Hide();
        }
    }

    private new void Hide()
    {
        base.Hide();
        ShowInTaskbar = false;
    }

    private new void Show()
    {
        ShowInTaskbar = true;
        base.Show();
    }

    private void NotifyIcon_MouseDoubleClick(object? sender, MouseEventArgs e)
    {
        Show();
        WindowState = FormWindowState.Normal;
        BringToFront();
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        if (seletMode == 1) { SwitchButton_Click(null, EventArgs.Empty); Hide();  }
        if (seletMode == 2) { await ProcessSharingLinkAsync(share!); }
    }

    private void 显示窗口ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Show();
        WindowState = FormWindowState.Normal;
    }

    private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        exitN2N = true;
        Close();
    }

    /// <summary>
    /// 窗口过程的回调函数 防止关机卡死
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        const int WM_QUERYENDSESSION = 0x0011;
        const int WM_ENDSESSION = 0x0016;

        switch (m.Msg)
        {
            case WM_QUERYENDSESSION:
                exitN2N = true;
                BeginInvoke(new Action(() => Close()));
                m.Result = new IntPtr(1);
                return;

            case WM_ENDSESSION:
                if (m.WParam != IntPtr.Zero)
                {
                    exitN2N = true;
                    BeginInvoke(new Action(() => Close()));
                    m.Result = IntPtr.Zero;
                    return;
                }
                break;
        }

        base.WndProc(ref m);
    }

    private async void SwitchButton_Click(object? sender, EventArgs e)
    {

        if (RoomTextBox.Text == "")
        {
            MessageBox.Show("请先填写房间名称...");
            return;
        }

        Configuration config = configManager.LoadConfig<Configuration>("config");
        string configMame = config.ConfigName;
        N2NConfiguration n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(configMame);

        if (n2NConfiguration.SuperNodeHostAndPort == "")
        {
            MessageBox.Show("请先选择一个服务器吧...");
            return;
        }

        if (config.FirewallEnabledOnExit)
        {
            FirewallOperateByObject(false, false, false);
        }

        await AddEdgeAsync(n2NConfiguration);
    }

    private async Task AddEdgeAsync(N2NConfiguration n2NConfiguration)
    {
        SwitchButton.Text = "启动中...";
        SwitchButton.Enabled = false;

        var startResult = await edgeNodeManage.StartNodeAsync(nodeName, n2NConfiguration);

        if (!startResult.Success)
        {
            StopMainEdge();
            MessageBox.Show(startResult.Message ?? "节点启动失败...");
            return;
        }

        await Task.Run(async () =>
        {
            int tryTimes = 20;
            await Task.Delay(500);

            while (tryTimes > 0)
            {
                try
                {
                    await Task.Delay(500);

                    var nodeInfo = edgeNodeManage.GetNodeInfo(nodeName);

                    SwitchButton.Invoke(() =>
                    {
                        if (nodeInfo != null && (!string.IsNullOrWhiteSpace(nodeInfo.LastStatusText) || nodeInfo.Status != null))
                        {
                            IPtextBox.Text = nodeInfo.Status?.Ip4Addr ?? string.Empty;
                            SwitchButton.Text = nodeInfo.LastStatusText ?? "启动中...";
                            StopButton.Enabled = true;
                            tryTimes = 0;
                        }
                    });
                    tryTimes -= 1;
                }
                catch (TaskCanceledException)
                {

                }
                catch { }
            }
        });

        SwitchButton.Text = "正在运行";

        if (SwitchButton.Enabled)
        {
            return;
        }

        if (IPtextBox.Text == "")
        {
            StopButton.Enabled = true;
            var tapAdapers = TapNetworkManager.ShowNetworkInterfaceMessage(AdapterType.TAP);
            var nodeInfo = edgeNodeManage.GetNodeInfo(nodeName);

            if (nodeInfo == null)
            {
                StopMainEdge();
                MessageBox.Show("核心启动失败...");
                return;
            }

            foreach (var tap in tapAdapers)
            {
                if (tap.Id == nodeInfo.Parameters.DeviceName)
                {
                    IPtextBox.Text = tap.IpAddresses[0].ToString(); // IPV4
                }
            }

            if (IPtextBox.Text == "")
            {
                StopMainEdge();
                MessageBox.Show("服务器连接超时...");
                return;
            }
        }
        else
        {
            var ping = await NetworkTool.PingHostAsync(IPtextBox.Text);

            await FirewallManager.AllowIPAsync("n2n-ip", IPtextBox.Text);
            await FirewallManager.AllowPingAsync("n2n-ping");

            minecraftLanProxy.SetBroadcastIp(IPtextBox.Text);
            if (configManager.LoadConfig<Configuration>("config").Minecraft)
            {
                try
                {
                    minecraftLanProxy.Start();
                }
                catch
                {
                    minecraftLanProxy.Stop();
                }
            }
            else
            {
                minecraftLanProxy.Stop();
            }

            if (!ping.Any(x => x != -1))
            {
                MessageBox.Show("IP 未在本地生效\n但你可以尝试继续使用");
                return;
            }
        }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        var activeNodes = edgeNodeManage.GetActiveNodes();

        if (e.CloseReason == CloseReason.UserClosing && !activeNodes.IsEmpty)
        {
            DialogResult result = MessageBox.Show("N2N正在运行中哦\n如果关闭会导致掉线", "要关掉我吗 ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
        }

        eventManager.StopAllEvents();
        exitN2N = true;
    }

    private async void SettingButton_Click(object sender, EventArgs e)
    {
        SettingForm settingFrom = new();
        settingFrom.ShowDialog();
        int settingChangeStatus = settingFrom.SettingChanging();
        if (settingChangeStatus != 0) // TODO 处理不同的配置文件变更提示
        {
            ReadConfig();

            if (settingChangeStatus == 1 && StopButton.Enabled)
            {
                if (!await StopMainEdgeAsync())
                {
                    MessageBox.Show("核心关闭失败，设置已保存但未重新启动");
                    return;
                }

                string configMame = configManager.LoadConfig<Configuration>("config").ConfigName;
                N2NConfiguration n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(configMame);

                await AddEdgeAsync(n2NConfiguration);
            }
            else
            {
                ApplyRuntimeConfiguration();
            }
        }
    }

    private async void StopButton_Click(object? sender, EventArgs e)
    {
        await StopMainEdgeAsync();
    }

    private void StopMainEdge()
    {
        edgeNodeManage.StopNode(nodeName);
        ResetMainEdgeView();
    }

    private async Task<bool> StopMainEdgeAsync()
    {
        bool stopped = await edgeNodeManage.StopNodeAsync(nodeName);
        ResetMainEdgeView();
        return stopped || edgeNodeManage.GetNodeInfo(nodeName) == null;
    }

    private void ResetMainEdgeView()
    {
        IPtextBox.Text = "";
        SwitchButton.Text = "一键启动";
        SwitchButton.Enabled = true;
        StopButton.Enabled = false;
        string configMame = configManager.LoadConfig<Configuration>("config").ConfigName;
        N2NConfiguration n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(configMame);
        RoomTextBox.Text = n2NConfiguration.Community;
    }

    private void ApplyRuntimeConfiguration()
    {
        Configuration currentConfig = configManager.LoadConfig<Configuration>("config");

        if (!currentConfig.Minecraft)
        {
            minecraftLanProxy.Stop();
            return;
        }

        if (string.IsNullOrWhiteSpace(IPtextBox.Text))
        {
            return;
        }

        minecraftLanProxy.SetBroadcastIp(IPtextBox.Text);
        try
        {
            minecraftLanProxy.Start();
        }
        catch
        {
            minecraftLanProxy.Stop();
        }
    }

    private async void UrlJoinbutton_Click(object sender, EventArgs e)
    {
        string configMame = configManager.LoadConfig<Configuration>("config").ConfigName;
        N2NConfiguration n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(configMame);

        (string? superNodeHostAndPort, string? community) = ShareForm.ShowShareForm(n2NConfiguration.SuperNodeHostAndPort, n2NConfiguration.Community, IPtextBox.Text);

        if (superNodeHostAndPort != null || community != null)
        {
            n2NConfiguration.SuperNodeHostAndPort = superNodeHostAndPort!;
            n2NConfiguration.Community = community!;
            await UrlJoin(n2NConfiguration);
        }
    }
    public async Task UrlJoin(N2NConfiguration n2NConfiguration)
    {
        if (StopButton.Enabled)
        {
            StopMainEdge();
        }

        RoomTextBox.TextChanged -= RoomTextBox_TextChanged;
        RoomTextBox.Text = n2NConfiguration.Community;
        RoomTextBox.TextChanged += RoomTextBox_TextChanged;

        await AddEdgeAsync(n2NConfiguration);
    }
    private void ToolButtom_Click(object sender, EventArgs e)
    {
        ToolForm toolFrom = new(IPtextBox.Text);
        toolFrom.ShowDialog();
    }

    private void RoomTextBox_TextChanged(object? sender, EventArgs e)
    {
        string configMame = configManager.LoadConfig<Configuration>("config").ConfigName;
        N2NConfiguration n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(configMame);
        n2NConfiguration.Community = RoomTextBox.Text;
        configManager.SaveConfig(configMame, n2NConfiguration);
    }

    private void 关闭N2NToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (StopButton.Enabled)
        {   
            StopMainEdge();
        }
    }

    private void MultipleEdgeButton_Click(object sender, EventArgs e)
    {
        if (IPtextBox.Text == "")
        {
            MessageBox.Show("请先启动N2N...");
            return;
        }
        MultipleEdgeForm multipleEdgeForm = new();
        multipleEdgeForm.ShowDialog();
    }
}
