using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NarrativeWorldCreator.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.RegionGraph.GraphDataTypes
{
    class Graph
    {
        List<Node> nodeList = new List<Node>();
        List<Edge> edgeList = new List<Edge>();
        public bool nodeCoordinatesGenerated;

        public Graph()
        {
            nodeCoordinatesGenerated = false;
        }

        public void addNode(String locationName)
        {
            nodeList.Add(new GraphDataTypes.Node(locationName));
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

        public Node getNode(String nm)
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
    }
}
