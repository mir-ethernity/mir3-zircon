using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Mir.Ethernity.ImageLibrary;
using Mir.Ethernity.MapLibrary;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapEditor.Interface
{
    public class MapFloorLayer : Panel
    {
        public override void InternalRender(RenderContext context)
        {
            if (Environment.MapActive == null) return;

            int minX = Math.Max(0, Environment.UserX - Environment.OffsetX - 4), maxX = Math.Min(Environment.MapActive.Width - 1, Environment.UserX + Environment.OffsetX + 4);
            int minY = Math.Max(0, Environment.UserY - Environment.OffsetY - 4), maxY = Math.Min(Environment.MapActive.Height - 1, Environment.UserY + Environment.OffsetY + 4);

            for (int y = minY; y <= maxY; y++)
            {
                if (y < 0) continue;
                if (y >= Environment.MapActive.Height) break;

                float drawY = (y - Environment.UserY + Environment.OffsetY) * Environment.CellHeight - Environment.UserOffsetY;

                for (int x = minX; x <= maxX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Environment.MapActive.Width) break;

                    float drawX = (x - Environment.UserX + Environment.OffsetX) * Environment.CellWidth - Environment.UserOffsetX;

                    MapCell tile = Environment.MapActive.Cells[x, y];

                    if (y % 2 == 0 && x % 2 == 0)
                    {
                        var file = LibraryManager.Get(tile.Back.TileType, tile.Back.FileType);
                        if (file != null)
                        {
                            var image = LibraryManager.GenerateTexture(file[tile.Back.ImageIndex][ImageType.Image]);
                            context.Batch.Draw(image, new Vector2(drawX, drawY), Color.White);
                        }
                    }
                }
            }

            for (int y = minY; y <= maxY; y++)
            {
                float drawY = (y - Environment.UserY + Environment.OffsetY + 1) * Environment.CellHeight - Environment.UserOffsetY;

                for (int x = minX; x <= maxX; x++)
                {
                    float drawX = (x - Environment.UserX + Environment.OffsetX) * Environment.CellWidth - Environment.UserOffsetX;

                    MapCell cell = Environment.MapActive.Cells[x, y];

                    if (cell.Middle != null && (cell.Middle.AnimationFrame == null))
                    {
                        var file = LibraryManager.Get(cell.Middle.TileType, cell.Middle.FileType);
                        if (file != null && file[cell.Middle.ImageIndex] != null)
                        {
                            var image = LibraryManager.GenerateTexture(file[cell.Middle.ImageIndex][ImageType.Image]);

                            if ((image.Width == Environment.CellWidth && image.Height == Environment.CellHeight)
                                || (image.Width == Environment.CellWidth * 2 && image.Height == Environment.CellHeight * 2))
                                context.Batch.Draw(image, new Vector2(drawX, drawY - Environment.CellHeight), Color.White);
                        }
                    }

                    if (cell.Front != null && (cell.Front.AnimationFrame == null))
                    {
                        var file = LibraryManager.Get(cell.Front.TileType, cell.Front.FileType);
                        if (file != null)
                        {
                            if (file[cell.Front.ImageIndex] != null)
                            {
                                var image = LibraryManager.GenerateTexture(file[cell.Front.ImageIndex][ImageType.Image]);

                                if ((image.Width == Environment.CellWidth && image.Height == Environment.CellHeight)
                                    || (image.Width == Environment.CellWidth * 2 && image.Height == Environment.CellHeight * 2))
                                    context.Batch.Draw(image, new Vector2(drawX, drawY - Environment.CellHeight), Color.White);
                            }
                        }
                    }
                }
            }
        }
    }

}
