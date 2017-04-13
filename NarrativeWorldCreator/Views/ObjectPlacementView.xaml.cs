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
    /// Interaction logic for ObjectPlacementView.xaml
    /// </summary>
    public partial class ObjectPlacementView : UserControl
    {
        public ObjectPlacementView()
        {
            InitializeComponent();
        }

        private void ToEntityAddition(object sender, RoutedEventArgs e)
        {
            var vectorVM = (Vector3ViewModel)this.OptionListView.SelectedItem;
            GetRegionPage().PlaceObjectAndToEntityAddition(vectorVM.OriginalVector);
        }

        private void ObjectPlacementSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var removedItems = e.RemovedItems;
            if (removedItems.Count > 0)
            {
                Vector3ViewModel entikaInstance = removedItems[0] as Vector3ViewModel;
            }
            var addedItems = e.AddedItems;
            if (addedItems.Count > 0)
            {
                Vector3ViewModel vector3VM = addedItems[0] as Vector3ViewModel;
                GetRegionPage().UpdateObjectPosition(vector3VM.OriginalVector);
            }
        }

        private RegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (RegionPage)mainWindow._mainFrame.NavigationService.Content;
        }
    }
}
