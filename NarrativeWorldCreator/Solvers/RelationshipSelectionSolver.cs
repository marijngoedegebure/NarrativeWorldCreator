using NarrativeWorldCreator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Solvers
{
    public static class RelationshipSelectionSolver
    {
        // Randomly choose from available relationships. Make sure to atleast select an on relationship
        public static void GetRandomRelationships(RelationshipSelectionAndInstancingViewModel riVM)
        {
            // Set relationships/entika instances to selected when used, they will be extracted using the view model
            var rnd = new Random();
            // First select one on relationship from all on relationships
            var count = riVM.OnRelationshipsSingle.Count + riVM.OnRelationshipsMultiple.Count;
            var index = rnd.Next(0, count);

            if (index < riVM.OnRelationshipsSingle.Count)
            {
                riVM.OnRelationshipsSingle[index].Selected = true;
                riVM.OnRelationshipsSingle[index].Focusable = false;
            }
            else
            {
                riVM.OnRelationshipsMultiple[index - riVM.OnRelationshipsSingle.Count].Selected = true;
                riVM.OnRelationshipsMultiple[index - riVM.OnRelationshipsSingle.Count].Focusable = false;
            }

            // Set each on relationship to no focusable
            foreach (var rel in riVM.OnRelationshipsSingle)
            {
                rel.Focusable = false;
            }

            foreach (var rel in riVM.OnRelationshipsMultiple)
            {
                rel.Focusable = false;
            }

            // Use a 50/50 chance to select an "other" relationship
            for (int i = 0; i < riVM.OtherRelationshipsSingle.Count; i++)
            {
                // Use a single side of the coin toss
                if (rnd.Next(2) == 0)
                {
                    riVM.OtherRelationshipsSingle[i].Selected = true;
                }
                riVM.OtherRelationshipsSingle[i].Focusable = false;
                foreach (var instance in riVM.OtherRelationshipsSingle[i].ObjectInstances)
                {
                    instance.Focusable = false;
                }
            }

            for (int i = 0; i < riVM.OtherRelationshipsMultiple.Count; i++)
            {
                // Use a single side of the coin toss
                if (rnd.Next(2) == 0)
                {
                    riVM.OtherRelationshipsMultiple[i].Selected = true;
                }
                riVM.OtherRelationshipsMultiple[i].Focusable = false;
                foreach (var instance in riVM.OtherRelationshipsMultiple[i].ObjectInstances)
                {
                    instance.Focusable = false;
                }
            }
        }
    }
}
