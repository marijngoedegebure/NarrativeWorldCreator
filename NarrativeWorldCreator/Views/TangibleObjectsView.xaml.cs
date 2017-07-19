using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.Metrics;
using NarrativeWorldCreator.Models.Metrics.TOTree;
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
    /// Interaction logic for TangibleObjectsView.xaml
    /// </summary>
    public partial class TangibleObjectsView : UserControl
    {
        public TangibleObjectsView()
        {
            InitializeComponent();
        }

        private void btnAddSelectedObject(object sender, RoutedEventArgs e)
        {
            var selectedItem = (TOTreeTangibleObject) this.ToListView.SelectedItem;

            // Kick off addition process
            var regionPage = GetRegionPage();
            regionPage.Next();
        }

        private void btnBackToMainMenu(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            regionPage.Back();
        }

        private ModeBaseRegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (ModeBaseRegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            RadioButton rb = (RadioButton)sender;
            if (rb.Content.Equals(Constants.All))
            {
                var tovVM = (TangibleObjectsValuedViewModel)this.DataContext;
                tovVM.LoadAll(regionPage.selectedNode, regionPage.selectedNode.TimePoints[regionPage.SelectedTimePoint], regionPage.Configuration);
                this.DataContext = tovVM;
            }
            else if (rb.Content.Equals(Constants.Decoration))
            {
                var tovVM = (TangibleObjectsValuedViewModel)this.DataContext;
                tovVM.LoadDecoration(regionPage.selectedNode, regionPage.selectedNode.TimePoints[regionPage.SelectedTimePoint], regionPage.Configuration);
                this.DataContext = tovVM;
            }
            else if (rb.Content.Equals(Constants.Required))
            {
                var tovVM = (TangibleObjectsValuedViewModel)this.DataContext;
                tovVM.LoadRequired(regionPage.selectedNode, regionPage.selectedNode.TimePoints[regionPage.SelectedTimePoint], regionPage.Configuration);
                this.DataContext = tovVM;
            }
        }
    }
}
