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
    public partial class MainModeRegionPage : ModeBaseRegionPage
    {
        // List of states
        #region Fields
        internal MainFillingMode CurrentFillingMode = MainFillingMode.None;

        internal enum MainFillingMode
        {
            None = 0,
            ClassSelection = 1,
            RelationshipSelection = 2,
            RelationshipInstancing = 3,
            Placement = 4,
            Repositioning = 5
        }

        EntikaInstance InstanceOfObjectToAdd;

        #endregion
        #region Setup

        public MainModeRegionPage(LocationNode selectedNode, NarrativeTimePoint SelectedTimePont)
        {
            InitializeComponent();
            this.selectedNode = selectedNode;
            this.SelectedTimePoint = SelectedTimePont;
            SelectedEntikaInstances = new List<EntikaInstance>();
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

        private void TangibleObjectsView_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshTangibleObjectsView();
        }

        private void TangibleObjectsSystemView_Loaded(object sender, RoutedEventArgs e)
        {
            if (TangibleObjectsSystemView.DataContext == null)
            {
                TangibleObjectsValuedViewModel toVM = new TangibleObjectsValuedViewModel();
                TangibleObjectsSystemView.DataContext = toVM;
            }
        }
        #endregion


        #region UIChanges
        internal override void Back()
        {
            // Check current mode
            switch (CurrentFillingMode)
            {
                case MainFillingMode.ClassSelection:
                    ChangeUIToMainMenu();
                    break;
                case MainFillingMode.RelationshipSelection:
                    ChangeUIToTOClassSelection();
                    break;
                case MainFillingMode.RelationshipInstancing:
                    ChangeUIToRelationshipSelection();
                    break;
                case MainFillingMode.Placement:
                    ChangeUIToRelationshipInstancing();
                    break;
                case MainFillingMode.Repositioning:
                    ChangeUIToMainMenu();
                    break;
                default:
                    break;
            }
            // Call function to switch to different mode
        }

        internal override void Next()
        {
            // Check current mode
            // Call function to switch to different mode
            switch (CurrentFillingMode)
            {
                case MainFillingMode.ClassSelection:
                    AddSelectedTangibleObject();
                    ChangeUIToRelationshipSelection();
                    break;
                case MainFillingMode.RelationshipSelection:
                    SetupInstancing();
                    ChangeUIToRelationshipInstancing();
                    break;
                case MainFillingMode.RelationshipInstancing:
                    SaveInstancingAndSetupPlacement();
                    ChangeUIToPlacement();
                    break;
                case MainFillingMode.Placement:
                    ApplyConfiguration();
                    ChangeUIToMainMenu();
                    break;
                case MainFillingMode.Repositioning:
                    ApplyConfiguration();
                    ChangeUIToMainMenu();
                    break;
                default:
                    break;
            }
        }

        internal void ChangeUIToTOClassSelection()
        {
            this.CurrentFillingMode = MainFillingMode.ClassSelection;
            // Reset values
            RelationshipSelectionAndInstancingViewModel riVM = new RelationshipSelectionAndInstancingViewModel();
            RelationshipSelectionView.DataContext = riVM;

            HideGenerationScenes();
            // Set tabitems to show correct ones
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            region_filling_2_1.Visibility = Visibility.Visible;
            region_filling_2_1_content.Visibility = Visibility.Visible;
            region_filling_2_2.Visibility = Visibility.Collapsed;
            region_filling_2_2_content.Visibility = Visibility.Collapsed;
            region_filling_2_3.Visibility = Visibility.Collapsed;
            region_filling_2_3_content.Visibility = Visibility.Collapsed;
            region_filling_2_4.Visibility = Visibility.Collapsed;
            region_filling_2_4_content.Visibility = Visibility.Collapsed;
            region_filling_2_5.Visibility = Visibility.Collapsed;
            region_filling_2_5_content.Visibility = Visibility.Visible;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            region_tabcontrol.SelectedIndex = 1;
            // Determine which view to be shown:
            if (SystemStateTracker.SelectClassSystem)
            {
                TangibleObjectsView.Visibility = Visibility.Collapsed;
                TangibleObjectsSystemView.Visibility = Visibility.Visible;
            }
            else
            {
                TangibleObjectsView.Visibility = Visibility.Visible;
                TangibleObjectsSystemView.Visibility = Visibility.Collapsed;
            }
        }

        internal void ChangeUIToRelationshipSelection()
        {
            // Reset values
            this.WorkInProgressConfiguration = this.SelectedTimePoint.Configuration.Copy();
            TopLeftSelectedGPUConfigurationResult = -1;
            BottomLeftSelectedGPUConfigurationResult = -1;
            TopRightSelectedGPUConfigurationResult = -1;
            BottomRightSelectedGPUConfigurationResult = -1;
            HideGenerationScenes();
            // Switch UI to next step (relation selection and instancing)
            CurrentFillingMode = MainFillingMode.RelationshipSelection;
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            region_filling_2_1.Visibility = Visibility.Collapsed;
            region_filling_2_1_content.Visibility = Visibility.Collapsed;
            region_filling_2_2.Visibility = Visibility.Visible;
            region_filling_2_2_content.Visibility = Visibility.Visible;
            region_filling_2_3.Visibility = Visibility.Collapsed;
            region_filling_2_3_content.Visibility = Visibility.Collapsed;
            region_filling_2_4.Visibility = Visibility.Collapsed;
            region_filling_2_4_content.Visibility = Visibility.Collapsed;
            region_filling_2_5.Visibility = Visibility.Collapsed;
            region_filling_2_5_content.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            region_tabcontrol.SelectedIndex = 2;

            // Determine which view to be shown:
            if (SystemStateTracker.SelectRelationshipSystem)
            {
                RelationshipSelectionView.Visibility = Visibility.Collapsed;
                RelationshipSelectionSystemView.Visibility = Visibility.Visible;
            }
            else
            {
                RelationshipSelectionView.Visibility = Visibility.Visible;
                RelationshipSelectionSystemView.Visibility = Visibility.Collapsed;
            }
        }

        internal void ChangeUIToRelationshipInstancing()
        {
            // Reset values
            this.WorkInProgressConfiguration = this.SelectedTimePoint.Configuration.Copy();
            TopLeftSelectedGPUConfigurationResult = -1;
            BottomLeftSelectedGPUConfigurationResult = -1;
            TopRightSelectedGPUConfigurationResult = -1;
            BottomRightSelectedGPUConfigurationResult = -1;
            HideGenerationScenes();
            // Switch UI to next step (relation selection and instancing)
            CurrentFillingMode = MainFillingMode.RelationshipInstancing;
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            region_filling_2_1.Visibility = Visibility.Collapsed;
            region_filling_2_1_content.Visibility = Visibility.Collapsed;
            region_filling_2_2.Visibility = Visibility.Collapsed;
            region_filling_2_2_content.Visibility = Visibility.Collapsed;
            region_filling_2_3.Visibility = Visibility.Visible;
            region_filling_2_3_content.Visibility = Visibility.Visible;
            region_filling_2_4.Visibility = Visibility.Collapsed;
            region_filling_2_4_content.Visibility = Visibility.Collapsed;
            region_filling_2_5.Visibility = Visibility.Collapsed;
            region_filling_2_5_content.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            region_tabcontrol.SelectedIndex = 3;

            // Determine which view to be shown:
            if (SystemStateTracker.SelectInstancesSystem)
            {
                InstanceSelectionView.Visibility = Visibility.Collapsed;
                InstanceSelectionSystemView.Visibility = Visibility.Visible;
            }
            else
            {
                InstanceSelectionView.Visibility = Visibility.Visible;
                InstanceSelectionSystemView.Visibility = Visibility.Collapsed;
            }
        }

        internal void ChangeUIToPlacement()
        {
            CurrentFillingMode = MainFillingMode.Placement;
            ShowGenerationScenes();
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            region_filling_2_1.Visibility = Visibility.Collapsed;
            region_filling_2_1_content.Visibility = Visibility.Collapsed;
            region_filling_2_2.Visibility = Visibility.Collapsed;
            region_filling_2_2_content.Visibility = Visibility.Collapsed;
            region_filling_2_3.Visibility = Visibility.Collapsed;
            region_filling_2_3_content.Visibility = Visibility.Collapsed;
            region_filling_2_4.Visibility = Visibility.Visible;
            region_filling_2_4_content.Visibility = Visibility.Visible;
            region_filling_2_5.Visibility = Visibility.Visible;
            region_filling_2_5_content.Visibility = Visibility.Visible;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;

            region_tabcontrol.SelectedIndex = 5;

            if (SystemStateTracker.SelectPositionSystem)
            {
                InspectGeneratedConfigurationsView.Visibility = Visibility.Collapsed;
                InspectGeneratedConfigurationsSystemView.Visibility = Visibility.Visible;
                region_filling_2_4.Visibility = Visibility.Collapsed;
                region_filling_2_4_content.Visibility = Visibility.Collapsed;
            }
            else
            {
                InspectGeneratedConfigurationsView.Visibility = Visibility.Visible;
                InspectGeneratedConfigurationsSystemView.Visibility = Visibility.Collapsed;
            }
        }

        internal void ShowGenerationScenes()
        {
            BasicScene.Visibility = Visibility.Collapsed;
            GeneratedScenes.Visibility = Visibility.Visible;
        }

        internal void HideGenerationScenes()
        {
            BasicScene.Visibility = Visibility.Visible;
            GeneratedScenes.Visibility = Visibility.Collapsed;
        }
        #endregion

        internal override void GenerateConfigurations()
        {
            GeneratedConfigurations = CudaGPUWrapper.CudaGPUWrapperCall(this.SelectedTimePoint, this.WorkInProgressConfiguration);
            if (SystemStateTracker.SelectPositionSystem)
            {
                GeneratedConfigurations = GeneratedConfigurations.OrderBy(gc => gc.TotalCosts).Take(4).ToList();
            }

            // Update list of configurations using generated list of regionpage
            var gcVM = (GenerateConfigurationsViewModel)InspectGeneratedConfigurationsView.DataContext;
            gcVM.Load(GeneratedConfigurations);
            if (SystemStateTracker.SelectPositionSystem)
            {
                this.InspectGeneratedConfigurationsSystemView.DataContext = gcVM;
                this.TopLeftSelectedGPUConfigurationResult = 0;
                this.TopRightSelectedGPUConfigurationResult = 1;
                this.BottomLeftSelectedGPUConfigurationResult = 2;
                this.BottomRightSelectedGPUConfigurationResult = 3;
            }
            else
            {
                this.InspectGeneratedConfigurationsView.DataContext = gcVM;
            }
        }

        internal void SaveInstancingAndSetupPlacement()
        {
            RelationshipSelectionAndInstancingViewModel rivm;
            // Retrieve rivm from correct source, system or user view
            if (SystemStateTracker.SelectInstancesSystem)
            {
                rivm = this.InstanceSelectionSystemView.GeneratedOptionsRelationshipSelectionListView.SelectedItem as RelationshipSelectionAndInstancingViewModel;
            }
            else
            {
                rivm = this.InstanceSelectionView.DataContext as RelationshipSelectionAndInstancingViewModel;
            }
            SaveRelationsAndInstancing(rivm);
            IntializeGenerateConfigurationsView(this.GenerateConfigurationsView);
            GenerateConfigurations();
        }

        internal void SetupInstancing()
        {
            // Retrieve data from correct view
            RelationshipSelectionAndInstancingViewModel rivm;
            if (SystemStateTracker.SelectRelationshipSystem)
            {
                rivm = RelationshipSelectionSystemView.GeneratedOptionsRelationshipSelectionListView.SelectedItem as RelationshipSelectionAndInstancingViewModel;
            }
            else
            {
                rivm = RelationshipSelectionView.DataContext as RelationshipSelectionAndInstancingViewModel;
            }

            // Transmit rivm to next (correct) view
            if (SystemStateTracker.SelectInstancesSystem)
            {
                // Generated instance options
                var list = RelationshipSelectionSolver.GetRandomRelationships(rivm, SystemStateTracker.NumberOfChoices);

                var VM = new RelationshipSelectionGeneratedOptionsViewModel();
                VM.Load(list);
                // Set datacontext of instance selection view
                this.InstanceSelectionSystemView.DataContext = VM;
            }
            else
            {
                this.InstanceSelectionView.DataContext = rivm;
            }
        }

        private void SaveRelationsAndInstancing(RelationshipSelectionAndInstancingViewModel rivm)
        {
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
            view.centroidX.Text = SystemStateTracker.centroidX.ToString();
            view.centroidY.Text = SystemStateTracker.centroidY.ToString();
            view.focalX.Text = SystemStateTracker.focalX.ToString();
            view.focalY.Text = SystemStateTracker.focalY.ToString();
            view.focalRot.Text = SystemStateTracker.focalRot.ToString();
            view.gridxDim.Text = SystemStateTracker.gridxDim.ToString();
        }

        internal void AddSelectedTangibleObject()
        {
            // Retrieve selected item from correct source
            TangibleObject selectedItem;
            if (SystemStateTracker.SelectClassSystem)
            {
                selectedItem = ((TOTreeTangibleObject)this.TangibleObjectsSystemView.ToListView.SelectedItem).TangibleObject;
            }
            else
            {
                selectedItem = ((TOTreeTangibleObject)this.TangibleObjectsView.ToListView.SelectedItem).TangibleObject;
            }
            if (selectedItem == null)
            {
                throw new Exception("Selected item is null given the current system/user configuration");
            }
            // Copy current configuration of timepoint so it can be changed.
            this.WorkInProgressConfiguration = this.SelectedTimePoint.Configuration.Copy();

            // Determine instance to add:
            InstanceOfObjectToAdd = new EntikaInstance(selectedItem);
            RelationshipSelectionAndInstancingViewModel riVM = new RelationshipSelectionAndInstancingViewModel();
            riVM.Load(this.selectedNode, this.SelectedTimePoint, InstanceOfObjectToAdd, this.WorkInProgressConfiguration.InstancedObjects, this.SelectedTimePoint.GetRemainingPredicates());
            if (riVM.OnRelationshipsMultiple.Count == 0 && riVM.OnRelationshipsSingle.Count == 0 && riVM.OnRelationshipsNone.Count == 0)
                throw new Exception("No on relationship at all");

            // Apply chosen results to timepoint
            // check whether it is the systems job or the user's
            if (SystemStateTracker.SelectRelationshipSystem)
            {
                var list = RelationshipSelectionSolver.GetRandomRelationships(riVM, SystemStateTracker.NumberOfChoices);
                
                var VM = new RelationshipSelectionGeneratedOptionsViewModel();
                VM.Load(list);

                RelationshipSelectionSystemView.DataContext = VM;
            }
            else
            {
                if (riVM.OnRelationshipsMultiple.Count > 0)
                    RelationshipSelectionView.OnRelationshipsMultipleListView.SelectedIndex = 0;
                if (riVM.OnRelationshipsSingle.Count > 0)
                    RelationshipSelectionView.OnRelationshipsSingleListView.SelectedIndex = 0;
                RelationshipSelectionView.DataContext = riVM;
            }
        }

        private void ApplyConfiguration()
        {
            GPUConfigurationResult gpuConfig;
            if (SystemStateTracker.SelectPositionSystem)
            {
                gpuConfig = (this.InspectGeneratedConfigurationsSystemView.ConfigurationsList.SelectedItem as GPUConfigurationResultViewModel).GPUConfigurationResult;
            }
            else
            {
                gpuConfig = this.GeneratedConfigurations[this.TopLeftSelectedGPUConfigurationResult];
            }
            
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
            TopLeftSelectedGPUConfigurationResult = -1;
            BottomLeftSelectedGPUConfigurationResult = -1;
            TopRightSelectedGPUConfigurationResult = -1;
            BottomRightSelectedGPUConfigurationResult = -1; ;
            GeneratedConfigurations = new List<GPUConfigurationResult>();
            InstanceOfObjectToAdd = null;
        }

        private void RefreshTangibleObjectsView()
        {
            TangibleObjectsValuedViewModel toVM = new TangibleObjectsValuedViewModel();
            toVM.LoadAll(this.selectedNode, this.SelectedTimePoint);
            TangibleObjectsView.DataContext = toVM;
            TangibleObjectsView.DefaultRB.IsChecked = true;
        }

        private void RefreshTangibleObjectsSystemView()
        {
            // Load all Tangible objects
            TangibleObjectsValuedViewModel toVM = new TangibleObjectsValuedViewModel();
            toVM.LoadAll(this.selectedNode, this.SelectedTimePoint);
            // Select #NumberOfChoices from available classes
            var tangibleObjectOptions = ClassSelectionSolver.GetRandomClasses(toVM, SystemStateTracker.NumberOfChoices);

            // Load options into right view
            TangibleObjectsValuedViewModel toVMOPtions = (TangibleObjectsSystemView.DataContext as TangibleObjectsValuedViewModel);
            toVMOPtions.LoadList(tangibleObjectOptions);
            //TangibleObjectsSystemView.DataContext = toVMOPtions;
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

        private void btnAddToCurrentRegion(object sender, RoutedEventArgs e)
        {
            // Check whether the first step is done by system or user:
            if (SystemStateTracker.SelectClassSystem)
            {
                RefreshTangibleObjectsSystemView();
            }
            else
            {
                RefreshTangibleObjectsView();
            }
            ChangeUIToTOClassSelection();
        }

        private void btnChangeCurrentRegion(object sender, RoutedEventArgs e)
        {
            this.WorkInProgressConfiguration = new Configuration();
            this.WorkInProgressConfiguration = this.SelectedTimePoint.Configuration.Copy();
            IntializeGenerateConfigurationsView(this.GenerateConfigurationsView2);
            ShowGenerationScenes();

            this.CurrentFillingMode = MainFillingMode.Repositioning;
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            region_filling_2_1.Visibility = Visibility.Collapsed;
            region_filling_2_1_content.Visibility = Visibility.Collapsed;
            region_filling_2_2.Visibility = Visibility.Collapsed;
            region_filling_2_2_content.Visibility = Visibility.Collapsed;
            region_filling_2_3.Visibility = Visibility.Collapsed;
            region_filling_2_3_content.Visibility = Visibility.Collapsed;
            region_filling_2_4.Visibility = Visibility.Visible;
            region_filling_2_4_content.Visibility = Visibility.Visible;
            region_filling_2_5.Visibility = Visibility.Visible;
            region_filling_2_5_content.Visibility = Visibility.Visible;
            region_tabcontrol.SelectedIndex = 4;
        }

        internal void ChangeUIToMainMenu()
        {
            // Reset views when coming from TO addition/placement
            HideGenerationScenes();

            this.CurrentFillingMode = MainFillingMode.None;
            region_filling_1.Visibility = Visibility.Visible;
            region_filling_1_content.Visibility = Visibility.Visible;
            region_filling_2_1.Visibility = Visibility.Collapsed;
            region_filling_2_1_content.Visibility = Visibility.Collapsed;
            region_filling_2_2.Visibility = Visibility.Collapsed;
            region_filling_2_2_content.Visibility = Visibility.Collapsed;
            region_filling_2_3.Visibility = Visibility.Collapsed;
            region_filling_2_3_content.Visibility = Visibility.Collapsed;
            region_filling_2_4.Visibility = Visibility.Collapsed;
            region_filling_2_4_content.Visibility = Visibility.Collapsed;
            region_filling_2_5.Visibility = Visibility.Collapsed;
            region_filling_2_5_content.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            region_tabcontrol.SelectedIndex = 0;
        }

        private void btnReturnToRegionCreation_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GetNavigationService(this);
            this.NavigationService.Navigate(new RegionCreationPage(selectedNode, this.SelectedTimePoint));
        }

        private void btnGraphPage_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GraphPage());
        }

        private void btnGotoDebugMode_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new DebugRegionPage(this.selectedNode, this.SelectedTimePoint));
        }

        public void SetMessageBoxText(string message)
        {
            MessageTextBox.Text = message;
        }

        private void btnGotoProcessSettingAdjustment(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new StepGenerationSettingPage(selectedNode, this.SelectedTimePoint));
        }
    }
}
