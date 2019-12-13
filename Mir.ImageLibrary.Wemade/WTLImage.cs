using System;
using System.Collections.Generic;
using System.Text;

namespace Mir.ImageLibrary.Wemade
{
    public class WTLImage : IImage
    {
        private byte[] _buffer;

        public bool HasData { get; }

        public ImageDataType DataType { get; }

        public ModificatorType Modificator { get; }

        public ushort Width { get; }

        public ushort Height { get; }

        public short OffsetX { get; }

        public short OffsetY { get; }

        public byte[] GetBuffer()
        {
            if (!HasData) throw new Exception();
            return _buffer;
        }

        public WTLImage(ushort width, ushort height, short offsetX, short offsetY, ModificatorType modificator, ImageDataType dataType, byte[] buffer)
        {
            HasData = true;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Modificator = modificator;
            DataType = dataType;
            _buffer = buffer;
        }

        public WTLImage(short offsetX, short offsetY, ModificatorType modificator)
        {
            HasData = false;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Modificator = modificator;
        }
    }
}
