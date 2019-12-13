using System;
using System.Collections.Generic;
using System.Text;

namespace Mir.ImageLibrary.Converter.BitmapConverters
{
    [BitmapConverter(ImageDataType.RGBA)]
    internal class RGBAConverter : IBitmapConverter
    {
        public byte[] ConvertBitmapToTexture(int width, int height, byte[] rgba)
        {
            return rgba;
        }

        public byte[] ConvertTextureToBitmap(int width, int height, byte[] buffer)
        {
            return buffer;
        }
    }
}
