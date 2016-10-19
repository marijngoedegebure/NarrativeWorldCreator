using System;
using System.Collections.Generic;

using System.Text;
using Common.Geometry;
using Common;
using System.IO;

namespace Common.Shapes
{
    public class ExtrudedShapei : BaseShapei, IExtrudedShapei
    {
        Shapei shape;
        Intervali heightInterval;
        
        public Shapei Shape { get { return shape; } }
        public int Height { get { return heightInterval.Length; } }
        public Intervali HeightInterval { get { return heightInterval; } }

        public ExtrudedShapei(Shapei shape, Intervali heightInterval)
        {
            this.shape = shape;
            this.heightInterval = new Intervali(heightInterval);
        }

        public ExtrudedShapei(Shape shape, Interval heightInterval, ToIntegerConversion c)
        {
            this.shape = BaseShapei.Convert(shape, c);
            this.heightInterval = Convert(heightInterval, c);
        }

        public override BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c)
        {
            return new ExtrudedShape(shape, heightInterval, c);
        }

        public override Boxi GetBoundingBox()
        {
            return Boxi.CreateFromPointList(shape.Points, heightInterval);
        }

        public override BaseShapei.EType Type
        {
            get { return EType.ExtShape; }
        }

        protected override IFlatShapei GetFlatShape()
        {
            return new FlatShapei(shape, 0);
        }

        public override BaseShapei Transform(Matrix4 matrix4)
        {
            Vec3 temp = new Vec3(0, 0, 0);
            temp = temp * matrix4;
            List<Vec2i> newPoints = new List<Vec2i>(shape.Points.Count);
            foreach (Vec2i p in shape.Points)
                newPoints.Add(Convert(p, matrix4));
            return new ExtrudedShapei(new Shapei(newPoints), heightInterval + (int)Math.Round(temp.Y));
        }

        public override BaseShapei Transform(BaseShapei.BasicTransformation baseTransform)
        {
            List<Vec2i> newPoints = new List<Vec2i>(shape.Points.Count);
            foreach (Vec2i p in shape.Points)
                newPoints.Add(Transform(p, baseTransform));
            return new ExtrudedShapei(new Shapei(newPoints), heightInterval);
        }

        public override BaseShapei Translate(Vec3i vec3i)
        {
            Vec2i move = new Vec2i(vec3i.X, vec3i.Z);
            List<Vec2i> newPoints = new List<Vec2i>(shape.Points.Count);
            foreach (Vec2i p in shape.Points)
                newPoints.Add(p + move);
            return new ExtrudedShapei(new Shapei(newPoints), heightInterval + vec3i.Y);
        }

        #region IExtrudedShapei Members


        public IFlatShapei Flatten()
        {
            return new FlatShapei(shape, heightInterval.Min);
        }

        #endregion

        public override bool Overlaps(BaseShapei baseShapei)
        {
            if (baseShapei is ExtrudedShapei)
            {
                ExtrudedShapei es = (ExtrudedShapei)baseShapei;
                if (heightInterval.Overlaps(es.heightInterval))
                    return es.shape.Overlaps(shape);
                return false;
            }
            else if (baseShapei is ExtrudedBoxi)
                return baseShapei.Overlaps(this);
            else
                throw new System.NotImplementedException();
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
            return shape.Points;
        }

        public override IEnumerable<BaseShapei> Cut(BaseShapei s)
        {
            if (s is ExtrudedShapei)
            {
                ExtrudedShapei es = (ExtrudedShapei)s;
                if (!heightInterval.Overlaps(es.heightInterval))
                    yield return this;
                else
                {
                    Tuple<IntervaliList, IntervaliList> cut = heightInterval.Cut(es.heightInterval);
                    foreach (Intervali i in cut.Item1)
                        yield return new ExtrudedShapei(shape, i);
                    foreach (Shapei sh in shape.Cut(es.shape))
                        foreach (Intervali i in cut.Item2)
                            yield return new ExtrudedShapei(sh, i);
                }
            }
        }

        public override Vec3i GetClosestPointTo(Vec3i pos)
        {
            Vec2i closestPoint2D = shape.GetClosestPointTo(new Vec2i(pos.X, pos.Z));
            return new Vec3i(closestPoint2D.X, heightInterval.GetClosestValueTo(pos.Y), closestPoint2D.Y);
        }
    }

    public class ExtrudedShape : BaseShape
    {
        Shape shape;
        Interval heightInterval;

        public Shape Shape { get { return shape; } }
        public Interval HeightInterval { get { return heightInterval; } }

        public override float Height
        {
            get { return heightInterval.Length; }
        }

        public override Vec3 MidPointOnFloor
        {
            get
            {
                Vec3 vec = (Vec3)shape.MidPoint;
                vec.Y = heightInterval.Min;
                return vec;
            }
        }

        public override Vec3 MidPoint
        {
            get
            {
                Vec3 vec = (Vec3)shape.MidPoint;
                vec.Y = heightInterval.Mid;
                return vec;
            }
        }

        public override float HeightToFloor
        {
            get { return heightInterval.Min; }
        }

        public ExtrudedShape(Shapei shape, Intervali heightInterval, FromIntegerConversion c)
        {
            this.shape = Convert(shape, c);
            this.heightInterval = Convert(heightInterval, c);
        }

        public ExtrudedShape(Shape shape, float positionY, float height)
        {
            this.shape = new Shape(shape.Points);
            heightInterval = new Interval(positionY, positionY + height);
            if (this.heightInterval.Min < 0 && this.heightInterval.Min > -0.01f)
                this.heightInterval.Min = 0;
        }

        public ExtrudedShape(Box box)
            : this(new Shape(new Box2((Vec2)box.Minimum, (Vec2)box.Maximum)),
                      box.Minimum.Y, box.Dimensions.Y)
        {
            if (this.heightInterval.Min < 0 && this.heightInterval.Min > -0.01f)
                this.heightInterval.Min = 0;
        }

        public override bool Overlaps(BaseShape shape)
        {
            if (shape is Point)
            {
                Point p = (Point)shape;
                if (!heightInterval.InInterval(p.PositionY))
                    return false;
                return this.shape.Contains(p.Pnt);
            }
            if (shape is ExtrudedLine)
            {
                ExtrudedLine el = (ExtrudedLine)shape;
                if (!heightInterval.Overlaps(el.HeightInterval))
                    return false;
                if (this.shape.Contains(el.Line.P1) || this.shape.Contains(el.Line.P2))
                    return true;
                foreach (Line2 l in this.shape.GetLines())
                    if (Line2.IntersectionOnBothLines(l, el.Line))
                        return true;
                return false;
            }
            else if (shape is FlatShape)
            {
                FlatShape fs = (FlatShape)shape;
                if (!heightInterval.InInterval(fs.PositionY))
                    return false;
                if (!this.shape.BoundingBox.Overlaps(fs.Shape.BoundingBox))
                    return false;
                return this.shape.Overlaps(fs.Shape);
            }
            else if (shape is ExtrudedShape)
            {
                ExtrudedShape es = (ExtrudedShape)shape;
                if (!heightInterval.Overlaps(es.heightInterval))
                    return false;
                if (!this.shape.BoundingBox.Overlaps(es.shape.BoundingBox))
                    return false;
                return this.shape.Overlaps(es.shape);
            }

            throw new NotImplementedException();
        }

        public override BaseShape Transform(Matrix4 transformation)
        {
            Vec3 temp = new Vec3(0, 0, 0);
            temp = temp * transformation;
            return new ExtrudedShape(shape.TransformedShape(transformation), heightInterval.Min + temp.Y, heightInterval.Length);
        }

        public override Node CreateNode(Material material)
        {
            Mesh m = SimpleShapes.CreateExtruded2DShape(shape.Points, heightInterval.Length, 1, 1, new Vec4(1, 1, 1), material, true);
            m.Position = new Vec3(m.Position.X, heightInterval.Min, m.Position.Z);
            return m;
        }

        internal List<BaseShape> Merge(BaseShape b)
        {
            List<BaseShape> ret = new List<BaseShape>();
            if (b == null)
            {
                List<BaseShape> bs = new List<BaseShape>();
                bs.Add(new ExtrudedShape(shape.Copy(), this.heightInterval.Min, this.heightInterval.Length));
                return bs;
            }
            else if (b is ExtrudedShape)
            {
                ExtrudedShape es = (ExtrudedShape)b;
                Interval newInterval = heightInterval.Merge(es.heightInterval);
                if (newInterval != null)
                {
                    CompoundShape cs = this.shape.PerformGPCOperation(es.shape, GpcWrapper.GpcOperation.Intersection);
                    foreach (Shape s in cs.Shapes)
                        ret.Add(new ExtrudedShape(s, newInterval.Min, newInterval.Length));
                }
            }
            else
                return BaseShape.Merge(this, b);
            return ret;
        }

        public override string ToString()
        {
            return "Shape '" + shape.ToString() + "' in " + heightInterval.ToString();
        }

        public override Box GetBoundingBox()
        {
            Box b = new Box(Vec3.Max, Vec3.Min);
            bool first = true;
            foreach (Vec2 v in shape.Points)
            {
                if (first)
                    b.AddPointToBoundingBox(new Vec3(v.X, heightInterval.Max, v.Y));
                else
                    b.AddPointToBoundingBox(new Vec3(v.X, heightInterval.Min, v.Y));
                first = false;
            }
            return b;
        }

        public override void Save(BinaryWriter w)
        {
            w.Write("ExtrudedShape");
            this.heightInterval.Save(w);
            this.shape.Save(w);
        }

        public ExtrudedShape(BinaryReader r)
        {
            this.heightInterval = new Interval(r);
            this.shape = Shape.Load(r);
            if (this.heightInterval.Min < 0 && this.heightInterval.Min > -0.01f)
                this.heightInterval.Min = 0;
        }

        public override string CreateFeatureDefinition()
        {
            string temp = "ExtrudedShape(";

            foreach (Vec2 v in shape.Points)
                temp += v.X.ToString() + ", " + v.Y.ToString() + ", ";

            temp += heightInterval.Min.ToString() + ", " + heightInterval.Length.ToString();

            temp += ")";
            return temp;
        }

        public override BaseShape Clone()
        {
            return new ExtrudedShape(this.shape, this.heightInterval.Min, this.heightInterval.Length);
        }

        public override BaseShape Offset(float p)
        {
            Shape offsetShape = shape.CreateOffsetShape((float)Math.Abs(p), p < 0);
            return new ExtrudedShape(offsetShape, this.heightInterval.Min, this.heightInterval.Length);
        }

        public override float DistanceToPoint(Vec3 point)
        {
            float vertDist = heightInterval.InInterval(point.Y) ? 0 : (float)Math.Min(Math.Abs(point.Y - heightInterval.Min), Math.Abs(heightInterval.Max - point.Y));
            float ret = 0;
            Vec2 v = point;
            if (shape.Contains(v))
                ret = vertDist;
            else
            {
                float horDist = shape.DistanceToPoint(v);
                if (vertDist > 0)
                    ret = (float)Math.Sqrt(vertDist * vertDist + horDist * horDist);
                else
                    ret = horDist;
            }
            return ret;
        }

        public override BaseShapei ConvertToInteger(BaseShapei.ToIntegerConversion c)
        {
            return new ExtrudedShapei(shape, heightInterval, c);
        }

        public override void Translate(Vec3 translation)
        {
            shape.Translate(translation);
            heightInterval.Move(translation.Y);
        }
    }
}
