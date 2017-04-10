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

            // Determine predicates according to effect
            Dictionary<NarrativePredicate, bool> predicates = new Dictionary<NarrativePredicate, bool>();
            foreach (var effect in nEvent.NarrativeAction.Effects)
            {
                var predicate = new NarrativePredicate();
                predicate.PredicateType = effect.PredicateType;
                // Map action parameters to effect, thus affected event objects to effect
                for (int j = 0; j < nEvent.NarrativeAction.Parameters.Count; j++)
                {
                    for (int k = 0; k < effect.ArgumentsAffected.Count; k++)
                    {
                        if (nEvent.NarrativeAction.Parameters[j].Name.Equals(effect.ArgumentsAffected[k].Name))
                        {
                            predicate.NarrativeObjects.Add(nEvent.NarrativeObjects[j]);
                        }
                    }
                }
                predicates.Add(predicate, effect.Value);
            }
            // Add or remove predicate to already known predicates
            foreach (KeyValuePair<NarrativePredicate, bool> element in predicates)
            {
                var predicateInstanceToAddOrRemove = new NarrativePredicateInstance(element.Key);
                var test = next.NarrativePredicateInstances[0].Equals(predicateInstanceToAddOrRemove);
                // Create new predicate instance and add it to the know predicate instances if doesn't already exist
                var equalInstance = (from predicateInstance in next.NarrativePredicateInstances
                                     where predicateInstance.Equals(predicateInstanceToAddOrRemove)
                                     select predicateInstance).FirstOrDefault();
                if (equalInstance != null)
                {
                    if(!element.Value)
                    {
                        next.NarrativePredicateInstances.Remove(equalInstance);
                    }
                }
                else
                {
                    if(element.Value)
                    {
                        next.NarrativePredicateInstances.Add(predicateInstanceToAddOrRemove);
                    }
                }
            }
        }
    }
}
