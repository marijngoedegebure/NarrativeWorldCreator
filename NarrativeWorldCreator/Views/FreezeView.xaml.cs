﻿using NarrativeWorldCreator.Models.NarrativeRegionFill;
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
    /// Interaction logic for FreezeView.xaml
    /// </summary>
    public partial class FreezeView : UserControl
    {
        public FreezeView()
        {
            InitializeComponent();
        }

        private void btnFreeze(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            var data = this.DataContext as SimpleSelectionViewModel;
            foreach (var instance in data.SelectedInstancedEntikaInstances)
            {
                regionPage.SelectedEntikaInstances.Where(sei => sei.Equals(instance)).FirstOrDefault().Frozen = true;
            }
            // Freeze items part of work in progress configuration, if it exists
            if (regionPage.WorkInProgressConfiguration != null)
            {
                foreach (var instance in data.SelectedInstancedEntikaInstances)
                {
                    regionPage.WorkInProgressConfiguration.InstancedObjects.Where(sei => sei.Equals(instance)).FirstOrDefault().Frozen = true;
                }
            }
            regionPage.RefreshViewsUsingSelected();
        }

        private MainModeRegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (MainModeRegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private void btnUnFreeze(object sender, RoutedEventArgs e)
        {
            var regionPage = GetRegionPage();
            var data = this.DataContext as SimpleSelectionViewModel;
            foreach (var instance in data.SelectedInstancedEntikaInstances)
            {
                regionPage.SelectedEntikaInstances.Where(sei => sei.Equals(instance)).FirstOrDefault().Frozen = false;
            }
            // Unfreeze items part of work in progress configuration, if it exists
            if (regionPage.WorkInProgressConfiguration != null)
            {
                foreach (var instance in data.SelectedInstancedEntikaInstances)
                {
                    regionPage.WorkInProgressConfiguration.InstancedObjects.Where(sei => sei.Equals(instance)).FirstOrDefault().Frozen = false;
                }
            }
            regionPage.RefreshViewsUsingSelected();
        }

        private void btnBack(object sender, RoutedEventArgs e)
        {
            GetRegionPage().Back();
        }

        private void btnDeselectAll(object sender, RoutedEventArgs e)
        {
            GetRegionPage().SelectedEntikaInstances = new List<EntikaInstance>();
            GetRegionPage().RefreshViewsUsingSelected();
        }
    }
}
