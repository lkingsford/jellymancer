using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    class JellyEnemy : JellyBit
    {
        public JellyEnemy(Texture2D sprite, Texture2D jellyPartSprite, int x, int y, int size, Random rng) : base (sprite, x, y, rng)
        {
            for (var i = 0; i <= size; ++i)
            {
            }
        }
    }
}
