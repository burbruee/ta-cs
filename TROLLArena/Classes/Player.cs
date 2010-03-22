// Test

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

        public bool isInvincible
        {
            get { return invincibilityTime > 0f; }
        }

        public Player(Texture2D texture) : base(texture) { }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Reset(float invincibilityTime)
        {

        }
    }
}
