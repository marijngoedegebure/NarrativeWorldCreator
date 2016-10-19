using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NarrativeWorldCreator.PDDL;
using NarrativeWorldCreator.RegionGraph.GraphDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.RegionGraph
{
    class GraphParser
    {
        // Creates graph based on currently loaded Narrative
        public static void createGraphBasedOnNarrative()
        {
            // Empty current graph
            SystemStateTracker.graph = new Graph();

            // Create nodes based on locations
            List<NarrativeObject> locations = SystemStateTracker.narrative.getNarrativeObjectsOfType("location");
            if (locations != null)
            {
                foreach (NarrativeObject location in locations)
                {
                    SystemStateTracker.graph.addNode(location.name);
                }
            }
            // Create edges based on move actions
            List<NarrativeEvent> moveEvents = SystemStateTracker.narrative.getNarrativeMoveEvents();
            foreach (NarrativeEvent ev in moveEvents)
            {
                Node to = SystemStateTracker.graph.getNode(ev.narrativeObjects.Last().name);
                Node from = SystemStateTracker.graph.getNode(ev.narrativeObjects[ev.narrativeObjects.Count - 2].name);
                SystemStateTracker.graph.addEdge(from, to);
            }
            SystemStateTracker.graph.initForceDirectedGraph();
        }
    }
}