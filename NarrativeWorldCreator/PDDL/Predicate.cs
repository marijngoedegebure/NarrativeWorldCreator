using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    class Predicate
    {
        public String name;
        public List<Argument> arguments = new List<Argument>();

        public Predicate(String nm)
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
