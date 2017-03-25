using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    // Actor that doesn't move and exists to be eaten
    class Food : Actor
    {
        public Food(Texture2D sprite, int x, int y) : base(sprite, x, y)
        {
            jellySizeIncrease = 1;
            foodOnly = true;
        }
    }
}
