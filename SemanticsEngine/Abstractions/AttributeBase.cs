/**************************************************************************
 * 
 * AttributeBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Abstractions;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Tools;
using Attribute = Semantics.Abstractions.Attribute;
using ValueType = Semantics.Utilities.ValueType;

namespace SemanticsEngine.Abstractions
{

    #region Class: AttributeBase
    /// <summary>
    /// A base of an attribute.
    /// </summary>
    public class AttributeBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: Attribute
        /// <summary>
        /// Gets the attribute of which this is an attribute base.
        /// </summary>
        protected internal Attribute Attribute
        {
            get
            {
                return this.IdHolder as Attribute;
            }
        }
        #endregion Property: Attribute

        #region Property: AttributeType
        /// <summary>
        /// The type of the attribute.
        /// </summary>
        private AttributeType attributeType = default(AttributeType);

        /// <summary>
        /// Gets the type of the attribute.
        /// </summary>
        public AttributeType AttributeType
        {
            get
            {
                return attributeType;
            }
        }
        #endregion Property: AttributeType

        #region Property: ValueType
        /// <summary>
        /// The value type of the attribute.
        /// </summary>
        private ValueType valueType = default(ValueType);

        /// <summary>
        /// Gets the value type of the attribute.
        /// </summary>
        public ValueType ValueType
        {
            get
            {
                return valueType;
            }
        }
        #endregion Property: ValueType

        #region Property: UnitCategory
        /// <summary>
        /// The unit category of the attribute.
        /// </summary>
        private UnitCategoryBase unitCategory = null;

        /// <summary>
        /// Gets the unit category of the attribute.
        /// </summary>
        public UnitCategoryBase UnitCategory
        {
            get
            {
                return unitCategory;
            }
        }
        #endregion Property: UnitCategory

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AttributeBase(Attribute attribute)
        /// <summary>
        /// Create an attribute base from the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to create an attribute base from.</param>
        protected internal AttributeBase(Attribute attribute)
            : base(attribute)
        {
            if (attribute != null)
            {
                this.attributeType = attribute.AttributeType;
                this.valueType = attribute.ValueType;
                this.unitCategory = BaseManager.Current.GetBase<UnitCategoryBase>(attribute.UnitCategory);
            }
        }
        #endregion Constructor: AttributeBase(Attribute attribute)

        #endregion Method Group: Constructors

    }
    #endregion Class: AttributeBase

    #region Class: AttributeValuedBase
    /// <summary>
    /// A base of a valued attribute.
    /// </summary>
    public class AttributeValuedBase : AbstractionValuedBase
    {

        #region Properties and Fields

        #region Property: AttributeValued
        /// <summary>
        /// Gets the valued attribute of which this is an attribute base.
        /// </summary>
        protected internal AttributeValued AttributeValued
        {
            get
            {
                return this.NodeValued as AttributeValued;
            }
        }
        #endregion Property: AttributeValued

        #region Property: AttributeBase
        /// <summary>
        /// Gets the attribute base.
        /// </summary>
        public AttributeBase AttributeBase
        {
            get
            {
                return this.NodeBase as AttributeBase;
            }
        }
        #endregion Property: AttributeBase

        #region Property: Value
        /// <summary>
        /// The value of the attribute.
        /// </summary>
        private ValueBase val = null;

        /// <summary>
        /// Gets the value of the attribute.
        /// </summary>
        public ValueBase Value
        {
            get
            {
                if (val == null)
                {
                    LoadValue();
                    if (val == null)
                        val = new NumericalValueBase(SemanticsSettings.Values.Value);
                }
                return val;
            }
        }

        /// <summary>
        /// Loads the value.
        /// </summary>
        private void LoadValue()
        {
            if (this.AttributeValued != null)
            {
                // Get the value
                val = BaseManager.Current.GetBase<ValueBase>(this.AttributeValued.Value);

                NumericalValueBase numericalValueBase = val as NumericalValueBase;
                if (numericalValueBase != null)
                {
                    // Set the unit to the base unit if no unit has been selected before, or reset it when there is no unit category
                    if (numericalValueBase.Unit == null && this.AttributeBase != null && this.AttributeBase.UnitCategory != null)
                        numericalValueBase.Unit = this.AttributeBase.UnitCategory.BaseUnit;
                    else if (this.AttributeBase.UnitCategory == null)
                        numericalValueBase.Unit = null;
                }
            }
        }
        #endregion Property: Value

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AttributeValuedBase(AttributeValued attributeValued)
        /// <summary>
        /// Create an attribute base from the given valued attribute.
        /// </summary>
        /// <param name="attributeValued">The valued attribute to create an attribute base from.</param>
        protected internal AttributeValuedBase(AttributeValued attributeValued)
            : base(attributeValued)
        {
            if (attributeValued != null)
            {
                if (BaseManager.PreloadProperties)
                    LoadValue();
            }
        }
        #endregion Constructor: AttributeValuedBase(AttributeValued attributeValued)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            if (this.NodeBase != null)
            {
                string returnString = this.NodeBase.DefaultName + " {";
                returnString += this.Value.ToString() + "}";
                return returnString;
            }
            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: AttributeValuedBase

    #region Class: AttributeConditionBase
    /// <summary>
    /// A condition on an attribute.
    /// </summary>
    public class AttributeConditionBase : AbstractionConditionBase
    {

        #region Properties and Fields

        #region Property: AttributeCondition
        /// <summary>
        /// Gets the attribute condition of which this is an attribute condition base.
        /// </summary>
        protected internal AttributeCondition AttributeCondition
        {
            get
            {
                return this.Condition as AttributeCondition;
            }
        }
        #endregion Property: AttributeCondition

        #region Property: AttributeBase
        /// <summary>
        /// Gets the attribute base of which this is an attribute condition base.
        /// </summary>
        public AttributeBase AttributeBase
        {
            get
            {
                return this.NodeBase as AttributeBase;
            }
        }
        #endregion Property: AttributeBase

        #region Property: Value
        /// <summary>
        /// The required value.
        /// </summary>
        private ValueConditionBase val = null;

        /// <summary>
        /// Gets the required value.
        /// </summary>
        public ValueConditionBase Value
        {
            get
            {
                if (val == null)
                    LoadValue();
                return val;
            }
        }

        /// <summary>
        /// Loads the value.
        /// </summary>
        private void LoadValue()
        {
            if (this.AttributeCondition != null)
                val = BaseManager.Current.GetBase<ValueConditionBase>(this.AttributeCondition.Value);
        }
        #endregion Property: Value

        #region Property: UnitCategory
        /// <summary>
        /// The required unit category.
        /// </summary>
        private UnitCategoryBase unitCategory = null;

        /// <summary>
        /// Gets the required unit category.
        /// </summary>
        public UnitCategoryBase UnitCategory
        {
            get
            {
                return unitCategory;
            }
        }
        #endregion Property: UnitCategory

        #region Property: UnitCategorySign
        /// <summary>
        /// The sign for the unit category in the condition.
        /// </summary>
        private EqualitySign? unitCategorySign = null;

        /// <summary>
        /// Gets the sign for the unit category in the condition.
        /// </summary>
        public EqualitySign? UnitCategorySign
        {
            get
            {
                return unitCategorySign;
            }
        }
        #endregion Property: UnitCategorySign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AttributeConditionBase(AttributeCondition attributeCondition)
        /// <summary>
        /// Create a new base of the given attribute condition.
        /// </summary>
        /// <param name="attributeCondition">The attribute condition to create a base of.</param>
        protected internal AttributeConditionBase(AttributeCondition attributeCondition)
            : base(attributeCondition)
        {
            if (attributeCondition != null)
            {
                this.unitCategory = BaseManager.Current.GetBase<UnitCategoryBase>(attributeCondition.UnitCategory);
                this.unitCategorySign = attributeCondition.UnitCategorySign;

                if (BaseManager.PreloadProperties)
                    LoadValue();
            }
        }
        #endregion Constructor: AttributeConditionBase(AttributeCondition attributeCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: AttributeConditionBase

    #region Class: AttributeChangeBase
    /// <summary>
    /// A change on an attribute.
    /// </summary>
    public class AttributeChangeBase : AbstractionChangeBase
    {

        #region Properties and Fields

        #region Property: AttributeChange
        /// <summary>
        /// Gets the attribute change of which this is an attribute change base.
        /// </summary>
        protected internal AttributeChange AttributeChange
        {
            get
            {
                return this.Change as AttributeChange;
            }
        }
        #endregion Property: AttributeChange

        #region Property: AttributeBase
        /// <summary>
        /// Gets the affected attribute base.
        /// </summary>
        public AttributeBase AttributeBase
        {
            get
            {
                return this.NodeBase as AttributeBase;
            }
        }
        #endregion Property: AttributeBase

        #region Property: Value
        /// <summary>
        /// The value to change to.
        /// </summary>
        private ValueChangeBase val = null;

        /// <summary>
        /// Gets the value to change to.
        /// </summary>
        public ValueChangeBase Value
        {
            get
            {
                if (val == null)
                    LoadValue();
                return val;
            }
        }

        /// <summary>
        /// Loads the value.
        /// </summary>
        private void LoadValue()
        {
            if (this.AttributeChange != null)
                val = BaseManager.Current.GetBase<ValueChangeBase>(this.AttributeChange.Value);
        }
        #endregion Property: Value

        #region Property: UnitCategory
        /// <summary>
        /// The unit category to change to.
        /// </summary>
        private UnitCategoryBase unitCategory = null;

        /// <summary>
        /// Gets the unit category to change to.
        /// </summary>
        public UnitCategoryBase UnitCategory
        {
            get
            {
                return unitCategory;
            }
        }
        #endregion Property: UnitCategory

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AttributeChangeBase(AttributeChange attributeChange)
        /// <summary>
        /// Creates a base of an attribute change.
        /// </summary>
        /// <param name="attributeChange">The attribute change to create a base of.</param>
        protected internal AttributeChangeBase(AttributeChange attributeChange)
            : base(attributeChange)
        {
            if (attributeChange != null)
            {
                this.unitCategory = BaseManager.Current.GetBase<UnitCategoryBase>(attributeChange.UnitCategory);

                if (BaseManager.PreloadProperties)
                    LoadValue();
            }
        }
        #endregion Constructor: AttributeChangeBase(AttributeChange attributeChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: AttributeChangeBase

}
