using System;
using System.Collections.Generic;

using System.Text;

namespace Common.Geometry 
{
    public class CustomRotationObject : Node
    {
        private Node childnode;
        Matrix4 customRotationMatrix, customRotationXmZYMatrix;

        public Node ChildNode { get { return childnode; } }
        public Matrix4 CustomRotationMatrix { get { return customRotationMatrix; } }

        public CustomRotationObject(Node childnode, float angle, Vec3 axis)
            : base(null) 
        {
            this.childnode = childnode;
            this.childnode.Parent = this;
            this.customRotationMatrix = Matrix4.ArbitraryRotation(angle, axis);
            this.customRotationXmZYMatrix = Matrix4.ArbitraryRotation(-angle, new Vec3(axis.X, axis.Z, axis.Y));
            OverrideRotationMatrix(customRotationMatrix, customRotationXmZYMatrix);
        }

        private CustomRotationObject(Node childnode, Matrix4 rot, Matrix4 rotXmZY)
            : base(null) {
            this.childnode = childnode;
            this.childnode.Parent = this;
            this.customRotationMatrix = rot;
            this.customRotationXmZYMatrix = rotXmZY;
            OverrideRotationMatrix(customRotationMatrix, customRotationXmZYMatrix);
        }

        public override NodeType IsType() 
        {
            return NodeType.CUSTOM_ROTATION_OBJECT;
        }

        public override uint NrOfTriangles() 
        {
            return childnode.NrOfTriangles();
        }

        public override void GetAllMaterialsInternal(List<Material> list) 
        {
            childnode.GetAllMaterialsInternal(list);
        }

        protected override void ImproveMeshesInternal(List<Mesh> currentMeshes) 
        {
            List<Mesh> l = childnode.ImproveMeshes();
            foreach (Mesh m in l)
            {
                for (int i = 0; i < m.smesh.vertexBuffer.CurrentSize(); ++i)
                {
                    m.smesh.vertexBuffer.array[i].m_position.Transform(GetTransformation());
                    m.smesh.vertexBuffer.array[i].m_normal.Transform(GetRotationTransformation());
                    m.smesh.vertexBuffer.array[i].m_normal.Normalize();
                }

                Mesh mesh = Mesh.GetMeshWithMaterial(currentMeshes, m.Material);
                if (mesh == null) 
                {
                    mesh = new Mesh(1000, m.Material);
                    mesh.m_name = m.m_name;
                    currentMeshes.Add(mesh);
                }
                mesh.Add(m);
            }
        }

        protected override IEnumerable<Mesh> GetSeparateMeshesInternal()
        {
            foreach (Mesh m in childnode.GetSeparateMeshes())
            {
                for (int i = 0; i < m.smesh.vertexBuffer.CurrentSize(); ++i)
                {
                    m.smesh.vertexBuffer.array[i].m_position.Transform(GetTransformation());
                    m.smesh.vertexBuffer.array[i].m_normal.Transform(GetRotationTransformation());
                    m.smesh.vertexBuffer.array[i].m_normal.Normalize();
                }
                yield return m;
            }
        }
        protected override IEnumerable<Mesh> GetOriginalMeshesInternal()
        {
            foreach (Mesh m in childnode.GetOriginalMeshes())
            {
                for (int i = 0; i < m.smesh.vertexBuffer.CurrentSize(); ++i)
                {
                    m.smesh.vertexBuffer.array[i].m_position.Transform(GetTransformation());
                    m.smesh.vertexBuffer.array[i].m_normal.Transform(GetRotationTransformation());
                    m.smesh.vertexBuffer.array[i].m_normal.Normalize();
                }
                yield return m;
            }
        }

        public override void GetBoundingBoxInternal(Matrix4 transformation, Box box) 
        {
            childnode.GetBoundingBoxInternal(customRotationMatrix * transformation, box);
        }

        internal protected override void GetExternalObjectsInternal(List<Object> list)
        {
            childnode.GetExternalObjectsInternal(list);
        }

        internal protected override Node CloneInternal(Node parent)
        {
            return new CustomRotationObject(childnode.Clone(), new Matrix4(customRotationMatrix), new Matrix4(customRotationXmZYMatrix));
        }

        public override void RecalculateNormals(bool flip)
        {
            childnode.RecalculateNormals(flip);
        }

        protected override void GetMeshInstancesInternal(List<TransformedObjectInstance> list)
        {
            foreach (TransformedObjectInstance mi in childnode.GetMeshInstances())
            {
                throw new NotImplementedException();
                //list.Add(new TransformedObjectInstance(mi, customRotationMatrix, customRotationXmZYMatrix));
            }
        }

        public override bool IsEmpty()
        {
            return childnode.IsEmpty();
        }
    }
}
