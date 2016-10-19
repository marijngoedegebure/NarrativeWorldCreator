using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;

namespace Common
{
    [Serializable]
    public class Vec4 : IDisposable, IStringRepresentative
    {
        #region Event: Vec4Handler
        /// <summary>
        /// A handler for the Vec4Handler event.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        public delegate void Vec4Handler(Vec4 vector);

        /// <summary>
        /// An event for a changed value (W, X, Y, or Z).
        /// </summary>
        public event Vec4Handler ValueChanged;
        #endregion Event: Vec4Handler

        private float x, y, z, w;

        public float X
        {
            get { return x; }
            set
            {
                x = value;
                if (ValueChanged != null)
                    ValueChanged(this);
            }
        }
        public float Y
        {
            get { return y; }
            set
            {
                y = value;
                if (ValueChanged != null)
                    ValueChanged(this);
            }
        }
        public float Z
        {
            get { return z; }
            set
            {
                z = value;
                if (ValueChanged != null)
                    ValueChanged(this);
            }
        }
        public float W
        {
            get { return w; }
            set
            {
                w = value;
                if (ValueChanged != null)
                    ValueChanged(this);
            }
        }

        public Vec3 XYZ
        {
            get { return new Vec3(x, y, z); }
        }

        public static Vec4 UnitX {
            get { return new Vec4(1, 0, 0, 0); }
        }

        public static Vec4 UnitY {
            get { return new Vec4(0, 1, 0, 0); }
        }

        public static Vec4 UnitZ {
            get { return new Vec4(0, 0, 1, 0); }
        }

        public static Vec4 UnitW {
            get { return new Vec4(0, 0, 0, 1); }
        }

        public static Vec4 One
        {
            get { return new Vec4(1, 1, 1, 1); }
        }

        public static Vec4 Zero {
            get { return new Vec4(0, 0, 0, 0); }
        }

        public static Vec4 Red { get { return new Vec4(1, 0, 0); } }
        public static Vec4 Green { get { return new Vec4(0, 1, 0); } }
        public static Vec4 Blue { get { return new Vec4(0, 0, 1); } }
        public static Vec4 Yellow { get { return new Vec4(1, 1, 0); } }
        public static Vec4 Purple { get { return new Vec4(1, 0, 1); } }
        public static Vec4 Turquoise { get { return new Vec4(0, 1, 1); } }
        public static Vec4 Orange { get { return new Vec4(1, 0.7f, 0); } }
        public static Vec4 White { get { return new Vec4(1, 1, 1); } }
        public static Vec4 Black { get { return new Vec4(0, 0, 0); } }
        public static Vec4 Gray { get { return new Vec4(0.5f, 0.5f, 0.5f); } }

        public Vec4() { this.x = 0; this.y = 0; this.z = 0; this.w = 1; }
        public Vec4(float X, float Y, float Z) { this.x = X; this.y = Y; this.z = Z; this.w = 1; }
        public Vec4(float X, float Y, float Z, float U) { this.x = X; this.y = Y; this.z = Z; this.w = U; }
        public Vec4(float[] parameters)
        {
            if (parameters.Length > 0)
                x = parameters[0];
            if (parameters.Length > 1)
                y = parameters[1];
            if (parameters.Length > 2)
                z = parameters[2];
            if (parameters.Length > 3)
                w = parameters[3];
        }
        public Vec4(Vec4 c) { x = c.x; y = c.y; z = c.z; w = c.w; }
        public Vec4(Vec3 c) { x = c.X; y = c.Y; z = c.Z; w = 1; }
        public Vec4(Vec3 c, float u) { x = c.X; y = c.Y; z = c.Z; this.w = u; }
        public Vec4(string str)
        {
            string[] list = str.Split(';');
            x = float.Parse(list[0], CommonSettings.Culture);
            y = float.Parse(list[1], CommonSettings.Culture);
            z = float.Parse(list[2], CommonSettings.Culture);
            w = list.Length > 3 ? float.Parse(list[3], CommonSettings.Culture) : 1;
        }
        public static Vec4 operator +(Vec4 v1, Vec4 v2) { return new Vec4(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w); }
        public static Vec4 operator -(Vec4 v1, Vec4 v2) { return new Vec4(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, v1.w - v2.w); }
        public static Vec4 operator *(float d, Vec4 v) { return new Vec4(d * v.x, d * v.y, d * v.z, d * v.w); }
        public static Vec4 operator /(Vec4 v, float d) { return (1 / d) * v; }

        public static Vec4 operator -(Vec4 vec)
        {
            Vec4 result = new Vec4(-vec.X, -vec.Y, -vec.Z, vec.W);
            return result;
        }

        public static Vec4 operator *(Vec4 vec, float d)
        {
            return d * vec;
        }

        public static bool operator >(Vec4 v1, Vec4 v2)
        {
            return (v1.W > v2.W && v1.X > v2.X && v1.Y > v2.Y && v1.Z > v2.Z);
        }

        public static bool operator <(Vec4 v1, Vec4 v2)
        {
            return (v1.W < v2.W && v1.X < v2.X && v1.Y < v2.Y && v1.Z < v2.Z);
        }

        public static bool operator >=(Vec4 v1, Vec4 v2)
        {
            return (v1.W >= v2.W && v1.X >= v2.X && v1.Y >= v2.Y && v1.Z >= v2.Z);
        }

        public static bool operator <=(Vec4 v1, Vec4 v2)
        {
            return (v1.W <= v2.W && v1.X <= v2.X && v1.Y <= v2.Y && v1.Z <= v2.Z);
        }

        public static bool operator ==(Vec4 v1, Vec4 v2)
        {
            if ((object)v1 == null && (object)v2 == null)
                return true;
            if ((object)v1 == null)
                return false;
            if ((object)v2 == null)
                return false;
            return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z && v1.w == v2.w;
        }

        public static bool operator !=(Vec4 v1, Vec4 v2)
        {
            return !(v1 == v2);
        }

        public static Vec4 ColorFromByteRGB(byte r, byte g, byte b)
        {
            return new Vec4(((float)r) / 255, ((float)g) / 255, ((float)b) / 255);
        }

        public Color ToColor()
        {
            return Color.FromArgb((int)(x * 255.0), (int)(y * 255.0), (int)(z * 255.0));
        }

		public Color ToAlphaColor()
		{
			return Color.FromArgb((int)(x * 255.0), (int)(y * 255.0), (int)(z * 255.0), (int)(w * 255.0)); ;
		}

        public string ToColorString()
        {
            return "" + (int)(x * 255.0) + ", " + (int)(y * 255.0) + ", " + (int)(z * 255.0);
        }

        public void Save(System.IO.BinaryWriter wr)
        {
            wr.Write((double)this.x);
            wr.Write((double)this.y);
            wr.Write((double)this.z);
            wr.Write((double)this.w);
        }

        internal Vec4(System.IO.BinaryReader br)
        {
            x = (float)br.ReadDouble();
            y = (float)br.ReadDouble();
            z = (float)br.ReadDouble();
            w = (float)br.ReadDouble();
        }

        public override bool Equals(object obj)
        {
            if ((object)this == null && obj == null)
                return true;
            if ((object)this == null)
                return false;
            if (obj == null)
                return false;
            return this == (Vec4)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        public static Vec4 Load(System.IO.BinaryReader r)
        {
            return new Vec4(r);
        }

        public override string ToString() {
            return String.Format("[{0:f}, {1:f}, {2:f}, {3:f}]", x, y, z, w);
        }

        public static bool Unproject(Vec3 w, Matrix4 modelMatrix, Matrix4 projMatrix, float[] viewport, out Vec4 obj)
        {

            Matrix4 p = modelMatrix * projMatrix;
            Matrix4 finalMatrix = p.Inverse();
 
            // Map x and y from window coordinates
            w.X = (w.X - viewport[0]) / viewport[2];
            w.Y = (w.Y - viewport[1]) / viewport[3];

            Vec4 w4 = new Vec4();
            // Map to range -1 to 1 
            w4.X = w.X * 2.0f - 1.0f;
            w4.Y = w.Y * 2.0f - 1.0f;
            w4.Z = w.Z * 2.0f - 1.0f;
            w4.W = 1.0f;

            obj = finalMatrix*w4;
     
            if (obj.W == 0.0)  // Small inaccuracy ...
                return false;


            obj /= obj.w;
     
            return true;
        }

        #region Method: LoadFromString(string saveString)
        /// <summary>
        /// Load the vector from the given string.
        /// </summary>
        /// <param name="saveString">The string to load a vector from.</param>
        /// <returns>The created vector.</returns>
        public static Vec4 LoadFromString(string saveString)
        {
            Vec4 ret = new Vec4();
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
            return this.W + ";" + this.X + ";" + this.Y + ";" + this.Z;
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
                if (values.Length == 4)
                {
                    this.W = float.Parse(values[0]);
                    this.X = float.Parse(values[1]);
                    this.Y = float.Parse(values[2]);
                    this.Z = float.Parse(values[3]);
                }
            }
            catch (Exception)
            {
            }
        }

        internal Vec4Simple Simple()
        {
            return new Vec4Simple(x, y, z, w);
        }

        public void Set(Vec4 vec4)
        {
            this.X = vec4.X;
            this.Y = vec4.Y;
            this.Z = vec4.Z;
            this.W = vec4.W;
        }
    }
}
