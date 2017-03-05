using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        KeyboardState oldState;

        // Whether a new key push
        protected List<Keys> keyPress;

        /// <summary>
        /// Update the logic of the game
        /// </summary>
        /// <param name="gametime"></param>
        public virtual void Update(GameTime gametime)
        {
            // Set oldState the first time
            if (oldState == null)
            {
                oldState = Keyboard.GetState();
            }

            // Get new keyboard state
            var newState = Keyboard.GetState();

            keyPress = new List<Keys>();

            foreach(var i in newState.GetPressedKeys())
            {
                if (oldState.IsKeyUp(i))
                {
                    keyPress.Add(i);
                }
            }

            oldState = newState;
            

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
