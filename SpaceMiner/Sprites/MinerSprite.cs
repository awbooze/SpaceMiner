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
    /// A class representing an asteroid mining station.
    /// </summary>
    public class MinerSprite : IPlayerStationSprite
    {
        private Texture2D texture;

        private Vector2 center;

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
                    throw new ArgumentException("The bounds of a MinerSprite must be a circle.");
                }
            }
        }

        private bool placed = false;

        public bool Placed => placed;

        private bool powered = false;

        public bool Powered { get => powered; set => throw new NotImplementedException(); }

        public MinerSprite(Vector2 center)
        {
            this.center = center;
            this.Bounds = new BoundingCircle(center, 16);
        }

        public MinerSprite(Vector2 center, bool prePlaced) : this(center)
        {
            placed = prePlaced;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Sprites/Miner");
        }

        public void Update(GameTime gameTime)
        {
            // Nothing here just yet.
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color drawColor = (placed) ? Color.White : Color.Gray;
            spriteBatch.Draw(texture, center - new Vector2(bounds.Radius, bounds.Radius), drawColor);
        }
    }
}
