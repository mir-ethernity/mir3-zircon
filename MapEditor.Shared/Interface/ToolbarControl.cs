using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapEditor.Interface
{
    public class ToolbarControl : VerticalStackPanel
    {
        private ImageButton _drag;
        private ImageButton _select;

        public ToolbarControl()
        {
            Widgets.Add(_drag = new ImageButton
            {
                Image = new TextureRegion(ContentManager.IconDrag)
            });
            _drag.Click += (s, e) => Environment.CursorTool = CursorTool.Drag;

            Widgets.Add(_select = new ImageButton
            {
                Image = new TextureRegion(ContentManager.IconDrag)
            });
            _select.Click += (s, e) => Environment.CursorTool = CursorTool.Selection;
        }
    }
}
