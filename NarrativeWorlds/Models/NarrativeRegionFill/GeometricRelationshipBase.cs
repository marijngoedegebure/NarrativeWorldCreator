using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class GeometricRelationshipBase
    {
        // Base class of the following relationships: Above, against, around, facing, followon, on, parallel
        // Has a source
        public NarrativeShape Source { get; set; }
        public List<NarrativeShape> Target { get; set; }

        public enum RelationshipTypes
        {
            Above,
            Against,
            Around,
            Facing,
            On,
            Parallel,
        }

        public RelationshipTypes RelationType { get; set; }
    }
}
