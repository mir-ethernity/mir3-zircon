using System;

namespace Mir.ImageLibrary
{

    public interface IImageLibrary : IDisposable
    {
        string Name { get; }
        int Length { get; }
        IImageSelectorType this[int index] { get; }
    }
}
