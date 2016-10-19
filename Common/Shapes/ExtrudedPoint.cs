using System;
using System.Collections.Generic;
using System.Text;
using Common;
using System.IO;

namespace Common.Shapes
{
    public class ExtrudedPointi : BaseShapei, IExtrudedShapei
    {
        Vec2i point;
        Intervali heightInterval;

        public int Height { get { return heightInterval.Length; } }
        public Intervali HeightInterval { get { return heightInterval; } }

        public ExtrudedPointi(Vec2 point, Interval heightInterval, ToIntegerConversion c)
        {
            this.point = Convert(point, c);
            this.heightInterval = Convert(heightInterval, c);
        }

        public ExtrudedPointi(Vec2i point, Intervali heightInterval)
        {
            this.heightInterval = new Intervali(heightInterval);
            this.point = new Vec2i(point);
        }

        public override BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c)
        {
            return new ExtrudedPoint(point, heightInterval, c);
        }

        public override Boxi GetBoundingBox()
        {
            return Boxi.CreateFromPointList(new Vec2i[] { point }, heightInterval);
        }

        public override BaseShapei.EType Type
        {
            get { return EType.ExtPoint; }
        }

        protected override IFlatShapei GetFlatShape()
        {
            return new Pointi(point, 0);
        }

        public override BaseShapei Transform(Matrix4 matrix4)
        {
            Vec3 temp = new Vec3(0, 0, 0);
            temp = temp * matrix4;
            return new ExtrudedPointi(new Vec2i(new Vec2(point) * matrix4), heightInterval + (int)Math.Round(temp.Y));
        }

        public override BaseShapei Transform(BaseShapei.BasicTransformation baseTransform)
        {
            return new ExtrudedPointi(Transform(point, baseTransform), heightInterval);
        }

        public override BaseShapei Translate(Vec3i vec3i)
        {
            return new ExtrudedPointi(point + new Vec2i(vec3i.X, vec3i.Z), heightInterval + vec3i.Y);
        }

        #region IExtrudedShapei Members


        public IFlatShapei Flatten()
        {
            return new Pointi(point, heightInterval.Min);
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
            yield return point;
        }

        public override IEnumerable<BaseShapei> Cut(BaseShapei s)
        {
            throw new NotImplementedException();
        }

        public override Vec3i GetClosestPointTo(Vec3i pos)
        {
            return new Vec3i(point.X, heightInterval.GetClosestValueTo(pos.Y), point.Y);
        }
    }

    public class ExtrudedPoint : BaseShape
    {
        Vec2 point;
        Interval heightInterval;

        public Vec2 Point { get { return point; } }
        public float PositionY { get { return heightInterval.Min; } set { heightInterval.Min = value; } }
        public override float Height { get { return heightInterval.Length; } }

        public override Vec3 MidPointOnFloor
        {
            get { Vec3 vec = point; vec.Y = PositionY; return vec; }
        }

        public override Vec3 MidPoint
        {
            get { Vec3 vec = point; vec.Y = heightInterval.Mid; return vec; }
        }

        public override float HeightToFloor
        {
            get { return PositionY; }
        }

        public void SetHeight(float value) {  heightInterval.Max = heightInterval.Min + value; }

        public ExtrudedPoint(Vec2i point, Intervali heightInterval, FromIntegerConversion c)
        {
            this.point = Convert(point, c);
            this.heightInterval = Convert(heightInterval, c);
        }

        public ExtrudedPoint(Vec2 point, float min, float max)
        {
            this.point = new Vec2(point);
            this.heightInterval = new Interval(min, max);
        }

        public override bool Overlaps(BaseShape shape)
        {
            throw new NotImplementedException();
        }

        public override BaseShape Transform(Common.Matrix4 transformation)
        {
            throw new NotImplementedException();
        }

        public override Common.Geometry.Node CreateNode(Common.Geometry.Material material)
        {
            throw new NotImplementedException();
        }

        internal List<BaseShape> Merge(BaseShape b)
        {
            List<BaseShape> ret = new List<BaseShape>();
            if (b is Point)
            {
                Point p = (Point)b;
                if (this.heightInterval.InInterval(p.PositionY))
                {
                    if ((p.Pnt - this.Point).length() < 0.001)
                        ret.Add(new Point(p.Pnt, p.PositionY));
                }
            }
            else if (b is ExtrudedPoint)
            {
                ExtrudedPoint p = (ExtrudedPoint)b;
                Interval i = heightInterval.Intersect(p.heightInterval);
                if (i != null)
                {
                    if ((p.Point - this.Point).length() < 0.001)
                        ret.Add(new ExtrudedPoint(p.Point, i.Min, i.Max));
                }
            }
            else if (b is FlatLine)
            {
                FlatLine fl = (FlatLine)b;
                if (this.heightInterval.InInterval(fl.PositionY))
                {
                    float factor;
                    float dist = fl.Line.DistanceTo(this.point, out factor);
                    if (dist < 0.001)
                        ret.Add(new Point(this.point, fl.PositionY));
                }
            }
            else if (b is ExtrudedLine)
            {
                ExtrudedLine el = (ExtrudedLine)b;
                Interval i = heightInterval.Intersect(el.HeightInterval);
                if (i != null)
                {
                    float factor;
                    float dist = el.Line.DistanceTo(this.point, out factor);
                    if (dist < 0.001)
                        ret.Add(new ExtrudedPoint(this.point, i.Min, i.Max));
                }
            }
            else if (b is FlatShape)
            {
                FlatShape fs = (FlatShape)b;
                if (heightInterval.InInterval(fs.PositionY))
                {
                    if (fs.Shape.Contains(point))
                        ret.Add(new Point(point, fs.PositionY));
                }
            }
            else
                throw new NotImplementedException();
            return ret;
        }

        public override string ToString()
        {
            return "Point '" + point + "' in " + heightInterval.ToString();
        }

        public override Box GetBoundingBox()
        {
            Box b = new Box(Vec3.Max, Vec3.Min);
            b.AddPointToBoundingBox(new Vec3(this.point.X, heightInterval.Min, this.point.Y));
            b.AddPointToBoundingBox(new Vec3(this.point.X, heightInterval.Max, this.point.Y));
            return b;
        }

        internal override object EvaluateFunctionInternal(string functionName, Common.MathParser.CustomTermEvaluater evaluator, object[] parameters)
        {
            return base.EvaluateFunctionInternal(functionName, evaluator, parameters);
        }

        public override void Save(BinaryWriter w)
        {
            w.Write("ExtrudedPoint");
            this.heightInterval.Save(w);
            this.point.Save(w);
        }

        public ExtrudedPoint(BinaryReader r)
        {
            this.heightInterval = new Interval(r);
            this.point = Vec2.Load(r);
        }

        public override string CreateFeatureDefinition()
        {
            throw new NotImplementedException();
        }

        public override BaseShape Clone()
        {
            return new ExtrudedPoint(this.point, this.heightInterval.Min, this.heightInterval.Max);
        }

        public override BaseShape Offset(float p)
        {
            Box2 b = new Box2((Vec2)this.point - new Vec2(p, p), (Vec2)this.point + new Vec2(p, p));
            return new ExtrudedShape(new Common.Geometry.Shape(b), this.heightInterval.Min, this.heightInterval.Length);
        }

        public override float DistanceToPoint(Vec3 point)
        {
            float vertDist = heightInterval.InInterval(point.Y) ? 0 : (float)Math.Min(Math.Abs(point.Y - heightInterval.Min), Math.Abs(heightInterval.Max - point.Y));

            float horDist = (((Vec2)point) - this.point).length();
            float ret = 0;
            if (vertDist > 0)
                ret = (float)Math.Sqrt(vertDist * vertDist + horDist * horDist);
            else
                ret = horDist;
            return ret;
        }

        public override BaseShapei ConvertToInteger(BaseShapei.ToIntegerConversion c)
        {
            return new ExtrudedPointi(point, heightInterval, c);
        }

        public override void Translate(Vec3 translation)
        {
            point.Translate((Vec2)translation);
            heightInterval.Move(translation.Y);
        }
    }
}
