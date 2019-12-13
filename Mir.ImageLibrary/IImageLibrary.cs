using System;
using System.Collections.Generic;

namespace Mir.ImageLibrary
{

    public interface IImageLibrary : IDisposable
    {
        string Name { get; }
        int Count { get; }
        IDictionary<ImageType, IImage> this[int index] { get; }
    }
}
