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
            ShowMinkowskiMinus = 4,
        }

        private RegionFillingModes CurrentRegionFillingMode = RegionFillingModes.None;

        private enum DrawingModes
        {
            UnderlyingRegion = 0,
            MinkowskiMinus = 1,
        }
        private DrawingModes CurrentDrawingMode = DrawingModes.UnderlyingRegion;
        private KeyboardState _previousKeyboardState;
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

            if (CurrentDrawingMode == DrawingModes.MinkowskiMinus)
            {
                drawNarrativeShapes();
            }
            if (CurrentDrawingMode == DrawingModes.UnderlyingRegion)
            {
                drawRegionPolygon();
            }

            // Draw all objects that have been added to the scene, potential accelleration by not first retrieving the narrativeshapes who use models and afterwards go through the list again to draw
            foreach(EntikaInstance instance in _currentRegionPage.SelectedTimePoint.TimePointSpecificFill.OtherObjectInstances)
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

        private void drawNarrativeShapes()
        {
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = proj;
            basicEffect.VertexColorEnabled = true;
            foreach (var shape in _currentRegionPage.SelectedTimePoint.TimePointSpecificFill.NarrativeShapes)
            {
                if (shape.Polygon != null)
                {
                    var result = HelperFunctions.GetMeshForPolygon(shape.Polygon);
                    List<VertexPositionColor> drawableTriangles = DrawingEngine.GetDrawableTriangles(result, Color.Aquamarine);
                    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                    {
                        // This is the all-important line that sets the effect, and all of its settings, on the graphics device
                        pass.Apply();
                        GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                            PrimitiveType.TriangleList,
                            drawableTriangles.ToArray(),
                            0,
                            drawableTriangles.Count / 3);
                    }
                }
            }
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

        public void drawEntikaInstance(EntikaInstance instance)
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
                    // Matrix.CreateRotationX(modelRotation) * , add later
                    effect.Parameters["WorldViewProjection"].SetValue(transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(modelPosition) * (view * proj));
                    effect.Parameters["Texture"].SetValue(texture);
                    if (_currentRegionPage.SelectedEntikaObject != null && _currentRegionPage.SelectedEntikaObject.Equals(instance))
                    {
                        effect.CurrentTechnique = effect.Techniques["TexturedSelected"];
                    }
                    else
                    {
                        effect.CurrentTechnique = effect.Techniques["Textured"];
                    }
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
            _previousKeyboardState = _keyboardState;
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

            if (_keyboardState.IsKeyUp(Keys.Tab) && _previousKeyboardState.IsKeyDown(Keys.Tab))
            {
                if (CurrentDrawingMode == DrawingModes.MinkowskiMinus)
                {
                    CurrentDrawingMode = DrawingModes.UnderlyingRegion;
                }
                else if (CurrentDrawingMode == DrawingModes.UnderlyingRegion)
                {
                    CurrentDrawingMode = DrawingModes.MinkowskiMinus;
                }
            }

            if (_currentRegionPage.selectedNode.Shape.Points.Count > 2)
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
                    NarrativeTimePoint ntp = ((RegionDetailTimePointViewModel)_currentRegionPage.RegionDetailTimePointView.DataContext).NarrativeTimePoint;
                    // Update PlanningEngine's available information, update wishlist
                    PlanningEngine.UpdateWishList(ntp);

                    // Select one on ?? criteria
                    var tuple = PlanningEngine.SelectTangibleObjectDestinationShape(ntp);
                    var tangibleObjectName = tuple.Item1;
                    var DestinationShape = tuple.Item2;

                    // Get valid position for shape
                    var position = PlanningEngine.GetPossibleLocationsV3(DestinationShape);
                    Model model = LoadModel(Path.GetFileNameWithoutExtension(tangibleObjectName));

                    // Create entika instance and update (currently relies on floor shape being used)
                    var ei = new EntikaInstance(tangibleObjectName, position, model, world);
                    DestinationShape.Relations[0].Targets.Add(ei);
                    ei.RelationshipsAsTarget.Add(DestinationShape.Relations[0]);

                    // Determine shapes for entika class instances
                    NarrativeTimePoint ntpRet = SolvingEngine.AddEntikaInstanceToTimePointBasic(ntp, ei, DestinationShape);
                    ((RegionDetailTimePointViewModel)_currentRegionPage.RegionDetailTimePointView.DataContext).NarrativeTimePoint = ntpRet;
                    

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
            foreach (EntikaInstance ieo in _currentRegionPage.SelectedTimePoint.TimePointSpecificFill.OtherObjectInstances)
            {
                // Calculate/retrieve boundingbox
                ieo.UpdateBoundingBoxAndShape(world);

                // Intersect ray with bounding box, if distance then select model
                float? distance = ray.Intersects(ieo.BoundingBox);
                if (distance != null)
                {
                    _currentRegionPage.ChangeSelectedObject(ieo);
                    return;
                }
            }
            _currentRegionPage.DeselectObject();
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
                _currentRegionPage.SelectedTimePoint.SetBaseShape(_currentRegionPage.selectedNode);
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
                _currentRegionPage.SelectedTimePoint.SetBaseShape(_currentRegionPage.selectedNode);
            }

            if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
                draggingVertexIndex = -1;
        }

        #endregion
    }
}