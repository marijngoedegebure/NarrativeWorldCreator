using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.ViewModel;
using NarrativeWorldCreator.Views;
using Semantics.Entities;
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
        internal LocationNode selectedNode;

        internal List<EntikaInstance> SelectedEntikaInstances;
        internal NarrativeTimePoint SelectedTimePoint;

        internal Configuration WorkInProgressConfiguration;
        internal EntikaInstance InstanceOfObjectToAdd;

        internal List<GPUConfigurationResult> GeneratedConfigurations;
        internal int LeftSelectedGPUConfigurationResult = -1;
        internal int RightSelectedGPUConfigurationResult = -1;

        internal bool editing = false;

        internal EntikaInstance MousePositionTest;

        internal virtual void ChangeSelectedObject(EntikaInstance ieo) {}

        internal virtual void RemoveSelectedInstances(List<EntikaInstanceValuedPredicate> instances) { }

        internal virtual void RefreshSelectedObjectView() {}

        internal virtual void ChangeUIToMainMenu() { }

        internal virtual void SuggestNewPositions() { }

        internal virtual void Back() { }

        internal virtual void Next() { }

        internal virtual void GenerateConfigurations() { }
    }
}
