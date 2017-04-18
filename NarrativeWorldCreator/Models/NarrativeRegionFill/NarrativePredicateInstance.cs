using NarrativeWorldCreator.Models.NarrativeInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    public class NarrativePredicateInstance
    {
        public NarrativePredicate NarrativePredicate { get; set; }

        public NarrativePredicateInstance(NarrativePredicate predicate)
        {
            this.NarrativePredicate = predicate;
        }

        public override bool Equals(object obj)
        {
            var item = obj as NarrativePredicateInstance;

            if (item == null)
            {
                return false;
            }
            return this.NarrativePredicate.Equals(item.NarrativePredicate);
        }
    }
}
