
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Semantics.Entities;
using NarrativeWorldCreator.Models.NarrativeInput;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.Models.NarrativeGraph;

namespace NarrativeWorldCreator.Models
{
    public class NarrativeWorld
    {
        // Narrative information
        public Narrative Narrative { get; set; }
        public Graph Graph { get; set; }
        public NarrativeTimeline NarrativeTimeline { get; set; }

        // Entika information
        public List<TangibleObject> AvailableTangibleObjects { get; set; }
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
