using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;
using System.IO;
using Common.Shapes;

namespace Common
{
    public class Line2i
    {
        Vec2i p1, p2;

        public Vec2i P1 { get { return p1; } }
        public Vec2i P2 { get { return p2; } }

        public Line2i(Vec2i p1, Vec2i p2) { this.p1 = p1; this.p2 = p2; }
        public Line2i(Line2i l) { this.p1 = new Vec2i(l.P1); this.p2 = new Vec2i(l.P2); }
        public Line2i(Line2 l) { this.p1 = new Vec2i(l.P1); this.p2 = new Vec2i(l.P2); }

        public float Length()
        {
            return (P2 - P1).length();
        }

        public Line2i Transform(Matrix4 matrix4)
        {
            Vec2i np1 = BaseShapei.Convert(P1, matrix4);
            Vec2i np2 = BaseShapei.Convert(P2, matrix4);
            return new Line2i(np1, np2);
        }

        public void Move(Vec2i vec2i)
        {
            p1.X += vec2i.X;
            p2.X += vec2i.X;
            p1.Y += vec2i.Y;
            p2.Y += vec2i.Y;
        }

        public Vec2i PointOnLine(float p)
        {
            return (p2 - p1) * p + p1;
        }

        public Vec2i ClosestPointOnLine(Vec2i point)
        {
            Vec2i line = P2 - P1;
            Vec2i tangent = line.Cross();
            Vec2i inters;
            float factor, factor2;
            Vec2i.Intersection(P1, P2, point, point + tangent, out factor, out factor2, out inters);

            if (factor < 0)
                inters = new Vec2i(P1);
            if (factor > 1)
                inters = new Vec2i(P2);
            return inters;
        }

        internal static bool IntersectionOnBothLines(Line2i l, Line2i l2)
        {
            Line2 lb = new Line2(l);
            Line2 l2b = new Line2(l2);
            return Line2.IntersectionOnBothLines(lb, l2b);
        }

        internal static bool IntersectionOnBothLines(Line2i l, Line2i l2, out Vec2i intersectionPoint)
        {
            Line2 lb = new Line2(l);
            Line2 l2b = new Line2(l2);
            return Line2.IntersectionOnBothLines(lb, l2b, out intersectionPoint);
        }

        public Vec2i Dir()
        {
            return p2 - p1;
        }

        internal Vec2i MidPoint()
        {
            return (p2 - p1) * 0.5f;
        }
    }

    public class Line2
    {
        Vec2 p1, p2;

        public Vec2 P1 { get { return p1; } }
        public Vec2 P2 { get { return p2; } }

        public Line2(Vec2 p1, Vec2 p2) { this.p1 = p1; this.p2 = p2; }
        public Line2(Line2 l) { this.p1 = new Vec2(l.P1); this.p2 = new Vec2(l.P2); }
        public Line2(Line2i l) { this.p1 = new Vec2(l.P1); this.p2 = new Vec2(l.P2); }

        public static implicit operator Line2(Line vec)
        {
            return new Line2((Vec2)vec.P1, (Vec2)vec.P2);
        }

        public float Length()
        {
            return (P2 - P1).length();
        }

        public float Angle(Line2 line)
        {
            Vec2 a = P2 - P1;
            a.Normalize();
            Vec2 b = line.P2 - line.P1;
            b.Normalize();
            return Vec2.AngleBetweenVectors(a, b);
        }

        public void Move(Vec2 m)
        {
            P1.Move(m);
            P2.Move(m);
        }

        public void RotateRound(Vec2 point, float angle)
        {
            P1.RotateAround(point, angle);
            P2.RotateAround(point, angle);
        }

        public void Cut(float startFactor, float endFactor)
        {
            Vec2 line = (P2 - P1);
            Vec2 np1 = startFactor * line + P1;
            Vec2 np2 = endFactor * line + P1;
            p1 = np1;
            p2 = np2;
        }

        public override string ToString()
        {
            return "Line:" + P1 + "-" + P2;
        }

        public void Lengthen(float p)
        {
            Vec2 ri = (P2 - P1);
            ri.Normalize();
            p2 = P1 + p * ri;
        }

        public void Transform(Matrix4 transf)
        {
            p1 = p1 * transf;
            p2 = p2 * transf;
        }

        public Vec2 PointOnLine(float t)
        {
            return t * (P2 - P1) + P1;
        }

        public List<Line2> pathLines(float dist)
        {
            List<Line2> path = new List<Line2>();

            float ang;

            Line2 xAxis = new Line2(new Vec2(0.0f, 0.0f), new Vec2(1.0f, 0.0f));
           
            Line2 tmpLine = new Line2(this);

            Vec2 translation;

            translation = xAxis.P1 - tmpLine.MidPoint();
            tmpLine.Translate(translation);

            ang = xAxis.Angle(tmpLine);
            if (ang >= 3.14159274f)
                ang -= 3.14159274f;

            tmpLine.RotateRound(tmpLine.MidPoint(), -ang);

            Line2 bL = new Line2(new Vec2(tmpLine.P1.X, tmpLine.P1.Y + dist), new Vec2(tmpLine.P1.X, tmpLine.P1.Y - dist));
            Line2 eL = new Line2(new Vec2(tmpLine.P2.X, tmpLine.P2.Y + dist), new Vec2(tmpLine.P2.X, tmpLine.P2.Y - dist));

            Line2 l1 = new Line2(bL.P1, eL.P1);
            Line2 l2 = new Line2(bL.P2, eL.P2);


            l1.RotateRound(tmpLine.MidPoint(), ang);
            l2.RotateRound(tmpLine.MidPoint(), ang);

            l1.Translate(-translation);
            l2.Translate(-translation);

            path.Add(l1);
            path.Add(l2);            

            return path;
        }

        public float DistanceTo(Vec2 point, out float factor)
        {
            Vec2 line = P2 - P1;
            Vec2 tangent = line.Cross();
            Vec2 inters;
            Vec2.Intersection(P1, P2, point, point + tangent, out factor, out inters);

            if (factor < 0)
                inters = P1;
            if (factor > 1)
                inters = P2;
            return (point - inters).length();
        }

        public Vec2 ClosestPointOnLine(Vec2 point)
        {
            Vec2 line = P2 - P1;
            Vec2 tangent = line.Cross();
            Vec2 inters;
            float factor;
            Vec2.Intersection(P1, P2, point, point + tangent, out factor, out inters);

            if (factor < 0)
                inters = new Vec2(P1);
            if (factor > 1)
                inters = new Vec2(P2);
            return inters;
        }

        public Vec2 IntersectionOnLine(Line2 l, out bool intersectionOnLine)
        {
            float factor;
            Vec2 inters;
            intersectionOnLine = Vec2.Intersection(P1, P2, l.P1, l.P2, out factor, out inters);
            if (factor < 0 || factor > 1 || double.IsNaN(factor))
                return null;
            return inters;
        }

        internal object IntersectionOnLineNoEdges(Line2 l)
        {
            float factor;
            Vec2 inters;
            Vec2.Intersection(P1, P2, l.P1, l.P2, out factor, out inters);
            if (factor <= 0.001 || factor >= 0.999)
                return null;
            return inters;
        }

        //---------------------------------------------------------------------

        public Vec2 lineIntersection(Line2 line, out bool intersectionOnLine)
        {            
            float x1, x2, x3, x4, y1, y2, y3, y4;
            intersectionOnLine = true;

            x1 = this.P1.X;
            y1 = this.P1.Y;
            x2 = this.p2.X;
            y2 = this.p2.Y;

            x3 = line.P1.X;
            y3 = line.P1.Y;
            x4 = line.P2.X;
            y4 = line.P2.Y;

            
            float d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            // If d is zero, there is no intersection
            if (d == 0) 
                return null;

            // Get the x and y
            float pre = (x1 * y2 - y1 * x2), post = (x3 * y4 - y3 * x4);
            float x = (pre * (x3 - x4) - (x1 - x2) * post) / d;
            float y = (pre * (y3 - y4) - (y1 - y2) * post) / d;

            // Check if the x and y coordinates are within both lines
            if (x < Math.Min(x1, x2) || x > Math.Max(x1, x2) || x < Math.Min(x3, x4) || x > Math.Max(x3, x4))
                intersectionOnLine = false;
                //return null;
            if (y < Math.Min(y1, y2) || y > Math.Max(y1, y2) || y < Math.Min(y3, y4) || y > Math.Max(y3, y4))
                intersectionOnLine = false;
                //return null;

            // Return the point of intersection
            Vec2 ret = new Vec2();
            ret.X = x;
            ret.Y = y;
            return ret;
        }


        //---------------------------------------------------------------------

        public bool IsPointOnLine(Vec2 p)
        {
            float tx = (p.X - p1.X) / (p2.X - p1.X);
            float ty = (p.Y - p1.Y) / (p2.Y - p1.Y);
            if (float.IsNaN(tx) || float.IsInfinity(tx))
                return Math.Abs(p1.X + (p2.X - p1.X) * ty - p.X) < 0.001 && MathUtil.InInterval(ty, -0.001, 1.001);
            if (float.IsNaN(ty) || float.IsInfinity(ty))
                return Math.Abs(p1.Y + (p2.Y - p1.Y) * tx - p.Y) < 0.001 && MathUtil.InInterval(tx, -0.001, 1.001);
            return tx == ty && MathUtil.InInterval(tx, 0, 1);
        }

        public static bool IntersectionOnBothLines(Line2 l1, Line2 l2)
        {
            float d1, d2;
            Vec2 dummy;
            Vec2.Intersection(l1.P1, l1.P2, l2.P1, l2.P2, out d1, out dummy);
            Vec2.Intersection(l2.P1, l2.P2, l1.P1, l1.P2, out d2, out dummy);
            if (d1 > 0 && d1 < 1 && d2 > 0 && d2 < 1)
                return true;
            if (float.IsNaN(d1) || float.IsNaN(d2))
            {
                //--- parallel lines
                return l1.IsPointOnLine(l2.p1) || l1.IsPointOnLine(l2.p2) || l2.IsPointOnLine(l1.p1) || l2.IsPointOnLine(l1.p2);
            }
            return false;
        }

        public static bool IntersectionOnBothLines(Line2 l1, Line2 l2, out Vec2i intersectionPoint)
        {
            float d1, d2;
            Vec2 inters;
            Vec2.Intersection(l1.P1, l1.P2, l2.P1, l2.P2, out d1, out inters);
            Vec2.Intersection(l2.P1, l2.P2, l1.P1, l1.P2, out d2, out inters);
            if (inters == null)
                intersectionPoint = null;
            else
                intersectionPoint = new Vec2i(inters);
            if (d1 > 0 && d1 < 1 && d2 > 0 && d2 < 1)
                return true;
            if (float.IsNaN(d1) || float.IsNaN(d2))
            {
                //--- parallel lines
                return l1.IsPointOnLine(l2.p1) || l1.IsPointOnLine(l2.p2) || l2.IsPointOnLine(l1.p1) || l2.IsPointOnLine(l1.p2);
            }
            return false;
        }

        internal bool IsStraight()
        {
            return p1.X == p2.X || p1.Y == p2.Y;
        }

        public Vec2 Dir()
        {
            Vec2 dir = p2 - p1;
            dir.Normalize();
            return dir;
        }

        public Vec2 MidPoint()
        {
            return 0.5f * (p1 + p2);
        }

        public bool LeftOfLine(Vec2 vec)
        {
            Line2 l = new Line2(vec, vec + new Vec2(1, 0));
            float dummy;
            Vec2 inters;
            Vec2.Intersection(l.P1, l.P2, P1, P2, out dummy, out inters);
            return vec.X < inters.X;
        }

        public bool RelativeLeftOfLine(Vec2 vec)
        {
            //--- http://wiki.answers.com/Q/How_do_you_determine_if_a_point_lies_to_the_left_or_right_of_a_line
            float x0 = vec.X, y0 = vec.Y;
            float x1 = p1.X, y1 = p1.Y;
            float x2 = p2.X, y2 = p2.Y;
            return (x0 - x1) * (y2 - y1) - (x2 - x1) * (y0 - y1) < 0;
        }

        public bool AboveLine(Vec2 vec)
        {
            Line2 l = new Line2(vec, vec + new Vec2(0, 1));
            float dummy;
            Vec2 inters;
            Vec2.Intersection(l.P1, l.P2, P1, P2, out dummy, out inters);
            return vec.Y > inters.Y;
        }

        public void Revert()
        {
            Vec2 temp = p1;
            p1 = p2;
            p2 = temp;
        }

        public void Save(BinaryWriter w)
        {
            this.p1.Save(w);
            this.p2.Save(w);
        }

        public Line2(BinaryReader r)
        {
            this.p1 = new Vec2(r);
            this.p2 = new Vec2(r);
        }

        internal void Translate(Vec2 translation)
        {
            p1.Translate(translation);
            p2.Translate(translation);
        }
    }

    public class Line
    {
        Vec3 p1, p2;

        public Vec3 P1 { get { return p1; } }
        public Vec3 P2 { get { return p2; } }

        public Line(Vec3 p1, Vec3 p2) { this.p1 = p1; this.p2 = p2; }
        public Line(Line l) { this.p1 = new Vec3(l.P1); this.p2 = new Vec3(l.P2); }

        public float Length()
        {
            return (P2 - P1).length();
        }

        public override string ToString()
        {
            return "Line:" + P1 + "-" + P2;
        }

        public void Transform(Matrix4 transf)
        {
            p1 = p1 * transf;
            p2 = p2 * transf;
        }

        public Vec3 PointOnLine(float t)
        {
            return t * (P2 - P1) + P1;
        }

        internal bool Intersects(Vec3[,] q)
        {
            Vec3 u = q[1, 0] - q[0, 0];
            Vec3 v = q[0, 1] - q[0, 0];
            Vec3 normal = u.Cross(v);
            normal.Normalize();

            Vec3 line = P2 - P1;
            line.Normalize();

            float b = line * normal;

            Vec3 temp = P1 - q[0, 0];
            //temp.Normalize();
            float a = -(normal * temp);

            if (Math.Round(b, 10) == 0)
            {
                if (a == 0)
                    return false;
                return false;
            }

            float r = a / b;

            if (r < 0 || r > 1)
                return false;


            Vec3 inters = this.PointOnLine(r);
            double uu = u * u;
            double uv = u * v;
            double vv = v * v;

            Vec3 w = inters - q[0, 0];
            double wu = w * u;
            double wv = w * v;
            double D = uv * uv - uu * vv;

            double s = (uv * wv - vv * wu) / D;

            if (!MathUtil.InInterval(s, 0, 1))
                return false;

            double t = (uv * wu - uu * wv) / D;

            if (!MathUtil.InInterval(t, 0, 1))
                return false;

            return true;

            //double denom = (q[1, 0] - q[0, 0]).Cross(q[0, 1] - q[0, 0]);
            //double u = (inters - q[0, 0]).Cross(q[0, 1] - q[0, 0]) / denom;
            //double v = (q[1, 0] - q[0, 0]).Cross(inters - q[0, 0]) / denom;
            ////double w = 1 - u - v;

            //if (MathUtil.InInterval(u, 0, 1) && MathUtil.InInterval(v, 0, 1))
            //    return true;

            //return false;
        }

        /// <summary>
        /// Check whether the line intersects with a sphere
        /// </summary>
        /// <param name="center">The center of the sphere</param>
        /// <param name="radius">The radius of the sphere</param>
        /// <returns>True if intersection</returns>
        public bool IntersectionWithSphere(Vec3 center, double radius)
        {
            double x1 = P1.X;
            double x2 = P2.X;
            double x3 = center.X;
            double y1 = P1.Y;
            double y2 = P2.Y;
            double y3 = center.Y;
            double z1 = P1.Z;
            double z2 = P2.Z;
            double z3 = center.Z;

            double a = Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2) + Math.Pow(z2 - z1, 2);
            double b = 2 * ((x2 - x1)*(x1 - x3) + (y2 - y1)*(y1 - y3) + (z2 - z1)*(z1 - z3));
            double c = Math.Pow(x3, 2) + Math.Pow(y3, 2) + Math.Pow(z3, 2) + Math.Pow(x1, 2) + Math.Pow(y1, 2) + Math.Pow(z1, 2) - 2 * (x3 * x1 + y3 * y1 + z3 * z1) - Math.Pow(radius, 2);

            double d = b * b - 4 * a * c;

            if (d < 0)
            {
                // There is no intersection
                return false;
            }
            else if (d == 0)
            {
                // There is exactly one intersection
                return true;
            }
            else //d > 0
            {
                // There are two intersections
                return true;
            }
        }

        public Vec3 GetPointWithComponentAt(Vec3.Component component, float p)
        {
            float x1 = component == Vec3.Component.X ? p1.Y : p1.X;
            float x2 = component == Vec3.Component.X ? p2.Y : p2.X;
            float y1 = component == Vec3.Component.Z ? p1.Y : p1.Z;
            float y2 = component == Vec3.Component.Z ? p2.Y : p2.Z;

            float t1 = p1.GetComponent(component);
            float t2 = p2.GetComponent(component);

            if (t1 == t2)
                return null;
            float factor = (p - t1) / (t2 - t1);

            float x = x1 + factor * (x2 - x1);
            float y = y1 + factor * (y2 - y1);

            Vec3 ret = null;
            switch (component)
            {
                case Vec3.Component.X:
                    ret = new Vec3(p, x, y);
                    break;
                case Vec3.Component.Y:
                    ret = new Vec3(x, p, y);
                    break;
                case Vec3.Component.Z:
                    ret = new Vec3(x, y, p);
                    break;
            }
            return ret;
        }
    }
}