using Microsoft.Xna.Framework;
using Mir.Ethernity.MapLibrary;
using Myra;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapEditor.Interface
{
    public class MapGridLayer : Panel
    {
        public override void InternalRender(RenderContext batch)
        {
            if (Environment.MapActive == null) return;

            int minX = Math.Max(0, Environment.UserX - Environment.OffsetX - 4), maxX = Math.Min(Environment.MapActive.Width - 1, Environment.UserX + Environment.OffsetX + 4);
            int minY = Math.Max(0, Environment.UserY - Environment.OffsetY - 4), maxY = Math.Min(Environment.MapActive.Height - 1, Environment.UserY + Environment.OffsetY + 25);

            for (int y = minY; y <= maxY; y++)
            {
                float drawY = (y - Environment.UserY + Environment.OffsetY) * Environment.CellHeight - Environment.UserOffsetY;

                for (int x = minX; x <= maxX; x++)
                {
                    float drawX = (x - Environment.UserX + Environment.OffsetX) * Environment.CellWidth - Environment.UserOffsetX;

                    var cell = Environment.MapActive.Cells.GetLength(0) > x && Environment.MapActive.Cells.GetLength(1) > y
                        ? Environment.MapActive.Cells[x, y]
                        : null;

                    Color color = Environment.MouseX == x && Environment.MouseY == y
                        ? Color.White
                        : Color.White * 0.5f;

                    var selected = Environment.SelectedCells.Any(scell => scell.X == x && scell.Y == y);

                    if (selected)
                    {
                        batch.Batch.Draw(GraphicsManager.WhitePixel, new Rectangle((int)drawX, (int)drawY, Environment.CellWidth, Environment.CellHeight), Color.Blue * 0.5f);
                    }
                    else if (Environment.ShowNoWalkGrid && cell.Flag)
                    {
                        batch.Batch.Draw(GraphicsManager.WhitePixel, new Rectangle((int)drawX, (int)drawY, Environment.CellWidth, Environment.CellHeight), Color.Red * 0.5f);
                    }

                    if (Environment.ShowGrid)
                        batch.Batch.DrawRectangle(new Rectangle((int)drawX, (int)drawY, Environment.CellWidth, Environment.CellHeight), color);
                }
            }
        }
    }
}
