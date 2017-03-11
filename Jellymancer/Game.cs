using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Jellymancer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<State> states = new List<State>();

        MainMenu mainMenu;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            mainMenu = new MainMenu(Content);
            states.Add(mainMenu);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load content for each state
            foreach (var i in states)
            {
                i.SpriteBatch = spriteBatch;
                i.LoadContent();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            foreach (var i in states)
            {
                i.LoadContent();
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            states[states.Count - 1].Update(gameTime);

            // Exit if the state demands it
            if (states[states.Count -1].toQuit)
            {
                Exit();
            }

            // If in menu
            if (states[states.Count - 1] == mainMenu)
            {
                if (mainMenu.chosen)
                {
                    switch (mainMenu.selectedItem)
                    {
                        case 0:
                            StartGame();
                            break;
                        case 1:
                            Exit();
                            break;
                    }
                    mainMenu.chosen = false;
                }
            }

            if (states[states.Count-1].toClose)
            {
                states.Remove(states[states.Count - 1]);
                if (states.Count == 0)
                {
                    states.Add(mainMenu);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.BlueViolet);
            states[states.Count - 1].Draw(gameTime);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Start the game state
        /// </summary>
        private void StartGame()
        {
            var game = new GameState(Content, states);
            game.LoadContent();
            game.SpriteBatch = spriteBatch;
            game.NewGame();
            states.Add(game);

            var intro = new IntroState(Content);
            intro.LoadContent();
            intro.SpriteBatch = spriteBatch;
            states.Add(intro);
        }
    }
}
