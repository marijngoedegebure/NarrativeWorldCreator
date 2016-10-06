using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.PDDL
{
    class PlanParser
    {
        public static Narrative parsePlan(String planPath, Narrative narrative)
        {
            string[] lines = Parser.parseText(File.ReadAllText(planPath));
            bool readEventMode = false;
            List<NarrativeEvent> narrativeEvents = new List<NarrativeEvent>();
            foreach (String line in lines)
            {
                string[] words = line.Split(null);
                if (words[0].Equals(""))
                    continue;
                if (words[0].Equals("define"))
                    continue;
                if (words[0].Equals("domain"))
                    continue;
                if (words[0].Equals("problem"))
                    continue;
                if (words[0].Equals("events"))
                {
                    readEventMode = true;
                    continue;
                }
                if (readEventMode)
                {
                    narrativeEvents.Add(readEvent(words, narrative.narrativeActions, narrative.types));
                    continue;
                }
            }
            narrative.narrativeEvents = narrativeEvents;
            return narrative;
        }

        private static NarrativeEvent readEvent(string[] words, List<NarrativeAction> narrativeActions, List<Type> types)
        {
            // Check if second word is a narrative action
            NarrativeEvent narrativeEvent = new NarrativeEvent();
            foreach (NarrativeAction narrativeAction in narrativeActions)
            {
                if (narrativeAction.name.Equals(words[1]))
                {
                    narrativeEvent.narrativeAction = narrativeAction;
                    break;
                }
            }
            if (narrativeEvent.narrativeAction == null)
                throw new Exception("Second argument is not a narrative action");
            // Check if last word is a location
            Type locationType = Parser.narrative.getType("location");
            List<NarrativeObject> objectsFilteredOnLocationType = Parser.narrative.getNarrativeObjectsOfType(locationType);
            NarrativeObject locationOfEvent = null;
            foreach( NarrativeObject filteredObject in objectsFilteredOnLocationType)
            {
                if (filteredObject.name.Equals(words.Last()))
                {
                    locationOfEvent = filteredObject;
                    break;
                }
            }
            if (locationOfEvent == null)
                throw new Exception("Last argument is not a location");
            // Read words in
            for(int i = 2; i < words.Length; i++)
            {
                foreach(NarrativeObject narrativeObject in Parser.narrative.narrativeObjects)
                {
                    if (words[i].Equals(narrativeObject.name))
                    {
                        if (narrativeEvent.narrativeAction.arguments[i - 2].type.name.Equals(narrativeObject.type.name))
                        {
                            narrativeEvent.narrativeObjects.Add(narrativeObject);
                            break;
                        }
                    }
                }
            }
            return narrativeEvent;
        }
    }
}
