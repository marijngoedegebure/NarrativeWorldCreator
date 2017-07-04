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
using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeTime;

namespace NarrativeWorldCreator.Pages
{
    /// <summary>
    /// Interaction logic for RegionCreationPage.xaml
    /// </summary>
    public partial class RegionCreationPage : Page
    {
        public LocationNode selectedNode;
        public NarrativeTimePoint selectedTimePoint;

        public RegionCreationPage(LocationNode selectedNode, NarrativeTimePoint selectedTimePoint)
        {
            InitializeComponent();
            this.selectedNode = selectedNode;
            this.selectedTimePoint = selectedTimePoint;
        }

        private void btnGraphPage_Click(object sender, RoutedEventArgs e)
        {
            // Back to graph
            this.NavigationService.Navigate(new GraphPage());
        }

        private void btnRegionPage_Click(object sender, RoutedEventArgs e)
        {
            // Forward to region page
            this.NavigationService.Navigate(new MainModeRegionPage(selectedNode, this.selectedTimePoint));
        }

        private void btnClassSelectionPage_Click(object sender, RoutedEventArgs e)
        {
            // Back to class selection page
            this.NavigationService.Navigate(new ClassSelectionPage(selectedNode, this.selectedTimePoint));
        }

        private void btnResetRegion(object sender, RoutedEventArgs e)
        {

        }
    }
}
