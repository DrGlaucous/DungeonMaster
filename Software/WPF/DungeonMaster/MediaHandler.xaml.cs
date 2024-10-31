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

using LibVLCSharp.Shared;
using LibVLCSharp.WPF;

namespace DungeonMaster
{
    /// <summary>
    /// Interaction logic for MediaHandler.xaml
    /// </summary>
    public partial class MediaHandler : UserControl
    {

        LibVLC _libVLC;
        LibVLCSharp.Shared.MediaPlayer _mediaPlayer;

        public event VoidDelegate? BufferStartCallback;

        //keep the current state of the mediabuffer playback
        //not needed with the VLC backend
        private bool MediaIsPaused = true;
        public bool PausedMedia
        {
            get
            {
                //return !VLCMediaBuffer.MediaPlayer.IsPlaying;
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
            ColorKeyWrapper.Effect = effect;


            MediaBuffer.LoadedBehavior = MediaState.Manual;
            MediaBuffer.MediaOpened += DoneLoading; //called once the media is loaded into RAM (I would prefer to use "BufferingStarted", but that doesn't seem to work)
            MediaBuffer.MediaFailed += ErrorHappened;


            //initialize the VLC elements
            //_libVLC = new LibVLC();
            //_mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(_libVLC);
            //VLCMediaBuffer.Loaded += (sender, e) => VLCMediaBuffer.MediaPlayer = _mediaPlayer;
            //Unloaded += UnloadControl;
            //_mediaPlayer.Playing += MMXT; //RoutedEventHandler(MediaStartedCallback);

        }


        //methods for the VLC player (which doesn't support color keying)
        /*
        //alert tie-ins that the media has actually started playing
        private void MMXT(object? sender, EventArgs e)
        {
            for (int i = 0; i < 10; ++i)
            {
                var tt = 9 + i;
            }

            BufferStartCallback?.Invoke();
        }

        //called when the overlay is closed
        private void UnloadControl(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Dispose();
            _libVLC.Dispose();
        }
        */


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

                MediaBuffer.Source = ueri;
                MediaBuffer.Visibility = Visibility.Visible;

                //var media = new Media(_libVLC, ueri);
                //VLCMediaBuffer.MediaPlayer.Media = media; //todo: fix this warning


                //from example:
                //if (!VLCMediaBuffer.MediaPlayer.IsPlaying)
                //{
                //    using (var media = new Media(_libVLC, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4")))
                //        VLCMediaBuffer.MediaPlayer.Play(media);
                //}

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

            //VLCMediaBuffer.MediaPlayer.Stop();

            return true;
        }

        public bool PauseAVMedia() {
            MediaBuffer.Pause();
            MediaIsPaused = true;

            //VLCMediaBuffer.MediaPlayer.Pause();


            return true;
        }
        public bool PlayAVMedia() {


            if (!MediaIsLoaded)
                return false;
            MediaBuffer.Play();
            MediaIsPaused = false;


            //VLCMediaBuffer.MediaPlayer.Play();


            return true;
        }


        //alert tie-ins that the media has actually started playing
        private void DoneLoading(object sender, RoutedEventArgs e) {
            MediaIsLoaded = true;
            BufferStartCallback?.Invoke();            
        }
        private void ErrorHappened(object? sender, ExceptionRoutedEventArgs e)
        {
            //todo: implement
        }







    }
}
