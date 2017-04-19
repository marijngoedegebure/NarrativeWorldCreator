using NarrativeWorldCreator.Models.NarrativeInput;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Parsers
{
    public static class BeliefSystem
    {
        public static void InitializeFirstTimePoint(NarrativeTimePoint first, List<Predicate> predicates)
        {
            foreach (var predicate in predicates)
            {
                first.AllPredicates.Add(new Predicate {
                    PredicateType = predicate.PredicateType,
                    EntikaClassNames = predicate.EntikaClassNames
                });
            }
        }

        public static void ApplyEventStoreInNextTimePoint(NarrativeTimePoint initial, NarrativeEvent nEvent, NarrativeTimePoint next)
        {
            // Copy initial to next
            next.CopyPredicates(initial);

            // Determine predicates according to effect
            Dictionary<Predicate, bool> predicates = new Dictionary<Predicate, bool>();
            foreach (var effect in nEvent.NarrativeAction.Effects)
            {
                var predicate = new Predicate();
                predicate.PredicateType = effect.PredicateType;
                // Map action parameters to effect, thus affected event objects to effect
                for (int j = 0; j < nEvent.NarrativeAction.Parameters.Count; j++)
                {
                    for (int k = 0; k < effect.ArgumentsAffected.Count; k++)
                    {
                        if (nEvent.NarrativeAction.Parameters[j].Name.Equals(effect.ArgumentsAffected[k].Name))
                        {
                            predicate.EntikaClassNames.Add(nEvent.NarrativeObjects[j].Name);
                        }
                    }
                }
                predicates.Add(predicate, effect.Value);
            }
            // Add or remove predicate to already known predicates
            foreach (KeyValuePair<Predicate, bool> element in predicates)
            {
                var predicateToAddOrRemove = element.Key;
                var test = next.AllPredicates[0].Equals(predicateToAddOrRemove);
                // Create new predicate instance and add it to the know predicate instances if doesn't already exist
                var equalInstance = (from predicate in next.AllPredicates
                                     where predicate.Equals(predicateToAddOrRemove)
                                     select predicate).FirstOrDefault();
                if (equalInstance != null)
                {
                    if(!element.Value)
                    {
                        next.AllPredicates.Remove(equalInstance);
                    }
                }
                else
                {
                    if(element.Value)
                    {
                        next.AllPredicates.Add(predicateToAddOrRemove);
                    }
                }
            }
        }
    }
}
