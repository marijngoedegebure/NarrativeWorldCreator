using Common;
using Common.Geometry;
using Microsoft.Xna.Framework;
using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public static class PlanningEngine
    {
        // Selects a new tangible object class together with a destination shape, based on all information available
        public static Tuple<string, NarrativeShape> SelectTangibleObjectDestinationShape(NarrativeTimePoint ntp)
        {

            return new Tuple<string, NarrativeShape>("couch", ntp.TimePointSpecificFill.NarrativeShapes[0]);
        }

        // Input of a selected node and timepoint
        // Basic version only allows placement of objects on the baseshape, which is reduced with each addition
        public static Vector3 GetPossibleLocationsBasic(Node node, NarrativeTimePoint ntp)
        {
            // Calculates remaining area
            var polygon = ReduceBaseShapeWithObjects(node, ntp);

            var mesh = HelperFunctions.GetMeshForPolygon(polygon);

            var triangles = mesh.Triangles;

            // Get random shape to place object in
            System.Random r = new System.Random();
            int randomNumber = r.Next(0, triangles.Count - 1);

            var selectedPolygon = triangles.ToList()[randomNumber];

            // Get random position inside polygon
            var position = getRandomPointOnTriangle(r, selectedPolygon, mesh);

            return new Vector3(position.X, position.Y, 0);
        }

        public static Vector3 GetPossibleLocationsV2(Node node, NarrativeTimePoint ntp)
        {
            // Take all narrative shapes known
            var shapes = ntp.TimePointSpecificFill.NarrativeShapes;

            // Go through each shape and determine it's polygon and it's restrictions (clearance/relationship)
            // Get random shape to place object in
            System.Random r = new System.Random();
            int randomNumber = r.Next(0, shapes.Count - 1);

            var mesh = HelperFunctions.GetMeshForPolygon(shapes[randomNumber].Polygon);

            var triangles = mesh.Triangles;

            randomNumber = r.Next(0, triangles.Count - 1);
            var selectedPolygon = triangles.ToList()[randomNumber];

            // Get random position inside polygon
            var position = getRandomPointOnTriangle(r, selectedPolygon, mesh);

            return new Vector3(position.X, position.Y, 0);
        }

        public static Vector3 GetPossibleLocationsV3(NarrativeShape shape)
        {
            var mesh = HelperFunctions.GetMeshForPolygon(shape.Polygon);

            var triangles = mesh.Triangles;

            System.Random r = new System.Random();
            var randomNumber = r.Next(0, triangles.Count - 1);
            var selectedTriangle = triangles.ToList()[randomNumber];

            // Get random position inside polygon
            var position = getRandomPointOnTriangle(r, selectedTriangle, mesh);

            return new Vector3(position.X, position.Y, shape.zpos);
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

        public static Polygon ReduceBaseShapeWithObjects(Node node, NarrativeTimePoint ntp)
        {
            Shape baseShape = node.Shape;
            Polygon basePolygon = new Polygon(baseShape.Points.ToList());
            for (int i = 0; i < ntp.TimePointSpecificFill.OtherObjectInstances.Count; i++)
            {
                basePolygon = HelperFunctions.DifferenceShapes(basePolygon, ntp.TimePointSpecificFill.OtherObjectInstances[i].OffLimitsShape.Polygon);
            }
            return basePolygon;
        }
    }
}
