using Common.Geometry;
using GameSemantics.Components;
using GameSemantics.Data;
using GameSemanticsEngine.GameContent;
using GameSemanticsEngine.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Semantics.Data;
using Semantics.Entities;
using SemanticsEngine.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class EntikaClassInstance
    {
        public string Name { get; set; }

        // Original TangibleObject class which this is an instance of
        public TangibleObject TangibleObject { get; set; }

        // Spaces derived from tangible object class
        public SpaceValued Clearance { get; set; }
        public SpaceValued OffLimits { get; set; }

        // Shape derived from spaces, models and position
        public Shape Shape { get; set; }
        public BoundingBox BoundingBox { get; set; }

        // XNA Model
        public Model Model { get; set; }

        // 3D position
        public Vector3 Position { get; set; }

        public EntikaClassInstance(string name, Vector3 pos, Model model)
        {
            this.Name = name;
            TangibleObject = DatabaseSearch.GetNode<TangibleObject>(name);
            foreach (SpaceValued space in TangibleObject.Spaces)
            {
                if (space.Space.DefaultName.Equals("Clearance"))
                    Clearance = space;
                if (space.Space.DefaultName.Equals("Off limits"))
                    OffLimits = space;
            }
            Position = pos;
            this.Model = model;

            //DetermineShape();
        }

        private void DetermineShape()
        {
            // Parse Spaces, determine settings for shape
            
            // Determine boundingbox based on model
        }

        public void UpdateBoundingBoxAndShape(Matrix world)
        {
            var worldTransform = world * Matrix.CreateTranslation(this.Position);
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            BoundingBox = new BoundingBox(min, max);
            Vector3[] corners = BoundingBox.GetCorners();
            var points = new List<Common.Vec2>();
            // Take the coordinates of the last 4 points as they are the bottom 4, given that the bottom of the boundingbox is parallel to the x plane
            for (int i = 4; i < corners.Length; i++)
            {
                points.Add(new Common.Vec2(corners[i].X, corners[i].Y));
            }
            Shape = new Shape(points);
        }
    }
}
