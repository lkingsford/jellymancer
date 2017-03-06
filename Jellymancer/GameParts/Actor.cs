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
    }
}
