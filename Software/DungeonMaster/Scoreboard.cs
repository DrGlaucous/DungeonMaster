using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace DungeonMaster
{
    public partial class Scoreboard : Form
    {
        private System.Timers.Timer timer_major = new(10); //every 1/100 of a second
        private DateTime last_trigger_time_major = DateTime.Now; //used to get the timespan between timer callbacks
        private TimeSpan timespan_major = TimeSpan.Zero;


        private System.Timers.Timer timer_minor = new(10);
        private DateTime last_trigger_time_minor = DateTime.Now;
        private TimeSpan timespan_stopwatch = TimeSpan.Zero;

        private TeamEntryData team_1_data = new();
        private TeamEntryData team_2_data = new();

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



        //adds time to the current major timer
        public void AddTimeMajor(TimeSpan newtime) { 
        
            timespan_major += newtime;
            if (timespan_major.CompareTo(TimeSpan.Zero) < 0) {
                timespan_major = TimeSpan.Zero;
            }

            SetTimerMajorTS(timespan_major);
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
            timespan_major = TimeSpan.Zero;
            SetTimerMajorTS(timespan_major);
        }

        //ticks the timer down and runs a callback when time has reached 0
        private void IncrementTimerMajor(Object? source, ElapsedEventArgs e) {
            TimeSpan delta_t = e.SignalTime - last_trigger_time_major;
            timespan_major -= delta_t;
            last_trigger_time_major = e.SignalTime;

            

            //outtatime
            if (timespan_major.CompareTo(TimeSpan.Zero) <= 0)
            {
                timer_major.Stop();
                timespan_major = TimeSpan.Zero;

                //run timer finished callback
                EventHandler?.Invoke();

            }

            SetTimerMajorTS(timespan_major);


        }

        //update the timer to display a timespan from an outside thread
        private void SetTimerMajorTS(TimeSpan time)
        {
            //update from outside threads
            if (TimerMajor.InvokeRequired)
            {
                TimerMajor.Invoke(new MethodInvoker(() => { SetTimerMajorTS(time); }));
                return;
            }

            int minutes = time.Minutes;
            int seconds = time.Seconds;
            int centiseconds = time.Milliseconds / 10;

            TimerMajor.Text = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + centiseconds.ToString("D2");

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
        public void ResetStopwatch() {
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
            //update from outside threads
            if (Stopwatch.InvokeRequired)
            {
                Stopwatch.Invoke(new MethodInvoker(() => { SetStopwatchTS(time); }));
                return;
            }

            int minutes = time.Minutes;
            int seconds = time.Seconds;
            int centiseconds = time.Milliseconds / 10;

            Stopwatch.Text = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + centiseconds.ToString("D2");

        }



        //load video and play it over the scoreboard (stretched to width/height)
        public void ShowVideo(String video_path)
        {



        }


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
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => { OnImageUpdate(); }));
                return;
            }

            Team1.Text = team_1_data.TeamName;
            Bot1.Text = team_1_data.BotName;

            Team2.Text = team_2_data.TeamName;
            Bot2.Text = team_2_data.BotName;
        }

        //invoked whenever image fields of the bound data change
        private void OnImageUpdate()
        {
            //update hande from outside threads
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => { OnImageUpdate(); }));
                return;
            }

            Bot1Pic.Image = team_1_data.BotImage;
            Bot2Pic.Image = team_2_data.BotImage;


        }



        //close entire application if window is closed
        private void ScoreboardFormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();

            //this.Hide();
            //e.Cancel = true; // this cancels the close event.
        }
    }
}
