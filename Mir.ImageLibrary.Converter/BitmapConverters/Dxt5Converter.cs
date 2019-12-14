
using Epsylon.TextureSquish;

namespace Mir.ImageLibrary.Converter.BitmapConverters
{
    [BitmapConverter(ImageDataType.Dxt5)]
    internal class Dxt5Converter : IBitmapConverter
    {
        public byte[] ConvertBitmapToTexture(int width, int height, byte[] rgba)
        {
            for (int i = 0; i < rgba.Length; i += 4)
            {
                //Reverse Red/Blue
                byte b = rgba[i];
                rgba[i] = rgba[i + 2];
                rgba[i + 2] = b;
            }

            var bitmap = new Bitmap(rgba, width, height);
            return bitmap.Compress(CompressionMode.Dxt5, CompressionOptions.None);
        }

        public byte[] ConvertTextureToBitmap(int width, int height, byte[] buffer)
        {
            var pixels = Epsylon.TextureSquish.Bitmap.Decompress(width, height, buffer, Epsylon.TextureSquish.CompressionMode.Dxt5).Data;
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
