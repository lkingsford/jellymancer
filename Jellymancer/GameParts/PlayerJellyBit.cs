using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    class PlayerJellyBit : Actor
    {
        public PlayerJellyBit(Texture2D sprite, int x, int y) : base(sprite, x, y)
        {

        }

        /// <summary>
        /// Move towards the given point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveTowards(int x, int y)
        {
            int dx = Math.Sign(this.x - x) * -1;
            int dy = Math.Sign(this.y - y) * -1;
            Move(dx, dy);
        }

        /// <summary>
        /// Try to move in a direction, return true if can
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        private bool TryMove(int dx, int dy)
        {
            int desiredx = x + dx;
            int desiredy = y + dy;

            // These all return if not moveable - so not storing
            // Check for bounds
            if (desiredx < 0 || desiredy < 0 || desiredx >= currentMap.Width || desiredy >= currentMap.Height) { return false; }

            // Check for map walkability, and move if can
            if (currentMap.map[desiredx, desiredy].walkable)
            {
                x = desiredx;
                y = desiredy;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Move/Act dx, dy if possible - Doesn't check for overlap though, and moves along one of the axes if both aren't possible but one is 
        /// </summary>
        /// <param name="dx">Relative X</param>
        /// <param name="dy">Relative Y</param>
        public override void Move(int dx, int dy)
        {
            if (TryMove(dx, dy)) { return; }
            
            if (TryMove(dx, 0)) { return; }
            if (TryMove(0, dy)) { return; }
        }
    }
}
