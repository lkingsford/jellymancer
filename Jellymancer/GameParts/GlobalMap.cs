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
        public MapTile[,] GetChunk(int chunkX, int chunkY)
        {
            var map = new MapTile[20, 20];
            for (var ix = 0; ix < 20; ++ix)
            {
                for (var iy = 0; iy < 20; ++iy)
                {
                    map[ix, iy] = new MapTile(content.Load<Texture2D>("Game/Tiles/Floor"), true);
                }
            }
            return map;
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
