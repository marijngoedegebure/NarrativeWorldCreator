﻿using NarrativeWorldCreator.Pages;
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
    /// Interaction logic for NarrativeTimelineRegionView.xaml
    /// </summary>
    public partial class NarrativeTimelineRegionView : UserControl
    {
        public NarrativeTimelineRegionView()
        {
            InitializeComponent();
        }

        private void TimeLineListViewItemChanged(object sender, SelectionChangedEventArgs e)
        {
            var removedItems = e.RemovedItems;
            if (removedItems.Count > 0)
            {
                NarrativeTimePointViewModel timePoint = removedItems[0] as NarrativeTimePointViewModel;
                if (timePoint.TimePoint != 0)
                {
                    timePoint.Selected = false;
                }
            }
            foreach (var ntpVM in (this.DataContext as NarrativeTimelineViewModel).NarrativeTimePoints)
            {
                ntpVM.Selected = false;
            }
            // Button button = sender as Button;
            var addedItems = e.AddedItems;
            if (addedItems.Count > 0)
            {
                NarrativeTimePointViewModel timePoint = addedItems[0] as NarrativeTimePointViewModel;
                if (timePoint.TimePoint != 0)
                {
                    timePoint.Selected = true;
                    var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                    var regionPage = (ModeBaseRegionPage)mainWindow._mainFrame.NavigationService.Content;
                        
                    regionPage.SelectedTimePoint = regionPage.selectedNode.TimePoints.IndexOf(timePoint.NarrativeTimePoint);
                    regionPage.UpdateConfiguration();
                    regionPage.UpdateDetailView(timePoint.NarrativeTimePoint);
                    // regionPage.fillNarrativeEntitiesList(timePoint.NarrativeTimePoint);
                    // regionPage.SelectedTimePoint.SwitchTimePoints(regionPage.selectedNode);
                }
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Border scroll_border = VisualTreeHelper.GetChild(lvTimePointsDataBinding, 0) as Border;
            ScrollViewer scroll = scroll_border.Child as ScrollViewer;
            if (e.Delta < 0) // wheel down
            {
                if (scroll.ExtentWidth > scroll.HorizontalOffset + e.Delta)
                {
                    scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset - e.Delta);
                }
                else
                {
                    scroll.ScrollToRightEnd();
                }
            }
            else //wheel up
            {
                if (scroll.HorizontalOffset + e.Delta > 0)
                {
                    scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset - e.Delta);
                }
                else
                {
                    scroll.ScrollToLeftEnd();
                }
            }
        }
    }
}
