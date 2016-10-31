using Microsoft.Win32;
using Narratives;
using NarrativeWorldCreator.Parsers;
using NarrativeWorldCreator.RegionGraph;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NarrativeWorldCreator.Pages
{
    /// <summary>
    /// Interaction logic for InitPage.xaml
    /// </summary>
    public partial class InitPage : Page
    {
        /*
        public String domainPath;
        public String problemPath;
        public String planPath;
        */
        public String domainPath = "..\\..\\..\\examples-pddl\\red-cap-domain.pddl";
        public String problemPath = "..\\..\\..\\examples-pddl\\red-cap-problem.pddl";
        public String planPath = "..\\..\\..\\examples-pddl\\red-cap-plan.pddl";

        public InitPage()
        {
            InitializeComponent();
        }

        private void btnOpenFileProblem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                problemPath = openFileDialog.FileName;
                problem_filename.Content = openFileDialog.SafeFileName;
            }
        }

        private void btnOpenFileDomain_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                domainPath = openFileDialog.FileName;
                domain_filename.Content = openFileDialog.SafeFileName;
            }
        }

        private void btnOpenFilePlan_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                planPath = openFileDialog.FileName;
                plan_filename.Content = openFileDialog.SafeFileName;
            }
        }

        private void btnLoadPDDL_Click(object sender, RoutedEventArgs e)
        {
            SystemStateTracker.NarrativeWorld.Narrative = new Narrative();
            Parser.parseDomain(domainPath);
            Parser.parseProblem(problemPath);
            Parser.parsePlan(planPath);
            // Show information on loaded narrative
            fillDetailView();
            loaded_narrative_detail_grid.Visibility = Visibility.Visible;
            Parser.createGraphBasedOnNarrative();
        }

        private void fillDetailView()
        {
            number_narrative_events.Content = SystemStateTracker.NarrativeWorld.Narrative.NarrativeEvents.Count;
            number_narrative_characters.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType("actor").Count;
            number_narrative_locations.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType("location").Count;
            number_narrative_objects.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType("thing").Count;
        }

        private void btnGoToGraphPage_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to graph page
            this.NavigationService.Navigate(new GraphPage());
        }
    }
}
