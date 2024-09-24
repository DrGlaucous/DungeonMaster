using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Effects;
using System.Reflection;

namespace DungeonMaster
{
    class ColorKeyAlphaEffect : ShaderEffect
    {


        //The explict input for this pixel shader.
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ColorKeyAlphaEffect), 0);


        //A refernce to the pixel shader used.
        private static PixelShader pixelShader;


        //Creates an instance of the shader from the included pixel shader.
        static ColorKeyAlphaEffect()
        {
            pixelShader = new PixelShader();
            pixelShader.UriSource = MakePackUri("ShaderSource/ColorKeyAlpha.ps");
        }


        //Creates an instance and updates the shader's variables to the default values.
        public ColorKeyAlphaEffect()
        {
            this.PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
        }


        // Gets or sets the Input of shader.
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }


        //assemble uri from local packed directory
        public static Uri MakePackUri(string relativeFile)
        {
            Assembly a = typeof(MainWindow).Assembly;

            // Extract the short name.
            string assemblyShortName = a.ToString().Split(',')[0];

            string uriString = "pack://application:,,,/" +
                assemblyShortName +
                ";component/" +
                relativeFile;

            return new Uri(uriString);
        }


    }
}
