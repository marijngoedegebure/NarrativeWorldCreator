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
            { "On", RelationshipBooleanTypes.On },
            { "Against", RelationshipBooleanTypes.Against },
            { "Facing", RelationshipBooleanTypes.Facing },
            { "Parallel", RelationshipBooleanTypes.Parallel }
        };

        public static Dictionary<string, RelationshipValuedTypes> RelationshipValuedDictionary = new Dictionary<string, RelationshipValuedTypes>
        {
            { "Front", RelationshipValuedTypes.Front },
            { "Back", RelationshipValuedTypes.Back },
            { "Left", RelationshipValuedTypes.Left },
            { "Right", RelationshipValuedTypes.Right }
        };

        public static bool IsRelationshipValued(string name)
        {
            return RelationshipValuedDictionary.Keys.Contains(name);
        }
    }
}
