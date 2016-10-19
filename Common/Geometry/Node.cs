using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Common.Geometry
{
    public enum NodeType { MESH, OBJECT, EXTERNAL_OBJECT, CUSTOM_ROTATION_OBJECT, OBJECT_REFERENCE, TRANSFORMATION_NODE }

    public class ImprovedMeshContainer
    {
        public List<Mesh> meshesInUse = new List<Mesh>();

        public Mesh GetMesh(Material material)
        {
            Mesh mesh = Mesh.GetMeshWithMaterial(meshesInUse, material);
            if (mesh == null)
            {
                mesh = new Mesh(100, material);
                meshesInUse.Add(mesh);
            }
            return mesh;
        }

        public int[] GetCurrentMeshVertexBufferSizes()
        {
            int[] arr = new int[meshesInUse.Count];
            int count = 0;
            foreach (Mesh m in meshesInUse)
                arr[count++] = m.smesh.vertexBuffer.CurrentSize();
            return arr;
        }

        public void Transform(int[] startArrays, Matrix4 transformation, Matrix4 rotationTransformation)
        {
            for (int i = 0; i < meshesInUse.Count; ++i)
            {
                int start = i < startArrays.Length ? startArrays[i] : 0;
                for (int j = start; j < meshesInUse[i].smesh.vertexBuffer.CurrentSize(); ++j)
                {
                    meshesInUse[i].smesh.vertexBuffer.array[j].Transform(transformation, rotationTransformation);
                }
            }
        }
    }

    public abstract class Node : AbstractNode, IDisposable
    {
        //static Dictionary<long, Node> nodesPerUID = new Dictionary<long, Node>();
        static Random r = new Random(123);
        private Node m_parent;
        public int mask = 0x3;

        public Node Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public delegate bool NodeCrawler(Node node, object parameters, ref object returnValue);

        private static long sUid = 0;

        private static long CreateUID()
        {
            return sUid++;
        }

        public abstract NodeType IsType();
        public abstract uint NrOfTriangles();
        public abstract void GetAllMaterialsInternal(List<Material> list);

        public void ImproveMeshesNew(ImprovedMeshContainer c)
        {
            ImproveMeshesNewInternal(c);
        }

        protected internal virtual void ImproveMeshesNewInternal(ImprovedMeshContainer c)
        {
            throw new NotImplementedException();
        }

        private long uid = CreateUID();
        public long UID { get { return uid; } }

        public Node(Node parent)
            : base()
        {
            m_parent = parent;
            //nodesPerUID.Add(uid, this);
        }
       
        public Node(Node parent, AbstractNode node)
            : base(node)
        {
            m_parent = parent;
            //nodesPerUID.Add(uid, this);
        }

        internal Node(Node parent, System.IO.BinaryReader br)
        {
            this.m_parent = parent;
            this.Position = new Vec3(br);
            this.Rotation = new Vec3(br);
            this.Scalation = new Vec3(br);
            //nodesPerUID.Add(uid, this);
        }

        public List<Material> GetAllMaterials()
        {
            List<Material> list = new List<Material>();
            GetAllMaterialsInternal(list);
            return list;
        }

        protected abstract void ImproveMeshesInternal(List<Mesh> currentMeshes);
        public abstract bool IsEmpty();

        public List<Mesh> ImproveMeshes()
        {
            List<Mesh> currentMeshes = new List<Mesh>();
            ImproveMeshesInternal(currentMeshes);
            return currentMeshes;
        }
        public void AddImprovedMeshes(List<Mesh> currentMeshList)
        {
            ImproveMeshesInternal(currentMeshList);
        }

        public IEnumerable<Mesh> GetOriginalMeshes()
        {
            foreach (Mesh m in GetOriginalMeshesInternal())
                yield return m;
        }

        public IEnumerable<Mesh> GetSeparateMeshes()
        {
            foreach (Mesh m in GetSeparateMeshesInternal())
                yield return m;
        }

        protected abstract IEnumerable<Mesh> GetSeparateMeshesInternal();
        protected abstract IEnumerable<Mesh> GetOriginalMeshesInternal();

        public abstract void GetBoundingBoxInternal(Matrix4 transformation, Box box);

        public Box GetBoundingBox(bool addTotalTransformation)
        {
            Box bb = new Box(Vec3.Max, Vec3.Min);
            if (addTotalTransformation)
                GetBoundingBoxInternal(GetParentTransformation(), bb);
            else
                GetBoundingBoxInternal(Matrix4.Identity, bb);
            return bb;
            //List<Mesh> list = this.ImproveMeshes();
            //foreach (Mesh m in list)
            //{
            //    foreach (VertexInfo vi in m.m_vertexBuffer)
            //    {
            //        if (vi.m_position.x < bb.Minimum.x)
            //            bb.Minimum.x = vi.m_position.x;
            //        if (vi.m_position.y < bb.Minimum.y)
            //            bb.Minimum.y = vi.m_position.y;
            //        if (vi.m_position.z < bb.Minimum.z)
            //            bb.Minimum.z = vi.m_position.z;
            //        if (vi.m_position.x > bb.Maximum.x)
            //            bb.Maximum.x = vi.m_position.x;
            //        if (vi.m_position.y > bb.Maximum.y)
            //            bb.Maximum.y = vi.m_position.y;
            //        if (vi.m_position.z > bb.Maximum.z)
            //            bb.Maximum.z = vi.m_position.z;
            //    }
            //}
            //return bb;
        }

        public object Crawl(NodeCrawler crawler, object parameters)
        {
            object ret = null;
            if (Crawl(crawler, parameters, ref ret))
                return ret;
            return ret;
        }

        public bool Crawl(NodeCrawler crawler, object parameters, ref object returnValue)
        {
            if (crawler(this, parameters, ref returnValue))
                return true;
            return HandleNodeCrawl(crawler, parameters, ref returnValue);
        }

        protected virtual bool HandleNodeCrawl(NodeCrawler crawler, object parameters, ref object returnValue)
        {
            return false;
        }

        public static bool CheckUID(Node node, object parameters, ref object returnValue)
        {
            long uid = (long)parameters;
            if (node.uid == uid)
            {
                returnValue = node;
                return true;
            }
            return false;
        }

        public static bool AddName(Node node, object parameters, ref object returnValue)
        {
            if (node is Object)
            {
                Object obj = (Object)node;
                if (obj.Name != "")
                {
                    List<string> names = (List<string>)parameters;
                    if (!names.Contains(obj.Name))
                        names.Add(obj.Name);
                }
            }
            return false;
        }

        public Node GetNodeWithUid(long uid)
        {
            return (Node)Crawl(Node.CheckUID, uid);
        }

        public List<string> GetAllObjectNames()
        {
            List<string> names = new List<string>();
            Crawl(Node.AddName, names);
            return names;
        }

        public virtual int FirstLevelOccuranceOf(string name, int level)
        {
            return -1;
        }

        public int FirstLevelOccuranceOf(string name)
        {
            return FirstLevelOccuranceOf(name, 0);
        }

        public virtual void GetAllInLevelContaining(List<Node> list, int level, string str,
                                                        int currentLevel)
        {
            //--- deliberately left empty
        }

        public List<Node> GetAllInLevelContaining(int level, string str)
        {
            List<Node> list = new List<Node>();
            GetAllInLevelContaining(list, level, str, 0);
            return list;
        }

        public Matrix4 GetParentTransformation()
        {
            if (m_parent != null)
                return m_parent.GetTransformation();
            return Matrix4.Identity;
        }

        public Matrix4 GetTotalTransformation()
        {
            if (m_parent != null)
                return GetTransformation() * m_parent.GetTotalTransformation();
            return GetTransformation();
        }

        public virtual void Save(System.IO.BinaryWriter wr)
        {
            Position.Save(wr);
            Rotation.Save(wr);
            Scalation.Save(wr);
        }

        public static Node Load(Node parent, System.IO.BinaryReader br)
        {
            char ch = br.ReadChar();
            switch (ch)
            {
                case 'o':
                    return new Object(parent, br);
                case 'm':
                    return new Mesh(parent, br);
                case 'e':
                    return new ExternalObject(parent, br);
            }
            throw new Exception("Error in Common.Object file!");
        }

        public void Save(string filename)
        {
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(System.IO.File.Create(filename));
            Save(bw);
            bw.Close();
        }

        public static Node Load(string filename)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.Open(BasicFunctionality.GetAppDirectory() + filename, System.IO.FileMode.Open));
            Node node = Load(null, br);
            br.Close();
            return node;
        }

        public override string ToString()
        {
            return "Node" + UID;
        }

        public List<TransformedObjectInstance> GetMeshInstances()
        {
            List<TransformedObjectInstance> list = new List<TransformedObjectInstance>();
            GetMeshInstancesInternal(list);
            return list;
        }

        protected abstract void GetMeshInstancesInternal(List<TransformedObjectInstance> list);

        public List<Object> GetExternalObjects()
        {
            List<Object> list = new List<Object>();
            GetExternalObjectsInternal(list);
            return list;
        }

        internal protected abstract void GetExternalObjectsInternal(List<Object> list);

        public Object GetNodeTree()
        {
            if (Parent == null)
                return new Object(this);
            else
            {
                Object obj = Parent.GetNodeTree();
                Object node = new Object(this);
                obj.AddNode(node);
                return node;
            }
        }

        public virtual bool ContainsId(long id)
        {
            return uid == id;
        }

        #region IDisposable Members

        public new virtual void Dispose()
        {
            base.Dispose();
            //GC.SuppressFinalize(this);
        }

        #endregion

        public virtual void WriteDebug(string p, System.IO.StreamWriter sw)
        {
            sw.WriteLine(p + " node: " + IsType().ToString());
        }

        public static Node GetFromUID(long uid)
        {
            throw new NotImplementedException();
            //Node ret;
            //if (!nodesPerUID.TryGetValue(uid, out ret))
            //    ret = null;
            //return ret;
        }


        public Node Clone()
        {
            return CloneInternal(null);
        }

        internal protected abstract Node CloneInternal(Node parent);

        public abstract void RecalculateNormals(bool flip);

        public virtual void Clean() { throw new NotImplementedException(); }

        public Object Wrap()
        {
            Object wrapper = new Object();
            wrapper.AddNode(this);
            return wrapper;
        }
    }
}
