using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.Metrics
{
    public class ObjectValuation
    {
        public double EndValue { get; set; }
        public List<Metric> Metrics { get; set; }

        public ObjectValuation()
        {
            this.Metrics = new List<Metric>();
        }
    }
}
