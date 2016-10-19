using System;
using System.Collections.Generic;

namespace Common
{
    [Serializable]
    public class Vec3d
    {
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

        public double Z
        {
            get { return z; }
            set { z = value; }
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(x * x + y * y + z * z);
            }
        }

        private double x;
        private double y;
        private double z;

        public Vec3d()
            : this(0, 0, 0)
        {
        }

        public Vec3d(Vec2d v, double z)
            : this(v.X, v.Y, z)
        {
        }

        public Vec3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3d(Vec3d toCopy)
        {
            this.x = toCopy.X;
            this.y = toCopy.Y;
            this.z = toCopy.Z;
        }

        public void setValues(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double SquareLength()
        {
            return x * x + y * y + z * z;
        }

        public static double Distance(Vec3d a, Vec3d b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y) + (a.Z - b.Z) * (a.Z - b.Z));
        }

        public static Vec3d operator +(Vec3d left, Vec3d right)
        {
            Vec3d result = new Vec3d(0, 0, 0);
            result.x = left.x + right.x;
            result.y = left.y + right.y;
            result.z = left.z + right.z;
            return result;
        }

        public static Vec3d operator -(Vec3d left, Vec3d right)
        {
            Vec3d result = new Vec3d(0, 0, 0);
            result.x = left.x - right.x;
            result.y = left.y - right.y;
            result.z = left.z - right.z;
            return result;
        }

        public static Vec3d operator -(Vec3d vec)
        {
            Vec3d result = new Vec3d(-vec.X, -vec.Y, -vec.Z);
            return result;
        }

        public static Vec3d operator *(Vec3d vec, double d)
        {
            Vec3d result = new Vec3d(vec.x * d, vec.y * d, vec.z * d);
            return result;
        }

        public static Vec3d operator *(double d, Vec3d vec)
        {
            return vec * d;
        }

        public static Vec3d operator /(Vec3d vec, double d)
        {
            double mult = 1.0 / d;
            return vec * mult;
        }

        public static double Dot(Vec3d left, Vec3d right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        public static Vec3d Cross(Vec3d left, Vec3d right)
        {
            double x = left.Y * right.Z - left.Z * right.Y;
            double y = left.Z * right.X - left.X * right.Z;
            double z = left.X * right.Y - left.Y * right.X;
            return new Vec3d(x, y, z);
        }

        public Vec3d Normalize()
        {
            double length = this.Length;
            if (length > 0)
            {
                double scale = 1.0 / length;
                x *= scale;
                y *= scale;
                z *= scale;
            }
            return this;
        }

        public static Vec3d Normalize(Vec3d v)
        {
            Vec3d result = new Vec3d(v.X, v.Y, v.Z);
            double length = v.Length;
            if (length > 0)
            {
                double scale = 1.0 / length;
                result.x *= scale;
                result.y *= scale;
                result.z *= scale;
            }
            return result;
        }

        public static Vec3d CalculateNormal(Vec3d pnt1, Vec3d pnt2, Vec3d pnt3)
        {
            return Vec3d.Normalize(Vec3d.Cross(pnt3 - pnt1, pnt2 - pnt1));
        }

        public override string ToString()
        {
            return String.Format("[{0:f}, {1:f}, {2:f}]", x, y, z);
        }

        public Vec2d ToVec2()
        {
            return new Vec2d(this.x, this.y);
        }

        public static Vec3d FromVec2(Vec2d v)
        {
            return new Vec3d(v.X, v.Y, 0);
        }

        public static Vec3d FromVec2(Vec2d v, double z)
        {
            return new Vec3d(v.X, v.Y, z);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Vec3d)
            {
                if (this == obj)
                {
                    return true;
                }
                Vec3d o = (Vec3d)obj;
                if (this.X == o.X && this.Y == o.Y && this.Z == o.Z)
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
            result = PRIME * result + ((Double)z).GetHashCode();
            return (int)result;
        }
    }
}
