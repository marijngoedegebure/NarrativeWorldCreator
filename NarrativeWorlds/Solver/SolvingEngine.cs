using Common;
using Common.Geometry;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;

namespace NarrativeWorlds
{
    public static class SolvingEngine
    {
        // Input of a selected node and timepoint
        // Basic version only allows placement of objects on the baseshape, which is reduced with each addition
        public static Vector3 GetPossibleLocationsBasic(Node node, NarrativeTimePoint ntp)
        {
            // Calculates remaining area
            var polygon = ReduceBaseShapeWithObjects(node, ntp);
            var mesh = GetMeshForPolygon(polygon);

            var triangles = mesh.Triangles;

            // Get random shape to place object in
            System.Random r = new System.Random();
            int randomNumber = r.Next(0, triangles.Count-1);

            var selectedPolygon = triangles.ToList()[randomNumber];
            
            // Get random position inside polygon
            var position = getRandomPointOnTriangle(r, selectedPolygon, mesh);

            return new Vector3(position.X, position.Y, 0);
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
            foreach(var vector in p.Contours.ToList()[0])
            {
                shapePoints.Add(new Vec2((float)vector.X, (float)vector.Y));
            }
            return new Shape(shapePoints);
        }

        // The basic version only removes shapes from the base shape and adds the new shape
        public static NarrativeTimePoint AddEntikaInstanceToTimePointBasic(NarrativeTimePoint ntp, EntikaInstance addition)
        {
            NarrativeShape BaseShape = ntp.TimePointSpecificFill.NarrativeShapes[0];
            // Create new off limits shape based on addition
            var offLimitPolygon = new Polygon(addition.GetBoundingBoxAsPoints());
            var additionOffLimitShape = new NarrativeShape(0, offLimitPolygon, NarrativeShape.ShapeType.Offlimits, addition);
            addition.OffLimitsShape = additionOffLimitShape;

            // Create new clearance limits shapes based on addition


            // Create new relationships based on addition

            var shapeResult = DifferenceShapes(BaseShape.Polygon, additionOffLimitShape.Polygon);
            ntp.TimePointSpecificFill.NarrativeShapes[0].Polygon = shapeResult;
            ntp.TimePointSpecificFill.NarrativeShapes.Add(additionOffLimitShape);
            ntp.TimePointSpecificFill.OtherObjectInstances.Add(addition);
            return ntp;
        }

        // Important method, returns an updated narrative time point. The narrative time point includes a list of shapes adjusted for the addition of the new shape
        public static NarrativeTimePoint AddShapeToTimePoint(NarrativeTimePoint ntp, NarrativeShape addition)
        {
            // First should always be ground/base shape
            NarrativeShape BaseShape = ntp.TimePointSpecificFill.NarrativeShapes[0];
            // Create list so that addition shape can be manipulated in each for loop.
            List<NarrativeShape> additionList = new List<NarrativeShape>();
            additionList.Add(addition);

            // Determine in which shapes the addition lies, loop through all shapes and determine the overlap
            foreach(NarrativeShape ns in ntp.TimePointSpecificFill.NarrativeShapes)
            {
                Polygon polygon = ns.Polygon;
                foreach (NarrativeShape add in additionList)
                {
                    // If it overlaps, the intersection should be removed from each shape
                    if (polygon.Overlaps(add.Polygon))
                    {
                        Polygon result = IntersectShapes(polygon, add.Polygon);
                        Polygon shapeResult = DifferenceShapes(polygon, result);
                        Polygon addResult = DifferenceShapes(add.Polygon, result);
                        // Retrieve all shapes from the results and add new shapes to the right lists, remove the adjusted shapes
                        // additionList.Add(new NarrativeShape(add.Name, add.Position, ));
                        // additionList.Remove(add);
                    }

                }
            }
            return ntp;
        }

        public static Polygon ReduceBaseShapeWithObjects(Node node, NarrativeTimePoint ntp)
        {
            Shape baseShape = node.Shape;
            Polygon basePolygon = new Polygon(baseShape.Points.ToList());
            for( int i = 0; i < ntp.TimePointSpecificFill.OtherObjectInstances.Count; i++)
            {
                basePolygon = SolvingEngine.DifferenceShapes(basePolygon, ntp.TimePointSpecificFill.OtherObjectInstances[i].OffLimitsShape.Polygon);
            }
            return basePolygon;
        }
    }
}
