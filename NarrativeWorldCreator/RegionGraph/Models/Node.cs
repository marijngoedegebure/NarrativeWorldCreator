using Microsoft.Xna.Framework;
using Narratives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.RegionGraph
{
    public class Node
    {
        String locationName;
        public List<Vector3> RegionOutlinePoints { get; set; }
        public List<NarrativeEvent> NarrativeEvents { get; set; }
        public List<NarrativeObject> NarrativeObjects { get; set; }

        public Node(String locationName)
        {
            this.locationName = locationName;
            RegionOutlinePoints = new List<Vector3>();
            NarrativeEvents = new List<NarrativeEvent>();
            NarrativeObjects = new List<NarrativeObject>();
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
