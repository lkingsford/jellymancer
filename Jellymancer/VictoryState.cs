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

        // We're word wrapping manually here...
        private const string winningText =
@"The corrupted brother, consumed, would be a threat no more
with the remains of its royal jelly reverted back to a natural,
insentient state.
Gluttony, whilst not dead, was restrained by Wisdom to make
both stronger.
And the new royal jelly left the Tomb of the Jelly King to seek
the brethren of its two strong cores.";
        private SpriteFont font;

        public override void LoadContent()
        {
            victoryTitle = content.Load<Texture2D>("Victory/Victory");
            font = content.Load<SpriteFont>("Victory/Story");
            base.LoadContent();
        }

        public override void Draw(GameTime gametime)
        {
            spriteBatch.GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.Draw(victoryTitle,
                             new Vector2((1280 - victoryTitle.Width) / 2, 50),
                             Color.White);
            spriteBatch.DrawString(font, winningText, new Vector2(230, 400), Color.Black);
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
