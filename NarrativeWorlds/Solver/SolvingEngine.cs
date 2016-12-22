using Common;
using Common.Geometry;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Geometry;

namespace NarrativeWorlds
{
    public static class SolvingEngine
    {
        // Input of a selected node and timepoint
        public static Vector3 GetPossibleLocations(Node node, NarrativeTimePoint ntp)
        {
            // Retrieve possible shapes where an object can be placed in (triangles of the node)
            var polygons = node.Mesh.Triangles;

            // Get random shape to place object in
            System.Random r = new System.Random();
            int randomNumber = r.Next(0, polygons.Count);

            var selectedPolygon = polygons.ToList()[randomNumber];
            
            // Get random position inside polygon
            var position = getRandomPointOnTriangle(r, selectedPolygon, node.Mesh);

            return new Vector3(position.X, position.Y, 0);

            // Todo:
            // Retrieve current instanced objects of node (basefill)

            // Retrieve current instanced narrative objects and characters

            // Retrieve/calculate base navmesh

            // Determine left over space of region by looping through instances and subtracting them from base navmesh
        }

        public static Vec2i getRandomPointOnTriangle(System.Random r, TriangleNet.Data.Triangle triangle, TriangleNet.Mesh mesh)
        {
            // Calculate area of triangle
            var vertices = mesh.Vertices.ToList();
            var v0 = new Vec2((float)vertices[triangle.P0].X, (float)vertices[triangle.P0].Y);
            var v1 = new Vec2((float)vertices[triangle.P1].X, (float)vertices[triangle.P1].Y);
            var v2 = new Vec2((float)vertices[triangle.P2].X, (float)vertices[triangle.P2].Y);

            
            var AB = (v1 - v0) / 1000f;
            var AC = (v2 - v0) / 1000f;
            float area = Math.Abs(AB.X * AC.Y - AC.X * AB.Y) * 0.5f;

            float a = (float)r.NextDouble();
            float b = (float)r.NextDouble();
            if (a + b > 1)
            {
                a = 1 - a;
                b = 1 - b;
            }
            Line2 l01 = new Line2(v0, v1);
            Vec2 p = l01.PointOnLine(a);
            Line2 l02 = new Line2(v0, v2);
            Vec2 p2 = l02.PointOnLine(b);
            return new Vec2i(p + (p2 - v0));
        }

        // Static functions that are not part of common but should be
        public static IEnumerable<Shape> MinkowskiMinus(Shape original, Shape secondary)
        {
            foreach (List<Vec2> list in MinkowskiMinus(original.Points.ToList(), secondary.Points.ToList()))
                yield return new Shape(list);
        }

        public static List<List<Vec2>> MinkowskiMinus(List<Vec2> A, List<Vec2> B)
        {

            List<Polygon> polys = new List<Polygon>();

            for (int b = 0; b < B.Count; ++b)
                polys.Add(new Polygon(new List<Vec2>(Minus(A, B[b]))));

            Polygon result = polys[0];
            for (int i = 1; i < polys.Count; ++i)
            {
                result = GpcWrapper.Clip(GpcWrapper.GpcOperation.Intersection, result, polys[i]);
                if (result == null || result.NumContours == 0)
                    return new List<List<Vec2>>();
            }

            List<List<Vec2>> shapes = new List<List<Vec2>>();

            for (int i = 0; i < result.NumContours; ++i)
            {
                List<Vec2d> vl = result[i];

                List<Vec2> points = new List<Vec2>();
                for (int j = 0; j < vl.Count; ++j)
                {
                    Vec2d tv = vl[j];
                    points.Add(new Vec2((float)tv.X, (float)tv.Y));
                }

                shapes.Add(points);
            }
            return shapes;
        }

        private static IEnumerable<Vec2> Minus(List<Vec2> A, Vec2 min)
        {
            foreach (Vec2 a in A)
                yield return a - min;
        }
    }
}
