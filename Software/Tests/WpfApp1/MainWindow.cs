using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;
using System.Diagnostics;
using System.Reflection;

namespace WpfApp1
{
    public partial class MainWindow
    {
        private System.Timers.Timer timer_minor = new(10);
        private int value = 0;

        static bool aa = false;

        private void MyMethod(object sender, RoutedEventArgs e)
        {
            if ((string)buttonn.Content == "APPLE")
            {
                buttonn.Content = "PEAR";

                if (!aa) {
                    timer_minor.Elapsed += IncrementStopwatch;
                    timer_minor.AutoReset = true;
                    timer_minor.Enabled = true;
                    timer_minor.Stop();
                    aa = true;
                }

                StartStopwatch();
            }
            else {
                buttonn.Content = "APPLE";
                StopStopwatch();
            }
            
        }

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

        //ticks the stopwatch up
        private void IncrementStopwatch(Object? source, ElapsedEventArgs e)
        {
            value += 1;
            log_left_accs(value);
        }

        //update stopwatch to display timespan from outside thread
        delegate void ParametrizedMethodInvoker5(int arg);
        void log_left_accs(int arg)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new ParametrizedMethodInvoker5(log_left_accs), arg);
                return;
            }
            StopwatchLabel.Content = arg.ToString();
        }



    }
}
