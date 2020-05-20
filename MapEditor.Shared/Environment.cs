using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mir.Ethernity.ImageLibrary;
using Mir.Ethernity.ImageLibrary.Zircon;
using Mir.Ethernity.MapLibrary;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor
{
    public enum CursorTool
    {
        Drag = 1,
        Selection = 2
    }

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
        public static CursorTool CursorTool { get; set; } = CursorTool.Drag;

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
        public static bool ShowGrid { get; set; } = false;
        public static bool ShowNoWalkGrid { get; set; }
        public static List<Point> SelectedCells { get; set; } = new List<Point>();

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

        public static void UpdateUserLocation(int x, int y)
        {
            UserScreenX = x * CellWidth;
            UserScreenY = y * CellHeight;

            UserX = x;
            UserY = y;

            UserOffsetX = 0;
            UserOffsetY = 0;

            UpdateMouseTile();
        }

        public static void UpdateMouseTile()
        {
            MouseX = ((Desktop.MousePosition.X + UserOffsetX) / CellWidth) + UserX - OffsetX;
            MouseY = ((Desktop.MousePosition.Y + UserOffsetY) / CellHeight) + UserY - OffsetY;
        }

        public static void LoadMap(string filePath)
        {
            using (var fs = File.OpenRead(filePath))
                MapActive = MapReader.Read(fs);

            UpdateUserScreen((MapActive.Width * CellWidth) / 2, (MapActive.Height * CellHeight) / 2);
        }

        public static bool AroundCell(int mouseX, int mouseY)
        {
            foreach (var cell in SelectedCells)
            {
                var freeUP = !SelectedCells.Any(x => x.X == cell.X && x.Y == cell.Y - 1);
                var freeDOWN = !SelectedCells.Any(x => x.X == cell.X && x.Y == cell.Y + 1);
                var freeLEFT = !SelectedCells.Any(x => x.X == cell.X - 1 && x.Y == cell.Y);
                var freeRIGHT = !SelectedCells.Any(x => x.X == cell.X + 1 && x.Y == cell.Y);

                if (freeUP && cell.Y - 1 == mouseY && cell.X == mouseX)
                    return true;
                else if (freeDOWN && cell.Y + 1 == mouseY && cell.X == mouseX)
                    return true;
                else if (freeLEFT && cell.X - 1 == mouseX && cell.Y == mouseY)
                    return true;
                else if (freeRIGHT && cell.X + 1 == mouseX && cell.Y == mouseY)
                    return true;
            }

            return false;
        }
    }
}
