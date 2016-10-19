using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
	public class Vec2i {
		public int X {
			get { return x; }
			set { x = value; }
		}

		public int Y {
			get { return y; }
			set { y = value; }
		}

		public Vec2i Cross()
		{
			return new Vec2i(Y, -X);
		}

		public int this[int index]
		{
			get
			{
				return (index == 0) ? X : Y;
			}
			set
			{
				switch (index)
				{
					case 0:
						X = value;
						return;
					case 1:
						Y = value;
						return;
				}
			}
		}

		public double Length {
			get {
				return Math.Sqrt(x * x + y * y);
			}
		}

		public double Length2 {
			get {
				return x * x + y * y;
			}
		}
		public Vec2 ToVec2() {
			return new Vec2(x, y);
		}

		public Vec2d ToVec2d() {
			return new Vec2d(x, y);
		}

		private int x;
		private int y;

		public Vec2i(Vec2 v) : this((int) Math.Round(v.X), (int) Math.Round(v.Y)) {
		}

		public Vec2i(Vec2d v) : this((int) Math.Round(v.X), (int) Math.Round(v.Y)) {
		}

		public Vec2i(Vec2i copy) {
			this.x = copy.x;
			this.y = copy.y;
		}

		public Vec2i(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public void setValues(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public double SquareLength() {
			return x * x + y * y;
		}

		public static double Distance(Vec2i a, Vec2i b) {
			return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
		}

		public static Vec2i operator +(Vec2i left, Vec2i right) {
			Vec2i result = new Vec2i(0, 0);
			result.X = left.X + right.X;
			result.Y = left.Y + right.Y;
			return result;
		}

		public static Vec2i operator -(Vec2i left, Vec2i right) {
			Vec2i result = new Vec2i(0, 0);
			result.X = left.X - right.X;
			result.Y = left.Y - right.Y;
			return result;
		}

		public static Vec2i operator -(Vec2i vec) {
			Vec2i result = new Vec2i(-vec.X, -vec.Y);
			return result;
		}

		public static Vec2i operator *(Vec2i vec, int d) {
			Vec2i result = new Vec2i(vec.X * d, vec.Y * d);
			return result;
		}

		public static Vec2i operator *(Vec2i vec, float d)
		{
			Vec2i result = new Vec2i((int)Math.Round((float)vec.X * d), (int)Math.Round((float)vec.Y * d));
			return result;
		}

		public static Vec2i operator *(int d, Vec2i vec) {
			return vec * d;
		}

		public static implicit operator Vec2i(Vec3i v)
		{
			return new Vec2i(v.X, v.Z);
		}

		public static double Dot(Vec2i left, Vec2i right) {
			return left.X * right.X + left.Y * right.Y;
		}

		public static double DistanceToLine(Vec2i point, Vec2i t1, Vec2i t2, out Vec2i intersectionPoint) {
			double factor = Math.Max(0, Math.Min(1.0, ((point.X - t1.X) * (t2.X - t1.X) + (point.Y - t1.Y) * (t2.Y - t1.Y)) / (t2 - t1).SquareLength()));
			Vec2i l = (t2 - t1);
			// Note: snap to integer coords
			intersectionPoint = new Vec2i((int) Math.Round(t1.X + factor * l.X), (int) Math.Round(t1.Y + factor * l.Y));
			return (point - intersectionPoint).Length;
		}

		public override string ToString() {
			return String.Format("[{0:d}, {1:d}]", x, y);
		}

		public override bool Equals(object obj) {
			if (obj == null || typeof(Vec2i) != obj.GetType()) {
				return false;
			} else return ((Vec2i)obj).X == x && ((Vec2i)obj).Y == y;
		}

		public override int GetHashCode() {
			int PRIME = 31;
			int result = 1;
			result = PRIME * result + x;
			result = PRIME * result + y;
			return result;
		}

		//public Vec2i Normalize() {
		//    double length = this.Length;
		//    if (length > 0) {
		//        double scale = 1.0 / length;
		//        X *= (int)scale;
		//        Y *= (int)scale;
		//    }
		//    return this;
		//}

		public Vec2d getNormalized() {
			double length = this.Length;
			double x = (double)X;
			double y = (double)Y;
			if (length > 0) {
				double scale = 1.0 / length;
				x = (double)X * scale;
				y = (double)Y * scale;
			}
			return new Vec2d(x, y);
		}

		public static bool Intersection(Vec2i p1, Vec2i p2, Vec2i p3, Vec2i p4, out float intersectionFactor, out float intersectionFactor2,
											out Vec2i intersectionPoint)
		{
			//--- see: http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline2d/
			float ua_denominator = ((float)(p4.Y - p3.Y) * (float)(p2.X - p1.X)) - ((float)(p4.X - p3.X) * (float)(p2.Y - p1.Y));
			if (ua_denominator == 0)
			{
				intersectionFactor = float.NaN;
				intersectionFactor2 = float.NaN;
				intersectionPoint = new Vec2i(0, 0);
				return false;
			}
			intersectionFactor = (((float)(p4.X - p3.X) * (float)(p1.Y - p3.Y)) - ((float)(p4.Y - p3.Y) * (float)(p1.X - p3.X))) / ua_denominator;
			intersectionFactor2 = (((float)(p2.X - p1.X) * (float)(p1.Y - p3.Y)) - ((float)(p2.Y - p1.Y) * (float)(p1.X - p3.X))) / ua_denominator;

			intersectionPoint = new Vec2i((int)Math.Round(p3.X + intersectionFactor2 * (float)(p4.X - p3.X)), (int)Math.Round(p3.Y + intersectionFactor2 * (float)(p4.Y - p3.Y)));

			return intersectionFactor >= 0 && intersectionFactor <= 1 && intersectionFactor2 >= 0 && intersectionFactor2 <= 1;
		}

		public float squareLength() { return (float)x * (float)x + (float)y * (float)y; }

		public float length() { return (float)Math.Sqrt(squareLength()); }

		public static List<Vec2i> MinkowskiSum(List<Vec2i> A, List<Vec2i> B)
		{
			List<Vec2i> ret = new List<Vec2i>();

			foreach(Vec2i b in B)
				foreach(Vec2i a in A)
					ret.Add(a + b);

			return QuickHull(ret);
		}

		private static List<Vec2i> QuickHull(List<Vec2i> points)
		{
			List<Vec2i> convexHull = new List<Vec2i>();
			if (points.Count < 3)
				return points;

			// find extremals
			int minPoint = -1, maxPoint = -1;
			int minX = int.MaxValue;
			int maxX = int.MinValue;

			for (int i = 0; i < points.Count; i++)
			{
				if (points[i].X < minX) 
				{
					minX = points[i].X;
					minPoint = i;
				} 
				if (points[i].X > maxX)
				{
					maxX = points[i].X;
					maxPoint = i;       
				}
			}

			Vec2i A = points[minPoint];
			Vec2i B = points[maxPoint];
			convexHull.Add(A);
			convexHull.Add(B);
			points.Remove(A);
			points.Remove(B);

			List<Vec2i> leftSet = new List<Vec2i>();
			List<Vec2i> rightSet = new List<Vec2i>();

			for (int i = 0; i < points.Count; i++)
			{
				Vec2i p = points[i];
				if (pointLocation(A, B, p) == -1)
					leftSet.Add(p);
				else
					rightSet.Add(p);
			}
			hullSet(A, B, rightSet, convexHull);
			hullSet(B, A, leftSet, convexHull);

			return convexHull;
		}

		private static void hullSet(Vec2i A, Vec2i B, List<Vec2i> set, List<Vec2i> hull)
		{
			int insertPosition = hull.IndexOf(B);
			if (set.Count == 0)
				return;
			if (set.Count == 1)
			{
				Vec2i p = set[0];
				set.Remove(p);
				hull.Insert(insertPosition, p);
				return;
			}

			int dist = int.MinValue;
			int furthestPoint = -1;
			for (int i = 0; i < set.Count; i++)
			{
				Vec2i p = set[i];
				int d = distance(A, B, p);
				if (d > dist)
				{
					dist = d;
					furthestPoint = i;
				}
			}

			Vec2i P = set[furthestPoint];
			set.RemoveAt(furthestPoint);
			hull.Insert(insertPosition, P);

			// Determine who's to the left of AP
			List<Vec2i> leftSetAP = new List<Vec2i>();
			for (int i = 0; i < set.Count; i++)
			{
				Vec2i M = set[i];
				if (pointLocation(A, P, M) == 1)
				{
					//set.remove(M);
					leftSetAP.Add(M);
				}
			}

			// Determine who's to the left of PB
			List<Vec2i> leftSetPB = new List<Vec2i>();
			for (int i = 0; i < set.Count; i++)
			{
				Vec2i M = set[i];
				if (pointLocation(P, B, M) == 1)
				{
					//set.remove(M);
					leftSetPB.Add(M);
				}
			}
			hullSet(A, P, leftSetAP, hull);
			hullSet(P, B, leftSetPB, hull);
		}

		private static int distance(Vec2i A, Vec2i B, Vec2i C)
		{
			int ABx = B.X - A.X;
			int ABy = B.Y - A.Y;
			int num = ABx * (A.Y - C.Y) - ABy * (A.X - C.X);
			if (num < 0)
				num = -num;
			return num;
		}

		private static int pointLocation(Vec2i A, Vec2i B, Vec2i P)
		{
			int cp1 = (B.X - A.X) * (P.Y - A.Y) - (B.Y - A.Y) * (P.X - A.X);
			return (cp1 > 0) ? 1 : -1;
		}

		internal static List<List<Vec2i>> MinkowskiMinus(List<Vec2i> A, List<Vec2i> B)
		{
			
			List<Polygon> polys = new List<Polygon>();

			for(int b = 0; b < B.Count; ++b)
				polys.Add(new Polygon(new List<Vec2i>(Minus(A, B[b]))));

			Polygon result = polys[0];
			for (int i = 1; i < polys.Count; ++i)
			{
				result = GpcWrapper.Clip(GpcWrapper.GpcOperation.Intersection, result, polys[i]);
				if (result == null || result.NumContours == 0)
					return new List<List<Vec2i>>();
			}

			List<List<Vec2i>> shapes = new List<List<Vec2i>>();

			for(int i = 0; i < result.NumContours; ++i)
			{
				List<Vec2d> vl = result[i];

				List<Vec2i> points = new List<Vec2i>();
				for(int j = 0; j < vl.Count; ++j)
				{
					Vec2d tv = vl[j];
					points.Add(new Vec2i((int)Math.Round(tv.X), (int)Math.Round(tv.Y)));
				}

				shapes.Add(points);
			}
			return shapes;
		}

		private static IEnumerable<Vec2i> Minus(List<Vec2i> A, Vec2i min)
		{
			foreach (Vec2i a in A)
				yield return a - min;
		}

		public float GetAngle()
		{
			Vec2 norm = new Vec2(this);
			norm.Normalize();
			float ang = (float)Math.Acos(norm.X);
			if (norm.Y < 0)
			{
				ang = 2.0f * (float)Math.PI - ang;
			}
			return ang;
		}

		public static List<Vec2i> FromVec2dList(List<Vec2d> points)
		{
			List<Vec2i> result = new List<Vec2i>(points.Count);
			foreach (Vec2d vd in points)
				result.Add(new Vec2i(vd));
			return result;
		}
	}
}

