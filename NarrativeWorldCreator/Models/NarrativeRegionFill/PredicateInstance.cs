using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    public class PredicateInstance
    {
        public string Name { get; set; }
        public List<EntikaInstance> AssociatedObjects { get; set; }
    }
}
