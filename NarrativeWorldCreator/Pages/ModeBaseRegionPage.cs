using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Pages
{
    public class ModeBaseRegionPage : BaseRegionPage
    {
        internal MainFillingMode CurrentFillingMode = MainFillingMode.MainMenu;

        // Class selection, relationship selection and placement belong to add mode
        // Manual and repositioning to change mode
        internal enum MainFillingMode
        {
            MainMenu = 0,
            ClassSelection = 1,
            RelationshipSelection = 2,
            ManualPlacement = 3,
            AutomatedPlacement = 4,
            Repositioning = 5,
            ManualChange = 6,
            SelectionChangeMode = 7
        }

        internal EntikaInstance InstanceOfObjectToAdd;

        // Used for repositioning
        internal Configuration WorkInProgressConfiguration = new Configuration();

        internal InstanceDelta WIPAdditionDelta;
        internal List<RelationshipDelta> WIPRelationshipDeltas;


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

        internal virtual void UpdateDetailView(NarrativeTimePoint narrativeTimePoint) { }
    }
}
