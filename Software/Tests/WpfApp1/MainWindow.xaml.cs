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
using RJCP.IO.Ports;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPortStream sps = new SerialPortStream();
       // private SerialPort sp = new SerialPort();

        //when we get new data
        public delegate void StringDelegate(string arg);
        //public event StringDelegate? DataGetEventHandler;

        public MainWindow()
        {
            InitializeComponent();

            sps.DataReceived += new EventHandler<SerialDataReceivedEventArgs>(SerialGetCb);


            int baud = 115200;
            sps.BaudRate = baud;
            sps.PortName = "COM3";
            sps.Open();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            sps.Write("<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA<AAA");
        }


        private void SerialGetCb(object? sender, SerialDataReceivedEventArgs e) {
            string data = sps.ReadExisting();
            WriteToWindow(data);
        }

        private void WriteToWindow(string new_text) {

            if (!Dispatcher.CheckAccess()) {
                Dispatcher.Invoke(new StringDelegate(WriteToWindow), new_text);
                return;
            }

            //normalize carriage returns
            new_text = new_text.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

            //get length for size buffering
            var more_len = new_text.Length;
            var curr_len = SerialInbox.Text.Length;

            SerialInbox.Text += new_text; //add text (edge case: could potentially run out of memory here!)

            //if > 0, we've added too much, crop to length
            var len_diff = curr_len + more_len - 2000;
            if (len_diff > 0)
            {
                SerialInbox.Text = SerialInbox.Text[len_diff..];
            }

            ScrollContainer.ScrollToBottom();

        }

    }


}