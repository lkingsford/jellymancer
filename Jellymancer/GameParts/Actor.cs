using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    /// <summary>
    /// An actor is a moving dude or thing in the game
    /// </summary>
    class Actor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Actor(Texture2D sprite, int x, int y)
        {
            this.sprite = sprite;
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Sprite that looks like
        /// </summary>
        public Texture2D sprite;

        /// <summary>
        /// Location, in respect to current map chunk
        /// </summary>
        public int x = 0, y = 0;

        /// <summary>
        /// Location which we're on
        /// </summary>
        public MapChunk currentMap = null;

        /// <summary>
        /// Move/Act dx, dy if possible
        /// </summary>
        /// <param name="dx">Relative X</param>
        /// <param name="dy">Relative Y</param>
        protected virtual void Move(int dx, int dy)
        {
            int desiredx = x + dx;
            int desiredy = y + dy;

            // These all return if not moveable - so not storing
            // Check for bounds
            if (desiredx < 0 || desiredy < 0 || desiredx >= currentMap.Width || desiredy >= currentMap.Height) {  return; }
            // Check for map walkability
            if (!currentMap.map[desiredx, desiredy].walkable) { return; }
            // This checks if any any monsters in space. Technically, might want to attack instead...
            if (currentMap.Actors.Any(i => i.x == desiredx && i.y == desiredy)) { return; }

            // If here, we can move
            x = desiredx;
            y = desiredy;
        }
    }
}
