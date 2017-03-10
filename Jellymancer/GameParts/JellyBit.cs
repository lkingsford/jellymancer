using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    class JellyBit : Actor
    {
        public Random rng;

        public JellyBit(Texture2D sprite, int x, int y, Random rng) : base(sprite, x, y)
        {
            choking = true;
            jellySizeIncrease = 1;
            this.rng = rng;
        }

        /// <summary>
        /// Try to move in a direction, return true if can
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public virtual bool TryMove(int dx, int dy)
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


        public override void Killed(Actor i)
        {
            parent?.Killed(i);
        }

        public Texture2D jellyPartSprite;
        /// <summary>
        /// Grow a new piece
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Grow(int x, int y)
        {
            var jellyPart = new JellyBit(jellyPartSprite, x, y, rng);
            jellyPart.parent = this;
            characterParts.Add(jellyPart);
            currentMap?.AddActor(jellyPart);
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

        int visitNo = 0;

        public const int MAX_DEPTH = 20;

        /// <summary>
        /// Explodes them out, and pulls them back until they touch the core
        /// </summary>
        public void ExplodeAndPullIn(int depth = 0)
        {
            if (depth > MAX_DEPTH) { return; }

            // Sort them by how far away they are from target
            var bitsByFarAway = characterParts.OrderBy(i => Math.Sqrt(Math.Pow((this.x - x), 2) + Math.Pow((this.y - y), 2)));

            // Explode them (remove overlap)
            foreach (var i in bitsByFarAway)
            {
                // Find nearest walkable spot 
                var place = FindClear(i.x, i.y, i) ?? new Tuple<int, int>(this.x, this.y);
                // Move to it
                i.x = place.Item1;
                i.y = place.Item2;
            }

            // And pull them back to touch the core
            visitNo = 0;
            bool notStuckProperly;
            do
            {
                var markedVisitBitsList = characterParts.Select(i => new VisitEntry(false, (JellyBit)i)).ToList();
                markedVisitBitsList.Add(new VisitEntry(true, this));
                visit(this, markedVisitBitsList);
                markedVisitBitsList.RemoveAll(i => i.actor == this);
                notStuckProperly = markedVisitBitsList.Any(i => !i.visited);
                if (notStuckProperly && depth < 20)
                {
                    MoveTowards(this.x, this.y, depth + 1);
                }
                else
                {
                    foreach (var tokill in markedVisitBitsList.Where(i => !i.visited && i.actor != this && !i.actor.dead))
                    {
                        tokill.actor.Choke();
                    }
                }
            }
            while (notStuckProperly && visitNo < 100);

            // And this bit's for the sake of the animation
            // If it's the same as another last turn, don't move
            // ... Wish I knew why this wasn't working
            foreach (var i in characterParts)
            {
                var v = characterParts.Where(j => (j.lastTurnX == i.x) && (j.lastTurnY == i.y));

                if (characterParts.Any(j => (j.lastTurnX == i.x) && (j.lastTurnY == i.y)))
                {
                    i.lastTurnX = i.x;
                    i.lastTurnY = i.y;
                }
            }

        }

        /// <summary>
        /// Move towards the given point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public bool MoveTowards(int x, int y, int depth = 0)
        {
            // Use pathfinding to move towards mouse click
            try
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
            catch (IndexOutOfRangeException)
            {
                base.MoveTowards(x, y);
            }
            catch (System.Exception)
            {
                base.MoveTowards(x, y);
            }

            // Sort bits by how close they are to target
            var bitsByDistance = characterParts.OrderByDescending(i => Math.Sqrt(Math.Pow((this.x - x), 2) + Math.Pow((this.y - y), 2)));

            // Move them in that order
            foreach (var i in bitsByDistance)
            {
                var ic = (JellyBit)i;
                ic.MoveTowards(x, y);
            }

            ExplodeAndPullIn(depth);

            return true;
        }


        /// <summary>
        /// Breadth first mess
        /// </summary>
        /// <param name="a"></param>
        /// <param name="visitList"></param>
        /// <returns></returns>
        private void visit(Actor a, List<VisitEntry> visitList)
        {
            visitNo += 1;

            var me = visitList.First(i => i.actor == a);
            if (me.visited && a != this) { return; }
            me.visited = true;

            var neighbours = visitList.Where(i => ((i.actor.x + 1 == a.x) && (i.actor.y == a.y)) ||
                                                 ((i.actor.x - 1 == a.x) && (i.actor.y == a.y)) ||
                                                 ((i.actor.y + 1 == a.y) && (i.actor.x == a.x)) ||
                                                 ((i.actor.y - 1 == a.y) && (i.actor.x == a.x))).ToList();

            for (int i = 0; i < neighbours.Count(); ++i)
            {
                visit(neighbours[i].actor, visitList);
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
                    for (var iy = y - r; iy <= y + r; ++iy)
                    {
                        // If in bounds
                        if ((ix < 0 || iy < 0 || ix >= currentMap.Width || iy >= currentMap.Height)) { continue; }

                        // Check if walkable
                        if (!currentMap.map[ix, iy].walkable) { continue; }

                        // If both, check if monsters on it
                        if (currentMap.Actors.Any(i => i != target && i.x == ix && i.y == iy && !i.dead)) { continue; }

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
                        if (!(borders || bordersParent)) { continue; }

                        // Got a candidate
                        toExamine.Add(new Tuple<int, int>(ix, iy));
                    }
                }

                if (toExamine.Count > 0)
                {
                    // Got one 
                    // Randomize order so not to always bias
                    var randomizedOrder = toExamine.OrderBy(i => rng.Next());
                    // And get the lowers
                    var bitsByFarAway = randomizedOrder.OrderBy(i => Math.Sqrt(Math.Pow((i.Item1 - x), 2) + Math.Pow((i.Item2 - y), 2)));
                    foreach (var i in bitsByFarAway)
                        return bitsByFarAway.First();
                }
                else
                {
                    // If too big, return
                    ++r;
                    if (r > 30)
                    {
                        return null;
                    }
                }
            }
        }

    }

}
