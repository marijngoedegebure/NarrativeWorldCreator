using Common;
using Common.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.Solvers;
using NarrativeWorldCreator.ViewModel;
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
            SuggestionMode = 4
        }

        private RegionFillingModes CurrentRegionFillingMode = RegionFillingModes.None;

        private enum DrawingModes
        {
            UnderlyingRegion = 0,
            MinkowskiMinus = 1,
        }
        private DrawingModes CurrentDrawingMode = DrawingModes.UnderlyingRegion;
        private KeyboardState _previousKeyboardState;

        private Model Ship;

        #endregion

        #region Methods
        protected override void Initialize()
        {
            base.Initialize();
            SystemStateTracker.RegionGraphicsDevice = this.GraphicsDevice;
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            cam.Pos = new Vector3(0.0f, 0.0f, 200.0f);

            _keyboard = new WpfKeyboard(this);
            _mouse = new WpfMouse(this);

            RasterizerState state = new RasterizerState();
            state.CullMode = CullMode.CullClockwiseFace;
            state.FillMode = FillMode.WireFrame;
            GraphicsDevice.RasterizerState = state;

            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            _currentRegionPage = (RegionPage)mainWindow._mainFrame.NavigationService.Content;

            // Load test models
            SystemStateTracker.DefaultModel = Content.Load<Model>("Models/BeachBall");
            Ship = Content.Load<Model>("Models/Ship");
        }

        private Model LoadModel(string assetName)
        {
            Model newModel = Content.Load<Model>(assetName);
            return newModel;
        }

        private void UpdateViewAndProj()
        {
            SystemStateTracker.view = Matrix.CreateLookAt(cam.Pos, cam.getCameraLookAt(), Vector3.Up);

            SystemStateTracker.proj = Matrix.CreatePerspectiveFieldOfView(Camera3d.VIEWANGLE, _graphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio,
                                              Camera3d.NEARCLIP, Camera3d.FARCLIP);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(_mouseState.LeftButton == ButtonState.Pressed ? Color.Black : Color.CornflowerBlue);
            SystemStateTracker.world = Matrix.Identity;
            UpdateViewAndProj();
            // since we share the GraphicsDevice with all hosts, we need to save and reset the states
            // this has to be done because spriteBatch internally sets states and doesn't reset themselves, fucking over any 3D rendering (which happens in the DemoScene)

            var blend = GraphicsDevice.BlendState;
            var depth = GraphicsDevice.DepthStencilState;
            var raster = GraphicsDevice.RasterizerState;
            var sampler = GraphicsDevice.SamplerStates[0];

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Blue);

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

            // Draw all objects that are known 2.0
            foreach(EntikaInstance instance in _currentRegionPage.SelectedTimePoint.GetEntikaInstancesWithoutFloor())
            {
                drawEntikaInstance2(instance);
            }
            this._currentRegionPage.UpdateFillDetailView();

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
            basicEffect.World = SystemStateTracker.world;
            basicEffect.View = SystemStateTracker.view;
            basicEffect.Projection = SystemStateTracker.proj;
            basicEffect.VertexColorEnabled = true;
            var selectedTO = _currentRegionPage.RetrieveSelectedTangibleObjectFromListView();
            foreach (var shape in _currentRegionPage.SelectedTimePoint.TimePointSpecificFill.NarrativeShapes)
            {
                if (shape.Polygon != null)
                {
                    var result = HelperFunctions.GetMeshForPolygon(shape.Polygon);
                    // If shape is compatible with currently selected entika object that can be placed, use a different color
                    List<VertexPositionColor> drawableTriangles = new List<VertexPositionColor>();
                    if (selectedTO != null)
                    {
                        var query = from ShapeRelation in shape.Relations
                                    from toRelation in selectedTO.RelationshipsAsTarget
                                    where ShapeRelation.Source.Name.Equals(toRelation.Source.DefaultName)
                                    select ShapeRelation;
                        if(query.Any())
                        {
                            drawableTriangles = DrawingEngine.GetDrawableTriangles(result, Color.Green);
                        }
                        else
                        {
                            drawableTriangles = DrawingEngine.GetDrawableTriangles(result, Color.Red);
                        }
                    }
                    else
                    {
                        drawableTriangles = DrawingEngine.GetDrawableTriangles(result, Color.Red);
                    }
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

            basicEffect.World = SystemStateTracker.world;
            basicEffect.View = SystemStateTracker.view;
            basicEffect.Projection = SystemStateTracker.proj;
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
            Texture2D texture = Content.Load<Texture2D>("maps/couch_diff");
            Vector3 modelPosition = instance.Position;
            // Rotation should be in radians, rotates model to top down view
            float modelRotation = ConvertToRadians(90.0f);

            // Draw the model. A model can have multiple meshes, so loop.
            //foreach (ModelMesh mesh in instance.Model.Meshes)
            //{

            //    foreach (Effect effect in mesh.Effects)
            //    {
            //        // Matrix.CreateRotationX(modelRotation) * , add later
            //        effect.Parameters["WorldViewProjection"].SetValue(transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(modelPosition) * (view * proj));
            //        effect.Parameters["Texture"].SetValue(texture);
            //        if (_currentRegionPage.SelectedEntikaObject != null && _currentRegionPage.SelectedEntikaObject.Equals(instance))
            //        {
            //            effect.CurrentTechnique = effect.Techniques["TexturedSelected"];
            //        }
            //        else
            //        {
            //            effect.CurrentTechnique = effect.Techniques["Textured"];
            //        }
            //        mesh.Draw();
            //    }
            //}

            // FBX model draw:
            if (instance.Model == null)
            {
                instance.Model = SystemStateTracker.DefaultModel;
                instance.UpdateBoundingBoxAndShape(SystemStateTracker.world);
            }

            if (!instance.Equals(_currentRegionPage.SelectedEntikaObject))
            {
                foreach (ModelMesh mesh in SystemStateTracker.DefaultModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.AmbientLightColor = new Vector3(0, 0, 0);
                        effect.View = SystemStateTracker.view;
                        effect.World = Matrix.CreateTranslation(modelPosition) * SystemStateTracker.world;
                        effect.Projection = SystemStateTracker.proj;
                    }
                    mesh.Draw();
                }
            }
            else
            {
                foreach (ModelMesh mesh in SystemStateTracker.DefaultModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.AmbientLightColor = new Vector3(1.0f, 0, 0);
                        effect.View = SystemStateTracker.view;
                        effect.World = Matrix.CreateTranslation(modelPosition) * SystemStateTracker.world;
                        effect.Projection = SystemStateTracker.proj;
                    }
                    mesh.Draw();
                }
            }
        }

        public void drawEntikaInstance2 (EntikaInstance instance)
        {
            if (instance.Position == null)
                return;
            drawEntikaInstance(instance);
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
                    effect.View = SystemStateTracker.view;
                    effect.Projection = SystemStateTracker.proj;
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
                CurrentRegionFillingMode = RegionFillingModes.SuggestionMode;
            }
            else if (_keyboardState.IsKeyDown(Keys.LeftShift))
            {
                CurrentRegionFillingMode = RegionFillingModes.ObjectDragging;
            }
            else if(_keyboardState.IsKeyDown(Keys.P))
            {
                CurrentRegionFillingMode = RegionFillingModes.ObjectPlacement;
            }
            else
            {
                CurrentRegionFillingMode = RegionFillingModes.None;
            }

            // Handle object suggestion mode
            if (CurrentRegionFillingMode == RegionFillingModes.SuggestionMode)
            {
                if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
                {
                    NarrativeTimePoint ntp = ((DetailTimePointViewModel)_currentRegionPage.RegionDetailTimePointView.DataContext).NarrativeTimePoint;
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
                    var ei = new EntikaInstance(tangibleObjectName, position, SystemStateTracker.DefaultModel, SystemStateTracker.world);

                    // Add new target to each of the overlapping relations of the shape
                    var query = from ShapeRelation in DestinationShape.Relations
                                from toRelation in ei.TangibleObject.RelationshipsAsTarget
                                where ShapeRelation.Source.Name.Equals(toRelation.Source.DefaultName)
                                select ShapeRelation;
                    foreach (var relation in query)
                    {
                        relation.Targets.Add(ei);
                        ei.RelationshipsAsTarget.Add(relation);
                    } 

                    // Determine shapes for entika class instances
                    NarrativeTimePoint ntpRet = SolvingEngine.AddEntikaInstanceToTimePointBasic(ntp, ei, DestinationShape);
                    ((DetailTimePointViewModel)_currentRegionPage.RegionDetailTimePointView.DataContext).NarrativeTimePoint = ntpRet;
                }
            }

            // Handle object placement
            if (CurrentRegionFillingMode == RegionFillingModes.ObjectPlacement)
            {
                if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
                {
                    HandleObjectPlacement();
                }
            }

            // Handle object selection
            if (CurrentRegionFillingMode == RegionFillingModes.None)
            {
                if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
                {
                    HandleObjectSelection();
                }

                if (_currentRegionPage.SelectedEntikaObject != null && _keyboardState.IsKeyUp(Keys.Delete) && _previousKeyboardState.IsKeyDown(Keys.Delete))
                {
                    HandleObjectRemoval();
                }
            }
        }

        private void HandleObjectRemoval()
        {
            var ei = _currentRegionPage.SelectedEntikaObject;
            NarrativeTimePoint ntp = ((DetailTimePointViewModel)_currentRegionPage.RegionDetailTimePointView.DataContext).NarrativeTimePoint;
            SolvingEngine.RemoveEntikaInstanceFromTimePoint(ntp, ei);
            _currentRegionPage.DeselectObject();
        }

        private void HandleObjectPlacement()
        {
            // Retrieve selected Tangible Object
            var to = _currentRegionPage.RetrieveSelectedTangibleObjectFromListView();
            if (to == null)
            {
                _currentRegionPage.SetMessageBoxText("Please select an object from the list");
                return;
            }

            // Get mouse coordinates
            var mouseCoords = CalculateMouseHitOnSurface();

            // Check if mouse coordinates are inside a shape
            NarrativeTimePoint ntp = ((DetailTimePointViewModel)_currentRegionPage.RegionDetailTimePointView.DataContext).NarrativeTimePoint;
            NarrativeShape DestinationShape =  null;
            // Todo, figure out if place is correct (within one of the shapes that allows placement of this object)
            foreach (var shape in ntp.TimePointSpecificFill.NarrativeShapes)
            {
                if (shape.Polygon.Contains(new Vec2d(mouseCoords.X, mouseCoords.Y)))
                {
                    DestinationShape = ntp.TimePointSpecificFill.NarrativeShapes[0];
                }                
            }
            if (DestinationShape == null)
            {
                _currentRegionPage.SetMessageBoxText("Selected position outside of allowed shape");
                return;
            }
            // Create entika instance
            Model model = LoadModel(Path.GetFileNameWithoutExtension(to.DefaultName));
            var ei = new EntikaInstance(to.DefaultName, mouseCoords, SystemStateTracker.DefaultModel, SystemStateTracker.world);

            // Add new target to each of the overlapping relations of the shape and of the instance
            var query = from ShapeRelation in DestinationShape.Relations
                        from toRelation in ei.TangibleObject.RelationshipsAsTarget
                        where ShapeRelation.Source.Name.Equals(toRelation.Source.DefaultName)
                        select ShapeRelation;
            foreach (var relation in query)
            {
                relation.Targets.Add(ei);
                ei.RelationshipsAsTarget.Add(relation);
            }
            
            // Store changes
            NarrativeTimePoint ntpRet = SolvingEngine.AddEntikaInstanceToTimePointBasic(ntp, ei, DestinationShape);
            ((DetailTimePointViewModel)_currentRegionPage.RegionDetailTimePointView.DataContext).NarrativeTimePoint = ntpRet;
        }

        private void HandleObjectSelection()
        {
            Ray ray = CalculateMouseRay();
            foreach (EntikaInstance ieo in _currentRegionPage.SelectedTimePoint.TimePointSpecificFill.OtherObjectInstances)
            {
                // Calculate/retrieve boundingbox
                ieo.UpdateBoundingBoxAndShape(SystemStateTracker.world);

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

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, SystemStateTracker.proj, SystemStateTracker.view, SystemStateTracker.world);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, SystemStateTracker.proj, SystemStateTracker.view, SystemStateTracker.world);

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

                Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, SystemStateTracker.proj, SystemStateTracker.view, SystemStateTracker.world);
                Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, SystemStateTracker.proj, SystemStateTracker.view, SystemStateTracker.world);

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