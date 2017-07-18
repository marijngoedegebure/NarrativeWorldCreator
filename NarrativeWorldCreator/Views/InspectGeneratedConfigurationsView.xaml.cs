using NarrativeWorldCreator.Pages;
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
    /// Interaction logic for InspectGeneratedConfigurationsView.xaml
    /// </summary>
    public partial class InspectGeneratedConfigurationsView : UserControl
    {
        public InspectGeneratedConfigurationsView()
        {
            InitializeComponent();
        }

        private ModeBaseRegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (ModeBaseRegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private void ConfigurationSelectionTopLeftChanged(object sender, SelectionChangedEventArgs e)
        {
            var regionPage = GetRegionPage();
            if (this.ConfigurationsListTopLeft.SelectedItems.Count > 0)
            {
                regionPage.TopLeftSelectedGPUConfigurationResult = this.ConfigurationsListTopLeft.SelectedIndex;
            }
            else
            {
                regionPage.TopLeftSelectedGPUConfigurationResult = -1;
            }
        }

        private void ConfigurationSelectionTopRightChanged(object sender, SelectionChangedEventArgs e)
        {
            var regionPage = GetRegionPage();
            if (this.ConfigurationsListTopRight.SelectedItems.Count > 0)
            {
                regionPage.TopRightSelectedGPUConfigurationResult = this.ConfigurationsListTopRight.SelectedIndex;
            }
            else
            {
                regionPage.TopRightSelectedGPUConfigurationResult = -1;
            }
        }

        private void ConfigurationSelectionBottomLeftChanged(object sender, SelectionChangedEventArgs e)
        {
            var regionPage = GetRegionPage();
            if (this.ConfigurationsListBottomLeft.SelectedItems.Count > 0)
            {
                regionPage.BottomLeftSelectedGPUConfigurationResult = this.ConfigurationsListBottomLeft.SelectedIndex;
            }
            else
            {
                regionPage.BottomRightSelectedGPUConfigurationResult = -1;
            }
        }

        private void ConfigurationSelectionBottomRightChanged(object sender, SelectionChangedEventArgs e)
        {
            var regionPage = GetRegionPage();
            if (this.ConfigurationsListBottomRight.SelectedItems.Count > 0)
            {
                regionPage.BottomRightSelectedGPUConfigurationResult = this.ConfigurationsListBottomRight.SelectedIndex;
            }
            else
            {
                regionPage.TopRightSelectedGPUConfigurationResult = -1;
            }
        }

        private void SaveConfiguration(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            if (this.ConfigurationsListTopLeft.SelectedIndex != -1)
            {
                regionPage.Next();
            }
        }
    }
}
