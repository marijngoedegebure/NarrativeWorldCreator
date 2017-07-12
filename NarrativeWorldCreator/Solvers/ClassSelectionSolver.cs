using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NarrativeWorldCreator.ViewModel;
using NarrativeWorldCreator.Models.Metrics.TOTree;

namespace NarrativeWorldCreator.Solvers
{
    public static class ClassSelectionSolver
    {
        /// <summary>
        /// Returns list of tangible objects, number is determined by the number of choices variable
        /// </summary>
        /// <param name="toVM"></param>
        /// <param name="NumberOfChoices"></param>
        /// <returns></returns>
        internal static List<TOTreeTangibleObject> GetRandomClasses(TangibleObjectsValuedViewModel toVM, int NumberOfChoices)
        {
            var list = new List<TOTreeTangibleObject>();
            var rnd = new Random();
            for (int i = 0; i < NumberOfChoices; i++)
            {
                var index = rnd.Next(0, toVM.TangibleObjectsValued.Count);
                list.Add(toVM.TangibleObjectsValued[index]);
            }
            return list;
        }
    }
}
