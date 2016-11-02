/**************************************************************************
 * 
 * FamilyBase.cs
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
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Abstractions
{

    #region Class: FamilyBase
    /// <summary>
    /// A base of a family.
    /// </summary>
    public class FamilyBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: Family
        /// <summary>
        /// Gets the family of which this is a family base.
        /// </summary>
        protected internal Family Family
        {
            get
            {
                return this.IdHolder as Family;
            }
        }
        #endregion Property: Family

        #region Property: Conditions
        /// <summary>
        /// The conditions of the family.
        /// </summary>
        private ConditionBase[] conditions = null;

        /// <summary>
        /// Gets the conditions of the family.
        /// </summary>
        public ReadOnlyCollection<ConditionBase> Conditions
        {
            get
            {
                if (conditions == null)
                {
                    LoadConditions();
                    if (conditions == null)
                        return new List<ConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ConditionBase>(conditions);
            }
        }

        /// <summary>
        /// Loads the conditions.
        /// </summary>
        private void LoadConditions()
        {
            if (this.Family != null)
            {
                List<ConditionBase> familyConditions = new List<ConditionBase>();
                foreach (Condition condition in this.Family.Conditions)
                    familyConditions.Add(BaseManager.Current.GetBase<ConditionBase>(condition));
                conditions = familyConditions.ToArray();
            }
        }
        #endregion Property: Conditions

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: FamilyBase(Family family)
        /// <summary>
        /// Creates a base of a family.
        /// </summary>
        /// <param name="family">The family to create a base of.</param>
        protected internal FamilyBase(Family family)
            : base(family)
        {
            if (family != null)
            {
                if (BaseManager.PreloadProperties)
                    LoadConditions();
            }
        }
        #endregion Constructor: FamilyBase(Family family)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: BelongsToFamily(EntityInstance entityInstance)
        /// <summary>
        /// Checks whether the given entity instance belongs to this family.
        /// </summary>
        /// <param name="entityInstance">The entity instance to check.</param>
        /// <returns>Returns whether the given entity instance belongs to this family.</returns>
        public bool BelongsToFamily(EntityInstance entityInstance)
        {
            if (entityInstance != null)
            {
                foreach (ConditionBase conditionBase in this.Conditions)
                {
                    if (!entityInstance.Satisfies(conditionBase, null))
                        return false;
                }
                return true;
            }
            return false;
        }
        #endregion Method: BelongsToFamily(EntityInstance entityInstance)

        #endregion Method Group: Other

    }
    #endregion Class: FamilyBase

}