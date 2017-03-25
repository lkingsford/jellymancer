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
            authorFont = content.Load<SpriteFont>("Menu/Author");
            base.LoadContent();
        }

        SpriteFont font;
        SpriteFont authorFont;
        Texture2D logo;

        List<String> items = new List<String>();
        public int selectedItem;
        public bool chosen;

        public const int MENU_LEFT = 100;
        public const int MENU_WIDTH = 300;
        public const int MENU_TOP = 450;
        public const int MENU_LINE_HEIGHT = 60;

        public const string AUTHOR_BLOCK =
@"
Lachlan Kingsford.
@thelochok
www.nerdygentleman.com
";

        /// <summary>
        /// Draw the game menu
        /// </summary>
        /// <param name="gametime"></param>
        public override void Draw(GameTime gametime)
        {
            spriteBatch.Begin();
            // Using for instead of foreach so to get index
            for (int i = 0; i < items.Count; i++)
            {
                spriteBatch.DrawString(font,
                                       items[i],
                                       new Vector2(MENU_LEFT, MENU_TOP + i * MENU_LINE_HEIGHT),
                                       i == selectedItem ? Color.White : Color.Gray);
                spriteBatch.DrawString(authorFont, AUTHOR_BLOCK, new Vector2(700, 350), Color.White);
                spriteBatch.Draw(logo, new Vector2(140, 100), Color.White);

            }
            
            spriteBatch.End();
            base.Draw(gametime);
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

            if (mousePress[LEFT_BUTTON])
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var left = MENU_LEFT;
                    var right = MENU_LEFT + MENU_WIDTH;
                    var top = MENU_TOP + i * MENU_LINE_HEIGHT;
                    var bottom = MENU_TOP + (i + 1) * MENU_LINE_HEIGHT;
                    var m = Mouse.GetState();
                    if (m.X > left && m.X < right && m.Y > top && m.Y < bottom)
                    {
                        chosen = true;
                        selectedItem = i;
                        break;
                    }
                }
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
