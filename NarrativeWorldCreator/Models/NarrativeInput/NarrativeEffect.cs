using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeInput
{
    public class NarrativeEffect
    {
        public PredicateType PredicateType { get; set; }
        public List<NarrativeArgument> ArgumentsAffected { get; set; }
        public bool Value { get; set; }

        public NarrativeEffect()
        {
            this.Value = true;
            this.ArgumentsAffected = new List<NarrativeArgument>();
        }
    }
}
