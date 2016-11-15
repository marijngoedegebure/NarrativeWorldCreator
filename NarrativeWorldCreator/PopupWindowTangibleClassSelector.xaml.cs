using Semantics.Data;
using Semantics.Entities;
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
using System.Windows.Shapes;

namespace NarrativeWorldCreator
{
    /// <summary>
    /// Interaction logic for PopupWindowTangibleClassSelector.xaml
    /// </summary>
    public partial class PopupWindowTangibleClassSelector : Window
    {
        public PopupWindowTangibleClassSelector()
        {
            InitializeComponent();
            fillClassSelector();
        }

        private void fillClassSelector()
        {
            List<TangibleObject> allTangibleObjects = DatabaseSearch.GetNodes<TangibleObject>(true);
            List<TangibleObject> filteredList = allTangibleObjects.Where(x => x.Children.Count == 0).ToList();
            lvClassSelectorDataBinding.ItemsSource = filteredList;
        }

        private void TangibleClass_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TangibleObject tangibleObject = button.DataContext as TangibleObject;
            SystemStateTracker.NarrativeWorld.SetTangibleClassNarrativeCharacters(tangibleObject);
            this.Close();
        }
    }
}
