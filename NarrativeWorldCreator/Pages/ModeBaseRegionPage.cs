using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Pages
{
    public class ModeBaseRegionPage : BaseRegionPage
    {
        // Newest way of generating stuff
        internal Dictionary<Configuration, GPUConfigurationResult> WIPandGenerationDict = new Dictionary<Configuration, GPUConfigurationResult>();

        // Used for repositioning
        internal Configuration WorkInProgressConfiguration = new Configuration();

        internal List<GPUConfigurationResult> GeneratedConfigurations;
        internal int LeftSelectedGPUConfigurationResult = -1;
        internal int RightSelectedGPUConfigurationResult = -1;

        // Other stuff
        internal bool editing = false;

        internal virtual void Back() { }

        internal virtual void Next() { }

        internal virtual void SuggestNewPositions() { }

        internal virtual void GenerateConfigurations() { }
    }
}
