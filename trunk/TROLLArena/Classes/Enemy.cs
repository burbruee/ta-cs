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
        private float speed;
        private Actor target;

        public Enemy(Texture2D texture, Actor target, float speed) : base(texture)
        {
            this.target = target;
            this.speed = speed;
        }

        public override void Update(GameTime gameTime)
        {
            if (target == null)
                return;

            Vector2 direction = Vector2.Normalize(target.Position - this.position);
            this.position += direction * speed;
        }
    }
}
