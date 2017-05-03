using NarrativeWorldCreator.ViewModel;
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

namespace NarrativeWorldCreator.Views
{
    /// <summary>
    /// Interaction logic for SelectedObjectDetailView.xaml
    /// </summary>
    public partial class SelectedObjectDetailView : UserControl
    {
        public SelectedObjectDetailView()
        {
            InitializeComponent();
        }

        private void btnFreeze(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            var data = this.DataContext as SelectedObjectDetailViewModel;
            foreach (var instance in data.SelectedInstancedEntikaInstances)
            {
                regionPage.SelectedEntikaInstances.Where(sei => sei.Equals(instance)).FirstOrDefault().Frozen = true;
            }
            (this.DataContext as SelectedObjectDetailViewModel).LoadSelectedInstances(regionPage.SelectedEntikaInstances);
        }

        private RegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (RegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private void btnUnFreeze(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            var data = this.DataContext as SelectedObjectDetailViewModel;
            foreach (var instance in data.SelectedInstancedEntikaInstances)
            {
                regionPage.SelectedEntikaInstances.Where(sei => sei.Equals(instance)).FirstOrDefault().Frozen = false;
            }
            (this.DataContext as SelectedObjectDetailViewModel).LoadSelectedInstances(regionPage.SelectedEntikaInstances);
        }
    }
}
