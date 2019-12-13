using Mir.ImageLibrary.Converter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Mir.ImageLibrary.Zircon.Editor
{
    public class ZirconImageLibraryEditor : ZirconImageLibrary, IImageLibraryEditor
    {
        public ZirconImageLibraryEditor() : base()
        {

        }

        public ZirconImageLibraryEditor(string name, Stream stream) : base(name, stream)
        {
        }

        public ZirconImageLibraryEditor(string zlPath)
            : base(zlPath)
        {
        }

        public void AddImage(ImageType type, IImage image)
        {
            Array.Resize(ref _images, _images.Length + 1);
            _images[_images.Length - 1] = new Dictionary<ImageType, IImage>()
            {
                { type, image }
            };
        }

        public void AddEmptyImage()
        {
            Array.Resize(ref _images, _images.Length + 1);
            _images[_images.Length - 1] = null;
        }

        public void InsertImage(int index, ImageType type, IImage image)
        {
            var tmp = new IDictionary<ImageType, IImage>[_images.Length + 1];
            tmp[index] = new Dictionary<ImageType, IImage>()
            {
                { type, image }
            };
            Array.Copy(_images, 0, tmp, 0, index - 1);
            Array.Copy(_images, index, tmp, index + 1, _images.Length - index);
        }

        public void RemoveBlanks(bool safe = false)
        {
            for (int i = _images.Length - 1; i >= 0; i--)
            {
                if (_images[i] == null)
                {
                    RemoveImage(i);
                }
            }
        }

        public void RemoveImage(int index)
        {
            var tmp = new IDictionary<ImageType, IImage>[_images.Length - 1];
            Array.Copy(_images, 0, tmp, 0, index - 1);
            Array.Copy(_images, index, tmp, index + 1, _images.Length - index);
        }

        public void ReplaceImage(int index, ImageType type, IImage image)
        {
            if (_images[index] == null)
            {
                _images[index] = new Dictionary<ImageType, IImage>();
            }

            if (_images[index].ContainsKey(type))
                _images[index][type] = image;
            else
                _images[index].Add(type, image);
        }

        public void SetOffsetX(int index, ImageType type, short offsetX)
        {
            ZirconImage image = (ZirconImage)(_images[index]?[type]);
            if (image == null) return;
            image.OffsetX = offsetX;
        }

        public void SetOffsetY(int index, ImageType type, short offsetY)
        {
            ZirconImage image = (ZirconImage)(_images[index]?[type]);
            if (image == null) return;
            image.OffsetY = offsetY;
        }

        public void Save(Stream stream)
        {
            int headerSize = 4 + _images.Length;

            foreach (var image in _images)
            {
                if (image == null) continue;
                headerSize += ZirconImage.HeaderSize;
            }

            int position = headerSize + 4;

            foreach (var image in _images)
            {
                if (image == null) continue;

                if (image.ContainsKey(ImageType.Image))
                {
                    var zircon = ((ZirconImage)image[ImageType.Image]).Position = position;
                    position += ZirconImage.CalculateImageDataSize(image[ImageType.Image].Width, image[ImageType.Image].Height);
                }
                else
                {
                    throw new FormatException("The main image is always required");
                }

                if (image.ContainsKey(ImageType.Shadow) && image[ImageType.Shadow].HasData)
                {
                    var zircon = ((ZirconImage)image[ImageType.Shadow]).Position = position;
                    position += ZirconImage.CalculateImageDataSize(image[ImageType.Shadow].Width, image[ImageType.Shadow].Height);
                }

                if (image.ContainsKey(ImageType.Overlay))
                {
                    var zircon = ((ZirconImage)image[ImageType.Overlay]).Position = position;
                    position += ZirconImage.CalculateImageDataSize(image[ImageType.Overlay].Width, image[ImageType.Overlay].Height);
                }
            }


            using (MemoryStream buffer = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(buffer))
            {
                writer.Write(headerSize);
                writer.Write(Count);

                foreach (var image in _images)
                {
                    writer.Write(image != null);
                    if (image == null) continue;

                    var img = (ZirconImage)image[ImageType.Image];
                    var shadow = image.ContainsKey(ImageType.Shadow) ? (ZirconImage)image[ImageType.Shadow] : null;
                    var overlay = image.ContainsKey(ImageType.Overlay) ? (ZirconImage)image[ImageType.Overlay] : null;

                    writer.Write(img.Position);
                    writer.Write(img.Width);
                    writer.Write(img.Height);
                    writer.Write(img.OffsetX);
                    writer.Write(img.OffsetY);

                    writer.Write((byte)(shadow?.Modificator == ModificatorType.Transform
                        ? 49
                        : (shadow?.Modificator == ModificatorType.Opacity
                        ? 50
                        : 0)));

                    writer.Write((ushort)(shadow?.Width ?? 0));
                    writer.Write((ushort)(shadow?.Height ?? 0));
                    writer.Write((short)(shadow?.OffsetX ?? 0));
                    writer.Write((short)(shadow?.OffsetY ?? 0));

                    writer.Write((ushort)(overlay?.Width ?? 0));
                    writer.Write((ushort)(overlay?.Height ?? 0));
                }

                foreach (var image in _images)
                {
                    if (image == null) continue;

                    var data = image[ImageType.Image].GetBuffer();
                    writer.Write(data);

                    if (image.ContainsKey(ImageType.Shadow) && image[ImageType.Shadow].HasData)
                    {
                        data = image[ImageType.Shadow].GetBuffer();
                        writer.Write(data);
                    }

                    if (image.ContainsKey(ImageType.Overlay))
                    {
                        data = image[ImageType.Overlay].GetBuffer();
                        writer.Write(data);
                    }
                }

                var b = buffer.ToArray();
                stream.Write(b, 0, b.Length);
            }
        }

        public IImage CreateImageFromRGBA(ushort width, ushort height, short offsetX, short offsetY, ModificatorType modificator, byte[] rgba)
        {
            var texture = BitmapConverter.ConvertBitmapToTexture(ImageDataType.Dxt1, width, height, rgba);
            return new ZirconImage(width, height, offsetX, offsetY, modificator, ImageDataType.Dxt1, texture);
        }

        public IImage CreateImageWithoutData(short offsetX, short offsetY, ModificatorType modificator)
        {
            return new ZirconImage(offsetX, offsetY, modificator);
        }
    }
}
