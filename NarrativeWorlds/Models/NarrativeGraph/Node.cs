using Common.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PDDLNarrativeParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Geometry;

namespace NarrativeWorlds
{
    public class Node
    {
        public String LocationName { get; set; }
        // Store base shape of region so it can be easily rendered
        public Shape Shape { get; set; }
        // Triangulated region points
        public TriangleNet.Mesh Mesh { get; set; }

        public List<NarrativeEvent> NarrativeEvents { get; set; }
        public List<NarrativeObject> NarrativeObjects { get; set; }

        public Node(String locationName)
        {
            this.LocationName = locationName;
            this.Shape = new Shape(new List<Common.Vec2>());
            NarrativeEvents = new List<NarrativeEvent>();
            NarrativeObjects = new List<NarrativeObject>();
        }

        public override bool Equals(System.Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Node n = (Node)obj;
            return LocationName.Equals(n.LocationName);
        }

        public void triangulatePolygon()
        {
            if (Shape.Points.Count > 2)
            {
                // Triangulate using Triangle.NET
                var geometry = new InputGeometry(Shape.Points.Count);
                for (int i = 0; i < Shape.Points.Count; i++)
                {
                    geometry.AddPoint(Shape.Points[i].X, Shape.Points[i].Y, 1);
                    geometry.AddSegment(i, (i + 1) % Shape.Points.Count, 2);
                }
                Mesh = new TriangleNet.Mesh();
                Mesh.Triangulate(geometry);
            }
            return;
        }

        public List<VertexPositionColor> GetDrawableTriangles(Color color)
        {
            List<VertexPositionColor> ret = new List<VertexPositionColor>();
            var triangles = this.Mesh.Triangles.ToList();
            var vertices = this.Mesh.Vertices.ToList();
            for (int i = 0; i < triangles.Count; i++)
            {
                ret.Add(new VertexPositionColor(new Vector3((float)vertices[triangles[i].P0].X, (float)vertices[triangles[i].P0].Y, 0), color));
                ret.Add(new VertexPositionColor(new Vector3((float)vertices[triangles[i].P1].X, (float)vertices[triangles[i].P1].Y, 0), color));
                ret.Add(new VertexPositionColor(new Vector3((float)vertices[triangles[i].P2].X, (float)vertices[triangles[i].P2].Y, 0), color));
            }
            return ret;
        }

        public List<Vector3> GetXNAPointsOfShape()
        {
            List<Vector3> vertices = new List<Vector3>();
            foreach (var point in Shape.Points)
            {
                vertices.Add(new Vector3(point.X, point.Y, 0));
            }
            return vertices;
        }
    }
}
