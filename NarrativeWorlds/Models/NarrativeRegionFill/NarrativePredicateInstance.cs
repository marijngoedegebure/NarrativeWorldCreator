using PDDLNarrativeParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds.Models.NarrativeRegionFill
{
    public class NarrativePredicateInstance
    {
        public NarrativePredicate NarrativePredicate { get; set; }

        public NarrativePredicateInstance(NarrativePredicate predicate)
        {
            this.NarrativePredicate = predicate;
        }
    }
}
