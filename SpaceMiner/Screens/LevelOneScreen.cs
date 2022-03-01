//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using SpaceMiner.Sprites;

namespace SpaceMiner.Screens
{
    public class LevelOneScreen : GameScreen
    {
        private new SpaceMinerGame Game => (SpaceMinerGame)base.Game;

        private SpriteBatch _spriteBatch;

        private Texture2D one, oRing;

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
                // Add a pre-placed miner
                new MinerSprite(new Vector2(475, 200), true, false),

                new MinerSprite(new Vector2(525, 200), true, false),

                new SolarPowerSprite(new Vector2(250, 200), true, false)
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

            one = Content.Load<Texture2D>("Sprites/1x1");
            oRing = Content.Load<Texture2D>("Sprites/O-Ring Ship");

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

                if (unplacedSprite != null &&
                    unplacedSprite.CanPlace != false &&
                    sprite.Bounds.CollidesWith(unplacedSprite.Bounds))
                {
                    unplacedSprite.CanPlace = false;
                }
            }

            foreach (IMinedSprite sprite in asteroidList)
            {
                sprite.Update(gameTime);

                if (unplacedSprite != null &&
                    unplacedSprite.CanPlace != false &&
                    sprite.Bounds.CollidesWith(unplacedSprite.Bounds))
                {
                    unplacedSprite.CanPlace = false;
                }
            }

            if (Game.CurrentMouseState.LeftButton == ButtonState.Pressed && unplacedSprite != null &&
                unplacedSprite.CanPlace)
            {
                // Place the player station sprite
                unplacedSprite.Placed = true;
                unplacedSprite.Selected = false;
                placedSpriteList.Add(unplacedSprite);

                if (Game.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
                {
                    // Place multiple sprites, so create another one
                    unplacedSprite = new MinerSprite(new Vector2(Game.CurrentMouseState.X, Game.CurrentMouseState.Y), false, true);
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

            // TODO: Abstract these into more classes
            DrawLine(_spriteBatch, new Vector2(475, 200), new Vector2(550, 250), one, Color.Red, 1);
            foreach (IMinedSprite sprite in asteroidList)
            {
                sprite.Draw(gameTime, _spriteBatch);
            }

            foreach (IPlayerStationSprite sprite in placedSpriteList)
            {
                // Draw all of the placed sprites
                sprite.Draw(gameTime, _spriteBatch);
            }

            if (unplacedSprite != null)
            {
                // Draw the unplaced sprite, if it exists
                unplacedSprite.Draw(gameTime, _spriteBatch);
            }

            // Draw the sprites I haven't abstracted yet
            _spriteBatch.Draw(oRing, new Vector2(800, 100), new Rectangle(0, 0, 64, 64), Color.White);

            _spriteBatch.End();
        }

        /// <summary>
        /// Draws a line between two points. Originally from a Stack Overflow answer by Cyral 
        /// (https://stackoverflow.com/a/16407171/10906388) licensed under CC BY-SA 3.0.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch instance to draw with</param>
        /// <param name="begin">The beginning point</param>
        /// <param name="end">The ending point</param>
        /// <param name="texture">The texture to draw</param>
        /// <param name="color">The color to tint the line</param>
        /// <param name="width">The width to draw the line, which defaults to one.</param>
        public void DrawLine(SpriteBatch spriteBatch, Vector2 begin, Vector2 end, Texture2D texture, Color color = default, int width = 1)
        {
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(texture, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
