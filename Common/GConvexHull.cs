// Finding a convex hull in the plane
// This program requires .Net version 2.0.
// Peter Sestoft (sestoft@itu.dk) * Java 2000-10-07, GC# 2001-10-27


using System;
using System.Collections.Generic;
using System.Text;


// ------------------------------------------------------------

// Find the convex hull of a point set in the plane

// An implementation of Graham's (1972) point elimination algorithm,
// as modified by Andrew (1979) to find lower and upper hull separately.

// This implementation correctly handles duplicate points, and
// multiple points with the same x-coordinate.

// 1. Sort the points lexicographically by increasing (x,y), thus 
//    finding also a leftmost point L and a rightmost point R.
// 2. Partition the point set into two lists, upper and lower, according as 
//    point is above or below the segment LR.  The upper list begins with 
//    L and ends with R; the lower list begins with R and ends with L.
// 3. Traverse the point lists clockwise, eliminating all but the extreme
//    points (thus eliminating also duplicate points).
// 4. Eliminate L from lower and R from upper, if necessary.
// 5. Join the point lists (in clockwise order) in an array.
namespace Common {
    public class Convexhull {
        public static Vec2[] GetConvexHull(Vec2[] pts) {
            return Vec2d.ToVec2Array(GetConvexHull(Vec2d.FromVec2Array(pts)));
        }

        public static Vec2d[] GetConvexHull(Vec2d[] pts) {
            // Sort points lexicographically by increasing (x, y)
            int N = pts.Length;
            quicksort(pts);
            Vec2d left = pts[0], right = pts[N - 1];
            // Partition into lower hull and upper hull
            CDLL<Vec2d> lower = new CDLL<Vec2d>(left), upper = new CDLL<Vec2d>(left);
            for (int i = 0; i < N; i++) {
                double det = area2(left, right, pts[i]);
                if (det > 0)
                    upper = upper.Append(new CDLL<Vec2d>(pts[i]));
                else if (det < 0)
                    lower = lower.Prepend(new CDLL<Vec2d>(pts[i]));
            }
            lower = lower.Prepend(new CDLL<Vec2d>(right));
            upper = upper.Append(new CDLL<Vec2d>(right)).Next;
            // Eliminate points not on the hull
            eliminate(lower);
            eliminate(upper);
            // Eliminate duplicate endpoints
            if (lower.Prev.val.Equals(upper.val))
                lower.Prev.Delete();
            if (upper.Prev.val.Equals(lower.val))
                upper.Prev.Delete();
            // Join the lower and upper hull
            Vec2d[] res = new Vec2d[lower.Size() + upper.Size()];
            lower.CopyInto(res, 0);
            upper.CopyInto(res, lower.Size());
            return res;
        }

        // Graham's scan
        private static void eliminate(CDLL<Vec2d> start) {
            CDLL<Vec2d> v = start, w = start.Prev;
            bool fwd = false;
            while (v.Next != start || !fwd) {
                if (v.Next == w)
                    fwd = true;
                if (area2(v.val, v.Next.val, v.Next.Next.val) < 0) // right turn
                    v = v.Next;
                else {                                       // left turn or straight
                    v.Next.Delete();
                    v = v.Prev;
                }
            }
        }

        private static bool lessThan(Vec2d p1, Vec2d p2) {
            return p1.X < p2.X || p1.X == p2.X && p1.Y < p2.Y;
        }

        // Twice the signed area of the triangle (p0, p1, p2)
        private static double area2(Vec2d p0, Vec2d p1, Vec2d p2) {
            return p0.X * (p1.Y - p2.Y) + p1.X * (p2.Y - p0.Y) + p2.X * (p0.Y - p1.Y);
        }

        private static void swap<T>(T[] arr, int s, int t) {
            T tmp = arr[s]; arr[s] = arr[t]; arr[t] = tmp;
        }

        // Typed OO-style quicksort a la Hoare/Wirth
        private static void qsort(Vec2d[] arr, int a, int b) {
            // sort arr[a..b]
            if (a < b) {
                int i = a, j = b;
                Vec2d x = arr[(i + j) / 2];
                do {
                    while (lessThan(arr[i], x)) i++;
                    while (lessThan(x, arr[j])) j--;
                    if (i <= j) {
                        swap<Vec2d>(arr, i, j);
                        i++; j--;
                    }
                } while (i <= j);
                qsort(arr, a, j);
                qsort(arr, i, b);
            }
        }

        private static void quicksort(Vec2d[] arr) {
            qsort(arr, 0, arr.Length - 1);
        }

        // Circular doubly linked lists of T
        private class CDLL<T> {
            private CDLL<T> prev, next;     // not null, except in deleted elements
            public T val;

            // A new CDLL node is a one-element circular list
            public CDLL(T val) {
                this.val = val; next = prev = this;
            }

            public CDLL<T> Prev {
                get { return prev; }
            }

            public CDLL<T> Next {
                get { return next; }
            }

            // Delete: adjust the remaining elements, make this one point nowhere
            public void Delete() {
                next.prev = prev; prev.next = next;
                next = prev = null;
            }

            public CDLL<T> Prepend(CDLL<T> elt) {
                elt.next = this; elt.prev = prev; prev.next = elt; prev = elt;
                return elt;
            }

            public CDLL<T> Append(CDLL<T> elt) {
                elt.prev = this; elt.next = next; next.prev = elt; next = elt;
                return elt;
            }

            public int Size() {
                int count = 0;
                CDLL<T> node = this;
                do {
                    count++;
                    node = node.next;
                } while (node != this);
                return count;
            }

            public void PrintFwd() {
                CDLL<T> node = this;
                do {
                    Console.WriteLine(node.val);
                    node = node.next;
                } while (node != this);
                Console.WriteLine();
            }

            public void CopyInto(T[] vals, int i) {
                CDLL<T> node = this;
                do {
                    vals[i++] = node.val;	// still, implicit checkcasts at runtime 
                    node = node.next;
                } while (node != this);
            }
        }

    }
}
