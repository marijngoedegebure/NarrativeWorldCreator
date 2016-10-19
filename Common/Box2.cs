using System;
using System.Collections.Generic;

using System.Text;
using Common.Geometry;
using System.Drawing;

namespace Common
{
    public class Box2i
    {
        public Vec2i min, max;

        public Box2i(Vec2i min, Vec2i max)
        {
            this.min = new Vec2i(min);
            this.max = new Vec2i(max);
        }

        internal bool IsInside(Vec2i v)
        {
            return v.X >= min.X && v.X <= max.X && v.Y >= min.Y && v.Y <= max.Y;
        }

        internal Line2 Cut(Line2 line)
        {
            if ((line.P1.X < min.X && line.P2.X < min.X) || (line.P1.X > max.X && line.P2.X > max.X) ||
                (line.P1.Y < min.Y && line.P2.Y < min.Y) || (line.P1.Y > max.Y && line.P2.Y > max.Y))
                return null;

            if (line.P1.X < min.X)
            {
                float fact = (min.X - line.P1.X) / (line.P2.X - line.P1.X);
                float yAtXOnMinX = fact * (line.P2.Y - line.P1.Y) + line.P1.Y;
                line = new Line2(new Vec2(min.X, yAtXOnMinX), line.P2);
            }
            if (line.P2.X < min.X)
            {
                float fact = (min.X - line.P2.X) / (line.P1.X - line.P2.X);
                float yAtXOnMinX = fact * (line.P1.Y - line.P2.Y) + line.P2.Y;
                line = new Line2(new Vec2(min.X, yAtXOnMinX), line.P1);
            }
            if (line.P1.X > max.X)
            {
                float fact = (max.X - line.P2.X) / (line.P1.X - line.P2.X);
                float yAtXOnMaxX = fact * (line.P1.Y - line.P2.Y) + line.P2.Y;
                line = new Line2(new Vec2(max.X, yAtXOnMaxX), line.P2);
            }
            if (line.P2.X > max.X)
            {
                float fact = (max.X - line.P1.X) / (line.P2.X - line.P1.X);
                float yAtXOnMaxX = fact * (line.P2.Y - line.P1.Y) + line.P1.Y;
                line = new Line2(new Vec2(max.X, yAtXOnMaxX), line.P1);
            }

            if ((line.P1.Y < min.Y && line.P2.Y < min.Y) || (line.P1.Y > max.Y && line.P2.Y > max.Y))
                return null;

            if (line.P1.Y < min.Y)
            {
                float fact = (min.Y - line.P1.Y) / (line.P2.Y - line.P1.Y);
                float xAtYOnMinY = fact * (line.P2.X - line.P1.X) + line.P1.X;
                line = new Line2(new Vec2(xAtYOnMinY, min.Y), line.P2);
            }
            if (line.P2.Y < min.Y)
            {
                float fact = (min.Y - line.P2.Y) / (line.P1.Y - line.P2.Y);
                float xAtYOnMinY = fact * (line.P1.X - line.P2.X) + line.P2.X;
                line = new Line2(new Vec2(xAtYOnMinY, min.Y), line.P1);
            }
            if (line.P1.Y > max.Y)
            {
                float fact = (max.Y - line.P2.Y) / (line.P1.Y - line.P2.Y);
                float xAtYOnMaxY = fact * (line.P1.X - line.P2.X) + line.P2.X;
                line = new Line2(new Vec2(xAtYOnMaxY, max.Y), line.P2);
            }
            if (line.P2.Y > max.Y)
            {
                float fact = (max.Y - line.P1.Y) / (line.P2.Y - line.P1.Y);
                float xAtYOnMaxY = fact * (line.P2.X - line.P1.X) + line.P1.X;
                line = new Line2(new Vec2(xAtYOnMaxY, max.Y), line.P1);
            }
            return line;
        }

        internal Shapei CreateShape()
        {
            List<Vec2i> points = new List<Vec2i>(4);
            points.Add(new Vec2i(min.X, min.Y));
            points.Add(new Vec2i(min.X, max.Y));
            points.Add(new Vec2i(max.X, max.Y));
            points.Add(new Vec2i(max.X, min.Y));
            return new Shapei(points);
        }

        private Vec2i[] GetCorners()
        {
            Vec2i[] ret = new Vec2i[4];
            ret[0] = new Vec2i(this.min);
            ret[1] = new Vec2i(this.min.X, this.max.Y);
            ret[2] = new Vec2i(this.max);
            ret[3] = new Vec2i(this.max.X, this.min.Y);
            return ret;
        }

        public List<Vec2i> Corners()
        {
            List<Vec2i> ret = new List<Vec2i>();
            foreach (Vec2i v2 in GetCorners())
                ret.Add(v2);
            return ret;
        }

        public int CalculateArea()
        {
            return (max.X - min.X) * (max.Y - min.Y);
        }

        public Box2i MinkowskiMinus(Box2i source)
        {
            Vec2i sourceDim = source.max - source.min;
            Vec2i targetDim = max - min;

            if (sourceDim.X > targetDim.X || sourceDim.Y > targetDim.Y)
                return null;
            return new Box2i(-source.min + min, -source.min + min + targetDim - sourceDim);
        }

        public static Box2i MaxMin()
        {
            return new Box2i(new Vec2i(int.MaxValue, int.MaxValue), new Vec2i(int.MinValue, int.MinValue));
        }

        public void AddPointToBoundingBox(Vec2i v)
        {
            for (int i = 0; i < 2; ++i)
            {
                if (v[i] < min[i])
                    min[i] = v[i];
                if (v[i] > max[i])
                    max[i] = v[i];
            }
        }

        public Vec2i GetRandomPoint(Random r)
        {
            int x = r.Next(max.X - min.X + 1) + min.X;
            int y = r.Next(max.Y - min.Y + 1) + min.Y;
            return new Vec2i(x, y);
        }

        public bool Overlaps(Box2i box)
        {
            Intervali x1 = new Intervali(min.X, max.X);
            Intervali x2 = new Intervali(box.min.X, box.max.X);
            if (x1.Overlaps(x2))
            {
                Intervali y1 = new Intervali(min.Y, max.Y);
                Intervali y2 = new Intervali(box.min.Y, box.max.Y);
                return y1.Overlaps(y2);
            }
            return false;
        }

        internal bool Overlaps(Shapei s)
        {
            return (new Shapei(this)).Overlaps(s);
        }

        internal Vec2i ClosestPointTo(Vec2i v)
        {
            Intervali x = new Intervali(min.X, max.X);
            Intervali y = new Intervali(min.Y, max.Y);
            return new Vec2i(x.GetClosestValueTo(v.X), y.GetClosestValueTo(v.Y));
        }
    }

    public class Box2
    {
        public enum Side { XMin, XMax, YMin, YMax };
        public Vec2 min, max;

        public Vec2 Midpoint { get { return 0.5f * (min + max); } }
        public Vec2 Dimensions { get { return (max - min); } }

        public Box2()
        {
            min = new Vec2();
            max = new Vec2();
        }

        public Box2(Vec2 min, Vec2 max)
        {
            this.min = new Vec2(min);
            this.max = new Vec2(max);
        }

        public Box2(Vec2i min, Vec2i max) {
            this.min = new Vec2(min);
            this.max = new Vec2(max);
        }

        public Box2(float width, float height)
        {
            this.min = new Vec2(0, 0);
            this.max = new Vec2(width, height);
        }

        public Box2(Box2 copy) : this(new Vec2(copy.min), new Vec2(copy.max)) { }

        public static Box2 From2Points(Vec2 v1, Vec2 v2)
        {
            float minX = Math.Min(v1.X, v2.X), maxX = Math.Max(v1.X, v2.X);
            float minY = Math.Min(v1.Y, v2.Y), maxY = Math.Max(v1.Y, v2.Y);
            return new Box2(new Vec2(minX, minY), new Vec2(maxX, maxY));
        }

        public double Area
        {
            get
            {
                Vec2 diff = max - min;
                return diff.X * diff.Y;
            }
        }

        public Line2 GetSide(Side side)
        {
            switch (side)
            {
                case Side.XMin:
                    return new Line2(new Vec2(this.min), new Vec2(this.min.X, this.max.Y));
                case Side.XMax:
                    return new Line2(new Vec2(this.max.X, this.min.Y), new Vec2(this.max));
                case Side.YMin:
                    return new Line2(new Vec2(this.min), new Vec2(this.max.X, this.min.Y));
                case Side.YMax:
                    return new Line2(new Vec2(this.min.X, this.max.Y), new Vec2(this.max));
            }
            return null;
        }

        public static implicit operator Box2(Box b)
        {
            return new Box2(b.Minimum, b.Maximum);
        }

        public void Move(Vec2 move)
        {
            this.min.Move(move);
            this.max.Move(move);
        }

        public static List<Box2> CutBoxes(Box2 source, List<Box2> cuttingBoxes)
        {
            List<Box2> list = new List<Box2>();
            list.Add(source);
            return CutBoxes(list, cuttingBoxes);
        }

        public static List<Box2> CutBoxes(List<Box2> source, List<Box2> cuttingBoxes)
        {
            List<Box2> list;
            List<Box2> old = source;
            foreach (Box2 b in cuttingBoxes)
            {
                list = old;
                old = new List<Box2>();

                foreach (Box2 b2 in list)
                    b2.Cut(b, old);
                old = MergeBack(old);
            }
            return old;
        }

        public void Cut(Box2 box, List<Box2> list)
        {
            List<Box2> newBoxes = new List<Box2>();
            newBoxes.Add(new Box2(this.min, this.max));
            newBoxes = Box2.Cut(newBoxes, box, Vec2.Component.X, box.min.X);
            newBoxes = Box2.Cut(newBoxes, box, Vec2.Component.X, box.max.X);
            newBoxes = Box2.Cut(newBoxes, box, Vec2.Component.Y, box.min.Y);
            newBoxes = Box2.Cut(newBoxes, box, Vec2.Component.Y, box.max.Y);

            List<Box2> temp = new List<Box2>();
            foreach (Box2 b in newBoxes)
            {
                if (!box.Contains(b.Midpoint))
                    temp.Add(b);
            }

            temp = MergeBack(temp);
            foreach (Box b in temp)
                list.Add(b);
            return;
        }

        private static List<Box2> MergeBack(List<Box2> temp)
        {
            bool changed;
            do
            {
                temp = MergeBack(temp, out changed, 1);
            } while (changed);
            do
            {
                temp = MergeBack(temp, out changed, 0);
            } while (changed);

            return temp;
        }

        private static List<Box2> MergeBack(List<Box2> temp, out bool isChanged, int comp)
        {
            List<Box2> newList = new List<Box2>();
            Box2 changed = null;
            for (int i = 0; i < temp.Count; ++i)
            {
                if (changed == null)
                {
                    for (int j = 0; j < temp.Count && changed == null; ++j)
                    {
                        if (i != j)
                        {
                            bool diff = false;
                            for (int k = 0; k < 3; ++k)
                            {
                                if (k != comp && (temp[i].min[k] != temp[j].min[k] || temp[i].max[k] != temp[j].max[k]))
                                    diff = true;
                            }
                            if (!diff && temp[j].min[comp] == temp[i].max[comp])
                            {
                                changed = temp[j];
                                newList.Add(new Box2(new Vec2(temp[i].min), new Vec2(temp[j].max)));
                            }
                        }
                    }
                    if (changed == null)
                        newList.Add(temp[i]);
                }
                else if (changed != temp[i])
                    newList.Add(temp[i]);
            }
            isChanged = changed != null;
            return newList;
        }

        private static List<Box2> Cut(List<Box2> list, Box2 toCut, Vec2.Component c, float val)
        {
            List<Box2> newList = new List<Box2>();
            foreach (Box2 b in list)
                b.Cut(toCut, c, val, newList);
            return newList;
        }

        public static implicit operator Shape(Box2 box)
        {
            List<Vec2> l = new List<Vec2>();
            Vec2[] corners = box.GetCorners();
            foreach (Vec2 v in corners)
                l.Add(v);
            return new Shape(l);
        }

        private void Cut(Box2 toCut, Vec2.Component c, float val, List<Box2> newList)
        {
            if (val <= min.GetComponent(c) || val >= max.GetComponent(c))
            {
                newList.Add(new Box2(new Vec2(min), new Vec2(max)));
                return;
            }
            Box2 b1 = new Box2(new Vec2(min), new Vec2(max));
            b1.max.SetComponent(c, val);
            Box2 b2 = new Box2(min, max);
            b2.min.SetComponent(c, val);
            newList.Add(b1);
            newList.Add(b2);
        }

        public bool Contains(Vec2 v)
        {
            return v.X >= min.X && v.X <= max.X && v.Y >= min.Y && v.Y <= max.Y;
        }

        public bool ContainsNoEdges(Vec2 v)
        {
            return v.X > min.X + 0.0001 && v.X < max.X - 0.0001 && v.Y > min.Y + 0.0001 && v.Y < max.Y - 0.0001;
        }

        public Line2 GetMidPerpendicularToSide(Side sideOfRec)
        {
            Vec2 mid = 0.5f * (min + max);
            switch (sideOfRec)
            {
                case Side.XMax:
                case Side.XMin:
                    return new Line2(new Vec2(mid.X, this.min.Y), new Vec2(mid.X, this.max.Y));
                case Side.YMax:
                case Side.YMin:
                    return new Line2(new Vec2(this.min.X, mid.Y), new Vec2(this.max.X, mid.Y));
            }
            return null;
        }

        public override string ToString()
        {
            return "Box:" + min + "-" + max;
        }

        public string ToFormattedString()
        {
            return "Box:" + min.ToFormattedString() + "-" + max.ToFormattedString();
        }

        public void AddPointToBoundingBox(Vec2 v)
        {
            for (int i = 0; i < 2; ++i)
            {
                if (v[i] < min[i])
                    min[i] = v[i];
                if (v[i] > max[i])
                    max[i] = v[i];
            }
        }

        public bool PointOnEdges(Vec2 point)
        {
            Line2 l = GetSide(Side.XMax);
            if (l.IsPointOnLine(point))
                return true;
            l = GetSide(Side.XMin);
            if (l.IsPointOnLine(point))
                return true;
            l = GetSide(Side.YMax);
            if (l.IsPointOnLine(point))
                return true;
            l = GetSide(Side.YMin);
            if (l.IsPointOnLine(point))
                return true;
            return false;
        }

        public Vec2 GetRandomPoint(Random r, int decimals)
        {
            float tx = (float)r.NextDouble() * (this.max.X - this.min.X) + this.min.X;
            float ty = (float)r.NextDouble() * (this.max.Y - this.min.Y) + this.min.Y;
            return new Vec2((float)Math.Round(tx, decimals), (float)Math.Round(ty, decimals));
        }

        public Rectangle CreateRectangle()
        {
            return new Rectangle((int)this.min.X, (int)this.min.Y, (int)this.Dimensions.X, (int)this.Dimensions.Y);
        }

        public bool ConnectedToBoxInList(List<Box2> newBoxes)
        {
            foreach (Box2 b in newBoxes)
                if (this.ConnectedToBox(b))
                    return true;
            return false;
        }

        public bool ConnectedToBox(Box2 b)
        {
            int pointsInCommon = 0;
            Vec2[] corners = this.GetCorners();
            for (int i = 0; i < 4 && pointsInCommon < 2; ++i)
                if (b.PointOnEdges(corners[i]))
                    ++pointsInCommon;
            corners = b.GetCorners();
            for (int i = 0; i < 4 && pointsInCommon < 2; ++i)
                if (this.PointOnEdges(corners[i]))
                    ++pointsInCommon;
            return pointsInCommon >= 2;
        }

        private Vec2[,] GetCorners2()
        {
            Vec2[,] ret = new Vec2[2, 2];
            ret[0, 0] = new Vec2(this.min);
            ret[0, 1] = new Vec2(this.min.X, this.max.Y);
            ret[1, 1] = new Vec2(this.max);
            ret[1, 0] = new Vec2(this.max.X, this.min.Y);
            return ret;
        }

        private Vec2[] GetCorners()
        {
            Vec2[] ret = new Vec2[4];
            ret[0] = new Vec2(this.min);
            ret[1] = new Vec2(this.min.X, this.max.Y);
            ret[2] = new Vec2(this.max);
            ret[3] = new Vec2(this.max.X, this.min.Y);
            return ret;
        }

        public List<Vec2> Corners()
        {
            List<Vec2> ret = new List<Vec2>();
            foreach (Vec2 v2 in GetCorners())
                ret.Add(v2);
            return ret;
        }

        static List<Line2> GetLines(Vec2[,] box)
        {
            List<Line2> lines = new List<Line2>();
            lines.Add(new Line2(box[0, 0], box[0, 1]));
            lines.Add(new Line2(box[0, 0], box[1, 0]));
            lines.Add(new Line2(box[1, 1], box[0, 1]));
            lines.Add(new Line2(box[1, 1], box[1, 0]));
            return lines;
        }

        internal static bool Overlap(Vec2[,] box1, Vec2[,] box2)
        {
            List<Line2> lines1 = GetLines(box1);
            List<Line2> lines2 = GetLines(box2);
            foreach (Line2 line in lines1)
            {
                foreach (Line2 line2 in lines2)
                {
                    bool i1, i2;
                    if ((object)line.IntersectionOnLine(line2, out i1) != null && (object)line2.IntersectionOnLine(line, out i2) != null && i1 && i2)
                        return true;
                }
            }
            return false;
        }

        internal static bool OverlapNoEdges(Vec2[,] box1, Vec2[,] box2)
        {
            List<Line2> lines1 = GetLines(box1);
            List<Line2> lines2 = GetLines(box2);
            foreach (Line2 line in lines1)
            {
                foreach (Line2 line2 in lines2)
                {
                    if ((object)line.IntersectionOnLineNoEdges(line2) != null && (object)line2.IntersectionOnLineNoEdges(line) != null)
                        return true;
                }
            }
            return false;
        }

        //internal bool IntersectedBy(Line2 l)
        //{
        //    double x1 = this.min.X; double x2 = this.max.X;
        //    double y1 = this.min.Y; double y2 = this.max.Y;
        //    if (l.P1.X < x1 && l.P2.X < x1)
        //        return false;
        //    if (l.P1.X > x2 && l.P2.X > x2)
        //        return false;
        //    if (l.P1.Y < y1 && l.P2.Y < y1)
        //        return false;
        //    if (l.P1.Y > y2 && l.P2.Y > y2)
        //        return false;
        //    if ((object)GetSide(Side.XMin).IntersectionOnLine(l) != null)
        //        return true;
        //    if ((object)GetSide(Side.XMax).IntersectionOnLine(l) != null)
        //        return true;
        //    if ((object)GetSide(Side.YMin).IntersectionOnLine(l) != null)
        //        return true;
        //    if ((object)GetSide(Side.YMax).IntersectionOnLine(l) != null)
        //        return true;
        //    return false;
        //}

        internal bool Intersects(Line2 line)
        {
            double st, et, fst = 0, fet = 1;
            Vec2 s = line.P1;
            Vec2 e = line.P2;

            for (int i = 0; i < 2; i++)
            {
                double si = s[i]; double ei = e[i];
                double mini = min[i]; double maxi = max[i];
                if (si < ei)
                {
                    if (si > maxi || ei < mini)
                        return false;
                    double di = ei - si;
                    st = (si < mini) ? (mini - si) / di : 0;
                    et = (ei > maxi) ? (maxi - si) / di : 1;
                }
                else
                {
                    if (ei > maxi || si < mini)
                        return false;
                    double di = ei - si;
                    st = (si > maxi) ? (maxi - si) / di : 0;
                    et = (ei < mini) ? (mini - si) / di : 1;
                }

                if (st > fst) fst = st;
                if (et < fet) fet = et;
                if (fet < fst)
                    return false;
            }
            return true;
        }

        public bool Overlaps(Box2 box)
        {
            bool overlap = Overlap(this.GetCorners2(), box.GetCorners2());
            if (overlap)
                return true;
            foreach (Vec2 v in this.GetCorners())
                if (box.Contains(v))
                    return true;
            foreach (Vec2 v in box.GetCorners())
                if (this.Contains(v))
                    return true;
            return false;
        }

        public bool OverlapsNoEdges(Box2 box)
        {
            bool overlap = OverlapNoEdges(this.GetCorners2(), box.GetCorners2());
            if (overlap)
                return true;
            foreach (Vec2 v in this.GetCorners())
                if (box.ContainsNoEdges(v))
                    return true;
            foreach (Vec2 v in box.GetCorners())
                if (this.ContainsNoEdges(v))
                    return true;
            return false;
        }

        public static Box2 MaxMin()
        {
            return new Box2(new Vec2(Vec2.Max), new Vec2(Vec2.Min));
        }

        public double Distance(Vec2 vec)
        {
            Vec2 midpoint = Midpoint;
            if (vec.X > midpoint.X)
                vec.X = midpoint.X - (vec.X - midpoint.X);
            if (vec.Y > midpoint.Y)
                vec.Y = midpoint.Y - (vec.Y - midpoint.Y);
            if (vec.Y > min.Y)
            {
                if (vec.X >= min.X)
                    return 0;
                return min.X - vec.X;
            }
            else
            {
                if (vec.X >= min.X)
                    return min.Y - vec.Y;
                return (min - vec).length();
            }
        }

        public static void RemoveAreasSmallerThan(List<Box2> boxes, double p)
        {
            List<Box2> clone = new List<Box2>(boxes.Count);
            foreach (Box2 b in boxes)
                clone.Add(new Box2(b));
            try
            {
                RemoveAreasSmallerThan(boxes, p, 0);
            }
            catch (Exception ex)
            {
                //System.IO.StreamWriter sw = new System.IO.StreamWriter("c:\\tim_boxes_test.txt");
                //foreach (Box2 b in clone)
                //    sw.WriteLine("" + b.min.X + ", " + b.min.Y + ", " + b.max.X + ", " + b.max.Y);
                //sw.Close();
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private static void RemoveAreasSmallerThan(List<Box2> boxes, double p, int level)
        {
            if (boxes.Count == 0)
                return;
            if (boxes.Count == 1)
            {
                if (boxes[0].Dimensions.X < p || boxes[0].Dimensions.Y < p)
                    boxes.Clear();
                return;
            }

            if (level > 20)
                throw new Exception("Dit komt nooit goed!");

            //Image img = new Bitmap(500, 500);
            //Graphics g = Graphics.FromImage(img);
            //foreach (Box2 b in boxes)
            //{
            //    Vec2 min = 5 * (b.min + new Vec2(5, 5));
            //    Vec2 max = 5 * (b.max + new Vec2(5, 5));
            //    Vec2 dim = max - min;
            //    Rectangle rec = new Rectangle((int)min.X, (int)min.Y, (int)dim.X, (int)dim.Y);
            //    g.DrawRectangle(Pens.Black, rec);
            //}
            //g.Dispose();
            //img.Save("C:\\boxes_debug" + (jefke++) + ".png");

            bool removedArea = false;
            foreach (Vec2.Component comp in Enum.GetValues(typeof(Vec2.Component)))
            {
                Vec2.Component other = Vec2.Component.X;
                if (comp == Vec2.Component.X)
                    other = Vec2.Component.Y;
                List<float> splitPoints = new List<float>();
                foreach (Box2 box in boxes)
                {
                    foreach (float d in new float[] { box.min.GetComponent(comp), box.max.GetComponent(comp) })
                        if (!splitPoints.Contains(d))
                            splitPoints.Add(d);
                }
                splitPoints.Sort();

                for (int splitI = 0; splitI < splitPoints.Count; ++splitI)
                {
                    float split = splitPoints[splitI];

                    IntervalList intervals = GetInsideBoxIntervals(boxes, split, comp, other);
                    foreach (Interval i in intervals)
                    {
                        if (i.Length < p)
                        {
                            List<Box2> toDelete = new List<Box2>();
                            List<Box2> toAdd = new List<Box2>();
                            foreach (Box2 box in boxes)
                            {
                                if ((new Interval(box.min.GetComponent(other),
                                                    box.max.GetComponent(other))).Overlaps(i))
                                {
                                    if (box.min.GetComponent(comp) == split)
                                    {
                                        box.min.SetComponent(comp, splitPoints[splitI + 1]);
                                        if (box.min.GetComponent(comp) == box.max.GetComponent(comp))
                                            toDelete.Add(box);
                                    }
                                    else if (box.max.GetComponent(comp) == split)
                                    {
                                        box.max.SetComponent(comp, splitPoints[splitI - 1]);
                                        if (box.min.GetComponent(comp) == box.max.GetComponent(comp))
                                            toDelete.Add(box);
                                    }
                                    else
                                    {
                                        Box2 newBox = new Box2(box);
                                        if (splitI - 1 < 0)
                                            toDelete.Add(box);
                                        else
                                        {
                                            box.max.SetComponent(comp, splitPoints[splitI - 1]);
                                            if (box.min.GetComponent(comp) == box.max.GetComponent(comp))
                                                toDelete.Add(box);
                                        }
                                        if (splitI + 1 < splitPoints.Count)
                                        {
                                            newBox.min.SetComponent(comp, splitPoints[splitI + 1]);
                                            if (newBox.min.GetComponent(comp) != newBox.max.GetComponent(comp))
                                                toAdd.Add(newBox);
                                        }
                                    }
                                }
                            }
                            foreach (Box2 box in toDelete)
                                boxes.Remove(box);

                            foreach (Box2 box in toAdd)
                                boxes.Add(box);
                            if (toDelete.Count != 0 || toAdd.Count != 0)
                            {
                                RemoveAreasSmallerThan(boxes, p, level + 1);
                                removedArea = true;
                                break;
                            }
                            else
                            {
                                // throw new NotImplementedException("Dit komt nooit goed!");
                            }
                        }
                    }
                    if (removedArea)
                        break;
                }
                if (removedArea)
                    break;
            }
        }

        public static IntervalList GetInsideBoxIntervals(List<Box2> boxes, double split, Vec2.Component comp,
                                                                                    Vec2.Component other)
        {
            IntervalList iList = new IntervalList();
            foreach (Box2 box in boxes)
            {
                if (split >= box.min.GetComponent(comp) && split <= box.max.GetComponent(comp))
                    iList.Add(new Interval(box.min.GetComponent(other), box.max.GetComponent(other)));
            }
            return iList;
        }

        internal void Translate(Vec2 translation)
        {
            min.Translate(translation);
            max.Translate(translation);
        }
    }
}
