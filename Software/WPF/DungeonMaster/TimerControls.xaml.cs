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


        public delegate void OnTimeAddHandler(TimeSpan delta_time); //(object sender, OnSerialGetEventHandler e)
        public event OnTimeAddHandler? EventHandler;// { add { handler += value; } remove { handler -= value } }


        public TimerControl()
        {
            InitializeComponent();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            int minutes = MinuteSelector.Number;
            int seconds = SecondSelector.Number;

            var delta_time = TimeSpan.FromSeconds(minutes * 60 + seconds);
            EventHandler?.Invoke(delta_time);

        }


        public void UpdateTimeDisplay(TimeSpan time) {
            int minutes = time.Minutes;
            int seconds = time.Seconds;
            int centiseconds = time.Milliseconds / 10;

            TimeDisplay.Text = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + centiseconds.ToString("D2");

        }

    }
}
