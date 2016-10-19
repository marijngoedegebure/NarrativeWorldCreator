using System;
using System.Collections.Generic;

using System.Text;

using Common;
using Common.Geometry;
using Common.MathParser;

namespace Common.Shapes.Transformations
{
    static class Transformations
    {
        class LineLengthComparer : IComparer<Line2>
        {
            #region IComparer<Line2> Members

            public int Compare(Line2 x, Line2 y)
            {
                return x.Length().CompareTo(y.Length());
            }

            #endregion
        }

        #region SplitLines
        internal static List<object> SplitLines(TransformationNode.ApplicationData data, object input, CustomTermEvaluater termEvaluater, List<string> parameters)
        {
            double distanceBetweenSplits = (double)Term.FromString(parameters[0]).GetValue(termEvaluater);
            bool allEqual = (bool)Term.FromString(parameters[1]).GetValue(termEvaluater);

            List<object> ret = new List<object>();

            if (input is FlatLine || input is List<FlatLine>)
            {
                List<FlatLine> list = ShapeType.ListCreator<FlatLine>.Create(input);

                List<FlatLine> list1 = new List<FlatLine>();
                List<Point> list2 = new List<Point>();
                ret.Add(list1);
                ret.Add(list2);

                foreach (FlatLine fl in list)
                {
                    List<Vec2> points = SplitLines(fl.Line, distanceBetweenSplits, allEqual);
                    for (int i = 0; i < points.Count; ++i)
                    {
                        list2.Add(new Point(points[i], fl.PositionY));
                        if (i > 0)
                            list1.Add(new FlatLine(new Line2(points[i - 1], points[i]), fl.PositionY));
                    }
                }
            }
            else if (input is ExtrudedLine || input is List<ExtrudedLine>)
            {
                List<ExtrudedLine> list = ShapeType.ListCreator<ExtrudedLine>.Create(input);

                List<ExtrudedLine> list1 = new List<ExtrudedLine>();
                List<ExtrudedPoint> list2 = new List<ExtrudedPoint>();
                ret.Add(list1);
                ret.Add(list2);

                foreach (ExtrudedLine el in list)
                {
                    List<Vec2> points = SplitLines(el.Line, distanceBetweenSplits, allEqual);
                    for (int i = 0; i < points.Count; ++i)
                    {
                        list2.Add(new ExtrudedPoint(points[i], el.HeightInterval.Min, el.HeightInterval.Max));
                        if (i > 0)
                            list1.Add(new ExtrudedLine(new Line2(points[i - 1], points[i]), el.HeightInterval.Min, el.HeightInterval.Length));
                    }
                }
            }
            else if (input is Path)
            {
                if (!allEqual)
                    throw new NotImplementedException();

                List<FlatLine> list1 = new List<FlatLine>();
                List<Point> list2 = new List<Point>();
                ret.Add(list1);
                ret.Add(list2);
                Path p = (Path)input;

                float plen = p.Length();
                int nrOfSplits = (int)Math.Round(plen / distanceBetweenSplits);
                float splitLength = nrOfSplits == 0 ? plen : plen / nrOfSplits;
                Vec2 p1 = p.Points[0];
                Vec2 p2 = null;
                float s = (plen - (nrOfSplits * splitLength)) * 0.5f;
                float h = p.Points[0].Y;
                for (int i = 0; i < nrOfSplits + 1; ++i)
                {
                    if (i > 0)
                        list2.Add(new Point(p1, h));

                    if (i < nrOfSplits && s < plen)
                    {
                        p2 = (Vec2)p.GetPointAt(s);
                        list1.Add(new FlatLine(new Line2(p1, p2), h));
                        p1 = p2;
                        s += splitLength;
                    }
                }
            }
            else
                throw new Exception("SplitLines doesn't work for this shape: " + input);

            return ret;
        }

        private static List<Vec2> SplitLines(Line2 l, double distanceBetweenSplits, bool allEqual)
        {
            if (!allEqual)
                throw new NotImplementedException();

            List<Vec2> points = new List<Vec2>();

            double len = l.Length();
            int nrOfSplits = (int)Math.Round(len / distanceBetweenSplits);
            float splitLength = nrOfSplits == 0 ? 1 : 1 / (float)nrOfSplits;
            float s = 0;
            for (int i = 0; i < nrOfSplits + 1; ++i)
            {
                if (i == 0)
                    points.Add(new Vec2(l.P1));
                else if (i == nrOfSplits)
                    points.Add(new Vec2(l.P2));
                else
                    points.Add(l.PointOnLine(s));

                s += splitLength;
            }
            return points;
        }
        #endregion

        #region Flatten extruded shapes
        internal static List<object> FlattenExtrudedShapes(TransformationNode.ApplicationData data, object input, CustomTermEvaluater termEvaluater, List<string> parameters)
        {
            List<object> ret = new List<object>();
            if (input is ExtrudedPoint)
            {
                ExtrudedPoint ep = (ExtrudedPoint)input;
                ret.Add(new Point(ep.Point, ep.PositionY));
            }
            else if (input is ExtrudedLine)
            {
                ExtrudedLine el = (ExtrudedLine)input;
                ret.Add(new FlatLine(el.Line, el.PositionY));
            }
            else if (input is ExtrudedShape)
            {
                ExtrudedShape es = (ExtrudedShape)input;
                ret.Add(new FlatShape(es.Shape, es.HeightInterval.Min));
            }
            else if (input is List<ExtrudedPoint>)
            {
                List<Point> list = new List<Point>();
                foreach(ExtrudedPoint ep in (List<ExtrudedPoint>)input)
                    list.Add(new Point(ep.Point, ep.PositionY));
                ret.Add(list);
            }
            else if (input is List<ExtrudedLine>)
            {
                List<FlatLine> list = new List<FlatLine>();
                foreach (ExtrudedLine el in (List<ExtrudedLine>)input)
                    list.Add(new FlatLine(el.Line, el.PositionY));
                ret.Add(list);
            }
            else if (input is List<ExtrudedShape>)
            {
                List<FlatShape> list = new List<FlatShape>();
                foreach (ExtrudedShape es in (List<ExtrudedShape>)input)
                    list.Add(new FlatShape(es.Shape, es.HeightInterval.Min));
                ret.Add(list);
            }
            else
                throw new Exception("FlattenExtrudedShapes doesn't work for this shape: " + input);
            return ret;
        }
        #endregion

        #region Brake up shape
        internal static List<object> BrakeUpShape(TransformationNode.ApplicationData data, object input, CustomTermEvaluater termEvaluater, List<string> parameters)
        {
            List<object> ret = new List<object>();

            if (!(input is ExtrudedShape || input is List<ExtrudedShape>))
            {
                List<Point> list1 = new List<Point>();
                List<FlatLine> list2 = new List<FlatLine>();
                ret.Add(list1);
                ret.Add(list2);

                if (input is Path)
                {
                    Path path = (Path)input;
                    foreach (Vec3 p in path.Points)
                        list1.Add(new Point((Vec2)p, p.Y));
                    foreach (Line l in path.GetLines())
                        list2.Add(new FlatLine((Line2)l, 0.5f * (l.P1.Y + l.P2.Y)));
                }
                else if (input is List<Path>)
                {
                    foreach (Path path in (List<Path>)input)
                    {
                        foreach (Vec3 p in path.Points)
                            list1.Add(new Point((Vec2)p, p.Y));
                        foreach (Line l in path.GetLines())
                            list2.Add(new FlatLine((Line2)l, 0.5f * (l.P1.Y + l.P2.Y)));
                    }
                }
                else if (input is FlatShape || input is List<FlatShape>)
                {
                    List<FlatShape> list = ShapeType.ListCreator<FlatShape>.Create(input);
                    foreach (FlatShape fs in list)
                    {
                        foreach (Vec2 point in fs.Shape.Points)
                            list1.Add(new Point(point, fs.PositionY));
                        foreach (Line2 line in fs.Shape.GetLines())
                            list2.Add(new FlatLine(line, fs.PositionY));
                    }
                }
                else
                    throw new Exception("BrakeUpShape doesn't work for this shape: " + input);
            }
            else
            {
                List<ExtrudedPoint> list1 = new List<ExtrudedPoint>();
                List<ExtrudedLine> list2 = new List<ExtrudedLine>();
                ret.Add(list1);
                ret.Add(list2);

                List<ExtrudedShape> list = ShapeType.ListCreator<ExtrudedShape>.Create(input);
                foreach (ExtrudedShape es in list)
                {
                    foreach (Vec2 point in es.Shape.Points)
                        list1.Add(new ExtrudedPoint(point, es.HeightInterval.Min, es.HeightInterval.Max));
                    foreach (Line2 line in es.Shape.GetLines())
                        list2.Add(new ExtrudedLine(line, es.HeightInterval.Min, es.HeightInterval.Length));
                }
            }
            return ret;
        }
        #endregion

        #region Get shape border
        internal static List<object> GetShapeBorder(TransformationNode.ApplicationData data, object input, CustomTermEvaluater termEvaluater, List<string> parameters)
        {
            List<object> ret = new List<object>();
            if (input is FlatShape)
            {
                FlatShape fs = (FlatShape)input;
                ret.Add(CreatePath(fs.Shape, fs.PositionY));
            }
            else if (input is ExtrudedShape)
            {
                ExtrudedShape es = (ExtrudedShape)input;
                ret.Add(CreatePath(es.Shape, es.HeightInterval.Min));
            }
            else if (input is List<FlatShape>)
            {
                List<FlatShape> list = (List<FlatShape>)input;
                List<Path> paths = new List<Path>();
                ret.Add(paths);
                foreach (FlatShape fs in list)
                    paths.Add(CreatePath(fs.Shape, fs.PositionY));
            }
            else if (input is List<ExtrudedShape>)
            {
                List<ExtrudedShape> list = (List<ExtrudedShape>)input;
                List<Path> paths = new List<Path>();
                ret.Add(paths);
                foreach (ExtrudedShape es in list)
                    paths.Add(CreatePath(es.Shape, es.HeightInterval.Min));
            }
            else
                throw new Exception("GetShapeBorder doesn't work for this shape: " + input);

            return ret;
        }

        private static Path CreatePath(Common.Geometry.Shape shape, float h)
        {
            List<Vec3> points = new List<Vec3>();
            foreach (Vec2 p in shape.Points)
                points.Add(new Vec3(p.X, h, p.Y));
            if (points.Count > 0)
                points.Add(new Vec3(shape.Points[0].X, h, shape.Points[0].Y));
            return new Path(points);
        }
        #endregion

        #region Create Rows
        internal static List<object> CreateRows(TransformationNode.ApplicationData data, object input, CustomTermEvaluater termEvaluater, List<string> parameters)
        {
            List<object> ret = new List<object>();

            List<Tuple<float, float>> heights = new List<Tuple<float, float>>();
            List<Shape> baseShapes = new List<Shape>();
            bool extruded = false;
            if (input is FlatShape || input is List<FlatShape>)
            {
                foreach(FlatShape fs in ShapeType.ListCreator<FlatShape>.Create(input))
                {
                    baseShapes.Add(fs.Shape);
                    heights.Add(new Tuple<float, float>(fs.PositionY, -1));
                }
            }
            else if (input is ExtrudedShape || input is List<ExtrudedShape>)
            {
                extruded = true;
                foreach (ExtrudedShape es in ShapeType.ListCreator<ExtrudedShape>.Create(input))
                {
                    baseShapes.Add(es.Shape);
                    heights.Add(new Tuple<float, float>(es.HeightInterval.Min, es.Height));
                }
            }
            else
                throw new Exception("CreateRows doesn't work for this shape: " + input);

            object orientationParameter = Term.FromString(parameters[3]).GetValue(termEvaluater);
            Vec2 orientation;

            List<FlatLine> flatLines = new List<FlatLine>();
            List<ExtrudedLine> extrudedLines = new List<ExtrudedLine>();
            
            int shapeCount = 0;
            foreach(Shape s in baseShapes)
            {
                Line2[] lines = s.GetLines();
                List<Line2> lineList = new List<Line2>(lines);
                Line2 line = null;
                if (orientationParameter is string)
                {
                    //first, last, random, longest, shortest, xaxis or yaxis
                    switch ((string)orientationParameter)
                    {
                        case "first":
                            line = lines[0];
                            orientation = line.Dir();
                            break;
                        case "last":
                            line = lines[lines.Length - 1];
                            orientation = line.Dir();
                            break;
                        case "random":
                            line = lines[termEvaluater.GetRandom().Next(lines.Length)];
                            orientation = line.Dir();
                            break;
                        case "longest":
                            lineList.Sort(new LineLengthComparer());
                            line = lineList[lineList.Count - 1];
                            orientation = line.Dir();
                            break;
                        case "shortest":
                            lineList.Sort(new LineLengthComparer());
                            line = lineList[0];
                            orientation = line.Dir();
                            break;
                        case "xaxis":
                            orientation = new Vec2(1, 0);
                            throw new NotImplementedException();
                        case "yaxis":
                            orientation = new Vec2(0, 1);
                            throw new NotImplementedException();
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    int index = orientationParameter is int ? (int)orientationParameter : (int)Math.Round((double)orientationParameter);
                    orientationParameter = lines[index].Dir();
                }

                float farthestPointDistance = float.MinValue;
                Vec2 farthersPoint = null;
                float dummy;
                foreach (Vec2 v in s.Points)
                {
                    if (v != line.P1 && v != line.P2)
                    {
                        float dist = line.DistanceTo(v, out dummy);
                        if (dist > farthestPointDistance)
                        {
                            farthestPointDistance = dist;
                            farthersPoint = v;
                        }
                    }
                }

                float rowWidth = (float)(double)Term.FromString(parameters[0]).GetValue(termEvaluater);
                float widthFirstLast = (float)(double)Term.FromString(parameters[1]).GetValue(termEvaluater);
                if (widthFirstLast == 0)
                    widthFirstLast = rowWidth;
                float rowDist = (float)(double)Term.FromString(parameters[2]).GetValue(termEvaluater);

                if (farthestPointDistance > widthFirstLast)
                {
                    List<Vec2> rowPoints = new List<Vec2>();
                    Vec2 perpendicularDir = line.Dir().Cross();
                    if (!s.ClockWise())
                        perpendicularDir = -perpendicularDir;
                    if (farthestPointDistance < (2 * widthFirstLast + rowDist))
                        rowPoints.Add(line.MidPoint() + (farthestPointDistance * 0.5f) * perpendicularDir);
                    else
                    {
                        float lTemp = farthestPointDistance - (2 * widthFirstLast + rowDist);
                        float total = (2 * widthFirstLast + rowDist);
                        int numExtraRows = (int)Math.Floor(lTemp / (rowDist + rowWidth));
                        total += numExtraRows * (rowDist + rowWidth);
                        Vec2 start = line.MidPoint() + ((farthestPointDistance * 0.5f) - (total * 0.5f) + (widthFirstLast * 0.5f)) * perpendicularDir;
                        rowPoints.Add(start);
                        start += ((widthFirstLast * 0.5f) + rowDist + (rowWidth * 0.5f)) * perpendicularDir;
                        for (int i = 0; i < numExtraRows; ++i)
                        {
                            rowPoints.Add(start);
                            start += (rowWidth + rowDist) * perpendicularDir;
                        }
                        start += (-(rowWidth * 0.5f) + (widthFirstLast * 0.5f)) * perpendicularDir;
                        rowPoints.Add(start);
                    }

                    Tuple<float, float> height = heights[shapeCount];
                    foreach (Vec2 rp in rowPoints)
                    {
                        List<Line2> intersectedLines = s.IntersectionWithLine(new Line2(rp - line.Dir() * 100, rp + 100 * line.Dir()));
                        foreach (Line2 l in intersectedLines)
                        {
                            if (extruded)
                                extrudedLines.Add(new ExtrudedLine(l, height.Item1, height.Item2));
                            else
                                flatLines.Add(new FlatLine(l, height.Item1));
                        }
                    }
                }
                ++shapeCount;
            }

            if (extruded)
                ret.Add(extrudedLines);
            else
                ret.Add(flatLines);

            return ret;
        }
        #endregion

        #region GetCenter
        internal static List<object> GetCenter(TransformationNode.ApplicationData data, object input, CustomTermEvaluater termEvaluater, List<string> parameters)
        {
            List<object> ret = new List<object>();

            if (input is BaseShape)
            {
                BaseShape bs = (BaseShape)input;
                Vec3 temp = new Vec3(bs.MidPointOnFloor);
                temp.Y = bs.HeightToFloor;
                ret.Add(new Point(temp));
            }
            else
                throw new Exception("GetCenter doesn't work on lists: " + input);

            return ret;
        }
        #endregion
    }
}
