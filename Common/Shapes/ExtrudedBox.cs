using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Geometry;

namespace Common.Shapes
{
    public class ExtrudedBoxi : BaseShapei, IExtrudedShapei
    {
        Intervali heightInterval;
        Box2i box;

        public Intervali HeightInterval { get { return heightInterval; } }
        public Box2i Box { get { return box; } }

        public ExtrudedBoxi(Box2i box, Intervali heightInterval)
        {
            this.box = new Box2i(box.min, box.max);
            this.heightInterval = new Intervali(heightInterval);
        }

        public ExtrudedBoxi(Boxi box)
        {
            this.box = new Box2i(new Vec2i(box.Min.X, box.Min.Z), new Vec2i(box.Max.X, box.Max.Z));
            this.heightInterval = new Intervali(box.Min.Y, box.Max.Y);
        }

        public ExtrudedBoxi(Box2 box, Interval heightInterval, ToIntegerConversion c)
        {
            this.box = BaseShapei.Convert(box, c);
            this.heightInterval = BaseShapei.Convert(heightInterval, c);
        }

        public override BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c)
        {
            return new ExtrudedBox(box, heightInterval, c);
        }

        public override Boxi GetBoundingBox()
        {
            return Boxi.CreateFromPointList(new Vec2i[] { box.min, box.max }, heightInterval);
        }

        public override BaseShapei.EType Type
        {
            get { return EType.ExtBox; }
        }

        protected override IFlatShapei GetFlatShape()
        {
            return new FlatBoxi(box, 0);
        }

        public override BaseShapei Transform(Matrix4 matrix4)
        {
            return ConvertToShape().Transform(matrix4);
        }

        private ExtrudedShapei ConvertToShape()
        {
            Shapei shape = new Shapei(box);
            return new ExtrudedShapei(shape, heightInterval);
        }

        public override BaseShapei Transform(BaseShapei.BasicTransformation baseTransform)
        {
            switch (baseTransform)
            {
                case BasicTransformation.Rotate0deg:
                    return this;
                case BasicTransformation.Rotate90deg:
                    return new ExtrudedBoxi(new Box2i(new Vec2i(-box.max.Y, box.min.X), new Vec2i(-box.min.Y, box.max.X)), heightInterval);
                case BasicTransformation.Rotate180deg:
                    return new ExtrudedBoxi(new Box2i(-box.max, -box.min), heightInterval);
                case BasicTransformation.Rotate270deg:
                    return new ExtrudedBoxi(new Box2i(new Vec2i(box.min.Y, -box.max.X), new Vec2i(box.max.Y, -box.min.X)), heightInterval);
                case BasicTransformation.None:
                default:
                    throw new NotImplementedException();
            }
        }

        public override BaseShapei Translate(Vec3i vec3i)
        {
            Intervali newHeight = new Intervali(heightInterval);
            newHeight.Move(vec3i.Y);
            Vec2i vec = new Vec2i(vec3i.X, vec3i.Z);
            return new ExtrudedBoxi(new Box2i(box.min + vec, box.max + vec), newHeight);
        }

        #region IExtrudedShapei Members


        public IFlatShapei Flatten()
        {
            return new FlatBoxi(box, heightInterval.Min);
        }

        #endregion

        public override bool Overlaps(BaseShapei baseShapei)
        {
            switch (baseShapei.Type)
            {
                case EType.ExtShape:
                    ExtrudedShapei es = (ExtrudedShapei)baseShapei;
                    return es.HeightInterval.Overlaps(heightInterval) && box.Overlaps(es.Shape);
                case EType.ExtBox:
                    ExtrudedBoxi eb = (ExtrudedBoxi)baseShapei;
                    return eb.HeightInterval.Overlaps(heightInterval) && box.Overlaps(eb.box);
                default:
                    throw new NotImplementedException();
            }
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
            return box.Corners();
        }

        public override IEnumerable<BaseShapei> Cut(BaseShapei s)
        {
            return new ExtrudedShapei(new Shapei(this.box), new Intervali(heightInterval)).Cut(s);
        }

        public override Vec3i GetClosestPointTo(Vec3i pos)
        {
            Vec2i closestPoint2D = box.ClosestPointTo(new Vec2i(pos.X, pos.Z));
            return new Vec3i(closestPoint2D.X, heightInterval.GetClosestValueTo(pos.Y), closestPoint2D.Y);
        }
    }

    public class ExtrudedBox : BaseShape
    {
        Interval heightInterval;
        Box2 box;

        public Interval HeightInterval { get { return heightInterval; } }
        public Box2 Box { get { return box; } }

        public ExtrudedBox(Box2i box, Intervali heightInterval, FromIntegerConversion c)
        {
            this.box = BaseShape.Convert(box, c);
            this.heightInterval = BaseShape.Convert(heightInterval, c);
        }

        public ExtrudedBox(Box box)
        {
            this.heightInterval = new Interval(box.Minimum.Y, box.Maximum.Y);
            this.box = new Box2(box.Minimum, box.Maximum);
        }

        public override bool Overlaps(BaseShape shape)
        {
            throw new NotImplementedException();
        }

        public override float DistanceToPoint(Vec3 point)
        {
            Vec3 straightDistances = new Vec3();
            straightDistances.X = point.X < box.min.X ? box.min.X - point.X : point.X > box.max.X ? point.X - box.max.X : 0;
            straightDistances.Z = point.Z < box.min.Y ? box.min.Y - point.Z : point.Z > box.max.Y ? point.Z - box.max.Y : 0;
            straightDistances.Y = point.Y < heightInterval.Min ? heightInterval.Min - point.Y : point.Y > heightInterval.Max ? point.Y - heightInterval.Max : 0;
            return straightDistances.length();
        }

        public override BaseShape Transform(Matrix4 transformation)
        {
            return (new ExtrudedShape(new Shape(box), heightInterval.Min, heightInterval.Length)).Transform(transformation);
        }

        public override Common.Geometry.Node CreateNode(Common.Geometry.Material material)
        {
            throw new NotImplementedException();
        }

        public override Vec3 MidPointOnFloor
        {
            get { throw new NotImplementedException(); }
        }

        public override Vec3 MidPoint
        {
            get { return new Vec3(box.Midpoint.X, heightInterval.Mid, box.Midpoint.Y); }
        }

        public override float Height
        {
            get { throw new NotImplementedException(); }
        }

        public override float HeightToFloor
        {
            get { throw new NotImplementedException(); }
        }

        public override Box GetBoundingBox()
        {
            return CreateBox();
        }

        public override void Save(System.IO.BinaryWriter w)
        {
            throw new NotImplementedException();
        }

        public override string CreateFeatureDefinition()
        {
            throw new NotImplementedException();
        }

        public override BaseShape Offset(float p)
        {
            throw new NotImplementedException();
        }

        public override BaseShape Clone()
        {
            return new ExtrudedBox(new Box(new Vec3(box.min.X, heightInterval.Min, box.min.Y), new Vec3(box.max.X, heightInterval.Max, box.max.Y)));
        }

        public override BaseShapei ConvertToInteger(BaseShapei.ToIntegerConversion c)
        {
            return new ExtrudedBoxi(box, heightInterval, c);
        }

        internal Box CreateBox()
        {
            return new Box(new Vec3(box.min.X, heightInterval.Min, box.min.Y), new Vec3(box.max.X, heightInterval.Max, box.max.Y));
        }

        public override string ToString()
        {
            return "Box: " + box.min.ToString() + "-" + box.max.ToString() + " at height " + heightInterval.ToString();
        }

        public override void Translate(Vec3 translation)
        {
            box.Translate(translation);
            heightInterval.Move(translation.Y);
        }
    }
}
