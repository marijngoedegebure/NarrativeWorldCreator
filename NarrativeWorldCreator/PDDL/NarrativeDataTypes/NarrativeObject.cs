using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    class NarrativeObject
    {
        public String name;
        public Type type;

        public NarrativeObject(String nm, Type tp)
        {
            name = nm;
            type = tp;
        }
    }
}
