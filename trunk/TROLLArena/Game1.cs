using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.IO;
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
        const float PLAYER_INVICIBILITY_TIME = 1.5f;
        private Player player;

        //Enemy
        const float ENEMY_BASE_SPEED = 2f;
        const float ENEMY_SPEED_VARIATION = 1f;
        float ENEMY_VELOCITY_X = 4f;
        float ENEMY_VELOCITY_Y = 3f;
        int enemyCount;

        //Font
        double score = 0;
        string framerate = "";        

        //Screens
        Color overlayColor = new Color(225, 0, 0, 143);

        //Lives
        private int lives;
        const int STARTING_LIVES = 3;

        //Screenshot
#if WINDOWS
        private ResolveTexture2D resolveTexture;
        private bool takeScreenshot;        
        private int screenshotNumber = 1;
        private Thread screenshotThread;
#endif

        //Textures
        Texture2D backgroundTexture;
        Texture2D playerTexture;
        Texture2D enemyTexture;
        Texture2D overlayTexture;
        Texture2D titleTexture;
        Texture2D levelChangeTexture;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        static Random random;

        GameState gameState;
        float deltaFPSTime = 0;

        double timer = 10;
        
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
            this.gameState = GameState.Title;



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
            enemyTexture = Content.Load<Texture2D>(@"Textures\japan");
            overlayTexture = Content.Load<Texture2D>(@"Textures\fill");
            levelChangeTexture = Content.Load<Texture2D>(@"Textures\GetReady");
            titleTexture = Content.Load<Texture2D>(@"Textures\title");


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
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        BeginGame();
                        this.gameState = GameState.LevelChange;
                    }
                    break;

                case GameState.LevelChange:

                    Thread.Sleep(3000);    
                    BeginLevel();
                    this.gameState = GameState.Playing;
                    
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
                    //this.score += gameTime.ElapsedGameTime.TotalSeconds;                  

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

                        Enemy enemy = actor as Enemy;
                        //Update enemies
                        enemyCount = (int)Actor.Actors.Count() - 1;
                        

                        if (gameTime.TotalGameTime.TotalSeconds > timer)
                        {
                            ENEMY_VELOCITY_X += .5f;
                            ENEMY_VELOCITY_Y += .5f;
                            Enemy.AddEnemies(1, enemyTexture, random.Next(0,200), ENEMY_BASE_SPEED, ENEMY_SPEED_VARIATION, ENEMY_VELOCITY_X, ENEMY_VELOCITY_Y);
                            score += 143;
                            timer = gameTime.TotalGameTime.TotalSeconds + 10d;
                        }
                         
                        if (enemy != null)
                        {
                            if (!player.IsInvincible && enemy.IsHarmful && Actor.CheckCollision(enemy, this.player))
                            {

                                IPHostEntry ip = null;
                                try
                                {
                                    ip = Dns.GetHostEntry("highscore.burbruee.se");
                                    Highscores.sendScore("1", "Burbruee", "Enemies: " + enemyCount + "<br />Died at " + gameTime.TotalGameTime.Minutes.ToString() + " min, " + gameTime.TotalGameTime.Seconds.ToString() + " sec", (int)score);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }

                                this.lives--;
                                this.gameState = GameState.Gameover;                                    
                            }
                        }
                    }

                    break;

                case GameState.Died:
                    Thread.Sleep(3000);
                    this.gameState = GameState.LevelChange;                                            

                    break;

                case GameState.Gameover:
                    takeScreenshot = true;
                    Screenshot();
                    Thread.Sleep(3000);                    
                    this.gameState = GameState.Title;
                    
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
                    DrawTitleScreen();
                    spriteBatch.DrawString(font, "PRESS SPACEBAR TO BEGIN", new Vector2(SCREEN_WIDTH/2-120, SCREEN_HEIGHT/2-30), Color.White);                    
                    break;                    

                case GameState.LevelChange:
                    DrawLevelChangeScreen();
                    break;

                case GameState.Playing:
                case GameState.Died:
                case GameState.Gameover:

                    //Draw Background
                    spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);

                    //Draw Actors                    
                    Actor.DrawActors(spriteBatch);
                    
                    //Draw Info
                    spriteBatch.DrawString(font, this.player.CollisionState, new Vector2(10, 600), Color.White);
                    spriteBatch.DrawString(font, "Time: " + gameTime.ElapsedGameTime.Minutes.ToString() + " min, " + gameTime.TotalGameTime.Seconds.ToString() + " sec", new Vector2(10, 600), Color.White);                    
                    spriteBatch.DrawString(font, "Enemies: " + enemyCount, new Vector2(10, 630), Color.White);
                    spriteBatch.DrawString(font, "Score: " + score.ToString("F2"), new Vector2(10, 690), Color.White);
                    spriteBatch.DrawString(font, "FPS: " + framerate, new Vector2(10, 660), Color.White);                    

                    //Draw Deathscreen
                    if (this.gameState == GameState.Died)
                        DrawDeathScreen();
                    else if (this.gameState == GameState.Gameover)
                        DrawGameOverScreen();

                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }



        #region gameplay methods

        private void BeginGame()
        {
            this.lives = STARTING_LIVES;
            this.score = 0;
        }

        private void BeginLevel()
        {
            Actor.Actors.Clear();
            this.score = 0;
            ENEMY_VELOCITY_X = 4f;
            ENEMY_VELOCITY_Y = 3f;

            Enemy.AddEnemies(1, enemyTexture, random.Next(0, 200), ENEMY_BASE_SPEED, ENEMY_SPEED_VARIATION, ENEMY_VELOCITY_X, ENEMY_VELOCITY_Y);
            this.player = new Player(playerTexture);
            this.player.Reset(PLAYER_INVICIBILITY_TIME);
        }

        #endregion

        #region utility methods

        private void Screenshot()
        {
            if (takeScreenshot && (screenshotThread == null || screenshotThread.Join(0)))
            {
                resolveTexture = new ResolveTexture2D(
                  graphics.GraphicsDevice,
                  graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
                  graphics.GraphicsDevice.PresentationParameters.BackBufferHeight,
                  1,
                  graphics.GraphicsDevice.PresentationParameters.BackBufferFormat);


                GraphicsDevice.ResolveBackBuffer(resolveTexture);
                takeScreenshot = false;

                screenshotThread = new Thread(() =>
                {
                    string filename;
                    filename = Window.Title + ".png";                    

                    resolveTexture.Save(filename,
                      ImageFileFormat.Png);
                });
                screenshotThread.Start();
            }
        }

        public static Vector2 GetRandomScreenPosition(int padding)
        {
            return new Vector2(random.Next(padding, SCREEN_WIDTH - padding), random.Next(padding, SCREEN_HEIGHT - padding));
        }

        public static float Range(float min, float max)
        {
            return (float)random.NextDouble() * (max - min) + min;
        }

        #endregion

        #region screens

        private void DrawTitleScreen()
        {
            spriteBatch.Draw(titleTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);
        }

        private void DrawDeathScreen()
        {
            spriteBatch.Draw(overlayTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), overlayColor);
            spriteBatch.DrawString(font, "YOU DIED!", new Vector2(590f, 150f), Color.White);
            spriteBatch.DrawString(font, "PRESS SPACEBAR TO RESTART!", new Vector2(500f, 180f), Color.White);
        }

        private void DrawGameOverScreen()
        {
            spriteBatch.Draw(overlayTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), overlayColor);
            spriteBatch.DrawString(font, "GAME OVER!", new Vector2(590f, 150f), Color.White);
            spriteBatch.DrawString(font, "PRESS ENTER TO RETURN TO TITLE SCREEN!", new Vector2(440f, 180f), Color.White);
        }

        private void DrawLevelChangeScreen()
        {
            spriteBatch.Draw(levelChangeTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);
            spriteBatch.DrawString(font, "GET READY!", new Vector2(590f, 150f), Color.White);
            spriteBatch.DrawString(font, "Lives: " + lives, new Vector2(590f, 180f), Color.White);            
        }

        #endregion

    }
}
