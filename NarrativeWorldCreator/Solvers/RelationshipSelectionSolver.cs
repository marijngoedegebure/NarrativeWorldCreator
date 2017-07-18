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
        public static List<RelationshipSelectionAndInstancingViewModel> GetRandomRelationships(RelationshipSelectionAndInstancingViewModel riVM, int NumberOfChoices)
        {
            // Set relationships/entika instances to selected when used, they will be extracted using the view model
            var rnd = new Random();
            var list = new List<RelationshipSelectionAndInstancingViewModel>();

            for (int i = 0; i < NumberOfChoices; i++)
            {
                // Make copy of original object
                var riVMCopy = riVM.CreateCopy();

                // First select one on relationship from all on relationships
                var count = riVMCopy.OnRelationshipsSingle.Count + riVMCopy.OnRelationshipsMultiple.Count;
                var index = rnd.Next(0, count);

                if (index < riVMCopy.OnRelationshipsSingle.Count)
                {
                    riVMCopy.OnRelationshipsSingle[index].Selected = true;
                    riVMCopy.OnRelationshipsSingle[index].Focusable = false;
                }
                else
                {
                    riVMCopy.OnRelationshipsMultiple[index - riVMCopy.OnRelationshipsSingle.Count].Selected = true;
                    riVMCopy.OnRelationshipsMultiple[index - riVMCopy.OnRelationshipsSingle.Count].Focusable = false;
                }

                // Set each on relationship to no focusable
                foreach (var rel in riVMCopy.OnRelationshipsSingle)
                {
                    rel.Focusable = false;
                }

                foreach (var rel in riVMCopy.OnRelationshipsMultiple)
                {
                    rel.Focusable = false;
                }

                // Use a 50/50 chance to select an "other" relationship
                for (int j = 0; j < riVMCopy.OtherRelationshipsSingle.Count; j++)
                {
                    // Use a single side of the coin toss
                    if (rnd.Next(2) == 0)
                    {
                        riVMCopy.OtherRelationshipsSingle[j].Selected = true;
                    }
                    riVMCopy.OtherRelationshipsSingle[j].Focusable = false;
                    foreach (var instance in riVMCopy.OtherRelationshipsSingle[j].ObjectInstances)
                    {
                        instance.Focusable = false;
                    }
                }

                for (int j = 0; j < riVMCopy.OtherRelationshipsMultiple.Count; j++)
                {
                    // Use a single side of the coin toss
                    if (rnd.Next(2) == 0)
                    {
                        riVMCopy.OtherRelationshipsMultiple[j].Selected = true;
                    }
                    riVMCopy.OtherRelationshipsMultiple[j].Focusable = false;
                    foreach (var instance in riVMCopy.OtherRelationshipsMultiple[j].ObjectInstances)
                    {
                        instance.Focusable = false;
                    }
                }
                list.Add(riVMCopy);
            }
            return list;
        }
    }
}
