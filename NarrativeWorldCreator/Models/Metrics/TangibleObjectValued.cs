using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.Metrics
{
    public class TangibleObjectValued : ObjectValuation
    {
        public TangibleObject TangibleObject { get; set; }

        public TangibleObjectValued (TangibleObject to) : base()
        {
            this.TangibleObject = to;
        }
    }
}
