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

        RelationshipInstance RelatedInstance;

        public RelationshipDelta(RelationshipInstance inst, RelationshipDeltaType deltaType)
        {
            this.DT = deltaType;
            this.RelatedInstance = inst;
        }
    }
}
