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

        Sprite player;
        Sprite enemy;

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
            player = new Sprite(Vector2.Zero);
            enemy = new Sprite(Vector2.Zero);
            
            player.texture = Content.Load<Texture2D>(@"Textures\TROLLET_HD_small");
            enemy.texture = Content.Load<Texture2D>(@"Textures\cat");          

            player.scale = 1f;
            
            player.position = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight - player.texture.Height / 2);            
            player.origin = new Vector2(player.texture.Width / 2, player.texture.Height / 2);

            Random random = new Random();
            
            enemy.scale = 1f;
            //enemy.position = new Vector2(graphics.PreferredBackBufferWidth / 2, enemy.texture.Height);
            enemy.position = new Vector2(random.Next(enemy.texture.Width / 2, graphics.PreferredBackBufferWidth - enemy.texture.Width / 2), random.Next(enemy.texture.Height / 2, graphics.PreferredBackBufferHeight - enemy.texture.Height / 2));
            enemy.origin = new Vector2(enemy.texture.Width / 2, enemy.texture.Height / 2);
            enemy.vX = random.Next(-8, 8);
            enemy.vY = random.Next(-8, 8);

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

            enemy.rotation = (float)gameTime.TotalGameTime.TotalSeconds * (float)1.5;
            enemy.position.X += enemy.vX;
            enemy.position.Y += enemy.vY;

            if (enemy.position.X < enemy.texture.Width / 2) enemy.vX = -enemy.vX;
            if (enemy.position.X > graphics.PreferredBackBufferWidth - enemy.texture.Width / 2) enemy.vX = -enemy.vX;
            if (enemy.position.Y < enemy.texture.Height / 2) enemy.vY = -enemy.vY;
            if (enemy.position.Y > graphics.PreferredBackBufferHeight - enemy.texture.Height / 2) enemy.vY = -enemy.vY;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            if (mousePos.X < player.origin.X) mousePos.X = player.origin.X;
            if (mousePos.Y < player.origin.Y) mousePos.Y = player.origin.Y;
            if (mousePos.X > graphics.PreferredBackBufferWidth - player.origin.X) mousePos.X = graphics.PreferredBackBufferWidth - player.origin.X;
            if (mousePos.Y > graphics.PreferredBackBufferHeight - player.origin.Y) mousePos.Y = graphics.PreferredBackBufferHeight - player.origin.Y;

            player.Draw(spriteBatch, player.texture, mousePos, Color.White, player.rotation, player.origin, player.scale, SpriteEffects.None);

            enemy.Draw(spriteBatch, enemy.texture, enemy.position, new Color(255, 255, 255, 255), enemy.rotation, enemy.origin, enemy.scale, SpriteEffects.None);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
