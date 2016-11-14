using PDDLNarrativeParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public static class NarrativeWorldParser
    {
        public static string LocationTypeName { get; set; }
        public static string CharacterTypeName { get; set; }
        public static string ObjectTypeName { get; set; }
        public static string MoveActionName { get; set; }
        public static string AtPredicateName { get; set; }

        public static NarrativeWorld NarrativeWorld { get; set; }


        public static void staticInput(string locationTypeName, string characterTypeName, string objectTypeName, string moveActionName, string atPredicateName)
        {
            NarrativeWorldParser.LocationTypeName = locationTypeName;
            NarrativeWorldParser.CharacterTypeName = characterTypeName;
            NarrativeWorldParser.ObjectTypeName = objectTypeName;
            NarrativeWorldParser.MoveActionName = moveActionName;
            NarrativeWorldParser.AtPredicateName = atPredicateName;
        }

        public static NarrativeWorld parse(Narrative narrative)
        {
            
            NarrativeWorldParser.NarrativeWorld = new NarrativeWorld();
            NarrativeWorldParser.NarrativeWorld.Graph = new Graph();
            NarrativeWorldParser.NarrativeWorld.NarrativeTimeline = new NarrativeTimeline();
            NarrativeWorldParser.NarrativeWorld.Narrative = narrative;
            createGraphBasedOnNarrative();
            createNarrativeTimeline();
            return NarrativeWorldParser.NarrativeWorld;
        }

        public static void createNarrativeTimeline()
        {
            // Create narrative characters for each narrative object with CharacterTypeName
            List<NarrativeObject> characters = NarrativeWorld.Narrative.getNarrativeObjectsOfType(CharacterTypeName);
            foreach(NarrativeObject character in characters)
            {
                NarrativeWorld.NarrativeCharacters.Add(new NarrativeCharacter(character.Name));
            }

            // Create narrative things for each narrative object with ObjectTypeName
            List<NarrativeObject> things = NarrativeWorld.Narrative.getNarrativeObjectsOfType(ObjectTypeName);
            foreach (NarrativeObject thing in things)
            {
                NarrativeWorld.NarrativeThings.Add(new NarrativeThing(thing.Name));
            }

            // Determine starting locations for each character and object using at() predicate add this as the first timepoint
            NarrativeTimePoint initialTimePoint = new NarrativeTimePoint();
            List<NarrativePredicate> predicates = NarrativeWorld.Narrative.getNarrativePredicates(AtPredicateName);
            foreach(NarrativePredicate predicate in predicates)
            {
                if(predicate.NarrativeObjects[0].Type.Name.Equals(CharacterTypeName))
                {
                    // Narrative character
                    NarrativeCharacter nc = NarrativeWorld.getNarrativeCharacter(predicate.NarrativeObjects[0].Name);
                    initialTimePoint.LocationOfNarrativeCharacters[nc] = NarrativeWorld.Graph.getNode(predicate.NarrativeObjects[1].Name);
                }
                else
                {
                    // Narrative object
                    NarrativeThing nt = NarrativeWorld.getNarrativeThing(predicate.NarrativeObjects[0].Name);
                    initialTimePoint.LocationOfNarrativeThings[nt] = NarrativeWorld.Graph.getNode(predicate.NarrativeObjects[1].Name);
                }
            }
            NarrativeWorld.NarrativeTimeline.NarrativeTimePoints.Add(initialTimePoint);
            // Check has predicate for objects that have no location yet
            // Add to timeline object, an object either has to be on a location or has to be carried by someone

            // Copy previous timepoint and adjust location of narrative objects according to action
            foreach (NarrativeEvent nevent in NarrativeWorld.Narrative.NarrativeEvents)
            {
                NarrativeTimePoint timePoint = new NarrativeTimePoint();
                timePoint.copy(initialTimePoint);
                timePoint.NarrativeEvent = nevent;
                // Check if move action
                // Do stuff based on this move action
                if(nevent.NarrativeAction.Name.Equals(MoveActionName))
                {
                    NarrativeCharacter nc = NarrativeWorld.getNarrativeCharacter(nevent.NarrativeObjects[0].Name);
                    timePoint.LocationOfNarrativeCharacters[nc] = NarrativeWorld.Graph.getNode(nevent.NarrativeObjects.Last().Name);
                }

                // Check if pickup action
                // Remove object from locations list, insert when dropped

                // Check for drop action

                // Check if object/character is used in unknown action while still in other location -> update to new location (might be also good idea for pickup action)
                else
                {
                    // Skip the last narrative object of the action, that is the location
                    for(int i = 0; i < nevent.NarrativeObjects.Count-1; i++)
                    {
                        if(nevent.NarrativeObjects[i].Type.Name.Equals(CharacterTypeName))
                        {
                            NarrativeCharacter nc = NarrativeWorld.getNarrativeCharacter(nevent.NarrativeObjects[i].Name);
                            timePoint.LocationOfNarrativeCharacters[nc] = NarrativeWorld.Graph.getNode(nevent.NarrativeObjects.Last().Name);
                        }
                        else if(nevent.NarrativeObjects[i].Type.Name.Equals(ObjectTypeName))
                        {
                            NarrativeThing nt = NarrativeWorld.getNarrativeThing(nevent.NarrativeObjects[i].Name);
                            timePoint.LocationOfNarrativeThings[nt] = NarrativeWorld.Graph.getNode(nevent.NarrativeObjects.Last().Name);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                NarrativeWorld.NarrativeTimeline.NarrativeTimePoints.Add(timePoint);
            }
        }

        public static void createGraphBasedOnNarrative()
        {
            // Create nodes based on locations
            List<NarrativeObject> locations = NarrativeWorldParser.NarrativeWorld.Narrative.getNarrativeObjectsOfType(NarrativeWorldParser.LocationTypeName);
            if (locations != null)
            {
                foreach (NarrativeObject location in locations)
                {
                    NarrativeWorldParser.NarrativeWorld.Graph.addNode(location.Name);
                }
            }
            // Create edges based on move actions
            // Get events based on Parser.MoveActionName
            List<NarrativeEvent> moveEvents = NarrativeWorldParser.NarrativeWorld.Narrative.getNarrativeMoveEvents(NarrativeWorldParser.MoveActionName);
            foreach (NarrativeEvent ev in moveEvents)
            {
                Node to = NarrativeWorldParser.NarrativeWorld.Graph.getNode(ev.NarrativeObjects.Last().Name);
                Node from = NarrativeWorldParser.NarrativeWorld.Graph.getNode(ev.NarrativeObjects[ev.NarrativeObjects.Count - 2].Name);
                NarrativeWorldParser.NarrativeWorld.Graph.addEdge(from, to);
            }
            NarrativeWorldParser.NarrativeWorld.Graph.initForceDirectedGraph();
        }
    }
}
