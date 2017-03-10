using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Jellymancer
{
    class GameOverState : State
    {
        public GameOverState(ContentManager content)
        {
            this.content = content;
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Draw(GameTime gametime)
        {
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
