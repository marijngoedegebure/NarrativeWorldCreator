using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.ViewModel;
using Semantics.Data;
using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Solvers
{
    public static class RelationshipSelectionSolver
    {
        internal static AutomatedResultsRelationshipSelectionViewModel GetRandomRelationships(AutomatedResultsRelationshipSelectionViewModel vM, int NumberOfTries, EntikaInstance instanceOfObjectToAdd, LocationNode selectedNode, List<EntikaInstance> instancedObjects)
        {
            var rnd = new Random();
            // This stores the unique solutions
            ObservableCollection<AutomatedRelationshipSelectionViewModel> resultsOC = new ObservableCollection<AutomatedRelationshipSelectionViewModel>();

            for (int i = 0; i < NumberOfTries; i++)
            {
                var arsVM = new AutomatedRelationshipSelectionViewModel();
                arsVM.CurrentInstance = instanceOfObjectToAdd;
                // Figure out on relationship with atleast one instance
                var ListOfInstanceIndicesPerOnRelationship = new Dictionary<int, List<int>>();
                for (int j = 0; j < instanceOfObjectToAdd.TangibleObject.RelationshipsAsTarget.Count; j++)
                {
                    var instanceList = new List<int>();
                    var rel = instanceOfObjectToAdd.TangibleObject.RelationshipsAsTarget[j];
                    if (!selectedNode.AvailableTangibleObjects.Contains(rel.Source) && !rel.Source.Equals(DatabaseSearch.GetNode<TangibleObject>(Constants.Floor)))
                        continue;
                    if (Constants.On.Equals(rel.RelationshipType.DefaultName))
                    {
                        for (var k = 0; k < instancedObjects.Count; k++)
                        {
                            var instance = instancedObjects[k];
                            if (rel.Source.Equals(instance.TangibleObject))
                            {
                                instanceList.Add(k);
                            }
                        }
                        ListOfInstanceIndicesPerOnRelationship[j] = instanceList;
                    }
                }
                // Find random index of keys in the dictionary that points to a relationship index of the tangible object
                var randomRelKeyIndex = rnd.Next(0, ListOfInstanceIndicesPerOnRelationship.Keys.Count);
                var onRelIndex = ListOfInstanceIndicesPerOnRelationship.Keys.ToList()[randomRelKeyIndex];
                // Find random index that points to an instance index
                var randomInstanceListIndex = rnd.Next(0, ListOfInstanceIndicesPerOnRelationship[onRelIndex].Count);
                var instanceIndex = ListOfInstanceIndicesPerOnRelationship[onRelIndex][randomInstanceListIndex];
                var orVM = new OnRelationshipViewModel();
                // Use randomized instance index to find index in instancedObjects list
                orVM.Load(instancedObjects[instanceIndex], instanceOfObjectToAdd.TangibleObject.RelationshipsAsTarget[onRelIndex]);
                arsVM.OnRelationship = orVM;

                // Figure out other relationships
                var OtherRelationshipsOC = new ObservableCollection<OtherRelationshipViewModel>();
                // First as target
                foreach (var rel in instanceOfObjectToAdd.TangibleObject.RelationshipsAsTarget)
                {
                    if (!selectedNode.AvailableTangibleObjects.Contains(rel.Source))
                        continue;
                    if (Constants.OtherRelationshipTypes.Contains(rel.RelationshipType.DefaultName))
                    {
                        var availableInstances = instancedObjects.Where(io => io.TangibleObject.Equals(rel.Source)).ToList();
                        if (availableInstances.Count > 0)
                        {
                            // 50/50 chance of continuing
                            if (rnd.Next(2) == 0)
                            {
                                var index = rnd.Next(0, availableInstances.Count);
                                var otherRVM = new OtherRelationshipViewModel();
                                otherRVM.Load(availableInstances[index], rel, true);
                                OtherRelationshipsOC.Add(otherRVM);
                            }
                        }
                    }
                }

                // Second as source
                foreach (var rel in instanceOfObjectToAdd.TangibleObject.RelationshipsAsSource)
                {
                    if (!selectedNode.AvailableTangibleObjects.Contains(rel.Targets[0]))
                        continue;
                    if (Constants.OtherRelationshipTypes.Contains(rel.RelationshipType.DefaultName))
                    {
                        var availableInstances = instancedObjects.Where(io => io.TangibleObject.Equals(rel.Targets[0])).ToList();
                        if (availableInstances.Count > 0)
                        {
                            // 50/50 chance of continuing
                            if (rnd.Next(2) == 0)
                            {
                                var index = rnd.Next(0, availableInstances.Count);
                                var otherRVM = new OtherRelationshipViewModel();
                                otherRVM.Load(availableInstances[index], rel, true);
                                OtherRelationshipsOC.Add(otherRVM);
                            }
                        }
                    }
                }
                arsVM.OtherRelationships = OtherRelationshipsOC;
                if (!resultsOC.Contains(arsVM))
                    resultsOC.Add(arsVM);
            }
            vM.CurrentInstance = instanceOfObjectToAdd;
            vM.Results = resultsOC;
            return vM;
        }
        
    }
}
