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

        private Point BoxSelectInitialCoords;
        private Point BoxSelectCurrentCoords;

        private Point RegionCreationInitialCoords;
        private Point RegionCreationCurrentCoords;

        private EntikaInstance repositioningObject;

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
                drawFloorWireframe();
            }
            if (CurrentDrawingMode == DrawingModes.UnderlyingRegion)
            {
                drawRegionPolygon();
            }

            //if (this._currentRegionPage.InstanceOfObjectToAdd != null && this._currentRegionPage.CurrentAdditionMode == RegionPage.AdditionMode.Placement)
            //{
            //    drawEntikaInstance(this._currentRegionPage.InstanceOfObjectToAdd, null);
            //}
            //if (this._currentRegionPage.MousePositionTest == null)
            //    this._currentRegionPage.MousePositionTest = new EntikaInstance("table", new Vector3(0, 0, 0), SystemStateTracker.DefaultModel, SystemStateTracker.world);
            //drawEntikaInstance(this._currentRegionPage.MousePositionTest);

            // Draw all objects that are known 2.0, but only when there is no other configuration selected
            if (_currentRegionPage.SelectedGPUConfigurationResult == -1)
            {
                foreach (EntikaInstance instance in _currentRegionPage.SelectedTimePoint.Configuration.GetEntikaInstancesWithoutFloor())
                {
                    drawEntikaInstance2(instance);
                }
            }
            else
            {
                // Draw each entika instance in GeneratedConfigurations
                foreach (var gpuResultInstance in _currentRegionPage.GeneratedConfigurations[_currentRegionPage.SelectedGPUConfigurationResult].Instances)
                {
                    drawGPUResultInstance(gpuResultInstance);
                }
            }
            if (this._currentRegionPage.CurrentFillingMode == RegionPage.FillingMode.Placement)
            {
                drawCentroidAndFocalPoint();
            }
            this.drawTestRuler();

            drawBoxSelect();

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

            basicEffect.World = SystemStateTracker.world;
            basicEffect.View = SystemStateTracker.view;
            basicEffect.Projection = SystemStateTracker.proj;
            basicEffect.VertexColorEnabled = true;
            
            // Draws a square of 0.25 by 0.25 ever 10 units
            for (int i = -40; i <= 40; i+=10)
            {
                for (int j = -40; j <= 40; j+=10)
                {
                    Color c = Color.DarkGreen;
                    if (i == 0 && j == 0)
                    {
                        c = Color.DarkBlue;
                    }
                    var quad = new Quad(new Vector3(i, j, 0), new Vector3(0, 0, 1), Vector3.Up, 0.25f, 0.25f, c);
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

        private void drawCentroidAndFocalPoint()
        {
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);

            basicEffect.World = SystemStateTracker.world;
            basicEffect.View = SystemStateTracker.view;
            basicEffect.Projection = SystemStateTracker.proj;
            basicEffect.VertexColorEnabled = true;
            // When in placement mode, visualize focal and centroid
            Vector3 centroid = new Vector3((float)SystemStateTracker.centroidX, (float)SystemStateTracker.centroidY, 0);

            // Create square based on centroid coordinates
            Quad quad = new Quad(centroid, new Vector3(centroid.X, centroid.Y, 1), Vector3.Up, 1, 1, Color.Blue);

            // Draw centroid
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleStrip,
                    quad.Vertices,
                    0,
                    2);
            }

            // Draw focal
            Vector3 focalPoint = new Vector3((float)SystemStateTracker.focalX, (float)SystemStateTracker.focalY, 0);
            Vector3 focalRotation = new Vector3(0.0f, (float)SystemStateTracker.focalRot, 0.0f);

            // Draw rotation indicator
            var rotMatrix = Matrix.CreateRotationZ(focalRotation.Y);
            var length = 3.0f;

            VertexPositionColor[] vertices = new VertexPositionColor[3];
            vertices[0] = new VertexPositionColor(Vector3.Transform(new Vector3(0, length, 0), rotMatrix) + focalPoint, Color.DarkRed);
            vertices[1] = new VertexPositionColor(Vector3.Transform(new Vector3(length / 2, 0, 0), rotMatrix) + focalPoint, Color.DarkRed);
            vertices[2] = new VertexPositionColor(Vector3.Transform(new Vector3(-length / 2, 0, 0), rotMatrix) + focalPoint, Color.DarkRed);
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    vertices,
                    0,
                    1);
            }

            // Draw basic point
            Vector3 focalLinePointEnd = new Vector3(focalPoint.X + 2.0f, focalPoint.Y + 2.0f, 0);
            focalLinePointEnd = Vector3.Transform(focalLinePointEnd, Matrix.CreateRotationY(focalRotation.Y));

            Quad quad2 = new Quad(focalPoint, new Vector3(focalPoint.X, focalPoint.Y, 1), Vector3.Up, 1, 1, Color.Red);
            var temp = CreateLine(focalPoint, focalLinePointEnd, Color.Red);
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleStrip,
                    quad2.Vertices,
                    0,
                    2);
            }
        }

        private void drawBoxSelect()
        {
            if (!BoxSelectInitialCoords.Equals(new Point()))
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = false };
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                _spriteBatch.Draw(SystemStateTracker.BoxSelectTexture, new Rectangle(BoxSelectInitialCoords.X, BoxSelectInitialCoords.Y, BoxSelectCurrentCoords.X - BoxSelectInitialCoords.X, BoxSelectCurrentCoords.Y - BoxSelectInitialCoords.Y), Color.White * 0.5f);

                _spriteBatch.End();
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            }
        }

        private void drawRegionCreationBox()
        {
            if (!RegionCreationInitialCoords.Equals(new Point()))
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = false };
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                _spriteBatch.Draw(SystemStateTracker.RegionCreationTexture, new Rectangle(RegionCreationInitialCoords.X, RegionCreationInitialCoords.Y, RegionCreationCurrentCoords.X - RegionCreationInitialCoords.X, RegionCreationCurrentCoords.Y - RegionCreationInitialCoords.Y), Color.White * 1.0f);

                _spriteBatch.End();
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            }
        }

        private void drawFloorWireframe()
        {
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.World = SystemStateTracker.world;
            basicEffect.View = SystemStateTracker.view;
            basicEffect.Projection = SystemStateTracker.proj;
            basicEffect.VertexColorEnabled = true;
            var floorInstance = this._currentRegionPage.SelectedTimePoint.Configuration.InstancedObjects.Where(io => io.Name.Equals(Constants.Floor)).FirstOrDefault();
            if (floorInstance != null)
            {
                var result = HelperFunctions.GetMeshForPolygon(floorInstance.Polygon);
                // If shape is compatible with currently selected entika object that can be placed, use a different color
                List<VertexPositionColor> drawableTriangles = new List<VertexPositionColor>();
                drawableTriangles = DrawingEngine.GetDrawableTriangles(result, Color.Red);
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

        public void drawEntikaInstance(EntikaInstance instance, Vector3? optionalAdjustedPosition, Vector3? optionalAdjustedRotation)
        {
            Vector3 modelPosition = instance.Position;
            if (optionalAdjustedPosition.HasValue)
            {
                modelPosition = optionalAdjustedPosition.GetValueOrDefault();
            }
            
            // Rotation should be in radians, rotates model to top down view
            float modelRotation = instance.Rotation.Y;
            if (optionalAdjustedRotation.HasValue)
            {
                modelRotation = optionalAdjustedRotation.GetValueOrDefault().Y;
            }

            // FBX model draw:
            var model = SystemStateTracker.NarrativeWorld.ModelsForTangibleObjects[instance.TangibleObject];
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    if (!_currentRegionPage.SelectedEntikaInstances.Contains(instance))
                    {
                        if (instance.Frozen)
                            effect.AmbientLightColor = new Vector3(0, 1.0f, 0);
                        else
                            effect.AmbientLightColor = new Vector3(0, 0, 0);
                    }
                    else
                    {
                        if (instance.Frozen)
                            effect.AmbientLightColor = new Vector3(1.0f, 1.0f, 0);
                        else
                            effect.AmbientLightColor = new Vector3(1.0f, 0, 0);
                    }
                    effect.View = SystemStateTracker.view;
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationZ(modelRotation) * Matrix.CreateTranslation(modelPosition) * SystemStateTracker.world;
                    effect.Projection = SystemStateTracker.proj;
                }
                mesh.Draw();
            }
            BasicEffect lineEffect = new BasicEffect(GraphicsDevice);
            lineEffect.LightingEnabled = false;
            lineEffect.TextureEnabled = false;
            lineEffect.VertexColorEnabled = true;
            var bbb = BoundingBoxBuffers.CreateBoundingBoxBuffers(instance.BoundingBox, GraphicsDevice);

            DrawBoundingBox(bbb, lineEffect, GraphicsDevice, SystemStateTracker.view, SystemStateTracker.proj, modelPosition);
            // Draw clearance quads:
            foreach (var clearance in instance.Clearances)
            {
                Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                foreach (var point in clearance.GetAllVertices())
                {
                    Vector3 transformedPosition = new Vector3((float)point.X, (float)point.Y, 0);

                    min = Vector3.Min(min, transformedPosition);
                    max = Vector3.Max(max, transformedPosition);
                }
                var bb = new BoundingBox(min, max);
                var bbb2 = BoundingBoxBuffers.CreateBoundingBoxBuffers(bb, GraphicsDevice);
                DrawBoundingBox(bbb2, lineEffect, GraphicsDevice, SystemStateTracker.view, SystemStateTracker.proj, modelPosition);
            }
        }

        private void DrawBoundingBox(BoundingBoxBuffers buffers, BasicEffect effect, GraphicsDevice graphicsDevice, Matrix view, Matrix projection, Vector3 position)
        {
            graphicsDevice.SetVertexBuffer(buffers.Vertices);
            graphicsDevice.Indices = buffers.Indices;

            effect.World = Matrix.Identity * Matrix.CreateTranslation(position);
            effect.View = view;
            effect.Projection = projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0,
                    buffers.VertexCount, 0, buffers.PrimitiveCount);
            }
        }

        public void drawEntikaInstance2 (EntikaInstance instance)
        {
            if (instance.Position == null)
                return;
            drawEntikaInstance(instance, null, null);
        }

        private void drawGPUResultInstance(GPUInstanceResult gpuResultInstance)
        {
            if (gpuResultInstance.entikaInstance.Name != Constants.Floor)
                drawEntikaInstance(gpuResultInstance.entikaInstance, gpuResultInstance.Position, gpuResultInstance.Rotation);
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
            if (this._currentRegionPage.MousePositionTest != null)
                this._currentRegionPage.MousePositionTest.Position = CalculateMouseHitOnSurface();
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
            // Handle object selection
            if (CurrentRegionFillingMode == RegionFillingModes.None)
            {
                if (_keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    if (_previousMouseState.LeftButton == ButtonState.Released && _mouseState.LeftButton == ButtonState.Pressed)
                    {
                        // Initialize box
                        BoxSelectInitialCoords = _mouseState.Position;
                        BoxSelectCurrentCoords = _mouseState.Position;
                    }
                    if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Pressed)
                    {
                        // reposition box to draw
                        BoxSelectCurrentCoords = _mouseState.Position;
                    }
                    if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
                    {
                        var initialCoordsHit = CalculateHitOnSurface(BoxSelectInitialCoords, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
                        var currentCoordsHit = CalculateHitOnSurface(BoxSelectCurrentCoords, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));

                        Vector3 bottomLeft = new Vector3(0, 0, -20.0f);
                        Vector3 topRight = new Vector3(0, 0, 200f);
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

                        foreach (EntikaInstance ieo in _currentRegionPage.SelectedTimePoint.Configuration.GetEntikaInstancesWithoutFloor())
                        {
                            // Create translated boundingbox
                            var min = ieo.BoundingBox.Min;
                            var max = ieo.BoundingBox.Max;

                            var bb = new BoundingBox(Vector3.Transform(min, Matrix.CreateTranslation(ieo.Position)), Vector3.Transform(max, Matrix.CreateTranslation(ieo.Position)));
                            var containmentType = selectionBoxBB.Contains(bb);
                            if (!(containmentType == ContainmentType.Disjoint))
                                this._currentRegionPage.ChangeSelectedObject(ieo);
                        }
                        this._currentRegionPage.RefreshSelectedObjectView();

                        // Figure out selection of objects inside box
                        BoxSelectCurrentCoords = new Point();
                        BoxSelectInitialCoords = new Point();
                    }
                }
                else if (_keyboardState.IsKeyDown(Keys.LeftControl))
                {
                    if (_previousMouseState.LeftButton == ButtonState.Released && _mouseState.LeftButton == ButtonState.Pressed)
                    {
                        // Initialize box
                        Ray ray = CalculateMouseRay();
                        foreach (EntikaInstance ieo in _currentRegionPage.SelectedTimePoint.Configuration.GetEntikaInstancesWithoutFloor())
                        {
                            // Create translated boundingbox
                            var min = ieo.BoundingBox.Min;
                            var max = ieo.BoundingBox.Max;

                            var bb = new BoundingBox(Vector3.Transform(min, Matrix.CreateTranslation(ieo.Position)), Vector3.Transform(max, Matrix.CreateTranslation(ieo.Position)));

                            // Intersect ray with bounding box, if distance then select model
                            float? distance = ray.Intersects(bb);
                            if (distance != null)
                            {
                                repositioningObject = ieo;
                                var hit = CalculateHitOnSurface(_mouseState.Position, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), repositioningObject.Position.Z));
                                repositioningObject.Position = hit;
                            }
                        }
                    }
                    if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Pressed && repositioningObject !=null)
                    {
                        // reposition box to draw
                        var hit = CalculateHitOnSurface(_mouseState.Position, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), repositioningObject.Position.Z));
                        repositioningObject.Position = hit;
                    }
                    if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released && repositioningObject != null)
                    {
                        repositioningObject = null;
                    }
                }
                else
                {
                    if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
                    {
                        Ray ray = CalculateMouseRay();
                        foreach (EntikaInstance ieo in _currentRegionPage.SelectedTimePoint.Configuration.GetEntikaInstancesWithoutFloor())
                        {
                            // Create translated boundingbox
                            var min = ieo.BoundingBox.Min;
                            var max = ieo.BoundingBox.Max;

                            var bb = new BoundingBox(Vector3.Transform(min, Matrix.CreateTranslation(ieo.Position)), Vector3.Transform(max, Matrix.CreateTranslation(ieo.Position)));

                            // Intersect ray with bounding box, if distance then select model
                            float? distance = ray.Intersects(bb);
                            if (distance != null)
                            {
                                _currentRegionPage.ChangeSelectedObject(ieo);
                                return;
                            }
                        }
                    }
                }
                    //if (_currentRegionPage.SelectedEntikaInstance != null && _keyboardState.IsKeyUp(Keys.Delete) && _previousKeyboardState.IsKeyDown(Keys.Delete))
                    //{
                    //}
            }
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

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, SystemStateTracker.proj, SystemStateTracker.view, SystemStateTracker.world);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, SystemStateTracker.proj, SystemStateTracker.view, SystemStateTracker.world);

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
            if (_keyboardState.IsKeyDown(Keys.LeftShift))
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
                // HandleVertexCreation();
                HandleRegionBoxCreation();
            }
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
                _currentRegionPage.selectedNode.Shape = new Shape(vertices);
                _currentRegionPage.selectedNode.triangulatePolygon();
                _currentRegionPage.SelectedTimePoint.SetBaseShape(_currentRegionPage.selectedNode);

                // Reset box
                RegionCreationCurrentCoords = new Point();
                RegionCreationInitialCoords = new Point();
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