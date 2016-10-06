using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    class NarrativeEvent
    {
        // Narrative action is not used in implementation and is thus not used
        // NarrativeAction narrativeAction;
        NarrativeObject location;

        public NarrativeEvent(NarrativeObject loc)
        {
            location = loc;
        }
    }
}
