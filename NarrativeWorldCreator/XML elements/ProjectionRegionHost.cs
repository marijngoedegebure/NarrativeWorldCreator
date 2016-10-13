//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using NarrativeWorldCreator.Hosting;
//using NarrativeWorldCreator.RegionGraph;
//using NarrativeWorldCreator.RegionGraph.GraphDataTypes;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NarrativeWorldCreator
//{
//    public class ProjectionRegionHost : D3D11Host
//    {
//        SpriteFont spriteFontCourierNew;

//        public override void Initialize()
//        {
//        }

//        public override void Load()
//        {
//        }

//        public override void Unload()
//        {
//        }

//        public override void Draw(TimeSpan gameTime)
//        {
//            base.GraphicsDeviceManager.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Blue);
//            drawModelExampleFunction();
//        }

//        public void drawModelExampleFunction()
//        {
//            Model myModel = Content.Load<Model>("cylinder");
//            Matrix[] transforms = new Matrix[myModel.Bones.Count];
//            Vector3 modelPosition = Vector3.Zero;
//            float modelRotation = 0.0f;
//            Vector3 cameraPosition = new Vector3(0.0f, 50.0f, 5000.0f);
//            myModel.CopyAbsoluteBoneTransformsTo(transforms);

//            // Draw the model. A model can have multiple meshes, so loop.
//            foreach (ModelMesh mesh in myModel.Meshes)
//            {
//                // This is where the mesh orientation is set, as well 
//                // as our camera and projection.
//                foreach (BasicEffect effect in mesh.Effects)
//                {
//                    effect.EnableDefaultLighting();
//                    effect.World = transforms[mesh.ParentBone.Index] *
//                        Matrix.CreateRotationY(modelRotation)
//                        * Matrix.CreateTranslation(modelPosition);
//                    effect.View = Matrix.CreateLookAt(cameraPosition,
//                        Vector3.Zero, Vector3.Up);
//                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
//                        MathHelper.ToRadians(45.0f), base.GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio,
//                        1.0f, 10000.0f);
//                }
//                // Draw the mesh, using the effects set above.
//                mesh.Draw();
//            }
//        }

//        public override void Update(TimeSpan gameTime)
//        {
//        }
//    }
//}
