using NarrativeWorldCreator.MetricEngines;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.ViewModel;
using Semantics.Entities;
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
    /// Interaction logic for RegionInitPage.xaml
    /// </summary>
    public partial class ClassSelectionPage : Page
    {
        public LocationNode selectedNode;
        public NarrativeTimePoint SelectedTimePoint;

        public ClassSelectionPage(LocationNode selectedNode, NarrativeTimePoint ntp)
        {
            InitializeComponent();
            this.selectedNode = selectedNode;
            this.SelectedTimePoint = ntp;

            // Retrieve required and dependent objects
            TangibleObjectMetricEngine.Weights = Constants.AllMetricWeights;
            TangibleObjectMetricEngine.BuildUpTOTree(SystemStateTracker.NarrativeWorld.AvailableTangibleObjects.Where(ato => !ato.DefaultName.Equals(Constants.Floor)).ToList());
            TangibleObjectMetricEngine.ApplyRequiredAndDependencies(this.SelectedTimePoint.PredicatesFilteredByCurrentLocation);
            var requiredAndDependentTTOs = TangibleObjectMetricEngine.TTOs.Where(tto => tto.Required || tto.RequiredDependency).ToList();
            // Create List for viewing
            var requiredTOs = new List<TangibleObject>();
            foreach (var reqTO in requiredAndDependentTTOs)
            {
                requiredTOs.Add(reqTO.TangibleObject);
            }
            var TOSVM = new TangibleObjectsSwapViewModel();
            TOSVM.Load(requiredTOs);
            this.DataContext = TOSVM;
        }

        private void btnGraphPage_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GraphPage());
        }

        private void btnRegionPage_Click(object sender, RoutedEventArgs e)
        {
            var requiredTOs = (this.DataContext as TangibleObjectsSwapViewModel).TangibleObjects;
            var selectedTOs = (this.SelectedSwapView.DataContext as TangibleObjectsSwapViewModel).TangibleObjects;
            var fillAvailableTO = new List<TangibleObject>();
            foreach (var to in requiredTOs)
            {
                fillAvailableTO.Add(to);
            }
            foreach( var to in selectedTOs)
            {
                fillAvailableTO.Add(to);
            }
            this.SelectedTimePoint.AvailableTangibleObjects = fillAvailableTO;
            if (SelectedTimePoint.FloorCreated)
                this.NavigationService.Navigate(new MainModeRegionPage(selectedNode, this.SelectedTimePoint));
            else
                this.NavigationService.Navigate(new RegionCreationPage(selectedNode, this.SelectedTimePoint));
        }

        private void AvailableSwapView_Loaded(object sender, RoutedEventArgs e)
        {
            var TOSVM = new TangibleObjectsSwapViewModel();
            // Remove required from available TO list
            var toList = SystemStateTracker.NarrativeWorld.AvailableTangibleObjects.Where(ato => !ato.DefaultName.Equals(Constants.Floor)).ToList();
            foreach (var to in (this.DataContext as TangibleObjectsSwapViewModel).TangibleObjects)
            {
                toList.Remove(to);
            }
            // Remove already selected from available TO list
            foreach (var to in (this.SelectedTimePoint.AvailableTangibleObjects))
            {
                toList.Remove(to);
            }
            TOSVM.Load(toList);
            this.AvailableSwapView.DataContext = TOSVM;
        }

        private void SelectedSwapView_Loaded(object sender, RoutedEventArgs e)
        {
            var toArraySelected = new TangibleObject[this.SelectedTimePoint.AvailableTangibleObjects.Count];
            this.SelectedTimePoint.AvailableTangibleObjects.CopyTo(toArraySelected);
            var toListSelected = toArraySelected.ToList();
            // Remove required from list of selected
            foreach (var to in (this.DataContext as TangibleObjectsSwapViewModel).TangibleObjects)
            {
                toListSelected.Remove(to);
            }

            var TOSVM = new TangibleObjectsSwapViewModel();
            TOSVM.Load(toListSelected);
            this.SelectedSwapView.DataContext = TOSVM;
        }

        private void btnMoveToSelected(object sender, RoutedEventArgs e)
        {
            // Add available to selected, remove to's from available
            var AvailableVM = (this.AvailableSwapView.DataContext as TangibleObjectsSwapViewModel);
            var SelectedVM = (this.SelectedSwapView.DataContext as TangibleObjectsSwapViewModel);
            var selectedItems = new TangibleObject[this.AvailableSwapView.TOLV.SelectedItems.Count];
            this.AvailableSwapView.TOLV.SelectedItems.CopyTo(selectedItems, 0);
            for (int i = 0; i < selectedItems.Count(); i++)
            {
                // Remove from Available
                AvailableVM.Remove(selectedItems[i]);

                // Add to selected
                SelectedVM.Add(selectedItems[i]);
            }
        }

        private void btnMoveToAvailable(object sender, RoutedEventArgs e)
        {
            // Add selected to available, remove to's from selected
            var AvailableVM = (this.AvailableSwapView.DataContext as TangibleObjectsSwapViewModel);
            var SelectedVM = (this.SelectedSwapView.DataContext as TangibleObjectsSwapViewModel);
            var selectedItems = new TangibleObject[this.AvailableSwapView.TOLV.SelectedItems.Count];
            this.AvailableSwapView.TOLV.SelectedItems.CopyTo(selectedItems, 0);
            for (int i = 0; i < selectedItems.Count(); i++)
            {
                // Remove from Available
                AvailableVM.Add(selectedItems[i]);

                // Add to selected
                SelectedVM.Remove(selectedItems[i]);
            }
        }
    }
}
