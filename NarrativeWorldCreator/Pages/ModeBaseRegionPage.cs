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
        internal int TopLeftSelectedGPUConfigurationResult = -1;
        internal int BottomLeftSelectedGPUConfigurationResult = -1;
        internal int TopRightSelectedGPUConfigurationResult = -1;
        internal int BottomRightSelectedGPUConfigurationResult = -1;

        internal virtual void Back() { }

        internal virtual void Next() { }

        internal virtual void SuggestNewPositions() { }

        internal virtual void GenerateConfigurations() { }

        internal override void UpdateSelectedObjectDetailView() { }

        internal override void RefreshSelectedObjectView() { }
    }
}
