using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.RegionGraph.GraphDataTypes
{
    class Node
    {
        String locationName;

        public Node(String locationName)
        {
            this.locationName = locationName;
        }

        public String getLocationName()
        {
            return locationName;
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Node n = (Node)obj;
            return locationName.Equals(n.locationName);
        }
    }
}
