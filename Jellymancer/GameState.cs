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
                                     currentMap.startX, currentMap.startY);
            currentMap.AddActor(pc);
            pc.rng = rng;
            pc.ExplodeAndPullIn();
        }


        // Tile dimesons in pixels
        const int TILE_WIDTH = 32;
        const int TILE_HEIGHT = 32;
        // Offset of where to show pixels
        const int X_OFFSET = 320;
        const int Y_OFFSET = 32;
        // Amount of tiles to show
        const int PLAYFIELD_WIDTH = 20;
        const int PLAYFIELD_HEIGHT = 20;

        double timeSince = 0.0f;
        const double ANIMATION_TIME = 0.3f;

        int camera_x = 0;
        int camera_y = 0;
        int last_camera_x = 0;
        int last_camera_y = 0;

        /// <summary>
        /// Draw game to the screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // Move camera to have Jelly roughly in middle
            var currentCameraX = camera_x;
            var currentCameraY = camera_y;
            camera_x = Math.Min(Math.Max(pc.x - PLAYFIELD_WIDTH / 2, 0), currentMap.Width - PLAYFIELD_WIDTH);
            camera_y = Math.Min(Math.Max(pc.y - PLAYFIELD_HEIGHT / 2, 0), currentMap.Height - PLAYFIELD_HEIGHT);
            if (currentCameraX != camera_x || currentCameraY != camera_y)
            {
                last_camera_x = currentCameraX;
                last_camera_y = currentCameraY;
            }

            // Draw Tiles
            for (var ix = camera_x; (ix < currentMap.Width) && (ix <= camera_x + PLAYFIELD_WIDTH) ; ++ix)
            {
                for (var iy = camera_y; (iy < currentMap.Height) && (iy <= camera_y + PLAYFIELD_HEIGHT); ++iy)
                {
                    spriteBatch.Draw(currentMap.map[ix, iy].img,
                                     new Vector2((ix - camera_x) * TILE_WIDTH + X_OFFSET,
                                                 (iy - camera_y) * TILE_HEIGHT + Y_OFFSET),
                                     Color.White);
                }
            }

            timeSince += gameTime.ElapsedGameTime.TotalSeconds;

            // Draw actors
            foreach (var i in currentMap.Actors)
            {
                var timeRatio = Math.Min(1, timeSince / ANIMATION_TIME);
                double x = i.x * timeRatio + i.lastTurnX * (1.0 - timeRatio);
                double y = i.y * timeRatio + i.lastTurnY * (1.0 - timeRatio);
                if ((x > camera_x) && 
                    (x < (camera_x + PLAYFIELD_WIDTH)) && 
                    (y > camera_y) && 
                    (y <(camera_y + PLAYFIELD_HEIGHT)))
                {
                    spriteBatch.Draw(i.sprite,
                                     new Vector2((int)((x - camera_x) * TILE_WIDTH + X_OFFSET),
                                                 (int)((y - camera_y) * TILE_HEIGHT + Y_OFFSET)),
                                     Color.White);
                }
            }

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
                var tileOverX = (mouseState.X - X_OFFSET) / TILE_WIDTH + camera_x;
                var tileOverY = (mouseState.Y - Y_OFFSET) / TILE_HEIGHT + camera_y;
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
                        if (chokedBy.Count > 0)
                        {
                            var chokedByDude = chokedBy.OrderByDescending(j => j.Value);
                            chokedByDude.First().Key.Killed(i);
                        }
                    }
                }

                // Kill deads
                currentMap.KillDeadActors();
            }

        }
    }
}
