using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for StreamOverlay.xaml
    /// </summary>
    public partial class StreamOverlay : Window
    {
        public StreamOverlay()
        {
            InitializeComponent();
        }

        public void TestPlayVideo()
        {
            ColorKeyAlphaEffect effect = new ColorKeyAlphaEffect();
            Brush brush = Effect.ImplicitInput;
            effect.Input = brush;

            TestMediaElement.Effect = effect;
            TestMediaElement.LoadedBehavior = MediaState.Manual;

            //make absolute path from relative one
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Sauce.mp4");
            var ueri = new Uri(path);

            TestMediaElement.Source = ueri;
            //TestMediaElement.Play();
            

        }

        private void Bttn_Click(object sender, RoutedEventArgs e)
        {
            TestPlayVideo();
            TestMediaElement.Play();
        }

        //close everything with this window
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }


    }
}
