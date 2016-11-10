using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.RegionGraph;
using Semantics.Abstractions;
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

namespace NarrativeWorldCreator.Pages
{
    /// <summary>
    /// Interaction logic for RegionPage.xaml
    /// </summary>
    public partial class RegionPage : Page
    {
        public Node selectedNode;

        public RegionPage(Node selectedNode)
        {
            InitializeComponent();
            this.selectedNode = selectedNode;
            fillDetailView();
            fillItemList();
            fillGroupCombobox();
        }

        private void fillItemList()
        {
            List<TangibleObject> allTangibleObjects = DatabaseSearch.GetNodes<TangibleObject>(true);
            List<TangibleObject> filteredList = allTangibleObjects.Where(x => x.Children.Count == 0).ToList();
            lvObjectsDataBinding.ItemsSource = filteredList;
        }

        private void fillGroupCombobox()
        {
            List<Group> allGroups = DatabaseSearch.GetNodes<Group>(true);
            lvGroupsDataBinding.ItemsSource = allGroups;
        }

        private void btnReturnToGraph_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GraphPage());
        }

        internal void fillDetailView()
        {
            narrative_location_name.Content = selectedNode.getLocationName();
            number_narrative_events.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeEventsOfLocation(selectedNode.getLocationName()).Distinct().Count();
            number_narrative_characters.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfTypeOfLocation("actor", selectedNode.getLocationName()).Distinct().Count();
            number_narrative_objects.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfTypeOfLocation("thing", selectedNode.getLocationName()).Distinct().Count();
        }
    }
}
