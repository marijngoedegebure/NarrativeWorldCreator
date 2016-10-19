using System;
using System.Collections.Generic;

using System.Text;
using Common.Shapes;

namespace Common.Geometry
{
    public class SimpleShapes
    {
        public enum BasePointRepresentation { Sphere, VerticalLine };

        static public Mesh CreateCylinder(uint segments, float radius, float topradius, float height, float wrapU, float wrapV, Vec4 color)
        {
            return CreateCylinder(segments, radius, topradius, height, wrapU, wrapV, false, false, color);
        }

        static public Mesh CreateCircle(uint segments, float radius, Vec3 midpoint, Material material, Vec4 color, bool flip)
        {
            Mesh mesh = new Mesh((int)segments + 2, material);
            mesh.smesh.vertexBuffer.array[mesh.smesh.vertexBuffer.NextIndex()] = new VertexInfoSimple(midpoint.Simple(), new Vec2Simple(0.5f, 0.5f), new Vec3Simple(0, 1, 0), color.Simple());
            float angle = 0;
            float angleStep = (float)((Math.PI * 2.0f) / (float)segments);
            for (int i = 0; i < segments + 1; ++i)
            {
                Vec3 newPosition = new Vec3(radius * (float)Math.Cos(angle), 0, radius * (float)Math.Sin(angle));
                mesh.smesh.vertexBuffer.array[mesh.smesh.vertexBuffer.NextIndex()] = new VertexInfoSimple(midpoint + newPosition, new Vec2(0.5f + (float)Math.Cos(angle) * 0.5f,
                                            0.5f + (float)Math.Sin(angle) * 0.5f), flip ? new Vec3(0, -1, 0) : new Vec3(0, 1, 0), color);
                angle = angle + angleStep;
            }
            int start = 1;
            for (int i = 0; i < segments; ++i)
            {
                mesh.m_indexBuffer.Add(0);
                if (flip)
                {
                    mesh.m_indexBuffer.Add(start + 0);
                    mesh.m_indexBuffer.Add(start + 1);
                }
                else
                {
                    mesh.m_indexBuffer.Add(start + 1);
                    mesh.m_indexBuffer.Add(start + 0);
                }
                start += 1;
            }
            return mesh;
        }

        static public Mesh CreateCylinder(uint segments, float radius, float topradius, float height, float wrapU, float wrapV, bool fillTop, bool fillBottom, Vec4 color)
        {
            int size = ((int)segments + 1) * 2;
            if (fillTop)
                size += (int)segments + 2;
            if (fillBottom)
                size += (int)segments + 2;
            Mesh mesh = new Mesh(size);

            float angleStep = (float)((Math.PI * 2.0) / (float)segments);
            float circum = 2.0f * (float)Math.PI * radius;

            float angle = 0;
            Vec3 normal = GetNormal(angle);
            Vec3 position = new Vec3(radius, 0, 0);
            Vec3 topposition = new Vec3(topradius, height, 0);

            for (int i = 0; i < segments + 1; ++i)
            {
                normal = GetNormal(angle);
                position = new Vec3(radius * normal.X, 0, radius * normal.Z);
                topposition = new Vec3(topradius * normal.X, height, topradius * normal.Z);

                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(position, new Vec2(angle / (float)Math.PI / 2 * wrapU * 100, 0), normal, color));
                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(topposition, new Vec2(angle / (float)Math.PI / 2 * wrapU * 100, height * wrapV * 100), normal, color));
                angle = angle + angleStep;
            }

            int start = 0;
            for (int i = 0; i < segments; ++i)
            {
                mesh.m_indexBuffer.Add(start + 0);
                mesh.m_indexBuffer.Add(start + 1);
                mesh.m_indexBuffer.Add(start + 2);

                mesh.m_indexBuffer.Add(start + 2);
                mesh.m_indexBuffer.Add(start + 1);
                mesh.m_indexBuffer.Add(start + 3);
                start += 2;
            }

            if (fillTop)
            {
                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(new Vec3(0, height, 0), new Vec2(0.5f, 0.5f), new Vec3(0, 1, 0), color));
                int centerIndex = mesh.smesh.vertexBuffer.CurrentSize() - 1;
                angle = 0;
                for (int i = 0; i < segments + 1; ++i)
                {
                    topposition = new Vec3(topradius * (float)Math.Cos(angle), height, topradius * (float)Math.Sin(angle));
                    mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(topposition, new Vec2(0.5f + (float)Math.Cos(angle) * 0.5f, 0.5f + (float)Math.Sin(angle) * 0.5f), new Vec3(0, 1, 0), color));
                    angle = angle + angleStep;
                }
                start = centerIndex + 1;
                for (int i = 0; i < segments; ++i)
                {
                    mesh.m_indexBuffer.Add(centerIndex);
                    mesh.m_indexBuffer.Add(start + 1);
                    mesh.m_indexBuffer.Add(start + 0);
                    start += 1;
                }
            }
            if (fillBottom)
            {
                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(new Vec3(0, 0, 0), new Vec2(0.5f, 0.5f), new Vec3(0, -1, 0), color));
                int centerIndex = mesh.smesh.vertexBuffer.CurrentSize() - 1;
                angle = 0;
                for (int i = 0; i < segments + 1; ++i)
                {
                    position = new Vec3(radius * (float)Math.Cos(angle), 0, radius * (float)Math.Sin(angle));
                    mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(position, new Vec2(0.5f + (float)Math.Cos(angle) * 0.5f, 0.5f + (float)Math.Sin(angle) * 0.5f), new Vec3(0, -1, 0), color));
                    angle = angle + angleStep;
                }
                start = centerIndex + 1;
                for (int i = 0; i < segments; ++i)
                {
                    mesh.m_indexBuffer.Add(centerIndex);
                    mesh.m_indexBuffer.Add(start + 0);
                    mesh.m_indexBuffer.Add(start + 1);
                    start += 1;
                }
            }

            return mesh;
        }

        //static public GPC.Vertex[] ReorderList(GPC.VertexList vlist)
        //{
        //    GPC.Vertex[] temp = new GPC.Vertex[vlist.Vertex.Length];
        //    int count = 0;
        //    temp[count++] = vlist.Vertex[0];

        //    for (int i = 1; i < vlist.Vertex.Length; i += 2)
        //        temp[count++] = vlist.Vertex[i];
        //    for (int i = vlist.Vertex.Length - 1; i > 1; i--)
        //        if (i % 2 == 0)
        //            temp[count++] = vlist.Vertex[i];
        //    return temp;
        //}

        //static public Mesh CreateExtruded2DShape(GPC.VertexList vertexlist, float height, float wrapU, float wrapV, Vec4 color)
        //{
        //    Mesh mesh = new Mesh((vertexlist.NofVertices + 1) * 2);

        //    Vec3 normal = new Vec3(1, 0, 0);
        //    Vec3 position = new Vec3(0, 0, 0);
        //    Vec3 topposition = new Vec3(0, height, 0);

        //    float tex = 0;

        //    GPC.Vertex[] vlist = ReorderList(vertexlist);

        //    for (int i = 0; i < vlist.Length + 1; ++i)
        //    {
        //        GPC.Vertex v1 = vlist[(i - 1 + vlist.Length) % vlist.Length];
        //        GPC.Vertex v2 = vlist[(i + 1) % vlist.Length];
        //        GPC.Vertex v = vlist[i % vlist.Length];

        //        normal = new Vec3((float)(v2.Y - v1.Y), 0, -(float)(v2.X - v1.X));
        //        float len = (new Vec2((float)(v2.X - v.X), (float)(v2.Y - v.Y))).length();
        //        normal.Normalize();
        //        position = new Vec3((float)v.X, 0, (float)v.Y);
        //        topposition = new Vec3((float)v.X, height, (float)v.Y);

        //        mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(position, new Vec2(tex * wrapU * 100, 0), normal, color));
        //        mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(topposition, new Vec2(tex * wrapU * 100, height * wrapV * 100), normal, color));

        //        tex += len;
        //    }

        //    int start = 0;
        //    for (int i = 0; i < vlist.Length; ++i)
        //    {
        //        mesh.m_indexBuffer.Add(start + 0);
        //        mesh.m_indexBuffer.Add(start + 2);
        //        mesh.m_indexBuffer.Add(start + 1);

        //        mesh.m_indexBuffer.Add(start + 2);
        //        mesh.m_indexBuffer.Add(start + 3);
        //        mesh.m_indexBuffer.Add(start + 1);
        //        start += 2;
        //    }

        //    return mesh;
        //}

        static public Mesh CreateExtruded2DShape(List<Vec2> vertexlist, float height, float wrapU,
                                                    float wrapV, Vec4 color, Material material, bool twoSided)
        {
            int size = (vertexlist.Count + 1) * 4;
            if (twoSided)
                size *= 2;
            Mesh mesh = new Mesh(size);
            mesh.Material = material;

            Vec3 normal = new Vec3(1, 0, 0);
            Vec3 position = new Vec3(0, 0, 0);
            Vec3 position2 = new Vec3(0, 0, 0);

            float tex = 0;

            Vec3 temp1 = vertexlist[1] - vertexlist[0];
            Vec3 temp2 = vertexlist[2] - vertexlist[0];
            Vec3 top = new Vec3(0, height, 0);
            temp1.Normalize();
            temp2.Normalize();
            if (temp1.Cross(temp2).Y < 0)
                vertexlist.Reverse();

            for (int i = 0; i < vertexlist.Count + 1; ++i)
            {
                //Vec2 v1 = vertexlist[(i - 1 + vertexlist.Count) % vertexlist.Count];
                Vec2 v2 = vertexlist[(i + 1) % vertexlist.Count];
                Vec2 v = vertexlist[i % vertexlist.Count];

                Vec2 cross = -(v2 - v).Cross();
                cross.Normalize();
                normal = cross;
                float len = (new Vec2(v2.X - v.X, v2.Y - v.Y)).length();
                position = new Vec3(v.X, 0, v.Y);
                position2 = new Vec3(v2.X, 0, v2.Y);

                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(position, new Vec2(tex * wrapU, 0), normal, color));
                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(position + top, new Vec2(tex * wrapU, height * wrapV), normal, color));
                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(position2 + top, new Vec2((tex + len) * wrapU, height * wrapV), normal, color));
                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(position2, new Vec2((tex + len) * wrapU, 0), normal, color));

                if (twoSided)
                {
                    mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(position, new Vec2(tex * wrapU, 0), -normal, color));
                    mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(position + top, new Vec2(tex * wrapU, height * wrapV), -normal, color));
                    mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(position2 + top, new Vec2((tex + len) * wrapU, height * wrapV), -normal, color));
                    mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(position2, new Vec2((tex + len) * wrapU, 0), -normal, color));
                }

                tex += len;
            }

            int start = 0;
            for (int i = 0; i < vertexlist.Count; ++i)
            {
                mesh.m_indexBuffer.Add(start + 0);
                mesh.m_indexBuffer.Add(start + 2);
                mesh.m_indexBuffer.Add(start + 1);

                mesh.m_indexBuffer.Add(start + 0);
                mesh.m_indexBuffer.Add(start + 3);
                mesh.m_indexBuffer.Add(start + 2);

                if (twoSided)
                {
                    mesh.m_indexBuffer.Add(start + 4);
                    mesh.m_indexBuffer.Add(start + 5);
                    mesh.m_indexBuffer.Add(start + 6);

                    mesh.m_indexBuffer.Add(start + 4);
                    mesh.m_indexBuffer.Add(start + 6);
                    mesh.m_indexBuffer.Add(start + 7);

                    start += 4;
                }
                start += 4;
            }

            Shape s = new Shape(vertexlist);
            int start2;
            foreach (List<Vec2> l in s.CreateTriangleStripList())
            {
                start2 = mesh.smesh.vertexBuffer.CurrentSize();
                foreach (Vec2 v in l)
                    mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(((Vec3)v) + top, v, new Vec3(0, 1, 0), color));
                for (int lt = 0; lt < l.Count - 2; ++lt)
                {
                    if (lt % 2 == 0 || twoSided)
                    {
                        mesh.m_indexBuffer.Add(start2 + lt + 0);
                        mesh.m_indexBuffer.Add(start2 + lt + 1);
                        mesh.m_indexBuffer.Add(start2 + lt + 2);
                    }
                    if (lt % 2 == 1 || twoSided)
                    {
                        mesh.m_indexBuffer.Add(start2 + lt + 0);
                        mesh.m_indexBuffer.Add(start2 + lt + 2);
                        mesh.m_indexBuffer.Add(start2 + lt + 1);
                    }
                }
            }

            return mesh;
        }

        static public Mesh CreateExtruded2DLine(Line2 line, float height, float wrapU, float wrapV,
                                                    Vec4 color, Material material, bool twoSided,
                                                    float heightFromFloor)
        {
            Mesh mesh = new Mesh(twoSided ? 8 : 4);
            mesh.m_type = MeshType.TRIANGLE_LIST;
            mesh.Material = material;

            Vec3 v1d = (Vec3)line.P1 + new Vec3(0, heightFromFloor, 0);
            Vec3 v2d = (Vec3)line.P2 + new Vec3(0, heightFromFloor, 0);
            Vec3 up = new Vec3(0, height, 0);
            Vec3 v1u = v1d + up;
            Vec3 v2u = v2d + up;

            Vec3 lineVec = (Vec3)(line.P2 - line.P1);
            float len = lineVec.length();
            lineVec.Normalize();
            Vec3 normal = lineVec.Cross(new Vec3(0, 1, 0));

            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(v1d, new Vec2(0, 0), normal, color));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(v1u, new Vec2(0, height * wrapV), normal, color));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(v2u, new Vec2(len * wrapU, height * wrapV), normal, color));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(v2d, new Vec2(len * wrapU, 0), normal, color));

            if (twoSided)
            {
                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(v1d, new Vec2(0, 0), -normal, color));
                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(v1u, new Vec2(0, height * wrapV), -normal, color));
                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(v2u, new Vec2(len * wrapU, height * wrapV), -normal, color));
                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(v2d, new Vec2(len * wrapU, 0), -normal, color));
            }

            mesh.m_indexBuffer.Add(0);
            mesh.m_indexBuffer.Add(2);
            mesh.m_indexBuffer.Add(1);
            mesh.m_indexBuffer.Add(0);
            mesh.m_indexBuffer.Add(3);
            mesh.m_indexBuffer.Add(2);

            if (twoSided)
            {
                mesh.m_indexBuffer.Add(4);
                mesh.m_indexBuffer.Add(5);
                mesh.m_indexBuffer.Add(6);
                mesh.m_indexBuffer.Add(4);
                mesh.m_indexBuffer.Add(6);
                mesh.m_indexBuffer.Add(7);
            }

            return mesh;
        }

        //static public TerrainMesh CreateTerrainMesh(double tileSize, int sizeX, int sizeY, double[,] height, double[, ,] color, double[, ,] texture, string texturename)
        //{
        //    TerrainMesh terrain = new TerrainMesh();
        //    terrain.m_rowLength = sizeX - 1;
        //    terrain.m_tileSize = tileSize;
        //    terrain.m_textureName = texturename;
        //    for (int j = 0; j < sizeY - 1; ++j)
        //    {
        //        Mesh m = new Mesh(new Material(texturename));
        //        m.m_type = MeshType.TRIANGLE_STRIP;
        //        int indexCount = 0;
        //        for (int i = 0; i < sizeX; ++i)
        //        {
        //            double himj = i > 0 ? height[i - 1, j] : height[i, j];
        //            double hijm = j > 0 ? height[i, j - 1] : height[i, j];
        //            double hipj = i < sizeX - 1 ? height[i + 1, j] : height[i, j];
        //            double hijp = j < sizeY - 1 ? height[i, j + 1] : height[i, j];
        //            Vec3 vi0 = new Vec3(-tileSize, himj, 0);
        //            Vec3 vi1 = new Vec3(tileSize, hipj, 0);
        //            Vec3 vj0 = new Vec3(0, himj, -tileSize);
        //            Vec3 vj1 = new Vec3(0, hipj, tileSize);
        //            Vec3 vvi = (vi1 - vi0);
        //            vvi.Normalize();
        //            Vec3 vvj = (vj1 - vj0);
        //            vvj.Normalize();

        //            Vec3 normal1 = vvi.Cross(-vvj);

        //            himj = i > 0 ? height[i - 1, j + 1] : height[i, j + 1];
        //            hijm = height[i, j];
        //            hipj = i < sizeX - 1 ? height[i + 1, j + 1] : height[i, j + 1];
        //            hijp = j < sizeY - 2 ? height[i, j + 2] : height[i, j + 1];
        //            vi0 = new Vec3(-tileSize, himj, 0);
        //            vi1 = new Vec3(tileSize, hipj, 0);
        //            vj0 = new Vec3(0, himj, -tileSize);
        //            vj1 = new Vec3(0, hipj, tileSize);
        //            vvi = (vi1 - vi0);
        //            vvi.Normalize();
        //            vvj = (vj1 - vj0);
        //            vvj.Normalize();

        //            Vec3 normal2 = vvi.Cross(-vvj);

        //            VertexInfo vi = new VertexInfo(new Vec3((double)i * tileSize, height[i, j], (double)j * tileSize),
        //                                            new Vec2(texture[i, j, 0], texture[i, j, 1]),
        //                                            normal1,
        //                                            new Vec4(color[i, j, 0], color[i, j, 1], color[i, j, 2], 1));
        //            m.smesh.vertexBuffer.Add(vi);
        //            vi = new VertexInfo(new Vec3((double)i * tileSize, height[i, j + 1], (double)(j + 1) * tileSize),
        //                                new Vec2(texture[i, j + 1, 0], texture[i, j + 1, 1]),
        //                                normal2,
        //                                new Vec4(color[i, j + 1, 0], color[i, j + 1, 1], color[i, j + 1, 2], 1));
        //            m.smesh.vertexBuffer.Add(vi);
        //            m.m_indexBuffer.Add(indexCount + 1);
        //            m.m_indexBuffer.Add(indexCount);

        //            indexCount += 2;
        //        }
        //        ArrayList list = new ArrayList();
        //        list.Add(m);
        //        terrain.m_meshes.Add(list);
        //        list = new ArrayList();
        //        list.Add(0);
        //        terrain.m_startIndices.Add(list);
        //    }
        //    return terrain;
        //}

        //static public TerrainMesh CreateTerrainMesh(double tileSize, string bitmap, string texturename, double minVal, double maxVal, out double[,] height)
        //{
        //    Bitmap bmp = (Bitmap)Bitmap.FromFile(bitmap);
        //    System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

        //    // Get the address of the first line.
        //    IntPtr ptr = bmpData.Scan0;

        //    // Declare an array to hold the bytes of the bitmap.
        //    int bytes = bmpData.Stride * bmp.Height;
        //    byte[] rgbValues = new byte[bytes];

        //    // Copy the RGB values into the array.
        //    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

        //    double diff = maxVal - minVal;

        //    height = new double[bmp.Width, bmp.Height];
        //    double[, ,] texuv = new double[bmp.Width, bmp.Height, 2];
        //    double[, ,] color = new double[bmp.Width, bmp.Height, 3];
        //    for (int i = 0; i < bmp.Height; ++i)
        //    {
        //        for (int j = 0; j < bmp.Width; ++j)
        //        {
        //            byte col = rgbValues[((i * bmp.Width) + j) * 3];

        //            height[j, i] = minVal + ((double)col / 255) * diff;
        //            texuv[j, i, 0] = j;
        //            texuv[j, i, 1] = i;
        //            color[j, i, 0] = 1;
        //            color[j, i, 1] = 1;
        //            color[j, i, 2] = 1;
        //        }
        //    }

        //    // Unlock the bits.
        //    bmp.UnlockBits(bmpData);

        //    return CreateTerrainMesh(tileSize, bmp.Width, bmp.Height, height, color, texuv, texturename);
        //}

        private static Vec3 GetNormal(double startAngle)
        {
            return new Vec3((float)Math.Cos(startAngle), 0, (float)Math.Sin(startAngle));
        }

        public static Mesh CreateBox(Vec3 midPoint, Vec3 dimensions, Material material, Vec4 color)
        {
            return CreateBox(midPoint, dimensions, material, color, false);
        }

        public static Mesh CreateInsideBox(Vec3 midPoint, Vec3 dimensions, Material material, Vec4 color)
        {
            return CreateBox(midPoint, dimensions, material, color, true);
        }

        private static Mesh CreateBox(Vec3 midPoint, Vec3 dimensions, Material material, Vec4 color, bool inside)
        {
            return CreateBox(midPoint, dimensions, material, inside, color);
        }

        public static Mesh CreateBox(Vec3 midPoint, Vec3 dimensions, Material material)
        {
            return CreateBox(midPoint, dimensions, material, false, new Vec4(1, 1, 1));
        }

        public static Mesh CreateDoorBox(float width, float heightFromFloor, float height, float thickness, Material material, bool flip)
        {
            float texU2 = 1;
            if (flip)
                texU2 = -1;

            float w2 = width * 0.5f;
            float t2 = thickness * 0.5f;
            VertexInfo vi0 = new VertexInfo(new Vec3(-w2, heightFromFloor, -t2), new Vec2(0, 0), new Vec3(0, 0, -1), new Vec4(1, 1, 1));
            VertexInfo vi1 = new VertexInfo(new Vec3(-w2, heightFromFloor + height, -t2), new Vec2(0, 1), new Vec3(0, 0, -1), new Vec4(1, 1, 1));
            VertexInfo vi2 = new VertexInfo(new Vec3(w2, heightFromFloor + height, -t2), new Vec2(texU2, 1), new Vec3(0, 0, -1), new Vec4(1, 1, 1));
            VertexInfo vi3 = new VertexInfo(new Vec3(w2, heightFromFloor, -t2), new Vec2(texU2, 0), new Vec3(0, 0, -1), new Vec4(1, 1, 1));
            Mesh m = new Mesh(8, material);
            m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi0)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m.m_indexBuffer.Add(0); m.m_indexBuffer.Add(1); m.m_indexBuffer.Add(2);
            m.m_indexBuffer.Add(2); m.m_indexBuffer.Add(3); m.m_indexBuffer.Add(0);

            vi0 = new VertexInfo(new Vec3(-w2, heightFromFloor, t2), new Vec2(0, 0), new Vec3(0, 0, 1), new Vec4(1, 1, 1));
            vi1 = new VertexInfo(new Vec3(-w2, heightFromFloor + height, t2), new Vec2(0, 1), new Vec3(0, 0, 1), new Vec4(1, 1, 1));
            vi2 = new VertexInfo(new Vec3(w2, heightFromFloor + height, t2), new Vec2(texU2, 1), new Vec3(0, 0, 1), new Vec4(1, 1, 1));
            vi3 = new VertexInfo(new Vec3(w2, heightFromFloor, t2), new Vec2(texU2, 0), new Vec3(0, 0, 1), new Vec4(1, 1, 1));
            m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi0)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m.m_indexBuffer.Add(4); m.m_indexBuffer.Add(6); m.m_indexBuffer.Add(5);
            m.m_indexBuffer.Add(6); m.m_indexBuffer.Add(4); m.m_indexBuffer.Add(7);
            return m;
        }

        public static Mesh[] CreateWallFaces(Vec2 start, Vec2 end, float height, float thickness, Material m1, Material m2, float innerStartAdjust, float innerEndAdjust, float outerStartAdjust, float outerEndAdjust)
        {
            return CreateWallFaces(start, end, height, 0, thickness, m1, m2, innerStartAdjust, innerEndAdjust, outerStartAdjust, outerEndAdjust);
        }

        public static Mesh[] CreateWallFaces(Vec2 start, Vec2 end, float height, float heightFromFloor, float thickness, Material m1, Material m2, float innerStartAdjust, float innerEndAdjust, float outerStartAdjust, float outerEndAdjust)
        {
            float t2 = thickness * 0.5f;

            Vec3 direction = (end - start);
            Vec3 cross = (end - start).Cross();
            direction.Normalize();
            cross.Normalize();

            Vec3 heightFromFloorVec = new Vec3(0, heightFromFloor, 0);

            Vec3 innerStart = (Vec3)start + t2 * cross - innerStartAdjust * direction + heightFromFloorVec;
            Vec3 innerEnd = (Vec3)end + t2 * cross + innerEndAdjust * direction + heightFromFloorVec;

            Vec3 outerStart = (Vec3)start - t2 * cross - outerStartAdjust * direction + heightFromFloorVec;
            Vec3 outerEnd = (Vec3)end - t2 * cross + outerEndAdjust * direction + heightFromFloorVec;

            Vec3 heightVec = new Vec3(0, height, 0);

            VertexInfo vi0 = VertexInfo.TexBasedOnPosVI(innerStart, cross, new Vec4(1, 1, 1), direction);
            VertexInfo vi1 = VertexInfo.TexBasedOnPosVI(innerStart + heightVec, cross, new Vec4(1, 1, 1), direction);
            VertexInfo vi2 = VertexInfo.TexBasedOnPosVI(innerEnd + heightVec, cross, new Vec4(1, 1, 1), direction);
            VertexInfo vi3 = VertexInfo.TexBasedOnPosVI(innerEnd, cross, new Vec4(1, 1, 1), direction);
            Mesh m = new Mesh(4, m1);
            m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi0)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m.m_indexBuffer.Add(0); m.m_indexBuffer.Add(1); m.m_indexBuffer.Add(2);
            m.m_indexBuffer.Add(2); m.m_indexBuffer.Add(3); m.m_indexBuffer.Add(0);
            Mesh me1 = m;

            vi0 = VertexInfo.TexBasedOnPosVI(outerStart, -cross, new Vec4(1, 1, 1), direction);
            vi1 = VertexInfo.TexBasedOnPosVI(outerStart + heightVec, -cross, new Vec4(1, 1, 1), direction);
            vi2 = VertexInfo.TexBasedOnPosVI(outerEnd + heightVec, -cross, new Vec4(1, 1, 1), direction);
            vi3 = VertexInfo.TexBasedOnPosVI(outerEnd, -cross, new Vec4(1, 1, 1), direction);
            m = new Mesh(4, m2);
            m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi0)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m.m_indexBuffer.Add(0); m.m_indexBuffer.Add(2); m.m_indexBuffer.Add(1);
            m.m_indexBuffer.Add(2); m.m_indexBuffer.Add(0); m.m_indexBuffer.Add(3);
            Mesh me2 = m;
            return new Mesh[] { me1, me2 };
        }

        public static Mesh[] CreateWallFaces(Vec2[,] points, float height, float heightFromFloor, float thickness, Material m1, Material m2)
        {
            float t2 = thickness * 0.5f;

            Vec3 direction = (points[1, 0] - points[0, 0]);
            Vec3 cross = ((Vec2)direction).Cross();
            direction.Normalize();
            cross.Normalize();

            Vec3 heightFromFloorVec = new Vec3(0, heightFromFloor, 0);

            Vec3 innerStart = (Vec3)points[0, 0] + heightFromFloorVec;
            Vec3 innerEnd = (Vec3)points[1, 0] + heightFromFloorVec;

            Vec3 outerStart = (Vec3)points[0, 1] + heightFromFloorVec;
            Vec3 outerEnd = (Vec3)points[1, 1] + heightFromFloorVec;

            Vec3 heightVec = new Vec3(0, height, 0);

            VertexInfo vi0 = VertexInfo.TexBasedOnPosVI(innerStart, -cross, new Vec4(1, 1, 1), direction);
            VertexInfo vi1 = VertexInfo.TexBasedOnPosVI(innerStart + heightVec, -cross, new Vec4(1, 1, 1), direction);
            VertexInfo vi2 = VertexInfo.TexBasedOnPosVI(innerEnd + heightVec, -cross, new Vec4(1, 1, 1), direction);
            VertexInfo vi3 = VertexInfo.TexBasedOnPosVI(innerEnd, -cross, new Vec4(1, 1, 1), direction);
            Mesh m = new Mesh(4, m1);
            m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi0)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m.m_indexBuffer.Add(0); m.m_indexBuffer.Add(2); m.m_indexBuffer.Add(1);
            m.m_indexBuffer.Add(2); m.m_indexBuffer.Add(0); m.m_indexBuffer.Add(3);
            Mesh me1 = m;

            vi0 = VertexInfo.TexBasedOnPosVI(outerStart, cross, new Vec4(1, 1, 1), direction);
            vi1 = VertexInfo.TexBasedOnPosVI(outerStart + heightVec, cross, new Vec4(1, 1, 1), direction);
            vi2 = VertexInfo.TexBasedOnPosVI(outerEnd + heightVec, cross, new Vec4(1, 1, 1), direction);
            vi3 = VertexInfo.TexBasedOnPosVI(outerEnd, cross, new Vec4(1, 1, 1), direction);
            m = new Mesh(4, m2);
            m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi0)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m.m_indexBuffer.Add(0); m.m_indexBuffer.Add(1); m.m_indexBuffer.Add(2);
            m.m_indexBuffer.Add(2); m.m_indexBuffer.Add(3); m.m_indexBuffer.Add(0);
            Mesh me2 = m;
            return new Mesh[] { me1, me2 };
        }

        public static Mesh CreateWallFace(Vec2 start, Vec2 end, float height, float heightFromFloor, Material mat)
        {
            Vec3 heightFromFloorVec = new Vec3(0, heightFromFloor, 0);
            Vec3 start3d = heightFromFloorVec + (Vec3)start;
            Vec3 end3d = heightFromFloorVec + (Vec3)end;
            Vec3 cross = (end - start).Cross();

            Vec3 heightVec = new Vec3(0, height, 0);

            Vec2 direction = end - start;

            VertexInfo vi0 = VertexInfo.TexBasedOnPosVI(start3d, cross, new Vec4(1, 1, 1), direction);
            VertexInfo vi1 = VertexInfo.TexBasedOnPosVI(start3d + heightVec, cross, new Vec4(1, 1, 1), direction);
            VertexInfo vi2 = VertexInfo.TexBasedOnPosVI(end3d + heightVec, cross, new Vec4(1, 1, 1), direction);
            VertexInfo vi3 = VertexInfo.TexBasedOnPosVI(end3d, cross, new Vec4(1, 1, 1), direction);
            Mesh m = new Mesh(4, mat);
            m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi0)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m.m_indexBuffer.Add(0); m.m_indexBuffer.Add(1); m.m_indexBuffer.Add(2);
            m.m_indexBuffer.Add(2); m.m_indexBuffer.Add(3); m.m_indexBuffer.Add(0);
            return m;
        }

        public static Object CreateWallSideFaces(Vec2 start, Vec2 end, float height, float thickness, float heightFromFloor, Material m1, Material m2)
        {
            Vec2 cross = (end - start).Cross();
            cross.Normalize();

            float t2 = thickness * 0.5f;

            Mesh meshLeft1 = CreateWallFace(start + t2 * cross, start, height, heightFromFloor, m1);
            Mesh meshLeft2 = CreateWallFace(start, start - t2 * cross, height, heightFromFloor, m2);

            Mesh meshRight1 = CreateWallFace(end, end + t2 * cross, height, heightFromFloor, m1);
            Mesh meshRight2 = CreateWallFace(end - t2 * cross, end, height, heightFromFloor, m2);

            Object obj = new Object();
            obj.AddNode(meshLeft1);
            obj.AddNode(meshLeft2);
            obj.AddNode(meshRight1);
            obj.AddNode(meshRight2);
            return obj;
        }

        public static Mesh CreateSphere(float radius, int heightSegments, int radialSegments, Vec4 color, Material material)
        {
            Mesh m = new Mesh(heightSegments * radialSegments, material);

            float verticalAngleStep = (float)(Math.PI) / (float)heightSegments;
            float horizontalAngleStep = (float)(2 * Math.PI) / radialSegments;

            for (float verticalAngle = 0; verticalAngle < Math.PI; verticalAngle += verticalAngleStep)
            {
                float h1 = radius * (float)Math.Cos(verticalAngle);
                float r1 = radius * (float)Math.Sin(verticalAngle);
                float h2 = radius * (float)Math.Cos(verticalAngle + verticalAngleStep);
                float r2 = radius * (float)Math.Sin(verticalAngle + verticalAngleStep);
                float texV1 = verticalAngle / (float)Math.PI;
                float texV2 = (verticalAngle + verticalAngleStep) / (float)Math.PI;

                //float angle = 0;
                for (float horizontalAngle = 0; horizontalAngle < Math.PI * 2f; horizontalAngle += horizontalAngleStep)
                {
                    float texU1 = horizontalAngle / ((float)Math.PI * 2f);
                    float texU2 = (horizontalAngle + horizontalAngleStep) / ((float)Math.PI * 2f);

                    Vec3 pos11 = new Vec3(r1 * (float)Math.Cos(horizontalAngle), h1, r1 * (float)Math.Sin(horizontalAngle));
                    Vec3 pos21 = new Vec3(r1 * (float)Math.Cos(horizontalAngle + horizontalAngleStep), h1, r1 * (float)Math.Sin(horizontalAngle + horizontalAngleStep));
                    Vec3 pos12 = new Vec3(r2 * (float)Math.Cos(horizontalAngle), h2, r2 * (float)Math.Sin(horizontalAngle));
                    Vec3 pos22 = new Vec3(r2 * (float)Math.Cos(horizontalAngle + horizontalAngleStep), h2, r2 * (float)Math.Sin(horizontalAngle + horizontalAngleStep));

                    Vec3 norm11 = pos11.normalize();
                    Vec3 norm12 = pos12.normalize();
                    Vec3 norm21 = pos21.normalize();
                    Vec3 norm22 = pos22.normalize();

                    Vec2 tex11 = new Vec2(texU1, texV1);
                    Vec2 tex12 = new Vec2(texU1, texV2);
                    Vec2 tex21 = new Vec2(texU2, texV1);
                    Vec2 tex22 = new Vec2(texU2, texV2);

                    int index = m.smesh.vertexBuffer.CurrentSize();
                    m.smesh.vertexBuffer.Add(new VertexInfoSimple(pos11, tex11, norm11, color));
                    m.smesh.vertexBuffer.Add(new VertexInfoSimple(pos12, tex12, norm12, color));
                    m.smesh.vertexBuffer.Add(new VertexInfoSimple(pos22, tex22, norm22, color));
                    m.smesh.vertexBuffer.Add(new VertexInfoSimple(pos21, tex21, norm21, color));

                    m.m_indexBuffer.Add(index + 0);
                    m.m_indexBuffer.Add(index + 2);
                    m.m_indexBuffer.Add(index + 1);

                    m.m_indexBuffer.Add(index + 0);
                    m.m_indexBuffer.Add(index + 3);
                    m.m_indexBuffer.Add(index + 2);
                }
            }

            return m;
        }

        public static Mesh[] CreateWallFeatureFaces(Vec2 start, Vec2 end, float height, float heightFromFloor, float thickness, Material m1, Material m2, bool closeBottom)
        {
            if (height == 0)
                return new Mesh[] { };

            float t2 = thickness * 0.5f;

            Vec3 direction = (end - start);
            Vec3 cross = (end - start).Cross();
            direction.Normalize();
            cross.Normalize();

            Vec3 heightFromFloorVec = new Vec3(0, heightFromFloor, 0);

            Vec3 innerStart = (Vec3)start + t2 * cross + heightFromFloorVec;
            Vec3 innerEnd = (Vec3)end + t2 * cross + heightFromFloorVec;

            Vec3 outerStart = (Vec3)start - t2 * cross + heightFromFloorVec;
            Vec3 outerEnd = (Vec3)end - t2 * cross + heightFromFloorVec;

            Vec3 heightVec = new Vec3(0, height, 0);

            VertexInfo vi0 = VertexInfo.TexBasedOnPosVI(innerStart, cross, new Vec4(1, 1, 1), direction);
            VertexInfo vi1 = VertexInfo.TexBasedOnPosVI(innerStart + heightVec, cross, new Vec4(1, 1, 1), direction);
            VertexInfo vi2 = VertexInfo.TexBasedOnPosVI(innerEnd + heightVec, cross, new Vec4(1, 1, 1), direction);
            VertexInfo vi3 = VertexInfo.TexBasedOnPosVI(innerEnd, cross, new Vec4(1, 1, 1), direction);
            Mesh m = new Mesh(4, m1);
            m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi0)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m.m_indexBuffer.Add(0); m.m_indexBuffer.Add(1); m.m_indexBuffer.Add(2);
            m.m_indexBuffer.Add(2); m.m_indexBuffer.Add(3); m.m_indexBuffer.Add(0);
            Mesh me1 = m;

            vi0 = VertexInfo.TexBasedOnPosVI(outerStart, -cross, new Vec4(1, 1, 1), direction);
            vi1 = VertexInfo.TexBasedOnPosVI(outerStart + heightVec, -cross, new Vec4(1, 1, 1), direction);
            vi2 = VertexInfo.TexBasedOnPosVI(outerEnd + heightVec, -cross, new Vec4(1, 1, 1), direction);
            vi3 = VertexInfo.TexBasedOnPosVI(outerEnd, -cross, new Vec4(1, 1, 1), direction);
            m = new Mesh(4, m2);
            m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi0)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); m.smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m.m_indexBuffer.Add(0); m.m_indexBuffer.Add(2); m.m_indexBuffer.Add(1);
            m.m_indexBuffer.Add(2); m.m_indexBuffer.Add(0); m.m_indexBuffer.Add(3);
            Mesh me2 = m;

            float t4 = 0.5f * t2;
            Mesh close1 = CreatePlane(new Vec3(), (end - start).length(), t2, new Vec3(0, 1, 0), "", new Vec4(1, 1, 1));
            close1.Material = m1;
            close1.Rotation.Y = -((Vec2)direction).GetAngle();

            Mesh close2 = CreatePlane(new Vec3(), (end - start).length(), t2, new Vec3(0, 1, 0), "", new Vec4(1, 1, 1));
            close2.Material = m2;
            close2.Rotation.Y = -((Vec2)direction).GetAngle();

            if (closeBottom)
            {
                close1.Rotation.Z = (float)Math.PI;
                close2.Rotation.Z = (float)Math.PI;
                close1.Position.Y = heightFromFloor;
                close2.Position.Y = heightFromFloor;
            }
            else
            {
                close1.Position.Y = heightFromFloor + height;
                close2.Position.Y = heightFromFloor + height;
            }

            close1.Position.X = (0.5f * (end + start)).X + t4 * cross.X;
            close2.Position.X = (0.5f * (end + start)).X - t4 * cross.X;
            close1.Position.Z = (0.5f * (end + start)).Y + t4 * cross.Z;
            close2.Position.Z = (0.5f * (end + start)).Y - t4 * cross.Z;


            return new Mesh[] { me1, me2, close1, close2 };
        }

        public static Mesh CreateBox(Vec3 midPoint, Vec3 dimensions, Material material, bool inside, Vec4 color)
        {
            Mesh mesh = new Mesh(24);

            // mesh.m_material.Ambient = new Vec4(color);
            mesh.Material = material;

            float[,] normals = { { 0, 0, -1 }, { 0, 0, 1 }, { -1, 0, 0 }, { 1, 0, 0 }, { 0, 1, 0 }, { 0, -1, 0 } };
            float dx = dimensions.X;
            float dy = dimensions.Y;
            float dz = dimensions.Z;

            float[, ,] parameters = {
                                        {     //--- vertex      texture uv
                                            { -0.5f, -0.5f, -0.5f, 0,  0  },
                                            { -0.5f,  0.5f, -0.5f, 0,  dy },
                                            {  0.5f,  0.5f, -0.5f, dx, dy },
                                            {  0.5f, -0.5f, -0.5f, dx, 0  },
                                        },
                                        {     //--- vertex      texture uv
                                            {  0.5f, -0.5f,  0.5f, dx, 0  },
                                            {  0.5f,  0.5f,  0.5f, dx, dy },
                                            { -0.5f,  0.5f,  0.5f, 0,  dy },
                                            { -0.5f, -0.5f,  0.5f, 0,  0  },
                                        },
                                        {     //--- vertex      texture uv
                                            { -0.5f, -0.5f,  0.5f, dz, 0  },
                                            { -0.5f,  0.5f,  0.5f, dz, dy },
                                            { -0.5f,  0.5f, -0.5f, 0,  dy },
                                            { -0.5f, -0.5f, -0.5f, 0,  0  },
                                        },
                                        {     //--- vertex      texture uv
                                            {  0.5f, -0.5f, -0.5f, 0,  0  },
                                            {  0.5f,  0.5f, -0.5f, 0,  dy },
                                            {  0.5f,  0.5f,  0.5f, dz, dy },
                                            {  0.5f, -0.5f,  0.5f, dz, 0  },
                                        },
                                        {     //--- vertex      texture uv
                                            { -0.5f,  0.5f,  0.5f, 0,  dz },
                                            {  0.5f,  0.5f,  0.5f, dx, dz },
                                            {  0.5f,  0.5f, -0.5f, dx, 0  },
                                            { -0.5f,  0.5f, -0.5f, 0,  0  },
                                        },
                                        {     //--- vertex      texture uv
                                            { -0.5f, -0.5f, -0.5f, 0,  0  },
                                            {  0.5f, -0.5f, -0.5f, dx, 0  },
                                            {  0.5f, -0.5f,  0.5f, dx, dz },
                                            { -0.5f, -0.5f,  0.5f, 0,  dz },
                                        },
                                    };

            for (int i = 0; i < normals.GetLength(0); ++i)
            {
                Vec3 normal = new Vec3(normals[i, 0], normals[i, 1], normals[i, 2]);
                for (int j = 0; j < 4; ++j)
                {
                    mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(
                                                midPoint + new Vec3(parameters[i, j, 0] * dx, parameters[i, j, 1] * dy, parameters[i, j, 2] * dz),
                                                new Vec2(parameters[i, j, 3], parameters[i, j, 4]),
                                                normal,
                                                color));
                }
                if (inside)
                {
                    mesh.m_indexBuffer.Add(i * 4 + 0); mesh.m_indexBuffer.Add(i * 4 + 2); mesh.m_indexBuffer.Add(i * 4 + 1);
                    mesh.m_indexBuffer.Add(i * 4 + 0); mesh.m_indexBuffer.Add(i * 4 + 3); mesh.m_indexBuffer.Add(i * 4 + 2);
                }
                else
                {
                    mesh.m_indexBuffer.Add(i * 4 + 0); mesh.m_indexBuffer.Add(i * 4 + 1); mesh.m_indexBuffer.Add(i * 4 + 2);
                    mesh.m_indexBuffer.Add(i * 4 + 0); mesh.m_indexBuffer.Add(i * 4 + 2); mesh.m_indexBuffer.Add(i * 4 + 3);
                }
            }
            return mesh;
        }

        public static Object CreateTransparentBox(Vec3 midPoint, Vec3 dimensions, Material material, Vec4 color)
        {
            Object obj = new Object();
            obj.AddNode(CreateInsideBox(midPoint, dimensions, material, color));
            obj.AddNode(CreateBox(midPoint, dimensions, material, color));
            obj.m_transparent = true;
            return obj;
        }

        public static Object CreateUpArrow(float height, float radius, float arrowRadius, float arrowLength, uint segments, Vec4 color)
        {
            Object obj = new Object();
            Mesh m1 = CreateCylinder(segments, radius, radius, height - arrowLength, 1, 1, false, true, color);
            m1.Material.AddMaterialMap(new FileMaterialMap("..\\content\\textures\\empty.png", MaterialMap.MapType.Diffuse));
            Mesh m2 = CreateCylinder(segments, arrowRadius, 0, arrowLength, 1, 1, false, true, color);
            m2.Material.AddMaterialMap(new FileMaterialMap("..\\content\\textures\\empty.png", MaterialMap.MapType.Diffuse));
            m2.Position = new Vec3(0, height - arrowLength, 0);
            obj.AddNode(m1);
            obj.AddNode(m2);
            return obj;
        }

        //public static Object CreateArrow(Vec3 direction, double height, double radius, double arrowRadius, double arrowLength, uint segments, Vec4 color)
        //{
        //    Object obj = CreateUpArrow(height, radius, arrowRadius, arrowLength, segments, color);
        //    obj.Rotation = Vec3.CreateRotationBetweenTwoDirectionVectors(new Vec3(0, 1, 0), direction);
        //    return obj;
        //}

        public static Mesh CreatePlane(Vec3 midpoint, float width, float height, Vec3 normal, string texture, Vec4 color)
        {
            return CreatePlane(midpoint, width, height, normal, texture, color, 1, 1);
        }

        public static Mesh CreatePlane(Vec3 midpoint, float width, float height, Vec3 normal, string texture, Vec4 color, float wrapU, float wrapV)
        {
            Mesh mesh = new Mesh(4);
            mesh.Material.AddMaterialMap(new FileMaterialMap(texture, MaterialMap.MapType.Diffuse));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(new Vec3(midpoint.X - width / 2, midpoint.Y, midpoint.Z - height / 2), new Vec2(0, 0), normal, color));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(new Vec3(midpoint.X + width / 2, midpoint.Y, midpoint.Z - height / 2), new Vec2(width / wrapU, 0), normal, color));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(new Vec3(midpoint.X + width / 2, midpoint.Y, midpoint.Z + height / 2), new Vec2(width / wrapU, height / wrapV), normal, color));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(new Vec3(midpoint.X - width / 2, midpoint.Y, midpoint.Z + height / 2), new Vec2(0, height / wrapV), normal, color));
            mesh.m_indexBuffer.Add(0); mesh.m_indexBuffer.Add(2); mesh.m_indexBuffer.Add(1);
            mesh.m_indexBuffer.Add(0); mesh.m_indexBuffer.Add(3); mesh.m_indexBuffer.Add(2);
            return mesh;
        }

        public static Node CreateMeshFrom2DTrianglesReverse(List<Vec2> triangleStrip, double wrapU, double wrapV, Vec4 color, Material mat)
        {
            Mesh mesh = new Mesh(triangleStrip.Count);
            mesh.m_type = MeshType.TRIANGLE_LIST;
            mesh.Material = mat;

            Vec3 normal = new Vec3(0, -1, 0);
            foreach (Vec2 vec in triangleStrip)
                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple((Vec3)vec, new Vec2(vec), normal, color));

            int nrOfTriangles = triangleStrip.Count - 2;
            for (int i = 0; i < nrOfTriangles; ++i)
            {
                mesh.m_indexBuffer.Add(i);

                if (i % 2 == 0)
                {
                    mesh.m_indexBuffer.Add(i + 2);
                    mesh.m_indexBuffer.Add(i + 1);
                }
                else
                {
                    mesh.m_indexBuffer.Add(i + 1);
                    mesh.m_indexBuffer.Add(i + 2);
                }
            }

            return mesh;
        }

        public static Mesh CreateMeshFrom2DTriangles(List<Vec2> triangleStrip, double wrapU, double wrapV, Vec4 color, Material mat)
        {
            Mesh mesh = new Mesh(triangleStrip.Count);
            mesh.m_type = MeshType.TRIANGLE_LIST;
            mesh.Material = mat;

            Vec3 normal = new Vec3(0, 1, 0);
            foreach (Vec2 vec in triangleStrip)
                mesh.smesh.vertexBuffer.Add(new VertexInfoSimple((Vec3)vec, new Vec2(vec), normal, color));

            int nrOfTriangles = triangleStrip.Count - 2;
            for (int i = 0; i < nrOfTriangles; ++i)
            {
                mesh.m_indexBuffer.Add(i);

                if (i % 2 == 0)
                {
                    mesh.m_indexBuffer.Add(i + 1);
                    mesh.m_indexBuffer.Add(i + 2);
                }
                else
                {
                    mesh.m_indexBuffer.Add(i + 2);
                    mesh.m_indexBuffer.Add(i + 1);
                }
            }

            return mesh;
        }

        public static Mesh CreateSkybox(string texture, Box box, int texType)
        {
            Mesh mesh = new Mesh(20);
            mesh.m_type = MeshType.TRIANGLE_LIST;
            mesh.Material = new Material(texture, MaterialMap.MapType.Diffuse);
            mesh.Material.AddShader(new Shader("../content/shaders/simple.vert", Shader.Type.Vertex));
            mesh.Material.AddShader(new Shader("../content/shaders/simple.frag", Shader.Type.Fragment));

            Vec3[, ,] points = box.GetPoints();
            Vec2 t00 = null, t10 = null, t01 = null, t11 = null;

            //--- North:
            if (texType == 1)
            {
                t00 = new Vec2(0.25f, 0.0f); t01 = new Vec2(0.25f, 0.25f); t10 = new Vec2(0.75f, 0.0f); t11 = new Vec2(0.75f, 0.25f);
            }

            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[0, 0, 0], t10, new Vec3(0, 0, 1), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[1, 0, 0], t00, new Vec3(0, 0, 1), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[1, 1, 0], t01, new Vec3(0, 0, 1), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[0, 1, 0], t11, new Vec3(0, 0, 1), new Vec4(1, 1, 1, 1)));
            mesh.m_indexBuffer.Add(0); mesh.m_indexBuffer.Add(1); mesh.m_indexBuffer.Add(2);
            mesh.m_indexBuffer.Add(0); mesh.m_indexBuffer.Add(2); mesh.m_indexBuffer.Add(3);

            //--- East:
            if (texType == 1)
            {
                t00 = new Vec2(1.0f, 0.25f); t01 = new Vec2(0.75f, 0.25f); t10 = new Vec2(1, 0.75f); t11 = new Vec2(0.75f, 0.75f);
            }

            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[0, 0, 1], t10, new Vec3(1, 0, 0), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[0, 0, 0], t00, new Vec3(1, 0, 0), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[0, 1, 0], t01, new Vec3(1, 0, 0), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[0, 1, 1], t11, new Vec3(1, 0, 0), new Vec4(1, 1, 1, 1)));
            mesh.m_indexBuffer.Add(4 + 0); mesh.m_indexBuffer.Add(4 + 1); mesh.m_indexBuffer.Add(4 + 2);
            mesh.m_indexBuffer.Add(4 + 0); mesh.m_indexBuffer.Add(4 + 2); mesh.m_indexBuffer.Add(4 + 3);

            //--- South:
            if (texType == 1)
            {
                t00 = new Vec2(0.25f, 1f); t01 = new Vec2(0.25f, 0.75f); t10 = new Vec2(0.75f, 1f); t11 = new Vec2(0.75f, 0.75f);
            }

            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[1, 0, 1], t00, new Vec3(0, 0, -1), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[0, 0, 1], t10, new Vec3(0, 0, -1), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[0, 1, 1], t11, new Vec3(0, 0, -1), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[1, 1, 1], t01, new Vec3(0, 0, -1), new Vec4(1, 1, 1, 1)));
            mesh.m_indexBuffer.Add(8 + 0); mesh.m_indexBuffer.Add(8 + 1); mesh.m_indexBuffer.Add(8 + 2);
            mesh.m_indexBuffer.Add(8 + 0); mesh.m_indexBuffer.Add(8 + 2); mesh.m_indexBuffer.Add(8 + 3);

            //--- West:
            if (texType == 1)
            {
                t00 = new Vec2(0f, 0.75f); t01 = new Vec2(0.25f, 0.75f); t10 = new Vec2(0f, 0.25f); t11 = new Vec2(0.25f, 0.25f);
            }

            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[1, 0, 0], t10, new Vec3(0, 0, -1), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[1, 0, 1], t00, new Vec3(0, 0, -1), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[1, 1, 1], t01, new Vec3(0, 0, -1), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[1, 1, 0], t11, new Vec3(0, 0, -1), new Vec4(1, 1, 1, 1)));
            mesh.m_indexBuffer.Add(12 + 0); mesh.m_indexBuffer.Add(12 + 1); mesh.m_indexBuffer.Add(12 + 2);
            mesh.m_indexBuffer.Add(12 + 0); mesh.m_indexBuffer.Add(12 + 2); mesh.m_indexBuffer.Add(12 + 3);

            //--- Top:
            if (texType == 1)
            {
                t00 = new Vec2(0.25f, 0.25f); t01 = new Vec2(0.25f, 0.75f); t10 = new Vec2(0.75f, 0.25f); t11 = new Vec2(0.75f, 0.75f);
            }

            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[1, 1, 0], t00, new Vec3(0, 0, -1), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[0, 1, 0], t10, new Vec3(0, 0, -1), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[1, 1, 1], t01, new Vec3(0, 0, -1), new Vec4(1, 1, 1, 1)));
            mesh.smesh.vertexBuffer.Add(new VertexInfoSimple(points[0, 1, 1], t11, new Vec3(0, 0, -1), new Vec4(1, 1, 1, 1)));
            mesh.m_indexBuffer.Add(16 + 0); mesh.m_indexBuffer.Add(16 + 3); mesh.m_indexBuffer.Add(16 + 1);
            mesh.m_indexBuffer.Add(16 + 0); mesh.m_indexBuffer.Add(16 + 2); mesh.m_indexBuffer.Add(16 + 3);

            return mesh;
        }

        public static Node CreateBaseShapeNode(Common.Shapes.BaseShape shape, Vec4 color, Vec4 frontFaceColor, bool addFrontVector, 
                                                Vec3 position, float rotation)
        {
            return CreateBaseShapeNode(shape, color, frontFaceColor, addFrontVector, position, rotation, BasePointRepresentation.Sphere);
        }


        public static Node CreateBaseShapeNode(Common.Shapes.BaseShape shape, Vec4 color, Vec4 frontFaceColor, bool addFrontVector, 
                                                Vec3 position, float rotation, BasePointRepresentation basePointRepresentation)
        {
            Node returnNode = null;
            if (shape is FlatBox)
            {
                FlatBox box = (FlatBox)shape;
                returnNode = SimpleShapes.CreatePlane(new Vec3(box.Box.Midpoint.X, box.PositionY, box.Box.Midpoint.Y), box.Box.Dimensions.X, box.Box.Dimensions.Y, new Vec3(0, 1, 0), "../content/textures/empty.png", color);
            }
            else if (shape is FlatShape)
            {
                FlatShape fs = (FlatShape)shape;
                Object o = new Object();
                foreach(List<Vec2> triStrip in fs.Shape.CreateTriangleStripList())
                    o.AddNode(SimpleShapes.CreateMeshFrom2DTriangles(triStrip, 1, 1, color, Material.Default()));
                o.Position.Y = fs.PositionY;
                returnNode = o;
            }
            else if (shape is ExtrudedShape)
            {
                Mesh m = SimpleShapes.CreateExtruded2DShape(((ExtrudedShape)shape).Shape.Points, ((ExtrudedShape)shape).Height, 1, 1, color, Material.Default(), false);
                m.Position.Y = ((ExtrudedShape)shape).HeightToFloor;
                returnNode = m;
            }
            else if (shape is ExtrudedBox)
            {
                ExtrudedBox eb = (ExtrudedBox)shape;
                Box box = eb.CreateBox();
                returnNode = SimpleShapes.CreateBox(box.Midpoint, box.Dimensions, Material.Default(), color);
                for (int i = 0; i < ((Mesh)returnNode).smesh.vertexBuffer.CurrentSize(); ++i)
                {
                    VertexInfoSimple vi = ((Mesh)returnNode).smesh.vertexBuffer.array[i];
                    if ((vi.m_normal.Vec3() - new Vec3(1, 0, 0)).length() < float.Epsilon)
                    {
                        vi.m_color.Set(frontFaceColor);
                    }
                    ((Mesh)returnNode).smesh.vertexBuffer.array[i] = vi;
                }
            }
            else if (shape is Point)
            {
                Point p = shape as Point;
                switch(basePointRepresentation)
                {
                    case BasePointRepresentation.Sphere:
                        Mesh m = SimpleShapes.CreateSphere(0.025f, 8, 8, color, Material.BasicColorMaterial(color));
                        m.Position = p.MidPoint;
                        return m;
                    case BasePointRepresentation.VerticalLine:
                        return SimpleShapes.CreateBox(new Vec3(p.Pnt.X, p.PositionY + 500, p.Pnt.Y), new Vec3(0.01f, 1000.0f, 0.01f), Material.BasicColorMaterial(color), color);
                    default:
                        throw new NotImplementedException();
                }
            }
            else if (shape is ExtrudedLine)
            {
                ExtrudedLine el = shape as ExtrudedLine;
                returnNode = SimpleShapes.CreateExtruded2DLine(el.Line, el.Height, 1, 1, color, Material.BasicColorMaterial(color), true, el.HeightToFloor);
            }
            else
                throw new NotImplementedException();

            if (addFrontVector)
            {
                Box bb = shape.GetBoundingBox();
                Object arrow = SimpleShapes.CreateUpArrow(bb.Dimensions.X * 0.25f, 0.01f, 0.025f, 0.1f, 32, color);
                arrow.Rotation.Z = (float)(Math.PI / 2);
                arrow.Position.Z = bb.Midpoint.Z;
                arrow.Position.X = bb.Maximum.X;
                arrow.Position.Y = bb.Midpoint.Y;
                Object o = new Object();
                o.AddNode(arrow);
                o.AddNode(returnNode);
                returnNode = o;
            }
            returnNode.Position += position;
            returnNode.Rotation.Y += rotation;
            return returnNode;
        }

        public static Object CreateAxes()
        {
            Mesh xAxis = CreateBox(new Vec3(0.5f, 0, 0), new Vec3(1, 0.05f, 0.05f), Material.BasicColorMaterial(Vec4.Red), Vec4.Red);
            Mesh yAxis = CreateBox(new Vec3(0, 0.5f, 0), new Vec3(0.05f, 1, 0.05f), Material.BasicColorMaterial(Vec4.Green), Vec4.Green);
            Mesh zAxis = CreateBox(new Vec3(0, 0, 0.5f), new Vec3(0.05f, 0.05f, 1), Material.BasicColorMaterial(Vec4.Blue), Vec4.Blue);
            Object axes = new Object();
            axes.AddNode(xAxis);
            axes.AddNode(yAxis);
            axes.AddNode(zAxis);
            return axes;
        }
    }
}
