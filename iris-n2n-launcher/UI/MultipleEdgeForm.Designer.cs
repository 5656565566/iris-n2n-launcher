namespace iris_n2n_launcher.UI
{
    partial class MultipleEdgeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultipleEdgeForm));
            tableLayoutPanel1 = new TableLayoutPanel();
            AddNodeButton = new Button();
            DelNodeButton = new Button();
            NodeDataGridView = new DataGridView();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NodeDataGridView).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 94F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6F));
            tableLayoutPanel1.Controls.Add(AddNodeButton, 1, 1);
            tableLayoutPanel1.Controls.Add(DelNodeButton, 1, 2);
            tableLayoutPanel1.Controls.Add(NodeDataGridView, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.Size = new Size(784, 441);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // AddNodeButton
            // 
            AddNodeButton.Dock = DockStyle.Fill;
            AddNodeButton.Image = Properties.Resources.添加;
            AddNodeButton.Location = new Point(740, 48);
            AddNodeButton.Margin = new Padding(4);
            AddNodeButton.Name = "AddNodeButton";
            AddNodeButton.Size = new Size(40, 36);
            AddNodeButton.TabIndex = 0;
            AddNodeButton.UseVisualStyleBackColor = true;
            AddNodeButton.Click += AddNodeButton_Click;
            // 
            // DelNodeButton
            // 
            DelNodeButton.Dock = DockStyle.Fill;
            DelNodeButton.Image = Properties.Resources.删除;
            DelNodeButton.Location = new Point(740, 92);
            DelNodeButton.Margin = new Padding(4);
            DelNodeButton.Name = "DelNodeButton";
            DelNodeButton.Size = new Size(40, 36);
            DelNodeButton.TabIndex = 1;
            DelNodeButton.UseVisualStyleBackColor = true;
            DelNodeButton.Click += DelNodeButton_Click;
            // 
            // NodeDataGridView
            // 
            NodeDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            NodeDataGridView.Dock = DockStyle.Fill;
            NodeDataGridView.Location = new Point(4, 4);
            NodeDataGridView.Margin = new Padding(4);
            NodeDataGridView.Name = "NodeDataGridView";
            tableLayoutPanel1.SetRowSpan(NodeDataGridView, 4);
            NodeDataGridView.Size = new Size(728, 433);
            NodeDataGridView.TabIndex = 2;
            // 
            // MultipleEdgeForm
            // 
            AutoScaleDimensions = new SizeF(10F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 441);
            Controls.Add(tableLayoutPanel1);
            Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "MultipleEdgeForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "多开节点";
            FormClosing += MultipleEdgeForm_FormClosing;
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NodeDataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Button AddNodeButton;
        private Button DelNodeButton;
        private DataGridView NodeDataGridView;
    }
}