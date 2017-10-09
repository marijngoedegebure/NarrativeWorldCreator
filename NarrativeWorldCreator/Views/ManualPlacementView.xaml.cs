using Microsoft.Xna.Framework;
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
    /// Interaction logic for ManualPlacementView.xaml
    /// </summary>
    public partial class ManualPlacementView : UserControl
    {
        public ManualPlacementView()
        {
            InitializeComponent();
        }


        private MainModeRegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (MainModeRegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            regionPage.Back();
        }

        private void SaveConfiguration(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            regionPage.Next();
        }

        private void ResetPlacement(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            regionPage.manualPlacementPosition = new Vector3();
            regionPage.manualPlacementRotation = new Vector3();
        }
    }
}
