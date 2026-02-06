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
            tlpNatMain = new TableLayoutPanel();
            StunHelpRichTextBox = new RichTextBox();
            groupBox1 = new GroupBox();
            tlpNatControls = new TableLayoutPanel();
            label2 = new Label();
            StunServerComboBox = new ComboBox();
            label3 = new Label();
            label1 = new Label();
            label4 = new Label();
            label5 = new Label();
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
            tlpSpeedMain = new TableLayoutPanel();
            SpeedTestRichTextBox = new RichTextBox();
            groupBox2 = new GroupBox();
            tlpSpeedControls = new TableLayoutPanel();
            label9 = new Label();
            SpeedTestAddrTextBox = new TextBox();
            label8 = new Label();
            label10 = new Label();
            label11 = new Label();
            EchoServerAddrTextBox = new TextBox();
            panelEchoBtns = new TableLayoutPanel();
            StartEchoButton = new Button();
            StopEchoButton = new Button();
            label7 = new Label();
            label6 = new Label();
            SpeedTestButton = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)RoomInfoGridView).BeginInit();
            tabPage2.SuspendLayout();
            tlpNatMain.SuspendLayout();
            groupBox1.SuspendLayout();
            tlpNatControls.SuspendLayout();
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
            tlpSpeedMain.SuspendLayout();
            groupBox2.SuspendLayout();
            tlpSpeedControls.SuspendLayout();
            panelEchoBtns.SuspendLayout();
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
            RoomInfoGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
            tabPage2.Controls.Add(tlpNatMain);
            tabPage2.Location = new Point(4, 28);
            tabPage2.Margin = new Padding(5, 6, 5, 6);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(5, 6, 5, 6);
            tabPage2.Size = new Size(726, 379);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "NAT 类型检测";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tlpNatMain
            // 
            tlpNatMain.ColumnCount = 2;
            tlpNatMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpNatMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpNatMain.Controls.Add(StunHelpRichTextBox, 1, 0);
            tlpNatMain.Controls.Add(groupBox1, 0, 0);
            tlpNatMain.Dock = DockStyle.Fill;
            tlpNatMain.Location = new Point(5, 6);
            tlpNatMain.Name = "tlpNatMain";
            tlpNatMain.RowCount = 1;
            tlpNatMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpNatMain.Size = new Size(716, 367);
            tlpNatMain.TabIndex = 9;
            // 
            // StunHelpRichTextBox
            // 
            StunHelpRichTextBox.Dock = DockStyle.Fill;
            StunHelpRichTextBox.Font = new Font("黑体", 15F, FontStyle.Regular, GraphicsUnit.Point, 134);
            StunHelpRichTextBox.Location = new Point(361, 3);
            StunHelpRichTextBox.Name = "StunHelpRichTextBox";
            StunHelpRichTextBox.ReadOnly = true;
            StunHelpRichTextBox.Size = new Size(352, 361);
            StunHelpRichTextBox.TabIndex = 8;
            StunHelpRichTextBox.Text = "";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(tlpNatControls);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(352, 361);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "NAT 检测";
            // 
            // tlpNatControls
            // 
            tlpNatControls.ColumnCount = 1;
            tlpNatControls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpNatControls.Controls.Add(label2, 0, 0);
            tlpNatControls.Controls.Add(StunServerComboBox, 0, 1);
            tlpNatControls.Controls.Add(label3, 0, 2);
            tlpNatControls.Controls.Add(label1, 0, 3);
            tlpNatControls.Controls.Add(label4, 0, 4);
            tlpNatControls.Controls.Add(label5, 0, 5);
            tlpNatControls.Controls.Add(StunTestButton, 0, 6);
            tlpNatControls.Dock = DockStyle.Fill;
            tlpNatControls.Location = new Point(3, 24);
            tlpNatControls.Name = "tlpNatControls";
            tlpNatControls.RowCount = 7;
            tlpNatControls.RowStyles.Add(new RowStyle());
            tlpNatControls.RowStyles.Add(new RowStyle());
            tlpNatControls.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpNatControls.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpNatControls.RowStyles.Add(new RowStyle());
            tlpNatControls.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpNatControls.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            tlpNatControls.Size = new Size(346, 334);
            tlpNatControls.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Bottom;
            label2.Location = new Point(3, 5);
            label2.Margin = new Padding(3, 5, 3, 5);
            label2.Name = "label2";
            label2.Size = new Size(340, 19);
            label2.TabIndex = 4;
            label2.Text = "STUN 服务器选择";
            // 
            // StunServerComboBox
            // 
            StunServerComboBox.Dock = DockStyle.Fill;
            StunServerComboBox.FormattingEnabled = true;
            StunServerComboBox.Items.AddRange(new object[] { "stun.miwifi.com", "stun.hot-chilli.net", "stun.fitauto.ru", "stun.voipstunt.com" });
            StunServerComboBox.Location = new Point(3, 32);
            StunServerComboBox.Name = "StunServerComboBox";
            StunServerComboBox.Size = new Size(340, 26);
            StunServerComboBox.TabIndex = 2;
            StunServerComboBox.Text = "stun.miwifi.com";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(3, 61);
            label3.Name = "label3";
            label3.Size = new Size(340, 40);
            label3.TabIndex = 5;
            label3.Text = "当前你的NAT 类型为 :";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(3, 101);
            label1.Name = "label1";
            label1.Size = new Size(340, 40);
            label1.TabIndex = 0;
            label1.Text = "检测看看?";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(3, 141);
            label4.Name = "label4";
            label4.Size = new Size(0, 19);
            label4.TabIndex = 6;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 160);
            label5.Name = "label5";
            label5.Size = new Size(0, 19);
            label5.TabIndex = 7;
            // 
            // StunTestButton
            // 
            StunTestButton.Dock = DockStyle.Fill;
            StunTestButton.Location = new Point(3, 267);
            StunTestButton.Name = "StunTestButton";
            StunTestButton.Size = new Size(340, 64);
            StunTestButton.TabIndex = 1;
            StunTestButton.Text = "一键检测";
            StunTestButton.UseVisualStyleBackColor = true;
            StunTestButton.Click += StunTestButton_Click;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(tableLayoutPanel1);
            tabPage3.Location = new Point(4, 28);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(726, 379);
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
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(726, 379);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // PingResultListBox
            // 
            tableLayoutPanel1.SetColumnSpan(PingResultListBox, 3);
            PingResultListBox.Dock = DockStyle.Fill;
            PingResultListBox.Font = new Font("黑体", 18F, FontStyle.Regular, GraphicsUnit.Point, 134);
            PingResultListBox.FormattingEnabled = true;
            PingResultListBox.ItemHeight = 24;
            PingResultListBox.Location = new Point(3, 48);
            PingResultListBox.Name = "PingResultListBox";
            PingResultListBox.Size = new Size(720, 328);
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
            PingTestButton.Size = new Size(177, 39);
            PingTestButton.TabIndex = 3;
            PingTestButton.Text = "Ping 测试";
            PingTestButton.UseVisualStyleBackColor = true;
            PingTestButton.Click += PingTestButton_Click;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(tableLayoutPanel2);
            tabPage4.Location = new Point(4, 28);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(726, 379);
            tabPage4.TabIndex = 5;
            tabPage4.Text = "端口映射";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
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
            tableLayoutPanel2.Size = new Size(726, 379);
            tableLayoutPanel2.TabIndex = 4;
            // 
            // TcpUdpForwDataGridView
            // 
            TcpUdpForwDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            TcpUdpForwDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TcpUdpForwDataGridView.Dock = DockStyle.Fill;
            TcpUdpForwDataGridView.Location = new Point(3, 3);
            TcpUdpForwDataGridView.Name = "TcpUdpForwDataGridView";
            TcpUdpForwDataGridView.RowHeadersWidth = 51;
            tableLayoutPanel2.SetRowSpan(TcpUdpForwDataGridView, 5);
            TcpUdpForwDataGridView.RowTemplate.Height = 27;
            TcpUdpForwDataGridView.Size = new Size(660, 373);
            TcpUdpForwDataGridView.TabIndex = 0;
            // 
            // TcpUdpForwDelButton
            // 
            TcpUdpForwDelButton.Anchor = AnchorStyles.Top;
            TcpUdpForwDelButton.FlatStyle = FlatStyle.Flat;
            TcpUdpForwDelButton.Image = Properties.Resources.删除;
            TcpUdpForwDelButton.Location = new Point(672, 70);
            TcpUdpForwDelButton.Margin = new Padding(3, 2, 3, 2);
            TcpUdpForwDelButton.Name = "TcpUdpForwDelButton";
            TcpUdpForwDelButton.Size = new Size(48, 48);
            TcpUdpForwDelButton.TabIndex = 2;
            TcpUdpForwDelButton.UseVisualStyleBackColor = true;
            TcpUdpForwDelButton.Click += TcpUdpForwDelButton_Click;
            // 
            // TcpUdpForwReButton
            // 
            TcpUdpForwReButton.Anchor = AnchorStyles.Top;
            TcpUdpForwReButton.FlatStyle = FlatStyle.Flat;
            TcpUdpForwReButton.Image = Properties.Resources.刷新;
            TcpUdpForwReButton.Location = new Point(672, 293);
            TcpUdpForwReButton.Margin = new Padding(3, 2, 3, 2);
            TcpUdpForwReButton.Name = "TcpUdpForwReButton";
            TcpUdpForwReButton.Size = new Size(48, 48);
            TcpUdpForwReButton.TabIndex = 3;
            TcpUdpForwReButton.UseVisualStyleBackColor = true;
            TcpUdpForwReButton.Click += TcpUdpForwReButton_Click;
            // 
            // TcpUdpForwAddButton
            // 
            TcpUdpForwAddButton.Anchor = AnchorStyles.Top;
            TcpUdpForwAddButton.FlatStyle = FlatStyle.Flat;
            TcpUdpForwAddButton.Image = Properties.Resources.添加;
            TcpUdpForwAddButton.Location = new Point(672, 2);
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
            tabPage5.Location = new Point(4, 28);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(726, 379);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "隔空传递";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
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
            tableLayoutPanel3.Size = new Size(726, 379);
            tableLayoutPanel3.TabIndex = 11;
            // 
            // FileTransferDataGridView
            // 
            FileTransferDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            FileTransferDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            FileTransferDataGridView.Dock = DockStyle.Fill;
            FileTransferDataGridView.Location = new Point(3, 3);
            FileTransferDataGridView.Name = "FileTransferDataGridView";
            tableLayoutPanel3.SetRowSpan(FileTransferDataGridView, 5);
            FileTransferDataGridView.Size = new Size(660, 373);
            FileTransferDataGridView.TabIndex = 7;
            // 
            // FileTransferAddButton
            // 
            FileTransferAddButton.Anchor = AnchorStyles.Top;
            FileTransferAddButton.FlatStyle = FlatStyle.Flat;
            FileTransferAddButton.Image = Properties.Resources.添加;
            FileTransferAddButton.Location = new Point(672, 2);
            FileTransferAddButton.Margin = new Padding(3, 2, 3, 2);
            FileTransferAddButton.Name = "FileTransferAddButton";
            FileTransferAddButton.Size = new Size(48, 48);
            FileTransferAddButton.TabIndex = 8;
            FileTransferAddButton.UseVisualStyleBackColor = true;
            FileTransferAddButton.Click += FileTransferAddButton_Click;
            // 
            // FileTransferDelButton
            // 
            FileTransferDelButton.Anchor = AnchorStyles.Top;
            FileTransferDelButton.FlatStyle = FlatStyle.Flat;
            FileTransferDelButton.Image = Properties.Resources.删除;
            FileTransferDelButton.Location = new Point(672, 70);
            FileTransferDelButton.Margin = new Padding(3, 2, 3, 2);
            FileTransferDelButton.Name = "FileTransferDelButton";
            FileTransferDelButton.Size = new Size(48, 48);
            FileTransferDelButton.TabIndex = 9;
            FileTransferDelButton.UseVisualStyleBackColor = true;
            FileTransferDelButton.Click += FileTransferDelButton_Click;
            // 
            // FileTransferReButton
            // 
            FileTransferReButton.Anchor = AnchorStyles.Top;
            FileTransferReButton.FlatStyle = FlatStyle.Flat;
            FileTransferReButton.Image = Properties.Resources.刷新;
            FileTransferReButton.Location = new Point(672, 293);
            FileTransferReButton.Margin = new Padding(3, 2, 3, 2);
            FileTransferReButton.Name = "FileTransferReButton";
            FileTransferReButton.Size = new Size(48, 48);
            FileTransferReButton.TabIndex = 10;
            FileTransferReButton.UseVisualStyleBackColor = true;
            FileTransferReButton.Click += FileTransferReButton_Click;
            // 
            // tabPage6
            // 
            tabPage6.Controls.Add(tlpSpeedMain);
            tabPage6.Location = new Point(4, 28);
            tabPage6.Name = "tabPage6";
            tabPage6.Padding = new Padding(3);
            tabPage6.Size = new Size(726, 379);
            tabPage6.TabIndex = 6;
            tabPage6.Text = "联机检测";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // tlpSpeedMain
            // 
            tlpSpeedMain.ColumnCount = 2;
            tlpSpeedMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpSpeedMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpSpeedMain.Controls.Add(SpeedTestRichTextBox, 1, 0);
            tlpSpeedMain.Controls.Add(groupBox2, 0, 0);
            tlpSpeedMain.Dock = DockStyle.Fill;
            tlpSpeedMain.Location = new Point(3, 3);
            tlpSpeedMain.Name = "tlpSpeedMain";
            tlpSpeedMain.RowCount = 1;
            tlpSpeedMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpSpeedMain.Size = new Size(720, 373);
            tlpSpeedMain.TabIndex = 10;
            // 
            // SpeedTestRichTextBox
            // 
            SpeedTestRichTextBox.Dock = DockStyle.Fill;
            SpeedTestRichTextBox.Font = new Font("黑体", 15F, FontStyle.Regular, GraphicsUnit.Point, 134);
            SpeedTestRichTextBox.Location = new Point(363, 3);
            SpeedTestRichTextBox.Name = "SpeedTestRichTextBox";
            SpeedTestRichTextBox.ReadOnly = true;
            SpeedTestRichTextBox.Size = new Size(354, 367);
            SpeedTestRichTextBox.TabIndex = 9;
            SpeedTestRichTextBox.Text = "联机质量测试  \n通过对方开启的 Echo 服务器进行检测  \n\n填写对方服务器地址 → 开始测试  \n\n说明：  \n- 此处填写 “对方” 的服务器地址  \n- 如无需对方测试你 无需自行启动服务\n";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(tlpSpeedControls);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(3, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(354, 367);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "联机质量检测";
            // 
            // tlpSpeedControls
            // 
            tlpSpeedControls.ColumnCount = 2;
            tlpSpeedControls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpSpeedControls.ColumnStyles.Add(new ColumnStyle());
            tlpSpeedControls.Controls.Add(label9, 0, 0);
            tlpSpeedControls.Controls.Add(SpeedTestAddrTextBox, 0, 1);
            tlpSpeedControls.Controls.Add(label8, 0, 2);
            tlpSpeedControls.Controls.Add(label10, 1, 2);
            tlpSpeedControls.Controls.Add(label11, 0, 3);
            tlpSpeedControls.Controls.Add(EchoServerAddrTextBox, 0, 4);
            tlpSpeedControls.Controls.Add(panelEchoBtns, 0, 5);
            tlpSpeedControls.Controls.Add(label7, 0, 6);
            tlpSpeedControls.Controls.Add(label6, 0, 7);
            tlpSpeedControls.Controls.Add(SpeedTestButton, 0, 8);
            tlpSpeedControls.Dock = DockStyle.Fill;
            tlpSpeedControls.Location = new Point(3, 24);
            tlpSpeedControls.Name = "tlpSpeedControls";
            tlpSpeedControls.RowCount = 9;
            tlpSpeedControls.RowStyles.Add(new RowStyle());
            tlpSpeedControls.RowStyles.Add(new RowStyle());
            tlpSpeedControls.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tlpSpeedControls.RowStyles.Add(new RowStyle());
            tlpSpeedControls.RowStyles.Add(new RowStyle());
            tlpSpeedControls.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            tlpSpeedControls.RowStyles.Add(new RowStyle());
            tlpSpeedControls.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpSpeedControls.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            tlpSpeedControls.Size = new Size(348, 340);
            tlpSpeedControls.TabIndex = 0;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Dock = DockStyle.Bottom;
            label9.Location = new Point(3, 5);
            label9.Margin = new Padding(3, 5, 3, 5);
            label9.Name = "label9";
            label9.Size = new Size(280, 19);
            label9.TabIndex = 4;
            label9.Text = "对方地址";
            // 
            // SpeedTestAddrTextBox
            // 
            tlpSpeedControls.SetColumnSpan(SpeedTestAddrTextBox, 2);
            SpeedTestAddrTextBox.Dock = DockStyle.Fill;
            SpeedTestAddrTextBox.Location = new Point(3, 32);
            SpeedTestAddrTextBox.Name = "SpeedTestAddrTextBox";
            SpeedTestAddrTextBox.Size = new Size(342, 28);
            SpeedTestAddrTextBox.TabIndex = 8;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Dock = DockStyle.Left;
            label8.Location = new Point(3, 63);
            label8.Name = "label8";
            label8.Size = new Size(169, 35);
            label8.TabIndex = 5;
            label8.Text = "Echo 服务器状态:";
            label8.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Dock = DockStyle.Right;
            label10.Location = new Point(289, 63);
            label10.Margin = new Padding(3, 0, 10, 0);
            label10.Name = "label10";
            label10.Size = new Size(49, 35);
            label10.TabIndex = 0;
            label10.Text = "未知";
            label10.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Dock = DockStyle.Bottom;
            label11.Location = new Point(3, 103);
            label11.Margin = new Padding(3, 5, 3, 5);
            label11.Name = "label11";
            label11.Size = new Size(280, 19);
            label11.TabIndex = 9;
            label11.Text = "我的地址:";
            // 
            // EchoServerAddrTextBox
            // 
            tlpSpeedControls.SetColumnSpan(EchoServerAddrTextBox, 2);
            EchoServerAddrTextBox.Dock = DockStyle.Fill;
            EchoServerAddrTextBox.Location = new Point(3, 130);
            EchoServerAddrTextBox.Name = "EchoServerAddrTextBox";
            EchoServerAddrTextBox.ReadOnly = true;
            EchoServerAddrTextBox.Size = new Size(342, 28);
            EchoServerAddrTextBox.TabIndex = 10;
            // 
            // panelEchoBtns
            // 
            panelEchoBtns.ColumnCount = 2;
            tlpSpeedControls.SetColumnSpan(panelEchoBtns, 2);
            panelEchoBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelEchoBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelEchoBtns.Controls.Add(StartEchoButton, 0, 0);
            panelEchoBtns.Controls.Add(StopEchoButton, 1, 0);
            panelEchoBtns.Dock = DockStyle.Fill;
            panelEchoBtns.Location = new Point(3, 164);
            panelEchoBtns.Name = "panelEchoBtns";
            panelEchoBtns.RowCount = 1;
            panelEchoBtns.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            panelEchoBtns.Size = new Size(342, 54);
            panelEchoBtns.TabIndex = 13;
            // 
            // StartEchoButton
            // 
            StartEchoButton.Dock = DockStyle.Fill;
            StartEchoButton.Location = new Point(3, 3);
            StartEchoButton.Name = "StartEchoButton";
            StartEchoButton.Size = new Size(165, 48);
            StartEchoButton.TabIndex = 12;
            StartEchoButton.Text = "启动服务";
            StartEchoButton.UseVisualStyleBackColor = true;
            StartEchoButton.Click += StartEchoButton_Click;
            // 
            // StopEchoButton
            // 
            StopEchoButton.Dock = DockStyle.Fill;
            StopEchoButton.Location = new Point(174, 3);
            StopEchoButton.Name = "StopEchoButton";
            StopEchoButton.Size = new Size(165, 48);
            StopEchoButton.TabIndex = 11;
            StopEchoButton.Text = "关闭服务";
            StopEchoButton.UseVisualStyleBackColor = true;
            StopEchoButton.Click += StopEchoButton_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(3, 221);
            label7.Name = "label7";
            label7.Size = new Size(0, 19);
            label7.TabIndex = 6;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(3, 240);
            label6.Name = "label6";
            label6.Size = new Size(0, 19);
            label6.TabIndex = 7;
            // 
            // SpeedTestButton
            // 
            tlpSpeedControls.SetColumnSpan(SpeedTestButton, 2);
            SpeedTestButton.Dock = DockStyle.Fill;
            SpeedTestButton.Location = new Point(3, 273);
            SpeedTestButton.Name = "SpeedTestButton";
            SpeedTestButton.Size = new Size(342, 64);
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
            tlpNatMain.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            tlpNatControls.ResumeLayout(false);
            tlpNatControls.PerformLayout();
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
            tlpSpeedMain.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            tlpSpeedControls.ResumeLayout(false);
            tlpSpeedControls.PerformLayout();
            panelEchoBtns.ResumeLayout(false);
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
        private TableLayoutPanel tlpNatMain;
        private TableLayoutPanel tlpNatControls;
        private TableLayoutPanel tlpSpeedMain;
        private TableLayoutPanel tlpSpeedControls;
        private TableLayoutPanel panelEchoBtns;
    }
}