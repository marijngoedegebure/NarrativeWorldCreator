using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class GeometricRelationshipBase
    {
        public GeometricRelationshipBase(RelationshipTypes type)
        {
            this.RelationType = type;
            Sources = new List<EntikaInstance>();
        }

        // Base class of the following relationships: Above, against, around, facing, followon, on, parallel
        // Has a source
        public List<EntikaInstance> Sources { get; set; }
        public EntikaInstance Target { get; set; }

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
