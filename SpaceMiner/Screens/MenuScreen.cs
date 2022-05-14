//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using SpaceMiner.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceMiner.Screens
{
    public class MenuScreen : GameScreen
    {
        public new SpaceMinerGame Game => (SpaceMinerGame)base.Game;

        public SpriteBatch SpriteBatch { get; private set; }

        private Vector2 _titlePosition;

        private readonly List<Button> _menuItems = new List<Button>();
        private readonly InputAction _menuSelect;
        private readonly InputAction _menuCancel;

        public MenuScreen(SpaceMinerGame game) : base(game)
        {
            _menuSelect = new InputAction(
                new[] { Buttons.A, Buttons.Start },
                new[] { Keys.Enter, Keys.Space },
                new[] { MouseButton.Left },
                true
             );
            _menuCancel = new InputAction(
                new[] { Buttons.B, Buttons.Back },
                new[] { Keys.Back, Keys.Escape },
                null,
                true
            );

            Button playItem = new Button(Game.GeneralFont, "Level 1");
            Button exitItem = new Button(Game.GeneralFont, "Exit");

            playItem.Selected += PlayGameMenuItemSelected;
            exitItem.Selected += ExitMenuItemSelected;

            _menuItems.Add(playItem);
            _menuItems.Add(exitItem);
        }

        private void PlayGameMenuItemSelected(object sender, EventArgs e)
        {
            ScreenManager.LoadScreen(new LevelOneScreen(Game));
        }

        private void ExitMenuItemSelected(object sender, EventArgs e)
        {
            Game.Exit();
        }

        public override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Calculate title size
            Vector2 titleSize = Game.TitleFont.MeasureString(Game.GameTitle);
            _titlePosition = new Vector2((Game.BackBufferWidth / 2) - (titleSize.X / 2), 5);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // Make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            if (_menuSelect.Occurred(Game.Input))
            {
                _menuItems.FirstOrDefault(b => b.Hovered)?.OnSelectEntry();
            }
            else if (_menuCancel.Occurred(Game.Input))
            {
                Game.Exit();
            }

            // Update each nested MenuEntry object
            for (int i = 0; i < _menuItems.Count; i++)
            {
                _menuItems[i].Update(gameTime);
            }
        }

        private void UpdateMenuEntryLocations()
        {
            // Start at Y = 200; each X value is generated per entry
            var position = new Vector2(0f, 200f);

            // Update each menu entry's location in turn
            foreach (Button menuItem in _menuItems)
            {
                // Each entry is to be centered horizontally
                position.X = Game.BackBufferWidth / 2 - menuItem.GetSize().X / 2;

                // Set the entry's position
                menuItem.Center = position;

                // Move down for the next entry the size of this entry
                position.Y += menuItem.GetSize().Y;

                if (menuItem.Bounds.CollidesWith(Game.Input.Position))
                {
                    menuItem.Hovered = true;
                }
                else
                {
                    menuItem.Hovered = false;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();

            Game.Tilemap.Draw(gameTime, SpriteBatch);

            for (int i = 0; i < _menuItems.Count; i++)
            {
                _menuItems[i].Draw(gameTime, SpriteBatch);
            }

            SpriteBatch.DrawString(Game.TitleFont, Game.GameTitle, _titlePosition, Color.White);
            SpriteBatch.DrawString(Game.GeneralFont, "To exit, hit escape (or the back button on a controller)", new Vector2(5, 505), Color.White);

            SpriteBatch.End();
        }
    }
}
