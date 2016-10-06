using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.RegionGraph.GraphDataTypes
{
    class Edge
    {
        public Node from;
        public Node to;

        public Edge(Node from, Node to)
        {
            this.from = from;
            this.to = to;
        }
    }
}
