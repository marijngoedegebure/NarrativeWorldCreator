using PDDLNarrativeParser;
using Semantics.Entities;
using SemanticsEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class NarrativeObjectEntikaLink
    {
        public NarrativeObject NarrativeObject { get; set; }
        public TangibleObject TangibleObject { get; set; }

        public NarrativeObjectEntikaLink(NarrativeObject narrativeObject, TangibleObject to)
        {
            this.NarrativeObject = narrativeObject;
            this.TangibleObject = to;
        }
    }
}
