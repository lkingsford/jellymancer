﻿using Jellymancer.GameParts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer
{
    class GameState : State
    {
        public GameState(ContentManager content)
        {
            this.content = content;
            this.globalMap = new GlobalMap(content);
        }

        /// <summary>
        /// Area being played on
        /// </summary>
        public MapTile[,] currentMap;

        /// <summary>
        /// The rest of the world
        /// </summary>
        public GlobalMap globalMap;

        /// <summary>
        /// Start a game
        /// </summary>
        public void NewGame()
        {
            currentMap = globalMap.GetChunk(0, 0);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            int width = 32;
            int height = 32;
            int xOffset = 320;
            int yOffset = 32;

            // Draw Tiles
            for (var ix = 0; ix < currentMap.GetLength(0); ++ix)
            {
                for (var iy = 0; iy < currentMap.GetLength(1); ++iy)
                {
                    spriteBatch.Draw(currentMap[ix, iy].img, new Vector2(ix * width + xOffset, iy * height + yOffset), Color.White);
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
        }
    }
}