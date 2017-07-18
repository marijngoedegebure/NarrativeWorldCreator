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
        internal NarrativeTimePoint SelectedTimePoint;

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

        internal void RemoveSelectedInstances(List<EntikaInstanceValuedPredicate> instances)
        {
            var relationsToRemove = new List<RelationshipInstance>();
            // Determine relationships to delete
            foreach (var instanceToRemove in instances)
            {
                foreach (var relation in this.SelectedTimePoint.Configuration.InstancedRelations)
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

            // Remove relationships
            foreach (var relationToRemove in relationsToRemove)
            {
                this.SelectedTimePoint.Configuration.InstancedRelations.Remove(relationToRemove);
            }

            // Remove instances
            foreach (var instanceToRemove in instances)
            {
                this.SelectedTimePoint.Configuration.InstancedObjects.Remove(instanceToRemove.EntikaInstanceValued.EntikaInstance);
            }

            // Return to changing menu
            this.RefreshSelectedObjectView();
            this.SelectedTimePoint.RegeneratePredicates();
        }

        internal virtual void UpdateSelectedObjectDetailView() { }

        internal virtual void RefreshSelectedObjectView() {}
    }
}
