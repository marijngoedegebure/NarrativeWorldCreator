using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    public class GPUConfigurationResult
    {
        public List<GPUInstanceResult> Instances = new List<GPUInstanceResult>();

        public float TotalCosts;
        public float PairWiseCosts;
        public float VisualBalanceCosts;
        public float FocalPointCosts;
        public float SymmetryCosts;

        public float ClearanceCosts;

        public float SurfaceAreaCosts;
    }
}
