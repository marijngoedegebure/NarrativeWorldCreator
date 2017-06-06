using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Semantics.Data;
using System.Collections.Generic;
using Semantics.Entities;
using GameSemantics.Data;
using GameSemantics.Components;
using System.Collections.ObjectModel;
using SemanticsEngine.Entities;
using GameSemanticsEngine.Tools;
using GameSemanticsEngine.GameContent;
using Semantics.Components;
using Semantics.Abstractions;
using GameSemanticsEngine.Interfaces;
using SemanticsEngine.Components;
using GameSemanticsEngine.Components;
using NarrativeWorldCreator.Solvers;

namespace NarrativeWorldCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static String redCapDatabase = "redcapV2.edp";
        public static String castleDatabase = "castle.edp";

        public MainWindow()
        {
            InitializeComponent();
            _mainFrame.Navigate(new InitPage());
            GameDatabase.Initialize();
            SystemStateTracker.EntikaPath = "..\\..\\..\\Entika databases\\";

            SystemStateTracker.LoadedFileName = castleDatabase;
            GameDatabase.LoadProject(SystemStateTracker.EntikaPath + SystemStateTracker.LoadedFileName);

            GameSemanticsEngine.GameSemanticsEngine.Initialize();

            // Entika test code
            //List<PhysicalEntity> allPhysicalEntities = DatabaseSearch.GetNodes<PhysicalEntity>(true);
            //List<PhysicalObject> allPhysicalObjects = DatabaseSearch.GetNodes<PhysicalObject>(true);
            //List<TangibleObject> allTangibleObjects = DatabaseSearch.GetNodes<TangibleObject>(true);
            //TangibleObject specificTangibleObject = DatabaseSearch.GetNode<TangibleObject>("couch");
            //List<Space> allSpaces = DatabaseSearch.GetNodes<Space>(true);
            //ReadOnlyCollection<GameObject> gameObjectForFirstPhysicalObject = GameDatabaseSearch.GetGameObjects(specificTangibleObject);

            //TangibleObjectInstance semanticInstance = GameInstanceManager.Current.Create(gameObjectForFirstPhysicalObject[0]);
            //ContentWrapper contentWrapper;
            //Dictionary<ModelInstance, Node> geometry = new Dictionary<ModelInstance, Node>();
            //bool truth = ContentManager.TryGetContentWrapper(semanticInstance, out contentWrapper);
            //if (ContentManager.TryGetContentWrapper(semanticInstance, out contentWrapper))
            //{
            //    foreach (ModelInstance modelInstance in contentWrapper.GetContent<ModelInstance>())
            //        continue;
            //}
        }
    }
}
