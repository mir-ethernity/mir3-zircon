using MapEditor.Interface.windows;
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
        private TextButton _configGridButton;

        public ToolboxControl()
        {
            Width = 200;
            Background = new SolidBrush(new Color(51, 51, 51));

            Widgets.Add(_configGridButton = new TextButton { Text = "Config Grid" });

            _configGridButton.Click += ConfigGridButton_Click;
        }

        private void ConfigGridButton_Click(object sender, EventArgs e)
        {
            var window = new GridConfigWindow();
            window.Show();
        }
    }
}
