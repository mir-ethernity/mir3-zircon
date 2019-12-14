using Mir.ImageLibrary.Converter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;
using System.Runtime.InteropServices;

namespace Mir.ImageLibrary.Wemade
{
    public class WTLImageLibrary : IImageLibrary
    {
        private readonly Stream _stream;
        private readonly BinaryReader _reader;
        private IDictionary<ImageType, IImage>[] _images;
        private WTLImageLibrary _shadowLibrary;

        public IDictionary<ImageType, IImage> this[int index] => _images[index];

        public string FilePath { get; }
        public string Name { get; }
        public int Count { get => _images.Length; }
        public bool IsNewVersion { get; private set; }



        public WTLImageLibrary(string path)
        {
            FilePath = path;
            Name = Path.GetFileNameWithoutExtension(path);
            //_stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            //_reader = new BinaryReader(_stream);

            _stream = new MemoryStream(File.ReadAllBytes(path));
            _reader = new BinaryReader(_stream);
        }

        public void Initialize()
        {
            _stream.Seek(2, SeekOrigin.Begin);
            var version = System.Text.Encoding.UTF8.GetString(_reader.ReadBytes(20)).TrimEnd('\0');
            IsNewVersion = version == "ILIB v2.0-WEMADE";

            _stream.Seek(28, SeekOrigin.Begin);
            var count = _reader.ReadInt32();

            _images = new IDictionary<ImageType, IImage>[count];

            if (IsNewVersion)
            {
                _reader.BaseStream.Seek(_stream.Length - count * 4, SeekOrigin.Begin);
            }

            var indexList = new int[count];
            for (int i = 0; i < count; i++)
                indexList[i] = _reader.ReadInt32();

            string shadowPath = FilePath.Replace(".wtl", "_S.wtl");

            if (File.Exists(shadowPath))
                _shadowLibrary = new WTLImageLibrary(shadowPath);

            for (var i = 0; i < indexList.Length; i++)
                CheckImage(i, indexList[i]);
        }

        private void CheckImage(int index, int position)
        {
            if (position == 0) return;

            _stream.Seek(position, SeekOrigin.Begin);

            IImage mainImage;
            IImage shadowImage = null;
            IImage maskImage = null;

            var width = _reader.ReadUInt16();
            var height = _reader.ReadUInt16();
            var x = _reader.ReadInt16();
            var y = _reader.ReadInt16();
            var shadowX = _reader.ReadInt16();
            var shadowY = _reader.ReadInt16();

            if (IsNewVersion)
            {
                var imageU1 = _reader.ReadByte();
                var imageTextureType = _reader.ReadByte();
                var maskU1 = _reader.ReadByte();
                var maskTextureType = _reader.ReadByte();

                var hasMask = maskTextureType > 0;
                var length = _reader.ReadInt32();
                if (length % 4 > 0) length += 4 - (length % 4);

                var buffer = ReadImage(index, _reader, length, width, height, imageTextureType, out ImageDataType dataType);
                mainImage = new WTLImage(width, height, x, y, ModificatorType.None, dataType, buffer);
                if (hasMask)
                {
                    buffer = ReadImage(index, _reader, length, width, height, imageTextureType, out ImageDataType dataTypeMask);
                    maskImage = new WTLImage(width, height, x, y, ModificatorType.None, dataTypeMask, buffer);

                    if (_shadowLibrary != null && _shadowLibrary.Count > index && _shadowLibrary[index] != null)
                        shadowImage = _shadowLibrary[index][ImageType.Image];
                    else if (shadowX != 0 || shadowY != 0)
                        shadowImage = new WTLImage(shadowX, shadowY, ModificatorType.None);
                }
            }
            else
            {
                var length = _reader.ReadByte() | _reader.ReadByte() << 8 | _reader.ReadByte() << 16;
                var shadow = _reader.ReadByte();
                var hasMask = ((shadow >> 7) == 1) ? true : false;

                throw new NotImplementedException();
            }

            _images[index] = new Dictionary<ImageType, IImage>
            {
                {ImageType.Image, mainImage }
            };

            if (shadowImage != null)
                _images[index].Add(ImageType.Shadow, shadowImage);

            if (maskImage != null)
                _images[index].Add(ImageType.Overlay, maskImage);
        }

        public unsafe byte[] ReadImage(int index, BinaryReader bReader, int imageLength, ushort outputWidth, ushort outputHeight, byte textureType, out ImageDataType dataType)
        {
            return IsNewVersion
                ? DecompressV2Texture(index, bReader, imageLength, outputWidth, outputHeight, textureType, out dataType)
                : DecompressV1Texture(index, bReader, imageLength, outputWidth, outputHeight, out dataType);
        }

        public void Dispose()
        {
            _stream.Dispose();
            _images = null;
        }

        private static void DecompressBlock(IList<byte> newPixels, byte[] block)
        {
            byte[] colours = new byte[8];
            Array.Copy(block, 0, colours, 0, 8);

            byte[] codes = new byte[16];

            int a = Unpack(block, 0, codes, 0);
            int b = Unpack(block, 2, codes, 4);

            for (int i = 0; i < 3; i++)
            {
                int c = codes[i];
                int d = codes[4 + i];

                if (a <= b)
                {
                    codes[8 + i] = (byte)((c + d) / 2);
                    codes[12 + i] = 0;
                }
                else
                {
                    codes[8 + i] = (byte)((2 * c + d) / 3);
                    codes[12 + i] = (byte)((c + 2 * d) / 3);
                }
            }

            codes[8 + 3] = 255;
            codes[12 + 3] = (a <= b) ? (byte)0 : (byte)255;
            for (int i = 0; i < 4; i++)
            {
                if ((codes[i * 4] == 0) && (codes[(i * 4) + 1] == 0) && (codes[(i * 4) + 2] == 0) && (codes[(i * 4) + 3] == 255))
                { //dont ever use pure black cause that gives transparency issues
                    codes[i * 4] = 1;
                    codes[(i * 4) + 1] = 1;
                    codes[(i * 4) + 2] = 1;
                }
            }

            byte[] indices = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                byte packed = block[4 + i];

                indices[0 + i * 4] = (byte)(packed & 0x3);
                indices[1 + i * 4] = (byte)((packed >> 2) & 0x3);
                indices[2 + i * 4] = (byte)((packed >> 4) & 0x3);
                indices[3 + i * 4] = (byte)((packed >> 6) & 0x3);
            }

            for (int i = 0; i < 16; i++)
            {
                byte offset = (byte)(4 * indices[i]);
                for (int j = 0; j < 4; j++)
                    newPixels[4 * i + j] = codes[offset + j];
            }
        }

        private unsafe byte[] DecompressV1Texture(int index, BinaryReader bReader, int imageLength, ushort outputWidth, ushort outputHeight, out ImageDataType dataType)
        {
            dataType = ImageDataType.RGBA;

            const int size = 8;
            int offset = 0, blockOffSet = 0;
            List<byte> countList = new List<byte>();
            int tWidth = 2;

            while (tWidth < outputWidth)
                tWidth *= 2;

            var _fBytes = bReader.ReadBytes(imageLength);

            var output = new byte[outputWidth * outputHeight * 4];

            fixed (byte* pixels = output)
            {
                int cap = outputWidth * outputHeight * 4;
                int currentx = 0;

                while (blockOffSet < imageLength)
                {
                    countList.Clear();
                    for (int i = 0; i < 8; i++)
                        countList.Add(_fBytes[blockOffSet++]);

                    for (int i = 0; i < countList.Count; i++)
                    {
                        int count = countList[i];

                        if (i % 2 == 0)
                        {
                            if (currentx >= tWidth)
                                currentx -= tWidth;

                            for (int off = 0; off < count; off++)
                            {
                                if (currentx < outputWidth)
                                    offset++;

                                currentx += 4;

                                if (currentx >= tWidth)
                                    currentx -= tWidth;
                            }
                            continue;
                        }

                        for (int c = 0; c < count; c++)
                        {
                            if (blockOffSet >= _fBytes.Length)
                                break;

                            byte[] newPixels = new byte[64];
                            byte[] block = new byte[size];

                            Array.Copy(_fBytes, blockOffSet, block, 0, size);
                            blockOffSet += size;
                            DecompressBlock(newPixels, block);

                            int pixelOffSet = 0;
                            byte[] sourcePixel = new byte[4];

                            for (int py = 0; py < 4; py++)
                            {
                                for (int px = 0; px < 4; px++)
                                {
                                    int blockx = offset % (outputWidth / 4);
                                    int blocky = offset / (outputWidth / 4);

                                    int x = blockx * 4;
                                    int y = blocky * 4;

                                    int destPixel = ((y + py) * outputWidth) * 4 + (x + px) * 4;

                                    Array.Copy(newPixels, pixelOffSet, sourcePixel, 0, 4);
                                    pixelOffSet += 4;

                                    if (destPixel + 4 > cap)
                                        break;
                                    for (int pc = 0; pc < 4; pc++)
                                        pixels[destPixel + pc] = sourcePixel[pc];
                                }
                            }
                            offset++;
                            if (currentx >= outputWidth)
                                currentx -= outputWidth;
                            currentx += 4;
                        }
                    }
                }
            }
            return output;
        }


        private unsafe byte[] DecompressV2Texture(int index, BinaryReader bReader, int imageLength, ushort outputWidth, ushort outputHeight, byte textureType, out ImageDataType dataType)
        {
            var buffer = bReader.ReadBytes(imageLength);

            int w = outputWidth + (4 - outputWidth % 4) % 4;
            int a = 1;
            while (true)
            {
                a *= 2;
                if (a >= w)
                {
                    w = a;
                    break;
                }
            }
            int h = outputHeight + (4 - outputHeight % 4) % 4;
            int e = w * h / 2;

            ImageDataType type;

            switch (textureType)
            {
                case 0:
                case 1:
                    type = ImageDataType.Dxt1;
                    break;
                case 3:
                    type = ImageDataType.Dxt3;
                    break;
                case 5:
                    type = ImageDataType.Dxt5;
                    break;
                default:
                    throw new NotImplementedException();
            }


            var decompressedBuffer = Ionic.Zlib.DeflateStream.UncompressBuffer(buffer);

            var bitmapData = BitmapConverter.ConvertTextureToBitmap(type, w, h, decompressedBuffer);
            var newBuffer = new byte[outputWidth * outputHeight * 4];

            var sourceRowSize = w * 4;
            var destRowSize = outputWidth * 4;
            var rows = (int)Math.Ceiling((decimal)bitmapData.Length / sourceRowSize);

            for (var r = 0; r < rows; r++)
            {
                var sourceStartOffset = sourceRowSize * r;
                var destStartOffset = destRowSize * r;
                var dataLength = destStartOffset + destRowSize > newBuffer.Length ? newBuffer.Length - destStartOffset : destRowSize;
                if (dataLength < 0) break;
                Array.Copy(bitmapData, sourceStartOffset, newBuffer, destStartOffset, dataLength);
            }


            // reconvert without black area
            dataType = type;
            return BitmapConverter.ConvertBitmapToTexture(dataType, outputWidth, outputHeight, newBuffer);
        }


        private static int Unpack(IList<byte> packed, int srcOffset, IList<byte> colour, int dstOffSet)
        {
            int value = packed[0 + srcOffset] | (packed[1 + srcOffset] << 8);
            // get components in the stored range
            byte red = (byte)((value >> 11) & 0x1F);
            byte green = (byte)((value >> 5) & 0x3F);
            byte blue = (byte)(value & 0x1F);

            // Scale up to 24 Bit
            colour[2 + dstOffSet] = (byte)((red << 3) | (red >> 2));
            colour[1 + dstOffSet] = (byte)((green << 2) | (green >> 4));
            colour[0 + dstOffSet] = (byte)((blue << 3) | (blue >> 2));
            colour[3 + dstOffSet] = 255;
            //*/
            return value;
        }
    }
}
