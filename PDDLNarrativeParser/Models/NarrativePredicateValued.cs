using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLNarrativeParser
{
    public class NarrativePredicateValued
    {
        public PredicateType PredicateType { get; set; }
        public List<NarrativeArgument> ArgumentsAffected { get; set; }
        public bool Value { get; set; }

        public NarrativePredicateValued()
        {
            this.Value = true;
            this.ArgumentsAffected = new List<NarrativeArgument>();
        }
    }
}
