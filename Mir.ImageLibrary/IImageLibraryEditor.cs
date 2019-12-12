using System.IO;
using System.Threading.Tasks;

namespace Mir.ImageLibrary
{
    public interface IImageLibraryEditor : IImageLibrary
    {
        void AddImage(ImageType type, IImage image);
        void InsertImage(int index, ImageType type, IImage image);
        void ReplaceImage(int index, ImageType type, IImage image);
        void RemoveImage(int index);
        void RemoveBlanks(bool safe = false);
        void SetOffsetX(int index, ImageType image, short temp);
        void SetOffsetY(int index, ImageType image, short temp);

        Task Save(Stream stream);
    }
}
