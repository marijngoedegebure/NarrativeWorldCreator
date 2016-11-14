using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLNarrativeParser
{
    public class Narrative
    {
        public int NarrativeId { get; set; }
        public IList<NarrativeObjectType> NarrativeObjectTypes { get; set; }
        public IList<PredicateType> PredicateTypes { get; set; }
        public IList<NarrativeObject> NarrativeObjects { get; set; }
        public IList<NarrativePredicate> NarrativePredicates { get; set; }
        public IList<NarrativeAction> NarrativeActions { get; set; }
        public IList<NarrativeEvent> NarrativeEvents { get; set; }

        public Narrative()
        {
            NarrativeObjectTypes = new List<NarrativeObjectType>();
            PredicateTypes = new List<PredicateType>();
            NarrativeObjects = new List<NarrativeObject>();
            NarrativePredicates = new List<NarrativePredicate>();
            NarrativeActions = new List<NarrativeAction>();
            NarrativeEvents = new List<NarrativeEvent>();
        }

        public List<NarrativeEvent> getNarrativeMoveEvents(string moveActionName)
        {
            List<NarrativeEvent> moveEvents = new List<NarrativeEvent>();
            foreach(NarrativeEvent ev in NarrativeEvents)
            {
                if(ev.NarrativeAction.Name.Equals(moveActionName))
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
            foreach(NarrativeObject narrativeObject in NarrativeObjects)
            {
                if (narrativeObject.Type.Name.Equals(t.Name))
                    filteredNarrativeObjects.Add(narrativeObject);
            }
            return filteredNarrativeObjects;
        }

        public List<NarrativeObject> getNarrativeObjectsOfTypeOfLocation(string s, string location)
        {
            List<NarrativeEvent> narrativeEventsOfLocation = getNarrativeEventsOfLocation(location);
            List<NarrativeObject> filteredNarrativeObjects = new List<NarrativeObject>();
            foreach(NarrativeEvent narrativeEvent in narrativeEventsOfLocation)
            {
                foreach (NarrativeObject narrativeObject in narrativeEvent.NarrativeObjects)
                {
                    if (narrativeObject.Type.Name.Equals(s))
                        filteredNarrativeObjects.Add(narrativeObject);
                }
            }
            return filteredNarrativeObjects;
        }

        public List<NarrativeEvent> getNarrativeEventsOfLocation(string location)
        {
            List<NarrativeEvent> filteredNarrativeEvents = new List<NarrativeEvent>();
            foreach (NarrativeEvent narrativeEvent in NarrativeEvents)
            {
                if(narrativeEvent.NarrativeObjects.Last().Name.Equals(location))
                {
                    filteredNarrativeEvents.Add(narrativeEvent);
                }
            }
            return filteredNarrativeEvents;
        }

        public List<NarrativeObject> getNarrativeObjectsOfLocation(string location)
        {
            List<NarrativeEvent> narrativeEventsOfLocation = getNarrativeEventsOfLocation(location);
            List<NarrativeObject> filteredNarrativeObjects = new List<NarrativeObject>();
            foreach (NarrativeEvent narrativeEvent in narrativeEventsOfLocation)
            {
                foreach (NarrativeObject narrativeObject in narrativeEvent.NarrativeObjects)
                {
                    filteredNarrativeObjects.Add(narrativeObject);
                }
            }
            return filteredNarrativeObjects;
        }

        public NarrativeObjectType getNarrativeObjectType(String name)
        {
            foreach(NarrativeObjectType t in NarrativeObjectTypes)
            {
                if (t.Name.Equals(name))
                    return t;
            }
            return null;
        }
    }
}
