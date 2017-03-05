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
    class MainMenu : State
    {
        public MainMenu(ContentManager content)
        {
            this.content = content;
            items.Add("Start Game");
            items.Add("Quit");
        }

        public override void LoadContent()
        {
            font = content.Load<SpriteFont>("Menu/Body");
            logo = content.Load<Texture2D>("Menu/Logo");
        }

        SpriteFont font;
        Texture2D logo;

        List<String> items = new List<String>();
        byte selectedItem;

        /// <summary>
        /// Draw the game menu
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            // Using for instead of foreach so to get index
            for (int i = 0; i < items.Count; i++)
            {
                spriteBatch.DrawString(font,
                                       items[i],
                                       new Vector2(100, 600 + i * 40),
                                       i == selectedItem ? Color.White : Color.Gray);
                spriteBatch.Draw(logo, new Vector2(140, 100), Color.White);
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Update according to input
        /// </summary>
        /// <param name="gametime"></param>
        public override void Update(GameTime gametime)
        {
            // Do key press stuff
            base.Update(gametime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                toQuit = true;
            }
        }

    }
}
