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
    /// Interaction logic for InstanceSelectedRelationshipsView.xaml
    /// </summary>
    public partial class RelationInstancingView : UserControl
    {
        public RelationInstancingView()
        {
            InitializeComponent();
        }

        private void BackToRelationSelection(object sender, RoutedEventArgs e)
        {
            GetRegionPage().BackToRelationshipSelection();
        }

        private void NextStep(object sender, RoutedEventArgs e)
        {
            // Retrieve selected entika instance for each relation
            var rivm = (RelationInstancingViewModel)RelationInstanceListView.DataContext;
            GetRegionPage().SaveInstancingOfRelationsAndGotoPlacement(rivm);
            //System.Collections.IList items = (System.Collections.IList)RelationInstanceListView.SelectedItems;
        }

        private RegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (RegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private void RelationInstanceSelectionChanged(object sender, SelectionChangedEventArgs e)
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
