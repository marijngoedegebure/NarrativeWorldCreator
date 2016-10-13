using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NarrativeWorldCreator.Hosting;
using NarrativeWorldCreator.Pages;
using NarrativeWorldCreator.RegionGraph;
using NarrativeWorldCreator.RegionGraph.GraphDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace NarrativeWorldCreator
{
    public class ProjectionGraphHost : D3D11Host
    {
        SpriteFont spriteFontCourierNew;

        public override void Initialize()
        {
        }

        public override void Load()
        {
            GraphParser.circleTexture = Content.Load<Texture2D>("Sprites/Circle");
            GraphParser.lineTexture = new Texture2D(base.GraphicsDeviceManager.GraphicsDevice, 1, 1);
            GraphParser.lineTexture.SetData<Color>( new Color[] { Color.Black });
            spriteFontCourierNew = Content.Load<SpriteFont>("Spritefonts/Courier New");
        }

        public override void Unload()
        {
        }

        public override void Draw(TimeSpan gameTime)
        {
            base.GraphicsDeviceManager.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Blue);
            SpriteBatch spriteBatch = new SpriteBatch(base.GraphicsDeviceManager.GraphicsDevice);
            //drawModelExampleFunction();
            //drawSpriteExampleFunction(spriteBatch);
            drawGraph(spriteBatch);
        }

        private void drawGraph(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            int height = ProjectionGraphHost.DefaultHeight - GraphParser.nodeHeight;
            int width = ProjectionGraphHost.DefaultWidth - GraphParser.nodeWidth;
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
                    new Vector2(GraphParser.NodeCollisionBoxes[e.from].X * height + (GraphParser.nodeWidth/2), GraphParser.NodeCollisionBoxes[e.from].Y * height + (GraphParser.nodeHeight/2)), //start of line
                    new Vector2(GraphParser.NodeCollisionBoxes[e.to].X * height + (GraphParser.nodeWidth / 2), GraphParser.NodeCollisionBoxes[e.to].Y * height + (GraphParser.nodeHeight / 2)) //end of line
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

        public void drawSpriteExampleFunction(SpriteBatch spriteBatch)
        {
            Texture2D circle = Content.Load<Texture2D>("Sprites/Circle");
            spriteBatch.Begin();

            spriteBatch.Draw(circle, new Rectangle(0, 0, 800, 480), Color.White);

            spriteBatch.End();
        }

        private float elapsedSinceLastStep = 0f;
        private float intervalStepTime = 0.1f;

        public override void Update(TimeSpan gameTime)
        {
            float totalElapsed = (float)gameTime.TotalSeconds;
            if (totalElapsed - elapsedSinceLastStep > intervalStepTime && GraphParser.temperature > GraphParser.DefaultMinimumTemperature)
            {
                GraphParser.stepForceDirectedGraph();
                elapsedSinceLastStep = totalElapsed;
            }
            // Convert positions and update collisionboxes
            Dictionary<Node, Vector2> convertedPositions = GraphParser.convertNodePositions();
            // Convert 0 to 1 float to screenposition (1080x1080)
            int height = ProjectionGraphHost.DefaultHeight - GraphParser.nodeHeight;
            int width = ProjectionGraphHost.DefaultWidth - GraphParser.nodeWidth;
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
        }
    }
}
