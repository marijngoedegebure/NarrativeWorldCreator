using System;
using System.Collections.Generic;

namespace Common
{
    public class OrientedBoundingBox
    {
        public double Length
        {
            get { return length; }
        }

        public double Width
        {
            get { return width; }
        }

        public Vec2d Origin
        {
            get { return origin; }
        }

        public Vec2d DirSideA
        {
            get { return dirSideA; }
        }

        public Vec2d DirSideB
        {
            get { return dirSideB; }
        }

        public Vec2d TopRight
        {
            get { return origin + length * dirSideA + width * dirSideB; }
        }

        public Vec2d TopLeft
        {
            get { return origin + length * dirSideA; }
        }

        public Vec2d BottomRight
        {
            get { return origin + width * dirSideB; }
        }

        public Vec2d BottomLeft
        {
            get { return origin; }
        }

        public Vec2d Center
        {
            get { return 0.25 * (p0 + p1 + p2 + p3); }
        }

        public double AspectRatio
        {
            get { return Width / Length; }
        }

        private Vec2d p0;
        private Vec2d p1;
        private Vec2d p2;
        private Vec2d p3;
        private Vec2d origin;
        private Vec2d dirSideA;
        private Vec2d dirSideB;
        private double length;
        private double width;

        public OrientedBoundingBox(Vec2d p0, Vec2d p1, Vec2d p2, Vec2d p3)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            // Order ccw
            List<Vec2d> inOrder = Polygon.GetCCWPolygon(GetPoints());
            this.p0 = inOrder[0];
            this.p1 = inOrder[1];
            this.p2 = inOrder[2];
            this.p3 = inOrder[3];
            // Derived values
            // TODO replace by Properties
            this.origin = this.p0;
            this.dirSideA = this.p1 - this.p0;
            this.dirSideB = this.p3 - this.p0;
            this.length = this.dirSideA.Length;
            this.width = this.dirSideB.Length;
            this.dirSideA.Normalize();
            this.dirSideB.Normalize();
        }

        public List<Vec2d> GetPoints()
        {
            List<Vec2d> result = new List<Vec2d>(4);
            result.Add(p0);
            result.Add(p1);
            result.Add(p2);
            result.Add(p3);
            return result;
        }

        public List<Vec2d> GetPointsDerived()
        {
            return GetPointsDerived(0.0);
        }

        public List<Vec2d> GetPointsDerived(double offset)
        {
            List<Vec2d> result = new List<Vec2d>(4);
            result.Add(origin + offset * dirSideA + offset * dirSideB);
            result.Add(origin + length * dirSideA - offset * dirSideA + offset * dirSideB);
            result.Add(origin + length * dirSideA + width * dirSideB - offset * dirSideA - offset * dirSideB);            
            result.Add(origin + width * dirSideB + offset * dirSideA - offset * dirSideB);             
            return result;
        }

        public LineVec2 GetLongestSide()
        {
            LineVec2 result = null;
            if (width > length)
            {
                result = new LineVec2(p0, p3);
            }
            else
            {
                result = new LineVec2(p0, p1);
            }
            return result;
        }

        public LineVec2 GetShortestSide()
        {
            LineVec2 result = GetLongestSide();
            if (result.B.Equals(p1))
            {
                result = new LineVec2(p0, p3);
            }
            else
            {
                result = new LineVec2(p0, p1);
            }
            return result;
        }

        public static OrientedBoundingBox OrientAround(OrientedBoundingBox obb, Vec2d pointA, Vec2d pointB)
        {
            double targetAngle = Math.Abs(MathUtilD.GetAngle(Vec2d.UnitX, (pointB - pointA).Normalize()));
            Vec2d point = 0.5 * (pointA + pointB);
            List<Vec2d> points = obb.GetPoints();
            double minDist = Double.MaxValue;
            int index = 0;
            for (int i = 0; i < points.Count; ++i)
            {
                int j = (i + 1) % points.Count;
                Vec2d p = 0.5 * (points[i] + points[j]);
                double dist = Vec2d.Distance(point, p);
                double angle = Math.Abs(MathUtilD.GetAngle(Vec2d.UnitX, (points[j] - points[i]).Normalize()));
                if (Math.Abs(targetAngle - angle) < 0.4 * Math.PI && dist < minDist)
                {
                    index = i;
                    minDist = dist;
                }
            }
            if (index > 0)
            {
                points = Polygon.ReorderPolygon(points, index);
            }
            return new OrientedBoundingBox(points[0], points[1], points[2], points[3]);
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is OrientedBoundingBox)
            {
                OrientedBoundingBox other = obj as OrientedBoundingBox;
                result = this.p0.Equals(other.p0);
                result &= this.p1.Equals(other.p1);
                result &= this.p2.Equals(other.p2);
                result &= this.p3.Equals(other.p3);
            }
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
