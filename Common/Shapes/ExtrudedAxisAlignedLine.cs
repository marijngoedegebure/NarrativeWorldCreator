using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Shapes
{
    public class ExtrudedAxisAlignedLinei : BaseShapei, IExtrudedShapei
    {
        Vec2.Component axis;
        int start, end;
        int onAxis;
        Intervali heightInterval;

        public Vec2.Component Axis { get { return axis; } }
        public int Start { get { return start; } }
        public int End { get { return end; } }
        public int OnAxis { get { return onAxis; } }
        public Intervali HeightInterval { get { return heightInterval; } }

        public ExtrudedAxisAlignedLinei(Vec2.Component axis, int start, int end, int onAxis, Intervali heightInterval)
        {
            this.axis = axis;
            this.start = start;
            this.end = end;
            this.onAxis = onAxis;
            this.heightInterval = heightInterval;
        }

        public ExtrudedAxisAlignedLinei(Vec2.Component axis, float start, float end, float onAxis, Interval heightInterval, ToIntegerConversion c)
        {
            this.axis = axis;
            this.start = c(start);
            this.end = c(end);
            this.onAxis = c(onAxis);
            this.heightInterval = Convert(heightInterval, c);
        }

        public override BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c)
        {
            return new ExtrudedAxisAlignedLine(axis, start, end, onAxis, heightInterval, c);
        }

        public override Boxi GetBoundingBox()
        {
            switch (axis)
            {
                case Vec2.Component.X:
                    return new Boxi(new Vec3i(onAxis, heightInterval.Min, Math.Min(start, end)), new Vec3i(onAxis, heightInterval.Max, Math.Max(start, end)));
                case Vec2.Component.Y:
                    return new Boxi(new Vec3i(Math.Min(start, end), heightInterval.Min, onAxis), new Vec3i(Math.Max(start, end), heightInterval.Max, onAxis));
                default:
                    throw new NotImplementedException();
            }
        }

        public override BaseShapei.EType Type
        {
            get { return EType.ExtAALine; }
        }

        protected override IFlatShapei GetFlatShape()
        {
            return new FlatAxisAlignedLinei(axis, start, end, onAxis, 0);
        }

        public override BaseShapei Transform(Matrix4 matrix4)
        {
            ExtrudedLinei el = ConvertToLine();
            return el.Transform(matrix4);
        }

        private ExtrudedLinei ConvertToLine()
        {
            switch (axis)
            {
                case Vec2.Component.X:
                    return new ExtrudedLinei(new Line2i(new Vec2i(onAxis, start), new Vec2i(onAxis, end)), heightInterval);
                case Vec2.Component.Y:
                    return new ExtrudedLinei(new Line2i(new Vec2i(start, onAxis), new Vec2i(end, onAxis)), heightInterval);
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
                        return new ExtrudedAxisAlignedLinei(Vec2.Component.X, start, end, -onAxis, heightInterval);
                    else
                        return new ExtrudedAxisAlignedLinei(Vec2.Component.Y, -end, -start, onAxis, heightInterval);
                case BasicTransformation.Rotate180deg:
                    return new ExtrudedAxisAlignedLinei(axis, -end, -start, -onAxis, heightInterval);
                case BasicTransformation.Rotate270deg:
                    if (axis == Vec2.Component.Y)
                        return new ExtrudedAxisAlignedLinei(Vec2.Component.X, -end, -start, onAxis, heightInterval);
                    else
                        return new ExtrudedAxisAlignedLinei(Vec2.Component.Y, start, end, -onAxis, heightInterval);
                case BasicTransformation.None:
                default:
                    throw new NotImplementedException();
            }
        }

        public override BaseShapei Translate(Vec3i vec3i)
        {
            Intervali newHeight = new Intervali(heightInterval);
            newHeight.Move(vec3i.Y);
            switch (axis)
            {
                case Vec2.Component.X:
                    return new ExtrudedAxisAlignedLinei(axis, start + vec3i.Z, end + vec3i.Z, onAxis + vec3i.X, newHeight);
                case Vec2.Component.Y:
                    return new ExtrudedAxisAlignedLinei(axis, start + vec3i.X, end + vec3i.X, onAxis + vec3i.Z, newHeight);
                default:
                    throw new NotImplementedException();
            }
        }

        #region IExtrudedShapei Members


        public IFlatShapei Flatten()
        {
            return new FlatAxisAlignedLinei(axis, start, end, onAxis, heightInterval.Min);
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
                    return new Vec3i(onAxis, heightInterval.GetClosestValueTo(pos.Y), valZ);
                case Vec2.Component.Y:
                    int valX = pos.X < start && pos.X < end ? Math.Min(start, end) : pos.X > start && pos.X > end ? Math.Max(start, end) : pos.X;
                    return new Vec3i(valX, heightInterval.GetClosestValueTo(pos.Y), onAxis);
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class ExtrudedAxisAlignedLine : BaseShape
    {
        Vec2.Component axis;
        float start, end;
        float onAxis;
        Interval heightInterval;


        public Vec2.Component Axis { get { return axis; } }
        public float OnAxis { get { return onAxis; } set { onAxis = value; } }
        public float Start { get { return start; } set { start = value; } }
        public float End { get { return end; } set { end = value; } }
        public Interval HeightInterval { get { return heightInterval; } }

        public Line2 Line
        {
            get
            {
                switch (axis)
                {
                    case Vec2.Component.X:
                        return new Line2(new Vec2(onAxis, start), new Vec2(onAxis, end));
                    case Vec2.Component.Y:
                        return new Line2(new Vec2(start, onAxis), new Vec2(end, onAxis));
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public ExtrudedAxisAlignedLine(Vec2.Component axis, float start, float end, float onAxis, Interval heightInterval)
        {
            this.axis = axis;
            this.start = start;
            this.end = end;
            this.onAxis = onAxis;
            this.heightInterval = heightInterval;
        }

        public ExtrudedAxisAlignedLine(Vec2.Component axis, int start, int end, int onAxis, Intervali heightInterval, FromIntegerConversion c)
        {
            this.axis = axis;
            this.start = c(start);
            this.end = c(end);
            this.onAxis = c(onAxis);
            this.heightInterval = Convert(heightInterval, c);
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
            get { return heightInterval.Length; }
        }

        public override float HeightToFloor
        {
            get { return heightInterval.Min; }
        }

        public override BaseShapei ConvertToInteger(BaseShapei.ToIntegerConversion c)
        {
            return new ExtrudedAxisAlignedLinei(axis, start, end, onAxis, heightInterval, c);
        }

        public override Box GetBoundingBox()
        {
            switch (axis)
            {
                case Vec2.Component.X:
                    return new Box(new Vec3(onAxis, heightInterval.Min, Math.Min(start, end)), new Vec3(onAxis, heightInterval.Max, Math.Max(start, end)));
                case Vec2.Component.Y:
                    return new Box(new Vec3(Math.Min(start, end), heightInterval.Min, onAxis), new Vec3(Math.Max(start, end), heightInterval.Max, onAxis));
                default:
                    throw new NotImplementedException();
            }
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
                    start += translation.Z;
                    end += translation.Z;
                    break;
                case Vec2.Component.Y:
                    onAxis += translation.Z;
                    start += translation.X;
                    end += translation.X;
                    break;
            }
            heightInterval.Move(translation.Y);
        }
    }
}
