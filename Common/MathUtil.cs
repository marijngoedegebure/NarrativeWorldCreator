using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Common.Util;

namespace Common
{
    public class MathUtil {
        private MathUtil() {
            // Empty
        }

        public static double InterpolateLinear(double v1, double v2, double fraction) {
            return v1 * (1.0 - fraction) + v2 * fraction;
        }

        public static double InterpolateCatmullRom(double v0, double v1, double v2, double v3, double fraction) {

            return 0.5 * ((2.0 * v1) + (-v0 + v2) * fraction + (2.0 * v0 - 5.0 * v1 + 4.0 * v2 - v3) * fraction * fraction + (-v0 + 3 * v1 - 3.0 * v2 + v3) * fraction * fraction * fraction);
        }

        public static double InterpolateCubic(double v0, double v1, double v2, double v3, double fraction) {
            double p = (v3 - v2) - (v0 - v1);
            double q = (v0 - v1) - p;
            double r = v2 - v0;
            double fraction2 = fraction * fraction;
            return p * fraction * fraction2 + q * fraction2 + r * fraction + v1;
        }

        public static int PowerOf2Log2(int n) {
            for (int i = 0; i < 31; i++) {
                if ((n & 1) == 1) {
                    return i;
                }
                n >>= 1;
            }
            return 0;
        }

        public static bool IsRectangle(List<Vec2> poly) 
        {
            return IsRectangle(poly, 0.00001f);
        }

        public static bool IsRectangle(List<Vec2> poly, float epsilon) 
        {
            bool result = poly.Count == 4;
            if (result) 
            {
                // Check angles
                for (int i = 0; result && i < poly.Count - 1; ++i) 
                {
                    int j = i + 1;
                    int k = (i + 2) % poly.Count;
                    Vec2 v1 = poly[j] - poly[i];
                    Vec2 v2 = poly[k] - poly[j];
                    v1.Normalize();
                    v2.Normalize();
                    float angle = Vec2.AngleBetweenVectors(v1, v2);
                    while (angle < -Math.PI) 
                    {
                        angle += 2.0f * (float)Math.PI;
                    }
                    while (angle >= Math.PI) 
                    {
                        angle -= 2.0f * (float)Math.PI;
                    }
                    angle = Math.Abs(angle);
                    result = Math.Abs(angle - 0.5f * Math.PI) < epsilon;
                }
            }
            return result;
        }

        public static void Normalize(double[,] map) {
            //assert map != null && map.length > 0;
            double min = 0;
            double max = 0;
            // find the min and max

            for (int y = 0; y < map.GetLength(0); y++) {
                for (int x = 0; x < map.GetLength(1); x++) {
                    double z = map[y, x];
                    min = Math.Min(z, min);
                    max = Math.Max(z, max);
                }
            }
            // Avoiding divide by zero
            if (max != min) {
                // Divide every height by the maximum to normalize to [0.0, 1.0]
                for (int y = 0; y < map.GetLength(0); y++) {
                    for (int x = 0; x < map.GetLength(1); x++) {
                        map[y, x] = (map[y, x] - min) / (max - min);
                    }
                }
            }
        }

        public static bool IntersectionLines(Vec2 p1, Vec2 p2, Vec2 p3, Vec2 p4, out float factorA, out float factorB, out Vec2 intersectionPoint)
        {
            bool result = false;
            float denominator = ((p4.Y - p3.Y) * (p2.X - p1.X)) - ((p4.X - p3.X) * (p2.Y - p1.Y));
            if (Math.Abs(denominator) < 0.0001) {
                // Lines are parallel
                intersectionPoint = new Vec2(0, 0);
                factorA = float.NaN;
                factorB = float.NaN;
            } else {
                factorA = (((p4.X - p3.X) * (p1.Y - p3.Y)) - ((p4.Y - p3.Y) * (p1.X - p3.X))) / denominator;
                factorB = (((p2.X - p1.X) * (p1.Y - p3.Y)) - ((p2.Y - p1.Y) * (p1.X - p3.X))) / denominator;
                intersectionPoint = p3 + factorB * (p4 - p3);
                result = factorA >= 0.0 && factorA <= 1.0 && factorB >= 0.0 && factorB <= 1.0;
            }
            return result;
        }

        public static bool IntersectsLineBox(Vec2 s, Vec2 e, Rect box) {
            double st, et, fst = 0, fet = 1;
            for (int i = 0; i < 2; i++) {
                double si = s[i]; 
                double ei = e[i];
                double mini = box.Min[i]; 
                double maxi = box.Max[i];
                if (si < ei) {
                    if (si > maxi || ei < mini) {
                        return false;
                    }
                    double di = ei - si;
                    st = (si < mini) ? (mini - si) / di : 0;
                    et = (ei > maxi) ? (maxi - si) / di : 1;
                } else {
                    if (ei > maxi || si < mini) {
                        return false;
                    }
                    double di = ei - si;
                    st = (si > maxi) ? (maxi - si) / di : 0;
                    et = (ei < mini) ? (mini - si) / di : 1;
                }

                if (st > fst) fst = st;
                if (et < fet) fet = et;
                if (fet < fst) {
                    return false;
                }
            }
            return true;
        }
        
        public static Vec2 CenterPointOfPolygon(List<Vec2> poly) {
            float x = 0.0f;
            float y = 0.0f;
            for (int i = 0; i < poly.Count; i++) {
                x += poly[i].X;
                y += poly[i].Y;
            }
            return new Vec2(x / poly.Count, y / poly.Count);
        }

        public static float PolygonArea(List<Vec2> poly) {
            return Math.Abs(PolygonAreaSigned(poly));
        }

        public static float PolygonAreaSigned(List<Vec2> poly) {
            float area = 0.0f;
            for (int i = 0; i < poly.Count; i++) {
                int j = (i + 1) % poly.Count;
                area += poly[i].X * poly[j].Y;
                area -= poly[j].X * poly[i].Y;
            }
            area *= 0.5f;
            return area;
        }

        public static Vec2 PolygonCenterOfMass(List<Vec2> poly) {
            float cx = 0.0f;
            float cy = 0.0f;
            float area = PolygonAreaSigned(poly);
            float factor = 0.0f;
            for (int i = 0; i < poly.Count; i++) {
                int j = (i + 1) % poly.Count;
                factor = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
                cx += (poly[i].X + poly[j].X) * factor;
                cy += (poly[i].Y + poly[j].Y) * factor;
            }
            area *= 6.0f;
            factor = 1.0f / area;
            cx *= factor;
            cy *= factor;
            return new Vec2(cx, cy);
        }

        public static bool PointInPolygon(List<Vec2> poly, float x, float y)
        {
            return PointInPolygon(poly, new Vec2(x, y));
        }

        public static bool PointInPolygon(List<Vec2> poly, Vec2 point) {
            int i, j = poly.Count - 1;
            bool oddNodes = false;
            for (i = 0; i < poly.Count; i++) {
                if (poly[i].Y < point.Y && poly[j].Y >= point.Y || poly[j].Y < point.Y && poly[i].Y >= point.Y) {
                    if (poly[i].X + (point.Y - poly[i].Y) / (double)(poly[j].Y - poly[i].Y) * (poly[j].X - poly[i].X) < point.X) {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }
            return oddNodes;
        }

        public static bool PointInPolygon(List<Point> poly, Vec2 point) {
            int i, j = poly.Count - 1;
            bool oddNodes = false;
            for (i = 0; i < poly.Count; i++) {
                if (poly[i].Y < point.Y && poly[j].Y >= point.Y || poly[j].Y < point.Y && poly[i].Y >= point.Y) {
                    if (poly[i].X + (point.Y - poly[i].Y) / (double)(poly[j].Y - poly[i].Y) * (poly[j].X - poly[i].X) < point.X) {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }
            return oddNodes;
        }

        public static Rect BoundingBoxOfPolygon(List<Vec2> poly) {
            double minX = Double.MaxValue;
            double minY = Double.MaxValue;
            double maxX = 0;
            double maxY = 0;
            foreach (Vec2 v in poly) {
                minX = Math.Min(minX, v.X);
                minY = Math.Min(minY, v.Y);
                maxX = Math.Max(maxX, v.X);
                maxY = Math.Max(maxY, v.Y);
            }
            return new Rect((int)minX, (int)minY, (int)Math.Round(maxX - minX), (int)Math.Round(maxY - minY));
        }

        public static bool LineIntersectsPolygon(List<Vec2> poly, Vec2 a, Vec2 b) {
            bool found = false;
            float fA, fB;
            Vec2 ip;
            if (MathUtil.IntersectionLines(a, b, poly[0], poly[poly.Count - 1], out fA, out fB, out ip)) {
                found = true;
            }
            for (int i = 0; !found && i < poly.Count - 1; i++) {
                if (MathUtil.IntersectionLines(a, b, poly[i], poly[i + 1], out fA, out fB, out ip)) {
                    found = true;
                }
            }
            return found;
        }

        /// <summary>
        /// Merges two polygons that have one connecting edge.
        /// </summary>
        /// <param name="p1">the first polygon</param>
        /// <param name="p2">the second polygon</param>
        /// <returns>the merged polygon</returns>
        public static bool MergePolygons(List<Vec2> p1, List<Vec2> p2, out List<Vec2> mp) {
            Console.WriteLine("Merging polygons");
            mp = new List<Vec2>(p1.Count + p2.Count - 2); // Two shared vertices
            bool p1CW = MathUtil.IsClockWise(p1);
            bool p2CW = MathUtil.IsClockWise(p2);
            int indexP1 = 0;
            bool found = false;
            while (indexP1 < p1.Count) {
                Vec2 p1s = p1[indexP1];
                mp.Add(p1s);
                // Find connection to other polygon
                for (int i = 0; !found && i < p2.Count; i++) {
                    if (p2[i].Equals(p1s)) {
                        if (p2[(i + (p1CW == p2CW ? -1 : 1) + p2.Count) % p2.Count].Equals(p1[(indexP1 + 1) % p1.Count])) {
                            found = true;
                            Console.WriteLine("Found merge point");
                            bool forward = p1CW == p2CW;
                            int step = forward ? 1 : -1;
                            int indexP2 = (i + step + p2.Count) % p2.Count;
                            int end = (i - step + p2.Count) % p2.Count;
                            while (indexP2 != end) {
                                mp.Add(p2[indexP2]);
                                indexP2 = (indexP2 + step + p2.Count) % p2.Count;
                            }
                        } else {
                            Console.WriteLine("Found other point");
                        }
                    }
                }
                // Loop increment
                indexP1++;
            }
            return found;
        }

        public static double PolygonPerimeter(List<Vec2> poly) {
            double result = 0.0;
            for (int i = 0; i < poly.Count; i++) {
                int j = (i + 1) % poly.Count;
                result += (poly[j] - poly[i]).Length;
            }
            return result;
        }

        public static bool IsClockWise(List<Vec2> poly) {
            double totalAngle = 0;
            for (int i = 0; i < poly.Count; ++i) {
                int j = (i + 1) % poly.Count;
                int k = (i + 2) % poly.Count;
                Vec2 v1 = poly[j] - poly[i];
                Vec2 v2 = poly[k] - poly[j];
                v1.Normalize();
                v2.Normalize();
                double angle = Vec2.AngleBetweenVectors(v1, v2);
                while (angle < -Math.PI) {
                    angle += 2 * Math.PI;
                }
                while (angle >= Math.PI) {
                    angle -= 2 * Math.PI;
                }
                totalAngle += angle;
            }
            return totalAngle < 0.0;
        }

        public static bool IsClockWiseAlternative(List<Vec2> poly)
        {
            double result = 0.0;
            for (int i = 0; i < poly.Count; ++i)
            {
                int j = (i + 1) % poly.Count;
                result += poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
            }
            return result > 0.0; // Because of x-right, y-down axis system
        }

        public static double GetClockwiseMostAngle(Vec2 v1, Vec2 v2) {
	        // Assume v1 and v2 normalized
	        double dot = Vec2.Dot(v1, v2);
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

        public static double PointProjectedOnLine(Vec2 point, Vec2 t1, Vec2 t2) {
            return ((point.X - t1.X) * (t2.X - t1.X) + (point.Y - t1.Y) * (t2.Y - t1.Y)) / (t2 - t1).SquareLength();
        }

        public static double distanceToLineSegment(Vec2 point, Vec2 t1, Vec2 t2, out float factor) {
            factor = (float)Math.Max(0.0, Math.Min(1.0, PointProjectedOnLine(point, t1, t2)));
            Vec2 intersectionPoint = t1 + factor * (t2 - t1);
            return (point - intersectionPoint).Length;
        }

        public static double DistanceToLine(Vec2 point, Vec2 t1, Vec2 t2, out float factor) {
            factor = (float)PointProjectedOnLine(point, t1, t2);
            Vec2 intersectionPoint = t1 + factor * (t2 - t1);
            return (point - intersectionPoint).Length;
        }

        public static bool IsOnLine(Vec2 point, Vec2 source, Vec2 target) {
            float factor;
            return distanceToLineSegment(point, source, target, out factor) < 0.0001;
        }

        public static bool IsOnLine(Vec2d point, Vec2d source, Vec2d target)
        {
            float factor;
            double result;

            Vec2 point_ = new Vec2(point);
            Vec2 source_ = new Vec2(source);
            Vec2 target_ = new Vec2(target);


            result = distanceToLineSegment(point_, source_, target_, out factor);

            return result < 0.1;
        }

        public static double DistanceToLineSegment(Vec3 point, Vec3 t1, Vec3 t2) {
            double term = (point - t1).Cross(point - t2).Length;
            return term / (t2 - t1).Length;
        }

        /// <summary>
        /// Returns a sorted list of intersection points for a line segment with a (closed) polygon.
        /// </summary>
        /// <param name="v1">Start of line segment</param>
        /// <param name="v2">End of line segment</param>
        /// <param name="polygon">Polygon to be intersected</param>
        /// <returns>All intersection points sorted to their distance on the line segment</returns>
        //public static List<IntersectionPoint> IntersectionLinePolygon(Vec2 v1, Vec2 v2, List<Vec2> polygon) {
        //    List<IntersectionPoint> ips = new List<IntersectionPoint>(2);
        //    Vec2 ip;
        //    float fA, fB;
        //    // Intersect with all polygon edges
        //    for (int i = 0; i < polygon.Count; i++) {
        //        Vec2 v3 = polygon[i];
        //        Vec2 v4 = polygon[(i + 1) % polygon.Count];
        //        if (MathUtil.IntersectionLines(v1, v2, v3, v4, out fA, out fB, out ip)) {
        //            ips.Add(new IntersectionPoint(ip, fA));
        //        }
        //    }
        //    // Sort the list
        //    ips.Sort();
        //    return ips;
        //}

        #region Ouwe Maths dingen TT
        public static float EarthRadius = 6378137;

        static int[] indicesOdd = new int[] { 0, 1, 2 }, indicesEven = new int[] { 0, 2, 1 };

        public static double DifferenceBetweenAngles(double a1, double a2)
        {
            double min = Math.Min(a1, a2);
            double max = Math.Max(a1, a2);

            while (Math.Abs(max - min) > Math.PI)
                min += Math.PI * 2;
            return max - min;
        }

        public static bool InInterval(double val, double i1, double i2)
        {
            return val >= i1 && val <= i2;
        }

        static Dictionary<Vec2, int> OrthoProject(List<Vec3> list, Vec3.Component direction)
        {
            Dictionary<Vec2, int> dic = new Dictionary<Vec2, int>();
            int count = 0;
            foreach (Vec3 v in list)
            {
                Vec2 v2 = v.ProjectTo(direction);
                if (!dic.ContainsKey(v2))
                    dic.Add(v2, count);
                ++count;
            }
            return dic;
        }

        public static List<List<Vec3>> PolygonToTriangleStrips(List<Vec3> polygon)
        {
            Vec3 normal = (polygon[1] - polygon[0]).Cross(polygon[2] - polygon[0]);
            normal.Normalize();
            double max = Math.Max(Math.Abs(normal.X), Math.Max(Math.Abs(normal.Y), Math.Abs(normal.Z)));
            Vec3.Component dir = Vec3.Component.X;
            if (Math.Abs(normal.Y) == max)
                dir = Vec3.Component.Y;
            else if (Math.Abs(normal.Z) == max)
                dir = Vec3.Component.Z;

            polygon.RemoveAt(polygon.Count - 1);

            Dictionary<Vec2, int> proj = OrthoProject(polygon, dir);
            List<List<Vec2>> poly2d = PolygonToTriangleStrips(proj.Keys);

            List<List<Vec3>> triStrips = new List<List<Vec3>>();
            foreach (List<Vec2> vlist in poly2d)
            {
                List<Vec3> strip = new List<Vec3>();
                foreach (Vec2 v in vlist)
                {
                    if (!proj.ContainsKey(v))
                    {
                        strip.Add(Vec3.ProjectPointToPlane(Vec2.CreateVec3(v, dir), polygon[0], polygon[1] - polygon[0], polygon[2] - polygon[0], dir));
                    }
                    else
                        strip.Add(polygon[proj[v]]);
                }
                triStrips.Add(strip);
            }
            return triStrips;
        }

        internal static List<List<Vec2i>> PolygonToTriangleStrips(List<Vec2i> vecs)
        {
            Polygon p = new Polygon(vecs);
            TriStrip t = p.ToTriangleStrip();
            List<List<Vec2i>> ret = new List<List<Vec2i>>();
            if (t.Count == 0)
                return ret; // throw new Exception("No triangle strip");
            foreach (List<Vec2d> vl in t.Strips)
                ret.Add(Vec2i.FromVec2dList(vl));
            return ret;
        }

        private static List<List<Vec2>> PolygonToTriangleStrips(Dictionary<Vec2, int>.KeyCollection keyCollection)
        {
            List<Vec2> temp = new List<Vec2>();
            foreach (Vec2 v in keyCollection)
                temp.Add(v);
            Polygon p = new Polygon(temp);
            TriStrip t = p.ToTriangleStrip();
            List<List<Vec2>> ret = new List<List<Vec2>>();
            if (t.Count == 0)
                return ret; // throw new Exception("No triangle strip");
            foreach (List<Vec2d> vl in t.Strips)
            {
                List<Vec2> vertList = new List<Vec2>();
                foreach (Vec2d v in vl)
                    vertList.Add(GetFromList(temp, new Vec2((float)v.X, (float)v.Y)));
                ret.Add(vertList);
            }
            return ret;
        }

        internal static Vec2 GetFromList(List<Vec2> list, Vec2 vec)
        {
            double minDist = double.MaxValue;
            Vec2 ret = null;

            foreach (Vec2 v in list)
            {
                double dist = (v - vec).length();
                if ((object)ret == null || dist < minDist)
                {
                    ret = v;
                    minDist = dist;
                }
            }
            if (minDist > 0.1)
                return vec;
            return ret;
        }


        public static List<Vec2> IntersectShapes(List<Vec2> shape1, List<Vec2> shape2)
        {
            Polygon p1 = new Polygon(shape1);
            Polygon p2 = new Polygon(shape2);

            Polygon p = GpcWrapper.Clip(GpcWrapper.GpcOperation.Intersection, p1, p2);
            if (p == null || p.NumContours > 1)
                return new List<Vec2>();
            return Vec2.FromVec2dList(p[0]);
        }

        public static List<Vec2i> IntersectShapes(List<Vec2i> shape1, List<Vec2i> shape2)
        {
            Polygon p1 = new Polygon(shape1);
            Polygon p2 = new Polygon(shape2);

            Polygon p = GpcWrapper.Clip(GpcWrapper.GpcOperation.Intersection, p1, p2);
            if (p == null || p.NumContours != 1)
                return new List<Vec2i>();
            return Vec2i.FromVec2dList(p[0]);
        }

        public static Polygon Shape1minus2(List<Vec2> shape1, List<Vec2> shape2)
        {
            Polygon p1 = new Polygon(shape1);
            Polygon p2 = new Polygon(shape2);

            return GpcWrapper.Clip(GpcWrapper.GpcOperation.Difference, p1, p2);
        }

        public static PointF[] Transform(List<Vec2i> list)
        {
            PointF[] temp = new PointF[list.Count];
            int count = 0;
            foreach (Vec2i v in list)
                temp[count++] = new PointF((float)v.X, (float)v.Y);
            return temp;
        }

        //public static List<Vec2i> Transform2i(GPC.Vertex[] list)
        //{
        //    List<Vec2i> temp = new List<Vec2i>();
        //    foreach (GPC.Vertex v in list)
        //        temp.Add(new Vec2i((int)Math.Round(v.X), (int)Math.Round(v.Y)));
        //    return temp;
        //}

        public static List<List<Vec2>> PolygonToVectorLists(Polygon p)
        {
            List<List<Vec2>> list = new List<List<Vec2>>();
            if (p == null)
                return list;
            foreach (List<Vec2d> vl in p.Contours)
                list.Add(Vec2.FromVec2dList(vl));
            return list;
        }

        public static Polygon VectorListsToPolygon(List<List<Vec2>> lists)
        {
            Polygon p = new Polygon(lists.Count);
            int index = 0;
            foreach (List<Vec2> list in lists)
            {
                p.SetContour(index, Vec2d.FromVec2List(list));
                p.SetHole(index++, false);
            }
            return p;
        }

        public static void ConvertLatLonAltToXYZ(Vec3 origin, List<Vec3> coordList)
        {
            foreach (Vec3 v in coordList)
                ConvertLatLonAltToXYZ(origin, v);
        }

        public static void ConvertLatLonAltToXYZ(Vec3 origin, Vec3 coord)
        {
            float rad = (float)Math.Cos(DegreeToRadian(origin.X)) * EarthRadius;
            float latDeg = coord.X - origin.X;
            float latM = (latDeg / 180) * (float)Math.PI * EarthRadius;
            float lonDeg = coord.Y - origin.Y;
            float lonM = (lonDeg / 180) * (float)Math.PI * EarthRadius;
            float altM = coord.Z - origin.Z;
            coord.X = latM;
            coord.Y = altM;
            coord.Z = lonM;
        }

        public static double DegreeToRadian(double p)
        {
            return p / 180 * Math.PI;
        }

        public static double RadianToDegree(double p)
        {
            return p / Math.PI * 180;
        }

        public static Geometry.Shape BoxesToShape(List<Box2> boxes)
        {
            if (boxes.Count == 0)
                return null;
            Polygon p = null;
            foreach (Box2 b in boxes)
            {
                if (b.Dimensions.X > 0.0001 && b.Dimensions.Y > 0.0001)
                {
                    Polygon pb = new Polygon(b.Corners());
                    if (p == null)
                        p = pb;
                    else
                    {
                        try
                        {
                            Polygon newPoly = GpcWrapper.Clip(GpcWrapper.GpcOperation.Union, p, pb);
                            p = newPoly;
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
            List<List<Vec2>> vlists = PolygonToVectorLists(p);
            if (vlists.Count == 0)
                return null;
            return new Geometry.Shape(vlists[0]);
        }

        public static Vec2[,] Verstek(Vec2 p0, Vec2 p1, Vec2 p2, Vec2 p3, float width)
        {
            Vec2 cross = p2 - p1;
            cross.Normalize();
            cross = cross.Cross();
            float w2 = 0.5f * width;

            Vec2[,] ret = new Vec2[2, 2];
            ret[0, 0] = p1 - w2 * cross;
            ret[0, 1] = p1 + w2 * cross;
            ret[1, 0] = p2 - w2 * cross;
            ret[1, 1] = p2 + w2 * cross;

            if (p0 != null)
            {
                Vec2 cross2 = p1 - p0;
                cross2.Normalize();
                cross2 = cross2.Cross();
                if (Math.Abs(cross2.X - cross.X) > 0.001 || Math.Abs(cross2.Y - cross.Y) > 0.001)
                {
                    Vec2 inters, inters2;
                    float dummy;
                    Vec2.Intersection(p0 - w2 * cross2, p1 - w2 * cross2, ret[0, 0], ret[1, 0], out dummy,
                                            out inters);
                    Vec2.Intersection(p0 + w2 * cross2, p1 + w2 * cross2, ret[0, 1], ret[1, 1], out dummy,
                                            out inters2);
                    ret[0, 0] = inters;
                    ret[0, 1] = inters2;
                }
            }

            if (p3 != null)
            {
                Vec2 cross2 = p3 - p2;
                cross2.Normalize();
                cross2 = cross2.Cross();
                if (Math.Abs(cross2.X - cross.X) > 0.001 || Math.Abs(cross2.Y - cross.Y) > 0.001)
                {
                    Vec2 inters, inters2;
                    float dummy;
                    Vec2.Intersection(p2 - w2 * cross2, p3 - w2 * cross2, ret[0, 0], ret[1, 0], out dummy,
                                            out inters);
                    Vec2.Intersection(p2 + w2 * cross2, p3 + w2 * cross2, ret[0, 1], ret[1, 1], out dummy,
                                            out inters2);
                    ret[1, 0] = inters;
                    ret[1, 1] = inters2;
                }
            }
            return ret;
        }

        public static Vec2i[,] Verstek(Vec2i p0, Vec2i p1, Vec2i p2, Vec2i p3, int width)
        {
            Vec2[,] verstek = Verstek(p0 == null ? null : new Vec2(p0), p1 == null ? null : new Vec2(p1), 
                                        p2 == null ? null : new Vec2(p2), p3 == null ? null : new Vec2(p3), width);
            Vec2i[,] versteki = new Vec2i[2, 2];
            for(int i = 0; i < 2; ++i)
                for(int j = 0; j < 2; ++j)
                    versteki[i, j] = new Vec2i(verstek[i, j]);
            return versteki;
        }

        public static List<Vec2> TriangleStripToList(List<Vec2> strip)
        {
            List<Vec2> list = new List<Vec2>();
            for (int i = 0; i < strip.Count - 2; ++i)
            {
                int[] indices = indicesOdd;
                if (i % 2 == 1)
                    indices = indicesEven;

                foreach (int index in indices)
                    list.Add(strip[i + index]);
            }
            return list;
        }
        #endregion

        public static Vec3 CalculateTangent(Vec3 v1, Vec3 v2, Vec3 st1, Vec3 st2)
        {
            Vec3 normal = v1.Cross(v2);

            float coef = 1.0f / (st1.X * st2.Y - st2.X * st1.Y);
            Vec3 tangent = new Vec3();

            tangent.X = coef * ((v1.X * st2.Y) + (v2.X * -st1.Y));
            tangent.Y = coef * ((v1.Y * st2.Y) + (v2.Y * -st1.Y));
            tangent.Z = coef * ((v1.Z * st2.Y) + (v2.Z * -st1.Y));

            return tangent.Normalize();
        }

        public static bool IsConvex(List<Vec2> p) 
        {
            int i, j, k;
            int flag = 0;
            double z;
            if (p.Count >= 3) {
                for (i = 0; i < p.Count; ++i) {
                    j = (i + 1) % p.Count;
                    k = (i + 2) % p.Count;
                    z = (p[j].X - p[i].X) * (p[k].Y - p[j].Y);
                    z -= (p[j].Y - p[i].Y) * (p[k].X - p[j].X);
                    if (z < 0) {
                        flag |= 1;
                    } else if (z > 0) {
                        flag |= 2;
                    }
                    if (flag == 3) {
                        return false;
                    }
                }
            }
            return flag != 0;
        }

        public static float GetAngle(Vec3 u, Vec3 v) {
            return (float)Math.Acos(Vec3.Dot(u, v));
        }

        public static float GetAngle(Vec2 u, Vec2 v) 
        {
            return (float)Math.Atan2(Vec2.Dot(new Vec2(-u.Y, u.X), v), Vec2.Dot(u, v));
        }

        public static Vec3 EulerAnglesFromAxes(Vec3 xAxis, Vec3 yAxis, Vec3 zAxis) {
            // See also http://www.geometrictools.com/Documentation/EulerAngles.pdf
            float thetaX, thetaY, thetaZ;
            float epsilon = 0.001f;
            if (xAxis.Z + epsilon < 1) {
                if (xAxis.Z - epsilon > -1) {
                    thetaY = (float)Math.Asin(-xAxis.Z);
                    thetaZ = (float)Math.Atan2(xAxis.Y, xAxis.X);
                    thetaX = (float)Math.Atan2(yAxis.Z, zAxis.Z);
                } else { // xAxis.Z = -1
                    // Not a unique solution: thetaX - thetaZ = atan2(-zAxis.Y,yAxis.Y)
                    thetaY = (float)(+Math.PI / 2.0);
                    thetaZ = (float)Math.Atan2(-zAxis.Y, yAxis.Y);
                    thetaX = 0;
                }
            } else { // xAxis.Z = +1
                // Not a unique solution: thetaX + thetaZ = atan2(-zAxis.Y,yAxis.Y)
                thetaY = (float)(-Math.PI / 2.0);
                thetaZ = (float)Math.Atan2(-zAxis.Y, yAxis.Y);
                thetaX = 0;
            }
            return new Vec3(thetaX, thetaY, thetaZ);
        }

        public static Plane BisectingPlane(Vec3 o, Vec3 a, Vec3 b) {
            Vec3 va = a - o;
            Vec3 vb = b - o;
            Vec3 c = va + vb;
            c.Normalize();
            Vec3 normal = va.Cross(vb).Normalize();
            Plane result = new Plane(o, c.Cross(normal));
            return result;
        }

		public static float AreaOfTriangle(Vec3 v0, Vec3 v1, Vec3 v2) {
			return Vec3.Cross(v1 - v0, v2 - v0).Length / 2.0f;
		}


    }
}
