using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mir.ImageLibrary.Zircon
{
    public class ZirconImageLibrary : IImageLibrary
    {
        private readonly Stream _stream;
        private readonly BinaryReader _reader;
        protected ZirconSelectorType[] _images;

        public string Name { get; protected set; }
        public int Length { get => _images?.Length ?? 0; }

        public IImageSelectorType this[int index]
        {
            get { return _images[index]; }
            internal set { _images[index] = (ZirconSelectorType)value; }
        }

        internal ZirconImageLibrary()
        {
            _images = new ZirconSelectorType[0];
            Name = string.Empty;
        }

        public ZirconImageLibrary(string name, Stream stream)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _reader = new BinaryReader(_stream);

            InitializeLibrary();
        }

        public ZirconImageLibrary(string zlPath)
            : this(Path.GetFileNameWithoutExtension(zlPath), new FileStream(zlPath, FileMode.Open, FileAccess.Read))
        {
        }

        public void Dispose()
        {
            _stream.Dispose();
            _reader.Dispose();
            _images = null;
        }

        private void InitializeLibrary()
        {
            if (_stream == null) return;

            _stream.Seek(0, SeekOrigin.Begin);

            var headerBufferSize = _reader.ReadInt32();
            var headerBuffer = _reader.ReadBytes(headerBufferSize);

            using (var ms = new MemoryStream(headerBuffer))
            using (var br = new BinaryReader(ms))
            {
                var count = br.ReadInt32();
                _images = new ZirconSelectorType[count];

                for (var i = 0; i < _images.Length; i++)
                {
                    if (!br.ReadBoolean()) continue;

                    ZirconImage image = null;
                    ZirconImage shadow = null;
                    ZirconImage overlay = null;

                    var dataType = ImageDataType.Dxt1;
                    var offset = br.ReadInt32();

                    var width = br.ReadUInt16();
                    var height = br.ReadUInt16();
                    var offsetX = br.ReadInt16();
                    var offsetY = br.ReadInt16();

                    var shadowTypeByte = br.ReadByte();
                    var shadowModificatorType = shadowTypeByte == 177 || shadowTypeByte == 176 || shadowTypeByte == 49
                        ? ModificatorType.Transform
                        : ModificatorType.Opacity;

                    var shadowWidth = br.ReadUInt16();
                    var shadowHeight = br.ReadUInt16();
                    var shadowOffsetX = br.ReadInt16();
                    var shadowOffsetY = br.ReadInt16();

                    var overlayWidth = br.ReadUInt16();
                    var overlayHeight = br.ReadUInt16();

                    image = new ZirconImage(offset, width, height, offsetX, offsetX, ModificatorType.None, dataType, _reader);

                    offset += ZirconImage.CalculateImageDataSize(width, height);

                    if ((shadowWidth > 0 && shadowHeight > 0) || shadowOffsetX != 0 && shadowOffsetY != 0)
                    {
                        shadow = shadowWidth == 0 || shadowHeight == 0
                            ? new ZirconImage(offsetX, offsetY, shadowModificatorType)
                            : new ZirconImage(offset, shadowWidth, shadowHeight, shadowOffsetX, shadowOffsetY, shadowModificatorType, dataType, _reader);

                        if (shadowWidth > 0 && shadowHeight > 0)
                            offset += ZirconImage.CalculateImageDataSize(shadowWidth, shadowHeight);
                    }

                    if (overlayWidth > 0 && overlayHeight > 0)
                    {
                        overlay = new ZirconImage(offset, overlayWidth, overlayHeight, offsetX, offsetY, ModificatorType.None, dataType, _reader);
                    }

                    _images[i] = new ZirconSelectorType(image, shadow, overlay);
                }
            }
        }
    }
}
