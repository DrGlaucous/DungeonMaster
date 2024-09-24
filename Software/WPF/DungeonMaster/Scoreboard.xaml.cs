using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for Scoreboard.xaml
    /// </summary>
    public partial class Scoreboard : Window
    {
        private System.Timers.Timer timer_major = new(20); //every 1/100 of a second
        private DateTime last_trigger_time_major = DateTime.Now; //used to get the timespan between timer callbacks

        //invokes update methods on change
        public delegate void OnTimeUpdateHandler(TimeSpan time);
        public event OnTimeUpdateHandler? TimeUpdateHandler;

        private TimeSpan timespan_major = TimeSpan.Zero;
        public TimeSpan TimespanMajor
        {
            get {
                return timespan_major;
            }
            set {
                timespan_major = value;
                TimeUpdateHandler?.Invoke(timespan_major);
            }
        }

        private System.Timers.Timer timer_minor = new(20);
        private DateTime last_trigger_time_minor = DateTime.Now;
        private TimeSpan timespan_stopwatch = TimeSpan.Zero;

        private TeamEntryData team_1_data = new();
        private TeamEntryData team_2_data = new();

        delegate void TimespanMethodInvoker(TimeSpan time);
        delegate void VoidMethodInvoker();


        public enum ScoreboardAction
        {
            StartMain, //start main countdown
            StopMain, //stop main countdown
            StartStopwatch, //start stopwatch countup
            StopStopwatch, //stop stopwatch countup
            ResetStopwatch, //reset stopwatch to 0
            AddTimeMsMain, //add more time to main timer
        }

        public Scoreboard()
        {
            InitializeComponent();

            timer_major.Elapsed += IncrementTimerMajor;
            timer_major.AutoReset = true;
            timer_major.Enabled = true;
            timer_major.Stop();


            timer_minor.Elapsed += IncrementStopwatch;
            timer_minor.AutoReset = true;
            timer_minor.Enabled = true;
            timer_minor.Stop();

        }

        //close everything with this window
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        //adds time to the current major timer
        public void AddTimeMajor(TimeSpan newtime)
        {

            TimespanMajor += newtime;
            if (TimespanMajor.CompareTo(TimeSpan.Zero) < 0)
            {
                TimespanMajor = TimeSpan.Zero;
            }

            SetTimerMajorTS(TimespanMajor);
        }

        //starts counting down the main timer
        public void StartTimerMajor()
        {
            timer_major.Start();
        }

        //stops counting down the main timer
        public void StopTimerMajor()
        {
            timer_major.Stop();
        }

        //sets main timer time to 0
        public void ResetTimerMajor()
        {
            TimespanMajor = TimeSpan.Zero;
            SetTimerMajorTS(TimespanMajor);
        }

        //ticks the timer down and runs a callback when time has reached 0
        private void IncrementTimerMajor(Object? source, ElapsedEventArgs e)
        {
            TimeSpan delta_t = e.SignalTime - last_trigger_time_major;
            TimespanMajor -= delta_t;
            last_trigger_time_major = e.SignalTime;



            //outtatime
            if (TimespanMajor.CompareTo(TimeSpan.Zero) <= 0)
            {
                timer_major.Stop();
                TimespanMajor = TimeSpan.Zero;

                //run timer finished callback
                EventHandler?.Invoke();

            }

            SetTimerMajorTS(TimespanMajor);


        }

        //update the timer to display a timespan from an outside thread
        private void SetTimerMajorTS(TimeSpan time)
        {
            //update hande from outside threads
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new TimespanMethodInvoker(SetTimerMajorTS), args: time);
                return;
            }


            int minutes = time.Minutes;
            int seconds = time.Seconds;
            int centiseconds = time.Milliseconds / 10;

            TimerMajor.Content = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + centiseconds.ToString("D2");

        }

        //callback when the main timer runs out
        public delegate void OnTimerMajorEnd(); //(object sender, OnSerialGetEventHandler e)
        public event OnTimerMajorEnd? EventHandler;



        //stopwatch methods (todo: make one class that handles both the main timer and stopwatch and can be instantiated)

        //starts counting up the stopwatch
        public void StartStopwatch()
        {
            timer_minor.Start();
        }

        //stops counting up the stopwatch
        public void StopStopwatch()
        {
            timer_minor.Stop();
        }

        //sets stopwatch time to 0
        public void ResetStopwatch()
        {
            timespan_stopwatch = TimeSpan.Zero;
            SetStopwatchTS(timespan_stopwatch);
        }

        //ticks the stopwatch up
        private void IncrementStopwatch(Object? source, ElapsedEventArgs e)
        {
            TimeSpan delta_t = e.SignalTime - last_trigger_time_minor;
            timespan_stopwatch += delta_t;
            last_trigger_time_minor = e.SignalTime;
            SetStopwatchTS(timespan_stopwatch);
        }
        //update stopwatch to display timespan from outside thread
        private void SetStopwatchTS(TimeSpan time)
        {
            //update hande from outside threads
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new TimespanMethodInvoker(SetStopwatchTS), args: time);
                return;
            }

            int minutes = time.Minutes;
            int seconds = time.Seconds;
            int centiseconds = time.Milliseconds / 10;

            Stopwatch.Content = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + centiseconds.ToString("D2");

        }


        //wraps the methods above into a single method that can be invoked from the TSC engine
        public void RunTscAction(ScoreboardAction action, int arg1)
        {
            //StartMain, //start main countdown
            //StopMain, //stop main countdown
            //StartStopwatch, //start stopwatch countup
            //StopStopwatch, //stop stopwatch countup
            //ResetStopwatch, //reset stopwatch to 0
            //AddTimeMsMain, //add more time to main timer

            switch (action)
            {
                default: { break; }
                case ScoreboardAction.StartMain:
                    {
                        StartTimerMajor();
                        break;
                    }
                case ScoreboardAction.StopMain:
                    {
                        StopTimerMajor();
                        break;
                    }
                case ScoreboardAction.StartStopwatch:
                    {
                        StartStopwatch();
                        break;
                    }
                case ScoreboardAction.StopStopwatch:
                    {
                        StopStopwatch();
                        break;
                    }
                case ScoreboardAction.ResetStopwatch:
                    {
                        ResetStopwatch();
                        break;
                    }
                case ScoreboardAction.AddTimeMsMain:
                    {
                        var tspan = TimeSpan.FromMilliseconds(arg1);
                        AddTimeMajor(tspan);
                        break;
                    }
            }
        
        }


        //load video and play it over the scoreboard (stretched to width/height)
        public void ShowVideo(String video_path)
        {



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
                Dispatcher.Invoke(new VoidMethodInvoker(OnTextUpdate));
                return;
            }

            Team1.Content = team_1_data.TeamName;
            Robot1.Content = team_1_data.BotName;

            Team2.Content = team_2_data.TeamName;
            Robot2.Content = team_2_data.BotName;
        }

        //invoked whenever image fields of the bound data change
        private void OnImageUpdate()
        {
            //update hande from outside threads
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new VoidMethodInvoker(OnImageUpdate));
                return;
            }

            Bot1Pic.Source = team_1_data.BotImage;
            Bot2Pic.Source = team_2_data.BotImage;


        }


    }
}
