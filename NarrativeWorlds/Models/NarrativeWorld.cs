using PDDLNarrativeParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Semantics.Entities;

namespace NarrativeWorlds
{
    public class NarrativeWorld
    {
        public Narrative Narrative { get; set; }
        public Graph Graph { get; set; }
        public NarrativeTimeline NarrativeTimeline { get; set; }
        public List<NarrativeObjectEntikaLink> NarrativeObjectEntikaLinks { get; set; }

        public NarrativeWorld ()
        {
            NarrativeObjectEntikaLinks = new List<NarrativeObjectEntikaLink>();
        }

        public NarrativeObjectEntikaLink getNarrativeObjectEntikaLink (string name)
        {
            return (from nc in NarrativeObjectEntikaLinks
                    where nc.NarrativeObject.Name.Equals(name)
                    select nc).Single();
        }
    }
}
