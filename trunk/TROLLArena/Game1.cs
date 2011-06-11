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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TROLLArena
{
    enum GameState
    {
        Setup,
        Title,
        LevelChange,
        Playing,
        Died,
        Gameover,
        Highscores
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

        //Fading
        int mAlphaValue = 1;
        int mFadeIncrement = 25;
        double mFadeDelay = .5;

        //Timer
        float getReadyTimer;

        //Name Input
        Xin xinComponent;
        string text;
        bool canType = true;

        //Parallax
        Parallax background;

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
        Texture2D playerTexture;
        Texture2D enemyTexture;
        Texture2D overlayTexture;
        Texture2D titleTexture;
        Texture2D levelChangeTexture;
        Texture2D starsTexture;

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
            xinComponent = new Xin(this);
            Components.Add(xinComponent);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = false;

            //this.Components.Add(new GamerServicesComponent(this));            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            text = "";
            random = new Random();
            this.gameState = GameState.Setup;
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
            playerTexture = Content.Load<Texture2D>(@"Textures\TROLLET_HD_small");
            enemyTexture = Content.Load<Texture2D>(@"Textures\cat");
            overlayTexture = Content.Load<Texture2D>(@"Textures\fill");
            levelChangeTexture = Content.Load<Texture2D>(@"Textures\GetReady");
            titleTexture = Content.Load<Texture2D>(@"Textures\title");

            background = new Parallax(
               Content,
               @"Textures\PrimaryBackground",
               @"Textures\ParallaxStars");

            if (File.Exists("score.dat"))
            {                
                StreamReader f = new StreamReader("score.dat");
                decHighscore = f.ReadLine();                
                decHighscore = Encryption.EncryptOrDecrypt(decHighscore, Encryption.key);
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
            string encHighscore = Encryption.EncryptOrDecrypt(this.highscore.ToString(), Encryption.key);
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
            
            //Decrement the delay by the number of seconds that have elapsed since
            //the last time that the Update method was called
            mFadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            //If the Fade delays has dropped below zero, then it is time to 
            //fade in/fade out the image a little bit more.
            if (mFadeDelay <= 0)
            {
                //Reset the Fade delay
                mFadeDelay = .035;

                //Increment/Decrement the fade value for the image
                mAlphaValue += mFadeIncrement;

                //If the AlphaValue is equal or above the max Alpha value or
                //has dropped below or equal to the min Alpha value, then 
                //reverse the fade
                if (mAlphaValue >= 255 || mAlphaValue <= 0)
                {
                    mFadeIncrement *= -1;
                }
            }

            switch (this.gameState)
            {
                case GameState.Setup:
                    background.BackgroundOffset += 1;
                    background.ParallaxOffset += 2;

                    if (canType)
                    {                                               
                        CheckKeys();
                    }
                    else
                        this.gameState = GameState.Title;

                    break;

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
                    //Thread.Sleep(3000);    
                    
                    getReadyTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (getReadyTimer >= 2.0f)
                    {
                        BeginLevel();
                        this.gameState = GameState.Playing;
                    }

                    break;

                case GameState.Playing:
                    float elapsed = (float)gameTime.ElapsedRealTime.TotalMilliseconds;                    
                    deltaFPSTime += elapsed;
                    
                    //Parallax
                    background.BackgroundOffset += 1;
                    background.ParallaxOffset += 2;

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
                                        ip = Dns.GetHostEntry("highscore.ettfyratre.se");
                                        Highscores.sendScore("1", text, "Enemies: " + enemyCount, highscore);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                    //takeScreenshot = true;
                                    //Screenshot();
                                }

                                this.lives--;
                                this.gameState = GameState.Gameover;                                    
                            }
                        }
                    }

                    break;

                case GameState.Died:
                    Thread.Sleep(3000);
                    this.gameState = GameState.Highscores;                                            

                    break;

                case GameState.Gameover:
                    Thread.Sleep(3000);                    
                    this.gameState = GameState.Highscores;
                    break;

                case GameState.Highscores:
                    if (score > highscore)
                    {
                        takeScreenshot = true;
                        Screenshot();
                    }
                    Thread.Sleep(5000);
                    this.gameState = GameState.Title;
                    break;
            }

            base.Update(gameTime);
        }

        private void CheckKeys()
        {
            string newChar = "";

            if (Xin.CheckKeyPress(Keys.A))
                newChar += "a";
            if (Xin.CheckKeyPress(Keys.B))
                newChar += "b";
            if (Xin.CheckKeyPress(Keys.C))
                newChar += "c";
            if (Xin.CheckKeyPress(Keys.D))
                newChar += "d";
            if (Xin.CheckKeyPress(Keys.E))
                newChar += "e";
            if (Xin.CheckKeyPress(Keys.F))
                newChar += "f";
            if (Xin.CheckKeyPress(Keys.G))
                newChar += "g";
            if (Xin.CheckKeyPress(Keys.H))
                newChar += "h";
            if (Xin.CheckKeyPress(Keys.I))
                newChar += "i";
            if (Xin.CheckKeyPress(Keys.J))
                newChar += "j";
            if (Xin.CheckKeyPress(Keys.K))
                newChar += "k";
            if (Xin.CheckKeyPress(Keys.L))
                newChar += "l";
            if (Xin.CheckKeyPress(Keys.M))
                newChar += "m";
            if (Xin.CheckKeyPress(Keys.N))
                newChar += "n";
            if (Xin.CheckKeyPress(Keys.O))
                newChar += "o";
            if (Xin.CheckKeyPress(Keys.P))
                newChar += "p";
            if (Xin.CheckKeyPress(Keys.Q))
                newChar += "q";
            if (Xin.CheckKeyPress(Keys.R))
                newChar += "r";
            if (Xin.CheckKeyPress(Keys.S))
                newChar += "s";
            if (Xin.CheckKeyPress(Keys.T))
                newChar += "t";
            if (Xin.CheckKeyPress(Keys.U))
                newChar += "u";
            if (Xin.CheckKeyPress(Keys.V))
                newChar += "v";
            if (Xin.CheckKeyPress(Keys.W))
                newChar += "w";
            if (Xin.CheckKeyPress(Keys.X))
                newChar += "x";
            if (Xin.CheckKeyPress(Keys.Y))
                newChar += "y";
            if (Xin.CheckKeyPress(Keys.Z))
                newChar += "z";
            if (Xin.CheckKeyPress(Keys.Back))
            {
                if (text.Length != 0)
                    text = text.Remove(text.Length - 1);
            }
	    if (Xin.IsKeyDown(Keys.RightShift) ||
	        Xin.IsKeyDown(Keys.LeftShift))            
	    {
	        newChar = newChar.ToUpper();
	    }
            if (Xin.CheckKeyPress(Keys.Enter))
                canType = false;

            text += newChar;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            switch (this.gameState)
            {
                case GameState.Setup:
                    DrawSetupScreen();
                    break;

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

                    //Draw Parallax background
                    background.Draw(spriteBatch);

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
                    {
                        DrawDeathScreen();
                        
                    }
                    else if (this.gameState == GameState.Gameover)
                    {
                        DrawGameOverScreen();
                        
                    }

                    break;
                case GameState.Highscores:
                    DrawHighscores();
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

        private void DrawSetupScreen()
        {
            background.Draw(spriteBatch);            
            spriteBatch.DrawString(title, "HEY NEW PLAYER, ENTER YOUR NAME HERE!", new Vector2(SCREEN_WIDTH / 2 - 300, SCREEN_HEIGHT / 2 - 100), new Color(255, 255, 255, (byte)MathHelper.Clamp(mAlphaValue, 0, 255)));
            spriteBatch.DrawString(title, "Name : " + text, new Vector2(SCREEN_WIDTH / 2 - 300, SCREEN_HEIGHT / 2 - 50), Color.Yellow);
        }

        private void DrawHighscores()
        {
            spriteBatch.Draw(levelChangeTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);
            Vector2 hsPos = new Vector2(250, 150);            
            spriteBatch.DrawString(title, "TROLLArena Highscores TOP10", new Vector2(450, 50), Color.Yellow);

            try
            {
                List<string[]> ls = new List<string[]>();
                ls = Highscores.HttpGet("http://highscore.ettfyratre.se/api.php", "?ModeID=1&Format=TOP10");

                Color colNick;

                for (int i = 0; i < 10; i++)
                {

                    if (ls[i][1] == text)
                    {
                        colNick = new Color(255, 255, 0);
                    }
                    else
                    {
                        colNick = new Color(255, 255, 255);
                    }

                    spriteBatch.DrawString(title, ls[i][0], hsPos, colNick);
                    spriteBatch.DrawString(title, ls[i][1], new Vector2(500, hsPos.Y), colNick);
                    spriteBatch.DrawString(title, ls[i][2], new Vector2(1000, hsPos.Y), colNick);
                    hsPos.Y += 50;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);                
                spriteBatch.DrawString(title, "Could not connect to server! Highscore not posted online.", hsPos, Color.White);
            }

            
        }

        private void DrawTitleScreen()
        {
            spriteBatch.Draw(titleTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);
            spriteBatch.DrawString(title, "Coding: Burbruee - Original Idea: Hideous (Mr. 142) - Music: Jallabert (mr_kruuuk)", new Vector2(50, 10), Color.White);
            spriteBatch.DrawString(title, "PRESS SPACEBAR TO BEGIN", new Vector2(SCREEN_WIDTH / 2 - 200, SCREEN_HEIGHT / 2), new Color(255, 255, 255, (byte)MathHelper.Clamp(mAlphaValue, 0, 255)));
            spriteBatch.DrawString(title, "OR ESC TO EXIT", new Vector2(SCREEN_WIDTH / 2 - 120, SCREEN_HEIGHT / 2 + 60), new Color(255, 255, 255, (byte)MathHelper.Clamp(mAlphaValue, 0, 255)));                    
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
            spriteBatch.DrawString(main, "GET READY!", new Vector2(520f, 180f), new Color(255, 255, 255, (byte)MathHelper.Clamp(mAlphaValue, 0, 255)));                  
        }

        #endregion

    }
}
