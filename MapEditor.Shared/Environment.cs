using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mir.Ethernity.ImageLibrary;
using Mir.Ethernity.ImageLibrary.Zircon;
using Mir.Ethernity.MapLibrary;
using Myra.Graphics2D.UI;
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

        public static int MapAnimation { get; set; }


        public static int UserX { get; set; }
        public static int UserY { get; set; }

        public static int MouseX { get; set; }
        public static int MouseY { get; set; }

        public static int UserScreenX { get; set; }
        public static int UserScreenY { get; set; }
        public static int UserOffsetX { get; set; }
        public static int UserOffsetY { get; set; }
        public static bool ShowGrid { get; set; } = true;
        public static bool ShowNoWalkGrid { get; set; }

        public static void UpdateUserScreen(int x, int y)
        {
            UserScreenX = x;
            UserScreenY = y;

            UserX = UserScreenX / CellWidth;
            UserY = UserScreenY / CellHeight;

            UserOffsetX = UserScreenX - UserX * CellWidth;
            UserOffsetY = UserScreenY - UserY * CellHeight;

            UpdateMouseTile();
        }

        public static void UpdateMouseTile()
        {
            MouseX = ((Desktop.MousePosition.X + UserOffsetX) / CellWidth) + UserX - OffsetX;
            MouseY = ((Desktop.MousePosition.Y + UserOffsetY) / CellHeight) + UserY - OffsetY;
        }
    }
}
