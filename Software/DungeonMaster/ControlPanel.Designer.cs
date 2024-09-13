namespace DungeonMaster
{
    partial class ControlPanel
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            judgeControls1 = new JudgeControls();
            boxControls1 = new BoxControls();
            splitContainer1 = new SplitContainer();
            test_button = new Button();
            blue_team = new TeamEntryWindow();
            red_team = new TeamEntryWindow();
            timerControls1 = new TimerControls();
            tableLayoutPanel14 = new TableLayoutPanel();
            serialManager1 = new SerialManager();
            terminal1 = new Terminal();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel14.SuspendLayout();
            SuspendLayout();
            // 
            // judgeControls1
            // 
            judgeControls1.BorderStyle = BorderStyle.FixedSingle;
            judgeControls1.Dock = DockStyle.Left;
            judgeControls1.Location = new Point(0, 0);
            judgeControls1.Name = "judgeControls1";
            judgeControls1.Size = new Size(351, 349);
            judgeControls1.TabIndex = 23;
            // 
            // boxControls1
            // 
            boxControls1.Dock = DockStyle.Left;
            boxControls1.Location = new Point(351, 0);
            boxControls1.Name = "boxControls1";
            boxControls1.Size = new Size(334, 349);
            boxControls1.TabIndex = 24;
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(3, 40);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.AutoScroll = true;
            splitContainer1.Panel1.Controls.Add(test_button);
            splitContainer1.Panel1.Controls.Add(blue_team);
            splitContainer1.Panel1.Controls.Add(red_team);
            splitContainer1.Panel1.Controls.Add(timerControls1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.AutoScroll = true;
            splitContainer1.Panel2.Controls.Add(boxControls1);
            splitContainer1.Panel2.Controls.Add(judgeControls1);
            splitContainer1.Size = new Size(1342, 370);
            splitContainer1.SplitterDistance = 774;
            splitContainer1.TabIndex = 26;
            // 
            // test_button
            // 
            test_button.Location = new Point(750, 58);
            test_button.Name = "test_button";
            test_button.Size = new Size(75, 23);
            test_button.TabIndex = 33;
            test_button.Text = "Test";
            test_button.UseVisualStyleBackColor = true;
            test_button.Click += test_button_Click;
            // 
            // blue_team
            // 
            blue_team.Location = new Point(512, 142);
            blue_team.Name = "blue_team";
            blue_team.Size = new Size(332, 128);
            blue_team.TabIndex = 32;
            // 
            // red_team
            // 
            red_team.Location = new Point(53, 128);
            red_team.Name = "red_team";
            red_team.Size = new Size(424, 152);
            red_team.TabIndex = 31;
            // 
            // timerControls1
            // 
            timerControls1.AutoSize = true;
            timerControls1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            timerControls1.Location = new Point(53, 33);
            timerControls1.Name = "timerControls1";
            timerControls1.Size = new Size(441, 70);
            timerControls1.TabIndex = 30;
            // 
            // tableLayoutPanel14
            // 
            tableLayoutPanel14.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel14.ColumnCount = 1;
            tableLayoutPanel14.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel14.Controls.Add(serialManager1, 0, 0);
            tableLayoutPanel14.Controls.Add(terminal1, 0, 2);
            tableLayoutPanel14.Controls.Add(splitContainer1, 0, 1);
            tableLayoutPanel14.Dock = DockStyle.Fill;
            tableLayoutPanel14.Location = new Point(0, 0);
            tableLayoutPanel14.Name = "tableLayoutPanel14";
            tableLayoutPanel14.RowCount = 3;
            tableLayoutPanel14.RowStyles.Add(new RowStyle());
            tableLayoutPanel14.RowStyles.Add(new RowStyle());
            tableLayoutPanel14.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel14.Size = new Size(1348, 632);
            tableLayoutPanel14.TabIndex = 27;
            // 
            // serialManager1
            // 
            serialManager1.AutoSize = true;
            serialManager1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            serialManager1.Location = new Point(3, 3);
            serialManager1.Name = "serialManager1";
            serialManager1.Size = new Size(401, 31);
            serialManager1.TabIndex = 29;
            // 
            // terminal1
            // 
            terminal1.Dock = DockStyle.Fill;
            terminal1.Location = new Point(3, 416);
            terminal1.Name = "terminal1";
            terminal1.Size = new Size(1342, 213);
            terminal1.TabIndex = 28;
            // 
            // ControlPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1348, 632);
            Controls.Add(tableLayoutPanel14);
            Name = "ControlPanel";
            Text = "Control Panel";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel14.ResumeLayout(false);
            tableLayoutPanel14.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private JudgeControls judgeControls1;
        private BoxControls boxControls1;
        private SplitContainer splitContainer1;
        private TableLayoutPanel tableLayoutPanel14;
        private Terminal terminal1;
        private SerialManager serialManager1;
        private TimerControls timerControls1;
        private Button test_button;
        private TeamEntryWindow blue_team;
        private TeamEntryWindow red_team;
    }
}
