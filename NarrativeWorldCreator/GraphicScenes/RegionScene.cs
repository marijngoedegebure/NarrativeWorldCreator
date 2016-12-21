using Common;
using Common.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using NarrativeWorlds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TriangleNet.Data;

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

        private int draggingVertexIndex;

        private enum RegionCreationModes
        {
            None = 0,
            VertexCreation = 1,
            VertexDragging = 2,
            VertexDeletion = 3,
        }

        private RegionCreationModes CurrentRegionCreationMode = RegionCreationModes.None;

        private enum RegionFillingModes
        {
            None = 0,
            ObjectPlacement = 1,
            ObjectDragging = 2,
            ObjectDeletion = 3,
        }

        private RegionFillingModes CurrentRegionFillingMode = RegionFillingModes.None;
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
            foreach(EntikaClassInstance instance in _currentRegionPage.selectedNode.EntikaClassInstances)
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
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            // Only draw triangles when there is 3 or more vertices
            if (_currentRegionPage.selectedNode.Shape.Points.Count > 2)
            {
                // List<VertexPositionColor> regionPoints = _currentRegionPage.selectedNode.RegionOutlinePoints;
                List<VertexPositionColor> regionPoints = new List<VertexPositionColor>();
                if (_currentRegionPage.CurrentMode == RegionPage.RegionPageMode.RegionFilling)
                {
                    regionPoints = _currentRegionPage.selectedNode.GetDrawableTriangles(Color.White);
                }
                if (_currentRegionPage.CurrentMode == RegionPage.RegionPageMode.RegionCreation)
                {
                    regionPoints = _currentRegionPage.selectedNode.GetDrawableTriangles(Color.Black);
                }
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    // This is the all-important line that sets the effect, and all of its settings, on the graphics device
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.TriangleList,
                        regionPoints.ToArray(),
                        0,
                        regionPoints.Count/3);
                }
                //foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                //{
                //    // This is the all-important line that sets the effect, and all of its settings, on the graphics device
                //    pass.Apply();
                //    GraphicsDevice.SetVertexBuffer(vertexBuffer);
                //    GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                //        PrimitiveType.TriangleList,
                //        _currentRegionPage.selectedNode.RegionOutlinePoints.ToArray(),
                //        0,
                //        _currentRegionPage.selectedNode.RegionOutlinePoints.Count,
                //        _currentRegionPage.selectedNode.triangleListIndices.ToArray(),
                //        0,
                //        _currentRegionPage.selectedNode.triangleListIndices.Count / 3);
                //}
            }

            // Draw lines for each triangle
            if (_currentRegionPage.selectedNode.Shape.Points.Count > 2)
            {
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                Color color;
                if (_currentRegionPage.CurrentMode == RegionPage.RegionPageMode.RegionCreation)
                    color = Color.Purple;
                else
                    color = Color.Black;
                var triangles = _currentRegionPage.selectedNode.Mesh.Triangles.ToList();
                var vertices = _currentRegionPage.selectedNode.Mesh.Vertices.ToList();
                foreach (var triangle in triangles)
                {
                    VertexPositionColor[] verticesLine;
                    // draw line between p0 and p1
                    verticesLine = CreateLine(vertices[triangle.P0], vertices[triangle.P1], color);
                    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                            PrimitiveType.TriangleList,
                            verticesLine,
                            0,
                            verticesLine.Length / 3);
                    }

                    // draw line between p1 and p2
                    verticesLine = CreateLine(vertices[triangle.P1], vertices[triangle.P2], color);
                    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                            PrimitiveType.TriangleList,
                            verticesLine,
                            0,
                            verticesLine.Length / 3);
                    }

                    // draw line between p2 and p0
                    verticesLine = CreateLine(vertices[triangle.P2], vertices[triangle.P0], color);
                    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                            PrimitiveType.TriangleList,
                            verticesLine,
                            0,
                            verticesLine.Length / 3);
                    }
                }
            }

            // Draw lines around polygon
            //if (_currentRegionPage.selectedNode.RegionOutlinePoints.Count > 1)
            //{
            //    GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            //    Color color;
            //    if (_currentRegionPage.CurrentMode == RegionPage.RegionPageMode.RegionCreation)
            //        color = Color.Purple;
            //    else
            //        color = Color.Black;
            //    List<Vector3> points = _currentRegionPage.selectedNode.RegionOutlinePoints;
            //    VertexPositionColor[] verticesLine;
            //    for (int i = 0; i < points.Count - 1; i++)
            //    {

            //        // Calculate center and rotation
            //        Vector3 center = (points[i] + points[i + 1]) / 2;
            //        verticesLine = CreateLine(points[i], points[i + 1], color);

            //        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //        {
            //            pass.Apply();
            //            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            //                PrimitiveType.TriangleList,
            //                verticesLine,
            //                0,
            //                verticesLine.Length/3);
            //        }
            //    }
            //    // Calculate center and rotation
            //    verticesLine = CreateLine(points[0], points[points.Count-1], color);
            //    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            //            PrimitiveType.TriangleList,
            //            verticesLine,
            //            0,
            //            verticesLine.Length / 3);
            //    }
            //}

            // Always draw the vertices
            if (_currentRegionPage.selectedNode.Shape.Points.Count > 0)
            {
                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                // Create quads based off vertex points
                List<Vector3> points = _currentRegionPage.selectedNode.GetXNAPointsOfShape();
                for (int i = 0; i < points.Count; i++)
                {
                    Color color = Color.Black;
                    if (_currentRegionPage.CurrentMode == RegionPage.RegionPageMode.RegionCreation)
                        color = Color.Red;
                    if (draggingVertexIndex != -1 && draggingVertexIndex == i)
                        color = Color.Yellow;
                    if (_currentRegionPage.CurrentMode == RegionPage.RegionPageMode.RegionFilling)
                        color = Color.DarkGray;
                    Quad quad = new Quad(points[i], new Vector3(points[i].X, points[i].Y, 1), Vector3.Up, 1, 1, color);
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

        private VertexPositionColor[] CreateLine(Vertex start, Vertex end, Color color)
        {
            VertexPositionColor[] line = new VertexPositionColor[6];
            // line triangle 1
            line[0] = new VertexPositionColor(new Vector3((float)start.X - 1, (float)start.Y - 1, 0), color);
            line[1] = new VertexPositionColor(new Vector3((float)start.X + 1, (float)start.Y + 1, 0), color);
            line[2] = new VertexPositionColor(new Vector3((float)end.X + 1, (float)end.Y + 1, 0), color);

            // Line triangle 2
            line[3] = new VertexPositionColor(new Vector3((float)end.X + 1, (float)end.Y + 1, 0), color);
            line[4] = new VertexPositionColor(new Vector3((float)end.X - 1, (float)end.Y - 1, 0), color);
            line[5] = new VertexPositionColor(new Vector3((float)start.X - 1, (float)start.Y - 1, 0), color);
            return line;
        }

        private VertexPositionColor[] CreateLine(Vector3 start, Vector3 end, Color color)
        {
            VertexPositionColor[] line = new VertexPositionColor[6];
            // line triangle 1
            line[0] = new VertexPositionColor(new Vector3(start.X - 1, start.Y - 1, start.Z), color);
            line[1] = new VertexPositionColor(new Vector3(start.X + 1, start.Y + 1, start.Z), color);
            line[2] = new VertexPositionColor(new Vector3(end.X + 1, end.Y + 1, end.Z), color);

            // Line triangle 2
            line[3] = new VertexPositionColor(new Vector3(end.X + 1, end.Y + 1, end.Z), color);
            line[4] = new VertexPositionColor(new Vector3(end.X - 1, end.Y - 1, end.Z), color);
            line[5] = new VertexPositionColor(new Vector3(start.X - 1, start.Y - 1, start.Z), color);
            return line;
        }

        private float ConvertToRadians(float angle)
        {
            return (float) (Math.PI / 180f) * angle;
        }

        public void drawEntikaInstance(EntikaClassInstance instance)
        {
            Texture2D texture = Content.Load<Texture2D>("maps/couch_diff"); ;
            Matrix[] transforms = new Matrix[instance.Model.Bones.Count];
            instance.Model.CopyAbsoluteBoneTransformsTo(transforms);
            Vector3 modelPosition = instance.Position;
            // Rotation should be in radians, rotates model to top down view
            float modelRotation = ConvertToRadians(90.0f);
            
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in instance.Model.Meshes)
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
            // cam.handleCamMoovementMouseInput(_mouseState, _previousMouseState, _keyboardState);

            if (_currentRegionPage.CurrentMode == RegionPage.RegionPageMode.RegionCreation)
            {
                // Handle region creation input:
                HandleRegionCreation();
            }
            else
            {
                HandleRegionFilling();
            }

            if(_currentRegionPage.selectedNode.Shape.Points.Count > 2)
            {
                _currentRegionPage.RegionCreated = true;
            }

            base.Update(time);
        }

        private void HandleRegionFilling()
        {
            if (_keyboardState.IsKeyDown(Keys.LeftControl))
            {
                CurrentRegionFillingMode = RegionFillingModes.ObjectPlacement;
            }
            else if (_keyboardState.IsKeyDown(Keys.LeftShift))
            {
                CurrentRegionFillingMode = RegionFillingModes.ObjectDragging;
            }
            else
            {
                CurrentRegionFillingMode = RegionFillingModes.None;
            }

            // Handle object placement
            if (CurrentRegionFillingMode == RegionFillingModes.ObjectPlacement)
            {
                if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
                {
                    // Calculate intersection with the plane through x = 0, y = 0, which should always hit due to the camera pointing directly downward
                    Model model = LoadModel(Path.GetFileNameWithoutExtension("couch"));
                    NarrativeTimePoint ntp = ((RegionDetailTimePointViewModel)_currentRegionPage.RegionDetailTimePointView.DataContext).NarrativeTimePoint;
                    var position = SolvingEngine.GetPossibleLocations(_currentRegionPage.selectedNode, ntp);
                    _currentRegionPage.selectedNode.EntikaClassInstances.Add(new EntikaClassInstance("couch", position, model));

                    // Determine shapes for entika class instances

                }
            }

            // Handle object selection
            if (CurrentRegionFillingMode == RegionFillingModes.None)
            {
                if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
                {
                    HandleObjectSelection();
                }
            }
        }

        private void HandleObjectSelection()
        {
            Ray ray = CalculateMouseRay();
            foreach(EntikaClassInstance ieo in _currentRegionPage.selectedNode.EntikaClassInstances)
            {
                // Calculate/retrieve boundingbox
                BoundingBox bbCurrentInstance = UpdateBoundingBox(ieo.Model, world * Matrix.CreateRotationY(ConvertToRadians(90.0f))
                        * Matrix.CreateTranslation(ieo.Position));

                // Intersect ray with bounding box, if distance then select model
                float? distance = ray.Intersects(bbCurrentInstance);
                if (distance != null)
                {
                    _currentRegionPage.ChangeSelectedObject(ieo);
                    return;
                }
            }
            _currentRegionPage.DeselectObject();
        }

        protected BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
        }

        private Vector3 CalculateMouseHitOnSurface()
        {
            Ray ray = CalculateMouseRay();
            float? distance = ray.Intersects(new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
            return ray.Position + ray.Direction * distance.Value;
        }

        private Ray CalculateMouseRay()
        {
            Vector3 nearsource = new Vector3((float)_mouseState.X, (float)_mouseState.Y, 0f);
            Vector3 farsource = new Vector3((float)_mouseState.X, (float)_mouseState.Y, 1f);

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, proj, view, world);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, proj, view, world);

            // Create ray using far and near point
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }

        private int CalculateCollisionQuad()
        {
            Ray ray = CalculateMouseRay();
            List<Vector3> points = _currentRegionPage.selectedNode.GetXNAPointsOfShape();
            for (int i = 0; i < points.Count; i++)
            {
                Quad quad = new Quad(points[i], new Vector3(points[i].X, points[i].Y, 1), Vector3.Up, 1, 1, Color.Red);
                BoundingBox box = new BoundingBox(new Vector3(points[i].X - 1, points[i].Y - 1, points[i].Z), new Vector3(points[i].X + 1, points[i].Y + 1, points[i].Z));
                float? distance = ray.Intersects(box);
                if (distance != null)
                    return i;
            }
            return -1;
        }

        private void HandleRegionCreation()
        {
            if (_keyboardState.IsKeyDown(Keys.LeftControl))
            {
                CurrentRegionCreationMode = RegionCreationModes.VertexDragging;
            }
            else if (_keyboardState.IsKeyDown(Keys.LeftShift))
            {
                CurrentRegionCreationMode = RegionCreationModes.VertexCreation;
            }
            else
            {
                CurrentRegionCreationMode = RegionCreationModes.None;
                draggingVertexIndex = -1;
            }

            // Vertex Dragging
            if (CurrentRegionCreationMode.Equals(RegionCreationModes.VertexDragging))
            {
                HandleVertexDragging();
            }

            // Vertex creation 
            if (CurrentRegionCreationMode.Equals(RegionCreationModes.VertexCreation))
            {
                HandleVertexCreation();
            }
        }

        private void HandleVertexCreation()
        {
            if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
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
                float? distance = ray.Intersects(new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
                Vector3 planeHit = ray.Position + ray.Direction * distance.Value;

                // Retrieve vertices from old shape and add the new vertex
                var vertices = _currentRegionPage.selectedNode.Shape.Points;
                vertices.Add(new Vec2(planeHit.X, planeHit.Y));
                _currentRegionPage.selectedNode.Shape = new Shape(vertices);
                _currentRegionPage.selectedNode.triangulatePolygon();
            }
        }

        private void HandleVertexDragging()
        {
            if (_previousMouseState.LeftButton == ButtonState.Released && _mouseState.LeftButton == ButtonState.Pressed)
                draggingVertexIndex = CalculateCollisionQuad();

            if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Pressed && draggingVertexIndex != -1)
            {
                Vector3 delta = Vector3.Subtract(new Vector3(_previousMouseState.Position.ToVector2(), 0f), new Vector3(_mouseState.Position.ToVector2(), 0f));
                var vertices = _currentRegionPage.selectedNode.Shape.Points;
                Vector3 mouseCoordsOnZPlane = CalculateMouseHitOnSurface();
                vertices[draggingVertexIndex] = new Vec2(mouseCoordsOnZPlane.X, mouseCoordsOnZPlane.Y);
                _currentRegionPage.selectedNode.Shape = new Shape(vertices);
                _currentRegionPage.selectedNode.triangulatePolygon();
            }

            if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
                draggingVertexIndex = -1;
        }

        #endregion
    }
}