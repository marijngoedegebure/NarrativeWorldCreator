/**************************************************************************
 * 
 * AbstractEntityBase.cs
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
using SemanticsEngine.Components;

namespace SemanticsEngine.Entities
{

    #region Class: AbstractEntityBase
    /// <summary>
    /// A base of an abstract entity.
    /// </summary>
    public class AbstractEntityBase : EntityBase
    {

        #region Properties and Fields

        #region Property: AbstractEntity
        /// <summary>
        /// Gets the abstract entity of which this is an abstract entity base.
        /// </summary>
        protected internal AbstractEntity AbstractEntity
        {
            get
            {
                return this.IdHolder as AbstractEntity;
            }
        }
        #endregion Property: AbstractEntity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractEntityBase(AbstractEntity abstractEntity)
        /// <summary>
        /// Creates a new abstract entity base from the given abstract entity.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to create the abstract entity base from.</param>
        protected internal AbstractEntityBase(AbstractEntity abstractEntity)
            : base(abstractEntity)
        {
        }
        #endregion Constructor: AbstractEntityBase(AbstractEntity abstractEntity)

        #region Constructor: AbstractEntityBase(AbstractEntityValued abstractEntityValued)
        /// <summary>
        /// Creates a new abstract entity base from the given valued abstract entity.
        /// </summary>
        /// <param name="abstractEntityValued">The valued abstract entity to create the abstract entity base from.</param>
        protected internal AbstractEntityBase(AbstractEntityValued abstractEntityValued)
            : base(abstractEntityValued)
        {
        }
        #endregion Constructor: AbstractEntityBase(AbstractEntityValued abstractEntityValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: AbstractEntityBase

    #region Class: AbstractEntityValuedBase
    /// <summary>
    /// A base of a valued abstract entity.
    /// </summary>
    public class AbstractEntityValuedBase : EntityValuedBase
    {

        #region Properties and Fields

        #region Property: AbstractEntityValued
        /// <summary>
        /// Gets the valued abstract entity of which this is an abstract entity base.
        /// </summary>
        protected internal AbstractEntityValued AbstractEntityValued
        {
            get
            {
                return this.NodeValued as AbstractEntityValued;
            }
        }
        #endregion Property: AbstractEntityValued

        #region Property: AbstractEntityBase
        /// <summary>
        /// Gets the abstract entity of which this is a valued abstract entity base.
        /// </summary>
        public AbstractEntityBase AbstractEntityBase
        {
            get
            {
                return this.NodeBase as AbstractEntityBase;
            }
        }
        #endregion Property: AbstractEntityBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractEntityValuedBase(AbstractEntityValued abstractEntityValued)
        /// <summary>
        /// Creates a new valued abstract entity base from the given valued abstract entity.
        /// </summary>
        /// <param name="abstractEntityValued">The valued abstract entity to create a valued abstract entity base from.</param>
        protected internal AbstractEntityValuedBase(AbstractEntityValued abstractEntityValued)
            : base(abstractEntityValued)
        {
        }
        #endregion Constructor: AbstractEntityValuedBase(AbstractEntityValued abstractEntityValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: AbstractEntityValuedBase

    #region Class: AbstractEntityConditionBase
    /// <summary>
    /// A condition on an abstract entity.
    /// </summary>
    public class AbstractEntityConditionBase : EntityConditionBase
    {

        #region Properties and Fields

        #region Property: AbstractEntityCondition
        /// <summary>
        /// Gets the abstract entity condition of which this is an abstract entity condition base.
        /// </summary>
        protected internal AbstractEntityCondition AbstractEntityCondition
        {
            get
            {
                return this.Condition as AbstractEntityCondition;
            }
        }
        #endregion Property: AbstractEntityCondition

        #region Property: AbstractEntityBase
        /// <summary>
        /// Gets the attribute base of which this is an attribute condition base.
        /// </summary>
        public AbstractEntityBase AbstractEntityBase
        {
            get
            {
                return this.NodeBase as AbstractEntityBase;
            }
        }
        #endregion Property: AbstractEntityBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractEntityConditionBase(AbstractEntityCondition abstractEntityCondition)
        /// <summary>
        /// Creates a base of the given abstract entity condition.
        /// </summary>
        /// <param name="abstractEntityCondition">The abstract entity condition to create a base of.</param>
        protected internal AbstractEntityConditionBase(AbstractEntityCondition abstractEntityCondition)
            : base(abstractEntityCondition)
        {
        }
        #endregion Constructor: AbstractEntityConditionBase(AbstractEntityCondition abstractEntityCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: AbstractEntityConditionBase

    #region Class: AbstractEntityChangeBase
    /// <summary>
    /// A change on an abstract entity.
    /// </summary>
    public class AbstractEntityChangeBase : EntityChangeBase
    {

        #region Properties and Fields

        #region Property: AbstractEntityChange
        /// <summary>
        /// Gets the abstract entity change of which this is an abstract entity change base.
        /// </summary>
        protected internal AbstractEntityChange AbstractEntityChange
        {
            get
            {
                return this.Change as AbstractEntityChange;
            }
        }
        #endregion Property: AbstractEntityChange

        #region Property: AbstractEntityBase
        /// <summary>
        /// Gets the affected abstract entity base.
        /// </summary>
        public AbstractEntityBase AbstractEntityBase
        {
            get
            {
                return this.NodeBase as AbstractEntityBase;
            }
        }
        #endregion Property: AbstractEntityBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractEntityChangeBase(AbstractEntityChange abstractEntityChange)
        /// <summary>
        /// Creates a base of the given abstract entity change.
        /// </summary>
        /// <param name="abstractEntityChange">The abstract entity change to create a base of.</param>
        protected internal AbstractEntityChangeBase(AbstractEntityChange abstractEntityChange)
            : base(abstractEntityChange)
        {
        }
        #endregion Constructor: AbstractEntityChangeBase(AbstractEntityChange abstractEntityChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: AbstractEntityChangeBase

}