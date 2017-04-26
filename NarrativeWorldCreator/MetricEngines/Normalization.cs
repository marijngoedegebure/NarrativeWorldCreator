using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.MetricEngines
{
    public class Normalization
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public Normalization()
        {
            this.Min = double.MaxValue;
            this.Max = double.MinValue;
        }
    }
}
