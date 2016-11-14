using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLNarrativeParser
{

    public class NarrativeArgument
    {
        public int NarrativeArgumentId { get; set; }
        public NarrativeObjectType Type { get; set; }

        public NarrativeArgument()
        {
        }
    }
}
