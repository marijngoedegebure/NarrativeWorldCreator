using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using NarrativeWorldCreator.Pages;
using Semantics.Data;
using System.Collections.Generic;
using Semantics.Entities;

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
            Database.Initialize();
            Database.LoadProject("C://Users//marij//Documents//Master Thesis own folder//Entika test databases//gamesimple.edp");
            var names = Database.Current.SelectAll<String>(LocalizationTables.NodeName);
            Database.Current.SelectAll<String>(3, LocalizationTables.NodeName, Columns.Name);
            List<PhysicalEntity> allPhysicalEntities = DatabaseSearch.GetNodes<PhysicalEntity>(true);
            List<PhysicalObject> allPhysicalObjects = DatabaseSearch.GetNodes<PhysicalObject>(true);
        }
    }
}
