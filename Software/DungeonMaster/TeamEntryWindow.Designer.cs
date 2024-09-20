
namespace DungeonMaster
{
    partial class TeamEntryWindow
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
            layout_frame = new TableLayoutPanel();
            tableLayoutPanel6 = new TableLayoutPanel();
            label1 = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            label3 = new Label();
            browse_button = new Button();
            team_field = new TextBox();
            BotImageWindow = new PictureBox();
            label2 = new Label();
            bot_field = new TextBox();
            team_color_label = new Label();
            layout_frame.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)BotImageWindow).BeginInit();
            SuspendLayout();
            // 
            // layout_frame
            // 
            layout_frame.AutoSize = true;
            layout_frame.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            layout_frame.BackColor = Color.FromArgb(192, 192, 255);
            layout_frame.ColumnCount = 1;
            layout_frame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout_frame.Controls.Add(tableLayoutPanel6, 0, 1);
            layout_frame.Controls.Add(team_color_label, 0, 0);
            layout_frame.Dock = DockStyle.Fill;
            layout_frame.Location = new Point(0, 0);
            layout_frame.Name = "layout_frame";
            layout_frame.RowCount = 2;
            layout_frame.RowStyles.Add(new RowStyle());
            layout_frame.RowStyles.Add(new RowStyle());
            layout_frame.Size = new Size(508, 241);
            layout_frame.TabIndex = 20;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.AutoSize = true;
            tableLayoutPanel6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel6.ColumnCount = 3;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.Controls.Add(label1, 0, 0);
            tableLayoutPanel6.Controls.Add(flowLayoutPanel1, 2, 0);
            tableLayoutPanel6.Controls.Add(team_field, 1, 0);
            tableLayoutPanel6.Controls.Add(BotImageWindow, 2, 1);
            tableLayoutPanel6.Controls.Add(label2, 0, 1);
            tableLayoutPanel6.Controls.Add(bot_field, 1, 1);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 18);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new RowStyle());
            tableLayoutPanel6.RowStyles.Add(new RowStyle());
            tableLayoutPanel6.Size = new Size(502, 220);
            tableLayoutPanel6.TabIndex = 18;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(70, 15);
            label1.TabIndex = 11;
            label1.Text = "Team Name";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel1.Controls.Add(label3);
            flowLayoutPanel1.Controls.Add(browse_button);
            flowLayoutPanel1.Location = new Point(222, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(148, 29);
            flowLayoutPanel1.TabIndex = 17;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            label3.AutoSize = true;
            label3.Location = new Point(3, 0);
            label3.Name = "label3";
            label3.Size = new Size(61, 29);
            label3.TabIndex = 16;
            label3.Text = "Bot Image";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // browse_button
            // 
            browse_button.Location = new Point(70, 3);
            browse_button.Name = "browse_button";
            browse_button.Size = new Size(75, 23);
            browse_button.TabIndex = 14;
            browse_button.Text = "Browse";
            browse_button.UseVisualStyleBackColor = true;
            browse_button.Click += browse_button_Click;
            // 
            // team_field
            // 
            team_field.Location = new Point(79, 3);
            team_field.Name = "team_field";
            team_field.Size = new Size(137, 23);
            team_field.TabIndex = 10;
            team_field.LostFocus += team_field_lost_focus;
            // 
            // BotImageWindow
            // 
            BotImageWindow.Dock = DockStyle.Fill;
            BotImageWindow.Location = new Point(222, 38);
            BotImageWindow.Name = "BotImageWindow";
            BotImageWindow.Size = new Size(277, 179);
            BotImageWindow.SizeMode = PictureBoxSizeMode.Zoom;
            BotImageWindow.TabIndex = 15;
            BotImageWindow.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 35);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 13;
            label2.Text = "Bot Name";
            // 
            // bot_field
            // 
            bot_field.Location = new Point(79, 38);
            bot_field.Name = "bot_field";
            bot_field.Size = new Size(137, 23);
            bot_field.TabIndex = 12;
            bot_field.Leave += bot_field_lost_focus;
            // 
            // team_color_label
            // 
            team_color_label.AutoSize = true;
            team_color_label.Location = new Point(3, 0);
            team_color_label.Name = "team_color_label";
            team_color_label.Size = new Size(45, 15);
            team_color_label.TabIndex = 19;
            team_color_label.Text = "X Team";
            // 
            // TeamEntryWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(layout_frame);
            Name = "TeamEntryWindow";
            Size = new Size(508, 241);
            layout_frame.ResumeLayout(false);
            layout_frame.PerformLayout();
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)BotImageWindow).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel layout_frame;
        private TableLayoutPanel tableLayoutPanel6;
        private Label label1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label label3;
        private Button browse_button;
        private TextBox team_field;
        private PictureBox BotImageWindow;
        private Label label2;
        private TextBox bot_field;
        private Label team_color_label;
    }
}
