using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.TreeLayouter
{
    public class Layout : Dictionary<Node, Vec2>
    {
        public static Layout operator *(Layout layout, float f)
        {
            Layout l = new Layout();
            Layout.Enumerator e = layout.GetEnumerator();
            while (e.MoveNext())
                l.Add(e.Current.Key, e.Current.Value * f);
            return l;
        }

        public static Layout operator +(Layout layout, Vec2 v)
        {
            Layout l = new Layout();
            Layout.Enumerator e = layout.GetEnumerator();
            while (e.MoveNext())
                l.Add(e.Current.Key, e.Current.Value + v);
            return l;
        }

        public static Layout operator *(Layout layout, Vec2 v)
        {
            Layout l = new Layout();
            Layout.Enumerator e = layout.GetEnumerator();
            while (e.MoveNext())
                l.Add(e.Current.Key, new Vec2(e.Current.Value.X * v.X, e.Current.Value.Y * v.Y));
            return l;
        }

        public static Layout operator -(Layout layout, Vec2 v) { return layout + (-v); }
        public static Layout operator /(Layout layout, float f) { return layout * (1f / f); }
    }
}
