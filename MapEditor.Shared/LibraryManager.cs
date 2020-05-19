using Microsoft.Xna.Framework.Graphics;
using Mir.Ethernity.ImageLibrary;
using Mir.Ethernity.ImageLibrary.Zircon;
using Mir.Ethernity.MapLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor
{
    public class ImageLibraryResolver
    {
        private IImageLibrary _imageLibrary;

        public string FileName { get; set; }


        public ImageLibraryResolver(string path)
        {
            FileName = path;
        }

        public IImageLibrary Get()
        {
            if (_imageLibrary == null)
                _imageLibrary = Environment.CreateImageLibrary(FileName);
            return _imageLibrary;
        }
    }

    public static class LibraryManager
    {
        private static Dictionary<string, ImageLibraryResolver> _mapLibraries = new Dictionary<string, ImageLibraryResolver>();
        private static Dictionary<IImage, Texture2D> _cache = new Dictionary<IImage, Texture2D>();

        static LibraryManager()
        {
            var rootMapData = @"E:\Debug\Client\Data\";

            var mapTileTypes = Enum.GetValues(typeof(MapTileType)).Cast<MapTileType>().ToArray();
            var mapFileTypes = Enum.GetValues(typeof(MapFileType)).Cast<MapFileType>().ToArray();
            foreach (var tileType in mapTileTypes)
            {
                foreach (var fileType in mapFileTypes)
                {
                    var path = $"{rootMapData}Map Data/";
                    if (tileType != MapTileType.Normal)
                        path += tileType.ToString() + "/";
                    path += fileType.ToString() + Environment.LibraryExtension;

                    if (File.Exists(path))
                        _mapLibraries.Add(GetMapLibraryKey(tileType, fileType), new ImageLibraryResolver(path));
                }
            }
        }
        private static string GetMapLibraryKey(MapTileType tileType, MapFileType fileType)
        {
            return $"{tileType.ToString()}_{fileType.ToString()}";
        }


        public static IImageLibrary Get(MapTileType tileType, MapFileType fileType)
        {
            if (!_mapLibraries.TryGetValue(GetMapLibraryKey(tileType, fileType), out ImageLibraryResolver resolver))
                return null;
            var library = resolver.Get();
            if (!library.Initialized) library?.Initialize();
            return library;
        }

        public static Texture2D GenerateTexture(IImage image)
        {
            if (_cache.ContainsKey(image))
                return _cache[image];

            Texture2D texture;

            int w = image.Width + (4 - image.Width % 4) % 4;
            int h = image.Height + (4 - image.Height % 4) % 4;

            var buffer = image.GetBuffer();

            if (image.Compression == CompressionType.Deflate)
            {
                using (var output = new MemoryStream())
                using (var gz = new DeflateStream(new MemoryStream(buffer), CompressionMode.Decompress))
                {
                    gz.CopyTo(output);
                    gz.Close();
                    buffer = output.ToArray();
                }
            }

            switch (image.DataType)
            {
                case ImageDataType.Dxt1:
                    texture = new Texture2D(Environment.GraphicsDevice, w, h, false, SurfaceFormat.Dxt1);
                    texture.SetData(buffer);
                    break;
                case ImageDataType.Dxt3:
                    texture = new Texture2D(Environment.GraphicsDevice, w, h, false, SurfaceFormat.Dxt3);
                    texture.SetData(buffer);
                    break;
                case ImageDataType.Dxt5:
                    texture = new Texture2D(Environment.GraphicsDevice, w, h, false, SurfaceFormat.Dxt5);
                    texture.SetData(buffer);
                    break;
                default:
                    throw new NotImplementedException();
            }

            _cache.Add(image, texture);

            return texture;
        }
    }
}
