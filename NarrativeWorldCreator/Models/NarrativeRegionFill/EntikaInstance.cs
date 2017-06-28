using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Semantics.Data;
using Semantics.Entities;
using System.Collections.Generic;
using System;
using Common;
using NarrativeWorldCreator.Solvers;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    public class EntikaInstance
    {
        // Original TangibleObject class which this is an instance of
        public string Name { get; set; }
        public TangibleObject TangibleObject { get; set; }
        // XNA Model
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        // Has either a polygon for a floor or a model
        public Polygon Polygon { get; set; }
        public BoundingBox BoundingBox { get; set; }

        public List<RelationshipInstance> RelationshipsAsSource { get; set; }

        public List<RelationshipInstance> RelationshipsAsTarget { get; set; }

        public List<Polygon> Clearances { get; set; }

        public bool Frozen { get; set; }

        private void SetupLists()
        {
            Clearances = new List<Polygon>();
            RelationshipsAsSource = new List<RelationshipInstance>();
            RelationshipsAsTarget = new List<RelationshipInstance>();
        }

        // Constructor for ground
        public EntikaInstance(string name, Polygon poly)
        {
            SetupLists();
            this.Name = name;
            this.Polygon = poly;
            this.Position = Vector3.Zero;
            this.Rotation = Vector3.Zero;
            // Ground is always frozen!
            this.Frozen = true;
            TangibleObject = DatabaseSearch.GetNode<TangibleObject>(name);
            UpdateBoundingBoxAndShape(null);
        }

        // Constructor for addition before placement
        public EntikaInstance(TangibleObject to)
        {
            SetupLists();
            this.Name = to.DefaultName + EntikaInstanceCount.Count;
            EntikaInstanceCount.Count++;
            this.TangibleObject = to;
            this.UpdateBoundingBoxAndShape(SystemStateTracker.world);
            foreach (SpaceValued space in this.TangibleObject.Spaces)
            {
                if (space.Space.DefaultName.Equals(Constants.Clearance))
                {
                    // Clearance should always be described as a extruded line, this implies three parameters and results in four coordinates which can be used as a definition of shape
                    this.Clearances.Add(new Polygon(HelperFunctions.ParseShapeDescription(space.ShapeDescription, this.BoundingBox)));
                }
            }
        }

        // Constructor used to copy an entika instance
        public EntikaInstance(EntikaInstance obj)
        {
            SetupLists();
            this.Name = obj.Name;
            this.TangibleObject = obj.TangibleObject;
            this.Position = new Vector3(obj.Position.X, obj.Position.Y, obj.Position.Z);
            this.Rotation = new Vector3(obj.Rotation.X, obj.Rotation.Y, obj.Rotation.Z);
            if (obj.Polygon != null)
                this.Polygon = new Polygon(obj.Polygon.GetAllVertices());
            this.BoundingBox = new BoundingBox(obj.BoundingBox.Min, obj.BoundingBox.Max);
            this.Frozen = obj.Frozen;
            foreach (var clearance in obj.Clearances)
            {
                this.Clearances.Add(new Polygon(clearance.GetAllVertices()));
            }
        }

        public void UpdateBoundingBoxAndShape(Matrix? world)
        {
            BoundingBox bb;
            // For each mesh of the model
            if (this.Polygon != null)
            {
                bb = GetBoundingBox(this.Polygon);
            }
            // Or using the declared polygon
            else
            {
                bb = GetBoundingBox(SystemStateTracker.NarrativeWorld.ModelsForTangibleObjects[this.TangibleObject], world.GetValueOrDefault() * Matrix.CreateRotationZ(this.Rotation.Y));
            }
            if (bb != null)
                this.BoundingBox = bb;
        }

        internal static BoundingBox GetBoundingBox(Polygon polygon)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var point in polygon.GetAllVertices())
            {
                Vector3 transformedPosition = new Vector3((float)point.X, (float)point.Y, 0);

                min = Vector3.Min(min, transformedPosition);
                max = Vector3.Max(max, transformedPosition);
            }
            return new BoundingBox(min, max);
        }

        public static BoundingBox GetBoundingBox(Model m, Matrix world)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            var worldTransform = world;
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in m.Meshes)
            {
                worldTransform = transforms[mesh.ParentBone.Index] * world;
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
            return new BoundingBox(min, max);
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

        public override bool Equals(object obj)
        {
            var item = obj as EntikaInstance;

            if (item == null)
            {
                return false;
            }

            return this.Name.Equals(item.Name);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }

    public static class EntikaInstanceCount
    {
        public static int Count = 0;
    }
}