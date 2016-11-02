/**************************************************************************
 * 
 * AttributeInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.ComponentModel;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;
using ValueType = Semantics.Utilities.ValueType;

namespace SemanticsEngine.Abstractions
{

    #region Class: AttributeInstance
    /// <summary>
    /// An instance of an attribute.
    /// </summary>
    public class AttributeInstance : AbstractionInstance
    {

        #region Properties and Fields

        #region Property: AttributeBase
        /// <summary>
        /// Gets the attribute base of which this is an attribute instance.
        /// </summary>
        public AttributeBase AttributeBase
        {
            get
            {
                if (this.AttributeValuedBase != null)
                    return this.AttributeValuedBase.AttributeBase as AttributeBase;
                return null;
            }
        }
        #endregion Property: AttributeBase

        #region Property: AttributeValuedBase
        /// <summary>
        /// Gets the valued attribute base of which this is an attribute instance.
        /// </summary>
        public AttributeValuedBase AttributeValuedBase
        {
            get
            {
                return this.Base as AttributeValuedBase;
            }
        }
        #endregion Property: AttributeValuedBase

        #region Property: AttributeType
        /// <summary>
        /// Gets the type of the attribute.
        /// </summary>
        public AttributeType AttributeType
        {
            get
            {
                if (this.AttributeValuedBase != null && this.AttributeValuedBase.AttributeBase != null)
                    return this.AttributeValuedBase.AttributeBase.AttributeType;

                return default(AttributeType);
            }
        }
        #endregion Property: AttributeType

        #region Property: Value
        /// <summary>
        /// Gets the value of the attribute.
        /// </summary>
        public ValueInstance Value
        {
            get
            {
                if (this.AttributeValuedBase != null)
                {
                    // Return the locally modified value, or create a new instance from the base and subscribe to possible changes
                    ValueInstance val = GetProperty<ValueInstance>("Value", null);
                    if (val == null)
                    {
                        val = InstanceManager.Current.Create<ValueInstance>(this.AttributeValuedBase.Value);
                        if (val != null)
                        {
                            if (valueChanged == null)
                                valueChanged = new PropertyChangedEventHandler(val_PropertyChanged);

                            val.PropertyChanged += valueChanged;

                            // Store random values, so they are not re-created with another random value
                            if (val.ValueBase.IsRandom)
                                SetProperty("Value", val);

                            // Set the variable instance holder to this owner
                            val.VariableInstanceHolder = this.Owner;
                        }
                    }
                    return val;
                }
                
                return null;
            }
            protected set
            {
                if (this.Value != value)
                {
                    if (valueChanged == null)
                        valueChanged = new PropertyChangedEventHandler(val_PropertyChanged);

                    this.Value.PropertyChanged -= valueChanged;

                    SetProperty("Value", value);

                    if (value != null)
                        value.PropertyChanged += valueChanged;
                }
            }
        }

        /// <summary>
        /// A handler for a changed value.
        /// </summary>
        private PropertyChangedEventHandler valueChanged = null;

        /// <summary>
        /// If the value changes, add the property and the modified value to the modifications table.
        /// </summary>
        private void val_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != null)
                SetProperty("Value", sender);
        }
        #endregion Property: Value

        #region Property: ValueType
        /// <summary>
        /// Gets the value type of the attribute.
        /// </summary>
        public ValueType ValueType
        {
            get
            {
                if (this.AttributeValuedBase != null && this.AttributeValuedBase.AttributeBase != null)
                    return this.AttributeValuedBase.AttributeBase.ValueType;

                return default(ValueType);
            }
        }
        #endregion Property: ValueType

        #region Property: UnitCategory
        /// <summary>
        /// Gets the unit category of the attribute.
        /// </summary>
        public UnitCategoryBase UnitCategory
        {
            get
            {
                if (this.AttributeBase != null)
                    return GetProperty<UnitCategoryBase>("UnitCategory", this.AttributeBase.UnitCategory);

                return null;
            }
            protected set
            {
                if (this.UnitCategory != value)
                    SetProperty("UnitCategory", value);
            }
        }
        #endregion Property: UnitCategory

        #region Property: Owner
        /// <summary>
        /// The owner.
        /// </summary>
        private EntityInstance owner = null;

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        internal EntityInstance Owner
        {
            get
            {
                return owner;
            }
            set
            {
                owner = value;
            }
        }
        #endregion Property: Owner
		
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AttributeInstance(AttributeValuedBase attributeValuedBase)
        /// <summary>
        /// Creates a new attribute instance from the given valued attribute base.
        /// </summary>
        /// <param name="attributeValuedBase">The valued attribute base to create the attribute instance from.</param>
        internal AttributeInstance(AttributeValuedBase attributeValuedBase)
            : base(attributeValuedBase)
        {
        }
        #endregion Constructor: AttributeInstance(AttributeValuedBase attributeValuedBase)

        #region Constructor: AttributeInstance(AttributeInstance attributeInstance)
        /// <summary>
        /// Clones the attribute instance.
        /// </summary>
        /// <param name="attributeInstance">The attribute instance to clone.</param>
        protected internal AttributeInstance(AttributeInstance attributeInstance)
            : base(attributeInstance)
        {
            if (attributeInstance != null)
            {
                this.Value = attributeInstance.Value.Clone();
                this.UnitCategory = attributeInstance.UnitCategory;
            }
        }
        #endregion Constructor: AttributeInstance(AttributeInstance attributeInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase nodeConditionBase)
        /// <summary>
        /// Check whether the given condition satisfies the attribute instance.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the attribute instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the attribute instance.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null && base.Satisfies(conditionBase, iVariableInstanceHolder))
            {
                AttributeConditionBase attributeConditionBase = conditionBase as AttributeConditionBase;
                if (attributeConditionBase != null)
                {
                    // Check whether the properties are satisfied
                    if (attributeConditionBase.Value == null || (this.Value != null && this.Value.Satisfies(attributeConditionBase.Value, iVariableInstanceHolder)) &&
                        (attributeConditionBase.UnitCategorySign == null || attributeConditionBase.UnitCategory == null || Toolbox.Compare(this.UnitCategory, (EqualitySign)attributeConditionBase.UnitCategorySign, attributeConditionBase.UnitCategory)))
                    {
                        return true;
                    }
                }
                else
                    return true;
            }
            return false;
        }
        #endregion Method: Satisfies(ConditionBase nodeConditionBase)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the attribute instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the attribute instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null && base.Apply(changeBase, iVariableInstanceHolder))
            {
                // Only mutable attributes can be changed
                if (this.AttributeType == AttributeType.Mutable)
                {
                    // Attribute change
                    AttributeChangeBase attributeChangeBase = changeBase as AttributeChangeBase;
                    if (attributeChangeBase != null)
                    {
                        // Apply the changes
                        if (attributeChangeBase.Value != null)
                            this.Value.Apply(attributeChangeBase.Value, iVariableInstanceHolder);
                        if (attributeChangeBase.UnitCategory != null)
                            this.UnitCategory = attributeChangeBase.UnitCategory;
                    }
                }
                else
                    return false;

                return true;
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.DefaultName + " = " + this.Value.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: AttributeInstance

}
