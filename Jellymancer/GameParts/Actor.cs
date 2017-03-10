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
            lastTurnX = x;
            this.y = y;
            lastTurnY = y;
            // How much bigger a jelly gets after eating it
            jellySizeIncrease = 4;
        }

        /// <summary>
        /// Sprite that looks like
        /// </summary>
        public Texture2D sprite;
        
        /// <summary>
        /// Location, in respect to current map chunk
        /// </summary>
        public int x = 0, y = 0;
        public int lastTurnX = 0, lastTurnY = 0;

        public int jellySizeIncrease;

        /// <summary>
        /// Whether this one is a killa by chokin'
        /// </summary>
        public bool choking = false;

        /// <summary>
        /// Whether food only - turn immediately
        /// </summary>
        public bool foodOnly = false;
     
        /// <summary>
        /// Location which we're on
        /// </summary>
        public MapChunk currentMap = null;

        public virtual void Attacked(Actor monsterInPos)
        {
            this.dead = true;
        }

        /// <summary>
        /// Move/Act dx, dy if possible
        /// </summary>
        /// <param name="dx">Relative X</param>
        /// <param name="dy">Relative Y</param>
        public virtual void Move(int dx, int dy)
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

        /// <summary>
        /// 'Slave' characters (parts that are associated with this, and generally controlled by it)
        /// </summary>
        public List<Actor> characterParts = new List<Actor>();

        /// <summary>
        /// Parent - if part
        /// </summary>
        public Actor parent = null;

        public  bool dead = false;

        /// <summary>
        /// May need to do things
        /// </summary>
        public virtual void Choke()
        {
            dead = true;
        }

        /// <summary>
        /// Has killed, not was killed
        /// </summary>
        /// <param name="i"></param>
        public virtual void Killed(Actor i)
        {
        }

        /// <summary>
        /// Move towards the given point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void MoveTowards(int x, int y)
        {
            int dx = Math.Sign(this.x - x) * -1;
            int dy = Math.Sign(this.y - y) * -1;
            Move(dx, dy);
        }

        public void MoveTowards(Actor a)
        {
            MoveTowards(a.x, a.y);
        }
    }
}
