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
            TestMediaElement.Source = new Uri("C:\\Users\\EdwardStuckey\\Documents\\GitHub\\DungeonMaster\\Software\\Tests\\Images\\Sauce.mp4");
            //TestMediaElement.Play();
        }

        private void Bttn_Click(object sender, RoutedEventArgs e)
        {
            TestPlayVideo();
            TestMediaElement.Play();
        }
    }
}
