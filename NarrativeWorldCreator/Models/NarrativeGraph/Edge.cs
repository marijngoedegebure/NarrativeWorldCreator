using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeGraph
{
    public class Edge
    {
        public LocationNode from;
        public LocationNode to;

        public Edge(LocationNode from, LocationNode to)
        {
            this.from = from;
            this.to = to;
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Edge e = (Edge)obj;
            // Equals if either both from nodes are equal and both to nodes are equal or if they are reversed.
            return (e.from.Equals(from) && e.to.Equals(to)) || (e.from.Equals(to) && e.to.Equals(from));
        }

        public override int GetHashCode()
        {
            return this.from.GetHashCode() + this.to.GetHashCode();
        }

        public LocationNode getOtherNode(LocationNode n)
        {
            if (n.Equals(to))
            {
                return from;
            }
            return to;
        }
    }
}
