using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds.Models.Metrics
{
    public class EntikaInstanceValued
    {
        public EntikaInstance EntikaInstance { get; set; }
        public double EndValue { get; set; }
        public List<Metric> Metrics { get; set; }

        public EntikaInstanceValued(EntikaInstance instance)
        {
            this.EntikaInstance = instance;
            this.Metrics = new List<Metric>();
        }
    }
}
