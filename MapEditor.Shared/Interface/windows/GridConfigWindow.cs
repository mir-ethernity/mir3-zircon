using Microsoft.Xna.Framework;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapEditor.Interface.windows
{
    public class GridConfigWindow : Window
    {
        private VerticalStackPanel _content;
        private CheckBox _showGridCheckbox;
        private CheckBox _showNoWalkCheckbox;

        public GridConfigWindow()
        {
            Title = "Grid Config";

            Content = _content = new VerticalStackPanel();
            _content.Padding = new Myra.Graphics2D.Thickness(10);
            _content.Widgets.Add(_showGridCheckbox = new CheckBox
            {
                Text = "Show grid",
                IsPressed = Environment.ShowGrid,
            });

            _content.Widgets.Add(_showNoWalkCheckbox = new CheckBox
            {
                Text = "Show no-walk",
                IsPressed = Environment.ShowNoWalkGrid,
            });

            _showGridCheckbox.PressedChanged += ShowGridCheckbox_PressedChanged;
            _showNoWalkCheckbox.PressedChanged += ShowNoWalkCheckbox_PressedChanged;
        }

        private void ShowNoWalkCheckbox_PressedChanged(object sender, EventArgs e)
        {
            Environment.ShowNoWalkGrid = _showNoWalkCheckbox.IsPressed;
        }

        private void ShowGridCheckbox_PressedChanged(object sender, EventArgs e)
        {
            Environment.ShowGrid = _showGridCheckbox.IsPressed;
        }
    }
}
