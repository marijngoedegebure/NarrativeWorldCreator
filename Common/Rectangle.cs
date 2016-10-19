using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable]
    public class Rect {
        public const int DIMENSIONS = 2;
        private Vec2d min;
        private Vec2d max;

        public int X {
            get { return (int)min.X; }
        }

        public int Y {
            get { return (int)min.Y; }
        }

        public int Width {
            get { return (int)(max.X - min.X); }
        }

        public int Height {
            get { return (int)(max.Y - min.Y); }
        }

        public Vec2d Min {
            get { return min; }
        }

        public Vec2d Max {
            get { return max; }
        }

        public Rect(int x, int y, int width, int height) {
            this.min = new Vec2d(x, y);
            this.max = new Vec2d(x + width, y + height);
        }

        public Rect(Vec2d min, Vec2d max) {
            int x = (int)min.X;
            int y = (int)min.Y;
            int width = (int)Math.Round(max.X - min.X);
            int height = (int)Math.Round(max.Y - min.Y);
            this.min = new Vec2d(x, y);
            this.max = new Vec2d(x + width, y + height);
        }

        public bool Contains(Vec2d v) {
            return Contains(v.X, v.Y);
        }

        public bool Contains(double x, double y) {
            return x >= this.min.X && x <= this.max.X &&  y >= this.min.Y && y <= this.max.Y;
        }

        public bool Contains(Rect rect) {
            return Contains(rect.X, rect.Y) && Contains(rect.X + rect.Width, rect.Y + rect.Height);
        }

        public bool Overlaps(Rect rect) {
            Vec2d min = Min;
            Vec2d max = Max;
            return rect.X < max.X && (rect.X + rect.Width) > min.X && rect.Y < max.Y && (rect.Y + rect.Height) > min.Y;
        }

        public void ExpandBy(Rect other) {
            ExpandBy(other.Min, other.Max);
        }

        public void ExpandBy(Vec2d min, Vec2d max) {
            int ex = (int)Math.Max(this.max.X, max.X);
            int ey = (int)Math.Max(this.max.Y, max.Y);
            int sx = (int)Math.Min(this.min.X, min.X);
            int sy = (int)Math.Min(this.min.Y, min.Y);
            this.min = new Vec2d(sx, sy);
            this.max = new Vec2d(ex, ey);
        }

        public override bool Equals(object obj) {
            bool result = false;
            if (obj is Rect) {
                Rect other = obj as Rect;
                result = this.min.Equals(other.min) && this.max.Equals(other.max);
            }
            return result;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public Rect Copy() {
            return new Rect(this.min, this.max);
        }

        public void Set(Vec2d min, Vec2d max) {
            int x = (int)min.X;
            int y = (int)min.Y;
            int width = (int)Math.Round(max.X - min.X);
            int height = (int)Math.Round(max.Y - min.Y);
            this.min = new Vec2d(x, y);
            this.max = new Vec2d(x + width, y + height);
        }

        public bool EdgeOverlaps(Rect r) {
            for (int i = 0; i < DIMENSIONS; i++) {
                if (Min[i] == r.Min[i] || Max[i] == r.Max[i]) {
                    return true;
                }
            }
            return false;
        }

        public bool Intersects(Rect r) {
            // Every dimension must intersect. If any dimension
            // does not intersect, return false immediately.
            for (int i = 0; i < DIMENSIONS; i++) {
                if (Max[i] < r.Min[i] || Min[i] > r.Max[i]) {
                    return false;
                }
            }
            return true;
        }

        public double Distance(Vec2d p) {
            double distanceSquared = 0;
            for (int i = 0; i < DIMENSIONS; i++) {
                double greatestMin = Math.Max(min[i], p[i]);
                double leastMax = Math.Min(max[i], p[i]);
                if (greatestMin > leastMax) {
                    distanceSquared += ((greatestMin - leastMax) * (greatestMin - leastMax));
                }
            }
            return Math.Sqrt(distanceSquared);
        }

        public double Enlargement(Rect r) {
            double enlargedArea = (Math.Max(max[0], r.max[0]) - Math.Min(min[0], r.min[0])) *
                                 (Math.Max(max[1], r.max[1]) - Math.Min(min[1], r.min[1]));

            return enlargedArea - Area();
        }

        public double Area() {
            return (max[0] - min[0]) * (max[1] - min[1]);
        }

        public List<Vec2d> AsPolygon(double offset) {
            List<Vec2d> result = new List<Vec2d>(4);
            result.Add(new Vec2d(min.X - offset, min.Y - offset));
            result.Add(new Vec2d(min.X - offset, max.Y + offset));             
            result.Add(new Vec2d(max.X + offset, max.Y + offset));
            result.Add(new Vec2d(max.X + offset, min.Y - offset));
            return result;
        }

        public List<Vec2d> AsPolygon() {
            return AsPolygon(0.0);
        }
    }
}
