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
using System.IO.Ports;

namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for SerialManager.xaml
    /// Manages all outgoing and incoming serial connections
    /// </summary>
    public partial class SerialManager : UserControl
    {
        //other things to run when we get serial data (namely the terminal window)
        //see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/event
        public event StringDelegate? DataGetEventHandler;// { add { handler += value; } remove { handler -= value } }

        private SerialPort SerialPort = new SerialPort();

        public SerialManager()
        {
            InitializeComponent();

            SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialGetCb);
            SerialPort.WriteTimeout = 1000;
            SerialPort.ReadTimeout = 1000;

        }

        private void PortScanButtonClicked(object sender, RoutedEventArgs e)
        {
            var port_name_list = SerialPort.GetPortNames();

            PortList.SelectedIndex = -1;
            PortList.Items.Clear();

            foreach (var i in port_name_list)
            {
                PortList.Items.Add(i);
            }
        }

        private void ConnectButtonClicked(object sender, RoutedEventArgs e)
        {
            if (SerialPort.IsOpen) //(has_connected)
            {
                SerialPort.Close();
                //has_connected = false;
                ConnectButton.Content = "Connect";
            }
            else
            {
                int baud_rate = 115200;

                if (int.TryParse(BaudList.Text, out int parsed))
                {
                    baud_rate = parsed;
                }

                SerialPort.BaudRate = baud_rate;


                if (PortList.Text != String.Empty)
                {
                    SerialPort.PortName = PortList.Text;
                }

                try
                {
                    SerialPort.Open();
                    //has_connected = true;
                    ConnectButton.Content = "Disconnect";
                }
                catch { } //do nothing for now

            }

        }

        //send string out to serial port, returns true if send was successful
        public void SendString(String data)
        {

            if (SerialPort.IsOpen)
            {

                try
                {
                    SerialPort.Write(data);
                    //return true;
                }
                catch {
                
                }
            }


            //return false;
        }


        //runs whenever we get stuff back on the serial interface
        private void SerialGetCb(object sender, SerialDataReceivedEventArgs e)
        {


            //var len = serial_port.BytesToRead;
            //List<byte> buffer = new List<byte>();
            //buffer.Capacity = len > 0 ? len : 1;

            var serial_data = SerialPort.ReadExisting();

            //?. is thread-safe invocation
            DataGetEventHandler?.Invoke(serial_data);

        }


    }
}
