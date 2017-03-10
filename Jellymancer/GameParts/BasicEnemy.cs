using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    class BasicEnemy : Actor
    {
        public BasicEnemy(Texture2D sprite, int x, int y)
            : base(sprite, x, y)
        {

        }

        // Do things
        public override void Act()
        {
            // Find the PC
            var pc = currentMap.Actors.First(i => i.GetType() == typeof(PlayerCharacter));

            MoveTowards(pc);
        }

        public override void MoveTowards(int x, int y)
        {
            var path = new DeenGames.Utils.AStarPathFinder.PathFinderFast(currentMap.pathGrid).FindPath(new DeenGames.Utils.Point(this.x, this.y), new DeenGames.Utils.Point(x, y));
            if (path != null && path.Count > 2)
            {
                var pathPos = (path[path.Count - 2]);
                base.MoveTowards(path[path.Count - 2].X, path[path.Count - 2].Y);
            }
            else
            {
                base.MoveTowards(x, y);
            }
        }

        public override void Move(int dx, int dy)
        {
            var monsterInPos = currentMap.Actors.FirstOrDefault(i => (i.x == x + dx) && (i.y == y + dy));
            if (monsterInPos == null)
            {
                base.Move(dx, dy);
            }
            else
            {
                monsterInPos.Attacked(monsterInPos);
            }
        }
    }
}
