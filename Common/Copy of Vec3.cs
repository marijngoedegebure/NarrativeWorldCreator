using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable()]
    public class Vec3 : IDisposable {
        public enum Component { X, Y, Z };

        public static readonly Vec3 Min = new Vec3(double.MinValue, double.MinValue, double.MinValue);
        public static readonly Vec3 Max = new Vec3(double.MaxValue, double.MaxValue, double.MaxValue);

        public double Length {
            get {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        public double this[int index]
        {
            get
            {
                return (index == 0) ? X : ((index == 2) ? Z : Y);
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        return;
                    case 1:
                        Y = value;
                        return;
                    case 2:
                        Z = value;
                        return;
                }
            }
        }

        public static Vec3 Down
        {
            get { return new Vec3(0, -1, 0); }
        }
        public static Vec3 Up
        {
            get { return new Vec3(0, 1, 0); }
        }

        public static Vec3 Left
        {
            get { return new Vec3(-1, 0, 0); }
        }
        public static Vec3 Right
        {
            get { return new Vec3(1, 0, 0); }
        }

        public static Vec3 Backward
        {
            get { return new Vec3(0, 0, -1); }
        }
        public static Vec3 Forward
        {
            get { return new Vec3(0, 0, 1); }
        }

        public static Vec3 Zero
        {
            get { return new Vec3(0, 0, 0); }
        }
        public static Vec3 One
        {
            get { return new Vec3(1, 1, 1); }
        }

        private double x = 0;
        public double X { get { return x; } set { x = value; m_changedTillLastCheck = true; } }

        private double y = 0;
        public double Y { get { return y; } set { y = value; m_changedTillLastCheck = true; } }

        private double z = 0;
        public double Z { get { return z; } set { z = value; m_changedTillLastCheck = true; } }
        
        private bool m_changedTillLastCheck = true;

        public Vec3()
        {
        }

        public Vec3(Vec2 v, double z) : this(v.X, v.Y, z) {
        }

        public Vec3(Vec3 copy)
            : this(copy.X, copy.Y, copy.Z)
        {
        }

        public Vec3(Vec4 c)
        { 
            if (c.u != 1)
            {
                x = c.X / c.u; y = c.Y / c.u; z = c.Z / c.u;
            }
            else
            {
                x = c.X; y = c.Y; z = c.Z;
            }
            this.m_changedTillLastCheck = true;
        }


        public Vec3(double x, double y, double z) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.m_changedTillLastCheck = true;
        }

        public Vec3(double[] parameters)
        {
            if (parameters.Length > 0)
                x = parameters[0];
            else
                x = 0;
            if (parameters.Length > 1)
                y = parameters[1];
            else
                y = 0;
            if (parameters.Length > 2)
                z = parameters[2];
            else
                z = 0;
            this.m_changedTillLastCheck = true;
        }

        public void setValues(double x, double y, double z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public double GetComponent(Component component)
        {
            return this[(int)component];
        }

        public void SetComponent(Component component, double value)
        {
            this[(int)component] = value;
        }

        public double SquareLength() {
            return X * X + Y * Y + Z * Z;
        }

        public static double Distance(Vec3 a, Vec3 b) {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        public void RotateY(double p)
        {
            //double cos = Math.Cos(p);
            //double sin = Math.Sin(p);
            //X = X * cos - Z * sin;
            //Z = X * sin + Z * cos;
            if (X == 0 && Z == 0)
                return;
            Vec2 temp = this;
            double ang = p + temp.GetAngle();
            double len = temp.length();
            double cos = Math.Cos(ang);
            double sin = Math.Sin(ang);
            X = len * cos;
            Z = len * sin;
        }

        internal void Abs()
        {
            X = X < 0 ? -X : X;
            Y = Y < 0 ? -Y : Y;
            Z = Z < 0 ? -Z : Z;
        }

        public static Vec3 operator +(Vec3 left, Vec3 right) {
            Vec3 result = new Vec3(0, 0, 0);
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            return result;
        }

        public static Vec3 operator -(Vec3 left, Vec3 right) {
            Vec3 result = new Vec3(0, 0, 0);
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            return result;
        }

        public static Vec3 operator -(Vec3 vec) {
            Vec3 result = new Vec3(-vec.X, -vec.Y, -vec.Z);
            return result;
        }

        public static Vec3 operator *(Vec3 vec, double d) {
            Vec3 result = new Vec3(vec.X * d, vec.Y * d, vec.Z * d);
            return result;
        }

        public static Vec3 operator *(double d, Vec3 vec) {
            return vec * d;
        }

        public static double operator *(Vec3 v1, Vec3 v2)
        { 
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z; 
        }

        public static Vec3 operator /(Vec3 vec, double d) {
            double mult = 1.0 / d;
            return vec * mult;
        }

        public static Vec3 operator /(double d, Vec3 v)
        {
            return new Vec3(d / v.X, d / v.Y, d / v.Z); 
        }

        public static bool operator >(Vec3 v1, Vec3 v2)
        {
            return (v1.X > v2.X && v1.Y > v2.Y && v1.Z > v2.Z);
        }

        public static bool operator <(Vec3 v1, Vec3 v2)
        {
            return (v1.X < v2.X && v1.Y < v2.Y && v1.Z < v2.Z);
        }

        public static bool operator >=(Vec3 v1, Vec3 v2)
        {
            return (v1.X >= v2.X && v1.Y >= v2.Y && v1.Z >= v2.Z);
        }

        public static bool operator <=(Vec3 v1, Vec3 v2)
        {
            return (v1.X <= v2.X && v1.Y <= v2.Y && v1.Z <= v2.Z);
        }

        public static implicit operator Vec3(Vec2 vec)
        {
            return new Vec3(vec.X, 0, vec.Y);
        }

        public static double Dot(Vec3 left, Vec3 right) {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        public static Vec3 Cross(Vec3 left, Vec3 right) {
            double x = left.Y * right.Z - left.Z * right.Y;
            double y = left.Z * right.X - left.X * right.Z;
            double z = left.X * right.Y - left.Y * right.X;
            return new Vec3(x, y, z);
        }

        public Vec3 Cross(Vec3 v)
        {
            return new Vec3(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
        }

        public Vec3 Normalize() {
            double length = this.Length;
            if (length > 0) {
                double scale = 1.0 / length;
                X *= scale;
                Y *= scale;
                Z *= scale;
            }
            return this;
        }

        public Vec3 normalize()
        {
            double length = this.Length;
            if (length > 0)
            {
                double scale = 1.0 / length;
                return new Vec3(X * scale, Y * scale, Z * scale);
            }
            return new Vec3(this);
        }

        public static Vec3 Normalize(Vec3 v) {
            Vec3 result = new Vec3(v.X, v.Y, v.Z);
            double length = v.Length;
            if (length > 0) {
                double scale = 1.0 / length;
                result.X *= scale;
                result.Y *= scale;
                result.Z *= scale;
            }
            return result;
        }

        public static Vec3 CalculateNormal(Vec3 pnt1, Vec3 pnt2, Vec3 pnt3) {
            return Vec3.Normalize(Vec3.Cross(pnt3 - pnt1, pnt2 - pnt1));
        }

        public override string ToString() {
            return String.Format("[{0:f}, {1:f}, {2:f}]", X, Y, Z);
        }

        public Vec2 ToVec2() {
            return new Vec2(this.X, this.Y);
        }
        
        public static Vec3 FromVec2(Vec2 v) {
            return new Vec3(v.X, v.Y, 0);
        }

        public static Vec3 FromVec2(Vec2 v, double z) {
            return new Vec3(v.X, v.Y, z);
        }

        public override bool Equals(object obj) {
            if (obj != null && obj is Vec3) {
                Vec3 o = (Vec3)obj;
                if (this.X == o.X && this.Y == o.Y && this.Z == o.Z) {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode() {
            double PRIME = 31;
            double result = 1;
            result = PRIME * result + ((Double)X).GetHashCode();
            result = PRIME * result + ((Double)Y).GetHashCode();
            result = PRIME * result + ((Double)Z).GetHashCode();
            return (int)result;
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Save/Load
        public void Save(System.IO.BinaryWriter wr)
        {
            wr.Write(this.X);
            wr.Write(this.Y);
            wr.Write(this.Z);
        }

        public Vec3(System.IO.BinaryReader br)
        {
            this.m_changedTillLastCheck = true;
            this.x = br.ReadDouble();
            this.y = br.ReadDouble();
            this.z = br.ReadDouble();
        }

        public static Vec3 Load(System.IO.BinaryReader r)
        {
            return new Vec3(r);
        }
        #endregion

        internal bool ChangedTillLastCheck()
        {
            bool temp = m_changedTillLastCheck;
            m_changedTillLastCheck = false;
            return temp;
        }

        internal Vec3 XmZY()
        {
            return new Vec3(X, Z, Y);
        }

        public double length() { return Math.Sqrt(squareLength()); }

        public double squareLength() { return X * X + Y * Y + Z * Z; }

        internal void Transform(Matrix4 transformation)
        {
            Vec3 vec = this * transformation;
            X = vec.X;
            Y = vec.Y;
            Z = vec.Z;
        }

        internal Vec2 ProjectTo(Component direction)
        {
            int dir = (int)direction;
            Vec2 vec = new Vec2();
            int j = 0;
            for (int i = 0; i < 3; ++i)
                if (i != dir)
                    vec[j++] = this[i];
            return vec;
        }

        public static Vec3 ProjectPointToPlane(Vec3 point, Vec3 planeOrigin,
                                                    Vec3 planeAxis1, Vec3 planeAxis2,
                                                    Vec3.Component projectionDirection)
        {
            Vec2 orig = planeOrigin.ProjectTo(projectionDirection);
            Vec2 ax1 = planeAxis1.ProjectTo(projectionDirection);
            Vec2 ax2 = planeAxis2.ProjectTo(projectionDirection);
            Vec2 p = point.ProjectTo(projectionDirection);
            Vec2 dummy;
            double t1, t2;
            Vec2.Intersection(orig, orig + ax1, p, p + ax2, out t1, out dummy);
            Vec2.Intersection(orig, orig + ax2, p, p + ax1, out t2, out dummy);
            return planeOrigin + t1 * planeAxis1 + t2 * planeAxis2;
        }

        public static Vec3 FromAngleY(double p)
        {
            return new Vec3(Math.Cos(p), 0, Math.Sin(p));
        }

        public static Vec3 FromVec2PlusY(Vec2 v, double y)
        {
            return new Vec3(v.X, y, v.Y);
        }

        public double angle(Vec3 v)
        {
            Vec3 v1 = this.normalize();
            Vec3 v2 = v.normalize();
            return Math.Acos(v1 * v2);
        }

        public void Move(Vec3 move)
        {
            X += move.X;
            Y += move.Y;
            Z += move.Z;
        }

        #region Method: Transform(Vec3 v, Quaternion q)
        /// <summary>
        /// Rotates the vector over the given quaternion
        /// </summary>
        public static Vec3 Transform(Vec3 v, Quaternion q)
        {
            //return q * v * q.Conjugate();

            double x2 = q.X + q.X; 
            double y2 = q.Y + q.Y; 
            double z2 = q.Z + q.Z;    
            double xx2 = q.X * x2; 
            double yy2 = q.Y * y2; 
            double zz2 = q.Z * z2; 
            double xy2 = q.X * y2; 
            double yz2 = q.Y * z2; 
            double zx2 = q.Z * x2; 
            double xw2 = q.W * x2; 
            double yw2 = q.W * y2; 
            double zw2 = q.W * z2;

            double v0 = v.X;
            double v1 = v.Y;
            double v2 = v.Z;

            return new Vec3((1.0d - yy2 - zz2) * v0 + (xy2 - zw2) * v1 + (zx2 + yw2) * v2,
                            (xy2 + zw2) * v0 + (1.0d - zz2 - xx2) * v1 + (yz2 - xw2) * v2,
                            (zx2 - yw2) * v0 + (yz2 + xw2) * v1 + (1.0d - xx2 - yy2) * v2);
        }
        #endregion Method: Transform(Vec3 v, Quaternion q)

    }
}