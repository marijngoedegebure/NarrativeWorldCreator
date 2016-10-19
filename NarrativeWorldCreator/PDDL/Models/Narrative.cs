using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    public class Narrative
    {
        public int NarrativeId { get; set; }
        public List<NarrativeObjectType> narrativeObjectTypes = new List<NarrativeObjectType>();
        public List<PredicateType> predicateTypes = new List<PredicateType>();
        public List<NarrativeObject> narrativeObjects = new List<NarrativeObject>();
        public List<NarrativePredicate> narrativePredicates = new List<NarrativePredicate>();
        public List<NarrativeAction> narrativeActions = new List<NarrativeAction>();
        public List<NarrativeEvent> narrativeEvents = new List<NarrativeEvent>();

        public Narrative()
        {

        }

        public List<NarrativeEvent> getNarrativeMoveEvents()
        {
            List<NarrativeEvent> moveEvents = new List<NarrativeEvent>();
            foreach(NarrativeEvent ev in narrativeEvents)
            {
                if(ev.narrativeAction.name.Equals("move"))
                {
                    moveEvents.Add(ev);
                }
            }
            return moveEvents;
        }

        public List<NarrativeObject> getNarrativeObjectsOfType(string s)
        {
            NarrativeObjectType t = getNarrativeObjectType(s);
            if (t != null)
                return getNarrativeObjectsOfType(t);
            return null;
        }

        public List<NarrativeObject> getNarrativeObjectsOfType(NarrativeObjectType t)
        {
            List<NarrativeObject> filteredNarrativeObjects = new List<NarrativeObject>();
            foreach(NarrativeObject narrativeObject in narrativeObjects)
            {
                if (narrativeObject.type.name.Equals(t.name))
                    filteredNarrativeObjects.Add(narrativeObject);
            }
            return filteredNarrativeObjects;
        }

        internal List<NarrativeObject> getNarrativeObjectsOfTypeOfLocation(string s, string location)
        {
            List<NarrativeEvent> narrativeEventsOfLocation = getNarrativeEventsOfLocation(location);
            List<NarrativeObject> filteredNarrativeObjects = new List<NarrativeObject>();
            foreach(NarrativeEvent narrativeEvent in narrativeEventsOfLocation)
            {
                foreach (NarrativeObject narrativeObject in narrativeEvent.narrativeObjects)
                {
                    if (narrativeObject.type.name.Equals(s))
                        filteredNarrativeObjects.Add(narrativeObject);
                }
            }
            return filteredNarrativeObjects;
        }

        internal List<NarrativeEvent> getNarrativeEventsOfLocation(string location)
        {
            List<NarrativeEvent> filteredNarrativeEvents = new List<NarrativeEvent>();
            foreach (NarrativeEvent narrativeEvent in narrativeEvents)
            {
                if(narrativeEvent.narrativeObjects.Last().name.Equals(location))
                {
                    filteredNarrativeEvents.Add(narrativeEvent);
                }
            }
            return filteredNarrativeEvents;
        }

        public NarrativeObjectType getNarrativeObjectType(String name)
        {
            foreach(NarrativeObjectType t in narrativeObjectTypes)
            {
                if (t.name.Equals(name))
                    return t;
            }
            return null;
        }
    }
}
