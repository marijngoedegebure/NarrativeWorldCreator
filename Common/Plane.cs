using System;
using System.Collections.Generic;

using System.Text;

namespace Common {
    public class Plane {
        public Vec3 Point { get { return point; }}
        public Vec3 Normal { get { return normal; } }

        private Vec3 point;
        private Vec3 normal;
        private float a;
        private float b;
        private float c;
        private float d;

        public Plane(Vec3 point, Vec3 normal) {
            this.point = point;
            this.normal = normal;
            this.a = normal.X;
            this.b = normal.Y;
            this.c = normal.Z;
            this.d = -a * point.X - b * point.Y - c * point.Z;
        }

        public float DistanceToPoint(Vec3 p) {
            float sn = -Vec3.Dot(normal, p - point);
            float sd = Vec3.Dot(normal, normal);
            float sb = sn / sd;
            Vec3 b = p + sb * normal;
            return Vec3.Distance(p, b);
        }

        public float SignedDistanceToPoint(Vec3 p) {
            float result = Vec3.Dot(normal, p);
            result += d;
            result /= normal.Length;
            return result;
        }

        public double AngleWithVector(Vec3 v) {
            return Math.Asin(Vec3.Dot(normal, v) / (normal.Length * v.Length));
        }

        public bool IntersectsWithLineSegment(Vec3 a, Vec3 b, out Vec3 ip, out float u) {
            u = float.MaxValue;
            float denominator = Vec3.Dot(normal, (b - a));
            if (denominator != 0.0) {
                u = Vec3.Dot(normal, (point - a)) / denominator;
                ip = a + u * (b - a);
            } else {
                ip = null;
            }
            return 0.0f < u && u < 1.0f;
        }

        public bool IsVertical() {
            return this.normal.Y < 0.5f;
        }

        public bool IsHorizontal() {
            return !IsVertical();
        }
    }
}
