using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeTime
{
    public class NarrativeTimeline
    {
        public List<NarrativeTimePoint> NarrativeTimePoints { get; set; }
        public List<PredicateType> PredicateTypes { get; set; }

        public NarrativeTimeline()
        {
            NarrativeTimePoints = new List<NarrativeTimePoint>();
            PredicateTypes = new List<PredicateType>();
        }

        public List<NarrativeTimePoint> getNarrativeTimePointsWithNode(LocationNode node)
        {

            return (from b in 
                        (from a in NarrativeTimePoints where a.Location != null select a)
                    where b.Location.Equals(node)
                    select b).ToList();
        }

        internal PredicateType GetPredicateType(string typeName)
        {
            return this.PredicateTypes.Where(pt => pt.Name.Equals(typeName)).FirstOrDefault();
        }
    }
}
