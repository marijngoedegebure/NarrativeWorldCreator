using Common;
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
    public class NarrativeShape
    {
        public string Name { get; set; }
        public enum ShapeType
        {
            Clearance,
            Offlimits,
            Relationship
        }
        public ShapeType Type { get; set; }
        // Original TangibleObject class which this is an instance of
        public TangibleObject TangibleObject { get; set; }
        // XNA Model
        public Vector3 Position { get; set; }
        public Model Model { get; set; }

        // A NarrativeShape can either be created using a model or from a shape, this boolean tracks exactly that
        public bool ModelUsage { get; set; }

        // Shape derived from spaces, models and position
        public Polygon Polygon { get; set; }
        // Zpos is simplified to be the same for the entire shape, all shapes are parallel to x and y axis
        public float zpos { get; set; }

        public BoundingBox BoundingBox { get; set; }

        List<GeometricRelationshipBase> RelationshipsAsSource { get; set; }

        List<GeometricRelationshipBase> RelationshipsAsTarget { get; set; }

        public NarrativeShape(string name, Vector3 pos, Model model, Matrix world)
        {
            this.Name = name;
            TangibleObject = DatabaseSearch.GetNode<TangibleObject>(name);
            Position = pos;
            this.Model = model;
            this.ModelUsage = true;
            UpdateBoundingBoxAndShape(world);
        }

        public NarrativeShape(string name, float zpos, Polygon polygon)
        {
            this.Name = name;
            this.zpos = zpos;
            this.Polygon = polygon;
            this.ModelUsage = false;
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
            this.Polygon = new Polygon(points);
        }
    }
}
