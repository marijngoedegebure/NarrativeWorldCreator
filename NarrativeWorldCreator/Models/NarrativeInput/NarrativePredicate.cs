using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeInput
{
    public class NarrativePredicate
    {
        public int NarrativePredicateId { get; set; }
        public PredicateType PredicateType { get; set; }
        public IList<NarrativeObject> NarrativeObjects { get; set; }

        public NarrativePredicate()
        {
            NarrativeObjects = new List<NarrativeObject>();
        }

        public override bool Equals(object obj)
        {
            var item = obj as NarrativePredicate;

            if (item == null)
            {
                return false;
            }
            if (this.NarrativeObjects.Count != item.NarrativeObjects.Count)
            {
                return false;
            }
            for(int i = 0; i < this.NarrativeObjects.Count; i++)
            {
                if(!this.NarrativeObjects[i].Equals(item.NarrativeObjects[i]))
                {
                    return false;
                }
            }
            return this.PredicateType.Equals(item.PredicateType);
        }

    }
}
