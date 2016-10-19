using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;
using System.IO;

namespace Common.Geometry
{
    public class Shader
    {
        static Parameter.VariableHandler defaultHandler = null;

        public static void SetDefaultParameterHandler(Parameter.VariableHandler handler)
        {
            defaultHandler = handler;
        }

        public enum Type { Vertex, Fragment }

        public class Parameter
        {
            public enum Type { Vec3, Double, Vec4Array, DoubleArray };

            public interface VariableHandler
            {
                object GetVariable(string name, long uid, object defaultValue);
            }

            string name, evaluationString = "";
            object defaultValue;
            VariableHandler handler = null;
            Type type;
            bool isFixed;

            public string Name { get { return name; } }
            public string EvaluationString { get { if (evaluationString == "") return Name; return evaluationString; } }
            public Type ParameterType { get { return type; } }
            public bool IsFixed { get { return isFixed; } }
            public object GetValue(long uid)
            {
                if (isFixed)
                    return defaultValue;
                else
                    return handler.GetVariable(EvaluationString, uid, defaultValue);
            }

            public static Parameter CreateFixedParameter(string name, Type type, object value)
            {
                Parameter p = new Parameter();
                p.type = type;
                p.isFixed = true;
                p.name = name;
                p.defaultValue = value;
                return p;
            }

            public static Parameter CreateVariableParameter(string name, Type type, object defaultValue, VariableHandler handler)
            {
                Parameter p = new Parameter();
                p.type = type;
                p.isFixed = false;
                p.name = name;
                p.defaultValue = defaultValue;
                System.Diagnostics.Debug.Assert(handler != null);
                p.handler = handler;
                return p;
            }

            public static Parameter CreateVariableParameter(string name, string evaluationString, Type type, object defaultValue,
                                                            VariableHandler handler)
            {
                Parameter p = new Parameter();
                p.type = type;
                p.isFixed = false;
                p.name = name;
                p.evaluationString = evaluationString;
                p.defaultValue = defaultValue;
                System.Diagnostics.Debug.Assert(handler != null);
                p.handler = handler;
                return p;
            }

            internal Parameter()
            {
            }

            internal Parameter(Parameter copy)
            {
                this.defaultValue = copy.defaultValue;
                this.evaluationString = copy.evaluationString;
                this.handler = copy.handler;
                this.isFixed = copy.isFixed;
                this.name = copy.name;
                this.type = copy.type;
            }

            internal static Parameter Load(BinaryReader r)
            {
                return new Parameter(r);
            }

            private Parameter(BinaryReader r)
            {
                this.evaluationString = r.ReadString();
                this.isFixed = r.ReadBoolean();
                this.name = r.ReadString();
                this.type = (Type)Enum.Parse(typeof(Type), r.ReadString(), true);
                if (r.ReadBoolean())
                {
                    switch (this.type)
                    {
                        case Type.Double:
                            this.defaultValue = r.ReadDouble();
                            break;
                        case Type.Vec3:
                            defaultValue = Vec3.Load(r);
                            break;
                    }
                }
            }

            internal void Save(BinaryWriter w)
            {
                w.Write(this.evaluationString);
                if (this.handler != null)
                    throw new NotImplementedException();
                w.Write(this.isFixed);
                w.Write(this.name);
                w.Write(this.type.ToString());
                w.Write(this.defaultValue != null);
                if (this.defaultValue != null)
                {
                    switch (this.type)
                    {
                        case Type.Double:
                            w.Write((double)defaultValue);
                            break;
                        case Type.Vec3:
                            ((Vec3)defaultValue).Save(w);
                            break;
                    }
                }
            }
        }

        List<Parameter> parameters = new List<Parameter>();
        Type type;
        string filename;

        public ReadOnlyCollection<Parameter> Parameters { get { return parameters.AsReadOnly(); } }
        public Type ShaderType { get { return type; } }
        public string Filename { get { return filename; } }

        public Shader(string filename, Type type)
        {
            this.filename = filename;
            this.type = type;
        }

        public Shader(BinaryReader r)
        {
            this.filename = r.ReadString();
            this.type = (Type)Enum.Parse(typeof(Type), r.ReadString(), true);
            int count = r.ReadInt32();
            for (int i = 0; i < count; ++i)
                AddParameter(Parameter.Load(r));
        }

        internal void Save(BinaryWriter wr)
        {
            wr.Write(this.filename);
            wr.Write(this.type.ToString());
            wr.Write(parameters.Count);
            foreach (Parameter p in parameters)
                p.Save(wr);
        }

        internal Shader(Shader copy)
        {
            this.filename = copy.filename;
            foreach (Parameter p in copy.parameters)
                this.parameters.Add(new Parameter(p));
            this.type = copy.type;
        }

        internal static Shader Load(System.IO.BinaryReader br)
        {
            return new Shader(br);
        }

        public void AddParameter(Parameter p)
        {
            this.parameters.Add(p);
        }

        internal static Shader Load(XmlNode node)
        {
            Shader shader = new Shader(node.Attributes["filename"].Value, (Shader.Type)Enum.Parse(typeof(Shader.Type), node.Attributes["type"].Value));
            foreach (XmlNode subnode in node.ChildNodes)
            {
                if (subnode.Name == "Parameter")
                {
                    shader.AddParameter(Parameter.CreateFixedParameter(subnode.Attributes["name"].Value, 
                                        (Parameter.Type)Enum.Parse(typeof(Parameter.Type), subnode.Attributes["type"].Value, true), 
                                        GetDefaultValue(subnode)));
                }
                else if (subnode.Name == "SemanticParameter")
                {
                    if (subnode.Attributes["evaluationString"] != null)
                        shader.AddParameter(Parameter.CreateVariableParameter(subnode.Attributes["name"].Value, subnode.Attributes["evaluationString"].Value,
                                        (Parameter.Type)Enum.Parse(typeof(Parameter.Type), subnode.Attributes["type"].Value, true),
                                        GetDefaultValue(subnode), defaultHandler));
                    else
                        shader.AddParameter(Parameter.CreateVariableParameter(subnode.Attributes["name"].Value,
                                        (Parameter.Type)Enum.Parse(typeof(Parameter.Type), subnode.Attributes["type"].Value, true),
                                        GetDefaultValue(subnode), defaultHandler));
                }
            }
            return shader;
        }

        private static object GetDefaultValue(XmlNode node)
        {
            if (node.Attributes["defaultValue"] != null)
            {
                switch (node.Attributes["type"].Value.ToLower())
                {
                    case "double":
                        return double.Parse(node.Attributes["defaultValue"].Value);
                    case "vec3":
                        string[] pars = node.Attributes["defaultValue"].Value.Split(';');
                        return new Vec3((float)double.Parse(pars[0]), (float)double.Parse(pars[1]), (float)double.Parse(pars[2]));
                    default:
                        throw new NotImplementedException();
                }
            }
            return null;
        }
    }
}
