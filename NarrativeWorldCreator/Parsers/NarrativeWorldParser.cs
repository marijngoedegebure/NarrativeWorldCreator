﻿using Common;
using Microsoft.Xna.Framework;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeInput;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using Semantics.Data;
using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Parsers
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
            // Parse Entika Information
            readAllTangibleObjects();

            // Parse Narrative information        
            createGraphBasedOnNarrative();
            copyPredicatesToTimeline();
            createNarrativeTimeline();
            processPredicatesToConstraints();

            // parseDependencyGraph();
            return NarrativeWorldParser.NarrativeWorld;
        }

        private static void copyPredicatesToTimeline()
        {
            NarrativeWorldParser.NarrativeWorld.NarrativeTimeline.PredicateTypes = NarrativeWorldParser.NarrativeWorld.Narrative.PredicateTypes;
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
                    var predicateListByLocation = (from predicate in timepoint.AllPredicates
                                                   from classOrPlace in predicate.EntikaClassNames
                                                   where classOrPlace.Equals(timepoint.Location.LocationName)
                                                   select predicate).ToList();
                    timepoint.PredicatesFilteredByCurrentLocation = predicateListByLocation;
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
            List<Predicate> predicates = NarrativeWorld.Narrative.StartingPredicates.ToList();
            BeliefSystem.InitializeFirstTimePoint(NarrativeWorld.NarrativeTimeline.NarrativeTimePoints[0], predicates);
            for (int i = 0; i < NarrativeWorld.Narrative.NarrativeEvents.Count; i++)
            {
                BeliefSystem.ApplyEventStoreInNextTimePoint(
                    NarrativeWorld.NarrativeTimeline.NarrativeTimePoints[i],
                    NarrativeWorld.Narrative.NarrativeEvents[i],
                    NarrativeWorld.NarrativeTimeline.NarrativeTimePoints[i + 1]);
            }

            foreach (var node in NarrativeWorld.Graph.getNodeList())
            {
                // Add basic floor instance to delta for each location node
                var floorInstance = new EntikaInstance(Constants.Floor, new Common.Polygon(new List<Vec2d>()));
                node.TimePoints[0].InstanceDeltas.Add(new InstanceDelta(0, floorInstance, InstanceDeltaType.Add, new Vector3(0, 0, 0), new Vector3(0, 0, 0)));
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
                    NarrativeWorldParser.NarrativeWorld.Graph.addNode(location.Name, location.Type.Name);
                }
            }
            // Create edges based on move actions
            // Get events based on Parser.MoveActionName
            List<NarrativeEvent> moveEvents = NarrativeWorldParser.NarrativeWorld.Narrative.getNarrativeMoveEvents(NarrativeWorldParser.MoveActionName);
            foreach (NarrativeEvent ev in moveEvents)
            {
                LocationNode to = NarrativeWorldParser.NarrativeWorld.Graph.getNode(ev.NarrativeObjects.Last().Name);
                LocationNode from = NarrativeWorldParser.NarrativeWorld.Graph.getNode(ev.NarrativeObjects[ev.NarrativeObjects.Count - 2].Name);
                NarrativeWorldParser.NarrativeWorld.Graph.addEdge(from, to);
            }
            NarrativeWorldParser.NarrativeWorld.Graph.initForceDirectedGraph();
        }

        private static void readAllTangibleObjects()
        {
            // Load all TangibleObjects that do not have any children, and thus are leafs
            TangibleObject physicalObjectClass = DatabaseSearch.GetNodes<TangibleObject>("physical Object").FirstOrDefault();
            List<TangibleObject> physicalClassLeaves = GetLeaves(physicalObjectClass);
            List<TangibleObject> allTangibleObjects = DatabaseSearch.GetNodes<TangibleObject>(true);
            NarrativeWorld.AvailableTangibleObjects = physicalClassLeaves;
        }

        private static List<TangibleObject> GetLeaves(TangibleObject current)
        {
            var ret = new List<TangibleObject>();
            foreach (TangibleObject child in current.PersonalChildren)
            {
                if (child.Children.Count == 0)
                {
                    ret.Add(child);
                }
                else
                {
                    ret.AddRange(GetLeaves(child));
                }
            }

            return ret;
        }

        private static void parseDependencyGraph()
        {
            foreach (var to in NarrativeWorld.AvailableTangibleObjects)
            {
                foreach (var relAsSource in to.RelationshipsAsSource)
                {

                }
                foreach(var relAsTarget in to.RelationshipsAsTarget)
                {

                }
            }
            var RelationshipsAsSource = NarrativeWorld.AvailableTangibleObjects[0].RelationshipsAsSource[0];
        }
    }
}
