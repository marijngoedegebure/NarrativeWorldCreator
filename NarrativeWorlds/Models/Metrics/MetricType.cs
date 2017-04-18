using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds.Models.Metrics
{
    public class MetricType
    {
        public string Name { get; set; }
        public MetricType(string name)
        {
            this.Name = name;
        }
    }
}
