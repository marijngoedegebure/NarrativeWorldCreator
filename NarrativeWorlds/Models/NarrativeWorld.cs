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

        public NarrativeCharacter getNarrativeCharacter(string name)
        {
            return (from nc in NarrativeCharacters
                    where nc.Name.Equals(name)
                    select nc).Single();
        }

        internal NarrativeThing getNarrativeThing(string name)
        {
            return (from nt in NarrativeThings
                    where nt.Name.Equals(name)
                    select nt).Single();
        }
    }
}
