using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TROLLArena
{
    class Actor
    {
        public static List<Actor> Actors;

        protected Texture2D texture;
        public Vector2 position;
        protected float rotation;
        protected float scale;
        protected Vector2 origin;
        protected SpriteEffects spriteEffects;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public SpriteEffects SpriteEffects
        {
            get { return spriteEffects; }
            set { spriteEffects = value; }
        }

        static Actor()
        {
            Actors = new List<Actor>();
        }

        public Actor(Texture2D texture)
        {
            this.texture = texture;
            this.scale = 1f;
            Actors.Add(this);
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.texture, this.position, null, Color.White, this.rotation, this.origin, this.scale, this.spriteEffects, 0f);
        }
    }
}
