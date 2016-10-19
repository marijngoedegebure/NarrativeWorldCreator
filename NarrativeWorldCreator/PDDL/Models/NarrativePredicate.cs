using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    public class NarrativePredicate
    {
        public int NarrativePredicateId { get; set; }
        public PredicateType predicateType;
        public List<NarrativeObject> narrativeObjects = new List<NarrativeObject>();

        public NarrativePredicate()
        {

        }
    }
}
