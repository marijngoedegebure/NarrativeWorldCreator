using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeInput
{
    public class NarrativeEvent
    {
        public NarrativeAction NarrativeAction { get; set; }
        public IList<NarrativeObject> NarrativeObjects { get; set; }

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
            return this.NarrativeAction.Equals(p.NarrativeAction);
        }

        public override int GetHashCode()
        {
            return this.NarrativeAction.GetHashCode();
        }
    }
}
