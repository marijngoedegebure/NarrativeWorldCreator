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
    public class RegionScene : WpfGame
    {
        #region Fields

        public Matrix view;
        public Matrix world;
        public Matrix proj;

        protected IGraphicsDeviceService _graphicsDeviceManager;
        protected WpfKeyboard _keyboard;
        protected KeyboardState _keyboardState;
        protected WpfMouse _mouse;
        protected MouseState _mouseState;
        protected MouseState _previousMouseState;
        protected SpriteBatch _spriteBatch;

        protected Camera3d cam = new Camera3d();

        protected enum RegionFillingModes
        {
            None = 0,
            ObjectPlacement = 1,
            ObjectDragging = 2,
            ObjectDeletion = 3,
            SuggestionMode = 4
        }

        protected RegionFillingModes CurrentRegionFillingMode = RegionFillingModes.None;

        protected enum DrawingModes
        {
            UnderlyingRegion = 0,
            MinkowskiMinus = 1,
        }
        protected DrawingModes CurrentDrawingMode = DrawingModes.UnderlyingRegion;
        protected KeyboardState _previousKeyboardState;

        protected Point BoxSelectInitialCoords;
        protected Point BoxSelectCurrentCoords;

        protected EntikaInstance repositioningObject;
        protected EntikaInstance rotationObject;

        protected float LineThickness = 0.1f;

        protected MainWindow mainWindow;

        protected ModeBaseRegionPage _currentRegionPage;

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

            mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;

            this._currentRegionPage = (ModeBaseRegionPage)mainWindow._mainFrame.NavigationService.Content;

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

        protected Model LoadModel(string assetName)
        {
            Model newModel = Content.Load<Model>(assetName);
            return newModel;
        }

        protected void UpdateViewAndProj()
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

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Blue);
            this.drawTestRuler();

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
            //    this._currentRegionPage.MousePositionTest = new EntikaInstance("table", new Vector3(0, 0, 0), this.DefaultModel, this.world);
            //drawEntikaInstance(this._currentRegionPage.MousePositionTest);

            // Draw all objects that are known 2.0
            drawEntikaInstances();

            // this base.Draw call will draw "all" components
            base.Draw(time);

            GraphicsDevice.BlendState = blend;
            GraphicsDevice.DepthStencilState = depth;
            GraphicsDevice.RasterizerState = raster;
            GraphicsDevice.SamplerStates[0] = sampler;
        }

        protected virtual void drawEntikaInstances()
        {
            foreach (EntikaInstance instance in _currentRegionPage.SelectedTimePoint.Configuration.GetEntikaInstancesWithoutFloor())
            {
                drawEntikaInstance2(instance);
            }
        }

        protected void drawTestRuler()
        {
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);

            basicEffect.World = this.world;
            basicEffect.View = this.view;
            basicEffect.Projection = this.proj;
            basicEffect.VertexColorEnabled = true;

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            // Draws a square of 0.25 by 0.25 ever 10 units
            for (int i = -10; i <= 10; i+=1)
            {
                for (int j = -10; j <= 10; j+=1)
                {
                    Color c = Color.DarkGreen;
                    if (i == 0 && j == 0)
                    {
                        c = Color.DarkBlue;
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

        protected void drawFloorWireframe()
        {
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.World = this.world;
            basicEffect.View = this.view;
            basicEffect.Projection = this.proj;
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

        protected void drawRegionPolygon()
        {
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);

            basicEffect.World = this.world;
            basicEffect.View = this.view;
            basicEffect.Projection = this.proj;
            basicEffect.VertexColorEnabled = true;
            // Triangles should be defined clockwise
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            // Only draw triangles when there is 3 or more vertices
            var floorInstance = this._currentRegionPage.SelectedTimePoint.Configuration.InstancedObjects.Where(io => io.Name.Equals(Constants.Floor)).FirstOrDefault();
            if (floorInstance != null && floorInstance.Polygon.GetAllVertices().Count > 2)
            {
                var result = HelperFunctions.GetMeshForPolygon(floorInstance.Polygon);
                // If shape is compatible with currently selected entika object that can be placed, use a different color
                List<VertexPositionColor> drawableTriangles = new List<VertexPositionColor>();
                // List<VertexPositionColor> regionPoints = _currentRegionPage.selectedNode.RegionOutlinePoints;
                List<VertexPositionColor> regionPoints = new List<VertexPositionColor>();
                regionPoints = DrawingEngine.GetDrawableTriangles(result, Color.White);
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
            if (floorInstance != null && floorInstance.Polygon.GetAllVertices().Count > 2)
            {
                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                var result = HelperFunctions.GetMeshForPolygon(floorInstance.Polygon);
                // If shape is compatible with currently selected entika object that can be placed, use a different color
                List<VertexPositionColor> drawableTriangles = new List<VertexPositionColor>();
                // List<VertexPositionColor> regionPoints = _currentRegionPage.selectedNode.RegionOutlinePoints;
                List<VertexPositionColor> points = new List<VertexPositionColor>();
                points = DrawingEngine.GetDrawableTriangles(result, Color.White);

                // Create quads based off vertex points
                for (int i = 0; i < points.Count; i++)
                {
                    Color color = Color.DarkGray;
                    Quad quad = new Quad(points[i].Position, new Vector3(points[i].Position.X, points[i].Position.Y, 1), Vector3.Up, (float) (0.001 * cam._pos.Z), (float)(0.001 * cam._pos.Z), color);
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

        protected VertexPositionColor[] CreateLine(Vertex start, Vertex end, Color color)
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

        protected VertexPositionColor[] CreateLine(Vector3 start, Vector3 end, Color color)
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
                    effect.View = this.view;
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationZ(modelRotation) * Matrix.CreateTranslation(modelPosition) * this.world;
                    effect.Projection = this.proj;
                }
                mesh.Draw();
            }
            BasicEffect lineEffect = new BasicEffect(GraphicsDevice);
            lineEffect.LightingEnabled = false;
            lineEffect.TextureEnabled = false;
            lineEffect.VertexColorEnabled = true;
            var bbOffLimits = instance.BoundingBox;
            if (optionalAdjustedPosition.HasValue)
            {
                bbOffLimits = EntikaInstance.GetBoundingBox(SystemStateTracker.NarrativeWorld.ModelsForTangibleObjects[instance.TangibleObject], Matrix.CreateTranslation(modelPosition));
            }
            var bbbOffLimits = BoundingBoxBuffers.CreateBoundingBoxBuffers(bbOffLimits, GraphicsDevice);

            DrawBoundingBox(bbbOffLimits, lineEffect, GraphicsDevice, this.view, this.proj);
            // Draw clearance quads:
            foreach (var clearance in instance.Clearances)
            {
                Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                foreach (var point in clearance.GetAllVertices())
                {
                    Vector3 transformedPosition = Vector3.Transform(new Vector3((float)point.X, (float)point.Y, 0), Matrix.CreateRotationZ(modelRotation) * Matrix.CreateTranslation(modelPosition));

                    min = Vector3.Min(min, transformedPosition);
                    max = Vector3.Max(max, transformedPosition);
                }
                var bbClearance = new BoundingBox(min, max);
                var bbbClearance = BoundingBoxBuffers.CreateBoundingBoxBuffers(bbClearance, GraphicsDevice);
                DrawBoundingBox(bbbClearance, lineEffect, GraphicsDevice, this.view, this.proj);
            }
        }

        protected void DrawBoundingBox(BoundingBoxBuffers buffers, BasicEffect effect, GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            graphicsDevice.SetVertexBuffer(buffers.Vertices);
            graphicsDevice.Indices = buffers.Indices;

            effect.World = this.world;
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

        protected virtual void drawGPUResultInstance(GPUInstanceResult gpuResultInstance)
        {
            if (gpuResultInstance.entikaInstance.Name != Constants.Floor)
                drawEntikaInstance(gpuResultInstance.entikaInstance, gpuResultInstance.Position, gpuResultInstance.Rotation);
        }

        //public void drawModelExampleFunction()
        //{
        //    Model myModel = Content.Load<Model>("cylinder");
        //    Matrix[] transforms = new Matrix[myModel.Bones.Count];
        //    Vector3 modelPosition = Vector3.Zero;
        //    float modelRotation = 90.0f;
        //    myModel.CopyAbsoluteBoneTransformsTo(transforms);
        //    // Draw the model. A model can have multiple meshes, so loop.
        //    foreach (ModelMesh mesh in myModel.Meshes)
        //    {
        //        // This is where the mesh orientation is set, as well 
        //        // as our camera and projection.
        //        foreach (BasicEffect effect in mesh.Effects)
        //        {
        //            effect.EnableDefaultLighting();
        //            effect.World = transforms[mesh.ParentBone.Index] *
        //                Matrix.CreateRotationY(modelRotation)
        //                * Matrix.CreateTranslation(modelPosition);
        //            effect.View = this.view;
        //            effect.Projection = this.proj;
        //        }
        //        // Draw the mesh, using the effects set above.
        //        mesh.Draw();
        //    }
        //}


        protected override void Update(GameTime time)
        {
            _previousMouseState = _mouseState;
            _previousKeyboardState = _keyboardState;
            _mouseState = _mouse.GetState();
            _keyboardState = _keyboard.GetState();
            cam.handleCamMovementKeyboardInput(_keyboardState);
            // cam.handleCamMoovementMouseInput(_mouseState, _previousMouseState, _keyboardState);

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
            base.Update(time);
        }
        #endregion
    }
}