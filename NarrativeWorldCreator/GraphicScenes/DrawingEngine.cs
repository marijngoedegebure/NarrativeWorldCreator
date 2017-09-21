using Common;
using Common.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.GraphicScenes
{
    public static class DrawingEngine
    {

        public static List<VertexPositionColor> GetDrawableTriangles(List<Vec2d> vertices, Color color)
        {
            List<VertexPositionColor> ret = new List<VertexPositionColor>();
            // Triangle 1 of rectanle
            ret.Add(new VertexPositionColor(new Vector3((float)vertices[3].X, (float)vertices[3].Y, 0), color));
            ret.Add(new VertexPositionColor(new Vector3((float)vertices[0].X, (float)vertices[0].Y, 0), color));
            ret.Add(new VertexPositionColor(new Vector3((float)vertices[1].X, (float)vertices[1].Y, 0), color));

            // Triangle 2 of rectangle
            ret.Add(new VertexPositionColor(new Vector3((float)vertices[3].X, (float)vertices[3].Y, 0), color));
            ret.Add(new VertexPositionColor(new Vector3((float)vertices[1].X, (float)vertices[1].Y, 0), color));
            ret.Add(new VertexPositionColor(new Vector3((float)vertices[2].X, (float)vertices[2].Y, 0), color));

            return ret;
        }

        public static List<VertexPositionColor> GetVPCForTrianleStrip(TriStrip triStrip, Color color)
        {
            var ret = new List<VertexPositionColor>();
            var strips = triStrip.Strips.ToList();
            for(int i = 0; i < triStrip.Count; i++)
            {
                var strip = strips[i].ToList();
                for(int j = 0; j < strip.Count; j++)
                {
                    ret.Add(new VertexPositionColor(new Vector3((float)strip[j].X, (float)strip[j].Y, 0), color));
                }
            }

            return ret;
        }
    }
}
