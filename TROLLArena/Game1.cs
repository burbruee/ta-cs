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
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            
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

            enemy.scale = 1f;
            enemy.position = new Vector2(graphics.PreferredBackBufferWidth / 2, enemy.texture.Height);
            enemy.origin = new Vector2(enemy.texture.Width / 2, enemy.texture.Height / 2);
            

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

            enemy.rotation = (float)gameTime.TotalGameTime.TotalSeconds * (float)157.5;

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
