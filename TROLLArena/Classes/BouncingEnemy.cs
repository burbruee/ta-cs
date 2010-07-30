using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TROLLArena
{
    class BouncingEnemy : Actor
    {
        private float vX;
        private float vY;
        private Random random;
        private float spawnTime;
        private int totalSpawns;

        public int TotalSpawns
        {
            get { return totalSpawns; }
            set { totalSpawns = value; }
        }

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
            this.spawnTime = 0;
            this.totalSpawns = 0;
            this.random = new Random();
            this.position = new Vector2(random.Next(100,1000), random.Next(70,200));
            this.origin = new Vector2(this.texture.Width / 2, this.texture.Height / 2);
        }

        public override void Update(GameTime gameTime)
        {
            if (this.position.X < (this.Origin.X)) this.vX = -this.vX;
            if (this.position.Y < (this.Origin.Y)) this.vY = -this.vY;
            if (this.position.X > (1280 - this.Origin.X)) this.vX = -this.vX;
            if (this.position.Y > (720 - this.Origin.Y)) this.vY = -this.vY;

            this.position.X += this.vX;
            this.position.Y += this.vY;
            this.rotation = (float)gameTime.TotalGameTime.TotalSeconds * (float)1.5;

            this.spawnTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (spawnTime >= 5000)
            {
                spawnTime -= 5000;
                this.totalSpawns+=2;
                new BouncingEnemy(this.texture, random.Next(2,5), random.Next(2,5));
            }

            //Vector2 direction = Vector2.Zero;
            //direction.X = this.vX;
            //direction.Y = this.vY;
            //this.position += direction;
        }
    }
}
