using Microsoft.Xna.Framework;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.Solvers;
using NarrativeWorldCreator.ViewModel;
using Semantics.Abstractions;
using Semantics.Components;
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
        public LocationNode selectedNode;

        public EntikaInstance SelectedEntikaObject;
        public NarrativeTimePoint SelectedTimePoint { get; internal set; }
        public EntikaInstance InstanceOfObjectToAdd;

        public enum RegionPageMode
        {
            RegionCreation = 0,
            RegionFilling = 1,
        }

        public RegionPageMode CurrentMode;

        public bool RegionCreated = false;

        public RegionPage(LocationNode selectedNode)
        {
            InitializeComponent();
            this.selectedNode = selectedNode;
            CurrentMode = RegionPageMode.RegionCreation;
            List<NarrativeTimePoint> ntpsFiltered = (from a in SystemStateTracker.NarrativeWorld.NarrativeTimeline.getNarrativeTimePointsWithNode(selectedNode) orderby a.TimePoint select a).ToList();
            SelectedTimePoint = ntpsFiltered[0];
        }

        internal void RemoveSelectedInstances(List<EntikaInstance> instances)
        {
            var relationsToRemove = new List<RelationshipInstance>();
            // Determine relationships to delete
            foreach (var instanceToRemove in instances)
            {
                foreach (var relation in this.SelectedTimePoint.InstancedRelations)
                {
                    if (relation.Source.Equals(instanceToRemove))
                    {
                        relationsToRemove.Add(relation);
                    }
                    else if (relation.Targets.Count > 0 && relation.Targets[0].Equals(instanceToRemove))
                    {
                        relationsToRemove.Add(relation);
                    }
                }
            }

            // Remove relationships
            foreach (var relationToRemove in relationsToRemove)
            {
                this.SelectedTimePoint.InstancedRelations.Remove(relationToRemove);
            }

            // Remove instances
            foreach (var instanceToRemove in instances)
            {
                this.SelectedTimePoint.InstancedObjects.Remove(instanceToRemove);
            }

            // Return to changing menu
            UpdateEntikaInstancesSelectionView();
            ChangeUIToChange();
        }

        internal void RepositionSelectedInstances(List<EntikaInstance> instances)
        {
            
        }

        public void SaveInstancingOfRelationsAndGotoPlacement(RelationInstancingViewModel rivm)
        {
            // Retrieve selected object for each relation

            // Add relation to EntikaInstance
            foreach (var relationVM in rivm.RelationshipInstances)
            {
                var relationInstance = relationVM.RelationshipInstance;
                foreach (var objectInstanceVM in relationVM.ObjectInstances)
                {
                    if (objectInstanceVM.Selected)
                    {
                        relationInstance.Source = objectInstanceVM.EntikaInstance;
                        this.SelectedTimePoint.InstancedRelations.Add(relationInstance);
                        break;
                    }
                }
                this.SelectedTimePoint.InstantiateRelationship(relationInstance);
            }
            this.SelectedTimePoint.InstancedObjects.Add(InstanceOfObjectToAdd);

            this.SelectedTimePoint.InstantiateAtPredicateForInstance(InstanceOfObjectToAdd);

            // Generate positions
            List<Vector3> positions = PlacementSolver.GenerateRandomPosition(this.SelectedTimePoint, InstanceOfObjectToAdd);

            ObjectPlacementViewModel opVM = new ObjectPlacementViewModel();
            opVM.Load(positions);
            ObjectPlacementView.DataContext = opVM;

            TangibleObjectsView.Visibility = Visibility.Collapsed;
            RelationshipSelectionView.Visibility = Visibility.Collapsed;
            RelationInstancingView.Visibility = Visibility.Collapsed;
            ObjectPlacementView.Visibility = Visibility.Visible;
        }

        public void AddSelectedTangibleObject(TangibleObject selectedItem)
        {
            InstanceOfObjectToAdd = new EntikaInstance(selectedItem);
            RelationshipSelectionViewModel riVM = new RelationshipSelectionViewModel();
            riVM.Load(selectedItem);
            RelationshipSelectionView.DataContext = riVM;
            TangibleObjectsView.Visibility = Visibility.Collapsed;
            RelationshipSelectionView.Visibility = Visibility.Visible;
            RelationInstancingView.Visibility = Visibility.Collapsed;
            ObjectPlacementView.Visibility = Visibility.Collapsed;
        }

        public void BackToTangibleObjectSelection()
        {
            TangibleObjectsView.Visibility = Visibility.Visible;
            RelationshipSelectionView.Visibility = Visibility.Collapsed;
            RelationInstancingView.Visibility = Visibility.Collapsed;
            ObjectPlacementView.Visibility = Visibility.Collapsed;
            InstanceOfObjectToAdd = null;
            RelationshipSelectionViewModel riVM = new RelationshipSelectionViewModel();
            RelationshipSelectionView.DataContext = riVM;
        }

        public void BackToRelationshipSelection()
        {
            TangibleObjectsView.Visibility = Visibility.Collapsed;
            RelationshipSelectionView.Visibility = Visibility.Visible;
            RelationInstancingView.Visibility = Visibility.Collapsed;
            ObjectPlacementView.Visibility = Visibility.Collapsed;
        }

        public void UpdateObjectPosition(Vector3 vector)
        {
            this.InstanceOfObjectToAdd.Position = vector;
        }

        public void PlaceObjectAndToEntityAddition(Vector3 vector)
        {
            this.InstanceOfObjectToAdd.Position = vector;

            TangibleObjectsView.Visibility = Visibility.Visible;
            RelationshipSelectionView.Visibility = Visibility.Collapsed;
            RelationInstancingView.Visibility = Visibility.Collapsed;
            ObjectPlacementView.Visibility = Visibility.Collapsed;

            ChangeUIToMainMenu();
        }

        public void InstanceSelectedRelationships(List<Relationship> relationships)
        {
            List<RelationshipInstance> relationInstances = new List<RelationshipInstance>();
            // Instanciate relationships
            foreach(var relation in relationships)
            {
                var instance = new RelationshipInstance();
                instance.BaseRelationship = relation;
                if (RelationshipTypes.IsRelationshipValued(relation.RelationshipType.DefaultName))
                {
                    instance.Valued = true;
                    instance.TargetRangeStart = Double.Parse(relation.Attributes[0].Value.ToString());
                    instance.TargetRangeEnd = Double.Parse(relation.Attributes[1].Value.ToString());
                }
                
                if (relation.Source.Equals(InstanceOfObjectToAdd.TangibleObject))
                {
                    instance.Source = InstanceOfObjectToAdd;
                }
                else
                {
                    instance.Targets.Add(InstanceOfObjectToAdd);
                }
                relationInstances.Add(instance);
            }

            RelationInstancingViewModel rivm = new RelationInstancingViewModel();
            rivm.Load(relationInstances, this.SelectedTimePoint.InstancedObjects);
            RelationInstancingView.DataContext = rivm;

            UpdateFillDetailView();

            TangibleObjectsView.Visibility = Visibility.Collapsed;
            RelationshipSelectionView.Visibility = Visibility.Collapsed;
            RelationInstancingView.Visibility = Visibility.Visible;
        }

        public TangibleObject RetrieveSelectedTangibleObjectFromListView()
        {
            return (TangibleObject) TangibleObjectsView.ToListView.SelectedItem;
        }

        private void TangibleObjectsView_Loaded(object sender, RoutedEventArgs e)
        {
            TangibleObjectsValuedViewModel toVM = new TangibleObjectsValuedViewModel();
            toVM.Load(this.SelectedTimePoint);
            TangibleObjectsView.DataContext = toVM;
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

            var ntpVM = (NarrativeTimelineControl.DataContext as NarrativeTimelineViewModel).NarrativeTimePoints.Where(ntp => ntp.NarrativeTimePoint.Equals(SelectedTimePoint)).FirstOrDefault();
            ntpVM.Selected = true;
        }

        private void RegionDetailTimePointView_Loaded(object sender, RoutedEventArgs e)
        {
            // Fill control with stuff
            DetailTimePointViewModel timePointViewModelObject =
               new DetailTimePointViewModel();
            timePointViewModelObject.LoadObjects(selectedNode, SelectedTimePoint);
            RegionDetailTimePointView.DataContext = timePointViewModelObject;
        }

        private void SelectedObjectDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            SelectedObjectDetailViewModel selectedObjectViewModelObject =
               new SelectedObjectDetailViewModel();
            SelectedObjectDetailView.DataContext = selectedObjectViewModelObject;
        }

        private void EntikaInstancesSelectionView_Loaded(object sender, RoutedEventArgs e)
        {
            EntikaInstancesSelectionViewModel eisVM = new EntikaInstancesSelectionViewModel();
            eisVM.Load(this.SelectedTimePoint);

            EntikaInstancesSelectionView.DataContext = eisVM;
        }

        public void UpdateEntikaInstancesSelectionView()
        {
            (EntikaInstancesSelectionView.DataContext as EntikaInstancesSelectionViewModel).Load(this.SelectedTimePoint);
        }

        public void ChangeSelectedObject(EntikaInstance ieo)
        {
            this.SelectedEntikaObject = ieo;
            (SelectedObjectDetailView.DataContext as SelectedObjectDetailViewModel).ChangeSelectedObject(ieo);
            SelectedObjectDetailView.ShowGrid();
        }

        public void DeselectObject()
        {
            this.SelectedEntikaObject = null;
            SelectedObjectDetailView.HideGrid();
        }

        private void FillDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            FillDetailViewModel fillDetailVM = new FillDetailViewModel();
            fillDetailVM.Load(this.SelectedTimePoint);

            FillDetailView.DataContext = fillDetailVM;
        }

        public void UpdateFillDetailView()
        {
            (FillDetailView.DataContext as FillDetailViewModel).Load(this.SelectedTimePoint);
        }

        public void UpdateDetailView(NarrativeTimePoint narrativeTimePoint)
        {
            // Update detailtab
            (RegionDetailTimePointView.DataContext as DetailTimePointViewModel).LoadObjects(selectedNode, narrativeTimePoint);
            UpdateFillDetailView();
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
            region_outlining_0.Visibility = Visibility.Collapsed;
            region_outlining_0_content.Visibility = Visibility.Collapsed;
            region_filling_1.Visibility = Visibility.Visible;
            region_filling_1_content.Visibility = Visibility.Visible;
            region_filling_2.Visibility = Visibility.Collapsed;
            region_filling_2_content.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            region_filling_20.Visibility = Visibility.Visible;
            region_filling_21.Visibility = Visibility.Visible;
            region_tabcontrol.SelectedIndex = 1;
        }

        public void SwitchModeToRegionCreation()
        {
            changeModes(RegionPageMode.RegionCreation);
            region_outlining_0.Visibility = Visibility.Visible;
            region_outlining_0_content.Visibility = Visibility.Visible;
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            region_filling_2.Visibility = Visibility.Collapsed;
            region_filling_2_content.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            region_filling_20.Visibility = Visibility.Collapsed;
            region_filling_21.Visibility = Visibility.Collapsed;
            region_tabcontrol.SelectedIndex = 0;
        }

        private void btnResetRegion(object sender, RoutedEventArgs e)
        {
            selectedNode.Shape = new Common.Geometry.Shape(new List<Common.Vec2>());            
        }

        public void SetMessageBoxText(string message)
        {
            MessageTextBox.Text = message;
        }

        private void btnAddToCurrentRegion(object sender, RoutedEventArgs e)
        {
            ChangeUIToAddition();
        }

        internal void ChangeUIToAddition()
        {
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            region_filling_2.Visibility = Visibility.Visible;
            region_filling_2_content.Visibility = Visibility.Visible;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            region_tabcontrol.SelectedIndex = 2;
        }

        private void btnChangeCurrentRegion(object sender, RoutedEventArgs e)
        {
            UpdateEntikaInstancesSelectionView();
            ChangeUIToChange();
        }

        internal void ChangeUIToChange()
        {
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            region_filling_2.Visibility = Visibility.Collapsed;
            region_filling_2_content.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Visible;
            region_filling_3_content.Visibility = Visibility.Visible;
            region_tabcontrol.SelectedIndex = 3;
        }

        internal void ChangeUIToMainMenu()
        {
            region_filling_1.Visibility = Visibility.Visible;
            region_filling_1_content.Visibility = Visibility.Visible;
            region_filling_2.Visibility = Visibility.Collapsed;
            region_filling_2_content.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            region_tabcontrol.SelectedIndex = 1;
        }

    }
}
