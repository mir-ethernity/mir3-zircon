using System;

namespace Mir.ImageLibrary.Converter
{
    public static class ImageLibraryConverter
    {
        public static TDestinationImageLibrary Convert<TDestinationImageLibrary>(IImageLibrary source) where TDestinationImageLibrary : IImageLibraryEditor
        {
            var editor = Activator.CreateInstance<TDestinationImageLibrary>();
           
            for (var i = 0; i < source.Count; i++)
            {
                var tmp = source[i];

                if(tmp == null)
                {
                    editor.AddEmptyImage();
                    continue;
                }

                foreach (var type in tmp)
                {
                    var imageType = type.Key;
                    var image = type.Value;

                    if (type.Value.HasData)
                    {
                        var buffer = type.Value.GetBuffer();
                        // var rgba = BitmapConverter.ConvertTextureToBitmap(image.DataType, image.Width, image.Height, buffer);
                        var newImage = editor.CreateImageFromTexture(image.Width, image.Height, image.OffsetX, image.OffsetY, image.Modificator, buffer, image.DataType);
                        editor.AddImage(imageType, newImage);
                    }
                    else
                    {
                        var newImage = editor.CreateImageWithoutData(image.OffsetX, image.OffsetY, image.Modificator);
                        editor.AddImage(imageType, newImage);
                    }
                }
            }

            return editor;
        }
    }
}
