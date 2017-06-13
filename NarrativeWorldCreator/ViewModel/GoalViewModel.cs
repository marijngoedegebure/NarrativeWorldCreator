using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class GoalViewModel
    {
        public GoalViewModel(Predicate pred, bool v)
        {
            this.PredicateType = pred.PredicateType;
            this.EntikaClassNames = pred.EntikaClassNames;
            this.Achieved = v;
        }

        public PredicateType PredicateType { get; set; }
        public List<string> EntikaClassNames { get; set; }
        public bool Achieved { get; set; }
    }
}
