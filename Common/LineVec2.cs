using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;

namespace Common
{
    /*public class LineVec2 : QuickGraph.IEdge<Vec2>
    {

        #region IEdge<Vec2> Members
        public Vec2 Source
        {
            get { return start; }
            set { start = value; }
        }

        public Vec2 Target
        {
            get { return end; }
            set { end = value; }
        }
        #endregion

        private Vec2 start;
        private Vec2 end;

        public Vec2 A
        {
            get { return start; }
            set { start = value; }
        }

        public Vec2 B
        {
            get { return end; }
            set { end = value; }
        }


        public LineVec2(Vec2 point1, Vec2 point2)
        {
            start = point1;
            end = point2;
        }

        public double length()
        {
            return Vec2.Distance(start, end);
        }

        public double distance(Vec2 point)
        {
            Vec2 intersection;
            return Vec2.DistanceToLine(point, start, end, out intersection);
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
    }*/
}
