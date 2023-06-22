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

        public Vector2 Center { get; set; }

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

        public bool Powered { get; set; } = false;

        public bool CanTransmitPower => false;

        public int MaxConnectionDistance => 100;

        public bool Selected { get; set; }

        public List<IPlayerStationSprite> NearbyStations { get; private set; } = new List<IPlayerStationSprite>();

        public List<IMinedSprite> NearbyAsteroids { get; private set; } = new List<IMinedSprite>();

        public int AmountToMine => 3;

        private readonly int mineDelayTime = 3;

        private int asteroidToMine = -1;

        private double timeSinceLastMined;

        private TimeSpan lastMined = TimeSpan.Zero;

        public MinerSprite(Vector2 center)
        {
            this.Center = center;
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
                CanPlace = true;
                bounds.Center = Center;
            }

            if (NearbyStations.Count > 0)
            {
                Powered = true;
            }

            if (Placed && Powered)
            {
                timeSinceLastMined = (gameTime.TotalGameTime - lastMined).TotalSeconds;

                if (timeSinceLastMined > mineDelayTime)
                {
                    asteroidToMine = new Random().Next(NearbyAsteroids.Count);
                    NearbyAsteroids[asteroidToMine].Mine(AmountToMine);
                    lastMined = gameTime.TotalGameTime;
                }
            }
        }

        public void Draw(SpaceMinerGame game, GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Placed)
            {
                // Display all of the possible asteroids this miner could mine if it's not placed.
                foreach (IMinedSprite sprite in NearbyAsteroids)
                {
                    game.DrawLine(spriteBatch, bounds.Center, sprite.Bounds.Center, Color.Red);
                }
            }
            else if (asteroidToMine > -1 && asteroidToMine < NearbyAsteroids.Count)
            {
                IMinedSprite sprite = NearbyAsteroids[asteroidToMine];
                float alpha = (float)(timeSinceLastMined / mineDelayTime);
                game.DrawLine(spriteBatch, bounds.Center, sprite.Bounds.Center, Color.Lerp(Color.Red, Color.Transparent, alpha));
            }

            Color drawColor = (Placed) ? Color.White : (CanPlace) ? Color.Gray : Color.Red;
            spriteBatch.Draw(texture, Center - new Vector2(bounds.Radius, bounds.Radius), drawColor);
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
                return otherMiner.Center == this.Center;
            }
            else
            {
                return false;
            }
        }
    }
}
