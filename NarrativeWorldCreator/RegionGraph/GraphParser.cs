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
        public static Graph graph = new Graph();
        public static Texture2D circleTexture;
        public static int nodeWidth = 100;
        public static int nodeHeight = 100;
        public static Texture2D lineTexture;

        // Force directed variables
        public const float DefaultStartingTemperature = 0.2f;
        public const float DefaultMinimumTemperature = 0.01f;
        public const float DefaultTemperatureAttenuation = 0.95f;

        public static float temperature;
        public static float temperatureAttenuation;

        public static Dictionary<Node, Vector2> NodePositions = new Dictionary<Node, Vector2>();
        public static Func<double, double> SpringForce { get; set; }
        public static Func<float, float> ElectricForce { get; set; }

        // Creates graph based on currently loaded Narrative
        public static void createGraphBasedOnNarrative()
        {
            // Empty current graph
            graph = new Graph();

            // Create nodes based on locations
            List<NarrativeObject> locations = Parser.narrative.getNarrativeObjectsOfType("location");
            if (locations != null)
            {
                foreach (NarrativeObject location in locations)
                {
                    graph.addNode(location.name);
                }
            }
            // Create edges based on move actions
            List<NarrativeEvent> moveEvents = Parser.narrative.getNarrativeMoveEvents();
            foreach (NarrativeEvent ev in moveEvents)
            {
                Node to = graph.getNode(ev.narrativeObjects.Last().name);
                Node from = graph.getNode(ev.narrativeObjects[ev.narrativeObjects.Count - 2].name);
                graph.addEdge(from, to);
            }
            randomlyGenerateCoordinates();
        }

        public static void randomlyGenerateCoordinates()
        {
            Random r = new Random();
            foreach (Node n in graph.getNodeList())
            {
                NodePositions[n] = new Vector2((float)r.NextDouble(), (float)r.NextDouble());
            }
            graph.nodeCoordinatesGenerated = true;
        }

        public static void initForceDirectedGraph()
        {
            temperature = DefaultStartingTemperature;
            temperatureAttenuation = DefaultTemperatureAttenuation;
            SpringForce = (d => 2 * Math.Log(d));
            ElectricForce = (d => 1 / (d * d));
            randomlyGenerateCoordinates();
        }

        public static void stepForceDirectedGraph()
        {
            Dictionary<Node, Vector2> forces = new Dictionary<Node, Vector2>();

            foreach (var u in graph.getNodeList())
            {
                Vector2 uPos = NodePositions[u];
                float xForce = 0, yForce = 0;
                // attraction forces
                foreach (var arc in graph.getEdgesOfNode(u))
                {
                    Vector2 vPos = NodePositions[arc.getOtherNode(u)];
                    float d = Vector2.Distance(uPos, vPos);
                    float force = temperature * (float) SpringForce(System.Convert.ToDouble(d));
                    xForce += (vPos.X - uPos.X) / d * force;
                    yForce += (vPos.Y - uPos.Y) / d * force;
                }
                // repulsion forces
                foreach (var v in graph.getNodeList())
                {
                    if (v == u) continue;
                    Vector2 vPos = NodePositions[v];
                    float d = Vector2.Distance(uPos, vPos);
                    float force = temperature * ElectricForce(d);
                    xForce += (uPos.X - vPos.X) / d * force;
                    yForce += (uPos.Y - vPos.Y) / d * force;
                }
                forces[u] = new Vector2(xForce, yForce);
            }

            foreach (var node in graph.getNodeList())
                NodePositions[node] += forces[node];
            temperature *= temperatureAttenuation;
        }
        public static void runForceDirectedGraph(double minimumTemperature = DefaultMinimumTemperature)
        {
            while (temperature > minimumTemperature) stepForceDirectedGraph();
            // Figure out min and max of X and Y
            List<Node> nodeList = graph.getNodeList();
            float minX = NodePositions[nodeList[0]].X;
            float maxX = NodePositions[nodeList[0]].X;
            float minY = NodePositions[nodeList[0]].Y;
            float maxY = NodePositions[nodeList[0]].Y;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (NodePositions[nodeList[i]].X > maxX)
                    maxX = NodePositions[nodeList[i]].X;
                if (NodePositions[nodeList[i]].X < minX)
                    minX = NodePositions[nodeList[i]].X;
                if (NodePositions[nodeList[i]].Y > maxY)
                    maxY = NodePositions[nodeList[i]].Y;
                if (NodePositions[nodeList[i]].Y < minY)
                    minY = NodePositions[nodeList[i]].Y;
            }
            // Normalize
            foreach(Node n in nodeList)
            {
                NodePositions[n] = new Vector2((NodePositions[n].X - minX) / (maxX - minX), (NodePositions[n].Y - minY) / (maxY - minY));
            }
        }
    }
}