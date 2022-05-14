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
using System.Collections.Generic;

namespace SpaceMiner.Sprites
{
    public class SolarPowerSprite : IPlayerStationSprite
    {
        private Texture2D texture;

        public Vector2 Center { get; set; }

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

        public bool CanTransmitPower => true;

        public int MaxConnectionDistance => 200;

        public bool Selected { get; set; }

        public List<IPlayerStationSprite> NearbyStations { get; private set; } = new List<IPlayerStationSprite>();

        private MouseState currentMouseState;

        public SolarPowerSprite(Vector2 center)
        {
            this.Center = center;
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
                CanPlace = true;
                bounds.Center = Center;
            }
        }

        public void Draw(SpaceMinerGame game, GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (IPlayerStationSprite sprite in NearbyStations)
            {
                game.DrawLine(spriteBatch, bounds.Center, sprite.Bounds.Center, Color.Blue);
            }

            Color drawColor = (Placed) ? Color.White : (CanPlace) ? Color.Gray : Color.Red;
            spriteBatch.Draw(texture, Center - new Vector2(bounds.Width / 2, bounds.Height / 2), drawColor);
        }

        public override bool Equals(object obj)
        {
            if (obj is IPlayerStationSprite otherSprite)
            {
                return Equals(otherSprite);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(IPlayerStationSprite otherSprite)
        {
            if (otherSprite is SolarPowerSprite otherPower)
            {
                return otherPower.Center == this.Center;
            }
            else
            {
                return false;
            }
        }
    }
}
