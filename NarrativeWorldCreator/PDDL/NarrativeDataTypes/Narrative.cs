using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    class Narrative
    {
        public List<Type> types = new List<Type>();
        public List<PredicateType> predicateTypes = new List<PredicateType>();
        public List<NarrativeObject> narrativeObjects = new List<NarrativeObject>();
        public List<NarrativePredicate> narrativePredicates = new List<NarrativePredicate>();

        public Narrative()
        {

        }
    }
}
