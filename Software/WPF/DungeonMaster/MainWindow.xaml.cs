using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static DungeonMaster.SerialManager;
using static DungeonMaster.Terminal;
using static DungeonMaster.TSCEngine;

namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TSCEngine TextScriptEngine = new();
        private Scoreboard Scoreboard = new();

        //team data (names, bot images)
        public TeamEntryData TeamRedData = new();
        public TeamEntryData TeamBlueData = new();



        public MainWindow()
        {
            InitializeComponent();

            //if scoreboard doesn't exist, make it and show it
            Scoreboard ??= new Scoreboard();
            Scoreboard.Show();

            //set and bind team sub-windows
            {
                //team_red_data.FormColor = Color.FromArgb(255, 192, 192);
                //team_blue_data.FormColor = Color.FromArgb(192, 192, 255);

                TeamRedData.FormLabel = "Red Team";     
                TeamBlueData.FormLabel = "Blue Team";
            
                //sub-menus will now have control over this data
                RedTeamWindow.BindData(TeamRedData);
                BlueTeamWindow.BindData(TeamBlueData);

                //scoreboard will now display this data
                Scoreboard.BindData(TeamRedData, TeamBlueData);
            }

            //link terminal to serial manager
            SerialManager.EventHandler += new OnSerialGetEventHandler(Terminal.WriteToWindow); //serial in goes to terminal
            Terminal.SendCommandHandler += new OnSendCommandHandler(TextScriptEngine.RunEventDebug); //terminal out goes to TSC

            TextScriptEngine.SendMessageHandler += new OnWriteOutHandler(Terminal.WriteToWindow); //tsc messages go to terminal
            TextScriptEngine.SendCommandHandler += new OnWriteOutHandler(SerialManager.SendString); //tsc commands go to terminal + serial


            //link timer control
            TimerControl.EventHandler += Scoreboard.AddTimeMajor;
            Scoreboard.TimeUpdateHandler += TimerControl.UpdateTimeDisplay;


            TextScriptEngine.LoadScript("./Script.tsc.txt");

            //test
            //Scoreboard.StartStopwatch();
        }


        //close everything with this window
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}