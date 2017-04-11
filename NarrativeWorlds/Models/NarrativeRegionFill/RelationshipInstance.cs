using Semantics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds.Models.NarrativeRegionFill
{
    public class RelationshipInstance
    {
        public EntikaInstance Source { get; set; }
        public List<EntikaInstance> Targets { get; set; }
        public Relationship BaseRelationship { get; set; }

        public RelationshipInstance()
        {
            Targets = new List<EntikaInstance>();
        }
    }
}
