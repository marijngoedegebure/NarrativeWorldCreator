using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.Metrics.InstanceTree
{
    public class InstanceTreeRelationship
    {
        public RelationshipInstance RelationshipInstance { get; set; }

        public InstanceTreeEntikaInstance Source { get; set; }

        public InstanceTreeEntikaInstance Target { get; set; }

        public InstanceTreeRelationship(RelationshipInstance relationship)
        {
            this.RelationshipInstance = relationship;
        }
    }
}
