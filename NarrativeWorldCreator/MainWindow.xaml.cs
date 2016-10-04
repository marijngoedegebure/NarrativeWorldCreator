using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using QuickGraph;

namespace NarrativeWorldCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public String domain;
        public String problem;

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
                problem = openFileDialog.FileName;
                txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void btnOpenFileDomain_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                domain = openFileDialog.FileName;
                txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void btnLoadPDDL_Click(object sender, RoutedEventArgs e)
        {
            var pd = new PDDLNET.DomainProblem(domain, problem);
            txtEditor.Text = pd.ToString();
        }
    }
}
