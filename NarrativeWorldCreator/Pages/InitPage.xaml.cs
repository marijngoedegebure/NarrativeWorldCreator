using Microsoft.Win32;
using Narratives;
using NarrativeWorlds;
using PDDLNarrativeParser;
using Semantics.Data;
using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NarrativeWorldCreator
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
        public static String basePath = "..\\..\\..\\Narratives\\examples-pddl\\";

        public static String redCapDomainPath = basePath + "red-cap-domain.pddl";
        public static String redCapProblemPath = basePath + "red-cap-problem.pddl";
        public static String redCapPlanPath = basePath + "red-cap-plan.pddl";

        public static String castleDomainPath = basePath + "castle-domain.pddl";
        public static String castleProblemPath = basePath + "castle-problem.pddl";
        public static String castlePlanPath = basePath + "castle-plan.pddl";

        public static String domainPath = redCapDomainPath;
        public static String problemPath = redCapProblemPath;
        public static String planPath = redCapPlanPath;

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
            Narrative narrative = PDDLNarrativeParser.Parser.parse(SystemStateTracker.LocationTypeName, SystemStateTracker.CharacterTypeName, SystemStateTracker.ObjectTypeName, SystemStateTracker.MoveActionName, domainPath, problemPath, planPath);
            NarrativeWorldParser.staticInput(
                SystemStateTracker.LocationTypeName,
                SystemStateTracker.CharacterTypeName,
                SystemStateTracker.ObjectTypeName,
                SystemStateTracker.MoveActionName,
                SystemStateTracker.AtPredicateName);
            SystemStateTracker.NarrativeWorld = NarrativeWorldParser.parse(narrative);
            // Create associations between character/object classes and entika classes based on names
            List<TangibleObject> allTangibleObjects = DatabaseSearch.GetNodes<TangibleObject>(true);
            List<TangibleObject> filteredList = allTangibleObjects.Where(x => x.Children.Count == 0).ToList();
            foreach (TangibleObject to in filteredList)
            {
                foreach (NarrativeCharacter nc in SystemStateTracker.NarrativeWorld.NarrativeCharacters)
                {
                    if (to.Names[0].Equals(nc.Name))
                    {
                        nc.TangibleObject = to;
                        continue;
                    }
                }
                foreach (NarrativeThing nt in SystemStateTracker.NarrativeWorld.NarrativeThings)
                {
                    if (to.Names[0].Equals(nt.Name))
                    {
                        nt.TangibleObject = to;
                        continue;
                    }
                }
            }

            // Show information on loaded narrative
            fillDetailView();
            loaded_narrative_detail_grid.Visibility = Visibility.Visible;
        }

        private void fillDetailView()
        {
            number_narrative_events.Content = SystemStateTracker.NarrativeWorld.Narrative.NarrativeEvents.Count;
            number_narrative_characters.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType(SystemStateTracker.CharacterTypeName).Count;
            number_narrative_locations.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType(SystemStateTracker.LocationTypeName).Count;
            number_narrative_objects.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType(SystemStateTracker.ObjectTypeName).Count;
        }

        private void btnGoToGraphPage_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to graph page
            this.NavigationService.Navigate(new GraphPage());
        }
    }
}
