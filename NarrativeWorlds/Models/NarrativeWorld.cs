using PDDLNarrativeParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class NarrativeWorld
    {
        public Narrative Narrative { get; set; }
        public Graph Graph { get; set; }
        public NarrativeTimeline NarrativeTimeline { get; set; }
        public List<NarrativeCharacter> NarrativeCharacters { get; set; }
        public List<NarrativeThing> NarrativeThings { get; set; }

        public NarrativeWorld ()
        {
            NarrativeCharacters = new List<NarrativeCharacter>();
            NarrativeThings = new List<NarrativeThing>();
        }
    }
}
