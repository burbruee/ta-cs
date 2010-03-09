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
        const float ENEMY_SCALE_TIME = .5f;

        private float baseSpeed;
        private float speedVariation;

        private Vector2 pointA, pointB;
        private float amount;
        private float moveTime;

        public Enemy(Texture2D texture, float baseSpeed, float speedVariation) : base(texture)
        {
            this.baseSpeed = baseSpeed;
            this.speedVariation = speedVariation;
        }

        public override void Update(GameTime gameTime)
        {

        }

        private void SetRandomMove()
        {

        }

        public static void AddEnemies(int numEnemies, Texture2D texture, float baseSpeed, float speedVariation)
        {

        }
    }
}
