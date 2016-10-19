using System;
using System.Collections.Generic;

using System.Text;
using System.Collections.ObjectModel;

namespace Common.Geometry
{
    public class LoDNode
    {
        public struct LoDChild
        {
            public readonly Interval interval;
            public readonly Object obj;

            internal LoDChild(float min, float max, Object obj)
            {
                this.interval = new Interval(min, max);
                this.obj = obj;
            }
        }

        List<LoDChild> children = new List<LoDChild>();
        Box boundingBox;

        public ReadOnlyCollection<LoDChild> Children { get { return children.AsReadOnly(); } }
        public double BoundingSphereRadius
        {
            get
            {
                return 0.5 * (boundingBox.Maximum - boundingBox.Minimum).length();
            }
        }
        public Vec3 Midpoint { get { return boundingBox.Midpoint; } }

        public LoDNode()
        {
            boundingBox = new Box(new Vec3(Vec3.Max), new Vec3(Vec3.Min));
        }

        public void AddChild(float min, float max, Object obj)
        {
            Box b = new Box(new Vec3(Vec3.Max), new Vec3(Vec3.Min));
            obj.GetBoundingBoxInternal(Matrix4.Identity, b);
            boundingBox.AddPointToBoundingBox(b.Minimum);
            boundingBox.AddPointToBoundingBox(b.Maximum);
            children.Add(new LoDChild(min, max, obj));
        }

        public void AddChild(float min, float max, Mesh mesh)
        {
            Box b = new Box(new Vec3(Vec3.Max), new Vec3(Vec3.Min));
            mesh.GetBoundingBoxInternal(Matrix4.Identity, b);
            boundingBox.AddPointToBoundingBox(b.Minimum);
            boundingBox.AddPointToBoundingBox(b.Maximum);
            Object obj = new Object();
            obj.AddNode(mesh);
            children.Add(new LoDChild(min, max, obj));
        }
    }
}
