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
            GetRegionPage().ToEntityAddition();
        }

        private RegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (RegionPage)mainWindow._mainFrame.NavigationService.Content;
        }
    }
}
