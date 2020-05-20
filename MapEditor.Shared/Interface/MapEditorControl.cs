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
        private ToolbarControl _toobar;

        public MapEditorControl()
        {
            ClipToBounds = true;

            Proportions.Add(new Proportion { Type = ProportionType.Auto });
            Proportions.Add(new Proportion { Type = ProportionType.Fill });
            Proportions.Add(new Proportion { Type = ProportionType.Auto });

            Widgets.Add(_toobar = new ToolbarControl());
            Widgets.Add(_preview = new MapPreview());
            Widgets.Add(_toolbox = new ToolboxControl());
        }

    }
}
