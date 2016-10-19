using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Geometry
{
    public class ObjectReference
    {
        Object obj;

        public Object Obj { get { return obj; } }

        public ObjectReference(Object obj)
        {
            this.obj = obj;
        }

        internal uint NrOfTriangles()
        {
            return obj.NrOfTriangles();
        }

        internal void GetAllMaterialsInternal(List<Material> list)
        {
            obj.GetAllMaterialsInternal(list);
        }

        internal List<Mesh> ImproveMeshes()
        {
            return obj.ImproveMeshes();
        }

        internal void GetBoundingBoxInternal(Matrix4 transformation, Box box)
        {
            obj.GetBoundingBoxInternal(transformation, box);
        }

        internal void RecalculateNormals(bool flip)
        {
            obj.RecalculateNormals(flip);
        }
    }

    public class ObjectInstance : Node
    {
        ObjectReference reference;

        public ObjectReference Reference { get { return reference; } }

        public ObjectInstance(ObjectReference reference, Node parent)
            : base(parent)
        {
            this.reference = reference;
        }

        public ObjectInstance(ObjectInstance instance, Node parent)
            : base(parent, instance)
        {
            this.reference = instance.reference;
        }

        public override NodeType IsType()
        {
            return NodeType.OBJECT_REFERENCE;
        }

        public override uint NrOfTriangles()
        {
            return reference.NrOfTriangles();
        }

        public override void GetAllMaterialsInternal(List<Material> list)
        {
            reference.GetAllMaterialsInternal(list);
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
            reference.GetBoundingBoxInternal(GetTransformation() * transformation, box);
        }

        internal protected override void GetExternalObjectsInternal(List<Object> list)
        {
            return;
        }

        internal protected override Node CloneInternal(Node parent)
        {
            return new ObjectInstance(this, parent);
        }

        public override void RecalculateNormals(bool flip)
        {
            reference.RecalculateNormals(flip);
        }

        protected override void GetMeshInstancesInternal(List<TransformedObjectInstance> list)
        {
            list.Add(new TransformedObjectInstance(this, new AbstractNode()));
        }

        public override bool IsEmpty()
        {
            return this.reference.Obj.IsEmpty();
        }
    }

    public class TransformedObjectInstance
    {
        ObjectInstance instance;
        List<AbstractNode> transformationNodes;

        public List<AbstractNode> TransformationNodes { get { return transformationNodes; } }
        public ObjectInstance Instance { get { return instance; } }

        public TransformedObjectInstance(ObjectInstance instance, AbstractNode transformation)
        {
            this.instance = instance;
            this.transformationNodes = new List<AbstractNode>();
            this.transformationNodes.Add(transformation);
        }

        public TransformedObjectInstance(TransformedObjectInstance instance, AbstractNode transformation)
        {
            this.instance = instance.instance;
            this.transformationNodes = new List<AbstractNode>();
            foreach (AbstractNode m in instance.transformationNodes)
                this.transformationNodes.Add(m);
            this.transformationNodes.Add(transformation);
        }
    }
}
