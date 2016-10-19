using System;
using System.Collections.Generic;

using System.Text;
using Common;
using Common.Geometry;
using System.IO;

namespace Common.Shapes
{
    public class ExtrudedLinei : BaseShapei, IExtrudedShapei
    {
        Line2i line;
        Intervali heightInterval;

        public Line2i Line { get { return line; } }
        public int Height { get { return heightInterval.Length; } }
        public Intervali HeightInterval { get { return heightInterval; } }

        public ExtrudedLinei(Line2 line, Interval heightInterval, ToIntegerConversion c)
        {
            this.line = Convert(line, c);
            this.heightInterval = Convert(heightInterval, c);
        }

        public ExtrudedLinei(Line2i line, Intervali heightInterval)
        {
            this.line = new Line2i(line);
            this.heightInterval = new Intervali(heightInterval);
        }

        public override BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c)
        {
            return new ExtrudedLine(line, heightInterval, c);
        }

        public override Boxi GetBoundingBox()
        {
            return Boxi.CreateFromPointList(new Vec2i[] { line.P1, line.P2 }, heightInterval);
        }

        public override BaseShapei.EType Type
        {
            get { return EType.ExtLine; }
        }

        protected override IFlatShapei GetFlatShape()
        {
            return new FlatLinei(line, 0);
        }

        public override BaseShapei Transform(Matrix4 matrix4)
        {
            Vec3 temp = new Vec3(0, 0, 0);
            temp = temp * matrix4;
            Intervali newHeight = new Intervali(heightInterval);
            newHeight.Move((int)Math.Round(temp.Y));
            return new ExtrudedLinei(line.Transform(matrix4), newHeight);
        }

        public override BaseShapei Transform(BaseShapei.BasicTransformation baseTransform)
        {
            return new ExtrudedLinei(new Line2i(Transform(line.P1, baseTransform), Transform(line.P2, baseTransform)), heightInterval);
        }

        public override BaseShapei Translate(Vec3i vec3i)
        {
            Line2i l = new Line2i(line);
            l.Move(new Vec2i(vec3i.X, vec3i.Z));
            return new ExtrudedLinei(l, heightInterval + vec3i.Y);
        }

        #region IExtrudedShapei Members


        public IFlatShapei Flatten()
        {
            return new FlatLinei(line, heightInterval.Min);
        }

        #endregion

        public override bool Overlaps(BaseShapei baseShapei)
        {
            throw new NotImplementedException();
        }

        public override int MinY
        {
            get { return heightInterval.Min; }
        }

        public override int MaxY
        {
            get { return heightInterval.Max; }
        }

        public override IEnumerable<Vec2i> GetPoints2D()
        {
            yield return line.P1;
            yield return line.P2;
        }

        public override IEnumerable<BaseShapei> Cut(BaseShapei s)
        {
            throw new NotImplementedException();
        }

        public override Vec3i GetClosestPointTo(Vec3i pos)
        {
            Vec2i closestPoint2D = line.ClosestPointOnLine(new Vec2i(pos.X, pos.Z));
            return new Vec3i(closestPoint2D.X, heightInterval.GetClosestValueTo(pos.Y), closestPoint2D.Y);
        }
    }

    public class ExtrudedLine : BaseShape
    {
        Line2 line;
        Interval heightInterval;

        public override float Height { get { return heightInterval.Length; } }
        public float PositionY { get { return heightInterval.Min; } }
        public Interval HeightInterval { get { return heightInterval; } }

        public override Vec3 MidPointOnFloor
        {
            get { Vec3 vec = line.MidPoint(); vec.Y = PositionY; return vec; }
        }

        public override Vec3 MidPoint
        {
            get { Vec3 vec = line.MidPoint(); vec.Y = heightInterval.Mid; return vec; }
        }

        public override float HeightToFloor
        {
            get { return PositionY; }
        }

        public Line2 Line { get { return line; } }

        public ExtrudedLine(Line2i line, Intervali heightInterval, FromIntegerConversion c)
        {
            this.line = Convert(line, c);
            this.heightInterval = Convert(heightInterval, c);
        }

        public ExtrudedLine(Line2 line, float positionY, float height)
        {
            this.line = new Line2(line);
            heightInterval = new Interval(positionY, positionY + height);
        }

        public override bool Overlaps(BaseShape shape)
        {
            if (shape is ExtrudedLine)
            {
                ExtrudedLine el = (ExtrudedLine)shape;
                if (!heightInterval.Overlaps(el.heightInterval))
                    return false;
                return Line2.IntersectionOnBothLines(line, el.line);
            }
            else if (shape is Point)
            {
                Point p = (Point)shape;
                return this.heightInterval.InInterval(p.PositionY) && this.line.IsPointOnLine(p.Pnt);
            }
            else if (shape is FlatShape)
                return shape.Overlaps(this);
            else if (shape is ExtrudedShape)
                return shape.Overlaps(this);

            throw new NotImplementedException();
        }

        public override BaseShape Transform(Matrix4 transformation)
        {
            Vec3 temp = new Vec3(0, 0, 0);
            temp = temp * transformation;
            return new ExtrudedLine(new Line2(line.P1.Transform(transformation), line.P2.Transform(transformation)), 
                                        heightInterval.Min + temp.Y, heightInterval.Length);
        }

        public override Common.Geometry.Node CreateNode(Common.Geometry.Material material)
        {
            Vec3 normal = (Vec3)(line.P2 - line.P1).Normalize().Cross();
            Common.Geometry.Object obj = new Common.Geometry.Object();
            obj.AddNode(SimpleShapes.CreateWallFace(line.P1, line.P2, heightInterval.Length, heightInterval.Min, material));
            obj.AddNode(SimpleShapes.CreateWallFace(line.P2, line.P1, heightInterval.Length, heightInterval.Min, material));
            return obj;
        }

        internal List<BaseShape> Merge(BaseShape b)
        {
            List<BaseShape> ret = new List<BaseShape>();
            if (b is ExtrudedLine)
            {
                ExtrudedLine el = (ExtrudedLine)b;
                Interval newInterval = heightInterval.Merge(el.heightInterval);
                if (newInterval != null)
                {
                    bool intersectionOnLine;
                    Vec2 inters = line.IntersectionOnLine(el.line, out intersectionOnLine);
                    if (inters != null && intersectionOnLine)
                    {
                        if (newInterval.Length == 0)
                            ret.Add(new Point(inters, newInterval.Min));
                        else
                            ret.Add(new ExtrudedPoint(inters, newInterval.Min, newInterval.Max));
                    }
                }
            }
            else if (b is FlatShape)
            {
                FlatShape f = (FlatShape)b;
                if (heightInterval.InInterval(f.PositionY))
                {
                    if (f.Shape.Contains(line.P1) && f.Shape.Contains(line.P2))
                        ret.Add(new FlatLine(line, f.PositionY));
                    else
                    {
                        foreach (Line2 l in f.Shape.IntersectionWithLine(line))
                            ret.Add(new FlatLine(l, f.PositionY));
                    }
                }
            }
            else if (b is ExtrudedShape)
            {
                ExtrudedShape es = (ExtrudedShape)b;
                if (es.HeightInterval.Overlaps(this.heightInterval))
                {
                    Interval hInt = es.HeightInterval.Merge(this.heightInterval);
                    if (es.Shape.Contains(line.P1) && es.Shape.Contains(line.P2))
                        ret.Add(new ExtrudedLine(line, hInt.Min, hInt.Length));
                    else
                    {
                        foreach (Line2 l in es.Shape.IntersectionWithLine(line))
                            ret.Add(new ExtrudedLine(l, hInt.Min, hInt.Length));
                    }
                }
            }
            else
                throw new NotImplementedException();
            return ret;
        }

        public override string ToString()
        {
            return "Extr. line '" + line.ToString() + "', height: " + heightInterval.ToString();
        }

        public override Box GetBoundingBox()
        {
            Box b = new Box(Vec3.Max, Vec3.Min);
            b.AddPointToBoundingBox(new Vec3(line.P1.X, heightInterval.Min, line.P1.Y));
            b.AddPointToBoundingBox(new Vec3(line.P2.X, heightInterval.Max, line.P2.Y));
            return b;
        }

        public override float GetAngle()
        {
            return line.Dir().GetAngle();
        }

        internal override object EvaluateFunctionInternal(string functionName, Common.MathParser.CustomTermEvaluater evaluator, object[] parameters)
        {
            switch (functionName)
            {
                case "length":
                    return (double)line.Length();
                case "P1":
                case "p1":
                case "start":
                    return new object[] { (double)line.P1.X, (double)line.P1.Y };
                case "P2":
                case "p2":
                case "end":
                    return new object[] { (double)line.P2.X, (double)line.P2.Y };
            }
            return base.EvaluateFunctionInternal(functionName, evaluator, parameters);
        }

        public override void Save(BinaryWriter w)
        {
            w.Write("ExtrudedLine");
            this.heightInterval.Save(w);
            this.line.Save(w);
        }

        public ExtrudedLine(BinaryReader r)
        {
            this.heightInterval = new Interval(r);
            this.line = new Line2(r);
        }

        public override string CreateFeatureDefinition()
        {
            return "ExtrudedLine((" + line.P1.X.ToString() + "; " + line.P1.Y.ToString() +
                        "), (" + line.P2.X.ToString() + "; " + line.P2.Y.ToString() + "), " + PositionY.ToString() + ", " + heightInterval.Length + ")";
        }

        public override BaseShape Clone()
        {
            return new ExtrudedLine(this.line, this.heightInterval.Min, this.heightInterval.Length);
        }

        public override BaseShape Offset(float p)
        {
            Vec2 cross = this.line.Dir().Cross();
            Shape s = new Shape(line.P1 + cross * p, line.P1 - cross * p, line.P2 - cross * p, line.P2 + cross * p);
            return new ExtrudedShape(s, this.heightInterval.Min, this.heightInterval.Length);
        }

        public override float DistanceToPoint(Vec3 point)
        {
            float vertDist = heightInterval.InInterval(point.Y) ? 0 : (float)Math.Min(Math.Abs(point.Y - heightInterval.Min), Math.Abs(heightInterval.Max - point.Y));
            float dummy;
            float horDist = this.line.DistanceTo((Vec2)point, out dummy);
            float ret = 0;
            if (vertDist > 0)
                ret = (float)Math.Sqrt(vertDist * vertDist + horDist * horDist);
            else
                ret = horDist;
            return ret;
        }

        public override BaseShapei ConvertToInteger(BaseShapei.ToIntegerConversion c)
        {
            return new ExtrudedLinei(line, heightInterval, c);
        }

        public override void Translate(Vec3 translation)
        {
            this.line.Translate(translation);
            heightInterval.Move(translation.Y);
        }
    }
}
