//#define DEBUG_HITBOX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace GravityMan
{
    public class Sprite
    {
        static Texture2D s_DebugHitBox;

        public static Random random = new Random();

        public Vector2 position, textureOrigin, force;

        public Animation animation;

        protected bool useOrigin = true;

        private Rectangle boundingBox;
        private float scale;

        public Sprite(Texture2D _texture)
        {
            animation = new Animation(_texture, 1);
            textureOrigin = new Vector2(animation.Width / 2, animation.Height / 2);
            position = new Vector2(0f, 0f);
            scale = 1.0f;
        }
        public static void Load(ContentManager content)
        {
            s_DebugHitBox = content.Load<Texture2D>("DebugBox");
        }
        public Vector2 Origin
        {
            get { return textureOrigin; }
        }
        public Rectangle SmallAABB
        {
            set { boundingBox = new Rectangle((int)value.X, (int)value.Y, value.Width, value.Height); }
            get { return boundingBox;}
        }
        public Rectangle AABB
        {
            set { new Rectangle((int)value.X, (int)value.Y, value.Width, value.Height); }
            get
            {
                return new Rectangle((int)position.X - (animation.Width / 2), (int)position.Y - (animation.Height / 2), animation.Width, animation.Height);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch, position, scale, useOrigin);
#if DEBUG_HITBOX
            Rectangle rect = AABB;
            Rectangle rectangle = SmallAABB;
            spriteBatch.Draw(s_DebugHitBox, rect, Color.White);
            spriteBatch.Draw(s_DebugHitBox, rectangle, Color.White);
#endif
        }

        public virtual void Update(GameTime gameTime)
        {
            animation.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}
