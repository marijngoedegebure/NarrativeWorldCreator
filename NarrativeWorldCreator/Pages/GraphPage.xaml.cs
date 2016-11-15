using NarrativeWorlds;
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
            fillListViews();
        }

        private void fillListViews()
        {
            lvTimePointsDataBinding.ItemsSource = (from a in SystemStateTracker.NarrativeWorld.NarrativeTimeline.NarrativeTimePoints orderby a.TimePoint select a).ToList();
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

        private void TimePointButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            NarrativeTimePoint timePoint = button.DataContext as NarrativeTimePoint;
            selectNode(timePoint.Location);
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0) // wheel down
            {
                if (ScrollViewerTimeLine.HorizontalOffset + e.Delta > 0)
                {
                    ScrollViewerTimeLine.ScrollToHorizontalOffset(ScrollViewerTimeLine.HorizontalOffset + e.Delta);
                }
                else
                {
                    ScrollViewerTimeLine.ScrollToLeftEnd();
                }
            }
            else //wheel up
            {
                if (ScrollViewerTimeLine.ExtentWidth > ScrollViewerTimeLine.HorizontalOffset + e.Delta)
                {
                    ScrollViewerTimeLine.ScrollToHorizontalOffset(ScrollViewerTimeLine.HorizontalOffset + e.Delta);
                }
                else
                {
                    ScrollViewerTimeLine.ScrollToRightEnd();
                }
            }
        }

        internal void selectNode(Node location)
        {
            this.selectedNode = location;
            narrative_location_name.Content = location.LocationName;
            number_narrative_events.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeEventsOfLocation(location.LocationName).Distinct().Count();
            number_narrative_characters.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfTypeOfLocation(SystemStateTracker.CharacterTypeName, location.LocationName).Distinct().Count();
            number_narrative_objects.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfTypeOfLocation(SystemStateTracker.ObjectTypeName, location.LocationName).Distinct().Count();
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
