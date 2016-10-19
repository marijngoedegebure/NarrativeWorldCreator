/**************************************************************************
 * 
 * Abstraction.cs
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

namespace Semantics.Abstractions
{

    #region Class: Abstraction
    /// <summary>
    /// An abstraction.
    /// </summary>
    public abstract class Abstraction : Node, IComparable<Abstraction>
    {

        #region Method Group: Constructors

        #region Constructor: Abstraction()
        /// <summary>
        /// Creates a new abstraction.
        /// </summary>
        protected Abstraction()
            : base()
        {
        }
        #endregion Constructor: Abstraction()

        #region Constructor: Abstraction(string name)
        /// <summary>
        /// Creates a new abstraction with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the abstraction.</param>
        protected Abstraction(string name)
            : base(name)
        {
        }
        #endregion Constructor: Abstraction(string name)

        #region Constructor: Abstraction(uint id)
        /// <summary>
        /// Creates a new abstraction from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an abstraction from.</param>
        protected Abstraction(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Abstraction(uint id)

        #region Constructor: Abstraction(Abstraction abstraction)
        /// <summary>
        /// Clones an abstraction.
        /// </summary>
        /// <param name="abstraction">The abstraction to clone.</param>
        protected Abstraction(Abstraction abstraction)
            : base(abstraction)
        {
        }
        #endregion Constructor: Abstraction(Abstraction abstraction)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the abstraction.
        /// </summary>
        /// <returns>A clone of the abstraction.</returns>
        public new Abstraction Clone()
        {
            return base.Clone() as Abstraction;
        }
        #endregion Method: Clone()

        #region Method: CompareTo(Abstraction other)
        /// <summary>
        /// Compares the abstraction to the other abstraction.
        /// </summary>
        /// <param name="other">The abstraction to compare to this abstraction.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Abstraction other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Abstraction other)

        #endregion Method Group: Other

    }
    #endregion Class: Abstraction

    #region Class: AbstractionValued
    /// <summary>
    /// A valued version of an abstraction.
    /// </summary>
    public abstract class AbstractionValued : NodeValued
    {

        #region Properties and Fields

        #region Property: Abstraction
        /// <summary>
        /// Gets the abstraction of which this is a valued abstraction.
        /// </summary>
        public Abstraction Abstraction
        {
            get
            {
                return this.Node as Abstraction;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Abstraction

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractionValued(uint id)
        /// <summary>
        /// Creates a new valued abstraction from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued abstraction from.</param>
        protected AbstractionValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AbstractionValued(uint id)

        #region Constructor: AbstractionValued(AbstractionValued abstractionValued)
        /// <summary>
        /// Clones a valued abstraction.
        /// </summary>
        /// <param name="abstractionValued">The valued abstraction to clone.</param>
        protected AbstractionValued(AbstractionValued abstractionValued)
            : base(abstractionValued)
        {
        }
        #endregion Constructor: AbstractionValued(AbstractionValued abstractionValued)

        #region Constructor: AbstractionValued(Abstraction abstraction)
        /// <summary>
        /// Creates a new valued abstraction from the given abstraction.
        /// </summary>
        /// <param name="abstraction">The abstraction to create the valued abstraction from.</param>
        protected AbstractionValued(Abstraction abstraction)
            : base(abstraction)
        {
        }
        #endregion Constructor: AbstractionValued(Abstraction abstraction)

        #region Method: Create(Abstraction abstraction)
        /// <summary>
        /// Create a valued abstraction of the given abstraction.
        /// </summary>
        /// <param name="abstraction">The abstraction to create a valued abstraction of.</param>
        /// <returns>A valued abstraction of the given abstraction.</returns>
        public static AbstractionValued Create(Abstraction abstraction)
        {
            Attribute attribute = abstraction as Attribute;
            if (attribute != null)
                return new AttributeValued(attribute);

            Scene scene = abstraction as Scene;
            if (scene != null)
                return new SceneValued(scene);

            return null;
        }
        #endregion Method: Create(Abstraction abstraction)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the valued abstraction.
        /// </summary>
        /// <returns>A clone of the valued abstraction.</returns>
        public new AbstractionValued Clone()
        {
            return base.Clone() as AbstractionValued;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: AbstractionValued

    #region Class: AbstractionCondition
    /// <summary>
    /// A condition on an abstraction.
    /// </summary>
    public abstract class AbstractionCondition : NodeCondition
    {

        #region Properties and Fields

        #region Property: Abstraction
        /// <summary>
        /// Gets or sets the required abstraction.
        /// </summary>
        public Abstraction Abstraction
        {
            get
            {
                return this.Node as Abstraction;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Abstraction

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractionCondition()
        /// <summary>
        /// Creates a new abstraction condition.
        /// </summary>
        protected AbstractionCondition()
            : base()
        {
        }
        #endregion Constructor: AbstractionCondition()

        #region Constructor: AbstractionCondition(uint id)
        /// <summary>
        /// Creates a new abstraction condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an abstraction condition from.</param>
        protected AbstractionCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AbstractionCondition(uint id)

        #region Constructor: AbstractionCondition(AbstractionCondition abstractionCondition)
        /// <summary>
        /// Clones an abstraction condition.
        /// </summary>
        /// <param name="abstractionCondition">The abstraction condition to clone.</param>
        protected AbstractionCondition(AbstractionCondition abstractionCondition)
            : base(abstractionCondition)
        {
        }
        #endregion Constructor: AbstractionCondition(AbstractionCondition abstractionCondition)

        #region Constructor: AbstractionCondition(Abstraction abstraction)
        /// <summary>
        /// Creates a condition for the given abstraction.
        /// </summary>
        /// <param name="abstraction">The abstraction to create a condition for.</param>
        protected AbstractionCondition(Abstraction abstraction)
            : base(abstraction)
        {
        }
        #endregion Constructor: AbstractionCondition(Abstraction abstraction)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the abstraction condition.
        /// </summary>
        /// <returns>A clone of the abstraction condition.</returns>
        public new AbstractionCondition Clone()
        {
            return base.Clone() as AbstractionCondition;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: AbstractionCondition

    #region Class: AbstractionChange
    /// <summary>
    /// A change on an abstraction.
    /// </summary>
    public abstract class AbstractionChange : NodeChange
    {

        #region Properties and Fields

        #region Property: Abstraction
        /// <summary>
        /// Gets the affected abstraction.
        /// </summary>
        public Abstraction Abstraction
        {
            get
            {
                return this.Node as Abstraction;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Abstraction

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractionChange()
        /// <summary>
        /// Creates a new abstraction change.
        /// </summary>
        protected AbstractionChange()
            : base()
        {
        }
        #endregion Constructor: AbstractionChange()

        #region Constructor: AbstractionChange(uint id)
        /// <summary>
        /// Creates a new abstraction change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an abstraction change from.</param>
        protected AbstractionChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AbstractionChange(uint id)

        #region Constructor: AbstractionChange(AbstractionChange abstractionChange)
        /// <summary>
        /// Clones an abstraction change.
        /// </summary>
        /// <param name="abstractionChange">The abstraction change to clone.</param>
        protected AbstractionChange(AbstractionChange abstractionChange)
            : base(abstractionChange)
        {
        }
        #endregion Constructor: AbstractionChange(AbstractionChange abstractionChange)

        #region Constructor: AbstractionChange(Abstraction abstraction)
        /// <summary>
        /// Creates a change for the given abstraction.
        /// </summary>
        /// <param name="abstraction">The abstraction to create a change for.</param>
        protected AbstractionChange(Abstraction abstraction)
            : base(abstraction)
        {
        }
        #endregion Constructor: AbstractionChange(Abstraction abstraction)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the abstraction change.
        /// </summary>
        /// <returns>A clone of the abstraction change.</returns>
        public new AbstractionChange Clone()
        {
            return base.Clone() as AbstractionChange;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: AbstractionChange

}