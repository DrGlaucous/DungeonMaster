using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for TimerControl.xaml
    /// </summary>
    public partial class TimerControl : UserControl
    {

        public event ScoreboardActionDelegate? ScoreboardControlHandler;// { add { handler += value; } remove { handler -= value } }

        //this is a bit of a hack since we want to zero the clock but can only adjust delta time, so we need to know what was on there already
        private TimeSpan LastUpdateTime = TimeSpan.Zero;

        public TimerControl()
        {
            InitializeComponent();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            int minutes = MinuteSelector.Number;
            int seconds = SecondSelector.Number;

            var delta_time = TimeSpan.FromSeconds(minutes * 60 + seconds);
            
            
            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.AddTimeMain, delta_time);

        }


        public void UpdateTimeDisplay(TimeSpan time) {

            //update hande from outside threads
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new TimespanDelegate(UpdateTimeDisplay), args: time);
                return;
            }

            LastUpdateTime = time;

            int minutes = (int)(time.TotalMinutes);
            int seconds = time.Seconds;
            int centiseconds = time.Milliseconds / 10;

            TimeDisplay.Text = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + centiseconds.ToString("D2");

        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            var delta_time = TimeSpan.Zero - LastUpdateTime;
            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.AddTimeMain, delta_time);
        }
    }
}
