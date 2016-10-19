/**************************************************************************
 * 
 * Substance.cs
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

    #region Class: Substance
    /// <summary>
    /// A substance.
    /// </summary>
    public class Substance : Matter, IComparable<Substance>
    {

        #region Method Group: Constructors

        #region Constructor: Substance()
        /// <summary>
        /// Creates a new substance.
        /// </summary>
        public Substance()
            : base()
        {
        }
        #endregion Constructor: Substance() 

        #region Constructor: Substance(uint id)
        /// <summary>
        /// Creates a new substance from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the substance from.</param>
        protected Substance(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Substance(uint id) 

        #region Constructor: Substance(string name)
        /// <summary>
        /// Creates a new substance with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the substance.</param>
        public Substance(string name)
            : base(name)
        {
        }
        #endregion Constructor: Substance(string name)

        #region Constructor: Substance(Substance substance)
        /// <summary>
        /// Clones a substance.
        /// </summary>
        /// <param name="substance">The substance to clone.</param>
        public Substance(Substance substance)
            : base(substance)
        {
        }
        #endregion Constructor: Substance(Substance substance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the substance.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Substance other)
        /// <summary>
        /// Compares the substance to the other substance.
        /// </summary>
        /// <param name="other">The substance to compare to this substance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Substance other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Substance other)

        #endregion Method Group: Other

    }
    #endregion Class: Substance

    #region Class: SubstanceValued
    /// <summary>
    /// An valued version of a substance.
    /// </summary>
    public class SubstanceValued : MatterValued
    {

        #region Properties and Fields

        #region Property: Substance
        /// <summary>
        /// Gets the substance of which this is a valued substance.
        /// </summary>
        public Substance Substance
        {
            get
            {
                return this.Node as Substance;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Substance

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SubstanceValued(uint id)
        /// <summary>
        /// Creates a new valued substance from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued substance from.</param>
        protected SubstanceValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SubstanceValued(uint id)

        #region Constructor: SubstanceValued(SubstanceValued substanceValued)
        /// <summary>
        /// Clones a valued substance.
        /// </summary>
        /// <param name="substanceValued">The valued substance to clone.</param>
        public SubstanceValued(SubstanceValued substanceValued)
            : base(substanceValued)
        {
        }
        #endregion Constructor: SubstanceValued(SubstanceValued substanceValued)

        #region Constructor: SubstanceValued(Substance substance)
        /// <summary>
        /// Creates a new valued substance from the given substance.
        /// </summary>
        /// <param name="substance">The substance to create the valued substance from.</param>
        public SubstanceValued(Substance substance)
            : base(substance)
        {
        }
        #endregion Constructor: SubstanceValued(Substance substance)

        #region Constructor: SubstanceValued(Substance substance, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new valued substance from the given substance in the given quantity.
        /// </summary>
        /// <param name="substance">The substance to create the valued substance from.</param>
        /// <param name="quantity">The quantity of the valued substance.</param>
        public SubstanceValued(Substance substance, NumericalValueRange quantity)
            : base(substance, quantity)
        {
        }
        #endregion Constructor: SubstanceValued(Substance substance, NumericalValueRange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: SubstanceValued

    #region Class: SubstanceCondition
    /// <summary>
    /// A condition on a substance.
    /// </summary>
    public class SubstanceCondition : MatterCondition
    {

        #region Properties and Fields

        #region Property: Substance
        /// <summary>
        /// Gets or sets the required substance.
        /// </summary>
        public Substance Substance
        {
            get
            {
                return this.Node as Substance;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Substance

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SubstanceCondition()
        /// <summary>
        /// Creates a new substance condition.
        /// </summary>
        public SubstanceCondition()
            : base()
        {
        }
        #endregion Constructor: SubstanceCondition()

        #region Constructor: SubstanceCondition(uint id)
        /// <summary>
        /// Creates a new substance condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the substance condition from.</param>
        protected SubstanceCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SubstanceCondition(uint id)

        #region Constructor: SubstanceCondition(SubstanceCondition substanceCondition)
        /// <summary>
        /// Clones a substance condition.
        /// </summary>
        /// <param name="substanceCondition">The substance condition to clone.</param>
        public SubstanceCondition(SubstanceCondition substanceCondition)
            : base(substanceCondition)
        {
        }
        #endregion Constructor: SubstanceCondition(SubstanceCondition substanceCondition)

        #region Constructor: SubstanceCondition(Substance substance)
        /// <summary>
        /// Creates a condition for the given substance.
        /// </summary>
        /// <param name="substance">The substance to create a condition for.</param>
        public SubstanceCondition(Substance substance)
            : base(substance)
        {
        }
        #endregion Constructor: SubstanceCondition(Substance substance)

        #region Constructor: SubstanceCondition(Substance substance, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given substance in the given quantity.
        /// </summary>
        /// <param name="substance">The substance to create a condition for.</param>
        /// <param name="quantity">The quantity of the substance condition.</param>
        public SubstanceCondition(Substance substance, NumericalValueCondition quantity)
            : base(substance, quantity)
        {
        }
        #endregion Constructor: SubstanceCondition(Substance substance, NumericalValueCondition quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: SubstanceCondition

    #region Class: SubstanceChange
    /// <summary>
    /// A change on a substance.
    /// </summary>
    public class SubstanceChange : MatterChange
    {

        #region Properties and Fields

        #region Property: Substance
        /// <summary>
        /// Gets or sets the affected substance.
        /// </summary>
        public Substance Substance
        {
            get
            {
                return this.Node as Substance;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Substance

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SubstanceChange()
        /// <summary>
        /// Creates a new substance change.
        /// </summary>
        public SubstanceChange()
            : base()
        {
        }
        #endregion Constructor: SubstanceChange()

        #region Constructor: SubstanceChange(uint id)
        /// <summary>
        /// Creates a new substance change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a substance change from.</param>
        protected SubstanceChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SubstanceChange(uint id)

        #region Constructor: SubstanceChange(SubstanceChange substanceChange)
        /// <summary>
        /// Clones a substance change.
        /// </summary>
        /// <param name="substanceChange">The substance change to clone.</param>
        public SubstanceChange(SubstanceChange substanceChange)
            : base(substanceChange)
        {
        }
        #endregion Constructor: SubstanceChange(SubstanceChange substanceChange)

        #region Constructor: SubstanceChange(Substance substance)
        /// <summary>
        /// Creates a change for the given substance.
        /// </summary>
        /// <param name="substance">The substance to create a change for.</param>
        public SubstanceChange(Substance substance)
            : base(substance)
        {
        }
        #endregion Constructor: SubstanceChange(Substance substance)

        #region Constructor: SubstanceChange(Substance substance, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given substance in the form of the given quantity.
        /// </summary>
        /// <param name="substance">The substance to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        public SubstanceChange(Substance substance, NumericalValueChange quantity)
            : base(substance, quantity)
        {
        }
        #endregion Constructor: SubstanceChange(Substance substance, NumericalValueChange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: SubstanceChange

}