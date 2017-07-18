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
    /// Interaction logic for RelationshipSelectionSystemView.xaml
    /// </summary>
    public partial class RelationshipSelectionSystemView : UserControl
    {
        public RelationshipSelectionSystemView()
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
    }
}
