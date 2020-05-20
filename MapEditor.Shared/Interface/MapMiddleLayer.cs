using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            int minX = Math.Max(0, Environment.UserX - Environment.OffsetX - 4), maxX = Math.Min(Environment.MapActive.Width - 1, Environment.UserX + Environment.OffsetX + 4);
            int minY = Math.Max(0, Environment.UserY - Environment.OffsetY - 4), maxY = Math.Min(Environment.MapActive.Height - 1, Environment.UserY + Environment.OffsetY + 25);

            for (int y = minY; y <= maxY; y++)
            {
                float drawY = (y - Environment.UserY + Environment.OffsetY + 1) * Environment.CellHeight - Environment.UserOffsetY;

                for (int x = minX; x <= maxX; x++)
                {
                    float drawX = (x - Environment.UserX + Environment.OffsetX) * Environment.CellWidth - Environment.UserOffsetX;

                    MapCell cell = Environment.MapActive.Cells[x, y];

                    if (cell.Middle != null)
                    {
                        var file = LibraryManager.Get(cell.Middle.TileType, cell.Middle.FileType);
                        if (file != null)
                        {
                            var index = cell.Middle.ImageIndex;

                            bool blend = false;
                            if (cell.Middle.AnimationFrame != null)
                            {
                                index += (ushort)(Environment.MapAnimation % (cell.Middle.AnimationFrame.Value & 0x7F));
                                blend = (cell.Middle.AnimationFrame.Value & 0x80) > 0;
                            }

                            if (file.Count > index)
                            {
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
                                        {
                                            context.Batch.End();
                                            context.Batch.Begin(blendState: BlendState.Additive);
                                            context.Batch.Draw(texture, new Vector2(drawX, drawY - texture.Height), Color.White);
                                            context.Batch.End();
                                            context.Batch.Begin();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // wrong image
                            }
                        }
                    }


                    if (cell.Front != null)
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
                                    {
                                        context.Batch.End();
                                        context.Batch.Begin(blendState: BlendState.Additive);
                                        context.Batch.Draw(texture, new Vector2(drawX, drawY - texture.Height), Color.White);
                                        context.Batch.End();
                                        context.Batch.Begin();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
