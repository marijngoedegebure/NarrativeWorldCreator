using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using NarrativeWorlds;
using System;
using System.Collections.Generic;
using System.Windows;

namespace NarrativeWorldCreator
{
    public class GraphScene : WpfGame
    {
        #region Fields

        private IGraphicsDeviceService _graphicsDeviceManager;
        private WpfKeyboard _keyboard;
        private KeyboardState _keyboardState;

        private WpfMouse _mouse;
        private MouseState _mouseState;
        private MouseState _previousMouseState;

        private SpriteBatch _spriteBatch;

        private SpriteFont spriteFontCourierNew;

        private Camera2d cam = new Camera2d();

        public static int height;
        public static int width;

        #endregion

        #region Methods

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
            //drawModelExampleFunction();
            //drawSpriteExampleFunction(spriteBatch);
            drawGraph(spriteBatch, SystemStateTracker.NarrativeWorld.Graph);

            // this base.Draw call will draw "all" components (we only added one)
            // since said component will use a spritebatch to render we need to let it draw before we reset the GraphicsDevice
            // otherwise it will just alter the state again and fuck over all the other hosts
            base.Draw(time);

            GraphicsDevice.BlendState = blend;
            GraphicsDevice.DepthStencilState = depth;
            GraphicsDevice.RasterizerState = raster;
            GraphicsDevice.SamplerStates[0] = sampler;

        }

        private void drawGraph(SpriteBatch spriteBatch, Graph graph)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred,
                        null,
                        null,
                        null,
                        null,
                        null,
                        cam.get_transformation(_graphicsDeviceManager.GraphicsDevice, (float) this.ActualWidth, (float) this.ActualHeight));
            // Draw each node
            if (!SystemStateTracker.NarrativeWorld.Graph.nodeCoordinatesGenerated)
                return;
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            var graphPage = (GraphPage)mainWindow._mainFrame.NavigationService.Content;
            foreach (Node n in graph.getNodeList())
            {
                // check if node is selected node
                if (graphPage.selectedNode != null && graphPage.selectedNode.Equals(n))
                {
                    spriteBatch.Draw(graph.circleSelectedTexture, null, graph.NodeCollisionBoxes[n]);
                }
                else
                {
                    spriteBatch.Draw(graph.circleTexture, null, graph.NodeCollisionBoxes[n]);
                }
                // Draw the location name
                spriteBatch.DrawString(spriteFontCourierNew, n.getLocationName(), new Vector2(graph.NodeCollisionBoxes[n].X +
                    (graph.nodeWidth / 2), graph.NodeCollisionBoxes[n].Y + (graph.nodeHeight / 2)), Color.Black,
                    0, spriteFontCourierNew.MeasureString(n.getLocationName()) / 2, 1.0f, SpriteEffects.None, 0.5f);
            }
            // Draw each edge
            foreach (Edge e in graph.getEdgeList())
            {
                DrawLine(spriteBatch, //draw line
                    new Vector2(graph.NodeCollisionBoxes[e.from].X + (graph.nodeWidth / 2), graph.NodeCollisionBoxes[e.from].Y + (graph.nodeHeight / 2)), //start of line
                    new Vector2(graph.NodeCollisionBoxes[e.to].X + (graph.nodeWidth / 2), graph.NodeCollisionBoxes[e.to].Y + (graph.nodeHeight / 2)) //end of line
                );
            }
            spriteBatch.End();
        }

        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            sb.Draw(SystemStateTracker.NarrativeWorld.Graph.lineTexture,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                Color.Red, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);
        }

        protected override void Initialize()
        {
            base.Initialize();
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);
            cam.Pos = new Vector2(500.0f, 200.0f);

            SystemStateTracker.NarrativeWorld.Graph.circleTexture = Content.Load<Texture2D>("Sprites/Circle");
            SystemStateTracker.NarrativeWorld.Graph.circleSelectedTexture = Content.Load<Texture2D>("Sprites/Circle-selected");
            SystemStateTracker.NarrativeWorld.Graph.lineTexture = new Texture2D(_graphicsDeviceManager.GraphicsDevice, 1, 1);
            SystemStateTracker.NarrativeWorld.Graph.lineTexture.SetData<Color>(new Color[] { Color.Black });
            spriteFontCourierNew = Content.Load<SpriteFont>("Spritefonts/Courier New");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _keyboard = new WpfKeyboard(this);
            _mouse = new WpfMouse(this);
            _previousMouseState = _mouse.GetState();

            height = (int)this.ActualHeight - SystemStateTracker.NarrativeWorld.Graph.nodeHeight;
            width = (int)this.ActualWidth - SystemStateTracker.NarrativeWorld.Graph.nodeWidth;
        }

        private float elapsedSinceLastStep = 0f;
        private float intervalStepTime = 0.1f;

        protected override void Update(GameTime time)
        {
            _previousMouseState = _mouseState;
            _mouseState = _mouse.GetState();
            _keyboardState = _keyboard.GetState();
            Graph graph = SystemStateTracker.NarrativeWorld.Graph;

            float totalElapsed = (float)time.TotalGameTime.TotalSeconds;
            if (totalElapsed - elapsedSinceLastStep > intervalStepTime && graph.temperature > Graph.DefaultMinimumTemperature)
            {
                graph.stepForceDirectedGraph();
                elapsedSinceLastStep = totalElapsed;
            }
            // Convert positions and update collisionboxes
            foreach (Node n in graph.getNodeList())
            {
                float x = graph.NodePositions[n].X * Graph.energyToDrawConversion;
                float y = graph.NodePositions[n].Y * Graph.energyToDrawConversion;
                Rectangle collisionBox = new Rectangle((int)x, (int)y, graph.nodeHeight, graph.nodeWidth);
                graph.NodeCollisionBoxes[n] = collisionBox;
            }

            // Collision detection of mouse with one of the regions
            if (graph.NodeCollisionBoxes.Count == graph.getNodeList().Count && _previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
            {
                // Incorporate translation into collision
                Matrix inverseViewMatrix = Matrix.Invert(cam.get_transformation(_graphicsDeviceManager.GraphicsDevice, (float)this.ActualWidth, (float)this.ActualHeight));
                Node collisionNode = graph.checkCollisions(Vector2.Transform(_mouseState.Position.ToVector2(), inverseViewMatrix));
                var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                var graphPage = (GraphPage)mainWindow._mainFrame.NavigationService.Content;
                if (collisionNode != null)
                {
                    graphPage.selectedNode = collisionNode;
                    graphPage.fillDetailView(collisionNode);
                }
                else
                {
                    // If no collision, reset selectedNode and interface
                    graphPage.selected_region_detail_grid.Visibility = Visibility.Collapsed;
                    graphPage.selectedNode = null;
                }
            }
            cam.handleCamMovementKeyboardInput(_keyboardState);
            cam.handleCamMoovementMouseInput(_mouseState, _previousMouseState);
            base.Update(time);
        }
        #endregion
    }
}