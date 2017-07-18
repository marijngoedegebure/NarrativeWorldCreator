using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NarrativeWorldCreator.ViewModel;

namespace NarrativeWorldCreator.Solvers
{
    public static class RelationshipInstancingSolver
    {
        // Given set of relationships with more than one option of instancing, randomly select an instance
        internal static List<RelationshipSelectionAndInstancingViewModel> GetRandomInstances(RelationshipSelectionAndInstancingViewModel riVM, int NumberOfChoices)
        {
            var rnd = new Random();
            var list = new List<RelationshipSelectionAndInstancingViewModel>();

            for (int i = 0; i < NumberOfChoices; i++)
            {
                var riVMCopy = riVM.CreateCopy();

                foreach (var rel in riVMCopy.OnRelationshipsMultiple)
                {
                    var index = rnd.Next(0, rel.ObjectInstances.Count);
                    rel.ObjectInstances[index].Selected = true;
                    foreach (var instance in rel.ObjectInstances)
                    {
                        instance.Focusable = false;
                    }
                }

                foreach (var rel in riVMCopy.OtherRelationshipsMultiple)
                {
                    var index = rnd.Next(0, rel.ObjectInstances.Count);
                    rel.ObjectInstances[index].Selected = true;
                    foreach (var instance in rel.ObjectInstances)
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
