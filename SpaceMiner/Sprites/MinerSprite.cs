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

        public bool Placed { get; set; }

        public bool CanPlace { get; set; } = true;

        private bool powered = false;

        public bool Powered { get => powered; set => throw new NotImplementedException(); }

        public bool CanTransmitPower => false;

        public int MaxConnectionDistance => 100;

        public bool Selected { get; set; }

        public List<IPlayerStationSprite> NearbyStations { get; private set; } = new List<IPlayerStationSprite>();

        public List<IMinedSprite> NearbyAsteroids { get; private set; } = new List<IMinedSprite>();

        private MouseState currentMouseState;

        public MinerSprite(Vector2 center)
        {
            this.center = center;
            this.Bounds = new BoundingCircle(center, 16);
        }

        public MinerSprite(Vector2 center, bool placed, bool selected) : this(center)
        {
            Selected = selected;
            Placed = placed;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Sprites/Miner");
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

        public void Draw(SpaceMinerGame game, GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (IPlayerStationSprite sprite in NearbyStations)
            {
                game.DrawLine(spriteBatch, bounds.Center, sprite.Bounds.Center, Color.Blue);
            }

            foreach (IMinedSprite sprite in NearbyAsteroids)
            {
                game.DrawLine(spriteBatch, bounds.Center, sprite.Bounds.Center, Color.Red);
            }

            Color drawColor = (Placed) ? Color.White : (CanPlace) ? Color.Gray : Color.Red;
            spriteBatch.Draw(texture, center - new Vector2(bounds.Radius, bounds.Radius), drawColor);
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
            if (otherSprite is MinerSprite otherMiner)
            {
                return otherMiner.center == this.center;
            }
            else
            {
                return false;
            }
        }
    }
}
