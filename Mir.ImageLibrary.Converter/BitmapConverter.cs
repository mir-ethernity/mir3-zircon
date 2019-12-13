using Mir.ImageLibrary.Converter.BitmapConverters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Mir.ImageLibrary.Converter
{
    public static class BitmapConverter
    {
        private static IDictionary<ImageDataType, IBitmapConverter> _converters;

        static BitmapConverter()
        {
            _converters = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => Attribute.IsDefined(x, typeof(BitmapConverterAttribute)))
                .ToDictionary(x => x.GetCustomAttribute<BitmapConverterAttribute>().Type, x => (IBitmapConverter)Activator.CreateInstance(x));
        }

        public static byte[] ConvertTextureToBitmap(ImageDataType bufferType, int width, int height, byte[] buffer)
        {
            if (!_converters.ContainsKey(bufferType))
                throw new NotImplementedException($"Data type {bufferType.ToString()} is not implemented");
            return _converters[bufferType].ConvertTextureToBitmap(width, height, buffer);
        }

        public static byte[] ConvertBitmapToTexture(ImageDataType bufferType, int width, int height, byte[] rgba)
        {
            if (!_converters.ContainsKey(bufferType))
                throw new NotImplementedException($"Data type {bufferType.ToString()} is not implemented");
            return _converters[bufferType].ConvertBitmapToTexture(width, height, rgba);
        }
    }
}
