using System;
using System.Collections.Generic;
using System.Text;
using Common;
using System.IO;

namespace Common.Shapes
{
    public class ShapeGroupi : BaseShapei
    {
        List<BaseShapei> shapes = new List<BaseShapei>();
        Boxi boundingBox = Boxi.MaxMinBox();

        public ShapeGroupi(List<BaseShape> shapes, ToIntegerConversion c)
        {
            foreach (BaseShape bs in shapes)
            {
                BaseShapei bsi = bs.ConvertToInteger(c);
                this.shapes.Add(bsi);
                boundingBox.AddBox(bsi.GetBoundingBox());
            }
        }

        public ShapeGroupi(List<BaseShapei> shapes)
        {
            this.shapes = shapes;
        }

        public override BaseShape ConvertFromInteger(BaseShape.FromIntegerConversion c)
        {
            return new ShapeGroup(this.shapes, c);
        }

        public override Boxi GetBoundingBox()
        {
            return boundingBox;
        }

        public override BaseShapei.EType Type
        {
            get { return EType.Group; }
        }

        protected override IFlatShapei GetFlatShape()
        {
            throw new NotImplementedException();
        }

        public override BaseShapei Transform(Matrix4 matrix4)
        {
            List<BaseShapei> newGroup = new List<BaseShapei>();
            foreach (BaseShapei bsi in shapes)
                newGroup.Add(bsi.Transform(matrix4));
            return new ShapeGroupi(shapes);
        }

        public override BaseShapei Transform(BaseShapei.BasicTransformation baseTransform)
        {
            List<BaseShapei> newGroup = new List<BaseShapei>();
            foreach (BaseShapei bsi in shapes)
                newGroup.Add(bsi.Transform(baseTransform));
            return new ShapeGroupi(shapes);
        }

        public override BaseShapei Translate(Vec3i vec3i)
        {
            List<BaseShapei> newGroup = new List<BaseShapei>();
            foreach (BaseShapei bsi in shapes)
                newGroup.Add(bsi.Translate(vec3i));
            return new ShapeGroupi(shapes);
        }

        public override bool Overlaps(BaseShapei baseShapei)
        {
            throw new NotImplementedException();
        }

        public override int MinY
        {
            get { return boundingBox.Min.Y; }
        }

        public override int MaxY
        {
            get { return boundingBox.Max.Y; }
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
            Vec3i closestPoint = null;
            float closestDistSquared = float.MaxValue;
            foreach (BaseShapei bs in this.shapes)
            {
                Vec3i p = bs.GetClosestPointTo(pos);
                float dist = (p - pos).squareLength();
                if (dist < closestDistSquared)
                {
                    closestDistSquared = dist;
                    closestPoint = p;
                }
            }
            return closestPoint;
        }
    }

    public class ShapeGroup : BaseShape
    {
        List<BaseShape> shapes = new List<BaseShape>();
        Box boundingBox = new Box(Vec3.Max, Vec3.Min);

        public ShapeGroup(List<BaseShape> shapes)
        {
            foreach (BaseShape bs in shapes)
            {
                this.shapes.Add(bs.Clone());
                boundingBox.AddBox(bs.GetBoundingBox());
            }
        }

        public ShapeGroup(List<BaseShapei> shapes, FromIntegerConversion c)
        {
            foreach (BaseShapei bsi in shapes)
            {
                BaseShape bs = bsi.ConvertFromInteger(c);
                this.shapes.Add(bs);
                boundingBox.AddBox(bs.GetBoundingBox());
            }
        }

        public override bool Overlaps(BaseShape shape)
        {
            foreach (BaseShape s in shapes)
                if (s.Overlaps(shape))
                    return true;
            return false;
        }

        public override BaseShape Transform(Common.Matrix4 transformation)
        {
            List<BaseShape> copy = new List<BaseShape>();
            foreach (BaseShape s in shapes)
                copy.Add(s.Transform(transformation));
            return new ShapeGroup(copy);
        }

        public override Common.Geometry.Node CreateNode(Common.Geometry.Material material)
        {
            Common.Geometry.Object obj = new Common.Geometry.Object("group");
            foreach (BaseShape s in shapes)
                obj.AddNode(s.CreateNode(material));
            return obj;
        }

        public override Common.Vec3 MidPointOnFloor
        {
            get { Vec3 v = boundingBox.Midpoint; v.Y = boundingBox.Minimum.Y; return v; }
        }

        public override Common.Vec3 MidPoint
        {
            get { return boundingBox.Midpoint; }
        }

        public override float Height
        {
            get { return boundingBox.Dimensions.Y; }
        }

        public override float HeightToFloor
        {
            get { return boundingBox.Minimum.Y; }
        }

        public override Common.Box GetBoundingBox()
        {
            return boundingBox;
        }

        public override void Save(BinaryWriter w)
        {
            w.Write("ShapeGroup");
            w.Write(shapes.Count);
        }

        public override string CreateFeatureDefinition()
        {
            throw new NotImplementedException();
        }

        public ShapeGroup(BinaryReader r)
        {
            int count = r.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                BaseShape shape = BaseShape.Load(r);
                this.shapes.Add(shape);
                boundingBox.AddBox(shape.GetBoundingBox());
            }
        }

        public override BaseShape Clone()
        {
            List<BaseShape> clones = new List<BaseShape>();
            foreach (BaseShape s in shapes)
                clones.Add(s.Clone());
            return new ShapeGroup(clones);
        }

        public override BaseShape Offset(float p)
        {
            List<BaseShape> shapes = new List<BaseShape>();
            foreach(BaseShape bs in this.shapes)
                shapes.Add(bs.Offset(p));
            return new ShapeGroup(shapes);
        }

        public override float DistanceToPoint(Vec3 point)
        {
            float minDist = float.MaxValue;
            foreach (BaseShape bs in shapes)
            {
                float dist = bs.DistanceToPoint(point);
                if (minDist > dist)
                    minDist = dist;
            }
            return minDist;
        }

        public override BaseShapei ConvertToInteger(BaseShapei.ToIntegerConversion c)
        {
            return new ShapeGroupi(this.shapes, c);
        }

        public override void Translate(Vec3 translation)
        {
            foreach (BaseShape shape in shapes)
                shape.Translate(translation);
        }
    }
}
