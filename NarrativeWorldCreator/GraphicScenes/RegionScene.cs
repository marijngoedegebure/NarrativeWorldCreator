using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using NarrativeWorldCreator.Pages;
using System;
using System.Collections.Generic;

namespace NarrativeWorldCreator
{
    public class RegionScene : WpfGame
    {
        #region Fields

        private IGraphicsDeviceService _graphicsDeviceManager;
        private WpfKeyboard _keyboard;
        private KeyboardState _keyboardState;
        private WpfMouse _mouse;
        private MouseState _mouseState;
        private MouseState _previousMouseState;
        private SpriteBatch _spriteBatch;

        private Camera3d cam = new Camera3d();

        private RegionPage _currentRegionPage;
        #endregion

        #region Methods
        protected override void Initialize()
        {
            base.Initialize();
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            cam.Pos = new Vector3(0.0f, 100.0f, 500.0f);

            _keyboard = new WpfKeyboard(this);
            _mouse = new WpfMouse(this);

            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            _currentRegionPage = (RegionPage)mainWindow._mainFrame.NavigationService.Content;
        }


        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(_mouseState.LeftButton == ButtonState.Pressed ? Color.Black : Color.CornflowerBlue);

            // since we share the GraphicsDevice with all hosts, we need to save and reset the states
            // this has to be done because spriteBatch internally sets states and doesn't reset themselves, fucking over any 3D rendering (which happens in the DemoScene)

            var blend = GraphicsDevice.BlendState;
            var depth = GraphicsDevice.DepthStencilState;
            var raster = GraphicsDevice.RasterizerState;
            var sampler = GraphicsDevice.SamplerStates[0];

            _graphicsDeviceManager.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Blue);
            SpriteBatch spriteBatch = new SpriteBatch(_graphicsDeviceManager.GraphicsDevice);
            drawModelExampleFunction();

            // this base.Draw call will draw "all" components (we only added one)
            // since said component will use a spritebatch to render we need to let it draw before we reset the GraphicsDevice
            // otherwise it will just alter the state again and fuck over all the other hosts
            base.Draw(time);

            GraphicsDevice.BlendState = blend;
            GraphicsDevice.DepthStencilState = depth;
            GraphicsDevice.RasterizerState = raster;
            GraphicsDevice.SamplerStates[0] = sampler;

        }

        public void drawModelExampleFunction()
        {
            Model myModel = Content.Load<Model>("cylinder");
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            Vector3 modelPosition = Vector3.Zero;
            float modelRotation = 90.0f;
            myModel.CopyAbsoluteBoneTransformsTo(transforms);
            // Set up the view matrix and projection matrix.
            Matrix view = Matrix.CreateLookAt(cam.Pos, cam.getCameraLookAt(), Vector3.Up);

            Matrix proj = Matrix.CreatePerspectiveFieldOfView(Camera3d.VIEWANGLE, _graphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio,
                                              Camera3d.NEARCLIP, Camera3d.FARCLIP);
            //var proj = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 1.0f, 1000.0f);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(modelPosition);
                    effect.View = view;
                    effect.Projection = proj;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        protected override void Update(GameTime time)
        {
            _previousMouseState = _mouseState;
            _mouseState = _mouse.GetState();
            _keyboardState = _keyboard.GetState();
            cam.handleCamMovementKeyboardInput(_keyboardState);
            cam.handleCamMoovementMouseInput(_mouseState, _previousMouseState);
            base.Update(time);
        }

        #endregion
    }
}