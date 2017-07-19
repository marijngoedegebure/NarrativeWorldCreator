using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    internal class RelationshipDelta
    {
        internal RelationshipDeltaType DT;

        internal enum RelationshipDeltaType
        {
            Add = 0,
            Remove = 1
        }

        internal int TimePoint;

        internal RelationshipInstance RelatedInstance;

        public RelationshipDelta(int tp, RelationshipInstance inst, RelationshipDeltaType deltaType)
        {
            this.TimePoint = tp;
            this.DT = deltaType;
            this.RelatedInstance = inst;
        }
    }
}
