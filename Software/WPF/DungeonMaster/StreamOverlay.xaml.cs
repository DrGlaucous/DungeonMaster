using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for StreamOverlay.xaml
    /// </summary>
    public partial class StreamOverlay : Window
    {
        public StreamOverlay()
        {
            InitializeComponent();
        }

        private TeamEntryData team_1_data = new();
        private TeamEntryData team_2_data = new();


        //close everything with this window
        private void WindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        //pass refrences to teamname and image data so they'll be updated with the settings
        public void BindData(TeamEntryData red_data, TeamEntryData blue_data)
        {

            this.team_1_data = red_data;
            this.team_2_data = blue_data;

            this.team_1_data.TxtEventHandler += OnTextUpdate;
            this.team_2_data.TxtEventHandler += OnTextUpdate;

            this.team_1_data.ImgEventHandler += OnImageUpdate;
            this.team_2_data.ImgEventHandler += OnImageUpdate;
        }

        //invoked whenever text fields of the bound data change
        private void OnTextUpdate()
        {
            //update hande from outside threads
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new VoidDelegate(OnTextUpdate));
                return;
            }

            Team1.Text = team_1_data.TeamName;
            Robot1.Text = team_1_data.BotName;

            Team2.Text = team_2_data.TeamName;
            Robot2.Text = team_2_data.BotName;
        }
        private void OnImageUpdate()
        {
            //update hande from outside threads
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new VoidDelegate(OnImageUpdate));
                return;
            }

            Bot1Pic.Source = team_1_data.BotImage;
            Bot2Pic.Source = team_2_data.BotImage;


        }

        public void UpdateMainTimeDisplay(TimeSpan time)
        {
            //update hande from outside threads
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new TimespanDelegate(UpdateMainTimeDisplay), args: time);
                return;
            }

            int minutes = (int)(time.TotalMinutes);
            int seconds = time.Seconds;
            int centiseconds = time.Milliseconds / 10;

            TimerMain.Text = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + centiseconds.ToString("D2");

        }
        public void UpdateSecondTimeDisplay(TimeSpan time)
        {
            //update hande from outside threads
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new TimespanDelegate(UpdateSecondTimeDisplay), args: time);
                return;
            }

            int minutes = (int)(time.TotalMinutes);
            int seconds = time.Seconds;
            int centiseconds = time.Milliseconds / 10;

            TimerSecond.Text = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + centiseconds.ToString("D2");

        }


    }
}
