using iris_n2n_launcher.N2N;
using iris_n2n_launcher.Utils;
using iris_n2n_launcher.Utils.FileTransfer;
using STUN.Enums;
using STUN.StunResult;
using System.Text;

namespace iris_n2n_launcher.UI;

public partial class ToolForm : Form
{
    private static readonly EdgeNodeManage edgeNodeManage = EdgeNodeManage.Instance;
    private static readonly TcpUdpForw tcpUdpForw = TcpUdpForw.Instance;
    private static FileTransferService fileTransferService = FileTransferService.Instance;
    private static string localIp = "127.0.0.1";

    private void EdgesInfo()
    {
        while (true)
        {
            Thread.Sleep(1000);

            var nodeInfo = edgeNodeManage.GetNodeInfo("n2n");

            if (nodeInfo == null)
            {
                continue;
            }

            nodeInfo.UdpManager.Read("edges");
            var edges = nodeInfo.UdpManager.GetReceivedData();

            List<float> columnWeights =
            [
                3, 3, 3, 2
            ];

            List<List<string>> edgesInfo = [["info", "用户名", "虚拟局域网ip", "网卡mac地址", "通信模式"]];


            foreach (var edge in edges)
            {
                if (edge.desc == null || edge.desc == "")
                {
                    continue;
                }

                string desc = edge.desc;
                int dashIndex = desc.IndexOf('-');

                if (dashIndex >= 0)
                {
                    desc = desc.Substring(0, dashIndex);
                }

                string ip4addr = ((string)edge.ip4addr).Split('/')[0];

                edgesInfo.Add([(string)edge.macaddr, desc, ip4addr, (string)edge.macaddr, (string)edge.mode]);
            }


            if (RoomInfoGridView.IsHandleCreated && !RoomInfoGridView.IsDisposed)
            {
                RoomInfoGridView.Invoke(() =>
                {
                    GridViewHelper.UpdateData(RoomInfoGridView, edgesInfo, columnWeights);
                });
            }
        }
    }

    private async Task TcpUdpForwInfoAsync()
    {
        try
        {
            var mappings = await tcpUdpForw.QueryMappingsAsync();

            List<float> columnWeights =
            [
                3, 3, 2
            ];

            List<List<string>> tcpUdpForwInfo = [["info", "源地址", "目标地址", "类型"]];

            foreach (var mapping in mappings)
            {
                if (mapping.listen_addr == null)
                {
                    continue;
                }

                tcpUdpForwInfo.Add([$"{mapping.listen_addr}-{mapping.mapping_type}", mapping.listen_addr, mapping.forward_addr, mapping.mapping_type]);
            }

            GridViewHelper.UpdateData(TcpUdpForwDataGridView, tcpUdpForwInfo, columnWeights);
        }
        catch (InvalidOperationException)
        {

        }
    }

    private void FileTransferInfo()
    {
        while (fileTransferService.IsRun())
        {
            Thread.Sleep(1000);
            if (FileTransferDataGridView.InvokeRequired)
            {
                try
                {
                    FileTransferDataGridView.Invoke(new Action(() =>
                    {
                        FileTransferRe();
                    }));
                }
                catch { }
            }
        }
    }

    private void FileTransferRe()
    {
        try
        {
            var queues = fileTransferService.GetTaskQueue();

            List<float> columnWeights =
            [
                3, 1, 2, 2, 1, 1
            ];

            List<List<string>> fileTransferInfo = [["info", "名称", "进度", "大小", "速度", "类型", "状态"]];

            foreach (var queue in queues)
            {
                if (queue.TaskId == null)
                {
                    continue;
                }

                string type = queue.Type == TransferType.Send ? "发送" : "接收";
                string status = "未知";

                if (queue.Status == TransferStatus.Completed)
                {
                    status = "完成";
                }
                if (queue.Status == TransferStatus.Failed)
                {
                    status = "失败";
                }
                if (queue.Status == TransferStatus.Progressing)
                {
                    status = "传输中";
                }
                if (queue.Status == TransferStatus.Canceled)
                {
                    status = "取消";
                }
                if (queue.Status == TransferStatus.Pending)
                {
                    status = "等待回复";
                }

                string fileSize = FormatFileSize(queue.FileSize);

                string transferSpeed = FormatTransferSpeed(queue.TransferSpeed);

                fileTransferInfo.Add([$"{queue.TaskId}", queue.FileName, $"{queue.Progress:0.##}%", fileSize, transferSpeed, type, status]);
            }

            GridViewHelper.UpdateData(FileTransferDataGridView, fileTransferInfo, columnWeights);
        }
        catch (InvalidOperationException)
        {

        }
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size = size / 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    private string FormatTransferSpeed(double bytesPerSecond)
    {
        string[] units = { "B/s", "KB/s", "MB/s", "GB/s" };
        int order = 0;
        double speed = bytesPerSecond;

        while (speed >= 1024 && order < units.Length - 1)
        {
            order++;
            speed = speed / 1024;
        }

        return $"{speed:0.##} {units[order]}";
    }

    private void ToolForm_Shown(object? sender, EventArgs e)
    {

        while (!RoomInfoGridView.IsHandleCreated)
        {
            Thread.Sleep(50);
        }

        var updateFileTransferThread = new Thread(FileTransferInfo)
        {
            IsBackground = true
        };

        var updateEdgesThread = new Thread(EdgesInfo)
        {
            IsBackground = true
        };

        updateFileTransferThread.Start();
        updateEdgesThread.Start();

    }

    public ToolForm(string ip = "127.0.0.1")
    {
        InitializeComponent();
        GridViewHelper.InitializeGridView(RoomInfoGridView);
        GridViewHelper.InitializeGridView(TcpUdpForwDataGridView);
        GridViewHelper.InitializeGridView(TcpUdpForwDataGridView);
        GridViewHelper.InitializeGridView(FileTransferDataGridView);

        RoomInfoGridView.Tag = new Dictionary<string, object>
        {
            ["OnRowDoubleClick"] = (Action<List<string>>)(row =>
            {
                if (row.Count > 2)
                {
                    string ip = row[1];

                    var t = new Thread(() => Clipboard.SetText(ip));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start(); // 暂无解决办法（

                    MessageBox.Show($"IP 已复制：{ip}");
                }
            })
        };

        localIp = ip;

        string[] ipParts = localIp.Split('.');
        if (ipParts.Length == 4)
        {
            PingIpBox.Text = $"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}.";
        }

        Shown += ToolForm_Shown;
    }
    private async void StunTestButton_Click(object sender, EventArgs e)
    {
        label1.Text = "检测中...";
        StunHelpRichTextBox.Text = "正在检测NAT类型...";
        StunTestButton.Enabled = false;

        try
        {
            var stunHelper = new StunHelper();
            var result = await stunHelper.Stun5780TestAsync(StunServerComboBox.Text);

            if (result != null)
            {
                if (result.BindingTestResult != BindingTestResult.Success)
                {
                    label1.Text = "UDP不可达";
                    StunHelpRichTextBox.Text = $"STUN绑定测试失败: {result.BindingTestResult}\n"
                        + "可能原因：\n"
                        + "▪ 防火墙阻止UDP\n"
                        + "▪ STUN服务器无响应\n"
                        + "▪ 网络地址转换失败";
                    return;
                }
                var natType5780 = DetermineNatType(result);
                UpdateUIWithDetailedNatType(natType5780, result);
            }
            else
            {
                var natType3489 = await stunHelper.Stun3489TestAsync(StunServerComboBox.Text);

                if (natType3489 != null)
                {
                    UpdateUIWithDetailedNatType(natType3489, null);
                }
                else
                {
                    label1.Text = "检测失败";
                    StunHelpRichTextBox.Text = $"服务器未返回\n"
                        + "建议：\n"
                        + "1. 尝试其他STUN服务器\n";
                }
            }

        }
        catch (Exception ex)
        {
            label1.Text = "检测异常";
            StunHelpRichTextBox.Text = $"STUN协议错误: {ex.Message}\n"
                + "建议：\n"
                + "1. 尝试其他STUN服务器\n"
                + "2. 检查本地网络策略";
        }
        finally
        {
            StunTestButton.Enabled = true;
        }
    }

    private static NatType DetermineNatType(StunResult5389 result)
    {
        if (result.FilteringBehavior == FilteringBehavior.EndpointIndependent)
        {
            return result.MappingBehavior == MappingBehavior.EndpointIndependent
                ? NatType.FullCone
                : NatType.RestrictedCone;
        }
        else if (result.FilteringBehavior == FilteringBehavior.AddressDependent)
        {
            return NatType.RestrictedCone;
        }
        else if (result.FilteringBehavior == FilteringBehavior.AddressAndPortDependent)
        {
            return result.MappingBehavior == MappingBehavior.AddressAndPortDependent
                ? NatType.Symmetric
                : NatType.PortRestrictedCone;
        }

        return NatType.Unknown;
    }

    private void UpdateUIWithDetailedNatType(NatType? natType, StunResult5389? result)
    {
        var sb = new StringBuilder();

        if (result != null)
        {
            sb.AppendLine($"▪ 映射行为: {result.MappingBehavior}");
            sb.AppendLine($"▪ 过滤行为: {result.FilteringBehavior}");
            sb.AppendLine($"▪ 公网端点: {result.PublicEndPoint}");
            sb.AppendLine($"▪ 本地端点: {result.LocalEndPoint}");
        }

        switch (natType)
        {
            case NatType.FullCone:
                label1.Text = "完全锥形NAT";
                sb.Insert(0, "任何外部主机均可访问映射端口\n");
                sb.AppendLine("➤ P2P兼容性最佳，适合作为主机");
                break;

            case NatType.RestrictedCone:
                label1.Text = "受限锥形NAT";
                sb.Insert(0, "仅允许通信过的IP访问\n");
                sb.AppendLine("➤ 需要先发送外出包才能接收");
                break;

            case NatType.PortRestrictedCone:
                label1.Text = "端口受限锥形NAT";
                sb.Insert(0, "严格限制IP和端口访问\n");
                sb.AppendLine("➤ 建议启用端口转发");
                break;

            case NatType.Symmetric:
                label1.Text = "对称NAT";
                sb.Insert(0, "每个连接使用独立映射\n");
                sb.AppendLine("➤ 需要服务器中转...");
                break;

            default:
                label1.Text = "未知类型";
                sb.Insert(0, "更换服务器或防火墙?\n");
                break;
        }

        StunHelpRichTextBox.Text = sb.ToString();
        label5.Visible = natType == NatType.Symmetric;
    }

    private void PingResultAdd(int rtt)
    {
        PingResultListBox.Items.Add($"{PingResultListBox.Items.Count}: {PingIpBox.Text} - {(rtt == -1 ? "超时" : rtt + "ms")}");
    }

    private async void PingTestButton_Click(object sender, EventArgs e)
    {
        PingTestButton.Enabled = false;
        PingResultListBox.Items.Clear();
        PingResultListBox.Items.Add("Ping 测试中...");
        PingIpBox.Enabled = false;
        var result = await NetworkTool.PingHostWithCallbackAsync(PingIpBox.Text, PingResultAdd, (int)PingUpDown.Value);
        PingResultListBox.Items.Add($"平均延迟为 {result[0]} ms\n 丢包率为 {result[1]} %");
        PingIpBox.Enabled = true;
        PingTestButton.Enabled = true;
    }

    private async void TcpUdpForwAddButton_Click(object sender, EventArgs e)
    {
        if (!tcpUdpForw.IsRun())
        {
            MessageBox.Show("请先开启功能...");
            return;
        }

        var listenAddr = InputForm.ShowInput("源地址");
        if (string.IsNullOrWhiteSpace(listenAddr))
            return;

        var forwardAddr = InputForm.ShowInput("目标地址");
        if (string.IsNullOrWhiteSpace(forwardAddr))
            return;

        var mappingType = InputForm.ShowInput("映射类型 (tcp / udp / tcpudp / udptcp)", "tcpudp");
        if (string.IsNullOrWhiteSpace(mappingType))
            return;

        mappingType = mappingType.ToLower();
        if (mappingType != "tcp" && mappingType != "udp" && mappingType != "tcpudp" && mappingType != "udptcp")
        {
            MessageBox.Show("映射类型必须是 tcp / udp / tcpudp / udptcp 之一", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var tempResult = MessageBox.Show("是否设置为临时映射？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        bool temp = (tempResult == DialogResult.Yes);

        try
        {
            var result = await tcpUdpForw.AddMappingAsync(listenAddr, forwardAddr, mappingType, temp);
            if (result)
            {
                MessageBox.Show("映射添加成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await TcpUdpForwInfoAsync();
            }
            else
            {
                MessageBox.Show("映射添加失败");
            }
        }
        catch (InvalidOperationException)
        {

        }
    }

    private async void TcpUdpForwDelButton_Click(object sender, EventArgs e)
    {
        if (TcpUdpForwDataGridView.SelectedRows.Count == 0)
        {
            MessageBox.Show("至少要选中一行...");
            return;
        }

        var selectedRow = TcpUdpForwDataGridView.SelectedRows[0];
        List<string> rowData = [];
        foreach (DataGridViewCell cell in selectedRow.Cells)
            rowData.Add(cell.Value?.ToString() ?? "");

        if (rowData.Count < 3)
        {
            return;
        }

        try
        {
            var result = await tcpUdpForw.DeleteMappingAsync(rowData[0], rowData[2]);
            if (result)
            {
                MessageBox.Show("映射删除成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await TcpUdpForwInfoAsync();
            }
            else
            {
                MessageBox.Show("映射删除失败");
            }
        }
        catch (InvalidOperationException)
        {

        }
    }

    private async void TcpUdpForwReButton_Click(object sender, EventArgs e)
    {
        await TcpUdpForwInfoAsync();
    }

    private void SelectedIndexChanged(object sender, EventArgs e)
    {
        int selectedIndex = tabControl1.SelectedIndex;

        if (selectedIndex == 3)
        {
            TcpUdpForwReButton_Click(sender, e);
        }
    }

    private void FileTransferAdd()
    {
        if (!fileTransferService.IsRun())
        {
            MessageBox.Show("请先开启功能...");
            return;
        }

        List<string> filePaths = [];

        string? server = InputForm.ShowInput("输入发送目标 ip", $"如需接收本机ip: {localIp}:{fileTransferService.ReceiverPort}");

        if (!NetworkTool.ValidateIPAddress(server))
        {
            return;
        }

        string ipAddress = server!.Split(':')[0];
        int port = int.Parse(server.Split(":")[1]);

        using (var openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Title = "选择要发送的文件";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "所有文件 (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePaths.Add(openFileDialog.FileName);
            }
            else
            {
                return;
            }
        }



        fileTransferService.SendFileToIp(ipAddress, filePaths, port);
    }

    private void FileTransferAddButton_Click(object sender, EventArgs e)
    {
        var t = new Thread(() => FileTransferAdd());
        t.SetApartmentState(ApartmentState.STA);
        t.Start();
    }

    private void FileTransferDelButton_Click(object sender, EventArgs e)
    {
        if (!fileTransferService.IsRun())
        {
            return;
        }
    }

    private void FileTransferReButton_Click(object sender, EventArgs e)
    {
        if (!fileTransferService.IsRun())
        {
            return;
        }

        FileTransferRe();
    }
}
