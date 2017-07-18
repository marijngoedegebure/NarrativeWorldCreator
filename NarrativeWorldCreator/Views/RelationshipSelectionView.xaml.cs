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
    /// Interaction logic for RelationshipSelectionAndInstancingView.xaml
    /// </summary>
    public partial class RelationshipSelectionView : UserControl
    {
        public RelationshipSelectionView()
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
            GetRegionPage().Next();
        }

        private void OnRelationshipsMultipleListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var removedItems = e.RemovedItems;
            if (removedItems.Count > 0)
            {
                if (removedItems[0].GetType().Equals(typeof(RelationshipExtendedViewModel)))
                {
                    RelationshipExtendedViewModel revm = removedItems[0] as RelationshipExtendedViewModel;
                    revm.Selected = false;
                }
            }
            var addedItems = e.AddedItems;
            if (addedItems.Count > 0)
            {
                if (addedItems[0].GetType().Equals(typeof(RelationshipExtendedViewModel)))
                {
                    RelationshipExtendedViewModel revm = addedItems[0] as RelationshipExtendedViewModel;
                    revm.Selected = true;
                }
            }
        }

        private void OtherRelationshipsMultipleListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var removedItems = e.RemovedItems;
            if (removedItems.Count > 0)
            {
                if (removedItems[0].GetType().Equals(typeof(RelationshipExtendedViewModel)))
                {
                    RelationshipExtendedViewModel revm = removedItems[0] as RelationshipExtendedViewModel;
                    revm.Selected = false;
                }
            }
            var addedItems = e.AddedItems;
            if (addedItems.Count > 0)
            {
                if (addedItems[0].GetType().Equals(typeof(RelationshipExtendedViewModel)))
                {
                    RelationshipExtendedViewModel revm = addedItems[0] as RelationshipExtendedViewModel;
                    revm.Selected = true;
                }
            }
        }

        private void OnRelationshipsSingleListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var removedItems = e.RemovedItems;
            if (removedItems.Count > 0)
            {
                if (removedItems[0].GetType().Equals(typeof(RelationshipExtendedViewModel)))
                {
                    RelationshipExtendedViewModel revm = removedItems[0] as RelationshipExtendedViewModel;
                    revm.Selected = false;
                }
            }
            var addedItems = e.AddedItems;
            if (addedItems.Count > 0)
            {
                if (addedItems[0].GetType().Equals(typeof(RelationshipExtendedViewModel)))
                {
                    RelationshipExtendedViewModel revm = addedItems[0] as RelationshipExtendedViewModel;
                    revm.Selected = true;
                }
            }
        }

        private void OtherRelationshipsSingleListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var removedItems = e.RemovedItems;
            if (removedItems.Count > 0)
            {
                if (removedItems[0].GetType().Equals(typeof(RelationshipExtendedViewModel)))
                {
                    RelationshipExtendedViewModel revm = removedItems[0] as RelationshipExtendedViewModel;
                    revm.Selected = false;
                }
            }
            var addedItems = e.AddedItems;
            if (addedItems.Count > 0)
            {
                if (addedItems[0].GetType().Equals(typeof(RelationshipExtendedViewModel)))
                {
                    RelationshipExtendedViewModel revm = addedItems[0] as RelationshipExtendedViewModel;
                    revm.Selected = true;
                }
            }
        }
    }
}
