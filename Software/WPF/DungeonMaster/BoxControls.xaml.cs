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
    /// Interaction logic for BoxControls.xaml
    /// </summary>
    public partial class BoxControls : UserControl
    {
        public event StringDelegate? SendResponseHandler;

        public BoxControls()
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
                case "RedReadyButton":
                    {
                        button_id = is_pressed ? ButtonEventNumbers.Jd : ButtonEventNumbers.Ju;
                        break;
                    }
                case "BlueReadyButton":
                    {
                        button_id = is_pressed ? ButtonEventNumbers.Id : ButtonEventNumbers.Iu;
                        break;
                    }
                default: { return; }
            }

            //assemble response
            //type: JC, UUID: -1, response type: buttonStatus
            string response = "<2:-1:1:" + ((int)button_id).ToString();
            SendResponseHandler?.Invoke(response);

        }

    }
}
