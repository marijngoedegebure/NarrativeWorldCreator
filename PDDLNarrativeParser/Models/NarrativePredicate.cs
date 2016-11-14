using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLNarrativeParser
{
    public class NarrativePredicate
    {
        public int NarrativePredicateId { get; set; }
        public PredicateType PredicateType { get; set; }
        public IList<NarrativeObject> NarrativeObjects { get; set; }

        public NarrativePredicate()
        {
            NarrativeObjects = new List<NarrativeObject>();
        }
    }
}
