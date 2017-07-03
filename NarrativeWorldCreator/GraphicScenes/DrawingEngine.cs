using Common;
using Common.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet.Geometry;

namespace NarrativeWorldCreator.GraphicScenes
{
    public static class DrawingEngine
    {
        public static TriangleNet.Mesh triangulatePolygon(Shape shape)
        {
            if (shape.Points.Count > 2)
            {
                // Triangulate using Triangle.NET
                var geometry = new InputGeometry(shape.Points.Count);
                for (int i = 0; i < shape.Points.Count; i++)
                {
                    geometry.AddPoint(shape.Points[i].X, shape.Points[i].Y, 1);
                    geometry.AddSegment(i, (i + 1) % shape.Points.Count, 2);
                }
                TriangleNet.Mesh mesh = new TriangleNet.Mesh();
                mesh.Triangulate(geometry);
                return mesh;
            }
            return null;
        }

        public static List<VertexPositionColor> GetDrawableTriangles(TriangleNet.Mesh mesh, Color color)
        {
            List<VertexPositionColor> ret = new List<VertexPositionColor>();
            var triangles = mesh.Triangles.ToList();
            var vertices = mesh.Vertices.ToList();
            for (int i = 0; i < triangles.Count; i++)
            {
                ret.Add(new VertexPositionColor(new Vector3((float)vertices[triangles[i].P0].X, (float)vertices[triangles[i].P0].Y, 0), color));
                ret.Add(new VertexPositionColor(new Vector3((float)vertices[triangles[i].P1].X, (float)vertices[triangles[i].P1].Y, 0), color));
                ret.Add(new VertexPositionColor(new Vector3((float)vertices[triangles[i].P2].X, (float)vertices[triangles[i].P2].Y, 0), color));
            }
            return ret;
        }

        public static List<VertexPositionColor> GetVPCForTrianleStrip(TriStrip triStrip, Color color)
        {
            var ret = new List<VertexPositionColor>();
            var strips = triStrip.Strips.ToList();
            for(int i = 0; i < triStrip.Count; i++)
            {
                var strip = strips[i].ToList();
                for(int j = 0; j < strip.Count; j++)
                {
                    ret.Add(new VertexPositionColor(new Vector3((float)strip[j].X, (float)strip[j].Y, 0), color));
                }
            }

            return ret;
        }
    }
}
