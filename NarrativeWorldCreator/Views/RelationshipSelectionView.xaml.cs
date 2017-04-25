﻿using Semantics.Components;
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
    /// Interaction logic for TangibleObjectAdditionView.xaml
    /// </summary>
    public partial class RelationshipSelectionView : UserControl
    {
        public RelationshipSelectionView()
        {
            InitializeComponent();
            
        }

        private void BackToSelection(object sender, RoutedEventArgs e)
        {
            GetRegionPage().BackToTangibleObjectSelection();
        }

        private void NextStep(object sender, RoutedEventArgs e)
        {
            // Retrieve selected relationships
            System.Collections.IList items = (System.Collections.IList)RelationListView.SelectedItems;
            List<Relationship> relations = items.Cast<Relationship>().ToList();
            GetRegionPage().InstanceSelectedRelationships(relations);
        }

        private RegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (RegionPage)mainWindow._mainFrame.NavigationService.Content;
        }
    }
}