using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models
{
    public static class Constants
    {
        // Supported Predicates
        public const string At = "at";
        public const string On = "on";

        // Relationship constants
        public const string GeneralRelationshipTypeValued = "valued";
        public const string GeneralRelationshipTypeBoolean = "boolean";
        public static List<string> GeometricRelationshipTypes = new List<string> { "on", "back", "front", "left", "right", "facing", "parallel", "rotated" };
        public static List<string> OtherRelationshipTypes = new List<string> { "back", "front", "left", "right", "facing", "parallel", "rotated" };

        public const string DecorationRelationshipType = "regionTypeClassAssociation";

        public const string Floor = "floor";

        public const string Clearance = "clearance";

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

        public static string ModelPath = "model path";

        public static string Neutral = "Neutral";
        public static string Decoration = "Decoration";
        public static string Required = "Required";

        public static bool IsRelationshipValued(string name)
        {
            return RelationshipValuedDictionary.Keys.Contains(name);
        }

        // All Metric Weights
        public static Dictionary<string, double> AllMetricWeights = new Dictionary<string, double>
        {
            { "incoming edges", 1 },
            { "outgoing edges", 1 },
            { "incoming decorative edges", 1 },
            { "outgoing decorative edges", 1 },
            { "incoming required edges", 1 },
            { "outgoing required edges", 1 },
            { "incoming edges available", 1 },
            { "outgoing edges available", 1 },
            { "required", 1 },
            { "required dependency", 1 },
            { "decoration weight", 1 },
            { "area", 3 }
        };

        // Required Metric Weights
        public static Dictionary<string, double> RequiredMetricWeights = new Dictionary<string, double>
        {
            { "incoming edges", 1 },
            { "outgoing edges", 1 },
            { "incoming decorative edges", 0.1 },
            { "outgoing decorative edges", 0.1 },
            { "incoming required edges", 2 },
            { "outgoing required edges", 2 },
            { "incoming edges available", 1 },
            { "outgoing edges available", 1 },
            { "required", 3 },
            { "required dependency", 3 },
            { "decoration weight", 0.1 },
            { "area", 3 }
        };

        // Decoration Metric Weights
        public static Dictionary<string, double> DecorationMetricWeights = new Dictionary<string, double>
        {
            { "incoming edges", 1 },
            { "outgoing edges", 1 },
            { "incoming decorative edges", 3 },
            { "outgoing decorative edges", 3 },
            { "incoming required edges", 0.1 },
            { "outgoing required edges", 0.1 },
            { "incoming edges available", 1 },
            { "outgoing edges available", 1 },
            { "required", 0.1 },
            { "required dependency", 0.1 },
            { "decoration weight", 3 },
            { "area", 3 }
        };
    }
}
