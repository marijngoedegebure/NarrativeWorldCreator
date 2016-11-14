using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Narratives;

namespace PDDLNarrativeParser
{
    class PlanParser
    {
        public static void parsePlan(String planPath)
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
                if (words[0].Equals("problem"))
                    continue;
                if (words[0].Equals("steps"))
                {
                    readEventMode = true;
                    if(words.Length > 1)
                        narrativeEvents.Add(readEvent(words, Parser.narrative.NarrativeActions, Parser.narrative.NarrativeObjectTypes));
                    continue;
                }
                if (readEventMode)
                {
                    narrativeEvents.Add(readEvent(words, Parser.narrative.NarrativeActions, Parser.narrative.NarrativeObjectTypes));
                    continue;
                }
            }
            Parser.narrative.NarrativeEvents = narrativeEvents;
        }

        private static NarrativeEvent readEvent(string[] words, ICollection<NarrativeAction> narrativeActions, ICollection<NarrativeObjectType> types)
        {
            // Check if second word is a narrative action
            NarrativeEvent narrativeEvent = new NarrativeEvent();
            foreach (NarrativeAction narrativeAction in narrativeActions)
            {
                if (narrativeAction.Name.Equals(words.First()))
                {
                    narrativeEvent.NarrativeAction = narrativeAction;
                    break;
                }
            }
            if (narrativeEvent.NarrativeAction == null)
                throw new Exception("Second argument is not a narrative action");
            // Check if last word is a location
            NarrativeObjectType placeType = Parser.narrative.getNarrativeObjectType(Parser.LocationTypeName);
            List<NarrativeObject> objectsFilteredOnLocationType = Parser.narrative.getNarrativeObjectsOfType(placeType);
            NarrativeObject locationOfEvent = null;
            foreach(NarrativeObject filteredObject in objectsFilteredOnLocationType)
            {
                if (filteredObject.Name.Equals(words.Last()))
                {
                    locationOfEvent = filteredObject;
                    break;
                }
            }
            if (locationOfEvent == null)
                throw new Exception("Last argument is not a location");
            narrativeEvent.Location = locationOfEvent;
            // Read words in
            for (int i = 1; i < words.Length; i++)
            {
                foreach(NarrativeObject narrativeObject in Parser.narrative.NarrativeObjects)
                {
                    if (words[i].Equals(narrativeObject.Name))
                    {
                        if (narrativeEvent.NarrativeAction.Arguments[i - 1].Type.Name.Equals(narrativeObject.Type.Name))
                        {
                            narrativeEvent.NarrativeObjects.Add(narrativeObject);
                            break;
                        }
                    }
                }
            }
            return narrativeEvent;
        }
    }
}
