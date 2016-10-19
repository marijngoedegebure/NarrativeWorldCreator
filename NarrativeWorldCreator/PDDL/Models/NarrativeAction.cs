using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    public class NarrativeAction
    {
        public int NarrativeActionId { get; set; }
        public String name;
        public List<NarrativeArgument> arguments = new List<NarrativeArgument>();
        // Missing: preconditions, effects

        public NarrativeAction(String nm)
        {
            name = nm;
        }
    }
}
