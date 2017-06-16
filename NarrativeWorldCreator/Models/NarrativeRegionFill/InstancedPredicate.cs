using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    public class InstancedPredicate
    {
        public Predicate Predicate { get; set; }
        public List<EntikaInstance> Instances { get; set; }

        public InstancedPredicate(Predicate p, List<EntikaInstance> eiList)
        {
            this.Predicate = p;
            this.Instances = eiList;
        }

        public override bool Equals(object obj)
        {
            var item = obj as InstancedPredicate;

            if (item == null)
            {
                return false;
            }
            if (this.Instances.Count != item.Instances.Count)
                return false;

            for (int i = 0; i < this.Instances.Count; i++)
            {
                if (!this.Instances[i].Equals(item.Instances[i]))
                {
                    return false;
                }
            }
            return this.Predicate.Equals(item.Predicate);
        }

        public override int GetHashCode()
        {
            return this.Instances.Count.GetHashCode() + this.Predicate.GetHashCode();
        }
    }
}
