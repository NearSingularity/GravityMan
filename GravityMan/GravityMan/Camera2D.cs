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
    public class Camera2D
    {
        static Camera2D s_Camera;
        public Matrix _transform;
        public Viewport _viewport;

        protected Vector2 _position;
        protected Vector2 _origin;
        protected Vector3 _scale;

        protected float _rotation;

        private int buffer = 50;
        private Rectangle? _limits;

        public Camera2D(Viewport viewport)
        {
            s_Camera = this;

            _viewport = viewport;
            _origin = new Vector2(viewport.Width / 2.0f, viewport.Height / 2.0f);
            _scale = Vector3.One;
            _rotation = 0.0f;
        }
        public static Camera2D GetCamera
        {
            get { return s_Camera; }
        }
        public Vector2 Origin
        {
            get { return _origin; }
        }
        public float Zoom
        {
            get { return _scale.X; }
        }
        public float Rotation
        {
            get { return _rotation; }
        }
        public Vector2 Position
        {
            get { return _position; }
        }
        public int Width
        {
            get { return _viewport.Width; }
        }
        public int Height
        {
            get { return _viewport.Height; }
        }

        public Rectangle? Limits
        {
            get { return _limits; }
            set
            {
                if (value != null)
                {
                    //Assigns limits but makes sure its always bigger than the viewport
                    _limits = new Rectangle
                    {
                        X = value.Value.X,
                        Y = value.Value.Y,
                        Width = System.Math.Max(_viewport.Width, value.Value.Width),
                        Height = System.Math.Max(_viewport.Height, value.Value.Height)
                    };
                }
                else
                {
                    _limits = null;
                }
            }
        }

        public void Update(GameTime gameTime, Player player)
        {
            _position = new Vector2(player.position.X - buffer, 0);
        }

        public Matrix get_transformation(Vector2 parallax)
        {
            // To add parallax, simply multiply it by the position
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f)) *
                   Matrix.CreateTranslation(new Vector3(-_origin, 0.0f)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(Zoom, Zoom, 1) *
                   Matrix.CreateTranslation(new Vector3(_origin, 0.0f));
        }
    }
}
