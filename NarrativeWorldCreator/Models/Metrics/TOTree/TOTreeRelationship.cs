using Semantics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.Metrics.TOTree
{
    public class TOTreeRelationship
    {
        public Relationship Relationship { get; set; }

        public TOTreeTangibleObject Source { get; set; }

        public TOTreeTangibleObject Target { get; set; }

        public TOTreeRelationship(Relationship relationship)
        {
            this.Relationship = relationship;
        }
    }
}
