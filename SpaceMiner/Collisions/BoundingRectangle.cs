//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using System;

namespace SpaceMiner.Collisions
{
    public class BoundingRectangle : IBounding
    {
        /// <summary>
        /// The center of this BoundingRectangle.
        /// </summary>
        public Vector2 Center { get; set; }

        /// <summary>
        /// The width of this BoundingRectangle.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// The height of this BoundingRectangle.
        /// </summary>
        public float Height { get; set; }

        public float Left => Center.X - (Width / 2);

        public float Right => Center.X + (Width / 2);

        public float Top => Center.Y - (Height / 2);

        public float Bottom => Center.Y + (Height / 2);

        /// <summary>
        /// A constructor for a BoundingRectangle which is actually a square.
        /// </summary>
        /// <param name="center">The center of this BoundingRectangle.</param>
        /// <param name="width">The width and height of this BoundingRectangle.</param>
        public BoundingRectangle(Vector2 center, float width)
        {
            Center = center;
            Width = width;
            Height = width;
        }

        /// <summary>
        /// Preferred constructor for the BoundingRectangle.
        /// </summary>
        /// <param name="center">The center of this BoundingRectangle.</param>
        /// <param name="width">The width of this BoundingRectangle.</param>
        /// <param name="height">The height of this BoundingRectangle.</param>
        public BoundingRectangle(Vector2 center, float width, float height)
        {
            Center = center;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// A constructor for a BoundingRectangle that takes a center and a size.
        /// </summary>
        /// <param name="center">The center of this BoundingRectangle.</param>
        /// <param name="size">The size of this BoundingRectangle.</param>
        public BoundingRectangle(Vector2 center, Vector2 size)
        {
            Center = center;
            Width = size.X;
            Height = size.Y;
        }

        /// <summary>
        /// Checks if this BoundingRectangle is colliding with another IBounding object. Draws on ideas 
        /// given in the course examples, but is substantially different in operation.
        /// </summary>
        /// <param name="other">The other IBounding object this may be colliding with.</param>
        /// <returns>true if the two objects are colliding, false otherwise.</returns>
        public bool CollidesWith(IBounding other)
        {
            if (other is BoundingRectangle otherRectangle)
            {
                return !(this.Right < otherRectangle.Left || this.Left > otherRectangle.Right ||
                    this.Top > otherRectangle.Bottom || this.Bottom < otherRectangle.Top);
            }
            else if (other is BoundingCircle otherCircle)
            {
                return otherCircle.CollidesWith(this);
            }
            else
            {
                throw new NotImplementedException("Other Bounding Shapes have not yet been implemented.");
            }
        }

        /// <summary>
        /// Checks if this BoundingRectangle is colliding with a point. Should only be used for 
        /// checking if input is happening, but may have other functions down the road.
        /// </summary>
        /// <param name="point">The point this may be colliding with.</param>
        /// <returns>true if the two objects are colliding, false otherwise.</returns>
        public bool CollidesWith(Point point)
        {
            return point.X >= this.Left && point.X <= this.Right && 
                point.Y >= this.Top && point.Y <= this.Bottom;
        }
    }
}
