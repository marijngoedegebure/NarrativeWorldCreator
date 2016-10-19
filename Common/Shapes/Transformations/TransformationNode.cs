using System;
using System.Collections.Generic;

using System.Text;
using Common;

using System.IO;
using Common.MathParser;

namespace Common.Shapes.Transformations
{
    public class TransformationNode
    {
        public class ApplicationData
        {
            public interface SceneInterface
            {
            }

            public readonly SceneInterface scene;
            public readonly Random random;

            public ApplicationData(SceneInterface scene, Random random)
            {
                this.scene = scene;
                this.random = random;
            }
        }

        public class Parameter : Common.CommandHandler
        {
            string name;
            string value;
            string comment;

            public string Name { get { return name; } }
            public string Comment { get { return comment; } }

            public string Value
            {
                get { return value; }
                set { ChangeProperty("Value", value); }
            }

            internal Parameter(Tuple<string, string, string> t)
            {
                this.name = t.Item1.Trim();
                this.value = t.Item2.Trim();
                this.comment = t.Item3.Trim();
            }

            protected override void ChangeProperty(string property, object newValue)
            {
                switch (property)
                {
                    case "Value":
                        value = (string)newValue;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            protected override void ChangeList(string list, object element, Common.ListChangedCommand.Type type)
            {
                throw new NotImplementedException();
            }

            internal void Save(BinaryWriter w)
            {
                w.Write(this.name);
                w.Write(this.value);
                w.Write(this.comment);
            }

            internal Parameter(BinaryReader r)
            {
                this.name = r.ReadString().Trim();
                this.value = r.ReadString().Trim();
                this.comment = r.ReadString().Trim();
            }
        }

        ShapeNode input;
        List<ShapeNode> output;
        List<Parameter> parameters = new List<Parameter>();
        TransformationType type;

        public IEnumerable<ShapeNode> OutputNodes { get { return output.AsReadOnly(); } }
        public IEnumerable<Parameter> Parameters { get { return parameters.AsReadOnly(); } }
        public TransformationType Type { get { return type; } }

        internal TransformationNode(TransformationType type, ShapeNode input, List<ShapeNode> output)
        {
            foreach (Tuple<string, string, string> t in type.parametersWithDefaultValueAndComment)
                parameters.Add(new Parameter(t));
            this.input = input;
            this.input.AddTransformation(this);
            this.output = output;
            this.type = type;
        }

        private List<string> GetParameterValues()
        {
            List<string> list = new List<string>();
            foreach (Parameter p in parameters)
                list.Add(p.Value);
            return list;
        }

        internal void Execute(TransformationNode.ApplicationData data, object inputShape, List<Tuple<ShapeNode, object>> list, CustomTermEvaluater evaluater)
        {
            List<object> outputShapes = type.transformation(data, inputShape, evaluater, GetParameterValues());
            for (int i = 0; i < outputShapes.Count; ++i)
                output[i].Execute(data, outputShapes[i], list, evaluater);
        }

        internal void Save(System.IO.BinaryWriter w)
        {
            w.Write(this.output.Count);
            foreach (ShapeNode sn in output)
                sn.Save(w);
            w.Write(this.parameters.Count);
            foreach (Parameter p in this.parameters)
                p.Save(w);
            w.Write(this.type.Name);
        }

        internal TransformationNode(Transformation parent, ShapeNode input, BinaryReader r, short version)
        {
            parent.AddTransformationNodeFromFile(this);
            int count = r.ReadInt32();
            output = new List<ShapeNode>();
            for (int i = 0; i < count; ++i)
                output.Add(new ShapeNode(parent, r, version));
            count = r.ReadInt32();
            for (int i = 0; i < count; ++i)
                this.parameters.Add(new Parameter(r));
            this.type = TransformationType.GetFromName(r.ReadString());

            this.input = input;
        }
    }
}
