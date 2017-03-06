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

        Texture2D pointer;

        /// <summary>
        /// Draw the state to the screen
        /// </summary>
        /// <param name="gametime"></param>
        public virtual void Draw(GameTime gametime)
        {
            spriteBatch.Begin();
            if (pointer != null)
            {
                spriteBatch.Draw(pointer, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Color.White);
            }
            spriteBatch.End();
        }

        KeyboardState oldKeyState;
        MouseState oldMouseState;

        // Whether a new key push
        protected List<Keys> keyPress;

        // Whether a new mouse push
        protected bool[] mousePress = new bool[3];

        // Consts for accessing mousePress
        const int LEFT_BUTTON = 0;
        const int RIGHT_BUTTON = 1;
        const int MIDDLE_BUTTON = 2;

        /// <summary>
        /// Update the logic of the game
        /// </summary>
        /// <param name="gametime"></param>
        public virtual void Update(GameTime gametime)
        {
            // Set old states the first time
            if (oldKeyState == null)
            {
                oldKeyState = Keyboard.GetState();
            }
            
            if (oldMouseState ==null)
            {
                oldMouseState = Mouse.GetState();
            }

            // Get new keyboard state
            var newKeyState = Keyboard.GetState();
            keyPress = new List<Keys>();

            // Get keys that have been newly pressed
            foreach(var i in newKeyState.GetPressedKeys())
            {
                if (oldKeyState.IsKeyUp(i))
                {
                    keyPress.Add(i);
                }
            }

            // Get new mouse state
            var newMouseState = Mouse.GetState();
            mousePress[LEFT_BUTTON] = (oldMouseState.LeftButton == ButtonState.Released) && (newMouseState.LeftButton == ButtonState.Pressed);
            mousePress[RIGHT_BUTTON] = (oldMouseState.RightButton == ButtonState.Released) && (newMouseState.RightButton == ButtonState.Pressed);
            mousePress[MIDDLE_BUTTON] = (oldMouseState.MiddleButton == ButtonState.Released) && (newMouseState.MiddleButton == ButtonState.Pressed);

            oldKeyState = newKeyState;
            oldMouseState = newMouseState;
        }

        /// <summary>
        /// Load content into state
        /// </summary>
        public virtual void LoadContent()
        {
            pointer = content.Load<Texture2D>("Common/Pointer");
        }

        public bool toQuit = false;
    }
}
