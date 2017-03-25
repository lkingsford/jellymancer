using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    class MapTile
    {
        public MapTile(Texture2D img, bool walkable)
        {
            this.img = img;
            this.walkable = walkable;
        }
        public readonly Texture2D img;
        public readonly bool walkable = false;
    }
}
