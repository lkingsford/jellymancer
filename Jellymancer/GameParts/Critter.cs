using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    class Critter : BasicEnemy
    {
        Random rng;

        public Critter(Texture2D sprite, int x, int y, Random rng) : base(sprite, x, y)
        {
            jellySizeIncrease = 1;
            this.rng = rng;
            dead = false;
        }

        // Move randomly
        public override void Act()
        {
            Move(rng.Next(-2, 3), rng.Next(-2, 3));
        }
    }
}
