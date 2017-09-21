using Common;
using Common.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using NarrativeWorldCreator.GraphicScenes.Primitives;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.Pages;
using NarrativeWorldCreator.Solvers;
using NarrativeWorldCreator.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TriangleNet.Data;

namespace NarrativeWorldCreator.GraphicScenes
{
    public class RegionCreationScene : WpfGame
    {
        #region Fields
        public Matrix view;
        public Matrix world;
        public Matrix proj;

        private IGraphicsDeviceService _graphicsDeviceManager;
        private WpfKeyboard _keyboard;
        private KeyboardState _keyboardState;
        private WpfMouse _mouse;
        private MouseState _mouseState;
        private MouseState _previousMouseState;
        private SpriteBatch _spriteBatch;

        private Camera3d cam = new Camera3d();

        private RegionCreationPage _currentPage;

        private int draggingVertexIndex;

        private enum RegionCreationModes
        {
            None = 0,
            VertexCreation = 1,
            VertexDragging = 2,
            VertexDeletion = 3,
        }

        private RegionCreationModes CurrentRegionCreationMode = RegionCreationModes.None;

        private enum DrawingModes
        {
            UnderlyingRegion = 0,
            MinkowskiMinus = 1,
        }
        private DrawingModes CurrentDrawingMode = DrawingModes.UnderlyingRegion;
        private KeyboardState _previousKeyboardState;

        private Point RegionCreationInitialCoords;
        private Point RegionCreationCurrentCoords;

        public float LineThickness = 0.1f;

        #endregion

        #region Methods
        protected override void Initialize()
        {
            base.Initialize();
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            cam.Pos = new Vector3(0.0f, 0.0f, 20.0f);

            _keyboard = new WpfKeyboard(this);
            _mouse = new WpfMouse(this);

            RasterizerState state = new RasterizerState();
            state.CullMode = CullMode.CullClockwiseFace;
            state.FillMode = FillMode.WireFrame;
            GraphicsDevice.RasterizerState = state;

            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            _currentPage = (RegionCreationPage)mainWindow._mainFrame.NavigationService.Content;

            // Load test models
            SystemStateTracker.DefaultModel = Content.Load<Model>("Models/beddresser1/beddresser1");
            SystemStateTracker.BoxSelectTexture = new Texture2D(GraphicsDevice, 1, 1);
            SystemStateTracker.BoxSelectTexture.SetData(new[] { new Color(1.0f, 1.0f, 1.0f, 0.2f) });

            SystemStateTracker.RegionCreationTexture = new Texture2D(GraphicsDevice, 1, 1);
            SystemStateTracker.RegionCreationTexture.SetData(new[] { new Color(1.0f, 1.0f, 1.0f, 0.2f) });

            // Load all models for the currently selected timepoint
            SystemStateTracker.NarrativeWorld.ModelsForTangibleObjects = new Dictionary<Semantics.Entities.TangibleObject, Model>();
            foreach (var to in SystemStateTracker.NarrativeWorld.AvailableTangibleObjects)
            {
                var modelPath = "";
                foreach (var att in to.Attributes)
                {
                    if (att.Node.DefaultName.Equals(Constants.ModelPath))
                    {
                        modelPath = att.Value.ToString();
                    }
                }
                modelPath = "Models/" + modelPath + "/" + modelPath;
                SystemStateTracker.NarrativeWorld.ModelsForTangibleObjects.Add(to, Content.Load<Model>(modelPath));
            }
        }

        private Model LoadModel(string assetName)
        {
            Model newModel = Content.Load<Model>(assetName);
            return newModel;
        }

        private void UpdateViewAndProj()
        {
            this.view = Matrix.CreateLookAt(cam.Pos, cam.getCameraLookAt(), Vector3.Up);

            this.proj = Matrix.CreatePerspectiveFieldOfView(Camera3d.VIEWANGLE, _graphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio,
                                              Camera3d.NEARCLIP, Camera3d.FARCLIP);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(_mouseState.LeftButton == ButtonState.Pressed ? Color.Black : Color.CornflowerBlue);
            this.world = Matrix.Identity;
            UpdateViewAndProj();
            // since we share the GraphicsDevice with all hosts, we need to save and reset the states
            // this has to be done because spriteBatch internally sets states and doesn't reset themselves, fucking over any 3D rendering (which happens in the DemoScene)

            var blend = GraphicsDevice.BlendState;
            var depth = GraphicsDevice.DepthStencilState;
            var raster = GraphicsDevice.RasterizerState;
            var sampler = GraphicsDevice.SamplerStates[0];

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Brown);
            
            drawRegionPolygon();
            this.drawTestRuler();

            // Draw all objects that are known 2.0, but only when there is no other configuration selected
            drawRegionCreationBox();

            // this base.Draw call will draw "all" components
            base.Draw(time);

            GraphicsDevice.BlendState = blend;
            GraphicsDevice.DepthStencilState = depth;
            GraphicsDevice.RasterizerState = raster;
            GraphicsDevice.SamplerStates[0] = sampler;
        }

        private void drawTestRuler()
        {
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);

            basicEffect.World = this.world;
            basicEffect.View = this.view;
            basicEffect.Projection = this.proj;
            basicEffect.VertexColorEnabled = true;

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            // calculate max and min of floor:
            double minX = Double.MaxValue;
            double minY = Double.MaxValue;
            double maxX = 0;
            double maxY = 0;
            if (_currentPage.selectedNode.FloorCreated)
            {
                foreach (var point in _currentPage.Floor.Polygon.GetAllVertices())
                {
                    minX = Math.Min(minX, point.X);
                    minY = Math.Min(minY, point.Y);
                    maxX = Math.Max(maxX, point.X);
                    maxY = Math.Max(maxY, point.Y);
                }
            }
            // Draws a square of 0.05 by 0.05 ever 1 unit
            for (int i = -10; i <= 10; i+=1)
            {
                for (int j = -10; j <= 10; j+=1)
                {
                    Color c = Color.White;
                    if (_currentPage.selectedNode.FloorCreated)
                    {
                        if (i > minX && i < maxX && j > minY && j < maxY)
                            c = Color.DarkGray;
                    }

                    var quad = new Quad(new Vector3(i, j, 0), new Vector3(0, 0, 1), Vector3.Up, 0.05f, 0.05f, c);
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

        private void drawRegionCreationBox()
        {
            if (!RegionCreationInitialCoords.Equals(new Point()))
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = false };
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                _spriteBatch.Draw(SystemStateTracker.RegionCreationTexture, new Rectangle(RegionCreationInitialCoords.X, RegionCreationInitialCoords.Y, RegionCreationCurrentCoords.X - RegionCreationInitialCoords.X, RegionCreationCurrentCoords.Y - RegionCreationInitialCoords.Y), Color.Gray);

                _spriteBatch.End();
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            }
        }

        public void drawRegionPolygon()
        {
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);

            basicEffect.World = this.world;
            basicEffect.View = this.view;
            basicEffect.Projection = this.proj;
            basicEffect.VertexColorEnabled = true;
            // Triangles should be defined clockwise
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            // Only draw triangles when there is 3 or more vertices
            var floorInstance = this._currentPage.Floor;
            if (floorInstance != null && floorInstance.Polygon.GetAllVertices().Count > 2)
            {
                var result = HelperFunctions.GetMeshForPolygon(floorInstance.Polygon);
                // If shape is compatible with currently selected entika object that can be placed, use a different color
                List<VertexPositionColor> drawableTriangles = new List<VertexPositionColor>();
                // List<VertexPositionColor> regionPoints = _currentRegionPage.selectedNode.RegionOutlinePoints;
                List<VertexPositionColor> regionPoints = new List<VertexPositionColor>();
                regionPoints = DrawingEngine.GetDrawableTriangles(result, Color.Black);
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
            //if (floorInstance != null && floorInstance.Polygon.GetAllVertices().Count > 2)
            //{
            //    GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            //    Color color;
            //    if (_currentRegionPage.CurrentMode == RegionPage.RegionPageMode.RegionCreation)
            //        color = Color.Purple;
            //    else
            //        color = Color.Black;
            //    var result = HelperFunctions.GetMeshForPolygon(floorInstance.Polygon);
            //    var triangles = result.Triangles.ToList();
            //    var vertices = result.Vertices.ToList();
            //    foreach (var triangle in triangles)
            //    {
            //        VertexPositionColor[] verticesLine;
            //        // draw line between p0 and p1
            //        verticesLine = CreateLine(vertices[triangle.P0], vertices[triangle.P1], color);
            //        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //        {
            //            pass.Apply();
            //            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            //                PrimitiveType.TriangleList,
            //                verticesLine,
            //                0,
            //                verticesLine.Length / 3);
            //        }

            //        // draw line between p1 and p2
            //        verticesLine = CreateLine(vertices[triangle.P1], vertices[triangle.P2], color);
            //        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //        {
            //            pass.Apply();
            //            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            //                PrimitiveType.TriangleList,
            //                verticesLine,
            //                0,
            //                verticesLine.Length / 3);
            //        }

            //        // draw line between p2 and p0
            //        verticesLine = CreateLine(vertices[triangle.P2], vertices[triangle.P0], color);
            //        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //        {
            //            pass.Apply();
            //            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            //                PrimitiveType.TriangleList,
            //                verticesLine,
            //                0,
            //                verticesLine.Length / 3);
            //        }
            //    }
            //}

            // Always draw the vertices
            //if (floorInstance != null && floorInstance.Polygon.GetAllVertices().Count > 2)
            //{
            //    GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            //    var result = HelperFunctions.GetMeshForPolygon(floorInstance.Polygon);
            //    // If shape is compatible with currently selected entika object that can be placed, use a different color
            //    List<VertexPositionColor> drawableTriangles = new List<VertexPositionColor>();
            //    // List<VertexPositionColor> regionPoints = _currentRegionPage.selectedNode.RegionOutlinePoints;
            //    List<VertexPositionColor> points = new List<VertexPositionColor>();
            //    points = DrawingEngine.GetDrawableTriangles(result, Color.White);

            //    // Create quads based off vertex points
            //    for (int i = 0; i < points.Count; i++)
            //    {

            //        Color color = Color.Black;
            //        color = Color.Red;
            //        if (draggingVertexIndex != -1 && draggingVertexIndex == i)
            //            color = Color.Yellow;
            //        Quad quad = new Quad(points[i].Position, new Vector3(points[i].Position.X, points[i].Position.Y, 1), Vector3.Up, (float) (0.001 * cam._pos.Z), (float)(0.001 * cam._pos.Z), color);
            //        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //        {
            //            pass.Apply();
            //            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            //                PrimitiveType.TriangleStrip,
            //                quad.Vertices,
            //                0,
            //                2);
            //        }
            //    }
            //}
        }

        private VertexPositionColor[] CreateLine(Vertex start, Vertex end, Color color)
        {
            VertexPositionColor[] line = new VertexPositionColor[6];
            // line triangle 1
            line[0] = new VertexPositionColor(new Vector3((float)start.X - this.LineThickness/2, (float)start.Y - this.LineThickness / 2, 0), color);
            line[1] = new VertexPositionColor(new Vector3((float)start.X + this.LineThickness / 2, (float)start.Y + this.LineThickness / 2, 0), color);
            line[2] = new VertexPositionColor(new Vector3((float)end.X + this.LineThickness / 2, (float)end.Y + this.LineThickness / 2, 0), color);

            // Line triangle 2
            line[3] = new VertexPositionColor(new Vector3((float)end.X + this.LineThickness / 2, (float)end.Y + this.LineThickness / 2, 0), color);
            line[4] = new VertexPositionColor(new Vector3((float)end.X - this.LineThickness / 2, (float)end.Y - 1, 0), color);
            line[5] = new VertexPositionColor(new Vector3((float)start.X - this.LineThickness / 2, (float)start.Y - this.LineThickness / 2, 0), color);
            return line;
        }

        private VertexPositionColor[] CreateLine(Vector3 start, Vector3 end, Color color)
        {
            VertexPositionColor[] line = new VertexPositionColor[6];
            // line triangle 1
            line[0] = new VertexPositionColor(new Vector3(start.X - this.LineThickness / 2, start.Y - this.LineThickness / 2, start.Z), color);
            line[1] = new VertexPositionColor(new Vector3(start.X + this.LineThickness / 2, start.Y + this.LineThickness / 2, start.Z), color);
            line[2] = new VertexPositionColor(new Vector3(end.X + this.LineThickness / 2, end.Y + this.LineThickness / 2, end.Z), color);

            // Line triangle 2
            line[3] = new VertexPositionColor(new Vector3(end.X + this.LineThickness / 2, end.Y + this.LineThickness / 2, end.Z), color);
            line[4] = new VertexPositionColor(new Vector3(end.X - this.LineThickness / 2, end.Y - this.LineThickness / 2, end.Z), color);
            line[5] = new VertexPositionColor(new Vector3(start.X - this.LineThickness / 2, start.Y - this.LineThickness / 2, start.Z), color);
            return line;
        }

        private float ConvertToRadians(float angle)
        {
            return (float) (Math.PI / 180f) * angle;
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
                    effect.View = this.view;
                    effect.Projection = this.proj;
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
            // Handle region creation input:
            HandleRegionCreation();
    
            var floorInstance = this._currentPage.Floor;
            if (floorInstance != null && floorInstance.Polygon.GetAllVertices().Count > 2)
            {
                _currentPage.selectedNode.FloorCreated = true;
            }

            base.Update(time);
        }

        private Vector3 CalculateHitOnSurface(Point p, Microsoft.Xna.Framework.Plane plane)
        {
            Ray ray = CalculateRay(p);
            float? distance = ray.Intersects(plane);
            return ray.Position + ray.Direction * distance.Value;
        }

        private Vector3 CalculateMouseHitOnSurface()
        {
            return CalculateHitOnSurface(_mouseState.Position, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
        }

        private Ray CalculateRay(Point p)
        {
            Vector3 nearsource = new Vector3((float)p.X, (float)p.Y, 0f);
            Vector3 farsource = new Vector3((float)p.X, (float)p.Y, 1f);

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, this.proj, this.view, this.world);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, this.proj, this.view, this.world);

            // Create ray using far and near point
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }

        private Ray CalculateMouseRay()
        {
            return CalculateRay(_mouseState.Position);
        }

        private int CalculateCollisionQuad()
        {
            Ray ray = CalculateMouseRay();
            var floorInstance = this._currentPage.Floor;
            var result = HelperFunctions.GetMeshForPolygon(floorInstance.Polygon);    
            List<VertexPositionColor> points = new List<VertexPositionColor>();
            points = DrawingEngine.GetDrawableTriangles(result, Color.White);
            for (int i = 0; i < points.Count; i++)
            {
                Quad quad = new Quad(points[i].Position, new Vector3(points[i].Position.X, points[i].Position.Y, 1), Vector3.Up, 1, 1, Color.Red);
                BoundingBox box = new BoundingBox(new Vector3(points[i].Position.X - 1, points[i].Position.Y - 1, points[i].Position.Z), new Vector3(points[i].Position.X + 1, points[i].Position.Y + 1, points[i].Position.Z));
                float? distance = ray.Intersects(box);
                if (distance != null)
                    return i;
            }
            return -1;
        }

        private void HandleRegionCreation()
        {
            HandleRegionBoxCreation();
        }

        private void HandleRegionBoxCreation()
        {
            if (_previousMouseState.LeftButton == ButtonState.Released && _mouseState.LeftButton == ButtonState.Pressed)
            {
                // Initialize box
                RegionCreationInitialCoords = _mouseState.Position;
                RegionCreationCurrentCoords = _mouseState.Position;
            }
            if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Pressed)
            {
                // reposition box to draw
                RegionCreationCurrentCoords = _mouseState.Position;
            }
            if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
            {
                var initialCoordsHit = CalculateHitOnSurface(RegionCreationInitialCoords, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
                var currentCoordsHit = CalculateHitOnSurface(RegionCreationCurrentCoords, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));

                Vector3 bottomLeft = new Vector3(0, 0, 0);
                Vector3 topRight = new Vector3(0, 0, 0);
                // Determine topLeft and bottomRight of rectangle
                if (initialCoordsHit.X < currentCoordsHit.X)
                {
                    bottomLeft.X = initialCoordsHit.X;
                    topRight.X = currentCoordsHit.X;
                }
                else
                {
                    bottomLeft.X = currentCoordsHit.X;
                    topRight.X = initialCoordsHit.X;
                }

                if (initialCoordsHit.Y < currentCoordsHit.Y)
                {
                    bottomLeft.Y = initialCoordsHit.Y;
                    topRight.Y = currentCoordsHit.Y;
                }
                else
                {
                    bottomLeft.Y = currentCoordsHit.Y;
                    topRight.Y = initialCoordsHit.Y;
                }
                var selectionBoxBB = new BoundingBox(bottomLeft, topRight);
                var vertices = new List<Vec2>();
                // Add each corner to region
                foreach (var corner in selectionBoxBB.GetCorners())
                {
                    vertices.Add(new Vec2(corner.X, corner.Y));
                }
                this._currentPage.Floor.Polygon = new Polygon(vertices);
                this._currentPage.Floor.UpdateBoundingBoxAndShape();

                // Reset box
                RegionCreationCurrentCoords = new Point();
                RegionCreationInitialCoords = new Point();
            }
        }

        #endregion
    }
}