namespace iris_n2n_launcher.UI
{
    partial class ShareForm
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
            JonRichTextBox = new RichTextBox();
            groupBox1 = new GroupBox();
            JoinButton = new Button();
            groupBox2 = new GroupBox();
            ShareButton = new Button();
            ShareRichTextBox = new RichTextBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // JonRichTextBox
            // 
            JonRichTextBox.BorderStyle = BorderStyle.None;
            JonRichTextBox.Dock = DockStyle.Fill;
            JonRichTextBox.Location = new Point(4, 25);
            JonRichTextBox.Margin = new Padding(4);
            JonRichTextBox.Name = "JonRichTextBox";
            JonRichTextBox.Size = new Size(513, 102);
            JonRichTextBox.TabIndex = 0;
            JonRichTextBox.Text = "";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(JoinButton);
            groupBox1.Controls.Add(JonRichTextBox);
            groupBox1.Location = new Point(13, 12);
            groupBox1.Margin = new Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4);
            groupBox1.Size = new Size(521, 131);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "请粘贴分享口令";
            // 
            // JoinButton
            // 
            JoinButton.Location = new Point(401, 98);
            JoinButton.Margin = new Padding(3, 2, 3, 2);
            JoinButton.Name = "JoinButton";
            JoinButton.Size = new Size(116, 29);
            JoinButton.TabIndex = 2;
            JoinButton.Text = "加入房间";
            JoinButton.UseVisualStyleBackColor = true;
            JoinButton.Click += JoinButton_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(ShareButton);
            groupBox2.Controls.Add(ShareRichTextBox);
            groupBox2.Location = new Point(8, 151);
            groupBox2.Margin = new Padding(4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4);
            groupBox2.Size = new Size(521, 131);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "请开启N2N才能生成分享口令";
            // 
            // ShareButton
            // 
            ShareButton.Location = new Point(406, 98);
            ShareButton.Margin = new Padding(3, 2, 3, 2);
            ShareButton.Name = "ShareButton";
            ShareButton.Size = new Size(111, 29);
            ShareButton.TabIndex = 2;
            ShareButton.Text = "复制分享口令";
            ShareButton.UseVisualStyleBackColor = true;
            ShareButton.Click += ShareButton_Click;
            // 
            // ShareRichTextBox
            // 
            ShareRichTextBox.BorderStyle = BorderStyle.None;
            ShareRichTextBox.Dock = DockStyle.Fill;
            ShareRichTextBox.Enabled = false;
            ShareRichTextBox.Location = new Point(4, 25);
            ShareRichTextBox.Margin = new Padding(4);
            ShareRichTextBox.Name = "ShareRichTextBox";
            ShareRichTextBox.Size = new Size(513, 102);
            ShareRichTextBox.TabIndex = 0;
            ShareRichTextBox.Text = "";
            // 
            // ShareForm
            // 
            AutoScaleDimensions = new SizeF(9F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(546, 303);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Font = new Font("黑体", 13.8F);
            Margin = new Padding(4);
            MaximizeBox = false;
            MaximumSize = new Size(562, 342);
            MinimizeBox = false;
            MinimumSize = new Size(562, 342);
            Name = "ShareForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Load += ShareForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox JonRichTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button JoinButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button ShareButton;
        private System.Windows.Forms.RichTextBox ShareRichTextBox;
    }
}