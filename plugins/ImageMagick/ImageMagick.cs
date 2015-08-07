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

using System.Runtime.InteropServices;

using SlimDX;
using SlimDX.Direct3D9;
using VVVV.PluginInterfaces.V2.EX9;
using VVVV.Utils.SlimDX;


//here you can change the vertex type
using VertexType = VVVV.Utils.SlimDX.TexturedVertex;

#endregion usings

namespace VVVV.Nodes
{

    #region PluginInfo
    [PluginInfo(Name = "Text",
            Category = "ImageMagick",
            Version = "",
            Help = "",
            Tags = "",
            AutoEvaluate = true)]
    #endregion PluginInfo
    public class TextImageMagick : IPluginEvaluate, IPartImportsSatisfiedNotification
    {
        #region fields & pins
        [Input("Input", DefaultString = "vvvv")]
        public IDiffSpread<string> FInText;
        private List<string> LastText;

        [Output("Output")]
        public ISpread<MagickImage> FOutput;

        [Import()]
        public ILogger FLogger;
        #endregion fields & pins

        public void OnImportsSatisfied()
        {
            //spreads have a length of one by default, change it
            //to zero so ResizeAndDispose works properly.
            FOutput.SliceCount = 0;
        }

        //called when data for any output pin is requested
        public void Evaluate(int SpreadMax)
        {
            if (FInText.IsChanged)
            {
                FOutput.SliceCount = SpreadMax;
                for (int i = 0; i < SpreadMax; i++)
                {
                    if (LastText == null)
                    {
                        MagickImage image = CreateText(FInText[i]);
                        FOutput[i] = image;
                    }
                    else if (FInText[i] != LastText[i])
                    {
                        MagickImage image = CreateText(FInText[i]);
                        FOutput[i] = image;
                    }
                    
                }
                LastText = new List<string>();
                for (int i = 0; i < SpreadMax; i++)
                {
                    LastText.Add(FInText[i]);
                }
            }

        }

        private MagickImage CreateText(string text)
        {
            MagickImage image = new MagickImage();

            image.FontPointsize = 50;
            DrawableText t = new DrawableText(0, 0, text);
            var metrics = image.FontTypeMetrics(t.Text);

            Bitmap bmp = new Bitmap((int)metrics.TextWidth, (int)metrics.TextHeight);
            MagickImage outimage = new MagickImage(bmp);
            outimage.BackgroundColor = new MagickColor(Color.Black);
            outimage.FillColor = new MagickColor(Color.White);
            outimage.TextGravity = Gravity.Center;
            outimage.FontPointsize = 50;
            outimage.Draw(t);

            outimage.Trim();

            return outimage;
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "ImageMagickTexture",
                Category = "ImageMagick EX9.Texture",
                Version = "",
                Help = "",
                Tags = "")]
    #endregion PluginInfo
    public class ImageMagickTexture : IPluginEvaluate, IPartImportsSatisfiedNotification
    {
        //little helper class used to store information for each
        //texture resource
        public class Info
        {
            public int Slice;
            public int Width;
            public int Height;
            public int HashCode;
        }

        [Input("Image")]
        public ISpread<MagickImage> FInImage;

        [Output("Texture Out")]
        public ISpread<TextureResource<Info>> FTextureOut;

        [Import]
        public ILogger FLogger;

        public void OnImportsSatisfied()
        {
            //spreads have a length of one by default, change it
            //to zero so ResizeAndDispose works properly.
            FTextureOut.SliceCount = 0;
        }

        //called when data for any output pin is requested
        public void Evaluate(int spreadMax)
        {
            FTextureOut.ResizeAndDispose(spreadMax, CreateTextureResource);
            for (int i = 0; i < spreadMax; i++)
            {
                var textureResource = FTextureOut[i];
                var info = textureResource.Metadata;
                //recreate textures if resolution was changed
                if (FInImage[i] == null)
                {
                    textureResource.Dispose();
                    textureResource = CreateTextureResource(i);
                }
                else if (info.Width != FInImage[i].Width || info.Height != FInImage[i].Height || info.HashCode != FInImage[i].GetHashCode())
                {
                    textureResource.Dispose();
                    textureResource = CreateTextureResource(i);
                }
                FTextureOut[i] = textureResource;
            }
        }

        TextureResource<Info> CreateTextureResource(int slice)
        {
            if (FInImage[slice] == null)
            {
                var emptyInfo = new Info() { Slice = slice, Width = 2, Height = 2, HashCode = 0 };
                return TextureResource.Create(emptyInfo, CreateEmptyTexture, null);
            }
            else
            {
                var info = new Info() { Slice = slice, Width = FInImage[slice].Width, Height = FInImage[slice].Height, HashCode = FInImage[slice].GetHashCode() };
                return TextureResource.Create(info, CreateTexture, UpdateTexture);
            }

        }

        //this method gets called, when Reinitialize() was called in evaluate,
        //or a graphics device asks for its data
        Texture CreateTexture(Info info, Device device)
        {
            FLogger.Log(LogType.Debug, "Creating new texture at slice: " + info.Slice);
            return TextureUtils.CreateTexture(device, Math.Max(info.Width, 1), Math.Max(info.Height, 1));
        }

        Texture CreateEmptyTexture(Info info, Device device)
        {
            return TextureUtils.CreateTexture(device, 2, 2);
        }

        //this method gets called, when Update() was called in evaluate,
        //or a graphics device asks for its texture, here you fill the texture with the actual data
        //this is called for each renderer, careful here with multiscreen setups, in that case
        //calculate the pixels in evaluate and just copy the data to the device texture here
        unsafe void UpdateTexture(Info info, Texture texture)
        {
            TextureUtils.CopyBitmapToTexture(FInImage[info.Slice].ToBitmap(), texture);
        }

    }
}






