using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class Vec3i
    {
        int x, y, z;
        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }
        public int Z { get { return z; } set { z = value; } }

        public static Vec3i Cross(Vec3i left, Vec3i right)
        {
            int x = left.Y * right.Z - left.Z * right.Y;
            int y = left.Z * right.X - left.X * right.Z;
            int z = left.X * right.Y - left.Y * right.X;
            return new Vec3i(x, y, z);
        }

        public Vec3i Cross(Vec3i right)
        {
            return Cross(this, right);
        }

        public int this[int index]
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

        public static implicit operator Vec3i(Vec2i v)
        {
            return new Vec3i(v.X, 0, v.Y);
        }

        public Vec3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3i(Vec3 v)
            : this((int)Math.Round(v.X), (int)Math.Round(v.Y), (int)Math.Round(v.Z))
        {
        }

        public Vec3i(Vec3i copy)
            : this(copy.x, copy.y, copy.z)
        {
        }

        public static Vec3i operator +(Vec3i a, Vec3i b) { return new Vec3i(a.x + b.x, a.y + b.y, a.z + b.z); }
        public static Vec3i operator -(Vec3i a, Vec3i b) { return new Vec3i(a.x - b.x, a.y - b.y, a.z - b.z); }
        public static Vec3i operator /(Vec3i a, int b) { return new Vec3i(a.x / b, a.y / b, a.z / b); }

        public float squareLength() { return (float)x * (float)x + (float)y * (float)y + (float)z * (float)z; }

        public float length() { return (float)Math.Sqrt(squareLength()); }
    }
}
