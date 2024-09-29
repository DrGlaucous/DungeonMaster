namespace DungeonMaster
{
    partial class SerialManager
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
            tableLayoutPanel1 = new TableLayoutPanel();
            port_list = new ComboBox();
            port_scan_btn = new Button();
            connect_btn = new Button();
            gen_label_1 = new Label();
            baud_list = new ComboBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(port_list, 1, 0);
            tableLayoutPanel1.Controls.Add(port_scan_btn, 0, 0);
            tableLayoutPanel1.Controls.Add(connect_btn, 4, 0);
            tableLayoutPanel1.Controls.Add(gen_label_1, 2, 0);
            tableLayoutPanel1.Controls.Add(baud_list, 3, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(453, 42);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // port_list
            // 
            port_list.Dock = DockStyle.Fill;
            port_list.FormattingEnabled = true;
            port_list.ImeMode = ImeMode.NoControl;
            port_list.Location = new Point(48, 3);
            port_list.Name = "port_list";
            port_list.Size = new Size(131, 23);
            port_list.TabIndex = 1;
            // 
            // port_scan_btn
            // 
            port_scan_btn.AutoSize = true;
            port_scan_btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            port_scan_btn.Dock = DockStyle.Fill;
            port_scan_btn.Location = new Point(3, 3);
            port_scan_btn.Name = "port_scan_btn";
            port_scan_btn.Size = new Size(39, 36);
            port_scan_btn.TabIndex = 1;
            port_scan_btn.Text = "Port";
            port_scan_btn.UseVisualStyleBackColor = true;
            port_scan_btn.Click += port_scan_btn_Click;
            // 
            // connect_btn
            // 
            connect_btn.AutoSize = true;
            connect_btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            connect_btn.Dock = DockStyle.Fill;
            connect_btn.Location = new Point(336, 3);
            connect_btn.Name = "connect_btn";
            connect_btn.Size = new Size(114, 36);
            connect_btn.TabIndex = 3;
            connect_btn.Text = "Connect";
            connect_btn.UseVisualStyleBackColor = true;
            connect_btn.Click += connect_btn_Click;
            // 
            // gen_label_1
            // 
            gen_label_1.AutoSize = true;
            gen_label_1.Dock = DockStyle.Fill;
            gen_label_1.Location = new Point(185, 0);
            gen_label_1.Name = "gen_label_1";
            gen_label_1.Size = new Size(18, 42);
            gen_label_1.TabIndex = 4;
            gen_label_1.Text = "@";
            gen_label_1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // baud_list
            // 
            baud_list.Dock = DockStyle.Fill;
            baud_list.FormattingEnabled = true;
            baud_list.Items.AddRange(new object[] { "2400", "9600", "19200", "38400", "57600", "115200", "250000" });
            baud_list.Location = new Point(209, 3);
            baud_list.Name = "baud_list";
            baud_list.Size = new Size(121, 23);
            baud_list.TabIndex = 2;
            // 
            // SerialManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "SerialManager";
            Size = new Size(453, 42);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private ComboBox port_list;
        private Button port_scan_btn;
        private Button connect_btn;
        private Label gen_label_1;
        private ComboBox baud_list;
    }
}
