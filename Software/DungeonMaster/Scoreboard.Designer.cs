namespace DungeonMaster
{
    partial class Scoreboard
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
            TimerMajor = new Label();
            Team1 = new Label();
            Bot1 = new Label();
            Bot2 = new Label();
            Team2 = new Label();
            Stopwatch = new Label();
            Bot1Pic = new PictureBox();
            Bot2Pic = new PictureBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            tableLayoutPanel5 = new TableLayoutPanel();
            tableLayoutPanel6 = new TableLayoutPanel();
            tableLayoutPanel7 = new TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)Bot1Pic).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Bot2Pic).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            SuspendLayout();
            // 
            // TimerMajor
            // 
            TimerMajor.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TimerMajor.AutoSize = true;
            TimerMajor.Font = new Font("Segoe UI", 48F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TimerMajor.ForeColor = SystemColors.ControlLight;
            TimerMajor.Location = new Point(3, 0);
            TimerMajor.Name = "TimerMajor";
            TimerMajor.Size = new Size(1212, 86);
            TimerMajor.TabIndex = 0;
            TimerMajor.Text = "00:00:00";
            TimerMajor.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Team1
            // 
            Team1.AutoSize = true;
            Team1.Font = new Font("Segoe UI", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Team1.ForeColor = SystemColors.ControlLight;
            Team1.Location = new Point(3, 0);
            Team1.Name = "Team1";
            Team1.Size = new Size(130, 47);
            Team1.TabIndex = 1;
            Team1.Text = "Team 1";
            // 
            // Bot1
            // 
            Bot1.AutoSize = true;
            Bot1.Font = new Font("Segoe UI", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Bot1.ForeColor = SystemColors.ControlLight;
            Bot1.Location = new Point(3, 95);
            Bot1.Name = "Bot1";
            Bot1.Size = new Size(144, 47);
            Bot1.TabIndex = 2;
            Bot1.Text = "Robot 1";
            // 
            // Bot2
            // 
            Bot2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Bot2.AutoSize = true;
            Bot2.Font = new Font("Segoe UI", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Bot2.ForeColor = SystemColors.ControlLight;
            Bot2.Location = new Point(94, 95);
            Bot2.Name = "Bot2";
            Bot2.Size = new Size(144, 47);
            Bot2.TabIndex = 4;
            Bot2.Text = "Robot 2";
            // 
            // Team2
            // 
            Team2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Team2.AutoSize = true;
            Team2.Font = new Font("Segoe UI", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Team2.ForeColor = SystemColors.ControlLight;
            Team2.Location = new Point(108, 0);
            Team2.Name = "Team2";
            Team2.Size = new Size(130, 47);
            Team2.TabIndex = 3;
            Team2.Text = "Team 2";
            // 
            // Stopwatch
            // 
            Stopwatch.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            Stopwatch.AutoSize = true;
            Stopwatch.BackColor = Color.Gray;
            Stopwatch.Font = new Font("Segoe UI", 27.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Stopwatch.ForeColor = SystemColors.ControlLight;
            Stopwatch.Location = new Point(3, 86);
            Stopwatch.Name = "Stopwatch";
            Stopwatch.Size = new Size(1212, 50);
            Stopwatch.TabIndex = 5;
            Stopwatch.Text = "00:00:00";
            Stopwatch.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Bot1Pic
            // 
            Bot1Pic.Dock = DockStyle.Fill;
            Bot1Pic.Location = new Point(243, 3);
            Bot1Pic.Name = "Bot1Pic";
            Bot1Pic.Size = new Size(235, 185);
            Bot1Pic.SizeMode = PictureBoxSizeMode.Zoom;
            Bot1Pic.TabIndex = 6;
            Bot1Pic.TabStop = false;
            // 
            // Bot2Pic
            // 
            Bot2Pic.Dock = DockStyle.Fill;
            Bot2Pic.Location = new Point(3, 3);
            Bot2Pic.Name = "Bot2Pic";
            Bot2Pic.Size = new Size(235, 185);
            Bot2Pic.SizeMode = PictureBoxSizeMode.AutoSize;
            Bot2Pic.TabIndex = 7;
            Bot2Pic.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 0);
            tableLayoutPanel1.Controls.Add(Bot2Pic, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(733, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(482, 191);
            tableLayoutPanel1.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(Team2, 0, 0);
            tableLayoutPanel2.Controls.Add(Bot2, 0, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(241, 0);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(241, 191);
            tableLayoutPanel2.TabIndex = 9;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Controls.Add(tableLayoutPanel4, 0, 0);
            tableLayoutPanel3.Controls.Add(Bot1Pic, 1, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(481, 191);
            tableLayoutPanel3.TabIndex = 9;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 1;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Controls.Add(Team1, 0, 0);
            tableLayoutPanel4.Controls.Add(Bot1, 0, 1);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(0, 0);
            tableLayoutPanel4.Margin = new Padding(0);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 2;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Size = new Size(240, 191);
            tableLayoutPanel4.TabIndex = 9;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 3;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel5.Controls.Add(tableLayoutPanel1, 2, 0);
            tableLayoutPanel5.Controls.Add(tableLayoutPanel3, 0, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 351);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Size = new Size(1218, 197);
            tableLayoutPanel5.TabIndex = 10;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel6.AutoSize = true;
            tableLayoutPanel6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel6.ColumnCount = 1;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.Controls.Add(Stopwatch, 0, 1);
            tableLayoutPanel6.Controls.Add(TimerMajor, 0, 0);
            tableLayoutPanel6.Location = new Point(3, 95);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new RowStyle());
            tableLayoutPanel6.RowStyles.Add(new RowStyle());
            tableLayoutPanel6.Size = new Size(1218, 136);
            tableLayoutPanel6.TabIndex = 11;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.BackColor = Color.Transparent;
            tableLayoutPanel7.ColumnCount = 1;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel7.Controls.Add(tableLayoutPanel6, 0, 0);
            tableLayoutPanel7.Controls.Add(tableLayoutPanel5, 0, 2);
            tableLayoutPanel7.Dock = DockStyle.Fill;
            tableLayoutPanel7.Location = new Point(0, 0);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 3;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 42.46824F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 20.6896553F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 36.6986732F));
            tableLayoutPanel7.Size = new Size(1224, 551);
            tableLayoutPanel7.TabIndex = 12;
            // 
            // Scoreboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1224, 551);
            Controls.Add(tableLayoutPanel7);
            DoubleBuffered = true;
            Name = "Scoreboard";
            Text = "Scoreboard";
            FormClosing += ScoreboardFormClosing;
            ((System.ComponentModel.ISupportInitialize)Bot1Pic).EndInit();
            ((System.ComponentModel.ISupportInitialize)Bot2Pic).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel7.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label TimerMajor;
        private Label Team1;
        private Label Bot1;
        private Label Bot2;
        private Label Team2;
        private Label Stopwatch;
        private PictureBox Bot1Pic;
        private PictureBox Bot2Pic;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private TableLayoutPanel tableLayoutPanel5;
        private TableLayoutPanel tableLayoutPanel6;
        private TableLayoutPanel tableLayoutPanel7;
    }
}