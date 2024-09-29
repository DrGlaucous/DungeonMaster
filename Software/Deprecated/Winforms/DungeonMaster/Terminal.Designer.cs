namespace DungeonMaster
{
    partial class Terminal
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
            tableLayoutPanel11 = new TableLayoutPanel();
            tableLayoutPanel12 = new TableLayoutPanel();
            serial_outbox = new TextBox();
            serial_send_button = new Button();
            serial_clear_button = new Button();
            serial_inbox = new TextBox();
            tableLayoutPanel11.SuspendLayout();
            tableLayoutPanel12.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel11
            // 
            tableLayoutPanel11.AutoSize = true;
            tableLayoutPanel11.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel11.ColumnCount = 1;
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel11.Controls.Add(tableLayoutPanel12, 0, 1);
            tableLayoutPanel11.Controls.Add(serial_inbox, 0, 0);
            tableLayoutPanel11.Dock = DockStyle.Fill;
            tableLayoutPanel11.Location = new Point(0, 0);
            tableLayoutPanel11.Name = "tableLayoutPanel11";
            tableLayoutPanel11.RowCount = 2;
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel11.RowStyles.Add(new RowStyle());
            tableLayoutPanel11.Size = new Size(492, 289);
            tableLayoutPanel11.TabIndex = 23;
            // 
            // tableLayoutPanel12
            // 
            tableLayoutPanel12.AutoSize = true;
            tableLayoutPanel12.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel12.ColumnCount = 3;
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel12.Controls.Add(serial_outbox, 1, 0);
            tableLayoutPanel12.Controls.Add(serial_send_button, 0, 0);
            tableLayoutPanel12.Controls.Add(serial_clear_button, 2, 0);
            tableLayoutPanel12.Dock = DockStyle.Fill;
            tableLayoutPanel12.Location = new Point(3, 255);
            tableLayoutPanel12.Name = "tableLayoutPanel12";
            tableLayoutPanel12.RowCount = 1;
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel12.Size = new Size(486, 31);
            tableLayoutPanel12.TabIndex = 24;
            // 
            // serial_outbox
            // 
            serial_outbox.Dock = DockStyle.Fill;
            serial_outbox.Location = new Point(52, 3);
            serial_outbox.Name = "serial_outbox";
            serial_outbox.Size = new Size(381, 23);
            serial_outbox.TabIndex = 1;
            // 
            // serial_send_button
            // 
            serial_send_button.AutoSize = true;
            serial_send_button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            serial_send_button.Location = new Point(3, 3);
            serial_send_button.Name = "serial_send_button";
            serial_send_button.Size = new Size(43, 25);
            serial_send_button.TabIndex = 23;
            serial_send_button.Text = "Send";
            serial_send_button.UseVisualStyleBackColor = true;
            serial_send_button.Click += serial_send_button_Click;
            // 
            // serial_clear_button
            // 
            serial_clear_button.AutoSize = true;
            serial_clear_button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            serial_clear_button.Location = new Point(439, 3);
            serial_clear_button.Name = "serial_clear_button";
            serial_clear_button.Size = new Size(44, 25);
            serial_clear_button.TabIndex = 24;
            serial_clear_button.Text = "Clear";
            serial_clear_button.UseVisualStyleBackColor = true;
            serial_clear_button.Click += serial_clear_button_Click;
            // 
            // serial_inbox
            // 
            serial_inbox.BackColor = SystemColors.ControlDarkDark;
            serial_inbox.Dock = DockStyle.Fill;
            serial_inbox.ForeColor = Color.Lime;
            serial_inbox.Location = new Point(3, 3);
            serial_inbox.Multiline = true;
            serial_inbox.Name = "serial_inbox";
            serial_inbox.ReadOnly = true;
            serial_inbox.ScrollBars = ScrollBars.Vertical;
            serial_inbox.Size = new Size(486, 246);
            serial_inbox.TabIndex = 0;
            // 
            // Terminal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel11);
            Name = "Terminal";
            Size = new Size(492, 289);
            tableLayoutPanel11.ResumeLayout(false);
            tableLayoutPanel11.PerformLayout();
            tableLayoutPanel12.ResumeLayout(false);
            tableLayoutPanel12.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel11;
        private TableLayoutPanel tableLayoutPanel12;
        private TextBox serial_outbox;
        private Button serial_send_button;
        private Button serial_clear_button;
        private TextBox serial_inbox;
    }
}
