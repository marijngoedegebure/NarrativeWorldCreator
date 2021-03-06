﻿using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Pages;
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
    /// Interaction logic for SuggestedConfigurationsView.xaml
    /// </summary>
    public partial class GenerateConfigurationsView : UserControl
    {
        public GenerateConfigurationsView()
        {
            InitializeComponent();
        }

        private ModeBaseRegionPage GetRegionPage()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            return (ModeBaseRegionPage)mainWindow._mainFrame.NavigationService.Content;
        }

        private void btnRefreshConfigurations(object sender, RoutedEventArgs e)
        {
            SaveValues();
            var regionPage = GetRegionPage();
            regionPage.GenerateConfigurations();
        }

        private void SaveValues()
        {
            SystemStateTracker.WeightFocalPoint = (float) this.SliderWeightFocalPoint.Value;
            SystemStateTracker.WeightPairWise = (float) this.SliderWeightPairWise.Value;
            SystemStateTracker.WeightSurfaceArea = (float) this.SliderWeightSurfaceArea.Value;
            SystemStateTracker.WeightClearance = (float) this.SliderWeightClearance.Value;
            SystemStateTracker.WeightOffLimits = (float)this.SliderWeightOffLimits.Value;
            SystemStateTracker.WeightSymmetry = (float)this.SliderWeightSymmetry.Value;
            SystemStateTracker.WeightVisualBalance = (float)this.SliderWeightVisualBalance.Value;

            SystemStateTracker.gridxDim = int.Parse(this.gridxDim.Text);
        }

        private void centroidX_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var value = 0.0f;
            if (float.TryParse(textBox.Text, out value))
            {
                SystemStateTracker.centroidX = value;
            }
        }

        private void centroidY_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var value = 0.0f;
            if (float.TryParse(textBox.Text, out value))
            {
                SystemStateTracker.centroidY = value;
            }
        }

        private void focalX_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var value = 0.0f;
            if (float.TryParse(textBox.Text, out value))
            {
                SystemStateTracker.focalX = value;
            }
        }

        private void focalY_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var value = 0.0f;
            if (float.TryParse(textBox.Text, out value))
            {
                SystemStateTracker.focalY = value;
            }
        }

        private void focalRot_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var value = 0.0f;
            if (float.TryParse(textBox.Text, out value))
            {
                SystemStateTracker.focalRot = value;
            }
        }
    }
}
