using System;
using System.Collections.Generic;
using System.Text;

namespace Mir.ImageLibrary
{
    [Flags]
    public enum ImageDataType : byte
    {
        RGBA = 0,
        Dxt1 = 1,
        Dxt3 = 2,
        Dxt5 = 3,
    }
}
