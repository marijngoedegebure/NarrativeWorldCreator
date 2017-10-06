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
    /// Interaction logic for InspectGeneratedConfigurationsSystemView.xaml
    /// </summary>
    public partial class InspectGeneratedConfigurationsSystemView : UserControl
    {
        public InspectGeneratedConfigurationsSystemView()
        {
            InitializeComponent();
        }

        private ModeBaseRegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (ModeBaseRegionPage)mainWindow._mainFrame.NavigationService.Content;
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

        }

        private void ShowOnScreen2(object sender, RoutedEventArgs e)
        {

        }

        private void ShowOnScreen3(object sender, RoutedEventArgs e)
        {

        }

        private void ShowOnScreen4(object sender, RoutedEventArgs e)
        {

        }

        private void Back(object sender, RoutedEventArgs e)
        {

        }
    }
}
