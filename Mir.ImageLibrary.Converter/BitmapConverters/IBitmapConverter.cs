using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Mir.ImageLibrary.Converter.BitmapConverters
{
    public interface IBitmapConverter
    {
        byte[] ConvertTextureToBitmap(int width, int height, byte[] buffer);
        byte[] ConvertBitmapToTexture(int width, int height, byte[] rgba);
    }
}
