using System;
using System.Collections.Generic;

using System.Text;
using Common.Geometry;
using Common;
using System.IO;

namespace Common.Shapes
{
    public class FlatShapei : BaseShapei, IFlatShapei
    {
        Shapei shape;
        int positionY;

        public Shapei Shape { get { return shape; } }
        public int PositionY { get { return positionY; } set { positionY = value; } }

        public FlatShapei(Shapei shape, int positionY)
        {
            this.shape = shape;
            this.positionY = positionY;
        }

        public FlatShapei(Shape shape, float positionY, ToIntegerConversion c)
        {
            this.shape = BaseShapei.Convert(shape, c);
            this.positionY = c(positionY);
        }

        public override BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c)
        {
            return new FlatShape(shape, positionY, c);
        }

        public override Boxi GetBoundingBox()
        {
            return Boxi.CreateFromPointList(shape.Points, positionY);
        }

        public override BaseShapei.EType Type
        {
            get { return EType.FlatShape; }
        }

        protected override IFlatShapei GetFlatShape()
        {
            return new FlatShapei(shape, 0);
        }

        #region IFlatShapei Members


        public IExtrudedShapei Extrude(Intervali heightInterval)
        {
            return new ExtrudedShapei(shape, heightInterval);
        }

        #endregion

        public override BaseShapei Transform(Matrix4 matrix4)
        {
            Vec3 temp = new Vec3(0, 0, 0);
            temp = temp * matrix4;
            List<Vec2i> newPoints = new List<Vec2i>(shape.Points.Count);
            foreach (Vec2i p in shape.Points)
                newPoints.Add(Convert(p, matrix4));
            return new FlatShapei(new Shapei(newPoints), positionY + (int)Math.Round(temp.Y));
        }

        public override BaseShapei Transform(BaseShapei.BasicTransformation baseTransform)
        {
            List<Vec2i> newPoints = new List<Vec2i>(shape.Points.Count);
            foreach (Vec2i p in shape.Points)
                newPoints.Add(Transform(p, baseTransform));
            return new FlatShapei(new Shapei(newPoints), positionY);
        }

        public override BaseShapei Translate(Vec3i vec3i)
        {
            Vec2i move = new Vec2i(vec3i.X, vec3i.Z);
            List<Vec2i> newPoints = new List<Vec2i>(shape.Points.Count);
            foreach (Vec2i p in shape.Points)
                newPoints.Add(p + move);
            return new FlatShapei(new Shapei(newPoints), positionY + vec3i.Y);
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
            return shape.Points;
        }

        public override IEnumerable<BaseShapei> Cut(BaseShapei s)
        {
            ExtrudedShapei es;
            FlatShapei fs;
            switch (s.Type)
            {
                case EType.ExtShape:
                    es = (ExtrudedShapei)s;
                    if (es.HeightInterval.InInterval(positionY))
                    {
                        foreach (Shapei shape in this.shape.Cut(es.Shape))
                            yield return new FlatShapei(shape, positionY);
                    }
                    else
                        yield return this;
                    break;
                case EType.FlatShape:
                    fs = (FlatShapei)s;
                    if (this.positionY == fs.positionY)
                    {
                        foreach (Shapei shape in this.shape.Cut(fs.shape))
                            yield return new FlatShapei(shape, positionY);
                    }
                    else
                        yield return this;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override Vec3i GetClosestPointTo(Vec3i pos)
        {
            Vec2i closestPoint2D = shape.GetClosestPointTo(new Vec2i(pos.X, pos.Z));
            if (closestPoint2D == null)
                return null;
            return new Vec3i(closestPoint2D.X, positionY, closestPoint2D.Y);
        }
    }

    public class FlatShape : BaseShape
    {
        Shape shape;
        float positionY;

        public float PositionY { get { return positionY; } }
        public Shape Shape { get { return shape; } }

        public override float Height
        {
            get { return 0; }
        }
        public override Vec3 MidPointOnFloor
        {
            get { Vec3 v = shape.MidPoint; v.Y = positionY; return v; }
        }

        public override Vec3 MidPoint
        {
            get
            {
                return MidPointOnFloor;
            }
        }

        public override float HeightToFloor
        {
            get { return PositionY; }
        }

        public FlatShape(Shapei shape, int positionY, FromIntegerConversion c)
        {
            this.shape = Convert(shape, c);
            this.positionY = c(positionY);
        }

        public FlatShape(Shape shape, float positionY)
        {
            this.shape = new Shape(shape.Points);
            this.positionY = positionY;
        }

        public override bool Overlaps(BaseShape shape)
        {
            if (shape is ExtrudedLine)
            {
                ExtrudedLine el = (ExtrudedLine)shape;
                if (!el.HeightInterval.InInterval(positionY))
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
                if (positionY != fs.positionY)
                    return false;
                if (!this.shape.BoundingBox.Overlaps(fs.shape.BoundingBox))
                    return false;
                return this.shape.Overlaps(fs.shape);
            }
            else if (shape is ExtrudedShape)
                return shape.Overlaps(this);
            else if (shape is Point) // [RMS] added Point overlap check for the Inside constraint
            {
                return this.shape.Contains((shape as Point).Pnt);
            } 
            else 
            {
                throw new NotImplementedException();
            }
        }

        public override BaseShape Transform(Common.Matrix4 transformation)
        {
            Vec3 temp = new Vec3(0, 0, 0);
            temp = temp * transformation;

            return new FlatShape(shape.TransformedShape(transformation), positionY + temp.Y);
        }

        public override Node CreateNode(Material material)
        {
            List<List<Vec2>> triangleStripLists = shape.CreateTriangleStripList();
            Common.Geometry.Object obj = new Common.Geometry.Object();
            if (triangleStripLists.Count == 1)
                obj.AddNode(SimpleShapes.CreateMeshFrom2DTriangles(triangleStripLists[0], 1, 1, new Vec4(1, 1, 1), material));
            else
            {
                foreach(List<Vec2> triStrip in triangleStripLists)
                    obj.AddNode(SimpleShapes.CreateMeshFrom2DTriangles(triStrip, 1, 1, new Vec4(1, 1, 1), material));
            }
            obj.Position = new Vec3(obj.Position.X, positionY, obj.Position.Z);
            return obj;
        }

        internal List<BaseShape> Merge(BaseShape b)
        {
            if (b == null)
            {
                List<BaseShape> bs = new List<BaseShape>();
                bs.Add(new FlatShape(shape.Copy(), positionY));
                return bs;
            }
            else if (b is ExtrudedPoint)
                return Merge(b, this);
            else if (b is ExtrudedShape)
            {
                ExtrudedShape es = (ExtrudedShape)b;
                if (es.HeightInterval.InInterval(this.positionY))
                {
                    CompoundShape intersection = this.shape.PerformGPCOperation(es.Shape, GpcWrapper.GpcOperation.Intersection);
                    List<BaseShape> ret = new List<BaseShape>();
                    foreach (Shape s in intersection.Shapes)
                        ret.Add(new FlatShape(s, this.positionY));
                    return ret;
                }
                else
                    return new List<BaseShape>();
            }
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Shape '" + shape.ToString() + "' on height " + positionY;
        }

        public override Box GetBoundingBox()
        {
            Box b = new Box(Vec3.Max, Vec3.Min);
            foreach (Vec2 v in shape.Points)
                    b.AddPointToBoundingBox(new Vec3(v.X, positionY, v.Y));
            return b;
        }

        public override void Save(BinaryWriter w)
        {
            w.Write("FlatShape");
            w.Write((double)this.positionY);
            this.shape.Save(w);
        }

        public FlatShape(BinaryReader r)
        {
            this.positionY = (float)r.ReadDouble();
            this.shape = Shape.Load(r);
        }

        public override string CreateFeatureDefinition()
        {
            string temp = "FlatShape(";

            foreach (Vec2 v in shape.Points)
                temp += v.X.ToString() + ", " + v.Y.ToString() + ", ";

            temp += positionY.ToString();

            temp += ")";
            return temp;
        }

        public override BaseShape Clone()
        {
            return new FlatShape(this.shape, this.positionY);
        }

        public override BaseShape Offset(float p)
        {
            Shape offsetShape = shape.CreateOffsetShape((float)Math.Abs(p), p < 0);
            return new FlatShape(offsetShape, positionY);
        }

        public override float DistanceToPoint(Vec3 point)
        {
            float vertDist = (float)Math.Abs(point.Y - positionY);
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

        protected override BaseShape[] SplitInternal(Vec3.Component component, int nrOfSplits)
        {
            if (component == Vec3.Component.Y)
            {
                return new BaseShape[] { new FlatShape(this.shape, positionY) };
            }
            else
            {
                Vec2.Component comp2 = Vec2.Component.X;
                Vec2.Component comp2b = Vec2.Component.Y;
                if (component == Vec3.Component.Z)
                {
                    comp2 = Vec2.Component.Y;
                    comp2b = Vec2.Component.X;
                }
                Box2 bb = this.shape.BoundingBox;
                float x = bb.Dimensions.GetComponent(comp2);
                float start = bb.min.GetComponent(comp2);
                float disp = x / (float)(nrOfSplits + 1);
                BaseShape[] t = new BaseShape[nrOfSplits + 1];
                int count = 0;
                Common.Geometry.Shape ss = new Shape(shape.Points);
                for (int i = 0; i < nrOfSplits; ++i)
                {
                    Vec2 s = new Vec2(), e = new Vec2();
                    s.SetComponent(comp2b, bb.min.GetComponent(comp2b));
                    e.SetComponent(comp2b, bb.max.GetComponent(comp2b));
                    s.SetComponent(comp2, start + disp * (float)(i + 1));
                    e.SetComponent(comp2, start + disp * (float)(i + 1));
                    Line2 splitLine = new Line2(s, e);
                    List<Shape> shapes = ss.SplitAlong(splitLine);
                    if (shapes.Count != 2)
                        throw new Exception("Ik weet ook niet wat we hiermee moeten aanvangen!");
                    if (shapes[0].BoundingBox.Dimensions.GetComponent(comp2) < shapes[1].BoundingBox.Dimensions.GetComponent(comp2))
                    {
                        t[count++] = new FlatShape(shapes[0], positionY);
                        ss = shapes[1];
                    }
                    else
                    {
                        t[count++] = new FlatShape(shapes[1], positionY);
                        ss = shapes[0];
                    }
                }
                t[count] = new FlatShape(ss, positionY);
                return t;
            }
        }

        protected override BaseShape ExtrudeInternal(float height)
        {
            return new ExtrudedShape(this.shape, this.positionY, height);
        }

        public override BaseShapei ConvertToInteger(BaseShapei.ToIntegerConversion c)
        {
            return new FlatShapei(shape, positionY, c);
        }

        public override void Translate(Vec3 translation)
        {
            shape.Translate(translation);
            positionY += translation.Y;
        }
    }
}
