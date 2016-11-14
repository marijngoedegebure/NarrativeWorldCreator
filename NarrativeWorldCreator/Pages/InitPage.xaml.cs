﻿using Microsoft.Win32;
using Narratives;
using NarrativeWorlds;
using System;
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
        public String domainPath = "..\\..\\..\\Narratives\\examples-pddl\\red-cap-domain.pddl";
        public String problemPath = "..\\..\\..\\Narratives\\examples-pddl\\red-cap-problem.pddl";
        public String planPath = "..\\..\\..\\Narratives\\examples-pddl\\red-cap-plan.pddl";

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
            SystemStateTracker.NarrativeWorld.Narrative = PDDLNarrativeParser.Parser.parse(SystemStateTracker.LocationTypeName, SystemStateTracker.CharacterTypeName, SystemStateTracker.ObjectTypeName, SystemStateTracker.MoveActionName, domainPath, problemPath, planPath);
            SystemStateTracker.NarrativeWorld.Graph = GraphParser.parse(SystemStateTracker.LocationTypeName, SystemStateTracker.CharacterTypeName, SystemStateTracker.ObjectTypeName, SystemStateTracker.MoveActionName, SystemStateTracker.NarrativeWorld.Narrative);
            // Show information on loaded narrative
            fillDetailView();
            loaded_narrative_detail_grid.Visibility = Visibility.Visible;
        }

        private void fillDetailView()
        {
            number_narrative_events.Content = SystemStateTracker.NarrativeWorld.Narrative.NarrativeEvents.Count;
            number_narrative_characters.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType(GraphParser.CharacterTypeName).Count;
            number_narrative_locations.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType(GraphParser.LocationTypeName).Count;
            number_narrative_objects.Content = SystemStateTracker.NarrativeWorld.Narrative.getNarrativeObjectsOfType(GraphParser.ObjectTypeName).Count;
        }

        private void btnGoToGraphPage_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to graph page
            this.NavigationService.Navigate(new GraphPage());
        }
    }
}
