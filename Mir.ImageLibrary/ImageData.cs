using System;
using System.Collections.Generic;
using System.Text;

namespace Mir.ImageLibrary
{
    public struct ImageData
    {
        public byte[] Buffer;
        public ImageDataType Type { get; set; }
    }
}
