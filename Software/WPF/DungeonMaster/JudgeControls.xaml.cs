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
using static DungeonMaster.ResponseEngine;

namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for JudgeControls.xaml
    /// </summary>
    public partial class JudgeControls : UserControl
    {
        public event StringDelegate? SendResponseHandler;


        public JudgeControls()
        {
            InitializeComponent();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var as_button = sender as Button;
            if (as_button != null)
            {
                HandleButtonEvent(as_button.Name, true);
            }
        }

        private void ButtonRelease(object sender, MouseButtonEventArgs e)
        {
            var as_button = sender as Button;
            if (as_button != null)
            {
                HandleButtonEvent(as_button.Name, false);
            }
        }



        private void HandleButtonEvent(string button_name, bool is_pressed)
        {
            ButtonEventNumbers button_id;// = ButtonId.StartMatch;

            switch (button_name)
            {
                case "A":
                    {
                        button_id = is_pressed ? ButtonEventNumbers.Ad : ButtonEventNumbers.Au;
                        break;
                    }
                case "B":
                    {
                        button_id = is_pressed ? ButtonEventNumbers.Bd : ButtonEventNumbers.Bu;
                        break;
                    }
                case "C":
                    {
                        button_id = is_pressed ? ButtonEventNumbers.Cd : ButtonEventNumbers.Cu;
                        break;
                    }
                case "D":
                    {
                        button_id = is_pressed ? ButtonEventNumbers.Dd : ButtonEventNumbers.Du;
                        break;
                    }
                case "E":
                    {
                        button_id = is_pressed ? ButtonEventNumbers.Ed : ButtonEventNumbers.Eu;
                        break;
                    }
                case "F":
                    {
                        button_id = is_pressed ? ButtonEventNumbers.Fd : ButtonEventNumbers.Fu;
                        break;
                    }
                case "G":
                    {
                        button_id = is_pressed ? ButtonEventNumbers.Gd : ButtonEventNumbers.Gu;
                        break;
                    }
                case "H":
                    {
                        button_id = is_pressed ? ButtonEventNumbers.Hd : ButtonEventNumbers.Hu;
                        break;
                    }
                default: { return; }
            }

            //assemble response
            //type: JC, UUID: -1, response type: buttonStatus
            string response = "<1:-1:1:" + ((int)button_id).ToString();
            SendResponseHandler?.Invoke(response);

        }

    }


}
