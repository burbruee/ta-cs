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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace TROLLArena
{
    enum GameState
    {
        Title,
        LevelChange,
        Playing,
        Died,
        Gameover
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Screen Size
        internal const int SCREEN_WIDTH = 960;
        internal const int SCREEN_HEIGHT = 540;
                
        //Player
        const float PLAYER_SPEED_SLOW = 5f;
        const float PLAYER_SPEED_FAST = 10f;
        private Player player;

        //Textures
        Texture2D backgroundTexture;
        Texture2D playerTexture;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        static Random random;

        GameState gameState;

        private string col;
        protected Vector2 mousePos;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            this.IsMouseVisible = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            random = new Random();
            this.gameState = GameState.Playing;

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

            font = Content.Load<SpriteFont>(@"Textures\Tahoma");
            backgroundTexture = Content.Load<Texture2D>(@"Textures\background");
            playerTexture = Content.Load<Texture2D>(@"Textures\TROLLET_HD_small");

            this.player = new Player(playerTexture);
            //player.StartScale(1.25f, 1);
            
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
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            mousePos = new Vector2(mouseState.X, mouseState.Y);

            // Allows the game to exit
            if (gamePadState.Buttons.Back == ButtonState.Pressed)
                this.Exit();

            switch (this.gameState)
            {
                case GameState.Title:
                    break;

                case GameState.LevelChange:
                    break;

                case GameState.Playing:

                    for (int i = Actor.Actors.Count - 1; i >= 0; i--)
                    {
                        Actor actor = Actor.Actors[i];
                        actor.Update(gameTime);

                        if (actor is Player)
                        {
                            Vector2 direction = new Vector2(mouseState.X, mouseState.Y);
                            /*
                            if (direction.Length() > 0f)
                                direction.Normalize();
                            else
                                direction = Vector2.Zero;
                            */

                            actor.Position = direction;

                            
                            if (actor.Position.X < 0)
                                actor.position.X = 0;
                            if (actor.Position.Y < 0)
                                actor.position.Y = 0;
                            if (actor.Position.X > graphics.PreferredBackBufferWidth - actor.Texture.Width)
                                actor.position.X = graphics.PreferredBackBufferWidth - actor.Texture.Width;
                            if (actor.Position.Y > graphics.PreferredBackBufferHeight - actor.Texture.Height)
                                actor.position.Y = graphics.PreferredBackBufferHeight - actor.Texture.Height;
                            
                        }
                    }

                    break;
                
                case GameState.Died:
                    break;
                
                case GameState.Gameover:
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
            
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            
            switch (this.gameState)
            {
                case GameState.Title:
                    break;
                
                case GameState.LevelChange:
                    break;
                
                case GameState.Playing:
                case GameState.Died:
                case GameState.Gameover:

                    //Draw Actors
                    spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);
                    Actor.DrawActors(spriteBatch);
                    
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
