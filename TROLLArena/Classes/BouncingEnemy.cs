using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TROLLArena
{
    class BouncingEnemy : Sprite
    {
        private float vX;
        private float vY;
        private Random random;        

        public float VX
        {
            get { return vX; }
            set { vX = value; }
        }

        public float VY
        {
            get { return vY; }
            set { vY = value; }
        }

        public BouncingEnemy(Texture2D texture, float vX, float vY)
            : base(texture)
        {
            this.vX = vX;
            this.vY = vY;            
            this.random = new Random();
            this.position = new Vector2(random.Next(100,1000), random.Next(70,200));
            this.origin = new Vector2(this.texture.Width / 2, this.texture.Height / 2);            
        }

        public override void Update(GameTime gameTime)
        {
            if (this.position.X < (this.Origin.X)) this.vX = -this.vX;
            if (this.position.Y < (this.Origin.Y)) this.vY = -this.vY;
            if (this.position.X > (Game1.SCREEN_WIDTH - this.Origin.X)) this.vX = -this.vX;
            if (this.position.Y > (Game1.SCREEN_HEIGHT - this.Origin.Y)) this.vY = -this.vY;

            this.position.X += this.vX;
            this.position.Y += this.vY;
            this.rotation = (float)gameTime.TotalGameTime.TotalSeconds * (float)-1.5;

            //Vector2 direction = Vector2.Zero;
            //direction.X = this.vX;
            //direction.Y = this.vY;
            //this.position += direction;
        }

        public static void AddEnemies(int numEnemies, Texture2D texture, float baseSpeed, float speedVariation)
        {
            for (int i = 0; i < numEnemies; i++)
            {
                BouncingEnemy enemy = new BouncingEnemy(texture, baseSpeed, speedVariation);
                enemy.Position = Game1.GetRandomScreenPosition(enemy.Radius);

            }
        }
    }
}
