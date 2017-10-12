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
using NarrativeWorldCreator.Models.NarrativeRegionFill;

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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Close tracker
            SystemStateTracker.TotalTimeSpent = DateTime.Now.Ticks - SystemStateTracker.Start.Ticks;

            // Extract usage data from all the locations/timepoints
            if (SystemStateTracker.NarrativeWorld.Graph != null || SystemStateTracker.NarrativeWorld.Narrative != null || SystemStateTracker.NarrativeWorld.NarrativeTimeline != null)
            {
                var nodes = SystemStateTracker.NarrativeWorld.Graph.getNodeList();
                foreach (var node in nodes)
                {
                    foreach (var timepoint in node.TimePoints)
                    {
                        foreach (var id in timepoint.InstanceDeltas)
                        {
                            if (id.DT.Equals(InstanceDeltaType.Add))
                            {
                                if (SystemStateTracker.TimesATangibleObjectIsUsed.ContainsKey(id.RelatedInstance.TangibleObject))
                                    SystemStateTracker.TimesATangibleObjectIsUsed[id.RelatedInstance.TangibleObject]++;
                                else
                                    SystemStateTracker.TimesATangibleObjectIsUsed[id.RelatedInstance.TangibleObject] = 1;

                                var tupleKey = new Tuple<string, TangibleObject>(node.LocationName, id.RelatedInstance.TangibleObject);
                                if (SystemStateTracker.UsageOfTangibleObjectsPerLocation.ContainsKey(tupleKey))
                                    SystemStateTracker.UsageOfTangibleObjectsPerLocation[tupleKey]++;
                                else
                                    SystemStateTracker.UsageOfTangibleObjectsPerLocation[tupleKey] = 1;
                            }
                        }

                        foreach (var rel in timepoint.RelationshipDeltas)
                        {
                            if (rel.DT.Equals(RelationshipDeltaType.Add))
                            {
                                if (SystemStateTracker.TimesARelationshipIsUsed.ContainsKey(rel.RelatedInstance.BaseRelationship))
                                    SystemStateTracker.TimesARelationshipIsUsed[rel.RelatedInstance.BaseRelationship]++;
                                else
                                    SystemStateTracker.TimesARelationshipIsUsed[rel.RelatedInstance.BaseRelationship] = 1;

                                var tupleKey = new Tuple<string, Relationship>(node.LocationName, rel.RelatedInstance.BaseRelationship);
                                if (SystemStateTracker.UsageOfRelationshipsPerLocation.ContainsKey(tupleKey))
                                    SystemStateTracker.UsageOfRelationshipsPerLocation[tupleKey]++;
                                else
                                    SystemStateTracker.UsageOfRelationshipsPerLocation[tupleKey] = 1;
                            }
                        }
                    }
                }

                // Save all values to text file
                using (StreamWriter file = new StreamWriter(
                    "testresults-" +
                    DateTime.Now.Year + "-" +
                    DateTime.Now.Month + "-" +
                    DateTime.Now.Day + "-" +
                    DateTime.Now.Hour + "-"
                    + DateTime.Now.Minute +
                    ".txt"))
                {
                    // Write interface usage
                    file.WriteLine("Interface usage;");
                    file.WriteLine("TotalNumberOfAddActions;{0}", SystemStateTracker.TotalNumberOfAddActions);
                    file.WriteLine("TotalNumberOfTotalChangeActions;{0}", SystemStateTracker.TotalNumberOfTotalChangeActions);
                    file.WriteLine("TotalNumberOfManualChangeActions;{0}", SystemStateTracker.TotalNumberOfManualChangeActions);
                    file.WriteLine("TotalNumberOfAutomatedChangeActions;{0}", SystemStateTracker.TotalNumberOfAutomatedChangeActions);
                    file.WriteLine("TotalNumberOfRemoveActions;{0}", SystemStateTracker.TotalNumberOfRemoveActions);

                    // Write time spent
                    file.WriteLine("Time spent;");
                    file.WriteLine("TotalTimeSpent;{0}", SystemStateTracker.TotalTimeSpent);
                    file.WriteLine("TimeSpentTotalPerLocation;");
                    foreach (var entry in SystemStateTracker.TimeSpentTotalPerLocation)
                        file.WriteLine("{0};{1}", entry.Key, entry.Value);

                    file.WriteLine("TimeSpentOnActionsPerLocation;");
                    file.WriteLine("Location;TimeSpentOnAddActions;TimeSpentOnAllChangeActions;TimeSpentOnManualChangeActions;TimeSpentOnAutomatedChangeActions;TimeSpentOnRemoveActions;");
                    foreach (var entry in SystemStateTracker.TimeSpentOnActionsPerLocation)
                        file.WriteLine("{0};{1};{2};{3};{4};{5}",
                            entry.Key,
                            entry.Value.TimeSpentOnAddActions,
                            entry.Value.TimeSpentOnAllChangeActions,
                            entry.Value.TimeSpentOnManualChangeActions,
                            entry.Value.TimeSpentOnAutomatedChangeActions,
                            entry.Value.TimeSpentOnRemoveActions);

                    // Write semantic usage
                    file.WriteLine("Semantic usage;");
                    file.WriteLine("TimesATangibleObjectIsUsed;");
                    foreach (var entry in SystemStateTracker.TimesATangibleObjectIsUsed)
                        file.WriteLine("{0};{1}", entry.Key, entry.Value);
                    file.WriteLine("TimesARelationshipIsUsed;");
                    foreach (var entry in SystemStateTracker.TimesARelationshipIsUsed)
                        file.WriteLine("{0};{1}", entry.Key, entry.Value);

                    file.WriteLine("UsageOfTangibleObjectsPerLocation;");
                    foreach (var entry in SystemStateTracker.UsageOfTangibleObjectsPerLocation)
                        file.WriteLine("{0};{1};{2}", entry.Key.Item1, entry.Key.Item2, entry.Value);
                    file.WriteLine("UsageOfRelationshipsPerLocation;");
                    foreach (var entry in SystemStateTracker.UsageOfRelationshipsPerLocation)
                        file.WriteLine("{0};{1};{2}", entry.Key.Item1, entry.Key.Item2, entry.Value);
                }
            }
            base.OnClosing(e);
        }
    }
}
