using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TROLLArena
{
    class Sprite
    {        
        public Vector2 position;
        public float rotation;
        public float scale;
        public Texture2D texture;
        public Vector2 origin;

        public Sprite(Vector2 position)
        {
            this.position = position;
            this.scale = 1f;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects)
        {            
            spriteBatch.Draw(texture, position, null, color, rotation, origin, scale, spriteEffects, 0f);
        }
    }
}
