using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using NarrativeWorldCreator.PDDL;
using NarrativeWorldCreator.RegionGraph.GraphDataTypes;
using NarrativeWorldCreator.RegionGraph;

namespace NarrativeWorldCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
        public String domainPath;
        public String problemPath;
        public String planPath;
        */
        public String domainPath = "..\\..\\..\\examples-pddl\\red-cap-domain.pddl";
        public String problemPath = "..\\..\\..\\examples-pddl\\red-cap-problem.pddl";
        public String planPath = "..\\..\\..\\examples-pddl\\red-cap-plan.pddl";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFileProblem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                problemPath = openFileDialog.FileName;
            }
        }

        private void btnOpenFileDomain_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                domainPath = openFileDialog.FileName;
            }
        }

        private void btnOpenFilePlan_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                planPath = openFileDialog.FileName;
            }
        }

        private void btnLoadPDDL_Click(object sender, RoutedEventArgs e)
        {
            Parser.narrative = new Narrative();
            Parser.parseDomain(domainPath);
            Parser.parseProblem(problemPath);
            Parser.parsePlan(planPath);
            // Temporary trigger for when to trigger the graph generation:
            GraphParser.createGraphBasedOnNarrative();
            GraphParser.initForceDirectedGraph();
            GraphParser.runForceDirectedGraph();
        }
    }
}
