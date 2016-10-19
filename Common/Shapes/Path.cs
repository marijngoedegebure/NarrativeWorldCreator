using System;
using System.Collections.Generic;

using System.Text;
using Common;
using System.Collections.ObjectModel;
using System.IO;

namespace Common.Shapes
{
    public class Pathi : BaseShapei
    {
        List<Vec3i> points = new List<Vec3i>();
        Boxi boundingBox = Boxi.MaxMinBox();

        public Pathi(List<Vec3> points, ToIntegerConversion c)
        {
            foreach (Vec3 v in points)
            {
                Vec3i vi = BaseShapei.Convert(v, c);
                boundingBox.AddPointToBoundingBox(vi);
                this.points.Add(vi);
            }
        }

        public override BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c)
        {
            return new Path(this.points, c);
        }

        public override Boxi GetBoundingBox()
        {
            return boundingBox;
        }

        public float TotalLength()
        {
            float total = 0;
            for (int i = 1; i < points.Count; ++i)
                total += (points[i] - points[i - 1]).length();
            return total;
        }

        public override BaseShapei.EType Type
        {
            get { return EType.Path; }
        }

        protected override IFlatShapei GetFlatShape()
        {
            throw new NotImplementedException();
        }

        public override BaseShapei Transform(Matrix4 matrix4)
        {
            throw new NotImplementedException();
        }

        public override BaseShapei Transform(BaseShapei.BasicTransformation baseTransform)
        {
            throw new NotImplementedException();
        }

        public override BaseShapei Translate(Vec3i vec3i)
        {
            throw new NotImplementedException();
        }

        public override bool Overlaps(BaseShapei baseShapei)
        {
            throw new NotImplementedException();
        }

        public override int MinY
        {
            get { throw new NotImplementedException(); }
        }

        public override int MaxY
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable<Vec2i> GetPoints2D()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<BaseShapei> Cut(BaseShapei s)
        {
            throw new NotImplementedException();
        }

        public override Vec3i GetClosestPointTo(Vec3i pos)
        {
            throw new NotImplementedException();
        }
    }

    public class Path : BaseShape
    {
        List<Vec3> points = new List<Vec3>();
        Box boundingBox = new Box(Vec3.Max, Vec3.Min);

        public ReadOnlyCollection<Vec3> Points { get { return points.AsReadOnly(); } }

        public Path()
        {
        }

        public Path(List<Vec3i> points, FromIntegerConversion c)
        {
            foreach (Vec3i vi in points)
            {
                Vec3 v = Convert(vi, c);
                boundingBox.AddPointToBoundingBox(v);
                this.points.Add(v);
            }
        }

        public Path(params Vec3[] points)
        {
            foreach (Vec3 p in points)
                AddPoint(new Vec3(p));
        }

        public Path(params float[] points)
        {
            for (int i = 0; (i + 2) < points.Length; i += 3)
                AddPoint(new Vec3(points[i], points[i + 1], points[i + 2]));
        }

        public Path(Path copy)
            : this(copy.points)
        {
        }

        public Path(List<Vec3> points)
        {
            foreach (Vec3 p in points)
                AddPoint(new Vec3(p));
        }

        public void AddPoint(Vec3 v)
        {
            this.points.Add(new Vec3(v));
            boundingBox.AddPointToBoundingBox(v);
        }

        public override bool Overlaps(BaseShape shape)
        {
            throw new NotImplementedException();
        }

        public override BaseShape Transform(Matrix4 transformation)
        {
            List<Vec3> newPoints = new List<Vec3>();
            foreach (Vec3 v in points)
                newPoints.Add(v * transformation);
            return new Path(newPoints);
        }

        public override Common.Geometry.Node CreateNode(Common.Geometry.Material material)
        {
            throw new NotImplementedException();
        }

        public override Vec3 MidPointOnFloor
        {
            get { Vec3 vec = new Vec3(boundingBox.Midpoint); vec.Y = boundingBox.Minimum.Y; return vec; }
        }

        public override Vec3 MidPoint
        {
            get
            {
                return boundingBox.Midpoint;
            }
        }

        public override float Height
        {
            get { return boundingBox.Dimensions.Y; }
        }

        public override float HeightToFloor
        {
            get { return boundingBox.Minimum.Y; }
        }

        public override Box GetBoundingBox()
        {
            return boundingBox;
        }

        public float TotalLength()
        {
            float total = 0;
            for (int i = 1; i < points.Count; ++i)
                total += (points[i] - points[i - 1]).length();
            return total;
        }

        public void Move(Vec3 move)
        {
            foreach (Vec3 v in points)
                v.Move(move);
        }

        public List<Line> GetLines()
        {
            List<Line> lines = new List<Line>();
            for (int i = 0; i + 1 < points.Count; ++i)
                lines.Add(new Line(points[i], points[i + 1]));
            return lines;
        }

        internal List<BaseShape> Merge(BaseShape b)
        {
            List<BaseShape> shapes = new List<BaseShape>();
            foreach (Line l in GetLines())
            {
                FlatLine f = new FlatLine(new Line2((Vec2)l.P1, (Vec2)l.P2), 0.5f * (l.P1.Y + l.P2.Y));
                shapes.AddRange(BaseShape.Merge(f, b));
            }
            return shapes;
        }

        public Vec3 GetPointAt(float start)
        {
            float temp = 0;
            foreach (Line l in GetLines())
            {
                float len = l.Length();
                if (start < temp + len)
                    return l.PointOnLine((start - temp) / len);
                else
                    temp += len;
            }
            return null;
        }

        public float Length()
        {
            float length = 0;
            foreach (Line l in GetLines())
                length += l.Length();
            return length;
        }

        public override void Save(BinaryWriter w)
        {
            w.Write("Path");
            w.Write(this.points.Count);
            foreach (Vec3 v in this.points)
                v.Save(w);
        }

        public Path(BinaryReader r)
        {
            int count = r.ReadInt32();
            for (int i = 0; i < count; ++i)
                AddPoint(Vec3.Load(r));
        }

        public override string CreateFeatureDefinition()
        {
            throw new NotImplementedException();
        }

        public override BaseShape Clone()
        {
            return new Path(this.points);
        }

        public override BaseShape Offset(float p)
        {
            throw new NotImplementedException();
        }

        public override float DistanceToPoint(Vec3 point)
        {
            throw new NotImplementedException();
        }

        public override BaseShapei ConvertToInteger(BaseShapei.ToIntegerConversion c)
        {
            return new Pathi(this.points, c);
        }

        public override void Translate(Vec3 translation)
        {
            foreach (Vec3 p in this.points)
                p.Translate(translation);
        }
    }
}
