//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceMiner
{
    public class SpaceMinerGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont orbitron;
        private SpriteFont exo;

        public SpaceMinerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Add initialization logic
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 960;
            _graphics.PreferredBackBufferHeight = 540;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Use this.Content to load game content
            // Load fonts
            orbitron = Content.Load<SpriteFont>("Fonts/Orbitron");
            exo = Content.Load<SpriteFont>("Fonts/Exo");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Draw here
            _spriteBatch.Begin();

            string title = "Space Miner";
            Vector2 titleSize = orbitron.MeasureString(title);

            _spriteBatch.DrawString(orbitron, title, new Vector2((_graphics.PreferredBackBufferWidth / 2) - (titleSize.X / 2), 
                5), Color.White);

            _spriteBatch.DrawString(exo, "To exit, hit escape (or the back button on a controller)", new Vector2(5, 505), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
