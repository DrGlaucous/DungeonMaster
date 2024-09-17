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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Net.Mime.MediaTypeNames;

namespace DungeonMaster
{
    public partial class Terminal : UserControl
    {
        //private string window_buffer;

        public Terminal()
        {
            InitializeComponent();

            serial_inbox.MaxLength = 5000; //programatically added text is not affected by this, but we use it for refrence
        }

        public void write_to_window(string new_text)
        {


            //update hande from outside threads
            if (serial_inbox.InvokeRequired)
            {
                serial_inbox.Invoke(new MethodInvoker(() => { write_to_window(new_text); }));
                return;
            }


            //normal stuff

            //normalize carriage returns
            new_text = new_text.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

            //get length for size buffering
            var more_len = new_text.Length;
            var curr_len = serial_inbox.Text.Length;

            //serial_inbox.Text += new_text; //add text (edge case: could potentially run out of memory here!)
            serial_inbox.AppendText(new_text);

            //if > 0, we've added too much, crop to length
            var len_diff = curr_len + more_len - serial_inbox.MaxLength;
            if (len_diff > 0)
            {
                serial_inbox.Text = serial_inbox.Text[len_diff..];

                //move cursor back to the bottom and scroll to it
                serial_inbox.SelectionStart = serial_inbox.TextLength;
                serial_inbox.ScrollToCaret();
            }




        }

        private void serial_send_button_Click(object sender, EventArgs e)
        {
            write_to_window(serial_outbox.Text);

            //write to bound destinations
            EventHandler?.Invoke(serial_outbox.Text);
        }

        private void serial_clear_button_Click(object sender, EventArgs e)
        {
            serial_inbox.Text = String.Empty;
        }


        public delegate bool OnSerialSendEventHandler(string out_text); //(object sender, OnSerialGetEventHandler e)
        public event OnSerialSendEventHandler EventHandler;// { add { handler += value; } remove { handler -= value } }


    }
}
