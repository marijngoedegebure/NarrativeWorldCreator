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
                regionPage.SelectedEntikaInstances.Where(sei => sei.Equals(instance.EntikaInstanceValued.EntikaInstance)).FirstOrDefault().Frozen = true;
                if (regionPage.CurrentFillingMode.Equals(BaseModeRegionPage.FillingMode.Repositioning))
                {
                    regionPage.WorkInProgressConfiguration.InstancedObjects.Where(io => io.Equals(instance.EntikaInstanceValued.EntikaInstance)).FirstOrDefault().Frozen = true;
                    regionPage.GenerateConfigurationsView2.RefreshConfigurations();
                }
                else if (regionPage.CurrentFillingMode.Equals(BaseModeRegionPage.FillingMode.Placement))
                {
                    regionPage.WorkInProgressConfiguration.InstancedObjects.Where(io => io.Equals(instance.EntikaInstanceValued.EntikaInstance)).FirstOrDefault().Frozen = true;
                    regionPage.GenerateConfigurationsView.RefreshConfigurations();
                }
            }
            regionPage.RefreshSelectedObjectView();
        }

        private BaseModeRegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (BaseModeRegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private void btnUnFreeze(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            var data = this.DataContext as SelectedObjectDetailViewModel;
            foreach (var instance in data.SelectedInstancedEntikaInstances)
            {
                regionPage.SelectedEntikaInstances.Where(sei => sei.Equals(instance.EntikaInstanceValued.EntikaInstance)).FirstOrDefault().Frozen = false;
                if (regionPage.CurrentFillingMode.Equals(BaseModeRegionPage.FillingMode.Repositioning))
                {
                    regionPage.WorkInProgressConfiguration.InstancedObjects.Where(io => io.Equals(instance.EntikaInstanceValued.EntikaInstance)).FirstOrDefault().Frozen = true;
                    regionPage.GenerateConfigurationsView2.RefreshConfigurations();
                }
                else if (regionPage.CurrentFillingMode.Equals(BaseModeRegionPage.FillingMode.Placement))
                {
                    regionPage.WorkInProgressConfiguration.InstancedObjects.Where(io => io.Equals(instance.EntikaInstanceValued.EntikaInstance)).FirstOrDefault().Frozen = true;
                    regionPage.GenerateConfigurationsView.RefreshConfigurations();
                }
            }
            regionPage.RefreshSelectedObjectView();
        }

        private void btnRemoveSelectedInstances(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            var data = this.DataContext as SelectedObjectDetailViewModel;
            regionPage.RemoveSelectedInstances(data.SelectedInstancedEntikaInstances.ToList());
            regionPage.RefreshSelectedObjectView();
        }
    }
}
