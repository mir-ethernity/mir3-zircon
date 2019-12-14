using System.IO;
using System.Threading.Tasks;

namespace Mir.ImageLibrary
{
    public interface IImageLibraryEditor : IImageLibrary
    {
        IImage CreateImageFromRGBA(ushort width, ushort height, short offsetX, short offsetY, ModificatorType modificator, byte[] rgba, ImageDataType destinationDataType);
        IImage CreateImageFromTexture(ushort width, ushort height, short offsetX, short offsetY, ModificatorType modificator, byte[] texture, ImageDataType textureDataType);
        IImage CreateImageWithoutData(short offsetX, short offsetY, ModificatorType modificator);

        void AddEmptyImage();
        void AddImage(ImageType type, IImage image);
        void InsertImage(int index, ImageType type, IImage image);
        void ReplaceImage(int index, ImageType type, IImage image);
    
        void RemoveImage(int index);
        void RemoveBlanks(bool safe = false);
        void SetOffsetX(int index, ImageType image, short temp);
        void SetOffsetY(int index, ImageType image, short temp);

        void Save(Stream stream);

    }
}
