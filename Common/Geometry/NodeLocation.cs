using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Geometry
{
    public class NodeLocation : AbstractNode
    {
        internal Vec3 CreateOrigin()
        {
            Vec3 origin = new Vec3();
            return origin * GetTransformation();
        }

        public NodeLocation(Vec3 translation)
        {
            Position = translation;
        }

        public NodeLocation(Vec3 translation, Vec3 rotation)
            : this(translation)
        {
            Rotation = rotation;
        }

        public NodeLocation(Vec3 translation, Vec3 rotation, Vec3 scalation)
            : this(translation, rotation)
        {
            Scalation = scalation;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
