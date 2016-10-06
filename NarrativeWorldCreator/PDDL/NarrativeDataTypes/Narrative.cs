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
        public List<NarrativeAction> narrativeActions = new List<NarrativeAction>();
        public List<NarrativeEvent> narrativeEvents = new List<NarrativeEvent>();

        public Narrative()
        {

        }

        public List<NarrativeObject> getNarrativeObjectsOfType(Type t)
        {
            List<NarrativeObject> filteredNarrativeObjects = new List<NarrativeObject>();
            foreach(NarrativeObject narrativeObject in narrativeObjects)
            {
                if (narrativeObject.type.name.Equals(t.name))
                    filteredNarrativeObjects.Add(narrativeObject);
            }
            return filteredNarrativeObjects;
        }

        public Type getType(String name)
        {
            foreach(Type t in types)
            {
                if (t.name.Equals(name))
                    return t;
            }
            return null;
        }
    }
}
