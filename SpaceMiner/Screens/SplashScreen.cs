//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace SpaceMiner.Screens
{
    public class SplashScreen : GameScreen
    {
        public new SpaceMinerGame Game => (SpaceMinerGame)base.Game;

        private SpriteBatch _spriteBatch;

        private Vector2 titlePosition;
        private Vector2 loadingPosition;
        private string loading = "Loading ...";

        private readonly TimeSpan splashTime = new TimeSpan(0, 0, 2);

        public SplashScreen(SpaceMinerGame game) : base(game)
        {
            // Nothing to call here
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Calculate sizes
            Vector2 titleSize = Game.TitleFont.MeasureString(Game.GameTitle);
            titlePosition = new Vector2((Game.BackBufferWidth / 2) - (titleSize.X / 2), 5);

            Vector2 loadingSize = Game.GeneralFont.MeasureString(loading);
            loadingPosition = new Vector2((Game.BackBufferWidth / 2) - (loadingSize.X / 2),
                (Game.BackBufferHeight) - (loadingSize.Y));

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // If two seconds have passed, move to the MenuScreen
            if (gameTime.TotalGameTime > splashTime)
            {
                ScreenManager.LoadScreen(new MenuScreen(Game));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            Game.Tilemap.Draw(gameTime, _spriteBatch);

            _spriteBatch.DrawString(Game.TitleFont, Game.GameTitle, titlePosition, Color.White);
            _spriteBatch.DrawString(Game.GeneralFont, loading, loadingPosition, Color.White);

            _spriteBatch.End();
        }
    }
}
