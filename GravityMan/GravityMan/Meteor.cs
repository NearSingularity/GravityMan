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
    public class Meteor : Sprite
    {
        static Texture2D s_Meteor, s_Explode;
        static List<Meteor> meteors = new List<Meteor>();
        static List<Meteor> deadMeteors = new List<Meteor>();

        static float delay, spawnTimer;

        public enum States { ALIVE, HIT, DEAD };
        public States state;

        protected bool isAlive, collision = false, flipHorizontal = false, flipVertical = false;
        protected Vector2 velocity;
        
        private int shortest = 2;
        private int longest = 6;

        Animation fall, dead;

        static Player m_Player;
        Vector2 playerPos;

        public Meteor():base(s_Meteor)
        {
            fall = new Animation(s_Meteor, 3);
            dead = new Animation(s_Explode, 4);

         
                int percent = random.Next(0, 100);
                int smallRandomX = random.Next(4, 8);
                int smallRandomY = random.Next(4, 8);
                int randomX = random.Next(0, (int)Camera2D.GetCamera.Width / 4);
                int randomY = random.Next(0, (int)Camera2D.GetCamera.Height);

                delay = random.Next(shortest, longest);

                if (percent <= 15)
                {
                    position = new Vector2((int)(Camera2D.GetCamera.Position.X) + (int)randomX, 0);
                    velocity = new Vector2(smallRandomX, smallRandomY);
                }
                else if (percent > 15 && percent < 85)
                {
                    position = new Vector2((int)(Camera2D.GetCamera.Position.X) + (int)Camera2D.GetCamera.Width - (int)randomX, 0);
                    velocity = new Vector2(-smallRandomX, smallRandomY);
                    flipHorizontal = true;
                }
                else if (percent >= 85)
                {
                    position = new Vector2((int)(Camera2D.GetCamera.Position.X) + (int)randomX, (int)Camera2D.GetCamera.Height + (int)randomY);
                    velocity = new Vector2(smallRandomX, -smallRandomY);
                    flipVertical = true;
                }
           

            
   
            animation = fall;

            isAlive = true;
        }

        public static void SetPlayer(Player play)
        {
            m_Player = play;
        }
 
        public static void Restart()
        {
            meteors.Clear();
            deadMeteors.Clear();
        }
        public static bool CollidePlayer(Player player)
        {
            player.SmallAABB = new Rectangle((int)player.position.X - (player.AABB.Width / 6), (int)player.position.Y - (player.AABB.Height / 6), player.AABB.Width / 4, player.AABB.Height / 2);

            foreach (Meteor meteor in meteors)
            {
                meteor.SmallAABB = new Rectangle((int)meteor.position.X - (meteor.AABB.Width / 6), (int)meteor.position.Y - (meteor.AABB.Height / 6), meteor.AABB.Width / 2, meteor.AABB.Height / 2);

                if (!meteor.collision)
                {
                    if (meteor.SmallAABB.Intersects(player.SmallAABB))
                    {
                        meteor.collision = true;
                        return true;
                    }
                }
            }
            return false;
        }
        static void Spawn(GameTime gameTime)
        {
            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (spawnTimer >= delay)
            {
                Meteor meteor = new Meteor();
                meteors.Add(meteor);
                spawnTimer -= delay;
            }
        }
        void HandleCollision()
        {
            if (collision == true)
            {
                velocity *= 0;
            }
            if (position.Y > Game.GetBot() + animation.Height)
                isAlive = false;
        }
        void HandleStates()
        {
            switch (state)
            {
                case States.ALIVE:
                    if (this.collision == true)
                        SetState(States.HIT);
                    break;
                case States.HIT:
                    if (dead.Done())
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
                    animation = fall;
                    break;
                case States.HIT:
                    animation = dead;
                    break;
                case States.DEAD:
                    isAlive = false;
                    break;
            }
        }
        public static void LoadContent(ContentManager content)
        {
            s_Meteor = content.Load<Texture2D>("SpriteSheet_Meteor");
            s_Explode = content.Load<Texture2D>("SpriteSheet_Explosion");
        }
        public override void Update(GameTime gameTime)
        {
            SpriteEffects spriteEffect = SpriteEffects.None;
            playerPos = m_Player.position;

            float totalTime = gameTime.TotalGameTime.Seconds;

            position += velocity;

            HandleCollision();
            HandleStates();

            foreach (Meteor meteor in meteors)
            {
                if (flipHorizontal == true)
                    spriteEffect = SpriteEffects.FlipHorizontally;
                if (flipVertical == true)
                    spriteEffect = SpriteEffects.FlipVertically;
                animation.SpriteEffect = spriteEffect;
            }

            if (totalTime > 30)
            {
                longest -= 1;
            }
            else if (totalTime > 60)
            {
                longest -= 2;
            }
            else if (totalTime > 90)
            {
                longest -= 3;
            }

            base.Update(gameTime);
        }
        public static void UpdateAll(GameTime gameTime)
        {
            Spawn(gameTime);
            foreach (Meteor meteor in meteors)
            {
                if (meteor.isAlive)
                    meteor.Update(gameTime);
                else
                    deadMeteors.Add(meteor);
            }
            foreach (Meteor deadMeteor in deadMeteors)
            {
                meteors.Remove(deadMeteor);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (Meteor meteor in meteors)
                meteor.Draw(spriteBatch);
        }
    }
}
