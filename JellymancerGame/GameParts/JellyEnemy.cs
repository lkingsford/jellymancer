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
        public JellyEnemy(Texture2D sprite, Texture2D jellyPartSprite, int x, int y, int size, Random rng, MapChunk currentMap) : base (sprite, x, y, rng)
        {
            this.currentMap = currentMap;
            this.jellyPartSprite = jellyPartSprite;
            for (var i = 0; i <= size; ++i)
            {
                Grow(x, y);
            }
            ExplodeAndPullIn(MAX_DEPTH - 2);
        }

        public override void Act()
        {
            base.Act();

            // Find the PC
            var pc = currentMap.Actors.First(i => i.GetType() == typeof(PlayerCharacter));

            MoveTowards(pc.x, pc.y);
        }

        public override void Choke()
        {
            base.Choke();
        }
    }
}
