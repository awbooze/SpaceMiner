//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using SpaceMiner.Screens;
using SpaceMiner.Sprites;
using System;
using System.Collections.Generic;

namespace SpaceMiner
{
    /// <summary>
    /// The Game class for this Monogame game.
    /// </summary>
    public class SpaceMinerGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private readonly ScreenManager _screenManager;
        private Texture2D lineTexture;

        /// <summary>
        /// The title of the game. Displayed in-game as the game title and at
        /// the top of the window.
        /// </summary>
        public string GameTitle { get; private set; } = "Space Miner";

        /// <summary>
        /// The graphics device's preferred width.
        /// </summary>
        public int BackBufferWidth => _graphics.PreferredBackBufferWidth;

        /// <summary>
        /// The graphics device's preferred height.
        /// </summary>
        public int BackBufferHeight => _graphics.PreferredBackBufferHeight;

        #region Input State
        /// <summary>
        /// The previous tick's GamePadState.
        /// </summary>
        public GamePadState PriorGamePadState { get; private set; }

        /// <summary>
        /// The current tick's GamePadState.
        /// </summary>
        public GamePadState CurrentGamePadState { get; private set; }

        /// <summary>
        /// The previous tick's KeyboardState.
        /// </summary>
        public KeyboardState PriorKeyboardState { get; private set; }

        /// <summary>
        /// The current tick's KeyboardState.
        /// </summary>
        public KeyboardState CurrentKeyboardState { get; private set; }

        /// <summary>
        /// The previous tick's MouseState.
        /// </summary>
        public MouseState PriorMouseState { get; private set; }

        /// <summary>
        /// The current tick's MouseState.
        /// </summary>
        public MouseState CurrentMouseState { get; private set; }
        #endregion

        public SpaceMinerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _screenManager = new ScreenManager();
            Components.Add(_screenManager);
        }

        /// <summary>
        /// Initialize the game, including setting up the graphical size and
        /// the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            // Add initialization logic
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 960;
            _graphics.PreferredBackBufferHeight = 540;
            _graphics.ApplyChanges();

            Window.Title = GameTitle;

            _screenManager.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// Load some content for this game, including the first screen.
        /// </summary>
        protected override void LoadContent()
        {
            // Load the texture for generic lines
            lineTexture = Content.Load<Texture2D>("Sprites/1x1");

            // Load sounds and music
            Song flow = Content.Load<Song>("Music/Flow");
            MediaPlayer.Volume = 0.8f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(flow);

            // Load the first screen
            _screenManager.LoadScreen(new SplashScreen(this));
        }

        /// <summary>
        /// Update the current game state, followed by a call to the
        /// <see cref="Draw(GameTime)">Draw</see> method directly after these
        /// operations.
        /// </summary>
        /// <param name="gameTime">The time state of this game.</param>
        protected override void Update(GameTime gameTime)
        {
            PriorKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            PriorMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            PriorGamePadState = CurrentGamePadState;
            CurrentGamePadState = GamePad.GetState(PlayerIndex.One);

            if (CurrentGamePadState.Buttons.Back == ButtonState.Pressed || CurrentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _screenManager.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the current game state right after the
        /// <see cref="Update(GameTime)">Update</see> method.
        /// </summary>
        /// <param name="gameTime">The time state of this game.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _screenManager.Draw(gameTime);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws a line between two points. Originally from a Stack Overflow
        /// answer by Cyral (https://stackoverflow.com/a/16407171/10906388)
        /// licensed under CC BY-SA 3.0.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch instance to draw with</param>
        /// <param name="begin">The beginning point</param>
        /// <param name="end">The ending point</param>
        /// <param name="color">The color to tint the line, which defaults to the default (White).</param>
        /// <param name="width">The width to draw the line, which defaults to one.</param>
        public void DrawLine(SpriteBatch spriteBatch, Vector2 begin, Vector2 end, Color color = default, int width = 1)
        {
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y)
            {
                angle = MathHelper.TwoPi - angle;
            }
            spriteBatch.Draw(lineTexture, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
