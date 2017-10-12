using Common;
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
        internal Vector3 manualPlacementPosition = new Vector3();
        internal Vector3 manualPlacementRotation = new Vector3();

        internal MainFillingMode PreviousFillingMode { get; private set; }

        public bool DebugMode;
        #endregion
        #region Setup

        public MainModeRegionPage(LocationNode selectedNode)
        {
            InitializeComponent();
            this.selectedNode = selectedNode;
            this.SelectedTimePoint = 0;
            SelectedEntikaInstances = new List<EntikaInstance>();

            UpdateConfiguration();

            // Set content of header
            this.PageHeader.Text = "Environment filling - " + this.selectedNode.LocationName;

            if (SystemStateTracker.AutomationDictionary[this.selectedNode.LocationName])
            {
                this.freeze_1.Visibility = Visibility.Visible;
            }
            else
            {
                this.freeze_1.Visibility = Visibility.Collapsed;
            }
        }

        private void NarrativeTimelineControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Fill control with stuff
            NarrativeTimelineViewModel narrativeTimelineViewModelObject =
               new NarrativeTimelineViewModel();
            narrativeTimelineViewModelObject.LoadFilteredTimePoints(selectedNode);

            NarrativeTimelineControl.DataContext = narrativeTimelineViewModelObject;

            var ntpVM = (NarrativeTimelineControl.DataContext as NarrativeTimelineViewModel).NarrativeTimePoints.Where(ntp => ntp.NarrativeTimePoint.Equals(this.selectedNode.TimePoints[SelectedTimePoint])).FirstOrDefault();
            ntpVM.Selected = true;
        }

        internal void SelectForScreen1(int selection)
        {
            this.TopLeftSelectedGPUConfigurationResult = selection;
            this.header_screen_1.Text = "Screen #1, showing option #" + (selection + 1);
        }

        internal void SelectForScreen2(int selection)
        {
            this.TopRightSelectedGPUConfigurationResult = selection;
            this.header_screen_2.Text = "Screen #2, showing option #" + (selection + 1);
        }

        internal void SelectForScreen3(int selection)
        {
            this.BottomLeftSelectedGPUConfigurationResult = selection;
            this.header_screen_3.Text = "Screen #3, showing option #" + (selection + 1);
        }

        internal void SelectForScreen4(int selection)
        {
            this.BottomRightSelectedGPUConfigurationResult = selection;
            this.header_screen_4.Text = "Screen #4, showing option #" + (selection + 1);
        }

        private void RegionDetailTimePointView_Loaded(object sender, RoutedEventArgs e)
        {
            // Fill control with stuff
            DetailTimePointViewModel timePointViewModelObject =
               new DetailTimePointViewModel();
            timePointViewModelObject.LoadObjects(selectedNode, this.selectedNode.TimePoints[SelectedTimePoint]);
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
                case MainFillingMode.AutomatedPlacement:
                    ChangeUIToRelationshipSelection();
                    break;
                case MainFillingMode.ManualPlacement:
                    ChangeUIToRelationshipSelection();
                    break;
                case MainFillingMode.SelectionChangeMode:
                    ChangeUIToMainMenu();
                    break;
                case MainFillingMode.ManualChange:
                    ChangeUIToMainMenu();
                    break;
                case MainFillingMode.Repositioning:
                    ChangeUIToMainMenu();
                    break;
                case MainFillingMode.Removal:
                    ChangeUIToMainMenu();
                    break;
                case MainFillingMode.Freeze:
                    ChangeUIToMainMenu();
                    break;
                default:
                    break;
            }
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
                    SaveInstancingAndSetupPlacement();
                    ChangeUIToPlacement();
                    break;
                case MainFillingMode.AutomatedPlacement:
                    ApplyConfiguration();
                    ChangeUIToMainMenu();
                    break;
                case MainFillingMode.ManualPlacement:
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
            add_mode_1.Visibility = Visibility.Visible;
            add_mode_1_content.Visibility = Visibility.Visible;
            add_mode_2.Visibility = Visibility.Collapsed;
            add_mode_2_content.Visibility = Visibility.Collapsed;
            add_mode_3_1.Visibility = Visibility.Collapsed;
            add_mode_3_1_content.Visibility = Visibility.Collapsed;
            add_mode_3_2.Visibility = Visibility.Collapsed;
            add_mode_3_2_content.Visibility = Visibility.Visible;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            if (!this.DebugMode)
            {
                inspection_2.Visibility = Visibility.Visible;
                inspection_3.Visibility = Visibility.Visible;
            }
            region_tabcontrol.SelectedIndex = 1;
            // Determine which view to be shown:
            if (SystemStateTracker.AutomationDictionary[this.selectedNode.LocationName])
            {
                TangibleObjectsView.Visibility = Visibility.Collapsed;
                TangibleObjectsSystemView.Visibility = Visibility.Visible;
                TangibleObjectsSystemView.ToListView.SelectedIndex = 0;
            }
            else
            {
                TangibleObjectsView.Visibility = Visibility.Visible;
                TangibleObjectsSystemView.Visibility = Visibility.Collapsed;
                TangibleObjectsView.ToListView.SelectedIndex = 0;
            }
        }

        internal void ChangeUIToRelationshipSelection()
        {
            this.CurrentFillingMode = MainFillingMode.RelationshipSelection;

            // Reset values
            this.WorkInProgressConfiguration = this.Configuration.Copy();
            this.WIPAdditionDelta = null;
            this.WIPRelationshipDeltas = new List<RelationshipDelta>();
            TopLeftSelectedGPUConfigurationResult = -1;
            BottomLeftSelectedGPUConfigurationResult = -1;
            TopRightSelectedGPUConfigurationResult = -1;
            BottomRightSelectedGPUConfigurationResult = -1;
            HideGenerationScenes();
            // Switch UI to next step (relation selection and instancing)
            CurrentFillingMode = MainFillingMode.RelationshipSelection;
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            add_mode_1.Visibility = Visibility.Collapsed;
            add_mode_1_content.Visibility = Visibility.Collapsed;
            add_mode_2.Visibility = Visibility.Visible;
            add_mode_2_content.Visibility = Visibility.Visible;
            add_mode_3_1.Visibility = Visibility.Collapsed;
            add_mode_3_1_content.Visibility = Visibility.Collapsed;
            add_mode_3_2.Visibility = Visibility.Collapsed;
            add_mode_3_2_content.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            if (!this.DebugMode)
            {
                inspection_2.Visibility = Visibility.Visible;
                inspection_3.Visibility = Visibility.Visible;
            }
            region_tabcontrol.SelectedIndex = 2;

            // Determine which view to be shown:
            if (SystemStateTracker.AutomationDictionary[this.selectedNode.LocationName])
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

        internal void ChangeUIToPlacement()
        {
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            add_mode_1.Visibility = Visibility.Collapsed;
            add_mode_1_content.Visibility = Visibility.Collapsed;
            add_mode_2.Visibility = Visibility.Collapsed;
            add_mode_2_content.Visibility = Visibility.Collapsed;

            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            if (!this.DebugMode)
            {
                inspection_2.Visibility = Visibility.Visible;
                inspection_3.Visibility = Visibility.Visible;
            }

            if (SystemStateTracker.AutomationDictionary[this.selectedNode.LocationName])
            {
                this.CurrentFillingMode = MainFillingMode.AutomatedPlacement;
                add_mode_3_1.Visibility = Visibility.Collapsed;
                add_mode_3_1_content.Visibility = Visibility.Collapsed;
                add_mode_3_2.Visibility = Visibility.Visible;
                add_mode_3_2_content.Visibility = Visibility.Visible;
                add_mode_3_3.Visibility = Visibility.Collapsed;
                add_mode_3_3_content.Visibility = Visibility.Collapsed;
                ShowGenerationScenes();
                region_tabcontrol.SelectedIndex = 4;
            }
            else
            {
                this.CurrentFillingMode = MainFillingMode.ManualPlacement;
                add_mode_3_1.Visibility = Visibility.Collapsed;
                add_mode_3_1_content.Visibility = Visibility.Collapsed;
                add_mode_3_2.Visibility = Visibility.Collapsed;
                add_mode_3_2_content.Visibility = Visibility.Collapsed;
                add_mode_3_3.Visibility = Visibility.Visible;
                add_mode_3_3_content.Visibility = Visibility.Visible;
                HideGenerationScenes();
                region_tabcontrol.SelectedIndex = 5;
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

        private void ChangeUIToManualChangeMode()
        {
            this.CurrentFillingMode = MainFillingMode.ManualChange;
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            this.change_mode_1.Visibility = Visibility.Collapsed;
            this.change_mode_1_content.Visibility = Visibility.Collapsed;
            this.change_mode_2.Visibility = Visibility.Visible;
            if (!this.DebugMode)
            {
                inspection_2.Visibility = Visibility.Visible;
                inspection_3.Visibility = Visibility.Visible;
            }
            region_tabcontrol.SelectedIndex = 7;
        }

        private void ChangeUIToChangeModeSelection()
        {
            this.CurrentFillingMode = MainFillingMode.SelectionChangeMode;

            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            this.change_mode_1.Visibility = Visibility.Visible;
            this.change_mode_1_content.Visibility = Visibility.Visible;
            this.change_mode_2.Visibility = Visibility.Collapsed;
            if (!this.DebugMode)
            {
                inspection_2.Visibility = Visibility.Visible;
                inspection_3.Visibility = Visibility.Visible;
            }
            region_tabcontrol.SelectedIndex = 6;
        }

        private void ChangeUIToAutomatedRepositioning()
        {
            this.CurrentFillingMode = MainFillingMode.Repositioning;
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            add_mode_1.Visibility = Visibility.Collapsed;
            add_mode_1_content.Visibility = Visibility.Collapsed;
            add_mode_2.Visibility = Visibility.Collapsed;
            add_mode_2_content.Visibility = Visibility.Collapsed;
            add_mode_3_1.Visibility = Visibility.Visible;
            add_mode_3_1_content.Visibility = Visibility.Visible;
            add_mode_3_2.Visibility = Visibility.Visible;
            add_mode_3_2_content.Visibility = Visibility.Visible;

            this.change_mode_1.Visibility = Visibility.Collapsed;
            this.change_mode_1_content.Visibility = Visibility.Collapsed;
            this.change_mode_2.Visibility = Visibility.Collapsed;
            if (!this.DebugMode)
            {
                inspection_2.Visibility = Visibility.Visible;
                inspection_3.Visibility = Visibility.Visible;
            }
            region_tabcontrol.SelectedIndex = 4;

            this.WorkInProgressConfiguration = new Configuration();
            this.WorkInProgressConfiguration = this.Configuration.Copy();
            IntializeGenerateConfigurationsView(this.GenerateConfigurationsView2);
            GenerateConfigurations();
            ShowGenerationScenes();
        }

        #endregion

        internal override void GenerateConfigurations()
        {
            GeneratedConfigurations = CudaGPUWrapper.CudaGPUWrapperCall(this.selectedNode.TimePoints[SelectedTimePoint], this.WorkInProgressConfiguration);
            GeneratedConfigurations = GeneratedConfigurations.OrderByDescending(gc => gc.TotalCosts).ToList();
            //foreach (var configuration in GeneratedConfigurations)
            //{
            //    Console.WriteLine("Configuration #?");
            //    Console.WriteLine("Clearance costs: " + configuration.ClearanceCosts);
            //    Console.WriteLine("Focal point costs: " + configuration.FocalPointCosts);
            //    Console.WriteLine("Off limits costs: " + configuration.OffLimitsCosts);
            //    Console.WriteLine("Pair wise costs: " + configuration.PairWiseCosts);
            //    Console.WriteLine("Surface area costs: " +  configuration.SurfaceAreaCosts);
            //    Console.WriteLine("Symmertry costs: " + configuration.SymmetryCosts);
            //    Console.WriteLine("Visual balance costs: " + configuration.VisualBalanceCosts);
            //    Console.WriteLine();
            //}
            if (SystemStateTracker.AutomationDictionary[this.selectedNode.LocationName])
            {
                // Take X to reduce options
                GeneratedConfigurations = GeneratedConfigurations.ToList();
            }

            // Update list of configurations using generated list of regionpage
            var gcVM = (GenerateConfigurationsViewModel)InspectGeneratedConfigurationsSystemView.DataContext;
            gcVM.Load(GeneratedConfigurations);
            this.InspectGeneratedConfigurationsSystemView.DataContext = gcVM;
            this.SelectForScreen1(0);
            this.SelectForScreen2(1);
            this.SelectForScreen3(2);
            this.SelectForScreen4(3);
        }

        internal void SaveInstancingAndSetupPlacement()
        {
            // Retrieve rivm from correct source, system or user view
            if (SystemStateTracker.AutomationDictionary[this.selectedNode.LocationName])
            {
                AutomatedRelationshipSelectionViewModel arsVM = this.RelationshipSelectionSystemView.GeneratedOptionsRelationshipSelectionListView.SelectedItem as AutomatedRelationshipSelectionViewModel;
                SaveRelationships(arsVM.OnRelationship, arsVM.OtherRelationships.ToList());
                IntializeGenerateConfigurationsView(this.GenerateConfigurationsView);
                GenerateConfigurations();
            }
            else
            {
                var onRelationship = (OnRelationshipViewModel)RelationshipSelectionView.OnRelationshipsListView.SelectedItems[0];
                var otherRelationships = RelationshipSelectionView.OtherRelationshipsListView.SelectedItems.Cast<OtherRelationshipViewModel>().ToList();
                SaveRelationships(onRelationship, otherRelationships);
            }
        }

        private void SaveRelationships(OnRelationshipViewModel onRelationship, List<OtherRelationshipViewModel> otherRelationships)
        {
            
            // Create new relationship instance
            var onRelationshipInstance = new RelationshipInstance();
            onRelationshipInstance.BaseRelationship = onRelationship.Relationship;
            onRelationshipInstance.Target = InstanceOfObjectToAdd;
            onRelationshipInstance.Source = onRelationship.Source;

            // Add relationship to different instances
            InstanceOfObjectToAdd.RelationshipsAsTarget.Add(onRelationshipInstance);
            onRelationshipInstance.Source.RelationshipsAsSource.Add(onRelationshipInstance);
            // Set start position of instance to add
            InstanceOfObjectToAdd.Position = new Vector3(onRelationshipInstance.Source.Position.X, onRelationshipInstance.Source.Position.Y, onRelationshipInstance.Source.BoundingBox.Max.Z);

            // Add on relationship to work in progress configuration
            this.WorkInProgressConfiguration.InstancedRelations.Add(onRelationshipInstance);
            this.WIPRelationshipDeltas.Add(new RelationshipDelta(this.SelectedTimePoint, onRelationshipInstance, RelationshipDeltaType.Add));

            foreach (var selectedItem in otherRelationships)
            {
                var other = selectedItem as OtherRelationshipViewModel;
                var otherRelationshipInstance = new RelationshipInstance();
                otherRelationshipInstance.BaseRelationship = other.Relationship;
                if (other.AsTarget)
                {
                    otherRelationshipInstance.Target = InstanceOfObjectToAdd;
                    otherRelationshipInstance.Source = other.SubjectInstance;

                    InstanceOfObjectToAdd.RelationshipsAsTarget.Add(otherRelationshipInstance);
                    otherRelationshipInstance.Source.RelationshipsAsSource.Add(otherRelationshipInstance);
                }
                else
                {
                    otherRelationshipInstance.Target = other.SubjectInstance;
                    otherRelationshipInstance.Source = InstanceOfObjectToAdd;

                    InstanceOfObjectToAdd.RelationshipsAsSource.Add(otherRelationshipInstance);
                    otherRelationshipInstance.Target.RelationshipsAsTarget.Add(otherRelationshipInstance);
                }

                this.WorkInProgressConfiguration.InstancedRelations.Add(otherRelationshipInstance);
                this.WIPRelationshipDeltas.Add(new RelationshipDelta(this.SelectedTimePoint, otherRelationshipInstance, RelationshipDeltaType.Add));
            }


            // this.WorkInProgressConfiguration.InstancedRelations.Add(otherRelationshipInstance);
            // this.WIPRelationshipDeltas.Add(new RelationshipDelta(this.SelectedTimePoint, otherRelationshipInstance, RelationshipDeltaType.Add));

            this.WorkInProgressConfiguration.InstancedObjects.Add(InstanceOfObjectToAdd);
            this.WIPAdditionDelta = new InstanceDelta(this.SelectedTimePoint, InstanceOfObjectToAdd, InstanceDeltaType.Add, null, null);
            this.manualPlacementPosition = InstanceOfObjectToAdd.Position;
        }

        public void IntializeGenerateConfigurationsView(GenerateConfigurationsView view)
        {
            GenerateConfigurationsViewModel gcVM = new GenerateConfigurationsViewModel();
            gcVM.Load(new List<GPUConfigurationResult>());
            InspectGeneratedConfigurationsSystemView.DataContext = gcVM;
            view.SliderWeightFocalPoint.Value = SystemStateTracker.WeightFocalPoint;
            view.SliderWeightPairWise.Value = SystemStateTracker.WeightPairWise;
            view.SliderWeightSymmetry.Value = SystemStateTracker.WeightSymmetry;
            view.SliderWeightVisualBalance.Value = SystemStateTracker.WeightVisualBalance;
            view.SliderWeightClearance.Value = SystemStateTracker.WeightClearance;
            view.SliderWeightOffLimits.Value = SystemStateTracker.WeightOffLimits;
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
            if (SystemStateTracker.AutomationDictionary[this.selectedNode.LocationName])
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
            this.WorkInProgressConfiguration = this.Configuration.Copy();

            // Determine instance to add:
            InstanceOfObjectToAdd = new EntikaInstance(selectedItem);
            RelationshipSelectionAndInstancingViewModel riVM = new RelationshipSelectionAndInstancingViewModel();
            riVM.Load(this.selectedNode, this.selectedNode.TimePoints[SelectedTimePoint], InstanceOfObjectToAdd, this.WorkInProgressConfiguration.InstancedObjects, this.selectedNode.TimePoints[SelectedTimePoint].GetRemainingPredicates());
            if (riVM.OnRelationshipsMultiple.Count == 0 && riVM.OnRelationshipsSingle.Count == 0 && riVM.OnRelationshipsNone.Count == 0)
                throw new Exception("No on relationship at all");

            // Apply chosen results to timepoint
            // check whether it is the systems job or the user's
            if (SystemStateTracker.AutomationDictionary[this.selectedNode.LocationName])
            {
                // Check whether class has atleast one on relationship
                var VM = new AutomatedResultsRelationshipSelectionViewModel();
                RelationshipSelectionSystemView.DataContext = RelationshipSelectionSolver.GetRandomRelationships(VM, SystemStateTracker.NumberOfTries, InstanceOfObjectToAdd, this.selectedNode, this.WorkInProgressConfiguration.InstancedObjects);
                RelationshipSelectionSystemView.GeneratedOptionsRelationshipSelectionListView.SelectedIndex = 0;
            }
            else
            {
                ManualRelationshipsViewModel mrVM = new ManualRelationshipsViewModel();
                mrVM.Load(this.selectedNode, InstanceOfObjectToAdd, this.WorkInProgressConfiguration.InstancedObjects, this.selectedNode.TimePoints[SelectedTimePoint].GetRemainingPredicates());
                RelationshipSelectionView.DataContext = mrVM;
                RelationshipSelectionView.OnRelationshipsListView.SelectedIndex = 0;
            }
        }

        private void ApplyConfiguration()
        {
            // Automatic application
            if (SystemStateTracker.AutomationDictionary[this.selectedNode.LocationName])
            {
                GPUConfigurationResult gpuConfig;
                gpuConfig = (this.InspectGeneratedConfigurationsSystemView.ConfigurationsList.DataContext as GenerateConfigurationsViewModel).GPUConfigurationResults[this.TopLeftSelectedGPUConfigurationResult].GPUConfigurationResult;

                for (int i = 0; i < gpuConfig.Instances.Count; i++)
                {
                    var WIPinstance = this.WorkInProgressConfiguration.InstancedObjects.Where(io => io.Equals(gpuConfig.Instances[i].entikaInstance)).FirstOrDefault();
                    var adjustedPos = new Vector3(gpuConfig.Instances[i].Position.X, gpuConfig.Instances[i].Position.Y, gpuConfig.Instances[i].Position.Z);
                    var adjustedRot = new Vector3(gpuConfig.Instances[i].Rotation.X, gpuConfig.Instances[i].Rotation.Y, gpuConfig.Instances[i].Rotation.Z);
                    if (!WIPinstance.Position.Equals(adjustedPos) || !WIPinstance.Rotation.Equals(adjustedRot))
                    {
                        WIPinstance.Position = adjustedPos;
                        WIPinstance.Rotation = adjustedRot;
                        WIPinstance.UpdateBoundingBoxAndShape();
                        if (WIPAdditionDelta != null && WIPAdditionDelta.RelatedInstance.Equals(WIPinstance))
                        {
                            WIPAdditionDelta.RelatedInstance.Position = adjustedPos;
                            WIPAdditionDelta.RelatedInstance.Rotation = adjustedRot;
                        }
                        else
                        {
                            var additionDeltaOfInstance = this.selectedNode.TimePoints[this.SelectedTimePoint].InstanceDeltas.Where(id => id.DT.Equals(InstanceDeltaType.Add) && id.RelatedInstance.Equals(WIPinstance)).FirstOrDefault();
                            if (additionDeltaOfInstance == null && !WIPAdditionDelta.RelatedInstance.Equals(WIPinstance))
                            {
                                this.selectedNode.TimePoints[this.SelectedTimePoint].InstanceDeltas.Add(new InstanceDelta(this.SelectedTimePoint, WIPinstance, InstanceDeltaType.Change, adjustedPos, adjustedRot));
                            }
                            else
                            {
                                additionDeltaOfInstance.Position = adjustedPos;
                                additionDeltaOfInstance.Rotation = adjustedRot;
                            }
                        }
                    }
                }
                if (this.WIPAdditionDelta != null)
                {
                    this.WIPAdditionDelta.Position = this.WIPAdditionDelta.RelatedInstance.Position;
                    this.WIPAdditionDelta.Rotation = this.WIPAdditionDelta.RelatedInstance.Rotation;
                }
            }
            // Manual application
            else
            {
                this.InstanceOfObjectToAdd.Position = manualPlacementPosition;
                this.InstanceOfObjectToAdd.Rotation = manualPlacementRotation;
                this.InstanceOfObjectToAdd.UpdateBoundingBoxAndShape();

                if (this.WIPAdditionDelta != null)
                {
                    this.WIPAdditionDelta.Position = manualPlacementPosition;
                    this.WIPAdditionDelta.Rotation = manualPlacementRotation;
                }
            }

            this.Configuration = WorkInProgressConfiguration;

            // Save deltas
            foreach (var WIPRelationshipDelta in WIPRelationshipDeltas)
            {
                this.selectedNode.TimePoints[this.SelectedTimePoint].RelationshipDeltas.Add(WIPRelationshipDelta);
            }
            this.selectedNode.TimePoints[this.SelectedTimePoint].InstanceDeltas.Add(WIPAdditionDelta);

            // Reset used variables
            WorkInProgressConfiguration = new Configuration();
            this.WIPAdditionDelta = null;
            this.WIPRelationshipDeltas = new List<RelationshipDelta>();
            TopLeftSelectedGPUConfigurationResult = -1;
            BottomLeftSelectedGPUConfigurationResult = -1;
            TopRightSelectedGPUConfigurationResult = -1;
            BottomRightSelectedGPUConfigurationResult = -1; ;
            GeneratedConfigurations = new List<GPUConfigurationResult>();
            InstanceOfObjectToAdd = null;

            manualPlacementRotation = new Vector3();
            manualPlacementPosition = new Vector3();
        }

        private void RefreshTangibleObjectsView()
        {
            TangibleObjectsValuedViewModel toVM = new TangibleObjectsValuedViewModel();
            toVM.LoadAll(this.selectedNode, this.selectedNode.TimePoints[SelectedTimePoint], this.Configuration);
            // Order by name
            toVM.TangibleObjectsValued.OrderBy(s => s.TangibleObject.DefaultName);
            TangibleObjectsView.DataContext = toVM;
        }

        private void RefreshTangibleObjectsSystemView()
        {
            //// Load options
            //var tangibleObjectOptions = ClassSelectionSolver.GetRandomClasses(toVM, SystemStateTracker.NumberOfChoices);

            //// Load options into right view
            //TangibleObjectsValuedViewModel toVMOPtions = (TangibleObjectsSystemView.DataContext as TangibleObjectsValuedViewModel);
            //toVMOPtions.LoadList(tangibleObjectOptions);

            // Load all Tangible objects
            TangibleObjectsValuedViewModel toVM = new TangibleObjectsValuedViewModel();
            toVM.LoadAll(this.selectedNode, this.selectedNode.TimePoints[SelectedTimePoint], this.Configuration);
            // Select #NumberOfChoices from available classes using importance value
            // Order by importance value
            toVM.TangibleObjectsValued.OrderBy(s => s.EndValue);
            TangibleObjectsSystemView.DefaultRB.IsChecked = true;
            TangibleObjectsSystemView.DataContext = toVM;
        }

        internal override void UpdateDetailView(NarrativeTimePoint narrativeTimePoint)
        {
            // Update detailtab
            this.RefreshViewsUsingSelected();
            (RegionDetailTimePointView.DataContext as DetailTimePointViewModel).LoadObjects(selectedNode, narrativeTimePoint);
        }

        private void btnReturnToInit_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ClassSelectionPage(selectedNode));
        }

        private void btnAddMode(object sender, RoutedEventArgs e)
        {
            // Check whether the first step is done by system or user:
            if (SystemStateTracker.AutomationDictionary[this.selectedNode.LocationName])
            {
                RefreshTangibleObjectsSystemView();
            }
            else
            {
                RefreshTangibleObjectsView();
            }
            ChangeUIToTOClassSelection();
        }

        private void btnChangeMode(object sender, RoutedEventArgs e)
        {
            if (SystemStateTracker.AutomationDictionary[this.selectedNode.LocationName])
            {
                ChangeUIToChangeModeSelection();
            }
            else
            {
                ChangeUIToManualChangeMode();
            }
        }

        private void btnRemoveMode(object sender, RoutedEventArgs e)
        {
            ChangeUIToRemovalMode();
        }

        internal void ChangeUIToMainMenu()
        {
            // Reset views when coming from TO addition/placement
            HideGenerationScenes();

            this.SelectedEntikaInstances = new List<EntikaInstance>();

            this.CurrentFillingMode = MainFillingMode.MainMenu;
            region_filling_1.Visibility = Visibility.Visible;
            region_filling_1_content.Visibility = Visibility.Visible;
            add_mode_1.Visibility = Visibility.Collapsed;
            add_mode_1_content.Visibility = Visibility.Collapsed;
            add_mode_2.Visibility = Visibility.Collapsed;
            add_mode_2_content.Visibility = Visibility.Collapsed;
            add_mode_3_1.Visibility = Visibility.Collapsed;
            add_mode_3_1_content.Visibility = Visibility.Collapsed;
            add_mode_3_2.Visibility = Visibility.Collapsed;
            add_mode_3_2_content.Visibility = Visibility.Collapsed;
            add_mode_3_3.Visibility = Visibility.Collapsed;
            add_mode_3_3_content.Visibility = Visibility.Collapsed;
            change_mode_1.Visibility = Visibility.Collapsed;
            change_mode_1_content.Visibility = Visibility.Collapsed;
            change_mode_2.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            removal_mode_1.Visibility = Visibility.Collapsed;
            removal_mode_1_content.Visibility = Visibility.Collapsed;
            if (!this.DebugMode)
            {
                inspection_2.Visibility = Visibility.Visible;
                inspection_3.Visibility = Visibility.Visible;
            }
            region_tabcontrol.SelectedIndex = 0;
        }

        private void ChangeUIToRemovalMode()
        {
            HideGenerationScenes();

            this.CurrentFillingMode = MainFillingMode.Removal;
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            add_mode_1.Visibility = Visibility.Collapsed;
            add_mode_1_content.Visibility = Visibility.Collapsed;
            add_mode_2.Visibility = Visibility.Collapsed;
            add_mode_2_content.Visibility = Visibility.Collapsed;
            add_mode_3_1.Visibility = Visibility.Collapsed;
            add_mode_3_1_content.Visibility = Visibility.Collapsed;
            add_mode_3_2.Visibility = Visibility.Collapsed;
            add_mode_3_2_content.Visibility = Visibility.Collapsed;
            add_mode_3_3.Visibility = Visibility.Collapsed;
            add_mode_3_3_content.Visibility = Visibility.Collapsed;
            change_mode_1.Visibility = Visibility.Collapsed;
            change_mode_1_content.Visibility = Visibility.Collapsed;
            change_mode_2.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            removal_mode_1.Visibility = Visibility.Visible;
            removal_mode_1_content.Visibility = Visibility.Visible;
            if (!this.DebugMode)
            {
                inspection_2.Visibility = Visibility.Visible;
                inspection_3.Visibility = Visibility.Visible;
            }
            region_tabcontrol.SelectedIndex = 9;
        }

        private void DeltaListView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDeltaListView();
        }

        public void UpdateDeltaListView()
        {
            var dlvm = new DeltaListViewModel();
            dlvm.Load(selectedNode, SelectedTimePoint);
            this.DeltaListView.DataContext = dlvm;
        }

        private void RemovalModeView_Loaded(object sender, RoutedEventArgs e)
        {
            SimpleSelectionViewModel rmVM = new SimpleSelectionViewModel();
            rmVM.LoadSelectedInstances(this.SelectedEntikaInstances);
            RemovalModeView.DataContext = rmVM;
        }

        internal void UpdateRemovalModeView()
        {
            (RemovalModeView.DataContext as SimpleSelectionViewModel).LoadSelectedInstances(this.SelectedEntikaInstances);
        }

        private void FreezeView_Loaded(object sender, RoutedEventArgs e)
        {
            SimpleSelectionViewModel rmVM = new SimpleSelectionViewModel();
            rmVM.LoadSelectedInstances(this.SelectedEntikaInstances);
            FreezeView.DataContext = rmVM;
        }

        internal void UpdateFreezeModeView()
        {
            (FreezeView.DataContext as SimpleSelectionViewModel).LoadSelectedInstances(this.SelectedEntikaInstances);
        }

        private void ManualChangeView_Loaded(object sender, RoutedEventArgs e)
        {
            SimpleSelectionViewModel rmVM = new SimpleSelectionViewModel();
            rmVM.LoadSelectedInstances(this.SelectedEntikaInstances);
            ManualChangeView.DataContext = rmVM;
        }

        internal void UpdateManualPlacementView()
        {
            (ManualChangeView.DataContext as SimpleSelectionViewModel).LoadSelectedInstances(this.SelectedEntikaInstances);
        }

        internal override void RefreshViewsUsingSelected()
        {
            RefreshSelectedObjectView();
            UpdateRemovalModeView();
            UpdateFreezeModeView();
            UpdateManualPlacementView();
        }

        internal override void RefreshSelectedObjectView()
        {
            var removalinstances = this.SelectedEntikaInstances.Where(sei => !this.Configuration.InstancedObjects.Contains(sei)).ToList();
            foreach (var removal in removalinstances)
            {
                this.SelectedEntikaInstances.Remove(removal);
            }
            (this.SelectedObjectDetailView.DataContext as SelectedObjectDetailViewModel).LoadSelectedInstances(this.SelectedEntikaInstances, this.selectedNode.TimePoints[SelectedTimePoint], this.Configuration);
        }

        private void SelectedObjectDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            SelectedObjectDetailViewModel selectedObjectViewModelObject =
               new SelectedObjectDetailViewModel();
            selectedObjectViewModelObject.LoadSelectedInstances(this.SelectedEntikaInstances, this.selectedNode.TimePoints[SelectedTimePoint], this.Configuration);
            SelectedObjectDetailView.DataContext = selectedObjectViewModelObject;
        }

        private void btnReturnToRegionCreation_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GetNavigationService(this);
            this.NavigationService.Navigate(new RegionCreationPage(selectedNode));
        }

        private void btnGraphPage_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GraphPage());
        }

        public void SetMessageBoxText(string message)
        {
            MessageTextBox.Text = message;
        }

        private void btnManualChangeMode(object sender, RoutedEventArgs e)
        {
            this.ChangeUIToManualChangeMode();
        }

        private void btnAutomatedRepositionMode(object sender, RoutedEventArgs e)
        {
            this.ChangeUIToAutomatedRepositioning();
        }

        private void btnBack(object sender, RoutedEventArgs e)
        {
            this.Back();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;

            switch (tabItem)
            {
                case "Freeze":
                    this.PreviousFillingMode = this.CurrentFillingMode;
                    this.CurrentFillingMode = MainFillingMode.Freeze;
                    break;
                default:
                    if (this.PreviousFillingMode != MainFillingMode.None)
                    {
                        this.CurrentFillingMode = this.PreviousFillingMode;
                        this.PreviousFillingMode = MainFillingMode.None;
                    }
                    return;
            }
        }
    }
}
