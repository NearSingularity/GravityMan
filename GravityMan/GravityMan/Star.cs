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
    public class Star : Sprite
    {
        static Texture2D s_Star, s_Explosion, s_DeadStar;

        static List<Star> stars = new List<Star>();
        static List<Star> deadStars = new List<Star>();

        static float delay, spawnTimer;

        public enum States { ALIVE, HIT, DEAD };
        public States state;

        public static bool collision = false;
        protected bool isAlive;

        Animation spin, explosion, dead;

        public Star():base(s_Star)
        {
            spin = new Animation(s_Star, 8);
            explosion = new Animation(s_Explosion, 4);
            dead = new Animation(s_DeadStar, 8);

            int randomPos = random.Next(0, (int)Game.GetBot());
            position = new Vector2((int)Camera2D.GetCamera.Width + Camera2D.GetCamera.Position.X, randomPos);
            delay = random.Next(2, 3);

            animation = spin;

            isAlive = true;
        }
        public static void Restart()
        {
            stars.Clear();
            deadStars.Clear();
        }
        public static bool CollideBullet(Bullet bullet)
        {
            foreach (Star star in deadStars)
            {
                if (star.AABB.Intersects(bullet.AABB))
                    return true;
            }
            return false;
        }
        public static bool CollidePlayer(Player player)
        {
            player.SmallAABB = new Rectangle((int)player.position.X - (player.AABB.Width / 6), (int)player.position.Y - (player.AABB.Height / 6), player.AABB.Width / 4, player.AABB.Height / 2);
            foreach (Star star in deadStars)
            {
                star.SmallAABB = new Rectangle((int)star.position.X - (star.AABB.Width / 6), (int)star.position.Y - (star.AABB.Height / 6), star.AABB.Width / 4, star.AABB.Height / 4);
                if (!collision)
                {
                    if (star.SmallAABB.Intersects(player.SmallAABB))
                    {
                        collision = true;
                        return true;
                    }
                }
            }
            return false;
        }
        void HandleCollision()
        {
            if (Bullet.CollideAll(this))
            {
                if(isAlive)
                    Score.AddPoints(25);
                isAlive = false; 
            }
            if (position.X < -Game.GetSide())
                isAlive = false;
        }
        void HandleStates()
        {
            switch (state)
            {
                case States.ALIVE:
                    if (isAlive == false)
                        SetState(States.HIT);
                    break;
                case States.HIT:
                    if (explosion.Done())
                        SetState(States.DEAD);
                    break;
            }
        }
        void SetState(States myState)
        {
            state = myState;

            switch (state)
            {
                case States.ALIVE:
                    animation = spin;
                    break;
                case States.HIT:
                    animation = explosion;
                    break;
                case States.DEAD:
                    animation = dead;
                    break;
            }
        }
        static void Spawner(GameTime gameTime)
        {
            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (spawnTimer >= delay)
            {
                Star star = new Star();
                stars.Add(star);
                spawnTimer -= delay;
            }
        }
        public static void LoadContent(ContentManager content)
        {
            s_Star = content.Load<Texture2D>("SpriteSheet_Star");
            s_Explosion = content.Load<Texture2D>("SpriteSheet_Explode");
            s_DeadStar = content.Load<Texture2D>("SpriteSheet_BlackHole");
        }
        public override void Update(GameTime gameTime)
        {
            HandleCollision();
            HandleStates();

            base.Update(gameTime);
        }
        public static void UpdateAll(GameTime gameTime)
        {
            Spawner(gameTime);
            foreach (Star star in stars)
            {
                if (star.isAlive)
                    star.Update(gameTime);
                else
                    deadStars.Add(star);
            }
            foreach (Star deadStar in deadStars)
            {
                deadStar.Update(gameTime);
                stars.Remove(deadStar);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (Star star in stars)
                star.Draw(spriteBatch);
            foreach (Star deadStar in deadStars)
                deadStar.Draw(spriteBatch);
        }
    }
}
