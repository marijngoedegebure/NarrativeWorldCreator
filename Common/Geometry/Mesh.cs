using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml;

namespace Common.Geometry
{
    public enum MeshType { TRIANGLE_LIST, TRIANGLE_STRIP, TRIANGLE_FAN };

    public struct SMesh
    {
        public VertexInfoArray vertexBuffer;

        public SMesh(int size)
        {
            vertexBuffer = new VertexInfoArray(size);
        }

        public void Dispose()
        {
            vertexBuffer.Clear();
        }
    }

    public class Mesh : Node
    {
        private Material m_material = Material.Default();
        public string m_name = "unnamed mesh";
        public SMesh smesh;
        public List<int> m_indexBuffer;
        public MeshType m_type = MeshType.TRIANGLE_LIST;

        public Material Material
        {
            get { return m_material; }
            set
            {
                if (value == null)
                    throw new Exception("A material can not be null!");
                m_material = value;
            }
        }

        public Mesh(int vertexBufferInitSize)
            : base(null)
        {
            smesh = new SMesh(vertexBufferInitSize);
            m_indexBuffer = new List<int>();
        }

        public Mesh(Mesh copy)
            : base(null)
        {
            Material = new Material(copy.Material);

            smesh = new SMesh(copy.smesh.vertexBuffer.CurrentSize());
            for (int i = 0; i < copy.smesh.vertexBuffer.CurrentSize(); ++i)
                smesh.vertexBuffer.array[smesh.vertexBuffer.NextIndex()] = copy.smesh.vertexBuffer.array[i];

            m_indexBuffer = new List<int>();
            foreach (int i in copy.m_indexBuffer)
                m_indexBuffer.Add(i);
            m_type = copy.m_type;
            m_name = copy.m_name;
        }

        public Mesh(Mesh copy, Node parent)
            : base(parent, copy)
        {
            Material = new Material(copy.Material);

            smesh = new SMesh(copy.smesh.vertexBuffer.CurrentSize());
            for (int i = 0; i < copy.smesh.vertexBuffer.CurrentSize(); ++i)
                smesh.vertexBuffer.array[smesh.vertexBuffer.NextIndex()] = copy.smesh.vertexBuffer.array[i];

            m_indexBuffer = new List<int>();
            foreach (int i in copy.m_indexBuffer)
                m_indexBuffer.Add(i);
            m_type = copy.m_type;
            m_name = copy.m_name;
        }

        public Mesh(int vertexBufferInitSize, string texFilename, MaterialMap.MapType mapType)
            : base(null)
        {
            Material.AddMaterialMap(new FileMaterialMap(texFilename, mapType));
            smesh = new SMesh(vertexBufferInitSize);
            m_indexBuffer = new List<int>();
        }

        public Mesh(int vertexBufferInitSize, Material material)
            : base(null)
        {
            Material = material;
            smesh = new SMesh(vertexBufferInitSize);
            m_indexBuffer = new List<int>();
        }
        public Mesh(int vertexBufferInitSize, Material material, string name)
            : base(null)
        {
            Material = material;
            smesh = new SMesh(vertexBufferInitSize);
            m_indexBuffer = new List<int>();
            m_name = name;
        }

        public void TranslateUV(Vec2 v)
        {
            for (int i = 0; i < smesh.vertexBuffer.CurrentSize(); ++i)
            {
                smesh.vertexBuffer.array[i].m_textureUV.Translate(v);
            }
        }

        public override NodeType IsType()
        {
            return NodeType.MESH;
        }

        public override uint NrOfTriangles()
        {
            if (m_type == MeshType.TRIANGLE_LIST)
                return (uint)(m_indexBuffer.Count / 3);
            else if (m_type == MeshType.TRIANGLE_STRIP)
                return (uint)(m_indexBuffer.Count - 2);
            return 0;
        }

        public override void GetAllMaterialsInternal(List<Material> list)
        {
            foreach (Material mat in list)
            {
                if (mat == Material)
                    return;
            }
            list.Add(Material);
            //if (!list.Contains(m_material.m_textureFilename))
            //    list.Add(m_material.m_textureFilename);
        }

        internal static Mesh GetMeshWithMaterial(List<Mesh> meshes, Material mat)
        {
            foreach (Mesh m in meshes)
                if (m.Material == mat)
                    return m;
            return null;
        }

        protected internal override void ImproveMeshesNewInternal(ImprovedMeshContainer c)
        {
            Mesh m = c.GetMesh(this.Material);
            int vertexCount = m.smesh.vertexBuffer.CurrentSize();
            m.Add(this);
            Matrix4 transformation = GetTransformation();
            Matrix4 rotationTransformation = GetRotationTransformation();
            for (int i = vertexCount; i < m.smesh.vertexBuffer.CurrentSize(); ++i)
            {
                m.smesh.vertexBuffer.array[i].Transform(transformation, rotationTransformation);
                m.smesh.vertexBuffer.array[i].m_textureUV.Set(this.m_material.UpdateTexUV(m.smesh.vertexBuffer.array[i].m_textureUV.Vec2()));
            }

        }

        protected override void ImproveMeshesInternal(List<Mesh> currentMeshes)
        {
            //for (int i = 0; i < smesh.vertexBuffer.CurrentSize(); ++i)
            //    currentMeshes[0].smesh.vertexBuffer.Add(smesh.vertexBuffer.array[i]);

            //Mesh meshToAdd = Mesh.GetMeshWithMaterial(currentMeshes, this.Material);
            //if (meshToAdd == null)
            //{
            //    meshToAdd = new Mesh(100);
            //    meshToAdd.m_type = MeshType.TRIANGLE_LIST;
            //    currentMeshes.Add(meshToAdd);
            //}
            //int index = meshToAdd.smesh.vertexBuffer.CurrentSize();
            //if (this.m_type != MeshType.TRIANGLE_LIST)
            //    throw new NotImplementedException();
            //foreach (int i in m_indexBuffer)
            //    meshToAdd.m_indexBuffer.Add(index + i);
            //for (int i = 0; i < smesh.vertexBuffer.CurrentSize(); ++i)
            //    meshToAdd.smesh.vertexBuffer.Add(smesh.vertexBuffer.array[i]);


            Matrix4 transformation = GetTransformation();
            Matrix4 rotationTransformation = GetRotationTransformation();
            Mesh newMesh = new Mesh(this);
            for (int i = 0; i < newMesh.smesh.vertexBuffer.CurrentSize(); ++i)
            {
                newMesh.smesh.vertexBuffer.array[i].Transform(transformation, rotationTransformation);
                newMesh.smesh.vertexBuffer.array[i].m_textureUV.Set(this.m_material.UpdateTexUV(newMesh.smesh.vertexBuffer.array[i].m_textureUV.Vec2()));
                //vi.m_position += m_position;
            }

            Mesh mesh = GetMeshWithMaterial(currentMeshes, newMesh.Material);
            if (mesh == null)
            {
                mesh = new Mesh(1000, newMesh.Material);
                mesh.m_name = newMesh.m_name;
                mesh.m_material.Scale = 1;
                mesh.m_material.RemapUV(new Vec2(), new Vec2(1, 1));
                currentMeshes.Add(mesh);
            }
            mesh.Add(newMesh);
            newMesh.Clean();
        }

        protected override IEnumerable<Mesh> GetSeparateMeshesInternal()
        {
            Matrix4 transformation = GetTransformation();
            Matrix4 rotationTransformation = GetRotationTransformation();
            Mesh newMesh = new Mesh(this);
            for (int i = 0; i < newMesh.smesh.vertexBuffer.CurrentSize(); ++i)
            {
                newMesh.smesh.vertexBuffer.array[i].m_position.Transform(transformation);
                newMesh.smesh.vertexBuffer.array[i].m_normal.Transform(rotationTransformation);
                newMesh.smesh.vertexBuffer.array[i].m_normal.Normalize();
                newMesh.smesh.vertexBuffer.array[i].m_textureUV.Set(this.m_material.UpdateTexUV(newMesh.smesh.vertexBuffer.array[i].m_textureUV.Vec2()));
            }
            yield return newMesh;
        }
        protected override IEnumerable<Mesh> GetOriginalMeshesInternal()
        {
            yield return this;
        }

        internal void Add(Mesh m)
        {
            int vertexCount = smesh.vertexBuffer.CurrentSize();
            for (int i = 0; i < m.smesh.vertexBuffer.CurrentSize(); ++i)
                smesh.vertexBuffer.Add(m.smesh.vertexBuffer.array[i]);
            if (m.m_type == MeshType.TRIANGLE_LIST)
            {
                foreach (int i in m.m_indexBuffer)
                    m_indexBuffer.Add(vertexCount + i);
            }
            else if (m.m_type == MeshType.TRIANGLE_STRIP)
            {
                for (int i = 0; i < m.m_indexBuffer.Count - 2; ++i)
                {
                    if (i % 2 == 0)
                    {
                        m_indexBuffer.Add(vertexCount + m.m_indexBuffer[i]);
                        m_indexBuffer.Add(vertexCount + m.m_indexBuffer[i + 2]);
                        m_indexBuffer.Add(vertexCount + m.m_indexBuffer[i + 1]);
                    }
                    else
                    {
                        m_indexBuffer.Add(vertexCount + m.m_indexBuffer[i]);
                        m_indexBuffer.Add(vertexCount + m.m_indexBuffer[i + 1]);
                        m_indexBuffer.Add(vertexCount + m.m_indexBuffer[i + 2]);
                    }
                }
            }
            else
                throw new NotImplementedException();
        }

        public void AddTriangle(VertexInfo[] triangle)
        {
#if DEBUG
            if (triangle.Length != 3)
                throw new Exception("A triangle should contain 3 VertexInfo objects");
#endif
            AddTriangle(triangle[0], triangle[1], triangle[2]);
        }

        public void AddTriangle(VertexInfo v1, VertexInfo v2, VertexInfo v3)
        {
            int offset = smesh.vertexBuffer.CurrentSize();
            m_indexBuffer.Add(offset + 0);
            m_indexBuffer.Add(offset + 1);
            m_indexBuffer.Add(offset + 2);
            smesh.vertexBuffer.Add(new VertexInfoSimple(v1)); 
            smesh.vertexBuffer.Add(new VertexInfoSimple(v2)); 
            smesh.vertexBuffer.Add(new VertexInfoSimple(v3));
        }

        public void AddTriangle(Vec3 v1, Vec3 v2, Vec3 v3)
        {
            AddTriangle(new VertexInfo(v1, new Vec2(1, 1), new Vec3(0, 0, 1), new Vec4(1, 1, 1, 1)),
                        new VertexInfo(v2, new Vec2(0, 1), new Vec3(0, 0, 1), new Vec4(1, 1, 1, 1)),
                        new VertexInfo(v3, new Vec2(0, 0), new Vec3(0, 0, 1), new Vec4(1, 1, 1, 1)));
        }
        public void AddTriangle(Vec2 v1, Vec2 v2, Vec2 v3)
        {
            AddTriangle(new Vec3(v1.X, 0, v1.Y), new Vec3(v2.X, 0, v2.Y), new Vec3(v3.X, 0, v3.Y));
        }
        public void AddTwoTriangles(VertexInfo[] quad)
        {
#if DEBUG
            if (quad.Length != 4)
                throw new Exception("A quad should contain 4 VertexInfo objects");
#endif
            AddTriangle(quad[0], quad[1], quad[2]);
            AddTriangle(quad[2], quad[3], quad[0]);
        }

        internal void AddDebugPoint(Vec3 point)
        {
            Vec3 temp = new Vec3(0.025f, 0, 0.0f);
            Vec3 temp2 = new Vec3(0.0f, 0.075f, 0.0f);
            AddTriangle(point, point + temp + temp2, point - temp + temp2);
        }

        public static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        public override void GetBoundingBoxInternal(Matrix4 transformation, Box box)
        {
            Matrix4 transform = GetTransformation() * transformation;
            for(int i = 0; i < smesh.vertexBuffer.CurrentSize(); ++i)
            {
                Vec3 v = smesh.vertexBuffer.array[i].m_position.Vec3() * transform;
                box.AddPointToBoundingBox(v);
            }
        }

        public new Box GetBoundingBox(bool addTotalTransformation)
        {
            Box bb = new Box(Vec3.Max, Vec3.Min);
            if (addTotalTransformation)
                GetBoundingBoxInternal(GetParentTransformation(), bb);
            else
                GetBoundingBoxInternal(Matrix4.Identity, bb);
            return bb;
        }

        public override int FirstLevelOccuranceOf(string name, int level)
        {
            if (m_name.ToLower().Contains(name))
                return level;
            return -1;
        }

        public override void GetAllInLevelContaining(List<Node> list, int level, string str, int currentLevel)
        {
            if (currentLevel != level)
                return;
            if (m_name.ToLower().Contains(str))
                list.Add(this);
        }

        private static void TriangleVolumeCalc(Vec3 v0, Vec3 v1, Vec3 v2,
                                                    ref double volume, ref double area)
        {
            volume += v0 * v1.Cross(v2);
            area += ((v1 - v0).Cross(v2 - v0)).length() / 2;
        }

        private void TriangleVolumeCalc(List<Vec3> vecs, int i0, int i1, int i2, 
                                                ref double volume, ref double area)
        {
            TriangleVolumeCalc(vecs[m_indexBuffer[i0]], vecs[m_indexBuffer[i1]], 
                                    vecs[m_indexBuffer[i2]], ref volume, ref area);
        }

        public void GetVolumeAndArea(out double volume, out double area)
        {
            List<Vec3> vecs = new List<Vec3>();
            Matrix4 transformation = GetParentTransformation();
            for (int i = 0; i < smesh.vertexBuffer.CurrentSize(); ++i)
                vecs.Add(smesh.vertexBuffer.array[i].m_position.Vec3() * transformation);
            volume = 0;
            area = 0;
            int nrOfTriangles;
            switch (m_type)
            {
                case MeshType.TRIANGLE_FAN:
                    nrOfTriangles = m_indexBuffer.Count - 2;
                    for (int i = 0; i < nrOfTriangles; ++i)
                        TriangleVolumeCalc(vecs, 0, i + 1, i + 2, ref volume, ref area);
                    break;
                case MeshType.TRIANGLE_LIST:
                    nrOfTriangles = m_indexBuffer.Count / 3;
                    for (int i = 0; i < nrOfTriangles; ++i)
                        TriangleVolumeCalc(vecs, i * 3, i * 3 + 1, i * 3 + 2, ref volume, ref area);
                    break;
                case MeshType.TRIANGLE_STRIP:
                    nrOfTriangles = m_indexBuffer.Count - 2;
                    for (int i = 0; i < nrOfTriangles; ++i)
                    {
                        if (i % 2 == 0)
                            TriangleVolumeCalc(vecs, i, i + 1, i + 2, ref volume, ref area);
                        else
                            TriangleVolumeCalc(vecs, i + 1, i, i + 2, ref volume, ref area);
                    }
                    break;
            }
            volume /= 6;
        }

        public override void Save(System.IO.BinaryWriter wr)
        {
            wr.Write('m');
            base.Save(wr);
            wr.Write(this.m_indexBuffer.Count);
            foreach (int i in this.m_indexBuffer)
                wr.Write(i);
            Material.Save(wr);
            wr.Write(m_name);
            wr.Write(m_type.ToString());
            wr.Write(this.smesh.vertexBuffer.CurrentSize());
            for(int i = 0; i < this.smesh.vertexBuffer.CurrentSize(); ++i)
                this.smesh.vertexBuffer.array[i].Save(wr);
        }

        internal Mesh(Node parent, System.IO.BinaryReader br)
            : base(parent, br)
        {
            int count = br.ReadInt32();
            this.m_indexBuffer = new List<int>();
            for (int i = 0; i < count; ++i)
                this.m_indexBuffer.Add(br.ReadInt32());
            Material = new Material(br);
            m_name = br.ReadString();
            m_type = (MeshType)Enum.Parse(typeof(MeshType), br.ReadString());
            count = br.ReadInt32();
            smesh = new SMesh(count);
            for (int i = 0; i < count; ++i)
                this.smesh.vertexBuffer.Add(new VertexInfoSimple(br));
        }

        public void AddTriangleExtended(Vec3 v1, Vec3 v2, Vec3 v3)
        {
            AddTriangleExtended(v1, v2, v3, CreateNormalFor(v1, v2, v3));
        }

        public void AddTriangleExtended(Vec3 v1, Vec3 v2, Vec3 v3, Vec4 color)
        {
            AddTriangleExtended(v1, v2, v3, CreateNormalFor(v1, v2, v3), color);
        }

        public Vec3 CreateNormalFor(Vec3 v1, Vec3 v2, Vec3 v3)
        {
            Vec3 p1 = v2 - v1;
            p1.Normalize();
            Vec3 p2 = v3 - v1;
            p2.Normalize();
            return p1.Cross(p2);
        }

        public void AddTriangleExtended(Vec3 v1, Vec3 v2, Vec3 v3, Vec3 normal)
        {
            AddTriangleExtended(v1, v2, v3, normal, new Vec4(1, 1, 1));
        }

        public void AddTriangleExtended(Vec3 v1, Vec3 v2, Vec3 v3, Vec3 normal, Vec4 color)
        {
            Vec2 d1 = v2 - v1, d2 = v3 - v1, d3 = v3 - v2;
            double l1 = d1.length(), l2 = d2.length(), l3 = d3.length();

            Vec2 direction = null;
            if (v1.Y == v2.Y)
                direction = d1;
            else if (v2.Y == v3.Y)
                direction = d3;
            else if (v3.Y == v1.Y)
                direction = d2;
            else
                direction = new Vec2(1, 0);

            VertexInfo vi1 = VertexInfo.TexBasedOnPosVI(v1, normal, color, direction);
            VertexInfo vi2 = VertexInfo.TexBasedOnPosVI(v2, normal, color, direction);
            VertexInfo vi3 = VertexInfo.TexBasedOnPosVI(v3, normal, color, direction);
            int start = smesh.vertexBuffer.CurrentSize();
            smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m_indexBuffer.Add(start); m_indexBuffer.Add(start + 1); m_indexBuffer.Add(start + 2);
        }

        public void AddTriangleExtended(Vec3 v1, Vec3 v2, Vec3 v3, Vec3 n1, Vec3 n2, Vec3 n3)
        {
            Vec2 d1 = v2 - v1, d2 = v3 - v1, d3 = v3 - v2;
            double l1 = d1.length(), l2 = d2.length(), l3 = d3.length();

            Vec2 direction = d1;
            if (l2 > l1 && l2 > l3)
                direction = d2;
            if (l3 > l1 && l3 > l2)
                direction = d3;

            VertexInfo vi1 = VertexInfo.TexBasedOnPosVI(v1, n1, new Vec4(1, 1, 1), direction);
            VertexInfo vi2 = VertexInfo.TexBasedOnPosVI(v2, n2, new Vec4(1, 1, 1), direction);
            VertexInfo vi3 = VertexInfo.TexBasedOnPosVI(v3, n3, new Vec4(1, 1, 1), direction);
            int start = smesh.vertexBuffer.CurrentSize();
            smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m_indexBuffer.Add(start); m_indexBuffer.Add(start + 1); m_indexBuffer.Add(start + 2);
        }

        public void AddTriangleExtendedXZ(Vec3 v1, Vec3 v2, Vec3 v3)
        {
            Vec3 p1 = v2 - v1;
            p1.Normalize();
            Vec3 p2 = v3 - v1;
            p2.Normalize();
            Vec3 normal = p1.Cross(p2);
            VertexInfo vi1 = VertexInfo.TexBasedOnPosVIXZ(v1, normal, new Vec4(1, 1, 1));
            VertexInfo vi2 = VertexInfo.TexBasedOnPosVIXZ(v2, normal, new Vec4(1, 1, 1));
            VertexInfo vi3 = VertexInfo.TexBasedOnPosVIXZ(v3, normal, new Vec4(1, 1, 1));
            int start = smesh.vertexBuffer.CurrentSize();
            smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m_indexBuffer.Add(start); m_indexBuffer.Add(start + 1); m_indexBuffer.Add(start + 2);
        }

        public void AddTriangleExtendedXZ(Vec3 v1, Vec3 v2, Vec3 v3, Vec3 n1, Vec3 n2, Vec3 n3)
        {
            Vec3 p1 = v2 - v1;
            p1.Normalize();
            Vec3 p2 = v3 - v1;
            p2.Normalize();
            Vec3 normal = p1.Cross(p2);
            VertexInfo vi1 = VertexInfo.TexBasedOnPosVIXZ(v1, n1, new Vec4(1, 1, 1));
            VertexInfo vi2 = VertexInfo.TexBasedOnPosVIXZ(v2, n2, new Vec4(1, 1, 1));
            VertexInfo vi3 = VertexInfo.TexBasedOnPosVIXZ(v3, n3, new Vec4(1, 1, 1));
            int start = smesh.vertexBuffer.CurrentSize();
            smesh.vertexBuffer.Add(new VertexInfoSimple(vi1)); smesh.vertexBuffer.Add(new VertexInfoSimple(vi2)); smesh.vertexBuffer.Add(new VertexInfoSimple(vi3));
            m_indexBuffer.Add(start); m_indexBuffer.Add(start + 1); m_indexBuffer.Add(start + 2);
        }

        internal protected override void GetExternalObjectsInternal(List<Object> list)
        {
        }

        /// <summary>
        /// Transform all vertices of the mesh
        /// </summary>
        /// <param name="transformation">The transformation matrix for the position</param>
        /// <param name="normalTransformation">The transformation matrix for the normal (should be rotation only), for transformation without rotation leave 'null'</param>
        public void Transform(Matrix4 transformation, Matrix4 normalTransformation)
        {
            for (int i = 0; i < smesh.vertexBuffer.CurrentSize(); ++i)
            {
                smesh.vertexBuffer.array[i].m_position.Transform(transformation);
                if (normalTransformation != null)
                    smesh.vertexBuffer.array[i].m_normal.Transform(normalTransformation);
            }
        }

        public List<List<int>> GetTriangles()
        {
            List<List<int>> triangles;
            int nrOfTriangles;
            switch (m_type)
            {
                case MeshType.TRIANGLE_FAN:
                    nrOfTriangles = m_indexBuffer.Count - 2;
                    triangles = new List<List<int>>(nrOfTriangles);
                    for (int i = 0; i < nrOfTriangles; ++i)
                    {
                        List<int> tri = new List<int>(3);
                        tri.Add(m_indexBuffer[0]);
                        tri.Add(m_indexBuffer[i + 1]);
                        tri.Add(m_indexBuffer[i + 2]);
                        triangles.Add(tri);
                    }
                    break;
                case MeshType.TRIANGLE_LIST:
                    nrOfTriangles = m_indexBuffer.Count / 3;
                    triangles = new List<List<int>>(nrOfTriangles);
                    for (int i = 0; i < nrOfTriangles; ++i)
                    {
                        List<int> tri = new List<int>(3);
                        tri.Add(m_indexBuffer[i * 3]);
                        tri.Add(m_indexBuffer[i * 3 + 1]);
                        tri.Add(m_indexBuffer[i * 3 + 2]);
                        triangles.Add(tri);
                    }
                    break;
                case MeshType.TRIANGLE_STRIP:
                    nrOfTriangles = m_indexBuffer.Count - 2;
                    triangles = new List<List<int>>(nrOfTriangles);
                    for (int i = 0; i < nrOfTriangles; ++i)
                    {
                        List<int> tri = new List<int>(3);
                        if (i % 2 == 0)
                        {
                            tri.Add(m_indexBuffer[i]);
                            tri.Add(m_indexBuffer[i + 1]);
                            tri.Add(m_indexBuffer[i + 2]);
                        }
                        else
                        {
                            tri.Add(m_indexBuffer[i + 1]);
                            tri.Add(m_indexBuffer[i]);
                            tri.Add(m_indexBuffer[i + 2]);
                        }
                        triangles.Add(tri);
                    }
                    break;
                default:
                    throw new Exception("Not implemented for this mesh type: " + m_type.ToString());
            }
            return triangles;
        }


        public void ConvertToTriangleList()
        {
            switch (m_type)
            {
                case MeshType.TRIANGLE_LIST:
                    break;
                case MeshType.TRIANGLE_STRIP:
                    List<int> newIndexBuffer = new List<int>((m_indexBuffer.Count - 2) * 3);
                    for (int i = 0; i < m_indexBuffer.Count - 2; ++i)
                    {
                        if (i % 2 == 0)
                        {
                            newIndexBuffer.Add(m_indexBuffer[i]);
                            newIndexBuffer.Add(m_indexBuffer[i + 1]);
                            newIndexBuffer.Add(m_indexBuffer[i + 2]);
                        }
                        else
                        {
                            newIndexBuffer.Add(m_indexBuffer[i + 1]);
                            newIndexBuffer.Add( m_indexBuffer[i]);
                            newIndexBuffer.Add(m_indexBuffer[i + 2]);
                        }
                    }
                    m_indexBuffer = newIndexBuffer;
                    break;
                case MeshType.TRIANGLE_FAN:
                default:
                    throw new NotImplementedException();
            }
            m_type = MeshType.TRIANGLE_LIST;
        }

        public override void Dispose()
        {
            //foreach (VertexInfoSimple vi in smesh.vertexBuffer)
            //    vi.Dispose();
            //m_material.Dispose();
            smesh.vertexBuffer.Clear();
            if (m_indexBuffer != null) 
            {
                m_indexBuffer.Clear();
                m_indexBuffer = null;
            }
            base.Dispose();
        }

        #region 3ds test: mag weg als collada werkt!
        public static void ExportTo3ds(List<Mesh> meshes, string path)
        {
            BinaryWriter w = new BinaryWriter(File.Open(path, FileMode.Create));

            w.Write((ushort)0x4d4d);
            w.Write((uint)(6 + FileLength(meshes)));
            ExportFile(meshes, w);
            //w.Write((ushort)0x4d4d);
            //w.Write((uint)6);

            w.Close();
        }

        private static void ExportFile(List<Mesh> meshes, BinaryWriter w)
        {
            ExportVersion(w);
            //ExportKFData(meshes, w);
            //ExportMeshData(meshes, w);
        }

        private static uint FileLength(List<Mesh> meshes)
        {
            return VersionLength();// +KFDataLength(meshes) + MeshDataLength(meshes);
        }

        private static void ExportVersion(BinaryWriter w)
        {
            w.Write((ushort)0x0002);
            w.Write((uint)5);
            w.Write((ushort)0);
            w.Write((ushort)1);
        }

        private static uint VersionLength()
        {
            return 10;
        }

        private static void ExportKFData(List<Mesh> meshes, BinaryWriter w)
        {
            w.Write((ushort)0xb000);
            w.Write(6);
        }

        private static uint KFDataLength(List<Mesh> meshes)
        {
            return 6;
        }

        private static void ExportMeshData(List<Mesh> meshes, BinaryWriter w)
        {
            w.Write((ushort)0x3d3d);
            w.Write(MeshDataLength(meshes));
            w.Write((ushort)0x3d3e);
            w.Write((uint)10);
            w.Write((ushort)1);
            w.Write((ushort)0);
            ExportNamedObject(meshes[0], w);
        }

        private static uint MeshDataLength(List<Mesh> meshes)
        {
            return 16 + NamedObjectLength(meshes[0]);
        }

        private static void ExportNamedObject(Mesh mesh, BinaryWriter w)
        {
            w.Write((ushort)0x4000);
            uint len = NamedObjectLength(mesh);
            w.Write(len);
            len -= 6;
            w.Write((ushort)0x4100);
            w.Write(len);
            len -= 6;
            ExportVertexArray(mesh, w);
            ExportFaceArray(mesh, w);
        }

        private static void ExportVertexArray(Mesh mesh, BinaryWriter w)
        {
            w.Write((short)0x4110);
            w.Write((int)mesh.smesh.vertexBuffer.CurrentSize() * 12 + 2 + 6);
            w.Write((short)mesh.smesh.vertexBuffer.CurrentSize());
            for(int i = 0; i < mesh.smesh.vertexBuffer.CurrentSize(); ++i)
            {
                Float32ToLong(mesh.smesh.vertexBuffer.array[i].m_position.X, w);
                Float32ToLong(mesh.smesh.vertexBuffer.array[i].m_position.Z, w);
                Float32ToLong(mesh.smesh.vertexBuffer.array[i].m_position.Y, w);
            }
        }

        private static void ExportFaceArray(Mesh mesh, BinaryWriter w)
        {
            w.Write((ushort)0x4120);
            w.Write((uint)mesh.NrOfTriangles() * 8 + 2 + 6);
            w.Write((ushort)mesh.NrOfTriangles());

            int count = 0;
            for (int i = 0; i < mesh.NrOfTriangles(); ++i)
            {
                w.Write((short)mesh.m_indexBuffer[count++]);
                w.Write((short)mesh.m_indexBuffer[count++]);
                w.Write((short)mesh.m_indexBuffer[count++]);
                w.Write((short)0);
            }
        }

        private static uint NamedObjectLength(Mesh mesh)
        {
            return (uint)(6 + 6 + mesh.smesh.vertexBuffer.CurrentSize() * 12 + 2 + 6 + (uint)mesh.NrOfTriangles() * 8 + 2 + 6);
        }


        private static void Float32ToLong(double p, BinaryWriter w)
        {
            w.Write((float)p);
        }
        #endregion

        //TODO: Look up textures in referenced object meshes ?
        public static HashSet<String> GetTextures(List<Mesh> meshes, string texturePath)
        {
            HashSet<String> textureList = new HashSet<String>();
            foreach (Common.Geometry.Mesh mesh in meshes)
            {
                foreach (Common.Geometry.Material material in mesh.GetAllMaterials())
                {
                    foreach (Common.Geometry.MaterialMap material_map in material.MaterialMaps)
                    {
                        if (material_map is FileMaterialMap)
                        {
                            FileMaterialMap fmm = (FileMaterialMap) material_map;
                            FileInfo fi = new FileInfo(texturePath + "/" + fmm.MapFilename.Replace("\\", "/"));
                            if (fi.Exists)
                                textureList.Add(fi.FullName);
                        }
                        else
                            throw new NotImplementedException();
                    }
                }
            }
            return textureList;
        }

        public static void ExportToCollada(List<Mesh> meshes, List<Object> externalObjects, List<TransformedObjectInstance> objectInstances, string path, string maxScriptPath, string texturePath, bool exportTextures, string relExportTexturePath)
        {
            List<string> usedMaterialNames = new List<string>();
            Dictionary<ObjectReference, List<Mesh>> referencedObjectMeshes = new Dictionary<ObjectReference, List<Mesh>>();
            List<Mesh> temp = new List<Mesh>();
            temp.AddRange(meshes);
            foreach (TransformedObjectInstance toi in objectInstances)
            {
                if (!referencedObjectMeshes.ContainsKey(toi.Instance.Reference))
                {
                    List<Mesh> temp2 = toi.Instance.Reference.ImproveMeshes();
                    referencedObjectMeshes.Add(toi.Instance.Reference, temp2);
                    temp.AddRange(temp2);
                }
            }
            foreach (Mesh m in temp)
            {
                string materialName = m.Material.Name;
                if (usedMaterialNames.Contains(materialName))
                {
                    int count2 = 0;
                    while (usedMaterialNames.Contains(materialName + count2))
                        ++count2;
                    materialName = materialName + count2;
                }
                m.Material.Name = materialName;
                usedMaterialNames.Add(materialName);
            }

            #region Collada file
            System.Xml.XmlTextWriter w = new System.Xml.XmlTextWriter(path, Encoding.UTF8);
            w.Formatting = System.Xml.Formatting.Indented;
            w.WriteStartDocument();
            w.Formatting = System.Xml.Formatting.Indented;

            w.WriteStartElement("COLLADA");
            {
                w.WriteAttributeString("xmlns", "http://www.collada.org/2008/03/COLLADASchema");
                w.WriteAttributeString("version", "1.4.0");

                {
                    w.WriteStartElement("asset");
                    {
                        w.WriteStartElement("created");
                        {
                            DateTime dt = DateTime.Now;
                            w.WriteString("" + dt.Year + "-" + dt.Month + "-" + dt.Day + "T" + dt.Hour + ":" + dt.Minute + ":" + dt.Second + "Z");
                        }
                        w.WriteEndElement();
                        w.WriteStartElement("version");
                        {
                            w.WriteString("1.0");
                        }
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();

                    w.WriteStartElement("library_images");
                    {
                        foreach (Mesh m in temp)
                        {
                            if (m.Material.MaterialMaps.Count > 0)
                            {
                                if (m.Material.MaterialMaps[0] is FileMaterialMap)
                                {
                                    FileMaterialMap fmm = (FileMaterialMap)m.Material.MaterialMaps[0];
                                    FileInfo fi = new FileInfo(fmm.MapFilename);
                                    w.WriteStartElement("image");
                                    {
                                        w.WriteAttributeString("id", "map_" + m.UID + "-image");
                                        w.WriteAttributeString("name", "map_" + m.UID);
                                        w.WriteStartElement("init_from");
                                        {
                                            if (exportTextures)
                                                if (relExportTexturePath == String.Empty)
                                                    w.WriteString(fi.Name);
                                                else
                                                    w.WriteString(relExportTexturePath + "/" + fi.Name);
                                            else
                                                w.WriteString(fi.FullName);
                                        }
                                        w.WriteEndElement();
                                    }
                                    w.WriteEndElement();
                                }
                                else
                                    throw new NotImplementedException();
                            }
                        }
                    }
                    w.WriteEndElement();

                    w.WriteStartElement("library_effects");
                    {
                        foreach (Mesh m in temp)
                            WriteEffect(m, m.Material, w);
                    }
                    w.WriteEndElement();

                    w.WriteStartElement("library_materials");
                    {
                        foreach (Mesh m in temp)
                            WriteMaterial(m.Material, w);
                    }
                    w.WriteEndElement();

                    w.WriteStartElement("library_geometries");
                    {
                        foreach (Mesh m in meshes)
                            WriteGeometry(m, w);
                        foreach (List<Mesh> mshs in referencedObjectMeshes.Values)
                        {
                            foreach (Mesh m in mshs)
                                WriteGeometry(m, w);
                        }
                    }
                    w.WriteEndElement();

                    w.WriteStartElement("library_visual_scenes");
                    {
                        w.WriteStartElement("visual_scene");
                        {
                            w.WriteAttributeString("id", "DefaultScene");

                            foreach(Mesh m in meshes)
                                WriteNode(m, w);

                            foreach (TransformedObjectInstance toi in objectInstances)
                                WriteTransformedObjectInstance(referencedObjectMeshes[toi.Instance.Reference], toi, w);
                        }
                        w.WriteEndElement();
                        //Dictionary<ObjectReference, List<Mesh>>.Enumerator en = referencedObjectMeshes.GetEnumerator();
                        //while (en.MoveNext())
                        //{
                        //    w.WriteStartElement("visual_scene");
                        //    {
                        //        w.WriteAttributeString("id", "ref_obj_" + en.Current.Key.Obj.UID);

                        //        //foreach (Mesh m in en.Current.Value)
                        //        //    WriteNode(m, w);

                        //        foreach (TransformedObjectInstance toi in objectInstances)
                        //            WriteTransformedObjectInstance(referencedObjectMeshes[toi.Instance.Reference], toi, w);
                        //    }
                        //    w.WriteEndElement();
                        //}
                    }
                    w.WriteEndElement();    
               
                    /*
                     *  <scene> 
                     *      <instance_visual_scene url="#DefaultScene"/>
                     *  </scene> */

                    w.WriteStartElement("scene");
                    {
                        w.WriteStartElement("instance_visual_scene");
                        {
                            w.WriteAttributeString("url", "#DefaultScene");
                        }
                        w.WriteEndElement();
                        //Dictionary<ObjectReference, List<Mesh>>.Enumerator en = referencedObjectMeshes.GetEnumerator();
                        //while (en.MoveNext())
                        //{
                        //    w.WriteStartElement("instance_visual_scene");
                        //    {
                        //        w.WriteAttributeString("url", "#ref_obj_" + en.Current.Key.Obj.UID);
                        //    }
                        //    w.WriteEndElement();
                        //}
                    }
                    w.WriteEndElement();

                }

            }
            w.WriteEndElement();

            w.WriteEndDocument();
            w.Close();
            #endregion

            #region MaxScript file
            StreamWriter sw = new StreamWriter(maxScriptPath, false, Encoding.ASCII);
            StreamReader sr = new StreamReader("..\\content\\settings\\MaxScriptFunctions.txt");
            while (!sr.EndOfStream)
                sw.WriteLine(sr.ReadLine());
            sr.Close();
            sw.WriteLine("\n\nimportFile \"" + path + "\"");
            foreach (Mesh m in meshes)
            {
                if (m.Material.MaterialMaps.Count > 0)
                {
                    if (m.Material.MaterialMaps[0] is FileMaterialMap)
                    {
                        FileMaterialMap fmm = (FileMaterialMap)m.Material.MaterialMaps[0];
                        string texture = (fmm.MapFilename.StartsWith(".") ? texturePath : "") + fmm.MapFilename.Replace("\\", "/");
                        sw.WriteLine("$mesh" + m.UID + ".material.diffuseMap = Bitmaptexture fileName:\"" + texture + "\"");
                        if (m.Material.MaterialMaps.Count > 1 && m.Material.MaterialMaps[1].Type == MaterialMap.MapType.Bump)
                        {
                            fmm = (FileMaterialMap)m.Material.MaterialMaps[1];
                            texture = (fmm.MapFilename.StartsWith(".") ? texturePath : "") + fmm.MapFilename.Replace("\\", "/");
                            sw.WriteLine("$mesh" + m.UID + ".material.bumpMap = Bitmaptexture fileName:\"" + texture + "\"");
                        }
                    }
                    else
                        throw new NotImplementedException();
                }
            }
            int count = 1;
            foreach (Object obj in externalObjects)
            {
                WriteExternalObjectInMaxScriptFile(obj, sw, texturePath, obj.Name + "_id" + count + "_");
                sw.WriteLine(")\n");
                ++count;
            }
            sw.Close();
            #endregion
        }

        private static void WriteTransformedObjectInstance(List<Mesh> meshes, TransformedObjectInstance toi, XmlTextWriter w)
        {
            List<AbstractNode> nodes = new List<AbstractNode>();
            foreach (AbstractNode an in toi.TransformationNodes)
                nodes.Add(an);
            WriteTransformedObjectInstance(nodes, meshes, toi, w);
        }

        private static void WriteTransformedObjectInstance(List<AbstractNode> nodes, List<Mesh> meshes, TransformedObjectInstance toi, XmlTextWriter w)
        {
            w.WriteStartElement("node");
            {
                for(int i = toi.TransformationNodes.Count - 1; i >= 0; --i)
                {
                    AbstractNode node = toi.TransformationNodes[i];
                    w.WriteStartElement("translate"); w.WriteString("" + node.Position.X + " " + node.Position.Y + " " + node.Position.Z); w.WriteEndElement();
                    w.WriteStartElement("rotate"); w.WriteString("0 0 1 " + node.Rotation.Z / (float)Math.PI * 180); w.WriteEndElement();
                    w.WriteStartElement("rotate"); w.WriteString("0 1 0 " + node.Rotation.Y / (float)Math.PI * 180); w.WriteEndElement();
                    w.WriteStartElement("rotate"); w.WriteString("1 0 0 " + node.Rotation.X / (float)Math.PI * 180); w.WriteEndElement();
                    w.WriteStartElement("scale"); w.WriteString("" + node.Scalation.X + " " + node.Scalation.Y + " " + node.Scalation.Z); w.WriteEndElement();
                }
                foreach (Mesh m in meshes)
                    WriteMeshInstance(m, w);
            }
            w.WriteEndElement();

            //if (nodes.Count > 0)
            //{
            //    AbstractNode node = nodes[nodes.Count - 1];
            //    w.WriteStartElement("node");
            //    {
            //        //w.WriteAttributeString("id", "mesh" + mesh.UID);
            //        //w.WriteAttributeString("name", "node" + mesh.UID);
            //        w.WriteStartElement("translate"); w.WriteString("" + node.Position.X + " " + node.Position.Y + " " + node.Position.Z); w.WriteEndElement();
            //        w.WriteStartElement("rotate"); w.WriteString("0 0 1 " + node.Rotation.Z); w.WriteEndElement();
            //        w.WriteStartElement("rotate"); w.WriteString("0 1 0 " + node.Rotation.Y); w.WriteEndElement();
            //        w.WriteStartElement("rotate"); w.WriteString("1 0 0 " + node.Rotation.X); w.WriteEndElement();
            //        w.WriteStartElement("scale"); w.WriteString("" + node.Scalation.X + " " + node.Scalation.Y + " " + node.Scalation.Z); w.WriteEndElement();

            //        nodes.RemoveAt(nodes.Count - 1);
            //        WriteTransformedObjectInstance(nodes,meshes, toi, w);
            //    }
            //    w.WriteEndElement();
            //}
            //else
            //{
            //    foreach (Mesh m in meshes)
            //        WriteMeshInstance(m, w);
            //}
        }

        private static void WriteExternalObjectInMaxScriptFile(Node obj, StreamWriter w, string path, string name)
        {
            if (obj is ExternalObject)
            {
                ExternalObject eo = (ExternalObject)obj;
                string s = eo.m_filename.Replace("\\", "/");
                if (!s.StartsWith("."))
                    s = "..\\models\\" + s.Substring(s.LastIndexOf('/'));
                //w.WriteLine("newObject = importFile2 \"" + path + s + "\" \"" + name + "\"");
                ////w.WriteLine("newObject.pivot = [0, 0, 0]");
                //w.WriteLine("if newObject != undefined then\n(");
                w.WriteLine("tname = \"" + name + "\"");
                w.WriteLine("actionMan.ExecuteAction 0 \"40021\" -- select all");
                w.WriteLine("max freeze selection");
                w.WriteLine("newObject = #()");
                w.WriteLine("");
                w.WriteLine("importFile \"" + path + s + "\" #noprompt");
                w.WriteLine("count = 1");
                w.WriteLine("actionMan.ExecuteAction 0 \"40021\" -- select all again");
                w.WriteLine("tempList = #()");
                w.WriteLine("append tempList $");
                w.WriteLine("for i = 1 to tempList.count do");
                w.WriteLine("(");
                w.WriteLine("\tif tempList[i] != undefined then");
                w.WriteLine("\t(");
                w.WriteLine("\t\ttempList[i].name = tname + (count as String)");
                w.WriteLine("\t\tcount = count + 1");
                w.WriteLine("\t)");
                w.WriteLine(")");
                w.WriteLine("actionMan.ExecuteAction 0 \"40021\" -- select all again");
                w.WriteLine("if $ != undefined then");
                w.WriteLine("(");
                w.WriteLine("\tnewObject = group $");
                w.WriteLine(")");
                w.WriteLine("else");
                w.WriteLine("(");
                w.WriteLine("\tnewObject = $");
                w.WriteLine(")");
                w.WriteLine("if newObject != undefined then\n(");
            }
            else
            {
                Node subObj = ((Object)obj).GetNodeAt(0);
                if (subObj is Object && ((Object)subObj).Name != "")
                    name = ((Object)subObj).Name + "_" + name;
                WriteExternalObjectInMaxScriptFile(subObj, w, path, name);
            }
            w.WriteLine("\tnewObject.pivot = [0, 0, 0]");
            if (obj.Scalation.X != 1 || obj.Scalation.Y != 1 || obj.Scalation.Z != 1)
                w.WriteLine("\tscale newObject [" + (float)obj.Scalation.X + ", " + (float)obj.Scalation.Z + ", " + (float)obj.Scalation.Y + "]");
            if (obj.Rotation.Z != 0)
                w.WriteLine("\trotate newObject (angleaxis " + (float)(obj.Rotation.Z / Math.PI * 180) + " [0,1,0])");
            if (obj.Rotation.Y != 0)
                w.WriteLine("\trotate newObject (angleaxis " + (float)-(obj.Rotation.Y / Math.PI * 180) + " [0,0,1])");
            if (obj.Rotation.X != 0)
                w.WriteLine("\trotate newObject (angleaxis " + (float)(obj.Rotation.X / Math.PI * 180) + " [1,0,0])");
            if (obj.Position.X != 0 || obj.Position.Y != 0 || obj.Position.Z != 0)
                w.WriteLine("\tmove newObject [" + (float)obj.Position.X + ", " + (float)obj.Position.Z + ", " + (float)obj.Position.Y + "]");
        }

        #region Collada export write functions
        private static void WriteNode(Mesh mesh, System.Xml.XmlTextWriter w)
        {
            w.WriteStartElement("node");
            {
                w.WriteAttributeString("id", "mesh" + mesh.UID);
                w.WriteAttributeString("name", "node" + mesh.UID);
                w.WriteStartElement("translate"); w.WriteString("0 0 0"); w.WriteEndElement();
                w.WriteStartElement("rotate"); w.WriteString("0 0 1 0"); w.WriteEndElement();
                w.WriteStartElement("rotate"); w.WriteString("0 1 0 0"); w.WriteEndElement();
                w.WriteStartElement("rotate"); w.WriteString("1 0 0 0"); w.WriteEndElement();
                w.WriteStartElement("scale"); w.WriteString("1 1 1"); w.WriteEndElement();
                WriteMeshInstance(mesh, w);
            }
            w.WriteEndElement();
        }

        private static void WriteMeshInstance(Mesh mesh, System.Xml.XmlTextWriter w)
        {
            w.WriteStartElement("instance_geometry");
            {
                w.WriteAttributeString("url", "#ID" + mesh.UID);

                w.WriteStartElement("bind_material");
                {
                    w.WriteStartElement("technique_common");
                    {
                        w.WriteStartElement("instance_material");
                        {
                            w.WriteAttributeString("symbol", mesh.Material.Name);
                            w.WriteAttributeString("target", "#" + mesh.Material.Name);
                        }
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                }
                w.WriteEndElement();
            }
            w.WriteEndElement();
        }

        private static void WriteGeometry(Mesh mesh, System.Xml.XmlTextWriter w)
        {
            w.WriteStartElement("geometry");
            {
                w.WriteAttributeString("id", "ID" + mesh.UID);
                w.WriteAttributeString("name", mesh.m_name);

                WriteMesh(mesh, w);
            }
            w.WriteEndElement();
        }

        private static void WriteMesh(Mesh mesh, System.Xml.XmlTextWriter w)
        {
            w.WriteStartElement("mesh");
            {
                w.WriteStartElement("source");
                {
                    w.WriteAttributeString("id", "ID" + mesh.UID + "-Pos");
                    w.WriteStartElement("float_array");
                    {
                        w.WriteAttributeString("id", "ID" + mesh.UID + "-Pos-array");
                        w.WriteAttributeString("count", "" + mesh.smesh.vertexBuffer.CurrentSize() * 3);
                        w.WriteString("\n");
                        for (int i = 0; i < mesh.smesh.vertexBuffer.CurrentSize(); ++i)
                        {
                            w.WriteString((float)mesh.smesh.vertexBuffer.array[i].m_position.X + " " + (float)mesh.smesh.vertexBuffer.array[i].m_position.Y + " " + (float)/*-*/mesh.smesh.vertexBuffer.array[i].m_position.Z + "\n");
                        }
                    }
                    w.WriteEndElement();
                    w.WriteStartElement("technique_common");
                    {
                        w.WriteStartElement("accessor");
                        {
                            w.WriteAttributeString("source", "#ID" + mesh.UID + "-Pos-array");
                            w.WriteAttributeString("count", "" + mesh.smesh.vertexBuffer.CurrentSize());
                            w.WriteAttributeString("stride", "3");

                            string[] temp = { "X", "Y", "Z" };
                            for (int i = 0; i < 3; ++i)
                            {
                                w.WriteStartElement("param");
                                {
                                    w.WriteAttributeString("name", temp[i]);
                                    w.WriteAttributeString("type", "float");
                                }
                                w.WriteEndElement();
                            }
                        }
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                }
                w.WriteEndElement();

                w.WriteStartElement("source");
                {
                    w.WriteAttributeString("id", "ID" + mesh.UID + "-Normal");
                    w.WriteStartElement("float_array");
                    {
                        w.WriteAttributeString("id", "ID" + mesh.UID + "-Normal-array");
                        w.WriteAttributeString("count", "" + mesh.smesh.vertexBuffer.CurrentSize() * 3);
                        w.WriteString("\n");
                        for (int i = 0; i < mesh.smesh.vertexBuffer.CurrentSize(); ++i)
                        {
                            w.WriteString((float)-mesh.smesh.vertexBuffer.array[i].m_normal.X + " " + (float)-mesh.smesh.vertexBuffer.array[i].m_normal.Y + " " + (float)-mesh.smesh.vertexBuffer.array[i].m_normal.Z + "\n");
                        }
                    }
                    w.WriteEndElement();
                    w.WriteStartElement("technique_common");
                    {
                        w.WriteStartElement("accessor");
                        {
                            w.WriteAttributeString("source", "#ID" + mesh.UID + "-Normal-array");
                            w.WriteAttributeString("count", "" + mesh.smesh.vertexBuffer.CurrentSize());
                            w.WriteAttributeString("stride", "3");

                            string[] temp = { "X", "Y", "Z" };
                            for (int i = 0; i < 3; ++i)
                            {
                                w.WriteStartElement("param");
                                {
                                    w.WriteAttributeString("name", temp[i]);
                                    w.WriteAttributeString("type", "float");
                                }
                                w.WriteEndElement();
                            }
                        }
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                }
                w.WriteEndElement();

                w.WriteStartElement("source");
                {
                    w.WriteAttributeString("id", "ID" + mesh.UID + "-UV0");
                    w.WriteStartElement("float_array");
                    {
                        w.WriteAttributeString("id", "ID" + mesh.UID + "-UV0-array");
                        w.WriteAttributeString("count", "" + mesh.smesh.vertexBuffer.CurrentSize() * 2);
                        w.WriteString("\n");
                        for (int i = 0; i < mesh.smesh.vertexBuffer.CurrentSize(); ++i)
                        {
                            w.WriteString((float)mesh.smesh.vertexBuffer.array[i].m_textureUV.X + " " + (float)mesh.smesh.vertexBuffer.array[i].m_textureUV.Y + "\n");
                        }
                    }
                    w.WriteEndElement();
                    w.WriteStartElement("technique_common");
                    {
                        w.WriteStartElement("accessor");
                        {
                            w.WriteAttributeString("source", "#ID" + mesh.UID + "-UV0-array");
                            w.WriteAttributeString("count", "" + mesh.smesh.vertexBuffer.CurrentSize());
                            w.WriteAttributeString("stride", "2");

                            string[] temp = { "S", "T" };
                            for (int i = 0; i < 2; ++i)
                            {
                                w.WriteStartElement("param");
                                {
                                    w.WriteAttributeString("name", temp[i]);
                                    w.WriteAttributeString("type", "float");
                                }
                                w.WriteEndElement();
                            }
                        }
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                }
                w.WriteEndElement();

                w.WriteStartElement("vertices");
                {
                    w.WriteAttributeString("id", "ID" + mesh.UID + "-Vtx");
                    
                    w.WriteStartElement("input");
                    {
                        w.WriteAttributeString("semantic", "POSITION");
                        w.WriteAttributeString("source", "#ID" + mesh.UID + "-Pos");
                    }
                    w.WriteEndElement();
                }
                w.WriteEndElement();

                w.WriteStartElement("polygons");
                {
                    w.WriteAttributeString("count", "" + mesh.NrOfTriangles());
                    w.WriteAttributeString("material", mesh.Material.Name);

                    w.WriteStartElement("input");
                    {
                        w.WriteAttributeString("semantic", "VERTEX");
                        w.WriteAttributeString("source", "#ID" + mesh.UID + "-Vtx");
                        w.WriteAttributeString("offset", "0");
                    }
                    w.WriteEndElement();

                    w.WriteStartElement("input");
                    {
                        w.WriteAttributeString("semantic", "NORMAL");
                        w.WriteAttributeString("source", "#ID" + mesh.UID + "-Normal");
                        w.WriteAttributeString("offset", "1");
                    }
                    w.WriteEndElement();

                    w.WriteStartElement("input");
                    {
                        w.WriteAttributeString("semantic", "TEXCOORD");
                        w.WriteAttributeString("source", "#ID" + mesh.UID + "-UV0");
                        w.WriteAttributeString("offset", "2");
                    }
                    w.WriteEndElement();

                    int count = 0;
                    for (int i = 0; i < mesh.NrOfTriangles(); ++i)
                    {
                        w.WriteStartElement("p");
                        {
                            w.WriteString(" " + mesh.m_indexBuffer[count + 0]);
                            w.WriteString(" " + mesh.m_indexBuffer[count + 0]);
                            w.WriteString(" " + mesh.m_indexBuffer[count + 0]);
                            w.WriteString(" " + mesh.m_indexBuffer[count + 1]);
                            w.WriteString(" " + mesh.m_indexBuffer[count + 1]);
                            w.WriteString(" " + mesh.m_indexBuffer[count + 1]);
                            w.WriteString(" " + mesh.m_indexBuffer[count + 2]);
                            w.WriteString(" " + mesh.m_indexBuffer[count + 2]);
                            w.WriteString(" " + mesh.m_indexBuffer[count + 2]);
                            count += 3;
                        }
                        w.WriteEndElement();
                    }
                }
                w.WriteEndElement();

            }
            w.WriteEndElement();
        }

        private static void WriteMaterial(Material material, System.Xml.XmlTextWriter w)
        {
            w.WriteStartElement("material");
            {
                w.WriteAttributeString("id", material.Name);
                w.WriteStartElement("instance_effect");
                {
                    w.WriteAttributeString("url", "#" + material.Name + "_effect");
                }
                w.WriteEndElement();

            }
            w.WriteEndElement();
        }

        private static void WriteEffect(Mesh mesh, Material material, System.Xml.XmlTextWriter w)
        {
            w.WriteStartElement("effect");
            {
                w.WriteAttributeString("id", material.Name + "_effect");

                w.WriteStartElement("profile_COMMON");
                {
                    w.WriteStartElement("technique");
                    {
                        w.WriteAttributeString("sid", material.Name + "p1");

                        w.WriteStartElement("phong");
                        {
                            WriteColor("ambient", material.Ambient, w);
                            if (material.MaterialMaps.Count == 0)
                                WriteColor("diffuse", material.Diffuse, w);
                            else
                                WriteTexture("diffuse", "map_" + mesh.UID + "-image", w);
                            WriteColor("specular", material.m_specularColor, w);

                            w.WriteStartElement("transparency");
                            {
                                w.WriteStartElement("float");
                                {
                                    w.WriteString("" + (float)material.Transparency);
                                }
                                w.WriteEndElement();
                            }
                            w.WriteEndElement();
                        }
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                }
                w.WriteEndElement();

            }
            w.WriteEndElement();
        }

        private static void WriteTexture(string p, string image, XmlTextWriter w)
        {
            w.WriteStartElement(p);
            {
                w.WriteStartElement("texture");
                {
                    w.WriteAttributeString("texture", image);
                    w.WriteAttributeString("texcoord", "CHANNEL0");
                    w.WriteStartElement("extra");
                    {
                        w.WriteStartElement("technique");
                        {
                            w.WriteAttributeString("profile", "MAYA");
                            w.WriteStartElement("wrapU");
                            {
                                w.WriteAttributeString("sid", "wrapU0");
                                w.WriteString("TRUE");
                            }
                            w.WriteEndElement();
                            w.WriteStartElement("wrapV");
                            {
                                w.WriteAttributeString("sid", "wrapV0");
                                w.WriteString("TRUE");
                            }
                            w.WriteEndElement();
                            w.WriteStartElement("blend_mode");
                            {
                                w.WriteString("ADD");
                            }
                            w.WriteEndElement();
                        }
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                }
                w.WriteEndElement();
            }
            w.WriteEndElement();
        }

        private static void WriteColor(string p, Vec4 color, System.Xml.XmlTextWriter w)
        {
            if (color == null)
                return;
            w.WriteStartElement(p);
            {
                w.WriteStartElement("color");
                {
                    w.WriteString("" + color.X + " " + color.Y + " " + color.Z + " " + color.W);
                }
                w.WriteEndElement();
            }
            w.WriteEndElement();
        }
        #endregion

        internal protected override Node CloneInternal(Node parent)
        {
            return new Mesh(this, parent);
        }

        public void AddIndices(params int[] indices)
        {
            foreach (int i in indices)
                m_indexBuffer.Add(i);
        }

        public void RecalculateNormals()
        {
            RecalculateNormals(false);
        }

        public override void RecalculateNormals(bool flip)
        {
            throw new NotImplementedException();
            ////--- change taking into account struct!
            //Dictionary<VertexInfoSimple, int> timesUsedInFace = new Dictionary<VertexInfoSimple, int>();
            //foreach (List<int> face in GetTriangles())
            //{
            //    VertexInfoSimple[] v = new VertexInfoSimple[3];
            //    for (int i = 0; i < 3; ++i)
            //        v[i] = smesh.vertexBuffer.array[face[i]];
            //    Vec3 normal = flip ? (v[2].m_position.Vec3() - v[0].m_position.Vec3()).Cross(v[1].m_position.Vec3() - v[0].m_position.Vec3()).normalize() :
            //                            (v[1].m_position.Vec3() - v[0].m_position.Vec3()).Cross(v[2].m_position.Vec3() - v[0].m_position.Vec3()).normalize();
            //    for (int i = 0; i < 3; ++i)
            //        SetNormal(v[i], timesUsedInFace, normal);
            //}
        }

        private void SetNormal(VertexInfoSimple vertexInfo, Dictionary<VertexInfoSimple, int> timesUsedInFace, Vec3 normal)
        {
            int t = 0;
            if (timesUsedInFace.ContainsKey(vertexInfo))
            {
                t = timesUsedInFace[vertexInfo];
                timesUsedInFace[vertexInfo] = t + 1;
                vertexInfo.m_normal.Set(((((float)t) * vertexInfo.m_normal.Vec3()) + normal).normalize());
            }
            else
            {
                timesUsedInFace.Add(vertexInfo, 1);
                vertexInfo.m_normal.Set(normal);
            }
        }

        protected override void GetMeshInstancesInternal(List<TransformedObjectInstance> list)
        {
            return;
        }

        public void ExportToObj(string path)
        {
            StreamWriter sw = new StreamWriter(path, false, Encoding.ASCII);
            sw.WriteLine("# obj file created with Common.Geometry.Mesh ExportToObj function\n# If it doesn't work, you're on your own!\n");

            for (int i = 0; i < smesh.vertexBuffer.CurrentSize(); ++i)
            {
                sw.WriteLine("v " + smesh.vertexBuffer.array[i].m_position.X + " " + smesh.vertexBuffer.array[i].m_position.Y + " " + smesh.vertexBuffer.array[i].m_position.Z);
                sw.WriteLine("vn " + smesh.vertexBuffer.array[i].m_normal.X + " " + smesh.vertexBuffer.array[i].m_normal.Y + " " + smesh.vertexBuffer.array[i].m_normal.Z);
                sw.WriteLine("vt " + smesh.vertexBuffer.array[i].m_textureUV.X + " " + smesh.vertexBuffer.array[i].m_textureUV.Y);
            }

            foreach (List<int> triangle in GetTriangles())
            {
                sw.Write("f");
                foreach (int i in triangle)
                    sw.Write(" " + (i + 1));
                sw.WriteLine("");
            }
            sw.Close();
        }

        public override void Clean()
        {
            this.smesh.vertexBuffer.Clear();
        }

        public override bool IsEmpty()
        {
            return m_indexBuffer.Count == 0;
        }
    }
}
