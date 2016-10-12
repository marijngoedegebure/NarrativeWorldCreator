using Microsoft.Win32;
using NarrativeWorldCreator.PDDL;
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
            Parser.narrative = new Narrative();
            Parser.parseDomain(domainPath);
            Parser.parseProblem(problemPath);
            Parser.parsePlan(planPath);
            // Navigate to graph page
            this.NavigationService.Navigate(new GraphPage());
        }
    }
}
