using MapEditor.Interface.layers;
using Microsoft.Xna.Framework;
using Mir.Ethernity.ImageLibrary;
using Mir.Ethernity.MapLibrary;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MapEditor.Interface
{
    public class MapPreview : Panel
    {
        private MapFloorLayer _floor;
        private MapMiddleLayer _middle;
        private MapGridLayer _grid;
        private MapFrontLayer _front;

        private Point _mousePosition;
        private Label _label;

        public MapPreview()
        {
            ClipToBounds = true;

            Widgets.Add(_floor = new MapFloorLayer());
            Widgets.Add(_middle = new MapMiddleLayer());
            Widgets.Add(_front = new MapFrontLayer());
            Widgets.Add(_grid = new MapGridLayer());

            Widgets.Add(_label = new Label() { TextColor = Color.White });

            TouchDown += MapPreview_TouchDown;
            TouchMoved += MapPreview_TouchMoved;
            TouchUp += MapPreview_TouchUp;
        }

        private void MapPreview_TouchUp(object sender, EventArgs e)
        {
            if (Environment.CursorTool == CursorTool.Selection)
            {
                if (Environment.SelectedCells.Count == 0 || Environment.AroundCell(Environment.MouseX, Environment.MouseY))
                    Environment.SelectedCells.Add(new Point(Environment.MouseX, Environment.MouseY));
            }
        }

        private void MapPreview_TouchDown(object sender, EventArgs e)
        {
            _mousePosition = Desktop.TouchPosition;
        }

        private void MapPreview_TouchMoved(object sender, EventArgs e)
        {
            if (Environment.CursorTool == CursorTool.Drag)
            {
                var movedPoint = Desktop.TouchPosition - _mousePosition;
                _mousePosition = Desktop.TouchPosition;
                Environment.UpdateUserScreen(Environment.UserScreenX - movedPoint.X, Environment.UserScreenY - movedPoint.Y);
            }
        }

        public override void UpdateLayout()
        {
            base.UpdateLayout();

            Environment.OffsetX = Bounds.Width / 2 / Environment.CellWidth;
            Environment.OffsetY = Bounds.Height / 2 / Environment.CellHeight;

            _label.Text = $"User: {Environment.UserX}:{Environment.UserY}, Mouse: {Environment.MouseX}:{Environment.MouseY}";
        }
    }
}
