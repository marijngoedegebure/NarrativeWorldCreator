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

        public static Narrative Narrative { get; set; }

        public static Graph Graph { get; set; }

        public static Graph parse(string LocationTypeName, string CharacterTypeName, string ObjectTypeName, string MoveActionName, Narrative narrative)
        {
            NarrativeWorldParser.LocationTypeName = LocationTypeName;
            NarrativeWorldParser.CharacterTypeName = CharacterTypeName;
            NarrativeWorldParser.ObjectTypeName = ObjectTypeName;
            NarrativeWorldParser.MoveActionName = MoveActionName;
            NarrativeWorldParser.Narrative = narrative;
            // Empty current graph
            NarrativeWorldParser.Graph = new Graph();
            createGraphBasedOnNarrative(narrative);
            return NarrativeWorldParser.Graph;
        }

        public static void createGraphBasedOnNarrative(Narrative narrative)
        {
            // Create nodes based on locations
            List<NarrativeObject> locations = narrative.getNarrativeObjectsOfType(NarrativeWorldParser.LocationTypeName);
            if (locations != null)
            {
                foreach (NarrativeObject location in locations)
                {
                    NarrativeWorldParser.Graph.addNode(location.Name);
                }
            }
            // Create edges based on move actions
            // Get events based on Parser.MoveActionName
            List<NarrativeEvent> moveEvents = narrative.getNarrativeMoveEvents(NarrativeWorldParser.MoveActionName);
            foreach (NarrativeEvent ev in moveEvents)
            {
                Node to = NarrativeWorldParser.Graph.getNode(ev.Location.Name);
                Node from = NarrativeWorldParser.Graph.getNode(ev.NarrativeObjects[ev.NarrativeObjects.Count - 2].Name);
                NarrativeWorldParser.Graph.addEdge(from, to);
            }
            NarrativeWorldParser.Graph.initForceDirectedGraph();
        }
    }
}
