using System;
using System.Collections.Generic;
using System.Text;
using Common;

namespace Common
{
    public class MathUtilD
    {
        private MathUtilD()
        {
            // Empty
        }

        public static double InterpolateLinear(double v1, double v2, double fraction)
        {
            return v1 * (1.0 - fraction) + v2 * fraction;
        }

        public static double InterpolateCatmullRom(double v0, double v1, double v2, double v3, double fraction)
        {

            return 0.5 * ((2.0 * v1) + (-v0 + v2) * fraction + (2.0 * v0 - 5.0 * v1 + 4.0 * v2 - v3) * fraction * fraction + (-v0 + 3 * v1 - 3.0 * v2 + v3) * fraction * fraction * fraction);
        }

        public static double InterpolateCubic(double v0, double v1, double v2, double v3, double fraction)
        {
            double p = (v3 - v2) - (v0 - v1);
            double q = (v0 - v1) - p;
            double r = v2 - v0;
            double fraction2 = fraction * fraction;
            return p * fraction * fraction2 + q * fraction2 + r * fraction + v1;
        }

        public static int PowerOf2Log2(int n)
        {
            for (int i = 0; i < 31; i++)
            {
                if ((n & 1) == 1)
                {
                    return i;
                }
                n >>= 1;
            }
            return 0;
        }

        public static void Normalize(double[,] map)
        {
            //assert map != null && map.length > 0;
            double min = 0;
            double max = 0;
            // find the min and max

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    double z = map[y, x];
                    min = Math.Min(z, min);
                    max = Math.Max(z, max);
                }
            }
            // Avoiding divide by zero
            if (max != min)
            {
                // Divide every height by the maximum to normalize to [0.0, 1.0]
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    for (int x = 0; x < map.GetLength(1); x++)
                    {
                        map[y, x] = (map[y, x] - min) / (max - min);
                    }
                }
            }
        }

        public static Vec2d RandomPointInTriangle(Vec2d v0, Vec2d v1, Vec2d v2)
        {
            double a = RandomNumber.Random();
            double b = RandomNumber.Random();
            if (a + b > 1.0)
            {
                a = 1.0 - a;
                b = 1.0 - b;
            }
            Vec2d p1 = v0 + a * (v1 - v0);
            Vec2d p2 = v0 + b * (v2 - v0);
            return p1 + (p2 - v0);
        }

        public static Vec2d RotateOnCircle(Vec2d center, double radius, double angle)
        {
            double cosAngle = Math.Cos(angle);
            double sinAngle = Math.Sin(angle);
            Vec2d top = center + new Vec2d(radius, 0);
            return new Vec2d(
                (top.X - center.X) * cosAngle - (top.Y - center.Y) * sinAngle + center.X,
                (top.Y - center.Y) * cosAngle + (top.X - center.X) * sinAngle + center.Y
                );
        }

        public static double AreaOfTriangle(Vec2d v0, Vec2d v1, Vec2d v2)
        {
            return Math.Abs((v1.X - v0.X) * (v2.Y - v0.Y) - (v2.X - v0.X) * (v1.Y - v0.Y)) / 2.0;
        }

        public static double DegreeToRadian(double p)
        {
            return p / 180.0 * Math.PI;
        }

        public static double RadianToDegree(double p)
        {
            return p / Math.PI * 180.0;
        }

        public static bool IntersectionLines(Vec2d p1, Vec2d p2, Vec2d p3, Vec2d p4, out double factorA, out double factorB, out Vec2d intersectionPoint)
        {
            bool result = false;
            double denominator = ((p4.Y - p3.Y) * (p2.X - p1.X)) - ((p4.X - p3.X) * (p2.Y - p1.Y));
            if (Math.Abs(denominator) < 0.0001)
            {
                // Lines are parallel
                intersectionPoint = new Vec2d(0, 0);
                factorA = Double.NaN;
                factorB = Double.NaN;
            }
            else
            {
                factorA = (((p4.X - p3.X) * (p1.Y - p3.Y)) - ((p4.Y - p3.Y) * (p1.X - p3.X))) / denominator;
                factorB = (((p2.X - p1.X) * (p1.Y - p3.Y)) - ((p2.Y - p1.Y) * (p1.X - p3.X))) / denominator;
                intersectionPoint = p3 + factorB * (p4 - p3);
                result = factorA >= 0.0 && factorA <= 1.0 && factorB >= 0.0 && factorB <= 1.0;
            }
            return result;
        }

        public static bool IntersectsLineBox(Vec2d s, Vec2d e, Rect box)
        {
            double st, et, fst = 0, fet = 1;
            for (int i = 0; i < 2; i++)
            {
                double si = s[i];
                double ei = e[i];
                double mini = box.Min[i];
                double maxi = box.Max[i];
                if (si < ei)
                {
                    if (si > maxi || ei < mini)
                    {
                        return false;
                    }
                    double di = ei - si;
                    st = (si < mini) ? (mini - si) / di : 0;
                    et = (ei > maxi) ? (maxi - si) / di : 1;
                }
                else
                {
                    if (ei > maxi || si < mini)
                    {
                        return false;
                    }
                    double di = ei - si;
                    st = (si > maxi) ? (maxi - si) / di : 0;
                    et = (ei < mini) ? (mini - si) / di : 1;
                }

                if (st > fst) fst = st;
                if (et < fet) fet = et;
                if (fet < fst)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool LineIntersectsPolygon(List<Vec2d> poly, Vec2d a, Vec2d b)
        {
            bool found = false;
            double fA, fB;
            Vec2d ip;

            if (poly.Count == 0)
                return found;

            if (MathUtilD.IntersectionLines(a, b, poly[0], poly[poly.Count - 1], out fA, out fB, out ip))
            {
                found = true;
            }
            for (int i = 0; !found && i < poly.Count - 1; i++)
            {
                if (MathUtilD.IntersectionLines(a, b, poly[i], poly[i + 1], out fA, out fB, out ip))
                {
                    found = true;
                }
            }
            return found;
        }       

        public static double PolylineLength(List<Vec2d> poly)
        {
            double result = 0.0;
            for (int i = 0; i < poly.Count - 1; ++i)
            {
                int j = i + 1;
                result += (poly[j] - poly[i]).Length;
            }
            return result;
        }

        public static bool IsRectangle(List<Vec2d> poly)
        {
            return IsRectangle(poly, 0.00001);
        }

        public static bool IsRectangle(List<Vec2d> poly, double epsilon)
        {
            bool result = poly.Count == 4;
            if (result)
            {
                // Check angles
                for (int i = 0; result && i < poly.Count - 1; ++i)
                {
                    int j = i + 1;
                    int k = (i + 2) % poly.Count;
                    Vec2d v1 = poly[j] - poly[i];
                    Vec2d v2 = poly[k] - poly[j];
                    v1.Normalize();
                    v2.Normalize();
                    double angle = Vec2d.AngleBetweenVectors(v1, v2);
                    while (angle < -Math.PI)
                    {
                        angle += 2.0 * Math.PI;
                    }
                    while (angle >= Math.PI)
                    {
                        angle -= 2.0 * Math.PI;
                    }
                    angle = Math.Abs(angle);
                    result = Math.Abs(angle - 0.5 * Math.PI) < epsilon;

                }
            }
            return result;
        }

        /// <summary>
        /// Returns a signed angle in radians between vector u and v.
        /// </summary>
        /// <param name="u">first (normalized!) vector</param>
        /// <param name="v">second (normalized!) vector</param>
        /// <returns></returns>
        public static double GetAngle(Vec2d u, Vec2d v)
        {
            return Math.Atan2(Vec2d.Dot(new Vec2d(-u.Y, u.X), v), Vec2d.Dot(u, v));
        }

        public static double GetClockwiseMostAngle(Vec2d v1, Vec2d v2)
        {
            // Assume v1 and v2 normalized
            double dot = Vec2d.Dot(v1, v2);
            // Limit dot to valid values [-1 .. 1]
            dot = Math.Max(-1.0, Math.Min(1.0, dot));
            double angle = Math.Acos(dot);
            angle = Math.Abs(angle);
            // Check clockwise / ccw
            angle = (v1.X * v2.Y - v1.Y * v2.X) < 0 ? 2 * Math.PI - angle : angle;
            //// Same direction (i.e. back) is to be maximum angle
            //angle = angle < 0.001 && angle > -0.001 ? 2 * Math::PI : angle;
            return angle;
        }

        public static double PointProjectedOnLine(Vec2d point, Vec2d t1, Vec2d t2)
        {
            return ((point.X - t1.X) * (t2.X - t1.X) + (point.Y - t1.Y) * (t2.Y - t1.Y)) / (t2 - t1).SquareLength();
        }

        public static double distanceToLineSegment(Vec2d point, Vec2d t1, Vec2d t2, out double factor)
        {
            factor = Math.Max(0.0, Math.Min(1.0, PointProjectedOnLine(point, t1, t2)));
            Vec2d intersectionPoint = t1 + factor * (t2 - t1);
            return (point - intersectionPoint).Length;
        }

        public static double DistanceToLine(Vec2d point, Vec2d t1, Vec2d t2)
        {
            double factor = PointProjectedOnLine(point, t1, t2);
            Vec2d intersectionPoint = t1 + factor * (t2 - t1);
            return (point - intersectionPoint).Length;
        }

        public static bool IsOnLine(Vec2d point, Vec2d source, Vec2d target)
        {
            double factor;
            return IsOnLine(point, source, target, out factor);
        }

        public static bool IsOnLine(Vec2d point, Vec2d source, Vec2d target, out double factor)
        {
            return distanceToLineSegment(point, source, target, out factor) < 0.0001;
        }

        /// <summary>
        /// Returns a sorted list of intersection points for a line segment with a (closed) polygon.
        /// </summary>
        /// <param name="v1">Start of line segment</param>
        /// <param name="v2">End of line segment</param>
        /// <param name="polygon">Polygon to be intersected</param>
        /// <returns>All intersection points sorted to their distance on the line segment</returns>
        public static List<IntersectionPoint> IntersectionLinePolygon(Vec2d v1, Vec2d v2, List<Vec2d> polygon)
        {
            List<IntersectionPoint> ips = new List<IntersectionPoint>(2);
            Vec2d ip;
            double fA, fB;
            // Intersect with all polygon edges
            for (int i = 0; i < polygon.Count; ++i)
            {
                Vec2d v3 = polygon[i];
                Vec2d v4 = polygon[(i + 1) % polygon.Count];
                if (MathUtilD.IntersectionLines(v1, v2, v3, v4, out fA, out fB, out ip))
                {
                    ips.Add(new IntersectionPoint(ip, fA));
                }
            }
            // Sort the list
            ips.Sort();
            return ips;
        }

        /// <summary>
        /// Returns a list of sub-polygons resulting from a split of a (closed) polygon by a line.
        /// </summary>
        /// <param name="v1">Start of line</param>
        /// <param name="v2">End of line</param>
        /// <param name="polygon">Polygon to be split</param>
        /// <returns>All resulting polygons, or the original polygon if the line does not intersect the polygon</returns>
        public static List<SubPolygon> SplitPolygonByLine(Vec2d v1, Vec2d v2, SubPolygon polygon)
        {
            // TODO cleanup and rewrite this to perform no unneeded line intersections
            List<SubPolygon> result = new List<SubPolygon>(2);
            Vec2d ip;
            double fA, fB;
            // Intersect with all polygon edges
            List<Vec2d> points = new List<Vec2d>();
            List<bool> onEdge = new List<bool>();
            List<Vec2d> finalRun = new List<Vec2d>();
            List<bool> finalRunOnEdge = new List<bool>();
            for (int i = 0; i < polygon.Count; ++i)
            {
                Vec2d v3 = polygon[i];
                // Run in progress
                if (points.Count > 0)
                {
                    points.Add(v3);
                    onEdge.Add(polygon.IsOnEdge(i));
                }
                else
                {
                    finalRun.Add(v3);
                    finalRunOnEdge.Add(polygon.IsOnEdge(i));
                }
                Vec2d v4 = polygon[(i + 1) % polygon.Count];
                MathUtilD.IntersectionLines(v1, v2, v3, v4, out fA, out fB, out ip);
                // If intersection of line with polygon segment
                if (0.0 <= fB && fB <= 1.0)
                {
                    if (points.Count == 0)
                    {
                        // Start of new run
                        points.Add(ip);
                        onEdge.Add(polygon.IsOnEdge(i));
                        if (fB > 0.0)
                        {
                            // ip != v3
                            finalRun.Add(ip);
                            finalRunOnEdge.Add(false);
                        }
                    }
                    else
                    {
                        // End of run
                        if (fB < 1.0)
                        {
                            points.Add(ip);
                            // Last edge from ip2 to ip1 is not on original polygon edge
                            onEdge.Add(false);
                        }
                        List<Vec2d> r = new List<Vec2d>(points.Count);
                        r.AddRange(points);
                        if (!Polygon.IsClockWise(r))
                        {
                            result.Add(new SubPolygon(r, onEdge.ToArray()));
                        }
                        else
                        {
                            finalRun.AddRange(r);
                            finalRunOnEdge.AddRange(onEdge);
                        }
                        points.Clear();
                        onEdge.Clear();
                        // Next run
                        points.Add(ip);
                        onEdge.Add(polygon.IsOnEdge(i));
                    }
                }
            }
            if (points.Count > 0)
            {
                finalRun.InsertRange(0, points);
                finalRunOnEdge.InsertRange(0, onEdge);
            }
            if (finalRun.Count > 0 && !Polygon.IsClockWise(finalRun))
            {
                result.Add(new SubPolygon(finalRun, finalRunOnEdge.ToArray()));
            }
            return result;
        }

        /// <summary>
        /// Returns a sorted list of intersection points for a line segment with a complex polygon.
        /// </summary>
        /// <param name="v1">Start of line segment</param>
        /// <param name="v2">End of line segment</param>
        /// <param name="polygon">Polygon to be intersected</param>
        /// <returns>All intersection points sorted to their distance on the line segment</returns>
        public static List<IntersectionPoint> IntersectionLinePolygon(Vec2d v1, Vec2d v2, Polygon polygon)
        {
            List<IntersectionPoint> ips = new List<IntersectionPoint>(2);
            Vec2d ip;
            double fA, fB;
            // Intersect with all polygon edges
            for (int j = 0; j < polygon.NumContours; ++j)
            {
                for (int i = 0; i < polygon[j].Count; ++i)
                {
                    Vec2d v3 = polygon[j][i];
                    Vec2d v4 = polygon[j][(i + 1) % polygon[j].Count];
                    if (MathUtilD.IntersectionLines(v1, v2, v3, v4, out fA, out fB, out ip))
                    {
                        ips.Add(new IntersectionPoint(ip, fA));
                    }
                }
            }
            // Sort the list
            ips.Sort();
            return ips;
        }

        public static bool OverlapsPolyLineBoxes(List<Vec2d> list, List<Rect> tileBounds)
        {
            bool result = false;
            for (int i = 0; !result && i < tileBounds.Count; i++)
            {
                result |= OverlapsPolyLineBox(list, tileBounds[i]);
            }
            return result;
        }

        public static bool OverlapsPolyLineBox(List<Vec2d> list, Rect rect)
        {
            bool result = false;
            for (int i = 0; !result && i < list.Count; i++)
            {
                result |= rect.Contains(list[i]);
            }
            for (int i = 0; !result && i < list.Count - 1; i++)
            {
                result |= IntersectsLineBox(list[i], list[i + 1], rect);
            }
            return result;
        }

        /// <summary>
        /// Determine whether the box overlaps any of the bounding boxes.
        /// </summary>
        /// <param name="box">a bounding box</param>
        /// <param name="bounds">bounding boxes</param>
        /// <returns></returns>
        public static bool OverlapsBoxBoxes(Rect box, List<Rect> bounds)
        {
            foreach (Rect bb in bounds)
            {
                if (bb.Overlaps(box))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool OverlapsPolygonPolygon(List<Vec2d> polyA, List<Vec2d> polyB)
        {
            foreach (Vec2d v in polyA)
            {
                if (Polygon.Contains(polyB, v))
                {
                    return true;
                }
            }
            foreach (Vec2d v in polyB)
            {
                if (Polygon.Contains(polyA, v))
                {
                    return true;
                }
            }
            for (int i = 0; i < polyA.Count; ++i)
            {
                int j = (i + 1) % polyA.Count;
                if (LineIntersectsPolygon(polyB, polyA[i], polyA[j]))
                {
                    return true;
                }
            }
            for (int i = 0; i < polyB.Count; ++i)
            {
                int j = (i + 1) % polyB.Count;
                if (LineIntersectsPolygon(polyA, polyB[i], polyB[j]))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Vec2d> CreateExtendedBoxAroundLine(Vec2d src, Vec2d dst, double width, double extent)
        {
            Vec2d dir = (dst - src).Normalize();
            Vec2d normal = new Vec2d(-dir.Y, dir.X);
            Vec2d extendedSrc = src - dir * extent;
            Vec2d extendedDst = dst + dir * extent;
            List<Vec2d> area = new List<Vec2d>(4);
            area.Add(extendedSrc + normal * width);
            area.Add(extendedDst + normal * width);
            area.Add(extendedDst - normal * width);
            area.Add(extendedSrc - normal * width);
            return area;
        }

        public static List<Vec2d> GetBoundsAroundLineStrip(List<Vec2d> spec, double width)
        {
            List<Vec2d> result = new List<Vec2d>(2 * spec.Count + 2);
            double halfWidth = 0.5 * width;
            Vec2d[] sideA = new Vec2d[spec.Count];
            Vec2d[] sideB = new Vec2d[spec.Count];
            for (int i = 0; i < spec.Count; ++i)
            {
                int k = i < spec.Count - 1 ? i : i - 1;
                int l = i < spec.Count - 1 ? i + 1 : i;
                Vec2d dir = (spec[l] - spec[k]).Normalize();
                Vec2d tan = new Vec2d(-dir.Y, dir.X);
                sideA[i] = spec[i] + tan * halfWidth;
                sideB[i] = spec[i] - tan * halfWidth;
            }
            result.Add(spec[0]);
            result.AddRange(sideA);
            result.Add(spec[spec.Count - 1]);
            result.AddRange(sideB);
            result.Reverse(spec.Count + 2, spec.Count);
            return result;
        }

        public static Vec2d PointOnLine(Vec2d line_p1, Vec2d line_p2, Vec2d projected_point)
        {
            double factor = PointProjectedOnLine(projected_point, line_p1, line_p2);
            Vec2d result = line_p1 + (line_p2 - line_p1) * factor;
            return result;
        }

        public static Vec2d[] LineCircleIntersection(Vec2d pointA, Vec2d pointB, Vec2d pointO, double radius)
        {
            Vec2d[] output = new Vec2d[2];
            Vec2d v1, v2;
            //Vector from point 1 to point 2
            v1 = pointB - pointA;
            //Vector from point 1 to the circle's center
            v2 = pointO - pointA;
            double dot = Vec2d.Dot(v1, v2);
            Vec2d proj1 = new Vec2d(((dot / (v1.Length2)) * v1.X), ((dot / (v1.Length2)) * v1.Y));
            Vec2d midpt = new Vec2d(pointA.X + proj1.X, pointA.Y + proj1.Y);
            double distToCenter = (midpt.X - pointO.X) * (midpt.X - pointO.X) + (midpt.Y - pointO.Y) * (midpt.Y - pointO.Y);
            if (distToCenter > radius * radius)
            {
                return output;
            }
            if (distToCenter == radius * radius)
            {
                output[0] = midpt;
                return output;
            }
            double distToIntersection;
            if (distToCenter == 0)
            {
                distToIntersection = radius;
            }
            else
            {
                distToCenter = Math.Sqrt(distToCenter);
                distToIntersection = Math.Sqrt(radius * radius - distToCenter * distToCenter);
            }
            double lineSegmentLength = 1 / v1.Length;
            v1 *= lineSegmentLength;
            v1 *= distToIntersection;
            output[0] = (midpt + v1);
            output[1] = (midpt - v1);
            return output;
        }
    }
}
