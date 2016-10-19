using System;
using System.Collections.Generic;
using System.Drawing;

namespace Common
{
    [Serializable]
    public class Vec2 : IDisposable, IStringRepresentative
    {

        #region Event: Vec2Handler
        /// <summary>
        /// A handler for the Vec2Handler event.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        public delegate void Vec2Handler(Vec2 vector);

        /// <summary>
        /// An event for a changed value (X or Y).
        /// </summary>
        public event Vec2Handler ValueChanged;
        #endregion Event: Vec2Handler

        static float standardComparisonPrecision = 0.01f;
        static Stack<float> comparisonPrecision = new Stack<float>();
        static float CompPrecision()
        {
            if (comparisonPrecision.Count == 0)
                return standardComparisonPrecision;
            return comparisonPrecision.Peek();
        }

        public static void PushComparisonPrecision(float p)
        {
            comparisonPrecision.Push(p);
        }

        public static void PopComparisonPrecision()
        {
            comparisonPrecision.Pop();
        }


        public enum Component { X, Y };

        public static readonly Vec2 Min = new Vec2(float.MinValue, float.MinValue);
        public static readonly Vec2 Max = new Vec2(float.MaxValue, float.MaxValue);
        public static readonly Vec2 UnitX = new Vec2(1, 0);
        public static readonly Vec2 UnitY = new Vec2(0, 1);

        public float this[int index] {
            get { return index == 0 ? x : index == 1 ? y : 0.0f; }
            set { if (index == 0) x = value; else if (index == 1) y = value; }
        }

        public float X {
            get { return x; }
            set
            {
                x = value;
                if (ValueChanged != null)
                    ValueChanged(this);
            }
        }

        public float Y {
            get { return y; }
            set 
            {
                y = value;
                if (ValueChanged != null)
                    ValueChanged(this);
            }
        }

        public float Length {
            get {
                return (float)Math.Sqrt(x * x + y * y);
            }
        }

        public float Length2 {
            get {
                return x * x + y * y;
            }
        }

        private float x;
        private float y;

        public static Vec2 Zero
        {
            get { return new Vec2(0, 0); }
        }
        public static Vec2 One
        {
            get { return new Vec2(1, 1); }
        }

        public Vec2()
        {
            this.x = 0;
            this.y = 0;
        }

        public Vec2(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public Vec2(Vec2d v) {
            this.x = (float)v.X;
            this.y = (float)v.Y;
        }

        public Vec2(Vec2 v) {
            this.x = v.X;
            this.y = v.Y;
        }

        public Vec2(Vec2i v) {
            this.x = v.X;
            this.y = v.Y;
        }

        public Vec2(Point p) {
            this.x = p.X;
            this.y = p.Y;
        }

        public Vec2(PointF p) {
            this.x = p.X;
            this.y = p.Y;
        }

        public Vec2(float[] parameters)
        {
            this.x = parameters[0];
            this.y = parameters[1];
        }

        public static implicit operator Vec2(Point p) {
            return new Vec2(p.X, p.Y);
        }

        public bool compare(Vec2 v)
        {
            if (Math.Round(this.X) == Math.Round(v.X) && Math.Round(this.Y) == Math.Round(v.Y))
                return true;
            else
                return false;
        }

        public bool compare(Vec2d v)
        {
            if (Math.Round(this.X) == Math.Round(v.X) && Math.Round(this.Y) == Math.Round(v.Y))
                return true;
            else
                return false;
        }

        public void setValues(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public float SquareLength() {
            return x * x + y * y;
        }

        public static float Distance(Vec2 a, Vec2 b) {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        public static float DistanceSquared(Vec2 a, Vec2 b) {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
        }

        public static Vec2 operator +(Vec2 left, Vec2 right) {
            Vec2 result = new Vec2(0, 0);
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            return result;
        }

        public static Vec2 operator -(Vec2 left, Vec2 right) {
            Vec2 result = new Vec2(0, 0);
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            return result;
        }

        public static Vec2 operator -(Vec2 vec) {
            Vec2 result = new Vec2(-vec.X, -vec.Y);
            return result;
        }

        public static Vec2 operator *(Vec2 vec, float d) {
            Vec2 result = new Vec2(vec.X * d, vec.Y * d);
            return result;
        }

        public static Vec2 operator *(float d, Vec2 vec) {
            return vec * d;
        }

        public static float operator *(Vec2 v1, Vec2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static Vec2 operator /(Vec2 vec, float d) {
            float mult = 1.0f / d;
            return vec * mult;
        }

        public static bool operator >(Vec2 v1, Vec2 v2)
        {
            return (v1.X > v2.X && v1.Y > v2.Y);
        }

        public static bool operator <(Vec2 v1, Vec2 v2)
        {
            return (v1.X < v2.X && v1.Y < v2.Y);
        }

        public static bool operator >=(Vec2 v1, Vec2 v2)
        {
            return (v1.X >= v2.X && v1.Y >= v2.Y);
        }

        public static bool operator <=(Vec2 v1, Vec2 v2)
        {
            return (v1.X <= v2.X && v1.Y <= v2.Y);
        }

        public static implicit operator Vec2(Vec3 vec)
        {
            return new Vec2(vec.X, vec.Z);
        }

        public float GetComponent(Component component)
        {
            return this[(int)component];
        }

        public void SetComponent(Component component, float value)
        {
            this[(int)component] = value;
        }

        public float GetComponent(Vec3.Component component3d)
        {
            switch (component3d)
            {
                case Vec3.Component.X:
                    return X;
                case Vec3.Component.Z:
                    return Y;
            }
            throw new Exception("No 3d Y component in 2d vector!");
        }

        public void SetComponent(Vec3.Component component3d, float p)
        {
            switch (component3d)
            {
                case Vec3.Component.X:
                    X = p;
                    break;
                case Vec3.Component.Z:
                    Y = p;
                    break;
                default:
                    throw new Exception("No 3d Y component in 2d vector!");
            }
        }

        public static float Dot(Vec2 left, Vec2 right) {
            return left.X * right.X + left.Y * right.Y;
        }

        public static float PerpDot(Vec2 v1, Vec2 v2) {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        public Vec2 Normalize() {
            float length = this.Length;
            if (length > 0) {
                float scale = 1.0f / length;
                X *= scale;
                Y *= scale;
            }
            return this;
        }

        internal Vec2 normalize()
        {
            float length = this.Length;
            if (length > 0)
            {
                float scale = 1.0f / length;
                return new Vec2(X * scale, Y * scale);
            }
            return null;
        }

        public static float DistanceToLine(Vec2 point, Vec2 t1, Vec2 t2, out Vec2 intersectionPoint) {
            float factor = (float)Math.Max(0, Math.Min(1.0, ((point.X - t1.X) * (t2.X - t1.X) + (point.Y - t1.Y) * (t2.Y - t1.Y)) / (t2 - t1).SquareLength()));
            intersectionPoint = t1 + factor * (t2 - t1);
            return (point - intersectionPoint).Length;
        }

        public override string ToString() {
            return String.Format("[{0:f}, {1:f}]", x, y);
        }

        public override bool Equals(object obj) {
            if (obj != null && obj is Vec2) {
                if (this == obj) {
                    return true;
                }
                Vec2 o = (Vec2)obj;
                if (Math.Abs(this.X - o.X) < CompPrecision() && Math.Abs(this.Y - o.Y) < CompPrecision())
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode() {
            float PRIME = 31;
            float result = 1;
            result = PRIME * result + ((float)x).GetHashCode();
            result = PRIME * result + ((float)y).GetHashCode();
            return (int)result;
        }

        public float GetAngle() {
            Vec2 norm = new Vec2(this);
            norm.Normalize();
            float ang = (float)Math.Acos(norm.X);
            if (norm.Y < 0) {
                ang = 2.0f * (float)Math.PI - ang;
            }
            return ang;
        }

        public static float AngleBetweenVectors(Vec2 v1, Vec2 v2) {
            if (v1.X == 0 && v1.Y == 0) {
                return 0.0f;
            }
            if (v2.X == 0 && v2.Y == 0) {
                return 0.0f;
            }
            float angle1 = v1.GetAngle();
            float angle2 = v2.GetAngle();
            return angle2 - angle1;
        }

        public static PointF[] ToPointFArray(List<Vec2> v2s) {
            return ToPointFArray(v2s, 1.0f, 0.0f, 0.0f);
        }

        public static PointF[] ToPointFArray(List<Vec2> v2s, float scale) {
            return ToPointFArray(v2s, scale, 0.0f, 0.0f);
        }

        /// <summary>
        /// Utility method for debug drawing of polygons, lines etc.
        /// </summary>
        /// <param name="v2s"></param>
        /// <param name="scale">scale to apply to vertices</param>
        /// <returns></returns>
        public static PointF[] ToPointFArray(List<Vec2> v2s, float scale, float xTrans, float yTrans) {
            PointF[] result = new PointF[v2s.Count];
            for (int i = 0; i < v2s.Count; i++) {
                result[i] = v2s[i].ToPointF(scale, xTrans, yTrans);
            }
            return result;
        }

        public Point ToPoint() {
            return new Point((int)this.x, (int)this.y);
        }

        public PointF ToPointF() {
            return ToPointF(1.0f, 0.0f, 0.0f);
        }

        public PointF ToPointF(float scale) {
            return ToPointF(scale, 0.0f, 0.0f);
        }

        public PointF ToPointF(float scale, float xTrans, float yTrans) {
            return new PointF((float)(scale * (this.X + xTrans)), (float)(scale * (this.Y + yTrans)));  
        }

        public static List<Point> ToPointList(List<Vec2> vs) {
            List<Point> result = new List<Point>(vs.Count);
            for (int i = 0; i < vs.Count; i++) {
                result.Add(vs[i].ToPoint());
            }
            return result;
        }

        public static List<Vec2> FromVec2iList(List<Vec2i> points) {
            List<Vec2> result = new List<Vec2>(points.Count);
            foreach (Vec2i vi in points) {
                result.Add(new Vec2(vi));
            }
            return result;
        }

        public static List<Vec2> FromVec2dList(List<Vec2d> points)
        {
            List<Vec2> result = new List<Vec2>(points.Count);
            foreach (Vec2d vd in points)
            {
                result.Add(new Vec2(vd));
            }
            return result;
        }

        public static List<Vec2> FromVec2dArray(Vec2d[] points)
        {
            List<Vec2> result = new List<Vec2>(points.Length);
            foreach (Vec2d vd in points)
            {
                result.Add(new Vec2(vd));
            }
            return result;
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion


        public float squareLength() { return x * x + y * y; }

        public float length() { return (float)Math.Sqrt(squareLength()); }

        public void Move(Vec2 m)
        {
            X += m.X;
            Y += m.Y;
        }

        internal string ToFormattedString()
        {
            return String.Format("{0:000.00}, {1:000.00}", X, Y);
        }

        public float DistanceToPoint(Vec2 p)
        {
            return (float)Math.Sqrt((double)((this.X - p.X) * (this.X - p.X) + (this.Y - p.Y) * (this.Y - p.Y)));
        }


        internal float DistanceToLine(Vec2 t1, Vec2 t2, out bool intersectionOnLine, out float factor)
        {
            factor = ((X - t1.X) * (t2.X - t1.X) + (Y - t1.Y) * (t2.Y - t1.Y)) / (t2 - t1).squareLength();
            intersectionOnLine = factor >= -0.001 && factor <= 1.001;
            Vec2 intersection = t1 + factor * (t2 - t1);
            return (this - intersection).length();
        }

        public float DistanceToLine(Line2 l, out bool intersectionOnLine, out float factor)
        {
            return DistanceToLine(l.P1, l.P2, out intersectionOnLine, out factor);
        }

        public static bool IsOnLine(Vec2 point, Vec2 lineA, Vec2 lineB)
        {
            bool dummy;
            float dummy2;
            float dist = point.DistanceToLine(lineA, lineB, out dummy, out dummy2);

            return dist < 0.0001;
        }

        public static bool Intersection(Vec2 p1, Vec2 p2, Vec2 p3, Vec2 p4, out float intersectionFactor, out Vec2 intersectionPoint)
        {
            float dummy;
            return Intersection(p1, p2, p3, p4, out intersectionFactor, out dummy, out intersectionPoint);
        }

        public static bool Intersection(Vec2 p1, Vec2 p2, Vec2 p3, Vec2 p4, out float intersectionFactor, out float intersectionFactor2, 
                                            out Vec2 intersectionPoint)
        {
            //--- see: http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline2d/
            float ua_denominator = ((p4.Y - p3.Y) * (p2.X - p1.X)) - ((p4.X - p3.X) * (p2.Y - p1.Y));
            if (ua_denominator == 0)
            {
                intersectionFactor = float.NaN;
                intersectionFactor2 = float.NaN;
                intersectionPoint = new Vec2(0, 0);
                return false;
            }
            intersectionFactor = (((p4.X - p3.X) * (p1.Y - p3.Y)) - ((p4.Y - p3.Y) * (p1.X - p3.X))) / ua_denominator;
            intersectionFactor2 = (((p2.X - p1.X) * (p1.Y - p3.Y)) - ((p2.Y - p1.Y) * (p1.X - p3.X))) / ua_denominator;

            intersectionPoint = p3 + intersectionFactor2 * (p4 - p3);

            return intersectionFactor >= 0 && intersectionFactor <= 1 && intersectionFactor2 >= 0 && intersectionFactor2 <= 1;
        }

        public Vec2 Cross(Vec2 p)
        {
            Vec2 test = p - this;
            test.Normalize();
            return new Vec2(test.Y, -test.X);
        }

        public Vec2 Cross()
        {
            return new Vec2(Y, -X);
        }

        internal static Vec3 CreateVec3(Vec2 v, Vec3.Component missingCoordinate)
        {
            switch (missingCoordinate)
            {
                case Vec3.Component.X:
                    return new Vec3(0, v.x, v.y);
                case Vec3.Component.Y:
                    return new Vec3(v.x, 0, v.y);
                case Vec3.Component.Z:
                    return new Vec3(v.x, v.y, 0);
            }
            return null;
        }

        internal void RotateAround(Vec2 point, float angle)
        {
            Vec2 line = this - point;
            line.Rotate(angle);
            X = point.X + line.X;
            Y = point.Y + line.Y;
        }

        public void Rotate(float angle)
        {
            float currentAngle = this.GetAngle();
            float len = this.length();
            currentAngle += angle;
            X = len * (float)Math.Cos(currentAngle);
            Y = len * (float)Math.Sin(currentAngle);
        }

        #region Save/Load
        public void Save(System.IO.BinaryWriter wr)
        {
            wr.Write((double)this.x);
            wr.Write((double)this.y);
        }

        internal Vec2(System.IO.BinaryReader br)
        {
            x = (float)br.ReadDouble();
            y = (float)br.ReadDouble();
        }

        public static Vec2 Load(System.IO.BinaryReader r)
        {
            return new Vec2(r);
        }
        #endregion

        public Vec2 Transform(Matrix4 transformation)
        {
            return (Vec2)(((Vec3)this) * transformation);
        }

        public Vec3 V3(float YpositionOfVec3)
        {
            return new Vec3(x, YpositionOfVec3, y);
        }

        internal void Translate(Vec2 translation)
        {
            X += translation.x;
            Y += translation.y;
        }

        internal Vec2Simple Simple()
        {
            return new Vec2Simple(x, y);
        }

        /// <summary>
        /// Create a string with all class data.
        /// </summary>
        /// <returns>A string with all class data.</returns>
        public string CreateString()
        {
            return this.X + ";" + this.Y;
        }

        /// <summary>
        /// Load the class data from the given string.
        /// </summary>
        /// <param name="stringToLoad">The string that contains the class data.</param>
        public void LoadString(string stringToLoad)
        {
            try
            {
                string[] values = stringToLoad.Split(';');
                if (values.Length == 2)
                {
                    this.X = float.Parse(values[0]);
                    this.Y = float.Parse(values[1]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
