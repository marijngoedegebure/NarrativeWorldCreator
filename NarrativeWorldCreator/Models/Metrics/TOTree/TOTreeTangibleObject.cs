using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.Metrics.TOTree
{
    public class TOTreeTangibleObject : Valuation
    {
        public TangibleObject TangibleObject { get; set; }
        public List<TOTreeRelationship> RelationshipsAsSource { get; set; }
        public List<TOTreeRelationship> RelationshipsAsTarget { get; set; }
        public bool Required { get; set; }
        public bool RequiredDependency { get; set; }
        public bool Decorative { get; set; }

        public TOTreeTangibleObject(TangibleObject to)
        {
            this.TangibleObject = to;
            RelationshipsAsSource = new List<TOTreeRelationship>();
            RelationshipsAsTarget = new List<TOTreeRelationship>();
            this.Required = false;
            this.Decorative = false;
        }
    }
}
