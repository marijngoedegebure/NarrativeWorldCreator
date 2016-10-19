using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Common.Geometry
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexInfoArray
    {
        public VertexInfoSimple[] array;
        int initSize;
        int index;

        public VertexInfoArray(int initSize)
        {
            this.initSize = initSize;
            array = new VertexInfoSimple[initSize];
            this.index = 0;
        }

        public int NextIndex()
        {
            if (index == array.Length)
            {
                VertexInfoSimple[] newArray = new VertexInfoSimple[array.Length * 2];
                array.CopyTo(newArray, 0);
                array = newArray;
            }
            return index++; 
        }

        public void Add(VertexInfoSimple v)
        {
            int index = NextIndex();
            array[index] = v;
        }

        public int CurrentSize()
        {
            return index;
        }

        public void Clear()
        {
            index = 0;
            array = null;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexInfoSimple : IDisposable
    {
        public Vec3Simple m_position;
        public Vec2Simple m_textureUV, m_textureUV2;
        public Vec3Simple m_normal;
        public Vec4Simple m_color;

        public VertexInfoSimple(BinaryReader r)
        {
            this.m_color = new Vec4Simple(r);
            this.m_normal = new Vec3Simple(r);
            this.m_position = new Vec3Simple(r);
            this.m_textureUV = new Vec2Simple(r);
            this.m_textureUV2 = new Vec2Simple(r);
        }

        public VertexInfoSimple(VertexInfo v)
        {
            this.m_color = new Vec4Simple(v.m_color);
            this.m_normal = new Vec3Simple(v.m_normal);
            this.m_position = new Vec3Simple(v.m_position);
            this.m_textureUV = new Vec2Simple(v.m_textureUV);
            this.m_textureUV2 = new Vec2Simple(v.m_textureUV2);
        }

        public VertexInfoSimple(Vec3Simple pos, Vec2Simple tex, Vec2Simple tex2, Vec3Simple normal, Vec4Simple color)
        {
            this.m_position = pos;
            this.m_textureUV = tex;
            this.m_textureUV2 = tex2;
            this.m_normal = normal;
            this.m_color = color;
        }

        public VertexInfoSimple(Vec3 pos, Vec2 tex, Vec2 tex2, Vec3 normal, Vec4 color)
        {
            this.m_position = pos.Simple();
            this.m_textureUV = tex.Simple();
            this.m_textureUV2 = tex2.Simple();
            this.m_normal = normal.Simple();
            this.m_color = color.Simple();
        }

        public VertexInfoSimple(Vec3 pos, Vec2 tex, Vec3 normal, Vec4 color)
        {
            this.m_position = pos.Simple();
            this.m_textureUV = tex.Simple();
            this.m_textureUV2 = new Vec2Simple(0, 0);
            this.m_normal = normal.Simple();
            this.m_color = color.Simple();
        }

        public VertexInfoSimple(Vec3Simple pos, Vec2Simple tex, Vec3Simple normal, Vec4Simple color)
        {
            this.m_position = pos;
            this.m_textureUV = tex;
            this.m_textureUV2 = new Vec2Simple(0, 0);
            this.m_normal = normal;
            this.m_color = color;
        }

        public VertexInfoSimple(VertexInfoSimple copy)
        {
            this.m_position = copy.m_position;
            this.m_textureUV = copy.m_textureUV;
            this.m_textureUV2 = copy.m_textureUV2;
            this.m_normal = copy.m_normal;
            this.m_color = copy.m_color;
        }

        internal void Save(System.IO.BinaryWriter wr)
        {
            this.m_color.Save(wr);
            this.m_normal.Save(wr);
            this.m_position.Save(wr);
            this.m_textureUV.Save(wr);
            this.m_textureUV2.Save(wr);
        }

        internal void Transform(Matrix4 transformation, Matrix4 rotationTransformation)
        {
            this.m_position.Transform(transformation);
            this.m_normal.Transform(rotationTransformation);
            this.m_normal.Normalize();
        }

        #region IDisposable Members

        public void Dispose()
        {
            m_color.Dispose();
            m_normal.Dispose();
            m_position.Dispose();
            m_textureUV.Dispose();
            m_textureUV2.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    public class VertexInfo : IDisposable
    {
        public Vec3 m_position;
        public Vec2 m_textureUV, m_textureUV2;
        public Vec3 m_normal;
        public Vec4 m_color;

        public VertexInfo(VertexInfo copy)
        {
            m_position = new Vec3(copy.m_position);
            m_textureUV = new Vec2(copy.m_textureUV);
            m_textureUV2 = new Vec2(copy.m_textureUV2);
            m_normal = new Vec3(copy.m_normal);
            m_color = new Vec4(copy.m_color);
        }

        public VertexInfo(float posX, float posY, float posZ, float tU, float tV, float nX, float nY, float nZ, float colR, float colG, float colB, float colA)
        {
            m_position = new Vec3(posX, posY, posZ);
            m_textureUV = new Vec2(tU, tV);
            m_textureUV2 = new Vec2();
            m_normal = new Vec3(nX, nY, nZ);
            m_color = new Vec4(colR, colG, colB, colA);
        }

        public VertexInfo(float posX, float posY, float posZ, float tU, float tV, float nX, float nY, float nZ, Vec4 color)
        {
            m_position = new Vec3(posX, posY, posZ);
            m_textureUV = new Vec2(tU, tV);
            m_textureUV2 = new Vec2();
            m_normal = new Vec3(nX, nY, nZ);
            m_color = new Vec4(color);
        }

        public VertexInfo(Vec3 position, Vec2 textureUV, Vec3 normal, Vec4 color)
        {
            m_position = new Vec3(position);
            m_textureUV = new Vec2(textureUV);
            m_textureUV2 = new Vec2();
            m_normal = new Vec3(normal);
            m_color = new Vec4(color);
        }

        public VertexInfo(Vec3 position, Vec2 textureUV, Vec2 textureUV2, Vec3 normal, Vec4 color)
        {
            m_position = new Vec3(position);
            m_textureUV = new Vec2(textureUV);
            m_textureUV2 = new Vec2(textureUV2);
            m_normal = new Vec3(normal);
            m_color = new Vec4(color);
        }

        public VertexInfo(Vec3 position, Vec2 textureUV, Vec3 normal, Vec3 color) {
            m_position = new Vec3(position);
            m_textureUV = new Vec2(textureUV);
            m_normal = new Vec3(normal);
            m_color = new Vec4(color);
        }

        public static VertexInfo operator +(VertexInfo v1, VertexInfo v2)
        {
            return new VertexInfo(v1.m_position + v2.m_position, v1.m_textureUV + v2.m_textureUV, v1.m_textureUV2 + v2.m_textureUV2, v1.m_normal + v2.m_normal, v1.m_color + v2.m_color);
        }
        public static VertexInfo operator -(VertexInfo v1, VertexInfo v2)
        {
            return new VertexInfo(v1.m_position - v2.m_position, v1.m_textureUV - v2.m_textureUV, v1.m_textureUV2 - v2.m_textureUV2, v1.m_normal - v2.m_normal, v1.m_color - v2.m_color);
        }
        public static VertexInfo operator *(float d, VertexInfo v)
        {
            return new VertexInfo(d * v.m_position, d * v.m_textureUV, d * v.m_textureUV2, d * v.m_normal, d * v.m_color);
        }

        internal void Save(System.IO.BinaryWriter wr)
        {
            this.m_color.Save(wr);
            this.m_normal.Save(wr);
            this.m_position.Save(wr);
            this.m_textureUV.Save(wr);
            this.m_textureUV2.Save(wr);
        }

        internal VertexInfo(System.IO.BinaryReader br)
        {
            m_color = new Vec4(br);
            m_normal = new Vec3(br);
            m_position = new Vec3(br);
            m_textureUV = new Vec2(br);
            m_textureUV2 = new Vec2(br);
        }

        public static VertexInfo TexBasedOnPosVI(Vec3 position, Vec3 normal, Vec4 color, Vec2 wallDirection)
        {
            wallDirection.Normalize();
            Vec2 pos2d = new Vec2(position.X, position.Z);
            Line2 lineA = new Line2(pos2d, pos2d + wallDirection);
            Line2 lineB = new Line2(new Vec2(0, 0), new Vec2(0, 1));
            bool yAxis = true;
            if (Math.Abs(lineA.P1.X - lineA.P2.X) < 0.001)
            {
                lineB = new Line2(new Vec2(0, 0), new Vec2(1, 0));
                yAxis = false;
            }
            Vec2 cross;
            float dummy;
            Vec2.Intersection(lineA.P1, lineA.P2, lineB.P1, lineB.P2, out dummy, out cross);
            float texU = (pos2d - cross).length();
            if ((yAxis && pos2d.X < 0) || (!yAxis && pos2d.Y < 0))
                texU = -texU;
            Vec2 texUV = new Vec2(texU, position.Y);
            return new VertexInfo(position, texUV, texUV, normal, color);
        }

        public static VertexInfo TexBasedOnPosVIXZ(Vec3 position, Vec3 normal, Vec4 color)
        {
            Vec2 texUV = new Vec2(position.X, position.Z);
            return new VertexInfo(position, texUV, texUV, normal, color);
        }

        public override string ToString()
        {
            return "Vec @ " + m_position.ToString();
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.m_color.Dispose();
            this.m_normal.Dispose();
            this.m_position.Dispose();
            this.m_textureUV.Dispose();
            this.m_textureUV2.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
