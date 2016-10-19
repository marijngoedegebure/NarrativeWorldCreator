using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    public class NarrativeObject
    {
        public int NarrativeObjectId { get; set; }
        public String name;
        public NarrativeObjectType type;

        public NarrativeObject(String nm, NarrativeObjectType tp)
        {
            name = nm;
            type = tp;
        }
    }
}
