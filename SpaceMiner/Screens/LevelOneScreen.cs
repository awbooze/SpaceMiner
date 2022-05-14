//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using SpaceMiner.Input;
using SpaceMiner.Sprites;

namespace SpaceMiner.Screens
{
    public class LevelOneScreen : GameScreen
    {
        public new SpaceMinerGame Game => (SpaceMinerGame)base.Game;

        private SpriteBatch _spriteBatch;

        private SoundEffect placeSprite;

        private List<IMinedSprite> asteroidList = new List<IMinedSprite>();
        private List<IPlayerStationSprite> placedSpriteList = new List<IPlayerStationSprite>();
        private IPlayerStationSprite unplacedSprite = null;
        private List<Button> _buttonList = new List<Button>();
        private readonly InputAction _buttonSelect;

        private int _totalMinerals = 0;
        private int _mineralsMined = 0;
        private int _mineralsAvailable = 0;

        private int levelHeight = 3200;
        private int levelWidth = 3200;
        private float zoom = 1.5f;

        private Point _viewportCenter;
        private Vector2 _scaledPosition;

        private Matrix transform;

        public LevelOneScreen(SpaceMinerGame game) : base(game)
        {
            // Place the current viewport in the center of the screen
            _viewportCenter = new Point(
                levelWidth / 2 - Game.BackBufferWidth / 2,
                levelHeight / 2 - Game.BackBufferHeight / 2
            );

            _buttonSelect = new InputAction(
                new[] { Buttons.A, Buttons.Start },
                new[] { Keys.Enter, Keys.Space },
                new[] { MouseButton.Left },
                true
             );
        }
        
        public override void Initialize()
        {
            // Reset elapsed time to zero for timekeeping
            Game.ResetElapsedTime();

            // Initialize Sprites
            asteroidList = new List<IMinedSprite>
            {
                new AsteroidSprite(new Vector2(1600, 1600), 800),
                new AsteroidSprite(new Vector2(1500, 1100), 500),
                new AsteroidSprite(new Vector2(1600, 1350), 400),
                new AsteroidSprite(new Vector2(1700, 1675), 825),
                new AsteroidSprite(new Vector2(1600, 1750), 1000),
                new AsteroidSprite(new Vector2(1450, 1700), 750),
                new AsteroidSprite(new Vector2(1450, 1800), 350),
                new AsteroidSprite(new Vector2(1700, 1400), 900),
                new AsteroidSprite(new Vector2(1850, 1350), 500),
                new AsteroidSprite(new Vector2(1900, 1600), 400),
                new AsteroidSprite(new Vector2(1200, 1600), 1000)
            };

            // Calculate total minerals available
            foreach (IMinedSprite sprite in asteroidList)
            {
                _totalMinerals += sprite.CurrentMinerals;
            }

            placedSpriteList = new List<IPlayerStationSprite>
            {
                // Add a pre-placed solar power sprite
                new SolarPowerSprite(new Vector2(1400, 1600), true, false)
            };

            // Add a non-pre-placed miner (will follow the cursor)
            unplacedSprite = new MinerSprite(new Vector2(0, 0), false, true);

            Button solarButton = new Button(Game.GeneralFont, "New Solar Power Plant") 
            {
                Center = new Vector2(200, Game.BackBufferHeight - 15)
            };
            solarButton.Selected += SolarButton_Selected;
            _buttonList.Add(solarButton);

            Button minerButton = new Button(Game.GeneralFont, "New Miner")
            {
                Center = new Vector2(450, Game.BackBufferHeight - 15)
            };
            minerButton.Selected += MinerButton_Selected;
            _buttonList.Add(minerButton);

            base.Initialize();
        }

        private void SolarButton_Selected(object sender, EventArgs e)
        {
            unplacedSprite = new SolarPowerSprite(_scaledPosition, false, true);
            unplacedSprite.LoadContent(Content);
        }

        private void MinerButton_Selected(object sender, EventArgs e)
        {
            unplacedSprite = new MinerSprite(_scaledPosition, false, true);
            unplacedSprite.LoadContent(Content);
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load sprite content
            foreach (IMinedSprite sprite in asteroidList)
            {
                sprite.LoadContent(Content);
            }

            foreach (IPlayerStationSprite sprite in placedSpriteList)
            {
                sprite.LoadContent(Content);
            }

            if (unplacedSprite != null)
            {
                unplacedSprite.LoadContent(Content);
            }

            placeSprite = Content.Load<SoundEffect>("Effects/blg_a_robo_08");

            // Lower the volume a bit so that people can hear sound effects
            MediaPlayer.Volume = 0.5f;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // Respond to mouse inputs
            if (Game.Input.CurrentMouseState.DeltaScrollWheelValue != 0)
            {
                zoom -= ((float)Game.Input.CurrentMouseState.DeltaScrollWheelValue) / 2400;

                if (zoom < 0.1f)
                {
                    zoom = 0.1f;
                }
                else if (zoom > 5f)
                {
                    zoom = 5f;
                }
            }

            Point mousePosition = Game.Input.Position;
            _scaledPosition = Vector2.Transform(new Vector2(mousePosition.X, mousePosition.Y), Matrix.Invert(transform));

            // Update logic here
            if (unplacedSprite != null)
            {
                // Updates unplacedSprite.CanPlace to true, among other things
                unplacedSprite.Update(gameTime);
            }

            foreach (IPlayerStationSprite sprite in placedSpriteList)
            {
                sprite.Update(gameTime);

                if (unplacedSprite != null)
                {
                    if (unplacedSprite.CanPlace != false &&
                        sprite.Bounds.CollidesWith(unplacedSprite.Bounds))
                    {
                        unplacedSprite.CanPlace = false;
                    }

                    // Connect sprites to each other if they can transmit power
                    if (sprite.CanTransmitPower)
                    {
                        // Calculate distance between sprites and whether or not it is larger than the maximum
                        // connection distance either sprite can make.
                        float distance = Vector2.Distance(unplacedSprite.Bounds.Center, sprite.Bounds.Center);
                        int maxConnectionDistance = 
                            unplacedSprite.MaxConnectionDistance > sprite.MaxConnectionDistance ? 
                            unplacedSprite.MaxConnectionDistance : 
                            sprite.MaxConnectionDistance;

                        // Put them on the list if inside the range
                        if (!unplacedSprite.NearbyStations.Contains(sprite) && distance < maxConnectionDistance)
                        {
                            unplacedSprite.NearbyStations.Add(sprite);
                            sprite.NearbyStations.Add(unplacedSprite);
                        }
                        // Take them off of the list if they move outside of range
                        else if (unplacedSprite.NearbyStations.Contains(sprite) && distance > maxConnectionDistance)
                        {
                            unplacedSprite.NearbyStations.Remove(sprite);
                            sprite.NearbyStations.Remove(unplacedSprite);
                        }
                    }
                }
            }

            foreach (IMinedSprite sprite in asteroidList)
            {
                sprite.Update(gameTime);

                if (unplacedSprite != null)
                {
                    if (unplacedSprite.CanPlace != false &&
                        sprite.Bounds.CollidesWith(unplacedSprite.Bounds))
                    {
                        unplacedSprite.CanPlace = false;
                    }

                    // Connect miner sprites to asteroids
                    if (unplacedSprite is MinerSprite miner)
                    {
                        float distance = Vector2.Distance(miner.Bounds.Center, sprite.Bounds.Center);

                        // Put them on the list if inside the range
                        if (!miner.NearbyAsteroids.Contains(sprite) && distance < miner.MaxConnectionDistance)
                        {
                            miner.NearbyAsteroids.Add(sprite);
                        }
                        // Take them off of the list if they move outside of range
                        else if (miner.NearbyAsteroids.Contains(sprite) && distance > miner.MaxConnectionDistance)
                        {
                            miner.NearbyAsteroids.Remove(sprite);
                        }
                    }
                }
            }

            // Mouse Actions
            if (unplacedSprite != null)
            {
                unplacedSprite.Center = _scaledPosition;

                if (Game.Input.CurrentMouseState.WasButtonJustDown(MouseButton.Left) &&
                    unplacedSprite.CanPlace)
                {
                    // Place the player station sprite
                    unplacedSprite.Placed = true;
                    unplacedSprite.Selected = false;
                    placedSpriteList.Add(unplacedSprite);
                    placeSprite.Play(1.0f, 0, 0);

                    if (Game.Input.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
                    {
                        // Place multiple sprites, so create another one
                        if (unplacedSprite is MinerSprite)
                        {
                            unplacedSprite = new MinerSprite(_scaledPosition, false, true);
                        }
                        else if (unplacedSprite is SolarPowerSprite)
                        {
                            unplacedSprite = new SolarPowerSprite(_scaledPosition, false, true);
                        }
                        else
                        {
                            // TODO: Make sure to handle this if I add another type of sprite!
                            unplacedSprite = null;
                        }
                        unplacedSprite.LoadContent(Content);
                    }
                    else
                    {
                        // Only place one sprite
                        unplacedSprite = null;
                    }
                }
            }
            
            if (Game.Input.CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                // Move the background
                _viewportCenter += Game.Input.CurrentMouseState.DeltaPosition;
            }

            // Update buttons
            if (_buttonSelect.Occurred(Game.Input))
            {
                _buttonList.FirstOrDefault(b => b.Hovered)?.OnSelectEntry();
            }

            foreach (Button button in _buttonList)
            {
                if (button.Bounds.CollidesWith(Game.Input.Position))
                {
                    button.Hovered = true;
                }
                else
                {
                    button.Hovered = false;
                }
                button.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Update matrix transformations
            Matrix zoomTranslation = Matrix.CreateTranslation(-_viewportCenter.X - Game.BackBufferWidth / 2, -_viewportCenter.Y - Game.BackBufferHeight / 2, 0);
            Matrix zoomScale = Matrix.CreateScale(zoom);
            Matrix viewportTranslation = Matrix.CreateTranslation(-_viewportCenter.X, -_viewportCenter.Y, 0);
            transform = zoomTranslation * zoomScale * Matrix.Invert(zoomTranslation) * viewportTranslation;

            // Draw Tilemap without transformations underneath everything
            _spriteBatch.Begin();
            Game.Tilemap.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            // Draw transformmed objects
            _spriteBatch.Begin(transformMatrix: transform);

            foreach (IMinedSprite sprite in asteroidList)
            {
                sprite.Draw(gameTime, _spriteBatch);
            }

            foreach (IPlayerStationSprite sprite in placedSpriteList)
            {
                // Draw all of the placed sprites
                sprite.Draw(Game, gameTime, _spriteBatch);
            }

            if (unplacedSprite != null)
            {
                // Draw the unplaced sprite, if it exists
                unplacedSprite.Draw(Game, gameTime, _spriteBatch);
            }

            _spriteBatch.End();

            // Draw text over top of everything else
            _spriteBatch.Begin();

            string time = $"{gameTime.TotalGameTime:mm\\:ss}";
            _spriteBatch.DrawString(Game.SmallFont, time, new Vector2(5, 5), Color.White);

            string minedText = $"{_mineralsMined}/{_totalMinerals} minerals mined";
            Vector2 minedTextLength = Game.SmallFont.MeasureString(minedText);
            _spriteBatch.DrawString(Game.SmallFont, minedText,
                new Vector2(Game.BackBufferWidth - 5 - minedTextLength.X, 5), Color.White);

            foreach (Button button in _buttonList)
            {
                button.Draw(gameTime, _spriteBatch);
            }

            _spriteBatch.End();
        }
    }
}
