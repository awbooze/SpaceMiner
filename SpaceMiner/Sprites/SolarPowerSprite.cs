//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceMiner.Collisions;
using System;

namespace SpaceMiner.Sprites
{
    public class SolarPowerSprite : IPlayerStationSprite
    {
        private Texture2D texture;

        private Vector2 center;

        private BoundingRectangle bounds;

        public IBounding Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                if (value is BoundingRectangle rectangle)
                {
                    bounds = rectangle;
                }
                else
                {
                    throw new ArgumentException("The bounds of a SolarPowerSprite must be a rectangle.");
                }
            }
        }

        public bool Placed { get; set; }

        public bool CanPlace { get; set; } = true;

        private bool powered = false;

        public bool Powered { get => powered; set => throw new NotImplementedException(); }

        public bool Selected { get; set; }

        private MouseState currentMouseState;

        public SolarPowerSprite(Vector2 center)
        {
            this.center = center;
            this.Bounds = new BoundingRectangle(center, 64);
        }

        public SolarPowerSprite(Vector2 center, bool placed, bool selected) : this(center)
        {
            Selected = selected;
            Placed = placed;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Sprites/Solar Power Plant");
        }

        public void Update(GameTime gameTime)
        {
            if (Selected && !Placed)
            {
                currentMouseState = Mouse.GetState();
                CanPlace = true;
                center = new Vector2(currentMouseState.X, currentMouseState.Y);
                Bounds.Center = center;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color drawColor = (Placed) ? Color.White : (CanPlace) ? Color.Gray : Color.Red;
            spriteBatch.Draw(texture, center - new Vector2(bounds.Width / 2, bounds.Height / 2), drawColor);
        }
    }
}
