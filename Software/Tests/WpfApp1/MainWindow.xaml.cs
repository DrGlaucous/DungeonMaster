using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private System.Timers.Timer timer_minor = new(10);
        private int value = 0;

        static bool aa = false;

        private void MyMethod(object sender, RoutedEventArgs e)
        {
            if ((string)buttonn.Content == "APPLE")
            {
                buttonn.Content = "PEAR";

                if (!aa)
                {
                    timer_minor.Elapsed += IncrementStopwatch;
                    timer_minor.AutoReset = true;
                    timer_minor.Enabled = true;
                    timer_minor.Stop();
                    aa = true;
                }

                StartStopwatch();
            }
            else
            {
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