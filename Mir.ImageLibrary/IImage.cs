using System;
using System.Collections.Generic;
using System.Text;

namespace Mir.ImageLibrary
{
    public interface IImage
    {
        bool HasData { get; }
        ImageDataType DataType { get; }
        ModificatorType Modificator { get; }

        ushort Width { get; }
        ushort Height { get; }
        short OffsetX { get; }
        short OffsetY { get; }

        byte[] GetBuffer();
    }
}
