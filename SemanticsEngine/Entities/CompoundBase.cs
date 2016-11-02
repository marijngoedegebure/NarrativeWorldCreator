/**************************************************************************
 * 
 * CompoundBase.cs
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
using Semantics.Entities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Entities
{

    #region Class: CompoundBase
    /// <summary>
    /// A base of a compound.
    /// </summary>
    public class CompoundBase : MatterBase
    {

        #region Properties and Fields

        #region Property: Compound
        /// <summary>
        /// Gets the compound of which this is a compound base.
        /// </summary>
        protected internal Compound Compound
        {
            get
            {
                return this.IdHolder as Compound;
            }
        }
        #endregion Property: Compound

        #region Property: Substances
        /// <summary>
        /// All the substances that this compound is made of.
        /// </summary>
        private SubstanceValuedBase[] substances = null;

        /// <summary>
        /// Gets all the substances that this compound is made of.
        /// </summary>
        public ReadOnlyCollection<SubstanceValuedBase> Substances
        {
            get
            {
                if (substances == null)
                {
                    LoadSubstances();
                    if (substances == null)
                        return new List<SubstanceValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SubstanceValuedBase>(substances);
            }
        }

        /// <summary>
        /// Loads the substances.
        /// </summary>
        private void LoadSubstances()
        {
            if (this.Compound != null)
            {
                List<SubstanceValuedBase> substanceValuedBases = new List<SubstanceValuedBase>();
                foreach (SubstanceValued substanceValued in this.Compound.Substances)
                    substanceValuedBases.Add(BaseManager.Current.GetBase<SubstanceValuedBase>(substanceValued));
                substances = substanceValuedBases.ToArray();
            }
        }
        #endregion Property: Substances

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CompoundBase(Compound compound)
        /// <summary>
        /// Creates a compound base from the given compound.
        /// </summary>
        /// <param name="compound">The compound to create a compound base from.</param>
        protected internal CompoundBase(Compound compound)
            : base(compound)
        {
            if (compound != null)
            {
                if (BaseManager.PreloadProperties)
                    LoadSubstances();
            }
        }
        #endregion Constructor: CompoundBase(Compound compound)

        #endregion Method Group: Constructors

    }
    #endregion Class: CompoundBase

    #region Class: CompoundValuedBase
    /// <summary>
    /// A base of a valued compound.
    /// </summary>
    public class CompoundValuedBase : MatterValuedBase
    {

        #region Properties and Fields

        #region Property: CompoundValued
        /// <summary>
        /// Gets the valued compound of which this is a valued compound base.
        /// </summary>
        protected internal CompoundValued CompoundValued
        {
            get
            {
                return this.NodeValued as CompoundValued;
            }
        }
        #endregion Property: CompoundValued

        #region Property: CompoundBase
        /// <summary>
        /// Gets the compound base of which this is a valued compound base.
        /// </summary>
        public CompoundBase CompoundBase
        {
            get
            {
                return this.NodeBase as CompoundBase;
            }
        }
        #endregion Property: CompoundBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CompoundValuedBase(CompoundValued compoundValued)
        /// <summary>
        /// Create a valued compound base from the given valued compound.
        /// </summary>
        /// <param name="compoundValued">The valued compound to create a valued compound base from.</param>
        protected internal CompoundValuedBase(CompoundValued compoundValued)
            : base(compoundValued)
        {
        }
        #endregion Constructor: CompoundValuedBase(CompoundValued compoundValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: CompoundValuedBase

    #region Class: CompoundConditionBase
    /// <summary>
    /// A condition on a compound.
    /// </summary>
    public class CompoundConditionBase : MatterConditionBase
    {

        #region Properties and Fields

        #region Property: CompoundCondition
        /// <summary>
        /// Gets the compound condition of which this is a compound condition base.
        /// </summary>
        protected internal CompoundCondition CompoundCondition
        {
            get
            {
                return this.Condition as CompoundCondition;
            }
        }
        #endregion Property: CompoundCondition

        #region Property: CompoundBase
        /// <summary>
        /// Gets the compound base of which this is a compound condition base.
        /// </summary>
        public CompoundBase CompoundBase
        {
            get
            {
                return this.NodeBase as CompoundBase;
            }
        }
        #endregion Property: CompoundBase

        #region Property: HasAllMandatorySubstances
        /// <summary>
        /// The value that indicates whether all mandatory substances are required.
        /// </summary>
        private bool? hasAllMandatorySubstances = null;

        /// <summary>
        /// Gets the value that indicates whether all mandatory substances are required.
        /// </summary>
        public bool? HasAllMandatorySubstances
        {
            get
            {
                return hasAllMandatorySubstances;
            }
        }
        #endregion Property: HasAllMandatorySubstances

        #region Property: Substances
        /// <summary>
        /// The required substances.
        /// </summary>
        private SubstanceConditionBase[] substances = null;

        /// <summary>
        /// Gets the required substances.
        /// </summary>
        public ReadOnlyCollection<SubstanceConditionBase> Substances
        {
            get
            {
                if (substances == null)
                {
                    LoadSubstances();
                    if (substances == null)
                        return new List<SubstanceConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SubstanceConditionBase>(substances);
            }
        }

        /// <summary>
        /// Loads the substances.
        /// </summary>
        private void LoadSubstances()
        {
            if (this.CompoundCondition != null)
            {
                List<SubstanceConditionBase> substanceConditionBases = new List<SubstanceConditionBase>();
                foreach (SubstanceCondition substanceCondition in this.CompoundCondition.Substances)
                    substanceConditionBases.Add(BaseManager.Current.GetBase<SubstanceConditionBase>(substanceCondition));
                substances = substanceConditionBases.ToArray();
            }
        }
        #endregion Property: Substances

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CompoundConditionBase(CompoundCondition compoundCondition)
        /// <summary>
        /// Creates a base of the given compound condition.
        /// </summary>
        /// <param name="compoundCondition">The compound condition to create a base of.</param>
        protected internal CompoundConditionBase(CompoundCondition compoundCondition)
            : base(compoundCondition)
        {
            if (compoundCondition != null)
            {
                this.hasAllMandatorySubstances = compoundCondition.HasAllMandatorySubstances;

                if (BaseManager.PreloadProperties)
                    LoadSubstances();
            }
        }
        #endregion Constructor: CompoundConditionBase(CompoundCondition compoundCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: CompoundConditionBase

    #region Class: CompoundChangeBase
    /// <summary>
    /// A change on a compound.
    /// </summary>
    public class CompoundChangeBase : MatterChangeBase
    {

        #region Properties and Fields

        #region Property: CompoundChange
        /// <summary>
        /// Gets the compound change of which this is a compound change base.
        /// </summary>
        protected internal CompoundChange CompoundChange
        {
            get
            {
                return this.Change as CompoundChange;
            }
        }
        #endregion Property: CompoundChange

        #region Property: CompoundBase
        /// <summary>
        /// Gets the affected compound base.
        /// </summary>
        public CompoundBase CompoundBase
        {
            get
            {
                return this.NodeBase as CompoundBase;
            }
        }
        #endregion Property: CompoundBase

        #region Property: Substances
        /// <summary>
        /// The substances to change.
        /// </summary>
        private SubstanceChangeBase[] substances = null;

        /// <summary>
        /// Gets the substances to change.
        /// </summary>
        public ReadOnlyCollection<SubstanceChangeBase> Substances
        {
            get
            {
                if (substances == null)
                {
                    LoadSubstances();
                    if (substances == null)
                        return new List<SubstanceChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SubstanceChangeBase>(substances);
            }
        }

        /// <summary>
        /// Loads the substances.
        /// </summary>
        private void LoadSubstances()
        {
            if (this.CompoundChange != null)
            {
                List<SubstanceChangeBase> substanceChangeBases = new List<SubstanceChangeBase>();
                foreach (SubstanceChange substanceChange in this.CompoundChange.Substances)
                    substanceChangeBases.Add(BaseManager.Current.GetBase<SubstanceChangeBase>(substanceChange));
                substances = substanceChangeBases.ToArray();
            }
        }
        #endregion Property: Substances

        #region Property: SubstancesToAdd
        /// <summary>
        /// The substances that should be added during the change.
        /// </summary>
        private SubstanceValuedBase[] substancesToAdd = null;

        /// <summary>
        /// Gets the substances that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<SubstanceValuedBase> SubstancesToAdd
        {
            get
            {
                if (substancesToAdd == null)
                {
                    LoadSubstancesToAdd();
                    if (substancesToAdd == null)
                        return new List<SubstanceValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SubstanceValuedBase>(substancesToAdd);
            }
        }

        /// <summary>
        /// Loads the substances to add.
        /// </summary>
        private void LoadSubstancesToAdd()
        {
            if (this.CompoundChange != null)
            {
                List<SubstanceValuedBase> substanceValuedBases = new List<SubstanceValuedBase>();
                foreach (SubstanceValued substanceValued in this.CompoundChange.SubstancesToAdd)
                    substanceValuedBases.Add(BaseManager.Current.GetBase<SubstanceValuedBase>(substanceValued));
                substancesToAdd = substanceValuedBases.ToArray();
            }
        }
        #endregion Property: SubstancesToAdd

        #region Property: SubstancesToRemove
        /// <summary>
        /// The substances that should be removed during the change.
        /// </summary>
        private SubstanceConditionBase[] substancesToRemove = null;

        /// <summary>
        /// Gets the substances that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<SubstanceConditionBase> SubstancesToRemove
        {
            get
            {
                if (substancesToRemove == null)
                {
                    LoadSubstancesToRemove();
                    if (substancesToRemove == null)
                        return new List<SubstanceConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SubstanceConditionBase>(substancesToRemove);
            }
        }

        /// <summary>
        /// Loads the substances to remove.
        /// </summary>
        private void LoadSubstancesToRemove()
        {
            if (this.CompoundChange != null)
            {
                List<SubstanceConditionBase> substanceConditionBases = new List<SubstanceConditionBase>();
                foreach (SubstanceCondition substanceCondition in this.CompoundChange.SubstancesToRemove)
                    substanceConditionBases.Add(BaseManager.Current.GetBase<SubstanceConditionBase>(substanceCondition));
                substancesToRemove = substanceConditionBases.ToArray();
            }
        }
        #endregion Property: SubstancesToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CompoundChangeBase(CompoundChange compoundChange)
        /// <summary>
        /// Creates a base of the given compound change.
        /// </summary>
        /// <param name="compoundChange">The compound change to create a base of.</param>
        protected internal CompoundChangeBase(CompoundChange compoundChange)
            : base(compoundChange)
        {
            if (compoundChange != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadSubstances();
                    LoadSubstancesToAdd();
                    LoadSubstancesToRemove();
                }
            }
        }
        #endregion Constructor: CompoundChangeBase(CompoundChange compoundChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: CompoundChange

}