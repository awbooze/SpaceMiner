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

namespace SpaceMiner.Screens
{
    public class MenuScreen : GameScreen
    {
        public new SpaceMinerGame Game => (SpaceMinerGame)base.Game;

        public SpriteBatch SpriteBatch { get; private set; }

        private Vector2 _titlePosition;
        private int _selectedItem;
        private readonly List<MenuItem> _menuItems = new List<MenuItem>();

        private readonly InputAction _menuUp;
        private readonly InputAction _menuDown;
        private readonly InputAction _menuSelect;
        private readonly InputAction _menuCancel;

        public MenuScreen(SpaceMinerGame game) : base(game)
        {
            _menuUp = new InputAction(
                new[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new[] { Keys.Up, Keys.W },
                null,
                true
            );
            _menuDown = new InputAction(
                new[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new[] { Keys.Down, Keys.S },
                null,
                true
            );
            _menuSelect = new InputAction(
                new[] { Buttons.A, Buttons.Start },
                new[] { Keys.Enter, Keys.Space },
                null,
                true
             );
            _menuCancel = new InputAction(
                new[] { Buttons.B, Buttons.Back },
                new[] { Keys.Back, Keys.Escape },
                null,
                true
            );

            MenuItem playItem = new MenuItem("Play Game");
            MenuItem exitItem = new MenuItem("Exit Game");

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

            if (_menuUp.Occurred(Game.Input))
            {
                _selectedItem--;

                if (_selectedItem < 0)
                {
                    _selectedItem = _menuItems.Count - 1;
                }
            }
            
            if (_menuDown.Occurred(Game.Input))
            {
                _selectedItem++;

                if (_selectedItem >= _menuItems.Count)
                {
                    _selectedItem = 0;
                }
            }
            
            if (_menuSelect.Occurred(Game.Input))
            {
                _menuItems[_selectedItem].OnSelectEntry();
            }
            else if (_menuCancel.Occurred(Game.Input))
            {
                Game.Exit();
            }

            // Update each nested MenuEntry object
            for (int i = 0; i < _menuItems.Count; i++)
            {
                bool isSelected = i == _selectedItem;
                _menuItems[i].Update(this, isSelected, gameTime);
            }
        }

        private void UpdateMenuEntryLocations()
        {
            // Start at Y = 200; each X value is generated per entry
            var position = new Vector2(0f, 200f);

            // Update each menu entry's location in turn
            foreach (var menuItem in _menuItems)
            {
                // Each entry is to be centered horizontally
                position.X = Game.BackBufferWidth / 2 - menuItem.GetWidth(this) / 2;

                // Set the entry's position
                menuItem.Position = position;

                // Move down for the next entry the size of this entry
                position.Y += menuItem.GetHeight(this);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();

            Game.Tilemap.Draw(gameTime, SpriteBatch);

            for (int i = 0; i < _menuItems.Count; i++)
            {
                bool isSelected = i == _selectedItem;
                _menuItems[i].Draw(this, isSelected, gameTime);
            }

            SpriteBatch.DrawString(Game.TitleFont, Game.GameTitle, _titlePosition, Color.White);
            SpriteBatch.DrawString(Game.GeneralFont, "To exit, hit escape (or the back button on a controller)", new Vector2(5, 505), Color.White);

            SpriteBatch.End();
        }
    }
}
