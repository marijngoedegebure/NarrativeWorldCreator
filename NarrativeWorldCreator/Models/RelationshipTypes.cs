using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models
{
    public static class RelationshipTypes
    {
        public enum RelationshipBooleanTypes
        {
            On,
            Against,
            Facing,
            Parallel
        }

        public enum RelationshipValuedTypes
        {
            Front,
            Back,
            Left,
            Right
        }

        public static Dictionary<string, RelationshipBooleanTypes> RelationshipBooleanDictionary = new Dictionary<string, RelationshipBooleanTypes>
        {
            { "on", RelationshipBooleanTypes.On },
            { "against", RelationshipBooleanTypes.Against },
            { "facing", RelationshipBooleanTypes.Facing },
            { "parallel", RelationshipBooleanTypes.Parallel }
        };

        public static Dictionary<string, RelationshipValuedTypes> RelationshipValuedDictionary = new Dictionary<string, RelationshipValuedTypes>
        {
            { "front", RelationshipValuedTypes.Front },
            { "back", RelationshipValuedTypes.Back },
            { "left", RelationshipValuedTypes.Left },
            { "right", RelationshipValuedTypes.Right }
        };

        public static bool IsRelationshipValued(string name)
        {
            return RelationshipValuedDictionary.Keys.Contains(name);
        }
    }
}
