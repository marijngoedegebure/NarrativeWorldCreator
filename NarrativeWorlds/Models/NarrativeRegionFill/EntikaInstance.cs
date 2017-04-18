using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Semantics.Data;
using Semantics.Entities;
using System.Collections.Generic;
using System;
using Common;

namespace NarrativeWorldCreator
{
    public class EntikaInstance
    {
        // Original TangibleObject class which this is an instance of
        public string Name { get; set; }
        public TangibleObject TangibleObject { get; set; }
        // XNA Model
        public Vector3 Position { get; set; }

        // Has either a polygon for a floor or a model
        public Model Model { get; set; }
        public Polygon Polygon { get; set; }
        public BoundingBox BoundingBox { get; set; }

        public NarrativeShape OffLimitsShape { get; set; }
        public List<NarrativeShape> ClearanceShapes { get; set; }

        public List<GeometricRelationshipBase> RelationshipsAsSource { get; set; }

        public List<GeometricRelationshipBase> RelationshipsAsTarget { get; set; }


        public EntikaInstance(string name, Vector3 pos, Model model, Matrix world)
        {
            SetupLists();
            this.Name = name + EntikaInstanceCount.Count;
            EntikaInstanceCount.Count++;
            this.TangibleObject = DatabaseSearch.GetNode<TangibleObject>(name);
            this.Position = pos;
            this.Model = model;
            UpdateBoundingBoxAndShape(world);
        }

        private void SetupLists()
        {
            ClearanceShapes = new List<NarrativeShape>();
            RelationshipsAsSource = new List<GeometricRelationshipBase>();
            RelationshipsAsTarget = new List<GeometricRelationshipBase>();
        }

        // Constructor for ground
        public EntikaInstance(string name, Polygon poly)
        {
            SetupLists();
            this.Name = name;
            this.Polygon = poly;
            TangibleObject = DatabaseSearch.GetNode<TangibleObject>(name);
            UpdateBoundingBoxAndShape(null);
        }

        // Constructor for addition before placement
        public EntikaInstance(TangibleObject to)
        {
            SetupLists();
            this.Name = to.DefaultName + EntikaInstanceCount.Count;
            EntikaInstanceCount.Count++;
            TangibleObject = to;
            this.Model = SystemStateTracker.DefaultModel;
        }


        public void UpdateBoundingBoxAndShape(Matrix? world)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            if (Model != null && world.HasValue)
            {
                var worldTransform = world.GetValueOrDefault() * Matrix.CreateTranslation(this.Position);
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
            }
            else
            {
                foreach (var point in Polygon.GetAllVertices())
                {
                    Vector3 transformedPosition = new Vector3((float)point.X, (float)point.Y, 0);

                    min = Vector3.Min(min, transformedPosition);
                    max = Vector3.Max(max, transformedPosition);
                }

            }
            // Create and return bounding box
            this.BoundingBox = new BoundingBox(min, max);
        }

        public static List<Common.Vec2> GetBoundingBoxAsPoints(BoundingBox bb)
        {
            Vector3[] corners = bb.GetCorners();
            var points = new List<Common.Vec2>();
            // Take the coordinates of the last 4 points as they are the bottom 4, given that the bottom of the boundingbox is parallel to the x plane
            for (int i = 4; i < corners.Length; i++)
            {
                points.Add(new Common.Vec2(corners[i].X, corners[i].Y));
            }
            return points;
        }
    }

    public static class EntikaInstanceCount
    {
        public static int Count = 0;
    }
}