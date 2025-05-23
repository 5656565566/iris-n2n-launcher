namespace iris_n2n_launcher.UI
{
    public partial class InputForm : Form
    {
        public string? Result { get; private set; }

        public InputForm()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Result = DataTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void NoButton_Click(object sender, EventArgs e)
        {
            Result = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// 显示输入框对话框并返回用户输入的字符串或 null（取消）
        /// </summary>
        public static string? ShowInput(string title = "请输入内容", string placeholder = "")
        {
            using var form = new InputForm();
            form.Text = title;
            form.DataTextBox.Text = placeholder;

            var result = form.ShowDialog();
            return result == DialogResult.OK ? form.Result : null;
        }
    }
}
