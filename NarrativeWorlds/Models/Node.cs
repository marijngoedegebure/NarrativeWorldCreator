using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PDDLNarrativeParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class Node
    {
        String locationName;
        public List<VertexPositionColor> RegionOutlinePoints { get; set; }
        public List<int> triangleListIndices { get; set; }
        public List<NarrativeEvent> NarrativeEvents { get; set; }
        public List<NarrativeObject> NarrativeObjects { get; set; }
        public List<InstancedEntikaObject> InstancedEntikaObjects { get; set; }

        public Node(String locationName)
        {
            this.locationName = locationName;
            RegionOutlinePoints = new List<VertexPositionColor>();
            triangleListIndices = new List<int>();
            NarrativeEvents = new List<NarrativeEvent>();
            NarrativeObjects = new List<NarrativeObject>();
            InstancedEntikaObjects = new List<InstancedEntikaObject>();
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

        public void triangulatePolygon()
        {
            for(int i = 1; i < RegionOutlinePoints.Count-1 ; i++)
            {
                triangleListIndices.Add(0);
                triangleListIndices.Add(i);
                triangleListIndices.Add(i + 1);
            }
        }
    }
}
