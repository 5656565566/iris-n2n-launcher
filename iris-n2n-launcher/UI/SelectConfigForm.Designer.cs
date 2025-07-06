namespace iris_n2n_launcher.UI
{
    partial class SelectConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectConfigForm));
            ConfigComboBox = new ComboBox();
            AbortButton = new Button();
            OkButton = new Button();
            RoomTextBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // ConfigComboBox
            // 
            ConfigComboBox.FormattingEnabled = true;
            ConfigComboBox.Location = new Point(12, 34);
            ConfigComboBox.Margin = new Padding(4);
            ConfigComboBox.Name = "ConfigComboBox";
            ConfigComboBox.Size = new Size(311, 29);
            ConfigComboBox.TabIndex = 0;
            ConfigComboBox.SelectedIndexChanged += ConfigComboBox_SelectedIndexChanged;
            // 
            // AbortButton
            // 
            AbortButton.Location = new Point(215, 128);
            AbortButton.Margin = new Padding(4);
            AbortButton.Name = "AbortButton";
            AbortButton.Size = new Size(107, 28);
            AbortButton.TabIndex = 1;
            AbortButton.Text = "取消";
            AbortButton.UseVisualStyleBackColor = true;
            AbortButton.Click += AbortButton_Click;
            // 
            // OkButton
            // 
            OkButton.Location = new Point(12, 128);
            OkButton.Margin = new Padding(4);
            OkButton.Name = "OkButton";
            OkButton.Size = new Size(107, 28);
            OkButton.TabIndex = 2;
            OkButton.Text = "确定";
            OkButton.UseVisualStyleBackColor = true;
            OkButton.Click += OkButton_Click;
            // 
            // RoomTextBox
            // 
            RoomTextBox.Location = new Point(12, 92);
            RoomTextBox.Margin = new Padding(4);
            RoomTextBox.Name = "RoomTextBox";
            RoomTextBox.Size = new Size(310, 28);
            RoomTextBox.TabIndex = 3;
            RoomTextBox.TextChanged += RoomTextBox_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(138, 21);
            label1.TabIndex = 4;
            label1.Text = "选择一个配置文件";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(13, 67);
            label2.Name = "label2";
            label2.Size = new Size(58, 21);
            label2.TabIndex = 5;
            label2.Text = "房间名";
            // 
            // SelectConfigForm
            // 
            AutoScaleDimensions = new SizeF(10F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(336, 174);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(RoomTextBox);
            Controls.Add(OkButton);
            Controls.Add(AbortButton);
            Controls.Add(ConfigComboBox);
            Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SelectConfigForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "配置选择";
            Load += SelectConfigForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox ConfigComboBox;
        private Button AbortButton;
        private Button OkButton;
        private TextBox RoomTextBox;
        private Label label1;
        private Label label2;

    }
}