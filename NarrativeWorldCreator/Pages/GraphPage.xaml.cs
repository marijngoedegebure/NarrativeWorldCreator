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

        internal void fillDetailView(Node location)
        {
            narrative_location_name.Content = location.LocationName;
            number_narrative_events.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeEventsOfLocation(location.LocationName).Distinct().Count();
            number_narrative_characters.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfTypeOfLocation(SystemStateTracker.CharacterTypeName, location.LocationName).Distinct().Count();
            number_narrative_objects.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfTypeOfLocation(SystemStateTracker.ObjectTypeName, location.LocationName).Distinct().Count();
            selected_region_detail_grid.Visibility = Visibility.Visible;
        }
    }
}
