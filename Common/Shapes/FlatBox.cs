using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Geometry;

namespace Common.Shapes
{
    public class FlatBoxi : BaseShapei, IFlatShapei
    {
        Box2i box;
        int positionY;

        public int PositionY { get { return positionY; } set { positionY = value; } }
        public Box2i Box { get { return box; } }

        public FlatBoxi(Box2i box, int positionY)
        {
            this.box = new Box2i(box.min, box.max);
            this.positionY = positionY;
        }

        public FlatBoxi(Box2 box, float positionY, ToIntegerConversion c)
        {
            this.box = Convert(box, c);
            this.positionY = c(positionY);
        }

        public override BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c)
        {
            return new FlatBox(box, positionY, c);
        }

        public override Boxi GetBoundingBox()
        {
            return Boxi.CreateFromPointList(new Vec2i[] { box.min, box.max }, positionY);
        }

        public override BaseShapei.EType Type
        {
            get { return EType.FlatBox; }
        }

        internal IFlatShapei MergeIn2D(BaseShapei b)
        {
            throw new NotImplementedException();
        }

        protected override IFlatShapei GetFlatShape()
        {
            return new FlatBoxi(box, 0);
        }

        #region IFlatShapei Members


        public IExtrudedShapei Extrude(Intervali heightInterval)
        {
            return new ExtrudedBoxi(box, heightInterval);
        }

        #endregion

        public override BaseShapei Transform(Matrix4 matrix4)
        {
            return ConvertToShape().Transform(matrix4);
        }

        private FlatShapei ConvertToShape()
        {
            Shapei shape = new Shapei(box);
            return new FlatShapei(shape, positionY);
        }

        public override BaseShapei Transform(BaseShapei.BasicTransformation baseTransform)
        {
            switch (baseTransform)
            {
                case BasicTransformation.Rotate0deg:
                    return this;
                case BasicTransformation.Rotate90deg:
                    return new FlatBoxi(new Box2i(new Vec2i(-box.max.Y, box.min.X), new Vec2i(-box.min.Y, box.max.X)), positionY);
                case BasicTransformation.Rotate180deg:
                    return new FlatBoxi(new Box2i(-box.max, -box.min), positionY);
                case BasicTransformation.Rotate270deg:
                    return new FlatBoxi(new Box2i(new Vec2i(box.min.Y, -box.max.X), new Vec2i(box.max.Y, -box.min.X)), positionY);
                case BasicTransformation.None:
                default:
                    throw new NotImplementedException();
            }
        }

        public override BaseShapei Translate(Vec3i vec3i)
        {
            Vec2i vec = new Vec2i(vec3i.X, vec3i.Z);
            return new FlatBoxi(new Box2i(box.min + vec, box.max + vec), positionY + vec3i.Y);
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
            return box.Corners();
        }

        public override IEnumerable<BaseShapei> Cut(BaseShapei s)
        {
            switch(s.Type)
            {
                case EType.ExtShape:
                    return (new FlatShapei(new Shapei(box), positionY)).Cut(s);
                case EType.FlatShape:
                    return (new FlatShapei(new Shapei(box), positionY)).Cut(s);
                default:
                    throw new System.NotImplementedException();
            }
        }

        public override Vec3i GetClosestPointTo(Vec3i pos)
        {
            Vec2i closestPoint2D = box.ClosestPointTo(new Vec2i(pos.X, pos.Z));
            return new Vec3i(closestPoint2D.X, positionY, closestPoint2D.Y);
        }
    }

    public class FlatBox : BaseShape
    {
        Box2 box;
        float positionY;

        public Box2 Box { get { return box; } }
        public float PositionY { get { return positionY; } }

        public FlatBox(Box2i box, int positionY, FromIntegerConversion c)
        {
            this.box = Convert(box, c);
            this.positionY = c(positionY);
        }

        public FlatBox(Box2 box, float positionY)
        {
            this.box = new Box2(box);
            this.positionY = positionY;
        }

        public override bool Overlaps(BaseShape shape)
        {
            throw new NotImplementedException();
        }

        public override float DistanceToPoint(Vec3 point)
        {
            throw new NotImplementedException();
        }

        public override BaseShape Transform(Matrix4 transformation)
        {
            return (new FlatShape(new Shape(this.box), positionY)).Transform(transformation);
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
            get { throw new NotImplementedException(); }
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
            return new Box(new Vec3(box.min.X, positionY, box.min.Y), new Vec3(box.max.X, positionY, box.max.Y));
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
            return new FlatBox(this.box, this.positionY);
        }

        public override BaseShapei ConvertToInteger(BaseShapei.ToIntegerConversion c)
        {
            return new FlatBoxi(box, positionY, c);
        }

        public override string ToString()
        {
            return "Box: " + box.min.ToString() + "-" + box.max.ToString() + " at height " + positionY;
        }

        public override void Translate(Vec3 translation)
        {
            box.Translate(translation);
            positionY += translation.Y;
        }
    }
}
