using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Narratives;
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
                    SystemStateTracker.graph.addNode(location.Name);
                }
            }
            // Create edges based on move actions
            List<NarrativeEvent> moveEvents = SystemStateTracker.narrative.getNarrativeMoveEvents();
            foreach (NarrativeEvent ev in moveEvents)
            {
                Node to = SystemStateTracker.graph.getNode(ev.NarrativeObjects.Last().Name);
                Node from = SystemStateTracker.graph.getNode(ev.NarrativeObjects[ev.NarrativeObjects.Count - 2].Name);
                SystemStateTracker.graph.addEdge(from, to);
            }
            SystemStateTracker.graph.initForceDirectedGraph();
        }
    }
}