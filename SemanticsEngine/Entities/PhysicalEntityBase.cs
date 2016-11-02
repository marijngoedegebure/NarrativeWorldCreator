/**************************************************************************
 * 
 * PhysicalEntityBase.cs
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

    #region Class: PhysicalEntityBase
    /// <summary>
    /// A base of a physical entity.
    /// </summary>
    public abstract class PhysicalEntityBase : EntityBase
    {

        #region Properties and Fields

        #region Property: PhysicalEntity
        /// <summary>
        /// Gets the physical entity of which this is a physical entity base.
        /// </summary>
        protected internal PhysicalEntity PhysicalEntity
        {
            get
            {
                return this.IdHolder as PhysicalEntity;
            }
        }
        #endregion Property: PhysicalEntity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalEntityBase(PhysicalEntity physicalEntity)
        /// <summary>
        /// Creates a new physical entity base from the given physical entity.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to create the physical entity base from.</param>
        protected PhysicalEntityBase(PhysicalEntity physicalEntity)
            : base(physicalEntity)
        {
        }
        #endregion Constructor: PhysicalEntityBase(PhysicalEntity physicalEntity)

        #region Constructor: PhysicalEntityBase(PhysicalEntityValued physicalEntityValued)
        /// <summary>
        /// Creates a new physical entity base from the given valued physical entity.
        /// </summary>
        /// <param name="physicalEntityValued">The valued physical entity to create the physical entity base from.</param>
        protected PhysicalEntityBase(PhysicalEntityValued physicalEntityValued)
            : base(physicalEntityValued)
        {
        }
        #endregion Constructor: PhysicalEntityBase(PhysicalEntityValued physicalEntityValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: PhysicalEntityBase

    #region Class: PhysicalEntityValuedBase
    /// <summary>
    /// A base of a valued physical entity.
    /// </summary>
    public abstract class PhysicalEntityValuedBase : EntityValuedBase
    {

        #region Properties and Fields

        #region Property: PhysicalEntityValued
        /// <summary>
        /// Gets the valued physical entity of which this is a physical entity base.
        /// </summary>
        protected internal PhysicalEntityValued PhysicalEntityValued
        {
            get
            {
                return this.NodeValued as PhysicalEntityValued;
            }
        }
        #endregion Property: PhysicalEntityValued

        #region Property: PhysicalEntityBase
        /// <summary>
        /// Gets the physical entity of which this is a valued physical entity base.
        /// </summary>
        public PhysicalEntityBase PhysicalEntityBase
        {
            get
            {
                return this.NodeBase as PhysicalEntityBase;
            }
        }
        #endregion Property: PhysicalEntityBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalEntityValuedBase(PhysicalEntityValued physicalEntityValued)
        /// <summary>
        /// Creates a new valued physical entity base from the given valued physical entity.
        /// </summary>
        /// <param name="physicalEntityValued">The valued physical entity to create a valued physical entity base from.</param>
        protected PhysicalEntityValuedBase(PhysicalEntityValued physicalEntityValued)
            : base(physicalEntityValued)
        {
        }
        #endregion Constructor: PhysicalEntityValuedBase(PhysicalEntityValued physicalEntityValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: PhysicalEntityValuedBase

    #region Class: PhysicalEntityConditionBase
    /// <summary>
    /// A condition on a physical entity.
    /// </summary>
    public abstract class PhysicalEntityConditionBase : EntityConditionBase
    {

        #region Properties and Fields

        #region Property: PhysicalEntityCondition
        /// <summary>
        /// Gets the physical entity condition of which this is a physical entity condition base.
        /// </summary>
        protected internal PhysicalEntityCondition PhysicalEntityCondition
        {
            get
            {
                return this.Condition as PhysicalEntityCondition;
            }
        }
        #endregion Property: PhysicalEntityCondition

        #region Property: PhysicalEntityBase
        /// <summary>
        /// Gets the physical entity base of which this is a physical entity condition base.
        /// </summary>
        public PhysicalEntityBase PhysicalEntityBase
        {
            get
            {
                return this.NodeBase as PhysicalEntityBase;
            }
        }
        #endregion Property: PhysicalEntityBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalEntityConditionBase(PhysicalEntityCondition physicalEntityCondition)
        /// <summary>
        /// Creates a base of the given physical entity condition.
        /// </summary>
        /// <param name="physicalEntityCondition">The physical entity condition to create a base of.</param>
        protected PhysicalEntityConditionBase(PhysicalEntityCondition physicalEntityCondition)
            : base(physicalEntityCondition)
        {
        }
        #endregion Constructor: PhysicalEntityConditionBase(PhysicalEntityCondition physicalEntityCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: PhysicalEntityConditionBase

    #region Class: PhysicalEntityChangeBase
    /// <summary>
    /// A change on a physical entity.
    /// </summary>
    public abstract class PhysicalEntityChangeBase : EntityChangeBase
    {

        #region Properties and Fields

        #region Property: PhysicalEntityChange
        /// <summary>
        /// Gets the physical entity change of which this is a physical entity change base.
        /// </summary>
        protected internal PhysicalEntityChange PhysicalEntityChange
        {
            get
            {
                return this.Change as PhysicalEntityChange;
            }
        }
        #endregion Property: PhysicalEntityChange

        #region Property: PhysicalEntityBase
        /// <summary>
        /// Gets the affected physical entity base.
        /// </summary>
        public PhysicalEntityBase PhysicalEntityBase
        {
            get
            {
                return this.NodeBase as PhysicalEntityBase;
            }
        }
        #endregion Property: PhysicalEntityBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalEntityChangeBase(PhysicalEntityChange physicalEntityChange)
        /// <summary>
        /// Creates a base of the given physical entity change.
        /// </summary>
        /// <param name="physicalEntityChange">The physical entity change to create a base of.</param>
        protected PhysicalEntityChangeBase(PhysicalEntityChange physicalEntityChange)
            : base(physicalEntityChange)
        {
        }
        #endregion Constructor: PhysicalEntityChangeBase(PhysicalEntityChange physicalEntityChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: PhysicalEntityChangeBase

}