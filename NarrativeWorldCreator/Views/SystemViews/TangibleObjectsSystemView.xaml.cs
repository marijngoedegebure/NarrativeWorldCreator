using NarrativeWorldCreator.Models.Metrics.TOTree;
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
    /// Interaction logic for TangibleObjectsSystemView.xaml
    /// </summary>
    public partial class TangibleObjectsSystemView : UserControl
    {
        public TangibleObjectsSystemView()
        {
            InitializeComponent();
        }

        private void btnAddSelectedObject(object sender, RoutedEventArgs e)
        {
            var selectedItem = (TOTreeTangibleObject)this.ToListView.SelectedItem;

            // Kick off addition process
            var regionPage = GetRegionPage();
            //regionPage.AddSelectedTangibleObject(selectedItem.TangibleObject);
        }

        private void btnBackToMainMenu(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            //regionPage.ChangeUIToMainMenu();
        }

        private BaseRegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (BaseRegionPage)mainWindow._mainFrame.NavigationService.Content;
        }
    }
}
