using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class TimePointSpecificFill
    {
        // List of Narrative Character instances
        List<NarrativeShape> NarrativeCharacterInstances { get; set; }

        // List of geometric relationships of character instances
        List<GeometricRelationshipBase> GeometricRelationshipsOfNarrativeCharacters { get; set; }

        // List of Narrative Object instances
        List<NarrativeShape> NarrativeThingInstances { get; set; }

        // List of geometric relationships of object instances
        List<GeometricRelationshipBase> GeometricRelationshipsOfNarrativeThings { get; set; }

        List<EntikaInstance> OtherObjectInstances { get; set; }

        // List of shapes
        public List<NarrativeShape> NarrativeShapes { get; set; }

        public TimePointSpecificFill()
        {
            NarrativeCharacterInstances = new List<NarrativeShape>();
            GeometricRelationshipsOfNarrativeCharacters = new List<GeometricRelationshipBase>();
            NarrativeThingInstances = new List<NarrativeShape>();
            GeometricRelationshipsOfNarrativeThings = new List<GeometricRelationshipBase>();
            NarrativeShapes = new List<NarrativeShape>();
        }
    }
}
