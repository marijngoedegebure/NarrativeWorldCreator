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

        public void addNode(String locationName)
        {
            nodeList.Add(new GraphDataTypes.Node(locationName));
        }

        public void addEdge(Node from, Node to)
        {
            edgeList.Add(new Edge(from, to));
        }

        public List<Node> getNodeList()
        {
            return nodeList;
        }

        public List<Edge> getEdgeList()
        {
            return edgeList;
        }
    }
}
