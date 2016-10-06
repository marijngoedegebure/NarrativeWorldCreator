using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    class NarrativeAction
    {
        public String name;
        public List<NarrativeArgument> arguments = new List<NarrativeArgument>();
        // Missing: preconditions, effects

        public NarrativeAction(String nm)
        {
            name = nm;
        }
    }
}
