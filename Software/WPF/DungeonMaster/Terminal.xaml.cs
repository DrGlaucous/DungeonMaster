using System;
using System.Collections.Generic;
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
    /// Interaction logic for Terminal.xaml
    /// </summary>
    public partial class Terminal : UserControl
    {
        public event StringDelegate? SendCommandHandler;// { add { handler += value; } remove { handler -= value } }

        static readonly int max_text_length = 5000;
        public Terminal()
        {
            InitializeComponent();
        }

        //example: update stopwatch to display timespan from outside thread
        //delegate void ParametrizedMethodInvoker5(int arg);
        //void log_left_accs(int arg)
        //{
        //    if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
        //    {
        //        Dispatcher.Invoke(new ParametrizedMethodInvoker5(log_left_accs), arg);
        //        return;
        //    }
        //    StopwatchLabel.Content = arg.ToString();
        //}


        delegate void ParametrizedMethodInvoker(string arg);
        public void WriteToWindow(string new_text)
        {

            //update hande from outside threads
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ParametrizedMethodInvoker(WriteToWindow), new_text);
                return;
            }

            //normal stuff

            //normalize carriage returns
            new_text = new_text.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

            //get length for size buffering
            var more_len = new_text.Length;
            var curr_len = SerialInbox.Text.Length;

            SerialInbox.Text += new_text; //add text (edge case: could potentially run out of memory here!)

            //if > 0, we've added too much, crop to length
            var len_diff = curr_len + more_len - max_text_length;
            if (len_diff > 0)
            {
                SerialInbox.Text = SerialInbox.Text[len_diff..];
            }

            ScrollContainer.ScrollToBottom();

        }

        private void SerialSendButtonClick(object sender, RoutedEventArgs e)
        {
            WriteToWindow(SerialOutbox.Text + '\n');

            //write to bound destinations
            SendCommandHandler?.Invoke(SerialOutbox.Text + '\n');
        }

        private void SerialClearButtonClick(object sender, RoutedEventArgs e)
        {
            SerialInbox.Text = String.Empty;
        }

    }
}
