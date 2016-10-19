using System;
using System.Collections.Generic;
using System.IO;

namespace Common
{
    public class ObjectAttributeDefinition
    {
        public enum ValueTypes { String, Int, Float };

        public string Name
        {
            get;
            set;
        }

        public ValueTypes ValueType
        {
            get;
            set;
        }

        private static Dictionary<string, ObjectAttributeDefinition> definitions = new Dictionary<string, ObjectAttributeDefinition>();

        private ObjectAttributeDefinition(string name, ValueTypes valueType)
        {
            this.Name = name;
            this.ValueType = valueType;
        }

        public static ObjectAttributeDefinition Get(string name, ValueTypes valueType)
        {
            ObjectAttributeDefinition result = null;
            if (definitions.ContainsKey(name))
            {
                result = definitions[name];
            }
            else
            {
                result = new ObjectAttributeDefinition(name, valueType);
                definitions[name] = result;
            }
            return result;
        }
    }

    public class ObjectAttribute
    {
        public ObjectAttributeDefinition Definition
        {
            get;
            private set;
        }

        public string Name
        {
            get { return Definition.Name; }
        }

        public object Value
        {
            get;
            set;
        }

        public ObjectAttributeDefinition.ValueTypes ValueType
        {
            get { return Definition.ValueType; }
        }

        public ObjectAttribute(string name, object value, ObjectAttributeDefinition.ValueTypes valueType)
        {
            this.Definition = ObjectAttributeDefinition.Get(name, valueType);
            this.Value = value;
        }
    }
}
