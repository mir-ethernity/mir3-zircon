using MapEditor.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using System;

namespace MapEditor
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MapEditorGame : Game
    {
        GraphicsDeviceManager graphics;
        private TimeSpan _animationTime = TimeSpan.Zero;
        private int _speedMove = 1;
        private TimeSpan _nextIncrementSpeed = TimeSpan.Zero;

        public MapEditorGame()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1200,
                PreferredBackBufferHeight = 800
            };
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }


        protected override void LoadContent()
        {
            MyraEnvironment.Game = this;
            Environment.GraphicsDevice = GraphicsDevice;

            FontManager.Normal = Content.Load<SpriteFont>("fonts/normal");
            
            GraphicsManager.Load(GraphicsDevice);

            Desktop.HasExternalTextInput = true;

            Desktop.Root = new InterfaceLayout();

            Window.TextInput += (s, a) =>
            {
                Desktop.OnChar(a.Character);
            };

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Environment.Time = gameTime;

            ProcessKeyboard();
            Environment.UpdateMouseTile();

            if (_animationTime == TimeSpan.Zero)
                _animationTime = gameTime.TotalGameTime.Add(TimeSpan.FromMilliseconds(100));

            if (_animationTime <= gameTime.TotalGameTime)
            {
                Environment.MapAnimation++;
                if (Environment.MapAnimation == 10000)
                    Environment.MapAnimation = 0;
                _animationTime = gameTime.TotalGameTime.Add(TimeSpan.FromMilliseconds(100));
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void ProcessKeyboard()
        {
            var state = Keyboard.GetState();

            var pressedW = state.IsKeyDown(Keys.W);
            var pressedA = state.IsKeyDown(Keys.A);
            var pressedS = state.IsKeyDown(Keys.S);
            var pressedD = state.IsKeyDown(Keys.D);

            if (!pressedW && !pressedA && !pressedS && !pressedD)
            {
                _nextIncrementSpeed = TimeSpan.Zero;
                _speedMove = 1;
                return;
            }

            if (_nextIncrementSpeed == TimeSpan.Zero)
                _nextIncrementSpeed = Environment.Time.TotalGameTime.Add(TimeSpan.FromMilliseconds(100));

            if (_nextIncrementSpeed <= Environment.Time.TotalGameTime && _speedMove < 50)
            {
                _nextIncrementSpeed = Environment.Time.TotalGameTime.Add(TimeSpan.FromMilliseconds(100));
                _speedMove += 2;
            }

            var x = Environment.UserScreenX;
            var y = Environment.UserScreenY;

            if (pressedW)
                y -= _speedMove;
            if (pressedA)
                x -= _speedMove;
            if (pressedS)
                y += _speedMove;
            if (pressedD)
                x += _speedMove;

            Environment.UpdateUserScreen(x, y);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.Black);

            Desktop.Render();
        }
    }
}
