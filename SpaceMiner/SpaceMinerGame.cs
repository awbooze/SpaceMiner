//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using SpaceMiner.Screens;
using SpaceMiner.Sprites;
using System;
using System.Collections.Generic;

namespace SpaceMiner
{
    public class SpaceMinerGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private readonly ScreenManager _screenManager;

        public string GameTitle { get; private set; } = "Space Miner";
        public int BackBufferWidth => _graphics.PreferredBackBufferWidth;
        public int BackBufferHeight => _graphics.PreferredBackBufferHeight;

        public KeyboardState PriorKeyboardState { get; private set; }
        public KeyboardState CurrentKeyboardState { get; private set; }

        public MouseState PriorMouseState { get; private set; }
        public MouseState CurrentMouseState { get; private set; }

        public SpaceMinerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _screenManager = new ScreenManager();
            Components.Add(_screenManager);
        }

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

        protected override void LoadContent()
        {
            // Load the first screen
            _screenManager.LoadScreen(new SplashScreen(this));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            PriorKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            PriorMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            _screenManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _screenManager.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
