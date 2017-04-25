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

        public const string DecorationRelationshipType = "regionTypeClassAssociation";
    }
}
