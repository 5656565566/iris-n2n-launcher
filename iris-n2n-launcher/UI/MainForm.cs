
using iris_n2n_launcher.N2N;
using iris_n2n_launcher.Utils;
using iris_n2n_launcher.Config;
using iris_n2n_launcher.TAP;
using static iris_n2n_launcher.Utils.FirewallHelper;
using iris_n2n_launcher.Utils.FileTransfer;
using System.Text.RegularExpressions;

namespace iris_n2n_launcher.UI;

public partial class MainForm : Form
{
    private readonly EdgeNodeManage edgeNodeManage = EdgeNodeManage.Instance;
    private readonly string nodeName = "n2n";
    private readonly ConfigManager configManager = ConfigManager.Instance;
    private static readonly TapNetworkManager tapNetworkManager = new();
    private static readonly MinecraftLanProxy minecraftLanProxy = MinecraftLanProxy.Instance;
    private static readonly FileTransferService fileTransferService = FileTransferService.Instance;
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
        var mianInfoThread = new Thread(MianInfo)
        {
            IsBackground = true
        };

        mianInfoThread.Start();
    }

    private void MianInfo()
    {
        while (true)
        {
            Thread.Sleep(2000);
            try
            {
                IPtextBox.Invoke(new Action(() => { MianEdge(); }));
            }
            catch { }
        }
    }

    private void MianEdge()
    {
        var nodes = edgeNodeManage.GetActiveNodes();

        int active = 1; // 无节点运行

        foreach (var node in nodes)
        {
            if (node.Key == "n2n")
            {
                active *= -1; // 主节点运行中计数为负
            }
            else
            {
                if (active > 0)
                {
                    active += 1;
                }
                else
                {
                    active -= 1;
                }
            }
        }

        if (active < 1)
        {
            
        }

        if (active > 1)
        {
            IPtextBox.Text = "其他节点运行中...";
        }

        if (active == 1)
        {
            if (IPtextBox.Text != "")
            {
                MessageBox.Show("进程崩溃...");
                StopMainEdge();
            }
        }

        if (active == -1)
        {
            foreach (var node in nodes)
            {
                if(node.Key == "n2n")
                {
                    try
                    {
                        IPtextBox.Text = node.Value.Status.ip4addr;
                    }
                    catch
                    {

                    }
                    SwitchButton.Text = "正在运行";
                    SwitchButton.Enabled = false;
                    StopButton.Enabled = true;
                }
            }

        }
    }

    private void ProcessSharingLink(string link)
    {

        string pattern = @"iris:\/\/[^\s]+";
        Regex regex = new Regex(pattern);

        Match match = regex.Match(link.TrimEnd('/'));

        if (match.Success)
        {
            string irisLink = match.Value;
            string[] share = ShareForm.ReadUrl(irisLink.Replace("iris://", "")).Split('#');

            var config = configManager.LoadConfig<Configuration>("config");
            var n2nconfig = configManager.LoadConfig<N2NConfiguration>(config.ConfigName);

            n2nconfig.SuperNodeHostAndPort = share[0];
            n2nconfig.Community = share[1];

            UrlJoin(n2nconfig);
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

    private void MainForm_Load(object sender, EventArgs e)
    {
        if (seletMode == 1) { SwitchButton_Click(null, EventArgs.Empty); Hide();  }
        if (seletMode == 2) { ProcessSharingLink(share!); }
        if (seletMode == 3) { IPtextBox.Invoke(new Action(() => { MianEdge(); })); }
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
                Environment.Exit(0);
                break;

            case WM_ENDSESSION:
                if (m.WParam != IntPtr.Zero)
                {
                    exitN2N = true;
                    Environment.Exit(0);
                }
                break;
        }

        base.WndProc(ref m);
    }

    private void SwitchButton_Click(object? sender, EventArgs e)
    {

        if (RoomTextBox.Text == "")
        {
            MessageBox.Show("请先填写房间名称...");
            return;
        }

        string configMame = configManager.LoadConfig<Configuration>("config").ConfigName;
        N2NConfiguration n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(configMame);

        if (n2NConfiguration.SuperNodeHostAndPort == "")
        {
            MessageBox.Show("请先选择一个服务器吧...");
            return;
        }

        AddEdge(n2NConfiguration);
    }

    private async void AddEdge(N2NConfiguration n2NConfiguration)
    {
        SwitchButton.Text = "启动中...";
        SwitchButton.Enabled = false;

        int states = await edgeNodeManage.StartNodeAsync(nodeName, n2NConfiguration);

        if (states == 10)
        {
            StopMainEdge();
            MessageBox.Show("检测到重复启动...");
            return;
        }

        if (states == 11)
        {
            StopMainEdge();
            MessageBox.Show("未能自动发现可用网卡...");
            return;
        }

        if (states == 12)
        {
            StopMainEdge();
            MessageBox.Show("进程创建失败...");
            return;
        }

        if (states == 13)
        {
            StopMainEdge();
            MessageBox.Show("添加到节点列表失败...");
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
                        if (nodeInfo != null && nodeInfo.Status != null)
                        {
                            IPtextBox.Text = nodeInfo!.Status!.ip4addr;
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
        exitN2N = true;
    }

    private void SettingButton_Click(object sender, EventArgs e)
    {
        SettingForm settingFrom = new();
        settingFrom.ShowDialog();
        if (settingFrom.SettingChanging() == 1) // TODO 处理不同的配置文件变更提示
        {
            ReadConfig();

            if (StopButton.Enabled)
            {
                StopMainEdge();

                string configMame = configManager.LoadConfig<Configuration>("config").ConfigName;
                N2NConfiguration n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(configMame);

                AddEdge(n2NConfiguration);
            }
        }
    }

    private void StopButton_Click(object? sender, EventArgs e)
    {
        StopMainEdge();
    }

    private void StopMainEdge()
    {
        edgeNodeManage.StopNode(nodeName);
        IPtextBox.Text = "";
        SwitchButton.Text = "一键启动";
        SwitchButton.Enabled = true;
        StopButton.Enabled = false;
        string configMame = configManager.LoadConfig<Configuration>("config").ConfigName;
        N2NConfiguration n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(configMame);
        RoomTextBox.Text = n2NConfiguration.Community;
    }

    private void UrlJoinbutton_Click(object sender, EventArgs e)
    {
        string configMame = configManager.LoadConfig<Configuration>("config").ConfigName;
        N2NConfiguration n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(configMame);

        (string? superNodeHostAndPort, string? community) = ShareForm.ShowShareForm(n2NConfiguration.SuperNodeHostAndPort, n2NConfiguration.Community, IPtextBox.Text);

        if (superNodeHostAndPort != null || community != null)
        {
            n2NConfiguration.SuperNodeHostAndPort = superNodeHostAndPort!;
            n2NConfiguration.Community = community!;
            UrlJoin(n2NConfiguration);
        }
    }
    public void UrlJoin(N2NConfiguration n2NConfiguration)
    {
        if (StopButton.Enabled)
        {
            StopMainEdge();
        }

        RoomTextBox.TextChanged -= RoomTextBox_TextChanged;
        RoomTextBox.Text = n2NConfiguration.Community;
        RoomTextBox.TextChanged += RoomTextBox_TextChanged;

        AddEdge(n2NConfiguration);
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
