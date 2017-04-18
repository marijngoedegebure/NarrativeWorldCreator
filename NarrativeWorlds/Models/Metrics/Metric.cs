using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds.Models.Metrics
{
    public class Metric
    {
        public MetricType MetricType { get; set; }
        public double Value { get; set; }

        public Metric(MetricType mt, double value)
        {
            this.MetricType = mt;
            this.Value = value;
        }
    }
}
