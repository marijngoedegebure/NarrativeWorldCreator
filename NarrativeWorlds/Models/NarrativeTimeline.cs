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

        public List<NarrativeTimePoint> getNarrativeTimePointsWithNode(Node node)
        {

            return (from b in (from a in NarrativeTimePoints
                where a.Location != null
                select a) where b.Location.Equals(node) select b).ToList();
        }
    }
}
