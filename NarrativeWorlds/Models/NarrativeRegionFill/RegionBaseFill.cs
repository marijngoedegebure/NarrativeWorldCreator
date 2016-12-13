using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class RegionBaseFill
    {
        // List of fill objects
        List<EntikaClassInstance> EntikaFillObjects { get; set; }

        // List of relations
        List<GeometricRelationshipBase> GeometricRelationshipsFillObjects { get; set; }
    }
}
