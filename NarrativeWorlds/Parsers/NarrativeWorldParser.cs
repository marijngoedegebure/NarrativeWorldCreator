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

        public static NarrativeWorld NarrativeWorld { get; set; }

        public static NarrativeWorld parse(string LocationTypeName, string CharacterTypeName, string ObjectTypeName, string MoveActionName, Narrative narrative)
        {
            NarrativeWorldParser.LocationTypeName = LocationTypeName;
            NarrativeWorldParser.CharacterTypeName = CharacterTypeName;
            NarrativeWorldParser.ObjectTypeName = ObjectTypeName;
            NarrativeWorldParser.MoveActionName = MoveActionName;
            NarrativeWorldParser.NarrativeWorld = new NarrativeWorld();
            NarrativeWorldParser.NarrativeWorld.Graph = new Graph();
            NarrativeWorldParser.NarrativeWorld.NarrativeTimeline = new NarrativeTimeline();
            NarrativeWorldParser.NarrativeWorld.Narrative = narrative;
            // Empty current graph
            
            createNarrativeTimeline();
            createGraphBasedOnNarrative();
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

            // Copy previous timepoint and adjust location of narrative objects according to action
            foreach (NarrativeEvent nevent in NarrativeWorld.Narrative.NarrativeEvents)
            {
                NarrativeTimePoint timePoint = new NarrativeTimePoint();
                // Check if move action
                // Do stuff based on this move action


                // Check if pickup action
                // Do stuff based on this action

                // Check if object is used while still in other location -> update to new location (might be also good idea for pickup action)

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
