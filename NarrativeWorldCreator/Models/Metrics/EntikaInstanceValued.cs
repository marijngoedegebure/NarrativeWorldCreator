using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.Metrics
{
    public class EntikaInstanceValued : ObjectValuation
    {
        public EntikaInstance EntikaInstance { get; set; }

        public EntikaInstanceValued(EntikaInstance instance) : base()
        {
            this.EntikaInstance = instance;
        }
    }
}
