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
        public String LocationName { get; set; }
        public List<VertexPositionColor> RegionOutlinePoints { get; set; }
        // Add navmesh

        public List<int> triangleListIndices { get; set; }
        public List<NarrativeEvent> NarrativeEvents { get; set; }
        public List<NarrativeObject> NarrativeObjects { get; set; }
        public List<EntikaClassInstance> EntikaClassInstances { get; set; }
        public RegionBaseFill RegionBaseFill { get; set; }

        public Node(String locationName)
        {
            this.LocationName = locationName;
            RegionOutlinePoints = new List<VertexPositionColor>();
            triangleListIndices = new List<int>();
            NarrativeEvents = new List<NarrativeEvent>();
            NarrativeObjects = new List<NarrativeObject>();
            EntikaClassInstances = new List<EntikaClassInstance>();
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Node n = (Node)obj;
            return LocationName.Equals(n.LocationName);
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
