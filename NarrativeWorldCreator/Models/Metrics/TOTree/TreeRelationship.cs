using Semantics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.Metrics.TOTree
{
    public class TreeRelationship
    {
        public Relationship Relationship { get; set; }

        public TreeTangibleObject Source { get; set; }

        public TreeTangibleObject Target { get; set; }

        public bool Required { get; set; }

        public TreeRelationship(Relationship relationship)
        {
            this.Relationship = relationship;
            this.Required = false;
        }
    }
}
