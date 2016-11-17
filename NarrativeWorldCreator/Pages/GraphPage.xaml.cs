using NarrativeWorlds;
using System;
using System.Collections.Generic;
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
        public NarrativeTimePoint selectedTimePoint;

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

        private void TimeLineListViewItemChanged(object sender, SelectionChangedEventArgs e)
        {
            // Button button = sender as Button;
            var addedItems = e.AddedItems;
            if (addedItems.Count > 0)
            {
                NarrativeTimePoint timePoint = addedItems[0] as NarrativeTimePoint;
                if (timePoint.TimePoint != 0)
                {
                    selectedTimePoint = timePoint;
                    fillDetailView(timePoint.Location);
                }
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Border scroll_border = VisualTreeHelper.GetChild(lvTimePointsDataBinding, 0) as Border;
            ScrollViewer scroll = scroll_border.Child as ScrollViewer;
            if (e.Delta < 0) // wheel down
            {
                if (scroll.ExtentWidth > scroll.HorizontalOffset + e.Delta)
                {
                    scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset - e.Delta);
                }
                else
                {
                    scroll.ScrollToRightEnd();
                }
            }
            else //wheel up
            {
                if (scroll.HorizontalOffset + e.Delta > 0)
                {
                    scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset - e.Delta);
                }
                else
                {
                    scroll.ScrollToLeftEnd();
                }
            }
        }

        internal void RegionPressed(Node pressed)
        {
            lvTimePointsDataBinding.SelectedItem = null;
            selectedTimePoint = null;
            fillDetailView(pressed);
        }

        internal void fillDetailView(Node location)
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

        public class NarrativeTimePointViewModel
        {
            private NarrativeTimePoint obj;
            private bool isSelected = false;

            public NarrativeTimePoint NarrativeTimePoint
            {
                get { return this.obj; }
            }

            public bool IsSelected
            {
                get { return this.isSelected; }
                set { this.isSelected = value; }
            }
        }
    }
}
