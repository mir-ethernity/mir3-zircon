using System;
using System.Collections.Generic;
using System.Text;

namespace Mir.ImageLibrary.Converter.BitmapConverters
{
    [BitmapConverter(ImageDataType.Dxt3)]
    internal class Dxt3Converter : IBitmapConverter
    {
        public byte[] ConvertBitmapToTexture(int width, int height, byte[] rgba)
        {
            var fixedColors = new byte[rgba.Length];
            Array.Copy(rgba, 0, fixedColors, 0, fixedColors.Length);

            for (int i = 0; i < rgba.Length; i += 4)
            {
                //Reverse Red/Blue
                byte b = fixedColors[i];
                fixedColors[i] = fixedColors[i + 2];
                fixedColors[i + 2] = b;
            }

            var bitmap = new Epsylon.TextureSquish.Bitmap(fixedColors, width, height);
            return bitmap.Compress(Epsylon.TextureSquish.CompressionMode.Dxt1, Epsylon.TextureSquish.CompressionOptions.None);
        }

        public byte[] ConvertTextureToBitmap(int width, int height, byte[] buffer)
        {
            var pixels = Epsylon.TextureSquish.Bitmap.Decompress(width, height, buffer, Epsylon.TextureSquish.CompressionMode.Dxt1).Data;

            for (int i = 0; i < pixels.Length; i += 4)
            {
                //Reverse Red/Blue
                byte b = pixels[i];
                pixels[i] = pixels[i + 2];
                pixels[i + 2] = b;
            }
            return pixels;
        }
    }
}
