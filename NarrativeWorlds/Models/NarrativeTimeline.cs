using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class NarrativeTimeline
    {
        public List<NarrativeTimePoint> NarrativeTimePoints { get; set; }

        public NarrativeTimeline()
        {
            NarrativeTimePoints = new List<NarrativeTimePoint>();
        }
    }
}
