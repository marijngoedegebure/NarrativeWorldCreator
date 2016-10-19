using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Common.Geometry
{
    public class Object : Node
    {
        string m_name = "";
        private List<Node> m_subnodes;
        public bool m_wireframe = false;
        public bool m_transparent = false;
        private Material overrideMaterial = null;

        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
            }
        }

        public Object()
            : base(null)
        {
            m_subnodes = new List<Node>();
        }

        public Object(AbstractNode node)
            : base(null, node)
        {
            m_subnodes = new List<Node>();
        }

        public Object(Node parent, AbstractNode node)
            : base(parent, node)
        {
            m_subnodes = new List<Node>();
        }

        public Object(string name)
            : base(null)
        {
            m_subnodes = new List<Node>();
            Name = name;
        }

        public Object(string name, Material overrideMaterial) 
            : base(null)
        {
            this.overrideMaterial = overrideMaterial;
            Name = name;
            m_subnodes = new List<Node>();
        }

        public override NodeType IsType()
        {
            return NodeType.OBJECT;
        }

        public override uint NrOfTriangles()
        {
            uint temp = 0;
            foreach (Node n in m_subnodes)
                temp += n.NrOfTriangles();
            return temp;
        }

        public override void GetAllMaterialsInternal(List<Material> list)
        {
            foreach (Node n in m_subnodes)
                n.GetAllMaterialsInternal(list);
        }

        protected internal override void ImproveMeshesNewInternal(ImprovedMeshContainer c)
        {
            int[] indices = c.GetCurrentMeshVertexBufferSizes();

            foreach (Node n in m_subnodes)
                n.ImproveMeshesNewInternal(c);

            c.Transform(indices, GetTransformation(), GetRotationTransformation());

            indices = null;
        }

        protected override void ImproveMeshesInternal(List<Mesh> currentMeshes)
        {
            //if (currentMeshes.Count == 0)
            //    currentMeshes.Add(new Mesh(1000));
            //foreach (Node n in m_subnodes)
            //    n.AddImprovedMeshes(currentMeshes);
            //return;
            Matrix4 transformation = GetTransformation();
            Matrix4 rotationTransformation = GetRotationTransformation();
            foreach (Node n in m_subnodes)
            {
                List<Mesh> l = n.ImproveMeshes();
                foreach (Mesh m in l)
                {
                    //Mesh copyMesh = new Mesh(m);
                    for (int i = 0; i < m.smesh.vertexBuffer.CurrentSize(); ++i)
                    {
                        m.smesh.vertexBuffer.array[i].Transform(transformation, rotationTransformation);
                        //m.smesh.vertexBuffer[i].Transform(transformation, rotationTransformation);
                        //VertexInfoSimple vi = m.smesh.vertexBuffer[i];
                        //vi.m_position.Set(vi.m_position.Vec3() * transformation);
                        //vi.m_normal.Set(vi.m_normal.Vec3() * rotationTransformation);
                        //vi.m_normal.Normalize();
                        ////vi.m_position += m_position;
                        //m.smesh.vertexBuffer[i] = vi;
                    }

                    Mesh mesh = Mesh.GetMeshWithMaterial(currentMeshes, m.Material);
                    if (mesh == null)
                    {
                        mesh = new Mesh(1000, m.Material);
                        mesh.m_name = m.m_name;
                        currentMeshes.Add(mesh);
                    }
                    mesh.Add(m);
                    m.Clean();
                }
            }
        }

        protected override IEnumerable<Mesh> GetOriginalMeshesInternal()
        {
            Matrix4 transformation = GetTransformation();
            Matrix4 rotationTransformation = GetRotationTransformation();
            foreach (Node n in m_subnodes)
            {
                foreach (Mesh m in n.GetOriginalMeshes())
                {
                    for (int i = 0; i < m.smesh.vertexBuffer.CurrentSize(); ++i)
                    {
                        m.smesh.vertexBuffer.array[i].m_position.Transform(transformation);
                        m.smesh.vertexBuffer.array[i].m_normal.Transform(rotationTransformation);
                        m.smesh.vertexBuffer.array[i].m_normal.Normalize();
                    }

                    yield return m;
                }
            }
        }

        protected override IEnumerable<Mesh> GetSeparateMeshesInternal()
        {
            Matrix4 transformation = GetTransformation();
            Matrix4 rotationTransformation = GetRotationTransformation();
            foreach (Node n in m_subnodes)
            {
                foreach (Mesh m in n.GetSeparateMeshes())
                {
                    for (int i = 0; i < m.smesh.vertexBuffer.CurrentSize(); ++i)
                    {
                        m.smesh.vertexBuffer.array[i].m_position.Transform(transformation);
                        m.smesh.vertexBuffer.array[i].m_normal.Transform(rotationTransformation);
                        m.smesh.vertexBuffer.array[i].m_normal.Normalize();
                    }

                    yield return m;
                }
            }
        }

        public Object GetSubobjectByName(string p)
        {
            foreach (Node n in m_subnodes)
            {
                if (n is Object)
                {
                    Object o = (Object)n;
                    if (o.Name == p)
                        return o;
                }
            }
            return null;
        }

        public override void GetBoundingBoxInternal(Matrix4 transformation, Box box)
        {
            foreach (Node n in m_subnodes)
                n.GetBoundingBoxInternal(GetTransformation() * transformation, box);
        }

        protected override bool HandleNodeCrawl(NodeCrawler crawler, object parameters, ref object returnValue)
        {
            foreach (Node n in m_subnodes)
            {
                if (n.Crawl(crawler, parameters, ref returnValue))
                    return true;
            }
            return false;
        }

        public override int FirstLevelOccuranceOf(string name, int level)
        {
            if (Name.ToLower().Contains(name))
                return level;
            foreach (Node n in m_subnodes)
            {
                int ret = n.FirstLevelOccuranceOf(name, level + 1);
                if (ret != -1)
                    return ret;
            }
            return -1;
        }

        public override void GetAllInLevelContaining(List<Node> list, int level, string str, int currentLevel)
        {
            if (currentLevel == level)
            {
                if (Name.ToLower().Contains(str))
                    list.Add(this);
                return;
            }
            if (currentLevel > level)
                return;
            foreach (Node n in m_subnodes)
                n.GetAllInLevelContaining(list, level, str, currentLevel + 1);
        }

        public void AddNode(Node node)
        {
            if (node == null)
                return;
            node.Parent = this;
            m_subnodes.Add(node);
            if (this.overrideMaterial != null)
            {
                if (node is Object)
                {
                    Object o = (Object)node;
                    if (o.overrideMaterial == null)
                        o.overrideMaterial = this.overrideMaterial;
                }
                else if (node is Mesh)
                    ((Mesh)node).Material = this.overrideMaterial;
            }
        }

        public int GetNrOfNodes()
        {
            return m_subnodes.Count;
        }

        public void RemoveNode(Node node)
        {
            if (node == null)
                return;
            node.Parent = null;
            m_subnodes.Remove(node);
        }

        public Node GetNodeAt(int index)
        {
            return m_subnodes[index];
        }

        public override void Save(System.IO.BinaryWriter wr)
        {
            wr.Write('o');
            base.Save(wr);
            wr.Write(this.Name);
            wr.Write(this.m_subnodes.Count);
            foreach (Node n in this.m_subnodes)
                n.Save(wr);
            wr.Write(this.m_transparent);
            wr.Write(this.m_wireframe);
        }

        internal Object(Node parent, System.IO.BinaryReader br)
            : base(parent, br)
        {
            this.Name = br.ReadString();
            int count = br.ReadInt32();
            this.m_subnodes = new List<Node>();
            for (int i = 0; i < count; ++i)
                m_subnodes.Add(Node.Load(this, br));
            this.m_transparent = br.ReadBoolean();
            this.m_wireframe = br.ReadBoolean();
        }

        internal protected override void GetExternalObjectsInternal(List<Object> list)
        {
            foreach (Node subnode in this.m_subnodes)
                subnode.GetExternalObjectsInternal(list);
        }

        public override void Dispose()
        {
            foreach (Node node in m_subnodes)
                node.Dispose();
            m_subnodes.Clear();
            m_subnodes = null;
            //base.Dispose();
            //GC.SuppressFinalize(this);
        }

        public override bool ContainsId(long id)
        {
            if (base.ContainsId(id))
                return true;
            foreach (Node node in m_subnodes)
                if (node.ContainsId(id))
                    return true;
            return false;
        }

        public override void WriteDebug(string p, System.IO.StreamWriter sw)
        {
            sw.WriteLine(p + " object:");
            foreach (Node n in m_subnodes)
                n.WriteDebug(p + " .", sw);
        }

        internal protected override Node CloneInternal(Node parent)
        {
            Object obj = new Object(parent, this);
            foreach (Node n in m_subnodes)
                obj.m_subnodes.Add(n.CloneInternal(obj));
            obj.m_name = this.m_name + "Clone" + UID;
            obj.m_transparent = this.m_transparent;
            obj.m_wireframe = this.m_wireframe;
            obj.mask = this.mask;
            obj.overrideMaterial = this.overrideMaterial;
            return obj;
        }

        public static Node CreateCloneInWorldCoordinates(Node node)
        {
            return CreateCloneInWorldCoordinates(node, Matrix4.Identity, Matrix4.Identity);
        }

        private static Node CreateCloneInWorldCoordinates(Node node, Matrix4 transf, Matrix4 rotTransf)
        {
            if (node.IsType() == NodeType.CUSTOM_ROTATION_OBJECT)
            {
                return CreateCloneInWorldCoordinates(((CustomRotationObject)node).ChildNode, node.GetTransformation() * transf,
                                                                                                node.GetRotationTransformation() * rotTransf);
            }
            else if (node.IsType() == NodeType.EXTERNAL_OBJECT)
            {
                throw new NotImplementedException();
            }
            else if (node.IsType() == NodeType.OBJECT)
            {
                Object obj = new Object(((Object)node).Name);
                foreach (Node n in ((Object)node).m_subnodes)
                    obj.AddNode(CreateCloneInWorldCoordinates(n, node.GetTransformation() * transf, node.GetRotationTransformation() * rotTransf));
                return obj;
            }
            else if (node.IsType() == NodeType.MESH)
            {
                Matrix4 t = node.GetTransformation() * transf;
                Matrix4 r = node.GetRotationTransformation() * rotTransf;
                Mesh o = (Mesh)node;
                Mesh m = new Mesh(o, null);
                for (int i = 0; i < m.smesh.vertexBuffer.CurrentSize(); ++i)
                {
                    m.smesh.vertexBuffer.array[i].m_position.Transform(t);
                    m.smesh.vertexBuffer.array[i].m_normal.Transform(r);
                }
                return m;
            }
            else
                throw new NotImplementedException();
        }

        public override void RecalculateNormals(bool flip)
        {
            foreach (Node n in m_subnodes)
                n.RecalculateNormals(flip);
        }

        public void ClearNodes()
        {
            m_subnodes.Clear();
        }

        protected override void GetMeshInstancesInternal(List<TransformedObjectInstance> list)
        {
            foreach (Node n in m_subnodes)
                foreach (TransformedObjectInstance ti in n.GetMeshInstances())
                    list.Add(new TransformedObjectInstance(ti, this));
        }

        public override void Clean()
        {
            foreach (Node child in m_subnodes)
                child.Clean();
        }

        public override bool IsEmpty()
        {
            if (m_subnodes.Count == 0)
                return true;
            foreach (Node n in m_subnodes)
                if (!n.IsEmpty())
                    return false;
            return true;
        }
    }
}
