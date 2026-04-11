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
                2, 3, 3, 3, 4
            ];

            List<List<string>> nodesInfo = [["info", "序号", "房间名", "状态", "最近错误"]];

            foreach (var node in nodes)
            {
                if (node.Key == "")
                {
                    continue;
                }

                string statusText = node.Value.LastStatusText ?? node.Value.Status?.Ip4Addr ?? "未知";
                string lastError = node.Value.LastErrorMessage ?? node.Value.LastWarningMessage ?? string.Empty;

                nodesInfo.Add([node.Value.Id, node.Value.Id, node.Value.Parameters.Community, statusText, lastError]);
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

        var n2NConfiguration = SelectConfigForm.ShowInput();

        if (n2NConfiguration == null)
        {
            return;
        }

        if (GetColumnData(NodeDataGridView, 1).Contains(n2NConfiguration.Community))
        {
            MessageBox.Show("房间名 不能重复");
            return;
        }

        var startResult = await edgeNodeManage.StartNodeAsync(nodeName, n2NConfiguration);

        if (!startResult.Success)
        {
            MessageBox.Show(startResult.Message ?? "节点启动失败...");
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
