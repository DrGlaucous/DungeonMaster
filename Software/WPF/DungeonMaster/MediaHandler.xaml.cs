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
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for MediaHandler.xaml
    /// </summary>
    public partial class MediaHandler : UserControl
    {
        //keep the current state of the mediabuffer playback
        private bool MediaIsPaused = true;
        public bool PausedMedia
        {
            get
            {
                return MediaIsPaused;
            }
        }


        private bool MediaIsLoaded = false;
        public bool LoadedMedia
        {
            get {
                return MediaIsLoaded;
            }
        }


        public MediaHandler()
        {
            InitializeComponent();

            //set up AV player and add color key
            ColorKeyAlphaEffect effect = new();
            Brush brush = Effect.ImplicitInput;
            effect.Input = brush;
            MediaBuffer.Effect = effect;
            MediaBuffer.LoadedBehavior = MediaState.Manual;
        }

        //path should be a local directory without the leading "./".
        //returns true if load was success
        public bool LoadAVMedia(String path)
        {
            try
            {
                //unload old source first
                UnloadAVMedia();

                //make absolute path from relative one
                var abspath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), path);
                var ueri = new Uri(abspath);

                //MediaBuf1.Close(); //required to prevent the last frame of the old video from showing when we start the new video.
                MediaBuffer.Source = ueri;
                MediaBuffer.Visibility = Visibility.Visible;
                MediaIsLoaded = true;
            }
            catch { return false; }

            return true;
        }
        public void UnloadAVMedia() {
            try
            {
                //stop old source first
                StopAVMedia();
                MediaBuffer.Close();
            }
            catch { }
        }

        public bool StopAVMedia()
        {
            MediaBuffer.Stop();
            MediaIsPaused = true;
            return true;
        }

        public bool PauseAVMedia() {
            MediaBuffer.Pause();
            MediaIsPaused = true;
            return true;
        }
        public bool PlayAVMedia() {

            if (!MediaIsLoaded)
                return false;

            MediaBuffer.Play();
            MediaIsPaused = false;
            return true;
        }







    }
}
