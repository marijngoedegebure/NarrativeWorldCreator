using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NarrativeWorldCreator.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.RegionGraph
{
    public class Graph
    {
        List<Node> nodeList = new List<Node>();
        List<Edge> edgeList = new List<Edge>();
        public bool nodeCoordinatesGenerated;

        // Drawing information
        public Texture2D circleTexture;
        public int nodeWidth = 100;
        public int nodeHeight = 100;
        public Texture2D lineTexture;

        // Energy based positioning
        public const float DefaultStartingTemperature = 0.2f;
        public const float DefaultMinimumTemperature = 0.01f;
        public const float DefaultTemperatureAttenuation = 0.95f;
        public float temperature;
        public float temperatureAttenuation;
        public Func<double, double> SpringForce { get; set; }
        public Func<float, float> ElectricForce { get; set; }

        // Energy positions to visualization constant
        public const float energyToDrawConversion = 200f;

        public Dictionary<Node, Vector2> NodePositions = new Dictionary<Node, Vector2>();
        public Dictionary<Node, Rectangle> NodeCollisionBoxes = new Dictionary<Node, Rectangle>();

        public Graph()
        {
            nodeCoordinatesGenerated = false;
        }

        public void addNode(string locationName)
        {
            Node node = new Node(locationName);
            node.NarrativeEvents = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeEventsOfLocation(locationName);
            node.NarrativeObjects = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfLocation(locationName);
            nodeList.Add(node);
        }

        public void addEdge(Node from, Node to)
        {
            Edge newEdge = new Edge(from, to);
            foreach(Edge e in edgeList)
            {
                if (e.Equals(newEdge))
                    return;
            }
            edgeList.Add(newEdge);
        }

        public Node getNode(string nm)
        {
            foreach(Node n in nodeList)
            {
                if (n.getLocationName().Equals(nm))
                    return n;
            }
            return null;
        }

        public List<Node> getNodeList()
        {
            return nodeList;
        }

        public List<Edge> getEdgeList()
        {
            return edgeList;
        }

        public List<Edge> getEdgesOfNode(Node n)
        {
            List<Edge> edges = new List<Edge>();
            foreach(Edge e in edgeList)
            {
                if(e.to.Equals(n) || e.from.Equals(n))
                {
                    edges.Add(e);
                }
            }
            return edges;
        }

        public void randomlyGenerateCoordinates()
        {
            Random r = new Random();
            foreach (Node n in SystemStateTracker.NarrativeWorld.Graph.getNodeList())
            {
                NodePositions[n] = new Vector2((float)r.NextDouble(), (float)r.NextDouble());
            }
            this.nodeCoordinatesGenerated = true;
        }

        public void initCollisionboxes()
        {
            foreach (Node n in SystemStateTracker.NarrativeWorld.Graph.getNodeList())
            {
                float x = NodePositions[n].X * energyToDrawConversion;
                float y = NodePositions[n].Y * energyToDrawConversion;
                Rectangle collisionBox = new Rectangle((int)x, (int)y, SystemStateTracker.NarrativeWorld.Graph.nodeHeight, SystemStateTracker.NarrativeWorld.Graph.nodeWidth);
                this.NodeCollisionBoxes[n] = collisionBox;
            }
        }

        public void initForceDirectedGraph()
        {
            this.temperature = DefaultStartingTemperature;
            this.temperatureAttenuation = DefaultTemperatureAttenuation;
            this.SpringForce = (d => 2 * Math.Log(d));
            this.ElectricForce = (d => 1 / (d * d));
            this.randomlyGenerateCoordinates();
            this.initCollisionboxes();
        }

        public void stepForceDirectedGraph()
        {
            Dictionary<Node, Vector2> forces = new Dictionary<Node, Vector2>();

            foreach (var u in this.getNodeList())
            {
                Vector2 uPos = this.NodePositions[u];
                float xForce = 0, yForce = 0;
                // attraction forces
                foreach (var arc in this.getEdgesOfNode(u))
                {
                    Vector2 vPos = NodePositions[arc.getOtherNode(u)];
                    float d = Vector2.Distance(uPos, vPos);
                    float force = temperature * (float)SpringForce(System.Convert.ToDouble(d));
                    xForce += (vPos.X - uPos.X) / d * force;
                    yForce += (vPos.Y - uPos.Y) / d * force;
                }
                // repulsion forces
                foreach (var v in this.getNodeList())
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

            foreach (var node in this.getNodeList())
                NodePositions[node] += forces[node];
            temperature *= temperatureAttenuation;
        }
        public void runForceDirectedGraph(double minimumTemperature = DefaultMinimumTemperature)
        {
            while (temperature > minimumTemperature) stepForceDirectedGraph();
        }

        public  void checkCollisions(Vector2 mousePosition)
        {
            foreach (Node n in this.getNodeList())
            {
                if (this.NodeCollisionBoxes[n].Contains(mousePosition))
                {
                    var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                    var graphPage = (GraphPage) mainWindow._mainFrame.NavigationService.Content;
                    graphPage.selectedNode = n;
                    graphPage.fillDetailView(n);
                }
            }
        }
    }
}
