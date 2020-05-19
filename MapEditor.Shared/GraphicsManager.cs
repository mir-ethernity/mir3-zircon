using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapEditor
{
    public class GraphicsManager
    {
        public static Texture2D WhitePixel { get; private set; }

        public static void Load(GraphicsDevice graphicsDevice)
        {
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.White });
            WhitePixel = texture;
        }
    }
}
