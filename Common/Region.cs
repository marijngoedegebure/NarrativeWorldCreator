using System;
using System.Collections.Generic;

namespace Common
{
    public class Region
    {
        public List<LineVec2> Segments
        {
            get;
            private set;
        }

        public int Count
        {
            get { return Segments.Count; }
        }

        public LineVec2 this[int index]
        {
            get { return Segments[index]; }
        }

        public Region()
        {
            Segments = new List<LineVec2>();
        }

        public List<Vec2d> GetPolygon()
        {
            List<Vec2d> result = new List<Vec2d>(this.Count);
            foreach (LineVec2 rs in Segments)
            {
                result.Add(rs.Source);
            }
            return Polygon.GetCCWPolygon(result);
        }

        public override String ToString()
        {
            return String.Format("Region {0:s}", GetPolygon());
        }
    }
}
