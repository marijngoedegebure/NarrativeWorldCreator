using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    public class NarrativeEvent
    {
        public int NarrativeEventId { get; set; }
        public NarrativeAction narrativeAction;
        public List<NarrativeObject> narrativeObjects = new List<NarrativeObject>();

        public NarrativeEvent()
        {
        }
    }
}
