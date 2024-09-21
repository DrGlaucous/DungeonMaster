using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static System.Net.Mime.MediaTypeNames;

namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public NumericUpDown()
        {
            InitializeComponent();

            Number = 8;
        }

        public int MinNumber { get; set; } = -100;
        public int MaxNumber { get; set; } = 100;


        //keeps "number" within the valid range
        int number = 0;
        public int Number {
            get
            {
                return number;
            }
            
            set
            {
                number = value;

                if (number > MaxNumber) {
                    number = MaxNumber;
                }
                if (number < MinNumber) {
                    number = MinNumber;
                }

                NumberInput.Text = number.ToString();
            }
        }


        //filters inputs to ints before we get them
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private void PreviewTextInput2(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }


        //converts text to a number
        private void GotText(object sender, TextChangedEventArgs e)
        {
            int holder = Number;
            try
            {
                holder = Int32.Parse(NumberInput.Text);
            }
            catch { }

            Number = holder;

        }

        private void MouseWheel2(object sender, MouseWheelEventArgs e)
        {
            //mouse moves in increments of 120
            Number = e.Delta / 120 + Number;
        }

        //checks for up and down arrow keys
        private void KeyDown2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) {
                Number += 1;
            }
            if (e.Key == Key.Down) {
                Number -= 1;
            }
        }

        //checks for clicking the UI up and down buttons
        private void UpArrow_Click(object sender, RoutedEventArgs e)
        {
            Number += 1;
        }

        private void DownArrow_Click(object sender, RoutedEventArgs e)
        {
            Number -= 1;
        }
    }
}
