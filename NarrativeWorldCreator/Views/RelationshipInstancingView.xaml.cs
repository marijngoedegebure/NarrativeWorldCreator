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
    /// Interaction logic for RelationshipInstancingView.xaml
    /// </summary>
    public partial class RelationshipInstancingView : UserControl
    {
        public RelationshipInstancingView()
        {
            InitializeComponent();
        }

        private MainModeRegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (MainModeRegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private void BackToSelection(object sender, RoutedEventArgs e)
        {
            GetRegionPage().Back();
        }

        private void NextStep(object sender, RoutedEventArgs e)
        {
            var rivm = this.DataContext as RelationshipSelectionAndInstancingViewModel;
            GetRegionPage().Next();
        }

        private void OnRelationMultipleObjectInstanceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var removedItems = e.RemovedItems;
            if (removedItems.Count > 0)
            {
                EntikaInstanceSelectionViewModel entikaInstance = removedItems[0] as EntikaInstanceSelectionViewModel;
                entikaInstance.Selected = false;
            }
            var addedItems = e.AddedItems;
            if (addedItems.Count > 0)
            {
                EntikaInstanceSelectionViewModel entikaInstance = addedItems[0] as EntikaInstanceSelectionViewModel;
                entikaInstance.Selected = true;
            }
        }

        private void OtherRelationMultipleObjectInstanceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var removedItems = e.RemovedItems;
            if (removedItems.Count > 0)
            {
                EntikaInstanceSelectionViewModel entikaInstance = removedItems[0] as EntikaInstanceSelectionViewModel;
                entikaInstance.Selected = false;
            }
            var addedItems = e.AddedItems;
            if (addedItems.Count > 0)
            {
                EntikaInstanceSelectionViewModel entikaInstance = addedItems[0] as EntikaInstanceSelectionViewModel;
                entikaInstance.Selected = true;
            }
        }
    }
}
