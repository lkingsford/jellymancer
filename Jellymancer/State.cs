using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer
{
    class State
    {
        public State()
        {

        }

        protected SpriteBatch spriteBatch;
        protected ContentManager content;

        /// <summary>
        /// The current sprite batch
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get
            {
                return spriteBatch;
            }
            set
            {
                spriteBatch = value;
            }
        }

        /// <summary>
        /// Draw the state to the screen
        /// </summary>
        /// <param name="gametime"></param>
        public virtual void Draw(GameTime gametime)
        {

        }

        /// <summary>
        /// Update the logic of the game
        /// </summary>
        /// <param name="gametime"></param>
        public virtual void Update(GameTime gametime)
        {

        }

        /// <summary>
        /// Load content into state
        /// </summary>
        public virtual void LoadContent()
        {

        }

        public bool toQuit = false;
    }
}
