/**************************************************************************
 * 
 * AbstractionBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Abstractions;
using SemanticsEngine.Components;

namespace SemanticsEngine.Abstractions
{

    #region Class: AbstractionBase
    /// <summary>
    /// A base of an abstraction.
    /// </summary>
    public abstract class AbstractionBase : NodeBase
    {

        #region Properties and Fields

        #region Property: Abstraction
        /// <summary>
        /// Gets the abstraction of which this is an abstraction base.
        /// </summary>
        protected internal Abstraction Abstraction
        {
            get
            {
                return this.IdHolder as Abstraction;
            }
        }
        #endregion Property: Abstraction

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractionBase(Abstraction abstraction)
        /// <summary>
        /// Creates a new abstraction base from the given abstraction.
        /// </summary>
        /// <param name="abstraction">The abstraction to create the abstraction base from.</param>
        protected AbstractionBase(Abstraction abstraction)
            : base(abstraction)
        {
        }
        #endregion Constructor: AbstractionBase(Abstraction abstraction)

        #region Constructor: AbstractionBase(AbstractionValued abstractionValued)
        /// <summary>
        /// Creates a new abstraction base from the given valued abstraction.
        /// </summary>
        /// <param name="abstractionValued">The valued abstraction to create the abstraction base from.</param>
        protected AbstractionBase(AbstractionValued abstractionValued)
            : base(abstractionValued)
        {
        }
        #endregion Constructor: AbstractionBase(AbstractionValued abstractionValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: AbstractionBase

    #region Class: AbstractionValuedBase
    /// <summary>
    /// A base of a valued abstraction.
    /// </summary>
    public abstract class AbstractionValuedBase : NodeValuedBase
    {

        #region Properties and Fields

        #region Property: AbstractionValued
        /// <summary>
        /// Gets the valued abstraction of which this is a valued abstraction base.
        /// </summary>
        protected internal AbstractionValued AbstractionValued
        {
            get
            {
                return this.NodeValued as AbstractionValued;
            }
        }
        #endregion Property: AbstractionValued

        #region Property: AbstractionBase
        /// <summary>
        /// Gets the abstraction of which this is a valued abstraction base.
        /// </summary>
        public AbstractionBase AbstractionBase
        {
            get
            {
                return this.NodeBase as AbstractionBase;
            }
        }
        #endregion Property: AbstractionBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractionValuedBase(AbstractionValued abstractionValued)
        /// <summary>
        /// Create a valued abstraction base from the given valued abstraction.
        /// </summary>
        /// <param name="abstractionValued">The valued abstraction to create a valued abstraction base from.</param>
        protected AbstractionValuedBase(AbstractionValued abstractionValued)
            : base(abstractionValued)
        {
        }
        #endregion Constructor: AbstractionValuedBase(AbstractionValued abstractionValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: AbstractionValuedBase

    #region Class: AbstractionConditionBase
    /// <summary>
    /// A condition on an abstraction.
    /// </summary>
    public abstract class AbstractionConditionBase : NodeConditionBase
    {

        #region Properties and Fields

        #region Property: AbstractionCondition
        /// <summary>
        /// Gets the abstraction condition of which this is an abstraction condition base.
        /// </summary>
        protected internal AbstractionCondition AbstractionCondition
        {
            get
            {
                return this.Condition as AbstractionCondition;
            }
        }
        #endregion Property: AbstractionCondition

        #region Property: AbstractionBase
        /// <summary>
        /// Gets the abstraction base of which this is an abstraction condition base.
        /// </summary>
        public AbstractionBase AbstractionBase
        {
            get
            {
                return this.NodeBase as AbstractionBase;
            }
        }
        #endregion Property: AbstractionBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractionConditionBase(AbstractionCondition abstractionCondition)
        /// <summary>
        /// Creates a base of the given abstraction condition.
        /// </summary>
        /// <param name="abstractionCondition">The abstraction condition to create a base of.</param>
        protected AbstractionConditionBase(AbstractionCondition abstractionCondition)
            : base(abstractionCondition)
        {
        }
        #endregion Constructor: AbstractionConditionBase(AbstractionCondition abstractionCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: AbstractionConditionBase

    #region Class: AbstractionChangeBase
    /// <summary>
    /// A change on an abstraction.
    /// </summary>
    public abstract class AbstractionChangeBase : NodeChangeBase
    {

        #region Properties and Fields

        #region Property: AbstractionChange
        /// <summary>
        /// Gets the abstraction change of which this is an abstraction change base.
        /// </summary>
        protected internal AbstractionChange AbstractionChange
        {
            get
            {
                return this.Change as AbstractionChange;
            }
        }
        #endregion Property: AbstractionChange

        #region Property: AbstractionBase
        /// <summary>
        /// Gets the affected abstraction base.
        /// </summary>
        public AbstractionBase AbstractionBase
        {
            get
            {
                return this.NodeBase as AbstractionBase;
            }
        }
        #endregion Property: AbstractionBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractionChangeBase(AbstractionChange abstractionChange)
        /// <summary>
        /// Creates a base of the given abstraction change.
        /// </summary>
        /// <param name="abstractionChange">The abstraction change to create a base of.</param>
        protected AbstractionChangeBase(AbstractionChange abstractionChange)
            : base(abstractionChange)
        {
        }
        #endregion Constructor: AbstractionChangeBase(AbstractionChange abstractionChange)

        #endregion Method Group: Constructors
        
    }
    #endregion Class: AbstractionChangeBase

}