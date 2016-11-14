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
        // Narrative event associated with this timepoint, just for potential later reference, should not be necessary
        public NarrativeEvent NarrativeEvent { get; set; }
        // List of narrative characters and their location when this event needs to occur
        public Dictionary<NarrativeCharacter, Node> LocationOfNarrativeCharacters { get; set; }
        // List of narrative objects and their location when this event needs to occur
        public Dictionary<NarrativeThing, Node> LocationOfNarrativeThings { get; set; }

        public NarrativeTimePoint()
        {
            LocationOfNarrativeCharacters = new Dictionary<NarrativeCharacter, Node>();
            LocationOfNarrativeThings = new Dictionary<NarrativeThing, Node>();
        }

        internal void copy(NarrativeTimePoint initialTimePoint)
        {
            foreach(KeyValuePair<NarrativeCharacter, Node> entry in initialTimePoint.LocationOfNarrativeCharacters)
            {
                this.LocationOfNarrativeCharacters[entry.Key] = entry.Value;
            }

            foreach (KeyValuePair<NarrativeThing, Node> entry in initialTimePoint.LocationOfNarrativeThings)
            {
                this.LocationOfNarrativeThings[entry.Key] = entry.Value;
            }
        }
    }
}
