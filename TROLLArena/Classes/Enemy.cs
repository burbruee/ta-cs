using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TROLLArena
{
    class Enemy : Actor
    {
        const float ENEMY_SCALE_TIME = 1.5f;

        private float baseSpeed;
        private float speedVariation;        

        private Vector2 pointA, pointB;
        private float amount;
        private float moveTime;

        private float angle = 0f;

        private float vX;
        private float vY;

        private int enemyType;

        public bool IsHarmful
        {
            get { return !this.IsScaling; }
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

        public int EnemyType
        {
            get { return enemyType; }
            set { enemyType = value; }
        }

        public Enemy(Texture2D texture, int enemyType, float baseSpeed, float speedVariation, float vX, float vY) : base(texture)
        {
            this.baseSpeed = baseSpeed;
            this.speedVariation = speedVariation;
            this.Scale = 0f;
            this.EnemyType = enemyType;            
            this.vX = vX;
            this.vY = vY;            
            this.StartScale((float)new Random().NextDouble() * 1 + .50f, ENEMY_SCALE_TIME);
        }

        public override void Update(GameTime gameTime)
        {
            if (this.enemyType < 100)
            {
                if (moveTime <= 0)
                    this.SetRandomMove();

                if (this.IsHarmful)
                {
                    amount += (float)gameTime.ElapsedGameTime.TotalSeconds / moveTime;
                    this.Position = Vector2.SmoothStep(this.pointA, this.pointB, this.amount);
                }

                if (this.amount >= 1f)
                    this.SetRandomMove();
            }
            else if (this.enemyType >= 100)
            {
                if (moveTime <= 0)
                    this.SetBouncingMove();

                if (this.IsHarmful)
                {
                    amount += (float)gameTime.ElapsedGameTime.TotalSeconds / moveTime;

                    this.position.X += this.vX;
                    this.position.Y += this.vY;
                }

                if (this.amount >= 1f)
                    this.SetBouncingMove();

                if (this.position.X < (this.Origin.X+10)) this.vX = -this.vX;
                if (this.position.Y < (this.Origin.Y)) this.vY = -this.vY;
                if (this.position.X > (Game1.SCREEN_WIDTH - this.Origin.X-10)) this.vX = -this.vX;
                if (this.position.Y > (Game1.SCREEN_HEIGHT - this.Origin.Y)) this.vY = -this.vY;
            }

            angle += 0.010f;

            this.rotation = angle;

            base.Update(gameTime);
     
        }

        private void SetRandomMove()
        {
            this.pointA = this.Position;
            this.pointB = Game1.GetRandomScreenPosition(this.Radius);

            this.moveTime = baseSpeed + Game1.Range(-speedVariation, speedVariation);
            this.amount = 0f;            
        }

        private void SetBouncingMove()
        {
            this.position.X += this.vX;
            this.position.Y += this.vY;

            this.moveTime = baseSpeed + Game1.Range(-speedVariation, speedVariation);
            this.amount = 0f;       
        }

        public static void AddEnemies(int numEnemies, Texture2D texture, int enemyType, float baseSpeed, float speedVariation, float vX, float vY)
        {            
            for (int i = 0; i < numEnemies; i++)
            {
                Enemy enemy = new Enemy(texture, enemyType, baseSpeed, speedVariation, vX, vY);
                enemy.Position = Game1.GetRandomScreenPosition(enemy.Radius);                                
                
            }
        }
    }
}
