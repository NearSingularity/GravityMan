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
    public class Coin : Sprite
    {
        static Texture2D s_Coin, s_CoinPickUp;

        static List<Coin> coins = new List<Coin>();
        static List<Coin> deadCoins = new List<Coin>();

        public enum States { ALIVE, HIT, DEAD};
        public States state;

        private static float delay, spawnTimer;
        private static int count = random.Next(1, 5);
        private bool isHit, isAlive;

        Animation spin, hit;

        public Coin():base(s_Coin)
        {
            spin = new Animation(s_Coin, 8);
            hit = new Animation(s_CoinPickUp, 3);

            int randomY = random.Next(0, (int)Game.GetBot());
            int buffer = random.Next(s_Coin.Width, (s_Coin.Width * 5));
            position = new Vector2((int)Camera2D.GetCamera.Width + Camera2D.GetCamera.Position.X + buffer, randomY);
            delay = random.Next(2, 5);

            animation = spin;

            isAlive = true;
            isHit = false;
        }
        public static void Restart()
        {
            coins.Clear();
            deadCoins.Clear();
        }
        public static bool CollidePlayer(Player player)
        {
            player.SmallAABB = new Rectangle((int)player.position.X - (player.AABB.Width / 6), (int)player.position.Y - (player.AABB.Height / 6), player.AABB.Width / 4, player.AABB.Height / 2);
            foreach (Coin coin in coins)
            {
                if (!coin.isHit)
                {
                    if (coin.AABB.Intersects(player.SmallAABB))
                    {
                        coin.isHit = true;
                        return true;
                    }
                }
            }
            return false;
        }
        void HandleCollision()
        {
            if (position.X < -Game.GetSide())
                isAlive = false;
        }
        void HandleStates()
        {
            switch (state)
            {
                case States.ALIVE:
                    if (isHit == true)
                        SetState(States.HIT);
                    break;
                case States.HIT:
                    if (hit.Done())
                    {
                        SetState(States.DEAD);
                        isAlive = false;
                    }
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
                    animation = hit;
                    break;
            }
        }
        static void Spawner(GameTime gameTime)
        {
            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (spawnTimer >= delay)
            {
                for (int i = 0; i < count; i++)
                {
                    Coin coin = new Coin();
                    coins.Add(coin);   
                }
                spawnTimer -= delay;
            }
        }
        public static void LoadContent(ContentManager content)
        {
            s_Coin = content.Load<Texture2D>("SpriteSheet_Coins");
            s_CoinPickUp = content.Load<Texture2D>("SpriteSheet_CoinPickUp");
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
            foreach (Coin coin in coins)
            {
                if (coin.isAlive)
                    coin.Update(gameTime);
                else
                    deadCoins.Add(coin);
            }
            foreach (Coin deadCoin in deadCoins)
                coins.Remove(deadCoin);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (Coin coin in coins)
                coin.Draw(spriteBatch);
        }
    }
}
