using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Narratives;

namespace NarrativeWorldCreator.Parsers
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
                    narrativeEvents.Add(readEvent(words, SystemStateTracker.NarrativeWorld.Narrative.NarrativeActions, SystemStateTracker.NarrativeWorld.Narrative.NarrativeObjectTypes));
                    continue;
                }
            }
            SystemStateTracker.NarrativeWorld.Narrative.NarrativeEvents = narrativeEvents;
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
            NarrativeObjectType locationType = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectType("location");
            List<NarrativeObject> objectsFilteredOnLocationType = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType(locationType);
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
            // Read words in
            for(int i = 1; i < words.Length; i++)
            {
                foreach(NarrativeObject narrativeObject in SystemStateTracker.NarrativeWorld.Narrative.NarrativeObjects)
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
