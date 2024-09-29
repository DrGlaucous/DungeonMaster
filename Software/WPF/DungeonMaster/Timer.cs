using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DungeonMaster
{


    //a single stopwatch-esque timer that handles running events and counting up/down.
    //there are multiple of these in use throughout the application
    public class Timer
    {


        private System.Timers.Timer SCTimer = new(20); //called every 20 ms
        private DateTime LastTriggerTime = DateTime.Now; //used to get the timespan between timer callbacks

        //list of events to run at certain times during the countdown
        private List<TimerEvent> TimerEvents = [];


        //outside methods get the most current time when "SCTimespan" is changed if their callback is added to this
        public event TimespanDelegate? TimeUpdateHandler;

        //link out to TSC engine in order to run events from the timer
        public event IntDelegate? RunTSC;



        //contains the current "time" on this timer.
        //we never directly touch this (we use SCTimespan)
        private TimeSpan _SCTimespan = TimeSpan.Zero;
        public TimeSpan SCTimespan
        {
            get
            {
                return _SCTimespan;
            }
            set
            {
                _SCTimespan = value;
                TimeUpdateHandler?.Invoke(_SCTimespan);
            }
        }
        //if true, the timer counts up. Otherwise it counts down
        public bool TickUp { get; set; }


        ///////////////
        //methods

        public Timer() {
            SCTimer.Elapsed += IncrementTimer;
            SCTimer.AutoReset = true;
            SCTimer.Enabled = true;
            SCTimer.Stop();
        }


        public void Start() {
            LastTriggerTime = DateTime.Now; //reset delta-time to zero
            SCTimer.Start();
        }

        public void Stop() {
            SCTimer.Stop();
        }

        public void Reset()
        {
            SCTimespan = TimeSpan.Zero;
            foreach (var tevent in TimerEvents)
            {
                tevent.Reset(SCTimespan);
            }
        }

        public void AddTime(TimeSpan newtime)
        {
            SCTimespan += newtime;
            foreach (var tevent in TimerEvents)
            {
                tevent.Reset(SCTimespan);
            }
        }

        public void AddEvent(int event_no, TimeSpan event_time)
        {
            //check for existing event (only one trigger per time for now)
            for (int i = 0; i < TimerEvents.Count; ++i) {
                if (TimerEvents[i].Event == event_no) {
                    TimerEvents[i].TimeTrigger = event_time;
                    TimerEvents[i].Reset(SCTimespan);
                    return;
                }
            }
            //add new event
            var tevent = new TimerEvent(event_no, event_time);
            tevent.Reset(SCTimespan);
            tevent.RunEventHandler += RunTSC;
            TimerEvents.Add(tevent);
        }

        //returns true if event was found and removed
        public bool RemoveEvent(int event_no)
        {
            for (int i = 0; i < TimerEvents.Count; ++i)
            {
                if (TimerEvents[i].Event == event_no)
                {
                    TimerEvents.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void ClearEvents()
        {
            TimerEvents.Clear();
        }

        //ticks the timer
        private void IncrementTimer(Object? source, ElapsedEventArgs e)
        {
            TimeSpan delta_t = e.SignalTime - LastTriggerTime;
            SCTimespan += TickUp ? delta_t : -delta_t; //tick up or down, depending on the direction of the setting
            LastTriggerTime = e.SignalTime;

            //runs events as-needed
            foreach (var tevent in TimerEvents)
            {
                tevent.Tick(SCTimespan);
            }
        }

    }

    //stores an event to run when the timer passes a certian time
    public class TimerEvent {

        public event IntDelegate? RunEventHandler; //sends output to TSC engine

        public int Event { get; set; }
        public TimeSpan TimeTrigger { get; set; }

        //the last time we checked that the timer has changed
        private TimeSpan LastCheckedTime = TimeSpan.Zero;

        public TimerEvent(int event_no, TimeSpan time_trigger) {
            Event = event_no;
            TimeTrigger = time_trigger;
        }

        //call this whenever we jump to a new time so we don't run an event when we shouldn't
        public void Reset(TimeSpan current_time)
        {
            LastCheckedTime = current_time;
        }

        public void Tick(TimeSpan current_time)
        {
            //if the two tracked times sit on opposite sides of the trigger
            if ((LastCheckedTime < TimeTrigger && current_time >= TimeTrigger)
            || (LastCheckedTime >= TimeTrigger && current_time < TimeTrigger))
            {
                //invoke event
                RunEventHandler?.Invoke(Event);
            }
            LastCheckedTime = current_time;
        }

    }




}
