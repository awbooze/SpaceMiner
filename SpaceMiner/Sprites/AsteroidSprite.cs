//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceMiner.Collisions;
using System;

namespace SpaceMiner.Sprites
{
    /// <summary>
    /// A class representing an asteroid in a certain position and with a certain number of minerals.
    /// </summary>
    public class AsteroidSprite : IMinedSprite
    {
        private Texture2D texture;
        //private Texture2D boundingCircleTexture;
        private int size = 128;
        private double animationTimer;
        private short animationFrame = 0;

        public Vector2 Center { get; private set; }

        private BoundingCircle bounds;

        public IBounding Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                if (value is BoundingCircle circle)
                {
                    bounds = circle;
                }
                else
                {
                    throw new ArgumentException("The bounds of an AsteroidSprite must be a circle.");
                }
            }
        }

        public int MaxMinerals { get; private set; }

        public int CurrentMinerals { get; private set; }

        public AsteroidSprite(Vector2 center, int maxMinerals)
        {
            Center = center;
            Bounds = new BoundingCircle(center, 48);
            MaxMinerals = maxMinerals;
            CurrentMinerals = maxMinerals;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Sprites/Asteroid");
            //boundingCircleTexture = content.Load<Texture2D>("Sprites/Circle");
        }

        public void Update(GameTime gameTime)
        {
            // Nothing to update, but framework is here.
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Update animation timer
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

            // Update animation frame
            if (animationTimer > 0.5)
            {
                animationFrame++;

                if (animationFrame > 63)
                {
                    animationFrame = 0;
                }

                animationTimer -= 0.5;
            }

            // Draw the asteroid
            var asteroidSource = new Rectangle((animationFrame % 8) * size, (animationFrame / 8) * size, size, size);
            // TODO: Change the color as the asteroid is depleted of minerals
            /*spriteBatch.Draw(
                boundingCircleTexture, 
                Center - new Vector2(bounds.Radius, bounds.Radius), 
                null, 
                Color.White, 
                0f, 
                Vector2.Zero, 
                (bounds.Radius * 2) / 32, 
                SpriteEffects.None, 
                0f
            );*/
            spriteBatch.Draw(texture, Center - new Vector2(size / 2, size / 2), asteroidSource, Color.White);
        }
    }
}
