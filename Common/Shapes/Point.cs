using System;
using System.Collections.Generic;

using System.Text;
using Common;
using Common.Geometry;
using System.IO;

namespace Common.Shapes
{
    public class Pointi : BaseShapei, IFlatShapei
    {
        Vec2i point;
        int positionY;

        public Vec2i Point { get { return point; } }
        public int PositionY { get { return positionY; } set { positionY = value; } }

        public Pointi(Vec2 point, float positionY, ToIntegerConversion c)
        {
            this.point = Convert(point, c);
            this.positionY = c(positionY);
        }

        public Pointi(Vec2i point, int positionY)
        {
            this.positionY = positionY;
            this.point = new Vec2i(point);
        }

        public Pointi(Vec3i point)
        {
            this.positionY = point.Y;
            this.point = new Vec2i(point.X, point.Z);
        }

        public override BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c)
        {
            return new Point(point, positionY, c);
        }

        public override Boxi GetBoundingBox()
        {
            return Boxi.CreateFromPointList(new Vec2i[] { point }, positionY);
        }

        public override BaseShapei.EType Type
        {
            get { return EType.FlatPoint; }
        }

        protected override IFlatShapei GetFlatShape()
        {
            return new Pointi(point, 0);
        }

        #region IFlatShapei Members


        public IExtrudedShapei Extrude(Intervali heightInterval)
        {
            return new ExtrudedPointi(point, heightInterval);
        }

        #endregion

        public override BaseShapei Transform(Matrix4 matrix4)
        {
            Vec3 temp = new Vec3(0, 0, 0);
            temp = temp * matrix4;
            return new Pointi(new Vec2i(new Vec2(point) * matrix4), positionY + (int)Math.Round(temp.Y));
        }

        public override BaseShapei Transform(BaseShapei.BasicTransformation baseTransform)
        {
            return new Pointi(Transform(point, baseTransform), positionY);
        }

        public override BaseShapei Translate(Vec3i vec3i)
        {
            return new Pointi(point + new Vec2i(vec3i.X, vec3i.Z), positionY + vec3i.Y);
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
            yield return point;
        }

        public override IEnumerable<BaseShapei> Cut(BaseShapei s)
        {
            throw new NotImplementedException();
        }

        public override Vec3i GetClosestPointTo(Vec3i pos)
        {
            return new Vec3i(point.X, positionY, point.Y);
        }
    }

    public class Point : BaseShape
    {
        Vec2 point;
        float positionY;

        public Vec2 Pnt { get { return point; } }
        public float PositionY { get { return positionY; } set { positionY = value; } }

        public override float Height
        {
            get { return 0; }
        }
        public override Vec3 MidPointOnFloor
        {
            get { Vec3 v = point; v.Y = positionY; return v; }
        }
        public override Vec3 MidPoint
        {
            get { return MidPointOnFloor; }
        }

        public override float HeightToFloor
        {
            get { return PositionY; }
        }

        public Point(Vec2i point, int positionY, FromIntegerConversion c)
        {
            this.point = Convert(point, c);
            this.positionY = c(positionY);
        }

        public Point(Vec2 point, float positionY)
        {
            this.point = new Vec2(point);
            this.positionY = positionY;
        }

        public Point(Vec3 point)
            : this((Vec2)point, point.Y)
        {
        }

        public override bool Overlaps(BaseShape shape)
        {
            throw new NotImplementedException();
        }

        public override BaseShape Transform(Common.Matrix4 transformation)
        {
            Vec3 temp = new Vec3(0, 0, 0);
            temp = temp * transformation;
            return new Point(point.Transform(transformation), positionY + temp.Y);
        }

        public override Common.Geometry.Node CreateNode(Common.Geometry.Material material)
        {
            return SimpleShapes.CreateBox(new Vec3(point.X, positionY, point.Y), new Vec3(0.025f, 0.025f, 0.025f), material);
        }

        internal List<BaseShape> Merge(BaseShape b)
        {
            List<BaseShape> ret = new List<BaseShape>();
            if (b is ExtrudedLine)
            {
                ExtrudedLine el = (ExtrudedLine)b;
                if (el.HeightInterval.InInterval(this.positionY))
                {
                    float factor;
                    float dist = el.Line.DistanceTo(this.point, out factor);
                    if (dist < 0.001)
                        ret.Add(new Point(this.point, this.positionY));
                }
            }
            else if (b is FlatShape)
            {
                FlatShape fs = (FlatShape)b;
                if (positionY == fs.PositionY)
                {
                    if (fs.Shape.Contains(point))
                        ret.Add(new Point(point, fs.PositionY));
                }
            }
            else if (b is ExtrudedShape)
            {
                ExtrudedShape es = (ExtrudedShape)b;
                if (es.HeightInterval.InInterval(this.positionY))
                {
                    if (es.Shape.Contains(point))
                        ret.Add(new Point(point, PositionY));
                }
            }
            else if (b == null)
                ret.Add(new Point(this.point, this.positionY));
            else
                throw new NotImplementedException();
            return ret;
        }

        public override string ToString()
        {
            return "Point '" + point.ToString() + "' on height " + positionY;
        }

        public override Box GetBoundingBox()
        {
            Box b = new Box(Vec3.Max, Vec3.Min);
            b.AddPointToBoundingBox(new Vec3(this.point.X, positionY, this.point.Y));
            return b;
        }

        public override void Save(System.IO.BinaryWriter w)
        {
            w.Write("Point");
            this.point.Save(w);
            w.Write((double)this.positionY);
        }

        public Point(BinaryReader r)
        {
            this.point = Vec2.Load(r);
            this.positionY = (float)r.ReadDouble();
        }

        public override string CreateFeatureDefinition()
        {
            return "Point(" + point.X.ToString() + ", " + point.Y.ToString() + ")";
        }

        public override BaseShape Clone()
        {
            return new Point(this.point, positionY);
        }

        public override BaseShape Offset(float p)
        {
            Box2 b = new Box2((Vec2)this.point - new Vec2(p, p), (Vec2)this.point + new Vec2(p, p));
            return new FlatShape(new Shape(b), this.positionY);
        }

        public override float DistanceToPoint(Vec3 point)
        {
            return (point - MidPointOnFloor).length();
        }

        public override BaseShapei ConvertToInteger(BaseShapei.ToIntegerConversion c)
        {
            return new Pointi(point, positionY, c);
        }

        public override void Translate(Vec3 translation)
        {
            point.Translate(translation);
            positionY += translation.Y;
        }
    }
}
