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
            // If x, y is inside the blob - then move internally
            int dx = Math.Sign(this.x - x) * -1;
            int dy = Math.Sign(this.y - y) * -1;
            Move(dx, dy);

            // Move bits towards

            // Sort bits by how close they are to target
            var bitsByDistance = characterParts.OrderByDescending(i => Math.Sqrt(Math.Pow((this.x - x), 2) + Math.Pow((this.y - y), 2)));
            
            // Move them in that order
            foreach (var i in bitsByDistance)
            {
                var ic = (PlayerJellyBit)i;
                ic.MoveTowards(x, y);
            }

            // And sort them by how far away they are from target
            var bitsByFarAway = characterParts.OrderBy(i => Math.Sqrt(Math.Pow((this.x - x), 2) + Math.Pow((this.y - y), 2)));

            // Explode them (remove overlap)
            foreach (var i in bitsByFarAway)
            {
                // Check if overlap
                //var overlap = bitsByFarAway.Where(j => i.x == j.x && i.y == j.y);
                // There should be 1... (this one)... otherwise expand
                //if (overlap.Count() > 1)
                {
                    // Find nearest walkable spot 
                    var place = FindClear(i.x, i.y, i) ?? new Tuple<int, int>(this.x, this.y);
                    // Move to it
                    i.x = place.Item1;
                    i.y = place.Item2;
                }
            }
        }

        /// <summary>
        /// Find nearest clear spot (might be the given one)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private Tuple<int, int> FindClear(int x, int y, Actor target)
        {
            var r = 0;
            var toExamine = new List<Tuple<int, int>>();
            // Keep going till found
            while (true)
            {
                for (var ix = x - r; ix <= x + r; ++ix)
                {
                    for (var iy = y -r; iy <= y + r; ++iy)
                    {
                        // If in bounds
                        if ((ix < 0 || iy < 0 || ix >= currentMap.Width || iy >= currentMap.Height)) { break; }

                        // Check if walkable
                        if (!currentMap.map[ix, iy].walkable) { break; }

                        // If both, check if monsters on it
                        if (currentMap.Actors.Any(i => i != target && i.x == ix && i.y == iy)) { break; }

                        // Check if borders another part in a cardinal direction
                        bool borders = characterParts.Any(i => (i.x == ix + 1 && i.y == iy) ||
                                                               (i.x == ix - 1 && i.y == iy) ||
                                                               (i.x == ix && i.y + 1 == iy) ||
                                                               (i.x == ix && i.y - 1 == iy));

                        // Check if immediately borders parent - in case not above
                        bool bordersParent = ((ix == this.x - 1 && iy == this.y) ||
                                              (ix == this.x + 1 && iy == this.y) ||
                                              (ix == this.x && iy == this.y - 1) ||
                                              (ix == this.x && iy == this.y + 1));

                        // Has to border a bit
                        if (!(borders || bordersParent)) { break; }
                        
                        // Got a candidate
                        toExamine.Add(new Tuple<int, int>(ix, iy));
                    }
                }

                if (toExamine.Count > 0)
                {
                    // Got one 
                    var bitsByFarAway = toExamine.OrderBy(i => Math.Sqrt(Math.Pow((i.Item1 - this.x), 2) + Math.Pow((i.Item2 - this.y), 2)));
                    return bitsByFarAway.First();
                }
                else
                { 
                    // If too big, return
                    ++r;
                    if (r > Math.Max(currentMap.Width / 2, currentMap.Height / 2)) 
                    {
                        return null;
                    }
                }
            }
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
