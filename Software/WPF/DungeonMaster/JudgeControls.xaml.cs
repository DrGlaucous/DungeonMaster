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
            ButtonId button_id;// = ButtonId.StartMatch;

            switch (button_name)
            {
                case "StartMatchButton":
                    {
                        button_id = ButtonId.StartMatch;
                        break;
                    }
                case "StopwatchButton":
                    {
                        button_id = ButtonId.Stopwatch;
                        break;
                    }
                case "PausePlayButton":
                    {
                        button_id = ButtonId.PausePlay;
                        break;
                    }
                case "RedWinsButton":
                    {
                        button_id = ButtonId.RedWins;
                        break;
                    }
                case "BlueWinsButton":
                    {
                        button_id = ButtonId.BlueWins;
                        break;
                    }
                case "EndMatchButton":
                    {
                        button_id = ButtonId.EndMatch;
                        break;
                    }
                default: { return; }
            }

            //assemble response
            //type: JC, UUID: -1, response type: buttonStatus
            string response = "<1:-1:1:" + ((int)button_id).ToString() + ' ' + (is_pressed? 'O' : 'F');
            SendResponseHandler?.Invoke(response);

        }

    }


}
