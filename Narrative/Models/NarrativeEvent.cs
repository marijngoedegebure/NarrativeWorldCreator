using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narratives
{
    public class NarrativeEvent
    {
        public int NarrativeEventId { get; set; }
        public NarrativeAction NarrativeAction { get; set; }
        public IList<NarrativeObject> NarrativeObjects { get; set; }

        public NarrativeEvent()
        {
            NarrativeObjects = new List<NarrativeObject>();
        }
    }
}
