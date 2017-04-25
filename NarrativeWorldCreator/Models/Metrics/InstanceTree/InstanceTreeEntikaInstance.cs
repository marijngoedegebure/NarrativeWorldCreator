using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.Metrics.InstanceTree
{
    public class InstanceTreeEntikaInstance
    {
        public EntikaInstance EntikaInstance { get; set; }
        public List<InstanceTreeRelationship> RelationshipsAsSource { get; set; }
        public List<InstanceTreeRelationship> RelationshipsAsTarget { get; set; }

        public InstanceTreeEntikaInstance(EntikaInstance instance)
        {
            this.EntikaInstance = instance;
            RelationshipsAsSource = new List<InstanceTreeRelationship>();
            RelationshipsAsTarget = new List<InstanceTreeRelationship>();
        }
    }
}
