//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using SpaceMiner.Sprites;

namespace SpaceMiner.Screens
{
    public class LevelOneScreen : GameScreen
    {
        private new SpaceMinerGame Game => (SpaceMinerGame)base.Game;

        private SpriteBatch _spriteBatch;

        private SoundEffect placeSprite;

        private List<IMinedSprite> asteroidList = new List<IMinedSprite>();
        private List<IPlayerStationSprite> placedSpriteList = new List<IPlayerStationSprite>();
        private IPlayerStationSprite unplacedSprite = null;

        private int levelHeight = 3200;
        private int levelWidth = 3200;
        private float zoom = 1.5f;

        private Point viewportPosition;

        private Matrix transform;

        public LevelOneScreen(SpaceMinerGame game) : base(game)
        {
            // Place the current viewport in the center of the screen
            viewportPosition = new Point(
                levelWidth / 2 - Game.BackBufferWidth / 2,
                levelHeight / 2 - Game.BackBufferHeight / 2
            );
        }
        
        public override void Initialize()
        {
            // Initialize Sprites
            asteroidList = new List<IMinedSprite>
            {
                new AsteroidSprite(new Vector2(1600, 1600), 800)
            };

            placedSpriteList = new List<IPlayerStationSprite>
            {
                // Add a pre-placed solar power sprite
                new SolarPowerSprite(new Vector2(1400, 1600), true, false)
            };

            // Add a non-pre-placed miner (will follow the cursor)
            unplacedSprite = new MinerSprite(new Vector2(0, 0), false, true);

            base.Initialize();
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
            // Respond to Zoom inputs
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
                Point mousePosition = Game.Input.CurrentMouseState.Position;
                Vector2 scaledMouse = Vector2.Transform(new Vector2(mousePosition.X, mousePosition.Y), Matrix.Invert(transform));
                unplacedSprite.Center = scaledMouse;

                if (Game.Input.CurrentMouseState.LeftButton == ButtonState.Pressed &&
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
                        unplacedSprite = new MinerSprite(scaledMouse, false, true);
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
                viewportPosition += Game.Input.CurrentMouseState.DeltaPosition;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Update matrix transformations
            Matrix zoomTranslation = Matrix.CreateTranslation(-viewportPosition.X - Game.BackBufferWidth / 2, -viewportPosition.Y - Game.BackBufferHeight / 2, 0);
            Matrix zoomScale = Matrix.CreateScale(zoom);
            Matrix viewportTranslation = Matrix.CreateTranslation(-viewportPosition.X, -viewportPosition.Y, 0);
            transform = zoomTranslation * zoomScale * Matrix.Invert(zoomTranslation) * viewportTranslation;

            // Draw Tilemap without transformations
            _spriteBatch.Begin();
            Game.Tilemap.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            // Draw inside spritebatch calls
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
        }
    }
}
