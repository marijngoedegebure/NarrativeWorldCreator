using Microsoft.Win32;
using NarrativeWorldCreator.Models.NarrativeInput;
using NarrativeWorldCreator.Parsers;
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

        public static String domainPath = castleDomainPath;
        public static String problemPath = castleProblemPath;
        public static String planPath = castlePlanPath;

        public InitPage()
        {
            InitializeComponent();
        }

        private void LoadPDDL()
        {
            Narrative narrative = Parser.parse(SystemStateTracker.LocationTypeName, SystemStateTracker.CharacterTypeName, SystemStateTracker.ObjectTypeName, SystemStateTracker.MoveActionName, domainPath, problemPath, planPath);
            NarrativeWorldParser.staticInput(
                SystemStateTracker.LocationTypeName,
                SystemStateTracker.CharacterTypeName,
                SystemStateTracker.ObjectTypeName,
                SystemStateTracker.MoveActionName,
                SystemStateTracker.AtPredicateName);
            SystemStateTracker.NarrativeWorld = NarrativeWorldParser.parse(narrative);
            // Show information on loaded narrative
            fillDetailView();
            loaded_narrative_detail_grid.Visibility = Visibility.Visible;
        }

        private void fillDetailView()
        {
            this.narrative_name_content.Content = SystemStateTracker.NarrativeWorld.Narrative.Name;
            number_narrative_events.Content = SystemStateTracker.NarrativeWorld.Narrative.NarrativeEvents.Count;
            number_narrative_characters.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType(SystemStateTracker.CharacterTypeName).Count;
            number_narrative_locations.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType(SystemStateTracker.LocationTypeName).Count;
            number_narrative_objects.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType(SystemStateTracker.ObjectTypeName).Count;
        }

        private void btnGoToGraphPage_Click(object sender, RoutedEventArgs e)
        {
            this.LoadPDDL();
            // Navigate to graph page
            this.NavigationService.Navigate(new GraphPage());
        }

        private void btnLoadNarrative_Click(object sender, RoutedEventArgs e)
        {
            LoadPDDL();
        }
    }
}
