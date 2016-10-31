using Narratives;
using NarrativeWorldCreator.RegionGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Parsers
{
    static class Parser
    {
        public static void parseDomain(String domainPath)
        {
            DomainParser.parseDomain(domainPath);
        }

        public static void parseProblem(String problemPath)
        {
            ProblemParser.parseProblem(problemPath);
        }

        public static String[] parseText(String text)
        {
            text = text.Replace("(", "");
            text = text.Replace(")", "");
            text = text.Replace(":", "");
            text = text.Replace("\t", "");
            text = text.Replace("\r", "");
            String[] lines = text.Split(new char[] { '\n' });
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
            }
            return lines;
        }

        public static void parsePlan(string planPath)
        {
            PlanParser.parsePlan(planPath);
        }

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
