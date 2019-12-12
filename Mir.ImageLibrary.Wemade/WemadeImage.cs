using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mir.ImageLibrary.Wemade
{
    public class WemadeImage : IImage
    {
        private byte[] _rgba;

        public ImageDataType DataType => ImageDataType.RGBA;
        public ModificatorType Modificator => ModificatorType.None;
        public ushort Width { get; }
        public ushort Height { get; }
        public short OffsetX { get; }
        public short OffsetY { get; }
        public bool HasData { get; }

        public WemadeImage(byte[] rgba, ushort width, ushort height, short offsetX, short offsetY)
        {
            _rgba = rgba;
            HasData = true;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public WemadeImage(short offsetX, short offsetY)
        {
            HasData = false;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public ImageData GetData()
        {
            if (!HasData) throw new Exception();
            return new ImageData
            {
                Type = ImageDataType.RGBA,
                Buffer = _rgba
            };
        }
    }
}
