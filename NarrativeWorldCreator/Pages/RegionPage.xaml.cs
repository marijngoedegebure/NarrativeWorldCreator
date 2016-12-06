using NarrativeWorlds;
using Semantics.Abstractions;
using Semantics.Data;
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

namespace NarrativeWorldCreator
{
    /// <summary>
    /// Interaction logic for RegionPage.xaml
    /// </summary>
    public partial class RegionPage : Page
    {
        public Node selectedNode;

        public enum RegionPageMode
        {
            RegionCreation = 1,
            RegionFilling = 2,
        }

        public RegionPageMode CurrentMode;

        public RegionPage(Node selectedNode)
        {
            InitializeComponent();
            this.selectedNode = selectedNode;
            CurrentMode = RegionPageMode.RegionCreation;
            //fillDetailView();
            fillItemList();
            fillGroupCombobox();
        }

        private void NarrativeTimelineControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Fill control with stuff
            NarrativeTimelineViewModel narrativeTimelineViewModelObject =
               new NarrativeTimelineViewModel();
            narrativeTimelineViewModelObject.LoadFilteredTimePoints(selectedNode);

            NarrativeTimelineControl.DataContext = narrativeTimelineViewModelObject;
        }

        private void RegionDetailTimePointView_Loaded(object sender, RoutedEventArgs e)
        {
            // Fill control with stuff
            RegionDetailTimePointViewModel timePointViewModelObject =
               new RegionDetailTimePointViewModel();
            timePointViewModelObject.LoadCharactersAndThings(selectedNode, (NarrativeTimelineControl.DataContext as NarrativeTimelineViewModel).NarrativeTimePoints[0].NarrativeTimePoint);
            RegionDetailTimePointView.DataContext = timePointViewModelObject;
        }

        private void fillItemList()
        {
            List<TangibleObject> allTangibleObjects = DatabaseSearch.GetNodes<TangibleObject>(true);
            List<TangibleObject> filteredList = allTangibleObjects.Where(x => x.Children.Count == 0).ToList();
            lvObjectsDataBinding.ItemsSource = filteredList;
        }

        internal void fillDetailView(NarrativeTimePoint narrativeTimePoint)
        {
            // Update detailtab
            (RegionDetailTimePointView.DataContext as RegionDetailTimePointViewModel).LoadCharactersAndThings(selectedNode, narrativeTimePoint);
        }

        private void fillGroupCombobox()
        {
            List<Group> allGroups = DatabaseSearch.GetNodes<Group>(true);
            lvGroupsDataBinding.ItemsSource = allGroups;
        }

        private void btnReturnToGraph_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GraphPage());
        }

        //internal void fillDetailView()
        //{
        //    narrative_location_name.Content = selectedNode.LocationName;
        //    number_narrative_events.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeEventsOfLocation(selectedNode.LocationName).Distinct().Count();
        //    number_narrative_characters.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfTypeOfLocation(SystemStateTracker.CharacterTypeName, selectedNode.LocationName).Distinct().Count();
        //    number_narrative_objects.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfTypeOfLocation(SystemStateTracker.ObjectTypeName, selectedNode.LocationName).Distinct().Count();
        //}

        private void btnSwitchModeToRegionFilling(object sender, RoutedEventArgs e)
        {
            CurrentMode = RegionPageMode.RegionFilling;
            region_creation_1.Visibility = Visibility.Collapsed;
            region_creation_3.Visibility = Visibility.Collapsed;
            region_creation_4.Visibility = Visibility.Collapsed;
            region_filling_1.Visibility = Visibility.Visible;
            region_filling_2.Visibility = Visibility.Visible;
            region_filling_3.Visibility = Visibility.Visible;
            region_filling_4.Visibility = Visibility.Visible;
        }

        private void btnSwitchModeToRegionCreation(object sender, RoutedEventArgs e)
        {
            CurrentMode = RegionPageMode.RegionCreation;
            region_creation_1.Visibility = Visibility.Visible;
            region_creation_3.Visibility = Visibility.Visible;
            region_creation_4.Visibility = Visibility.Visible;
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_2.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_4.Visibility = Visibility.Collapsed;
        }
    }
}
