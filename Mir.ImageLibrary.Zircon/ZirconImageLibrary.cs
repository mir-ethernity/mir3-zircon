using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mir.ImageLibrary.Zircon
{
    public class ZirconImageLibrary : IImageLibrary
    {
        public ushort Version { get; protected set; }
        private Stream _stream;
        private BinaryReader _reader;
        protected IDictionary<ImageType, IImage>[] _images;

        public string Name { get; protected set; }
        public int Count { get => _images?.Length ?? 0; }

        public IDictionary<ImageType, IImage> this[int index]
        {
            get { return _images[index]; }
            internal set { _images[index] = value; }
        }

        internal ZirconImageLibrary()
        {
            _images = new IDictionary<ImageType, IImage>[0];
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
            _stream?.Dispose();
            _reader?.Dispose();
            _images = null;
            _stream = null;
            _reader = null;
        }

        private int GetImageLength(BinaryReader br, ushort width, ushort height)
        {
            if (Version > 0)
            {
                return br.ReadInt32();
            }
            else
            {
                if (width == 0 && height == 0) return 0;
                int w = width + (4 - width % 4) % 4;
                int h = height + (4 - height % 4) % 4;
                return (w * h) / 2;
            }
        }

        private void InitializeLibrary()
        {
            if (_stream == null) return;

            _stream.Seek(0, SeekOrigin.Begin);

            var libNameBuff = _reader.ReadBytes(6);
            var libName = Encoding.ASCII.GetString(libNameBuff);
            if (libName.Equals("ZIRCON"))
            {
                Version = _reader.ReadUInt16();
            }
            else
            {
                Version = 0;
                _stream.Seek(0, SeekOrigin.Begin);
            }

            var headerBufferSize = _reader.ReadInt32();
            var headerBuffer = _reader.ReadBytes(headerBufferSize);

            using (var ms = new MemoryStream(headerBuffer))
            using (var br = new BinaryReader(ms))
            {
                var count = br.ReadInt32();
                _images = new IDictionary<ImageType, IImage>[count];

                for (var i = 0; i < _images.Length; i++)
                {
                    if (!br.ReadBoolean()) continue;

                    ZirconImage image = null;
                    ZirconImage shadow = null;
                    ZirconImage overlay = null;

                    var offset = br.ReadInt32();

                    var width = br.ReadUInt16();
                    var height = br.ReadUInt16();
                    var offsetX = br.ReadInt16();
                    var offsetY = br.ReadInt16();
                    var dataType = ImageDataType.Dxt1;
                    if (Version > 0) dataType = (ImageDataType)br.ReadByte();
                    var imageLength = GetImageLength(br, width, height);

                    var shadowTypeByte = br.ReadByte();
                    var shadowModificatorType = shadowTypeByte == 177 || shadowTypeByte == 176 || shadowTypeByte == 49
                        ? ModificatorType.Transform
                        : (shadowTypeByte == 50 ? ModificatorType.Opacity : ModificatorType.None);

                    var shadowWidth = br.ReadUInt16();
                    var shadowHeight = br.ReadUInt16();
                    var shadowOffsetX = br.ReadInt16();
                    var shadowOffsetY = br.ReadInt16();
                    var shadowDataType = ImageDataType.Dxt1;
                    if (Version > 0) shadowDataType = (ImageDataType)br.ReadByte();
                    var shadowImageLength = GetImageLength(br, width, height) ;

                    var overlayWidth = br.ReadUInt16();
                    var overlayHeight = br.ReadUInt16();
                    var overlayDataType = ImageDataType.Dxt1;
                    if (Version > 0) overlayDataType = (ImageDataType)br.ReadByte();
                    var overlayImageLength = GetImageLength(br, overlayWidth, overlayHeight);


                    image = new ZirconImage(offset, imageLength, width, height, offsetX, offsetX, ModificatorType.None, dataType, _reader);

                    offset += imageLength;

                    if ((shadowWidth > 0 && shadowHeight > 0) || shadowOffsetX != 0 && shadowOffsetY != 0)
                    {
                        shadow = shadowWidth == 0 || shadowHeight == 0
                            ? new ZirconImage(offsetX, offsetY, shadowModificatorType)
                            : new ZirconImage(offset, shadowImageLength, shadowWidth, shadowHeight, shadowOffsetX, shadowOffsetY, shadowModificatorType, shadowDataType, _reader);

                        if (shadowWidth > 0 && shadowHeight > 0)
                        {
                            offset += shadowImageLength;
                        }
                    }

                    if (overlayWidth > 0 && overlayHeight > 0)
                        overlay = new ZirconImage(offset, overlayImageLength, overlayWidth, overlayHeight, offsetX, offsetY, ModificatorType.None, overlayDataType, _reader);


                    _images[i] = new Dictionary<ImageType, IImage>()
                    {
                        { ImageType.Image, image },
                    };

                    if (shadow != null)
                        _images[i].Add(ImageType.Shadow, shadow);

                    if (shadow != null)
                        _images[i].Add(ImageType.Overlay, overlay);
                }
            }
        }
    }
}
