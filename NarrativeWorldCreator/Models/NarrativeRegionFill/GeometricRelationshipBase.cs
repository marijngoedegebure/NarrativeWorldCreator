using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    public class GeometricRelationshipBase
    {
        public GeometricRelationshipBase(RelationshipTypes type)
        {
            this.RelationType = type;
            Targets = new List<EntikaInstance>();
        }

        public GeometricRelationshipBase()
        {
        }

        // Base class of the following relationships: Above, against, around, facing, followon, on, parallel
        // Has a source
        public EntikaInstance Source { get; set; }
        public List<EntikaInstance> Targets { get; set; }

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
