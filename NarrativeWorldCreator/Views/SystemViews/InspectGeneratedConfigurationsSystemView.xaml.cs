using NarrativeWorldCreator.Pages;
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
    /// Interaction logic for InspectGeneratedConfigurationsSystemView.xaml
    /// </summary>
    public partial class InspectGeneratedConfigurationsSystemView : UserControl
    {
        public InspectGeneratedConfigurationsSystemView()
        {
            InitializeComponent();
        }

        private MainModeRegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (MainModeRegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private void SaveConfiguration(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            if (this.ConfigurationsList.SelectedIndex != -1)
            {
                regionPage.Next();
            }
        }

        private void ShowOnScreen1(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            var selection = this.ConfigurationsList.SelectedIndex;
            regionPage.SelectForScreen1(selection);
        }

        private void ShowOnScreen2(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            var selection = this.ConfigurationsList.SelectedIndex;
            regionPage.SelectForScreen2(selection);
        }

        private void ShowOnScreen3(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            var selection = this.ConfigurationsList.SelectedIndex;
            regionPage.SelectForScreen3(selection);
        }

        private void ShowOnScreen4(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            var selection = this.ConfigurationsList.SelectedIndex;
            regionPage.SelectForScreen4(selection);
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            regionPage.Back();
        }

        private void RefreshOptions(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            regionPage.GenerateConfigurations();
        }
    }
}
