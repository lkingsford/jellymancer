using Jellymancer.GameParts;
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

        public Random rng = new Random();

        /// <summary>
        /// Start a game
        /// </summary>
        public void NewGame()
        {
            currentMap = globalMap.GetChunk(0, 0);
            pc = new PlayerCharacter(content.Load<Texture2D>("Game/Sprites/JellyCore"),
                                     content.Load<Texture2D>("Game/Sprites/JellyBody"),
                                     8, 8);
            currentMap.AddActor(pc);
            pc.rng = rng;
        }


        const int WIDTH = 32;
        const int HEIGHT = 32;
        const int X_OFFSET = 320;
        const int Y_OFFSET = 32;

        double timeSince = 0.0f;
        const double ANIMATION_TIME = 0.3f;

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

            timeSince += gameTime.ElapsedGameTime.TotalSeconds;

            // Draw actors
            foreach (var i in currentMap.Actors)
            {
                var timeRatio = Math.Min(1, timeSince / ANIMATION_TIME);
                double x = i.x * timeRatio + i.lastTurnX * (1.0 - timeRatio);
                double y = i.y * timeRatio + i.lastTurnY * (1.0 - timeRatio);
                spriteBatch.Draw(i.sprite, new Vector2((int)(x * WIDTH + X_OFFSET), (int)(y * HEIGHT + Y_OFFSET)), Color.White);
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


            spriteBatch.End();
            base.Draw(gameTime);
        }

        // Check if map walkable
        private bool Walkable(int x, int y)
        {
            if (x < 0 || y < 0 || x >= currentMap.Width || y >= currentMap.Height) { return false; }
            return currentMap.map[x, y].walkable;
        }

        // Check if monster on tile
        private Actor MonsterOnTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= currentMap.Width || y >= currentMap.Height) { return null; }

            var a = currentMap.Actors.Where(i => (i.x == x) && (i.y == y));
            return a.Count() > 0 ? a.First() : null;
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
                foreach(var i in currentMap.Actors)
                {
                    i.lastTurnX = i.x;
                    i.lastTurnY = i.y;
                }

                var mouseState = Mouse.GetState();
                var tileOverX = (mouseState.X - X_OFFSET) / WIDTH;
                var tileOverY = (mouseState.Y - Y_OFFSET) / HEIGHT;
                pc.MoveTowards(tileOverX, tileOverY);
                timeSince = 0;

                // Do choking
                //   To prevent changing in foreach
                var actorsBeforeChoke = new List<Actor>(currentMap.Actors);
                foreach(var i in actorsBeforeChoke)
                {
                    int neighboursOccupied = 0;
                    var chokedBy = new Dictionary<Actor, int>();

                    // Check if neighbours are choking or by wall
                    for (var ix = i.x - 1; ix <= i.x + 1; ++ix)
                    {
                        for (var iy = i.y - 1; iy <= i.y + 1; ++iy)
                        {
                            if (!Walkable(ix, iy))
                            {
                                neighboursOccupied += 1;
                            }
                            else
                            {
                                var a = MonsterOnTile(ix, iy);
                                if (a != null)
                                {
                                    if ((a.choking == true) &&
                                        (!i.characterParts.Contains(a)) &&
                                        (!a.characterParts.Contains(i)) &&
                                        (i != a))
                                    {
                                        // It was easier to deal with this logical bit seperately
                                        
                                        if (((i.parent != null || a.parent != null) && (i.parent != a.parent)) ||
                                            ((i.parent == null && a.parent == null)))
                                        {
                                            neighboursOccupied += 1;
                                            if (chokedBy.ContainsKey(a))
                                            {
                                                ++chokedBy[a];
                                            }
                                            else
                                            {
                                                chokedBy[a] = 1;
                                            }
                                        }
                                        
                                    }
                                }
                            }
                        }
                    }

                    if (neighboursOccupied >= 8)
                    {
                        i.Choke();
                        var chokedByDude = chokedBy.OrderByDescending(j => j.Value);
                        chokedByDude.First().Key.Killed(i);
                    }
                }

                // Kill deads
                currentMap.KillDeadActors();
            }

        }
    }
}
