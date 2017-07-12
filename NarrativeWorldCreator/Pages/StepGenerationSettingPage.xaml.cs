using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NarrativeWorldCreator.Pages
{
    /// <summary>
    /// Interaction logic for StepGenerationSettingPage.xaml
    /// </summary>
    public partial class StepGenerationSettingPage : Page
    {
        public LocationNode selectedNode;

        public NarrativeTimePoint SelectedTimePoint { get; internal set; }

        public StepGenerationSettingPage(LocationNode sldNode, NarrativeTimePoint sldTimePoint)
        {
            this.selectedNode = sldNode;
            this.SelectedTimePoint = sldTimePoint;
            InitializeComponent();
            this.ClassSelector.IsChecked = SystemStateTracker.SelectClassSystem;
            this.RelationshipSelector.IsChecked = SystemStateTracker.SelectRelationshipSystem;
            this.InstanceSelector.IsChecked = SystemStateTracker.SelectInstancesSystem;
            this.PositionSelector.IsChecked = SystemStateTracker.SelectPositionSystem;

        }

        private void btnSaveAndBackRegionEditor(object sender, RoutedEventArgs e)
        {
            // Save changes
            SystemStateTracker.SelectClassSystem = this.ClassSelector.IsChecked.GetValueOrDefault();
            SystemStateTracker.SelectRelationshipSystem = this.RelationshipSelector.IsChecked.GetValueOrDefault();
            SystemStateTracker.SelectInstancesSystem = this.InstanceSelector.IsChecked.GetValueOrDefault();
            SystemStateTracker.SelectPositionSystem = this.PositionSelector.IsChecked.GetValueOrDefault();
            this.NavigationService.Navigate(new MainModeRegionPage(this.selectedNode, this.SelectedTimePoint));
        }

        private void btnDiscardAndBackRegionEditor(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MainModeRegionPage(this.selectedNode, this.SelectedTimePoint));
        }
    }
}
