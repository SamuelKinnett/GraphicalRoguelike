using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GraphicalRoguelike
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        SpriteSheet OverworldTerrain;   //land, grass etc.
        TerrainGenerator terrainGenerator;
        Rendering renderer;

        Texture2D floor;    //Normal floor tiles
        public int[,] worldMap;

        const int testWorldSize = 100; //TESTING
        double worldGenerationTime = 0; //TESTING

        enum GameState
        {
            SplashScreen,
            MainMenu,
            GeneratingWorld,
            ViewingWorld
        };

        //set current game state
        GameState currentGameState = GameState.SplashScreen;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //set window resolution
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            //set window title
            this.Window.Title = "Cellular Automata Map Generation";
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
            worldMap = new int[testWorldSize, testWorldSize];

            terrainGenerator = new TerrainGenerator();
            renderer = new Rendering();

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

            floor = this.Content.Load<Texture2D>("Floor");
            OverworldTerrain = new SpriteSheet(floor, 39, 21);

            font = this.Content.Load<SpriteFont>("NormalText");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            switch (currentGameState)
            {

                case GameState.SplashScreen:
                    if (2 - (float)gameTime.TotalGameTime.Seconds <= 0)
                    {
                        currentGameState = GameState.GeneratingWorld;
                    }
                    break;

                case GameState.GeneratingWorld:

                    //testing purposes
                    double temp;
                    double temp2; 

                    temp = gameTime.TotalGameTime.TotalMilliseconds;
                    Array.Copy(terrainGenerator.GenerateWorld(testWorldSize, testWorldSize, "TestWorld"), worldMap, worldMap.Length);
                    temp2 = gameTime.TotalGameTime.TotalMilliseconds;

                    worldGenerationTime = (float)temp2 - (float)temp;

                    currentGameState = GameState.ViewingWorld;
                    break;

                case GameState.ViewingWorld:

                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            /*
            Vector2 tempVector;
            int tempCount = 0;
            for (int y = 0; y < 39; y++)
            {
                for (int x = 0; x < 21; x++)
                {
                    tempVector.X = x * 16;
                    tempVector.Y = y * 16;
                    OverworldTerrain.Draw(spriteBatch, tempVector, tempCount);
                    tempCount++;
                }
            }
            */

            switch (currentGameState)
            {

                case(GameState.SplashScreen):
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin();
                    spriteBatch.DrawString(font, "A game by Samuel Kinnett", new Vector2(Window.ClientBounds.Width / 2 - 120, Window.ClientBounds.Height / 2 - 100), Color.White);
                    spriteBatch.End();
                    break;

                case(GameState.GeneratingWorld):
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin();
                    spriteBatch.DrawString(font, "Generating World...", new Vector2(0, 0), Color.White);
                    spriteBatch.End();
                    break;

                case(GameState.ViewingWorld):
                    GraphicsDevice.Clear(new Color(0, 0, 85));
                    renderer.RenderWorld(spriteBatch, OverworldTerrain, testWorldSize, testWorldSize, worldMap);
                    spriteBatch.Begin();
                    spriteBatch.DrawString(font, "This world was generated in " + worldGenerationTime + " milliseconds.", new Vector2(0, 450), Color.White);
                    spriteBatch.End();
                    break;

                default:
                    GraphicsDevice.Clear(Color.Black);
                    //spriteBatch.Begin();
                    //spriteBatch.DrawString(font, "Something has gone awry", new Vector2(100, 100), Color.Black);
                    //spriteBatch.End();
                break;

            }

            base.Draw(gameTime);
        }
    }
}
