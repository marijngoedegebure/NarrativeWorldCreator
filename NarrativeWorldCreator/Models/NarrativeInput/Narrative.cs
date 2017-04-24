using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeInput
{
    public class Narrative
    {
        public int NarrativeId { get; set; }
        public string Name { get; set; }
        public List<NarrativeObjectType> NarrativeObjectTypes { get; set; }
        public List<PredicateType> PredicateTypes { get; set; }
        public List<NarrativeObject> NarrativeObjects { get; set; }
        public List<Predicate> StartingPredicates { get; set; }
        public List<NarrativeAction> NarrativeActions { get; set; }
        public List<NarrativeEvent> NarrativeEvents { get; set; }

        public Narrative()
        {
            NarrativeObjectTypes = new List<NarrativeObjectType>();
            PredicateTypes = new List<PredicateType>();
            NarrativeObjects = new List<NarrativeObject>();
            StartingPredicates = new List<Predicate>();
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
                if (CheckType(narrativeObject.Type, t))
                    filteredNarrativeObjects.Add(narrativeObject);
            }
            return filteredNarrativeObjects;
        }

        public static bool CheckType(NarrativeObjectType type1, NarrativeObjectType type2)
        {
            if (type1.Name.Equals(type2.Name))
            {
                return true;
            }
            if (type1.ParentType != null)
            {
                return CheckType(type1.ParentType, type2);
            }
            return false;
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

        public List<Predicate> getStartingPredicates(String predicateType)
        {
            return (from a in StartingPredicates
                    where a.PredicateType.Name.Equals(predicateType)
                    select a).ToList();
        }
    }
}
