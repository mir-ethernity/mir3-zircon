using Microsoft.Xna.Framework;
using Mir.Ethernity.MapLibrary;
using Myra;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapEditor.Interface
{
    public class MapGridLayer : Panel
    {
        public override void InternalRender(RenderContext batch)
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
                int drawY = (y - mapY + Environment.OffsetY) * Environment.CellHeight - mapYO;

                for (int x = minX; x <= maxX; x++)
                {
                    int drawX = (x - mapX + Environment.OffsetX) * Environment.CellWidth - mapXO;

                    batch.Batch.DrawRectangle(new Rectangle(drawX, drawY, Environment.CellWidth, Environment.CellHeight), Color.White * 0.5f);
                }
            }
        }
    }
}
