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
    public class Bullet : Sprite
    {
        static Texture2D s_Bullet;
        static List<Bullet> bullets = new List<Bullet>();
        static List<Bullet> deadBullets = new List<Bullet>();

        public bool isActive;

        private Vector2 velocity;

        public Bullet(Vector2 _position):base(s_Bullet)
        { 
            animation = new Animation(s_Bullet, 4);

            position = _position;
            velocity = new Vector2(15f, 0f);

            isActive = true;
            bullets.Add(this);   
        }
        public static void Restart()
        {
            bullets.Clear();
            deadBullets.Clear();
        }
        public static bool CollideAll(Star star)
        {
            foreach (Bullet bullet in bullets)
            {
                if (bullet.AABB.Intersects(star.AABB))
                    return true;
            }
            return false;
        } 
        public static void LoadContent(ContentManager content)
        {
            s_Bullet = content.Load<Texture2D>("SpriteSheet_Bullet");
        }

        public static void UpdateAll(GameTime gameTime)
        {
            foreach (Bullet bullet in bullets)
            {
                if(bullet.isActive)
                    bullet.Update(gameTime);
                else
                    deadBullets.Add(bullet);
            }
            foreach (Bullet deadBullet in deadBullets)
            {
                bullets.Remove(deadBullet);
            }
        }

        public override void Update(GameTime gameTime)
        {
            position += velocity;

            if (position.X > Camera2D.GetCamera.Width + Camera2D.GetCamera.Position.X || position.X < 0
                || position.Y > Camera2D.GetCamera.Height || position.Y < 0)
                    isActive = false;
            if (Star.CollideBullet(this))
                isActive = false;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (Bullet bullet in bullets)
            {
                if(bullet.isActive)
                    bullet.Draw(spriteBatch);
            }
        }
    }
}
