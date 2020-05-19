using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mir.Ethernity.ImageLibrary;
using Mir.Ethernity.ImageLibrary.Zircon;
using Mir.Ethernity.MapLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor
{
    public static class Environment
    {
        public static IMapReader MapReader { get; set; }
        public static int CellWidth { get; set; } = 48;
        public static int CellHeight { get; set; } = 32;
        public static GraphicsDevice GraphicsDevice { get; internal set; }
        public static string LibraryExtension { get; internal set; } = ".Zl";
        public static int OffsetX { get; internal set; }
        public static int OffsetY { get; internal set; }
        public static Func<string, IImageLibrary> CreateImageLibrary { get; set; }

        public static GameTime Time { get; set; }

        public static Map MapActive { get; set; }
        public static int MapX { get; set; }
        public static int MapY { get; set; }
        public static int MapAnimation { get; set; }
    }
}
