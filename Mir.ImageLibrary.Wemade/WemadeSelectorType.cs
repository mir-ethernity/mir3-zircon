using System;
using System.Collections.Generic;
using System.Text;

namespace Mir.ImageLibrary.Wemade
{
    public class WemadeSelectorType : IImageSelectorType
    {
        private IDictionary<ImageType, WemadeImage> _images;

        public WemadeSelectorType(
            WemadeImage image,
            WemadeImage shadow,
            WemadeImage overlay
        )
        {
            _images = new Dictionary<ImageType, WemadeImage>
            {
                { ImageType.Image, image },
                { ImageType.Shadow, shadow },
                { ImageType.Overlay, overlay }
            };
        }

        public IImage this[ImageType type]
        {
            get { return _images[type]; }
            internal set { _images[type] = (WemadeImage)value; }
        }

        public int Length => _images.Count;
    }
}
