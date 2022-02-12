//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;

namespace SpaceMiner.Collisions
{
    /// <summary>
    /// An interface which sets public properties and methods for bounding elements.
    /// </summary>
    public interface IBounding
    {
        /// <summary>
        /// The center of this bounding element.
        /// </summary>
        public Vector2 Center { get; set; }

        /// <summary>
        /// Checks if this IBounding struct is colliding with another one.
        /// </summary>
        /// <param name="other">The other IBounding struct this may be colliding with.</param>
        /// <returns>true if the two structs are colliding, false otherwise.</returns>
        public bool CollidesWith(IBounding other);
    }
}
