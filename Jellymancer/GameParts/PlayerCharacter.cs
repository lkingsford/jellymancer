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
        internal Random random;

        /// <summary>
        /// Move towards the given point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void MoveTowards(int x, int y)
        {
            // If x, y is inside the blob - then get closer to core
            // if x, y is outside the blob, move blob in dir 

            // In Core
            int core_dx = Math.Sign(this.x - x) * -1;
            int core_dy = Math.Sign(this.y - y) * -1;
            Move(core_dx, core_dy);

            double target_x = x;
            double target_y = y;

            // Apply movement force to all components 
            foreach (var part in characterParts)
            {
                var p = (JellyBit)part;

                // Initialise with current X and current Y if bigger than 1.5 out
                if (Math.Abs(p.precise_x - p.x) > 1.5 || Math.Abs(p.precise_y = p.y) > 1.5)
                {
                    p.precise_x = p.x;
                    p.precise_y = p.y;
                }

                // Get angle
                var theta = Math.PI + Math.Atan2(p.precise_x - target_x, p.precise_y - target_y);
                var distance = Math.Min(1 , Math.Sqrt(Math.Pow(p.precise_x - target_x, 2) + Math.Pow(p.precise_y - target_y, 2)));


                p.precise_x += distance * Math.Sin(theta);
                p.precise_y += distance * Math.Cos(theta);


            }

            bool done = false;
            int attempts = 0;
            while (!done && attempts < 3)
            {
                done = true;
                
                // While haven't finished moving...
                var oldList = new List<Tuple<int, int>>();
                foreach (var part in characterParts)
                {
                    oldList.Add(new Tuple<int, int>(part.x, part.y));
                }

                // Reduce overlap
                foreach (var part in characterParts)
                {
                    var p = (JellyBit)part;
                    p.precise_x += random.NextDouble() / 10.0;
                    p.precise_y += random.NextDouble() / 10.0;
                }

                // Get locations for attraction and repelling
                var locations = new List<Tuple<double, double, JellyBit>>();
                locations.Add(new Tuple<double, double, JellyBit>(this.x, this.y, this));

                foreach (var part in characterParts)
                {
                    var p = (JellyBit)part;
                    locations.Add(new Tuple<double, double, JellyBit>(p.precise_x, p.precise_y, p));
                }

                // Attract each other (use same locations)
                foreach (var part in characterParts)
                {
                    var p = (JellyBit)part;
                    foreach (var loc in locations)
                    {
                        if (loc.Item3 != part)
                        {
                            var distance = Math.Sqrt(Math.Pow((loc.Item1 - p.precise_x), 2) + Math.Pow((loc.Item2 - p.precise_y), 2));
                            var attraction = Math.Min(0.3, (Math.Pow(1.5, 1.0 / distance)));
                            var theta = Math.Atan2(loc.Item1 - p.precise_x, loc.Item2 - p.precise_y);
                            p.precise_x += attraction * Math.Sin(theta);
                            p.precise_y += attraction * Math.Cos(theta);
                        }
                    }
                }

                // Repel from each other and wall
                foreach (var part in characterParts)
                {
                    var p = (JellyBit)part;
                    // Hack to avoid wall
                    if (!currentMap.GetWalkable(part.x, part.y)) { locations.Add(new Tuple<double, double, JellyBit>(part.x, part.y, null)); }
                    if (!currentMap.GetWalkable(part.x + 1, part.y)) { locations.Add(new Tuple<double, double, JellyBit>(part.x + 1, part.y, null)); }
                    if (!currentMap.GetWalkable(part.x - 1, part.y)) { locations.Add(new Tuple<double, double, JellyBit>(part.x - 1, part.y, null)); }
                    if (!currentMap.GetWalkable(part.x, part.y + 1)) { locations.Add(new Tuple<double, double, JellyBit>(part.x, part.y + 1, null)); }
                    if (!currentMap.GetWalkable(part.x, part.y - 1)) { locations.Add(new Tuple<double, double, JellyBit>(part.x, part.y - 1, null)); }
                    foreach (var loc in locations)
                    {
                        if (loc.Item3 != part)
                        {
                            var distance = Math.Sqrt(Math.Pow((loc.Item1 - p.precise_x), 2) + Math.Pow((loc.Item2 - p.precise_y), 2));
                            var theta = Math.Atan2(loc.Item1 - p.precise_x, loc.Item2 - p.precise_y);
                            var deattraction = 1.2; //part == null ? 1.3 : 0.5;
                            if (distance < 0.7)
                            {
                                p.precise_x -= deattraction * Math.Sin(theta);
                                p.precise_y -= deattraction * Math.Cos(theta);
                            }
                        }
                    }
                }


                // Finalise x, y
                foreach (var part in characterParts)
                {
                    var p = (JellyBit)part;
                    p.x = (int)Math.Round(p.precise_x);
                    p.y = (int)Math.Round(p.precise_y);
                }

                // If any changed, not done
                for (var i = 0; i < characterParts.Count; ++i)
                {
                    if (!(oldList[i].Item1 == characterParts[i].x && oldList[i].Item2 == characterParts[i].y ))
                    {
                        done = false;
                        break;
                    }
                }
                ++attempts;
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
            var jellyPart = new JellyBit(jellyPartSprite, x, y);
            characterParts.Add(jellyPart);
            currentMap?.AddActor(jellyPart);
        }
    }
}
