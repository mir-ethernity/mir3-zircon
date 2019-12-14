using System;
using System.Collections.Generic;
using System.IO;

namespace Mir.ImageLibrary.Wemade
{
    public class WemadeImageLibrary : IImageLibrary
    {
        private int[] _palette;
        private readonly Stream _mainStream;
        private readonly BinaryReader _mainReader;
        private readonly Stream _indexStream;
        private byte _fileType;
        protected IDictionary<ImageType, IImage>[] _images;
        private int _version;

        public IDictionary<ImageType, IImage> this[int index] => _images[index];

        public string Name { get; private set; }
        public int Count => _images.Length;

        public WemadeImageLibrary(string name, byte fileType, Stream mainStream, Stream indexStream)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _fileType = fileType;
            _indexStream = indexStream ?? throw new ArgumentNullException(nameof(indexStream));
            _mainStream = mainStream ?? throw new ArgumentNullException(nameof(mainStream));
            _mainReader = new BinaryReader(_mainStream);
        }

        public WemadeImageLibrary(string path)
            : this(
                  Path.GetFileNameWithoutExtension(path),
                  GetFileType(path),
                  new FileStream(path, FileMode.Open, FileAccess.Read),
                  new FileStream(GetIndexPath(path), FileMode.Open, FileAccess.Read)
            )
        {
        }

        public void Dispose()
        {
            _mainStream.Dispose();
            _mainReader.Dispose();
            _indexStream.Dispose();
            _images = null;
        }

        public void Initialize()
        {
            _mainStream.Seek(0, SeekOrigin.Begin);

            byte[] buffer;
            _palette = new int[256] { -16777216, -8388608, -16744448, -8355840, -16777088, -8388480, -16744320, -4144960, -11173737, -6440504, -8686733, -13817559, -10857902, -10266022, -12437191, -14870504, -15200240, -14084072, -15726584, -886415, -2005153, -42406, -52943, -2729390, -7073792, -7067368, -13039616, -9236480, -4909056, -4365486, -12445680, -21863, -10874880, -9225943, -5944783, -7046285, -4369871, -11394800, -8703720, -13821936, -7583183, -7067392, -4378368, -3771566, -9752296, -3773630, -3257856, -5938375, -10866408, -14020608, -15398912, -12969984, -16252928, -14090240, -11927552, -6488064, -2359296, -2228224, -327680, -6524078, -7050422, -9221591, -11390696, -7583208, -7846895, -11919104, -14608368, -2714534, -3773663, -1086720, -35072, -5925756, -12439263, -15200248, -14084088, -14610432, -13031144, -7576775, -12441328, -9747944, -8697320, -7058944, -7568261, -9739430, -11910599, -14081768, -12175063, -4872812, -8688806, -3231340, -5927821, -7572646, -4877197, -2710157, -1071798, -1063284, -8690878, -9742791, -4352934, -10274560, -2701651, -11386327, -7052520, -1059155, -5927837, -10266038, -4348549, -10862056, -4355023, -13291223, -7043997, -8688822, -5927846, -10859991, -6522055, -12439280, -1069791, -15200256, -14081792, -6526208, -7044006, -11386344, -9741783, -8690911, -6522079, -2185984, -10857927, -13555440, -3228293, -10266055, -7044022, -3758807, -15688680, -12415926, -13530046, -15690711, -16246768, -16246760, -16242416, -15187415, -5917267, -9735309, -15193815, -15187382, -13548982, -10238242, -12263937, -7547153, -9213127, -532935, -528500, -530688, -9737382, -10842971, -12995089, -11887410, -13531979, -13544853, -2171178, -4342347, -7566204, -526370, -16775144, -16246727, -16248791, -16246784, -16242432, -16756059, -16745506, -15718070, -15713941, -15707508, -14591323, -15716006, -15711612, -13544828, -15195855, -11904389, -11375707, -14075549, -15709474, -14079711, -11908551, -14079720, -11908567, -8684734, -6513590, -10855895, -12434924, -13027072, -10921728, -3525332, -9735391, -14077696, -13551344, -13551336, -12432896, -11377896, -10849495, -13546984, -15195904, -15191808, -15189744, -10255286, -9716406, -10242742, -10240694, -10838966, -11891655, -10238390, -10234294, -11369398, -13536471, -10238374, -11354806, -15663360, -15193832, -11892662, -11868342, -16754176, -16742400, -16739328, -16720384, -16716288, -16712960, -11904364, -10259531, -8680234, -9733162, -8943361, -3750194, -7039844, -6515514, -13553351, -14083964, -15204220, -11910574, -11386245, -10265997, -3230217, -7570532, -8969524, -2249985, -1002454, -2162529, -1894477, -1040, -6250332, -8355712, -65536, -16711936, -256, -16776961, -65281, -16711681, -1, };

            if (_fileType == 0) //at least we know it's a .wil file up to now
            {
                buffer = _mainReader.ReadBytes(48);
                _fileType = (byte)(buffer[26] == 64 ? 2 : buffer[2] == 73 ? 3 : _fileType);

                if (_fileType == 0)
                {
                    _palette = new int[_mainReader.ReadInt32()];
                    _mainStream.Seek(4, SeekOrigin.Current);
                    _version = _mainReader.ReadInt32();
                    _mainStream.Seek(_version == 0 ? 0 : 4, SeekOrigin.Current);
                    for (int i = 1; i < _palette.Length; i++)
                        _palette[i] = _mainReader.ReadInt32() + (255 << 24);
                }
            }

            LoadIndexFile();
        }

        private void LoadIndexFile()
        {
            _indexStream.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(_indexStream);
            switch (_fileType)
            {
                case 4:
                    _indexStream.Seek(24, SeekOrigin.Begin);
                    break;
                case 3:
                    reader.ReadBytes(26);
                    if (reader.ReadUInt16() != 0xB13A)
                        _indexStream.Seek(24, SeekOrigin.Begin);
                    break;
                case 2:
                    reader.ReadBytes(52);
                    break;
                default:
                    reader.ReadBytes(_version == 0 ? 48 : 52);
                    break;
            }

            var count = (int)((reader.BaseStream.Length - reader.BaseStream.Position) / 4);
            _images = new Dictionary<ImageType, IImage>[count];

            for (var i = 0; i < _images.Length; i++)
            {
                var position = reader.ReadInt32();
                CheckImage(i, position);
            }
        }

        private void CheckImage(int index, int position)
        {
            _mainStream.Seek(position, SeekOrigin.Begin);

            var bo16bit = false;
            if (_fileType == 1)
            {
                bo16bit = (_mainReader.ReadByte() == 5 ? true : false);
                _mainReader.ReadBytes(3);
            }

            var width = _mainReader.ReadInt16();
            var height = _mainReader.ReadInt16();
            var offsetX = _mainReader.ReadInt16();
            var offsetY = _mainReader.ReadInt16();
            var size = width * height;
            var hasShadow = false;
            short shadowOffsetX = 0;
            short shadowOffsetY = 0;

            switch (_fileType)
            {
                case 1:
                    size = _mainReader.ReadInt32();
                    break;

                case 4:
                    bo16bit = true;
                    size = _mainReader.ReadInt32();
                    break;

                case 2:
                    bo16bit = true;
                    _mainReader.ReadInt16();
                    _mainReader.ReadInt16();
                    size = _mainReader.ReadInt32();
                    width = (size < 6) ? (short)0 : width;
                    break;

                case 3:
                    bo16bit = true;
                    hasShadow = _mainReader.ReadByte() == 1 ? true : false;
                    shadowOffsetX = _mainReader.ReadInt16();
                    shadowOffsetY = _mainReader.ReadInt16();
                    size = _mainReader.ReadInt32() * 2;
                    break;
            }
            width = (size == 0) ? (short)0 : width; //this makes sure blank images aren't being processed

            if (width == 0) return;

            int dataPosition = (int)_mainStream.Position;

            CreateImages(index, dataPosition, size, bo16bit, width, height, offsetX, offsetY, hasShadow, shadowOffsetX, shadowOffsetY);
        }

        private void CreateImages(int index, int dataPosition, int size, bool is16bit, short width, short height, short offsetX, short offsetY, bool hasShadow, short shadowOffsetX, short shadowOffsetY)
        {
            switch (_fileType)
            {
                case 0:
                    CreateWemadeUncompressed(index, dataPosition, size, is16bit, width, height, offsetX, offsetY, hasShadow, shadowOffsetX, shadowOffsetY);
                    break;
                case 2:
                    CreateUnknown1(index, dataPosition, size, is16bit, width, height, offsetX, offsetY, hasShadow, shadowOffsetX, shadowOffsetY);
                    break;
                case 1://shanda wzl file compressed
                case 4://shanda miz file compressed
                    CreateShandaCompressed(index, dataPosition, size, is16bit, width, height, offsetX, offsetY, hasShadow, shadowOffsetX, shadowOffsetY);
                    break;
                case 3:
                    CreateWemadeMir3Images(index, dataPosition, size, is16bit, width, height, offsetX, offsetY, hasShadow, shadowOffsetX, shadowOffsetY);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private unsafe byte[] DecompressPallete(byte[] bytes, short width, short height, bool is16bit)
        {
            var result = new byte[width * height * 4];
            int index = 0;
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    if (is16bit)
                    {
                        var tmp = BitConverter.GetBytes(Convert16bitTo32bit(bytes[index++] + (bytes[index++] << 8)));
                        Array.Copy(tmp, 0, result, y * width + x, 4);
                    }
                    else
                    {
                        var tmp = BitConverter.GetBytes(_palette[bytes[index++]]);
                        Array.Copy(tmp, 0, result, y * width + x, 4);
                    }
                }
                if (((_fileType == 1) || (_fileType == 4)) & (width % 4 > 0))
                    index += WidthBytes(is16bit ? 16 : 8, width) - (width * (is16bit ? 2 : 1));
            }
            return result;
        }

        private int Convert16bitTo32bit(int color)
        {
            byte red = (byte)((color & 0xf800) >> 8);
            byte green = (byte)((color & 0x07e0) >> 3);
            byte blue = (byte)((color & 0x001f) << 3);
            return ((red << 0x10) | (green << 0x8) | blue) | (255 << 24);//the final or is setting alpha to max so it'll display (since mir2 images have no alpha layer)
        }

        private int WidthBytes(int nBit, int nWidth)
        {
            return (((nWidth * nBit) + 31) >> 5) * 4;
        }

        private void CreateUnknown1(int index, int dataPosition, int size, bool is16bit, short width, short height, short offsetX, short offsetY, bool hasShadow, short shadowOffsetX, short shadowOffsetY)
        {
            byte Compressed = _mainReader.ReadByte();
            _mainReader.ReadBytes(5);
            byte[] bytes;
            if (Compressed != 8)
            {
                bytes = _mainReader.ReadBytes(size - 6);
            }
            else
            {

                bytes = _mainReader.ReadBytes(size - 6);
                bytes = Ionic.Zlib.DeflateStream.UncompressBuffer(bytes);
            }

            var image = new WemadeImage(DecompressPallete(bytes, width, height, is16bit), (ushort)width, (ushort)height, offsetX, offsetY);
            _images[index] = new Dictionary<ImageType, IImage>()
            {
                { ImageType.Image, image }
            };
        }

        private void CreateShandaCompressed(int index, int dataPosition, int size, bool is16bit, short width, short height, short offsetX, short offsetY, bool hasShadow, short shadowOffsetX, short shadowOffsetY)
        {
            using (var output = new MemoryStream())
            using (var deflateStream = new Ionic.Zlib.ZlibStream(output, Ionic.Zlib.CompressionMode.Decompress))
            {
                deflateStream.Write(_mainReader.ReadBytes(size), 0, size);
                var bytes = output.ToArray();
                var image = new WemadeImage(DecompressPallete(bytes, width, height, is16bit), (ushort)width, (ushort)height, offsetX, offsetY);
                _images[index] = new Dictionary<ImageType, IImage>()
                {
                    { ImageType.Image, image }
                };
            }
        }

        private void CreateWemadeUncompressed(int index, int dataPosition, int size, bool is16bit, short width, short height, short offsetX, short offsetY, bool hasShadow, short shadowOffsetX, short shadowOffsetY)
        {
            if (_palette.Length > 256)
            {
                is16bit = true;
                size = size * 2;
            }
            var bytes = _mainReader.ReadBytes(size);
            var image = new WemadeImage(DecompressPallete(bytes, width, height, is16bit), (ushort)width, (ushort)height, offsetX, offsetY);

            _images[index] = new Dictionary<ImageType, IImage>()
            {
                { ImageType.Image, image }
            };
        }

        private void CreateWemadeMir3Images(int index, int dataPosition, int size, bool is16bit, short width, short height, short offsetX, short offsetY, bool hasShadow, short shadowOffsetX, short shadowOffsetY)
        {
            var hasMask = false;

            byte[][] pixels = new byte[2][];
            pixels[0] = new byte[width * height * 2];
            pixels[1] = new byte[width * height * 2];
            byte[] buffer = _mainReader.ReadBytes(size * 2);
            int end = 0, start = 0, count;

            int nX, x = 0;
            for (int Y = height - 1; Y >= 0; Y--)
            {
                int OffSet = start * 2;
                end += buffer[OffSet];
                start++;
                nX = start;
                OffSet += 2;
                while (nX < end)
                {
                    switch (buffer[OffSet])
                    {
                        case 192: //No Colour
                            nX += 2;
                            x += buffer[OffSet + 3] << 8 | buffer[OffSet + 2];
                            OffSet += 4;
                            break;

                        case 193:  //Solid Colour
                        case 195:
                            nX += 2;
                            count = buffer[OffSet + 3] << 8 | buffer[OffSet + 2];
                            OffSet += 4;
                            for (int i = 0; i < count; i++)
                            {
                                pixels[0][(Y * width + x) * 2] = buffer[OffSet];
                                pixels[0][(Y * width + x) * 2 + 1] = buffer[OffSet + 1];
                                OffSet += 2;
                                if (x >= width) continue;
                                x++;
                            }
                            nX += count;
                            break;

                        case 194:  //Overlay Colour
                            hasMask = true;
                            nX += 2;
                            count = buffer[OffSet + 3] << 8 | buffer[OffSet + 2];
                            OffSet += 4;
                            for (int i = 0; i < count; i++)
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    pixels[j][(Y * width + x) * 2] = buffer[OffSet];
                                    pixels[j][(Y * width + x) * 2 + 1] = buffer[OffSet + 1];
                                }
                                OffSet += 2;
                                if (x >= width) continue;
                                x++;
                            }
                            nX += count;
                            break;
                    }
                }
                end++;
                start = end;
                x = 0;
            }

            var image = new WemadeImage(DecompressPallete(pixels[0], width, height, is16bit), (ushort)width, (ushort)height, offsetX, offsetY);
            var shadow = hasShadow ? new WemadeImage(shadowOffsetX, shadowOffsetY) : null;
            var mask = hasMask ? new WemadeImage(DecompressPallete(pixels[1], width, height, is16bit), (ushort)width, (ushort)height, offsetX, offsetY) : null;

            _images[index] = new Dictionary<ImageType, IImage>()
            {
                { ImageType.Image, image },
            };

            if (shadow != null)
                _images[index].Add(ImageType.Shadow, shadow);

            if (shadow != null)
                _images[index].Add(ImageType.Overlay, mask);
        }

        internal static byte GetFileType(string path)
        {
            var ext = Path.GetExtension(path).ToUpperInvariant();
            switch (ext)
            {
                case ".WIZ":
                    return 1;
                case ".MZL":
                    return 4;
                default:
                    throw new NotImplementedException();
            }
        }

        internal static string GetIndexPath(string path)
        {
            var type = GetFileType(path);
            switch (type)
            {
                case 0:
                    return Path.ChangeExtension(path, ".WZX");
                case 4:
                    return Path.ChangeExtension(path, ".MIX");
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
