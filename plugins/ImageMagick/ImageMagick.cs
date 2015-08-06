#region usings
using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;

using ImageMagick;
using ImageMagick.Drawables;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "ImageMagick", Category = "ImageMagick", Help = "", Tags = "")]
	#endregion PluginInfo
	public class ImageMagickNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input", DefaultString = "hello c#")]
		public ISpread<string> FInText;

        [Input("Create", IsBang = true)]
        public IDiffSpread<bool> FInCreate;

        [Input("Path", StringType = StringType.Filename)]
        public ISpread<string> FInPath;

        [Output("Output")]
		public ISpread<string> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = SpreadMax;

            if (FInCreate.IsChanged)
            {
                for (int i = 0; i < SpreadMax; i++)
                {
                    if (FInCreate[i])
                    {
                        MagickImage image = CreateText(FInText[i]);
                        image.Write(FInPath[i]);
                    }
                }
            }

			//FLogger.Log(LogType.Debug, "Logging to Renderer (TTY)");
		}

        private MagickImage CreateText(string text)
        {
            System.Drawing.Bitmap bmp = new Bitmap(1000, 1000);
            MagickImage image = new MagickImage(bmp);
            image.BackgroundColor = new MagickColor(Color.Black);
            image.FillColor = new MagickColor(Color.White);
            image.TextGravity = Gravity.Center;
            image.FontPointsize = 50;
            DrawableText t = new DrawableText(0, 0, text);
            t.Text = "Test";
            image.Draw(t);
            image.Trim();

            return image;
        }
	}
}
