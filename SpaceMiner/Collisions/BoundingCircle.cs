//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using System;

namespace SpaceMiner.Collisions
{
    public class BoundingCircle : IBounding
    {
        /// <summary>
        /// The center of this BoundingCircle.
        /// </summary>
        public Vector2 Center { get; set; }

        /// <summary>
        /// The radius of this BoundingCircle.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Preferred constructor for the BoundingCircle.
        /// </summary>
        /// <param name="center">The center of this BoundingCircle.</param>
        /// <param name="radius">The radius of this BoundingCircle.</param>
        public BoundingCircle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Checks if this BoundingCircle is colliding with another IBounding object. Draws on ideas 
        /// given in the course examples, but is substantially different in operation.
        /// </summary>
        /// <param name="other">The other IBounding object this may be colliding with.</param>
        /// <returns>true if the two objects are colliding, false otherwise.</returns>
        public bool CollidesWith(IBounding other)
        {
            if (other is BoundingCircle otherCircle)
            {
                return Math.Pow(this.Radius + otherCircle.Radius, 2) >=
                    Math.Pow(this.Center.X - otherCircle.Center.X, 2) +
                    Math.Pow(this.Center.Y - otherCircle.Center.Y, 2);
            }
            else if (other is BoundingRectangle otherRectangle)
            {
                float nearestX = MathHelper.Clamp(this.Center.X, otherRectangle.Left, otherRectangle.Right);
                float nearestY = MathHelper.Clamp(this.Center.Y, otherRectangle.Top, otherRectangle.Bottom);

                return Math.Pow(this.Radius, 2) >=
                    Math.Pow(this.Center.X - nearestX, 2) +
                    Math.Pow(this.Center.Y - nearestY, 2);
            }
            else
            {
                throw new NotImplementedException("Other Bounding Shapes have not yet been implemented.");
            }
        }
    }
}
