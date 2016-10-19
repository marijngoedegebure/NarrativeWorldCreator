using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    public class PredicateType
    {
        public int PredicateTypeId { get; set; }
        public String name;
        public List<NarrativeArgument> arguments = new List<NarrativeArgument>();

        public PredicateType(String nm)
        {
            name = nm;
        }

        public void addArgument(NarrativeObjectType tp)
        {
            arguments.Add(new NarrativeArgument(tp));
        }
    }
}
