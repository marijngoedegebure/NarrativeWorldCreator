//#define DO_TIMING
//#define USE_GPC
#define USE_CLIPPER

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
#if DO_TIMING
using System.Diagnostics;
#endif

namespace Common
{
    public class IntersectionPoint : IComparable<IntersectionPoint>
    {
        public Vec2d Point { get { return point; } }
        public double Factor { get { return factor; } }

        protected Vec2d point;
        protected double factor;

        public IntersectionPoint(Vec2d point, double factor)
        {
            this.point = point;
            this.factor = factor;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is IntersectionPoint)
            {
                if (this == obj)
                {
                    return true;
                }
                IntersectionPoint o = (IntersectionPoint)obj;
                return this.point.Equals(o.Point);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.point.GetHashCode();
        }

        #region IComparable<IntersectionPoint> Members

        public int CompareTo(IntersectionPoint other)
        {
            return this.Factor.CompareTo(other.Factor);
        }

        #endregion
    }

    public class SegmentIntersectionPoint : IntersectionPoint, IComparable<SegmentIntersectionPoint>
    {
        public int SegmentIndex { get { return segmentIndex; } }

        protected int segmentIndex;

        public SegmentIntersectionPoint(int segmentIndex, Vec2d point, double factor)
            : base(point, factor)
        {
            this.segmentIndex = segmentIndex;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is SegmentIntersectionPoint)
            {
                if (this == obj)
                {
                    return true;
                }
                SegmentIntersectionPoint o = (SegmentIntersectionPoint)obj;
                return segmentIndex == o.segmentIndex && point.Equals(o.point);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Point.GetHashCode();
        }

        #region IComparable<SegmentIntersectionPoint> Members
        public int CompareTo(SegmentIntersectionPoint other)
        {
            int result = this.segmentIndex.CompareTo(other.segmentIndex);
            if (result == 0)
            {
                result = this.Factor.CompareTo(other.Factor);
            }
            return result;
        }
        #endregion
    }

    public class ObjectSegmentIntersectionPoint : SegmentIntersectionPoint, IComparable<ObjectSegmentIntersectionPoint>
    {
        public int ObjectIndex { get { return objectIndex; } }

        protected int objectIndex;

        public ObjectSegmentIntersectionPoint(int objectIndex, int segmentIndex, Vec2d point, double factor)
            : base(segmentIndex, point, factor)
        {
            this.objectIndex = objectIndex;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is ObjectSegmentIntersectionPoint)
            {
                if (this == obj)
                {
                    return true;
                }
                ObjectSegmentIntersectionPoint o = (ObjectSegmentIntersectionPoint)obj;
                return objectIndex == o.objectIndex && segmentIndex == o.segmentIndex && point.Equals(o.point);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Point.GetHashCode();
        }

        #region IComparable<ObjectSegmentIntersectionPoint> Members
        public int CompareTo(ObjectSegmentIntersectionPoint other)
        {
            int result = this.objectIndex.CompareTo(other.objectIndex);
            if (result == 0)
            {
                result = this.segmentIndex.CompareTo(other.segmentIndex);
                if (result == 0)
                {
                    result = this.factor.CompareTo(other.factor);
                }
            }
            return result;
        }
        #endregion
    }

    public class SubPolygon
    {
        /// <summary>
        ///  Returns a ccw-ordered list of vertices of the sub-polygon.
        /// </summary>
        public List<Vec2d> Points
        {
            get;
            private set;
        }

        /// <summary>
        ///  Returns a ccw-ordered list of vertices of the sub-polygon.
        ///  The points are ordered starting with the longest edge on the border of the polygon.
        /// </summary>
        public List<Vec2d> PointsInOrder
        {
            get
            {
                List<Vec2d> result = new List<Vec2d>(Count);
                int index = getLongestBorderEdgeIndex();
                // Reorder vertices
                for (int i = index; i < Count; ++i)
                {
                    result.Add(this[i]);
                }
                for (int i = 0; i < index; ++i)
                {
                    result.Add(this[i]);
                }
                if (Polygon.IsClockWise(result))
                {
                    result.Reverse();
                }
                return result;
            }
        }

        /// <summary>
        /// Returns the vertex count of the sub-polygon.
        /// </summary>
        public int Count
        {
            get { return Points.Count; }
        }

        /// <summary>
        /// Returns the number of edges of the sub-polygon.
        /// </summary>
        public int NumEdges
        {
            get { return onEdge.Length; }
        }

        /// <summary>
        /// Whether this subpolygon is considered valid by the specific subdivision generator.
        /// </summary>
        public bool Valid
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the vertex of the sub-polygon at the given vertex index.
        /// </summary>
        /// <param name="index">vertex index</param>
        /// <returns>vertex at index</returns>
        public Vec2d this[int index]
        {
            get { return Points[index]; }
        }

        private bool[] onEdge;

        public SubPolygon(List<Vec2d> points)
            : this(points, false)
        {
        }

        public SubPolygon(List<Vec2d> points, bool[] onEdgeValues)
            : this(points, false)
        {
            Array.Copy(onEdgeValues, this.onEdge, onEdgeValues.Length);
        }

        public SubPolygon(List<Vec2d> points, bool allOnEdge)
        {
            this.Points = new List<Vec2d>(points.Count);
            this.Points.AddRange(points);
            this.onEdge = new bool[points.Count];
            for (int i = 0; i < onEdge.Length; ++i)
            {
                onEdge[i] = allOnEdge;
            }
        }
        
        /// <summary>
        /// Returns whether any of the edges of the sub-polygon are on the original polygon.
        /// </summary>
        /// <returns>whether any edge overlaps the original edges</returns>
        public bool IsOnEdge()
        {
            bool result = false;
            for (int i = 0; !result && i < onEdge.Length; ++i)
            {
                result |= onEdge[i];
            }
            return result;
        }

        /// <summary>
        /// Returns whether the edge at the given index is on an edge of the original polygon.
        /// </summary>
        /// <param name="index">edge index [0..NumEdges - 1]</param>
        /// <returns>whether the edge is on an original edge</returns>
        public bool IsOnEdge(int index)
        {
            bool result = false;
            if (index >= 0 && index < onEdge.Length)
            {
                result = onEdge[index];
            }
            return result;
        }

        /// <summary>
        /// Sets whether the edge at the given index is on the border of the original polygon.
        /// </summary>
        /// <param name="index">edge index</param>
        /// <param name="value">on edge or internal</param>
        public void SetOnEdge(int index, bool value)
        {
            if (index >= 0 && index < onEdge.Length)
            {
                onEdge[index] = value;
            }
        }

        /// <summary>
        /// Returns the index of the longest edge that is on the border of the polygon.
        /// </summary>
        /// <returns></returns>
        private int getLongestBorderEdgeIndex()
        {
            int result = 0;
            double maxLength = 0.0;
            for (int i = 0; i < Count; ++i)
            {
                double length;
                if (onEdge[i] && (length = Vec2d.Distance(Points[i], Points[(i + 1) % Count])) > maxLength)
                {
                    maxLength = length;
                    result = i;
                }
            }
            return result;
        }

    }

    [Serializable]
    public class Polygon
    {
        #region NeedsCleaningUp
        // TODO: rename to BoundingBox()
        public void GetFloatBoundingBox(out double xMin, out double yMin, out double xMax, out double yMax)
        {
            xMin = double.MaxValue;
            xMax = 0;
            yMin = double.MaxValue;
            yMax = 0;
            for (int j = 0; j < contours.Length; ++j)
            {
                if (!IsHole(j))
                {
                    for (int i = 0; i < contours[j].Count; ++i)
                    {
                        xMin = Math.Min(xMin, contours[j][i].X);
                        yMin = Math.Min(yMin, contours[j][i].Y);
                        xMax = Math.Max(xMax, contours[j][i].X);
                        yMax = Math.Max(yMax, contours[j][i].Y);
                    }
                }
            }
        }

        // TODO: is used?
        public float[] GetAsFloatArray()
        {
            int num_verts = 0;
            for (int i = 0; i < this.NumContours; ++i)
            {
                num_verts += this[i].Count;
            }
            float[] output = new float[num_verts * 2];
            int counter = 0;
            for (int i = 0; i < this.NumContours; ++i)
            {
                for (int j = 0; j < this[i].Count; ++j)
                {
                    output[counter++] = (float)(this[i][j].X);
                    output[counter++] = (float)(this[i][j].Y);
                }
            }
            return output;
        }

        // TODO: change method case
        public void removeExceedingPoints()
        {
            foreach (List<Vec2d> contour in this.contours)
            {
                for (int i = 0; i < contour.Count - 2; ++i)
                {
                    if (MathUtil.IsOnLine(contour[i + 1], contour[i], contour[i + 2]))
                    {
                        contour.Remove(contour[i + 1]);
                        i = 0;
                        continue;
                    }
                }
                for (int i = 0; i < contour.Count; ++i)
                {
                    contour[i].X = Math.Round(contour[i].X);
                    contour[i].Y = Math.Round(contour[i].Y);
                }
            }
        }

        // TODO: is used?
        public int NumPoints()
        {
            if (NumContours == 1)
                return contours[0].Count;
            else return -1;
        }

        // TODO: is used?
        public List<Polygon> getContours()
        {
            List<Polygon> lPolygons = new List<Polygon>();

            for (int i = 0; i < contours.Length; i++)
                lPolygons.Add(new Polygon(this.contours[i]));

            return lPolygons;
        }

        // TODO: is used?
        public IEnumerable<List<Vec2d>> Contours
        {
            get
            {
                foreach (List<Vec2d> contour in contours)
                    yield return contour;
            }
        }
        #endregion

        #region Fields
        /// <summary>
        /// XML element for number of contours.
        /// </summary>
        private const string XML_ELEMENT_NUM_CONTOURS = "numContours";
        /// <summary>
        /// XML element to represent a polygon contour.
        /// </summary>
        private const string XML_ELEMENT_CONTOUR = "contour";
        /// <summary>
        /// XML element for number of vertices for a contour.
        /// </summary>
        private const string XML_ELEMENT_CONTOUR_COUNT = "contourCount";
        /// <summary>
        /// XML element to indicate whether a contour is a hole.
        /// </summary>
        private const string XML_ELEMENT_IS_HOLE = "isHole";
        /// <summary>
        /// Whether a non-hole contour is oriented clock-wise (holes are oriented in reverse order).
        /// </summary>
        private const bool cwContourOrientation = false;
        /// <summary>
        /// Polygon contours, i.e. subpolygons, can be holes.
        /// </summary>
        private readonly List<Vec2d>[] contours;
        /// <summary>
        /// Whether contour[i] is a hole.
        /// </summary>
        private readonly bool[] holes;
        #endregion

        #region Constructors
        public Polygon(int contourCount)
        {
            this.contours = new List<Vec2d>[contourCount];
            for (int i = 0; i < this.NumContours; ++i)
            {
                this.contours[i] = new List<Vec2d>();
            }
            this.holes = new bool[contourCount];
        }

        public Polygon(List<Vec2> vs)
        {
            this.contours = new List<Vec2d>[1];
            this.holes = new bool[1];
            List<Vec2d> p = new List<Vec2d>(vs.Count);
            foreach (Vec2 v in vs)
            {
                p.Add(new Vec2d(v));
            }
            SetContourAndHole(0, p, false);
        }

        public Polygon(List<Vec2i> vs)
        {
            this.contours = new List<Vec2d>[1];
            this.holes = new bool[1];
            List<Vec2d> p = new List<Vec2d>(vs.Count);
            foreach (Vec2i v in vs)
            {
                p.Add(new Vec2d(v));
            }
            SetContourAndHole(0, p, false);
        }

        public Polygon(List<Vec2d> vs)
        {
            this.contours = new List<Vec2d>[1];
            this.holes = new bool[1];
            SetContourAndHole(0, vs, false);
        }
        #endregion

        #region Queries
        //public ReadOnlyCollection<Vec2d> this[int index] {
        //    get {
        //        ReadOnlyCollection<Vec2d> result = null;
        //        if (index >= 0 && index < NumContours) {
        //            result = new ReadOnlyCollection<Vec2d>(contours[index]);
        //        }
        //        return result;
        //    }
        //}

        public List<Vec2d> this[int index]
        {
            get
            {
                List<Vec2d> result = null;
                if (index >= 0 && index < NumContours)
                {
                    result = contours[index];
                }
                return result;
            }
        }

        public int NumContours
        {
            get { return contours.Length; }
        }

        public bool IsSimple()
        {
            return NumContours == 1 && !IsHole(0);
        }

        public bool IsConvex()
        {
            return IsSimple() && IsConvex(this.contours[0]);
        }

        public static bool IsConvex(List<Vec2d> poly)
        {
            int i, j, k;
            int flag = 0;
            double z;
            if (poly.Count >= 3)
            {
                for (i = 0; i < poly.Count; ++i)
                {
                    j = (i + 1) % poly.Count;
                    k = (i + 2) % poly.Count;
                    z = (poly[j].X - poly[i].X) * (poly[k].Y - poly[j].Y);
                    z -= (poly[j].Y - poly[i].Y) * (poly[k].X - poly[j].X);
                    if (z < 0)
                    {
                        flag |= 1;
                    }
                    else if (z > 0)
                    {
                        flag |= 2;
                    }
                    if (flag == 3)
                    {
                        return false;
                    }
                }
            }
            return flag != 0;
        }

        public bool IsHole(int index)
        {
            bool result = false;
            if (index >= 0 && index < NumContours)
            {
                result = holes[index];
                if (!isOrientationConsistent(index))
                {
                    Console.WriteLine("[DEBUG] Polygon orientation not consistent.");
                }
            }
            return result;
        }

        public void SetHole(int index, bool hole)
        {
            if (index >= 0 && index < NumContours)
            {
                holes[index] = hole;
                checkOrientationConsistency(index);
            }
        }

        public void SetContour(int index, List<Vec2d> vs)
        {
            SetContourAndHole(index, vs, false);
        }

        public void SetContourAndHole(int index, List<Vec2d> vs, bool hole)
        {
            if (index >= 0 && index < NumContours)
            {
                contours[index] = vs;
                holes[index] = hole;
                checkOrientationConsistency(index);
            }
        }

        public int GetNumberOfFilledContours()
        {
            int holeCount = 0;
            for (int i = 0; i < NumContours; ++i)
            {
                if (!holes[i])
                {
                    holeCount++;
                }
            }
            return holeCount;
        }

        public int GetNumberOfHoles()
        {
            int holeCount = 0;
            for (int i = 0; i < NumContours; ++i)
            {
                if (holes[i])
                {
                    holeCount++;
                }
            }
            return holeCount;
        }

        public int GetNumberOfVertices()
        {
            int result = 0;
            for (int i = 0; i < NumContours; ++i)
            {
                result += contours[i].Count;
            }
            return result;
        }

        public static double Perimeter(List<Vec2d> poly)
        {
            double result = 0.0;
            for (int i = 0; i < poly.Count; ++i)
            {
                int j = (i + 1) % poly.Count;
                result += (poly[j] - poly[i]).Length;
            }
            return result;
        }       

        public bool Contains(Vec2d v)
        {
            return Contains(v.X, v.Y);
        }

        public static bool Contains(List<Vec2d> poly, double x, double y)
        {
            return Contains(poly, new Vec2d(x, y));
        }

        public static bool Contains(List<Vec2d> poly, Vec2d point)
        {
            int i, j = poly.Count - 1;
            bool result = false;
            for (i = 0; i < poly.Count; ++i)
            {
                if (poly[i].Y < point.Y && poly[j].Y >= point.Y || poly[j].Y < point.Y && poly[i].Y >= point.Y)
                {
                    if (poly[i].X + (point.Y - poly[i].Y) / (double)(poly[j].Y - poly[i].Y) * (poly[j].X - poly[i].X) < point.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public bool Contains(double x, double y)
        {
            bool result = false;
            for (int i = 0; i < this.NumContours; ++i)
            {
                if (Contains(this[i], x, y))
                {
                    result = !this.holes[i];
                }
            }
            return result;
        }

        public Rect GetBoundingBox()
        {
            return BoundingBox();
        }

        public Rect BoundingBox()
        {
            int xMin = Int32.MaxValue;
            int xMax = 0;
            int yMin = Int32.MaxValue;
            int yMax = 0;
            for (int j = 0; j < contours.Length; ++j)
            {
                if (!IsHole(j))
                {
                    Rect bBox = BoundingBox(contours[j]);
                    xMin = Math.Min(xMin, bBox.X);
                    yMin = Math.Min(yMin, bBox.Y);
                    xMax = Math.Max(xMax, bBox.X + bBox.Width);
                    yMax = Math.Max(yMax, bBox.Y + bBox.Height);
                }
            }
            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        public static Rect BoundingBox(List<Vec2d> poly)
        {
            double minX = Double.MaxValue;
            double minY = Double.MaxValue;
            double maxX = 0;
            double maxY = 0;
            for (int i = 0; i < poly.Count; i++)
            {
                Vec2d v = poly[i];

                minX = Math.Min(minX, v.X);
                minY = Math.Min(minY, v.Y);
                maxX = Math.Max(maxX, v.X);
                maxY = Math.Max(maxY, v.Y);
            }
            return new Rect((int)minX, (int)minY, (int)Math.Ceiling(maxX - minX), (int)Math.Ceiling(maxY - minY));
        }

        public Polygon GetSubPolygon(int index)
        {
            Polygon result = null;
            if (isValidContourIndex(index) && !IsHole(index))
            {
                int endIndex = index + 1;
                bool found = false;
                while (!found && endIndex < this.NumContours)
                {
                    if (IsHole(endIndex))
                    {
                        ++endIndex;
                    }
                    else
                    {
                        found = true;
                    }
                }
                result = new Polygon(endIndex - index);
                result.SetContourAndHole(0, new List<Vec2d>(this[index]), false);
                for (int i = index + 1; i < endIndex && i < this.NumContours; ++i)
                {
                    result.SetContourAndHole(i - index, new List<Vec2d>(this[i]), true);
                }
            }
            return result;
        }

        public bool InHole(Vec2d v)
        {
            bool insideHole = false;
            for (int i = 0; !insideHole && i < this.NumContours; i++)
            {
                if (this.holes[i] && Polygon.Contains(this[i], v))
                {
                    insideHole = true;
                }
            }
            return insideHole;
        }

        public static Vec2d Center(List<Vec2d> poly)
        {
            double x = 0.0;
            double y = 0.0;
            for (int i = 0; i < poly.Count; i++)
            {
                x += poly[i].X;
                y += poly[i].Y;
            }
            return new Vec2d(x / poly.Count, y / poly.Count);
        }

        public static Vec2d CenterOfMass(List<Vec2d> poly)
        {
            double cx = 0.0;
            double cy = 0.0;
            double area = Polygon.AreaSigned(poly);
            double factor = 0.0;
            for (int i = 0; i < poly.Count; i++)
            {
                int j = (i + 1) % poly.Count;
                factor = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
                cx += (poly[i].X + poly[j].X) * factor;
                cy += (poly[i].Y + poly[j].Y) * factor;
            }
            area *= 6.0;
            factor = 1.0 / area;
            cx *= factor;
            cy *= factor;
            return new Vec2d(cx, cy);
        }

        public double MaxDistanceToPoint(Vec2d point)
        {
            double result = 0.0;
            for (int i = 0; i < this.NumContours; ++i)
            {
                if (!IsHole(i))
                {
                    result = Math.Max(result, MaxDistanceToPoint(this[i], point));
                }
            }
            return result;
        }

        /// <summary>
        /// Minimum distance of the specified point to the specified polygon.
        /// Returns 0 if point is contained in polygon.
        /// </summary>
        /// <param name="poly">The (simple) polygon.</param>
        /// <param name="point">The point.</param>
        /// <returns>Minimum distance.</returns>
        public static double MinDistanceToPoint(List<Vec2d> poly, Vec2d point)
        {
            double result = Contains(poly, point) ? 0.0 : Double.MaxValue;
            if (result > 0.0)
            {
                result = distance(poly, point);
            }
            return result;
        }


        public static double MinDistanceToBounds(List<Vec2d> poly, Vec2d point)
        {
            return distance(poly, point);
        }

        public static double MaxDistanceToPoint(List<Vec2d> poly, Vec2d center)
        {
            double maxDistance = 0.0;
            for (int i = 0; i < poly.Count; ++i)
            {
                double dist = Vec2d.Distance(poly[i], center);
                if (dist > maxDistance)
                {
                    maxDistance = dist;
                }
            }
            return maxDistance;
        }

        public bool Overlaps(Polygon poly)
        {
            for (int j = 0; j < poly.NumContours; ++j)
            {
                if (!poly.IsHole(j))
                {
                    if (Overlaps(poly[j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Overlaps(List<Vec2d> poly)
        {
            for (int j = 0; j < contours.Length; ++j)
            {
                if (!IsHole(j))
                {
                    if (MathUtilD.OverlapsPolygonPolygon(poly, contours[j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public double Area()
        {
            double area = 0.0;
            for (int j = 0; j < contours.Length; ++j)
            {
                if (!IsHole(j))
                {
                    area += Area(this[j]);
                }
            }
            return area;
        }

        public static double Area(List<Vec2d> poly)
        {
            return Math.Abs(AreaSigned(poly));
        }

        public static double AreaSigned(List<Vec2d> poly)
        {
            double area = 0.0;
            for (int i = 0; i < poly.Count; ++i)
            {
                int j = (i + 1) % poly.Count;
                // (x2-x1) (y2+y1)
                area += poly[i].X * poly[j].Y;
                area -= poly[j].X * poly[i].Y;
            }
            area *= 0.5;
            return area;
        }

        public static bool IsClockWise(List<Vec2d> poly)
        {
            double result = 0.0;
            double resultAlt = 0.0;
            for (int i = 0; i < poly.Count; ++i)
            {
                int j = (i + 1) % poly.Count;
                result += poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
                resultAlt += (poly[j].X - poly[i].X) * (poly[j].Y + poly[i].Y);
            }
            bool cw = result > 0.0;
            bool cwAlt = result > 0.0;
            if (cw != cwAlt)
            {
                Console.WriteLine("[ERROR] Disagreement on isClockWise: {0} vs. {1}", cw, cwAlt);
            }
            return result > 0.0; // Because of x-right, y-down axis system
        }


        public double Distance(Vec2d point)
        {
            double minDist = Double.MaxValue;
            for (int j = 0; j < contours.Length; ++j)
            {
                if (!IsHole(j))
                {
                    minDist = Math.Min(minDist, distance(contours[j], point));
                }
            }
            return minDist;
        }        

        public List<Vec2d> GetRandomPoints(int count)
        {
            return GetRandomPoints(count, 0.0);
        }

        public List<Vec2d> GetRandomPoints(int count, double minDist)
        {
            List<Vec2d> result = new List<Vec2d>(count);
            TriStrip strip = GpcWrapper.PolygonToTristrip(this);
            int numFound = 0;
            double totalArea = 0.0;
            double[] stripAreas = new double[strip.Count];
            double[][] triangleAreas = new double[strip.Count][];
            for (int j = 0; j < strip.Count; ++j)
            {
                triangleAreas[j] = new double[strip[j].Count - 2];
                double stripArea = 0.0;
                for (int i = 0; i < strip[j].Count - 2; ++i)
                {
                    double area = MathUtilD.AreaOfTriangle(strip[j][i], strip[j][i + 1], strip[j][i + 2]);
                    triangleAreas[j][i] = area;
                    stripArea += area;
                }
                stripAreas[j] = stripArea;
                totalArea += stripArea;
                if (stripArea > 0.0)
                {
                    stripArea = 1.0 / stripArea;
                    double sum = 0.0;
                    for (int i = 0; i < strip[j].Count - 2; ++i)
                    {
                        triangleAreas[j][i] *= stripArea;
                        triangleAreas[j][i] += sum;
                        sum = triangleAreas[j][i];
                    }
                }
            }
            if (totalArea > 0.0)
            {
                totalArea = 1.0 / totalArea;
                double sum = 0.0;
                for (int j = 0; j < strip.Count; ++j)
                {
                    stripAreas[j] *= totalArea;
                    stripAreas[j] += sum;
                    sum = stripAreas[j];
                }
            }
            while (numFound < count)
            {
                double sR = RandomNumber.Random();
                double tR = RandomNumber.Random();
                int stripIndex = 0;
                while (sR > stripAreas[stripIndex])
                {
                    stripIndex++;
                }
                int triangleIndex = 0;
                while (tR > triangleAreas[stripIndex][triangleIndex])
                {
                    triangleIndex++;
                }
                Vec2d v0 = strip[stripIndex][triangleIndex];
                Vec2d v1 = strip[stripIndex][triangleIndex + 1];
                Vec2d v2 = strip[stripIndex][triangleIndex + 2];
                Vec2d loc = MathUtilD.RandomPointInTriangle(v0, v1, v2);
                if (minDist <= 0.0 || Distance(loc) > minDist)
                {
                    // Valid location
                    result.Add(loc);
                }
                numFound++;
            }
            return result;
        }
        #endregion

        #region Conversions
        public static List<Vec2d> GetCCWPolygon(List<Vec2d> points)
        {
            List<Vec2d> result = new List<Vec2d>(points.Count);
            result.AddRange(points);
            if (IsClockWise(points))
            {
                result.Reverse();
            }
            return result;
        }

        public static List<Vec2d> ReorderPolygon(List<Vec2d> points, int index)
        {
            List<Vec2d> result = new List<Vec2d>(points.Count);
            for (int i = index; i < points.Count; ++i)
            {
                result.Add(points[i]);
            }
            for (int i = 0; i < index; ++i)
            {
                result.Add(points[i]);
            }
            return result;
        }

        public List<LineVec2> GetBoundaryLines()
        {
            List<LineVec2> result = null;
            if (IsSimple())
            {
                result = new List<LineVec2>(this[0].Count);
                for (int i = 0; i < this[0].Count; ++i)
                {
                    int j = (i + 1) % this[0].Count;
                    result.Add(new LineVec2(this[0][i], this[0][j]));
                }
            }
            return result;
        }

        public List<Vec2d> GetAllVertices()
        {
            List<Vec2d> result = new List<Vec2d>(GetNumberOfVertices());
            for (int j = 0; j < contours.Length; ++j)
            {
                for (int i = 0; i < contours[j].Count; i++)
                {
                    result.Add(contours[j][i]);
                }
            }
            return result;
        }

        public List<Line2> ToLines()
        {
            List<Line2> lines = new List<Line2>();
            for (int c = 0; c < contours.Length; c++)
            {
                for (int i = 0; i < contours[c].Count; i++)
                {
                    int j = (i + 1) % contours[c].Count;
                    lines.Add(new Line2(new Vec2(contours[c][i]), new Vec2(contours[c][j])));
                }
            }
            return lines;
        }

        public GraphicsPath ToGraphicsPath()
        {
            GraphicsPath path = new GraphicsPath();
            for (int i = 0; i < this.NumContours; i++)
            {
                PointF[] points = Vec2d.ToPointFArray(contours[i]);
                if (IsHole(i))
                {
                    Array.Reverse(points);
                }
                path.AddPolygon(points);
            }
            return path;
        }

        public TriStrip ToTriangleStrip()
        {
            return GpcWrapper.PolygonToTristrip(this);
        }
#endregion

        #region Operations
        public Polygon GetUnion(Polygon poly)
        {
            return Union(poly);
        }

        public Polygon GetOverlap(Polygon poly)
        {
            return Intersection(poly);
        }

        public Polygon Difference(Polygon poly)
        {
            Polygon result = null;
            if (poly != null)
            {
#if DO_TIMING
                Stopwatch sw = new Stopwatch();
                sw.Start();
#endif
#if USE_GPC
                result = GpcWrapper.Clip(GpcWrapper.GpcOperation.Difference, this, poly);
#if DO_TIMING
                sw.Stop();
                Console.WriteLine("[Debug] GPC difference clipping took {0} ms.", sw.ElapsedMilliseconds);
                sw.Restart();
#endif
#endif
#if USE_CLIPPER
                result = ClipperWrapper.GetDifference(this, poly);
#if DO_TIMING
                sw.Stop();
                Console.WriteLine("[Debug] Clipper difference clipping took {0} ms.", sw.ElapsedMilliseconds);
#endif
#endif
            }
            return result;
        }

        public Polygon Union(Polygon poly)
        {
            Polygon result = null;
            if (poly != null)
            {
#if DO_TIMING
                Stopwatch sw = new Stopwatch();
                sw.Start();
#endif
#if USE_GPC
                result = GpcWrapper.Clip(GpcWrapper.GpcOperation.Union, this, poly);
#if DO_TIMING
                sw.Stop();
                Console.WriteLine("[Debug] GPC union clipping took {0} ms.", sw.ElapsedMilliseconds);
                sw.Restart();
#endif
#endif
#if USE_CLIPPER
                result = ClipperWrapper.GetUnion(this, poly);
#if DO_TIMING
                sw.Stop();
                Console.WriteLine("[Debug] Clipper union clipping took {0} ms.", sw.ElapsedMilliseconds);
#endif
#endif
            }
            return result;
        }

        public Polygon Intersection(Polygon poly)
        {
            Polygon result = null;
            if (poly != null)
            {
#if DO_TIMING
                Stopwatch sw = new Stopwatch();
                sw.Start();
#endif
#if USE_GPC
                result = GpcWrapper.Clip(GpcWrapper.GpcOperation.Intersection, this, poly);
#if DO_TIMING
                sw.Stop();
                Console.WriteLine("[Debug] GPC intersection clipping took {0} ms.", sw.ElapsedMilliseconds);
                sw.Restart();
#endif
#endif
#if USE_CLIPPER
                result = ClipperWrapper.GetIntersection(this, poly);
#if DO_TIMING
                sw.Stop();
                Console.WriteLine("[Debug] Clipper intersection clipping took {0} ms.", sw.ElapsedMilliseconds);
#endif
#endif
            }
            return result;
        }

        public Polygon Offset(double offset)
        {
            Polygon result = this;
            if (this.NumContours == 1)
            {
#if USE_CLIPPER
                result = ClipperWrapper.GetOffsetPolygon(this, offset);
#endif
            }
            return result;
        }
        #endregion

        #region Reader/writer
        public static Polygon FromXML(XmlReader reader)
        {
            // Number of contours
            reader.ReadStartElement(XML_ELEMENT_NUM_CONTOURS);
            int contoursCount = reader.ReadContentAsInt();
            reader.ReadEndElement();
            Polygon result = new Polygon(contoursCount);
            reader.ReadStartElement(XML_ELEMENT_CONTOUR);
            for (int j = 0; j < contoursCount; ++j)
            {
                // Number of points in contour
                reader.ReadStartElement(XML_ELEMENT_CONTOUR_COUNT);
                int contourCount = reader.ReadContentAsInt();
                reader.ReadEndElement();
                List<Vec2d> contour = new List<Vec2d>(contourCount);
                // Whether contour is hole
                reader.ReadStartElement(XML_ELEMENT_IS_HOLE);
                bool isHole = reader.ReadContentAsBoolean();
                reader.ReadEndElement();
                // Points
                for (int i = 0; i < contourCount; ++i)
                {
                    contour.Add(Vec2d.FromXML(reader));
                }
                result.SetContourAndHole(j, contour, isHole);
            }
            reader.ReadEndElement();
            return result;
        }

        public void ToXML(XmlWriter writer)
        {
            // Number of contours
            writer.WriteStartElement(XML_ELEMENT_NUM_CONTOURS);
            writer.WriteValue(contours.Length);
            writer.WriteEndElement();
            writer.WriteStartElement(XML_ELEMENT_CONTOUR);
            for (int j = 0; j < contours.Length; ++j)
            {
                // Number of points in contour
                writer.WriteStartElement(XML_ELEMENT_CONTOUR_COUNT);
                writer.WriteValue(contours[j].Count);
                writer.WriteEndElement();
                // Whether contour is hole
                writer.WriteStartElement(XML_ELEMENT_IS_HOLE);
                writer.WriteValue(holes[j]);
                writer.WriteEndElement();
                // Points
                for (int i = 0; i < contours[j].Count; ++i)
                {
                    contours[j][i].ToXML(writer);
                }
            }
            writer.WriteEndElement();
        }
        #endregion

        #region Helpers
        private void checkOrientationConsistency(int index)
        {
            if (!isOrientationConsistent(index))
            {
                this.contours[index].Reverse();
            }
        }

        private bool isOrientationConsistent(int index)
        {
            return IsClockWise(this.contours[index]) == cwContourOrientation != this.holes[index];
        }

        private static double distance(List<Vec2d> poly, Vec2d point)
        {
            double minDist = Double.MaxValue;
            for (int i = 0; i < poly.Count; ++i)
            {
                Vec2d t1 = poly[i];
                Vec2d t2 = poly[(i + 1) % poly.Count];
                double factor = Math.Max(0.0, Math.Min(1.0, MathUtilD.PointProjectedOnLine(point, t1, t2)));
                Vec2d intersectionPoint = t1 + factor * (t2 - t1);
                minDist = Math.Min(minDist, (point - intersectionPoint).Length);
            }
            return minDist;
        }

        private bool isValidContourIndex(int index)
        {
            return index >= 0 && index < this.NumContours;
        }
        #endregion
    }
}
