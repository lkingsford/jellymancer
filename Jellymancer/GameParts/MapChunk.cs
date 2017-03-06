using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    /// <summary>
    /// Basic part of the world map
    /// </summary>
    class MapChunk
    {
        /// <summary>
        /// Create new map chunk
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="content"></param>
        public MapChunk(int width, int height, ContentManager content)
        {
            // I don't feel content should be in this....

            map = new MapTile[width, height];
            for (var ix = 0; ix < width; ++ix)
            {
                for (var iy = 0; iy < height; ++iy)
                {
                    map[ix, iy] = new MapTile(content.Load<Texture2D>("Game/Tiles/Floor"), true);
                }
            }

            Width = width;
            Height = height;
        }

        public MapTile[,] map;

        public readonly int Width;
        public readonly int Height;

    }
}
