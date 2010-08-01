using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.IO;
using System.Diagnostics;
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
        SpriteFont font;
        SpriteFont main;
        SpriteFont title;

        //Score
        int score = 0;
        int highscore = 0;
        string decHighscore = "";
        string framerate = "";        

        //Screens
        Color overlayColor = new Color(225, 0, 0, 143);

        //Lives
        private int lives;
        const int STARTING_LIVES = 3;

        //Screenshot
        private ResolveTexture2D resolveTexture;
        private bool takeScreenshot;                
        private Thread screenshotThread;

        //Textures
        Texture2D backgroundTexture;
        Texture2D playerTexture;
        Texture2D enemyTexture;
        Texture2D overlayTexture;
        Texture2D titleTexture;
        Texture2D levelChangeTexture;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;        
        static Random random;

        GameState gameState;
        float deltaFPSTime = 0;

        double timer = 10;
        bool songStart = false;
        bool newHighscore = false;

        protected Vector2 mousePos;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            this.IsMouseVisible = false;

            this.Components.Add(new GamerServicesComponent(this));            
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
            SoundHelper.Initialize();

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
            main = Content.Load<SpriteFont>(@"Textures\LeviBrush");
            title = Content.Load<SpriteFont>(@"Textures\TitleFont");
            backgroundTexture = Content.Load<Texture2D>(@"Textures\background");
            playerTexture = Content.Load<Texture2D>(@"Textures\TROLLET_HD_small");
            enemyTexture = Content.Load<Texture2D>(@"Textures\japan");
            overlayTexture = Content.Load<Texture2D>(@"Textures\fill");
            levelChangeTexture = Content.Load<Texture2D>(@"Textures\GetReady");
            titleTexture = Content.Load<Texture2D>(@"Textures\title");

            if (File.Exists("score.dat"))
            {
                StreamReader f = new StreamReader("score.dat");
                decHighscore = f.ReadLine();                
                decHighscore = Encryption.EncryptOrDecrypt(decHighscore, "9pu3esp4t-uSethe=adRecrekucrexu7raj-pranEwerethaxaZ&puf2edrah6wa");
                this.highscore = Int32.Parse(decHighscore);
                f.Close();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here    
            StreamWriter file = new StreamWriter("score.dat");
            string encHighscore = Encryption.EncryptOrDecrypt(this.highscore.ToString(), "9pu3esp4t-uSethe=adRecrekucrexu7raj-pranEwerethaxaZ&puf2edrah6wa");
            file.WriteLine(encHighscore);
            file.Close();
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

            SoundHelper.Update();
            if (!songStart)
            {
                SoundHelper.PlaySound(0);
                songStart = true;
            } 
            // Allows the game to exit
            if (gamePadState.Buttons.Back == ButtonState.Pressed)
                this.Exit();

            switch (this.gameState)
            {
                case GameState.Title:                    
                    //songStart = false;
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {                        
                        BeginGame();
                        this.gameState = GameState.LevelChange;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        Exit();                    

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

                    for (int i = Sprite.Sprites.Count - 1; i >= 0; i--)
                    {
                        Sprite actor = Sprite.Sprites[i];
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
                        enemyCount = (int)Sprite.Sprites.Count() - 1;
                        

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
                            if (!player.IsInvincible && enemy.IsHarmful && Sprite.CheckCollision(enemy, this.player))
                            {
                                SoundHelper.PlaySound(2);
                                if (score > highscore)
                                {
                                    newHighscore = true;
                                    highscore = score;                                    
                                    IPHostEntry ip = null;
                                    try
                                    {
                                        ip = Dns.GetHostEntry("highscore.burbruee.se");
                                        Highscores.sendScore("1", "Burbruee", "Enemies: " + enemyCount, highscore);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                    takeScreenshot = true;
                                    Screenshot();
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
                    break;                    

                case GameState.LevelChange:
                    DrawLevelChangeScreen();
                    break;

                case GameState.Playing:
                case GameState.Died:
                case GameState.Gameover:
                    if (!songStart)
                    {                        
                        songStart = true;
                    }

                    //Draw Background
                    spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);

                    //Draw Actors                    
                    Sprite.DrawActors(spriteBatch);

                    

                    //Draw Info                    
                    //spriteBatch.DrawString(title, "Time: " + gameTime.ElapsedGameTime.Minutes.ToString() + " min, " + gameTime.TotalGameTime.Seconds.ToString() + " sec", new Vector2(10, 560), Color.White);                                        

                    
                    spriteBatch.DrawString(title, "Enemies: " + enemyCount, new Vector2(10, 580), Color.White);
                    spriteBatch.DrawString(title, "Score: " + score, new Vector2(10, 620), Color.White);
                    spriteBatch.DrawString(title, "Highscore: " + highscore.ToString(), new Vector2(10, 660), Color.White);
                    //spriteBatch.DrawString(title, "FPS: " + framerate, new Vector2(10, 680), Color.White);                    

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
            Sprite.Sprites.Clear();
            this.score = 0;
            this.newHighscore = false;
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
            return new Vector2(random.Next(padding, SCREEN_WIDTH - 40 - padding), random.Next(padding, SCREEN_HEIGHT - 40 - padding));
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
            spriteBatch.DrawString(title, "Coding: Burbruee - Original Idea: Hideous (Mr. 142) - Music: Jallabert (mr_kruuuk)", new Vector2(50, 10), Color.White);                    
            spriteBatch.DrawString(title, "PRESS SPACEBAR TO BEGIN", new Vector2(SCREEN_WIDTH / 2 - 200, SCREEN_HEIGHT / 2 ), Color.White);
            spriteBatch.DrawString(title, "OR ESC TO EXIT", new Vector2(SCREEN_WIDTH / 2 - 120, SCREEN_HEIGHT / 2 + 60), Color.White);                    
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
            spriteBatch.DrawString(main, "GAME OVER!", new Vector2(520f, 160f), Color.White);
            if (newHighscore)
            {
                spriteBatch.DrawString(main, "NEW HIGHSCORE!", new Vector2(470f, 220f), Color.White);
            }
        }

        private void DrawLevelChangeScreen()
        {
            spriteBatch.Draw(levelChangeTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);
            spriteBatch.DrawString(main, "GET READY!", new Vector2(520f, 180f), Color.White);                  
        }

        #endregion

    }
}
