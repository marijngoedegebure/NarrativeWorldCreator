/**************************************************************************
 * 
 * SubstanceBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Entities;

namespace SemanticsEngine.Entities
{

    #region Class: SubstanceBase
    /// <summary>
    /// A base of a substance.
    /// </summary>
    public class SubstanceBase : MatterBase
    {

        #region Properties and Fields

        #region Property: Substance
        /// <summary>
        /// Gets the substance of which this is a substance base.
        /// </summary>
        protected internal Substance Substance
        {
            get
            {
                return this.IdHolder as Substance;
            }
        }
        #endregion Property: Substance

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SubstanceBase(Substance substance)
        /// <summary>
        /// Creates a new substance base from the given substance.
        /// </summary>
        /// <param name="substance">The substance to create the substance base from.</param>
        protected internal SubstanceBase(Substance substance)
            : base(substance)
        {
        }
        #endregion Constructor: SubstanceBase(Substance substance)

        #endregion Method Group: Constructors

    }
    #endregion Class: SubstanceBase

    #region Class: SubstanceValuedBase
    /// <summary>
    /// A base of a valued substance.
    /// </summary>
    public class SubstanceValuedBase : MatterValuedBase
    {

        #region Properties and Fields

        #region Property: SubstanceValued
        /// <summary>
        /// Gets the valued substance of which this is a valued substance base.
        /// </summary>
        protected internal SubstanceValued SubstanceValued
        {
            get
            {
                return this.MatterValued as SubstanceValued;
            }
        }
        #endregion Property: SubstanceValued

        #region Property: SubstanceBase
        /// <summary>
        /// Gets the substance base.
        /// </summary>
        public SubstanceBase SubstanceBase
        {
            get
            {
                return this.NodeBase as SubstanceBase;
            }
        }
        #endregion Property: SubstanceBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SubstanceValuedBase(SubstanceValued substanceValued)
        /// <summary>
        /// Create a valued substance base from the given valued substance.
        /// </summary>
        /// <param name="substanceValued">The valued substance to create a valued substance base from.</param>
        protected internal SubstanceValuedBase(SubstanceValued substanceValued)
            : base(substanceValued)
        {
        }
        #endregion Constructor: SubstanceValuedBase(SubstanceValued substanceValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: SubstanceValuedBase

    #region Class: SubstanceConditionBase
    /// <summary>
    /// A condition on a substance.
    /// </summary>
    public class SubstanceConditionBase : MatterConditionBase
    {

        #region Properties and Fields

        #region Property: SubstanceCondition
        /// <summary>
        /// Gets the substance condition of which this is a substance condition base.
        /// </summary>
        protected internal SubstanceCondition SubstanceCondition
        {
            get
            {
                return this.Condition as SubstanceCondition;
            }
        }
        #endregion Property: SubstanceCondition

        #region Property: SubstanceBase
        /// <summary>
        /// Gets the substance base of which this is a substance condition base.
        /// </summary>
        public SubstanceBase SubstanceBase
        {
            get
            {
                return this.NodeBase as SubstanceBase;
            }
        }
        #endregion Property: SubstanceBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SubstanceConditionBase(SubstanceCondition substanceCondition)
        /// <summary>
        /// Creates a base of the given substance condition.
        /// </summary>
        /// <param name="substanceCondition">The substance condition to create a base of.</param>
        protected internal SubstanceConditionBase(SubstanceCondition substanceCondition)
            : base(substanceCondition)
        {
        }
        #endregion Constructor: SubstanceConditionBase(SubstanceCondition substanceCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: SubstanceConditionBase

    #region Class: SubstanceChangeBase
    /// <summary>
    /// A change on a substance.
    /// </summary>
    public class SubstanceChangeBase : MatterChangeBase
    {

        #region Properties and Fields

        #region Property: SubstanceChange
        /// <summary>
        /// Gets the substance change of which this is a substance change base.
        /// </summary>
        protected internal SubstanceChange SubstanceChange
        {
            get
            {
                return this.Change as SubstanceChange;
            }
        }
        #endregion Property: SubstanceChange

        #region Property: SubstanceBase
        /// <summary>
        /// Gets the affected substance base.
        /// </summary>
        public SubstanceBase SubstanceBase
        {
            get
            {
                return this.NodeBase as SubstanceBase;
            }
        }
        #endregion Property: SubstanceBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SubstanceChangeBase(SubstanceChange substanceChange)
        /// <summary>
        /// Creates a base of the given substance change.
        /// </summary>
        /// <param name="substanceChange">The substance change to create a base of.</param>
        protected internal SubstanceChangeBase(SubstanceChange substanceChange)
            : base(substanceChange)
        {
        }
        #endregion Constructor: SubstanceChangeBase(SubstanceChange substanceChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: SubstanceChangeBase

}