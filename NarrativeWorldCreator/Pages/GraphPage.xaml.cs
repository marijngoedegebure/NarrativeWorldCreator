using NarrativeWorlds;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for GraphPage.xaml
    /// </summary>
    public partial class GraphPage : Page
    {
        public Node selectedNode;

        public GraphPage()
        {
            InitializeComponent();
            if (!SystemStateTracker.NarrativeWorld.Graph.nodeCoordinatesGenerated)
                SystemStateTracker.NarrativeWorld.Graph.initForceDirectedGraph();
        }

        private void NarrativeTimelineControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Fill control with stuff
            NarrativeTimelineViewModel narrativeTimelineViewModelObject =
               new NarrativeTimelineViewModel();
            narrativeTimelineViewModelObject.LoadTimePoints();

            NarrativeTimelineControl.DataContext = narrativeTimelineViewModelObject;
        }

        private void GraphDetailTimePointListControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Fill control with stuff
            GraphDetailTimePointListViewModel timePointListViewModelObject =
               new GraphDetailTimePointListViewModel();
            GraphDetailTimePointListControl.DataContext = timePointListViewModelObject;
        }

        private void btnGoToRegionPage_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new RegionPage(selectedNode));
        }

        private void btnReloadGraph_Click(object sender, RoutedEventArgs e)
        {
            // Re-initialize Force directed graph
            SystemStateTracker.NarrativeWorld.Graph.initForceDirectedGraph();
        }

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new InitPage());
        }

        internal void RegionPressed(Node pressed)
        {
            fillDetailView(pressed);
        }

        internal void fillDetailView(Node location)
        {
            this.selectedNode = location;
            // Get NarrativeTimePoints associated with the node
            List<NarrativeTimePoint> narrativeTimePointsOfNode = SystemStateTracker.NarrativeWorld.NarrativeTimeline.getNarrativeTimePointsWithNode(location);
            (GraphDetailTimePointListControl.DataContext as GraphDetailTimePointListViewModel).LoadTimePoints(selectedNode);
            selected_region_detail_grid.Visibility = Visibility.Visible;
        }

        internal void unselectNode()
        {
            // If no collision, reset selectedNode and interface
            this.selected_region_detail_grid.Visibility = Visibility.Collapsed;
            this.selectedNode = null;
        }
    }
}
