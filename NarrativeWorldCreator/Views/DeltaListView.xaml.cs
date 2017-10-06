using NarrativeWorldCreator.Models.NarrativeRegionFill;
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
    /// Interaction logic for DeltaListView.xaml
    /// </summary>
    public partial class DeltaListView : UserControl
    {
        public DeltaListView()
        {
            InitializeComponent();
        }

        private void btnRemoveSelectedDeltas(object sender, RoutedEventArgs e)
        {
            var parentPage = GetParentPage();
            var instanceDeltasToRemove = new List<InstanceDelta>();
            foreach (InstanceDelta instanceDelta in this.InstanceDeltaListView.SelectedItems)
            {
                instanceDeltasToRemove.Add(instanceDelta);
            }
            var relationshipDeltasToRemove = new List<RelationshipDelta>();
            foreach (RelationshipDelta relationshipDelta in this.RelationshipDeltaListView.SelectedItems)
            {
                relationshipDeltasToRemove.Add(relationshipDelta);
            }
            parentPage.RemoveDeltas(instanceDeltasToRemove, relationshipDeltasToRemove);
            parentPage.UpdateDeltaListView();
        }

        private MainModeRegionPage GetParentPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (MainModeRegionPage)mainWindow._mainFrame.NavigationService.Content;
        }
    }
}
