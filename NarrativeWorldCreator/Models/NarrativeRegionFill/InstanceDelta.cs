using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    public enum InstanceDeltaType
    {
        Add = 0,
        Remove = 1,
        Change = 2
    }

    public class InstanceDelta
    {
        public InstanceDeltaType DT { get; set; }

        public EntikaInstance RelatedInstance { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        // Timepoint relative to selected node
        public int TimePoint;

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
