using PDDLNarrativeParser;
using NarrativeWorldCreator.RegionGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models
{
    public class NarrativeWorld
    {
        public Narrative Narrative { get; set; }
        public Graph Graph { get; set; }

        public NarrativeWorld ()
        {
        }
    }
}
