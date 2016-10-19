/**************************************************************************
 * 
 * AbstractEntity.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using Semantics.Components;
using Semantics.Data;

namespace Semantics.Entities
{

    #region Class: AbstractEntity
    /// <summary>
    /// An abstract entity.
    /// </summary>
    public class AbstractEntity : Entity, IComparable<AbstractEntity>
    {

        #region Method Group: Constructors

        #region Constructor: AbstractEntity()
        /// <summary>
        /// Creates a new abstract entity.
        /// </summary>
        public AbstractEntity()
            : base()
        {
        }
        #endregion Constructor: AbstractEntity()

        #region Constructor: AbstractEntity(uint id)
        /// <summary>
        /// Creates a new abstract entity from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an abstract entity from.</param>
        protected AbstractEntity(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AbstractEntity(uint id)

        #region Constructor: AbstractEntity(string name)
        /// <summary>
        /// Creates a new abstract entity with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the abstract entity.</param>
        public AbstractEntity(string name)
            : base(name)
        {
        }
        #endregion Constructor: AbstractEntity(string name)

        #region Constructor: AbstractEntity(AbstractEntity abstractEntity)
        /// <summary>
        /// Clones an abstract entity.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to clone.</param>
        public AbstractEntity(AbstractEntity abstractEntity)
            : base(abstractEntity)
        {
        }
        #endregion Constructor: AbstractEntity(AbstractEntity abstractEntity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the abstract entity.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(AbstractEntity other)
        /// <summary>
        /// Compares the abstract entity to the other abstract entity.
        /// </summary>
        /// <param name="other">The abstract entity to compare to this abstract entity.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(AbstractEntity other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(AbstractEntity other)

        #endregion Method Group: Other

    }
    #endregion Class: AbstractEntity

    #region Class: AbstractEntityValued
    /// <summary>
    /// A valued version of an abstract entity.
    /// </summary>
    public class AbstractEntityValued : EntityValued
    {

        #region Properties and Fields

        #region Property: AbstractEntity
        /// <summary>
        /// Gets the abstract entity of which this is a valued abstract entity.
        /// </summary>
        public AbstractEntity AbstractEntity
        {
            get
            {
                return this.Node as AbstractEntity;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: AbstractEntity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractEntityValued(uint id)
        /// <summary>
        /// Creates a new valued abstract entity from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued abstract entity from.</param>
        protected AbstractEntityValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AbstractEntityValued(uint id)

        #region Constructor: AbstractEntityValued(AbstractEntityValued abstractEntityValued)
        /// <summary>
        /// Clones a valued abstract entity.
        /// </summary>
        /// <param name="abstractEntityValued">The valued abstract entity to clone.</param>
        public AbstractEntityValued(AbstractEntityValued abstractEntityValued)
            : base(abstractEntityValued)
        {
        }
        #endregion Constructor: AbstractEntityValued(AbstractEntityValued abstractEntityValued)

        #region Constructor: AbstractEntityValued(AbstractEntity abstractEntity)
        /// <summary>
        /// Creates a new valued abstract entity from the given abstract entity.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to create the valued abstract entity from.</param>
        public AbstractEntityValued(AbstractEntity abstractEntity)
            : base(abstractEntity)
        {
        }
        #endregion Constructor: AbstractEntityValued(AbstractEntity abstractEntity)

        #region Constructor: AbstractEntityValued(AbstractEntity abstractEntity, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new valued abstract entity from the given abstract entity in the given quantity.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to create the valued abstract entity from.</param>
        /// <param name="quantity">The quantity of the valued abstract entity.</param>
        public AbstractEntityValued(AbstractEntity abstractEntity, NumericalValueRange quantity)
            : base(abstractEntity, quantity)
        {
        }
        #endregion Constructor: AbstractEntityValued(AbstractEntity abstractEntity, NumericalValueRange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: AbstractEntityValued

    #region Class: AbstractEntityCondition
    /// <summary>
    /// A condition on an abstract entity.
    /// </summary>
    public class AbstractEntityCondition : EntityCondition
    {

        #region Properties and Fields

        #region Property: AbstractEntity
        /// <summary>
        /// Gets or sets the required abstract entity.
        /// </summary>
        public AbstractEntity AbstractEntity
        {
            get
            {
                return this.Node as AbstractEntity;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: AbstractEntity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractEntityCondition()
        /// <summary>
        /// Creates a new abstract entity condition.
        /// </summary>
        public AbstractEntityCondition()
            : base()
        {
        }
        #endregion Constructor: AbstractEntityCondition()

        #region Constructor: AbstractEntityCondition(uint id)
        /// <summary>
        /// Creates a new abstract entity condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the abstract entity condition from.</param>
        protected AbstractEntityCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AbstractEntityCondition(uint id)

        #region Constructor: AbstractEntityCondition(AbstractEntityCondition abstractEntityCondition)
        /// <summary>
        /// Clones an abstract entity condition.
        /// </summary>
        /// <param name="abstractEntityCondition">The abstract entity condition to clone.</param>
        public AbstractEntityCondition(AbstractEntityCondition abstractEntityCondition)
            : base(abstractEntityCondition)
        {
        }
        #endregion Constructor: AbstractEntityCondition(AbstractEntityCondition abstractEntityCondition)

        #region Constructor: AbstractEntityCondition(AbstractEntity abstractEntity)
        /// <summary>
        /// Creates a condition for the given abstract entity.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to create a condition for.</param>
        public AbstractEntityCondition(AbstractEntity abstractEntity)
            : base(abstractEntity)
        {
        }
        #endregion Constructor: AbstractEntityCondition(AbstractEntity abstractEntity)

        #region Constructor: AbstractEntityCondition(AbstractEntity abstractEntity, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given abstract entity in the given quantity.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to create a condition for.</param>
        /// <param name="quantity">The quantity of the abstract entity condition.</param>
        public AbstractEntityCondition(AbstractEntity abstractEntity, NumericalValueCondition quantity)
            : base(abstractEntity, quantity)
        {
        }
        #endregion Constructor: AbstractEntityCondition(AbstractEntity abstractEntity, NumericalValueCondition quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: AbstractEntityCondition

    #region Class: AbstractEntityChange
    /// <summary>
    /// A change on an abstract entity.
    /// </summary>
    public class AbstractEntityChange : EntityChange
    {

        #region Properties and Fields

        #region Property: AbstractEntity
        /// <summary>
        /// Gets or sets the affected abstract entity.
        /// </summary>
        public AbstractEntity AbstractEntity
        {
            get
            {
                return this.Node as AbstractEntity;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: AbstractEntity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractEntityChange()
        /// <summary>
        /// Creates a new abstract entity change.
        /// </summary>
        public AbstractEntityChange()
            : base()
        {
        }
        #endregion Constructor: AbstractEntityChange()

        #region Constructor: AbstractEntityChange(uint id)
        /// <summary>
        /// Creates a new abstract entity change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an abstract entity change from.</param>
        protected AbstractEntityChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AbstractEntityChange(uint id)

        #region Constructor: AbstractEntityChange(AbstractEntityChange abstractEntityChange)
        /// <summary>
        /// Clones an abstract entity change.
        /// </summary>
        /// <param name="abstractEntityChange">The abstract entity change to clone.</param>
        public AbstractEntityChange(AbstractEntityChange abstractEntityChange)
            : base(abstractEntityChange)
        {
        }
        #endregion Constructor: AbstractEntityChange(AbstractEntityChange abstractEntityChange)

        #region Constructor: AbstractEntityChange(AbstractEntity abstractEntity)
        /// <summary>
        /// Creates a change for the given abstract entity.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to create a change for.</param>
        public AbstractEntityChange(AbstractEntity abstractEntity)
            : base(abstractEntity)
        {
        }
        #endregion Constructor: AbstractEntityChange(AbstractEntity abstractEntity)

        #region Constructor: AbstractEntityChange(AbstractEntity abstractEntity, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given abstract entity in the form of the given quantity.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        public AbstractEntityChange(AbstractEntity abstractEntity, NumericalValueChange quantity)
            : base(abstractEntity, quantity)
        {
        }
        #endregion Constructor: AbstractEntityChange(AbstractEntity abstractEntity, NumericalValueChange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: AbstractEntityChange

}