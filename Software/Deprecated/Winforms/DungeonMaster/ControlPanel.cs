using static DungeonMaster.SerialManager;
using static DungeonMaster.Terminal;
using static DungeonMaster.TSCEngine;

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
                team_red_data.FormLabel = "Red Team";
                team_red_data.FormColor = Color.FromArgb(255, 192, 192);
                team_blue_data.FormLabel = "Blue Team";
                team_blue_data.FormColor = Color.FromArgb(192, 192, 255);
                //sub-menus will now have control over this data
                red_team.BindData(team_red_data);
                blue_team.BindData(team_blue_data);

                //scoreboard will now display this data
                scoreboard.BindData(team_red_data, team_blue_data);
            }

            //link terminal to serial manager
            serialManager1.EventHandler += new OnSerialGetEventHandler(terminal1.write_to_window);
            terminal1.EventHandler += new OnSerialSendEventHandler(serialManager1.SendString);

            //testing stuff:
            scoreboard.AddTimeMajor(TimeSpan.FromSeconds(32));
            //scoreboard.StartStopwatch();
            TSCEngine engine = new TSCEngine();
            engine.LoadAndParseScript("");


        }


        //team data (names, bot images)
        public TeamEntryData team_red_data = new();
        public TeamEntryData team_blue_data = new();


        private Scoreboard scoreboard = new();
        private StreamOverlay stream_overlay = new();

        private void test_button_Click(object sender, EventArgs e)
        {
            string unused = String.Empty;

        }
    }



}
