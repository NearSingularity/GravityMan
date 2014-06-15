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
    public class Player : Sprite
    {
        static Texture2D s_Idle, s_Run, s_Jump, s_Slide, s_Dead;

        static Player instance;

        KeyboardState previousKBState, currentKBState;
        GamePadState previousGPState, currentGPState;

        Animation idle, run, jump, slide, dead;

        public enum States { RUN, JUMP, IDLE, HIT, SLIDE, DEAD, TOTAL};
        public States state;

        public static int playerLives;
        public static bool blackHoleHit = false, coinHit = false, meteorHit = false;

        protected Vector2 spawnPoint = new Vector2(50f, Game.GetBot());

        private bool flipHorizontal = false, flipVertical = false, backwards = false, isDead = false;

        private static Rectangle rectangle;
        private float gravity = 1.0f;
        private float friction = 3f;
        private float boost = 250f;
        private float acceleration = 300f;
        private float gravityRate = 400f;
        private float timer = 0f;
        private float speedBoostTimer = 0f;
        private float boostRate = 0f;
        private float deltaTime;
        private float direction;
        private float shootRate = .25f;
        private float shootTimer = 0f;
        
        private const int NIL = 0;
        private const int BOOST_RATE = 5;
        private const float SPEED_BOOST = 1.0f;

        public Player():base(s_Idle)
        {
            boostRate = BOOST_RATE;
            playerLives = 3; 

            idle = new Animation(s_Idle, 1);
            run = new Animation(s_Run, 9);
            jump = new Animation(s_Jump, 7);
            slide = new Animation(s_Slide, 12);
            dead = new Animation(s_Dead, 5);

            rectangle = this.AABB;
           
            position = spawnPoint;
            
            animation = run;
            instance = this;
        }
        public Vector2 GetPos()
        {
            return position;
        }

        public static void LoadContent(ContentManager content)
        {
            s_Idle = content.Load<Texture2D>("SpriteSheet_Idle");
            s_Run = content.Load<Texture2D>("SpriteSheet_Run");
            s_Jump = content.Load<Texture2D>("SpriteSheet_Jump");
            s_Slide = content.Load<Texture2D>("SpriteSheet_Slide");
            s_Dead = content.Load<Texture2D>("SpriteSheet_Dead");
        }
        public override void Update(GameTime gameTime)
        {
            //Makes Stars and Coins pointless
            //Score.AddPoints(1);

            HandleSpriteMovement(gameTime);
            HandleCollisions();

            position.Y = MathHelper.Clamp(position.Y, (animation.Height / 2),  Game.GetBot() - (animation.Height / 2));
            position.X = MathHelper.Clamp(position.X, 0f, float.PositiveInfinity);

            if (playerLives <= 0)
                isDead = true;

            base.Update(gameTime);
        }
        public void HandleCollisions()
        {
            if (Star.CollidePlayer(this))
            {
                blackHoleHit = true;
                AudioComponent.Get().PlaySound("DeathToBlackHole");
                Collision();
            }
            else if (Meteor.CollidePlayer(this))
            {
                meteorHit = true;
                AudioComponent.Get().PlaySound("Explosion");
                Collision();
            }
            else if (Coin.CollidePlayer(this))
            {
                coinHit = true;
                AudioComponent.Get().PlaySound("CoinCollect");
                Score.AddPoints(15);
            }
        }
        public void HandleSpriteMovement(GameTime gameTime)
        {
            AudioEmitter emitter = new AudioEmitter();
            SpriteEffects spriteEffect = SpriteEffects.None;  

            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer += deltaTime;
            shootTimer += deltaTime;

            previousKBState = currentKBState;
            currentKBState = Keyboard.GetState();
            previousGPState = currentGPState;
            currentGPState = GamePad.GetState(PlayerIndex.One);

            Vector2 move = currentGPState.ThumbSticks.Left;
            move.Y = -move.Y;
            move.X = 1.0f;

            if (shootTimer > shootRate)
            {
                Bullet bullet = new Bullet(this.position);

                AudioComponent.Get().PlaySound("Shoot");
                shootTimer = 0.0f;
            }
          
            if (timer > boostRate)
            {
                if (currentGPState.IsButtonDown(Buttons.RightShoulder) && previousGPState.IsButtonDown(Buttons.RightShoulder)
                    ||currentKBState.IsKeyDown(Keys.Right) && previousKBState.IsKeyUp(Keys.Right))
                {
                    speedBoostTimer = SPEED_BOOST;
                    direction = 1f;
                    timer = 0f;
                }
                if (currentGPState.IsButtonDown(Buttons.LeftShoulder) && previousGPState.IsButtonDown(Buttons.LeftShoulder)
                    ||currentKBState.IsKeyDown(Keys.Left) && previousKBState.IsKeyUp(Keys.Left))
                {
                    speedBoostTimer = SPEED_BOOST; 
                    direction = -friction;
                    timer = 0f;
                    backwards = true;
                }
            }
            if (currentKBState.IsKeyDown(Keys.Up) == true && previousKBState.IsKeyUp(Keys.Up))
            {
                //AudioComponent.Get().PlaySound("GravUp");
                move.Y -= 1.0f;
            }

            if (currentKBState.IsKeyDown(Keys.Down) == true && previousKBState.IsKeyUp(Keys.Down))
            {
                //AudioComponent.Get().PlaySound("GravDown");
                move.Y += 1.0f;
            }
            if (currentGPState.ThumbSticks.Left.Y > 0 && previousGPState.ThumbSticks.Left.Y == 0)
            {
                //AudioComponent.Get().PlaySound("GravUp");
                move.Y -= currentGPState.ThumbSticks.Left.Y;
            }
            if (currentGPState.ThumbSticks.Left.Y < 0 && previousGPState.ThumbSticks.Left.Y == 0)
            {
                //AudioComponent.Get().PlaySound("GravDown");
                move.Y += currentGPState.ThumbSticks.Left.Y;
            }

            if (move.Y < 0)
            {
                flipVertical = true;
                gravity = -1;
            }
            else if (move.Y > 0)
            {
                flipVertical = false;
                gravity = 1;
            }

            if (move.X > 0)
                flipHorizontal = false;
            if (move.X < 0 || backwards)
                flipHorizontal = true;

            SpeedBoost(deltaTime, direction);

            switch (state)
            {
                case States.IDLE:
                    if (move.X > 0 || move.X < 0)
                        SetState(States.RUN);
                    if (move.Y * gravity > 0)
                        SetState(States.JUMP);
                    if (blackHoleHit || meteorHit)
                        SetState(States.DEAD);
                    break;
                case States.RUN:
                    if (move.X == 0)
                        SetState(States.IDLE);
                    if (move.Y * gravity > 0)
                        SetState(States.JUMP);
                    if (speedBoostTimer > 0)
                        SetState(States.SLIDE);
                    if (blackHoleHit || meteorHit)
                        SetState(States.DEAD);
                    break;
                case States.JUMP:
                    if (animation.Done())
                        SetState(States.IDLE);
                    if (speedBoostTimer > 0)
                        SetState(States.SLIDE);
                    if (blackHoleHit || meteorHit)
                        SetState(States.DEAD);
                    break;
                case States.SLIDE:
                    if (blackHoleHit || meteorHit)
                        SetState(States.DEAD);
                    if (animation.Done())
                        SetState(States.RUN);
                    break;
                case States.DEAD:
                    if (animation.Done())
                        Reset();
                    break;
            }

            if (flipVertical == true)
                spriteEffect |= SpriteEffects.FlipVertically;
            if (flipHorizontal == true)
                spriteEffect |= SpriteEffects.FlipHorizontally;

            animation.SpriteEffect = spriteEffect;

            position.Y += (gravityRate - friction) * gravity * deltaTime;
            position.X += (acceleration - friction) * move.X * deltaTime;
        }

        void SetState(States myState)
        {
            state = myState;

            switch (state)
            {
                case States.IDLE:
                    animation = idle;
                    break;
                case States.RUN:
                    backwards = false;
                    animation = run;
                    break; 
                case States.JUMP:
                    animation = jump;
                    animation.StartOver();
                    break;
                case States.SLIDE:
                    animation = slide;
                    animation.StartOver();
                    break;
                case States.DEAD:
                    animation = dead;
                    break;
            }  
        }
        void Collision()
        {
            acceleration = NIL;
            friction = NIL;
            gravityRate = NIL;
            boost = NIL;
        }
        void Reset() 
        {
            position = spawnPoint;
            playerLives--;

            SetState(States.RUN);

            blackHoleHit = false;
            meteorHit = false;
            flipVertical = false;
            flipHorizontal = false;
            Star.collision = false;

            gravity = 1;
            friction = 3f; 
            boost = 250f;
            acceleration = 300f;
            gravityRate = 400f;
        }  
        void SpeedBoost(float deltaTime, float direction)
        {
            speedBoostTimer -= deltaTime;
            if (speedBoostTimer > 0f)
                position.X += boost * direction * deltaTime;
        }
        public bool IsDead
        {
            get { return isDead; }
        }
    }
}
