using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static DungeonMaster.ResponseEngine;
using static DungeonMaster.SerialManager;
using static DungeonMaster.Terminal;
using static DungeonMaster.TSCEngine;


using System.IO;
using System.Reflection;


namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TSCEngine TextScriptEngine = new();
        private ResponseEngine ResponseEngine = new();
        private Scoreboard Scoreboard = new();
        private StreamOverlay StreamOverlay = new();

        //team data (names, bot images)
        public TeamEntryData TeamRedData = new();
        public TeamEntryData TeamBlueData = new();



        public MainWindow()
        {
            InitializeComponent();

            //if scoreboard doesn't exist, make it and show it
            Scoreboard ??= new Scoreboard();
            Scoreboard.Show();

            StreamOverlay ??= new StreamOverlay();
            StreamOverlay.Show();

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

            //link script interfaces
            {
                //link terminal to serial manager
                SerialManager.DataGetEventHandler += new OnSerialGetEventHandler(Terminal.WriteToWindow); //serial in goes to terminal
                SerialManager.DataGetEventHandler += new OnSerialGetEventHandler(ResponseEngine.ParseResponse); //serial in goes to response engine

                ResponseEngine.RunEventHandler += new OnRunEventHandler(TextScriptEngine.PushEvent); //responses goes to TSC queue
                Terminal.SendCommandHandler += new OnSendCommandHandler(TextScriptEngine.RunEventDebug); //terminal out goes to TSC debug

                TextScriptEngine.SendMessageHandler += new OnWriteOutHandler(Terminal.WriteToWindow); //tsc messages go to terminal
                TextScriptEngine.SendCommandHandler += new OnWriteOutHandler(SerialManager.SendString); //tsc commands go to serial

                SimJudgeControls.SendResponseHandler += new JudgeControls.OnSerialGetEventHandler(ResponseEngine.ParseResponse);
                SimBoxControls.SendResponseHandler += new BoxControls.OnSerialGetEventHandler(ResponseEngine.ParseResponse);
            }

            //link timer control
            TimerControl.EventHandler += Scoreboard.AddTimeMajor;
            Scoreboard.TimeUpdateHandler += TimerControl.UpdateTimeDisplay;

            TextScriptEngine.ScoreboardControlHandler += Scoreboard.RunTscAction; //bind TSC to scoreboard control
            TextScriptEngine.LoadScript("./Script.tsc.txt");

            //test
            //Scoreboard.StartStopwatch();
            //ResponseEngine.ParseResponse("");

            //var resources = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

            //var uri = new Uri("pack://application:,,,/Scoreboard.png");
            //var uri2 = new Uri("pack://application:,,,/ShaderSource/ColorKeyAlpha.ps");
            ////var testimg = new BitmapImage(uri);

            //var uri3 = MakePackUri("ShaderSource/ColorKeyAlpha.ps");

            //var resourceInfo = Application.GetResourceStream(uri3);


            //var pixelShader = new PixelShader();
            //pixelShader.UriSource = uri3;


        }


        //public static Uri MakePackUri(string relativeFile)
        //{
        //    Assembly a = typeof(MainWindow).Assembly;

        //    // Extract the short name.
        //    string assemblyShortName = a.ToString().Split(',')[0];

        //    string uriString = "pack://application:,,,/" +
        //        assemblyShortName +
        //        ";component/" +
        //        relativeFile;

        //    return new Uri(uriString);
        //}





        //close everything with this window
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}