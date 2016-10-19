/**************************************************************************
 * 
 * Material.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using Semantics.Components;
using Semantics.Data;

namespace Semantics.Entities
{

    #region Class: Material
    /// <summary>
    /// A material.
    /// </summary>
    public class Material : Matter, IComparable<Material>
    {

        #region Method Group: Constructors

        #region Constructor: Material()
        /// <summary>
        /// Creates a new material.
        /// </summary>
        public Material()
            : base()
        {
        }
        #endregion Constructor: Material()

        #region Constructor: Material(uint id)
        /// <summary>
        /// Creates a new material from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the material from.</param>
        protected Material(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Material(uint id)

        #region Constructor: Material(string name)
        /// <summary>
        /// Creates a new material with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the material.</param>
        public Material(string name)
            : base(name)
        {
        }
        #endregion Constructor: Material(string name)

        #region Constructor: Material(Material material)
        /// <summary>
        /// Clones a material.
        /// </summary>
        /// <param name="material">The material to clone.</param>
        public Material(Material material)
            : base(material)
        {
        }
        #endregion Constructor: Material(Material material)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the material.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Action other)
        /// <summary>
        /// Compares the material to the other material.
        /// </summary>
        /// <param name="other">The material to compare to this material.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Material other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Action other)

        #endregion Method Group: Other

    }
    #endregion Class: Material

    #region Class: MaterialValued
    /// <summary>
    /// A valued version of a material.
    /// </summary>
    public class MaterialValued : MatterValued
    {

        #region Properties and Fields

        #region Property: Material
        /// <summary>
        /// Gets the material of which this is a valued material.
        /// </summary>
        public Material Material
        {
            get
            {
                return this.Node as Material;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Material

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MaterialValued(uint id)
        /// <summary>
        /// Creates a new valued material from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued material from.</param>
        protected MaterialValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: MaterialValued(uint id)

        #region Constructor: MaterialValued(MaterialValued materialValued)
        /// <summary>
        /// Clones a valued material.
        /// </summary>
        /// <param name="materialValued">The valued material to clone.</param>
        public MaterialValued(MaterialValued materialValued)
            : base(materialValued)
        {
        }
        #endregion Constructor: MaterialValued(MaterialValued materialValued)

        #region Constructor: MaterialValued(Material material)
        /// <summary>
        /// Creates a new valued material from the given material.
        /// </summary>
        /// <param name="element">The material to create a valued material from.</param>
        public MaterialValued(Material material)
            : base(material)
        {
        }
        #endregion Constructor: MaterialValued(Material material)

        #region Constructor: MaterialValued(Material material, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new valued material from the given material in the given quantity.
        /// </summary>
        /// <param name="element">The material to create a valued material from.</param>
        /// <param name="quantity">The quantity of the valued material.</param>
        public MaterialValued(Material material, NumericalValueRange quantity)
            : base(material, quantity)
        {
        }
        #endregion Constructor: MaterialValued(Material material, NumericalValueRange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: MaterialValued

    #region Class: MaterialCondition
    /// <summary>
    /// A condition on a material.
    /// </summary>
    public class MaterialCondition : MatterCondition
    {

        #region Properties and Fields

        #region Property: Material
        /// <summary>
        /// Gets or sets the required material.
        /// </summary>
        public Material Material
        {
            get
            {
                return this.Node as Material;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Material

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MaterialCondition()
        /// <summary>
        /// Creates a new material condition.
        /// </summary>
        public MaterialCondition()
            : base()
        {
        }
        #endregion Constructor: MaterialCondition()

        #region Constructor: MaterialCondition(uint id)
        /// <summary>
        /// Creates a new material condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the material condition from.</param>
        protected MaterialCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: MaterialCondition(uint id)

        #region Constructor: MaterialCondition(MaterialCondition materialCondition)
        /// <summary>
        /// Clones an material condition.
        /// </summary>
        /// <param name="materialCondition">The material condition to clone.</param>
        public MaterialCondition(MaterialCondition materialCondition)
            : base(materialCondition)
        {
        }
        #endregion Constructor: MaterialCondition(MaterialCondition materialCondition)

        #region Constructor: MaterialCondition(Material material)
        /// <summary>
        /// Creates a condition for the given material.
        /// </summary>
        /// <param name="material">The material to create a condition for.</param>
        public MaterialCondition(Material material)
            : base(material)
        {
        }
        #endregion Constructor: MaterialCondition(Material material)

        #region Constructor: MaterialCondition(Material material, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given material in the given quantity.
        /// </summary>
        /// <param name="material">The material to create a condition for.</param>
        /// <param name="quantity">The quantity of the material condition.</param>
        public MaterialCondition(Material material, NumericalValueCondition quantity)
            : base(material, quantity)
        {
        }
        #endregion Constructor: MaterialCondition(Material material, NumericalValueCondition quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: MaterialCondition

    #region Class: MaterialChange
    /// <summary>
    /// A change on a material.
    /// </summary>
    public class MaterialChange : MatterChange
    {

        #region Properties and Fields

        #region Property: Material
        /// <summary>
        /// Gets or sets the affected material.
        /// </summary>
        public Material Material
        {
            get
            {
                return this.Node as Material;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Material

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MaterialChange()
        /// <summary>
        /// Creates a new material change.
        /// </summary>
        public MaterialChange()
            : base()
        {
        }
        #endregion Constructor: MaterialChange()

        #region Constructor: MaterialChange(uint id)
        /// <summary>
        /// Creates a new material change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a material change from.</param>
        protected MaterialChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: MaterialChange(uint id)

        #region Constructor: MaterialChange(MaterialChange materialChange)
        /// <summary>
        /// Clones a material change.
        /// </summary>
        /// <param name="materialChange">The material change to clone.</param>
        public MaterialChange(MaterialChange materialChange)
            : base(materialChange)
        {
        }
        #endregion Constructor: MaterialChange(MaterialChange materialChange)

        #region Constructor: MaterialChange(Material material)
        /// <summary>
        /// Creates a change for the given material.
        /// </summary>
        /// <param name="material">The material to create a change for.</param>
        public MaterialChange(Material material)
            : base(material)
        {
        }
        #endregion Constructor: MaterialChange(Material material)

        #region Constructor: MaterialChange(Material material, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given material in the form of the given quantity.
        /// </summary>
        /// <param name="material">The material to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        public MaterialChange(Material material, NumericalValueChange quantity)
            : base(material, quantity)
        {
        }
        #endregion Constructor: MaterialChange(Material material, NumericalValueChange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: MaterialChange

}