using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TROLLArena
{
    class DrunkenEnemy : Enemy
    {
        private const float FREQUENCY = 4f;
        private const float AMPLITUDE = 3.5f;

        public DrunkenEnemy(Texture2D texture, Actor target, float speed)
            : base(texture, target, speed)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);  

            Vector2 offset = Vector2.Zero;
            offset.X = (float)Math.Sin(gameTime.TotalGameTime.Seconds * FREQUENCY) * AMPLITUDE;
            offset.Y = (float)Math.Cos(gameTime.TotalGameTime.Seconds * FREQUENCY) * AMPLITUDE;
            this.position += offset;
        }

    }
}
