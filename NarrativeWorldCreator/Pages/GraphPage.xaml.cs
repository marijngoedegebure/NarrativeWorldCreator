using NarrativeWorldCreator.RegionGraph;
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
    /// Interaction logic for GraphPage.xaml
    /// </summary>
    public partial class GraphPage : Page
    {
        public GraphPage()
        {
            InitializeComponent();
            // Temporary trigger for when to trigger the graph generation:
            GraphParser.createGraphBasedOnNarrative();
            GraphParser.initForceDirectedGraph();
            // Temporarily disabled to test force directed graph
            // GraphParser.runForceDirectedGraph();
        }

        private void btnRegionSelection_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new RegionPage());
        }

        private void btnReloadGraph_Click(object sender, RoutedEventArgs e)
        {
            // Re-initialize Force directed graph
            GraphParser.initForceDirectedGraph();
        }

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new InitPage());
        }
    }
}
