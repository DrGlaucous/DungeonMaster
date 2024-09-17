using static DungeonMaster.SerialManager;
using static DungeonMaster.Terminal;

namespace DungeonMaster
{
    public partial class ControlPanel : Form
    {
        public ControlPanel()
        {
            InitializeComponent();

            //if scoreboard doesn't exist, make it and show it
            scoreboard ??= new Scoreboard();
            scoreboard.Show();

            stream_overlay ??= new StreamOverlay();
            stream_overlay.Show();

            //set and bind team sub-windows
            {
                scoreboard_data.team_red_data.form_label = "Red Team";
                scoreboard_data.team_red_data.form_color = Color.FromArgb(255, 192, 192);
                scoreboard_data.team_blue_data.form_label = "Blue Team";
                scoreboard_data.team_blue_data.form_color = Color.FromArgb(192, 192, 255);
                //sub-menus will now have control over this data
                red_team.bind_data(scoreboard_data.team_red_data);
                blue_team.bind_data(scoreboard_data.team_blue_data);
            }

            //link terminal to serial manager
            serialManager1.EventHandler += new OnSerialGetEventHandler(terminal1.write_to_window);
            terminal1.EventHandler += new OnSerialSendEventHandler(serialManager1.SendString);

        }



        private ScoreboardData scoreboard_data = new();
        private Scoreboard scoreboard = new();
        private StreamOverlay stream_overlay = new();

        private void test_button_Click(object sender, EventArgs e)
        {
            string unused = String.Empty;

        }
    }

    //non-form classes must be placed down here
    public class ScoreboardData
    {
        //timers
        public TimeSpan main_timer = TimeSpan.Zero;
        public TimeSpan pin_timer = TimeSpan.Zero;

        //team data (names, bot images)
        public TeamEntryData team_red_data = new();
        public TeamEntryData team_blue_data = new();


    }

}
