using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Common.Geometry
{
    public class Shapei
    {
        List<Vec2i> points = new List<Vec2i>();

        public ReadOnlyCollection<Vec2i> Points { get { return points.AsReadOnly(); } }
        Box2i boundingBox;

        public Box2i BoundingBox { get { return boundingBox; } }

        public Shapei(List<Vec2i> points)
        {
            foreach (Vec2i v in points)
                this.points.Add(new Vec2i(v));
            CalculateBoundingBox();
        }

        public Shapei(params Vec2i[] points)
        {
            foreach (Vec2i v in points)
                this.points.Add(new Vec2i(v));
            CalculateBoundingBox();
        }

        public Shapei(Box2i box)
            : this(box.Corners())
        {
        }

        private void CalculateBoundingBox()
        {
            boundingBox = Box2i.MaxMin();
            foreach (Vec2i v in points)
                boundingBox.AddPointToBoundingBox(v);
        }

        public double CalculateArea()
        {
            double areaRuben = 0.0;
            for (int i = 0; i < Points.Count; i++)
            {
                int j = (i + 1) % Points.Count;
                areaRuben += Points[i].X * Points[j].Y;
                areaRuben -= Points[j].X * Points[i].Y;
            }
            areaRuben /= 2.0;
            return Math.Abs(areaRuben);
        }

        public bool Contains(Vec2i v)
        {
            //--- http://alienryderflex.com/polygon/

            int i, j = points.Count - 1;
            bool oddNodes = false;
            int x = v.X;
            int y = v.Y;

            for (i = 0; i < points.Count; i++)
            {
                Vec2i pi = points[i];
                Vec2i pj = points[j];

                if (pi.Y < y && pj.Y >= y || pj.Y < y && pi.Y >= y)
                {
                    if (pi.X + (float)(y - pi.Y) / (float)(pj.Y - pi.Y) * (float)(pj.X - pi.X) < x)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;
        }

        public IEnumerable<Line2i> IntersectionWithLine(Line2i line)
        {
            List<Vec2> points = new List<Vec2>();
            foreach (Vec2i vi in this.points)
                points.Add(new Vec2(vi));
            Shape s = new Shape(points);
            foreach (Line2 l in s.IntersectionWithLine(new Line2(line)))
                yield return new Line2i(l);
        }

        public IEnumerable<Shapei> Cut(Shapei b)
        {
            List<Vec2> pointsA = new List<Vec2>();
            foreach (Vec2i vi in this.points)
                pointsA.Add(new Vec2(vi));
            List<Vec2> pointsB = new List<Vec2>();
            foreach (Vec2i vi in b.points)
                pointsB.Add(new Vec2(vi));
            Shape A = new Shape(pointsA);
            Shape B = new Shape(pointsB);
            foreach(Shape S in A.Cut(B).Shapes)
            {
                List<Vec2i> pointsS = new List<Vec2i>(S.Points.Count);
                foreach (Vec2 v in S.Points)
                    pointsS.Add(new Vec2i(v));
                yield return new Shapei(pointsS);
            }
        }

        public Vec2i GetRandomPoint(Random r)
        {
            return PointOnShape(r);
        }

        internal Vec2i PointOnShape(Random r)
        {
            if (points.Count == 1)
                return new Vec2i(points[0]);
            if (points.Count == 2)
            {
                Line2i line = new Line2i(points[0], points[1]);
                return line.PointOnLine((float)r.NextDouble());
            }

            List<Vec2i[]> triangles = CreateTriangles();

            double[] areas = new double[triangles.Count];
            double total = 0;
            for (int i = 0; i < triangles.Count; ++i)
            {
                Vec2i v0 = triangles[i][0];
                Vec2i v1 = triangles[i][1];
                Vec2i v2 = triangles[i][2];
                //--- http://en.wikipedia.org/wiki/Triangle (Computing the area of a triangle)
                Vec2 AB = (new Vec2(v1) - new Vec2(v0)) / 1000f;
                Vec2 AC = (new Vec2(v2) - new Vec2(v0)) / 1000f;
                float area = Math.Abs(AB.X * AC.Y - AC.X * AB.Y) * 0.5f;
                if (double.IsNaN(area))
                    area = 0;
                areas[i] = area;
                total += area;
            }
            double rand = r.NextDouble() * total;
            double count = 0;
            for (int i = 0; i < areas.Length; ++i)
            {
                if (rand >= count && rand < count + areas[i])
                {
                    float a = (float)r.NextDouble();
                    float b = (float)r.NextDouble();
                    if (a + b > 1)
                    {
                        a = 1 - a;
                        b = 1 - b;
                    }
                    Vec2i v0 = triangles[i][0];
                    Vec2i v1 = triangles[i][1];
                    Vec2i v2 = triangles[i][2];
                    Line2 l01 = new Line2(new Line2i(v0, v1));
                    Vec2 p = l01.PointOnLine(a);
                    Line2 l02 = new Line2(new Line2i(v0, v2));
                    Vec2 p2 = l02.PointOnLine(b);
                    return new Vec2i(p + (p2 - new Vec2(v0)));
                }
                count += areas[i];
            }
            throw new Exception("Euhm");
        }


        public List<Vec2i[]> CreateTriangles()
        {
            List<Vec2i[]> list = new List<Vec2i[]>();
            foreach (List<Vec2i> triStrip in CreateTriangleStripList())
            {
                Vec2i v1 = null;
                Vec2i v2 = triStrip[0];
                Vec2i v3 = triStrip[1];
                for (int i = 2; i < triStrip.Count; ++i)
                {
                    v1 = v2;
                    v2 = v3;
                    v3 = triStrip[i];
                    Vec2i[] triangle = new Vec2i[3];
                    triangle[0] = v1;
                    triangle[1] = v2;
                    triangle[2] = v3;
                    list.Add(triangle);
                }
            }
            return list;
        }

        public List<List<Vec2i>> CreateTriangleStripList()
        {
            if (Math.Abs(boundingBox.max.X - boundingBox.min.X) < 0.0001 ||
                Math.Abs(boundingBox.max.Y - boundingBox.min.Y) < 0.0001)
                return new List<List<Vec2i>>();
            List<Vec2i> vecs = new List<Vec2i>();
            foreach (Vec2i p in points)
                vecs.Add(new Vec2i(p));
            if (vecs.Count == 0)
                return new List<List<Vec2i>>();
            vecs.Add(new Vec2i(vecs[0]));
            List<List<Vec2i>> tris = MathUtil.PolygonToTriangleStrips(vecs);
            return tris;
        }

        public bool Overlaps(Shapei shape)
        {
            if (!boundingBox.Overlaps(shape.boundingBox))
                return false;
            //--- ToDo: iets sneller maken misschien?
            foreach (Vec2i p in points)
                if (shape.Contains(p))
                    return true;
            foreach (Vec2i p in shape.points)
                if (Contains(p))
                    return true;
            foreach (Line2i l in GetLines())
            {
                foreach (Line2i l2 in shape.GetLines())
                    if (Line2i.IntersectionOnBothLines(l, l2))
                        return true;
            }
            return false;
        }

        public IEnumerable<Line2i> GetLines()
        {
            Line2i[] lines = new Line2i[points.Count];
            for (int i = 0; i < points.Count; ++i)
                lines[i] = new Line2i(points[i], points[(i + 1) % points.Count]);
            return lines;
        }

        public IEnumerable<Shapei> MinkowskiMinus(Shapei shapei)
        {
            foreach (List<Vec2i> list in Vec2i.MinkowskiMinus(this.points, shapei.points))
                yield return new Shapei(list);
        }

        public Shapei Offset(int offset, bool smallest)
        {
            List<Vec2i>[] offsetShapes = new List<Vec2i>[2];
            for (int i = 0; i < 2; ++i)
                offsetShapes[i] = new List<Vec2i>();
            for (int i = 0; i < points.Count; ++i)
            {
                Vec2i prev = points[(i - 1 + points.Count) % points.Count];
                Vec2i current = points[i % points.Count];
                Vec2i next = points[(i + 1) % points.Count];
                Vec2i[,] offsetPoints = MathUtil.Verstek(prev, current, next, null, offset * 2);
                offsetShapes[0].Add(offsetPoints[0, 0]);
                offsetShapes[1].Add(offsetPoints[0, 1]);
            }
            Shapei s1 = new Shapei(offsetShapes[0]);
            Shapei s2 = new Shapei(offsetShapes[1]);
            if (s1.CalculateArea() < s2.CalculateArea())
                if (smallest)
                    return s1;
                else
                    return s2;
            else
                if (smallest)
                    return s2;
                else
                    return s1;
        }

        internal IEnumerable<Shapei> Intersection(Shapei shapei)
        {
            yield return new Shapei(MathUtil.IntersectShapes(this.points, shapei.points));
        }

        internal Vec2i GetClosestPointTo(Vec2i v)
        {
            if (Contains(v))
                return new Vec2i(v);
            Vec2i closestPoint = null;
            float closestDistSquared = float.MaxValue;
            foreach (Line2i l in GetLines())
            {
                Vec2i p = l.ClosestPointOnLine(v);
                float dist = (p - v).squareLength();
                if (dist < closestDistSquared)
                {
                    closestDistSquared = dist;
                    closestPoint = p;
                }
            }
            return closestPoint;
        }
    }

    public class Shape
    {
        Vec2 translation = new Vec2();
        List<Vec2> points = new List<Vec2>();
        Box2 boundingBox = new Box2(new Vec2(Vec2.Max), new Vec2(Vec2.Min));

        public Vec2 Translation { get { return translation; } set { translation = value; } }
        public List<Vec2> Points { get { return points; } }
        public Box2 BoundingBox { get { return boundingBox; } }
        public Vec2 MidPoint { get { return boundingBox.Midpoint; } }

        public Shape(List<Vec2> points)
        {
            for(int i = 0; i < points.Count; ++i)
            {
                //if (this.points.Count == 0 || !Vec2.IsOnLine(points[i], points[(i - 1 + points.Count) % points.Count], points[(i + 1) % points.Count]))
                {
                    this.points.Add(new Vec2(points[i]));
                    boundingBox.AddPointToBoundingBox(points[i]);
                }
            }
        }

        public Shape(params Vec2[] points)
        {
            JointConstructor(points);
        }

        public Shape(Shape shape)
        {
            if (shape != null)
            {
                foreach (Vec2 p in shape.points)
                    this.points.Add(new Vec2(p));
                this.boundingBox = new Box2(shape.boundingBox);
            }
        }

        public bool IsConvex()
        {
            //--- http://local.wasp.uwa.edu.au/~pbourke/geometry/clockwise/source1.c
            int n = points.Count;
            int i, j, k;
            int flag = 0;
            double z;

            if (n < 3)
                return true;

            for (i = 0; i < n; i++)
            {
                j = (i + 1) % n;
                k = (i + 2) % n;
                z = (points[j].X - points[i].X) * (points[k].Y - points[j].Y);
                z -= (points[j].Y - points[i].Y) * (points[k].X - points[j].X);
                if (z < 0)
                    flag |= 1;
                else if (z > 0)
                    flag |= 2;
                if (flag == 3)
                    return false;
            }
            if (flag != 0)
                return true;
            else
                throw new Exception("incomputables eg: colinear points (see site in documentation)");
        }

        public bool ClockWise()
        {
            double totalAngle = 0;
            Line2[] lines = GetLines();
            for (int i = 0; i < lines.Length; ++i)
            {
                Line2 l1 = lines[i];
                Line2 l2 = lines[(i + 1) % lines.Length];
                Vec2 v1 = l1.P2 - l1.P1;
                Vec2 v2 = l2.P2 - l2.P1;
                v1.Normalize();
                v2.Normalize();
                double angle = Vec2.AngleBetweenVectors(v1, v2);
                while (angle < -Math.PI)
                    angle += 2 * Math.PI;
                while (angle >= Math.PI)
                    angle -= 2 * Math.PI;
                totalAngle += angle;
            }
            return totalAngle < 0;
        }

        private void JointConstructor(Vec2[] points2)
        {
            List<Vec2> points = new List<Vec2>(points2.Length);
            for (int i = 0; i < points2.Length; ++i) 
            {
                int i2 = (i + 1) % points2.Length;
                if (Math.Abs(points2[i].X - points2[i2].X) > 0.0001 || Math.Abs(points2[i].Y - points2[i2].Y) > 0.0001)
                    points.Add(points2[i]);
            }


            for (int i = 0; i < points.Count; ++i)
            {
                if (!Vec2.IsOnLine(points[i], points[(i - 1 + points.Count) % points.Count], points[(i + 1) % points.Count]))
                {
                    this.points.Add(new Vec2(points[i]));
                    boundingBox.AddPointToBoundingBox(points[i]);
                }
            }
        }

        public Shape(params float[] coordinates)
        {
            Vec2[] points = new Vec2[coordinates.Length / 2];
            for (int i = 0; i < coordinates.Length; i += 2)
                points[i / 2] = new Vec2(coordinates[i], coordinates[i + 1]);
            JointConstructor(points);
        }

        public void RebuildBoundingBox()
        {
            boundingBox = new Box2(new Vec2(Vec2.Max), new Vec2(Vec2.Min));
            foreach (Vec2 p in points)
                boundingBox.AddPointToBoundingBox(p);
        }

        public Shape(Box2 box)
            : this(box.Corners())
        {
        }

        private Shape()
        {
        }

        public List<Vec2> TranslatedPoints()
        {
            List<Vec2> ret = new List<Vec2>();
            foreach (Vec2 p in points)
                ret.Add(p + translation);
            return ret;
        }

        public Line2[] GetLines()
        {
            Line2[] lines = new Line2[points.Count];
            for (int i = 0; i < points.Count; ++i)
                lines[i] = new Line2(points[i] + translation, points[(i + 1) % points.Count] + translation);
            return lines;
        }

        public Line2[] GetLinesToOrigin()
        {
            Line2[] lines = new Line2[points.Count];
            for (int i = 0; i < points.Count; ++i)
                lines[i] = new Line2(points[i], points[(i + 1) % points.Count]);
            return lines;
        }

        public bool Contains(Vec2 v)
        {
            //--- http://alienryderflex.com/polygon/

            int i, j = points.Count - 1;
            bool oddNodes = false;
            double x = v.X;
            double y = v.Y;

            for (i = 0; i < points.Count; i++)
            {
                Vec2 pi = points[i];
                Vec2 pj = points[j];

                if (pi.Y < y && pj.Y >= y || pj.Y < y && pi.Y >= y)
                {
                    if (pi.X + (y - pi.Y) / (pj.Y - pi.Y) * (pj.X - pi.X) < x)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;

            //if (v.X < boundingBox.min.X + translation.X)
            //    return false;
            //if (v.X > boundingBox.max.X + translation.X)
            //    return false;
            //bool InFigure = false;
            //foreach (double d in GetHorizontalIntersections(v.Y))
            //{
            //    if (v.X == d)
            //        return true;
            //    if (v.X < d)
            //        return InFigure;
            //    InFigure = !InFigure;
            //}
            //if (InFigure)
            //    throw new Exception("I screwed it up again!");
            //return false;
        }

        private List<double> GetHorizontalIntersections(float sy)
        {
            List<Vec2> inters = GetIntersections(new Line2(new Vec2(0, sy), new Vec2(1, sy)));
            List<double> doubles = new List<double>();

            foreach (Vec2 v in inters)
            {
                double xx = Math.Round(v.X, 3);
                if (!doubles.Contains(xx))
                    doubles.Add(xx);
            }
            doubles.Sort();
            return doubles;
        }

        public List<Vec2> GetIntersections(Line2 line)
        {
            List<Vec2> list = new List<Vec2>();

            bool dummy;
            foreach (Line2 l in GetLines())
            {
                Vec2 v1 = l.IntersectionOnLine(line, out dummy);
                if ((object)v1 != null)
                    list.Add(v1);
            }
            return list;
        }

        public bool Contains(Shape s)
        {
            foreach (Vec2 v in s.points)
            {
                if (!Contains(v + s.translation))
                    return false;
            }
            Line2[] l1 = GetLines();
            Line2[] l2 = s.GetLines();

            foreach (Line2 ll1 in l1)
            {
                foreach (Line2 ll2 in l2)
                {
                    if (!(ll1.IsPointOnLine(ll2.P1) && ll1.IsPointOnLine(ll2.P2)))
                    {
                        if (Line2.IntersectionOnBothLines(ll1, ll2))
                            return false;
                    }
                }
            }

            return true;
        }

        public bool Contains(Line2 line)
        {
            if (!Contains(line.P1))
                return false;
            if (!Contains(line.P2))
                return false;
            Line2[] l1 = GetLines();

            foreach (Line2 ll1 in l1)
            {
                if (!(ll1.IsPointOnLine(line.P1) && ll1.IsPointOnLine(line.P2)))
                {
                    if (Line2.IntersectionOnBothLines(ll1, line))
                        return false;
                }
            }

            return true;
        }

        //public Shape CreateShapeForCenterToFitLine(Vec2 l1, Vec2 l2)
        //{
        //    return new Shape(MathUtil.PolygonToVectorLists(CreateShapeForCenterToFitLine2(l1, l2))[0]);
        //}

        //private GPC.Polygon CreateShapeForCenterToFitLine2(Vec2 l1, Vec2 l2)
        //{
        //    List<Vec2> v1 = new List<Vec2>();
        //    List<Vec2> v2 = new List<Vec2>();
        //    foreach (Vec2 v in points)
        //    {
        //        v1.Add(v + translation - l1);
        //        v2.Add(v + translation - l2);
        //    }

        //    GPC.Polygon p = GPC.GpcWrapper.Clip(GpcWrapper.GpcOperation.Intersection, 
        //                                MathUtil.VectorListToPolygon(v1), MathUtil.VectorListToPolygon(v2));
        //    //return p;
        //    return ReviewFitLineShape(p, l1, l2);
        //}

        //private GPC.Polygon ReviewFitLineShape(GPC.Polygon p, Vec2 l1, Vec2 l2)
        //{
        //    List<List<Vec2>> newlist = new List<List<Vec2>>();
        //    List<List<Vec2>> lists = MathUtil.PolygonToVectorLists(p);
        //    foreach (List<Vec2> list in lists)
        //    {
        //        newlist.Add(ReviewFitLineShape2(list, l1, l2));
        //    }
        //    return MathUtil.VectorListsToPolygon(newlist);
        //}

        private List<Vec2> ReviewFitLineShape2(List<Vec2> list, Vec2 l1, Vec2 l2)
        {
            List<Vec2> newlist = new List<Vec2>();
            for (int i = list.Count; i < list.Count * 2; ++i)
            {
                Vec2 vprev = list[(i - 1) % list.Count];
                Vec2 vcurr = list[(  i  ) % list.Count];
                Vec2 vnext = list[(i + 1) % list.Count];
                //if (Contains(new Line2(vprev - l1, vcurr - l1)) && Contains(new Line2(vprev - l2, vcurr - l2)) &&
                //    Contains(new Line2(vcurr - l1, vnext - l1)) && Contains(new Line2(vcurr - l2, vnext - l2)))
                if (Contains(new Line2(vcurr + l1, vcurr + l2)))
                    newlist.Add(vcurr);
            }
            return newlist;
        }

        //public List<List<Vec2>> CreateShapeForCenterToFitShape(Shape shape)
        //{
        //    Line2[] lines = shape.GetLinesToOrigin();
        //    GPC.Polygon p = CreateShapeForCenterToFitLine(lines[0]);
        //    if (p.NofContours == 0)
        //        return new List<List<Vec2>>();
        //    for (int i = 1; i < lines.Length; ++i)
        //    {
        //        GPC.Polygon pp = CreateShapeForCenterToFitLine(lines[i]);
        //        if (pp.NofContours > 0)
        //        {
        //            p = GPC.GpcWrapper.Clip(GpcWrapper.GpcOperation.Intersection,
        //                                        p, pp);
        //        }
        //        if (p.NofContours == 0)
        //            return new List<List<Vec2>>();
        //    }
        //    return MathUtil.PolygonToVectorLists(p);
        //}

        //public List<List<Vec2>> CreateShapeForCenterToFitShapes(List<Shape> shapes)
        //{
        //    GPC.Polygon p = null;
        //    bool first = true;
        //    foreach(Shape s in shapes)
        //    {
        //        Line2[] lines = s.GetLinesToOrigin();
        //        int start = 0;
        //        if (first)
        //        {
        //            first = false;
        //            p = CreateShapeForCenterToFitLine(lines[0]);
        //            start = 1;
        //            if (p.NofContours == 0)
        //                return new List<List<Vec2>>();
        //        }
                
        //        for (int i = start; i < lines.Length; ++i)
        //        {
        //            p = GPC.GpcWrapper.Clip(GpcWrapper.GpcOperation.Intersection,
        //                                        p, CreateShapeForCenterToFitLine(lines[i]));
        //            if (p.NofContours == 0)
        //                return new List<List<Vec2>>();
        //        }
        //    }
        //    return MathUtil.PolygonToVectorLists(p);
        //}

        //private GPC.Polygon CreateShapeForCenterToFitLine(Line2 line)
        //{
        //    return CreateShapeForCenterToFitLine2(line.P1, line.P2);
        //}

        public static Shape operator -(Shape shape)
        {
            Shape sh = new Shape();
            sh.Translation = -shape.Translation;
            foreach (Vec2 v in shape.points)
            {
                sh.points.Add(-v);
                sh.boundingBox.AddPointToBoundingBox(-v);
            }
            return sh;
        }

        public List<List<Vec2>> CreateTriangleStripList()
        {
            if (Math.Abs(boundingBox.max.X - boundingBox.min.X) < 0.0001 ||
                Math.Abs(boundingBox.max.Y - boundingBox.min.Y) < 0.0001)
                return new List<List<Vec2>>();
            List<Vec3> vecs = new List<Vec3>();
            foreach(Vec2 p in points)
                vecs.Add((Vec3)p);
            if (vecs.Count == 0)
                return new List<List<Vec2>>();
            vecs.Add(new Vec3(vecs[0]));
            List<List<Vec3>> tris = MathUtil.PolygonToTriangleStrips(vecs);
            List<List<Vec2>> tris2d = new List<List<Vec2>>();
            foreach (List<Vec3> t in tris)
            {
                List<Vec2> temp = new List<Vec2>();
                for (int i = 0; i < t.Count; i += 2)
                {
                    try
                    {
                        if (i + 1 < t.Count)
                            temp.Add((Vec2)t[i + 1]);
                        temp.Add((Vec2)t[i]);

                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                }
                if (t.Count % 2 == 1)
                    temp.Add(t[t.Count - 1]);
                tris2d.Add(temp);
            }
            return tris2d;
        }

        public double CalculateArea()
        {
            double areaRuben = 0.0;
            for (int i = 0; i < Points.Count; i++)
            {
                int j = (i + 1) % Points.Count;
                areaRuben += Points[i].X * Points[j].Y;
                areaRuben -= Points[j].X * Points[i].Y;
            }
            areaRuben /= 2.0;
            return Math.Abs(areaRuben);
        }
        
        public void AddToDrawing(System.Drawing.Graphics g, float scale, System.Drawing.Color color)
        {
            System.Drawing.Pen pen = new System.Drawing.Pen(color);
            for (int i = 0; i < points.Count; ++i)
            {
                int j = (i + 1) % points.Count;
                Vec2 pi = points[i];
                Vec2 pj = points[j];
                pi = scale * pi;
                pj = scale * pj;
                g.DrawLine(pen, (float)pi.X, (float)pi.Y, (float)pj.X, (float)pj.Y);
            }
        }

        public void AddToDrawing(System.Drawing.Graphics g, float scale, double controlHeight,
                                    System.Drawing.Color color)
        {
            System.Drawing.Pen pen = new System.Drawing.Pen(color);
            for (int i = 0; i < points.Count; ++i)
            {
                int j = (i + 1) % points.Count;
                Vec2 pi = points[i];
                Vec2 pj = points[j];
                pi = scale * pi;
                pj = scale * pj;
                g.DrawLine(pen, (float)pi.X, (float)(controlHeight - pi.Y), 
                                (float)pj.X, (float)(controlHeight - pj.Y));
            }
        }

        class LineAngleLinePair : IComparable
        {
            public readonly Line2 l1, l2;
            public readonly double angle;

            public LineAngleLinePair(Line2 l1, Line2 l2)
            {
                angle = Vec2.AngleBetweenVectors(l1.Dir(), l2.Dir());
                this.l1 = l1;
                this.l2 = l2;
            }

            #region IComparable Members

            public int CompareTo(object obj)
            {
                LineAngleLinePair l = (LineAngleLinePair)obj;
                if (angle == l.angle)
                    return Length().CompareTo(l.Length());
                return Math.Abs(0.5 * Math.PI - angle).CompareTo(Math.Abs(0.5 * Math.PI - l.angle));
            }

            private double Length()
            {
                return (l1.Length() + l2.Length());
            }

            #endregion
        }

        public List<Box2> CreateBoxListInsideShape(Random r, double maxLineLength, double maxDegreesOf90)
        {
            ////--- speciale nieuwe versie

            //List<LineAngleLinePair> lalps = new List<LineAngleLinePair>();
            //for (int ii = 1; ii <= points.Count; ++ii)
            //{
            //    Vec2 pp0 = points[ii - 1];
            //    Vec2 pp1 = points[ii % points.Count];
            //    Vec2 pp2 = points[(ii + 1) % points.Count];

            //    lalps.Add(new LineAngleLinePair(new Line2(pp1, pp0), new Line2(pp1, pp2)));
            //}
            //lalps.Sort();
            //LineAngleLinePair lal = lalps[0];
            //Vec2 ts1 = lal.l1.P1;
            //Vec2 ts2 = ts1 + 0.75 * lal.l1.Length() * lal.l1.Dir();
            //Vec2 ts3 = ts2 + 0.75 * lal.l2.Length() * lal.l2.Dir();
            //Vec2 ts4 = ts1 + 0.75 * lal.l2.Length() * lal.l2.Dir();
            //Shape tempS = new Shape(ts1, ts2, ts3, ts4);
            //if (!tempS.ClockWise())
            //    tempS.points.Reverse();
            //return tempS;

            //--- ouwe versie

            if (points.Count == 0)
                return null;

            double maxRadiansOfHalfPi = maxDegreesOf90 / 180 * Math.PI;

            List<Vec2> clone = new List<Vec2>();
            foreach (Vec2 v in this.points)
                clone.Add(new Vec2(v));

            int i = 0;
            for (i = 0; i < clone.Count; ++i)
            {
                Line2 l = new Line2(GetPoint(clone, i), GetPoint(clone, i + 1));
                if (!l.IsStraight())
                {
                    //--- checken of de angle groot genoeg is
                    double angle = l.Angle(new Line2(new Vec2(0, 0), new Vec2(1, 0)));
                    while (angle >= 0.5 * Math.PI)
                        angle -= 0.5 * Math.PI;
                    while (angle < 0)
                        angle += 0.5 * Math.PI;

                    if (angle >= 0.25 * Math.PI)
                        angle = (0.5 * Math.PI) - angle;

                    if (angle > maxRadiansOfHalfPi) //--- einde check
                    {
                        double len = l.Length();
                        if (len > maxLineLength)
                        {
                            int parts = (int)Math.Round(len / maxLineLength);
                            for (int m = 1; m < parts; ++m)
                            {
                                float randomParameter = r != null ? ((float)r.NextDouble() - 0.5f) * (1 / (float)parts) : 0;
                                clone.Insert(i + 1, l.PointOnLine(randomParameter + (float)m / (float)parts));
                                ++i;
                            }
                        }
                    }
                }
            }

            Vec2 p1 = GetPoint(clone, 0);
            Vec2 p2 = GetPoint(clone, 1);
            Vec2 p3 = GetPoint(clone, 2);
            Vec2 p4 = GetPoint(clone, 3);
            List<Box2> ret = new List<Box2>();
            ret.Add(new Box2(this.boundingBox));
            i = 0;
            List<Box2> boxesToCut = new List<Box2>();
            for (int j = 0; j < clone.Count; ++j)
                boxesToCut.Add(Box2.From2Points(GetPoint(clone, j), GetPoint(clone, j + 1)));
            ret = Box2.CutBoxes(ret, boxesToCut);
            if (ret.Count == 0)
            {
                return null;
            }

            //double scaleX = (boundingBox.Dimensions.X - (r.NextDouble() + 4)) / boundingBox.Dimensions.X;
            //double scaleZ = (boundingBox.Dimensions.Y - (r.NextDouble() + 4)) / boundingBox.Dimensions.Y;
            //Matrix4 transformation = Matrix4.Translation(-boundingBox.Midpoint) * Matrix4.Scalation(new Vec3(scaleX, 1, scaleZ)) * Matrix4.Translation(boundingBox.Midpoint);// *Matrix4.Translation(new Vec3(r.NextDouble() - 0.5, 0, r.NextDouble() - 0.5));
            List<Box2> ret2 = new List<Box2>();
            foreach (Box2 b in ret)
                if (Contains(b.Midpoint)/* && b.Dimensions.X > 1.25 && b.Dimensions.Y > 1.25*/)
                {
                    Box2 block = new Box2(b.min/* * transformation*/, b.max/* * transformation*/);
                    ret2.Add(block);
                }

            Box2.RemoveAreasSmallerThan(ret2, 1.5);
            return ret2;
        }

        public Shape CreateStraightLineShapeInsideShape(Random r, double maxLineLength, double maxDegreesOf90)
        {
            List<Box2> boxes = CreateBoxListInsideShape(r, maxLineLength, maxDegreesOf90);
            if (boxes == null)
                return null;
            return MathUtil.BoxesToShape(boxes);
        }

        public static Vec2 GetPoint(List<Vec2> list, int p)
        {
            return list[p % list.Count];
        }

        class IndexIntersectionPair : IComparable
        {
            public readonly int index;
            public readonly Vec2 intersection;

            public IndexIntersectionPair(int index, Vec2 intersection)
            {
                this.index = index;
                this.intersection = intersection;
            }

            #region IComparable Members

            public int CompareTo(object obj)
            {
                IndexIntersectionPair iip = (IndexIntersectionPair)obj;
                if (intersection.X == iip.intersection.X)
                    return intersection.Y.CompareTo(iip.intersection.Y);
                return intersection.X.CompareTo(iip.intersection.X);
            }

            #endregion
        }

        public List<Shape> SplitAlong(Line2 splitLine)
        {
            //--- TODO: dit hele ding opkuisen: hele brakke code :D
            List<Shape> newShapes = new List<Shape>();
            List<IndexIntersectionPair> intersections = new List<IndexIntersectionPair>();
            for (int i = 0; i < points.Count; ++i)
            {
                Vec2 p1 = points[i];
                Vec2 p2 = points[(i + 1) % points.Count];

                Vec2 intersection;
                float factor;
                Vec2.Intersection(p1, p2, splitLine.P1, splitLine.P2, out factor, out intersection);
                if (MathUtil.InInterval(factor, 0, 1))
                    intersections.Add(new IndexIntersectionPair(i, intersection));
            }

            intersections.Sort();

            if (intersections.Count > 0)
            {
                for (int i = 0; i < intersections.Count - 1; ++i)
                {
                    HandleIntersection(newShapes, intersections, i);
                }
            }
            List<Shape> nonDuplicateShapes = new List<Shape>();
            foreach (Shape s in newShapes)
            {
                if (nonDuplicateShapes.Count == 0)
                    nonDuplicateShapes.Add(s);
                else
                {
                    bool duplicate = false;
                    foreach(Shape s2 in nonDuplicateShapes)
                        if (s.DuplicateOf(s2))
                        {
                            duplicate = true;
                            break;
                        }
                    if (!duplicate)
                        nonDuplicateShapes.Add(s);
                }
            }
            return nonDuplicateShapes;
        }

        private class DoubleVec2TupleComparer : IComparer<Tuple<double, Vec2>>
        {
            #region IComparer<Tuple<double, Vec2>> Members

            public int Compare(Tuple<double, Vec2> x, Tuple<double, Vec2> y)
            {
                return x.Item1.CompareTo(y.Item1);
            }

            #endregion
        }

        public List<Line2> IntersectionWithLine(Line2 line)
        {
            List<Line2> lines = new List<Line2>();
            List<Tuple<double, Vec2>> intersections = new List<Tuple<double, Vec2>>();
            for (int i = 0; i < points.Count; ++i)
            {
                Vec2 p1 = points[i];
                Vec2 p2 = points[(i + 1) % points.Count];

                Vec2 intersection;
                float factor;
                if (Vec2.Intersection(line.P1, line.P2, p1, p2, out factor, out intersection))
                    intersections.Add(new Tuple<double, Vec2>(factor, intersection));
            }


            Vec2.PushComparisonPrecision(0.0001f);

            //--- remove duplicates
            List<int> duplicates = new List<int>();
            List<Vec2> done = new List<Vec2>();
            int count = 0;
            foreach (Tuple<double, Vec2> t in intersections)
            {
                bool found = false;
                foreach(Vec2 v in done)
                {
                    if (v.Equals(t.Item2))
                    {
                        duplicates.Add(count);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    done.Add(t.Item2);
                ++count;
            }
            for (int i = duplicates.Count; i > 0; --i)
                intersections.RemoveAt(i);

            foreach (Vec2 v in new Vec2[] { line.P1, line.P2 })
            {
                if (this.Contains(v))
                {
                    bool found = false;
                    foreach (Tuple<double, Vec2> t in intersections)
                    {
                        if (v.Equals(t.Item2))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        if (v == line.P1)
                            intersections.Add(new Tuple<double, Vec2>(0, v));
                        else
                            intersections.Add(new Tuple<double, Vec2>(1, v));
                }
            }

            intersections.Sort(new DoubleVec2TupleComparer());

            Vec2.PopComparisonPrecision();

            if (intersections.Count % 2 != 0)
            {
                if (intersections.Count > 2)
                {
                    //--- odd number of intersections: do something special
                    //--- we're going to try out every line and check whether or not it is inside the shape

                    for (int i = 0; i < intersections.Count - 1; ++i)
                    {
                        Line2 l = new Line2(intersections[i].Item2, intersections[i + 1].Item2);
                        if (this.Contains(l.MidPoint()))
                        {
                            if (lines.Count > 0 && lines[lines.Count - 1].P2.Equals(l.P1))
                            {
                                Vec2 temp = lines[lines.Count - 1].P1;
                                lines.RemoveAt(lines.Count - 1);
                                lines.Add(new Line2(temp, intersections[i + 1].Item2));
                            }
                            else
                                lines.Add(l);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < intersections.Count; i += 2)
                    lines.Add(new Line2(intersections[i].Item2, intersections[i + 1].Item2));
            }
            return lines;
        }

        private void HandleIntersection(List<Shape> newShapes, List<IndexIntersectionPair> intersections, int intersectionIndex)
        {
            IndexIntersectionPair iip1 = intersections[intersectionIndex];
            IndexIntersectionPair iip2 = intersections[intersectionIndex + 1];
            Vec2 p1 = iip1.intersection;
            Vec2 p2 = iip2.intersection;
            Vec2 mid = 0.5f * (p1 + p2);
            if (Contains(mid))
            {
                List<int> usedKs;
                List<Vec2> shape1 = CreateShapeFromIntersection(intersections, intersectionIndex, 
                                                                    iip1, iip2, p1, p2, out usedKs);

                List<Vec2> shape2 = CreateShapeFromIntersection(intersections, intersectionIndex, 
                                                                    iip2, iip1, p2, p1, out usedKs);

                newShapes.Add(new Shape(shape1));
                newShapes.Add(new Shape(shape2));
            }
        }

        private List<Vec2> CreateShapeFromIntersection(List<IndexIntersectionPair> intersections, 
                    int intersectionIndex, IndexIntersectionPair iip1, IndexIntersectionPair iip2, 
                    Vec2 p1, Vec2 p2, out List<int> usedKs)
        {
            List<Vec2> shape = new List<Vec2>();
            shape.Add(p1);

            usedKs = new List<int>();
            usedKs.Add(intersectionIndex);
            for (int i = (iip1.index + 1) % points.Count; ; i = (i + 1) % points.Count)
            {
                shape.Add(points[i]);
                for (int k = 0; k < intersections.Count - 1; ++k)
                {
                    IndexIntersectionPair kiip1 = intersections[k];
                    IndexIntersectionPair kiip2 = intersections[k + 1];
                    Vec2 kp1 = kiip1.intersection;
                    Vec2 kp2 = kiip2.intersection;
                    Vec2 kmid = 0.5f * (kp1 + kp2);
                    if (Contains(kmid))
                    {
                        if (!usedKs.Contains(k))
                        {
                            usedKs.Add(k);
                            if (i == kiip1.index)
                            {
                                shape.Add(kp1);
                                shape.Add(kp2);
                                i = kiip2.index;
                            }
                            else if (i == kiip2.index)
                            {
                                shape.Add(kp2);
                                shape.Add(kp1);
                                i = kiip1.index;
                            }
                        }
                    }
                }

                if (i == iip2.index)
                    break;
            }
            shape.Add(p2);
            return shape;
        }

        private bool DuplicateOf(Shape s2)
        {
            if (points.Count != s2.points.Count)
                return false;
            for(int i = 0; i < points.Count; ++i)
            {
                if (points[i] == s2.points[0])
                {
                    for (int j = 1; j < points.Count; ++j)
                    {
                        if (points[(i + j) % points.Count] != s2.points[j])
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public CompoundShape PerformGPCOperation(Shape shape, GpcWrapper.GpcOperation operation)
        {
            Polygon poly = new Polygon(this.points);

            Polygon clipPoly = new Polygon(shape.points);

            TriStrip t = GpcWrapper.ClipToTristrip(operation, poly, clipPoly);

            List<List<Vec2>> newShapes = new List<List<Vec2>>();
            foreach (List<Vec2d> vl in t.Strips)
            {
                List<Vec2> vertList = Vec2.FromVec2dList(vl);

                List<Vec2> orderedList = new List<Vec2>();
                for (int i = 1; i < vertList.Count; i += 2)
                    orderedList.Add(vertList[i]);
                int last = vertList.Count - 1;
                if (last % 2 == 1)
                    --last;
                for (int i = last; i >= 0; i -= 2)
                    orderedList.Add(vertList[i]);

                newShapes.Add(orderedList);
            }

            return new CompoundShape(newShapes);
        }

        public CompoundShape Cut(Shape cutShape)
        {
            return PerformGPCOperation(cutShape, GpcWrapper.GpcOperation.Difference);
        }

        public Shape CreateOffsetShape(float offset, bool smallest)
        {
            List<Vec2>[] offsetShapes = new List<Vec2>[2];
            for (int i = 0; i < 2; ++i)
                offsetShapes[i] = new List<Vec2>();
            for (int i = 0; i < points.Count; ++i)
            {
                Vec2 prev = points[(i - 1 + points.Count) % points.Count];
                Vec2 current = points[i % points.Count];
                Vec2 next = points[(i + 1) % points.Count];
                Vec2[,] offsetPoints = MathUtil.Verstek(prev, current, next, null, offset * 2);
                offsetShapes[0].Add(offsetPoints[0, 0]);
                offsetShapes[1].Add(offsetPoints[0, 1]);
            }
            Shape s1 = new Shape(offsetShapes[0]);
            Shape s2 = new Shape(offsetShapes[1]);
            if (s1.CalculateArea() < s2.CalculateArea())
                if (smallest)
                    return s1;
                else
                    return s2;
            else
                if (smallest)
                    return s2;
                else
                    return s1;
        }

        public Shape RotateToLongestLine()
        {
            Line2 longestLine = null;
            double maxLength = double.MinValue;
            foreach (Line2 line in GetLines())
            {
                double len = line.Length();
                if (len > maxLength)
                {
                    maxLength = len;
                    longestLine = line;
                }
            }

            if (longestLine == null)
                return null;
            Line2 xAxis = new Line2(new Vec2(0, 0), new Vec2(1, 0));
            Matrix4 transformation = Matrix4.Translation(MidPoint) * Matrix4.RotationY(longestLine.Angle(xAxis)) *
                                        Matrix4.Translation(-MidPoint);
            List<Vec2> rotatedPoints = new List<Vec2>();
            foreach (Vec2 p in points)
                rotatedPoints.Add((Vec2)((Vec3)p * transformation));
            return new Shape(rotatedPoints);
        }

        public Vec2 ClosestPointTo(Vec2 point)
        {
            double minDist = double.MaxValue;
            Vec2 ret = null;
            foreach (Vec2 v in points)
            {
                double dist = (point - v).length();
                if (dist < minDist)
                {
                    minDist = dist;
                    ret = v;
                }
            }
            return ret;
        }

        public void RemoveSelfIntersections()
        {
            List<Vec2> newListOfPoints = new List<Vec2>(this.points.Count);
            foreach (Vec2 v in points)
                newListOfPoints.Add(new Vec2(v));

            bool done = false;
            while (!done)
            {
                int i = 0;
                int j = 0;
                bool found = false;
                for (i = 0; !found && i < newListOfPoints.Count; i++)
                {
                    j = (i + 1) % newListOfPoints.Count;
                    while (!found && j != i)
                    {
                        int l = (i + 1) % newListOfPoints.Count;
                        int k = (j + 1) % newListOfPoints.Count;
                        float factor;
                        Vec2 ip;

                        if (Vec2.Intersection(newListOfPoints[i], newListOfPoints[l], newListOfPoints[j], newListOfPoints[k], out factor, out ip))
                        {
                            if (factor > 0 && factor < 1)
                            {
                                found = true;
                                // Create region1 ip, regionPoints[l], ...
                                List<Vec2> r1 = new List<Vec2>();
                                r1.Add(ip);
                                int ir1 = l;
                                do
                                {
                                    r1.Add(newListOfPoints[ir1]);
                                    ir1 = (ir1 + 1) % newListOfPoints.Count;
                                }
                                while (ir1 != k);

                                // Create region2 ip, regionPoints[k], ...
                                List<Vec2> r2 = new List<Vec2>();
                                r2.Add(ip);
                                int ir2 = k;
                                do
                                {
                                    r2.Add(newListOfPoints[ir2]);
                                    ir2 = (ir2 + 1) % newListOfPoints.Count;
                                }
                                while (ir2 != l);

                                double p1 = MathUtil.PolygonPerimeter(r1);
                                double p2 = MathUtil.PolygonPerimeter(r2);
                                newListOfPoints = p1 > p2 ? r1 : r2;
                            }
                        }
                        j = k;
                    }
                }
                done = !found;
            }
            this.points = newListOfPoints;
            RebuildBoundingBox();
        }

        public bool Enclosed(Line2 l, Line2 prev, Line2 next)
        {
            Vec2 direction = l.Dir();
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                Vec2 pointAbove = l.MidPoint() + new Vec2(0, 0.5f);
                bool same;
                if (Contains(pointAbove))
                    same = l.AboveLine(prev.P1) && l.AboveLine(next.P2);
                else
                    same = !l.AboveLine(prev.P1) && !l.AboveLine(next.P2);
                return same;
            }
            else
            {
                Vec2 pointLeft = l.MidPoint() + new Vec2(-0.5f, 0);
                bool same;
                if (Contains(pointLeft))
                    same = l.LeftOfLine(prev.P1) && l.LeftOfLine(next.P2);
                else
                    same = !l.LeftOfLine(prev.P1) && !l.LeftOfLine(next.P2);
                return same;
            }
        }

        public void Save(System.IO.BinaryWriter w)
        {
            w.Write(this.points.Count);
            foreach (Vec2 p in points)
                p.Save(w);
            this.translation.Save(w);
        }
        
        public static Shape Load(System.IO.BinaryReader r)
        {
            int count = r.ReadInt32();
            List<Vec2> points = new List<Vec2>();
            for (int i = 0; i < count; ++i)
                points.Add(Vec2.Load(r));
            Shape s = new Shape(points);
            s.Translation = Vec2.Load(r);
            return s;
        }

        public List<Vec2[]> CreateTriangles()
        {
            List<Vec2[]> list = new List<Vec2[]>();
            foreach (List<Vec2> triStrip in CreateTriangleStripList())
            {
                Vec2 v1 = null;
                Vec2 v2 = triStrip[0];
                Vec2 v3 = triStrip[1];
                for (int i = 2; i < triStrip.Count; ++i)
                {
                    v1 = v2;
                    v2 = v3;
                    v3 = triStrip[i];
                    Vec2[] triangle = new Vec2[3];
                    triangle[0] = v1;
                    triangle[1] = v2;
                    triangle[2] = v3;
                    list.Add(triangle);
                }
            }
            return list;
        }

        internal Vec2 PointOnShape(Random r)
        {
            if (points.Count == 1)
                return new Vec2(points[0]);
            if (points.Count == 2)
            {
                Line2 line = new Line2(points[0], points[1]);
                return line.PointOnLine((float)r.NextDouble());
            }

            List<Vec2[]> triangles = CreateTriangles();

            double[] areas = new double[triangles.Count];
            double total = 0;
            for (int i = 0; i < triangles.Count; ++i)
            {
                Vec2 v0 = triangles[i][0];
                Vec2 v1 = triangles[i][1];
                Vec2 v2 = triangles[i][2];
                //--- http://en.wikipedia.org/wiki/Triangle (Computing the area of a triangle)
                Vec2 AB = (new Vec2(v1) - new Vec2(v0));
                Vec2 AC = (new Vec2(v2) - new Vec2(v0));
                float area = Math.Abs(AB.X * AC.Y - AC.X * AB.Y) * 0.5f;
                if (double.IsNaN(area))
                    area = 0;
                areas[i] = area;
                total += area;
            }
            double rand = r.NextDouble() * total;
            double count = 0;
            for (int i = 0; i < areas.Length; ++i)
            {
                if (rand >= count && rand < count + areas[i])
                {
                    float a = (float)r.NextDouble();
                    float b = (float)r.NextDouble();
                    if (a + b > 1)
                    {
                        a = 1 - a;
                        b = 1 - b;
                    }
                    Vec2 v0 = triangles[i][0];
                    Vec2 v1 = triangles[i][1];
                    Vec2 v2 = triangles[i][2];
                    Line2 l01 = new Line2(v0, v1);
                    Vec2 p = l01.PointOnLine(a);
                    Line2 l02 = new Line2(v0, v2);
                    Vec2 p2 = l02.PointOnLine(b);
                    return p + (p2 - v0);
                }
                count += areas[i];
            }
            throw new Exception("Euhm");
        }

        public bool Overlaps(Shape shape)
        {
            if (!boundingBox.Overlaps(shape.boundingBox))
                return false;
            //--- ToDo: iets sneller maken misschien?
            foreach (Vec2 p in points)
                if (shape.Contains(p))
                    return true;
            foreach (Vec2 p in shape.points)
                if (Contains(p))
                    return true;
            foreach (Line2 l in GetLines())
            {
                foreach (Line2 l2 in shape.GetLines())
                    if (Line2.IntersectionOnBothLines(l, l2))
                        return true;
            }
            return false;
        }

        public Vec2 GetRandomPoint(Random r)
        {
            //if (!ClockWise())
            //    this.points.Reverse();
            return PointOnShape(r);
        }

        public Shape TransformedShape(Matrix4 transformation)
        {
            List<Vec2> newPoints = new List<Vec2>();
            foreach (Vec2 point in Points)
                newPoints.Add(point.Transform(transformation));
            return new Shape(newPoints);
        }

        public Shape Copy()
        {
            Shape s = new Shape();
            foreach (Vec2 p in this.points)
                s.points.Add(new Vec2(p));
            s.boundingBox = new Box2(this.boundingBox);
            return s;
        }

        public float DistanceToPoint(Vec2 v)
        {
            float minDist = float.MaxValue;
            foreach (Line2 l in GetLines())
            {
                float dummy;
                float dist = l.DistanceTo(v, out dummy);
                if (minDist > dist)
                    minDist = dist;
            }
            return minDist;
        }

        public override string ToString()
        {
            string s = "";
            foreach (Vec2 p in points)
                s += s == "" ? p.ToString() : " ; " + p.ToString();
            return "Sh (" + s + ")";
        }

        //public GPC.Polygon CreateGPCPolygon()
        //{
        //    GPC.VertexList vertexList = new GPC.VertexList();
        //    vertexList.Vertex = new GPC.Vertex[points.Count];
        //    int count = 0;
        //    foreach (Vec2 v in points)
        //        vertexList.Vertex[count++] = new GPC.Vertex(v.X, v.Y);
        //    vertexList.NofVertices = count;
        //    GPC.Polygon poly = new GPC.Polygon();
        //    poly.AddContour(vertexList, false);
        //    return poly;
        //}

        internal void Translate(Vec2 translation)
        {
            boundingBox.Translate(translation);
            foreach (Vec2 point in points)
                point.Translate(translation);
        }

        public void Straighten4PointShape()
        {
            if (points.Count != 4)
                return;

            Line2 front = new Line2(points[0], points[1]);
            Vec2 frontDir = front.Dir();
            Vec2 cross = frontDir.Cross();

            float dummy;
            Vec2 inters;
            Vec2.Intersection(points[2], points[2] + frontDir, points[3], points[3] + cross, out dummy, out inters);

            points[3] = inters;

            Vec2.Intersection(points[2], points[2] + frontDir, points[1], points[1] + cross, out dummy, out inters);
            points[2] = inters;
            Vec2.Intersection(points[3], points[3] + frontDir, points[0], points[0] + cross, out dummy, out inters);
            points[3] = inters;

            RebuildBoundingBox();
        }
    }

    public class CompoundShape
    {
        List<Shape> shapes;
        public List<Shape> Shapes { get { return shapes; } }

        public CompoundShape(List<Shape> shapes)
        {
            this.shapes = shapes;
        }

        public CompoundShape(List<List<Vec2>> shapes)
        {
            this.shapes = new List<Shape>();
            foreach (List<Vec2> vlist in shapes)
                this.shapes.Add(new Shape(vlist));
        }

        //public List<List<Vec2>> CreateShapeForCenterToFitShapes(List<Shape> shapes)
        //{
        //    GPC.Polygon p = null;
        //    bool first = true;
        //    foreach (Shape s in shapes)
        //    {
        //        Line2[] lines = s.GetLinesToOrigin();
        //        int start = 0;
        //        if (first)
        //        {
        //            first = false;
        //            p = CreateShapeForCenterToFitLine(lines[0]);
        //            start = 1;
        //            if (p.NofContours == 0)
        //                return new List<List<Vec2>>();
        //        }

        //        for (int i = start; i < lines.Length; ++i)
        //        {
        //            try
        //            {
        //                p = GPC.GpcWrapper.Clip(GpcWrapper.GpcOperation.Intersection,
        //                                            p, CreateShapeForCenterToFitLine(lines[i]));
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            if (p.NofContours == 0)
        //                return new List<List<Vec2>>();
        //        }
        //    }
        //    return MathUtil.PolygonToVectorLists(p);
        //}

        //private GPC.Polygon CreateShapeForCenterToFitLine(Line2 line)
        //{
        //    List<List<Vec2>> v1 = new List<List<Vec2>>();
        //    List<List<Vec2>> v2 = new List<List<Vec2>>();
        //    foreach (Shape s in shapes)
        //    {
        //        List<Vec2> l1 = new List<Vec2>();
        //        List<Vec2> l2 = new List<Vec2>();
        //        foreach (Vec2 v in s.Points)
        //        {
        //            l1.Add(v + s.Translation - line.P1);
        //            l2.Add(v + s.Translation - line.P2);
        //        }
        //        v1.Add(l1);
        //        v2.Add(l2);
        //    }

        //    GPC.Polygon p = GPC.GpcWrapper.Clip(GpcWrapper.GpcOperation.Intersection,
        //                                MathUtil.VectorListsToPolygon(v1), MathUtil.VectorListsToPolygon(v2));
        //    //return p;
        //    return ReviewFitLineShape(p, line.P1, line.P2);
        //}

        //private GPC.Polygon ReviewFitLineShape(GPC.Polygon p, Vec2 l1, Vec2 l2)
        //{
        //    List<List<Vec2>> newlist = new List<List<Vec2>>();
        //    List<List<Vec2>> lists = MathUtil.PolygonToVectorLists(p);
        //    foreach (List<Vec2> list in lists)
        //    {
        //        newlist.Add(ReviewFitLineShape2(list, l1, l2));
        //    }
        //    return MathUtil.VectorListsToPolygon(newlist);
        //}

        //private List<Vec2> ReviewFitLineShape2(List<Vec2> list, Vec2 l1, Vec2 l2)
        //{
        //    List<Vec2> newlist = new List<Vec2>();
        //    for (int i = list.Count; i < list.Count * 2; ++i)
        //    {
        //        Vec2 vprev = list[(i - 1) % list.Count];
        //        Vec2 vcurr = list[(i) % list.Count];
        //        Vec2 vnext = list[(i + 1) % list.Count];
        //        //if (Contains(new Line2(vprev - l1, vcurr - l1)) && Contains(new Line2(vprev - l2, vcurr - l2)) &&
        //        //    Contains(new Line2(vcurr - l1, vnext - l1)) && Contains(new Line2(vcurr - l2, vnext - l2)))
        //        if (Contains(new Line2(vcurr + l1, vcurr + l2)))
        //            newlist.Add(vcurr);
        //    }
        //    return newlist;
        //}

        public bool Contains(Line2 line)
        {
            bool p1InShape = false;
            bool p2InShape = false;
            for (int i = 0; i < shapes.Count; ++i)
            {
                if (!p1InShape)
                    p1InShape = shapes[i].Contains(line.P1);
                if (!p2InShape)
                    p2InShape = shapes[i].Contains(line.P2);
                if (p1InShape && p2InShape)
                    return true;
            }
            return false;
        }

        public bool Contains(Vec2 v)
        {
            for (int i = 0; i < shapes.Count; ++i)
                if (shapes[i].Contains(v))
                    return true;
            return false;
        }

        //public List<Vec2> CreateListOfPositionsNotInShape(Box2 area, double gridSize)
        //{
        //    List<Vec2> ret = new List<Vec2>();
        //    double xx = area.min.X;
        //    while (xx <= area.max.X)
        //    {
        //        double yy = area.min.Y;
        //        while (yy <= area.max.Y)
        //        {
        //            Vec2 p = new Vec2(xx, yy);
        //            if (!Contains(p))
        //                ret.Add(p);
        //            yy += gridSize;
        //        }
        //        xx += gridSize;
        //    }
        //    return ret;
        //}

        public CompoundShape Cut(CompoundShape shapeToCut)
        {
            List<List<Vec2>> vectorLists = new List<List<Vec2>>();
            foreach(Shape s in shapes)
                vectorLists.Add(s.Points);
            Polygon polySource = MathUtil.VectorListsToPolygon(vectorLists);
            Polygon poly = new Polygon(polySource.NumContours + shapeToCut.shapes.Count);
            int index = 0;
            foreach (List<Vec2d> c in polySource.Contours)
                poly.SetContourAndHole(index++, c, false);

            foreach (Shape s in shapeToCut.shapes)
                poly.SetContourAndHole(index++, Vec2d.FromVec2List(s.Points), true);

            List<List<Vec2>> newShapes = new List<List<Vec2>>();
            TriStrip t = poly.ToTriangleStrip();
            List<List<Vec2>> ret = new List<List<Vec2>>();
            foreach (List<Vec2d> vl in t.Strips)
            {
                List<Vec2> vertList = Vec2.FromVec2dList(vl);

                List<Vec2> orderedList = new List<Vec2>();
                for (int i = 1; i < vertList.Count; i += 2)
                    orderedList.Add(vertList[i]);
                int last = vertList.Count - 1;
                if (last % 2 == 1)
                    --last;
                for (int i = last; i >= 0; i -= 2)
                    orderedList.Add(vertList[i]);

                newShapes.Add(orderedList);
            }

            return new CompoundShape(newShapes);
        }

        public Vec2 PointOnShape(Random r)
        {
            double[] areas = new double[shapes.Count];
            double total = 0;
            for (int i = 0; i < areas.Length; ++i)
            {
                areas[i] = shapes[i].CalculateArea();
                total += areas[i];
            }
            double rand = r.NextDouble() * total;
            double count = 0;
            for (int i = 0; i < areas.Length; ++i)
            {
                if (rand >= count && rand < count + areas[i])
                    return shapes[i].PointOnShape(r);
                count += areas[i];
            }
            throw new Exception("Euhm");
        }

        public CompoundShape Union()
        {
            if (shapes.Count < 2)
                return new CompoundShape(this.shapes);

            foreach (Shape s in this.shapes)
                if (s.ClockWise())
                    s.Points.Reverse();

            Polygon poly = GpcWrapper.Clip(GpcWrapper.GpcOperation.Union, new Polygon(shapes[0].Points), new Polygon(shapes[1].Points));
            for (int i = 2; i < shapes.Count; ++i)
                poly = GpcWrapper.Clip(GpcWrapper.GpcOperation.Union, poly, new Polygon(shapes[i].Points));

            List<Shape> nshapes = new List<Shape>();
            for (int i = 0; i < poly.NumContours; ++i)
            {
                if (poly.IsHole(i))
                    throw new Exception("CompoundShape cannot handle holes!");

                List<Vec2> shapePoints = Vec2.FromVec2dList(poly[i]);

                Shape newShape = new Shape(shapePoints);
                if (newShape.ClockWise())
                    newShape.Points.Reverse();
                nshapes.Add(newShape);
            }
            return new CompoundShape(nshapes);
        }
    }

    public class ShapeWithHoles
    {
        List<bool> isHole = new List<bool>();
        List<Shape> shapes = new List<Shape>();

        public void AddShape(Shape shape, bool isHole)
        {
            shapes.Add(shape);
            this.isHole.Add(isHole);
        }

        public Shape Shape(int i) { return shapes[i]; }
        public bool IsHole(int i) { return isHole[i]; }

        public int Count { get { return shapes.Count; } }
    }
}
