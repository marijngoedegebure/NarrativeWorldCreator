using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeInput
{
    public class NarrativeAction
    {
        public int NarrativeActionId { get; set; }
        public string Name { get; set; }
        public IList<NarrativeArgument> Parameters { get; set; }
        public List<NarrativeEffect> Preconditions { get; set; }
        public List<NarrativeEffect> Effects { get; set; }
        // Missing: preconditions, effects

        public NarrativeAction()
        {
            Parameters = new List<NarrativeArgument>();
            Preconditions = new List<NarrativeEffect>();
            Effects = new List<NarrativeEffect>();
        }
    }
}
