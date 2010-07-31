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

        public Vector2 position;
        protected Texture2D texture;
        protected Color tint;
        protected float rotation;
        protected Vector2 origin;
        protected SpriteEffects spriteEffects;

        private string collisionState;

        //Flicker
        const float FLICKER_FREQUENCY = 4f;
        private bool visible;
        private bool flicker;        
        double nextFlickerUpdate;
         
        //Scaling
        private float scale;
        private float scaleTime;
        private float scalePerSecond;

        #region properties

        public string CollisionState
        {
            get { return collisionState; }
            set { collisionState = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public float Scale
        {
            get { return this.scale; }
            set { this.scale = value; }
        }

        public bool IsScaling
        {
            get { return this.scaleTime > 0f; } 
        }

        public Vector2 Origin
        {
            get 
            { 
                return new Vector2(this.texture.Width / 2, this.texture.Height / 2); 
            }
        }

        public int Radius
        {
            get { return this.texture.Width / 2;  }
        }

        public bool Flicker
        {
            set 
            { 
                this.flicker = value; 
                this.visible = !value;
                
            }
        }

        public SpriteEffects SpriteEffects
        {
            get { return spriteEffects; }
            set { spriteEffects = value; }
        }

        #endregion

        static Actor()
        {
            Actors = new List<Actor>();
        }

        public Actor(Texture2D texture)
        {
            Actors.Add(this);
            this.texture = texture;
            this.visible = true;
            this.origin = new Vector2(texture.Width/2, texture.Height/2);
            this.tint = Color.White;
            this.scale = 1f;
            this.collisionState = "";
            //this.StartScale(1, 2);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (this.flicker)
            {
                if (gameTime.TotalGameTime.TotalSeconds > nextFlickerUpdate)
                {
                    this.visible = !this.visible;
                    this.nextFlickerUpdate = gameTime.TotalGameTime.TotalSeconds + 1 / FLICKER_FREQUENCY;
                }
            }

            if (this.IsScaling)
            {
                this.scale += this.scalePerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
                this.scaleTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public static void DrawActors(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Actors.Count; i++)
            {
                Actors[i].Draw(spriteBatch);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.visible)
                spriteBatch.Draw(this.texture, this.position, null, this.tint, this.rotation, this.origin, this.scale, this.spriteEffects, 0f);
        }

        public static bool CheckCollision(Actor actorA, Actor actorB)
        {
            float distance = Vector2.Distance(actorA.Position, actorB.Position);

            return distance < actorA.Radius + actorB.Radius;
        }

        public bool CheckCollisionWithAny()
        {
            return false;
        }

        public void StartScale(float targetScale, float scaleTime)
        {
            if (this.IsScaling)
                return;

            this.scaleTime = scaleTime;
            this.scalePerSecond = (targetScale - this.scale) / scaleTime;
        }
    }
}
