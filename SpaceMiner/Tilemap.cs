//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpaceMiner
{
    public class Tilemap
    {
        /// <summary>
        /// The dimensions of tiles and the map
        /// </summary>
        int _tileWidth, _tileHeight, _mapWidth, _mapHeight;

        /// <summary>
        /// The tileset texture
        /// </summary>
        Texture2D _tilesetTexture;

        /// <summary>
        /// The tile info in the tileset. In more complicated environments, 
        /// this might be a class or a struct.
        /// </summary>
        Rectangle[] _tiles;

        /// <summary>
        /// The tile map data
        /// </summary>
        int[] _map;

        /// <summary>
        /// The file name of the map file
        /// </summary>
        string _filename;

        public Tilemap(string filename, int tileWidth, int tileHeight, int mapWidth, int mapHeight)
        {
            _filename = filename;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
        }

        public void LoadContent(ContentManager content)
        {
            _tilesetTexture = content.Load<Texture2D>(_filename);

            // Initialize tile bounds
            int tilesetColumns = _tilesetTexture.Width / _tileWidth;
            int tilesetRows = _tilesetTexture.Height / _tileHeight;
            _tiles = new Rectangle[tilesetColumns * tilesetRows];

            for (int y = 0; y < tilesetColumns; y++)
            {
                for (int x = 0; x < tilesetRows; x++)
                {
                    int index = y * tilesetColumns + x;
                    _tiles[index] = new Rectangle(
                        x * _tileWidth,
                        y * _tileHeight,
                        _tileWidth,
                        _tileHeight
                    );
                }
            }

            _map = new int[_mapWidth * _mapHeight];
            Random random = new Random();

            for (int i = 0; i < _mapWidth * _mapHeight; i++)
            {
                // Generate random, mostly empty starfield
                double next = random.NextDouble();
                if (next < 0.93)
                {
                    _map[i] = 0;
                }
                else if (next < 0.985)
                {
                    _map[i] = 1;
                }
                else if (next < 0.99)
                {
                    _map[i] = 2;
                }
                else if (next < 0.997)
                {
                    _map[i] = 3;
                }
                else
                {
                    _map[i] = 4;
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    // Map currently counts from 1, but arrays start at 0
                    int index = _map[y * _mapWidth + x] - 1;
                    if (index == -1)
                    {
                        continue;
                    }

                    spriteBatch.Draw(
                        _tilesetTexture,
                        new Vector2(x * _tileWidth, y * _tileHeight),
                        _tiles[index],
                        Color.Lerp(Color.White, Color.Black, 0.3f)   //Some (faked) transparency
                    );
                }
            }
        }
    }
}
