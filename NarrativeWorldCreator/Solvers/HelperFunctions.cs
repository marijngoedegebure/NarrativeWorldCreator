using Common;
using Common.Geometry;
using Common.Shapes;
using Microsoft.Xna.Framework;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Solvers
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

        public static List<Vec2> ParseShapeDescription(BaseShapeDescription ShapeDescription, Microsoft.Xna.Framework.BoundingBox bb)
        {
            List<float> values = new List<float>();
            // Parse parameters into values
            foreach (var parameterName in ShapeDescription.GetParameterNames())
            {
                var descriptor = ShapeDescription.GetParameterValue(parameterName);
                var valueDescriptor = descriptor.Substring(4, 3);
                var axis = descriptor.Substring(7, 1);
                float value = 0.0f;
                if (valueDescriptor.Equals("Min"))
                {
                    if (axis.Equals("X"))
                    {
                        value = bb.Min.X;
                    }
                    else if (axis.Equals("Y"))
                    {
                        value = bb.Min.Y;
                    }
                    else if (axis.Equals("Z"))
                    {
                        value = bb.Min.Z;
                    }
                }
                else if (valueDescriptor.Equals("Max"))
                {
                    if (axis.Equals("X"))
                    {
                        value = bb.Max.X;
                    }
                    else if (axis.Equals("Y"))
                    {
                        value = bb.Max.Y;
                    }
                    else if (axis.Equals("Z"))
                    {
                        value = bb.Max.Z;
                    }
                }
                else if (valueDescriptor.Equals("Mid"))
                {
                    if (axis.Equals("X"))
                    {
                        value = bb.Max.X - ((bb.Max.X - bb.Min.X) / 2);
                    }
                    else if (axis.Equals("Y"))
                    {
                        value = bb.Max.Y - ((bb.Max.Y - bb.Min.Y) / 2);
                    }
                    else if (axis.Equals("Z"))
                    {
                        value = bb.Max.Z - ((bb.Max.Z - bb.Min.Z) / 2);
                    }
                }
                else
                {
                    break;
                }
                var translation = descriptor.Substring(8);
                translation = translation.Replace("(", "");
                translation = translation.Replace(")", "");
                if (!translation.Equals(""))
                {
                    var sign = translation.Substring(0, 1);
                    if (sign.Equals("+"))
                    {
                        value = value + (float)Double.Parse(translation.Substring(1));
                    }
                    else if (sign.Equals("-"))
                    {
                        value = value - (float)Double.Parse(translation.Substring(1));
                    }
                }
                values.Add(value);
            }
            // parse derived paramater values into list of points
            var clearanceBB = new Microsoft.Xna.Framework.BoundingBox(new Vector3(values[0], values[1], values[2]), new Vector3(values[3], values[4], values[5]));
            return EntikaInstance.GetBoundingBoxAsPoints(clearanceBB);
        }
    }
}
