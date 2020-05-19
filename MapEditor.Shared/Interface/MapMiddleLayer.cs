using Microsoft.Xna.Framework;
using Mir.Ethernity.ImageLibrary;
using Mir.Ethernity.MapLibrary;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapEditor.Interface
{
    public class MapMiddleLayer : Panel
    {
        public override void InternalRender(RenderContext context)
        {
            if (Environment.MapActive == null) return;

            var mapXP = (float)Environment.MapX / Environment.CellWidth;
            var mapYP = (float)Environment.MapY / Environment.CellHeight;

            var mapX = (int)Math.Floor(mapXP);
            var mapY = (int)Math.Floor(mapYP);

            var mapXO = Environment.MapX - mapX * Environment.CellWidth;
            var mapYO = Environment.MapY - mapY * Environment.CellHeight;

            int minX = Math.Max(0, mapX - Environment.OffsetX - 4), maxX = Math.Min(Environment.MapActive.Width - 1, mapX + Environment.OffsetX + 4);
            int minY = Math.Max(0, mapY - Environment.OffsetY - 4), maxY = Math.Min(Environment.MapActive.Height - 1, mapY + Environment.OffsetY + 25);

            for (int y = minY; y <= maxY; y++)
            {
                int drawY = (y - mapY + Environment.OffsetY + 1) * Environment.CellHeight - mapYO;

                for (int x = minX; x <= maxX; x++)
                {
                    int drawX = (x - mapX + Environment.OffsetX) * Environment.CellWidth - mapXO;

                    MapCell cell = Environment.MapActive.Cells[x, y];

                    if (cell.Middle != null && (cell.Middle.AnimationFrame == null))
                    {
                        var file = LibraryManager.Get(cell.Middle.TileType, cell.Middle.FileType);
                        if (file != null)
                        {
                            var index = cell.Middle.ImageIndex;

                            bool blend = false;
                            if (cell.Middle.AnimationFrame != null)
                            {
                                index += (ushort)(Environment.MapAnimation % (cell.Middle.AnimationFrame.Value & 0x4F));
                                blend = (cell.Middle.AnimationFrame.Value & 0x50) > 0;
                            }

                            var image = file[index];
                            if (image != null)
                            {
                                var texture = LibraryManager.GenerateTexture(image[ImageType.Image]);

                                if ((texture.Width != Environment.CellWidth || texture.Height != Environment.CellHeight)
                                    && (texture.Width != Environment.CellWidth * 2 || texture.Height != Environment.CellHeight * 2))
                                {
                                    if (!blend)
                                        context.Batch.Draw(texture, new Vector2(drawX, drawY - texture.Height), Color.White);
                                    else
                                        //using (GraphicsManager.Instance.UseBlend(true, 0.5f))
                                        context.Batch.Draw(texture, new Vector2(drawX, drawY - texture.Height), Color.White);
                                }
                            }
                        }
                    }


                    if (cell.Front != null && (cell.Front.AnimationFrame == null))
                    {
                        var file = LibraryManager.Get(cell.Front.TileType, cell.Front.FileType);
                        if (file != null)
                        {
                            var index = cell.Front.ImageIndex;

                            bool blend = false;
                            if (cell.Front.AnimationFrame != null)
                            {
                                index += (ushort)(Environment.MapAnimation % (cell.Front.AnimationFrame.Value & 0x7F));
                                blend = (cell.Front.AnimationFrame.Value & 0x80) > 0;
                            }

                            var image = file[index];
                            if (image != null)
                            {
                                var texture = LibraryManager.GenerateTexture(image[ImageType.Image]);

                                if ((texture.Width != Environment.CellWidth || texture.Height != Environment.CellHeight)
                                    && (texture.Width != Environment.CellWidth * 2 || texture.Height != Environment.CellHeight * 2))
                                {
                                    if (!blend)
                                        context.Batch.Draw(texture, new Vector2(drawX, drawY - texture.Height), Color.White);
                                    else
                                        context.Batch.Draw(texture, new Vector2(drawX, drawY - texture.Height), Color.White);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
