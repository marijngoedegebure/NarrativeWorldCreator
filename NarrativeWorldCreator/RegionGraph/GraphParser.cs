using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Narratives;
using NarrativeWorldCreator.RegionGraph;
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
            SystemStateTracker.NarrativeWorld.Graph = new Graph();

            // Create nodes based on locations
            List<NarrativeObject> locations = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType("location");
            if (locations != null)
            {
                foreach (NarrativeObject location in locations)
                {
                    SystemStateTracker.NarrativeWorld.Graph.addNode(location.Name);
                }
            }
            // Create edges based on move actions
            List<NarrativeEvent> moveEvents = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeMoveEvents();
            foreach (NarrativeEvent ev in moveEvents)
            {
                Node to = SystemStateTracker.NarrativeWorld.Graph.getNode(ev.NarrativeObjects.Last().Name);
                Node from = SystemStateTracker.NarrativeWorld.Graph.getNode(ev.NarrativeObjects[ev.NarrativeObjects.Count - 2].Name);
                SystemStateTracker.NarrativeWorld.Graph.addEdge(from, to);
            }
            SystemStateTracker.NarrativeWorld.Graph.initForceDirectedGraph();
        }
    }
}