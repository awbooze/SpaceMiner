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
    /// An interface which contains all public properties required for objects the player
    /// may mine.
    /// </summary>
    public interface IMinedSprite
    {
        public Vector2 Center { get; }

        public IBounding Bounds { get; set; }

        public int MaxMinerals { get; }

        public int CurrentMinerals { get; }

        public void LoadContent(ContentManager content);

        public void Update(GameTime gameTime);

        public int Mine();

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
