using System;
using System.Collections.Generic;
using System.Text;

namespace Mir.ImageLibrary
{
    [Flags]
    public enum ModificatorType : byte
    {
        None = 0x00,
        Transform = 0x01,
        Opacity = 0x02
    }
}
