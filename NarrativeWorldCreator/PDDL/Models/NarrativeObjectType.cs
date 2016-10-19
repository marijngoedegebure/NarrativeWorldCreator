using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    public class NarrativeObjectType
    {
        public int NarrativeObjectTypeId { get; set; }
        public String name { get; set; }

        public NarrativeObjectType(String nm)
        {
            name = nm;
        }
    }
}
