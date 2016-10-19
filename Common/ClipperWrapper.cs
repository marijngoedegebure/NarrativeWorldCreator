using System;
using System.Collections.Generic;
using ClipperLib;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

namespace Common {
    public static class ClipperWrapper {
        /// <summary>
        /// Precision scale of clipper. Since the library uses integers, all floating point values are multiplied by this precision scale.
        /// For instance, if the unit of the coordinates is meters, a precision scale of 1000 gives mm accuracy.
        /// </summary>
        private const double PrecisionScale = 1000.0;
        private static readonly Clipper ClipperEngine = new Clipper();

        public static Polygon GetDifference(Polygon subject, Polygon clip) {
            return ClipPolygon(subject, clip, ClipType.ctDifference);
        }

        public static Polygon GetIntersection(Polygon subject, Polygon clip) {
            return ClipPolygon(subject, clip, ClipType.ctIntersection);
        }

        public static Polygon GetUnion(Polygon subject, Polygon clip) {
            return ClipPolygon(subject, clip, ClipType.ctUnion);
        }

        public static Polygon GetXor(Polygon subject, Polygon clip) {
            return ClipPolygon(subject, clip, ClipType.ctXor);
        }

        public static Polygon GetOffsetPolygon(Polygon subject, double offset)
        {
            Polygon result;
            Paths clprResult = new Paths();
            // Convert to library structure
            Paths clprSubject = PolygonToClprPaths(subject);
            // Execute clip action
            lock (ClipperEngine)
            {
                ClipperOffset clipperOffset = new ClipperOffset();
                clipperOffset.AddPaths(clprSubject, JoinType.jtSquare, EndType.etClosedPolygon);
                clipperOffset.Execute(ref clprResult, offset * PrecisionScale);
                //clprResult = Clipper.OffsetPolygons(clprSubject, offset * PRECISION_SCALE);
                result = ClprPathsToPolygon(clprResult);
            }
            // Convert back to common polygon
            return result;
        }

        public static List<Vec2d> GetOffsetPolygon(List<Vec2d> subject, double offset)
        {
            List<Vec2d> result;
            Paths clprResult = new Paths();
            // Convert to library structure
            Paths clprSubject = PolygonToClprPaths(subject);
            // Execute clip action
            lock (ClipperEngine)
            {
                ClipperOffset clipperOffset = new ClipperOffset();
                clipperOffset.AddPaths(clprSubject, JoinType.jtSquare, EndType.etClosedPolygon);
                clipperOffset.Execute(ref clprResult, offset * PrecisionScale);
                //clprResult = Clipper.OffsetPolygons(clprSubject, offset * PRECISION_SCALE);
                result = ClprPathsToSimplePolygon(clprResult);
            }
            // Convert back to common polygon
            return result;
        }

        private static Polygon ClipPolygon(Polygon subject, Polygon clip, ClipType operation) {
            Paths clprResult = new Paths();
            // Convert to library structure
            Paths clprSubject = PolygonToClprPaths(subject);
            Paths clprClip = PolygonToClprPaths(clip);
            // Execute clip action
            lock (ClipperEngine) {
                ClipperEngine.Clear();
                ClipperEngine.AddPaths(clprSubject, PolyType.ptSubject, true);
                ClipperEngine.AddPaths(clprClip, PolyType.ptClip, true);
                ClipperEngine.Execute(operation, clprResult);
            }
            // Convert back to common polygon
            return ClprPathsToPolygon(clprResult);
        }

        private static Polygon ClprPathsToPolygon(Paths polygons) {
            Polygon result = null;
            if (polygons.Count <= 0)
                return null;

            result = new Polygon(polygons.Count);
            for (int j = 0; j < polygons.Count; ++j) {
                bool hole = !Clipper.Orientation(polygons[j]);
                List<Vec2d> contour = new List<Vec2d>(polygons[j].Count);
                for (int i = 0; i < polygons[j].Count; ++i) {
                    IntPoint p = polygons[j][i];
                    double x = p.X / PrecisionScale;
                    double y = p.Y / PrecisionScale;
                    contour.Add(new Vec2d(x, y));
                }
                result.SetContourAndHole(j, contour, hole);
            }
            return result;
        }

        private static List<Vec2d> ClprPathsToSimplePolygon(Paths polygons)
        {
            List<Vec2d> result = null;
            if (polygons.Count != 1) 
                return null;

            result = new List<Vec2d>(polygons[0].Count);
            for (int i = 0; i < polygons.Count; ++i)
            {
                IntPoint p = polygons[0][i];
                double x = p.X / PrecisionScale;
                double y = p.Y / PrecisionScale;
                result.Add(new Vec2d(x, y));
            }
            return result;
        }

        private static Paths PolygonToClprPaths(Polygon polygon) {
            Paths result = new List<List<IntPoint>>(polygon.NumContours);
            for (int j = 0; j < polygon.NumContours; ++j) {
                result.Add(new List<IntPoint>(polygon[j].Count));
                for (int i = 0; i < polygon[j].Count; ++i) 
                {
                    Vec2d v = polygon[j][i];
                    IntPoint p;
                    p.X = (Int32)(v.X * PrecisionScale);
                    p.Y = (Int32)(v.Y * PrecisionScale);
                    result[j].Add(p);
                }
                if (Clipper.Orientation(result[j]) == polygon.IsHole(j))
                {
                    result[j].Reverse();
                }
            }
            return result;
        }

        private static Paths PolygonToClprPaths(List<Vec2d> polygon)
        {
            Paths result = new List<List<IntPoint>>(1) {new List<IntPoint>(polygon.Count)};
            for (int i = 0; i < polygon.Count; ++i)
            {
                Vec2d v = polygon[i];
                IntPoint p;
                p.X = (Int32)(v.X * PrecisionScale);
                p.Y = (Int32)(v.Y * PrecisionScale);
                result[0].Add(p);
            }
            return result;
        }
    }
}
