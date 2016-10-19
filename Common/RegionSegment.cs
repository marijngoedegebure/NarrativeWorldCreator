using System;
using System.Collections.Generic;
using System.Text;
using QuickGraph;

namespace Common
{
    public class RegionSegment : IEdge<Vec2d>
    {
        #region IEdge<Vec2d> Members
        public Vec2d Source
        {
            get { return start; }
            set { start = value; }
        }

        public Vec2d Target
        {
            get { return end; }
            set { end = value; }
        }
        #endregion

        public bool Marked
        {
            get;
            set;
        }

        public double Width
        {
            get;
            private set;
        }

        private Vec2d start;
        private Vec2d end;

        public RegionSegment(Vec2d start, Vec2d end)
            : this(start, end, 0.0)
        {
        }

        public RegionSegment(Vec2d start, Vec2d end, double width)
        {
            this.Marked = false;
            this.start = start;
            this.end = end;
            this.Width = width;
        }

        public override string ToString()
        {
            return String.Format("RegionSegment from {0:s} to {1:s} marked[{2:b}]", start, end, Marked);
        }

        public override int GetHashCode()
        {
            double PRIME = 31;
            double result = 1;
            result = PRIME * result + start.X;
            result = PRIME * result + start.Y;
            result = PRIME * result + end.X;
            result = PRIME * result + end.Y;
            result = PRIME * result + (end - start).Length;
            return (int)result;
        }

        public override bool Equals(object obj)
        {
            if (obj is RegionSegment && obj != null)
            {
                RegionSegment o = (RegionSegment)obj;
                if (o.Source == this.Source
                    && o.Target == this.Target
                    && o.Marked == this.Marked)
                    return true;
            }
            return false;
        }
    }
}
