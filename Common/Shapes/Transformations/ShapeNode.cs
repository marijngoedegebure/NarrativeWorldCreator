using System;
using System.Collections.Generic;
using System.Text;
using Common;

using System.IO;
using Common.MathParser;

namespace Common.Shapes.Transformations
{
    public class ShapeNode : Common.CommandHandler
    {
        Transformation parent;
        string name;
        internal ShapeType type;
        bool nameFixed = false;
        List<TransformationNode> transformations = new List<TransformationNode>();

        public IEnumerable<TransformationNode> Transformations { get { return transformations.AsReadOnly(); } }

        public string Name
        {
            get { return name; }
            set { if (!nameFixed) ExecutePropertyChangedCommand("Name", name, value); }
        }

        public Transformation Parent { get { return parent; } }

        public ShapeType Type { get { return type; } }

        public bool IsEnabled { get { return !nameFixed; } }

        public ShapeNode(Transformation parent, ShapeType type)
        {
            this.type = type;
            this.parent = parent;
            name = "";
        }

        public ShapeNode(Transformation parent, ShapeType type, string name, bool nameFixed)
        {
            this.parent = parent;
            this.type = type;
            this.name = name;
            this.nameFixed = nameFixed;
        }

        protected override void ChangeProperty(string property, object newValue)
        {
            switch (property)
            {
                case "Name":
                    name = (string)newValue;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void ChangeList(string list, object element, Common.ListChangedCommand.Type type)
        {
            switch (list)
            {
                case "Transformations":
                    TransformationNode tnode = (TransformationNode)element;
                    if (type == Common.ListChangedCommand.Type.ElementAdded)
                        transformations.Add(tnode);
                    else if (type == Common.ListChangedCommand.Type.ElementRemoved)
                        transformations.Remove(tnode);
                    break;
            }
        }

        internal void AddTransformation(TransformationNode node)
        {
            ExecuteListChangedCommand("Transformations", node, Common.ListChangedCommand.Type.ElementAdded);
        }

        public List<Tuple<ShapeNode, object>> Execute(TransformationNode.ApplicationData data, object inputShape)
        {
            List<Tuple<ShapeNode, object>> list = new List<Tuple<ShapeNode, object>>();
            Execute(data, inputShape, list, new BasicMathFunctionSolver(data.random));
            return list;
        }

        internal void Execute(TransformationNode.ApplicationData data, object inputShape, List<Tuple<ShapeNode, object>> list, CustomTermEvaluater evaluater)
        {
            list.Add(new Tuple<ShapeNode, object>(this, inputShape));
            foreach (TransformationNode t in transformations)
                t.Execute(data, inputShape, list, evaluater);
        }

        internal void Save(BinaryWriter w)
        {
            w.Write(this.name);
            w.Write(this.nameFixed);

            w.Write(this.transformations.Count);
            foreach (TransformationNode tn in this.transformations)
                tn.Save(w);

            w.Write(type.Name);
        }

        internal ShapeNode(Transformation parent, BinaryReader r, short version)
        {
            this.parent = parent;
            parent.AddShapeNodeFromFile(this);
            this.name = r.ReadString();
            this.nameFixed = r.ReadBoolean();

            int count = r.ReadInt32();
            for(int i = 0; i < count; ++i)
                this.transformations.Add(new TransformationNode(parent, this, r, version));

            type = ShapeType.GetFromName(r.ReadString());
        }
    }
}
