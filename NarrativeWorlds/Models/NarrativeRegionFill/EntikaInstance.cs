using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Semantics.Data;
using Semantics.Entities;
using System.Collections.Generic;

namespace NarrativeWorlds
{
    public class EntikaInstance
    {
        // Original TangibleObject class which this is an instance of
        public string Name { get; set; }
        public TangibleObject TangibleObject { get; set; }
        // XNA Model
        public Vector3 Position { get; set; }
        public Model Model { get; set; }
        public BoundingBox BoundingBox { get; set; }

        public NarrativeShape OffLimitsShape { get; set; }
        public List<NarrativeShape> ClearanceShape { get; set; }
        public List<NarrativeShape> RelationShapes { get; set; }

        List<GeometricRelationshipBase> RelationshipsAsSource { get; set; }

        List<GeometricRelationshipBase> RelationshipsAsTarget { get; set; }


        public EntikaInstance(string name, Vector3 pos, Model model, Matrix world)
        {
            this.Name = name;
            TangibleObject = DatabaseSearch.GetNode<TangibleObject>(name);
            this.Position = pos;
            this.Model = model;
            UpdateBoundingBoxAndShape(world);
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
            this.BoundingBox = new BoundingBox(min, max);
        }

        public List<Common.Vec2> GetBoundingBoxAsPoints()
        {
            Vector3[] corners = this.BoundingBox.GetCorners();
            var points = new List<Common.Vec2>();
            // Take the coordinates of the last 4 points as they are the bottom 4, given that the bottom of the boundingbox is parallel to the x plane
            for (int i = 4; i < corners.Length; i++)
            {
                points.Add(new Common.Vec2(corners[i].X, corners[i].Y));
            }
            return points;
        }
    }
}