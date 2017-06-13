using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.Metrics
{
    public class Metric
    {
        public MetricType MetricType { get; set; }
        public double Value { get; set; }
        public double Weight { get; set; }

        public Metric(MetricType mt, double value, double weight)
        {
            this.MetricType = mt;
            this.Value = value;
            this.Weight = weight;
        }

        public override string ToString()
        {
            return MetricType.ToString() + " " + Value + " " + Weight;
        }
    }
}
