using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    class GluttonEnemy : JellyEnemy
    {
        public GluttonEnemy(Texture2D sprite, 
            Texture2D jellyPartSprite,
            int x, 
            int y, 
            int size,
            Random rng, 
            MapChunk currentMap) 
            : base (sprite, jellyPartSprite, x, y, size, rng, currentMap)
        {
            
        }

        /// <summary>
        /// Killed - so grow a bit
        /// </summary>
        /// <param name="actor"></param>
        public override void Killed(Actor actor)
        {
            foreach (var i in actor.characterParts)
            {
                Killed(i);
            }
            for (var i = 0; i <= actor.jellySizeIncrease; ++i)
            {
                Grow(actor.x, actor.y);
                ExplodeAndPullIn();
            }
        }

        public override void Act()
        {
            characterParts.RemoveAll(i => i.dead);

            // Find the PC
            var pc = currentMap.Actors.First(i => i.GetType() == typeof(PlayerCharacter));

            // Find nearest food
            var food = currentMap.Actors.Where(i => i.foodOnly);
            var closestFood = food.OrderBy(i => Math.Abs(this.x - i.x) + Math.Abs(this.y - i.y)).FirstOrDefault();

            // Eat food if closer than PC
            if (closestFood != null)
            {
                try
                {
                    var closestFoodPath = Pathfinding.Pathfinder.FindPath(currentMap.pathGrid, new Tuple<int,int>(x, y), new Tuple<int, int>(closestFood.x, closestFood.y));
                    var pcPath = Pathfinding.Pathfinder.FindPath(currentMap.pathGrid, new Tuple<int, int>(x, y), new Tuple<int, int>(pc.x, pc.y));
                    if (closestFoodPath.Count < pcPath.Count)
                    {
                        MoveTowards(closestFood.x, closestFood.y);
                    }
                    else
                    {
                        MoveTowards(pc.x, pc.y);
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    base.MoveTowards(pc.x, pc.y);
                }
            }
            else
            {
                MoveTowards(pc.x, pc.y);
            }

        }
    }
}
