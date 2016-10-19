/**************************************************************************
 * 
 * PhysicalEntity.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using Semantics.Components;

namespace Semantics.Entities
{

    #region Class: PhysicalEntity
    /// <summary>
    /// A physical entity.
    /// </summary>
    public abstract class PhysicalEntity : Entity, IComparable<PhysicalEntity>
    {

        #region Method Group: Constructors

        #region Constructor: PhysicalEntity()
        /// <summary>
        /// Creates a new physical entity.
        /// </summary>
        protected PhysicalEntity()
            : base()
        {
        }
        #endregion Constructor: PhysicalEntity()

        #region Constructor: PhysicalEntity(uint id)
        /// <summary>
        /// Creates a new physical entity from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a physical entity from.</param>
        protected PhysicalEntity(uint id)
            : base(id)
        {
        }
        #endregion Constructor: PhysicalEntity(uint id)

        #region Constructor: PhysicalEntity(string name)
        /// <summary>
        /// Creates a new physical entity with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the physical entity.</param>
        protected PhysicalEntity(string name)
            : base(name)
        {
        }
        #endregion Constructor: PhysicalEntity(string name)

        #region Constructor: PhysicalEntity(PhysicalEntity physicalEntity)
        /// <summary>
        /// Clones the physical entity.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to clone.</param>
        protected PhysicalEntity(PhysicalEntity physicalEntity)
            : base(physicalEntity)
        {
        }
        #endregion Constructor: PhysicalEntity(PhysicalEntity physicalEntity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the physical entity.
        /// </summary>
        /// <returns>A clone of the physical entity.</returns>
        public new PhysicalEntity Clone()
        {
            return base.Clone() as PhysicalEntity;
        }
        #endregion Method: Clone()

        #region Method: CompareTo(PhysicalEntity other)
        /// <summary>
        /// Compares the physical entity to the other physical entity.
        /// </summary>
        /// <param name="other">The physical entity to compare to this physical entity.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(PhysicalEntity other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(PhysicalEntity other)

        #endregion Method Group: Other

    }
    #endregion Class: PhysicalEntity

    #region Class: PhysicalEntityValued
    /// <summary>
    /// A valued version of a physical entity.
    /// </summary>
    public abstract class PhysicalEntityValued : EntityValued
    {

        #region Properties and Fields

        #region Property: PhysicalEntity
        /// <summary>
        /// Gets the physical entity of which this is a valued physical entity.
        /// </summary>
        public PhysicalEntity PhysicalEntity
        {
            get
            {
                return this.Node as PhysicalEntity;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: PhysicalEntity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalEntityValued(uint id)
        /// <summary>
        /// Creates a new valued physical entity from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued physical entity from.</param>
        protected PhysicalEntityValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: PhysicalEntityValued(uint id)

        #region Constructor: PhysicalEntityValued(PhysicalEntityValued physicalEntityValued)
        /// <summary>
        /// Clones a valued physical entity.
        /// </summary>
        /// <param name="physicalEntityValued">The valued physical entity to clone.</param>
        protected PhysicalEntityValued(PhysicalEntityValued physicalEntityValued)
            : base(physicalEntityValued)
        {
        }
        #endregion Constructor: PhysicalEntityValued(PhysicalEntityValued physicalEntityValued)

        #region Constructor: PhysicalEntityValued(PhysicalEntity physicalEntity)
        /// <summary>
        /// Creates a new valued physical entity from the given physical entity.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to create the valued physical entity from.</param>
        protected PhysicalEntityValued(PhysicalEntity physicalEntity)
            : base(physicalEntity)
        {
        }
        #endregion Constructor: PhysicalEntityValued(PhysicalEntity physicalEntity)

        #region Constructor: PhysicalEntityValued(PhysicalEntity physicalEntity, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new valued physical entity from the given physical entity in the given quantity.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to create the valued physical entity from.</param>
        /// <param name="quantity">The quantity of the valued physical entity.</param>
        protected PhysicalEntityValued(PhysicalEntity physicalEntity, NumericalValueRange quantity)
            : base(physicalEntity, quantity)
        {
        }
        #endregion Constructor: PhysicalEntityValued(PhysicalEntity physicalEntity, NumericalValueRange quantity)

        #region Method: Create(PhysicalEntity physicalEntity)
        /// <summary>
        /// Create a valued physical entity of the given physical entity.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to create a valued physical entity of.</param>
        /// <returns>A valued physical entity of the given physical entity.</returns>
        public static PhysicalEntityValued Create(PhysicalEntity physicalEntity)
        {
            PhysicalObject physicalObject = physicalEntity as PhysicalObject;
            if (physicalObject != null)
                return PhysicalObjectValued.Create(physicalObject);

            Matter matter = physicalEntity as Matter;
            if (matter != null)
                return MatterValued.Create(matter);

            Element element = physicalEntity as Element;
            if (element != null)
                return new ElementValued(element);

            return null;
        }
        #endregion Method: Create(PhysicalEntity physicalEntity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the valued physical entity.
        /// </summary>
        /// <returns>A clone of the valued physical entity.</returns>
        public new PhysicalEntityValued Clone()
        {
            return base.Clone() as PhysicalEntityValued;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: PhysicalEntityValued

    #region Class: PhysicalEntityCondition
    /// <summary>
    /// A condition on a physical entity.
    /// </summary>
    public abstract class PhysicalEntityCondition : EntityCondition
    {

        #region Properties and Fields

        #region Property: PhysicalEntity
        /// <summary>
        /// Gets the required physical entity.
        /// </summary>
        public PhysicalEntity PhysicalEntity
        {
            get
            {
                return this.Node as PhysicalEntity;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: PhysicalEntity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalEntityCondition()
        /// <summary>
        /// Creates a new physical entity condition.
        /// </summary>
        protected PhysicalEntityCondition()
            : base()
        {
        }
        #endregion Constructor: PhysicalEntityCondition()

        #region Constructor: PhysicalEntityCondition(uint id)
        /// <summary>
        /// Creates a new physical entity condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a physical entity condition from.</param>
        protected PhysicalEntityCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: PhysicalEntityCondition(uint id)

        #region Constructor: PhysicalEntityCondition(PhysicalEntityCondition physicalEntityCondition)
        /// <summary>
        /// Clones a physical entity condition.
        /// </summary>
        /// <param name="physicalEntityCondition">The physical entity condition to clone.</param>
        protected PhysicalEntityCondition(PhysicalEntityCondition physicalEntityCondition)
            : base(physicalEntityCondition)
        {
        }
        #endregion Constructor: PhysicalEntityCondition(PhysicalEntityCondition physicalEntityCondition)

        #region Constructor: PhysicalEntityCondition(PhysicalEntity physicalEntity)
        /// <summary>
        /// Creates a condition for the given physical entity.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to create a condition for.</param>
        protected PhysicalEntityCondition(PhysicalEntity physicalEntity)
            : base(physicalEntity)
        {
        }
        #endregion Constructor: PhysicalEntityCondition(PhysicalEntity physicalEntity)

        #region Constructor: PhysicalEntityCondition(PhysicalEntity physicalEntity, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given physical entity in the given quantity.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to create a condition for.</param>
        /// <param name="quantity">The quantity of the physical entity condition.</param>
        protected PhysicalEntityCondition(PhysicalEntity physicalEntity, NumericalValueCondition quantity)
            : base(physicalEntity, quantity)
        {
        }
        #endregion Constructor: PhysicalEntityCondition(PhysicalEntity physicalEntity, NumericalValueCondition quantity)

        #region Method: Create(PhysicalEntity physicalEntity)
        /// <summary>
        /// Create a physical entity condition of the given physical entity.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to create a physical entity condition of.</param>
        /// <returns>A physical entity condition of the given physical entity.</returns>
        public static PhysicalEntityCondition Create(PhysicalEntity physicalEntity)
        {
            Element element = physicalEntity as Element;
            if (element != null)
                return new ElementCondition(element);

            Matter matter = physicalEntity as Matter;
            if (matter != null)
                return MatterCondition.Create(matter);

            PhysicalObject physicalObject = physicalEntity as PhysicalObject;
            if (physicalObject != null)
                return PhysicalObjectCondition.Create(physicalObject);

            return null;
        }
        #endregion Method: Create(PhysicalEntity physicalEntity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the physical entity condition.
        /// </summary>
        /// <returns>A clone of the physical entity condition.</returns>
        public new PhysicalEntityCondition Clone()
        {
            return base.Clone() as PhysicalEntityCondition;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: PhysicalEntityCondition

    #region Class: PhysicalEntityChange
    /// <summary>
    /// A change on a physical entity.
    /// </summary>
    public abstract class PhysicalEntityChange : EntityChange
    {

        #region Properties and Fields

        #region Property: PhysicalEntity
        /// <summary>
        /// Gets or sets the affected physical entity.
        /// </summary>
        public PhysicalEntity PhysicalEntity
        {
            get
            {
                return this.Node as PhysicalEntity;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: PhysicalEntity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalEntityChange()
        /// <summary>
        /// Creates a new physical entity change.
        /// </summary>
        protected PhysicalEntityChange()
            : base()
        {
        }
        #endregion Constructor: PhysicalEntityChange()

        #region Constructor: PhysicalEntityChange(uint id)
        /// <summary>
        /// Creates a new physical entity change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a physical entity change from.</param>
        protected PhysicalEntityChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: PhysicalEntityChange(uint id)

        #region Constructor: PhysicalEntityChange(PhysicalEntityChange physicalEntityChange)
        /// <summary>
        /// Clones a physical entity change.
        /// </summary>
        /// <param name="physicalEntityChange">The physical entity change to clone.</param>
        protected PhysicalEntityChange(PhysicalEntityChange physicalEntityChange)
            : base(physicalEntityChange)
        {
        }
        #endregion Constructor: PhysicalEntityChange(PhysicalEntityChange physicalEntityChange)

        #region Constructor: PhysicalEntityChange(PhysicalEntity physicalEntity)
        /// <summary>
        /// Creates a change for the given physical entity.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to create a change for.</param>
        protected PhysicalEntityChange(PhysicalEntity physicalEntity)
            : base(physicalEntity)
        {
        }
        #endregion Constructor: PhysicalEntityChange(PhysicalEntity physicalEntity)

        #region Constructor: PhysicalEntityChange(PhysicalEntity physicalEntity, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given physical entity in the form of the given quantity.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        protected PhysicalEntityChange(PhysicalEntity physicalEntity, NumericalValueChange quantity)
            : base(physicalEntity, quantity)
        {
        }
        #endregion Constructor: PhysicalEntityChange(PhysicalEntity physicalEntity, NumericalValueChange quantity)

        #region Method: Create(PhysicalEntity physicalEntity)
        /// <summary>
        /// Create a physical entity change of the given physical entity.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to create a physical entity change of.</param>
        /// <returns>A physical entity change of the given physical entity.</returns>
        public static PhysicalEntityChange Create(PhysicalEntity physicalEntity)
        {
            Element element = physicalEntity as Element;
            if (element != null)
                return new ElementChange(element);

            Matter matter = physicalEntity as Matter;
            if (matter != null)
                return MatterChange.Create(matter);

            PhysicalObject physicalObject = physicalEntity as PhysicalObject;
            if (physicalObject != null)
                return PhysicalObjectChange.Create(physicalObject);

            return null;
        }
        #endregion Method: Create(PhysicalEntity physicalEntity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the physical entity change.
        /// </summary>
        /// <returns>A clone of the physical entity change.</returns>
        public new PhysicalEntityChange Clone()
        {
            return base.Clone() as PhysicalEntityChange;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: PhysicalEntityChange

}