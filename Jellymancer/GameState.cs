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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Jellymancer
{
    class GameState : State
    {
        public GameState(ContentManager content, List<State> states)
        {
            this.content = content;
            this.globalMap = new GlobalMap(content);
            this.states = states;
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

        /// <summary>
        /// It amused me
        /// </summary>
        public GluttonEnemy badHombre;

        public bool musicOn = true;

        public Random rng = new Random();

        public bool gameover = false;

        /// <summary>
        /// Start a game
        /// </summary>
        public void NewGame()
        {
            currentMap = globalMap.GetChunk(0, 0);
            pc = new PlayerCharacter(content.Load<Texture2D>("Game/Sprites/JellyCore"),
                                     content.Load<Texture2D>("Game/Sprites/JellyBody"),
                                     currentMap.startX, currentMap.startY, rng);
            currentMap.AddActor(pc);
            pc.ExplodeAndPullIn();

            healthui = new Texture2D[5];
            healthui[4] = content.Load<Texture2D>("Game/UI/Health4");
            healthui[3] = content.Load<Texture2D>("Game/UI/Health3");
            healthui[2] = content.Load<Texture2D>("Game/UI/Health2");
            healthui[1] = content.Load<Texture2D>("Game/UI/Health1");

            // Find victory dude
            badHombre = (GluttonEnemy)currentMap.Actors.First(i => i.GetType() == typeof(GluttonEnemy));

            gluttonTalks = new SoundEffect[4];
            gluttonTalks[0] = content.Load<SoundEffect>("Game/Sounds/Glutton1");
            gluttonTalks[1] = content.Load<SoundEffect>("Game/Sounds/Glutton2");
            gluttonTalks[2] = content.Load<SoundEffect>("Game/Sounds/Glutton3");
            gluttonTalks[3] = content.Load<SoundEffect>("Game/Sounds/Glutton4");

            gluttonTalked = new bool[4];
            gluttonTalked[0] = false;
            gluttonTalked[1] = false;
            gluttonTalked[2] = false;
            gluttonTalked[3] = false;

            // Add BGM
            tunes = new List<Song>();
            for (var i = 0; i <= 5; ++i)
            {
                tunes.Add(content.Load<Song>($"Game/BGM/{i}"));
            }
            // Randomise
            tunes = tunes.OrderBy(i => rng.Next()).ToList(); 
            StartTunes();
                    
        }

        Texture2D[] healthui;

        List<Song> tunes;
        int currentTune = 0;

        // Tile dimesons in pixels
        const int TILE_WIDTH = 32;
        const int TILE_HEIGHT = 32;
        // Offset of where to show pixels
        const int X_OFFSET = 0;
        const int Y_OFFSET = 0;
        // Amount of tiles to show
        const int PLAYFIELD_WIDTH = 40;
        const int PLAYFIELD_HEIGHT = 22;

        double timeSince = 0.0f;
        const double ANIMATION_TIME = 0.3f;
        const double SLOW_ANIMATION_TIME = 1.0f;

        int camera_x = 0;
        int camera_y = 0;
        int last_camera_x = 0;
        int last_camera_y = 0;
        private List<State> states;

        // Dead actor list for animation
        private List<Actor> deadActors;

        /// <summary>
        /// Draw game to the screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);

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
            var timeRatio = Math.Min(1, timeSince / ANIMATION_TIME);
            var slowRatio = Math.Min(1, timeSince / SLOW_ANIMATION_TIME);

            foreach (var i in currentMap.Actors)
            {
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

            // Draw dead actor animation
            if (deadActors != null)
            {
                foreach (var i in deadActors)
                {
                    spriteBatch.Draw(
                        i.sprite,
                        new Rectangle((int)((i.x - camera_x + (slowRatio * 0.5)) * TILE_WIDTH + X_OFFSET),
                                      (int)((i.y - camera_y + (slowRatio * 0.5)) * TILE_HEIGHT + Y_OFFSET),
                                      (int)((1.0 - slowRatio) * TILE_WIDTH),
                                      (int)((1.0 - slowRatio) * TILE_HEIGHT)),
                        Color.White);
                }
            }

            // Draw health
            spriteBatch.Draw(healthui[pc.hp], new Vector2(50, 50), Color.White);

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

            bool acted = false; 
            
            if (keyPress.Contains(Keys.F1))
            {
                musicOn = !musicOn;
                if (musicOn)
                {
                    StartTunes();
                }
                else
                {
                    StopTunes();
                }
            }

            if (keyPress.Contains(Keys.F2))
            {
                NextTune();
            }

            if (mousePress[LEFT_BUTTON])
            {
                foreach (var i in currentMap.Actors)
                {
                    i.lastTurnX = i.x;
                    i.lastTurnY = i.y;
                }

                var mouseState = Mouse.GetState();
                var tileOverX = (mouseState.X - X_OFFSET) / TILE_WIDTH + camera_x;
                var tileOverY = (mouseState.Y - Y_OFFSET) / TILE_HEIGHT + camera_y;
                pc.MoveTowards(tileOverX, tileOverY);
                timeSince = 0;
                acted = true;
            }

            if (mousePress[RIGHT_BUTTON])
            {
                var mouseState = Mouse.GetState();
                var tileOverX = (mouseState.X - X_OFFSET) / TILE_WIDTH + camera_x;
                var tileOverY = (mouseState.Y - Y_OFFSET) / TILE_HEIGHT + camera_y;
                var a = currentMap.Actors.FirstOrDefault(i => (i.x == tileOverX) && (i.y == tileOverY));
                if (a != null && a.parent == pc)
                {
                    acted = true;
                    a.dead = true;
                    pc.eatSelf((JellyBit)a);
                }
            }

            // If any move made
            if (acted)
            {
                // Do choking
                //   To prevent changing in foreach
                //   Also to prevent checking every stupid jelly everywhere
                var actorsBeforeChoke = currentMap.Actors.Where(i => (Math.Abs(i.x - pc.x) < 30) && (Math.Abs(i.y - pc.y) < 30)).ToList();

                foreach(var i in actorsBeforeChoke)
                {
                    int neighboursOccupied = 0;
                    var chokedBy = new Dictionary<Actor, int>();

                    if (i.characterParts.Count == 0)
                    {
                        // This is if it's a regular one bit thing
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

                        if (i.foodOnly)
                        {
                            if (neighboursOccupied >= 1)
                            {
                                if (chokedBy.Count > 0)
                                {
                                    i.Choke();
                                    var chokedByDude = chokedBy.OrderByDescending(j => j.Value);
                                    chokedByDude.First().Key.Killed(i);
                                }
                            }
                        }
                        else
                        {
                            if (neighboursOccupied >= 8)
                            {
                                if (chokedBy.Count > 0)
                                {
                                    i.Choke();
                                    var chokedByDude = chokedBy.OrderByDescending(j => j.Value);
                                    chokedByDude.First().Key.Killed(i);
                                }
                            }
                        }
                    } 
                    else
                    {
                        // This is a Jelly and has to be dealt with funnily
                        // Every bit has to be surrounded by itself or an enemy to die
                        var parts = i.characterParts.ToList();
                        parts.Add(i);
                        int surroundedCount = 0;

                        // Get list of open spaces around the outside
                        var surroundingFloor = new List<Tuple<int, int>>();
                        foreach (var j in parts)
                        {
                            for (var ix = j.x - 1; ix <= j.x + 1; ++ix)
                            {
                                for (var iy = j.y - 1; iy <= j.y + 1; ++iy)
                                {
                                    // If not part of self, and open
                                    if (!(parts.Any(k => (k.x == ix && k.y == iy))) &&
                                        currentMap.map[ix, iy].walkable)
                                    {
                                        surroundingFloor.Add(new Tuple<int, int>(ix, iy));
                                    }
                                }
                            }
                        }

                        // Check if surrounds are occupied
                        foreach (var j in surroundingFloor)
                        {
                            var monsterOnTile = currentMap.Actors.FirstOrDefault(k => (k.x == j.Item1) && (k.y == j.Item2));
                            if (monsterOnTile != null && !monsterOnTile.foodOnly)
                            {
                                if (!monsterOnTile.parent?.characterParts.Contains(i) ?? false)
                                {
                                    if (chokedBy.ContainsKey(monsterOnTile))
                                    {
                                        ++chokedBy[monsterOnTile];
                                    }
                                    else
                                    {
                                        chokedBy[monsterOnTile] = 1;
                                    }
                                }
                                surroundedCount += 1;
                            }
                        }

                        // If all surrounds occupied, it's dead Jim
                        if ((surroundedCount >= surroundingFloor.Count) || (surroundingFloor.Count == 0))
                        {
                            i.dead = true;
                            if (chokedBy.Count > 0)
                            {
                                var chokedByDude = chokedBy.OrderByDescending(j => j.Value);
                                chokedByDude.First().Key.Killed(i);
                            }
                            i.Choke();
                            currentMap.KillDeadActors();
                        }

                    }
                }

                // Kill deads
                deadActors = currentMap.KillDeadActors();
                
                // Move all the baddies that are kinda close
                foreach(var i in currentMap.Actors.Where(i=>(Math.Abs(i.x - pc.x) < 20) && (Math.Abs(i.y - pc.y) < 20)))
                {
                    i.Act();
                }

                // Play sounds if necessary
                if (pc.y > 10 && !gluttonTalked[0])
                {
                    gluttonTalks[0].Play();
                    gluttonTalked[0] = true;
                }

                if (pc.y > 50 && !gluttonTalked[1])
                {
                    gluttonTalks[1].Play();
                    gluttonTalked[1] = true;
                }


                if (pc.y > 100 && !gluttonTalked[2])
                {
                    gluttonTalks[2].Play();
                    gluttonTalked[2] = true;
                }

                if (!gluttonTalked[3] && (Math.Abs(pc.x - badHombre.x) + Math.Abs(pc.y - badHombre.y) < 20))
                {
                    gluttonTalks[3].Play();
                    gluttonTalked[3] = true;
                }

                if (musicOn && 
                    MediaPlayer.State != MediaState.Playing)
                {
                    NextTune();
                }

                // Check losing and victory condition
                if (pc.dead)
                {
                    GameOver();
                }

                if (badHombre.dead)
                {
                    Victory();
                }

                currentMap.KillDeadActors();
            }

        }

        private void StopTunes()
        {
            MediaPlayer.Stop();
        }

        private void StartTunes()
        {
            if (currentTune >= tunes.Count) { currentTune = 0; }
            MediaPlayer.Play(tunes[currentTune]);
        }

        private void NextTune()
        {
            currentTune += 1;
            if (currentTune >= tunes.Count) { currentTune = 0; }
            MediaPlayer.Play(tunes[currentTune]);
        }

        SoundEffect[] gluttonTalks;
        bool[] gluttonTalked;

        private void GameOver()
        {
            states.Remove(this);
            var gameOverState = new GameOverState(content);
            gameOverState.LoadContent();
            gameOverState.SpriteBatch = SpriteBatch;
            states.Add(gameOverState);
        }

        private void Victory()
        {
            states.Remove(this);
            var victoryState = new VictoryState(content);
            victoryState.LoadContent();
            victoryState.SpriteBatch = SpriteBatch;
            states.Add(victoryState);
        }
    }
}
