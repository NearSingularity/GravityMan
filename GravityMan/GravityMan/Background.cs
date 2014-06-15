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
    public class Background : Sprite
    {
        Texture2D backgroundTexture;

        private readonly Camera2D _camera;
        
        public Background(Camera2D camera, Texture2D _texture):base(_texture)
        {
            backgroundTexture = _texture;
   
            _camera = camera;

            Parallax = Vector2.One;
            useOrigin = false;
        }

        public Vector2 Parallax
        {
            get;
            set;
        }

        public void LoadGraphicsContent(ContentManager content, string assetName)
        {
            this.backgroundTexture = content.Load<Texture2D>(assetName);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _camera.get_transformation(Parallax));

            int first = (int)(_camera.Position.X * Parallax.X / backgroundTexture.Width);
            position.X = (first * backgroundTexture.Width);
            base.Draw(spriteBatch);
            position.X += backgroundTexture.Width;
            base.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}