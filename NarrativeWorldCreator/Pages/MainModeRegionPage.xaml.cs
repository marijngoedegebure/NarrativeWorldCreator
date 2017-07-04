using Microsoft.Xna.Framework;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.Metrics;
using NarrativeWorldCreator.Models.Metrics.TOTree;
using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.Pages;
using NarrativeWorldCreator.Solvers;
using NarrativeWorldCreator.ViewModel;
using NarrativeWorldCreator.Views;
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
    public partial class MainModeRegionPage : BaseRegionPage
    {
        internal void GenerateConfigurations()
        {
            GeneratedConfigurations = CudaGPUWrapper.CudaGPUWrapperCall(this.SelectedTimePoint, this.WorkInProgressConfiguration);
            // Update list of configurations using generated list of regionpage
            var gcVM = (GenerateConfigurationsViewModel)InspectGeneratedConfigurationsView.DataContext;
            gcVM.Load(GeneratedConfigurations);
            this.InspectGeneratedConfigurationsView.DataContext = gcVM;
        }

        internal void RefreshSelectedObjectView()
        {
            var removalinstances = this.SelectedEntikaInstances.Where(sei => !this.SelectedTimePoint.Configuration.InstancedObjects.Contains(sei)).ToList();
            foreach ( var removal in removalinstances)
            {
                this.SelectedEntikaInstances.Remove(removal);
            }
            (this.SelectedObjectDetailView.DataContext as SelectedObjectDetailViewModel).LoadSelectedInstances(this.SelectedEntikaInstances, this.SelectedTimePoint);
        }

        internal void SuggestNewPositions()
        {
            this.WorkInProgressConfiguration = new Configuration();
            this.WorkInProgressConfiguration = this.SelectedTimePoint.Configuration.Copy();
            IntializeGenerateConfigurationsView(this.GenerateConfigurationsView2);
            CurrentFillingMode = FillingMode.Repositioning;
            EntikaInstancesSelectionView.Visibility = Visibility.Collapsed;
            GenerateConfigurationsView2.Visibility = Visibility.Visible;
        }

        internal void RepositionAndBackToMenu()
        {
            ApplyConfiguration();
            CurrentFillingMode = FillingMode.None;
            EntikaInstancesSelectionView.Visibility = Visibility.Visible;
            GenerateConfigurationsView2.Visibility = Visibility.Collapsed;
        }

        internal void BackToMenu()
        {
            EntikaInstancesSelectionView.Visibility = Visibility.Visible;
            GenerateConfigurationsView2.Visibility = Visibility.Collapsed;
            CurrentFillingMode = FillingMode.None;
            WorkInProgressConfiguration = new Configuration();
            LeftSelectedGPUConfigurationResult = -1;
            RightSelectedGPUConfigurationResult = -1;
        }

        public bool RegionCreated = false;

        public MainModeRegionPage(LocationNode selectedNode, NarrativeTimePoint SelectedTimePont)
        {
            InitializeComponent();
            this.selectedNode = selectedNode;
            this.SelectedTimePoint = SelectedTimePont;
            SelectedEntikaInstances = new List<EntikaInstance>();
        }

        internal void RemoveSelectedInstances(List<EntikaInstanceValuedPredicate> instances)
        {
            var relationsToRemove = new List<RelationshipInstance>();
            // Determine relationships to delete
            foreach (var instanceToRemove in instances)
            {
                foreach (var relation in this.SelectedTimePoint.Configuration.InstancedRelations)
                {
                    if (relation.Source.Equals(instanceToRemove.EntikaInstanceValued.EntikaInstance))
                    {
                        relationsToRemove.Add(relation);
                    }
                    else if (relation.Target.Equals(instanceToRemove.EntikaInstanceValued.EntikaInstance))
                    {
                        relationsToRemove.Add(relation);
                    }
                }
            }

            // Remove relationships
            foreach (var relationToRemove in relationsToRemove)
            {
                this.SelectedTimePoint.Configuration.InstancedRelations.Remove(relationToRemove);
            }

            // Remove instances
            foreach (var instanceToRemove in instances)
            {
                this.SelectedTimePoint.Configuration.InstancedObjects.Remove(instanceToRemove.EntikaInstanceValued.EntikaInstance);
            }

            // Return to changing menu
            this.RefreshSelectedObjectView();
            this.SelectedTimePoint.RegeneratePredicates();
            UpdateEntikaInstancesSelectionView();
        }

        public void SaveInstancingOfRelationsAndGotoPlacement(RelationshipSelectionAndInstancingViewModel rivm)
        {
            // this.relationShipSelectionAndInstancing = rivm;

            // Save instance and relationships to WIPconfiguration
            var onRelationshipMultipleVM = rivm.OnRelationshipsMultiple.Where(or => or.Selected).FirstOrDefault();
            var onRelationshipSingleVM = rivm.OnRelationshipsSingle.Where(or => or.Selected).FirstOrDefault();
            RelationshipExtendedViewModel onRelationshipVM;
            if (onRelationshipMultipleVM != null)
            {
                onRelationshipVM = onRelationshipMultipleVM;
            }
            else if (onRelationshipSingleVM != null)
            {
                onRelationshipVM = onRelationshipSingleVM;
            }
            else
            {
                throw new Exception("No on relationship selected");
            }

            var onRelationshipInstance = new RelationshipInstance();
            onRelationshipInstance.BaseRelationship = onRelationshipVM.Relationship;
            onRelationshipInstance.Target = onRelationshipVM.Target;
            onRelationshipInstance.Source = onRelationshipVM.ObjectInstances.Where(oi => oi.Selected).FirstOrDefault().EntikaInstance;

            InstanceOfObjectToAdd.RelationshipsAsTarget.Add(onRelationshipInstance);
            onRelationshipInstance.Source.RelationshipsAsSource.Add(onRelationshipInstance);
            InstanceOfObjectToAdd.Position = new Vector3(onRelationshipInstance.Source.Position.X, onRelationshipInstance.Source.Position.Y, onRelationshipInstance.Source.BoundingBox.Max.Z);

            this.WorkInProgressConfiguration.InstancedRelations.Add(onRelationshipInstance);

            // Parse other relationships
            // Single
            foreach (var otherRelationVM in rivm.OtherRelationshipsSingle.Where(or => or.Selected).ToList())
            {
                var otherRelationshipInstance = new RelationshipInstance();
                otherRelationshipInstance.BaseRelationship = otherRelationVM.Relationship;
                bool source = false;
                if (otherRelationVM.Source == null)
                {
                    otherRelationshipInstance.Source = otherRelationVM.ObjectInstances.Where(oi => oi.Selected).FirstOrDefault().EntikaInstance;
                    otherRelationshipInstance.Target = otherRelationVM.Target;
                }
                else
                {
                    otherRelationshipInstance.Source = otherRelationVM.Source;
                    otherRelationshipInstance.Target = otherRelationVM.ObjectInstances.Where(oi => oi.Selected).FirstOrDefault().EntikaInstance;
                    source = true;
                }

                if (Constants.IsRelationshipValued(otherRelationshipInstance.BaseRelationship.RelationshipType.DefaultName))
                {
                    otherRelationshipInstance.Valued = true;
                    otherRelationshipInstance.TargetRangeStart = Double.Parse(otherRelationshipInstance.BaseRelationship.Attributes[0].Value.ToString());
                    otherRelationshipInstance.TargetRangeEnd = Double.Parse(otherRelationshipInstance.BaseRelationship.Attributes[1].Value.ToString());
                }

                if (source)
                {
                    InstanceOfObjectToAdd.RelationshipsAsSource.Add(otherRelationshipInstance);
                    otherRelationshipInstance.Target.RelationshipsAsTarget.Add(otherRelationshipInstance);
                }
                else
                {
                    InstanceOfObjectToAdd.RelationshipsAsTarget.Add(otherRelationshipInstance);
                    otherRelationshipInstance.Source.RelationshipsAsSource.Add(otherRelationshipInstance);
                }

                this.WorkInProgressConfiguration.InstancedRelations.Add(otherRelationshipInstance);
            }

            // Multiple
            foreach (var otherRelationVM in rivm.OtherRelationshipsMultiple.Where(or => or.Selected).ToList())
            {
                var otherRelationshipInstance = new RelationshipInstance();
                otherRelationshipInstance.BaseRelationship = otherRelationVM.Relationship;
                bool source = false;
                if (otherRelationVM.Source == null)
                {
                    otherRelationshipInstance.Source = otherRelationVM.ObjectInstances.Where(oi => oi.Selected).FirstOrDefault().EntikaInstance;
                    otherRelationshipInstance.Target = otherRelationVM.Target;
                }
                else
                {
                    otherRelationshipInstance.Source = otherRelationVM.Source;
                    otherRelationshipInstance.Target = otherRelationVM.ObjectInstances.Where(oi => oi.Selected).FirstOrDefault().EntikaInstance;
                    source = true;
                }

                if (Constants.IsRelationshipValued(otherRelationshipInstance.BaseRelationship.RelationshipType.DefaultName))
                {
                    otherRelationshipInstance.Valued = true;
                    otherRelationshipInstance.TargetRangeStart = Double.Parse(otherRelationshipInstance.BaseRelationship.Attributes[0].Value.ToString());
                    otherRelationshipInstance.TargetRangeEnd = Double.Parse(otherRelationshipInstance.BaseRelationship.Attributes[1].Value.ToString());
                }

                if (source)
                {
                    InstanceOfObjectToAdd.RelationshipsAsSource.Add(otherRelationshipInstance);
                    otherRelationshipInstance.Target.RelationshipsAsTarget.Add(otherRelationshipInstance);
                }
                else
                {
                    InstanceOfObjectToAdd.RelationshipsAsTarget.Add(otherRelationshipInstance);
                    otherRelationshipInstance.Source.RelationshipsAsSource.Add(otherRelationshipInstance);
                }

                this.WorkInProgressConfiguration.InstancedRelations.Add(otherRelationshipInstance);
                
            }

            this.WorkInProgressConfiguration.InstancedObjects.Add(InstanceOfObjectToAdd);

            IntializeGenerateConfigurationsView(this.GenerateConfigurationsView);

            CurrentFillingMode = FillingMode.Placement;
            TangibleObjectsView.Visibility = Visibility.Collapsed;
            RelationshipSelectionAndInstancingView.Visibility = Visibility.Collapsed;
            GenerateConfigurationsView.Visibility = Visibility.Visible;
            generation_1.Visibility = Visibility.Visible;
            BasicWindow.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            BasicScene.Visibility = Visibility.Collapsed;
            GenerationSceneLeft.Visibility = Visibility.Visible;
            GenerationSceneRight.Visibility = Visibility.Visible;
        }

        public void IntializeGenerateConfigurationsView(GenerateConfigurationsView view)
        {
            GenerateConfigurationsViewModel gcVM = new GenerateConfigurationsViewModel();
            gcVM.Load(new List<GPUConfigurationResult>());
            InspectGeneratedConfigurationsView.DataContext = gcVM;
            view.SliderWeightFocalPoint.Value = SystemStateTracker.WeightFocalPoint;
            view.SliderWeightPairWise.Value = SystemStateTracker.WeightPairWise;
            view.SliderWeightSymmetry.Value = SystemStateTracker.WeightSymmetry;
            view.SliderWeightVisualBalance.Value = SystemStateTracker.WeightVisualBalance;
            view.centroidX.Text           = SystemStateTracker.centroidX.ToString();
            view.centroidY.Text           = SystemStateTracker.centroidY.ToString();
            view.focalX.Text              = SystemStateTracker.focalX.ToString();
            view.focalY.Text              = SystemStateTracker.focalY.ToString();
            view.focalRot.Text            = SystemStateTracker.focalRot.ToString();
            view.gridxDim.Text            = SystemStateTracker.gridxDim.ToString();
            view.RefreshConfigurations();
        }

        public void BackToRelationshipSelectionAndInstancing()
        {
            this.WorkInProgressConfiguration = this.SelectedTimePoint.Configuration.Copy();
            LeftSelectedGPUConfigurationResult = -1;
            RightSelectedGPUConfigurationResult = -1;

            CurrentFillingMode = FillingMode.RelationSelectionAndInstancting;
            TangibleObjectsView.Visibility = Visibility.Collapsed;
            RelationshipSelectionAndInstancingView.Visibility = Visibility.Visible;
            GenerateConfigurationsView.Visibility = Visibility.Collapsed;
            BasicScene.Visibility = Visibility.Visible;
            GenerationSceneLeft.Visibility = Visibility.Collapsed;
            GenerationSceneRight.Visibility = Visibility.Collapsed;
        }

        public void AddSelectedTangibleObject(TangibleObject selectedItem)
        {
            // Copy current configuration of timepoint so it can be changed.
            this.WorkInProgressConfiguration = this.SelectedTimePoint.Configuration.Copy();

            // Determine instance to add:
            InstanceOfObjectToAdd = new EntikaInstance(selectedItem);
            RelationshipSelectionAndInstancingViewModel riVM = new RelationshipSelectionAndInstancingViewModel();
            riVM.Load(this.SelectedTimePoint, InstanceOfObjectToAdd, this.WorkInProgressConfiguration.InstancedObjects, this.SelectedTimePoint.GetRemainingPredicates());
            if (riVM.OnRelationshipsMultiple.Count == 0 && riVM.OnRelationshipsSingle.Count == 0 && riVM.OnRelationshipsNone.Count == 0)
                throw new Exception("No on relationship at all");
            RelationshipSelectionAndInstancingView.DataContext = riVM;
            if (riVM.OnRelationshipsMultiple.Count > 0)
                RelationshipSelectionAndInstancingView.OnRelationshipsMultipleListView.SelectedIndex = 0;
            if (riVM.OnRelationshipsSingle.Count > 0)
                RelationshipSelectionAndInstancingView.OnRelationshipsSingleListView.SelectedIndex = 0;

            // Switch UI to next step (relation selection and instancing)
            CurrentFillingMode = FillingMode.RelationSelectionAndInstancting;
            TangibleObjectsView.Visibility = Visibility.Collapsed;
            RelationshipSelectionAndInstancingView.Visibility = Visibility.Visible;
            GenerateConfigurationsView.Visibility = Visibility.Collapsed;
        }

        public void BackToTangibleObjectSelection()
        {
            CurrentFillingMode = FillingMode.ClassSelection;
            TangibleObjectsView.Visibility = Visibility.Visible;
            RelationshipSelectionAndInstancingView.Visibility = Visibility.Collapsed;
            GenerateConfigurationsView.Visibility = Visibility.Collapsed;
            InstanceOfObjectToAdd = null;
            RelationshipSelectionAndInstancingViewModel riVM = new RelationshipSelectionAndInstancingViewModel();
            RelationshipSelectionAndInstancingView.DataContext = riVM;
        }

        public void PlaceObjectAndToEntityAddition()
        {
            // Apply chosen results to timepoint
            ApplyConfiguration();
            InstanceOfObjectToAdd = null;

            // Regenerate predicates based on newly applied configuration
            this.SelectedTimePoint.RegeneratePredicates();


            CurrentFillingMode = FillingMode.ClassSelection;
            TangibleObjectsView.Visibility = Visibility.Visible;
            RelationshipSelectionAndInstancingView.Visibility = Visibility.Collapsed;
            GenerateConfigurationsView.Visibility = Visibility.Collapsed;
            generation_1.Visibility = Visibility.Collapsed;
            BasicWindow.ColumnDefinitions[1].Width = new GridLength(0);
            BasicScene.Visibility = Visibility.Visible;
            GenerationSceneLeft.Visibility = Visibility.Collapsed;
            GenerationSceneRight.Visibility = Visibility.Collapsed;

            ChangeUIToMainMenu();
        }

        private void ApplyConfiguration()
        {
            var gpuConfig = this.GeneratedConfigurations[this.RightSelectedGPUConfigurationResult];
            for (int i = 0; i < gpuConfig.Instances.Count; i++)
            {
                var WIPinstance = this.WorkInProgressConfiguration.InstancedObjects.Where(io => io.Equals(gpuConfig.Instances[i].entikaInstance)).FirstOrDefault();
                WIPinstance.Position = new Vector3(gpuConfig.Instances[i].Position.X, gpuConfig.Instances[i].Position.Y, gpuConfig.Instances[i].Position.Z);
                WIPinstance.Rotation = new Vector3(gpuConfig.Instances[i].Rotation.X, gpuConfig.Instances[i].Rotation.Y, gpuConfig.Instances[i].Rotation.Z);
                WIPinstance.UpdateBoundingBoxAndShape();
            }
            this.SelectedTimePoint.Configuration = WorkInProgressConfiguration;

            // Reset used variables
            WorkInProgressConfiguration = new Configuration();
            LeftSelectedGPUConfigurationResult = -1;
            RightSelectedGPUConfigurationResult = -1;
            GeneratedConfigurations = new List<GPUConfigurationResult>();
    }

        private void TangibleObjectsView_Loaded(object sender, RoutedEventArgs e)
        {
            TangibleObjectsValuedViewModel toVM = new TangibleObjectsValuedViewModel();
            toVM.LoadAll(this.SelectedTimePoint);
            TangibleObjectsView.DataContext = toVM;
            TangibleObjectsView.DefaultRB.IsChecked = true;
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
            selectedObjectViewModelObject.LoadSelectedInstances(this.SelectedEntikaInstances, this.SelectedTimePoint);
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
            if (!this.SelectedEntikaInstances.Contains(ieo))
            {
                this.SelectedEntikaInstances.Add(ieo);
            }
            else
                this.SelectedEntikaInstances.Remove(ieo);
            (SelectedObjectDetailView.DataContext as SelectedObjectDetailViewModel).LoadSelectedInstances(this.SelectedEntikaInstances, this.SelectedTimePoint);
        }

        public void UpdateDetailView(NarrativeTimePoint narrativeTimePoint)
        {
            // Update detailtab
            this.RefreshSelectedObjectView();
            (RegionDetailTimePointView.DataContext as DetailTimePointViewModel).LoadObjects(selectedNode, narrativeTimePoint);
        }

        private void btnReturnToInit_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ClassSelectionPage(selectedNode, this.SelectedTimePoint));
        }


        private void btnGraphPage_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GraphPage());
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
            region_tabcontrol.SelectedIndex = 1;
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
            region_tabcontrol.SelectedIndex = 2;
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

        private void btnReturnToRegionCreation_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GetNavigationService(this);
            this.NavigationService.Navigate(new RegionCreationPage(selectedNode, this.SelectedTimePoint));
        }

        private void btnGotoAlternativeFillingMode_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AlternativeModeRegionPage(selectedNode, this.SelectedTimePoint));
        }
    }
}
