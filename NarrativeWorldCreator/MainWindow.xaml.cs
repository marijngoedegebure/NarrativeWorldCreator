using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using QuickGraph;
using NarrativeWorldCreator.PDDL;

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
        public String domainPath = "C:\\Users\\marij\\Documents\\Master Thesis own folder\\Own application\\NarrativeWorldCreator\\examples-pddl\\red-cap-domain.pddl";
        public String problemPath = "C:\\Users\\marij\\Documents\\Master Thesis own folder\\Own application\\NarrativeWorldCreator\\examples-pddl\\red-cap-problem.pddl";
        public String planPath = "C:\\Users\\marij\\Documents\\Master Thesis own folder\\Own application\\NarrativeWorldCreator\\examples-pddl\\red-cap-plan.pddl";

        private Parser parser = new Parser();

        public MainWindow()
        {
            InitializeComponent();
            var edges = new SEdge<int>[] { new SEdge<int>(1, 2), new SEdge<int>(0, 1) };
            var graph = edges.ToAdjacencyGraph<int, SEdge<int>>(false);
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
            parser.parseDomain(domainPath);
        }
    }
}
