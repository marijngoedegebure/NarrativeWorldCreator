using Semantics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NarrativeWorldCreator.Solvers;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    public class RelationshipInstance
    {
        public EntikaInstance Source { get; set; }
        public EntikaInstance Target { get; set; }
        public Relationship BaseRelationship { get; set; }

        // Parameters for energy function
        public bool Valued { get; set; }
        public double? TargetRangeStart { get; set; }
        public double? TargetRangeEnd { get; set; }

        public double Energy { get; set; }

        public RelationshipInstance()
        {
            Valued = false;
        }

        public RelationshipInstance(RelationshipInstance rel)
        {
            this.Valued = rel.Valued;
            this.TargetRangeStart = rel.TargetRangeStart;
            this.TargetRangeEnd = rel.TargetRangeEnd;
            this.Energy = rel.Energy;
            this.BaseRelationship = rel.BaseRelationship;
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            RelationshipInstance rel = (RelationshipInstance)obj;
            if (!this.Source.Equals(rel.Source))
                return false;
            if (!this.Target.Equals(rel.Target))
                return false;
            if (!this.BaseRelationship.Equals(rel.BaseRelationship))
                return false;
            if (!this.Valued.Equals(rel.Valued))
                return false;
            if (!this.TargetRangeStart.Equals(rel.TargetRangeStart))
                return false;
            if (!this.TargetRangeEnd.Equals(rel.TargetRangeEnd))
                return false;
            if (!this.Energy.Equals(rel.Energy))
                return false;

            return true;
        }
    }
}
