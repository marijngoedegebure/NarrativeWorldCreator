using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLNarrativeParser
{
    public class NarrativeEvent
    {
        public int NarrativeEventId { get; set; }
        public NarrativeAction NarrativeAction { get; set; }
        public IList<NarrativeObject> NarrativeObjects { get; set; }
        public NarrativeObject Location { get; set; }

        public NarrativeEvent()
        {
            NarrativeObjects = new List<NarrativeObject>();
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            NarrativeEvent p = (NarrativeEvent)obj;
            return (NarrativeEventId == p.NarrativeEventId);
        }
    }
}
