using System.Runtime.InteropServices;
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

    private async void ShareButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ShareRichTextBox.Text))
            return;

        // 显示提示
        ToolTip toolTip = new();
        toolTip.Show("分享链接已复制到剪贴板", ShareButton, ShareButton.Width, ShareButton.Height, 2000);

        var message = ShareRichTextBox.Text;

        bool success = await CopyToClipboardAsync(message);

        if (!success)
        {
            MessageBox.Show(this,
                "剪贴板正被其他程序使用 请关闭远程桌面、文档或剪贴板管理工具后重试\n" +
                "建议手动复制链接\n",
                "复制失败",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private Task<bool> CopyToClipboardAsync(string text)
    {
        return Task.Run(() =>
        {
            for (int retry = 0; retry < 3; retry++)
            {
                try
                {
                    bool result = false;
                    var thread = new Thread(() =>
                    {
                        try
                        {
                            try { Clipboard.Clear(); } catch { }
                            Thread.Sleep(50);

                            Clipboard.SetText(text);
                            result = true;
                        }
                        catch (ExternalException)
                        {
                            result = false;
                        }
                    });

                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join(1000); // 等待最多1秒

                    if (result)
                        return true;
                }
                catch
                {
                    // 忽略异常，继续重试
                }

                if (retry < 4)
                    Thread.Sleep(200);
            }

            return false;
        });
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
