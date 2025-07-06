using iris_n2n_launcher.Config;
using iris_n2n_launcher.N2N;

namespace iris_n2n_launcher.UI
{
    public partial class SelectConfigForm : Form
    {
        public N2NConfiguration n2NConfiguration;
        public string roomName;
        private static readonly ConfigManager configManager = ConfigManager.Instance;
        public SelectConfigForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 显示对话框并返回用户输入的字符串或 null（取消）
        /// </summary>
        public static N2NConfiguration? ShowInput()
        {
            using var form = new SelectConfigForm();

            var result = form.ShowDialog();

            return result == DialogResult.OK ? form.n2NConfiguration : null;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (n2NConfiguration != null)
            {
                n2NConfiguration.Community = roomName;
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }
            Close();
        }

        private void AbortButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SelectConfigForm_Load(object sender, EventArgs e)
        {
            var config = configManager.LoadConfig<Configuration>("config");

            ConfigComboBox.Text = config.ConfigName;
            n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(ConfigComboBox.Text);

            ConfigComboBox.Items.Clear();

            var configList = configManager.ListConfigs();

            foreach (var configName in configList)
            {
                if (configName != null && configName != "config")
                {
                    ConfigComboBox.Items.Add(configName);
                }
            }
        }

        private void ConfigComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            n2NConfiguration = configManager.LoadConfig<N2NConfiguration>(ConfigComboBox.Text);
        }

        private void RoomTextBox_TextChanged(object sender, EventArgs e)
        {
            roomName = RoomTextBox.Text;
        }
    }
}
