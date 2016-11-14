using NarrativeWorldCreator.RegionGraph;
using PDDLNarrativeParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Parsers
{
    public static class GraphParser
    {
        public const string LocationTypeName = "place";
        public const string CharacterTypeName = "character";
        public const string ObjectTypeName = "thing";
        public const string MoveActionName = "move";

        public static void parse(string domainPath, string problemPath, string planPath)
        {
            SystemStateTracker.NarrativeWorld.Narrative = PDDLNarrativeParser.Parser.parse(LocationTypeName, CharacterTypeName, ObjectTypeName, MoveActionName, domainPath, problemPath, planPath);
        }

        public static void createGraphBasedOnNarrative()
        {
            // Empty current graph
            SystemStateTracker.NarrativeWorld.Graph = new Graph();

            // Create nodes based on locations
            List<NarrativeObject> locations = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType(GraphParser.LocationTypeName);
            if (locations != null)
            {
                foreach (NarrativeObject location in locations)
                {
                    SystemStateTracker.NarrativeWorld.Graph.addNode(location.Name);
                }
            }
            // Create edges based on move actions
            // Get events based on Parser.MoveActionName
            List<NarrativeEvent> moveEvents = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeMoveEvents(GraphParser.MoveActionName);
            foreach (NarrativeEvent ev in moveEvents)
            {
                Node to = SystemStateTracker.NarrativeWorld.Graph.getNode(ev.Location.Name);
                Node from = SystemStateTracker.NarrativeWorld.Graph.getNode(ev.NarrativeObjects[ev.NarrativeObjects.Count - 2].Name);
                SystemStateTracker.NarrativeWorld.Graph.addEdge(from, to);
            }
            SystemStateTracker.NarrativeWorld.Graph.initForceDirectedGraph();
        }
    }
}
