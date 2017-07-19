using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
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

namespace NarrativeWorldCreator.Pages
{
    /// <summary>
    /// Interaction logic for DebugRegionPage.xaml
    /// </summary>
    public partial class DebugRegionPage : ModeBaseRegionPage
    {
        #region Fields
        internal MainFillingMode CurrentFillingMode = MainFillingMode.None;

        internal enum MainFillingMode
        {
            None = 0,
            ClassSelection = 1,
            RelationSelectionAndInstancting = 2,
            Placement = 3,
            Reposition = 4
        }
        #endregion
        #region Setup

        public DebugRegionPage(LocationNode selectedNode, int SelectedTimePoint)
        {
            InitializeComponent();
            this.selectedNode = selectedNode;
            this.SelectedTimePoint = SelectedTimePoint;
            SelectedEntikaInstances = new List<EntikaInstance>();

            UpdateConfiguration();
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

            var ntpVM = (NarrativeTimelineControl.DataContext as NarrativeTimelineViewModel).NarrativeTimePoints.Where(ntp => ntp.NarrativeTimePoint.Equals(this.selectedNode.TimePoints[SelectedTimePoint])).FirstOrDefault();
            ntpVM.Selected = true;
        }

        private void RegionDetailTimePointView_Loaded(object sender, RoutedEventArgs e)
        {
            // Fill control with stuff
            DetailTimePointViewModel timePointViewModelObject =
               new DetailTimePointViewModel();
            timePointViewModelObject.LoadObjects(selectedNode, this.selectedNode.TimePoints[SelectedTimePoint]);
            RegionDetailTimePointView.DataContext = timePointViewModelObject;
        }

        private void SelectedObjectDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            SelectedObjectDetailViewModel selectedObjectViewModelObject =
               new SelectedObjectDetailViewModel();
            selectedObjectViewModelObject.LoadSelectedInstances(this.SelectedEntikaInstances, this.selectedNode.TimePoints[SelectedTimePoint], this.Configuration);
            SelectedObjectDetailView.DataContext = selectedObjectViewModelObject;
        }
        #endregion


        #region UIChanges
        #endregion

        private void btnReturnToRegionCreation_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GetNavigationService(this);
            this.NavigationService.Navigate(new RegionCreationPage(selectedNode));
        }

        private void btnReturnToInit_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ClassSelectionPage(selectedNode));
        }


        private void btnGraphPage_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GraphPage());
        }

        private void btnGotoMainFillingMode_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MainModeRegionPage(selectedNode));
        }

        public void SetMessageBoxText(string message)
        {
            MessageTextBox.Text = message;
        }

        internal override void UpdateSelectedObjectDetailView()
        {
            (SelectedObjectDetailView.DataContext as SelectedObjectDetailViewModel).LoadSelectedInstances(this.SelectedEntikaInstances, this.selectedNode.TimePoints[SelectedTimePoint], this.Configuration);
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

        public void UpdateDetailView(NarrativeTimePoint narrativeTimePoint)
        {
            // Update detailtab
            this.RefreshSelectedObjectView();
            (RegionDetailTimePointView.DataContext as DetailTimePointViewModel).LoadObjects(selectedNode, narrativeTimePoint);
        }
    }
}
