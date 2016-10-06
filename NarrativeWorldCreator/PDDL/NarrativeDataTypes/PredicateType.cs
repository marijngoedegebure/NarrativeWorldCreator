using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    class PredicateType
    {
        public String name;
        public List<NarrativeArgument> arguments = new List<NarrativeArgument>();

        public PredicateType(String nm)
        {
            name = nm;
        }

        public void addArgument(Type tp)
        {
            arguments.Add(new NarrativeArgument(tp));
        }
    }
}
