using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec4Simple : IDisposable
    {
        float x, y, z, w;

        public float X { get { return x; } }
        public float Y { get { return y; } }
        public float Z { get { return z; } }
        public float W { get { return w; } }

        public Vec4Simple(Vec4 v)
            : this(v.X, v.Y, v.Z, v.W)
        {
        }

        public Vec4Simple(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vec4Simple(BinaryReader r)
        {
            this.x = (float)r.ReadDouble();
            this.y = (float)r.ReadDouble();
            this.z = (float)r.ReadDouble();
            this.w = (float)r.ReadDouble();
        }

        internal void Save(System.IO.BinaryWriter wr)
        {
            wr.Write((double)x);
            wr.Write((double)y);
            wr.Write((double)z);
            wr.Write((double)w);
        }

        internal void Set(Vec4 v)
        {
            this.x = v.X;
            this.y = v.Y;
            this.z = v.Z;
            this.w = v.W;
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vec3Simple : IDisposable
    {
        float x, y, z;

        public float X { get { return x; } }
        public float Y { get { return y; } }
        public float Z { get { return z; } }

        public Vec3Simple(Vec3 v)
            : this(v.X, v.Y, v.Z)
        {
        }

        public Vec3Simple(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public void Normalize()
        {
            float length = this.Length();
            if (length > 0)
            {
                float scale = 1.0f / length;
                x *= scale;
                y *= scale;
                z *= scale;
            }
        }

        public Vec3 Vec3()
        {
            return new Vec3(x, y, z);
        }

        public void Set(Vec3 v)
        {
            x = v.X;
            y = v.Y;
            z = v.Z;
        }

        public Vec3Simple(BinaryReader r)
        {
            this.x = (float)r.ReadDouble();
            this.y = (float)r.ReadDouble();
            this.z = (float)r.ReadDouble();
        }

        internal void Save(System.IO.BinaryWriter wr)
        {
            wr.Write((double)x);
            wr.Write((double)y);
            wr.Write((double)z);
        }

        public void Transform(Matrix4 m)
        {
            if (m.matrix[0] == 1 && m.matrix[1] == 0 && m.matrix[2] == 0 && m.matrix[3] == 0 &&
                m.matrix[4] == 0 && m.matrix[5] == 1 && m.matrix[6] == 0 && m.matrix[7] == 0 &&
                m.matrix[8] == 0 && m.matrix[9] == 0 && m.matrix[10] == 1 && m.matrix[11] == 0 &&
                m.matrix[12] == 0 && m.matrix[13] == 0 && m.matrix[14] == 0 && m.matrix[15] == 1)
                return;
            //new Vec4(v.X * m.matrix[0] + v.Y * m.matrix[1] + v.Z * m.matrix[2] + m.matrix[3],
            //                v.X * m.matrix[4] + v.Y * m.matrix[5] + v.Z * m.matrix[6] + m.matrix[7],
            //                v.X * m.matrix[8] + v.Y * m.matrix[9] + v.Z * m.matrix[10] + m.matrix[11],
            //                v.X * m.matrix[12] + v.Y * m.matrix[13] + v.Z * m.matrix[14] + m.matrix[15])            
            
            float w = X * m.matrix[12] + Y * m.matrix[13] + Z * m.matrix[14] + m.matrix[15];
            float tempX = X, tempY = Y, tempZ = Z;
            x = (tempX * m.matrix[0] + tempY * m.matrix[1] + tempZ * m.matrix[2] + m.matrix[3]) / w;
            y = (tempX * m.matrix[4] + tempY * m.matrix[5] + tempZ * m.matrix[6] + m.matrix[7]) / w;
            z = (tempX * m.matrix[8] + tempY * m.matrix[9] + tempZ * m.matrix[10] + m.matrix[11]) / w;
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        public string ToString(IFormatProvider format)
        {
            return "[" + this.x.ToString(format)
                 + "; " + this.y.ToString(format)
                 + "; " + this.z.ToString(format)
                 + "]";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2Simple : IDisposable
    {
        float x, y;

        public float X { get { return x; } }
        public float Y { get { return y; } }

        public Vec2Simple(Vec2 v)
            : this(v.X, v.Y)
        {
        }

        public Vec2Simple(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        internal void Translate(Vec2 v)
        {
            x += v.X;
            y += v.Y;
        }

        public Vec2 Vec2()
        {
            return new Vec2(x, y);
        }

        public void Set(Vec2 v)
        {
            this.x = v.X;
            this.y = v.Y;
        }

        public Vec2Simple(BinaryReader r)
        {
            this.x = (float)r.ReadDouble();
            this.y = (float)r.ReadDouble();
        }

        internal void Save(System.IO.BinaryWriter wr)
        {
            wr.Write((double)x);
            wr.Write((double)y);
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    class Vec3Converter : TypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    [Serializable(), TypeConverter(typeof(Vec3Converter))]
    public class Vec3 : IDisposable, IStringRepresentative {

        #region Event: Vec3Handler
        /// <summary>
        /// A handler for the Vec3Handler event.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        public delegate void Vec3Handler(Vec3 vector);

        /// <summary>
        /// An event for a changed value (X, Y, or Z).
        /// </summary>
        public event Vec3Handler ValueChanged;
        #endregion Event: Vec3Handler
        
        public enum Component { X, Y, Z };

        public static readonly Vec3 Min = new Vec3(float.MinValue, float.MinValue, float.MinValue);
        public static readonly Vec3 Max = new Vec3(float.MaxValue, float.MaxValue, float.MaxValue);

        public float X
        {
            get { return x; }
            set
            {
                if (x != value)
                {
                    x = value;
                    m_changedTillLastCheck = true;
                    if (ValueChanged != null)
                        ValueChanged(this);
                }
            }
        }

        public float Y {
            get { return y; }
            set
            {
                if (y != value)
                {
                    y = value;
                    m_changedTillLastCheck = true;
                    if (ValueChanged != null)
                        ValueChanged(this);
                }
            }
        }

        public float Z {
            get { return z; }
            set {
                if (z != value)
                {
                    z = value;
                    m_changedTillLastCheck = true;
                    if (ValueChanged != null)
                        ValueChanged(this);
                }
            }
        }

        public float Length {
            get {
                return (float)Math.Sqrt(x * x + y * y + z * z);
            }
        }

        public static Vec3 UnitX {
            get { return new Vec3(1, 0, 0); }
        }

        public static Vec3 UnitY {
            get { return new Vec3(0, 1, 0); }
        }

        public static Vec3 UnitZ {
            get { return new Vec3(0, 0, 1); }
        }


        public float this[int index]
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

        private float x;
        private float y;
        private float z;
        private bool m_changedTillLastCheck = true;

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

        public Vec3()
        {
        }

        public Vec3(Vec2 v, float z) : this(v.X, v.Y, z) {
        }

        public Vec3(Vec3 copy)
            : this(copy.X, copy.Y, copy.Z)
        {
        }

        public Vec3(Vec3i copy)
            : this(copy.X, copy.Y, copy.Z)
        {
        }

        public Vec3(Vec4 c)
        {
            if (c.W == 0)
                throw new Exception("Division by zero!");
            if (c.W != 1)
            {
                x = c.X / c.W; y = c.Y / c.W; z = c.Z / c.W;
            }
            else
            {
                x = c.X; y = c.Y; z = c.Z;
            }
            this.m_changedTillLastCheck = true;
        }


        public Vec3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.m_changedTillLastCheck = true;
        }

        public Vec3(double x, double y, double z) {
            this.x = (float)x;
            this.y = (float)y;
            this.z = (float)z;
            this.m_changedTillLastCheck = true;
        }

        public Vec3(float[] parameters)
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

        public void setValues(float x, float y, float z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float GetComponent(Component component)
        {
            return this[(int)component];
        }

        public void SetComponent(Component component, float value)
        {
            this[(int)component] = value;
        }

        /// <summary>
        /// Length * Length using the properties instead of the fields.
        /// </summary>
        /// <returns></returns>
        public float SquareLength() {
            return X * X + Y * Y + Z * Z;
        }

        public static float Distance(Vec3 a, Vec3 b) {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y) + (a.Z - b.Z) * (a.Z - b.Z));
        }

        public static float SquaredDistance(Vec3 a, Vec3 b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y) + (a.Z - b.Z) * (a.Z - b.Z);
        }

        public void RotateY(float p)
        {
            //float cos = Math.Cos(p);
            //float sin = Math.Sin(p);
            //X = X * cos - Z * sin;
            //Z = X * sin + Z * cos;
            if (X == 0 && Z == 0)
                return;
            Vec2 temp = this;
            float ang = p + temp.GetAngle();
            float len = temp.length();
            float cos = (float)Math.Cos(ang);
            float sin = (float)Math.Sin(ang);
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

        public static Vec3 operator *(Vec3 vec, float d) {
            Vec3 result = new Vec3(vec.X * d, vec.Y * d, vec.Z * d);
            return result;
        }

        public static Vec3 operator *(float d, Vec3 vec) {
            return vec * d;
        }

        public static float operator *(Vec3 v1, Vec3 v2)
        { 
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z; 
        }

        public static Vec3 operator /(Vec3 vec, float d) {
            float mult = 1.0f / d;
            return vec * mult;
        }

        public static Vec3 operator /(float d, Vec3 v)
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

        public static float Dot(Vec3 left, Vec3 right) {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        public static Vec3 Cross(Vec3 left, Vec3 right) {
            float x = left.Y * right.Z - left.Z * right.Y;
            float y = left.Z * right.X - left.X * right.Z;
            float z = left.X * right.Y - left.Y * right.X;
            return new Vec3(x, y, z);
        }

        public Vec3 Cross(Vec3 v)
        {
            return new Vec3(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
        }

        /// <summary>
        /// Normalizes this vector and returns a reference to this vector.
        /// </summary>
        public Vec3 Normalize() {
            float length = this.Length;
            if (length > 0) {
                float scale = 1.0f / length;
                X *= scale;
                Y *= scale;
                Z *= scale;
            }
            return this;
        }

        /// <summary>
        /// Creates a new vector that is a normalized version of this vector.
        /// </summary>
        public Vec3 normalize()
        {
            float length = this.Length;
            if (length > 0)
            {
                float scale = 1.0f / length;
                return new Vec3(X * scale, Y * scale, Z * scale);
            }
            return new Vec3(this);
        }

        /// <summary>
        /// Creates and returns a new vector that is a normalized version of the given vector.
        /// </summary>
        public static Vec3 Normalize(Vec3 v) {
            Vec3 result = new Vec3(v.X, v.Y, v.Z);
            float length = v.Length;
            if (length > 0) {
                float scale = 1.0f / length;
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

        public static Vec3 FromVec2(Vec2 v, float z) {
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
            float PRIME = 31;
            float result = 1;
            result = PRIME * result + ((float)x).GetHashCode();
            result = PRIME * result + ((float)y).GetHashCode();
            result = PRIME * result + ((float)z).GetHashCode();
            return (int)result;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.ValueChanged = null;
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Save/Load
        public void Save(System.IO.BinaryWriter wr)
        {
            wr.Write((double)this.x);
            wr.Write((double)this.y);
            wr.Write((double)this.z);
        }

        public Vec3(System.IO.BinaryReader br)
        {
            this.m_changedTillLastCheck = true;
            this.x = (float)br.ReadDouble();
            this.y = (float)br.ReadDouble();
            this.z = (float)br.ReadDouble();
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
            return new Vec3(X, -Z, Y);
        }

        public float length() { return (float)Math.Sqrt(squareLength()); }

        /// <summary>
        /// Length * Length using only the fields (and not the properties).
        /// </summary>
        public float squareLength() { return x * x + y * y + z * z; }

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
            float t1, t2;
            Vec2.Intersection(orig, orig + ax1, p, p + ax2, out t1, out dummy);
            Vec2.Intersection(orig, orig + ax2, p, p + ax1, out t2, out dummy);
            return planeOrigin + t1 * planeAxis1 + t2 * planeAxis2;
        }

        public static Vec3 FromAngleY(float p)
        {
            return new Vec3((float)Math.Cos(p), 0, (float)Math.Sin(p));
        }

        public static Vec3 FromVec2PlusY(Vec2 v, float y)
        {
            return new Vec3(v.X, y, v.Y);
        }

        public float angle(Vec3 v)
        {
            Vec3 v1 = this.normalize();
            Vec3 v2 = v.normalize();
            return (float)Math.Acos(v1 * v2);
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

            float x2 = q.X + q.X; 
            float y2 = q.Y + q.Y; 
            float z2 = q.Z + q.Z;    
            float xx2 = q.X * x2; 
            float yy2 = q.Y * y2; 
            float zz2 = q.Z * z2; 
            float xy2 = q.X * y2; 
            float yz2 = q.Y * z2; 
            float zx2 = q.Z * x2; 
            float xw2 = q.W * x2; 
            float yw2 = q.W * y2; 
            float zw2 = q.W * z2;

            float v0 = v.X;
            float v1 = v.Y;
            float v2 = v.Z;

            return new Vec3((1.0f - yy2 - zz2) * v0 + (xy2 - zw2) * v1 + (zx2 + yw2) * v2,
                            (xy2 + zw2) * v0 + (1.0f - zz2 - xx2) * v1 + (yz2 - xw2) * v2,
                            (zx2 - yw2) * v0 + (yz2 + xw2) * v1 + (1.0f - xx2 - yy2) * v2);
        }
        #endregion Method: Transform(Vec3 v, Quaternion q)


        internal void Translate(Vec3 translation)
        {
            X += translation.x;
            Y += translation.y;
            Z += translation.z;
        }

        #region Method: LoadFromString(string saveString)
        /// <summary>
        /// Load the vector from the given string.
        /// </summary>
        /// <param name="saveString">The string to load a vector from.</param>
        /// <returns>The created vector.</returns>
        public static Vec3 LoadFromString(string saveString)
        {
            Vec3 ret = new Vec3();
            ret.LoadString(saveString);
            return ret;
        }
        #endregion Method: LoadFromString(string saveString)

        /// <summary>
        /// Create a string with all class data.
        /// </summary>
        /// <returns>A string with all class data.</returns>
        public string CreateString()
        {
            return this.X + ";" + this.Y + ";" + this.Z;
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
                if (values.Length == 3)
                {
                    this.X = float.Parse(values[0]);
                    this.Y = float.Parse(values[1]);
                    this.Z = float.Parse(values[2]);
                }
            }
            catch (Exception)
            {
            }
        }

        internal Vec3Simple Simple()
        {
            return new Vec3Simple(x, y, z);
        }
    }
}