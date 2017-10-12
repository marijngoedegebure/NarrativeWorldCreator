using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.TestMetrics
{
    public class TimeSpentOnActions
    {
        public long TimeSpentOnAddActions = 0;
        public long TimeSpentOnAllChangeActions = 0;
        public long TimeSpentOnManualChangeActions = 0;
        public long TimeSpentOnAutomatedChangeActions = 0;
        public long TimeSpentOnRemoveActions = 0;

        public override string ToString()
        {
            return "Add: " + TimeSpentOnAddActions + ", Manual change: " + this.TimeSpentOnManualChangeActions + ", Automated change: " + this.TimeSpentOnAutomatedChangeActions + ", Remove: " + this.TimeSpentOnRemoveActions;
        }
    }
}
