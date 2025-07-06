namespace iris_n2n_launcher.UI
{
    partial class InputForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox DataTextBox;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button AbortButton;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputForm));
            DataTextBox = new TextBox();
            OkButton = new Button();
            AbortButton = new Button();
            SuspendLayout();
            // 
            // DataTextBox
            // 
            DataTextBox.Location = new Point(8, 12);
            DataTextBox.Margin = new Padding(4);
            DataTextBox.Name = "DataTextBox";
            DataTextBox.Size = new Size(363, 26);
            DataTextBox.TabIndex = 0;
            // 
            // OkButton
            // 
            OkButton.Location = new Point(171, 66);
            OkButton.Margin = new Padding(4);
            OkButton.Name = "OkButton";
            OkButton.Size = new Size(96, 32);
            OkButton.TabIndex = 1;
            OkButton.Text = "确定";
            OkButton.UseVisualStyleBackColor = true;
            OkButton.Click += OkButton_Click;
            // 
            // AbortButton
            // 
            AbortButton.Location = new Point(275, 66);
            AbortButton.Margin = new Padding(4);
            AbortButton.Name = "AbortButton";
            AbortButton.Size = new Size(96, 32);
            AbortButton.TabIndex = 2;
            AbortButton.Text = "取消";
            AbortButton.UseVisualStyleBackColor = true;
            AbortButton.Click += AbortButton_Click;
            // 
            // InputForm
            // 
            AcceptButton = OkButton;
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 109);
            Controls.Add(AbortButton);
            Controls.Add(OkButton);
            Controls.Add(DataTextBox);
            Font = new Font("微软雅黑", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 134);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "输入";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
