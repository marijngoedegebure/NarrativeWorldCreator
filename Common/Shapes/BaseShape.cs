using System;
using System.Collections.Generic;

using System.Text;
using Common.Geometry;
using Common;
using System.IO;
using System.Reflection;
using Common.MathParser;

namespace Common.Shapes
{
    public interface IExtrudedShapei
    { 
        Intervali HeightInterval { get; }
        IFlatShapei Flatten();
    }
    public interface IFlatShapei 
    { 
        int PositionY { get; set; }
        IExtrudedShapei Extrude(Intervali heightInterval);
    }

    public abstract class BaseShapei
    {
        public enum BasicTransformation { None = 0, Rotate0deg, Rotate90deg, Rotate180deg, Rotate270deg }
        public enum EType { FlatPoint = 0, ExtPoint = 1, FlatAALine = 2, ExtAALine = 3, FlatLine = 4, ExtLine = 5, FlatBox = 6, ExtBox = 7, FlatShape = 8, ExtShape = 9, Path = 10, Group = 11 };
        public delegate int ToIntegerConversion(float val);

        public abstract BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c);
        public abstract Boxi GetBoundingBox();
        public abstract EType Type { get; }

        protected static Box2i Convert(Box2 box, ToIntegerConversion c)
        {
            return new Box2i(Convert(box.min, c), Convert(box.max, c));
        }

        protected static Vec2i Convert(Vec2 vec2, ToIntegerConversion c)
        {
            return new Vec2i(c(vec2.X), c(vec2.Y));
        }

        protected static Vec3i Convert(Vec3 vec3, ToIntegerConversion c)
        {
            return new Vec3i(c(vec3.X), c(vec3.Y), c(vec3.Z));
        }

        protected static Intervali Convert(Interval interval, ToIntegerConversion c)
        {
            return new Intervali(c(interval.Min), c(interval.Max));
        }

        protected static Line2i Convert(Line2 line, ToIntegerConversion c)
        {
            return new Line2i(Convert(line.P1, c), Convert(line.P2, c));
        }

        protected static Shapei Convert(Shape shape, ToIntegerConversion c)
        {
            List<Vec2i> points = new List<Vec2i>();
            foreach (Vec2 p in shape.Points)
                points.Add(Convert(p, c));
            return new Shapei(points);
        }

        public static IEnumerable<BaseShapei> Merge(BaseShapei a, BaseShapei b)
        {
            if (a == null || b == null)
            {
                if (a != null)
                    yield return a;
                else if (b != null)
                    yield return b;
                else
                    yield return null;
                yield break;
            }

            if ((a is IFlatShapei || a is IExtrudedShapei) && (b is IFlatShapei || b is IExtrudedShapei))
            {
                object mergedHeights = MergeHeightsOfFlatOrExtrudedShapes(a, b);
                if (mergedHeights != null)
                {
                    if (mergedHeights is int)
                    {
                        int posY = (int)mergedHeights;
                        //--- Flat
                        foreach (IFlatShapei fs in MergeIn2D(a, b))
                        {
                            fs.PositionY = posY;
                            yield return fs as BaseShapei;
                        }
                    }
                    else if (mergedHeights is Intervali)
                    {
                        Intervali interv = (Intervali)mergedHeights;
                        //--- Extruded
                        foreach (IFlatShapei fs in MergeIn2D(a, b))
                        {
                            IExtrudedShapei es = fs.Extrude(new Intervali(interv));
                            yield return es as BaseShapei;
                        }
                    }
                    else
                        throw new Exception("Hey, zooot! Mijn pa is wel polies eh.");
                }
            }
            else
            {
                //--- Special cases:
                throw new NotImplementedException();
            }
        }

        private static IEnumerable<IFlatShapei> MergeIn2D(BaseShapei a, BaseShapei b)
        {
            if ((int)b.Type > (int)a.Type)
            {
                foreach (IFlatShapei ifs in MergeIn2D(b, a))
                    yield return ifs;

            }
            else
            {
                IFlatShapei fa = a.GetFlatShape();
                IFlatShapei fb = b.GetFlatShape();
                foreach (IFlatShapei ifs in MergeIn2D(fa, fb))
                    yield return ifs;
            }
        }

        private static IEnumerable<IFlatShapei> MergeIn2D(IFlatShapei fa, IFlatShapei fb)
        {
            if (fa is Pointi)
            {
                #region Point
                //--- we can assume that b is also a point
                Pointi pa = (Pointi)fa;
                Pointi pb = (Pointi)fb;
                if (pa.Point.X == pb.Point.X && pa.Point.Y == pb.Point.Y)
                    yield return new Pointi(pa.Point, 0);
                #endregion
            }
            else if (fa is FlatAxisAlignedLinei)
            {
                #region AALine
                FlatAxisAlignedLinei faala = (FlatAxisAlignedLinei)fa;
                if (fb is Pointi)
                {
                    Pointi pb = (Pointi)fb;
                    switch (faala.Axis)
                    {
                        case Vec2.Component.X:
                            if (faala.OnAxis == pb.Point.X && pb.Point.Y >= faala.Start && pb.Point.Y <= faala.End)
                                yield return new Pointi(pb.Point, 0);
                            break;
                        case Vec2.Component.Y:
                            if (faala.OnAxis == pb.Point.Y && pb.Point.X >= faala.Start && pb.Point.X <= faala.End)
                                yield return new Pointi(pb.Point, 0);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    FlatAxisAlignedLinei faalb = (FlatAxisAlignedLinei)fb;
                    if (faala.Axis != faalb.Axis)
                    {
                        if (faala.Axis != Vec2.Component.X)
                        {
                            FlatAxisAlignedLinei temp = faala;
                            faala = faalb;
                            faalb = temp;
                        }
                        if (faala.OnAxis >= faalb.Start && faala.OnAxis <= faalb.End && faalb.OnAxis >= faala.Start && faalb.OnAxis <= faala.End)
                            yield return new Pointi(new Vec2i(faala.OnAxis, faalb.OnAxis), 0);
                    }
                    else
                    {
                        if (faala.OnAxis == faalb.OnAxis)
                        {
                            Intervali merged = faala.Interval().Merge(faalb.Interval());
                            if (merged != null)
                                yield return new FlatAxisAlignedLinei(faala.Axis, merged.Min, merged.Max, faala.OnAxis, 0);
                        }
                    }
                }
                #endregion
            }
            else if (fa is FlatLinei)
            {
                #region FlatLine
                FlatLinei fla = (FlatLinei)fa;
                if (fb is Pointi)
                {
                    Vec2i inters;
                    Pointi pb = (Pointi)fb;
                    if (Vec2i.DistanceToLine(pb.Point, fla.Line.P1, fla.Line.P2, out inters) < 1)
                        yield return new Pointi(inters, 0);
                }
                else if (fb is FlatAxisAlignedLinei)
                {
                    FlatAxisAlignedLinei faalb = (FlatAxisAlignedLinei)fb;

                    Vec2i min = new Vec2i(Math.Min(fla.Line.P1.X, fla.Line.P2.X), Math.Min(fla.Line.P1.Y, fla.Line.P2.Y));
                    Vec2i max = new Vec2i(Math.Max(fla.Line.P1.X, fla.Line.P2.X), Math.Max(fla.Line.P1.Y, fla.Line.P2.Y));

                    switch (faalb.Axis)
                    {
                        case Vec2.Component.X:
                            if (faalb.OnAxis >= min.X && faalb.OnAxis <= max.X)
                            {
                                if (min.X == max.X)
                                {
                                    Intervali merged = (new Intervali(min.X, max.X)).Merge(faalb.Interval());
                                    if (merged != null)
                                        yield return new FlatAxisAlignedLinei(Vec2.Component.X, merged.Min, merged.Max, faalb.OnAxis, 0);
                                }
                                else
                                {
                                    double fact = (double)(faalb.OnAxis - min.X) / (double)(max.X - min.X);
                                    int y = min.X == fla.Line.P1.X ? fla.Line.P1.Y + (int)Math.Round(fact * (fla.Line.P2.Y - fla.Line.P1.Y)) : fla.Line.P2.Y + (int)Math.Round(fact * (fla.Line.P1.Y - fla.Line.P2.Y));
                                    if (faalb.Interval().InInterval(y))
                                        yield return new Pointi(new Vec2i(faalb.OnAxis, y), 0);
                                }
                            }
                            break;
                        case Vec2.Component.Y:
                            if (faalb.OnAxis >= min.Y && faalb.OnAxis <= max.Y)
                            {
                                if (min.Y == max.Y)
                                {
                                    Intervali merged = (new Intervali(min.Y, max.Y)).Merge(faalb.Interval());
                                    if (merged != null)
                                        yield return new FlatAxisAlignedLinei(Vec2.Component.Y, merged.Min, merged.Max, faalb.OnAxis, 0);
                                }
                                else
                                {
                                    double fact = (double)(faalb.OnAxis - min.Y) / (double)(max.Y - min.Y);
                                    int x = min.Y == fla.Line.P1.Y ? fla.Line.P1.X + (int)Math.Round(fact * (fla.Line.P2.X - fla.Line.P1.X)) : fla.Line.P2.X + (int)Math.Round(fact * (fla.Line.P1.X - fla.Line.P2.X));
                                    if (faalb.Interval().InInterval(x))
                                        yield return new Pointi(new Vec2i(x, faalb.OnAxis), 0);
                                }
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    FlatLinei flb = (FlatLinei)fb;
                    float factor1, factor2;
                    Vec2i inters;
                    if (Vec2i.Intersection(fla.Line.P1, fla.Line.P2, flb.Line.P1, flb.Line.P2, out factor1, out factor2, out inters))
                        yield return new Pointi(inters, 0);
                }
                #endregion
            }
            else if (fa is FlatBoxi)
            {
                #region Box
                FlatBoxi fba = (FlatBoxi)fa;
                if (fb is Pointi)
                {
                    Pointi pb = (Pointi)fb;
                    if (fba.Box.IsInside(pb.Point))
                        yield return new Pointi(pb.Point, 0);
                }
                else if (fb is FlatAxisAlignedLinei)
                {
                    FlatAxisAlignedLinei faalb = (FlatAxisAlignedLinei)fb;
                    switch (faalb.Axis)
                    {
                        case Vec2.Component.X:
                            if (faalb.OnAxis >= fba.Box.min.X && faalb.OnAxis <= fba.Box.max.X)
                            {
                                Intervali merged = (new Intervali(fba.Box.min.X, fba.Box.max.X)).Merge(faalb.Interval());
                                if (merged != null)
                                    yield return new FlatAxisAlignedLinei(Vec2.Component.X, merged.Min, merged.Max, faalb.OnAxis, 0);
                            }
                            break;
                        case Vec2.Component.Y:
                            if (faalb.OnAxis >= fba.Box.min.Y && faalb.OnAxis <= fba.Box.max.Y)
                            {
                                Intervali merged = (new Intervali(fba.Box.min.Y, fba.Box.max.Y)).Merge(faalb.Interval());
                                if (merged != null)
                                    yield return new FlatAxisAlignedLinei(Vec2.Component.Y, merged.Min, merged.Max, faalb.OnAxis, 0);
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else if (fb is FlatLinei)
                {
                    FlatLinei flb = (FlatLinei)fb;
                    Line2 line = new Line2(flb.Line);
                    Line2 cut = fba.Box.Cut(line);
                    if (cut != null)
                        yield return new FlatLinei(new Line2i(cut), 0);
                }
                else
                {
                    FlatBoxi fbb = (FlatBoxi)fb;
                    Intervali mergedX = (new Intervali(fba.Box.min.X, fba.Box.max.X)).Merge(new Intervali(fbb.Box.min.X, fbb.Box.max.X));
                    Intervali mergedY = (new Intervali(fba.Box.min.Y, fba.Box.max.Y)).Merge(new Intervali(fbb.Box.min.Y, fbb.Box.max.Y));
                    if (mergedX != null && mergedY != null)
                        yield return new FlatBoxi(new Box2i(new Vec2i(mergedX.Min, mergedY.Min), new Vec2i(mergedX.Max, mergedY.Max)), 0);
                }
                #endregion
            }
            else
            {
                #region Shape
                FlatShapei fsa = (FlatShapei)fa;
                if (fb is Pointi)
                {
                    Pointi pb = (Pointi)fb;
                    if (fsa.Shape.Contains(pb.Point))
                        yield return new Pointi(pb.Point, 0);
                }
                else if (fb is FlatAxisAlignedLinei)
                {
                    FlatAxisAlignedLinei faalb = (FlatAxisAlignedLinei)fb;
                    foreach (Line2i l in fsa.Shape.IntersectionWithLine(faalb.CreateLine()))
                        yield return new FlatLinei(l, 0);
                }
                else if (fb is FlatLinei)
                {
                    FlatLinei flb = (FlatLinei)fb;
                    foreach (Line2i l in fsa.Shape.IntersectionWithLine(flb.Line))
                        yield return new FlatLinei(l, 0);
                }
                else if (fb is FlatBoxi)
                {
                    FlatBoxi fbb = (FlatBoxi)fb;
                    foreach (Shapei s in fsa.Shape.Intersection(fbb.Box.CreateShape()))
                        yield return new FlatShapei(s, 0);
                }
                else
                {
                    FlatShapei fsb = (FlatShapei)fb;
                    foreach (Shapei s in fsa.Shape.Intersection(fsb.Shape))
                        yield return new FlatShapei(s, 0);
                }
                #endregion
            }
        }

        protected abstract IFlatShapei GetFlatShape();

        private static object MergeHeightsOfFlatOrExtrudedShapes(BaseShapei a, BaseShapei b)
        {
            if (a is IFlatShapei)
            {
                IFlatShapei fa = (IFlatShapei)a;
                if (b is IFlatShapei)
                {
                    IFlatShapei fb = (IFlatShapei)b;
                    if (fa.PositionY == fb.PositionY)
                        return fa.PositionY;
                    else
                        return null;
                }
                else if (b is IExtrudedShapei)
                {
                    IExtrudedShapei eb = (IExtrudedShapei)b;
                    if (eb.HeightInterval.InInterval(fa.PositionY))
                        return fa.PositionY;
                    else
                        return null;
                }
                else
                    throw new Exception("Aaargh, hoe kan dit nu toch weer fout lopen?");
            }
            else if (a is IExtrudedShapei)
            {
                IExtrudedShapei ea = (IExtrudedShapei)a;
                if (b is IFlatShapei)
                    return MergeHeightsOfFlatOrExtrudedShapes(b, a);
                else if (b is IExtrudedShapei)
                    return ea.HeightInterval.Merge(((IExtrudedShapei)b).HeightInterval);
                else
                    throw new Exception("Aaargh, hoe kan dit nu toch weer fout lopen? bis");
            }
            else
                throw new Exception("Hoe... kan... dit... nu?");
        }

        public abstract BaseShapei Transform(Matrix4 matrix4);

        public abstract BaseShapei Transform(BasicTransformation baseTransform);

        public abstract BaseShapei Translate(Vec3i vec3i);

        internal static Vec2i Transform(Vec2i vec2i, BasicTransformation baseTransform)
        {
            switch (baseTransform)
            {
                case BasicTransformation.Rotate0deg:
                    return vec2i;
                case BasicTransformation.Rotate90deg:
                    return new Vec2i(-vec2i.Y, vec2i.X);
                case BasicTransformation.Rotate180deg:
                    return new Vec2i(-vec2i.X, -vec2i.Y);
                case BasicTransformation.Rotate270deg:
                    return new Vec2i(vec2i.Y, -vec2i.X);
                case BasicTransformation.None:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        internal static Vec2i Convert(Vec2i p, Matrix4 matrix4)
        {
            Vec3 vec = (new Vec3(p.X, 0, p.Y)) * matrix4;
            return new Vec2i((int)Math.Round(vec.X), (int)Math.Round(vec.Z));
        }

        public abstract bool Overlaps(BaseShapei baseShapei);
        public abstract int MinY { get; }
        public abstract int MaxY { get; }
        public abstract IEnumerable<Vec2i> GetPoints2D();

        public static BaseShapei MinkowskiSum(BaseShapei A, BaseShapei B)
        {
            int Alen = A.MaxY - A.MinY;
            int Blen = B.MaxY - B.MinY;
            int minY = (A.MinY - Blen) - B.MinY;
            int len = Alen + Blen;

            List<Vec2i> a = new List<Vec2i>(A.GetPoints2D());
            List<Vec2i> b = new List<Vec2i>(B.GetPoints2D());
            List<Vec2i> sum = Vec2i.MinkowskiSum(a, b);
            IFlatShapei flatShape;
            if (sum.Count == 1)
                flatShape = new Pointi(sum[0], minY);
            else if (sum.Count == 2)
                flatShape = new FlatLinei(new Line2i(sum[0], sum[1]), minY);
            else
                flatShape = new FlatShapei(new Shapei(sum), minY);

            BaseShapei ret;
            if (len == 0)
                ret = flatShape as BaseShapei;
            else
                ret = flatShape.Extrude(new Intervali(minY, minY + len)) as BaseShapei;
            return ret;
        }

        public static IEnumerable<BaseShapei> Cut(List<BaseShapei> A, BaseShapei B)
        {
            foreach (BaseShapei a in A)
                foreach (BaseShapei a2 in a.Cut(B))
                    yield return a2;
        }

        public abstract IEnumerable<BaseShapei> Cut(BaseShapei s);
        public abstract Vec3i GetClosestPointTo(Vec3i pos);
    }

    public abstract class BaseShape : FunctionalObject
    {
        public delegate float FromIntegerConversion(int val);

        protected static Box2 Convert(Box2i box, FromIntegerConversion c)
        {
            return new Box2(Convert(box.min, c), Convert(box.max, c));
        }

        protected static Vec2 Convert(Vec2i vec2i, FromIntegerConversion c)
        {
            return new Vec2(c(vec2i.X), c(vec2i.Y));
        }

        protected static Vec3 Convert(Vec3i vec3i, FromIntegerConversion c)
        {
            return new Vec3(c(vec3i.X), c(vec3i.Y), c(vec3i.Z));
        }

        protected static Interval Convert(Intervali interval, FromIntegerConversion c)
        {
            return new Interval(c(interval.Min), c(interval.Max));
        }

        protected static Line2 Convert(Line2i line, FromIntegerConversion c)
        {
            return new Line2(Convert(line.P1, c), Convert(line.P2, c));
        }

        protected static Shape Convert(Shapei shape, FromIntegerConversion c)
        {
            List<Vec2> points = new List<Vec2>();
            foreach (Vec2i p in shape.Points)
                points.Add(Convert(p, c));
            return new Shape(points);
        }

        public abstract bool Overlaps(BaseShape shape);
        public abstract float DistanceToPoint(Vec3 point);

        public abstract BaseShape Transform(Common.Matrix4 transformation);

        public abstract Node CreateNode(Material material);

        public abstract Vec3 MidPointOnFloor { get; }
        public abstract Vec3 MidPoint { get; }
        public abstract float Height { get; }
        public abstract float HeightToFloor { get; }

        public abstract BaseShapei ConvertToInteger(BaseShapei.ToIntegerConversion c);

        public static List<BaseShape> Merge(BaseShape a, BaseShape b)
        {
            if (a == null)
            {
                if (b == null)
                {
                    List<BaseShape> ret = new List<BaseShape>();
                    ret.Add(null);
                    return null;
                }
                else
                    return Merge(b, a);
            }
            else if (a is Point)
            {
                return ((Point)a).Merge(b);
            }
            else if (a is ExtrudedPoint)
            {
                if (b is Point)
                    return Merge(b, a);
                return ((ExtrudedPoint)a).Merge(b);
            }
            else if (a is FlatLine)
            {
                if (b is Point || b is ExtrudedPoint)
                    return Merge(b, a);
                return ((FlatLine)a).Merge(b);
            }
            else if (a is Path)
            {
                if (b is Point || b is ExtrudedPoint || b is FlatLine)
                    return Merge(b, a);
                return ((Path)a).Merge(b);
            }
            else if (a is ExtrudedLine)
            {
                if (b is Point || b is ExtrudedPoint || b is FlatLine)
                    return Merge(b, a);
                return ((ExtrudedLine)a).Merge(b);
            }
            else if (a is FlatShape)
            {
                if (b is Point || b is ExtrudedPoint || b is FlatLine || b is ExtrudedLine)
                    return Merge(b, a);
                return ((FlatShape)a).Merge(b);
            }
            else if (a is ExtrudedShape)
            {
                if (b is Point || b is ExtrudedPoint || b is FlatLine || b is ExtrudedLine || b is FlatShape)
                    return Merge(b, a);
                return ((ExtrudedShape)a).Merge(b);
            }
            throw new NotImplementedException();
        }

        public abstract Box GetBoundingBox();

        public virtual float GetAngle()
        {
            return 0;
        }

        #region FunctionalObject Members

        public object EvaluateFunction(string functionName, Common.MathParser.CustomTermEvaluater evaluator, params object[] parameters)
        {
            switch (functionName)
            {
                default:
                    return EvaluateFunctionInternal(functionName, evaluator, parameters);
            }
        }

        internal virtual object EvaluateFunctionInternal(string functionName, Common.MathParser.CustomTermEvaluater evaluator, object[] parameters)
        {
            switch (functionName)
            {
                case "height":
                    return (double)Height;
                case "angle":
                    return (double)GetAngle();
                case "mid":
                    return new object[] { (double)MidPointOnFloor.X, (double)MidPointOnFloor.Y };
                case "bb":
                    return new FunctionalBox(GetBoundingBox());
                case "offset":
                    return this.Offset((float)(double)parameters[0]);
                case "split":
                    return this.Split((string)parameters[0], (int)Math.Round((double)parameters[1]));
                case "extrude":
                    return this.Extrude((float)(double)parameters[0]);
            }

            throw new NotImplementedException();
        }

        public List<object> EvaluateFunctionOnList(string function, Common.MathParser.CustomTermEvaluater evaluator, Common.MathParser.ListEvaluationAid lea, List<object[]> parameters)
        {
            List<object> ret = new List<object>(lea.Size());
            object obj = EvaluateFunction(function, evaluator, parameters);
            for (int i = 0; i < lea.Size(); ++i)
                ret.Add(obj);
            return ret;
        }

        #endregion

        public abstract void Save(BinaryWriter w);

        public static BaseShape Load(BinaryReader r)
        {
            string classname = r.ReadString();
            Type classType = Type.GetType("Common.Shapes." + classname);

            ConstructorInfo ci = classType.GetConstructor(new Type[] { typeof(BinaryReader) });
            return (BaseShape) ci.Invoke(new object[] { r });
        }

        public abstract string CreateFeatureDefinition();

        public abstract BaseShape Offset(float p);

        public abstract BaseShape Clone();

        public static bool TestAdjacency(BaseShape a, BaseShape b)
        {
            if ((a is FlatShape || a is ExtrudedShape) && (b is FlatShape || b is ExtrudedShape))
            {
                Interval hA = a is FlatShape ? new Interval(((FlatShape)a).PositionY, ((FlatShape)a).PositionY) : ((ExtrudedShape)a).HeightInterval;
                Interval hB = b is FlatShape ? new Interval(((FlatShape)b).PositionY, ((FlatShape)b).PositionY) : ((ExtrudedShape)b).HeightInterval;
                Interval intersection = hA.Intersect(hB);
                bool heightAdjacent = intersection != null && intersection.Length > 0.01;
                //bool heightAdjacent = hB.InInterval(hA.Min) && hB.InInterval(hA.Max);
                //if (hA.Length > 0 && hB.Length > 0)
                //{
                //    Interval i = hA.Merge(hB);
                //    if (i != null && i.Length > 0)
                //        heightAdjacent = true;
                //}
                if (!heightAdjacent)
                    return false;
                Shape sA = a is FlatShape ? ((FlatShape)a).Shape : ((ExtrudedShape)a).Shape;
                Shape sB = b is FlatShape ? ((FlatShape)b).Shape : ((ExtrudedShape)b).Shape;
                Shape sAL = sA.CreateOffsetShape(0.05f, false);
                Shape sBL = sB.CreateOffsetShape(0.05f, false);

                foreach (Line2 lA in sA.GetLines())
                {
                    if (sBL.Contains(lA.P1) && sBL.Contains(lA.P2))
                        return true;
                }
                foreach (Line2 lB in sB.GetLines())
                {
                    if (sAL.Contains(lB.P1) && sAL.Contains(lB.P2))
                        return true;
                }
                return false;
            }
            else
                throw new NotImplementedException();
        }

        public static object CreateFromString(string p, CustomTermEvaluater evaluater)
        {
            int index = p.IndexOf(':');
            string first = p.Substring(0, index);
            string second = p.Substring(index + 1);
            List<Term> parameters;
            float height;
            float[] points;
            switch (first.Trim().ToLower())
            {
                case "flatshape":
                    parameters = Term.ParseCommaSeperatedListOfTerms(second);
                    points = new float[parameters.Count - 1];
                    for(int i = 0; i < parameters.Count - 1; ++i)
                        points[i] = (float)(double)parameters[i].GetValue(evaluater);
                    height = (float)(double)parameters[parameters.Count - 1].GetValue(evaluater);
                    return new FlatShape(new Shape(points), height);
                case "extrudedshape":
                    parameters = Term.ParseCommaSeperatedListOfTerms(second);
                    points = new float[parameters.Count - 2];
                    for (int i = 0; i < parameters.Count - 2; ++i)
                        points[i] = (float)(double)parameters[i].GetValue(evaluater);
                    float heightToFloor = (float)(double)parameters[parameters.Count - 2].GetValue(evaluater);
                    height = (float)(double)parameters[parameters.Count - 1].GetValue(evaluater);
                    return new ExtrudedShape(new Shape(points), heightToFloor, height);
                default:
                    throw new NotImplementedException();
            }
        }

        public BaseShape[] Split(string component, int nrOfSplits)
        {
            return SplitInternal((Vec3.Component)Enum.Parse(typeof(Vec3.Component), component, true), nrOfSplits);
        }

        protected virtual BaseShape[] SplitInternal(Vec3.Component component, int nrOfSplits)
        {
            throw new NotImplementedException();
        }

        public BaseShape Extrude(float height)
        {
            return ExtrudeInternal(height);
        }

        protected virtual BaseShape ExtrudeInternal(float height)
        {
            throw new NotImplementedException();
        }

        public abstract void Translate(Vec3 translation);
    }
}
