using iris_n2n_launcher.Config;
using iris_n2n_launcher.N2N;
using iris_n2n_launcher.Utils;

namespace iris_n2n_launcher.UI;

public partial class MultipleEdgeForm : Form
{
    private readonly EdgeNodeManage edgeNodeManage = EdgeNodeManage.Instance;
    private ConfigManager configManager = ConfigManager.Instance;
    private bool exit = false;
    public MultipleEdgeForm()
    {
        InitializeComponent();
        GridViewHelper.InitializeGridView(NodeDataGridView);

        var updateNodesInfo = new Thread(NodesInfo)
        {
            IsBackground = true
        };

        updateNodesInfo.Start();
    }

    private void NodesInfo()
    {
        while (!exit)
        {
            Thread.Sleep(1000);
            if (NodeDataGridView.InvokeRequired)
            {
                try
                {
                    NodeDataGridView.Invoke(new Action(() =>
                    {
                        ReNodes();
                    }));
                }
                catch { }
            }
        }
    }

    private void ReNodes()
    {
        try
        {
            var nodes = edgeNodeManage.GetActiveNodes();

            List<float> columnWeights =
            [
                2, 3, 3
            ];

            List<List<string>> nodesInfo = [["info", "序号", "房间名", "ip地址"]];

            foreach (var node in nodes)
            {
                if (node.Key == "")
                {
                    continue;
                }

                string ip4addr = "未知";

                if (node.Value.Status != null)
                {
                    ip4addr = (string)node.Value.Status.ip4addr;
                }

                nodesInfo.Add([node.Value.Id, node.Value.Id, node.Value.Parameters.Community, ip4addr]);
            }

            GridViewHelper.UpdateData(NodeDataGridView, nodesInfo, columnWeights);
        }
        catch (InvalidOperationException)
        {

        }
    }
    static List<string> GetColumnData(DataGridView dataGridView, int num)
    {
        var result = new List<string>();

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (!row.IsNewRow)
            {
                var cellValue = row.Cells[num].Value?.ToString() ?? string.Empty;
                result.Add(cellValue);
            }
        }

        return result;
    }

    private async void AddNodeButton_Click(object sender, EventArgs e)
    {
        var nodeName = InputForm.ShowInput("起个名字?", GenerateRandomString());

        if (nodeName == null || nodeName == "")
        {
            return;
        }

        if (GetColumnData(NodeDataGridView, 0).Contains(nodeName) || nodeName == "n2n")
        {
            MessageBox.Show("节点ID 不能重复");
            return;
        }

        var config = configManager.LoadConfig<Configuration>("config");
        var n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(config.ConfigName);

        var room = InputForm.ShowInput("输入一个新的房间名", "");

        if (room == null || room == "")
        {
            return;
        }

        if (GetColumnData(NodeDataGridView, 1).Contains(room))
        {
            MessageBox.Show("房间名 不能重复");
            return;
        }

        n2NConfiguration.Community = room;

        int states = await edgeNodeManage.StartNodeAsync(nodeName, n2NConfiguration);

        if (states == 10)
        {
            MessageBox.Show("检测到重复启动...");
            return;
        }

        if (states == 11)
        {
            MessageBox.Show("未能自动发现可用网卡...\n或许前往设置添加一个?");
            return;
        }

        if (states == 12)
        {
            MessageBox.Show("进程创建失败...");
            return;
        }

        if (states == 13)
        {
            MessageBox.Show("添加到节点列表失败...");
            return;
        }
    }

    public static string GenerateRandomString(int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]);
    }

    private void DelNodeButton_Click(object sender, EventArgs e)
    {

        if (NodeDataGridView.SelectedRows.Count == 0)
        {
            MessageBox.Show("至少要选中一行...");
            return;
        }

        var selectedRow = NodeDataGridView.SelectedRows[0];
        List<string> rowData = [];
        foreach (DataGridViewCell cell in selectedRow.Cells)
            rowData.Add(cell.Value?.ToString() ?? "");

        if (rowData.Count != 3)
        {
            return;
        }

        if (rowData[0] == null)
        {
            return;
        }

        if (rowData[0] == "序号")
        {
            return;
        }

        if (rowData[0] == "n2n")
        {
            MessageBox.Show("前往主页面关闭主节点");
            return;
        }

        edgeNodeManage.StopNode(rowData[0]);
    }

    private void MultipleEdgeForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        exit = true;
    }
}
