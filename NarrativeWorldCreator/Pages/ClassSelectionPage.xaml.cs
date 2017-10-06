using NarrativeWorldCreator.MetricEngines;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.Metrics.TOTree;
using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
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

        public ClassSelectionPage(LocationNode selectedNode)
        {
            InitializeComponent();
            this.selectedNode = selectedNode;

            calculateMetricsAndRefreshLists();

            // Set content of header
            this.PageHeader.Text = "Entity selection - " + this.selectedNode.LocationName;
        }

        private void calculateMetricsAndRefreshLists()
        {
            // Retrieve required and dependent objects
            MetricEngine.SetWeightsToAll();
            var predicates = new List<Predicate>();
            foreach (var timepoint in this.selectedNode.TimePoints)
            {
                predicates = predicates.Concat(timepoint.PredicatesFilteredByCurrentLocation).ToList();
            }

            MetricEngine.SetupMetricEngineUsingTO(SystemStateTracker.NarrativeWorld.AvailableTangibleObjects.Where(ato => !ato.DefaultName.Equals(Constants.Floor)).ToList(), predicates);
            var requiredAndDependentTTOs = MetricEngine.TTOs.Where(tto => tto.Required || tto.RequiredDependency).ToList();
            // Create List for viewing
            var requiredAndDependentTOs = new List<TangibleObject>();
            foreach (var reqTO in requiredAndDependentTTOs)
            {
                requiredAndDependentTOs.Add(reqTO.TangibleObject);
            }
            var TOSVM = new TangibleObjectsSwapViewModel();
            TOSVM.Load(requiredAndDependentTOs);
            this.DataContext = TOSVM;
        }

        private void btnGraphPage_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GraphPage());
        }

        private void btnRegionPage_Click(object sender, RoutedEventArgs e)
        {
            var requiredTOs = (this.DataContext as TangibleObjectsSwapViewModel).TangibleObjects;
            var selectedTOs = (this.SelectedSwapView.DataContext as TangibleObjectsValuedViewModel).TangibleObjectsValued;
            var fillAvailableTO = new List<TangibleObject>();
            foreach (var to in requiredTOs)
            {
                fillAvailableTO.Add(to);
            }
            foreach( var to in selectedTOs)
            {
                fillAvailableTO.Add(to.TangibleObject);
            }
            this.selectedNode.AvailableTangibleObjects = SystemStateTracker.NarrativeWorld.AvailableTangibleObjects.Where(ato => !ato.DefaultName.Equals(Constants.Floor)).ToList();
            if (selectedNode.FloorCreated)
                this.NavigationService.Navigate(new MainModeRegionPage(selectedNode));
            else
                this.NavigationService.Navigate(new RegionCreationPage(selectedNode));
        }

        private void AvailableSwapView_Loaded(object sender, RoutedEventArgs e)
        {
            // Determine scores for each TO:
            var listOfValuedTangibleObjects = MetricEngine.GetOrderingTOUsingTO(this.selectedNode, SystemStateTracker.NarrativeWorld.AvailableTangibleObjects.Where(x => x.Children.Count == 0).ToList(), this.selectedNode.TimePoints[0].GetRemainingPredicates());

            // Remove required from available TO list
            // var toList = SystemStateTracker.NarrativeWorld.AvailableTangibleObjects.Where(ato => !ato.DefaultName.Equals(Constants.Floor)).ToList();
            foreach (var to in (this.DataContext as TangibleObjectsSwapViewModel).TangibleObjects)
            {
                listOfValuedTangibleObjects.Remove(listOfValuedTangibleObjects.Where(listVTO => listVTO.TangibleObject.Equals(to)).FirstOrDefault());
            }
            // Remove already selected from available TO list
            foreach (var to in (this.selectedNode.AvailableTangibleObjects))
            {
                listOfValuedTangibleObjects.Remove(listOfValuedTangibleObjects.Where(listVTO => listVTO.TangibleObject.Equals(to)).FirstOrDefault());
            }
            TangibleObjectsValuedViewModel toVM = new TangibleObjectsValuedViewModel();
            toVM.LoadList(listOfValuedTangibleObjects);
            this.AvailableSwapView.DataContext = toVM;
        }

        private void SelectedSwapView_Loaded(object sender, RoutedEventArgs e)
        {
            var listOfValuedTangibleObjects = MetricEngine.GetOrderingTOUsingTO(this.selectedNode, SystemStateTracker.NarrativeWorld.AvailableTangibleObjects.Where(x => x.Children.Count == 0).ToList(), this.selectedNode.TimePoints[0].GetRemainingPredicates());
            var listToRemove = new List<TOTreeTangibleObject>();
            // Determine to's to remove
            foreach (var tto in listOfValuedTangibleObjects)
            {
                if (this.selectedNode.AvailableTangibleObjects.Where(ato => ato.Equals(tto.TangibleObject)).FirstOrDefault() == null)
                {
                    listToRemove.Add(tto);
                }
            }

            foreach (var tto in listToRemove)
            {
                listOfValuedTangibleObjects.Remove(tto);
            }

            // Remove required from list of selected
            foreach (var to in (this.DataContext as TangibleObjectsSwapViewModel).TangibleObjects)
            {
                listOfValuedTangibleObjects.Remove(listOfValuedTangibleObjects.Where(listVTO => listVTO.TangibleObject.Equals(to)).FirstOrDefault());
            }

            TangibleObjectsValuedViewModel toVM = new TangibleObjectsValuedViewModel();
            toVM.LoadList(listOfValuedTangibleObjects);
            this.SelectedSwapView.DataContext = toVM;
        }

        private void btnMoveToSelected(object sender, RoutedEventArgs e)
        {
            // Add available to selected, remove to's from available
            var AvailableVM = (this.AvailableSwapView.DataContext as TangibleObjectsValuedViewModel);
            var SelectedVM = (this.SelectedSwapView.DataContext as TangibleObjectsValuedViewModel);
            var selectedItems = new TOTreeTangibleObject[this.AvailableSwapView.ToListView.SelectedItems.Count];
            this.AvailableSwapView.ToListView.SelectedItems.CopyTo(selectedItems, 0);
            for (int i = 0; i < selectedItems.Count(); i++)
            {
                // Remove from Available
                AvailableVM.TangibleObjectsValued.Remove(selectedItems[i]);

                // Add to selected
                SelectedVM.TangibleObjectsValued.Add(selectedItems[i]);
            }
        }

        private void btnMoveToAvailable(object sender, RoutedEventArgs e)
        {
            // Add selected to available, remove to's from selected
            var AvailableVM = (this.AvailableSwapView.DataContext as TangibleObjectsValuedViewModel);
            var SelectedVM = (this.SelectedSwapView.DataContext as TangibleObjectsValuedViewModel);
            var selectedItems = new TOTreeTangibleObject[this.SelectedSwapView.ToListView.SelectedItems.Count];
            this.SelectedSwapView.ToListView.SelectedItems.CopyTo(selectedItems, 0);
            for (int i = 0; i < selectedItems.Count(); i++)
            {
                // Remove from Available
                AvailableVM.TangibleObjectsValued.Add(selectedItems[i]);

                // Add to selected
                SelectedVM.TangibleObjectsValued.Remove(selectedItems[i]);
            }
        }
    }
}
