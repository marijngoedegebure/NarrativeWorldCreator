using NarrativeWorldCreator.PDDL;
using NarrativeWorldCreator.RegionGraph.GraphDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator
{
    static class SystemStateTracker
    {
        public enum NarrativeLoaded
        {
            Empty = 0,
            Loaded = 1
        }
        public enum CurrentPage
        {
            Init = 0,
            Graph = 1,
            Region = 2
        }

        public static Narrative narrative = new Narrative();
        public static Graph graph = new Graph();
        public static NarrativeLoaded narrativeLoaded = NarrativeLoaded.Empty;
        public static CurrentPage currentPage = CurrentPage.Init;
    }
}
