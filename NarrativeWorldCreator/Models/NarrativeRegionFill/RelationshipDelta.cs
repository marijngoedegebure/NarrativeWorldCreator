using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    public enum RelationshipDeltaType
    {
        Add = 0,
        Remove = 1
    }

    public class RelationshipDelta
    {
        public RelationshipDeltaType DT { get; set; }

        public int TimePoint { get; set; }

        public RelationshipInstance RelatedInstance { get; set; }

        public RelationshipDelta(int tp, RelationshipInstance inst, RelationshipDeltaType deltaType)
        {
            this.TimePoint = tp;
            this.DT = deltaType;
            this.RelatedInstance = inst;
        }
    }
}
