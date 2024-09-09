using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DirectShowLib;
//using Windows.Media.Playback

namespace DungeonMaster
{
    public partial class StreamOverlay : Form
    {
        public StreamOverlay()
        {
            InitializeComponent();
        }




        private IGraphBuilder graphBuilder;
        private IMediaControl mediaControl;
        private IVMRWindowlessControl9 vmrWindowlessControl;

        public void method(string videoFilePath, IntPtr handle) {

            //graphBuilder = (IGraphBuilder)new FilterGraph();
            //mediaControl = (IMediaControl)graphBuilder;

            //// Add the VMR9 Renderer to the Graph
            //IBaseFilter videoRenderer = (IBaseFilter)new VideoMixingRenderer9();
            //graphBuilder.AddFilter(videoRenderer, "Video Renderer");

            //// Configure VMR9 for windowless mode and alpha channel rendering
            //vmrWindowlessControl = (IVMRWindowlessControl9)videoRenderer;
            //vmrWindowlessControl.SetVideoClippingWindow(handle);
            //vmrWindowlessControl.SetAspectRatioMode(VMR9AspectRatioMode.LetterBox);


            //// Load the video into the Graph
            //graphBuilder.RenderFile(videoFilePath, null);

            //// Run the video
            //mediaControl.Run();



        }
    }
}
