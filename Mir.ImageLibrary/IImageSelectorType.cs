namespace Mir.ImageLibrary
{
    public interface IImageSelectorType
    {
        int Length { get; }
        IImage this[ImageType type] { get; }
    }
}
