using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using NarrativeWorldCreator.PDDL;
using NarrativeWorldCreator.RegionGraph.GraphDataTypes;
using NarrativeWorldCreator.RegionGraph;
using NarrativeWorldCreator.Pages;

namespace NarrativeWorldCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _mainFrame.Navigate(new InitPage());
        }
    }
}
