using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    public class NarrativeArgument
    {
        public int NarrativeArgumentId { get; set; }
        public NarrativeObjectType type;

        public NarrativeArgument(NarrativeObjectType tp)
        {
            type = tp;
        }
    }
}
