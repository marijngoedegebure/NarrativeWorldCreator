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
    /// Interaction logic for ModeView.xaml
    /// </summary>
    public partial class ModeView : UserControl
    {
        public ModeView()
        {
            InitializeComponent();
        }

        private void btnSwitchModeToRegionOutlining(object sender, RoutedEventArgs e)
        {
            region_outlining_3.Visibility = Visibility.Visible;
            region_filling_3.Visibility = Visibility.Collapsed;
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            RegionPage parentPage = (RegionPage)mainWindow._mainFrame.NavigationService.Content;
            parentPage.SwitchModeToRegionCreation();
        }

        private void btnSwitchModeToRegionFilling(object sender, RoutedEventArgs e)
        {
            region_outlining_3.Visibility = Visibility.Collapsed;
            region_filling_3.Visibility = Visibility.Visible;
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            RegionPage parentPage = (RegionPage)mainWindow._mainFrame.NavigationService.Content;
            parentPage.SwitchModeToRegionFilling();
        }
    }
}
