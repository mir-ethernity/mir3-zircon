using System;
using System.Collections.Generic;
using System.Text;

namespace Mir.ImageLibrary.Zircon
{
    public class ZirconSelectorType : IImageSelectorType
    {
        private IDictionary<ImageType, ZirconImage> _images;

        public ZirconSelectorType(
            ZirconImage image,
            ZirconImage shadow,
            ZirconImage overlay
        )
        {
            _images = new Dictionary<ImageType, ZirconImage>
            {
                { ImageType.Image, image },
                { ImageType.Shadow, shadow },
                { ImageType.Overlay, overlay }
            };
        }

        public IImage this[ImageType type]
        {
            get { return _images[type]; }
            internal set { _images[type] = (ZirconImage)value; }
        }

        public int Length => _images.Count;
    }
}
