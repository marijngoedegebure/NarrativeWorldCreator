using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NarrativeWorldCreator.Hosting;
using NarrativeWorldCreator.RegionGraph;
using NarrativeWorldCreator.RegionGraph.GraphDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private Dictionary<Node, Vector2> convertNodePositions( Dictionary<Node, Vector2> positions)
        {
            // Figure out min and max of X and Y
            List<Node> nodeList = GraphParser.graph.getNodeList();
            float minX = positions[nodeList[0]].X;
            float maxX = positions[nodeList[0]].X;
            float minY = positions[nodeList[0]].Y;
            float maxY = positions[nodeList[0]].Y;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (positions[nodeList[i]].X > maxX)
                    maxX = positions[nodeList[i]].X;
                if (positions[nodeList[i]].X < minX)
                    minX = positions[nodeList[i]].X;
                if (positions[nodeList[i]].Y > maxY)
                    maxY = positions[nodeList[i]].Y;
                if (positions[nodeList[i]].Y < minY)
                    minY = positions[nodeList[i]].Y;
            }
            // Normalize
            Dictionary<Node, Vector2> convertedPositions = new Dictionary<Node, Vector2>();
            foreach (Node n in nodeList)
            {
                convertedPositions[n] = new Vector2((positions[n].X - minX) / (maxX - minX), (positions[n].Y - minY) / (maxY - minY));
            }
            return convertedPositions;
        }

        private void drawGraph(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            int height = ProjectionGraphHost.DefaultHeight - GraphParser.nodeHeight;
            int width = ProjectionGraphHost.DefaultWidth - GraphParser.nodeWidth;
            // Draw each node
            if (!GraphParser.graph.nodeCoordinatesGenerated)
                return;
            Dictionary<Node, Vector2> convertedPositions = convertNodePositions(GraphParser.NodePositions);
            foreach (Node n in GraphParser.graph.getNodeList())
            {
                // Convert 0 to 1 float to screenposition (1920x1080)
                float x = convertedPositions[n].X * height;
                float y = convertedPositions[n].Y * height;
                spriteBatch.Draw(GraphParser.circleTexture, new Rectangle((int)x, (int)y, GraphParser.nodeHeight, GraphParser.nodeWidth), Color.White);
                // Draw the location name
                spriteBatch.DrawString(spriteFontCourierNew, n.getLocationName(), new Vector2(x + (GraphParser.nodeWidth / 2), y + (GraphParser.nodeHeight / 2)), Color.Black,
                    0, spriteFontCourierNew.MeasureString(n.getLocationName()) / 2, 1.0f, SpriteEffects.None, 0.5f);
            }
            // Draw each edge
            foreach (Edge e in GraphParser.graph.getEdgeList())
            {
                DrawLine(spriteBatch, //draw line
                    new Vector2(convertedPositions[e.from].X * height + (GraphParser.nodeWidth/2), convertedPositions[e.from].Y * height + (GraphParser.nodeHeight/2)), //start of line
                    new Vector2(convertedPositions[e.to].X * height + (GraphParser.nodeWidth / 2), convertedPositions[e.to].Y * height + (GraphParser.nodeHeight / 2)) //end of line
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
        }
    }
}
