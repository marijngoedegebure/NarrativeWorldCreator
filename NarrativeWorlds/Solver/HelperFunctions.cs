using Common;
using Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet.Geometry;

namespace NarrativeWorlds
{
    // Set of functions used to help out the planning and solving engine
    public static class HelperFunctions
    {
        public static Polygon DifferenceShapes(Polygon original, Polygon secondary)
        {
            return GpcWrapper.Clip(GpcWrapper.GpcOperation.Difference, original, secondary);
        }

        public static Polygon IntersectShapes(Polygon original, Polygon secondary)
        {
            return GpcWrapper.Clip(GpcWrapper.GpcOperation.Intersection, original, secondary);
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

        public static TriangleNet.Mesh GetMeshForPolygon(Polygon p)
        {
            // Triangulate using Triangle.NET
            var geometry = new InputGeometry(p.GetAllVertices().Count());
            var contours = p.Contours.ToList();
            for (int i = 0; i < p.NumContours; i++)
            {
                var points = new List<TriangleNet.Geometry.Point>();
                foreach (var vector in contours[i])
                {
                    points.Add(new TriangleNet.Geometry.Point(vector.X, vector.Y));
                }
                if (p.IsHole(i))
                {
                    geometry.AddRingAsHole(points);
                }
                else
                {
                    geometry.AddRing(points);
                }
            }
            TriangleNet.Mesh mesh = new TriangleNet.Mesh();
            mesh.Triangulate(geometry);
            return mesh;
        }

        // Only converts the first contour of a polygon
        public static Shape PolygonToShape(Polygon p)
        {
            List<Vec2> shapePoints = new List<Vec2>();
            foreach (var vector in p.Contours.ToList()[0])
            {
                shapePoints.Add(new Vec2((float)vector.X, (float)vector.Y));
            }
            return new Shape(shapePoints);
        }
    }
}
