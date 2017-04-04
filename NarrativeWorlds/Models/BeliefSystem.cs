using NarrativeWorlds.Models.NarrativeRegionFill;
using PDDLNarrativeParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds.Models
{
    public static class BeliefSystem
    {
        public static void InitializeFirstTimePoint(NarrativeTimePoint first, List<NarrativePredicate> predicates)
        {
            foreach (var predicate in predicates)
            {
                first.NarrativePredicateInstances.Add(new NarrativePredicateInstance(predicate));
            }
        }

        public static void ApplyEventStoreInNextTimePoint(NarrativeTimePoint initial, NarrativeEvent nEvent, NarrativeTimePoint next)
        {
            // Copy initial to next
            next.CopyInstancedNarrativePredicates(initial);

            // Transform (instantiate or remove) predicate instances according to event
            foreach (var effect in nEvent.NarrativeAction.Effects)
            {
                for (int j = 0; j < nEvent.NarrativeAction.Parameters.Count; j++)
                {
                    if (nEvent.NarrativeAction.Effects[i].Equals(nEvent.NarrativeAction.Parameters[j]))
                    {
                        // 
                        nEvent.NarrativeAction.Parameters[j+1]
                    }
                }
            }
        }
    }
}
