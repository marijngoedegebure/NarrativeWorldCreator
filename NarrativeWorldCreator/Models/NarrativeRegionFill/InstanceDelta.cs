using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    internal class InstanceDelta
    {
        internal InstanceDeltaType DT;

        internal enum InstanceDeltaType
        {
            Add = 0,
            Remove = 1,
            Change = 2
        }

        internal EntikaInstance RelatedInstance;

        internal Vector3 Position;
        internal Vector3 Rotation;

        // Timepoint relative to selected node
        internal int TimePoint;

        public InstanceDelta(int tp, EntikaInstance inst, InstanceDeltaType deltaType, Vector3? pos, Vector3? rot)
        {
            this.TimePoint = tp;
            this.DT = deltaType;
            this.RelatedInstance = inst;
            this.Position = pos.GetValueOrDefault();
            this.Rotation = rot.GetValueOrDefault();
        }
    }
}
