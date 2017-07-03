using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.GraphicScenes
{
    public class Camera2d
    {
        public const float DEFAULTZOOM = 0.05f;
        public const float DEFAULTMOVE = 5.0f;
        public const float SCROLLZOOMMODIFIER = 0.0005f;

        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation

        public Camera2d()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        }

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }
        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Matrix get_transformation(GraphicsDevice graphicsDevice, float ViewportWidth, float ViewportHeight)
        {
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(ViewportWidth * 0.5f, ViewportHeight * 0.5f, 0));
            return _transform;
        }

        public void handleCamMoovementMouseInput(MouseState _mouseState, MouseState _previousMouseState)
        {
            if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Pressed)
            {
                // Dragging mode enabled, move camera with delta of previous en current mousestate
                Vector2 delta = Vector2.Subtract(_previousMouseState.Position.ToVector2(), _mouseState.Position.ToVector2());
                this.Move(delta);
            }
        }

        public void handleCamMovementKeyboardInput(KeyboardState _keyboardState)
        {
            // Handle Cam movement en zoom
            if (_keyboardState.IsKeyDown(Keys.Left))
            {
                this.Move(new Vector2(-Camera2d.DEFAULTMOVE, 0.0f));
            }
            if (_keyboardState.IsKeyDown(Keys.Right))
            {
                this.Move(new Vector2(Camera2d.DEFAULTMOVE, 0.0f));
            }
            if (_keyboardState.IsKeyDown(Keys.Down))
            {
                this.Move(new Vector2(0.0f, Camera2d.DEFAULTMOVE));
            }
            if (_keyboardState.IsKeyDown(Keys.Up))
            {
                this.Move(new Vector2(0.0f, -Camera2d.DEFAULTMOVE));
            }
            if (_keyboardState.IsKeyDown(Keys.OemMinus))
            {
                this.Zoom = this.Zoom - Camera2d.DEFAULTZOOM;
            }
            if (_keyboardState.IsKeyDown(Keys.OemPlus))
            {
                this.Zoom = this.Zoom + Camera2d.DEFAULTZOOM;
            }
        }
    }
}
