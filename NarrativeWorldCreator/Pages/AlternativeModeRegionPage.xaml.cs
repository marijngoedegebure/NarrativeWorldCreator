﻿using Microsoft.Xna.Framework;
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
    public partial class AlternativeModeRegionPage : ModeBaseRegionPage
    {
        #region Fields
        internal AlternativeFillingMode CurrentFillingMode = AlternativeFillingMode.None;

        internal enum AlternativeFillingMode
        {
            None = 0,
            ClassSelection = 1,
            FinalSelection = 2,
            Reposition = 3
        }
        #endregion

        #region Setup
        public AlternativeModeRegionPage(LocationNode selectedNode, NarrativeTimePoint SelectedTimePont)
        {
            InitializeComponent();
            this.selectedNode = selectedNode;
            this.SelectedTimePoint = SelectedTimePont;
            SelectedEntikaInstances = new List<EntikaInstance>();
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

        #endregion

        #region UIChanges
        internal override void Back()
        {
            // Check current mode
            switch (CurrentFillingMode)
            {
                case AlternativeFillingMode.ClassSelection:
                    ChangeUIToMainMenu();
                    break;
                case AlternativeFillingMode.FinalSelection:
                    ChangeUIToTOClassSelection();
                    break;
                case AlternativeFillingMode.Reposition:
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
                case AlternativeFillingMode.ClassSelection:
                    AddSelectedTangibleObject();
                    ChangeUIToFinalSelection();
                    break;
                case AlternativeFillingMode.FinalSelection:
                    ApplyConfiguration();
                    ChangeUIToMainMenu();
                    break;
                case AlternativeFillingMode.Reposition:
                    ApplyConfiguration();
                    ChangeUIToMainMenu();
                    break;
                default:
                    break;
            }
        }

        internal void ChangeUIToTOClassSelection()
        {
            this.CurrentFillingMode = AlternativeFillingMode.ClassSelection;
            // Reset values
            RelationshipSelectionAndInstancingViewModel riVM = new RelationshipSelectionAndInstancingViewModel();
            RelationshipSelectionAndInstancingView.DataContext = riVM;

            HideGenerationScenes();

            // Set views of region filling 1 to correct state
            TangibleObjectsView.Visibility = Visibility.Visible;
            RelationshipSelectionAndInstancingView.Visibility = Visibility.Collapsed;
            GenerateConfigurationsView.Visibility = Visibility.Collapsed;
            generation_1.Visibility = Visibility.Collapsed;
            // Set tabitems to show correct ones
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            region_filling_2.Visibility = Visibility.Visible;
            region_filling_2_content.Visibility = Visibility.Visible;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            region_tabcontrol.SelectedIndex = 1;
        }

        internal void ChangeUIToFinalSelection()
        {
            this.CurrentFillingMode = AlternativeFillingMode.FinalSelection;

            ShowGenerationScenes();

            TangibleObjectsView.Visibility = Visibility.Collapsed;
            RelationshipSelectionAndInstancingView.Visibility = Visibility.Collapsed;
            GenerateConfigurationsView.Visibility = Visibility.Visible;
            generation_1.Visibility = Visibility.Visible;
            // Set tabitems to show correct Visible
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            region_filling_2.Visibility = Visibility.Visible;
            region_filling_2_content.Visibility = Visibility.Visible;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            region_tabcontrol.SelectedIndex = 1;
        }

        internal void ShowGenerationScenes()
        {
            BasicScene.Visibility = Visibility.Collapsed;
            GenerationSceneLeft.Visibility = Visibility.Visible;
            GenerationSceneRight.Visibility = Visibility.Visible;
            BasicWindow.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
        }

        internal void HideGenerationScenes()
        {
            BasicScene.Visibility = Visibility.Visible;
            GenerationSceneLeft.Visibility = Visibility.Collapsed;
            GenerationSceneRight.Visibility = Visibility.Collapsed;
            BasicWindow.ColumnDefinitions[1].Width = new GridLength(0);
        }

        #endregion

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
            view.RefreshConfigurations();
        }

        internal void AddSelectedTangibleObject()
        {
            var selectedItem = ((TOTreeTangibleObject)this.TangibleObjectsView.ToListView.SelectedItem).TangibleObject;
            // Copy current configuration of timepoint so it can be changed.
            
            // Do this times the amount of scenes will be drawn, which for now, is 2. The gpu results are null for now
            this.WIPandGenerationDict[this.SelectedTimePoint.Configuration.Copy()] = null;
            this.WIPandGenerationDict[this.SelectedTimePoint.Configuration.Copy()] = null;

            // Determine instance to add:
            var InstanceOfObjectToAdd = new EntikaInstance(selectedItem);

            foreach (var config in WIPandGenerationDict.Keys)
            {
                // Use relationship selection solver to figure out relationships to add
                RelationshipSelectionAndInstancingViewModel riVM = new RelationshipSelectionAndInstancingViewModel();
                riVM.Load(this.SelectedTimePoint, InstanceOfObjectToAdd, config.InstancedObjects, this.SelectedTimePoint.GetRemainingPredicates());
                RelationshipSelectionAndInstancingSolver.GetRandomRelationships(riVM);

                // Use relationship instance solver to figure out instances of each relationship

                // Save instance and relationships to WIPconfiguration
                var onRelationshipMultipleVM = riVM.OnRelationshipsMultiple.Where(or => or.Selected).FirstOrDefault();
                var onRelationshipSingleVM = riVM.OnRelationshipsSingle.Where(or => or.Selected).FirstOrDefault();
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

                config.InstancedRelations.Add(onRelationshipInstance);

                // Parse other relationships
                // Single
                foreach (var otherRelationVM in riVM.OtherRelationshipsSingle.Where(or => or.Selected).ToList())
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

                    config.InstancedRelations.Add(otherRelationshipInstance);
                }

                // Multiple
                foreach (var otherRelationVM in riVM.OtherRelationshipsMultiple.Where(or => or.Selected).ToList())
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

                    config.InstancedRelations.Add(otherRelationshipInstance);

                }

                config.InstancedObjects.Add(InstanceOfObjectToAdd);

                if (riVM.OnRelationshipsMultiple.Count == 0 && riVM.OnRelationshipsSingle.Count == 0 && riVM.OnRelationshipsNone.Count == 0)
                    throw new Exception("No on relationship at all");
                RelationshipSelectionAndInstancingView.DataContext = riVM;
                if (riVM.OnRelationshipsMultiple.Count > 0)
                    RelationshipSelectionAndInstancingView.OnRelationshipsMultipleListView.SelectedIndex = 0;
                if (riVM.OnRelationshipsSingle.Count > 0)
                    RelationshipSelectionAndInstancingView.OnRelationshipsSingleListView.SelectedIndex = 0;
            }

            // Goto GenerateConfigurationsView
            // IntializeGenerateConfigurationsView(this.GenerateConfigurationsView);
        }

        public void ApplyConfiguration()
        {
            // Apply chosen results to timepoint
            var gpuConfig = this.GeneratedConfigurations[this.RightSelectedGPUConfigurationResult];
            //for (int i = 0; i < gpuConfig.Instances.Count; i++)
            //{
            //    var WIPinstance = this.WorkInProgressConfiguration.InstancedObjects.Where(io => io.Equals(gpuConfig.Instances[i].entikaInstance)).FirstOrDefault();
            //    WIPinstance.Position = new Vector3(gpuConfig.Instances[i].Position.X, gpuConfig.Instances[i].Position.Y, gpuConfig.Instances[i].Position.Z);
            //    WIPinstance.Rotation = new Vector3(gpuConfig.Instances[i].Rotation.X, gpuConfig.Instances[i].Rotation.Y, gpuConfig.Instances[i].Rotation.Z);
            //    WIPinstance.UpdateBoundingBoxAndShape();
            //}
            //this.SelectedTimePoint.Configuration = WorkInProgressConfiguration;

            // Reset used variables
            WIPandGenerationDict = new Dictionary<Configuration, GPUConfigurationResult>();
            LeftSelectedGPUConfigurationResult = -1;
            RightSelectedGPUConfigurationResult = -1;
            GeneratedConfigurations = new List<GPUConfigurationResult>();

            // Regenerate predicates based on newly applied configuration
            this.SelectedTimePoint.RegeneratePredicates();
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
            ChangeUIToTOClassSelection();
        }

        private void btnChangeCurrentRegion(object sender, RoutedEventArgs e)
        {
            this.WorkInProgressConfiguration = new Configuration();
            this.WorkInProgressConfiguration = this.SelectedTimePoint.Configuration.Copy();
            IntializeGenerateConfigurationsView(this.GenerateConfigurationsView2);

            ShowGenerationScenes();

            this.CurrentFillingMode = AlternativeFillingMode.Reposition;
            region_filling_1.Visibility = Visibility.Collapsed;
            region_filling_1_content.Visibility = Visibility.Collapsed;
            region_filling_2.Visibility = Visibility.Collapsed;
            region_filling_2_content.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Visible;
            region_filling_3_content.Visibility = Visibility.Visible;
            generation_1.Visibility = Visibility.Visible;
            region_tabcontrol.SelectedIndex = 3;
        }

        internal void ChangeUIToMainMenu()
        {
            // Reset views when coming from TO addition/placement
            generation_1.Visibility = Visibility.Collapsed;
            BasicWindow.ColumnDefinitions[1].Width = new GridLength(0);
            BasicScene.Visibility = Visibility.Visible;
            GenerationSceneLeft.Visibility = Visibility.Collapsed;
            GenerationSceneRight.Visibility = Visibility.Collapsed;

            this.CurrentFillingMode = AlternativeFillingMode.None;
            region_filling_1.Visibility = Visibility.Visible;
            region_filling_1_content.Visibility = Visibility.Visible;
            region_filling_2.Visibility = Visibility.Collapsed;
            region_filling_2_content.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Collapsed;
            region_filling_3_content.Visibility = Visibility.Collapsed;
            region_tabcontrol.SelectedIndex = 0;
        }

        private void btnReturnToRegionCreation_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new RegionCreationPage(selectedNode, this.SelectedTimePoint));
        }

        private void btnGotoBaseFillingMode_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MainModeRegionPage(selectedNode, this.SelectedTimePoint));
        }

        private void btnGotoDebugMode_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new DebugRegionPage(this.selectedNode, this.SelectedTimePoint));
        }

        internal override void GenerateConfigurations()
        {
            WIPandGenerationDict[this.WIPandGenerationDict.Keys.ToList()[0]] = CudaGPUWrapper.CudaGPUWrapperCall(this.SelectedTimePoint, this.WIPandGenerationDict.Keys.ToList()[0])[0];
            // Update list of configurations using generated list of regionpage
            var gcVM = (GenerateConfigurationsViewModel)InspectGeneratedConfigurationsView.DataContext;
            gcVM.Load(GeneratedConfigurations);
            this.InspectGeneratedConfigurationsView.DataContext = gcVM;
        }
    }
}
