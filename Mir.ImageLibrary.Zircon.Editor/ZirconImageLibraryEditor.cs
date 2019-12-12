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
            _images[_images.Length - 1] = new ZirconSelectorType(null, null, null);
            _images[_images.Length - 1][type] = image;
        }

        public void InsertImage(int index, ImageType type, IImage image)
        {
            var tmp = new ZirconSelectorType[_images.Length + 1];
            tmp[index] = new ZirconSelectorType(null, null, null);
            tmp[index][type] = image;
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
            var tmp = new ZirconSelectorType[_images.Length - 1];
            Array.Copy(_images, 0, tmp, 0, index - 1);
            Array.Copy(_images, index, tmp, index + 1, _images.Length - index);
        }

        public void ReplaceImage(int index, ImageType type, IImage image)
        {
            if (_images[index] == null)
            {
                _images[index] = new ZirconSelectorType(null, null, null);
            }

            _images[index][type] = image;
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

        public async Task Save(Stream stream)
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

                if (image[ImageType.Image] != null)
                {
                    var zircon = ((ZirconImage)image[ImageType.Image]).Position = position;
                    position += ZirconImage.CalculateImageDataSize(image[ImageType.Image].Width, image[ImageType.Image].Height);
                }

                if (image[ImageType.Shadow] != null)
                {
                    var zircon = ((ZirconImage)image[ImageType.Shadow]).Position = position;
                    position += ZirconImage.CalculateImageDataSize(image[ImageType.Shadow].Width, image[ImageType.Shadow].Height);
                }

                if (image[ImageType.Overlay] != null)
                {
                    var zircon = ((ZirconImage)image[ImageType.Image]).Position = position;
                    position += ZirconImage.CalculateImageDataSize(image[ImageType.Overlay].Width, image[ImageType.Overlay].Height);
                }
            }


            using (MemoryStream buffer = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(buffer))
            {
                writer.Write(headerSize);
                writer.Write(_images.Length);

                foreach (ZirconSelectorType image in _images)
                {
                    writer.Write(image != null);
                    if (image == null) continue;

                    var img = (ZirconImage)image[ImageType.Image];
                    var shadow = (ZirconImage)image[ImageType.Shadow];
                    var overlay = (ZirconImage)image[ImageType.Overlay];

                    writer.Write(img.Position);
                    writer.Write(img.Width);
                    writer.Write(img.Height);
                    writer.Write(img.OffsetX);
                    writer.Write(img.OffsetY);
                    writer.Write((byte)(img.Modificator == ModificatorType.Transform ? 49 : 50));

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

                    if (image[ImageType.Image] != null)
                    {
                        var data = image[ImageType.Image].GetData();
                        writer.Write(data.Buffer);
                    }

                    if (image[ImageType.Shadow] != null)
                    {
                        var data = image[ImageType.Shadow].GetData();
                        writer.Write(data.Buffer);
                    }

                    if (image[ImageType.Overlay] != null)
                    {
                        var data = image[ImageType.Overlay].GetData();
                        writer.Write(data.Buffer);
                    }
                }

                var b = buffer.ToArray();
                await stream.WriteAsync(b, 0, b.Length);
            }
        }
    }
}
