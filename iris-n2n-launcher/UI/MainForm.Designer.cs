using System.Windows.Forms;

namespace iris_n2n_launcher.UI
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            try
            {
                base.Dispose(disposing);
            }
            catch { }
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            N2NContextMenu = new ContextMenuStrip(components);
            显示窗口ToolStripMenuItem = new ToolStripMenuItem();
            关闭N2NToolStripMenuItem = new ToolStripMenuItem();
            退出ToolStripMenuItem = new ToolStripMenuItem();
            SwitchButton = new Button();
            IPtextBox = new TextBox();
            RoomTextBox = new TextBox();
            SettingButton = new Button();
            label1 = new Label();
            label2 = new Label();
            StopButton = new Button();
            ToolButtom = new Button();
            UrlJoinbutton = new Button();
            panel6 = new Panel();
            MultipleEdgeButton = new Button();
            N2NNotifyIcon = new NotifyIcon(components);
            N2NContextMenu.SuspendLayout();
            panel6.SuspendLayout();
            SuspendLayout();
            // 
            // N2NContextMenu
            // 
            N2NContextMenu.ImageScalingSize = new Size(20, 20);
            N2NContextMenu.Items.AddRange(new ToolStripItem[] { 显示窗口ToolStripMenuItem, 关闭N2NToolStripMenuItem, 退出ToolStripMenuItem });
            N2NContextMenu.Name = "ContextMenu";
            N2NContextMenu.Size = new Size(181, 92);
            // 
            // 显示窗口ToolStripMenuItem
            // 
            显示窗口ToolStripMenuItem.Name = "显示窗口ToolStripMenuItem";
            显示窗口ToolStripMenuItem.Size = new Size(180, 22);
            显示窗口ToolStripMenuItem.Text = "显示窗口";
            显示窗口ToolStripMenuItem.Click += 显示窗口ToolStripMenuItem_Click;
            // 
            // 关闭N2NToolStripMenuItem
            // 
            关闭N2NToolStripMenuItem.Name = "关闭N2NToolStripMenuItem";
            关闭N2NToolStripMenuItem.Size = new Size(180, 22);
            关闭N2NToolStripMenuItem.Text = "关闭N2N";
            关闭N2NToolStripMenuItem.Click += 关闭N2NToolStripMenuItem_Click;
            // 
            // 退出ToolStripMenuItem
            // 
            退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            退出ToolStripMenuItem.Size = new Size(180, 22);
            退出ToolStripMenuItem.Text = "退出";
            退出ToolStripMenuItem.Click += 退出ToolStripMenuItem_Click;
            // 
            // SwitchButton
            // 
            SwitchButton.Font = new Font("微软雅黑", 12F, FontStyle.Bold, GraphicsUnit.Point, 134);
            SwitchButton.Location = new Point(5, 165);
            SwitchButton.Margin = new Padding(6, 5, 6, 5);
            SwitchButton.Name = "SwitchButton";
            SwitchButton.Size = new Size(236, 32);
            SwitchButton.TabIndex = 0;
            SwitchButton.Text = "一键启动";
            SwitchButton.UseVisualStyleBackColor = true;
            SwitchButton.Click += SwitchButton_Click;
            // 
            // IPtextBox
            // 
            IPtextBox.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            IPtextBox.Location = new Point(6, 35);
            IPtextBox.Margin = new Padding(6, 5, 6, 5);
            IPtextBox.Name = "IPtextBox";
            IPtextBox.ReadOnly = true;
            IPtextBox.Size = new Size(235, 29);
            IPtextBox.TabIndex = 1;
            // 
            // RoomTextBox
            // 
            RoomTextBox.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            RoomTextBox.Location = new Point(6, 95);
            RoomTextBox.Margin = new Padding(6, 5, 6, 5);
            RoomTextBox.Name = "RoomTextBox";
            RoomTextBox.Size = new Size(236, 29);
            RoomTextBox.TabIndex = 2;
            // 
            // SettingButton
            // 
            SettingButton.Font = new Font("Microsoft Sans Serif", 10.4999981F, FontStyle.Bold, GraphicsUnit.Point, 134);
            SettingButton.Location = new Point(181, 128);
            SettingButton.Margin = new Padding(6, 5, 6, 5);
            SettingButton.Name = "SettingButton";
            SettingButton.Size = new Size(60, 31);
            SettingButton.TabIndex = 3;
            SettingButton.Text = "设置";
            SettingButton.UseVisualStyleBackColor = true;
            SettingButton.Click += SettingButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(6, 9);
            label1.Margin = new Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new Size(110, 21);
            label1.TabIndex = 4;
            label1.Text = "虚拟局域网 IP";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label2.Location = new Point(5, 67);
            label2.Margin = new Padding(6, 0, 6, 0);
            label2.Name = "label2";
            label2.Size = new Size(106, 21);
            label2.TabIndex = 5;
            label2.Text = "自定义房间名";
            // 
            // StopButton
            // 
            StopButton.Enabled = false;
            StopButton.Font = new Font("Microsoft Sans Serif", 10.4999981F, FontStyle.Bold, GraphicsUnit.Point, 134);
            StopButton.Location = new Point(6, 128);
            StopButton.Margin = new Padding(6, 5, 6, 5);
            StopButton.Name = "StopButton";
            StopButton.Size = new Size(60, 31);
            StopButton.TabIndex = 7;
            StopButton.Text = "关闭";
            StopButton.UseVisualStyleBackColor = true;
            StopButton.Click += StopButton_Click;
            // 
            // ToolButtom
            // 
            ToolButtom.Font = new Font("Microsoft Sans Serif", 10.4999981F, FontStyle.Bold, GraphicsUnit.Point, 134);
            ToolButtom.Location = new Point(109, 128);
            ToolButtom.Margin = new Padding(6, 5, 6, 5);
            ToolButtom.Name = "ToolButtom";
            ToolButtom.Size = new Size(60, 31);
            ToolButtom.TabIndex = 9;
            ToolButtom.Text = "工具";
            ToolButtom.UseVisualStyleBackColor = true;
            ToolButtom.Click += ToolButtom_Click;
            // 
            // UrlJoinbutton
            // 
            UrlJoinbutton.Font = new Font("微软雅黑", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 134);
            UrlJoinbutton.Location = new Point(124, 66);
            UrlJoinbutton.Margin = new Padding(6, 5, 6, 5);
            UrlJoinbutton.Name = "UrlJoinbutton";
            UrlJoinbutton.Size = new Size(118, 27);
            UrlJoinbutton.TabIndex = 12;
            UrlJoinbutton.Text = "分享/快捷加入";
            UrlJoinbutton.UseVisualStyleBackColor = true;
            UrlJoinbutton.Click += UrlJoinbutton_Click;
            // 
            // panel6
            // 
            panel6.BackColor = Color.WhiteSmoke;
            panel6.Controls.Add(MultipleEdgeButton);
            panel6.Location = new Point(218, 9);
            panel6.Margin = new Padding(2, 3, 2, 3);
            panel6.Name = "panel6";
            panel6.Size = new Size(24, 24);
            panel6.TabIndex = 13;
            // 
            // MultipleEdgeButton
            // 
            MultipleEdgeButton.FlatStyle = FlatStyle.Flat;
            MultipleEdgeButton.Image = Properties.Resources.添加;
            MultipleEdgeButton.Location = new Point(-8, -6);
            MultipleEdgeButton.Margin = new Padding(2, 3, 2, 3);
            MultipleEdgeButton.Name = "MultipleEdgeButton";
            MultipleEdgeButton.Size = new Size(39, 35);
            MultipleEdgeButton.TabIndex = 0;
            MultipleEdgeButton.UseVisualStyleBackColor = true;
            MultipleEdgeButton.Click += MultipleEdgeButton_Click;
            // 
            // N2NNotifyIcon
            // 
            N2NNotifyIcon.ContextMenuStrip = N2NContextMenu;
            N2NNotifyIcon.Icon = (Icon)resources.GetObject("N2NNotifyIcon.Icon");
            N2NNotifyIcon.Text = "N2N启动器";
            N2NNotifyIcon.Visible = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(246, 211);
            Controls.Add(panel6);
            Controls.Add(UrlJoinbutton);
            Controls.Add(ToolButtom);
            Controls.Add(StopButton);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(SettingButton);
            Controls.Add(RoomTextBox);
            Controls.Add(IPtextBox);
            Controls.Add(SwitchButton);
            Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(6, 5, 6, 5);
            MaximizeBox = false;
            MaximumSize = new Size(262, 250);
            MinimumSize = new Size(262, 250);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "N2N";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            SizeChanged += MainForm_SizeChanged;
            N2NContextMenu.ResumeLayout(false);
            panel6.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.TextBox IPtextBox;
        private System.Windows.Forms.Button SettingButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip N2NContextMenu;
        private System.Windows.Forms.ToolStripMenuItem 显示窗口ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.TextBox RoomTextBox;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button ToolButtom;
        private System.Windows.Forms.Button UrlJoinbutton;
        public Button SwitchButton;
        private Panel panel6;
        private Button MultipleEdgeButton;
        private ToolStripMenuItem 关闭N2NToolStripMenuItem;
        private NotifyIcon N2NNotifyIcon;
    }
}

