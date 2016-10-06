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
        public List<Argument> arguments = new List<Argument>();

        public PredicateType(String nm)
        {
            name = nm;
        }

        public void addArgument(Type tp)
        {
            arguments.Add(new Argument(tp));
        }
    }

    class Argument
    {
        public Type type;

        public Argument(Type tp)
        {
            type = tp;
        }
    }
}
