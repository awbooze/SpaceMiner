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
        public new SpaceMinerGame Game => (SpaceMinerGame)base.Game;

        private SpriteBatch _spriteBatch;

        private Texture2D oRing;
        private SoundEffect placeSprite;

        private List<IMinedSprite> asteroidList = new List<IMinedSprite>();
        private List<IPlayerStationSprite> placedSpriteList = new List<IPlayerStationSprite>();
        private IPlayerStationSprite unplacedSprite = null;

        public LevelOneScreen(SpaceMinerGame game) : base(game)
        {
            // Nothing here
        }
        
        public override void Initialize()
        {
            // Initialize Sprites
            asteroidList = new List<IMinedSprite>
            {
                new AsteroidSprite(new Vector2(550, 250), 800)
            };

            placedSpriteList = new List<IPlayerStationSprite>
            {
                // Add a pre-placed solar power sprite
                new SolarPowerSprite(new Vector2(400, 200), true, false)
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

            oRing = Content.Load<Texture2D>("Sprites/O-Ring Ship");
            placeSprite = Content.Load<SoundEffect>("Effects/blg_a_robo_08");

            // Lower the volume a bit so that people can hear sound effects
            MediaPlayer.Volume = 0.5f;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
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

            if (Game.Input.CurrentMouseState.WasButtonJustDown(MonoGame.Extended.Input.MouseButton.Left)
                && unplacedSprite != null && unplacedSprite.CanPlace)
            {
                // Place the player station sprite
                unplacedSprite.Placed = true;
                unplacedSprite.Selected = false;
                placedSpriteList.Add(unplacedSprite);
                placeSprite.Play(1.0f, 0, 0);

                if (Game.Input.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
                {
                    // Place multiple sprites, so create another one
                    unplacedSprite = new MinerSprite(new Vector2(Game.Input.CurrentMouseState.X, Game.Input.CurrentMouseState.Y), false, true);
                    unplacedSprite.LoadContent(Content);
                }
                else
                {
                    // Only place one sprite
                    unplacedSprite = null;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw inside spritebatch calls
            _spriteBatch.Begin();

            Game.Tilemap.Draw(gameTime, _spriteBatch);

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

            // Draw the sprites I haven't abstracted yet
            _spriteBatch.Draw(oRing, new Vector2(800, 100), new Rectangle(0, 0, 64, 64), Color.White);

            _spriteBatch.End();
        }
    }
}
