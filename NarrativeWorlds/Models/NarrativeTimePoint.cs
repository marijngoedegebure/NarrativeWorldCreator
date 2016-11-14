using PDDLNarrativeParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class NarrativeTimePoint
    {
        // Narrative event associated with this timepoint
        NarrativeEvent NarrativeEvent { get; set; }
        // List of narrative characters and their location when this event needs to occur
        public Dictionary<NarrativeCharacter, Node> LocationOfNarrativeCharacters { get; set; }
        // List of narrative objects and their location when this event needs to occur
        public Dictionary<NarrativeThing, Node> LocationOfNarrativeThings { get; set; }

        public NarrativeTimePoint()
        {
            LocationOfNarrativeCharacters = new Dictionary<NarrativeCharacter, Node>();
            LocationOfNarrativeThings = new Dictionary<NarrativeThing, Node>();
        }
    }
}
