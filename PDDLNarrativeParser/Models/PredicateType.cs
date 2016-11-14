using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLNarrativeParser
{
    public class PredicateType
    {
        public int PredicateTypeId { get; set; }
        public string Name { get; set; }
        public IList<NarrativeArgument> Arguments { get; set; }

        public PredicateType()
        {
            Arguments = new List<NarrativeArgument>();
        }
    }
}
