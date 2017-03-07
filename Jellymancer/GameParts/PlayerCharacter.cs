using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    class PlayerCharacter : Actor
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PlayerCharacter (Texture2D sprite, Texture2D jellyPartSprite, int x, int y) : base (sprite, x, y)
        {
            this.jellyPartSprite = jellyPartSprite;

            for (var ix = x - 2; ix < x + 2; ++ix)
            {
                for (var iy = x - 2; iy < y + 2; ++iy)
                {
                    if (!(ix == x && iy == y))
                    {
                        Grow(ix, iy);
                    }
                }
            }
        }

        /// <summary>
        /// Sprite for additional bits of jelly
        /// </summary>
        public Texture2D jellyPartSprite;

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

            // Move parts if can
            foreach (var i in characterParts)
            {
                i.Move(dx, dy);
            }
        }

        /// <summary>
        /// Grow a new piece
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Grow(int x, int y)
        {
            var jellyPart = new PlayerJellyBit(jellyPartSprite, x, y);
            characterParts.Add(jellyPart);
            currentMap?.AddActor(jellyPart);
        }
    }
}
