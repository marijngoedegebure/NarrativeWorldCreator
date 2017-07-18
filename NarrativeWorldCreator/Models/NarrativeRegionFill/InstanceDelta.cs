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
            Move = 2
        }

        EntikaInstance RelatedInstance;

        Vector3 Position;

        public InstanceDelta(EntikaInstance inst, InstanceDeltaType deltaType, Vector3? pos)
        {
            this.DT = deltaType;
            this.RelatedInstance = inst;
            this.Position = pos.GetValueOrDefault();
        }
    }
}
