﻿using Microsoft.Xna.Framework.Graphics;
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

            for (var ix = x - 2; ix <= x + 2; ++ix)
            {
                for (var iy = x - 2; iy <= y + 2; ++iy)
                {
                    if (!(ix == x && iy == y))
                    {
                        Grow(ix, iy);
                    }
                }
            }

            choking = true;
        }

        /// <summary>
        /// Sprite for additional bits of jelly
        /// </summary>
        public Texture2D jellyPartSprite;
        internal Random rng;

        /// <summary>
        /// Move towards the given point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public new bool MoveTowards(int x, int y)
        {
            // If x, y is inside the blob - then get closer to core
            // if x, y is outside the blob, move blob in dir 

            int dx = Math.Sign(this.x - x) * -1;
            int dy = Math.Sign(this.y - y) * -1;
            Move(dx, dy);

            // Sort bits by how close they are to target
            var bitsByDistance = characterParts.OrderByDescending(i => Math.Sqrt(Math.Pow((this.x - x), 2) + Math.Pow((this.y - y), 2)));

            // Move them in that order
            foreach (var i in bitsByDistance)
            {
                var ic = (JellyBit)i;
                ic.MoveTowards(x, y);
            }

            // And sort them by how far away they are from target
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
                var markedVisitBitsList = characterParts.Select(i => new Tuple<bool, JellyBit>(false, (JellyBit)i)).ToList();
                markedVisitBitsList.Add(new Tuple<bool, JellyBit>(true, this));
                visit(this, markedVisitBitsList);
                markedVisitBitsList.RemoveAll(i => i.Item2 == this);
                notStuckProperly = markedVisitBitsList.Any(i => !i.Item1);
                if (notStuckProperly)
                {
                    MoveTowards(this.x, this.y);
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

            return true;
        }

        int visitNo = 0;

        /// <summary>
        /// Breadth first mess
        /// </summary>
        /// <param name="a"></param>
        /// <param name="visitList"></param>
        /// <returns></returns>
        private void visit(Actor a, List<Tuple<bool, JellyBit>> visitList)
        {
            visitNo += 1;

            var me = visitList.First(i => i.Item2 == a);
            if (me.Item1 && a != this) { return; }
            visitList.Remove(me);
            visitList.Add(new Tuple<bool, JellyBit>(true, me.Item2));

            var neighbours = visitList.Where(i => ((i.Item2.x + 1 == a.x) && (i.Item2.y == a.y)) ||
                                                 ((i.Item2.x - 1 == a.x) && (i.Item2.y == a.y)) ||
                                                 ((i.Item2.y + 1 == a.y) && (i.Item2.x == a.x)) ||
                                                 ((i.Item2.y - 1 == a.y) && (i.Item2.x == a.x))).ToList();

            for (int i = 0; i < neighbours.Count(); ++i)
            {
                visit(neighbours[i].Item2, visitList);
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
                        if (currentMap.Actors.Any(i => i != target && i.x == ix && i.y == iy)) { continue; }

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
                    var randomizedOrder = toExamine.OrderBy(i=>rng.Next());
                    // And get the lowers
                    var bitsByFarAway = randomizedOrder.OrderBy(i => Math.Sqrt(Math.Pow((i.Item1 - x), 2) + Math.Pow((i.Item2 - y), 2)));
                    foreach (var i in bitsByFarAway)
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
