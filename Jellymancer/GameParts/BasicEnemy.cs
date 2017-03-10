using Jellymancer.Helper;
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
        public void Act()
        {
            // Find the PC
            var pc = currentMap.Actors.First(i => i.GetType() == typeof(PlayerCharacter));

            MoveTowards(pc);
        }

        public override void MoveTowards(int x, int y)
        {
            var path = UglyPath.GetPath(new Helper.UglyPath.coord(this.x, this.y),
                                        new Helper.UglyPath.coord(x, y),
                                        CheckCollision);
            if (path != null)
            {
                base.MoveTowards(path.First().X, path.First().Y);
            }
            else
            {
                base.MoveTowards(x, y);
            }
        }

        private bool CheckCollision(int x, int y)
        {
            if (x < 0 || y < 0 || x >= currentMap.Width || y >= currentMap.Height) { return false; }
            return currentMap.map[x, y].walkable; // && currentMap.Actors.Any(i => (i.x == x) && (i.y == y));
        }
    }
}
