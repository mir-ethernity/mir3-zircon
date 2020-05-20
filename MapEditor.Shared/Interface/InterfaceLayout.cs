using Microsoft.Xna.Framework;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor.Interface
{
    public class InterfaceLayout : VerticalStackPanel
    {
        private MenuItem _menuFileOpen;
        private MapEditorControl _editor;

        public InterfaceLayout()
        {
            Proportions.Add(new Proportion { Type = ProportionType.Auto });
            Proportions.Add(new Proportion { Type = ProportionType.Fill });

            var mainMenu = new HorizontalMenu
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new SolidBrush(Color.Gray),
                Id = "MainMenu",
                Items =
                {
                    new MenuItem
                    {
                        Text = "&File",
                        Id = "MenuFile",
                        Items =
                        {
                            (_menuFileOpen = new MenuItem { Text = "&Open", ShortcutText = "Ctrl+O", Id = "MenuFileOpen" })
                        }
                    }
                }
            };

            Widgets.Add(mainMenu);
            Widgets.Add(_editor = new MapEditorControl());

            _menuFileOpen.Selected += MenuFileOpen_Selected;
        }

        private void MenuFileOpen_Selected(object sender, EventArgs e)
        {
            var fileDialog = new FileDialog(FileDialogMode.OpenFile);
            fileDialog.Filter = "*.map";
            fileDialog.ShowModal();

            fileDialog.Closed += (s, a) =>
            {
                if (!fileDialog.Result)
                {
                    return;
                }

                Environment.LoadMap(fileDialog.FilePath);
            };
        }
    }
}
