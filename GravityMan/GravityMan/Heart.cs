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
    public class Heart : Sprite
    {
        static Texture2D heart;

        private Vector2 origin;

        public Heart():base(heart)
        {
            origin = new Vector2(Game.GetSide(), Game.GetBot() + 300f);
        }

        public static void LoadContent(ContentManager content)
        {
            heart = content.Load<Texture2D>("Heart");
        }

        public static void DrawLives(SpriteBatch spriteBatch)
        {
            DrawAll(spriteBatch, Player.playerLives);
        }

        public static void DrawAll(SpriteBatch spriteBatch, int lives)
        {
            for (int i = 1; i <= lives; ++i)
            {
                spriteBatch.Draw(heart, new Vector2((Game.GetSide()/2 - 75f) + (i * heart.Width), heart.Height), Color.White);
            }
        }

    }
}
