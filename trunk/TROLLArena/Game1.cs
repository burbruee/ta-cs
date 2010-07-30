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
        internal const int SCREEN_WIDTH = 1280;
        internal const int SCREEN_HEIGHT = 720;

        //Player
        const float PLAYER_SPEED_SLOW = 5f;
        const float PLAYER_SPEED_FAST = 10f;
        private Player player;

        //Font
        double score = 0;
        string framerate = "";

        //Textures
        Texture2D backgroundTexture;
        Texture2D playerTexture;
        Texture2D enemyTexture;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        static Random random;

        GameState gameState;
        float deltaFPSTime = 0;
        private float _ElapsedTime, _TotalFrames, _Fps;
        private bool _ShowFPS;

        private int enemies;

        private string col;
        protected Vector2 mousePos;

        public bool ShowFPS
        {
            get { return _ShowFPS; }
            set { _ShowFPS = value; }
        }

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
            enemyTexture = Content.Load<Texture2D>(@"Textures\japan_HD");

            this.player = new Player(playerTexture);            
            new BouncingEnemy(enemyTexture, random.Next(1,10), random.Next(1,10));
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
                    float elapsed = (float)gameTime.ElapsedRealTime.TotalMilliseconds;                    
                    deltaFPSTime += elapsed;

                    if (deltaFPSTime > 1000)
                    {
                        float fps = 1000 / elapsed;
                        framerate = fps.ToString("F2");
                        deltaFPSTime -= 1;
                    }
                    
                    //Update score
                    score += gameTime.ElapsedGameTime.TotalSeconds;                  

                    for (int i = Actor.Actors.Count - 1; i >= 0; i--)
                    {
                        Actor actor = Actor.Actors[i];
                        actor.Update(gameTime);



                        if (actor is Player)
                        {
                            Vector2 direction = new Vector2(mouseState.X, mouseState.Y);

                            actor.Position = direction;


                            if (actor.Position.X < actor.Origin.X)
                                actor.position.X = actor.Origin.X;
                            if (actor.Position.Y < actor.Origin.Y)
                                actor.position.Y = actor.Origin.Y;
                            if (actor.Position.X > graphics.PreferredBackBufferWidth - actor.Origin.X)
                                actor.position.X = graphics.PreferredBackBufferWidth - actor.Origin.X;
                            if (actor.Position.Y > graphics.PreferredBackBufferHeight - actor.Origin.Y)
                                actor.position.Y = graphics.PreferredBackBufferHeight - actor.Origin.Y;

                            continue;
                        }

                        BouncingEnemy enemy = actor as BouncingEnemy;
                        //Update enemies
                        enemies = enemy.TotalSpawns;
                        if (enemy != null)
                        {
                            if (Actor.CheckCollision(enemy, this.player))
                            {
                                this.gameState = GameState.Died;
                                Screenshot();
                                Highscores.sendScore("1", "Burbruee", "XNA Test", (int)score);
                            }
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
                    spriteBatch.DrawString(font, "Enemies: " + enemies, new Vector2(10, 630), Color.White);
                    spriteBatch.DrawString(font, "Score: " + score.ToString("F2"), new Vector2(10, 690), Color.White);
                    spriteBatch.DrawString(font, "FPS: " + framerate, new Vector2(10, 660), Color.White);

                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void Screenshot()
        {            
            
            using (ResolveTexture2D screenshot = new ResolveTexture2D(graphics.GraphicsDevice,
                   graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
                   graphics.GraphicsDevice.PresentationParameters.BackBufferHeight, 1,
                   graphics.GraphicsDevice.PresentationParameters.BackBufferFormat
                                                         ))
            {
                graphics.GraphicsDevice.ResolveBackBuffer(screenshot);
                screenshot.Save("screenshot.png", ImageFileFormat.Png);
            }
        }

    }
}
