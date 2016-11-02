/**************************************************************************
 * 
 * MaterialBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Entities;

namespace SemanticsEngine.Entities
{

    #region Class: MaterialBase
    /// <summary>
    /// A base of a material.
    /// </summary>
    public class MaterialBase : MatterBase
    {

        #region Properties and Fields

        #region Property: Material
        /// <summary>
        /// Gets the material of which this is a material base.
        /// </summary>
        protected internal Material Material
        {
            get
            {
                return this.IdHolder as Material;
            }
        }
        #endregion Property: Material

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MaterialBase(Material material)
        /// <summary>
        /// Creates a material base from the given material.
        /// </summary>
        /// <param name="material">The material to create a material base from.</param>
        protected internal MaterialBase(Material material)
            : base(material)
        {
        }
        #endregion Constructor: MaterialBase(Material material)

        #endregion Method Group: Constructors

    }
    #endregion Class: MaterialBase

    #region Class: MaterialValuedBase
    /// <summary>
    /// A base of a valued material.
    /// </summary>
    public class MaterialValuedBase : MatterValuedBase
    {

        #region Properties and Fields

        #region Property: MaterialValued
        /// <summary>
        /// Gets the valued material of which this is a valued material base.
        /// </summary>
        protected internal MaterialValued MaterialValued
        {
            get
            {
                return this.NodeValued as MaterialValued;
            }
        }
        #endregion Property: MaterialValued

        #region Property: MaterialBase
        /// <summary>
        /// Gets the material base of which this is a valued material base.
        /// </summary>
        public MaterialBase MaterialBase
        {
            get
            {
                return this.NodeBase as MaterialBase;
            }
        }
        #endregion Property: MaterialBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MaterialValuedBase(MaterialValued materialValued)
        /// <summary>
        /// Create a valued material base from the given valued material.
        /// </summary>
        /// <param name="materialValued">The valued material to create a valued material base from.</param>
        protected internal MaterialValuedBase(MaterialValued materialValued)
            : base(materialValued)
        {
        }
        #endregion Constructor: MaterialValuedBase(MaterialValued materialValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: MaterialValuedBase

    #region Class: MaterialConditionBase
    /// <summary>
    /// A condition on a material.
    /// </summary>
    public class MaterialConditionBase : MatterConditionBase
    {

        #region Properties and Fields

        #region Property: MaterialCondition
        /// <summary>
        /// Gets the material condition of which this is a material condition base.
        /// </summary>
        protected internal MaterialCondition MaterialCondition
        {
            get
            {
                return this.Condition as MaterialCondition;
            }
        }
        #endregion Property: MaterialCondition

        #region Property: MaterialBase
        /// <summary>
        /// Gets the material base of which this is a material condition base.
        /// </summary>
        public MaterialBase MaterialBase
        {
            get
            {
                return this.NodeBase as MaterialBase;
            }
        }
        #endregion Property: MaterialBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MaterialConditionBase(MaterialCondition materialCondition)
        /// <summary>
        /// Creates a base of the given material condition.
        /// </summary>
        /// <param name="materialCondition">The material condition to create a base of.</param>
        protected internal MaterialConditionBase(MaterialCondition materialCondition)
            : base(materialCondition)
        {
        }
        #endregion Constructor: MaterialConditionBase(MaterialCondition materialCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: MaterialConditionBase

    #region Class: MaterialChangeBase
    /// <summary>
    /// A change on a material.
    /// </summary>
    public class MaterialChangeBase : MatterChangeBase
    {

        #region Properties and Fields

        #region Property: MaterialChange
        /// <summary>
        /// Gets the material change of which this is a material change base.
        /// </summary>
        protected internal MaterialChange MaterialChange
        {
            get
            {
                return this.Change as MaterialChange;
            }
        }
        #endregion Property: MaterialChange

        #region Property: MaterialBase
        /// <summary>
        /// Gets the affected material base.
        /// </summary>
        public MaterialBase MaterialBase
        {
            get
            {
                return this.NodeBase as MaterialBase;
            }
        }
        #endregion Property: MaterialBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MaterialChangeBase(MaterialChange materialChange)
        /// <summary>
        /// Creates a base of the given material change.
        /// </summary>
        /// <param name="materialChange">The material change to create a base of.</param>
        protected internal MaterialChangeBase(MaterialChange materialChange)
            : base(materialChange)
        {
        }
        #endregion Constructor: MaterialChangeBase(MaterialChange materialChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: MaterialChangeBase

}