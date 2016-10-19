using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Shapes
{
    public class FlatAxisAlignedLinei : BaseShapei, IFlatShapei
    {
        Vec2.Component axis;
        int start, end;
        int onAxis;
        int positionY;

        public Vec2.Component Axis { get { return axis; } }
        public int Start { get { return start; } }
        public int End { get { return end; } }
        public int OnAxis { get { return onAxis; } }
        public int PositionY { get { return positionY; } set { positionY = value; } }

        public FlatAxisAlignedLinei(Vec2.Component axis, int start, int end, int onAxis, int positionY)
        {
            this.axis = axis;
            this.start = start;
            this.end = end;
            this.onAxis = onAxis;
            this.positionY = positionY;
        }

        public FlatAxisAlignedLinei(Vec2.Component axis, float start, float end, float onAxis, float positionY, ToIntegerConversion c)
        {
            this.axis = axis;
            this.start = c(start);
            this.end = c(end);
            this.onAxis = c(onAxis);
            this.positionY = c(positionY);
        }

        public override BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c)
        {
            return new FlatAxisAlignedLine(axis, start, end, onAxis, positionY, c);
        }

        public override Boxi GetBoundingBox()
        {
            switch (axis)
            {
                case Vec2.Component.X:
                    return new Boxi(new Vec3i(onAxis, positionY, Math.Min(start, end)), new Vec3i(onAxis, positionY, Math.Max(start, end)));
                case Vec2.Component.Y:
                    return new Boxi(new Vec3i(Math.Min(start, end), positionY, onAxis), new Vec3i(Math.Max(start, end), positionY, onAxis));
                default:
                    throw new NotImplementedException();
            }
        }

        public override BaseShapei.EType Type
        {
            get { return EType.FlatAALine; }
        }

        internal IFlatShapei MergeIn2D(BaseShapei b)
        {
            throw new NotImplementedException();
        }

        internal Intervali Interval()
        {
            return new Intervali(Math.Min(start, end), Math.Max(start, end));
        }

        internal Line2i CreateLine()
        {
            switch (axis)
            {
                case Vec2.Component.X:
                    return new Line2i(new Vec2i(onAxis, start), new Vec2i(onAxis, end));
                case Vec2.Component.Y:
                    return new Line2i(new Vec2i(start, onAxis), new Vec2i(end, onAxis));
                default:
                    throw new NotImplementedException();
            }
        }

        protected override IFlatShapei GetFlatShape()
        {
            return new FlatAxisAlignedLinei(axis, start, end, onAxis, 0);
        }

        public IExtrudedShapei Extrude(Intervali heightInterval)
        {
            return new ExtrudedAxisAlignedLinei(axis, start, end, onAxis, heightInterval);
        }

        public override BaseShapei Transform(Matrix4 matrix4)
        {
            FlatLinei fl = ConvertToLine();
            return fl.Transform(matrix4);
        }

        private FlatLinei ConvertToLine()
        {
            switch (axis)
            {
                case Vec2.Component.X:
                    return new FlatLinei(new Line2i(new Vec2i(onAxis, start), new Vec2i(onAxis, end)), positionY);
                case Vec2.Component.Y:
                    return new FlatLinei(new Line2i(new Vec2i(start, onAxis), new Vec2i(end, onAxis)), positionY);
                default:
                    throw new NotImplementedException();
            }
        }

        public override BaseShapei Transform(BaseShapei.BasicTransformation baseTransform)
        {
            switch (baseTransform)
            {
                case BasicTransformation.Rotate0deg:
                    return this;
                case BasicTransformation.Rotate90deg:
                    if (axis == Vec2.Component.Y)
                        return new FlatAxisAlignedLinei(Vec2.Component.X, start, end, -onAxis, positionY);
                    else
                        return new FlatAxisAlignedLinei(Vec2.Component.Y, -end, -start, onAxis, positionY);
                case BasicTransformation.Rotate180deg:
                    return new FlatAxisAlignedLinei(axis, -end, -start, -onAxis, positionY);
                case BasicTransformation.Rotate270deg:
                    if (axis == Vec2.Component.Y)
                        return new FlatAxisAlignedLinei(Vec2.Component.X, -end, -start, onAxis, positionY);
                    else
                        return new FlatAxisAlignedLinei(Vec2.Component.Y, start, end, -onAxis, positionY);
                case BasicTransformation.None:
                default:
                    throw new NotImplementedException();
            }
        }

        public override BaseShapei Translate(Vec3i vec3i)
        {
            switch (axis)
            {
                case Vec2.Component.X:
                    return new FlatAxisAlignedLinei(axis, start + vec3i.Z, end + vec3i.Z, onAxis + vec3i.X, positionY + vec3i.Y);
                case Vec2.Component.Y:
                    return new FlatAxisAlignedLinei(axis, start + vec3i.X, end + vec3i.X, onAxis + vec3i.Z, positionY + vec3i.Y);
                default:
                    throw new NotImplementedException();
            }
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
            switch (axis)
            {
                case Vec2.Component.X:
                    yield return new Vec2i(onAxis, start);
                    yield return new Vec2i(onAxis, end);
                    break;
                case Vec2.Component.Y:
                    yield return new Vec2i(start, onAxis);
                    yield return new Vec2i(end, onAxis);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override IEnumerable<BaseShapei> Cut(BaseShapei s)
        {
            throw new NotImplementedException();
        }

        public override Vec3i GetClosestPointTo(Vec3i pos)
        {
            switch (axis)
            {
                case Vec2.Component.X:
                    int valZ = pos.Z < start && pos.Z < end ? Math.Min(start, end) : pos.Z > start && pos.Z > end ? Math.Max(start, end) : pos.Z;
                    return new Vec3i(onAxis, positionY, valZ);
                case Vec2.Component.Y:
                    int valX = pos.X < start && pos.X < end ? Math.Min(start, end) : pos.X > start && pos.X > end ? Math.Max(start, end) : pos.X;
                    return new Vec3i(valX, positionY, onAxis);
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class FlatAxisAlignedLine : BaseShape
    {
        Vec2.Component axis;
        float start, end;
        float onAxis;
        float positionY;

        public FlatAxisAlignedLine(Vec2.Component axis, float start, float end, float onAxis, float positionY)
        {
            this.axis = axis;
            this.start = start;
            this.end = end;
            this.onAxis = onAxis;
            this.positionY = positionY;
        }

        public FlatAxisAlignedLine(Vec2.Component axis, int start, int end, int onAxis, int positionY, FromIntegerConversion c)
        {
            this.axis = axis;
            this.start = c(start);
            this.end = c(end);
            this.onAxis = c(onAxis);
            this.positionY = c(positionY);
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
            throw new NotImplementedException();
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

        public override BaseShapei ConvertToInteger(BaseShapei.ToIntegerConversion c)
        {
            return new FlatAxisAlignedLinei(axis, start, end, onAxis, positionY, c);
        }

        public override Box GetBoundingBox()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void Translate(Vec3 translation)
        {
            switch (axis)
            {
                case Vec2.Component.X:
                    onAxis += translation.X;
                    start += translation.Y;
                    end += translation.Y;
                    break;
                case Vec2.Component.Y:
                    onAxis += translation.Y;
                    start += translation.X;
                    end += translation.X;
                    break;
            }
            positionY += translation.Y;
        }
    }
}
