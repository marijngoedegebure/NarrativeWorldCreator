using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.Metrics.TOTree
{
    public class TreeTangibleObject : Valuation
    {
        public TangibleObject TangibleObject { get; set; }
        public List<TreeRelationship> RelationshipsAsSource { get; set; }
        public List<TreeRelationship> RelationshipsAsTarget { get; set; }

        public TreeTangibleObject(TangibleObject to)
        {
            this.TangibleObject = to;
            RelationshipsAsSource = new List<TreeRelationship>();
            RelationshipsAsTarget = new List<TreeRelationship>();
        }
    }
}
