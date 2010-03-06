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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        const float PLAYER_SPEED = 5.0f;
        const float ENEMY_SPEED = 2.0f;

        private Player player1;
        private Enemy enemy1;
        private BouncingEnemy bounce;
        private Actor bg;

        private string col;

        Random random = new Random();
        protected Vector2 mousePos;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = true;
            
            graphics.ApplyChanges();
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
            bg = new Actor(Content.Load<Texture2D>(@"Textures\background"));
            bg.Scale = .85f;

            player1 = new Player(Content.Load<Texture2D>(@"Textures\TROLLET_HD_small"));
            player1.Position = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight - player1.Texture.Height / 2);
            player1.Origin = new Vector2(player1.Texture.Width / 2, player1.Texture.Height / 2);

            bounce = new BouncingEnemy(Content.Load<Texture2D>(@"Textures\cat"), random.Next(-8, 8), random.Next(-8, 8));
            enemy1 = new Enemy(Content.Load<Texture2D>(@"Textures\cat"), player1, ENEMY_SPEED);
            //new DrunkenEnemy(Content.Load<Texture2D>(@"Textures\japan_HD"), player1, ENEMY_SPEED);

            //enemy1.Scale = 2f;      
            //enemy1.Origin = new Vector2(enemy1.Texture.Width / 2, enemy1.Texture.Height / 2);
            
            //bounce.VX = random.Next(-8, 8);
            //bounce.VY = random.Next(-8, 8);
            
            // Scale bg for 1024x600 res
            //bg.scale = .85f;
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

            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            mousePos = new Vector2(mouseState.X, mouseState.Y);

            Vector2 direction = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.Up))
                direction.Y--;
            if (keyboardState.IsKeyDown(Keys.Down))
                direction.Y++;
            if (keyboardState.IsKeyDown(Keys.Left))
                direction.X--;
            if (keyboardState.IsKeyDown(Keys.Right))
                direction.X++;

            if (direction.Length() > 0.0f)
                direction.Normalize();

            player1.Position += direction * PLAYER_SPEED;

            foreach (Actor actor in Actor.Actors)
                actor.Update(gameTime);
 
            //enemy.rotation = (float)gameTime.TotalGameTime.TotalSeconds * (float)1.5;
            //enemy.position.X += enemy.vX;
            //enemy.position.Y += enemy.vY;

            int x1 = (int)player1.Position.X + player1.Texture.Width;
            int y1 = (int)player1.Position.Y + player1.Texture.Height;
            int w1 = player1.Texture.Width;
            int h1 = player1.Texture.Height;

            int x2 = (int)enemy1.Position.X + enemy1.Texture.Width;
            int y2 = (int)enemy1.Position.Y + enemy1.Texture.Height;
            int w2 = enemy1.Texture.Width;
            int h2 = enemy1.Texture.Height;

            int x3 = (int)bounce.Position.X + bounce.Texture.Width;
            int y3 = (int)bounce.Position.Y + bounce.Texture.Height;
            int w3 = bounce.Texture.Width;
            int h3 = bounce.Texture.Height;
                        
            if (CollisionCheck((short)x1, (short)x2, (short)w1, (short)w2, (short)y1, (short)y2, (short)h1, (short)h2) || CollisionCheck((short)x1, (short)x3, (short)w1, (short)w3, (short)y1, (short)y3, (short)h1, (short)h3))
            {
                col = "Collided";
            }
            else
            {
                col = "No Collision";
            }

            if (mousePos.X < player1.Origin.X) mousePos.X = player1.Origin.X;
            if (mousePos.Y < player1.Origin.Y) mousePos.Y = player1.Origin.Y;
            if (mousePos.X > graphics.PreferredBackBufferWidth - player1.Origin.X) mousePos.X = graphics.PreferredBackBufferWidth - player1.Origin.X;
            if (mousePos.Y > graphics.PreferredBackBufferHeight - player1.Origin.Y) mousePos.Y = graphics.PreferredBackBufferHeight - player1.Origin.Y;

            player1.Position = mousePos;

            base.Update(gameTime);
        }

        bool CollisionCheck(Int16 x1, Int16 x2, Int16 w1, Int16 w2, Int16 y1, Int16 y2, Int16 h1, Int16 h2)
        {
            return ( ( x2 >= x1 - ( w1 + w2 ) / 2 ) && ( x2 <= x1 + ( w1 + w2 ) / 2 ) && ( y2 >= y1 - ( h1 + h2 ) / 2 ) && ( y2 <= y1 + ( h1 + h2 ) / 2 )); 
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            
            //bg.Draw(spriteBatch, bg.texture, bg.position, new Color(255, 255, 255, 255), bg.rotation, bg.origin, bg.scale, SpriteEffects.None);
            //player.Draw(spriteBatch, player.texture, mousePos, Color.White, player.rotation, player.origin, player.scale, SpriteEffects.None);
            //enemy.Draw(spriteBatch, enemy.texture, enemy.position, new Color(255, 255, 255, 255), enemy.rotation, enemy.origin, enemy.scale, SpriteEffects.None);            
            foreach (Actor actor in Actor.Actors)
                actor.Draw(spriteBatch);

            spriteBatch.DrawString(font, col, new Vector2(100, 100), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
