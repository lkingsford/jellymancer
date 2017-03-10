using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    class PlayerCharacter : JellyBit
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PlayerCharacter (Texture2D sprite, Texture2D jellyPartSprite, int x, int y, Random rng) : base (sprite, x, y, rng)
        {
            this.jellyPartSprite = jellyPartSprite;

            for (var ix = x - 1; ix <= x + 1; ++ix)
            {
                for (var iy = y - 1; iy <= y + 1; ++iy)
                {
                    if (!(ix == x && iy == y))
                    {
                        Grow(ix, iy);
                    }
                }
            }
            hp = 4;
            choking = true;
        }


        class VisitEntry
        {
            public Actor actor;
            public bool visited;

            public VisitEntry(bool visited, Actor a)
            {
                actor = a;
                this.visited = visited;
            }

        }

        public int hp;
        
        /// <summary>
        /// Attacked by monster
        /// </summary>
        /// <param name="monsterInPos"></param>
        public override void Attacked(Actor monsterInPos)
        {
            this.hp -= 1;
            if (this.hp == 0)
            {
                this.dead = true;
            }
        }

        public void eatSelf(JellyBit i)
        {
            i.dead = true;
            characterParts.Remove(i);
            ExplodeAndPullIn();
            hp += 1;
            if (hp > 4) { hp = 4; }
        }


        /// <summary>
        /// Move/Act dx, dy if possible - changed to allow bigger dudes
        /// </summary>
        /// <param name="dx">Relative X</param>
        /// <param name="dy">Relative Y</param>
        public override void Move(int dx, int dy)
        {
            int desiredx = x + dx;
            int desiredy = y + dy;

            // These all return if not moveable - so not storing
            // Check for bounds
            if (desiredx < 0 || desiredy < 0 || desiredx >= currentMap.Width || desiredy >= currentMap.Height) { return; }
            // Check for map walkability
            if (!currentMap.map[desiredx, desiredy].walkable) { return; }
            // This checks if any other monsters in space. Technically, might want to attack instead...
            if (currentMap.Actors.Any(i => i.x == desiredx && i.y == desiredy && !characterParts.Contains(i))) { return; }

            // If here, we can move
            x = desiredx;
            y = desiredy;

        }


        /// <summary>
        /// Killed - so grow a bit
        /// </summary>
        /// <param name="actor"></param>
        public override void Killed(Actor actor)
        {
            for (var i = 0; i <= actor.jellySizeIncrease; ++i)
            {
                Grow(actor.x, actor.y);
                ExplodeAndPullIn();
            }
        }
    }
}
