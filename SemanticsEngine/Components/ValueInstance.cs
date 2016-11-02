/**************************************************************************
 * 
 * ValueInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using Common;
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: ValueInstance
    /// <summary>
    /// An instance of a value.
    /// </summary>
    public abstract class ValueInstance : Instance
    {

        #region Properties and Fields

        #region Property: ValueBase
        /// <summary>
        /// Gets the value base of which this is a value instance.
        /// </summary>
        public ValueBase ValueBase
        {
            get
            {
                return this.Base as ValueBase;
            }
        }
        #endregion Property: ValueBase

        #region Property: VariableInstanceHolder
        /// <summary>
        /// The variable instance holder.
        /// </summary>
        private IVariableInstanceHolder iVariableInstanceHolder = null;

        /// <summary>
        /// Gets or sets the variable instance holder.
        /// </summary>
        internal IVariableInstanceHolder VariableInstanceHolder
        {
            get
            {
                return iVariableInstanceHolder;
            }
            set
            {
                iVariableInstanceHolder = value;
            }
        }
        #endregion Property: VariableInstanceHolder
		
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ValueInstance(ValueBase valueBase)
        /// <summary>
        /// Creates a new value instance from the given value base.
        /// </summary>
        /// <param name="valueBase">The value base to create the value instance from.</param>
        protected ValueInstance(ValueBase valueBase)
            : base(valueBase)
        {
        }
        #endregion Constructor: ValueInstance(ValueBase valueBase)

        #region Constructor: ValueInstance(ValueInstance valueInstance)
        /// <summary>
        /// Clones a value instance.
        /// </summary>
        /// <param name="valueInstance">The value instance to clone.</param>
        protected ValueInstance(ValueInstance valueInstance)
            : base(valueInstance)
        {
        }
        #endregion Constructor: ValueInstance(ValueInstance valueInstance)

        #region Method: Create(ValueBase valueBase)
        /// <summary>
        /// Create a value instance from the given value base.
        /// </summary>
        /// <param name="valueBase">The value base to create an instance of.</param>
        /// <returns>An instance of the given value base.</returns>
        public static ValueInstance Create(ValueBase valueBase)
        {
            if (valueBase != null)
            {
                NumericalValueBase numericalValueBase = valueBase as NumericalValueBase;
                if (numericalValueBase != null)
                    return new NumericalValueInstance(numericalValueBase);

                BoolValueBase boolValueBase = valueBase as BoolValueBase;
                if (boolValueBase != null)
                    return new BoolValueInstance(boolValueBase);

                StringValueBase stringValueBase = valueBase as StringValueBase;
                if (stringValueBase != null)
                    return new StringValueInstance(stringValueBase);

                VectorValueBase vectorValueBase = valueBase as VectorValueBase;
                if (vectorValueBase != null)
                    return new VectorValueInstance(vectorValueBase);
            }
            return null;
        }
        #endregion Method: Create(ValueBase valueBase)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the value instance.
        /// </summary>
        public new ValueInstance Clone()
        {
            return base.Clone() as ValueInstance;
        }
        #endregion Method: Clone()

        #region Method: Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the value.
        /// </summary>
        /// <param name="valueConditionBase">The value condition to compare to the value.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the value.</returns>
        public abstract bool Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder);
        #endregion Method: Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the value.
        /// </summary>
        /// <param name="valueChangeBase">The change to apply to the value.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        public abstract bool Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder);
        #endregion Method: Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: ReplaceBy(ValueInstance valueInstance)
        /// <summary>
        /// Replace the values of this value instance by the given one.
        /// </summary>
        /// <param name="valueInstance">The value instance to get the values from.</param>
        protected internal abstract void ReplaceBy(ValueInstance valueInstance);
        #endregion Method: ReplaceBy(ValueInstance valueInstance)

        #endregion Method Group: Other

    }
    #endregion Class: ValueInstance

    #region Class: BoolValueInstance
    /// <summary>
    /// A value that consists of a bool.
    /// </summary>
    public class BoolValueInstance : ValueInstance
    {

        #region Properties and Fields

        #region Property: BoolValueBase
        /// <summary>
        /// Gets the bool value base of which this is a bool value instance.
        /// </summary>
        public BoolValueBase BoolValueBase
        {
            get
            {
                return this.Base as BoolValueBase;
            }
        }
        #endregion Property: BoolValueBase

        #region Property: Bool
        /// <summary>
        /// The custom bool.
        /// </summary>
        private bool customBool = SemanticsSettings.Values.Boolean;

        /// <summary>
        /// The bool variable instance.
        /// </summary>
        private BoolVariableInstance boolVariableInstance = null;

        /// <summary>
        /// Gets or sets the boolean value.
        /// </summary>
        public bool Bool
        {
            get
            {
                if (this.BoolValueBase != null)
                {
                    bool boolean;
                    if (TryGetProperty<bool>("Bool", out boolean))
                        return boolean;
                    else
                    {
                        if (this.BoolValueBase.Variable != null)
                        {
                            if (boolVariableInstance == null)
                                boolVariableInstance = new BoolVariableInstance(this.BoolValueBase.Variable, this.VariableInstanceHolder);
                            return boolVariableInstance.Bool;
                        }
                        else
                            return this.BoolValueBase.Bool;
                    }
                }
                else
                    return customBool;
            }
            set
            {
                if (this.Bool != value)
                {
                    if (this.BoolValueBase != null)
                        SetProperty("Bool", value);
                    else
                    {
                        customBool = value;
                        NotifyPropertyChanged("Bool");
                    }
                }
            }
        }
        #endregion Property: Bool

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: BoolValueInstance(BoolValueBase boolValueBase)
        /// <summary>
        /// Creates a new bool value instance from the given bool value base.
        /// </summary>
        /// <param name="boolValueBase">The bool value base to create the bool value instance from.</param>
        internal BoolValueInstance(BoolValueBase boolValueBase)
            : base(boolValueBase)
        {
            if (boolValueBase != null)
            {
                // Set the possible random value
                if (boolValueBase.IsRandom)
                    this.Bool = RandomNumber.RandomF() > 0.5f;
            }
        }
        #endregion Constructor: BoolValueInstance(BoolValueBase boolValueBase)

        #region Constructor: BoolValueInstance(BoolValueBase boolValueBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Creates a new bool value instance from the given bool value base.
        /// </summary>
        /// <param name="boolValueBase">The bool value base to create the bool value instance from.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        internal BoolValueInstance(BoolValueBase boolValueBase, IVariableInstanceHolder iVariableInstanceHolder)
            : this(boolValueBase)
        {
            this.VariableInstanceHolder = iVariableInstanceHolder;
        }
        #endregion Constructor: BoolValueInstance(BoolValueBase boolValueBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Constructor: BoolValueInstance(BoolValueInstance boolValueInstance)
        /// <summary>
        /// Clones the bool value.
        /// </summary>
        /// <param name="boolValueInstance">The bool value to clone.</param>
        public BoolValueInstance(BoolValueInstance boolValueInstance)
            : base(boolValueInstance)
        {
            if (boolValueInstance != null)
                this.Bool = boolValueInstance.Bool;
        }
        #endregion Constructor: BoolValueInstance(BoolValueInstance boolValueInstance)

        #region Constructor: BoolValueInstance(bool value)
        /// <summary>
        /// Creates a bool value from the given bool.
        /// </summary>
        /// <param name="value">The bool to create a bool value from.</param>
        public BoolValueInstance(bool value)
            : base(null as ValueBase)
        {
            this.Bool = value;
        }
        #endregion Constructor: BoolValueInstance(bool value)

        #endregion Method Group: Constructors

        #region Method Group: Operator overloading

        #region Method: operator ==(BoolValueInstance value1, BoolValueInstance value2)
        /// <summary>
        /// Checks whether the first value is equal to the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns whether the first value is equal to the second value.</returns>
        public static bool operator ==(BoolValueInstance value1, BoolValueInstance value2)
        {
            // If both are null, or both are same instance, return true
            if (Object.ReferenceEquals(value1, value2))
                return true;

            // If one is null, but not both, return false
            if (((object)value1 == null) || ((object)value2 == null))
                return false;

            // Compare both booleans
            return value1.Bool == value2.Bool;
        }
        #endregion Method: operator ==(BoolValueInstance value1, BoolValueInstance value2)

        #region Method: operator !=(BoolValueInstance value1, BoolValueInstance value2)
        /// <summary>
        /// Checks whether the first value is not equal to the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns whether the first value is not equal to the second value.</returns>
        public static bool operator !=(BoolValueInstance value1, BoolValueInstance value2)
        {
            return !(value1 == value2);
        }
        #endregion Method: operator !=(BoolValueInstance value1, BoolValueInstance value2)

        #region Method: Equals(object obj)
        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current System.Object.</param>
        /// <returns>True when equal, false when inequal.</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        #endregion Method: Equals(object obj)

        #region Method: GetHashCode()
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Method: GetHashCode()

        #endregion Method Group: Operator overloading

        #region Method Group: Other

        #region Method: Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the value.
        /// </summary>
        /// <param name="valueConditionBase">The value condition to compare to the value.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the value.</returns>
        public override bool Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (valueConditionBase != null)
            {
                BoolValueConditionBase boolValueConditionBase = valueConditionBase as BoolValueConditionBase;
                if (boolValueConditionBase != null)
                {
                    bool? boolean = boolValueConditionBase.GetValue(iVariableInstanceHolder);
                    return boolean == null || Toolbox.Compare(this.Bool, EqualitySign.Equal, (bool)boolean);
                }
            }
            return false;
        }
        #endregion Method: Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the value.
        /// </summary>
        /// <param name="valueChangeBase">The change to apply to the value.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        public override bool Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (valueChangeBase != null)
            {
                BoolValueChangeBase boolValueChangeBase = valueChangeBase as BoolValueChangeBase;
                if (boolValueChangeBase != null)
                {
                    if (boolValueChangeBase.Reverse)
                        this.Bool = !this.Bool;
                    else
                    {
                        bool? boolean = boolValueChangeBase.GetValue(iVariableInstanceHolder);
                        if (boolean != null)
                            this.Bool = (bool)boolean;
                    }
                }
                return true;
            }
            return false;
        }
        #endregion Method: Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: ReplaceBy(ValueInstance valueInstance)
        /// <summary>
        /// Replace the values of this value instance by the given one.
        /// </summary>
        /// <param name="valueInstance">The value instance to get the values from.</param>
        protected internal override void ReplaceBy(ValueInstance valueInstance)
        {
            BoolValueInstance boolValueInstance = valueInstance as BoolValueInstance;
            if (boolValueInstance != null)
                this.Bool = boolValueInstance.Bool;
        }
        #endregion Method: ReplaceBy(ValueInstance valueInstance)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.Bool.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: BoolValueInstance

    #region Class: NumericalValueInstance
    /// <summary>
    /// A numerical value, optionally accompanied by a prefix and a unit.
    /// </summary>
    public class NumericalValueInstance : ValueInstance
    {

        #region Properties and Fields

        #region Property: NumericalValueBase
        /// <summary>
        /// Gets the numerical value base of which this is a numerical value instance.
        /// </summary>
        public NumericalValueBase NumericalValueBase
        {
            get
            {
                return this.Base as NumericalValueBase;
            }
        }
        #endregion Property: NumericalValueBase

        #region Property: Value
        /// <summary>
        /// The custom numerical value.
        /// </summary>
        private float customValue = SemanticsSettings.Values.Value;

        /// <summary>
        /// The variable instance.
        /// </summary>
        private VariableInstance variableInstance = null;

        /// <summary>
        /// Gets or sets the numerical value.
        /// </summary>
        public float Value
        {
            get
            {
                if (this.NumericalValueBase != null)
                {
                    float val;
                    if (TryGetProperty<float>("Value", out val))
                        return val;
                    else
                    {
                        if (this.NumericalValueBase.Variable != null)
                        {
                            if (variableInstance == null)
                            {
                                if (this.NumericalValueBase.Variable is NumericalVariableBase)
                                    variableInstance = new NumericalVariableInstance((NumericalVariableBase)this.NumericalValueBase.Variable, this.VariableInstanceHolder);
                                else if (this.NumericalValueBase.Variable is TermVariableBase)
                                    variableInstance = new TermVariableInstance((TermVariableBase)this.NumericalValueBase.Variable, this.VariableInstanceHolder);
                            }
                            if (variableInstance != null)
                            {
                                if (variableInstance is NumericalVariableInstance)
                                    return ((NumericalVariableInstance)variableInstance).Value;
                                else if (variableInstance is TermVariableInstance)
                                    return ((TermVariableInstance)variableInstance).Value;
                            }
                        }
                        return this.NumericalValueBase.Value;
                    }
                }
                else
                    return customValue;
            }
            set
            {
                if (this.Value != value)
                {
                    // Make sure the value does not exceed the min or max
                    float newValue = Math.Max(this.Min, Math.Min((float)value, this.Max));

                    this.baseValue = null;

                    // Set the value and reset the base value
                    if (this.NumericalValueBase != null)
                        SetProperty("Value", newValue);
                    else
                    {
                        customValue = newValue;
                        NotifyPropertyChanged("Value");
                    }
                }
            }
        }
        #endregion Property: Value

        #region Property: Prefix
        /// <summary>
        /// Gets the prefix of the value.
        /// </summary>
        public Prefix Prefix
        {
            get
            {
                if (this.NumericalValueBase != null)
                    return GetProperty<Prefix>("Prefix", this.NumericalValueBase.Prefix);
                return default(Prefix);
            }
            protected internal set
            {
                if (this.Prefix != value)
                {
                    SetProperty("Prefix", value);
                    baseValue = null;
                }
            }
        }
        #endregion Property: Prefix

        #region Property: Unit
        /// <summary>
        /// Gets the optional unit of this value.
        /// </summary>
        public UnitBase Unit
        {
            get
            {
                if (this.NumericalValueBase != null)
                    return GetProperty<UnitBase>("Unit", this.NumericalValueBase.Unit);
                return null;
            }
            protected internal set
            {
                if (this.Unit != value)
                {
                    SetProperty("Unit", value);
                    baseValue = null;
                }
            }
        }
        #endregion Property: Unit

        #region Property: Min
        /// <summary>
        /// The minimum value.
        /// </summary>
        private float customMin = SemanticsSettings.Values.MinValue;
        
        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        public float Min
        {
            get
            {
                if (this.NumericalValueBase != null)
                    return GetProperty<float>("Min", this.NumericalValueBase.Min);
                return customMin;
            }
            set
            {
                if (this.Min != value)
                {
                    // Make sure the minimum does not exceed the maximum
                    float min = 0;
                    if (!ignoreMinExceeding)
                        min = Math.Min((float)value, this.Max);
                    else
                        min = value;

                    if (this.NumericalValueBase != null)
                        SetProperty("Min", min);
                    else
                        customMin = min;

                    // Make sure the value does not exceed the minimum
                    if (this.Value < min)
                        this.Value = min;
                }
            }
        }

        /// <summary>
        /// Indicates whether an exceeding correction for the minimum should be applied.
        /// </summary>
        private bool ignoreMinExceeding = false;
        #endregion Property: Min

        #region Property: Max
        /// <summary>
        /// The maximum value.
        /// </summary>
        private float customMax = SemanticsSettings.Values.MaxValue;
        
        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        public float Max
        {
            get
            {
                if (this.NumericalValueBase != null)
                    return GetProperty<float>("Max", this.NumericalValueBase.Max);
                return customMax;
            }
            set
            {
                if (this.Max != value)
                {
                    // Make sure the maximum does not exceed the minimum
                    float max = Math.Max((float)value, this.Min);

                    if (this.NumericalValueBase != null)
                        SetProperty("Max", max);
                    else
                        customMax = max;

                    // Make sure the value does not exceed the maximum
                    if (this.Value > max)
                        this.Value = max;
                }
            }
        }
        #endregion Property: Max

        #region Property: BaseValue
        /// <summary>
        /// The value that is converted to the base unit, without a prefix.
        /// </summary>
        private float? baseValue = null;

        /// <summary>
        /// Gets the value that is converted to the base unit, without a prefix.
        /// </summary>
        public float BaseValue
        {
            get
            {
                if (baseValue == null)
                    baseValue = Utils.GetBaseValue(this.Value, this.Prefix, this.Unit);
                return (float)baseValue;
            }
        }
        #endregion: Property: BaseValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NumericalValueInstance(NumericalValueBase numericalValueBase)
        /// <summary>
        /// Creates a new numerical value instance from the given numerical value base.
        /// </summary>
        /// <param name="numericalValueBase">The numerical value base to create the numerical value instance from.</param>
        internal NumericalValueInstance(NumericalValueBase numericalValueBase)
            : base(numericalValueBase)
        {
            if (numericalValueBase != null)
            {
                // Set the possible random value
                if (numericalValueBase.IsRandom)
                {
                    float randomMin = 0;
                    float randomMax = 0;
                    if (numericalValueBase.RandomMin != null)
                        randomMin = (float)numericalValueBase.RandomMin;
                    else
                        randomMin = numericalValueBase.Min;
                    if (numericalValueBase.RandomMax != null)
                        randomMax = (float)numericalValueBase.RandomMax;
                    else
                        randomMax = numericalValueBase.Max;
                    this.Value = RandomNumber.RandomF(randomMin, randomMax);
                }
            }
        }
        #endregion Constructor: NumericalValueInstance(NumericalValueBase numericalValueBase)

        #region Constructor: NumericalValueInstance(NumericalValueBase numericalValueBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Creates a new numerical value instance from the given numerical value base.
        /// </summary>
        /// <param name="numericalValueBase">The numerical value base to create the numerical value instance from.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        internal NumericalValueInstance(NumericalValueBase numericalValueBase, IVariableInstanceHolder iVariableInstanceHolder)
            : this(numericalValueBase)
        {
            this.VariableInstanceHolder = iVariableInstanceHolder;
        }
        #endregion Constructor: NumericalValueInstance(NumericalValueBase numericalValueBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Constructor: NumericalValueInstance(NumericalValueInstance numericalValueInstance)
        /// <summary>
        /// Clones the numerical value.
        /// </summary>
        /// <param name="numericalValueInstance">The numerical value to clone.</param>
        public NumericalValueInstance(NumericalValueInstance numericalValueInstance)
            : base(numericalValueInstance)
        {
            if (numericalValueInstance != null)
            {
                this.ignoreMinExceeding = true;
                this.Min = numericalValueInstance.Min;
                this.ignoreMinExceeding = false;
                this.Max = numericalValueInstance.Max;
                this.Prefix = numericalValueInstance.Prefix;
                this.Unit = numericalValueInstance.Unit;
                this.Value = numericalValueInstance.Value;
            }
        }
        #endregion Constructor: NumericalValueInstance(NumericalValueInstance numericalValueInstance)

        #region Constructor: NumericalValueInstance(float value)
        /// <summary>
        /// Creates a numerical value from the given float.
        /// </summary>
        /// <param name="value">The float to create a numerical value from.</param>
        public NumericalValueInstance(float value)
            : this(value, Prefix.None, null, SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MaxValue)
        {
        }

        /// <summary>
        /// Creates a numerical value instance from the given float.
        /// </summary>
        /// <param name="val">The float to create a numerical value instance from.</param>
        /// <returns>A numerical value instance from the given float.</returns>
        public static implicit operator NumericalValueInstance(float val)
        {
            return new NumericalValueInstance(val);
        }
        #endregion Constructor: NumericalValueInstance(float value)

        #region Constructor: NumericalValueInstance(float value, UnitBase unit)
        /// <summary>
        /// Creates a numerical value from the given float and unit.
        /// </summary>
        /// <param name="value">The float of the numerical value.</param>
        /// <param name="unit">The unit of the numerical value.</param>
        public NumericalValueInstance(float value, UnitBase unit)
            : this(value, Prefix.None, unit, SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MaxValue)
        {
        }
        #endregion Constructor: NumericalValueInstance(float value, UnitBase unit)

        #region Constructor: NumericalValueInstance(float value, Prefix prefix, UnitBase unit)
        /// <summary>
        /// Creates a numerical value from the given float, prefix and unit.
        /// </summary>
        /// <param name="value">The float of the numerical value.</param>
        /// <param name="prefix">The prefix of the numerical value.</param>
        /// <param name="unit">The unit of the numerical value.</param>
        public NumericalValueInstance(float value, Prefix prefix, UnitBase unit)
            : this(value, prefix, unit, SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MaxValue)
        {
        }
        #endregion Constructor: NumericalValueInstance(float value, Prefix prefix, UnitBase unit)

        #region Constructor: NumericalValueInstance(float value, Prefix prefix, UnitBase unit, float min, float max)
        /// <summary>
        /// Creates a numerical value from the given float, prefix, unit, min, and max.
        /// </summary>
        /// <param name="value">The float of the numerical value.</param>
        /// <param name="prefix">The prefix of the numerical value.</param>
        /// <param name="unit">The unit of the numerical value.</param>
        /// <param name="min">The minimum of the numerical value.</param>
        /// <param name="max">The maximum of the numerical value.</param>
        public NumericalValueInstance(float value, Prefix prefix, UnitBase unit, float min, float max)
            : base(null as ValueBase)
        {
            this.ignoreMinExceeding = true;
            this.Min = min;
            this.ignoreMinExceeding = false;
            this.Max = max;
            this.Prefix = prefix;
            this.Unit = unit;
            this.Value = value;
        }
        #endregion Constructor: NumericalValueInstance(float value, Prefix prefix, UnitBase unit, float min, float max)

        #endregion Method Group: Constructors

        #region Method Group: Operator overloading

        #region Method: operator ==(NumericalValueInstance value1, NumericalValueInstance value2)
        /// <summary>
        /// Checks whether the first value is equal to the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns whether the first value is equal to the second value.</returns>
        public static bool operator ==(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            // If both are null, or both are same instance, return true
            if (Object.ReferenceEquals(value1, value2))
                return true;

            // If one is null, but not both, return false
            if (((object)value1 == null) || ((object)value2 == null))
                return false;

            return Toolbox.Compare(value1.BaseValue, EqualitySignExtended.Equal, value2.BaseValue);
        }

        /// <summary>
        /// Checks whether the first value is equal to the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns whether the first value is equal to the second value.</returns>
        public static bool Compare(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            return value1 == value2;
        }
        #endregion Method: operator ==(NumericalValueInstance value1, NumericalValueInstance value2)

        #region Method: operator !=(NumericalValueInstance value1, NumericalValueInstance value2)
        /// <summary>
        /// Checks whether the first value is not equal to the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns whether the first value is not equal to the second value.</returns>
        public static bool operator !=(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            return !(value1 == value2);
        }
        #endregion Method: operator !=(NumericalValueInstance value1, NumericalValueInstance value2)

        #region Method: operator >(NumericalValueInstance value1, NumericalValueInstance value2)
        /// <summary>
        /// Checks whether the first value is greater than the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The first value.</param>
        /// <returns>Returns whether the first value is greater than the second value.</returns>
        public static bool operator >(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");

            if (value2 == null)
                throw new ArgumentNullException("value2");

            return Toolbox.Compare(value1.BaseValue, EqualitySignExtended.Greater, value2.BaseValue);
        }
        #endregion Method: operator >(NumericalValueInstance value1, NumericalValueInstance value2)

        #region Method: operator >=(NumericalValueInstance value1, NumericalValueInstance value2)
        /// <summary>
        /// Checks whether the first value is greater than or equal to the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The first value.</param>
        /// <returns>Returns whether the first value is greater than or equal to the second value.</returns>
        public static bool operator >=(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");

            if (value2 == null)
                throw new ArgumentNullException("value2");

            return Toolbox.Compare(value1.BaseValue, EqualitySignExtended.GreaterOrEqual, value2.BaseValue);
        }
        #endregion Method: operator >=(NumericalValueInstance value1, NumericalValueInstance value2)

        #region Method: operator <(NumericalValueInstance value1, NumericalValueInstance value2)
        /// <summary>
        /// Checks whether the first value is lower than the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The first value.</param>
        /// <returns>Returns whether the first value is lower than the second value.</returns>
        public static bool operator <(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");

            if (value2 == null)
                throw new ArgumentNullException("value2");

            return Toolbox.Compare(value1.BaseValue, EqualitySignExtended.Lower, value2.BaseValue);
        }
        #endregion Method: operator <(NumericalValueInstance value1, NumericalValueInstance value2)

        #region Method: operator <=(NumericalValueInstance value1, NumericalValueInstance value2)
        /// <summary>
        /// Checks whether the first value is lower than or equal to the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The first value.</param>
        /// <returns>Returns whether the first value is lower than or equal to the second value.</returns>
        public static bool operator <=(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");

            if (value2 == null)
                throw new ArgumentNullException("value2");

            return Toolbox.Compare(value1.BaseValue, EqualitySignExtended.LowerOrEqual, value2.BaseValue);
        }
        #endregion Method: operator <=(NumericalValueInstance value1, NumericalValueInstance value2)

        #region Method: operator +(NumericalValueInstance value1, NumericalValueInstance value2)
        /// <summary>
        /// Returns the sum of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the sum of the two values.</returns>
        public static NumericalValueInstance operator +(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");

            if (value2 == null)
                throw new ArgumentNullException("value2");

            return new NumericalValueInstance(Toolbox.CalcValue(value1.BaseValue, ValueChangeType.Increase, value2.BaseValue), value1.Prefix, value1.Unit, value1.Min, value2.Max);
        }

        /// <summary>
        /// Returns the sum of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the sum of the two values.</returns>
        public static NumericalValueInstance Add(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            return value1 + value2;
        }
        #endregion Method: operator +(NumericalValueInstance value1, NumericalValueInstance value2)

        #region Method: operator -(NumericalValueInstance value1, NumericalValueInstance value2)
        /// <summary>
        /// Returns the difference of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the difference of the two values.</returns>
        public static NumericalValueInstance operator -(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");

            if (value2 == null)
                throw new ArgumentNullException("value2");

            return new NumericalValueInstance(Toolbox.CalcValue(value1.BaseValue, ValueChangeType.Decrease, value2.BaseValue), value1.Prefix, value1.Unit, value1.Min, value2.Max);
        }

        /// <summary>
        /// Returns the difference of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the difference of the two values.</returns>
        public static NumericalValueInstance Subtract(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            return value1 - value2;
        }
        #endregion Method: operator -(NumericalValueInstance value1, NumericalValueInstance value2)

        #region Method: operator *(NumericalValueInstance value1, NumericalValueInstance value2)
        /// <summary>
        /// Returns the multiplication of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the multiplication of the two values.</returns>
        public static NumericalValueInstance operator *(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");

            if (value2 == null)
                throw new ArgumentNullException("value2");

            return new NumericalValueInstance(Toolbox.CalcValue(value1.BaseValue, ValueChangeType.Multiply, value2.BaseValue), value1.Prefix, value1.Unit, value1.Min, value2.Max);
        }

        /// <summary>
        /// Returns the multiplication of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the multiplication of the two values.</returns>
        public static NumericalValueInstance Multiply(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            return value1 * value2;
        }
        #endregion Method: operator *(NumericalValueInstance value1, NumericalValueInstance value2)

        #region Method: operator *(float scalar, NumericalValueInstance numericalValue)
        /// <summary>
        /// Returns the value multiplied by the scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="numericalValue">The value.</param>
        /// <returns>Returns the value multiplied by the scalar.</returns>
        public static NumericalValueInstance operator *(float scalar, NumericalValueInstance numericalValue)
        {
            if (numericalValue == null)
                throw new ArgumentNullException("numericalValue");

            return new NumericalValueInstance(numericalValue.Value * scalar, numericalValue.Prefix, numericalValue.Unit, numericalValue.Min, numericalValue.Max);
        }

        /// <summary>
        /// Returns the value multiplied by the scalar.
        /// </summary>
        /// <param name="floatValue">The scalar.</param>
        /// <param name="numericalValue">The value.</param>
        /// <returns>Returns the value multiplied by the scalar.</returns>
        public static NumericalValueInstance Multiply(float scalar, NumericalValueInstance numericalValue)
        {
            return scalar * numericalValue;
        }
        #endregion Method: operator *(float scalar, NumericalValueInstance numericalValue)

        #region Method: operator *(NumericalValueInstance numericalValue, float scalar)
        /// <summary>
        /// Returns the value multiplied by the scalar.
        /// </summary>
        /// <param name="numericalValue">The value.</param>
        /// <param name="scalar">The scalar</param>
        /// <returns>Returns the value multiplied by the scalar.</returns>
        public static NumericalValueInstance operator *(NumericalValueInstance numericalValue, float scalar)
        {
            if (numericalValue == null)
                throw new ArgumentNullException("numericalValue");

            return scalar * numericalValue;
        }

        /// <summary>
        /// Returns the value multiplied by the scalar.
        /// </summary>
        /// <param name="numericalValue">The value.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>Returns the value multiplied by the scalar.</returns>
        public static NumericalValueInstance Multiply(NumericalValueInstance numericalValue, float scalar)
        {
            return scalar * numericalValue;
        }
        #endregion Method: operator *(NumericalValueInstance numericalValue, float scalar)

        #region Method: operator /(NumericalValueInstance value1, NumericalValueInstance value2)
        /// <summary>
        /// Returns the division of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the division of the two values.</returns>
        public static NumericalValueInstance operator /(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");

            if (value2 == null)
                throw new ArgumentNullException("value2");

            return new NumericalValueInstance(Toolbox.CalcValue(value1.BaseValue, ValueChangeType.Divide, value2.BaseValue), value1.Prefix, value1.Unit, value1.Min, value2.Max);
        }

        /// <summary>
        /// Returns the division of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the division of the two values.</returns>
        public static NumericalValueInstance Divide(NumericalValueInstance value1, NumericalValueInstance value2)
        {
            return value1 / value2;
        }
        #endregion Method: operator /(NumericalValueInstance value1, NumericalValueInstance value2)

        #region Method: operator /(NumericalValueInstance numericalValue, float scalar)
        /// <summary>
        /// Returns the value divided by the scalar.
        /// </summary>
        /// <param name="numericalValue">The value.</param>
        /// <param name="scalar">The scalar</param>
        /// <returns>Returns the value divided by the scalar.</returns>
        public static NumericalValueInstance operator /(NumericalValueInstance numericalValue, float scalar)
        {
            if (numericalValue == null)
                throw new ArgumentNullException("numericalValue");

            return new NumericalValueInstance(numericalValue.Value / scalar, numericalValue.Prefix, numericalValue.Unit, numericalValue.Min, numericalValue.Max);
        }

        /// <summary>
        /// Returns the value divided by the scalar.
        /// </summary>
        /// <param name="numericalValue">The value.</param>
        /// <param name="scalar">The scalar</param>
        /// <returns>Returns the value divided by the scalar.</returns>
        public static NumericalValueInstance Divide(NumericalValueInstance numericalValue, float scalar)
        {
            return numericalValue / scalar;
        }
        #endregion Method: operator /(NumericalValueInstance numericalValue, float scalar)

        #region Method: Equals(object obj)
        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current System.Object.</param>
        /// <returns>True when equal, false when inequal.</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        #endregion Method: Equals(object obj)

        #region Method: GetHashCode()
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Method: GetHashCode()

        #endregion Method Group: Operator overloading

        #region Method Group: Other

        #region Method: Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the value.
        /// </summary>
        /// <param name="valueConditionBase">The value condition to compare to the value.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the value.</returns>
        public override bool Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (valueConditionBase != null)
            {
                NumericalValueConditionBase numericalValueConditionBase = valueConditionBase as NumericalValueConditionBase;
                if (numericalValueConditionBase != null)
                {
                    float? val = numericalValueConditionBase.GetValue(iVariableInstanceHolder);
                    float? min = numericalValueConditionBase.GetMin(iVariableInstanceHolder);
                    float? max = numericalValueConditionBase.GetMax(iVariableInstanceHolder);
                    return
                        ((numericalValueConditionBase.ValueSign == null || val == null || Toolbox.Compare(this.BaseValue, (EqualitySignExtended)numericalValueConditionBase.ValueSign, (float)val)) &&
                        (numericalValueConditionBase.MinSign == null || min == null || Toolbox.Compare(this.Min, (EqualitySignExtended)numericalValueConditionBase.MinSign, (float)min)) &&
                        (numericalValueConditionBase.MaxSign == null || max == null || Toolbox.Compare(this.Max, (EqualitySignExtended)numericalValueConditionBase.MaxSign, (float)max)));
                }
            }
            return false;
        }
        #endregion Method: Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the value.
        /// </summary>
        /// <param name="valueChangeBase">The change to apply to the value.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        public override bool Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (valueChangeBase != null)
            {
                NumericalValueChangeBase numericalValueChangeBase = valueChangeBase as NumericalValueChangeBase;
                if (numericalValueChangeBase != null)
                {
                    if (numericalValueChangeBase.MinChange != null)
                    {
                        float? min = numericalValueChangeBase.GetMin(iVariableInstanceHolder);
                        if (min != null)
                            this.Min = Toolbox.CalcValue(this.Min, (ValueChangeType)numericalValueChangeBase.MinChange, (float)min);
                    }
                    if (numericalValueChangeBase.MaxChange != null)
                    {
                        float? max = numericalValueChangeBase.GetMax(iVariableInstanceHolder);
                        if (max != null)
                            this.Max = Toolbox.CalcValue(this.Max, (ValueChangeType)numericalValueChangeBase.MaxChange, (float)max);
                    }
                    if (numericalValueChangeBase.ValueChangeType != null)
                    {
                        float? val = numericalValueChangeBase.GetValue(iVariableInstanceHolder);
                        if (val != null)
                            this.Value = Toolbox.CalcValue(this.BaseValue, (ValueChangeType)numericalValueChangeBase.ValueChangeType, (float)val);
                    }
                }
                return true;
            }
            return false;
        }
        #endregion Method: Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: ReplaceBy(ValueInstance valueInstance)
        /// <summary>
        /// Replace the values of this value instance by the given one.
        /// </summary>
        /// <param name="valueInstance">The value instance to get the values from.</param>
        protected internal override void ReplaceBy(ValueInstance valueInstance)
        {
            NumericalValueInstance numericalValueInstance = valueInstance as NumericalValueInstance;
            if (numericalValueInstance != null)
            {
                this.Value = numericalValueInstance.Value;
                this.Prefix = numericalValueInstance.Prefix;
                this.Unit = numericalValueInstance.Unit;
            }
        }
        #endregion Method: ReplaceBy(ValueInstance valueInstance)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            string str = this.Value.ToString(CommonSettings.Culture);

            if (this.Unit != null)
            {
                if (this.Prefix != Prefix.None)
                    str += " " + this.Prefix.ToString();
                str += " " + this.Unit.DefaultName;
            }

            return str;
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: NumericalValueInstance

    #region Class: StringValueInstance
    /// <summary>
    /// A value that consists of a string.
    /// </summary>
    public class StringValueInstance : ValueInstance
    {

        #region Properties and Fields

        #region Property: StringValueBase
        /// <summary>
        /// Gets the string value base of which this is a string value instance.
        /// </summary>
        public StringValueBase StringValueBase
        {
            get
            {
                return this.Base as StringValueBase;
            }
        }
        #endregion Property: StringValueBase

        #region Property: String
        /// <summary>
        /// The custom string.
        /// </summary>
        private string customString = SemanticsSettings.Values.String;

        /// <summary>
        /// The string variable instance.
        /// </summary>
        private StringVariableInstance stringVariableInstance = null;

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        public string String
        {
            get
            {
                if (this.StringValueBase != null)
                {
                    string str;
                    if (TryGetProperty<string>("String", out str))
                        return str;
                    else
                    {
                        if (this.StringValueBase.Variable != null)
                        {
                            if (stringVariableInstance == null)
                                stringVariableInstance = new StringVariableInstance(this.StringValueBase.Variable, this.VariableInstanceHolder);
                            return stringVariableInstance.String;
                        }
                        else
                            return this.StringValueBase.String;
                    }
                }
                else
                    return customString;
            }
            set
            {
                if (this.String != value)
                {
                    if (value == null || value.Length <= this.MaxLength)
                    {
                        if (this.StringValueBase != null)
                            SetProperty("String", value);
                        else
                        {
                            customString = value;
                            NotifyPropertyChanged("String");
                        }
                    }
                }
            }
        }
        #endregion Property: String

        #region Property: MaxLength
        /// <summary>
        /// Gets or sets the maximum length of the string.
        /// </summary>
        public uint MaxLength
        {
            get
            {
                if (this.StringValueBase != null)
                    return GetProperty<uint>("MaxLength", this.StringValueBase.MaxLength);
                return SemanticsSettings.Values.MaxLength;
            }
            set
            {
                if (this.MaxLength != value)
                    SetProperty("MaxLength", value);

                String str = this.String;
                if (str != null && str.Length > value)
                    this.String = str.Remove((int)value, str.Length - (int)value);
            }
        }
        #endregion Property: MaxLength

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StringValueInstance(StringValueBase stringValueBase)
        /// <summary>
        /// Creates a new string value instance from the given string value base.
        /// </summary>
        /// <param name="stringValueBase">The string value base to create the string value instance from.</param>
        internal StringValueInstance(StringValueBase stringValueBase)
            : base(stringValueBase)
        {
            if (stringValueBase != null)
            {
                // Set the possible random value
                if (stringValueBase.IsRandom)
                {
                    String randomString = String.Empty;
                    int randNumber;
                    // Loop 'max length' times to generate a random number or character
                    for (int i = 0; i < stringValueBase.MaxLength; i++)
                    {
                        if (RandomNumber.Next(1, 3) == 1)
                            randNumber = RandomNumber.Next(97, 123); //char {a-z}
                        else
                            randNumber = RandomNumber.Next(48, 58); //int {0-9}

                        // Append the random char or digit to the random string
                        randomString = randomString + (char)randNumber;
                    }
                    this.String = randomString;
                }
            }
        }
        #endregion Constructor: StringValueInstance(StringValueBase stringValueBase)

        #region Constructor: StringValueInstance(StringValueBase stringValueBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Creates a new string value instance from the given string value base.
        /// </summary>
        /// <param name="stringValueBase">The string value base to create the string value instance from.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        internal StringValueInstance(StringValueBase stringValueBase, IVariableInstanceHolder iVariableInstanceHolder)
            : this(stringValueBase)
        {
            this.VariableInstanceHolder = iVariableInstanceHolder;
        }
        #endregion Constructor: StringValueInstance(StringValueBase stringValueBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Constructor: StringValueInstance(StringValueInstance stringValueInstance)
        /// <summary>
        /// Clones the string value.
        /// </summary>
        /// <param name="stringValueInstance">The string value to clone.</param>
        public StringValueInstance(StringValueInstance stringValueInstance)
            : base(stringValueInstance)
        {
            if (stringValueInstance != null)
            {
                this.String = stringValueInstance.String;
                this.MaxLength = stringValueInstance.MaxLength;
            }
        }
        #endregion Constructor: StringValueInstance(StringValueInstance stringValueInstance)

        #region Constructor: StringValueInstance(StringValue stringValue)
        /// <summary>
        /// Creates an instance of the given string value.
        /// </summary>
        /// <param name="stringValue">The string value to create an instance of.</param>
        public StringValueInstance(StringValue stringValue)
            : this(stringValue == null ? null : stringValue.String)
        {
        }
        #endregion Constructor: StringValueInstance(StringValue stringValue)

        #region Constructor: StringValueInstance(string value)
        /// <summary>
        /// Creates a string value from the given string.
        /// </summary>
        /// <param name="value">The string to create a string value from.</param>
        public StringValueInstance(string value)
            : base(null as ValueBase)
        {
            this.String = value;
        }
        #endregion Constructor: StringValueInstance(string value)

        #endregion Method Group: Constructors

        #region Method Group: Operator overloading

        #region Method: operator ==(StringValueInstance value1, StringValueInstance value2)
        /// <summary>
        /// Checks whether the first value is equal to the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns whether the first value is equal to the second value.</returns>
        public static bool operator ==(StringValueInstance value1, StringValueInstance value2)
        {
            // If both are null, or both are same instance, return true
            if (Object.ReferenceEquals(value1, value2))
                return true;

            // If one is null, but not both, return false
            if (((object)value1 == null) || ((object)value2 == null))
                return false;

            // Compare both strings
            return value1.String == value2.String;
        }
        #endregion Method: operator ==(StringValueInstance value1, StringValueInstance value2)

        #region Method: operator !=(StringValueInstance value1, StringValueInstance value2)
        /// <summary>
        /// Checks whether the first value is not equal to the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns whether the first value is not equal to the second value.</returns>
        public static bool operator !=(StringValueInstance value1, StringValueInstance value2)
        {
            return !(value1 == value2);
        }
        #endregion Method: operator !=(StringValueInstance value1, StringValueInstance value2)

        #region Method: operator +(StringValueInstance value1, StringValueInstance value2)
        /// <summary>
        /// Returns the sum of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the sum of the two values.</returns>
        public static StringValueInstance operator +(StringValueInstance value1, StringValueInstance value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");

            if (value2 == null)
                throw new ArgumentNullException("value2");

            return new StringValueInstance(value1.String + value2.String);
        }

        /// <summary>
        /// Returns the sum of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the sum of the two values.</returns>
        public static StringValueInstance Add(StringValueInstance value1, StringValueInstance value2)
        {
            return value1 + value2;
        }
        #endregion Method: operator +(StringValueInstance value1, StringValueInstance value2)

        #region Method: Equals(object obj)
        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current System.Object.</param>
        /// <returns>True when equal, false when inequal.</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        #endregion Method: Equals(object obj)

        #region Method: GetHashCode()
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Method: GetHashCode()

        #endregion Method Group: Operator overloading

        #region Method Group: Other

        #region Method: Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the value.
        /// </summary>
        /// <param name="valueConditionBase">The value condition to compare to the value.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the value.</returns>
        public override bool Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (valueConditionBase != null)
            {
                StringValueConditionBase stringValueConditionBase = valueConditionBase as StringValueConditionBase;
                if (stringValueConditionBase != null)
                {
                    string str = stringValueConditionBase.GetValue(iVariableInstanceHolder);
                    return stringValueConditionBase.StringSign == null || str == null || Toolbox.Compare(this.String, (EqualitySign)stringValueConditionBase.StringSign, str);
                }
            }
            return false;
        }
        #endregion Method: Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the value.
        /// </summary>
        /// <param name="valueChangeBase">The change to apply to the value.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        public override bool Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (valueChangeBase != null)
            {
                StringValueChangeBase stringValueChangeBase = valueChangeBase as StringValueChangeBase;
                if (stringValueChangeBase != null)
                {
                    string str = stringValueChangeBase.GetValue(iVariableInstanceHolder);
                    if (str != null)
                        this.String = (string)str;
                }
                return true;
            }
            return false;
        }
        #endregion Method: Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: ReplaceBy(ValueInstance valueInstance)
        /// <summary>
        /// Replace the values of this value instance by the given one.
        /// </summary>
        /// <param name="valueInstance">The value instance to get the values from.</param>
        protected internal override void ReplaceBy(ValueInstance valueInstance)
        {
            StringValueInstance stringValueInstance = valueInstance as StringValueInstance;
            if (stringValueInstance != null)
                this.String = stringValueInstance.String;
        }
        #endregion Method: ReplaceBy(ValueInstance valueInstance)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.String;
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: StringValueInstance

    #region Class: TermValueInstance
    /// <summary>
    /// A value that consists of a term.
    /// </summary>
    public class TermValueInstance : ValueInstance
    {

        #region Properties and Fields

        #region Property: TermValueBase
        /// <summary>
        /// Gets the term value base of which this is a term value instance.
        /// </summary>
        public TermValueBase TermValueBase
        {
            get
            {
                return this.Base as TermValueBase;
            }
        }
        #endregion Property: TermValueBase

        #region Property: Value
        /// <summary>
        /// Gets the term value.
        /// </summary>
        public float Value
        {
            get
            {
                if (this.TermValueBase != null)
                    return this.TermValueBase.GetValue(this.VariableInstanceHolder);
                
                return SemanticsSettings.Values.Value;
            }
        }
        #endregion Property: Value

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TermValueInstance(TermValueBase termValueBase)
        /// <summary>
        /// Creates a new term value instance from the given term value base.
        /// </summary>
        /// <param name="termValueBase">The term value base to create the term value instance from.</param>
        internal TermValueInstance(TermValueBase termValueBase)
            : base(termValueBase)
        {
        }
        #endregion Constructor: TermValueInstance(TermValueBase termValueBase)

        #region Constructor: TermValueInstance(TermValueBase termValueBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Creates a new term value instance from the given term value base.
        /// </summary>
        /// <param name="termValueBase">The term value base to create the term value instance from.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        internal TermValueInstance(TermValueBase termValueBase, IVariableInstanceHolder iVariableInstanceHolder)
            : this(termValueBase)
        {
            this.VariableInstanceHolder = iVariableInstanceHolder;
        }
        #endregion Constructor: TermValueInstance(TermValueBase termValueBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Constructor: TermValueInstance(TermValueInstance termValueInstance)
        /// <summary>
        /// Clones the term value.
        /// </summary>
        /// <param name="termValueInstance">The term value to clone.</param>
        public TermValueInstance(TermValueInstance termValueInstance)
            : base(termValueInstance)
        {
        }
        #endregion Constructor: TermValueInstance(TermValueInstance termValueInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the value.
        /// </summary>
        /// <param name="valueConditionBase">The value condition to compare to the value.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the value.</returns>
        public override bool Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            return false;
        }
        #endregion Method: Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the value.
        /// </summary>
        /// <param name="valueChangeBase">The change to apply to the value.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        public override bool Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            return false;
        }
        #endregion Method: Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: ReplaceBy(ValueInstance valueInstance)
        /// <summary>
        /// Replace the values of this value instance by the given one.
        /// </summary>
        /// <param name="valueInstance">The value instance to get the values from.</param>
        protected internal override void ReplaceBy(ValueInstance valueInstance)
        {
        }
        #endregion Method: ReplaceBy(ValueInstance valueInstance)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.Value.ToString(CommonSettings.Culture);
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: TermValueInstance

    #region Class: VectorValueInstance
    /// <summary>
    /// A value that consists of a four-dimensional vector.
    /// </summary>
    public class VectorValueInstance : ValueInstance
    {

        #region Properties and Fields

        #region Property: VectorValueBase
        /// <summary>
        /// Gets the vector value base of which this is a vector value instance.
        /// </summary>
        public VectorValueBase VectorValueBase
        {
            get
            {
                return this.Base as VectorValueBase;
            }
        }
        #endregion Property: VectorValueBase

        #region Property: Vector
        /// <summary>
        /// The custom vector.
        /// </summary>
        private Vec4 customVector = null;

        /// <summary>
        /// The vector variable instance.
        /// </summary>
        private VectorVariableInstance vectorVariableInstance = null;

        /// <summary>
        /// A handler for a changed vector.
        /// </summary>
        private Vec4.Vec4Handler vectorChanged = null;
        
        /// <summary>
        /// A temporary vector for notifications.
        /// </summary>
        private Vec4 tempVector = null;

        /// <summary>
        /// Gets or sets the four-dimensional vector.
        /// </summary>
        public Vec4 Vector
        {
            get
            {
                if (tempVector != null)
                {
                    Vec4 toReturn = new Vec4(tempVector);
                    tempVector = null;
                    return toReturn;
                }

                if (vectorChanged == null)
                    vectorChanged = new Vec4.Vec4Handler(vector_ValueChanged);

                if (this.VectorValueBase != null)
                {
                    Vec4 vector = null;
                    if (TryGetProperty<Vec4>("Vector", out vector))
                        return vector;
                    else
                    {
                        if (this.VectorValueBase.Variable != null)
                        {
                            if (vectorVariableInstance == null)
                            {
                                vectorVariableInstance = new VectorVariableInstance(this.VectorValueBase.Variable, this.VariableInstanceHolder);
                                vectorVariableInstance.Vector.ValueChanged += vectorChanged;
                            }
                            vector = vectorVariableInstance.Vector;
                        }
                        else
                        {
                            vector = new Vec4(this.VectorValueBase.Vector);
                            vector.ValueChanged += vectorChanged;
                        }
                        return vector;
                    }
                }
                else
                {
                    if (customVector == null)
                    {
                        customVector = new Vec4(SemanticsSettings.Values.Vector4X, SemanticsSettings.Values.Vector4Y, SemanticsSettings.Values.Vector4Z, SemanticsSettings.Values.Vector4W);
                        customVector.ValueChanged += vectorChanged;
                    }
                    return customVector;
                }
            }
            set
            {
                Vec4 vector = this.Vector;
                if (vector != value)
                {
                    if (vectorChanged == null)
                        vectorChanged = new Vec4.Vec4Handler(vector_ValueChanged);

                    if (vector != null)
                        vector.ValueChanged -= vectorChanged;

                    // Make sure the value does not exceed the min or max
                    if (value != null)
                    {
                        Vec4 min = this.Min;
                        Vec4 max = this.Max;
                        float x = Math.Max(min.X, Math.Min(value.X, max.X));
                        float y = Math.Max(min.Y, Math.Min(value.Y, max.Y));
                        float z = Math.Max(min.Z, Math.Min(value.Z, max.Z));
                        float w = Math.Max(min.W, Math.Min(value.W, max.W));
                        value = new Vec4(x, y, z, w);
                    }

                    // Notify of this new change, before actually changing the vector
                    this.tempVector = value;
                    NotifyPropertyChanged("Vector");

                    // Now change the vector
                    if (this.VectorValueBase != null)
                        SetProperty("Vector", value);
                    else
                    {
                        customVector = value;
                        NotifyPropertyChanged("Vector");
                    }

                    if (value != null)
                        value.ValueChanged += vectorChanged;
                }
            }
        }

        /// <summary>
        /// Notify any changes of the vector.
        /// </summary>
        private void vector_ValueChanged(Vec4 vector)
        {
            this.Vector = vector;
        }
        #endregion Property: Vector

        #region Property: Min
        /// <summary>
        /// A handler for a changed minimum.
        /// </summary>
        Vec4.Vec4Handler minChanged = null;

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        public Vec4 Min
        {
            get
            {
                Vec4 min = null;

                if (minChanged == null)
                    minChanged = new Vec4.Vec4Handler(min_ValueChanged);

                if (this.VectorValueBase != null)
                {
                    min = GetProperty<Vec4>("Min");
                    if (min == null)
                    {
                        min = new Vec4(this.VectorValueBase.Min);
                        min.ValueChanged += minChanged;
                    }
                }
                else
                {
                    min = new Vec4(SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MinValue);
                    min.ValueChanged += minChanged;
                }

                return min;
            }
            set
            {
                Vec4 min = this.Min;
                if (min != value)
                {
                    if (minChanged == null)
                        minChanged = new Vec4.Vec4Handler(min_ValueChanged);

                    if (min != null)
                        min.ValueChanged -= minChanged;

                    // Make sure the minimum does not exceed the maximum
                    if (value != null && !ignoreMinExceeding)
                    {
                        Vec4 max = this.Max;
                        float x = Math.Min(value.X, max.X);
                        float y = Math.Min(value.Y, max.Y);
                        float z = Math.Min(value.Z, max.Z);
                        float w = Math.Min(value.W, max.W);
                        value = new Vec4(x, y, z, w);
                    }

                    SetProperty("Min", value);

                    // Make sure the value does not exceed the minimum
                    if (this.Vector < min)
                        this.Vector = new Vec4(min);

                    if (value != null)
                        value.ValueChanged += minChanged;
                }
            }
        }

        /// <summary>
        /// Indicates whether an exceeding correction for the minimum should be applied.
        /// </summary>
        private bool ignoreMinExceeding = false;

        /// <summary>
        /// Notify any changes of the minimum.
        /// </summary>
        private void min_ValueChanged(Vec4 min)
        {
            this.Min = min;
        }
        #endregion Property: Min

        #region Property: Max
        /// <summary>
        /// A handler for a changed maximum.
        /// </summary>
        Vec4.Vec4Handler maxChanged = null;

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        public Vec4 Max
        {
            get
            {
                Vec4 max = null;

                if (maxChanged == null)
                    maxChanged = new Vec4.Vec4Handler(max_ValueChanged);

                if (this.VectorValueBase != null)
                {
                    max = GetProperty<Vec4>("Max");
                    if (max == null)
                    {
                        max = new Vec4(this.VectorValueBase.Max);
                        max.ValueChanged += maxChanged;
                    }
                }
                else
                {
                    max = new Vec4(SemanticsSettings.Values.MaxValue, SemanticsSettings.Values.MaxValue, SemanticsSettings.Values.MaxValue, SemanticsSettings.Values.MaxValue);
                    max.ValueChanged += maxChanged;
                }

                return max;
            }
            set
            {
                Vec4 max = this.Max;
                if (max != value)
                {
                    if (maxChanged == null)
                        maxChanged = new Vec4.Vec4Handler(max_ValueChanged);

                    if (max != null)
                        max.ValueChanged -= maxChanged;

                    // Make sure the maximum does not exceed the minimum
                    if (value != null)
                    {
                        Vec4 min = this.Min;
                        float x = Math.Max(value.X, min.X);
                        float y = Math.Max(value.Y, min.Y);
                        float z = Math.Max(value.Z, min.Z);
                        float w = Math.Max(value.W, min.W);
                        value = new Vec4(x, y, z, w);
                    }
                    
                    SetProperty("Max", value);

                    // Make sure the value does not exceed the maximum
                    if (this.Vector > max)
                        this.Vector = new Vec4(max);

                    if (value != null)
                        value.ValueChanged += maxChanged;
                }
            }
        }

        /// <summary>
        /// Notify any changes of the maximum.
        /// </summary>
        private void max_ValueChanged(Vec4 max)
        {
            this.Max = max;
        }
        #endregion Property: Max

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: VectorValueInstance(VectorValueBase vectorValueBase)
        /// <summary>
        /// Creates a new vector value instance from the given vector value base.
        /// </summary>
        /// <param name="vectorValueBase">The vector value base to create the vector value instance from.</param>
        internal VectorValueInstance(VectorValueBase vectorValueBase)
            : base(vectorValueBase)
        {
            if (vectorValueBase != null)
            {
                // Set the possible random value
                if (vectorValueBase.IsRandom)
                {
                    Vec4 min = this.Min;
                    Vec4 max = this.Max;
                    Vec4 randomVector = new Vec4();
                    randomVector.X = RandomNumber.RandomF(min.X, max.X);
                    randomVector.Y = RandomNumber.RandomF(min.Y, max.Y);
                    randomVector.Z = RandomNumber.RandomF(min.Z, max.Z);
                    randomVector.W = RandomNumber.RandomF(min.W, max.W);
                    this.Vector = randomVector;
                }
            }
        }
        #endregion Constructor: VectorValueInstance(VectorValueBase vectorValueBase)

        #region Constructor: VectorValueInstance(VectorValueBase vectorValueBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Creates a new vector value instance from the given vector value base.
        /// </summary>
        /// <param name="vectorValueBase">The vector value base to create the vector value instance from.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        internal VectorValueInstance(VectorValueBase vectorValueBase, IVariableInstanceHolder iVariableInstanceHolder)
            : this(vectorValueBase)
        {
            this.VariableInstanceHolder = iVariableInstanceHolder;
        }
        #endregion Constructor: VectorValueInstance(VectorValueBase vectorValueBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Constructor: VectorValueInstance(VectorValueInstance vectorValueInstance)
        /// <summary>
        /// Clones the vector value.
        /// </summary>
        /// <param name="vectorValueInstance">The vector value to clone.</param>
        public VectorValueInstance(VectorValueInstance vectorValueInstance)
            : base(vectorValueInstance)
        {
            if (vectorValueInstance != null)
            {
                if (vectorValueInstance.Min != null)
                {
                    this.ignoreMinExceeding = true;
                    this.Min = new Vec4(vectorValueInstance.Min);
                    this.ignoreMinExceeding = false;
                }
                if (vectorValueInstance.Max != null)
                    this.Max = new Vec4(vectorValueInstance.Max);
                if (vectorValueInstance.Vector != null)
                    this.Vector = new Vec4(vectorValueInstance.Vector);
            }
        }
        #endregion Constructor: VectorValueInstance(VectorValueInstance vectorValueInstance)

        #region Constructor: VectorValueInstance(VectorValue vectorValue)
        /// <summary>
        /// Creates an instance of the given vector value.
        /// </summary>
        /// <param name="vectorValue">The vector value to create an instance of.</param>
        public VectorValueInstance(VectorValue vectorValue)
            : this(vectorValue == null ? null : vectorValue.Vector)
        {
        }
        #endregion Constructor: VectorValueInstance(VectorValue vectorValue)

        #region Constructor: VectorValueInstance(Vec4 value)
        /// <summary>
        /// Creates a vector value from the given vector.
        /// </summary>
        /// <param name="value">The vector to create a vector value from.</param>
        public VectorValueInstance(Vec4 value)
            : base(null as ValueBase)
        {
            this.Vector = value;
        }
        #endregion Constructor: VectorValueInstance(Vec4 value)

        #endregion Method Group: Constructors

        #region Method Group: Operator overloading

        #region Method: operator ==(VectorValue value1, VectorValue value2)
        /// <summary>
        /// Checks whether the first value is equal to the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns whether the first value is equal to the second value.</returns>
        public static bool operator ==(VectorValueInstance value1, VectorValueInstance value2)
        {
            // If both are null, or both are the same, return true
            if (Object.ReferenceEquals(value1, value2))
                return true;

            // If one is null, but not both, return false
            if (((object)value1 == null) || ((object)value2 == null))
                return false;

            // Compare both vectors
            return value1.Vector == value2.Vector;
        }
        #endregion Method: operator ==(VectorValue value1, VectorValue value2)

        #region Method: operator !=(VectorValue value1, VectorValue value2)
        /// <summary>
        /// Checks whether the first value is not equal to the second value.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns whether the first value is not equal to the second value.</returns>
        public static bool operator !=(VectorValueInstance value1, VectorValueInstance value2)
        {
            return !(value1 == value2);
        }
        #endregion Method: operator !=(VectorValue value1, VectorValue value2)

        #region Method: operator +(VectorValue value1, VectorValue value2)
        /// <summary>
        /// Returns the sum of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the sum of the two values.</returns>
        public static VectorValueInstance operator +(VectorValueInstance value1, VectorValueInstance value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");

            if (value2 == null)
                throw new ArgumentNullException("value2");

            return new VectorValueInstance(value1.Vector + value2.Vector);
        }

        /// <summary>
        /// Returns the sum of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the sum of the two values.</returns>
        public static VectorValueInstance Add(VectorValueInstance value1, VectorValueInstance value2)
        {
            return value1 + value2;
        }
        #endregion Method: operator +(VectorValue value1, VectorValue value2)

        #region Method: operator -(VectorValue value1, VectorValue value2)
        /// <summary>
        /// Returns the difference of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the difference of the two values.</returns>
        public static VectorValueInstance operator -(VectorValueInstance value1, VectorValueInstance value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");

            if (value2 == null)
                throw new ArgumentNullException("value2");

            return new VectorValueInstance(value1.Vector - value2.Vector);
        }

        /// <summary>
        /// Returns the difference of two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns the difference of the two values.</returns>
        public static VectorValueInstance Subtract(VectorValueInstance value1, VectorValueInstance value2)
        {
            return value1 - value2;
        }
        #endregion Method: operator -(VectorValue value1, VectorValue value2)

        #region Method: Equals(object obj)
        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current System.Object.</param>
        /// <returns>True when equal, false when inequal.</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        #endregion Method: Equals(object obj)

        #region Method: GetHashCode()
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Method: GetHashCode()

        #endregion Method Group: Operator overloading

        #region Method Group: Other

        #region Method: Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the value.
        /// </summary>
        /// <param name="valueConditionBase">The value condition to compare to the value.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the value.</returns>
        public override bool Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (valueConditionBase != null)
            {
                VectorValueConditionBase vectorValueConditionBase = valueConditionBase as VectorValueConditionBase;
                if (vectorValueConditionBase != null)
                {
                    Vec4 vector = vectorValueConditionBase.GetValue(iVariableInstanceHolder);
                    return vectorValueConditionBase.VectorSign == null || vector == null || Toolbox.Compare(this.Vector, (EqualitySignExtended)vectorValueConditionBase.VectorSign, vector);
                }
            }
            return false;
        }
        #endregion Method: Satisfies(ValueConditionBase valueConditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the value.
        /// </summary>
        /// <param name="valueChangeBase">The change to apply to the value.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        public override bool Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (valueChangeBase != null)
            {
                VectorValueChangeBase vectorValueChangeBase = valueChangeBase as VectorValueChangeBase;
                if (vectorValueChangeBase != null)
                {
                    if (vectorValueChangeBase.VectorChange != null)
                    {
                        Vec4 vector = vectorValueChangeBase.GetValue(iVariableInstanceHolder);
                        if (vector != null)
                            this.Vector = Toolbox.CalcValue(this.Vector, (ValueChangeType)vectorValueChangeBase.VectorChange, vector);
                    }
                }
                return true;
            }
            return false;
        }
        #endregion Method: Apply(ValueChangeBase valueChangeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: ReplaceBy(ValueInstance valueInstance)
        /// <summary>
        /// Replace the values of this value instance by the given one.
        /// </summary>
        /// <param name="valueInstance">The value instance to get the values from.</param>
        protected internal override void ReplaceBy(ValueInstance valueInstance)
        {
            VectorValueInstance vectorValueInstance = valueInstance as VectorValueInstance;
            if (vectorValueInstance != null)
                this.Vector = new Vec4(vectorValueInstance.Vector);
        }
        #endregion Method: ReplaceBy(ValueInstance valueInstance)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.Vector.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: VectorValueInstance

}