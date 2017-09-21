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
    public class Camera3d
    {
        public const float DEFAULTMOVEARROWS = 0.05f;
        public const float DEFAULTZOOM = 2.0f;
        public const float SCROLLZOOMMODIFIER = 0.0005f;
        public const float NEARCLIP = 1.0f;
        public const float FARCLIP = 2000.0f;
        public static float VIEWANGLE = MathHelper.ToRadians(45.0f);
        public static float MINZOOM = 1.6f;
        public static float MAXZOOM = 28.0f;

        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector3 _pos; // Camera Position
        protected float _rotation; // Camera Rotation

        public Camera3d()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector3.Zero;
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
        public void Move(Vector3 amount)
        {
            var zoomVector = new Vector3(_pos.Z, _pos.Z, 1);
            _pos += amount * zoomVector;
            if (_pos.Z < Camera3d.MINZOOM)
            {
                _pos.Z = Camera3d.MINZOOM;
            }
            if (_pos.Z > Camera3d.MAXZOOM)
            {
                _pos.Z = Camera3d.MAXZOOM;
            }
        }
        // Get set position
        public Vector3 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Vector3 getCameraLookAt()
        {
            return new Vector3(this.Pos.X, this.Pos.Y, 0.0f);
        }

        public void handleCamMoovementMouseInput(MouseState _mouseState, MouseState _previousMouseState, KeyboardState _keyboardState)
        {
            if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Pressed && !_keyboardState.IsKeyDown(Keys.LeftShift))
            {
                // Dragging mode enabled, move camera with delta of previous en current mousestate
                Vector3 delta = Vector3.Subtract(new Vector3(_previousMouseState.Position.ToVector2(), 0f), new Vector3(_mouseState.Position.ToVector2(), 0f));
                this.Move(delta);
            }
        }

        public void handleCamMovementKeyboardInput(KeyboardState _keyboardState)
        {
            // Handle Cam movement en zoom
            if (_keyboardState.IsKeyDown(Keys.Left))
            {
                this.Move(new Vector3(-Camera3d.DEFAULTMOVEARROWS, 0.0f, 0.0f));
            }
            if (_keyboardState.IsKeyDown(Keys.Right))
            {
                this.Move(new Vector3(Camera3d.DEFAULTMOVEARROWS, 0.0f, 0.0f));
            }
            if (_keyboardState.IsKeyDown(Keys.Down))
            {
                this.Move(new Vector3(0.0f, -Camera3d.DEFAULTMOVEARROWS, 0.0f));
            }
            if (_keyboardState.IsKeyDown(Keys.Up))
            {
                this.Move(new Vector3(0.0f, Camera3d.DEFAULTMOVEARROWS, 0.0f));
            }
            if (_keyboardState.IsKeyDown(Keys.OemMinus))
            {
                // Adjust zoom level
                this.Move(new Vector3(0.0f, 0.0f, Camera3d.DEFAULTZOOM));
            }
            if (_keyboardState.IsKeyDown(Keys.OemPlus))
            {
                this.Move(new Vector3(0.0f, 0.0f, -Camera3d.DEFAULTZOOM));
            }
        }
    }
}
