//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceMiner.Collisions;

namespace SpaceMiner.Sprites
{
    /// <summary>
    /// An interface which contains all public properties required for various stations the player
    /// may construct.
    /// </summary>
    public interface IPlayerStationSprite
    {
        /// <summary>
        /// The station's collision boundry.
        /// </summary>
        public IBounding Bounds { get; set; }

        /// <summary>
        /// Whether the station has been placed or if the player is just considering placing it.
        /// </summary>
        public bool Placed { get; }

        /// <summary>
        /// Whether the station has a power connection.
        /// </summary>
        public bool Powered { get; set; }

        public void LoadContent(ContentManager content);

        public void Update(GameTime gameTime);

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
