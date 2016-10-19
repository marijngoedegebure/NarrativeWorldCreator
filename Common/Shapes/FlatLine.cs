using System;
using System.Collections.Generic;

using System.Text;
using Common;
using Common.Geometry;
using System.IO;

namespace Common.Shapes
{
    public class FlatLinei : BaseShapei, IFlatShapei
    {
        Line2i line;
        int positionY;

        public Line2i Line { get { return line; } }
        public int PositionY { get { return positionY; } set { positionY = value; } }

        public FlatLinei(Line2i line, int positionY)
        {
            this.line = new Line2i(line);
            this.positionY = positionY;
        }

        public FlatLinei(Line2 line, float positionY, ToIntegerConversion c)
        {
            this.line = BaseShapei.Convert(line, c);
            this.positionY = c(positionY);
        }

        public override BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c)
        {
            return new FlatLine(line, positionY, c);
        }

        public override Boxi GetBoundingBox()
        {
            return Boxi.CreateFromPointList(new Vec2i[] { line.P1, line.P2 }, positionY);
        }

        public override BaseShapei.EType Type
        {
            get { return EType.FlatLine; }
        }

        protected override IFlatShapei GetFlatShape()
        {
            return new FlatLinei(line, 0);
        }

        #region IFlatShapei Members


        public IExtrudedShapei Extrude(Intervali heightInterval)
        {
            return new ExtrudedLinei(line, heightInterval);
        }

        #endregion

        public override BaseShapei Transform(Matrix4 matrix4)
        {
            Vec3 temp = new Vec3(0, 0, 0);
            temp = temp * matrix4;
            return new FlatLinei(line.Transform(matrix4), positionY);
        }

        public override BaseShapei Transform(BaseShapei.BasicTransformation baseTransform)
        {
            return new FlatLinei(new Line2i(Transform(line.P1, baseTransform), Transform(line.P2, baseTransform)), positionY);
        }

        public override BaseShapei Translate(Vec3i vec3i)
        {
            Line2i l = new Line2i(line);
            l.Move(new Vec2i(vec3i.X, vec3i.Z));
            return new FlatLinei(l, positionY + vec3i.Y);
        }

        public override bool Overlaps(BaseShapei baseShapei)
        {
            throw new NotImplementedException();
        }

        public override int MinY
        {
            get { return positionY; }
        }

        public override int MaxY
        {
            get { return positionY; }
        }

        public override IEnumerable<Vec2i> GetPoints2D()
        {
            yield return line.P1;
            yield return line.P2;
        }

        class PointsComparerFromLineStart : IComparer<Vec2i>
        {
            Vec2i lineStart;

            public PointsComparerFromLineStart(Vec2i lineStart)
            {
                this.lineStart = lineStart;
            }

            #region IComparer<Vec2i> Members

            public int Compare(Vec2i x, Vec2i y)
            {
                return (x - lineStart).squareLength().CompareTo((y - lineStart).squareLength());
            }

            #endregion
        }

        public override IEnumerable<BaseShapei> Cut(BaseShapei s)
        {
            switch (s.Type)
            {
                case EType.ExtShape:
                    ExtrudedShapei es = (ExtrudedShapei)s;
                    if (this.positionY < es.HeightInterval.Min || this.positionY > es.HeightInterval.Max)
                        yield return new FlatLinei(this.line, positionY);
                    else
                    {
                        PointsComparerFromLineStart pcfls = new PointsComparerFromLineStart(this.line.P1);
                        List<Vec2i> intersectionPoints = new List<Vec2i>();
                        foreach(Line2i line in es.Shape.GetLines())
                        {
                            Vec2i point;
                            if (Line2i.IntersectionOnBothLines(this.line, line, out point))
                                intersectionPoints.Add(point);
                        }
                        intersectionPoints.Add(this.line.P1);
                        intersectionPoints.Add(this.line.P2);
                        intersectionPoints.Sort(pcfls);
                        
                        for (int i = 0; i < intersectionPoints.Count - 1; ++i)
                        {
                            Line2i possibleLine = new Line2i(intersectionPoints[i], intersectionPoints[i + 1]);
                            if (!es.Shape.Contains(possibleLine.MidPoint()))
                                yield return new FlatLinei(possibleLine, positionY);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override Vec3i GetClosestPointTo(Vec3i pos)
        {
            Vec2i closestPoint2D = line.ClosestPointOnLine(new Vec2i(pos.X, pos.Z));
            return new Vec3i(closestPoint2D.X, positionY, closestPoint2D.Y);
        }
    }

    public class FlatLine : BaseShape
    {
        Line2 line;
        float positionY;

        public float PositionY { get { return positionY; } }

        public override float Height
        {
            get { return 0; }
        }

        public override Vec3 MidPointOnFloor
        {
            get
            {
                Vec3 midpoint = line.MidPoint();
                midpoint.Y = positionY;
                return midpoint;
            }
        }

        public override Vec3 MidPoint
        {
            get
            {
                return MidPointOnFloor;
            }
        }

        public Line2 Line { get { return line; } }

        public override float HeightToFloor
        {
            get { return PositionY; }
        }

        public FlatLine(Line2i line, int positionY, FromIntegerConversion c)
        {
            this.line = Convert(line, c);
            this.positionY = c(positionY);
        }

        public FlatLine(Line2 line, float positionY)
        {
            this.line = new Line2(line);
            this.positionY = positionY;
        }

        public override bool Overlaps(BaseShape shape)
        {
            if (shape is FlatLine)
            {
                FlatLine fl = (FlatLine)shape;
                if (positionY != fl.positionY)
                    return false;
                return Line2.IntersectionOnBothLines(line, fl.line);
            }
            else if (shape is ExtrudedLine)
                return shape.Overlaps(this);
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
            return new FlatLine(new Line2(line.P1.Transform(transformation), line.P2.Transform(transformation)), positionY + temp.Y);
        }

        public override Common.Geometry.Node CreateNode(Common.Geometry.Material material)
        {
            Vec3 normal = (Vec3)(line.P2 - line.P1).Normalize().Cross();
            Common.Geometry.Object obj = new Common.Geometry.Object();
            obj.AddNode(SimpleShapes.CreateWallFace(line.P1, line.P2, 0.05f, positionY, material));
            obj.AddNode(SimpleShapes.CreateWallFace(line.P2, line.P1, 0.05f, positionY, material));
            return obj;
        }

        internal List<BaseShape> Merge(BaseShape b)
        {
            List<BaseShape> ret = new List<BaseShape>();
            if (b is ExtrudedLine)
            {
                ExtrudedLine el = (ExtrudedLine)b;
                if (el.HeightInterval.InInterval(positionY))
                {
                    bool intersectionOnLine;
                    Vec2 point = line.IntersectionOnLine(el.Line, out intersectionOnLine);
                    if (point != null && intersectionOnLine)
                        ret.Add(new Point(point, positionY));
                }
            }
            else if (b is FlatShape)
            {
                FlatShape fs = (FlatShape)b;
                if (Math.Abs(this.positionY - fs.PositionY) < 0.001)
                {
                    if (fs.Shape.Contains(line.P1) && fs.Shape.Contains(line.P2))
                        ret.Add(new FlatLine(line, this.positionY));
                    else
                    {
                        foreach (Line2 l in fs.Shape.IntersectionWithLine(line))
                            ret.Add(new FlatLine(l, this.positionY));
                    }
                }
            }
            else if (b is ExtrudedShape)
            {
                ExtrudedShape es = (ExtrudedShape)b;
                if (es.HeightInterval.InInterval(this.positionY))
                {
                    if (es.Shape.Contains(line.P1) && es.Shape.Contains(line.P2))
                        ret.Add(new FlatLine(line, this.positionY));
                    else
                    {
                        foreach (Line2 l in es.Shape.IntersectionWithLine(line))
                            ret.Add(new FlatLine(l, this.positionY));
                    }
                }
            }
            else
                throw new NotImplementedException();
            return ret;
        }

        public override string ToString()
        {
            return "Line '" + line.ToString() + "' on height " + positionY;
        }

        public override Box GetBoundingBox()
        {
            Box b = new Box(Vec3.Max, Vec3.Min);
            b.AddPointToBoundingBox(new Vec3(line.P1.X, positionY, line.P1.Y));
            b.AddPointToBoundingBox(new Vec3(line.P2.X, positionY, line.P2.Y));
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
            w.Write("FlatLine");
            this.line.Save(w);
            w.Write((double)this.positionY);
        }

        public FlatLine(BinaryReader r)
        {
            this.line = new Line2(r);
            this.positionY = (float)r.ReadDouble();
        }

        public override string CreateFeatureDefinition()
        {
            throw new NotImplementedException();
        }

        public override BaseShape Clone()
        {
            return new FlatLine(this.line, this.positionY);
        }

        public override BaseShape Offset(float p)
        {
            Vec2 cross = this.line.Dir().Cross();
            Shape s = new Shape(line.P1 + cross * p, line.P1 - cross * p, line.P2 - cross * p, line.P2 + cross * p);
            return new FlatShape(s, this.positionY);
        }

        public override float DistanceToPoint(Vec3 point)
        {
            float vertDist = (float)Math.Abs(point.Y - positionY);
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
            return new FlatLinei(line, positionY, c);
        }

        public override void Translate(Vec3 translation)
        {
            line.Translate((Vec2)translation);
            positionY += translation.Y;
        }
    }
}
