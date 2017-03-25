using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jellymancer
{
    class HowToPlayState : State
    {
        public HowToPlayState(ContentManager content)
        {
            this.content = content;
        }

        Texture2D howToPlayTitle;

        public override void LoadContent()
        {
            howToPlayTitle = content.Load<Texture2D>("Playing/Playing");
            base.LoadContent();
        }

        public override void Draw(GameTime gametime)
        {
            spriteBatch.GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.Draw(howToPlayTitle,
                             new Vector2(0,0),
                             Color.White);
            spriteBatch.End();
            base.Update(gametime);
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
            if (mousePress[LEFT_BUTTON])
            {
                this.toClose = true;
            }
        }
    }
}
