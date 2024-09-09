namespace DungeonMaster
{
    partial class TimerControls
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel5 = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            m_adder = new NumericUpDown();
            m_label = new Label();
            add_time_button = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            s_adder = new NumericUpDown();
            s_label = new Label();
            time_label = new TextBox();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)m_adder).BeginInit();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)s_adder).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.AutoSize = true;
            tableLayoutPanel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel5.ColumnCount = 1;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Controls.Add(tableLayoutPanel4, 0, 1);
            tableLayoutPanel5.Controls.Add(time_label, 0, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(0, 0);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 2;
            tableLayoutPanel5.RowStyles.Add(new RowStyle());
            tableLayoutPanel5.RowStyles.Add(new RowStyle());
            tableLayoutPanel5.Size = new Size(441, 98);
            tableLayoutPanel5.TabIndex = 10;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.AutoSize = true;
            tableLayoutPanel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel4.ColumnCount = 3;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel4.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel4.Controls.Add(add_time_button, 2, 0);
            tableLayoutPanel4.Controls.Add(tableLayoutPanel3, 1, 0);
            tableLayoutPanel4.Location = new Point(3, 32);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new Size(388, 35);
            tableLayoutPanel4.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.Controls.Add(m_adder, 1, 0);
            tableLayoutPanel2.Controls.Add(m_label, 0, 0);
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(150, 29);
            tableLayoutPanel2.TabIndex = 5;
            // 
            // m_adder
            // 
            m_adder.Location = new Point(27, 3);
            m_adder.Minimum = new decimal(new int[] { 100, 0, 0, int.MinValue });
            m_adder.Name = "m_adder";
            m_adder.Size = new Size(120, 23);
            m_adder.TabIndex = 4;
            // 
            // m_label
            // 
            m_label.AutoSize = true;
            m_label.Dock = DockStyle.Fill;
            m_label.Location = new Point(3, 0);
            m_label.Name = "m_label";
            m_label.Size = new Size(18, 29);
            m_label.TabIndex = 3;
            m_label.Text = "M";
            m_label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // add_time_button
            // 
            add_time_button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            add_time_button.Location = new Point(310, 3);
            add_time_button.Name = "add_time_button";
            add_time_button.Size = new Size(75, 29);
            add_time_button.TabIndex = 7;
            add_time_button.Text = "Add";
            add_time_button.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.AutoSize = true;
            tableLayoutPanel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.Controls.Add(s_adder, 1, 0);
            tableLayoutPanel3.Controls.Add(s_label, 0, 0);
            tableLayoutPanel3.Location = new Point(159, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(145, 29);
            tableLayoutPanel3.TabIndex = 6;
            // 
            // s_adder
            // 
            s_adder.Location = new Point(22, 3);
            s_adder.Minimum = new decimal(new int[] { 100, 0, 0, int.MinValue });
            s_adder.Name = "s_adder";
            s_adder.Size = new Size(120, 23);
            s_adder.TabIndex = 4;
            // 
            // s_label
            // 
            s_label.AutoSize = true;
            s_label.Dock = DockStyle.Fill;
            s_label.Location = new Point(3, 0);
            s_label.Name = "s_label";
            s_label.Size = new Size(13, 29);
            s_label.TabIndex = 3;
            s_label.Text = "S";
            s_label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // time_label
            // 
            time_label.Dock = DockStyle.Fill;
            time_label.Location = new Point(3, 3);
            time_label.Name = "time_label";
            time_label.ReadOnly = true;
            time_label.Size = new Size(435, 23);
            time_label.TabIndex = 1;
            time_label.Text = "00:00:00";
            time_label.TextAlign = HorizontalAlignment.Center;
            // 
            // TimerControls
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel5);
            Name = "TimerControls";
            Size = new Size(441, 98);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)m_adder).EndInit();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)s_adder).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel5;
        private TableLayoutPanel tableLayoutPanel4;
        private TableLayoutPanel tableLayoutPanel2;
        private NumericUpDown m_adder;
        private Label m_label;
        private Button add_time_button;
        private TableLayoutPanel tableLayoutPanel3;
        private NumericUpDown s_adder;
        private Label s_label;
        private TextBox time_label;
    }
}
