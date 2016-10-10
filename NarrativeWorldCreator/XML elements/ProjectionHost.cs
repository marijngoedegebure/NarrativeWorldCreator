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
    public class ProjectionHost : D3D11Host
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
            base.GraphicsDeviceManager.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Purple);
            SpriteBatch spriteBatch = new SpriteBatch(base.GraphicsDeviceManager.GraphicsDevice);
            //drawModelExampleFunction();
            //drawSpriteExampleFunction(spriteBatch);
            drawGraph(spriteBatch);
        }

        private void drawGraph(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            int height = ProjectionHost.DefaultHeight - GraphParser.nodeHeight;
            int width = ProjectionHost.DefaultWidth - GraphParser.nodeWidth;
            // Draw each node
            if (!GraphParser.graph.nodeCoordinatesGenerated)
                return;
            foreach (Node n in GraphParser.graph.getNodeList())
            {
                // Convert 0 to 1 float to screenposition (1920x1080)
                float x = GraphParser.NodePositions[n].X * width;
                float y = GraphParser.NodePositions[n].Y * height;
                spriteBatch.Draw(GraphParser.circleTexture, new Rectangle((int)x, (int)y, GraphParser.nodeHeight, GraphParser.nodeWidth), Color.White);
                // Draw the location name
                spriteBatch.DrawString(spriteFontCourierNew, n.getLocationName(), new Vector2(x + (GraphParser.nodeWidth / 2), y + (GraphParser.nodeHeight / 2)), Color.Black,
                    0, spriteFontCourierNew.MeasureString(n.getLocationName()) / 2, 1.0f, SpriteEffects.None, 0.5f);
            }
            // Draw each edge
            foreach (Edge e in GraphParser.graph.getEdgeList())
            {
                DrawLine(spriteBatch, //draw line
                    new Vector2(GraphParser.NodePositions[e.from].X * width + (GraphParser.nodeWidth/2), GraphParser.NodePositions[e.from].Y * height + (GraphParser.nodeHeight/2)), //start of line
                    new Vector2(GraphParser.NodePositions[e.to].X * width + (GraphParser.nodeWidth / 2), GraphParser.NodePositions[e.to].Y * height + (GraphParser.nodeHeight / 2)) //end of line
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

        public void drawModelExampleFunction()
        {
            Model myModel = Content.Load<Model>("cylinder");
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            Vector3 modelPosition = Vector3.Zero;
            float modelRotation = 0.0f;
            Vector3 cameraPosition = new Vector3(0.0f, 50.0f, 5000.0f);
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
                    effect.View = Matrix.CreateLookAt(cameraPosition,
                        Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), base.GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio,
                        1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        public void drawSpriteExampleFunction(SpriteBatch spriteBatch)
        {
            Texture2D circle = Content.Load<Texture2D>("Sprites/Circle");
            spriteBatch.Begin();

            spriteBatch.Draw(circle, new Rectangle(0, 0, 800, 480), Color.White);

            spriteBatch.End();
        }

        public override void Update(TimeSpan gameTime)
        {
        }
    }
}
