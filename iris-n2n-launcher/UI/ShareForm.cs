using System.Text;
using System.Text.RegularExpressions;

namespace iris_n2n_launcher.UI;

public partial class ShareForm : Form
{
    private string? superNodeHostAndPort;
    private string? community;
    private string? n2NVirtualNetworkIP;

    public ShareForm(string? SuperNodeHostAndPort, string? Community, string N2NVirtualNetworkIP)
    {
        InitializeComponent();
        superNodeHostAndPort = SuperNodeHostAndPort;
        community = Community;
        n2NVirtualNetworkIP = N2NVirtualNetworkIP;
    }

    private void ShareForm_Load(object sender, EventArgs e)
    {
        if (n2NVirtualNetworkIP != null && n2NVirtualNetworkIP != "") { ShareUrl(); }
    }

    private void ShareUrl()
    {
        groupBox2.Text = "点击按钮复制口令, 分享给好友吧";
        string data = $"{superNodeHostAndPort}#{community}";
        ShareRichTextBox.Text = $"我正在使用N2N联机\n我的IP是:{n2NVirtualNetworkIP}\niris://{NewUrl(data)}\n使用 WIN + R 快捷键填入 iris 链接快速启动";

        superNodeHostAndPort = null;
        community = null;
    }

    private void ShareButton_Click(object sender, EventArgs e)
    {

        if (ShareRichTextBox.Text != "")
        {
            ToolTip toolTip = new();
            toolTip.Show("分享链接已复制到剪贴板", ShareButton, ShareButton.Width, ShareButton.Height, 2000);

            var message = ShareRichTextBox.Text;
            var t = new Thread(() => Clipboard.SetText(message));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
    }

    private void JoinButton_Click(object sender, EventArgs e)
    {
        string pattern = @"iris:\/\/[^\s]+";
        Regex regex = new Regex(pattern);

        Match match = regex.Match(JonRichTextBox.Text);

        if (match.Success)
        {
            string irisLink = match.Value;
            string[] config = ReadUrl(irisLink.Replace("iris://", "")).Split('#');

            superNodeHostAndPort = config[0];
            community = config[1];

            DialogResult = DialogResult.OK;

            Close();
        }
        else
        {
            MessageBox.Show("未找到 iris 链接");
        }
    }
    public static (string?, string?) ShowShareForm(string? SuperNodeHostAndPort, string? Community, string N2NVirtualNetworkIP)
    {
        using var form = new ShareForm(SuperNodeHostAndPort, Community, N2NVirtualNetworkIP);
        var result = form.ShowDialog();

        return result == DialogResult.OK ? (form.superNodeHostAndPort!, form.community!) : (null, null);
    }

    public static string NewUrl(string originalString)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(originalString);
        string base64String = Convert.ToBase64String(bytes);

        return base64String;
    }
    public static string ReadUrl(string base64String)
    {
        byte[] bytes = Convert.FromBase64String(base64String);
        string originalString = Encoding.UTF8.GetString(bytes);

        return originalString;
    }
}
