using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceMiner.Collisions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceMiner.Input
{
    public class Button
    {
        public string Text { private get; set; }

        private SpriteFont _font;

        // Eventually allow labels to have icons
        public Texture2D Icon { private get; set; }

        public Vector2 Center
        {
            get => _center;
            set
            {
                _center = value;
                Bounds.Center = value;
            }
        }

        private Vector2 _center;

        public BoundingRectangle Bounds { get; private set; }

        public bool Hovered { get; set; }

        public event EventHandler Selected;

        protected internal virtual void OnSelectEntry()
        {
            Selected?.Invoke(this, null);
        }

        public Button(SpriteFont font, string text)
        {
            _font = font;
            Text = text;
            Bounds = new BoundingRectangle(Center, GetSize());
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color color = Hovered ? Color.Red : Color.White;

            spriteBatch.DrawString(_font, Text, Center - new Vector2(Bounds.Width / 2, Bounds.Height / 2), color);
        }

        public Vector2 GetSize()
        {
            return _font.MeasureString(Text);
        }
    }
}
