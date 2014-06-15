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
    public class Animation
    {
        Texture2D image;
        
        protected Vector2 origin;

        protected float frameTime;
        protected float frameRate;

        private int maxFrame;
        private int frameIndex;
        private int spriteHeight;
        private int spriteWidth;

        private bool isDone;

        private SpriteEffects spriteEffect;

        public Animation(Texture2D _image, int frameCount)
        {
            frameRate = 10f;
            image = _image;
            maxFrame = frameCount;
            spriteHeight = image.Height;
            spriteWidth = image.Width / frameCount;
            origin = new Vector2(spriteWidth / 2, spriteHeight / 2);
        }

        public void Update(float deltaTime)
        {
            isDone = false;
            frameTime += frameRate * deltaTime;

            while(frameTime >= maxFrame)
            {
                isDone = true;
                frameTime -= maxFrame;
            }
            frameIndex = (int)frameTime;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, float scale, bool useOrigin)
        {
            spriteBatch.Draw(image, position, new Rectangle(frameIndex * spriteWidth, 0, spriteWidth, spriteHeight), Color.White, 0.0f, useOrigin? origin : Vector2.Zero, scale, SpriteEffect, 0);
        }
        public void StartOver()
        {
            isDone = false;
            frameTime = 0f;
            frameIndex = 0;
        }
        public bool Done()
        {
            return isDone;
        }
        public SpriteEffects SpriteEffect
        {
            get { return spriteEffect; }
            set { spriteEffect = value; }
        }
        public Vector2 SpriteOrigin
        {
            get { return origin; }
        }
        public int Width
        {
            get { return spriteWidth; }
        }
        public int Height
        {
            get { return spriteHeight; }
        }
    }
}
