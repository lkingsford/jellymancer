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
    class VictoryState : State
    {
        public VictoryState(ContentManager content)
        {
            this.content = content;
        }

        Texture2D victoryTitle;

        public override void LoadContent()
        {
            victoryTitle = content.Load<Texture2D>("Victory/Victory");
            base.LoadContent();
        }

        public override void Draw(GameTime gametime)
        {
            spriteBatch.GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.Draw(victoryTitle,
                             new Vector2((1280 - victoryTitle.Width) / 2, 50),
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
