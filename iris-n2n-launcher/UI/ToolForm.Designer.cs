namespace iris_n2n_launcher.UI
{
    partial class ToolForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolForm));
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            RoomInfoGridView = new DataGridView();
            tabPage2 = new TabPage();
            StunHelpRichTextBox = new RichTextBox();
            groupBox1 = new GroupBox();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            StunServerComboBox = new ComboBox();
            label1 = new Label();
            StunTestButton = new Button();
            tabPage3 = new TabPage();
            tableLayoutPanel1 = new TableLayoutPanel();
            PingResultListBox = new ListBox();
            PingIpBox = new TextBox();
            PingUpDown = new NumericUpDown();
            PingTestButton = new Button();
            tabPage4 = new TabPage();
            tableLayoutPanel2 = new TableLayoutPanel();
            TcpUdpForwDataGridView = new DataGridView();
            TcpUdpForwDelButton = new Button();
            TcpUdpForwReButton = new Button();
            TcpUdpForwAddButton = new Button();
            tabPage5 = new TabPage();
            tableLayoutPanel3 = new TableLayoutPanel();
            FileTransferDataGridView = new DataGridView();
            FileTransferAddButton = new Button();
            FileTransferDelButton = new Button();
            FileTransferReButton = new Button();
            tabPage6 = new TabPage();
            SpeedTestRichTextBox = new RichTextBox();
            groupBox2 = new GroupBox();
            StartEchoButton = new Button();
            StopEchoButton = new Button();
            EchoServerAddrTextBox = new TextBox();
            label11 = new Label();
            SpeedTestAddrTextBox = new TextBox();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            SpeedTestButton = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)RoomInfoGridView).BeginInit();
            tabPage2.SuspendLayout();
            groupBox1.SuspendLayout();
            tabPage3.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PingUpDown).BeginInit();
            tabPage4.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TcpUdpForwDataGridView).BeginInit();
            tabPage5.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)FileTransferDataGridView).BeginInit();
            tabPage6.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Controls.Add(tabPage6);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Margin = new Padding(5, 6, 5, 6);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(734, 411);
            tabControl1.TabIndex = 0;
            tabControl1.SelectedIndexChanged += SelectedIndexChanged;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(RoomInfoGridView);
            tabPage1.Location = new Point(4, 28);
            tabPage1.Margin = new Padding(5, 6, 5, 6);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(5, 6, 5, 6);
            tabPage1.Size = new Size(726, 379);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "虚拟局域网房间信息";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // RoomInfoGridView
            // 
            RoomInfoGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            RoomInfoGridView.Dock = DockStyle.Fill;
            RoomInfoGridView.Location = new Point(5, 6);
            RoomInfoGridView.Name = "RoomInfoGridView";
            RoomInfoGridView.RowHeadersWidth = 51;
            RoomInfoGridView.RowTemplate.Height = 23;
            RoomInfoGridView.ScrollBars = ScrollBars.Vertical;
            RoomInfoGridView.Size = new Size(716, 367);
            RoomInfoGridView.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(StunHelpRichTextBox);
            tabPage2.Controls.Add(groupBox1);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Margin = new Padding(5, 6, 5, 6);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(5, 6, 5, 6);
            tabPage2.Size = new Size(726, 381);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "NAT 类型检测";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // StunHelpRichTextBox
            // 
            StunHelpRichTextBox.Font = new Font("黑体", 15F, FontStyle.Regular, GraphicsUnit.Point, 134);
            StunHelpRichTextBox.Location = new Point(351, 9);
            StunHelpRichTextBox.Name = "StunHelpRichTextBox";
            StunHelpRichTextBox.ReadOnly = true;
            StunHelpRichTextBox.Size = new Size(367, 362);
            StunHelpRichTextBox.TabIndex = 8;
            StunHelpRichTextBox.Text = "";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(StunServerComboBox);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(StunTestButton);
            groupBox1.Location = new Point(8, 9);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(337, 362);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "NAT 检测";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 247);
            label5.Name = "label5";
            label5.Size = new Size(0, 19);
            label5.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 202);
            label4.Name = "label4";
            label4.Size = new Size(0, 19);
            label4.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 117);
            label3.Name = "label3";
            label3.Size = new Size(209, 19);
            label3.TabIndex = 5;
            label3.Text = "当前你的NAT 类型为 :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 35);
            label2.Name = "label2";
            label2.Size = new Size(159, 19);
            label2.TabIndex = 4;
            label2.Text = "STUN 服务器选择";
            // 
            // StunServerComboBox
            // 
            StunServerComboBox.FormattingEnabled = true;
            StunServerComboBox.Items.AddRange(new object[] { "stun.miwifi.com", "stun.hot-chilli.net", "stun.fitauto.ru", "stun.voipstunt.com" });
            StunServerComboBox.Location = new Point(6, 57);
            StunServerComboBox.Name = "StunServerComboBox";
            StunServerComboBox.Size = new Size(325, 26);
            StunServerComboBox.TabIndex = 2;
            StunServerComboBox.Text = "stun.miwifi.com";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 156);
            label1.Name = "label1";
            label1.Size = new Size(99, 19);
            label1.TabIndex = 0;
            label1.Text = "检测看看?";
            // 
            // StunTestButton
            // 
            StunTestButton.Location = new Point(6, 300);
            StunTestButton.Name = "StunTestButton";
            StunTestButton.Size = new Size(325, 56);
            StunTestButton.TabIndex = 1;
            StunTestButton.Text = "一键检测";
            StunTestButton.UseVisualStyleBackColor = true;
            StunTestButton.Click += StunTestButton_Click;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(tableLayoutPanel1);
            tabPage3.Location = new Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(726, 381);
            tabPage3.TabIndex = 3;
            tabPage3.Text = "Ping测试";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Controls.Add(PingResultListBox, 0, 1);
            tableLayoutPanel1.Controls.Add(PingIpBox, 0, 0);
            tableLayoutPanel1.Controls.Add(PingUpDown, 1, 0);
            tableLayoutPanel1.Controls.Add(PingTestButton, 2, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10.6895742F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 89.3104248F));
            tableLayoutPanel1.Size = new Size(726, 381);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // PingResultListBox
            // 
            tableLayoutPanel1.SetColumnSpan(PingResultListBox, 3);
            PingResultListBox.Dock = DockStyle.Fill;
            PingResultListBox.Font = new Font("黑体", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            PingResultListBox.FormattingEnabled = true;
            PingResultListBox.ItemHeight = 24;
            PingResultListBox.Location = new Point(3, 43);
            PingResultListBox.Name = "PingResultListBox";
            PingResultListBox.Size = new Size(720, 335);
            PingResultListBox.TabIndex = 1;
            // 
            // PingIpBox
            // 
            PingIpBox.Dock = DockStyle.Fill;
            PingIpBox.Font = new Font("黑体", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 134);
            PingIpBox.Location = new Point(3, 3);
            PingIpBox.Name = "PingIpBox";
            PingIpBox.Size = new Size(429, 32);
            PingIpBox.TabIndex = 2;
            // 
            // PingUpDown
            // 
            PingUpDown.Dock = DockStyle.Fill;
            PingUpDown.Font = new Font("黑体", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 134);
            PingUpDown.Location = new Point(438, 3);
            PingUpDown.Name = "PingUpDown";
            PingUpDown.Size = new Size(102, 32);
            PingUpDown.TabIndex = 4;
            PingUpDown.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // PingTestButton
            // 
            PingTestButton.Dock = DockStyle.Fill;
            PingTestButton.Location = new Point(546, 3);
            PingTestButton.Name = "PingTestButton";
            PingTestButton.Size = new Size(177, 34);
            PingTestButton.TabIndex = 3;
            PingTestButton.Text = "Ping 测试";
            PingTestButton.UseVisualStyleBackColor = true;
            PingTestButton.Click += PingTestButton_Click;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(tableLayoutPanel2);
            tabPage4.Location = new Point(4, 26);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(726, 381);
            tabPage4.TabIndex = 5;
            tabPage4.Text = "端口映射";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 92.5F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 7.5F));
            tableLayoutPanel2.Controls.Add(TcpUdpForwDataGridView, 0, 0);
            tableLayoutPanel2.Controls.Add(TcpUdpForwDelButton, 1, 1);
            tableLayoutPanel2.Controls.Add(TcpUdpForwReButton, 1, 3);
            tableLayoutPanel2.Controls.Add(TcpUdpForwAddButton, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 5;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 41F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
            tableLayoutPanel2.Size = new Size(726, 381);
            tableLayoutPanel2.TabIndex = 4;
            // 
            // TcpUdpForwDataGridView
            // 
            TcpUdpForwDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TcpUdpForwDataGridView.Dock = DockStyle.Fill;
            TcpUdpForwDataGridView.Location = new Point(3, 3);
            TcpUdpForwDataGridView.Name = "TcpUdpForwDataGridView";
            TcpUdpForwDataGridView.RowHeadersWidth = 51;
            tableLayoutPanel2.SetRowSpan(TcpUdpForwDataGridView, 5);
            TcpUdpForwDataGridView.RowTemplate.Height = 27;
            TcpUdpForwDataGridView.Size = new Size(665, 375);
            TcpUdpForwDataGridView.TabIndex = 0;
            // 
            // TcpUdpForwDelButton
            // 
            TcpUdpForwDelButton.FlatStyle = FlatStyle.Flat;
            TcpUdpForwDelButton.Image = Properties.Resources.删除;
            TcpUdpForwDelButton.Location = new Point(674, 70);
            TcpUdpForwDelButton.Margin = new Padding(3, 2, 3, 2);
            TcpUdpForwDelButton.Name = "TcpUdpForwDelButton";
            TcpUdpForwDelButton.Size = new Size(48, 48);
            TcpUdpForwDelButton.TabIndex = 2;
            TcpUdpForwDelButton.UseVisualStyleBackColor = true;
            TcpUdpForwDelButton.Click += TcpUdpForwDelButton_Click;
            // 
            // TcpUdpForwReButton
            // 
            TcpUdpForwReButton.FlatStyle = FlatStyle.Flat;
            TcpUdpForwReButton.Image = Properties.Resources.刷新;
            TcpUdpForwReButton.Location = new Point(674, 294);
            TcpUdpForwReButton.Margin = new Padding(3, 2, 3, 2);
            TcpUdpForwReButton.Name = "TcpUdpForwReButton";
            TcpUdpForwReButton.Size = new Size(48, 48);
            TcpUdpForwReButton.TabIndex = 3;
            TcpUdpForwReButton.UseVisualStyleBackColor = true;
            TcpUdpForwReButton.Click += TcpUdpForwReButton_Click;
            // 
            // TcpUdpForwAddButton
            // 
            TcpUdpForwAddButton.FlatStyle = FlatStyle.Flat;
            TcpUdpForwAddButton.Image = Properties.Resources.添加;
            TcpUdpForwAddButton.Location = new Point(674, 2);
            TcpUdpForwAddButton.Margin = new Padding(3, 2, 3, 2);
            TcpUdpForwAddButton.Name = "TcpUdpForwAddButton";
            TcpUdpForwAddButton.Size = new Size(48, 48);
            TcpUdpForwAddButton.TabIndex = 1;
            TcpUdpForwAddButton.UseVisualStyleBackColor = true;
            TcpUdpForwAddButton.Click += TcpUdpForwAddButton_Click;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(tableLayoutPanel3);
            tabPage5.Location = new Point(4, 26);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(726, 381);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "隔空传递";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 92.5F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 7.5F));
            tableLayoutPanel3.Controls.Add(FileTransferDataGridView, 0, 0);
            tableLayoutPanel3.Controls.Add(FileTransferAddButton, 1, 0);
            tableLayoutPanel3.Controls.Add(FileTransferDelButton, 1, 1);
            tableLayoutPanel3.Controls.Add(FileTransferReButton, 1, 3);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 5;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 41F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 18F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
            tableLayoutPanel3.Size = new Size(726, 381);
            tableLayoutPanel3.TabIndex = 11;
            // 
            // FileTransferDataGridView
            // 
            FileTransferDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            FileTransferDataGridView.Dock = DockStyle.Fill;
            FileTransferDataGridView.Location = new Point(3, 3);
            FileTransferDataGridView.Name = "FileTransferDataGridView";
            tableLayoutPanel3.SetRowSpan(FileTransferDataGridView, 5);
            FileTransferDataGridView.Size = new Size(665, 375);
            FileTransferDataGridView.TabIndex = 7;
            // 
            // FileTransferAddButton
            // 
            FileTransferAddButton.FlatStyle = FlatStyle.Flat;
            FileTransferAddButton.Image = Properties.Resources.添加;
            FileTransferAddButton.Location = new Point(674, 2);
            FileTransferAddButton.Margin = new Padding(3, 2, 3, 2);
            FileTransferAddButton.Name = "FileTransferAddButton";
            FileTransferAddButton.Size = new Size(48, 48);
            FileTransferAddButton.TabIndex = 8;
            FileTransferAddButton.UseVisualStyleBackColor = true;
            FileTransferAddButton.Click += FileTransferAddButton_Click;
            // 
            // FileTransferDelButton
            // 
            FileTransferDelButton.FlatStyle = FlatStyle.Flat;
            FileTransferDelButton.Image = Properties.Resources.删除;
            FileTransferDelButton.Location = new Point(674, 70);
            FileTransferDelButton.Margin = new Padding(3, 2, 3, 2);
            FileTransferDelButton.Name = "FileTransferDelButton";
            FileTransferDelButton.Size = new Size(48, 48);
            FileTransferDelButton.TabIndex = 9;
            FileTransferDelButton.UseVisualStyleBackColor = true;
            FileTransferDelButton.Click += FileTransferDelButton_Click;
            // 
            // FileTransferReButton
            // 
            FileTransferReButton.FlatStyle = FlatStyle.Flat;
            FileTransferReButton.Image = Properties.Resources.刷新;
            FileTransferReButton.Location = new Point(674, 294);
            FileTransferReButton.Margin = new Padding(3, 2, 3, 2);
            FileTransferReButton.Name = "FileTransferReButton";
            FileTransferReButton.Size = new Size(48, 48);
            FileTransferReButton.TabIndex = 10;
            FileTransferReButton.UseVisualStyleBackColor = true;
            FileTransferReButton.Click += FileTransferReButton_Click;
            // 
            // tabPage6
            // 
            tabPage6.Controls.Add(SpeedTestRichTextBox);
            tabPage6.Controls.Add(groupBox2);
            tabPage6.Location = new Point(4, 28);
            tabPage6.Name = "tabPage6";
            tabPage6.Padding = new Padding(3);
            tabPage6.Size = new Size(726, 379);
            tabPage6.TabIndex = 6;
            tabPage6.Text = "联机检测";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // SpeedTestRichTextBox
            // 
            SpeedTestRichTextBox.Dock = DockStyle.Right;
            SpeedTestRichTextBox.Font = new Font("黑体", 15F, FontStyle.Regular, GraphicsUnit.Point, 134);
            SpeedTestRichTextBox.Location = new Point(356, 3);
            SpeedTestRichTextBox.Name = "SpeedTestRichTextBox";
            SpeedTestRichTextBox.ReadOnly = true;
            SpeedTestRichTextBox.Size = new Size(367, 373);
            SpeedTestRichTextBox.TabIndex = 9;
            SpeedTestRichTextBox.Text = "联机质量测试  \n通过对方开启的 Echo 服务器进行检测  \n\n填写对方服务器地址 → 开始测试  \n\n说明：  \n- 此处填写 “对方” 的服务器地址  \n- 如无需对方测试你 无需自行启动服务\n";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(StartEchoButton);
            groupBox2.Controls.Add(StopEchoButton);
            groupBox2.Controls.Add(EchoServerAddrTextBox);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(SpeedTestAddrTextBox);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(label10);
            groupBox2.Controls.Add(SpeedTestButton);
            groupBox2.Dock = DockStyle.Left;
            groupBox2.Location = new Point(3, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(337, 373);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "联机质量检测";
            // 
            // StartEchoButton
            // 
            StartEchoButton.Location = new Point(6, 192);
            StartEchoButton.Name = "StartEchoButton";
            StartEchoButton.Size = new Size(121, 43);
            StartEchoButton.TabIndex = 12;
            StartEchoButton.Text = "启动服务";
            StartEchoButton.UseVisualStyleBackColor = true;
            StartEchoButton.Click += StartEchoButton_Click;
            // 
            // StopEchoButton
            // 
            StopEchoButton.Location = new Point(211, 192);
            StopEchoButton.Name = "StopEchoButton";
            StopEchoButton.Size = new Size(120, 43);
            StopEchoButton.TabIndex = 11;
            StopEchoButton.Text = "关闭服务";
            StopEchoButton.UseVisualStyleBackColor = true;
            StopEchoButton.Click += StopEchoButton_Click;
            // 
            // EchoServerAddrTextBox
            // 
            EchoServerAddrTextBox.Location = new Point(6, 158);
            EchoServerAddrTextBox.Name = "EchoServerAddrTextBox";
            EchoServerAddrTextBox.ReadOnly = true;
            EchoServerAddrTextBox.Size = new Size(325, 28);
            EchoServerAddrTextBox.TabIndex = 10;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(6, 136);
            label11.Name = "label11";
            label11.Size = new Size(99, 19);
            label11.TabIndex = 9;
            label11.Text = "我的地址:";
            // 
            // SpeedTestAddrTextBox
            // 
            SpeedTestAddrTextBox.Location = new Point(6, 57);
            SpeedTestAddrTextBox.Name = "SpeedTestAddrTextBox";
            SpeedTestAddrTextBox.Size = new Size(325, 28);
            SpeedTestAddrTextBox.TabIndex = 8;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(6, 247);
            label6.Name = "label6";
            label6.Size = new Size(0, 19);
            label6.TabIndex = 7;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 202);
            label7.Name = "label7";
            label7.Size = new Size(0, 19);
            label7.TabIndex = 6;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(6, 88);
            label8.Name = "label8";
            label8.Size = new Size(169, 19);
            label8.TabIndex = 5;
            label8.Text = "Echo 服务器状态:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(6, 35);
            label9.Name = "label9";
            label9.Size = new Size(89, 19);
            label9.TabIndex = 4;
            label9.Text = "对方地址";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(282, 88);
            label10.Name = "label10";
            label10.Size = new Size(49, 19);
            label10.TabIndex = 0;
            label10.Text = "未知";
            // 
            // SpeedTestButton
            // 
            SpeedTestButton.Location = new Point(6, 300);
            SpeedTestButton.Name = "SpeedTestButton";
            SpeedTestButton.Size = new Size(325, 56);
            SpeedTestButton.TabIndex = 1;
            SpeedTestButton.Text = "一键检测";
            SpeedTestButton.UseVisualStyleBackColor = true;
            SpeedTestButton.Click += SpeedTestButton_Click;
            // 
            // ToolForm
            // 
            AutoScaleDimensions = new SizeF(9F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(734, 411);
            Controls.Add(tabControl1);
            Font = new Font("黑体", 13.8F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5, 6, 5, 6);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ToolForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormClosed += ToolForm_FormClosed;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)RoomInfoGridView).EndInit();
            tabPage2.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPage3.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PingUpDown).EndInit();
            tabPage4.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)TcpUdpForwDataGridView).EndInit();
            tabPage5.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)FileTransferDataGridView).EndInit();
            tabPage6.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView RoomInfoGridView;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button StunTestButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox StunServerComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox PingResultListBox;
        private System.Windows.Forms.Button PingTestButton;
        private System.Windows.Forms.NumericUpDown PingUpDown;
        public System.Windows.Forms.TextBox PingIpBox;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView TcpUdpForwDataGridView;
        private System.Windows.Forms.Button TcpUdpForwAddButton;
        private System.Windows.Forms.Button TcpUdpForwReButton;
        private Button TcpUdpForwDelButton;
        private Button FileTransferReButton;
        private Button FileTransferDelButton;
        private Button FileTransferAddButton;
        private DataGridView FileTransferDataGridView;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private RichTextBox StunHelpRichTextBox;
        private TabPage tabPage6;
        private RichTextBox SpeedTestRichTextBox;
        private GroupBox groupBox2;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Button SpeedTestButton;
        private TextBox EchoServerAddrTextBox;
        private Label label11;
        private TextBox SpeedTestAddrTextBox;
        private Button StartEchoButton;
        private Button StopEchoButton;
    }
}