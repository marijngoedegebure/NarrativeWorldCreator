using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NarrativeWorldCreator.Pages
{
    public class BaseRegionPage : Page
    {
        public LocationNode selectedNode;

        public List<EntikaInstance> SelectedEntikaInstances;
        public NarrativeTimePoint SelectedTimePoint { get; internal set; }

        public Configuration WorkInProgressConfiguration;
        public EntikaInstance InstanceOfObjectToAdd;

        public List<GPUConfigurationResult> GeneratedConfigurations;
        public int LeftSelectedGPUConfigurationResult = -1;
        public int RightSelectedGPUConfigurationResult = -1;

        public EntikaInstance MousePositionTest;

        public FillingMode CurrentFillingMode = FillingMode.ClassSelection;

        public enum FillingMode
        {
            None = 0,
            ClassSelection = 1,
            RelationSelectionAndInstancting = 2,
            Placement = 3,
            Repositioning = 4
        }

    }
}
