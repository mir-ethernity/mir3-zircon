using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mir.ImageLibrary.Zircon
{
    public class ZirconImage : IImage
    {
        private readonly BinaryReader _fileReader;
        private byte[] _buffer;

        public bool HasData { get; }
        public ModificatorType Modificator { get; }

        public int Position { get; internal set; }

        public ushort Width { get; }

        public ushort Height { get; }

        public short OffsetX { get; internal set; }

        public short OffsetY { get; internal set; }

        public ImageDataType DataType { get; }
        public static int HeaderSize { get; internal set; }

        public ZirconImage(short offsetX, short offsetY, ModificatorType modificator)
        {
            HasData = false;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Modificator = modificator;
        }

        public ZirconImage(int position, ushort width, ushort height, short offsetX, short offsetY, ModificatorType modificator, ImageDataType dataType, BinaryReader fileReader)
        {
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
            HasData = true;
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
            Position = 0;
            HasData = true;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Modificator = modificator;
            DataType = dataType;
        }

        public ImageData GetData()
        {
            var size = CalculateImageDataSize(Width, Height);

            if (_buffer == null && _fileReader == null) throw new ApplicationException();

            if (_buffer == null)
            {
                _fileReader.BaseStream.Seek(Position, SeekOrigin.Begin);
                _buffer = _fileReader.ReadBytes(size);
            }

            return new ImageData { Buffer = _buffer, Type = ImageDataType.Dxt1 };
        }

        internal static int CalculateImageDataSize(ushort width, ushort height)
        {
            int w = width + (4 - width % 4) % 4;
            int h = height + (4 - height % 4) % 4;
            return (w * h) / 2;
        }
    }
}
