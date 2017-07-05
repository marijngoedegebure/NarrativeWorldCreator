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

        private void ConfigurationSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var regionPage = GetRegionPage();
            if (this.ConfigurationsList.SelectedItems.Count > 0)
            {
                regionPage.LeftSelectedGPUConfigurationResult = this.ConfigurationsList.SelectedIndex;
                regionPage.RightSelectedGPUConfigurationResult = this.ConfigurationsList.SelectedIndex;
            }
            else
            {
                regionPage.LeftSelectedGPUConfigurationResult = -1;
                regionPage.RightSelectedGPUConfigurationResult = -1;
            }
        }

        private void SaveConfiguration(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            if (this.ConfigurationsList.SelectedIndex != -1)
            {
                regionPage.Next();
            }
        }
    }
}
