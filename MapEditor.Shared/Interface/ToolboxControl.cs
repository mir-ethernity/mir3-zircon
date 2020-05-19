using Microsoft.Xna.Framework;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor.Interface
{
    public class ToolboxControl : VerticalStackPanel
    {
        public ToolboxControl()
        {
            Width = 200;
            Background = new SolidBrush(new Color(51, 51, 51));
        }
    }
}
