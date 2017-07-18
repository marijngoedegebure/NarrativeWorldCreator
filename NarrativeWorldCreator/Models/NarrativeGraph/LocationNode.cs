using Common.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Geometry;

namespace NarrativeWorldCreator.Models.NarrativeGraph
{
    public class LocationNode
    {
        public string LocationName { get; set; }
        // Subset of timepoints for this node
        public List<NarrativeTimePoint> TimePoints { get; set; }
        public string LocationType { get; set; }

        // List of TangibleObjects selected for this region/timepoint
        public List<TangibleObject> AvailableTangibleObjects { get; set; }

        public LocationNode(String locationName, string locationType)
        {
            this.LocationName = locationName;
            this.LocationType = locationType;
            this.TimePoints = new List<NarrativeTimePoint>();
            AvailableTangibleObjects = new List<TangibleObject>();
        }

        public override bool Equals(System.Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            LocationNode n = (LocationNode)obj;
            return LocationName.Equals(n.LocationName);
        }

        public override int GetHashCode()
        {
            return this.LocationName.GetHashCode();
        }
    }
}
