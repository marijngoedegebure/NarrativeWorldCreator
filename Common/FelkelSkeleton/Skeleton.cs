using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Common.FelkelSkeleton
{
    public static class Skel
    {

        public class Contour : System.Collections.Generic.List<Common.FelkelSkeleton.Skel.Point>
        {
        }
        public class ContourVector : System.Collections.Generic.List<Contour>
        {
        }

        const int EOF = -1;
        const double M_PI = Math.PI;
        const double MIN_DIFF = 0.0000005;
        const double MIN_ANGLE_DIFF = 0.0000005;
        const double INFINITY = double.MaxValue;
        //--- PRIM FILE


        public struct Segment
        {
          public Segment (Point p) : this(p, new Point()) {}
          public Segment(Point p, Point q)
          {
               a=p;
              b = q;
          }
          public Segment(Number x1, Number y1, Number x2, Number y2)
          {
              a = new Point(x1, y1);
              b = new Point(x2, y2);
          }

          public Point a, b;
        }
        
        public class VertexList : List<Vertex>
        {

            public VertexList() { }
            public Vertex prev(int i) { return this[(i == 0) ? this.Count - 1 : i - 1]; }
            public Vertex next(int i) { return this[(i == this.Count - 1) ? 0 : i + 1]; }

            public void push_back(Vertex x)
            {
                System.Diagnostics.Debug.Assert(x.prevVertex == null || facingTowards(x.leftLine, x.prevVertex.rightLine));
                System.Diagnostics.Debug.Assert(x.nextVertex == null || facingTowards(x.rightLine, x.nextVertex.leftLine));
                x.ID = this.Count;       // automaticke cislovani
                this.Add(x);
            }

            internal Vertex back()
            {
                return this[this.Count - 1];
            }
        }

        public static Number normalizeAngle (Number angle)
        {
          if (angle >=  M_PI) { angle = angle - 2*M_PI; return normalizeAngle (angle); }
          if (angle < -M_PI) { angle = angle + 2*M_PI; return normalizeAngle (angle); }
          return angle;
        }

        public static bool pointOnRay (Point p, Ray r)
        { 
            return (p == r.origin || (new Ray (r.origin, p)).angle == r.angle); 
        }

        public static bool colinear (Ray a, Ray b)
        {
          Number aa = a.angle;
          Number ba = b.angle;
          Number aa2 = a.angle + M_PI;
          aa = normalizeAngle (aa);
          ba = normalizeAngle (ba);
          aa2 = normalizeAngle (aa2);
          return ba == aa || ba == aa2;
        }

        public static Point intersection (Ray  a, Ray b)
        {
          if (a.origin == b.origin) return a.origin;
          if (pointOnRay (b.origin, a) && pointOnRay (a.origin, b))
              return new Point ((a.origin.x + b.origin.x)/2, (a.origin.y + b.origin.y)/2);
          if (pointOnRay (b.origin, a)) return b.origin;
          if (pointOnRay (a.origin, b)) return a.origin;
          if (colinear (a, b)) return new Point (INFINITY, INFINITY);

          Number sa = Math.Sin (a.angle);
          Number sb = Math.Sin (b.angle);
          Number ca = Math.Cos (a.angle);
          Number cb = Math.Cos (b.angle);
          Number x = sb*ca - sa*cb;
          if (x == 0.0) return new Point (INFINITY, INFINITY);
          Number u = (cb*(a.origin.y - b.origin.y) - sb*(a.origin.x - b.origin.x))/x;
          if (u != 0.0 && u < 0.0) return new Point (INFINITY, INFINITY);
          if ((ca*(b.origin.y - a.origin.y) - sa*(b.origin.x - a.origin.x))/x > 0) return new Point (INFINITY, INFINITY);
          return new Point (a.origin.x + u*ca, a.origin.y + u*sa);
        }

        public static Point intersectionAnywhere (Ray a, Ray b)
        {
          if (a.origin == b.origin) return a.origin;
          if (pointOnRay (b.origin, a) && pointOnRay (a.origin, b))
              return new Point ((a.origin.x + b.origin.x)/2, (a.origin.y + b.origin.y)/2);
          if (pointOnRay (b.origin, a)) return b.origin;
          if (pointOnRay (a.origin, b)) return a.origin;
          if (pointOnRay (b.origin, a.opaque ()) && pointOnRay (a.origin, b.opaque ()))
              return new Point ((a.origin.x + b.origin.x)/2, (a.origin.y + b.origin.y)/2);

          if (colinear (a, b)) return new Point (INFINITY, INFINITY);
          Number sa = Math.Sin (a.angle);
          Number sb = Math.Sin (b.angle);
          Number ca = Math.Cos (a.angle);
          Number cb = Math.Cos (b.angle);
          Number x = sb*ca - sa*cb;
          if (x == 0.0) return new Point (INFINITY, INFINITY);
          Number u = (cb*(a.origin.y - b.origin.y) - sb*(a.origin.x - b.origin.x))/x;
          return new Point (a.origin.x + u*ca, a.origin.y + u*sa);
        }

        public static bool facingTowards (Ray a, Ray b) { return pointOnRay (a.origin, b) && pointOnRay (b.origin, a) && !(a.origin == b.origin); }

        public static Number dist (Point p, Point q)
        {
          return Math.Sqrt ((p.x-q.x)*(p.x-q.x) + (p.y - q.y)*(p.y - q.y));
        }

        public static Number dist (Ray l, Point p)
        {
            return dist(p, l);
        }

        public static Number dist (Point p, Ray l)
        {
          Number a = l.angle - new Ray (l.origin, p).angle;
          Number d = Math.Sin (a) * dist (l.origin, p);
          if (d < 0.0) return -d;
          return d;
        }
        public static bool SIMILAR(double a, double b) { return  ((a)-(b) < MIN_DIFF && (b)-(a) < MIN_DIFF); }
        public static bool ANGLE_SIMILAR(double a, double b) { return (normalizedAngle (a) - normalizedAngle (b) < MIN_ANGLE_DIFF && normalizedAngle (b) - normalizedAngle (a) < MIN_ANGLE_DIFF); }
        public static Number normalizedAngle (Number angle) { Number temp = angle; normalizeAngle (temp); return temp; }

        public class Ray
        {
            internal Point origin;
            internal Number angle;

            internal Ray() : this(new Point(0, 0), new Point(0, 0)) {}
            internal Ray(Point p) : this(p, new Point(0, 0)) {}

            internal Ray(Point p, Point q)
            {
                this.origin = p;
                angle = normalizeAngle(new Number(Math.Atan2(q.y - p.y, q.x - p.x)));
            }
            internal Ray (Point p, Number a) 
            {
                origin = p;
                angle = normalizeAngle(a);
            }
            internal Ray opaque () { return new Ray (origin, angle + M_PI); }

            public override string ToString()
            {
                return "Ray (" + origin + " < " + angle + ")";
            }
        };

        public struct Number
        {
            //static bool SIMILAR(double a, double b) { return Skel.SIMILAR(a, b); }

            double n;
            public Number (double x)
            {
                this.n = x;
            }
            //  operator const double& (void) const { return n; }
            public static implicit operator Number(double x) { return new Number(x); }
            public static implicit operator double (Number n) { return n.n; }

            public static bool operator== (Number t, Number x) { return SIMILAR(t.n, x.n); }
            public static bool operator!= (Number t, Number x) { return !SIMILAR (t.n, x.n); }
            public static bool operator<= (Number t, Number x) { return t.n < x.n || t == x; }
            public static bool operator>= (Number t, Number x) { return t.n > x.n || t == x; }
            public static bool operator<  (Number t, Number x) { return t.n < x.n && t != x; }
            public static bool operator>  (Number t, Number x) { return t.n > x.n && t != x; }

            public static bool operator== (Number t, double x) { return t == new Number (x); }
            public static bool operator!= (Number t, double x) { return t != new Number (x); }
            public static bool operator<= (Number t, double x) { return t <= new Number (x); }
            public static bool operator>= (Number t, double x) { return t >= new Number (x); }
            public static bool operator<  (Number t, double x) { return t <  new Number (x); }
            public static bool operator>  (Number t, double x) { return t >  new Number (x); }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override string ToString()
            {
                return n.ToString();
            }
        }

        public class Point
        {
            public static Point Infinity = new Point(INFINITY, INFINITY);
            public Number x, y;
          
            public Point ()  { x = 0; y = 0; }
            public Point (Number X, Number Y) { x = X; y = Y; }
            public static bool operator ==(Point t, Point p) { if ((object)t == null) return ((object)p) == null; if (((object)p) == null) return false; return t.x == p.x && t.y == p.y; }
            public static bool operator!= (Point t, Point p) { return t.x != p.x || t.y != p.y; }
            public static Point operator * (Point t, Number n) { return new Point (n*t.x, n*t.y); }
            public int isInfinite () { return (this == Infinity) ? 1 : 0; }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override string ToString()
            {
                return "(" + x + ", " + y + ")";
            }
        }
        //---


        //    /*****************************************************************************************************************************\
        //    **                                                     Globalni promenne                                                     **
        //    \*****************************************************************************************************************************/

        public static PriorityQueue<Intersection, Intersection> iq = new PriorityQueue<Intersection, Intersection>(Intersection.Comparer.Instance);          // prioritni fronta pruseciku setridena podle vzdalenosti od nositelky (tj. podle vysky ve strese)
        static Skeleton skeleton = new Skeleton();             // vystupni struktura obsahujici kostru
        public static VertexList vl = new VertexList();          // SLAV, jednotlive cykly (LAV) jsou udrzovany pomoci ukazatelu nextVertex, prevVertex
        public static List<Point> vPoints = new List<Point>();
        public static List<Line2> SSLines = new List<Line2>();
        public static List<Line2> pathLines = new List<Line2>();

        public static Ray angleAxis (Point b, Point a, Point c) // vrati osu uhlu abc prochazejici bodem b
        {
          Ray ba = new Ray(b, a);
          Ray bc = new Ray(b, c);
          if (ba.angle > bc.angle) ba.angle = ba.angle - 2*M_PI;
          return new Ray (b, (ba.angle + bc.angle) / 2);
        }

        public class SkeletonLine
        {
            public class SkeletonPoint
              {
                  public SkeletonPoint()
                      : this(new Vertex(), null, null)
                  {
                  }

                  public SkeletonPoint(Vertex v)
                      : this(v, null, null)
                {
                }
                public SkeletonPoint (Vertex v, SkeletonLine l) : this(v, l, null)
                {
                }
                public SkeletonPoint (Vertex v, SkeletonLine l, SkeletonLine r)
                {
                    vertex = v;
                    left = l;
                    right = r;
                }
                public Vertex vertex;                // ukazatel na vrchol (obsahuje souradnice)
                public SkeletonLine left, right;          // kridla
                public int leftID ()  { if (left == null) return -1; return ((SkeletonLine)left).ID; }
                public int rightID ()  { if (right == null) return -1; return ((SkeletonLine)right).ID; }
                public int vertexID ()  { if (vertex == null) return -1; return vertex.ID; }
              }

                public int ID;                                // Cislo automaticky pridelovane pri vkladani do kostry

            public SkeletonLine (Vertex l, Vertex h)
            {
                ID = -1;
                      lower = new SkeletonPoint(l);
                      higher = new SkeletonPoint(h);
            }
              public static implicit operator Segment (SkeletonLine s) { return new Segment (s.lower.vertex.point, s.higher.vertex.point); } // Jen pro ladeni

            public SkeletonPoint lower, higher;                       // dva body typu SkeletonPoint
              public static bool operator == (SkeletonLine t, SkeletonLine s)
            { if ((object)t == null) return (object)s == null; if ((object)s == null) return false; return t.higher.vertex.ID == s.higher.vertex.ID && t.lower.vertex.ID == s.lower.vertex.ID; }
              public static bool operator != (SkeletonLine t, SkeletonLine s) { return !(t == s); }
              public static bool operator < (SkeletonLine t, SkeletonLine s)  { System.Diagnostics.Debug.Assert (false); return false; }     // kvuli STL, jinak se nepouziva
              public static bool operator >(SkeletonLine t, SkeletonLine s) { return !(t == s) && !(t < s); }     // kvuli STL, jinak se nepouziva

              public override int GetHashCode()
              {
                  return base.GetHashCode();
              }

              public override bool Equals(object obj)
              {
                  return base.Equals(obj);
              }
        }

        public class Vertex
        {
          internal Point point;                       // souradnice vrcholu
          internal Ray axis;                          // bisektor
          internal Ray leftLine, rightLine;           // leva a prava hranicni usecka, axis je jejich osou
          internal Vertex leftVertex, rightVertex;  // 2 vrcholy, jejich zaniknutim vzikl tento
          internal Vertex nextVertex, prevVertex;   // vrchol sousedici v LAV
          internal Vertex higher;                    // vrchol vznikly pri zaniknuti tohoto
          internal bool done;                         // priznak aktivity
          internal int ID;                            // cislo automaticky pridelovane pri vkladani do LAV
          internal SkeletonLine leftSkeletonLine, rightSkeletonLine, advancingSkeletonLine; // Pouzivano pri konstrukci kostry z okridlenych hran

            public Vertex () 
              { 
                  ID = -1;
              }  

            public Vertex(Point p) : this(p, new Point(), new Point())
            {
            }

            public Vertex(Point p, Point prev) : this(p, prev, new Point())
            {
            }
 
            // Bezparametricky konstruktor kvuli STL
          public Vertex (Point p, Point prev, Point next)
          {
              point = p;
              axis = angleAxis(p, prev, next);
              leftLine = new Ray (p, prev);
              rightLine = new Ray(p, next);
              higher = null;
              leftVertex = null;
              rightVertex = null;
              nextVertex = null;
              prevVertex = null;
              done = false;
              ID = -1;
                leftSkeletonLine = null;
              rightSkeletonLine = null;
              advancingSkeletonLine = null;
          }
          public Vertex (Point p, Vertex left, Vertex right)
          {
              point = p;
              done = false;
              higher = null;
              ID = -1;
              leftSkeletonLine = null;
              rightSkeletonLine = null;
              advancingSkeletonLine = null;
              this.leftLine = left.leftLine;    // hrany puvodni kontury, jejichz osou bude vytvareny bisektor vedouci z tohoto vrcholu
              this.rightLine = right.rightLine;
              this.leftVertex = left;
              this.rightVertex = right;

              System.Diagnostics.Debug.Assert (dist (point, leftLine) == dist (point, rightLine)); // vytvareny vchol musi byt stejne daleko od obou hran
              Point i = intersection (leftLine, rightLine);                       // pro urceni smeru bisektoru je potreba znat souradnice
              if (i.x == INFINITY)            // prusecik neni smerem dopredu     // jeste jednoho bodu, vhodnym adeptem je prusecik nositelek
                 {                                                                // hran puvodni kontury
                   System.Diagnostics.Debug.Assert (i.y == INFINITY);
                   i = intersectionAnywhere (leftLine, rightLine);                // Anywhere => prusecik primek a ne poloprimek
                   if (i.x == INFINITY)       // rovnobezne hrany
                      {
                        System.Diagnostics.Debug.Assert (i.y == INFINITY);
                        axis = new Ray (point, leftLine.angle);                       // tvorba bisektoru pri rovnobeznych hranach => || s nimi
                      }
                   else                       // prusecik smerem dozadu
                      {
                        axis = new Ray (point, i);
                        axis.angle = normalizeAngle (axis.angle + M_PI);                                       // tvorba bisektoru
                      }
                 }
              else                            // prusecik smerem dopredu
                 {
                   axis = new Ray (point, i);                                         // tvorba bisektoru
                 }
          }
          public Vertex highest () { return higher != null ? higher.highest() : this; }
          public bool atContour () { return leftVertex == this && rightVertex == this; }
          public Point GetPoint() { return this.point; }

          public static bool operator ==(Vertex t, Vertex v) { if ((object)v == null) return ((object)t) == null; if (((object)t) == null) return false;  return t.point == v.point; }
          public static bool operator !=(Vertex t, Vertex v) { return !(t == v); }
          public static bool operator < (Vertex t, Vertex v) { System.Diagnostics.Debug.Assert (false); return false; } // kvuli STL, jinak se nepouziva
          public static bool operator >(Vertex t, Vertex v) { return !(t == v) && !(t < v); } // kvuli STL, jinak se nepouziva
            // data

          public override int GetHashCode()
          {
              return base.GetHashCode();
          }

          public override bool Equals(object obj)
          {
              return base.Equals(obj);
          }

          public override string ToString()
          {
              return "Vertex(" + point + ", axis:" + this.axis + ")";
          }
        };

        public class Intersection
        {
            public class Comparer : Comparer<Intersection>
            {
                public static Comparer Instance = new Comparer();

                public override int Compare(Intersection x, Intersection y)
                {
                    return x > y ? -1 : 1;
                }
            }

            public Point poi;                         // Souradnice pruseciku
            public Vertex leftVertex, rightVertex;  // 2 vrcholy, jejichz bisektory vytvorily tento prusecik
            public Number height;                     // vzdalenost od nositelky
            public enum Type { CONVEX, NONCONVEX };
            public Type type;

            public Intersection() { }
            // Bezparametricky konstruktor kvuli STL
            public Intersection(Vertex v)          // Vypocet pruseciku pro dany vrchol
            {
                poi = null;
                type = Type.NONCONVEX;
                height = 0;
                rightVertex = null;
                leftVertex = null;

                System.Diagnostics.Debug.Assert(v.prevVertex == null || facingTowards(v.leftLine, v.prevVertex.rightLine));
                System.Diagnostics.Debug.Assert(v.nextVertex == null || facingTowards(v.rightLine, v.nextVertex.leftLine));

                Vertex l = v.prevVertex;       // sousedi v LAV
                Vertex r = v.nextVertex;

                System.Diagnostics.Debug.Assert(v.leftLine.angle == v.leftVertex.leftLine.angle);
                System.Diagnostics.Debug.Assert(v.rightLine.angle == v.rightVertex.rightLine.angle);

                Number al = v.axis.angle - l.axis.angle;
                al = normalizeAngle(al);
                Number ar = v.axis.angle - r.axis.angle;
                ar = normalizeAngle(ar);
                Point i1 = facingTowards(v.axis, l.axis) ? new Point(INFINITY, INFINITY) : intersection(v.axis, l.axis); // pruseciky se sousedn.
                Point i2 = facingTowards(v.axis, r.axis) ? new Point(INFINITY, INFINITY) : intersection(v.axis, r.axis); // vrcholy - edge event
                Number d1 = dist(v.point, i1);     // d1 - vzdalenost od nositelky pruseciku bisektoru s bisektorem leveho souseda v LAV
                Number d2 = dist(v.point, i2);     // d2 - vzdalenost od nositelky pruseciku bisektoru s bisektorem praveho souseda v LAV

                Vertex leftPointer, rightPointer;
                Point p = null;
                Number d3 = INFINITY;               // d3 - u nekonvexnich vrcholu vzdalenost bodu B od nositelky
                Number av = v.leftLine.angle - v.rightLine.angle;     // test na otevrenost uhlu u vrcholu V => konvexni/nekonvexni
                av = normalizeAngle(av);
                if (av > 0.0 &&
                   (intersection(v.leftLine, v.rightLine) == v.point || intersection(v.leftLine, v.rightLine) == new Point(INFINITY, INFINITY)))
                    d3 = nearestIntersection(v, out leftPointer, out rightPointer, out p);   // zjisteni vzdalenosti bodu B od nositelky

                // vyber nejnizsiho bodu a nastaveni spravnych hodnot do prave vytvareneho pruseciku
                if (d1 <= d2 && d1 <= d3)
                {
                    leftVertex = l;
                    rightVertex = v;
                    poi = i1;
                    type = Type.CONVEX;
                    height = dist(v.leftLine, i1);
                }
                else if (d2 <= d1 && d2 <= d3) { leftVertex = v; rightVertex = r; poi = i2; type = Type.CONVEX; height = dist(v.rightLine, i2); }
                else if (d3 <= d1 && d3 <= d2) { poi = p; leftVertex = rightVertex = v; type = Type.NONCONVEX; height = d3; }

                if (poi == new Point(INFINITY, INFINITY)) height = INFINITY;
                if (type == Type.NONCONVEX && invalidIntersection(v, this)) height = INFINITY;
            }

                  public static bool operator < (Intersection t, Intersection i) { return t.height < i.height; }  // pro usporadani v prioritni fronte
                  public static bool operator > (Intersection t, Intersection i) { return t.height >= i.height; }  // pro usporadani v prioritni fronte
                  public static bool operator == (Intersection t,Intersection i) { return t.poi == i.poi; }
                  public static bool operator !=(Intersection t, Intersection i) { return !(t == i); }
            public override int  GetHashCode()
            {
 	             return base.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

        }

                public class Skeleton : List <SkeletonLine>
                {
                    public void push_back (SkeletonLine x)
                                   {
                                     x.ID = Count;     // automaticke cislovani
                                     Add(x);
                                   }

                    internal SkeletonLine back()
                    {
                        return this[this.Count - 1];
                    }
                };

            //    /******************************************************************************************************************************\
            //    **                                             Operace pro vystup                                                             **
            //    \******************************************************************************************************************************/

                public static string S (Vertex v)
                {
                    string os = "";
                  os += v.point.x + " " + v.point.y + " ";
                  if (v.advancingSkeletonLine != null) os += v.advancingSkeletonLine.ID;
                                        else os += -1;
                  os += "\n";
                  return os;
                }

                public static string S (VertexList v)
                {
                    string os = "";
                    os += v.Count +  "\n";
                    foreach (Vertex i in v)
                        os += S(i);
                    return os;
                }

                public static string S (SkeletonLine sl)
                {
                    string os = "";
                    os += sl.lower.vertexID() + " " + sl.higher.vertexID() + " "
                       + sl.lower.leftID() + " " + sl.lower.rightID() + " "
                       + sl.higher.leftID() + " " + sl.higher.rightID() + "\n";
                    return os;
                }

                public static string S (Skeleton s)
                {
                    string os = "";
                    os += s.Count + "\n";
                    foreach(SkeletonLine si in s)
                        os += S(si);
                    return os;
                }

            //    /*****************************************************************************************************************************\
            //    **                                                      Funkce algoritmu                                                     **
            //    \*****************************************************************************************************************************/

                public static bool intersectionFromLeft (Ray l, Vertex v) // pouze pro ladeni
                {
                  if (intersection (l, v.axis) != Point.Infinity) return false;
                  if (v.rightVertex == v) return false;
                  if (intersection (l, v.rightVertex.axis) != Point.Infinity) return true;
                  return intersectionFromLeft (l, v.rightVertex);
                }

                public static bool intersectionFromRight (Ray l, Vertex v) // pouze pro ladeni
                {
                  if (intersection (l, v.axis) != Point.Infinity) return false;
                  if (v.leftVertex == v) return false;
                  if (intersection (l, v.leftVertex.axis) != Point.Infinity) return true;
                  return intersectionFromRight (l, v.leftVertex);
                }


            static Point intersectionOfTypeB(Vertex v, Vertex left, Vertex right)
            {                          // vrati souradnice pruseciku jen pokud je platny (lezi ve vyseci)
                System.Diagnostics.Debug.Assert(v.prevVertex == null || facingTowards(v.leftLine, v.prevVertex.rightLine));
                System.Diagnostics.Debug.Assert(v.nextVertex == null || facingTowards(v.rightLine, v.nextVertex.leftLine));
                System.Diagnostics.Debug.Assert(left.prevVertex == null || facingTowards(left.leftLine, left.prevVertex.rightLine));
                System.Diagnostics.Debug.Assert(left.nextVertex == null || facingTowards(left.rightLine, left.nextVertex.leftLine));
                System.Diagnostics.Debug.Assert(right.prevVertex == null || facingTowards(right.leftLine, right.prevVertex.rightLine));
                System.Diagnostics.Debug.Assert(right.nextVertex == null || facingTowards(right.rightLine, right.nextVertex.leftLine));

                Point pl = (intersection(v.axis, left.rightLine));       // test protne-li bisektor danou hranu
                Point pr = (intersection(v.axis, right.leftLine));
                if (pl == Point.Infinity && pr == Point.Infinity)
                    return new Point(INFINITY, INFINITY);                  // lezi-li hrana "za" bisektorem, inidkuje se neuspech

                Point p = null;
                if (pl != Point.Infinity) p = pl;
                if (pr != Point.Infinity) p = pr;
                System.Diagnostics.Debug.Assert(p != Point.Infinity);
                System.Diagnostics.Debug.Assert(pl == Point.Infinity || pr == Point.Infinity || pl == pr);

                Point poi = coordinatesOfAnyIntersectionOfTypeB(v, left, right);    // zjisteni souradnic potencialniho bodu
                Number al = left.axis.angle - left.rightLine.angle;                  // uhly urcujici vysec, kam musi bod poi padnout
                Number ar = right.axis.angle - right.leftLine.angle;                 //

                Number alp = (new Ray(left.point, poi)).angle - left.rightLine.angle;     // uhly k bodu poi
                Number arp = (new Ray(right.point, poi)).angle - right.leftLine.angle;    //

                al = normalizeAngle(al); ar = normalizeAngle(ar); alp = normalizeAngle(alp); arp = normalizeAngle(arp);
                System.Diagnostics.Debug.Assert(al <= 0.0);
                System.Diagnostics.Debug.Assert(ar >= 0.0 || ar == -M_PI);

                if ((alp > 0.0 || alp < al) && !ANGLE_SIMILAR(alp, 0) && !ANGLE_SIMILAR(alp, al))  // porovnani uhlu
                    return new Point(INFINITY, INFINITY);                                             // a ignorovani bodu lezicich mimo vysec
                if ((arp < 0.0 || arp > ar) && !ANGLE_SIMILAR(arp, 0) && !ANGLE_SIMILAR(arp, ar))  // porovnani uhlu
                    return new Point(INFINITY, INFINITY);                                             // a ignorovani bodu lezicich mimo vysec
                return poi;            // doslo-li se az sem, lezi bod ve vyseci
            }

            static Point coordinatesOfAnyIntersectionOfTypeB(Vertex v, Vertex left, Vertex right)
            {                                         // vrati souradnice pruseciku
                Point p1 = intersectionAnywhere(v.rightLine, right.leftLine);
                Point p2 = intersectionAnywhere(v.leftLine, left.rightLine);
                Point poi;
                if (p1 != new Point(INFINITY, INFINITY) && p2 != new Point(INFINITY, INFINITY))
                {
                    if (pointOnRay(p1, v.rightLine)) return new Point(INFINITY, INFINITY);
                    if (pointOnRay(p2, v.leftLine)) return new Point(INFINITY, INFINITY);
                    poi = intersectionAnywhere(angleAxis(p1, p2, v.point), v.axis);     // puleni uhlu a zjisteni pruseciku s bisektorem od V
                }
                else //if (p1 != Point (INFINITY, INFINITY))                               // specialni pripad rovnobeznosti
                {
                    poi = intersectionAnywhere(left.rightLine, v.axis);
                    poi.x = (poi.x + v.point.x) / 2;                                      // to je pak prusecik na polovicni vzdalenosti
                    poi.y = (poi.y + v.point.y) / 2;                                      // mezi vrcholem V a protilehlou hranou
                }
                return poi;
            }

            public static Number nearestIntersection(Vertex v, out Vertex left, out Vertex right, out Point p) // vrati nejblizsi z pruseciku typu B
            {
                Number minDist = INFINITY;                    // neplatna hodnota
                Vertex minI = null;      // neplatny iterator
                Vertex i;

                Point poi;
                Number d;
                for (int count = 0; count < vl.Count; ++count)     // iterace pres vsechny LAV
                {
                    i = vl[count];
                    if (i.done) continue;                                          // vynechani jiz zpracovanych vrcholu
                    if (i.nextVertex == null || i.prevVertex == null) continue; // osamely vrchol na spicce - neni v LAV
                    if (i == v || i.nextVertex == v) continue;                 // ignorovani hran vychazejicich z V
                    System.Diagnostics.Debug.Assert(i.rightVertex != null);
                    System.Diagnostics.Debug.Assert(i.leftVertex != null);
                    poi = intersectionOfTypeB(v, i, i.nextVertex);      // zjisteni souradnic potencialniho bodu B
                    if (poi == new Point(INFINITY, INFINITY)) continue;                  // vetsinou - nelezi-li bod ve spravne vyseci
                    d = dist(poi, v.point);                                   // zjisteni vzdalenosti od vrcholu
                    if (d < minDist) { minDist = d; minI = i; }                       // a vyber nejblizsiho
                }
                if (minDist == INFINITY)
                {
                    left = null;
                    right = null;
                    p = null;
                    return INFINITY;                               // nenalezen zadny vhodny bod B
                }

                i = minI;
                poi = coordinatesOfAnyIntersectionOfTypeB(v, i, i.nextVertex);

                d = dist(poi, v.leftLine);               // zjisteni vzdalenosti vrcholu V od nositelky (vyska ve strese)
                System.Diagnostics.Debug.Assert(d == dist(poi, v.rightLine));
                System.Diagnostics.Debug.Assert(d == dist(poi, i.rightLine));
                System.Diagnostics.Debug.Assert(d == dist(poi, i.nextVertex.leftLine));

                p = poi;                                         // nastaveni navracenych hodnot
                left = i;
                right = i.nextVertex;

                return d;                                        // vysledkem je vzdalenost od nositelky
            }

            public static bool invalidIntersection(Vertex v, Intersection its)        //
            {
                foreach (Vertex i in vl)
                {
                    if (i.done) continue;
                    if (i.nextVertex == null || i.prevVertex == null) continue; // osamely vrchol na spicce
                    Point poi = intersection(v.axis, i.axis);
                    if (poi == Point.Infinity) continue;
                    if (i == its.leftVertex || i == its.rightVertex) continue;

                    Number dv = dist(poi, v.leftLine);
                    Number dvx = dist(poi, v.rightLine);
                    //        System.Diagnostics.Debug.Assert (SIMILAR (dv, dist (poi, v.rightLine)));
                    System.Diagnostics.Debug.Assert(dv == dvx);
                    if (dv >= its.height) continue;

                    Number di = dist(poi, i.leftLine);
                    System.Diagnostics.Debug.Assert(di == dist(poi, i.rightLine));
                    if (di > dv + MIN_DIFF) continue;
                    //        if (di > is.height) continue;

                    return true;
                }
                return false;
            }

                public static void applyConvexIntersection (Intersection i)        // byl-li nejnizsi prusecik ve fronte konvexniho typu ...
                {
                  Vertex vtx =new Vertex(i.poi, i.leftVertex, i.rightVertex);        // vytvoreni noveho vrcholu na miste pruseciku + spocitani bisektoru
                  System.Diagnostics.Debug.Assert (vtx.point != Point.Infinity);

                  Vertex newNext = i.rightVertex.nextVertex;            // zapojeni vrcholu do LAV
                  Vertex newPrev = i.leftVertex.prevVertex;
                  vtx.prevVertex = newPrev;
                  vtx.nextVertex = newNext;
                  vl.push_back (vtx);
                  Vertex vtxPointer = vl.back();
                  newPrev.nextVertex = vtxPointer;
                  newNext.prevVertex = vtxPointer;
                  i.leftVertex.higher = vtxPointer;
                  i.rightVertex.higher = vtxPointer;

                  i.leftVertex.done = true;                             // oznaceni starych vrcholu za zpracovane
                  i.rightVertex.done = true;

                  Intersection newI = new Intersection(vtxPointer);                          // spocitani nejnizsiho pruseciku pro bisektor vedouci v noveho vrcholu
                  if (newI.height != INFINITY) iq.Enqueue (newI, newI);

                  skeleton.push_back (new SkeletonLine (i.leftVertex, vtxPointer));  // ulozeni dvou car do kostry
                  SkeletonLine lLinePtr = skeleton.back ();
                  skeleton.push_back (new SkeletonLine (i.rightVertex, vtxPointer));
                  SkeletonLine rLinePtr = skeleton.back ();

                  lLinePtr.lower.right = i.leftVertex.leftSkeletonLine;      // a pospojovani okridlenych hran kostry
                  lLinePtr.lower.left = i.leftVertex.rightSkeletonLine;
                  lLinePtr.higher.right = rLinePtr;
                  rLinePtr.lower.right = i.rightVertex.leftSkeletonLine;
                  rLinePtr.lower.left = i.rightVertex.rightSkeletonLine;
                  rLinePtr.higher.left = lLinePtr;

                  if (i.leftVertex.leftSkeletonLine != null) i.leftVertex.leftSkeletonLine.higher.left = lLinePtr;
                  if (i.leftVertex.rightSkeletonLine != null) i.leftVertex.rightSkeletonLine.higher.right = lLinePtr;

                  if (i.rightVertex.leftSkeletonLine != null) i.rightVertex.leftSkeletonLine.higher.left = rLinePtr;
                  if (i.rightVertex.rightSkeletonLine != null) i.rightVertex.rightSkeletonLine.higher.right = rLinePtr;

                  vtxPointer.leftSkeletonLine = lLinePtr;
                  vtxPointer.rightSkeletonLine = rLinePtr;

                  i.leftVertex.advancingSkeletonLine = lLinePtr;
                  i.rightVertex.advancingSkeletonLine = rLinePtr;
                }


public static void applyNonconvexIntersection (Intersection i)  // byl-li nejnizsi prusecik ve fronte nekonvexniho typu ...
{
  System.Diagnostics.Debug.Assert (i.leftVertex == i.rightVertex);          // nekonvexni typ pruseciku ukazuje jen na jeden vrchol, z nehoz byl vytvoren

  Vertex leftPointer, rightPointer;              // dva konce protilehle hrany
  Point p;                                         // souradnice pruseciku
  Number d3 = INFINITY;                                                     // opetovne nalezeni pruseciku a koncovych bodu
  d3 = nearestIntersection (i.leftVertex, out leftPointer, out rightPointer, out p); // protilehle hrany - nutne kvuli moznosti vicekrat
  if (d3 == INFINITY) return;                                               // delene jedne hrany.
                                                   // pri soubehu vice "ramen" kostry muze nastat situace, kdy je puvodni
  if (p != i.poi) return;                          // protilehla hrana jiz zpracovana, pak se novy prusecik nalezne jinde,
                                                   // a puvodni tedy jiz nema vyznam => return.
  Vertex v1 = new Vertex (p, rightPointer, i.rightVertex);    // zalozeni dvou novych vrcholu na miste pruseciku + spocitani bisektoru
  Vertex v2 = new Vertex (p, i.leftVertex, leftPointer);

  System.Diagnostics.Debug.Assert (v1.point != Point.Infinity);
  System.Diagnostics.Debug.Assert (v2.point != Point.Infinity);

  i.leftVertex.done = true;                     // oznaceni puvodniho vrcholu za zpracovany
//  i.rightVertex.done = true;

  Vertex newNext1 = i.rightVertex.nextVertex;  // zapojeni prvniho vrcholu do LAV
  Vertex newPrev1 = leftPointer.highest ();
  v1.prevVertex = newPrev1;
  v1.nextVertex = newNext1;
  vl.push_back (v1);
  Vertex v1Pointer = vl.back ();
  newPrev1.nextVertex = v1Pointer;
  newNext1.prevVertex = v1Pointer;
  i.rightVertex.higher = v1Pointer;

  Vertex newNext2 = rightPointer.highest ();   // zapojeni druheho vrcholu do LAV
  Vertex newPrev2 = i.leftVertex.prevVertex;
  v2.prevVertex = newPrev2;
  v2.nextVertex = newNext2;
  vl.push_back (v2);
  Vertex v2Pointer = vl.back ();
  newPrev2.nextVertex = v2Pointer;
  newNext2.prevVertex = v2Pointer;
  i.leftVertex.higher = v2Pointer;

  skeleton.push_back (new SkeletonLine (i.rightVertex, v1Pointer)); // tvorba kostry - okridlene hrany
  SkeletonLine linePtr = skeleton.back ();
  skeleton.push_back (new SkeletonLine (v1Pointer, v2Pointer));     // nulova delka - pomocna hrana
  SkeletonLine auxLine1Ptr = skeleton.back ();
  skeleton.push_back (new SkeletonLine (v2Pointer, v1Pointer));     // nulova delka - pomocna hrana
  SkeletonLine auxLine2Ptr = skeleton.back ();

  linePtr.lower.right = i.leftVertex.leftSkeletonLine;      // navazani okridlenych hran do kostry
  linePtr.lower.left = i.leftVertex.rightSkeletonLine;

  v1Pointer.rightSkeletonLine = v2Pointer.leftSkeletonLine = linePtr;
  v1Pointer.leftSkeletonLine = auxLine1Ptr;
  v2Pointer.rightSkeletonLine = auxLine2Ptr;

  auxLine1Ptr.lower.right = auxLine2Ptr;
  auxLine2Ptr.lower.left = auxLine1Ptr;

  if (i.leftVertex.leftSkeletonLine != null) i.leftVertex.leftSkeletonLine.higher.left = linePtr;
  if (i.leftVertex.rightSkeletonLine != null) i.leftVertex.rightSkeletonLine.higher.right = linePtr;
  i.leftVertex.advancingSkeletonLine = linePtr;

  if (newNext1 == newPrev1)                                 // specialni pripad, kdy by novy LAV obsahoval pouze 2 vrcholy
     {
       v1Pointer.done = true;                            // oba oznacit jako zpracovane
       newNext1.done = true;
       skeleton.push_back (new SkeletonLine (v1Pointer, newNext1)); // a doplnit kostru o jejich spojnici
       linePtr = skeleton.back ();
       linePtr.lower.right  = v1Pointer.leftSkeletonLine;
       linePtr.lower.left   = v1Pointer.rightSkeletonLine;
       linePtr.higher.right = newNext1.leftSkeletonLine;
       linePtr.higher.left  = newNext1.rightSkeletonLine;

       if (v1Pointer.leftSkeletonLine != null)  v1Pointer.leftSkeletonLine .higher.left  = linePtr;
       if (v1Pointer.rightSkeletonLine != null) v1Pointer.rightSkeletonLine.higher.right = linePtr;
       if (newNext1.leftSkeletonLine != null)   newNext1 .leftSkeletonLine .higher.left  = linePtr;
       if (newNext1.rightSkeletonLine != null)  newNext1 .rightSkeletonLine.higher.right = linePtr;
     }
  else
     {
       Intersection i1 = new Intersection(v1Pointer);                        // tvorba noveho pruseciku pro bisektor vedouci z 1. noveho vrcholu
       if (i1.height != INFINITY) iq.Enqueue (i1, i1);             // a ulozeni do fronty
     }
  if (newNext2 == newPrev2)                                 // specialni pripad, kdy by novy LAV obsahoval pouze 2 vrcholy
     {
       v2Pointer.done = true;                            // oba oznacit jako zpracovane
       newNext2.done = true;
       skeleton.push_back (new SkeletonLine (v2Pointer, newNext2)); // a doplnit kostru o jejich spojnici
       linePtr = skeleton.back ();
       linePtr.lower.right  = v2Pointer.leftSkeletonLine;
       linePtr.lower.left   = v2Pointer.rightSkeletonLine;
       linePtr.higher.right = newNext2.leftSkeletonLine;
       linePtr.higher.left  = newNext2.rightSkeletonLine;

       if (v2Pointer.leftSkeletonLine != null)  v2Pointer.leftSkeletonLine .higher.left  = linePtr;
       if (v2Pointer.rightSkeletonLine != null) v2Pointer.rightSkeletonLine.higher.right = linePtr;
       if (newNext2.leftSkeletonLine != null)   newNext2 .leftSkeletonLine .higher.left  = linePtr;
       if (newNext2.rightSkeletonLine != null)  newNext2 .rightSkeletonLine.higher.right = linePtr;
     }
  else
     {
       Intersection i2 =new Intersection(v2Pointer);                        // tvorba noveho pruseciku pro bisektor vedouci z 2. noveho vrcholu
       if (i2.height != INFINITY) iq.Enqueue (i2, i2);             // a ulozeni do fronty
     }
}

                public static void applyLast3 (Intersection i)                            // zpracovani LAVu obsahujici 3 vrcholy => spicka strechy
                {
                    System.Diagnostics.Debug.Assert(i.leftVertex.nextVertex == i.rightVertex);           // overeni korektnosti propojeni LAVu
                    System.Diagnostics.Debug.Assert(i.rightVertex.prevVertex == i.leftVertex);
                    System.Diagnostics.Debug.Assert(i.leftVertex.prevVertex.prevVertex == i.rightVertex);
                    System.Diagnostics.Debug.Assert(i.rightVertex.nextVertex.nextVertex == i.leftVertex);

                    Vertex v1 = i.leftVertex;
                    Vertex v2 = i.rightVertex;
                    Vertex v3 = i.leftVertex.prevVertex;
                    v1.done = true;                                                  // oznaceni vsech tri vrcholu za zpracovane
                    v2.done = true;
                    v3.done = true;

                    Point is1 = facingTowards(v1.axis, v2.axis) ? new Point(INFINITY, INFINITY) : intersection(v1.axis, v2.axis);
                    Point is2 = facingTowards(v2.axis, v3.axis) ? new Point(INFINITY, INFINITY) : intersection(v2.axis, v3.axis);
                    Point is3 = facingTowards(v3.axis, v1.axis) ? new Point(INFINITY, INFINITY) : intersection(v3.axis, v1.axis);

                  Point its = i.poi;                                                // souradnice spicky
                  System.Diagnostics.Debug.Assert (its == is1 || is1 == Point.Infinity);
                  System.Diagnostics.Debug.Assert (its == is2 || is2 == Point.Infinity);
                  System.Diagnostics.Debug.Assert (its == is3 || is3 == Point.Infinity);

                  Vertex v = new Vertex (its);                                                   // zalozeni noveho vrcholu na spicce, bisektor se nepocita

                  v.done = true;                                                   // i novy vrchol jei nebude dale zpracovavan
                  vl.push_back (v);
                  Vertex vtxPointer = vl.back ();
                  skeleton.push_back (new SkeletonLine (v1, vtxPointer));             // ulozeni tri car do kostry
                  SkeletonLine line1Ptr = skeleton.back ();
                  skeleton.push_back (new SkeletonLine (v2, vtxPointer));
                  SkeletonLine line2Ptr = skeleton.back ();
                  skeleton.push_back (new SkeletonLine (v3, vtxPointer));
                  SkeletonLine line3Ptr = skeleton.back ();

                  line1Ptr.higher.right = line2Ptr;                             // zapojeni okridlenych hran
                  line2Ptr.higher.right = line3Ptr;
                  line3Ptr.higher.right = line1Ptr;

                  line1Ptr.higher.left = line3Ptr;
                  line2Ptr.higher.left = line1Ptr;
                  line3Ptr.higher.left = line2Ptr;

                  line1Ptr.lower.left = v1.rightSkeletonLine;
                  line1Ptr.lower.right = v1.leftSkeletonLine;

                  line2Ptr.lower.left = v2.rightSkeletonLine;
                  line2Ptr.lower.right = v2.leftSkeletonLine;

                  line3Ptr.lower.left = v3.rightSkeletonLine;
                  line3Ptr.lower.right = v3.leftSkeletonLine;

                  if (v1.leftSkeletonLine != null) v1.leftSkeletonLine.higher.left = line1Ptr;
                  if (v1.rightSkeletonLine != null) v1.rightSkeletonLine.higher.right = line1Ptr;

                  if (v2.leftSkeletonLine != null) v2.leftSkeletonLine.higher.left = line2Ptr;
                  if (v2.rightSkeletonLine != null) v2.rightSkeletonLine.higher.right = line2Ptr;

                  if (v3.leftSkeletonLine != null) v3.leftSkeletonLine.higher.left = line3Ptr;
                  if (v3.rightSkeletonLine != null) v3.rightSkeletonLine.higher.right = line3Ptr;

                  v1.advancingSkeletonLine = line1Ptr;
                  v2.advancingSkeletonLine = line2Ptr;
                  v3.advancingSkeletonLine = line3Ptr;
                }


                public static Skeleton makeSkeleton (ContourVector contours)          // hlavni smycka vytvarejici kostru
                {
                  while (iq.Count > 0) iq.Dequeue ();                               // vymazani globalnich kontajneru
                  vl.Clear();                          // ... prioritni fronta pruseciku, SLAV a vysledna kostra
                  skeleton.Clear();

                  for (int ci = 0; ci < contours.Count; ci++)              // pro kazdou samostatnou konturu, tj. pro obrys i diry
                      {
                        Contour points = contours [ci];                  // zpristupneni jednoho polygonu
                        while (points.Count > 0 && points[0] == points[points.Count - 1])
                               points.RemoveAt(points.Count - 1);

                        // vyhazeni duplicitnich bodu - dva sousedni body nesmeji mit stejne souradnice, jinak by nesel urcit bisektor
                        if (points.Count == 0) break;                    // prazdny seznam bodu.prazdny seznam hran
                        int first = 0;

                      int next = 0;
                        while ((++next) < points.Count)
                              {
                                  if (points[first] == points[next]) 
                                    points.RemoveAt (next);
                                else 
                                      first = next;
                                  next = first;
                              }

                        // vytvoreni seznamu vrcholu
                        int s = points.Count;
                        for (int f = 0; f < s; f++)
                            {
                              vl.push_back (new Vertex (points [f], points [(s+f-1)%s], points [(s+f+1)%s]));  // zaroven spocita bisektory
                            }
                     }

                  if (vl.Count < 3) return skeleton;  // kostra alespon pro trojuhelnik, jinak se vrati prazdna kostra

                  // provazani vrcholu - tvorba jednotlivych LAVu ve SLAV
                  Vertex i;
                  int vn = 0, cn = 0;
                  Vertex contourBegin = null;
                  for (int ii = 0; ii <vl.Count; ++ii)
                      {
                          i = vl[ii];
                        i.prevVertex = vl.prev (ii);         // ukazatele obsahuji adresy vrcholu jiz ulozenych do seznamu
                        i.nextVertex = vl.next (ii);         // => lze je navazat az ted, kdy se jejich adresa uz nebude menit
                        i.leftVertex = i;
                        i.rightVertex = i;
                        if (vn == 0) contourBegin = i;
                        if (vn == contours [cn].Count - 1)
                           {
                             i.nextVertex = contourBegin;
                             (contourBegin).prevVertex = i;
                             vn = 0;
                             cn ++;
                           }
                        else vn ++;
                      }

                  foreach (Skel.Vertex vi in Skel.vl)
                  {
                      if (!vi.done)
                          vPoints.Add(vi.point);
                  }

                  // vytvoreni fronty pruseciku - pro kazdy vrchol jeden prusecik
                  for (int ii = 0; ii < vl.Count; ++ii)
                      {
                          i = vl[ii];
                        if (!i.done)
                           {
                             Intersection its = new Intersection(i);
                             if (its.height != INFINITY) iq.Enqueue (its, its);  // ulozeni do prioritni fronty
                           }
                      }

                  while (iq.Count > 0)                              // hlavni cyklus - dokud je nejaky prusecik ve fronte
                        {
                          Intersection its = iq.Peek ().Value;             // vyzvednuti nejnizsiho pruseciku
                          iq.Dequeue();

                          if (its.leftVertex.done && its.rightVertex.done) continue;   // pokud byly vrcholy ukazovane prusecikem zpracovany
                                                                                         // v predchozich iteracich => vzit dalsi prusecik
                          if (its.leftVertex.done || its.rightVertex.done)             // velmi ridka situace, kdy byl zpracovan pouze jeden
                             {                                                           // z vrcholu
                                 Intersection leftI = new Intersection(its.leftVertex);
                                 Intersection rightI = new Intersection(its.rightVertex);
                                 if (!its.leftVertex.done) iq.Enqueue(leftI, leftI);    // pro ten nezpracovany je spocitan novy
                                 if (!its.rightVertex.done) iq.Enqueue(rightI, rightI);  // prusecik a je ulozen do fronty
                               continue;                                                 // a aktualni prusecik jiz neni zpracovan
                             }

                          System.Diagnostics.Debug.Assert (its.leftVertex.prevVertex != its.rightVertex);
                          System.Diagnostics.Debug.Assert (its.rightVertex.nextVertex != its.leftVertex);
                          if (its.type == Intersection.Type.CONVEX)
                              if (its.leftVertex.prevVertex.prevVertex == its.rightVertex ||
                                  its.rightVertex.nextVertex.nextVertex == its.leftVertex)
                                  applyLast3 (its);                                        // zpracovani pripadu, kdy v LAVu zbyvaji jen 3 vrcholy
                             else applyConvexIntersection (its);                           // zpracovani EDGE EVENT
                          if (its.type == Intersection.Type.NONCONVEX)
                              applyNonconvexIntersection (its);                            // nebo zpracovani SPLIT EVENT
                        }
                  return skeleton;                                                       // vraci se odkaz na vytvorenou kostru
                }

                public static Skeleton makeSkeleton (Contour points)  // zkratka pokud je tvorena kostra pro jedinou konturu bez der
                {
                    ContourVector vv = new ContourVector();
                  vv.Add(points);
                  return makeSkeleton (vv);
                }

        /*****************************************************************************************************************************\
        **                                                 To test the implementation ...                                            **
        \*****************************************************************************************************************************/


                public static int buildContourFromStream (StreamReader stream, Contour result) // nacteni jedne kontury ze streamu, vraci pocet bodu
                {
                    result.Clear();
                  int count;
                  if (stream.EndOfStream) return EOF;
                  count = int.Parse(stream.ReadLine());                                             // nejdriv pocet vrcholu
                  if (stream.EndOfStream) return EOF;
                  for (int f = 0; f < count; f++)
                      {
                        Number x, y;
                        string[] temp = stream.ReadLine().Trim().Split(new char[] { ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
                        x = double.Parse(temp[0]);
                        y = double.Parse(temp[1]);
                                     result.Add (new Point (x, y));
                      }
                    if (!stream.EndOfStream)
                  stream.ReadLine();
                  return count;
                }

            public static int buildContourVectorFromStream (StreamReader stream, ContourVector result) // nacteni seznamu kontur ze streamu - i diry
                {
                  int total = 0;
                  result.Clear();
                 int count = 0;
                 if (stream.EndOfStream) return EOF;
                 count = int.Parse(stream.ReadLine());
                       if (stream.EndOfStream) return EOF;
                  for (int f = 0; f < count; f++)
                        {
                          result.Add (new Contour ());                               // pak jednotlive kontury
                          total += buildContourFromStream (stream, result [f]);
                        }
                  return total;
                }

                public static void outFace (Vertex v, TextWriter stream)
                {
                  SkeletonLine l = v.advancingSkeletonLine;
                  if (l == null) throw new Exception("Integrity error !!!\n");
                  SkeletonLine next = l.higher.right;
                  Point last = l.lower.vertex.point;

                  stream.Write( last.x + " " + last.y + " ");
                  do {
                       if (last != l.lower.vertex.point)
                          {
                            last = l.lower.vertex.point;
                            stream.Write( last.x + " " + last.y + " ");
                          }
                       else if (last != l.higher.vertex.point)
                          {
                            last = l.higher.vertex.point;
                            stream.Write( last.x + " " + last.y + " ");
                          }
                       if (next == null) break;
                            if (next.lower.left == l) { l = next; next = next.higher.right; }
                       else if (next.higher.left == l) { l = next; next = next.lower.right; }
                       else System.Diagnostics.Debug.Assert (false);
                    } while (l != null);
                  stream.Write('\n');
                }

                public static void drawFace(Vertex v, System.Drawing.Bitmap bitmap)
                {
                    SkeletonLine l = v.advancingSkeletonLine;
                    if (l == null) throw new Exception("Integrity error !!!\n");
                    SkeletonLine next = l.higher.right;
                    Point last = l.lower.vertex.point;

                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                    List<System.Drawing.PointF> points = new List<System.Drawing.PointF>();

                    points.Add(new System.Drawing.PointF((float)last.x, (float)last.y));
                    do
                    {
                        if (last != l.lower.vertex.point)
                        {
                            last = l.lower.vertex.point;
                            points.Add(new System.Drawing.PointF((float)last.x, (float)last.y));
                        }
                        else if (last != l.higher.vertex.point)
                        {
                            last = l.higher.vertex.point;
                            points.Add(new System.Drawing.PointF((float)last.x, (float)last.y));
                        }
                        if (next == null) break;
                        if (next.lower.left == l) { l = next; next = next.higher.right; }
                        else if (next.higher.left == l) { l = next; next = next.lower.right; }
                        else System.Diagnostics.Debug.Assert(false);
                    } while (l != null);
                    g.DrawPolygon(System.Drawing.Pens.Black, points.ToArray());
                    g.Dispose();
                }

                public static void drawPath(System.Drawing.Bitmap bm)
                {
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bm);

                    Polygon poly = getPolygon();

                    g.DrawPath(System.Drawing.Pens.Black, poly.ToGraphicsPath());
 
                    for (int i = 0; i < pathLines.Count; i++)
                    {
                        //P1
                        //g.DrawEllipse(System.Drawing.Pens.Green, pathLines[i].P1.X - 2.5f, pathLines[i].P1.Y - 2.5f, 5.0f, 5.0f);
                        //P2
                        //g.DrawEllipse(System.Drawing.Pens.Gold, pathLines[i].P2.X-2.5f, pathLines[i].P2.Y-2.5f, 5.0f, 5.0f);

                        //P1
                        //g.DrawEllipse(System.Drawing.Pens.Green, pathLines[i + 1].P1.X - 2.5f, pathLines[i + 1].P1.Y - 2.5f, 5.0f, 5.0f);
                        //P2
                        //g.DrawEllipse(System.Drawing.Pens.Gold, pathLines[i + 1].P2.X- 2.5f, pathLines[i + 1].P2.Y- 2.5f, 5.0f, 5.0f);

                        g.DrawLine(System.Drawing.Pens.Blue,  pathLines[i].P1.X, pathLines[i].P1.Y, pathLines[i].P2.X, pathLines[i].P2.Y);
                        //g.DrawLine(System.Drawing.Pens.Red, pathLines[i + 1].P1.X, pathLines[i + 1].P1.Y, pathLines[i + 1].P2.X, pathLines[i + 1].P2.Y);
                        //g.DrawLine(System.Drawing.Pens.Black, SSLines[i/2].P1.X, SSLines[i/2].P1.Y, SSLines[i/2].P2.X, SSLines[i/2].P2.Y);
                    }

                    g.Dispose();
                }


                public static Polygon getPolygon()
                {
                    List<Vec2d> vertices = new List<Vec2d>();
                    for (int i = 0; i < Skel.vPoints.Count; i++)
                    {
                        vertices.Add(new Vec2d(Skel.vPoints[i].x, Skel.vPoints[i].y));
                    }
                    return new Polygon(vertices);
                }

                public static int[] generatePath(float dist)
                {
                    Line2 xAxis = new Line2(new Vec2(0.0f, 0.0f), new Vec2(1.0f, 0.0f));

                    for (int l = 0; l < SSLines.Count; l++)
                    {
                        for (int ll = l + 1; ll < SSLines.Count; ll++)
                        {
                            if (SSLines[l].P1.X == SSLines[ll].P1.X && SSLines[l].P1.Y == SSLines[ll].P1.Y &&
                                SSLines[l].P2.X == SSLines[ll].P2.X && SSLines[l].P2.Y == SSLines[ll].P2.Y)
                            {
                                SSLines.Remove(SSLines[ll]);
                            }
                        }
                    }

                    int[] dimensions = new int[2];

                    dimensions[0] = 0;
                    dimensions[1] = 0;

                    List<Vec2> vertices = new List<Vec2>();
                    List<int>[] connections;

                    for (int l = 0; l < SSLines.Count; l++)
                    {
                        bool found = false;
                        for (int v = 0; v < vertices.Count; v++)
                        {
                            if (vertices[v].X == SSLines[l].P1.X && vertices[v].Y == SSLines[l].P1.Y)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            vertices.Add(SSLines[l].P1);

                        found = false;
                        for (int v = 0; v < vertices.Count; v++)
                        {
                            if (vertices[v].X == SSLines[l].P2.X && vertices[v].Y == SSLines[l].P2.Y)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            vertices.Add(SSLines[l].P2);
                    }

                    connections = new List<int>[vertices.Count];
                    for (int c = 0; c < vertices.Count; c++)
                    {
                        connections[c] = new List<int>();
                    }

                    for (int l = 0; l < SSLines.Count; l++)
                    {
                        for (int v = 0; v < vertices.Count; v++)
                        {
                            if (SSLines[l].P1.X == vertices[v].X && SSLines[l].P1.Y == vertices[v].Y)
                                connections[v].Add(l);

                            if (SSLines[l].P2.X == vertices[v].X && SSLines[l].P2.Y == vertices[v].Y)
                                connections[v].Add(l);
                        }
                    }

                    Polygon poly = getPolygon();

                    for (int l = 0; l < SSLines.Count; l++)
                    {
                        List<Line2> lines;

                        lines = SSLines[l].pathLines(dist);

                        pathLines.Add(lines[0]);
                        pathLines.Add(lines[1]);
                    }

                    for (int c = 0; c < connections.Count(); c++)
                    {
                        Vec2 intersection;
                        float ang;
                        bool inline;

                        if (connections[c].Count == 2)
                        {
                            if (SSLines[connections[c][0]].P1.X == SSLines[connections[c][1]].P2.X &&
                                SSLines[connections[c][0]].P1.Y == SSLines[connections[c][1]].P2.Y)
                            {
                                
                                ang = Math.Abs(SSLines[connections[c][0]].Angle(SSLines[connections[c][1]]));
                                if (ang > 0.0f)
                                {
                                    intersection = pathLines[connections[c][0] * 2].lineIntersection(pathLines[connections[c][1] * 2], out inline);
                                    if (intersection != null)
                                    {
                                        pathLines[connections[c][0] * 2].P1.X = intersection.X;
                                        pathLines[connections[c][0] * 2].P1.Y = intersection.Y;
                                        pathLines[connections[c][1] * 2].P2.X = intersection.X;
                                        pathLines[connections[c][1] * 2].P2.Y = intersection.Y;
                                    }

                                    intersection = pathLines[connections[c][0] * 2 + 1].lineIntersection(pathLines[connections[c][1] * 2 + 1], out inline);
                                    if (intersection != null)
                                    {
                                        pathLines[connections[c][0] * 2 + 1].P1.X = intersection.X;
                                        pathLines[connections[c][0] * 2 + 1].P1.Y = intersection.Y;
                                        pathLines[connections[c][1] * 2 + 1].P2.X = intersection.X;
                                        pathLines[connections[c][1] * 2 + 1].P2.Y = intersection.Y;
                                    }
                                }
                            }
                            else if (SSLines[connections[c][1]].P1.X == SSLines[connections[c][0]].P2.X &&
                                     SSLines[connections[c][1]].P1.Y == SSLines[connections[c][0]].P2.Y)
                            {
                                ang = Math.Abs(SSLines[connections[c][0]].Angle(SSLines[connections[c][1]]));
                                if (ang > 0.0f)
                                {
                                    intersection = pathLines[connections[c][0] * 2].lineIntersection(pathLines[connections[c][1] * 2], out inline);
                                    if (intersection != null)
                                    {
                                        pathLines[connections[c][1] * 2].P1.X = intersection.X;
                                        pathLines[connections[c][1] * 2].P1.Y = intersection.Y;
                                        pathLines[connections[c][0] * 2].P2.X = intersection.X;
                                        pathLines[connections[c][0] * 2].P2.Y = intersection.Y;
                                    }

                                    intersection = pathLines[connections[c][0] * 2 + 1].lineIntersection(pathLines[connections[c][1] * 2 + 1], out inline);
                                    if (intersection != null)
                                    {
                                        pathLines[connections[c][1] * 2 + 1].P1.X = intersection.X;
                                        pathLines[connections[c][1] * 2 + 1].P1.Y = intersection.Y;
                                        pathLines[connections[c][0] * 2 + 1].P2.X = intersection.X;
                                        pathLines[connections[c][0] * 2 + 1].P2.Y = intersection.Y;
                                    }
                                }
                            }
                            else if (SSLines[connections[c][0]].P1.X == SSLines[connections[c][1]].P1.X &&
                                     SSLines[connections[c][0]].P1.Y == SSLines[connections[c][1]].P1.Y)
                            {
                                ang = Math.Abs(SSLines[connections[c][0]].Angle(SSLines[connections[c][1]]));
                                if (ang > 0.0f)
                                {
                                    //intersection = pathLines[connections[c][0] * 2].lineIntersection(pathLines[connections[c][1] * 2], out inline);
                                    //if (intersection != null)
                                    //{
                                    //    pathLines[connections[c][1] * 2].P1.X = intersection.X;
                                    //    pathLines[connections[c][1] * 2].P1.Y = intersection.Y;
                                    //    pathLines[connections[c][0] * 2].P1.X = intersection.X;
                                    //    pathLines[connections[c][0] * 2].P1.Y = intersection.Y;
                                    //}

                                    //intersection = pathLines[connections[c][0] * 2 + 1].lineIntersection(pathLines[connections[c][1] * 2 + 1], out inline);
                                    //if (intersection != null)
                                    //{
                                    //    pathLines[connections[c][1] * 2 + 1].P1.X = intersection.X;
                                    //    pathLines[connections[c][1] * 2 + 1].P1.Y = intersection.Y;
                                    //    pathLines[connections[c][0] * 2 + 1].P1.X = intersection.X;
                                    //    pathLines[connections[c][0] * 2 + 1].P1.Y = intersection.Y;
                                    //}
                                }
                            }
                            else if (SSLines[connections[c][1]].P2.X == SSLines[connections[c][0]].P2.X &&
                                     SSLines[connections[c][1]].P2.Y == SSLines[connections[c][0]].P2.Y)
                            {
                                ang = Math.Abs(SSLines[connections[c][0]].Angle(SSLines[connections[c][1]]));
                                if (ang > 0.0f)
                                {
                                    //intersection = pathLines[connections[c][0] * 2].lineIntersection(pathLines[connections[c][1] * 2], out inline);
                                    //if (intersection != null)
                                    //{
                                    //    pathLines[connections[c][1] * 2].P2.X = intersection.X;
                                    //    pathLines[connections[c][1] * 2].P2.Y = intersection.Y;
                                    //    pathLines[connections[c][0] * 2].P2.X = intersection.X;
                                    //    pathLines[connections[c][0] * 2].P2.Y = intersection.Y;
                                    //}

                                    //intersection = pathLines[connections[c][0] * 2 + 1].lineIntersection(pathLines[connections[c][1] * 2 + 1], out inline);
                                    //if (intersection != null)
                                    //{
                                    //    pathLines[connections[c][1] * 2 + 1].P2.X = intersection.X;
                                    //    pathLines[connections[c][1] * 2 + 1].P2.Y = intersection.Y;
                                    //    pathLines[connections[c][0] * 2 + 1].P2.X = intersection.X;
                                    //    pathLines[connections[c][0] * 2 + 1].P2.Y = intersection.Y;
                                    //}
                                }
                            }
                        }
                    }
                    //-----------------------------------------------------
                    List<Line2> contourLines = poly.ToLines();

                    for (int cl = 0; cl < contourLines.Count; cl++)
                    {
                        if (dimensions[0] < (int)contourLines[cl].P1.X)
                            dimensions[0] = (int)contourLines[cl].P1.X + 20;
                        
                        if (dimensions[0] < (int)contourLines[cl].P2.X)
                            dimensions[0] = (int)contourLines[cl].P2.X + 20;

                        if (dimensions[1] < (int)contourLines[cl].P1.Y)
                            dimensions[1] = (int)contourLines[cl].P1.Y + 20;

                        if (dimensions[1] < (int)contourLines[cl].P2.Y)
                            dimensions[1] = (int)contourLines[cl].P2.Y + 20;

                    }

                    for (int l = 0; l < pathLines.Count; l++)
                    {
                        List<Vec2> intersections = new List<Vec2>();
                        bool[] inside = new bool[2];

                        for (int c = 0; c < contourLines.Count; c++)
                        {
                            Vec2 intersection;
                            bool occurs = false;

                            intersection = pathLines[l].IntersectionOnLine(contourLines[c], out occurs);
                            if (occurs)
                                intersections.Add(intersection);
                        }

                        inside[0] = poly.Contains(pathLines[l].P1.X, pathLines[l].P1.Y);
                        inside[1] = poly.Contains(pathLines[l].P2.X, pathLines[l].P2.Y);

                        Line2 l1, l2;

                        if (intersections.Count == 1)
                        {
                            if (inside[0])
                            {
                                l1 = new Line2(pathLines[l].P1, intersections[0]);
                                pathLines[l] = l1;
                            }
                            else
                            {
                                l1 = new Line2(pathLines[l].P2, intersections[0]);
                                pathLines[l] = l1;
                            }
                        }
                        else if(intersections.Count == 2)
                        {
                            if (inside[0] && inside[1])
                            {
                                Vec2 p1, p2;
                                p1 = pathLines[l].P1;
                                p2 = pathLines[l].P2;
                                if (p1.DistanceToPoint(intersections[0]) < p2.DistanceToPoint(intersections[0]))
                                {
                                    l1 = new Line2(p1, intersections[0]);
                                    l2 = new Line2(p2, intersections[1]);
                                }
                                else
                                {
                                    l1 = new Line2(p1, intersections[1]);
                                    l2 = new Line2(p2, intersections[0]);
                                }
                                pathLines[l] = l1;
                                pathLines.Insert(0, l2);
                                l++;
                            }
                            else
                            {
                                l1 = new Line2(intersections[0], intersections[1]);
                                pathLines[l] = l1;
                            }
                        }
                        else if (!inside[0] && !inside[1] && intersections.Count == 0)
                        {
                            pathLines.Remove(pathLines[l]);
                            l--;
                        }
                    }
                    //-----------------------------------------------------
                    return dimensions;
                }

                public static void addPath(float dist, Vertex v)
                {
                    SkeletonLine l = v.advancingSkeletonLine;
                    if (l == null) 
                        throw new Exception("Integrity error !!!\n");
                        
                    SkeletonLine next = l.higher.right;
                    Point last = l.lower.vertex.point;
                    do
                    {
                        if (last != l.lower.vertex.point)
                        {
                            last = l.lower.vertex.point;
                        }
                        else if (last != l.higher.vertex.point)
                        {
                            last = l.higher.vertex.point;
                        }
                        if (next == null) break;
                        if (next.lower.left == l) { l = next; next = next.higher.right; }
                        else if (next.higher.left == l) { l = next; next = next.lower.right; }
                        else System.Diagnostics.Debug.Assert(false);

                        if (!vPoints.Contains(l.lower.vertex.point) && !vPoints.Contains(l.higher.vertex.point))
                        {
                            Line2 line = new Line2(new Vec2((float)l.lower.vertex.point.x, (float)l.lower.vertex.point.y),
                                                   new Vec2((float)l.higher.vertex.point.x, (float)l.higher.vertex.point.y));

                            if(line.Length() > 0.0f)
                                SSLines.Add(line);
                        }
                    } while (l != null);
                }
    }
}
