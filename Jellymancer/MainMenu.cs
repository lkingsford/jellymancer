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
            base.LoadContent();
        }

        SpriteFont font;
        Texture2D logo;

        List<String> items = new List<String>();
        public int selectedItem;
        public bool chosen;

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
                                       new Vector2(100, 450 + i * 60),
                                       i == selectedItem ? Color.White : Color.Gray);
                spriteBatch.Draw(logo, new Vector2(140, 100), Color.White);
            }
            
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Update according to input
        /// </summary>
        /// <param name="gametime"></param>
        public override void Update(GameTime gametime)
        {
            // Do key press stuff
            base.Update(gametime);

            if (keyPress.Contains(Keys.Escape))
            {
                toQuit = true;
            }

            if (keyPress.Contains(Keys.Up))
            {
                menuUp();
            }

            if (keyPress.Contains(Keys.Down))
            {
                menuDown();
            }

            if (keyPress.Contains(Keys.Enter) || keyPress.Contains(Keys.Space))
            {
                chosen = true;
            }
        }

        private void menuDown()
        {
            selectedItem += 1;
            selectedItem = Math.Min(items.Count - 1, selectedItem);
        }

        private void menuUp()
        {
            selectedItem -= 1;
            selectedItem = Math.Max(selectedItem, 0);
        }
    }
}
