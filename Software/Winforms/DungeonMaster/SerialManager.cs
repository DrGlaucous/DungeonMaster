using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace DungeonMaster
{
    public partial class SerialManager : UserControl
    {
        private SerialPort serial_port = new SerialPort();
        //private bool has_connected = false; //used to flip-flop the "Connect" button status

        public SerialManager()
        {
            InitializeComponent();

            //cb example
            //public delegate void SerialDataReceivedEventHandler(object sender, System.IO.Ports.SerialDataReceivedEventArgs e);


            //if NULL, make a new one
            //serial_port ??= new SerialPort();

            serial_port.DataReceived += new SerialDataReceivedEventHandler(SerialGetCb);

            //EventHandler += new OnSerialGetEventHandler(test_event_handler);
            //EventHandler += new OnSerialGetEventHandler(test_event_handler2);


        }

        private void port_scan_btn_Click(object sender, EventArgs e)
        {
            var port_name_list = SerialPort.GetPortNames();

            port_list.SelectedIndex = -1;
            port_list.Items.Clear();

            foreach (var i in port_name_list)
            {
                port_list.Items.Add(i);
            }
        }

        private void connect_btn_Click(object sender, EventArgs e)
        {
            if (serial_port.IsOpen) //(has_connected)
            {
                serial_port.Close();
                //has_connected = false;
                connect_btn.Text = "Connect";
            }
            else
            {
                int baud_rate = 9600;

                if (int.TryParse(baud_list.Text, out int parsed))
                {
                    baud_rate = parsed;
                }

                serial_port.BaudRate = baud_rate;


                if (port_list.Text != String.Empty)
                {
                    serial_port.PortName = port_list.Text;
                }

                try
                {
                    serial_port.Open();
                    //has_connected = true;
                    connect_btn.Text = "Disconnect";
                }
                catch { } //do nothing for now

            }

        }

        //send string out to serial port, returns true if send was successful
        public bool SendString(String data) {

            if (serial_port.IsOpen) {

                try
                {
                    serial_port.Write(data);
                    return true;
                }
                catch { }
            }


            return false;
        }

        //other things to run when we get serial data (namely the terminal window)
        //see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/event
        public delegate void OnSerialGetEventHandler(string serial_data); //(object sender, OnSerialGetEventHandler e)
        public event OnSerialGetEventHandler? EventHandler;// { add { handler += value; } remove { handler -= value } }

        //runs whenever we get stuff back on the serial interface
        private void SerialGetCb(object sender, SerialDataReceivedEventArgs e) {


            //var len = serial_port.BytesToRead;
            //List<byte> buffer = new List<byte>();
            //buffer.Capacity = len > 0 ? len : 1;

            var serial_data = serial_port.ReadExisting();

            //?. is thread-safe invocation
            EventHandler?.Invoke(serial_data);

        }




    }
}
