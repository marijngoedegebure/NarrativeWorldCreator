using NarrativeWorldCreator.Models.NarrativeRegionFill;
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
    /// Interaction logic for RemovalModeView.xaml
    /// </summary>
    public partial class RemovalModeView : UserControl
    {
        public RemovalModeView()
        {
            InitializeComponent();
        }

        private MainModeRegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (MainModeRegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private void btnBack(object sender, RoutedEventArgs e)
        {
            GetRegionPage().Back();
        }

        private void btnRemoveSelectedInstances(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            regionPage.RemoveSelectedInstances(regionPage.SelectedEntikaInstances);
            regionPage.RefreshViewsUsingSelected();
        }

        private void btnDeselectAll(object sender, RoutedEventArgs e)
        {
            GetRegionPage().SelectedEntikaInstances = new List<EntikaInstance>();
        }
    }
}
