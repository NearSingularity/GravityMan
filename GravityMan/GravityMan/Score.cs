using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace GravityMan
{
    public class Score
    {
        static Score s_theScore;
        static SpriteFont font;
        static int score;
        static string text;

        Vector2 origin = new Vector2( (Game.GetSide()/2) - 55f ,  0f);

        public Score()
        {
            score = 0;
            s_theScore = this;
        }
        public static void AddPoints(int points)
        {
            score += points;
        }
        public static int GET()
        {
            return score;
        }
        public static Score Get()
        {
            return s_theScore;
        }
        public static void LoadContent(ContentManager content, string asset)
        {
            font = content.Load<SpriteFont>(asset);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            text = "Score: " + score;
            spriteBatch.DrawString(font, text, origin, Color.DarkTurquoise);
        }
    }
}
