using Microsoft.Xna.Framework;
using Mir.Ethernity.MapLibrary;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor.Interface
{
    public class MapEditorControl : HorizontalStackPanel
    {
        private MapPreview _preview;
        private ToolboxControl _toolbox;

        public MapEditorControl()
        {
            Proportions.Add(new Proportion { Type = ProportionType.Fill });
            Proportions.Add(new Proportion { Type = ProportionType.Auto });

            Widgets.Add(_preview = new MapPreview());
            Widgets.Add(_toolbox = new ToolboxControl());

            LoadMap(@"E:\Debug\Client\Map\d4301.map");
        }

        public void LoadMap(string filePath)
        {
            using (var fs = File.OpenRead(filePath))
                Environment.MapActive = Environment.MapReader.Read(fs);

            Environment.UpdateUserScreen((Environment.MapActive.Width * Environment.CellWidth) / 2, (Environment.MapActive.Height * Environment.CellHeight) / 2);
        }
    }
}
