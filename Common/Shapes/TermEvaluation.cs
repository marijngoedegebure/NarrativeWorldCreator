using System;
using System.Collections.Generic;

using System.Text;
using Common.Shapes;
using Common.Geometry;

namespace Common.Shapes
{
    public static class TermEvaluation
    {
        public static object Evaluate(string function, object[] terms, out bool found)
        {
            found = true;
            object ret = null;
            switch (function.ToLower())
            {
                case "offsetshape2d":
                    FlatShape shape = (FlatShape)terms[0];
                    float offset = (float)(double)terms[1];

                    Shape shape2d = shape.Shape;
                    Shape newShape2d = shape2d.CreateOffsetShape((float)Math.Abs(offset), offset < 0);

                    ret = new FlatShape(newShape2d, shape.PositionY);
                    break;
                case "bottomofshape3d":
                    ExtrudedShape eshape = (ExtrudedShape)terms[0];
                    return new FlatShape(eshape.Shape, eshape.HeightToFloor);
                default:
                    found = false;
                    break;
            }
            return ret;
        }
    }
}
