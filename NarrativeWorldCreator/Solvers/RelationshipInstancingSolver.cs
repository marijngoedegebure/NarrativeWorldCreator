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
        internal static void GetRandomInstances(RelationshipSelectionAndInstancingViewModel riVM)
        {
            var rnd = new Random();
            foreach (var rel in riVM.OnRelationshipsMultiple)
            {
                var index = rnd.Next(0, rel.ObjectInstances.Count);
                rel.ObjectInstances[index].Selected = true;
                foreach (var instance in rel.ObjectInstances)
                {
                    instance.Focusable = false;
                }
            }

            foreach (var rel in riVM.OtherRelationshipsMultiple)
            {
                var index = rnd.Next(0, rel.ObjectInstances.Count);
                rel.ObjectInstances[index].Selected = true;
                foreach (var instance in rel.ObjectInstances)
                {
                    instance.Focusable = false;
                }
            }
        }
    }
}
