using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using NarrativeWorldCreator.RegionGraph;
using NarrativeWorldCreator.RegionGraph.GraphDataTypes;
using System;
using System.Collections.Generic;

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
        private SpriteBatch _spriteBatch;

        private SpriteFont spriteFontCourierNew;

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
            drawGraph(spriteBatch);

            // this base.Draw call will draw "all" components (we only added one)
            // since said component will use a spritebatch to render we need to let it draw before we reset the GraphicsDevice
            // otherwise it will just alter the state again and fuck over all the other hosts
            base.Draw(time);

            GraphicsDevice.BlendState = blend;
            GraphicsDevice.DepthStencilState = depth;
            GraphicsDevice.RasterizerState = raster;
            GraphicsDevice.SamplerStates[0] = sampler;

        }

        private void drawGraph(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            // Draw each node
            if (!GraphParser.graph.nodeCoordinatesGenerated)
                return;
            foreach (Node n in GraphParser.graph.getNodeList())
            {

                spriteBatch.Draw(GraphParser.circleTexture, GraphParser.NodeCollisionBoxes[n], Color.White);
                // Draw the location name
                spriteBatch.DrawString(spriteFontCourierNew, n.getLocationName(), new Vector2(GraphParser.NodeCollisionBoxes[n].X +
                    (GraphParser.nodeWidth / 2), GraphParser.NodeCollisionBoxes[n].Y + (GraphParser.nodeHeight / 2)), Color.Black,
                    0, spriteFontCourierNew.MeasureString(n.getLocationName()) / 2, 1.0f, SpriteEffects.None, 0.5f);
            }
            // Draw each edge
            foreach (Edge e in GraphParser.graph.getEdgeList())
            {
                DrawLine(spriteBatch, //draw line
                    new Vector2(GraphParser.NodeCollisionBoxes[e.from].X + (GraphParser.nodeWidth / 2), GraphParser.NodeCollisionBoxes[e.from].Y + (GraphParser.nodeHeight / 2)), //start of line
                    new Vector2(GraphParser.NodeCollisionBoxes[e.to].X + (GraphParser.nodeWidth / 2), GraphParser.NodeCollisionBoxes[e.to].Y + (GraphParser.nodeHeight / 2)) //end of line
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


            sb.Draw(GraphParser.lineTexture,
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

            GraphParser.circleTexture = Content.Load<Texture2D>("Sprites/Circle");
            GraphParser.lineTexture = new Texture2D(_graphicsDeviceManager.GraphicsDevice, 1, 1);
            GraphParser.lineTexture.SetData<Color>(new Color[] { Color.Black });
            spriteFontCourierNew = Content.Load<SpriteFont>("Spritefonts/Courier New");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _keyboard = new WpfKeyboard(this);
            _mouse = new WpfMouse(this);

            height = (int)this.ActualHeight - GraphParser.nodeHeight;
            width = (int)this.ActualWidth - GraphParser.nodeWidth;

            //Components.Add(new DrawMeComponent(this));
        }

        private float elapsedSinceLastStep = 0f;
        private float intervalStepTime = 0.1f;

        protected override void Update(GameTime time)
        {
            _mouseState = _mouse.GetState();
            _keyboardState = _keyboard.GetState();

            float totalElapsed = (float)time.TotalGameTime.TotalSeconds;
            if (totalElapsed - elapsedSinceLastStep > intervalStepTime && GraphParser.temperature > GraphParser.DefaultMinimumTemperature)
            {
                GraphParser.stepForceDirectedGraph();
                elapsedSinceLastStep = totalElapsed;
            }
            // Convert positions and update collisionboxes
            Dictionary<Node, Vector2> convertedPositions = GraphParser.convertNodePositions();
            // Convert 0 to 1 float to screenposition (1080x1080)
            height = (int)this.ActualHeight - GraphParser.nodeHeight;
            width = (int)this.ActualWidth - GraphParser.nodeWidth;
            foreach (Node n in GraphParser.graph.getNodeList())
            {
                float x = convertedPositions[n].X * height;
                float y = convertedPositions[n].Y * height;
                Rectangle collisionBox = new Rectangle((int)x, (int)y, GraphParser.nodeHeight, GraphParser.nodeWidth);
                GraphParser.NodeCollisionBoxes[n] = collisionBox;
            }

            // Check if mouse is inside node of graph
            var mouseState = Mouse.GetState();
            var mousePosition = new Point(mouseState.X, mouseState.Y);
            foreach (Node n in GraphParser.graph.getNodeList())
            {
                if (GraphParser.NodeCollisionBoxes.Count == GraphParser.graph.getNodeList().Count)
                {
                    // Check if the mouse position is inside the rectangle
                    if (mouseState.LeftButton == ButtonState.Released)
                    {
                        if (GraphParser.NodeCollisionBoxes[n].Contains(mousePosition))
                        {
                            continue;
                            // var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                            // mainWindow._mainFrame.NavigationService.Navigate(new RegionPage());
                        }
                    }
                }
            }
            base.Update(time);
        }

        #endregion
    }
}