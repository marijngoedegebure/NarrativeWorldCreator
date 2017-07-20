using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.ViewModel;
using NarrativeWorldCreator.Views;
using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NarrativeWorldCreator.Pages
{
    public class BaseRegionPage : Page
    {
        internal LocationNode selectedNode;

        internal List<EntikaInstance> SelectedEntikaInstances;
        // Index of timepoint based on the list of timepoints a node has
        internal int SelectedTimePoint;

        // Current configuration of a timepoint
        public Configuration Configuration { get; set; }

        internal void ChangeSelectedObject(EntikaInstance ieo)
        {
            if (!this.SelectedEntikaInstances.Contains(ieo))
            {
                this.SelectedEntikaInstances.Add(ieo);
            }
            else
                this.SelectedEntikaInstances.Remove(ieo);

        }

        internal void RemoveDeltas(List<InstanceDelta> instanceDeltasToRemove, List<RelationshipDelta> relationshipDeltasToRemove)
        {
            foreach (var instanceDeltaToRemove in instanceDeltasToRemove)
            {
                this.selectedNode.TimePoints[this.SelectedTimePoint].InstanceDeltas.Remove(instanceDeltaToRemove);
            }

            foreach (var relationshipDeltaToRemove in relationshipDeltasToRemove)
            {
                this.selectedNode.TimePoints[this.SelectedTimePoint].RelationshipDeltas.Remove(relationshipDeltaToRemove);
            }
            UpdateConfiguration();
        }

        internal void RemoveSelectedInstances(List<EntikaInstanceValuedPredicate> instances)
        {
            // Add removal delta's or delete add delta. Second is done when the delta has been added at the current timepoint
            var instanceDeltasToRemove = new List<InstanceDelta>();
            var found = false;
            var instanceRemovalDeltasToAdd = new List<InstanceDelta>();
            foreach (var instance in instances) {
                foreach (var instanceDelta in this.selectedNode.TimePoints[SelectedTimePoint].InstanceDeltas)
                {
                    if (instanceDelta.DT.Equals(InstanceDeltaType.Add) && instanceDelta.RelatedInstance.Equals(instance.EntikaInstanceValued.EntikaInstance) && instanceDelta.TimePoint.Equals(this.SelectedTimePoint))
                    {
                        // Remove delta
                        instanceDeltasToRemove.Add(instanceDelta);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    instanceRemovalDeltasToAdd.Add(new InstanceDelta(this.SelectedTimePoint, instance.EntikaInstanceValued.EntikaInstance, InstanceDeltaType.Remove, null, null));
                }
            }

            // Determine relevant relationships of instance
            var relationsToRemove = new List<RelationshipInstance>();
            foreach (var instanceToRemove in instances)
            {
                foreach (var relation in this.Configuration.InstancedRelations)
                {
                    if (relation.Source.Equals(instanceToRemove.EntikaInstanceValued.EntikaInstance))
                    {
                        relationsToRemove.Add(relation);
                    }
                    else if (relation.Target.Equals(instanceToRemove.EntikaInstanceValued.EntikaInstance))
                    {
                        relationsToRemove.Add(relation);
                    }
                }
            }

            // Determine deltas to remove/add removal deltas\
            found = false;
            var relationDeltasToRemove = new List<RelationshipDelta>();
            var relationshipRemovalDeltasToAdd = new List<RelationshipDelta>();
            foreach (var relationToRemove in relationsToRemove)
            {
                foreach (var relationDelta in this.selectedNode.TimePoints[SelectedTimePoint].RelationshipDeltas)
                {
                    if (relationDelta.DT.Equals(RelationshipDeltaType.Add) && relationDelta.RelatedInstance.Equals(relationToRemove) && relationDelta.TimePoint.Equals(this.SelectedTimePoint))
                    {
                        // Remove delta
                        relationDeltasToRemove.Add(relationDelta);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    relationshipRemovalDeltasToAdd.Add(new RelationshipDelta(this.SelectedTimePoint, relationToRemove, RelationshipDeltaType.Remove));
                }
            }

            // Update delta lists
            // Delete deltas
            foreach (var deltaToRemove in instanceDeltasToRemove)
            {
                this.selectedNode.TimePoints[SelectedTimePoint].InstanceDeltas.Remove(deltaToRemove);
            }
            foreach (var deltaToRemove in relationDeltasToRemove)
            {
                this.selectedNode.TimePoints[SelectedTimePoint].RelationshipDeltas.Remove(deltaToRemove);
            }
            // Add removal deltas
            foreach (var removalDelta in instanceRemovalDeltasToAdd)
            {
                this.selectedNode.TimePoints[SelectedTimePoint].InstanceDeltas.Add(removalDelta);
            }
            foreach (var removalDelta in relationshipRemovalDeltasToAdd)
            {
                this.selectedNode.TimePoints[SelectedTimePoint].RelationshipDeltas.Add(removalDelta);
            }

            // Update configuration
            this.UpdateConfiguration();

            // Return to changing menu
            this.RefreshSelectedObjectView();
            this.selectedNode.TimePoints[SelectedTimePoint].RegeneratePredicates(this.Configuration);
        }

        internal void ConsistencyCheckDeltas()
        {
            // Check whether there are delta's in later timepoints that do change delta's on removed delta's
            // Check whether there are change/removal delta's that relate to instances that were never added
        }

        internal void UpdateConfiguration()
        {
            Configuration = new Configuration();

            // Determine current configuration using deltas
            for (int i = 0; i <= this.SelectedTimePoint; i++)
            {
                var instanceDeltaToRemoveDueToInconsistency = new List<InstanceDelta>();
                foreach (var instanceDelta in this.selectedNode.TimePoints[i].InstanceDeltas)
                {
                    if (instanceDelta.DT.Equals(InstanceDeltaType.Add))
                    {
                        instanceDelta.RelatedInstance.Position = instanceDelta.Position;
                        instanceDelta.RelatedInstance.Rotation = instanceDelta.Rotation;
                        instanceDelta.RelatedInstance.UpdateBoundingBoxAndShape();
                        this.Configuration.InstancedObjects.Add(instanceDelta.RelatedInstance);
                    }
                    else if (instanceDelta.DT.Equals(InstanceDeltaType.Remove))
                    {
                        // If not succesfully removed, it can not be found ergo the removal delta is inconsistent
                        if (!this.Configuration.InstancedObjects.Remove(instanceDelta.RelatedInstance))
                        {
                            instanceDeltaToRemoveDueToInconsistency.Add(instanceDelta);
                        }
                    }
                    else if (instanceDelta.DT.Equals(InstanceDeltaType.Change))
                    {
                        var found = false;
                        foreach (var instance in this.Configuration.InstancedObjects)
                        {
                            if (instance.Equals(instanceDelta.RelatedInstance))
                            {
                                instance.Position = instanceDelta.Position;
                                instance.Rotation = instanceDelta.Rotation;
                                instance.UpdateBoundingBoxAndShape();
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            instanceDeltaToRemoveDueToInconsistency.Add(instanceDelta);
                        }
                    }
                }
                // Remove inconsistent delta's
                foreach (var instanceDelta in instanceDeltaToRemoveDueToInconsistency)
                {
                    this.selectedNode.TimePoints[i].InstanceDeltas.Remove(instanceDelta);
                }


                // Do the same for relationshipdelta's
                var relationshipDeltaToRemoveDueToInconsistency = new List<RelationshipDelta>();
                foreach (var relationshipDelta in this.selectedNode.TimePoints[i].RelationshipDeltas)
                {
                    if (relationshipDelta.DT.Equals(RelationshipDeltaType.Add))
                    {
                        this.Configuration.InstancedRelations.Add(relationshipDelta.RelatedInstance);
                    }
                    else if (relationshipDelta.DT.Equals(RelationshipDeltaType.Remove))
                    {
                        if(!this.Configuration.InstancedRelations.Remove(relationshipDelta.RelatedInstance))
                        {
                            relationshipDeltaToRemoveDueToInconsistency.Add(relationshipDelta);
                        }
                    }
                }
                // Remove inconsistent delta's
                foreach (var relationshipDelta in relationshipDeltaToRemoveDueToInconsistency)
                {
                    this.selectedNode.TimePoints[i].RelationshipDeltas.Remove(relationshipDelta);
                }

            }
        }

        internal virtual void UpdateSelectedObjectDetailView() { }

        internal virtual void RefreshSelectedObjectView() {}
    }
}
