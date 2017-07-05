using NarrativeWorldCreator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Solvers
{
    public static class RelationshipSelectionAndInstancingSolver
    {
        // Randomly choose from available relationships. Make sure to atleast select an on relationship
        public static void GetRandomRelationships(RelationshipSelectionAndInstancingViewModel riVM)
        {
            // Set relationships/entika instances to selected when used, they will be extracted using the view model
            Random rnd = new Random();
            // Go through on relationships, select one, select one of the instances it is associated with
            // Determine amount of on relationships available with one or multiple:
            var relationshipCount = riVM.OnRelationshipsSingle.Count + riVM.OnRelationshipsMultiple.Count;
            var selectedRelationshipIndex = rnd.Next(0, relationshipCount -1);
            if (selectedRelationshipIndex < riVM.OnRelationshipsSingle.Count)
            {
                riVM.OnRelationshipsSingle[selectedRelationshipIndex].Selected = true;
                // Since it is a relationship with a single instance, make it selected as well
                riVM.OnRelationshipsSingle[selectedRelationshipIndex].ObjectInstances[0].Selected = true;
            }
            else
            {
                // Correct the index for being second list in line
                riVM.OnRelationshipsMultiple[selectedRelationshipIndex - riVM.OnRelationshipsSingle.Count].Selected = true;

                // Multiple instances available, so select randomly
                var amountOfInstances = riVM.OnRelationshipsMultiple[selectedRelationshipIndex - riVM.OnRelationshipsSingle.Count].ObjectInstances.Count;
                var instanceIndex = rnd.Next(0, amountOfInstances - 1);
                riVM.OnRelationshipsMultiple[selectedRelationshipIndex - riVM.OnRelationshipsSingle.Count].ObjectInstances[instanceIndex].Selected = true;
            }

            // Go through all other relationships and with a 50% chance add it
            foreach (var relationship in riVM.OtherRelationshipsSingle)
            {
                // If so, mark this relationship as selected
                if (rnd.Next(2) == 0)
                {
                    relationship.Selected = true;
                    // Since it is single, also select the instance:
                    relationship.ObjectInstances[0].Selected = true;
                }
            }

            foreach (var relationship in riVM.OtherRelationshipsMultiple)
            {
                // If so, mark this relationship as selected
                if (rnd.Next(2) == 0)
                {
                    relationship.Selected = true;
                    // Since it has multiple instances, randomly select one
                    var amountOfInstances = relationship.ObjectInstances.Count;
                    var instanceIndex = rnd.Next(0, amountOfInstances - 1);
                    relationship.ObjectInstances[instanceIndex].Selected = true;
                }
            }

        }
    }
}
