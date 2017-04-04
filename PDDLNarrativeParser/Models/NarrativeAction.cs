using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLNarrativeParser
{
    public class NarrativeAction
    {
        public int NarrativeActionId { get; set; }
        public string Name { get; set; }
        public IList<NarrativeArgument> Parameters { get; set; }
        public List<NarrativePredicateValued> Preconditions { get; set; }
        public List<NarrativePredicateValued> Effects { get; set; }
        // Missing: preconditions, effects

        public NarrativeAction()
        {
            Parameters = new List<NarrativeArgument>();
            Preconditions = new List<NarrativePredicateValued>();
            Effects = new List<NarrativePredicateValued>();
        }
    }
}
