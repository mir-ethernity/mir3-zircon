using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mir.ImageLibrary.Zircon
{
    public class ZirconImage : IImage
    {
        public const int HeaderSize = 40;

        private readonly BinaryReader _fileReader;
        private byte[] _buffer;

        public bool HasData { get; }
        public int ImageLength { get; }
        public ModificatorType Modificator { get; }

        public int Position { get; internal set; }

        public ushort Width { get; }

        public ushort Height { get; }

        public short OffsetX { get; internal set; }

        public short OffsetY { get; internal set; }

        public ImageDataType DataType { get; }

        public ZirconImage(short offsetX, short offsetY, ModificatorType modificator)
        {
            HasData = false;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Modificator = modificator;
        }

        public ZirconImage(int position, int imageLength, ushort width, ushort height, short offsetX, short offsetY, ModificatorType modificator, ImageDataType dataType, BinaryReader fileReader)
        {
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
            HasData = true;
            ImageLength = imageLength;
            Position = position;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Modificator = modificator;
            DataType = dataType;
        }

        public ZirconImage(ushort width, ushort height, short offsetX, short offsetY, ModificatorType modificator, ImageDataType dataType, byte[] buffer)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            ImageLength = _buffer.Length;
            Position = 0;
            HasData = true;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Modificator = modificator;
            DataType = dataType;
        }

        public byte[] GetBuffer()
        {
            var result = _buffer;
            if (result == null && _fileReader == null) throw new ApplicationException();

            if (result == null)
            {
                _fileReader.BaseStream.Seek(Position, SeekOrigin.Begin);
                result = _fileReader.ReadBytes(ImageLength);
            }

            return result;
        }
    }
}
