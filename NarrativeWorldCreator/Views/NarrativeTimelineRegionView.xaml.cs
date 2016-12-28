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
            // Button button = sender as Button;
            var addedItems = e.AddedItems;
            if (addedItems.Count > 0)
            {
                NarrativeTimePointViewModel timePoint = addedItems[0] as NarrativeTimePointViewModel;
                if (timePoint.TimePoint != 0)
                {
                    timePoint.Selected = true;
                    var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                    if (mainWindow._mainFrame.NavigationService.Content.GetType().Equals(typeof(GraphPage)))
                    {
                        var graphPage = (GraphPage)mainWindow._mainFrame.NavigationService.Content;
                        graphPage.fillDetailView(timePoint.NarrativeTimePoint.Location);
                    }
                    else
                    {
                        var regionPage = (RegionPage)mainWindow._mainFrame.NavigationService.Content;
                        regionPage.fillDetailView(timePoint.NarrativeTimePoint);
                    }
                }
            }
            var removedItems = e.RemovedItems;
            if (removedItems.Count > 0)
            {
                NarrativeTimePointViewModel timePoint = removedItems[0] as NarrativeTimePointViewModel;
                if (timePoint.TimePoint != 0)
                {
                    timePoint.Selected = false;
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
