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
        public IList<NarrativeArgument> Arguments { get; set; }
        // Missing: preconditions, effects

        public NarrativeAction()
        {
            Arguments = new List<NarrativeArgument>();
        }
    }
}
