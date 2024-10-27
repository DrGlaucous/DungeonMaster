using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static DungeonMaster.Timer;

namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for Scoreboard.xaml
    /// </summary>
    public partial class Scoreboard : Window
    {


        Timer timer_major = new(); //big clock
        Timer timer_minor = new(); //stopwatch


        //private System.Timers.Timer timer_minor = new(20);
        //private DateTime last_trigger_time_minor = DateTime.Now;
        //private TimeSpan timespan_stopwatch = TimeSpan.Zero;


        private TeamEntryData team_1_data = new();
        private TeamEntryData team_2_data = new();


        //delegates for updating some methods from outside threads
        //delegate void TimespanMethodInvoker(TimeSpan time);
        //delegate void VoidMethodInvoker();


        public enum ScoreboardAction
        {
            StartMain, //start main countdown
            StopMain, //stop main countdown
            StartStopwatch, //start stopwatch countup
            StopStopwatch, //stop stopwatch countup
            ResetStopwatch, //reset stopwatch to 0
            AddTimeMain, //add more time to main timer
            ResetMain, //resets main time

            AddTimerEvent, //add an event to run at a certain time
            RemoveTimerEvent, //remove a time-triggered event
            ClearTimerEvent, //remove all timer-triggered events

            ShowImage, //overlays a static image onscreen
            HideImage, //hides the overlayed image
            
            LoadMedia, //loads media to be played back
            UnloadMedia, //unloads currently loaded media
            PlayMedia, //trys to play the media
            PauseMedia, //trys to pause the media
            StopMedia, //stops + rewinds the media
            HideMedia, //hides media image (only for video files)
            ShowMedia, //shows media image (ony for video files)
        }

        public Scoreboard()
        {
            InitializeComponent();

            //timer_minor.Elapsed += IncrementStopwatch;
            //timer_minor.AutoReset = true;
            //timer_minor.Enabled = true;
            //timer_minor.Stop();

            //set up timer objects
            timer_major.TickUp = false;
            timer_major.TimeUpdateHandler += SetTimerMajorTS; //update screen element when this is changed

            timer_minor.TickUp = true;
            timer_minor.TimeUpdateHandler += SetStopwatchTS; //update screen element when this is changed




        }

        //close everything with this window
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }


        //adds time to the current major timer (reqired for outside access)
        public void AddTimeMajor(TimeSpan newtime)
        {
            timer_major.AddTime(newtime);
        }

        //update the timer to display a timespan from an outside thread
        private void SetTimerMajorTS(TimeSpan time)
        {
            //update hande from outside threads
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new TimespanDelegate(SetTimerMajorTS), args: time);
                return;
            }


            int minutes = (int)(time.TotalMinutes);
            int seconds = time.Seconds;
            int centiseconds = time.Milliseconds / 10;

            Countdown.Content = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + centiseconds.ToString("D2");

        }



        //callback when the main timer runs out
        //public event OnTimerMajorEnd? EventHandler;


        //pass the time-changed method out from the internal class so other events can run when this changes
        public event TimespanDelegate? TimeMajorUpdateHandler {
            add {
                timer_major.TimeUpdateHandler += value;
            }
            remove {
                timer_major.TimeUpdateHandler -= value;
            }
        }
        public event TimespanDelegate? TimeMinorUpdateHandler
        {
            add
            {
                timer_minor.TimeUpdateHandler += value;
            }
            remove
            {
                timer_minor.TimeUpdateHandler -= value;
            }
        }

        //used to send timer callbacks out to the TSC engine
        public event IntDelegate? RunTSCEvent
        {
            add
            {
                timer_minor.RunTSC += value;
                timer_major.RunTSC += value;
            }
            remove
            {
                timer_minor.RunTSC -= value;
                timer_major.RunTSC -= value;
            }
        }


        //update stopwatch to display timespan from outside thread
        private void SetStopwatchTS(TimeSpan time)
        {
            //update hande from outside threads
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new TimespanDelegate(SetStopwatchTS), args: time);
                return;
            }

            int minutes = (int)(time.TotalMinutes);
            int seconds = time.Seconds;
            int centiseconds = time.Milliseconds / 10;

            Stopwatch.Content = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + centiseconds.ToString("D2");

        }


        private bool LoadImage(String path)
        {
            try
            {
                //make absolute path from relative one
                var abspath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), path);
                ImageOverlay.Source = new BitmapImage(new Uri(abspath));
            }
            catch { }
            return false;
        }


        //wraps the methods above into a single method that can be invoked from the TSC engine
        public void RunAction(ScoreboardAction action, params object[] list)
        {
            if (!Dispatcher.CheckAccess())
            {
                //Dispatcher.Invoke(() => RunAction(action, list));
                Dispatcher.Invoke(new ScoreboardActionDelegate(RunAction), action, list);
                return;
            }

            switch (action)
            {
                default: { break; }

                //stopwatch actions
                case ScoreboardAction.StartStopwatch:
                    {
                        timer_minor.Start();
                        break;
                    }
                case ScoreboardAction.StopStopwatch:
                    {
                        timer_minor.Stop();
                        break;
                    }
                case ScoreboardAction.ResetStopwatch:
                    {
                        timer_minor.Reset();
                        break;
                    }
                //counter actions
                case ScoreboardAction.StartMain:
                    {
                        timer_major.Start();
                        break;
                    }
                case ScoreboardAction.StopMain:
                    {
                        timer_major.Stop();
                        break;
                    }
                case ScoreboardAction.AddTimeMain:
                    {
                        try
                        {
                            var arg1 = (TimeSpan)list[0];
                            timer_major.AddTime(arg1);
                        }
                        catch { }
                        break;
                    }
                case ScoreboardAction.ResetMain:
                    {
                        try
                        {
                            timer_major.Reset();
                        }
                        catch { }
                        break;
                    }

                //add-remove event actions
                case ScoreboardAction.AddTimerEvent:
                    {
                        try
                        {
                            int arg1 = (int)list[0];
                            var arg2 = (TimeSpan)list[1];
                            timer_major.AddEvent(arg1, arg2);
                        }
                        catch { }
                        break;
                    }
                case ScoreboardAction.RemoveTimerEvent:
                    {
                        try
                        {
                            int arg1 = (int)list[0];
                            timer_major.RemoveEvent(arg1);
                        }
                        catch { }
                        break;
                    }
                case ScoreboardAction.ClearTimerEvent:
                    {
                        try
                        {
                            timer_major.ClearEvents();
                        }
                        catch { }
                        break;
                    }

                //media actions
                case ScoreboardAction.ShowImage:
                    {
                        try
                        {
                            string arg1 = (string)list[0];
                            LoadImage(arg1);
                        }
                        catch { }
                        break;
                    }
                case ScoreboardAction.HideImage:
                    {
                        ImageOverlay.Source = new BitmapImage();
                        break;
                    }                
                case ScoreboardAction.LoadMedia:
                    {
                        try
                        {
                            int arg1 = (int)list[0];
                            string arg2 = (string)list[1];
                            if (arg1 == 0)
                            {
                                MediaBuf1.LoadAVMedia(arg2);
                            }
                            else
                            {
                                MediaBuf2.LoadAVMedia(arg2);
                            }
                        }
                        catch { }
                        break;
                    }
                case ScoreboardAction.UnloadMedia:
                case ScoreboardAction.PlayMedia:
                case ScoreboardAction.PauseMedia:
                case ScoreboardAction.StopMedia:
                case ScoreboardAction.HideMedia:
                case ScoreboardAction.ShowMedia:
                    {
                        try
                        {
                            int arg1 = (int)list[0];
                            MediaHandler handler_ref = (arg1 == 0)? MediaBuf1 : MediaBuf2;
                            
                            switch (action) {
                                case ScoreboardAction.UnloadMedia: { handler_ref.UnloadAVMedia(); break; }
                                case ScoreboardAction.PlayMedia: { handler_ref.PlayAVMedia(); break; }
                                case ScoreboardAction.PauseMedia: { handler_ref.PauseAVMedia(); break; }
                                case ScoreboardAction.StopMedia: { handler_ref.StopAVMedia(); break; }
                                case ScoreboardAction.HideMedia: { handler_ref.Visibility = Visibility.Hidden; break; }
                                case ScoreboardAction.ShowMedia: { handler_ref.Visibility = Visibility.Visible; break; }
                            }
                        }
                        catch { }
                        break;
                    }


            }

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
                Dispatcher.Invoke(new VoidDelegate(OnImageUpdate));
                return;
            }

            Bot1Pic.Source = team_1_data.BotImage;
            Bot2Pic.Source = team_2_data.BotImage;


        }


    }
}
