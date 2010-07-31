using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TROLLArena
{
    class Player : Actor
    {
        private float invincibilityTime;

        public bool IsInvincible
        {
            get { return invincibilityTime > 0f; }
        }

        public Player(Texture2D texture) : base(texture) { }

        public override void Update(GameTime gameTime)
        {
            if (this.IsInvincible)
            {
                this.invincibilityTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (!this.IsInvincible)
                {
                    this.Flicker = false;
                    this.CollisionState = "";
                }
            }

            base.Update(gameTime);
        }

        public void Reset(float invincibilityTime)
        {
            this.Position = new Vector2(Game1.SCREEN_WIDTH / 2, Game1.SCREEN_HEIGHT / 2);
            this.invincibilityTime = invincibilityTime;
            this.Flicker = true;            
        }
    }
}
