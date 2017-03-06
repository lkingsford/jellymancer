using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    class GlobalMap
    {
        private readonly ContentManager content;

        public GlobalMap(ContentManager content)
        {
            this.content = content;
        }

        /// <summary>
        /// Return a chunk of the map. May involve generating it.
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <returns></returns>
        public MapChunk GetChunk(int chunkX, int chunkY)
        {
            return new MapChunk(20, 20, content);
        }

        /// <summary>
        /// Save a chunk of the map... if that's something we're doing? I guess?
        /// </summary>
        /// <param name="originX"></param>
        /// <param name="originY"></param>
        /// <param name="chunk"></param>
        public void SaveChunk(long originX, long originY, MapTile[,] chunk)
        {

        }
    }
}
