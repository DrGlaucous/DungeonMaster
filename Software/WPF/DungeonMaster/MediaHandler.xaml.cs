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
                MediaIsLoaded = false;

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


        //alert tie-ins that the media has finished loading and can be played immedately
        private void DoneLoading(object sender, RoutedEventArgs e) {
            MediaIsLoaded = true;
            BufferStartCallback?.Invoke(); //breakout of of TSC wait
        }
        private void ErrorHappened(object? sender, ExceptionRoutedEventArgs e)
        {
            //todo: implement
        }







    }
}
