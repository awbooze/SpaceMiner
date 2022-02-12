//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceMiner.Sprites;
using System;
using System.Collections.Generic;

namespace SpaceMiner
{
    public class SpaceMinerGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont orbitron;
        private SpriteFont exo;

        private Texture2D power, miningLaser, asteroid, oRing;

        private List<IPlayerStationSprite> placedSpriteList = new List<IPlayerStationSprite>();
        private IPlayerStationSprite unplacedSprite = null;

        private string title = "Space Miner";
        private double animationTimer;
        private short animationFrame = 1;

        private MouseState priorMouseState;
        private MouseState currentMouseState;

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

            Window.Title = title;

            // Initialize Sprites
            placedSpriteList = new List<IPlayerStationSprite>
            {
                // Add a pre-placed miner
                new MinerSprite(new Vector2(475, 200), true, false),

                new MinerSprite(new Vector2(525, 200), true, false)
            };

            // Add a non-pre-placed miner (will follow the cursor)
            unplacedSprite = new MinerSprite(new Vector2(0, 0), false, true);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Use this.Content to load game content
            // Load fonts
            orbitron = Content.Load<SpriteFont>("Fonts/Orbitron");
            exo = Content.Load<SpriteFont>("Fonts/Exo");

            // Load sprite content
            power = Content.Load<Texture2D>("Sprites/Solar Power Plant");
            
            foreach (IPlayerStationSprite sprite in placedSpriteList)
            {
                sprite.LoadContent(Content);
            }

            if (unplacedSprite != null)
            {
                unplacedSprite.LoadContent(Content);
            }

            miningLaser = Content.Load<Texture2D>("Sprites/Mining Laser");
            asteroid = Content.Load<Texture2D>("Sprites/Asteroid");
            oRing = Content.Load<Texture2D>("Sprites/O-Ring Ship");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            priorMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            // Update logic here
            if (unplacedSprite != null)
            {
                unplacedSprite.Update(gameTime);
            }

            foreach (IPlayerStationSprite sprite in placedSpriteList)
            {
                sprite.Update(gameTime);

                if (unplacedSprite != null && sprite.Bounds.CollidesWith(unplacedSprite.Bounds))
                {
                    unplacedSprite.CanPlace = false;
                }
            }

            if (currentMouseState.LeftButton == ButtonState.Pressed && unplacedSprite != null && 
                unplacedSprite.CanPlace)
            {
                // Place the player station sprite
                unplacedSprite.Placed = true;
                unplacedSprite.Selected = false;
                placedSpriteList.Add(unplacedSprite);
                unplacedSprite = null;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);

            // Draw here
            _spriteBatch.Begin();

            Vector2 titleSize = orbitron.MeasureString(title);

            _spriteBatch.DrawString(orbitron, title, new Vector2((_graphics.PreferredBackBufferWidth / 2) - (titleSize.X / 2), 
                5), Color.White);

            _spriteBatch.DrawString(exo, "To exit, hit escape (or the back button on a controller)", new Vector2(5, 505), Color.White);
            
            // TODO: Abstract these into classes
            _spriteBatch.Draw(power, new Vector2(250, 200), Color.White);
            DrawLine(_spriteBatch, new Vector2(475, 200), new Vector2(500 + 64, 200 + 64), miningLaser);
            foreach (IPlayerStationSprite sprite in placedSpriteList)
            {
                sprite.Draw(gameTime, _spriteBatch);
            }

            if (unplacedSprite != null)
            {
                unplacedSprite.Draw(gameTime, _spriteBatch);
            }
            _spriteBatch.Draw(oRing, new Vector2(800, 100), new Rectangle(0, 0, 64, 64), Color.White);

            // Update animation timer
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

            // Update animation frame
            if (animationTimer > 0.2)
            {
                animationFrame++;

                if (animationFrame > 3)
                {
                    animationFrame = 1;
                }

                animationTimer -= 0.2;
            }

            // Draw the asteroid
            var asteroidSource = new Rectangle(animationFrame * 128, (animationFrame % 8) * 128, 128, 128);
            _spriteBatch.Draw(asteroid, new Vector2(500, 200), asteroidSource, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws a line between two points. Originally from a Stack Overflow answer by Cyral 
        /// (https://stackoverflow.com/a/16407171/10906388) licensed under CC BY-SA 3.0.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch instance to draw with</param>
        /// <param name="begin">The beginning point</param>
        /// <param name="end">The ending point</param>
        /// <param name="texture">The texture to draw</param>
        /// <param name="width">The width to draw the line, which defaults to one.</param>
        public void DrawLine(SpriteBatch spriteBatch, Vector2 begin, Vector2 end, Texture2D texture, int width = 1)
        {
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(texture, r, null, Color.White, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
