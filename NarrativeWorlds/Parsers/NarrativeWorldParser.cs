using NarrativeWorlds.Models;
using PDDLNarrativeParser;
using Semantics.Data;
using Semantics.Entities;
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
            setupObjectTOLink();
            createGraphBasedOnNarrative();
            createNarrativeTimeline();
            processPredicatesToConstraints();
            return NarrativeWorldParser.NarrativeWorld;
        }

        private static void processPredicatesToConstraints()
        {
            // Determine Object and relation requirements using predicates

            // For each narrative timepoint
            foreach(var timepoint in NarrativeWorld.NarrativeTimeline.NarrativeTimePoints)
            {
                // Filter predicates on the ones that mention the location
                if (timepoint.Location != null)
                {
                    var predicateListByLocation = (from predicateInstance in timepoint.NarrativePredicateInstances
                                                   from narrativeObject in predicateInstance.NarrativePredicate.NarrativeObjects
                                                   where narrativeObject.Name.Equals(timepoint.Location.LocationName)
                                                   select predicateInstance).ToList();
                    timepoint.PredicatesInstancesFilteredLocation = predicateListByLocation;
                }
            }
        }

        /// <summary>
        /// Determines the predicates that are avaiable at each moment in the story
        /// </summary>
        public static void createNarrativeTimeline()
        {           
            // InitialTimePoint does not have a location node associated with it, possible solution would be the addition of annotation of a starting node for the story
            NarrativeTimePoint initialTimePoint = new NarrativeTimePoint(0);
            NarrativeWorld.NarrativeTimeline.NarrativeTimePoints.Add(initialTimePoint);
            // Initialize each timepoint
            for (int i = 0; i < NarrativeWorld.Narrative.NarrativeEvents.Count; i++)
            {
                NarrativeTimePoint timePoint = new NarrativeTimePoint(i+1);
                timePoint.NarrativeEvent = NarrativeWorld.Narrative.NarrativeEvents[i];
                // Last "NarrativeObject" is the name of the location
                var Node = NarrativeWorld.Graph.getNode(NarrativeWorld.Narrative.NarrativeEvents[i].NarrativeObjects.Last().Name);
                timePoint.Location = Node;
                Node.TimePoints.Add(timePoint);
                NarrativeWorld.NarrativeTimeline.NarrativeTimePoints.Add(timePoint);
            }

            // Load all predicates to determine starting requirements of each location
            List<NarrativePredicate> predicates = NarrativeWorld.Narrative.NarrativePredicates.ToList();
            BeliefSystem.InitializeFirstTimePoint(NarrativeWorld.NarrativeTimeline.NarrativeTimePoints[0], predicates);
            for (int i = 0; i < NarrativeWorld.Narrative.NarrativeEvents.Count; i++)
            {
                BeliefSystem.ApplyEventStoreInNextTimePoint(
                    NarrativeWorld.NarrativeTimeline.NarrativeTimePoints[i],
                    NarrativeWorld.Narrative.NarrativeEvents[i],
                    NarrativeWorld.NarrativeTimeline.NarrativeTimePoints[i + 1]);
            }
        }

        private static void setupObjectTOLink()
        {
            foreach (NarrativeObject no in NarrativeWorld.Narrative.NarrativeObjects)
            {
                TangibleObject tangibleObject = DatabaseSearch.GetNode<TangibleObject>(no.Name.ToLower());
                NarrativeWorld.NarrativeObjectEntikaLinks.Add(new NarrativeObjectEntikaLink(no, tangibleObject));
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
