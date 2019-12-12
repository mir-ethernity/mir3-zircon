using System;
using System.Collections.Generic;
using System.Text;

namespace Mir.ImageLibrary.BitmapConverter.Converters
{
    internal class BitmapConverterAttribute : Attribute
    {
        public ImageDataType Type { get; }

        public BitmapConverterAttribute(ImageDataType type)
        {
            Type = type;
        }
    }
}
