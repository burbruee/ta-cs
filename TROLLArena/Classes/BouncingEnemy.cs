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
            this.position = new Vector2(300, 200);
            this.origin = new Vector2(this.texture.Width / 2, this.texture.Height / 2);
        }

        public override void Update(GameTime gameTime)
        {
            if (this.position.X < (this.texture.Width / 2)) this.vX = -this.vX;
            if (this.position.Y < (this.texture.Height / 2)) this.vY = -this.vY;
            if (this.position.X > (1024 - this.texture.Width / 2)) this.vX = -this.vX;
            if (this.position.Y > (600 - this.texture.Height / 2)) this.vY = -this.vY;

            this.position.X += this.vX;
            this.position.Y += this.vY;
            this.rotation = (float)gameTime.TotalGameTime.TotalSeconds * (float)1.5;

            //Vector2 direction = Vector2.Zero;
            //direction.X = this.vX;
            //direction.Y = this.vY;
            //this.position += direction;
        }
    }
}
