/**************************************************************************
 * 
 * PredicateTypeBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Semantics.Abstractions;
using Semantics.Components;
using SemanticsEngine.Components;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Abstractions
{

    #region Class: PredicateTypeBase
    /// <summary>
    /// A base of a predicate type.
    /// </summary>
    public class PredicateTypeBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: PredicateType
        /// <summary>
        /// Gets the predicate type of which this is a predicate type base.
        /// </summary>
        protected internal PredicateType PredicateType
        {
            get
            {
                return this.IdHolder as PredicateType;
            }
        }
        #endregion Property: PredicateType

        #region Property: Predicates
        /// <summary>
        /// The predicates of the predicate type.
        /// </summary>
        private PredicateBase[] predicates = null;
        
        /// <summary>
        /// Gets the predicates of the predicate type.
        /// </summary>
        public ReadOnlyCollection<PredicateBase> Predicates
        {
            get
            {
                if (predicates == null)
                {
                    LoadPredicates();
                    if (predicates == null)
                        return new List<PredicateBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PredicateBase>(predicates);
            }
        }

        /// <summary>
        /// Loads the predicates.
        /// </summary>
        private void LoadPredicates()
        {
            if (this.PredicateType != null)
            {
                List<PredicateBase> predicateBases = new List<PredicateBase>();
                foreach (Predicate predicate in this.PredicateType.PersonalPredicates)
                    predicateBases.Add(BaseManager.Current.GetBase<PredicateBase>(predicate));
                predicates = predicateBases.ToArray();
            }
        }
        #endregion Property: Predicates

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PredicateTypeBase(PredicateType predicateType)
        /// <summary>
        /// Create a predicate type base from the given predicate type.
        /// </summary>
        /// <param name="predicateType">The predicate type to create a predicate type base from.</param>
        protected internal PredicateTypeBase(PredicateType predicateType)
            : base(predicateType)
        {
            if (predicateType != null)
            {
                if (BaseManager.PreloadProperties)
                    LoadPredicates();
            }
        }
        #endregion Constructor: PredicateTypeBase(PredicateType predicateType)

        #endregion Method Group: Constructors

    }
    #endregion Class: PredicateTypeBase

}
