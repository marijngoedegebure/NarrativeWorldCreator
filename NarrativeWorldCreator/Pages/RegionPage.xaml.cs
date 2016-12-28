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
        public EntikaClassInstance SelectedInstancedEntikaObject;

        public enum RegionPageMode
        {
            RegionCreation = 0,
            RegionFilling = 1,
        }

        public RegionPageMode CurrentMode;

        public bool RegionCreated = false;

        public RegionPage(Node selectedNode)
        {
            InitializeComponent();
            this.selectedNode = selectedNode;
            CurrentMode = RegionPageMode.RegionCreation;
            //fillDetailView();
            fillItemList();
            fillGroupCombobox();
        }

        private void ModeControl_Loaded(object sender, RoutedEventArgs e)
        {
            ModeViewModel modeVM = new ModeViewModel();
            modeVM.ChangeModes(CurrentMode);
            ModeControl.DataContext = modeVM;
        }

        private void RegionHeader_Loaded(object sender, RoutedEventArgs e)
        {
            NodeViewModel nodeVM = new NodeViewModel();
            nodeVM.Load(selectedNode);
            RegionHeaderControl.DataContext = nodeVM;
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

        private void SelectedObjectDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            SelectedObjectDetailViewModel selectedObjectViewModelObject =
               new SelectedObjectDetailViewModel();
            SelectedObjectDetailView.DataContext = selectedObjectViewModelObject;
        }

        public void ChangeSelectedObject(EntikaClassInstance ieo)
        {
            this.SelectedInstancedEntikaObject = ieo;
            (SelectedObjectDetailView.DataContext as SelectedObjectDetailViewModel).ChangeSelectedObject(ieo);
            SelectedObjectDetailView.ShowGrid();
        }

        public void DeselectObject()
        {
            this.SelectedInstancedEntikaObject = null;
            SelectedObjectDetailView.HideGrid();
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

        private void changeModes(RegionPageMode newMode)
        {
            CurrentMode = newMode;
            (ModeControl.DataContext as ModeViewModel).ChangeModes(newMode);
        }

        public void SwitchModeToRegionFilling()
        {
            changeModes(RegionPageMode.RegionFilling);
            region_creation_1.Visibility = Visibility.Collapsed;
            region_creation_4.Visibility = Visibility.Collapsed;
            region_filling_1.Visibility = Visibility.Visible;
            region_filling_2.Visibility = Visibility.Visible;
            region_filling_4.Visibility = Visibility.Visible;
        }

        public void SwitchModeToRegionCreation()
        {
            changeModes(RegionPageMode.RegionCreation);
            region_creation_1.Visibility = Visibility.Visible;
            region_creation_4.Visibility = Visibility.Visible;
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_2.Visibility = Visibility.Collapsed;
            region_filling_4.Visibility = Visibility.Collapsed;
        }

        private void btnResetRegion(object sender, RoutedEventArgs e)
        {
            selectedNode.Shape = new Common.Geometry.Shape(new List<Common.Vec2>());            
        }
    }
}
