using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using NarrativeWorlds;
using System;
using System.Collections.Generic;
using System.IO;

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

        private VertexBuffer vertexBuffer;

        private Matrix view;
        private Matrix proj;
        private Matrix world;

        private Effect effect;
        #endregion

        #region Methods
        protected override void Initialize()
        {
            base.Initialize();
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            cam.Pos = new Vector3(0.0f, 0.0f, 200.0f);

            _keyboard = new WpfKeyboard(this);
            _mouse = new WpfMouse(this);

            effect = Content.Load<Effect>("Effects/Textured");

            RasterizerState state = new RasterizerState();
            state.CullMode = CullMode.CullClockwiseFace;
            state.FillMode = FillMode.WireFrame;
            GraphicsDevice.RasterizerState = state;

            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            _currentRegionPage = (RegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private Model LoadModel(string assetName)
        {
            Model newModel = Content.Load<Model>(assetName);
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect;
            return newModel;
        }

        private void UpdateViewAndProj()
        {
            view = Matrix.CreateLookAt(cam.Pos, cam.getCameraLookAt(), Vector3.Up);

            proj = Matrix.CreatePerspectiveFieldOfView(Camera3d.VIEWANGLE, _graphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio,
                                              Camera3d.NEARCLIP, Camera3d.FARCLIP);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(_mouseState.LeftButton == ButtonState.Pressed ? Color.Black : Color.CornflowerBlue);
            world = Matrix.Identity;
            UpdateViewAndProj();
            // since we share the GraphicsDevice with all hosts, we need to save and reset the states
            // this has to be done because spriteBatch internally sets states and doesn't reset themselves, fucking over any 3D rendering (which happens in the DemoScene)

            var blend = GraphicsDevice.BlendState;
            var depth = GraphicsDevice.DepthStencilState;
            var raster = GraphicsDevice.RasterizerState;
            var sampler = GraphicsDevice.SamplerStates[0];

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Blue);
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            drawRegionPolygon();

            // Draw all objects that have been added to the scene
            foreach(InstancedEntikaObject instance in _currentRegionPage.selectedNode.InstancedEntikaObjects)
            {
                drawEntikaInstance(instance);
            }

            // this base.Draw call will draw "all" components
            base.Draw(time);

            GraphicsDevice.BlendState = blend;
            GraphicsDevice.DepthStencilState = depth;
            GraphicsDevice.RasterizerState = raster;
            GraphicsDevice.SamplerStates[0] = sampler;
        }

        public void drawRegionPolygon()
        {
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);

            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = proj;
            basicEffect.VertexColorEnabled = true;
            // Triangles should be defined clockwise
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            // Only draw triangles when there is 3 or more vertices
            if (_currentRegionPage.selectedNode.RegionOutlinePoints.Count > 2)
            {
                vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, _currentRegionPage.selectedNode.RegionOutlinePoints.Count, BufferUsage.WriteOnly);
                vertexBuffer.SetData(_currentRegionPage.selectedNode.RegionOutlinePoints.ToArray());
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    // This is the all-important line that sets the effect, and all of its settings, on the graphics device
                    pass.Apply();
                    GraphicsDevice.SetVertexBuffer(vertexBuffer);
                    GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                        PrimitiveType.TriangleList,
                        _currentRegionPage.selectedNode.RegionOutlinePoints.ToArray(),
                        0,
                        _currentRegionPage.selectedNode.RegionOutlinePoints.Count,
                        _currentRegionPage.selectedNode.triangleListIndices.ToArray(),
                        0,
                        _currentRegionPage.selectedNode.triangleListIndices.Count / 3);
                }
            }

            // Always draw the vertices
            if (_currentRegionPage.selectedNode.RegionOutlinePoints.Count > 0) {
                // Create quads based off vertex points
                foreach(VertexPositionColor vertex in _currentRegionPage.selectedNode.RegionOutlinePoints)
                {
                    Quad quad = new Quad(vertex.Position, new Vector3(vertex.Position.X, vertex.Position.Y, 1), Vector3.Up, 1, 1);
                    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                            PrimitiveType.TriangleStrip,
                            quad.Vertices,
                            0,
                            2);
                    }
                }
                
            }
        }

        private float ConvertToRadians(float angle)
        {
            return (float) (Math.PI / 180f) * angle;
        }

        public void drawEntikaInstance(InstancedEntikaObject instance)
        {
            Model myModel = LoadModel(Path.GetFileNameWithoutExtension(instance.ModelInstance.Name));
            Texture2D texture = Content.Load<Texture2D>("maps/couch_diff"); ;
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            Vector3 modelPosition = instance.Position;
            // Rotation should be in radians, rotates model to top down view
            float modelRotation = ConvertToRadians(90.0f);
            myModel.CopyAbsoluteBoneTransformsTo(transforms);
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["WorldViewProjection"].SetValue(transforms[mesh.ParentBone.Index] * Matrix.CreateRotationX(modelRotation) * Matrix.CreateTranslation(modelPosition) * (view * proj));
                    effect.Parameters["Texture"].SetValue(texture);
                    effect.CurrentTechnique = effect.Techniques["Textured"];
                    //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    //{
                    //    pass.Apply();
                    //    //effect.EnableDefaultLighting();
                    //    //effect.World = transforms[mesh.ParentBone.Index] *
                    //    //    Matrix.CreateRotationY(modelRotation)
                    //    //    * Matrix.CreateTranslation(modelPosition);
                    //    //effect.View = view;
                    //    //effect.Projection = proj;
                    //    //effect.TextureEnabled = true;
                    //    //effect.Texture = texture;
                    //    // Draw the mesh, using the effects set above.

                    //}
                    mesh.Draw();
                }
            }
        }

        public void drawModelExampleFunction()
        {
            Model myModel = Content.Load<Model>("cylinder");
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            Vector3 modelPosition = Vector3.Zero;
            float modelRotation = 90.0f;
            myModel.CopyAbsoluteBoneTransformsTo(transforms);
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
            cam.handleCamMoovementMouseInput(_mouseState, _previousMouseState, _keyboardState);

            if (_currentRegionPage.CurrentMode == RegionPage.RegionPageMode.RegionCreation)
            {
                // Handle region creation:
                // First step, check whether left button has been clicked
                if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released && _keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    // Retrieve world coordinates of current mouse position
                    Vector3 nearsource = new Vector3((float)_mouseState.X, (float)_mouseState.Y, 0f);
                    Vector3 farsource = new Vector3((float)_mouseState.X, (float)_mouseState.Y, 1f);

                    Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, proj, view, world);
                    Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, proj, view, world);

                    // Create ray using far and near point
                    Vector3 direction = farPoint - nearPoint;
                    direction.Normalize();
                    Ray ray = new Ray(nearPoint, direction);

                    // Calculate intersection with the plane through x = 0, y = 0, which should always hit due to the camera pointing directly downward
                    float? distance = ray.Intersects(new Plane(new Vector3(0, 0, 1), 0));
                    Vector3 planeHit = ray.Position + ray.Direction * distance.Value;
                    _currentRegionPage.selectedNode.RegionOutlinePoints.Add(new VertexPositionColor(planeHit, Color.Black));
                    _currentRegionPage.selectedNode.triangulatePolygon();
                }
            }
            else
            {
                // Handle object placement
                if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released && _keyboardState.IsKeyDown(Keys.LeftControl))
                {
                    Vector3 nearsource = new Vector3((float)_mouseState.X, (float)_mouseState.Y, 0f);
                    Vector3 farsource = new Vector3((float)_mouseState.X, (float)_mouseState.Y, 1f);

                    Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, proj, view, world);
                    Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, proj, view, world);

                    // Create ray using far and near point
                    Vector3 direction = farPoint - nearPoint;
                    direction.Normalize();
                    Ray ray = new Ray(nearPoint, direction);

                    // Calculate intersection with the plane through x = 0, y = 0, which should always hit due to the camera pointing directly downward
                    float? distance = ray.Intersects(new Plane(new Vector3(0, 0, 1), 0));
                    Vector3 planeHit = ray.Position + ray.Direction * distance.Value;
                    _currentRegionPage.selectedNode.InstancedEntikaObjects.Add(new InstancedEntikaObject("couch", planeHit));
                }
            }

            base.Update(time);
        }

        #endregion
    }
}