using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    public class Predicate
    {
        public PredicateType PredicateType { get; set; }
        // Holds a string representation of entika class and locations
        public List<string> EntikaClassNames { get; set; }
        // public IList<NarrativeObject> NarrativeObjects { get; set; }

        public Predicate()
        {
            EntikaClassNames = new List<string>();
        }

        public Predicate(PredicateType pt, List<string> classNamesList)
        {
            this.PredicateType = pt;
            this.EntikaClassNames = classNamesList;
        }

        public override bool Equals(object obj)
        {
            var item = obj as Predicate;

            if (item == null)
            {
                return false;
            }
            if (this.EntikaClassNames.Count != item.EntikaClassNames.Count)
            {
                return false;
            }
            for(int i = 0; i < this.EntikaClassNames.Count; i++)
            {
                if(!this.EntikaClassNames[i].Equals(item.EntikaClassNames[i]))
                {
                    return false;
                }
            }
            return this.PredicateType.Equals(item.PredicateType);
        }

        public override int GetHashCode()
        {
            return this.EntikaClassNames.Count.GetHashCode() + this.PredicateType.GetHashCode();
        }

    }
}
