using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceMiner.Collisions;
using System;
using System.Collections.Generic;
using System.Text;

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
                    throw new ArgumentException("The bounds of a MinerSprite must be a rectangle.");
                }
            }
        }

        private bool placed = false;

        public bool Placed { get => placed; set => placed = value; }

        public bool CanPlace { get; set; } = true;

        private bool powered = false;

        public bool Powered { get => powered; set => throw new NotImplementedException(); }

        public bool Selected { get; set; }

        private MouseState currentMouseState;

        public SolarPowerSprite(Vector2 center)
        {
            this.center = center;
            this.Bounds = new BoundingRectangle(center, 32);
        }

        public SolarPowerSprite(Vector2 center, bool placed, bool selected) : this(center)
        {
            Selected = selected;
            this.placed = placed;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Sprites/Solar Power Plant");
        }

        public void Update(GameTime gameTime)
        {
            if (Selected && !placed)
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
