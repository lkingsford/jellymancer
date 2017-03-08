﻿using Jellymancer.GameParts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer
{
    class GameState : State
    {
        public GameState(ContentManager content)
        {
            this.content = content;
            this.globalMap = new GlobalMap(content);
        }

        /// <summary>
        /// Area being played on
        /// </summary>
        public MapChunk currentMap;

        /// <summary>
        /// The rest of the world
        /// </summary>
        public GlobalMap globalMap;

        /// <summary>
        /// The player. Seperate as to control and things.
        /// </summary>
        public PlayerCharacter pc;

        Random rng;

        /// <summary>
        /// Start a game
        /// </summary>
        public void NewGame()
        {
            currentMap = globalMap.GetChunk(0, 0);
            pc = new PlayerCharacter(content.Load<Texture2D>("Game/Sprites/JellyCore"),
                                     content.Load<Texture2D>("Game/Sprites/JellyBody"),
                                     16, 16);
            currentMap.AddActor(pc);

            rng = new Random();
            pc.random = rng;
        }


        const int WIDTH = 32;
        const int HEIGHT = 32;
        const int X_OFFSET = 320;
        const int Y_OFFSET = 32;

        /// <summary>
        /// Draw game to the screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();


            // Draw Tiles
            for (var ix = 0; ix < currentMap.Width; ++ix)
            {
                for (var iy = 0; iy < currentMap.Height; ++iy)
                {
                    spriteBatch.Draw(currentMap.map[ix, iy].img, new Vector2(ix * WIDTH + X_OFFSET, iy * HEIGHT + Y_OFFSET), Color.White);
                }
            }

            // Draw actors
            foreach (var i in currentMap.Actors)
            {
                spriteBatch.Draw(i.sprite, new Vector2(i.x * WIDTH + X_OFFSET, i.y * HEIGHT + Y_OFFSET), Color.White);
            }

            // Draw debug data - mouse position 
            var mouseState = Mouse.GetState();
            var tileOverX = (mouseState.X - X_OFFSET) / WIDTH;
            var tileOverY = (mouseState.Y - Y_OFFSET) / HEIGHT;
            spriteBatch.DrawString(content.Load<SpriteFont>("Game/Fonts/Debug"),
                                       string.Format("{0},{1}",tileOverX, tileOverY),
                                       new Vector2(100, 100),
                                       Color.Yellow
                                       );

            // Draw debug data - mouse position distance from dude
            var dist = Math.Sqrt(Math.Pow((tileOverX - pc.x), 2) + Math.Pow((tileOverY - pc.y), 2));
            spriteBatch.DrawString(content.Load<SpriteFont>("Game/Fonts/Debug"),
                                       dist.ToString(),
                                       new Vector2(100, 130),
                                       Color.Yellow
                                       );

            int y = 50; 
            foreach (var i in pc.characterParts)
            {
                spriteBatch.DrawString(content.Load<SpriteFont>("Game/Fonts/Debug"),
                                       $"{i.x} {i.y}",
                                       new Vector2(10, y),
                                       Color.Yellow
                                       );
                y += 30;
            }


            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Do control and update game stuff. Basically - game logic goes here.
        /// </summary>
        /// <param name="gametime"></param>
        public override void Update(GameTime gametime)
        {
            // Base needs to be called first to get pressed
            base.Update(gametime);


            if (mousePress[LEFT_BUTTON])
            {
                var mouseState = Mouse.GetState();
                var tileOverX = (mouseState.X - X_OFFSET) / WIDTH;
                var tileOverY = (mouseState.Y - Y_OFFSET) / HEIGHT;
                pc.MoveTowards(tileOverX, tileOverY);
            }

        }
    }
}
