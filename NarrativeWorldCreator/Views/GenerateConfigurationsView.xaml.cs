using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.ViewModel;
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

namespace NarrativeWorldCreator.Views
{
    /// <summary>
    /// Interaction logic for SuggestedConfigurationsView.xaml
    /// </summary>
    public partial class GenerateConfigurationsView : UserControl
    {
        public GenerateConfigurationsView()
        {
            InitializeComponent();
        }

        private void RefreshConfigurations(object sender, RoutedEventArgs e)
        {
            // Retrieve values of input fields (that may have changed) and pass them to systemstatetracker
            SystemStateTracker.WeightFocalPoint = float.Parse(this.WeightFocalPoint.Text);
            SystemStateTracker.WeightPairWise = float.Parse(this.WeightPairWise.Text);
            SystemStateTracker.WeightSymmetry = float.Parse(this.WeightSymmetry.Text);
            SystemStateTracker.WeightVisualBalance = float.Parse(this.WeightVisualBalance.Text);
            SystemStateTracker.centroidX = double.Parse(this.centroidX.Text);
            SystemStateTracker.centroidY = double.Parse(this.centroidY.Text);
            SystemStateTracker.focalX = double.Parse(this.focalX.Text);
            SystemStateTracker.focalY = double.Parse(this.focalY.Text);
            SystemStateTracker.focalRot = double.Parse(this.focalRot.Text);
            SystemStateTracker.gridxDim = int.Parse(this.gridxDim.Text);

            // Call configuration generation using current timepoint
            var regionPage = GetRegionPage();
            regionPage.GenerateConfigurations();

            // Update list of configurations using generated list of regionpage
            var gcVM = (GenerateConfigurationsViewModel)this.DataContext;
            gcVM.Load(regionPage.GeneratedConfigurations);
            // this.DataContext = gcVM;
        }

        private RegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (RegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private void ConfigurationSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var regionPage = GetRegionPage();
            if (this.ConfigurationsList.SelectedItems.Count > 0)
                regionPage.SelectedGPUConfigurationResult = this.ConfigurationsList.SelectedIndex;
            else
                regionPage.SelectedGPUConfigurationResult = -1;
        }

        private void ResetSelectionList(object sender, RoutedEventArgs e)
        {
            this.ConfigurationsList.SelectedIndex = -1;
            var regionPage = GetRegionPage();

            regionPage.SelectedGPUConfigurationResult = this.ConfigurationsList.SelectedIndex;
        }
    }
}
