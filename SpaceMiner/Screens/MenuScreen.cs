//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace SpaceMiner.Screens
{
    public class MenuScreen : GameScreen
    {
        private new SpaceMinerGame Game => (SpaceMinerGame)base.Game;

        private SpriteBatch _spriteBatch;
        private SpriteFont orbitron;
        private SpriteFont exo;

        private Vector2 titlePosition;
        private Vector2 instructionPosition;
        private string instructions = "Press Enter to Start";

        public MenuScreen(SpaceMinerGame game) : base(game)
        {
            // Nothing to call here
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts
            orbitron = Content.Load<SpriteFont>("Fonts/Orbitron");
            exo = Content.Load<SpriteFont>("Fonts/Exo");

            // Calculate sizes
            Vector2 titleSize = orbitron.MeasureString(Game.GameTitle);
            titlePosition = new Vector2((Game.BackBufferWidth / 2) - (titleSize.X / 2), 5);

            Vector2 instructionSize = exo.MeasureString(instructions);
            instructionPosition = new Vector2((Game.BackBufferWidth / 2) - (instructionSize.X / 2),
                (Game.BackBufferHeight / 2) - (instructionSize.Y / 2));

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Game.CurrentKeyboardState.IsKeyDown(Keys.Enter))
            {
                ScreenManager.LoadScreen(new LevelOneScreen(Game));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.DrawString(orbitron, Game.GameTitle, titlePosition, Color.White);
            _spriteBatch.DrawString(exo, instructions, instructionPosition, Color.White);
            _spriteBatch.DrawString(exo, "To exit, hit escape (or the back button on a controller)", new Vector2(5, 505), Color.White);

            _spriteBatch.End();
        }
    }
}
