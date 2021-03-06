﻿using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Jellymancer
{
    class GameOverState : State
    {
        Song gameOverSong;
        public GameOverState(ContentManager content)
        {
            this.content = content;
            gameOverSong = content.Load<Song>("GameOver/GameOverMusic");
            MediaPlayer.Play(gameOverSong);

        }

        Texture2D gameOverTitle;

        public override void LoadContent()
        {
            gameOverTitle = content.Load<Texture2D>("GameOver/GameOver");
            base.LoadContent();
        }

        public override void Draw(GameTime gametime)
        {
            spriteBatch.GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.Draw(gameOverTitle,
                             new Vector2((1280 - gameOverTitle.Width) / 2, 100),
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
                MediaPlayer.Stop();
            }
        }
    }
}
