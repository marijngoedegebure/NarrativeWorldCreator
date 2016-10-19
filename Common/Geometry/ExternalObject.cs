using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Common.Geometry
{
    public class ExternalObject : Node
    {
        public string m_filename;
        public Material m_material;
        Box boundingBox;

        public ExternalObject(string filename, Material mat, Box boundingbox)
            : base(null)
        {
            m_filename = filename;
            m_material = mat;
            this.boundingBox = boundingbox;
        }

        public ExternalObject(Node parent, ExternalObject copy, Box boundingbox)
            : base(parent, copy)
        {
            this.m_filename = copy.m_filename;
            this.m_material = copy.m_material;
            this.boundingBox = boundingbox;
        }

        public override NodeType IsType()
        {
            return NodeType.EXTERNAL_OBJECT;
        }

        public override uint NrOfTriangles()
        {
            return 0;
        }

        public override void GetAllMaterialsInternal(List<Material> list)
        {
            return;
        }

        protected override void ImproveMeshesInternal(List<Mesh> currentMeshes)
        {
            return;
        }

        protected override IEnumerable<Mesh> GetSeparateMeshesInternal()
        {
            yield break;
        }
        protected override IEnumerable<Mesh> GetOriginalMeshesInternal()
        {
            yield break;
        }

        public override void GetBoundingBoxInternal(Matrix4 transformation, Box box)
        {
            Vec3[, ,] points = boundingBox.GetPoints();
            for (int x = 0; x < 2; ++x)
                for (int y = 0; y < 2; ++y)
                    for (int z = 0; z < 2; ++z)
                        box.AddPointToBoundingBox((points[x, y, z] * transformation));
            //box.AddPointToBoundingBox(
            //return boundingBox;
            //throw new Exception("The method or operation is not implemented.");
        }

        public override void Save(System.IO.BinaryWriter wr)
        {
            wr.Write('e');
            base.Save(wr);
            this.boundingBox.Save(wr);
            wr.Write(this.m_filename);
            this.m_material.Save(wr);
            //throw new Exception("Deprecated!");
            //wr.Write('e');
            //base.Save(wr);
            //wr.Write(m_filename);
            //wr.Write(m_material);
        }

        internal ExternalObject(Node parent, System.IO.BinaryReader br) : base(parent, br)
        {
            this.boundingBox = Box.Load(br);
            this.m_filename = br.ReadString();
            this.m_material = new Material(br);
            //throw new Exception("Deprecated!");
            //m_filename = br.ReadString();
            //m_material = br.ReadString();
        }

        internal protected override void GetExternalObjectsInternal(List<Object> list)
        {
            Object node = this.Parent.GetNodeTree();
            node.AddNode(new ExternalObject(node, this, boundingBox));
            Object p = node;
            while (p.Parent != null)
                p = (Object)p.Parent;
            list.Add(p);
        }

        internal protected override Node CloneInternal(Node parent)
        {
            return new ExternalObject(parent, this, this.boundingBox);
        }

        public override void RecalculateNormals(bool flip)
        {
            //--- Deliberately left empty
        }

        protected override void GetMeshInstancesInternal(List<TransformedObjectInstance> list)
        {
            //--- Deliberately left empty
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}
