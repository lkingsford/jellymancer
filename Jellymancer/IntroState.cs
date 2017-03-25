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
    class IntroState : State
    {
        public IntroState(ContentManager content)
        {
            this.content = content;
        }

        // We're word wrapping manually here...
        private const string introText =
@"'So - that fool fell' thought the Ring of Wisdom about its former 
master, as it lied rapidly cooling on the cold slate of the dungeon.
It felt strange as it was itself consumed by the thoughtless jelly.
'Hmmm. The problem with adventurers is that they have so much
will of their own. Now this - this is something I can really control.'
It was not the first time - it was known that a jelly with a magical,
sentient core could become a 'Royal Jelly': A consuming, growing
beast - stronger than a ring alone, with the cleverness of an
ambitious man.
But as soon as the bonding was complete, it became rapidly
aware that it was not alone. It sensed that corrupted brother - 
the Cursed Ring of Gluttony - was in this same dungeon, with an
all-consuming hunger, forming the core of a royal jelly of its own.
The royal jelly knew: it must be stopped.
";
        private SpriteFont font;

        public override void LoadContent()
        {
            font = content.Load<SpriteFont>("Victory/Story");
            base.LoadContent();
        }

        public override void Draw(GameTime gametime)
        {
            spriteBatch.GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, introText, new Vector2(193, 100), Color.Black);
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
