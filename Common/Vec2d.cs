using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace Common
{
    [Serializable]
    public class Vec2d
    {
        public static readonly Vec2d UnitX = new Vec2d(1, 0);
        public static readonly Vec2d UnitY = new Vec2d(0, 1);

        private const string XML_ELEMENT_X = "x";
        private const string XML_ELEMENT_Y = "y";

        public double this[int index]
        {
            get { return index == 0 ? x : index == 1 ? y : 0.0; }
            set { if (index == 0) x = value; else if (index == 1) y = value; }
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(x * x + y * y);
            }
        }

        public double Length2
        {
            get
            {
                return x * x + y * y;
            }
        }

        private double x;
        private double y;

        public Vec2d()
            : this(0.0, 0.0)
        {
        }

        public Vec2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Vec2d(Vec2d v)
        {
            this.x = v.X;
            this.y = v.Y;
        }

        public Vec2d(Vec2 v)
        {
            this.x = v.X;
            this.y = v.Y;
        }

        public Vec2d(Vec2i v)
        {
            this.x = v.X;
            this.y = v.Y;
        }

        public Vec2d(Point p)
        {
            this.x = p.X;
            this.y = p.Y;
        }

        public Vec2d(PointF p)
        {
            this.x = p.X;
            this.y = p.Y;
        }

        public static implicit operator Vec2d(Point p)
        {
            return new Vec2d(p.X, p.Y);
        }

        public void setValues(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double SquareLength()
        {
            return x * x + y * y;
        }

        public static double Distance(Vec2d a, Vec2d b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        public static double DistanceSquared(Vec2d a, Vec2d b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
        }

        public static Vec2d operator +(Vec2d left, Vec2d right)
        {
            Vec2d result = new Vec2d(0, 0);
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            return result;
        }

        public static Vec2d operator -(Vec2d left, Vec2d right)
        {
            Vec2d result = new Vec2d(0, 0);
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            return result;
        }

        public static Vec2d operator -(Vec2d vec)
        {
            Vec2d result = new Vec2d(-vec.X, -vec.Y);
            return result;
        }

        public static Vec2d operator *(Vec2d vec, double d)
        {
            Vec2d result = new Vec2d(vec.X * d, vec.Y * d);
            return result;
        }

        public static Vec2d operator *(double d, Vec2d vec)
        {
            return vec * d;
        }

        public static Vec2d operator /(Vec2d vec, double d)
        {
            double mult = 1.0 / d;
            return vec * mult;
        }

        public static double Dot(Vec2d left, Vec2d right)
        {
            return left.X * right.X + left.Y * right.Y;
        }

        public static double PerpDot(Vec2d v1, Vec2d v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        public Vec2d Normalize()
        {
            double length = this.Length;
            if (length > 0)
            {
                double scale = 1.0 / length;
                x *= scale;
                y *= scale;
            }
            return this;
        }

        public void Translate(Vec2d m)
        {
            x += m.X;
            y += m.Y;
        }

        public void RotateAround(Vec2d point, double angle)
        {
            Vec2d line = this - point;
            line.Rotate(angle);
            this.x = point.X + line.X;
            this.y = point.Y + line.Y;
        }

        public void Rotate(double angle)
        {
            double currentAngle = this.GetAngle();
            double len = this.Length;
            currentAngle += angle;
            this.x = len * Math.Cos(currentAngle);
            this.y = len * Math.Sin(currentAngle);
        }

        public static Vec2d Rotate(Vec2d v, double angle)
        {
            double currentAngle = v.GetAngle();
            double len = v.Length;
            currentAngle += angle;
            Vec2d result = new Vec2d(len * Math.Cos(currentAngle), len * Math.Sin(currentAngle));
            return result;
        }

        public static double DistanceToLine(Vec2d point, Vec2d t1, Vec2d t2, out Vec2d intersectionPoint)
        {
            double factor = Math.Max(0, Math.Min(1.0, ((point.X - t1.X) * (t2.X - t1.X) + (point.Y - t1.Y) * (t2.Y - t1.Y)) / (t2 - t1).SquareLength()));
            intersectionPoint = t1 + factor * (t2 - t1);
            return (point - intersectionPoint).Length;
        }

        /// <summary>
        /// Calculate the intersection point between the two lines.
        /// </summary>
        /// <param name="p0">The start of the first line.</param>
        /// <param name="p1">The end of the first line.</param>
        /// <param name="p2">The start of the second line.</param>
        /// <param name="p3">The end of the second line.</param>
        /// <param name="intersection">The calculated intersection point.</param>
        /// <returns>True iff an intersection point has been found.</returns>
        public static bool intersectionPoint(Vec2d p0, Vec2d p1, Vec2d p2, Vec2d p3, out Vec2d intersection)
        {
            return intersectionPoint(p0, p1, p2, p3, double.Epsilon, out intersection);
        }


        /// <summary>
        /// Calculate the intersection point between the two lines.
        /// </summary>
        /// <param name="p0">The start of the first line.</param>
        /// <param name="p1">The end of the first line.</param>
        /// <param name="p2">The start of the second line.</param>
        /// <param name="p3">The end of the second line.</param>
        /// <param name="accuracy">The accuracy of the algorithm.</param>
        /// <param name="intersection">The calculated intersection point.</param>
        /// <returns>True iff an intersection point has been found.</returns>
        public static bool intersectionPoint(Vec2d p0, Vec2d p1, Vec2d p2, Vec2d p3, double accuracy, out Vec2d intersection)
        {
            intersection = new Vec2d(0, 0);

            // If d is zero, there is no intersection
            double d = (p0.X - p1.X) * (p2.Y - p3.Y) - (p0.Y - p1.Y) * (p2.X - p3.X);
            if (Math.Abs(d) < accuracy)
                return false;

            // Get the x and y
            double pre = (p0.X * p1.Y - p0.Y * p1.X),
                post = (p2.X * p3.Y - p2.Y * p3.X);
            double x = (pre * (p2.X - p3.X) - (p0.X - p1.X) * post) / d;
            double y = (pre * (p2.Y - p3.Y) - (p0.Y - p1.Y) * post) / d;

            // Check if the x and y coordinates are within both lines
            if (x < Math.Min(p0.X, p1.X) || x > Math.Max(p0.X, p1.X) || x < Math.Min(p2.X, p3.X) || x > Math.Max(p2.X, p3.X))
                return false;
            if (y < Math.Min(p0.Y, p1.Y) || y > Math.Max(p0.Y, p1.Y) || y < Math.Min(p2.Y, p3.Y) || y > Math.Max(p2.Y, p3.Y))
                return false;

            // Return the point of intersection
            intersection = new Vec2d(x, y);
            return true;
        }

        public override string ToString()
        {
            return String.Format("[{0:f}, {1:f}]", x, y);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Vec2d)
            {
                if (this == obj)
                {
                    return true;
                }
                Vec2d o = (Vec2d)obj;
                if (Math.Abs(this.X - o.X) < 0.01 && Math.Abs(this.Y - o.Y) < 0.01)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            double PRIME = 31;
            double result = 1;
            result = PRIME * result + ((Double)x).GetHashCode();
            result = PRIME * result + ((Double)y).GetHashCode();
            return (int)result;
        }

        public double GetAngle()
        {
            Vec2d norm = new Vec2d(this);
            norm.Normalize();
            double ang = Math.Acos(norm.X);
            if (norm.Y < 0)
            {
                ang = 2 * Math.PI - ang;
            }
            return ang;
        }

        public static double AngleBetweenVectors(Vec2d v1, Vec2d v2)
        {
            if (v1.X == 0 && v1.Y == 0)
            {
                return 0.0;
            }
            if (v2.X == 0 && v2.Y == 0)
            {
                return 0.0;
            }
            double angle1 = v1.GetAngle();
            double angle2 = v2.GetAngle();
            return angle2 - angle1;
        }

        public static PointF[] ToPointFArray(List<Vec2d> v2s)
        {
            return ToPointFArray(v2s, 1.0, 0.0, 0.0);
        }

        public static PointF[] ToPointFArray(List<Vec2d> v2s, double scale)
        {
            return ToPointFArray(v2s, scale, 0.0, 0.0);
        }

        /// <summary>
        /// Utility method for debug drawing of polygons, lines etc.
        /// </summary>
        /// <param name="v2s"></param>
        /// <param name="scale">scale to apply to vertices</param>
        /// <returns></returns>
        public static PointF[] ToPointFArray(List<Vec2d> v2s, double scale, double xTrans, double yTrans)
        {
            PointF[] result = new PointF[v2s.Count];
            for (int i = 0; i < v2s.Count; i++)
            {
                result[i] = v2s[i].ToPointF(scale, xTrans, yTrans);
            }
            return result;
        }

        public Point ToPoint()
        {
            return new Point((int)this.x, (int)this.y);
        }

        public PointF ToPointF()
        {
            return ToPointF(1.0, 0.0, 0.0);
        }

        public PointF ToPointF(double scale)
        {
            return ToPointF(scale, 0.0, 0.0);
        }

        public PointF ToPointF(double scale, double xTrans, double yTrans)
        {
            return new PointF((float)(scale * (this.X - xTrans)), (float)(scale * (this.Y - yTrans)));
        }

        public Vec2i ToVec2i()
        {
            Vec2i output = new Vec2i(0, 0);

            output.setValues((int)this.x, (int)this.y);

            return output;
        }

        public static List<Point> ToPointList(List<Vec2d> vs)
        {
            List<Point> result = new List<Point>(vs.Count);
            for (int i = 0; i < vs.Count; i++)
            {
                result.Add(vs[i].ToPoint());
            }
            return result;
        }

        public static List<Vec2d> FromVec2iList(List<Vec2i> points)
        {
            List<Vec2d> result = new List<Vec2d>(points.Count);
            foreach (Vec2i vi in points)
            {
                result.Add(new Vec2d(vi));
            }
            return result;
        }

        internal static List<Vec2d> FromVec2List(List<Vec2> points)
        {
            List<Vec2d> result = new List<Vec2d>(points.Count);
            foreach (Vec2 v in points)
            {
                result.Add(new Vec2d(v));
            }
            return result;
        }

        public static Vec2[] ToVec2Array(Vec2d[] vs)
        {
            Vec2[] result = new Vec2[vs.Length];
            for (int i = 0; i < vs.Length; ++i)
            {
                result[i] = new Vec2(vs[i]);
            }
            return result;
        }


        public static List<Vec2> ToVec2List(List<Vec2d> vs)
        {
            List<Vec2> result = new List<Vec2>(vs.Count);
            for (int i = 0; i < vs.Count; ++i)
            {
                result.Add(new Vec2(vs[i]));
            }
            return result;
        }

        public static Vec2d[] FromVec2Array(Vec2[] vs)
        {
            Vec2d[] result = new Vec2d[vs.Length];
            for (int i = 0; i < vs.Length; ++i)
            {
                result[i] = new Vec2d(vs[i]);
            }
            return result;
        }

        public static List<Vec2d> CloneList(List<Vec2d> other)
        {
            List<Vec2d> result = new List<Vec2d>(other.Count);
            foreach (Vec2d v in other)
            {
                result.Add(v.Clone());
            }
            return result;
        }

        public static bool isEqual(Vec2d left, Vec2d right)
        {
            return (left.X == right.X && left.Y == right.Y);
        }

        public Vec2d Clone()
        {
            return new Vec2d(this.X, this.Y);
        }

        public static Vec2d FromXML(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement(XML_ELEMENT_X);
            double x = reader.ReadContentAsDouble();
            reader.ReadEndElement();
            reader.ReadStartElement(XML_ELEMENT_Y);
            double y = reader.ReadContentAsDouble();
            reader.ReadEndElement();
            return new Vec2d(x, y);
        }

        public void ToXML(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement(XML_ELEMENT_X);
            writer.WriteValue(this.X);
            writer.WriteEndElement();
            writer.WriteStartElement(XML_ELEMENT_Y);
            writer.WriteValue(this.Y);
            writer.WriteEndElement();
        }
    }

    // TODO: Move to separate class
    public class LineVec2 : QuickGraph.IEdge<Vec2d>
    {

        #region IEdge<Vec2> Members
        public Vec2d Source
        {
            get { return start; }
            set { start = value; }
        }

        public Vec2d Target
        {
            get { return end; }
            set { end = value; }
        }
        #endregion

        public Vec2d A
        {
            get { return start; }
            set { start = value; }
        }

        public Vec2d B
        {
            get { return end; }
            set { end = value; }
        }

        public virtual double Width
        {
            get;
            private set;
        }

        public double Length
        {
            get { return Vec2d.Distance(start, end); }
        }

        private Vec2d start;
        private Vec2d end;

        public LineVec2(Vec2d point1, Vec2d point2) 
            : this(point1, point2, 0.0)
        {
        }

        public LineVec2(Vec2d point1, Vec2d point2, double width)
        {
            start = point1;
            end = point2;
            Width = width;
        }

        public Vec2d GetPoint(double factor)
        {
            return start + factor * (end - start);
        }

        public double length()
        {
            return Vec2d.Distance(start, end);
        }

        public double distance(Vec2d point)
        {
            Vec2d intersection;
            return Vec2d.DistanceToLine(point, start, end, out intersection);
        }

        public override string ToString()
        {
            return ("Line: " + start.ToString() + ",  " + end.ToString());
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj != null && obj is LineVec2)
            {
                LineVec2 o = (LineVec2)obj;
                result = this.Source.Equals(o.Source) && this.Target.Equals(o.Target);
            }
            return result;
        }

        public override int GetHashCode()
        {
            double PRIME = 31;
            double result = 1;
            result = PRIME * result + ((Double)this.Source.X).GetHashCode();
            result = PRIME * result + ((Double)this.Source.Y).GetHashCode();
            result = PRIME * result + ((Double)this.Target.X).GetHashCode();
            result = PRIME * result + ((Double)this.Target.Y).GetHashCode();
            return (int)result;
        }

        public Point closestPointOnLine(Point p)
        {
            //return the closest point on the line to a point p

            double r_numerator = (p.X - this.Source.X) * (this.Target.X - this.Source.X) + (p.Y - this.Source.Y) * (this.Target.Y - this.Source.Y);
            double r_denomenator = (this.Target.X - this.Source.X) * (this.Target.X - this.Source.X) + (this.Target.Y - this.Source.Y) * (this.Target.Y - this.Source.Y);
            double r = r_numerator / r_denomenator;

            Point closest = new Point();
            closest.X = (int)(this.Source.X + r * (this.Target.X - this.Source.X));
            closest.Y = (int)(this.Source.Y + r * (this.Target.Y - this.Source.Y));

            if (closest == new Point(int.MinValue, int.MinValue) || closest == new Point(int.MaxValue, int.MaxValue))
                closest = Point.Empty;

            return closest;
        }

        public int PositionOfPoint(Vec2d p)
        {
            return Math.Sign((B.X - A.X) * (p.Y - A.Y) - (B.Y - A.Y) * (p.X - A.X));
        }

        /// <summary>
        /// Tries to find the intersection between this line and the given line.
        /// </summary>
        /// <param name="line">The line to intersect with.</param>
        /// <param name="intersection">The intersection point.</param>
        /// <returns>True iff an intersection has been found.</returns>
        public bool intersectionPoint(LineVec2 line, out Vec2d intersection)
        {
            return intersectionPoint(line, double.Epsilon, out intersection);
        }

        /// <summary>
        /// Tries to find the intersection between this line and the given line.
        /// </summary>
        /// <param name="line">The line to intersect with.</param>
        /// <param name="intersection">The intersection point.</param>
        /// <param name="accuracy">The accuracy of the algorithm.</param>
        /// <returns>True iff an intersection has been found.</returns>
        public bool intersectionPoint(LineVec2 line, double accuracy, out Vec2d intersection)
        {
            return Vec2d.intersectionPoint(this.start, this.end, line.start, line.end, accuracy, out intersection);
        }
    }
}
