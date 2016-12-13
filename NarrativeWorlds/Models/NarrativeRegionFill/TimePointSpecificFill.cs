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
        List<EntikaClassInstance> NarrativeCharacterInstances { get; set; }

        // List of geometric relationships of character instances
        List<GeometricRelationshipBase> GeometricRelationshipsOfNarrativeCharacters { get; set; }

        // List of Narrative Object instances
        List<EntikaClassInstance> NarrativeThingInstances { get; set; }

        // List of geometric relationships of object instances
        List<GeometricRelationshipBase> GeometricRelationshipsOfNarrativeThings { get; set; }
    }
}
