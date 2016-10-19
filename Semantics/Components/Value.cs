/**************************************************************************
 * 
 * Value.cs
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
using System.Collections.Generic;
using System.Reflection;
using Common;
using Semantics.Abstractions;
using Semantics.Data;
using Semantics.Utilities;
using ValueType = Semantics.Utilities.ValueType;

namespace Semantics.Components
{

    #region Class: Value
    /// <summary>
    /// A value.
    /// </summary>
    public abstract class Value : IdHolder
    {

        #region Properties and Fields

        #region Property: IsRandom
        /// <summary>
        /// Gets or sets the value that indicates whether the value should be random.
        /// </summary>
        public bool IsRandom
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Value, Columns.IsRandom);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Value, Columns.IsRandom, value);
                NotifyPropertyChanged("IsRandom");
            }
        }
        #endregion Property: IsRandom
        
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Value()
        /// <summary>
        /// Creates a value.
        /// </summary>
        protected Value()
            : base()
        {
        }
        #endregion Constructor: Value() 

        #region Constructor: Value(uint id)
        /// <summary>
        /// Creates a value from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a value from.</param>
        protected Value(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Value(uint id) 

        #region Constructor: Value(Value value)
        /// <summary>
        /// Clones a value.
        /// </summary>
        /// <param name="value">The value to clone.</param>
        protected Value(Value value)
            : base()
        {
            if (value != null)
            {
                Database.Current.StartChange();

                this.IsRandom = value.IsRandom;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Value(Value value)

        #region Method: Create(ValueType valueType)
        /// <summary>
        /// Creates a new value of the given value type.
        /// </summary>
        /// <param name="valueType">The value type to create a value from.</param>
        public static Value Create(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Numerical:
                    return new NumericalValue(SemanticsSettings.Values.Value);
                case ValueType.Boolean:
                    return new BoolValue(SemanticsSettings.Values.Boolean);
                case ValueType.String:
                    return new StringValue(SemanticsSettings.Values.String);
                case ValueType.Vector:
                    return new VectorValue(new Vec4(SemanticsSettings.Values.Vector4X, SemanticsSettings.Values.Vector4Y, SemanticsSettings.Values.Vector4Y, SemanticsSettings.Values.Vector4Z));
                case ValueType.Term:
                    return new TermValue(new NumericalValue(SemanticsSettings.Values.Value), Operator.Addition, new NumericalValue(SemanticsSettings.Values.Value));
                default:
                    break;
            }
            return null;
        }
        #endregion Method: Create(ValueType valueType)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the value.
        /// </summary>
        /// <returns>A clone of the value.</returns>
        public Value Clone()
        {
            try
            {
                Type type = this.GetType();
                return type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type }, null).Invoke(new object[] { this }) as Value;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: Value

    #region Class: BoolValue
    /// <summary>
    /// A value that consists of a bool.
    /// </summary>
    public sealed class BoolValue : Value
    {

        #region Properties and Fields

        #region Property: Bool
        /// <summary>
        /// Gets or sets the boolean value.
        /// </summary>
        public bool Bool
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.BoolValue, Columns.Bool);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.BoolValue, Columns.Bool, value);
                NotifyPropertyChanged("Bool");
            }
        }
        #endregion Property: Bool

        #region Property: Variable
        /// <summary>
        /// Gets or sets the bool variable to represent the bool instead.
        /// </summary>
        public BoolVariable Variable
        {
            get
            {
                return Database.Current.Select<BoolVariable>(this.ID, ValueTables.BoolValue, Columns.Variable);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.BoolValue, Columns.Variable, value);
                NotifyPropertyChanged("Variable");
            }
        }
        #endregion Property: Variable

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: BoolValue()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static BoolValue()
        {
            // Variable
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(BoolVariable), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.BoolValue, typeof(BoolValue), dict);
        }
        #endregion Static Constructor: BoolValue()

        #region Constructor: BoolValue(BoolValue boolValue)
        /// <summary>
        /// Clones the bool value.
        /// </summary>
        /// <param name="boolValue">The bool value to clone.</param>
        public BoolValue(BoolValue boolValue)
            : base(boolValue)
        {
            if (boolValue != null)
            {
                Database.Current.StartChange();

                this.Bool = boolValue.Bool;
                if (boolValue.Variable != null)
                    this.Variable = boolValue.Variable;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: BoolValue(BoolValue boolValue)

        #region Constructor: BoolValue(uint id)
        /// <summary>
        /// Creates a bool value from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a value from.</param>
        private BoolValue(uint id)
            : base(id)
        {
        }
        #endregion Constructor: BoolValue(uint id)

        #region Constructor: BoolValue(bool value)
        /// <summary>
        /// Creates a bool value from the given bool.
        /// </summary>
        /// <param name="value">The bool to create a bool value from.</param>
        public BoolValue(bool value)
            : base()
        {
            Database.Current.StartChange();

            this.Bool = value;

            Database.Current.StopChange();
        }

        /// <summary>
        /// Creates a bool value from the given bool.
        /// </summary>
        /// <param name="value">The bool to create a bool value from.</param>
        /// <returns>A bool value from the given bool.</returns>
        public static implicit operator BoolValue(bool value)
        {
            return new BoolValue(value);
        }
        #endregion Constructor: BoolValue(bool value)

        #region Constructor: BoolValue(BoolVariable boolVariable)
        /// <summary>
        /// Creates a bool value from the given bool variable.
        /// </summary>
        /// <param name="boolVariable">The bool variable to create a bool value from.</param>
        public BoolValue(BoolVariable boolVariable)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = boolVariable;

            Database.Current.StopChange();
        }
        #endregion Constructor: BoolValue(BoolVariable boolVariable)

        #endregion Method Group: Constructors

        #region Method Group: Other

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
    #endregion Class: BoolValue

    #region Class: NumericalValue
    /// <summary>
    /// A numerical value, optionally accompanied by a prefix and a unit.
    /// </summary>
    public sealed class NumericalValue : Value
    {

        #region Properties and Fields

        #region Property: Value
        /// <summary>
        /// Gets or sets the numerical value.
        /// </summary>
        public float Value
        {
            get
            {
                return Database.Current.Select<float>(this.ID, ValueTables.NumericalValue, Columns.Value);
            }
            set
            {
                // Make sure the value does not exceed the min or max
                if (!this.ignoreMinMaxChecks)
                    value = Math.Max(this.Min, Math.Min((float)value, this.Max));

                baseValue = null;

                Database.Current.Update(this.ID, ValueTables.NumericalValue, Columns.Value, value);
                NotifyPropertyChanged("Value");
            }
        }
        #endregion Property: Value

        #region Property: Variable
        /// <summary>
        /// Gets or sets the variable to represent the value instead. Only valid for numerical and term variables!
        /// </summary>
        public Variable Variable
        {
            get
            {
                return Database.Current.Select<Variable>(this.ID, ValueTables.NumericalValue, Columns.Variable);
            }
            set
            {
                if (value is NumericalVariable || value is TermVariable)
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValue, Columns.Variable, value);
                    NotifyPropertyChanged("Variable");
                }
            }
        }
        #endregion Property: Variable

        #region Property: Prefix
        /// <summary>
        /// Gets or sets the prefix of the value.
        /// </summary>
        public Prefix Prefix
        {
            get
            {
                return Database.Current.Select<Prefix>(this.ID, ValueTables.NumericalValue, Columns.Prefix);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValue, Columns.Prefix, value);
                NotifyPropertyChanged("Prefix");

                baseValue = null;
            }
        }
        #endregion Property: Prefix

        #region Property: Unit
        /// <summary>
        /// Gets or sets the optional unit of this value.
        /// </summary>
        public Unit Unit
        {
            get
            {
                return Database.Current.Select<Unit>(this.ID, ValueTables.NumericalValue, Columns.Unit);
            }
            set
            {
                if (value == null || this.UnitCategory == null || this.UnitCategory.HasUnit(value))
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValue, Columns.Unit, value);
                    NotifyPropertyChanged("Unit");

                    baseValue = null;
                }
            }
        }
        #endregion Property: Unit

        #region Property: UnitCategory
        /// <summary>
        /// Gets or sets the optional unit category of this numerical value, to which the unit should be restricted.
        /// </summary>
        public UnitCategory UnitCategory
        {
            get
            {
                return Database.Current.Select<UnitCategory>(this.ID, ValueTables.NumericalValue, Columns.UnitCategory);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValue, Columns.UnitCategory, value);
                NotifyPropertyChanged("UnitCategory");

                // Set the unit to the default unit of the category if it has not been set before, or when the unit does not belong to the unit category
                if (value != null)
                {
                    if (this.Unit == null)
                        this.Unit = value.BaseUnit;
                    else
                    {
                        if (!value.HasUnit(this.Unit))
                            this.Unit = value.BaseUnit;
                    }
                }
            }
        }
        #endregion Property: UnitCategory

        #region Property: Min
        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public float Min
        {
            get
            {
                return Database.Current.Select<float>(this.ID, ValueTables.NumericalValue, Columns.Min);
            }
            set
            {
                // Make sure the minimum does not exceed the maximum
                if (!this.ignoreMinMaxChecks && !ignoreMinExceeding)
                    value = Math.Min((float)value, this.Max);

                Database.Current.Update(this.ID, ValueTables.NumericalValue, Columns.Min, value);
                NotifyPropertyChanged("Min");

                // Make sure the value does not exceed the minimum
                if (!this.ignoreMinMaxChecks && this.Value < value)
                    this.Value = value;
            }
        }

        /// <summary>
        /// Indicates whether an exceeding correction for the minimum should be applied.
        /// </summary>
        private bool ignoreMinExceeding = false;
        #endregion Property: Min

        #region Property: Max
        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public float Max
        {
            get
            {
                return Database.Current.Select<float>(this.ID, ValueTables.NumericalValue, Columns.Max);
            }
            set
            {
                // Make sure the maximum does not exceed the minimum
                if (!this.ignoreMinMaxChecks)
                    value = Math.Max((float)value, this.Min);

                Database.Current.Update(this.ID, ValueTables.NumericalValue, Columns.Max, value);
                NotifyPropertyChanged("Max");

                // Make sure the value does not exceed the maximum
                if (!this.ignoreMinMaxChecks && this.Value > value)
                    this.Value = value;
            }
        }
        #endregion Property: Max

        #region Property: RandomMin
        /// <summary>
        /// Gets or sets the minimum value of the random. Only valid when IsRandom has been set to true.
        /// If not set, the regular Min will be used when the random is applied.
        /// </summary>
        public float? RandomMin
        {
            get
            {
                return Database.Current.Select<float?>(this.ID, ValueTables.NumericalValue, Columns.RandomMin);
            }
            set
            {
                if (value != null)
                {
                    // Make sure the minimum does not exceed the maximum
                    if (!this.ignoreMinMaxChecks && !ignoreMinExceeding && this.RandomMax != null)
                        value = Math.Min((float)value, (float)this.RandomMax);
                }

                Database.Current.Update(this.ID, ValueTables.NumericalValue, Columns.RandomMin, value);
                NotifyPropertyChanged("RandomMin");
            }
        }
        #endregion Property: RandomMin

        #region Property: RandomMax
        /// <summary>
        /// Gets or sets the maximum value of the random. Only valid when IsRandom has been set to true.
        /// If not set, the regular Max will be used when the random is applied.
        /// </summary>
        public float? RandomMax
        {
            get
            {
                return Database.Current.Select<float?>(this.ID, ValueTables.NumericalValue, Columns.RandomMax);
            }
            set
            {
                if (value != null)
                {
                    // Make sure the maximum does not exceed the minimum
                    if (!this.ignoreMinMaxChecks && this.RandomMin != null)
                        value = Math.Max((float)value, (float)this.RandomMin);
                }

                Database.Current.Update(this.ID, ValueTables.NumericalValue, Columns.RandomMax, value);
                NotifyPropertyChanged("RandomMax");
            }
        }
        #endregion Property: RandomMax

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
                    baseValue = Toolbox.GetBaseValue(this.Value, this.Prefix, this.Unit);
                return (float)baseValue;
            }
        }
        #endregion: Property: BaseValue

        #region Field: ignoreMinMaxChecks
        /// <summary>
        /// Indicates whether all checks for min/max should be ignored.
        /// </summary>
        private bool ignoreMinMaxChecks = false;
        public bool IgnoreMinMaxChecks
        {
            get { return ignoreMinMaxChecks; }
            set { ignoreMinMaxChecks = value; }
        }
        #endregion Field: ignoreMinMaxChecks
		
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: NumericalValue()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static NumericalValue()
        {
            // Variable, unit, and unit category
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(NumericalVariable), EntryType.Nullable));
            dict.Add(Columns.Unit, new Tuple<Type, EntryType>(typeof(Unit), EntryType.Nullable));
            dict.Add(Columns.UnitCategory, new Tuple<Type, EntryType>(typeof(UnitCategory), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.NumericalValue, typeof(NumericalValue), dict);
        }
        #endregion Static Constructor: NumericalValue()

        #region Constructor: NumericalValue(NumericalValue numericalValue)
        /// <summary>
        /// Clones the numerical value.
        /// </summary>
        /// <param name="numericalValue">The numerical value to clone.</param>
        public NumericalValue(NumericalValue numericalValue)
            : base(numericalValue)
        {
            if (numericalValue != null)
            {
                Database.Current.StartChange();

                this.ignoreMinExceeding = true;
                this.Min = numericalValue.Min;
                this.RandomMin = numericalValue.RandomMin;
                this.ignoreMinExceeding = false;
                this.Max = numericalValue.Max;
                this.RandomMax = numericalValue.RandomMax;
                this.Prefix = numericalValue.Prefix;
                this.Unit = numericalValue.Unit;
                this.UnitCategory = numericalValue.UnitCategory;
                this.Value = numericalValue.Value;
                if (numericalValue.Variable != null)
                    this.Variable = numericalValue.Variable;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: NumericalValue(NumericalValue numericalValue)

        #region Constructor: NumericalValue(uint id)
        /// <summary>
        /// Creates a numerical value from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a value from.</param>
        private NumericalValue(uint id)
            : base(id)
        {
        }
        #endregion Constructor: NumericalValue(uint id)

        #region Constructor: NumericalValue(float value)
        /// <summary>
        /// Creates a numerical value from the given float.
        /// </summary>
        /// <param name="val">The float to create a numerical value from.</param>
        public NumericalValue(float value)
            : this(value, Prefix.None, null)
        {
        }

        /// <summary>
        /// Creates a numerical value from the given float.
        /// </summary>
        /// <param name="val">The float to create a numerical value from.</param>
        /// <returns>A numerical value from the given float.</returns>
        public static implicit operator NumericalValue(float val)
        {
            return new NumericalValue(val);
        }
        #endregion Constructor: NumericalValue(float value)

        #region Constructor: NumericalValue(float value, Unit unit)
        /// <summary>
        /// Creates a numerical value from the given float and unit.
        /// </summary>
        /// <param name="value">The float of the numerical value.</param>
        /// <param name="unit">The unit of the numerical value.</param>
        public NumericalValue(float value, Unit unit)
            : this(value, Prefix.None, unit)
        {
        }
        #endregion Constructor: NumericalValue(float value, Unit unit)

        #region Constructor: NumericalValue(float value, UnitCategory unitCategory)
        /// <summary>
        /// Creates a numerical value from the given float and unit category.
        /// </summary>
        /// <param name="value">The float of the numerical value.</param>
        /// <param name="unitCategory">The unit category of the numerical value.</param>
        public NumericalValue(float value, UnitCategory unitCategory)
            : this(value, Prefix.None, null)
        {
            Database.Current.StartChange();

            this.UnitCategory = unitCategory;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValue(float value, UnitCategory unitCategory)

        #region Constructor: NumericalValue(float value, UnitCategory unitCategory, float min, float max)
        /// <summary>
        /// Creates a numerical value from the given float and unit category.
        /// </summary>
        /// <param name="value">The float of the numerical value.</param>
        /// <param name="unitCategory">The unit category of the numerical value.</param>
        /// <param name="min">The minimum of the numerical value.</param>
        /// <param name="max">The maximum of the numerical value.</param>
        public NumericalValue(float value, UnitCategory unitCategory, float min, float max)
            : this(value, Prefix.None, null, min, max)
        {
            Database.Current.StartChange();

            this.UnitCategory = unitCategory;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValue(float value, UnitCategory unitCategory, float min, float max)

        #region Constructor: NumericalValue(float value, Prefix prefix, Unit unit)
        /// <summary>
        /// Creates a numerical value from the given float, prefix and unit.
        /// </summary>
        /// <param name="value">The float of the numerical value.</param>
        /// <param name="prefix">The prefix of the numerical value.</param>
        /// <param name="unit">The unit of the numerical value.</param>
        public NumericalValue(float value, Prefix prefix, Unit unit)
            : base()
        {
            SetValues(value, prefix, unit, SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MaxValue, true);
        }
        #endregion Constructor: NumericalValue(float value, Prefix prefix, Unit unit)

        #region Constructor: NumericalValue(float value, Prefix prefix, Unit unit, float min, float max)
        /// <summary>
        /// Creates a numerical value from the given float, prefix, unit, min, and max.
        /// </summary>
        /// <param name="value">The float of the numerical value.</param>
        /// <param name="prefix">The prefix of the numerical value.</param>
        /// <param name="unit">The unit of the numerical value.</param>
        /// <param name="min">The minimum of the numerical value.</param>
        /// <param name="max">The maximum of the numerical value.</param>
        public NumericalValue(float value, Prefix prefix, Unit unit, float min, float max)
            : base()
        {
            SetValues(value, prefix, unit, min, max, false);
        }
        #endregion Constructor: NumericalValue(float value, Prefix prefix, Unit unit, float min, float max)

        #region Method: SetValues(float value, Prefix prefix, Unit unit, float min, float max, bool ignoreChecks)
        /// <summary>
        /// Set all the values, possibly ignoring all checks.
        /// </summary>
        /// <param name="value">The float of the numerical value.</param>
        /// <param name="prefix">The prefix of the numerical value.</param>
        /// <param name="unit">The unit of the numerical value.</param>
        /// <param name="min">The minimum of the numerical value.</param>
        /// <param name="max">The maximum of the numerical value.</param>
        /// <param name="ignoreMinMaxChecks">Indicates whether all checks for min/max should be ignored.</param>
        private void SetValues(float value, Prefix prefix, Unit unit, float min, float max, bool ignoreMinMaxChecks)
        {
            Database.Current.StartChange();

            this.ignoreMinMaxChecks = ignoreMinMaxChecks;
            this.ignoreMinExceeding = true;
            this.Min = min;
            this.ignoreMinExceeding = false;
            this.Max = max;
            this.Prefix = prefix;
            this.Unit = unit;
            this.Value = value;
            this.ignoreMinMaxChecks = false;

            Database.Current.StopChange();
        }
        #endregion Method: SetValues(float value, Prefix prefix, Unit unit, float min, float max, bool ignoreChecks)

        #region Constructor: NumericalValue(NumericalVariable numericalVariable)
        /// <summary>
        /// Creates a numerical value from the given numerical variable.
        /// </summary>
        /// <param name="numericalVariable">The numerical variable to create a numerical value from.</param>
        public NumericalValue(NumericalVariable numericalVariable)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = numericalVariable;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValue(NumericalVariable numericalVariable)

        #endregion Method Group: Constructors

        #region Method Group: Other

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
    #endregion Class: NumericalValue

    #region Class: StringValue
    /// <summary>
    /// A value that consists of a string.
    /// </summary>
    public sealed class StringValue : Value
    {

        #region Properties and Fields

        #region Property: String
        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        public string String
        {
            get
            {
                return Database.Current.Select<string>(this.ID, ValueTables.StringValue, Columns.String);
            }
            set
            {
                if (value == null || value.Length <= this.MaxLength)
                {
                    Database.Current.Update(this.ID, ValueTables.StringValue, Columns.String, value);
                    NotifyPropertyChanged("String");
                }
            }
        }
        #endregion Property: String

        #region Property: Variable
        /// <summary>
        /// Gets or sets the string variable to represent the string instead.
        /// </summary>
        public StringVariable Variable
        {
            get
            {
                return Database.Current.Select<StringVariable>(this.ID, ValueTables.StringValue, Columns.Variable);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.StringValue, Columns.Variable, value);
                NotifyPropertyChanged("Variable");
            }
        }
        #endregion Property: Variable

        #region Property: MaxLength
        /// <summary>
        /// Gets or sets the maximum length of the string.
        /// </summary>
        public uint MaxLength
        {
            get
            {
                return Database.Current.Select<uint>(this.ID, ValueTables.StringValue, Columns.MaxLength);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.StringValue, Columns.MaxLength, value);
                NotifyPropertyChanged("MaxLength");

                String str = this.String;
                if (str != null && str.Length > value)
                    this.String = str.Remove((int)value, str.Length - (int)value);
            }
        }
        #endregion Property: MaxLength

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: StringValue()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static StringValue()
        {
            // Variable
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(StringVariable), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.StringValue, typeof(StringValue), dict);
        }
        #endregion Static Constructor: StringValue()

        #region Constructor: StringValue(StringValue stringValue)
        /// <summary>
        /// Clones the string value.
        /// </summary>
        /// <param name="stringValue">The string value to clone.</param>
        public StringValue(StringValue stringValue)
            : base(stringValue)
        {
            if (stringValue != null)
            {
                Database.Current.StartChange();

                this.String = stringValue.String;
                if (stringValue.Variable != null)
                    this.Variable = stringValue.Variable;
                this.MaxLength = stringValue.MaxLength;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: StringValue(StringValue stringValue)

        #region Constructor: StringValue(uint id)
        /// <summary>
        /// Creates a string value from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a value from.</param>
        private StringValue(uint id)
            : base(id)
        {
        }
        #endregion Constructor: StringValue(uint id)

        #region Constructor: StringValue(string value)
        /// <summary>
        /// Creates a string value from the given string.
        /// </summary>
        /// <param name="value">The string to create a string value from.</param>
        public StringValue(string value)
            : base()
        {
            Database.Current.StartChange();

            this.MaxLength = SemanticsSettings.Values.MaxLength;
            this.String = value;

            Database.Current.StopChange();
        }

        /// <summary>
        /// Creates a string value from the given string.
        /// </summary>
        /// <param name="value">The string to create a string value from.</param>
        /// <returns>A string value from the given string.</returns>
        public static implicit operator StringValue(string value)
        {
            return new StringValue(value);
        }
        #endregion Constructor: StringValue(string value)

        #region Constructor: StringValue(StringVariable stringVariable)
        /// <summary>
        /// Creates a string value from the given string variable.
        /// </summary>
        /// <param name="stringVariable">The string variable to create a string value from.</param>
        public StringValue(StringVariable stringVariable)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = stringVariable;

            Database.Current.StopChange();
        }
        #endregion Constructor: StringValue(StringVariable stringVariable)

        #endregion Method Group: Constructors

        #region Method Group: Other

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
    #endregion Class: StringValue

    #region Class: VectorValue
    /// <summary>
    /// A value that consists of a four-dimensional vector.
    /// </summary>
    public sealed class VectorValue : Value
    {

        #region Properties and Fields

        #region Property: Vector
        /// <summary>
        /// A handler for a change in the vector.
        /// </summary>
        private Vec4.Vec4Handler vectorChanged;
        
        /// <summary>
        /// The vector value.
        /// </summary>
        private Vec4 vector = null;
        
        /// <summary>
        /// Gets or sets the vector value.
        /// </summary>
        public Vec4 Vector
        {
            get
            {
                if (vector == null)
                {
                    vector = Database.Current.Select<Vec4>(this.ID, ValueTables.VectorValue, Columns.Vector);

                    if (vector == null)
                        vector = Vec4.Zero;
                    
                    if (vectorChanged == null)
                        vectorChanged = new Vec4.Vec4Handler(vector_ValueChanged);

                    vector.ValueChanged += vectorChanged;
                }

                return vector;
            }
            set
            {
                if (vectorChanged == null)
                    vectorChanged = new Vec4.Vec4Handler(vector_ValueChanged);

                if (vector != null)
                    vector.ValueChanged -= vectorChanged;

                // Make sure the value does not exceed the min or max
                Vec4 min = this.Min;
                Vec4 max = this.Max;
                if (min != null && max != null)
                {
                    ignoreVectorChange = true;
                    vector.X = Math.Max(min.X, Math.Min(value.X, max.X));
                    vector.Y = Math.Max(min.Y, Math.Min(value.Y, max.Y));
                    vector.Z = Math.Max(min.Z, Math.Min(value.Z, max.Z));
                    vector.W = Math.Max(min.W, Math.Min(value.W, max.W));
                    ignoreVectorChange = false;
                }
                Database.Current.Update(this.ID, ValueTables.VectorValue, Columns.Vector, vector);

                if (vector != null)
                    vector.ValueChanged += vectorChanged;

                NotifyPropertyChanged("Vector");
            }
        }

        /// <summary>
        /// Indicates whether a change in the vector should be ignored.
        /// </summary>
        private bool ignoreVectorChange = false;

        /// <summary>
        /// Updates the database when a value of the vector changes.
        /// </summary>
        private void vector_ValueChanged(Vec4 vec)
        {
            if (!ignoreVectorChange)
                this.Vector = vec;
        }
        #endregion Property: Vector

        #region Property: Variable
        /// <summary>
        /// Gets or sets the vector variable to represent the vector instead.
        /// </summary>
        public VectorVariable Variable
        {
            get
            {
                return Database.Current.Select<VectorVariable>(this.ID, ValueTables.VectorValue, Columns.Variable);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.VectorValue, Columns.Variable, value);
                NotifyPropertyChanged("Variable");
            }
        }
        #endregion Property: Variable

        #region Property: Min
        /// <summary>
        /// A handler for a change in the minimum.
        /// </summary>
        private Vec4.Vec4Handler minChanged;
        
        /// <summary>
        /// The minimum value.
        /// </summary>
        private Vec4 min = null;

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public Vec4 Min
        {
            get
            {
                if (min == null)
                {
                    min = Database.Current.Select<Vec4>(this.ID, ValueTables.VectorValue, Columns.Min);

                    if (min == null)
                        min = new Vec4(SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MinValue);

                    if (minChanged == null)
                        minChanged = new Vec4.Vec4Handler(min_ValueChanged);

                    min.ValueChanged += minChanged;
                }

                return min;
            }
            set
            {
                if (minChanged == null)
                    minChanged = new Vec4.Vec4Handler(min_ValueChanged);

                if (min != null)
                    min.ValueChanged -= minChanged;

                // Make sure the minimum does not exceed the maximum
                if (!ignoreMinExceeding && value != null)
                {
                    Vec4 max = this.Max;
                    if (max != null)
                    {
                        ignoreMinChange = true;
                        value.X = Math.Min(value.X, max.X);
                        value.Y = Math.Min(value.Y, max.Y);
                        value.Z = Math.Min(value.Z, max.Z);
                        value.W = Math.Min(value.W, max.W);
                        ignoreMinChange = false;
                    }
                }

                min = value;
                Database.Current.Update(this.ID, ValueTables.VectorValue, Columns.Min, min);

                // Make sure the value does not exceed the minimum
                if (value != null)
                {
                    Vec4 vec = this.Vector;
                    if (vec == null)
                        this.Vector = new Vec4(value);
                    else
                    {
                        float x = Math.Max(vec.X, value.X);
                        float y = Math.Max(vec.Y, value.Y);
                        float z = Math.Max(vec.Z, value.Z);
                        float w = Math.Max(vec.W, value.W);
                        Vec4 newVec = new Vec4(x, y, z, w);
                        if (vec.X != newVec.X || vec.Y != newVec.Y || vec.Z != newVec.Z || vec.W != newVec.W)
                            this.Vector = newVec;
                    }
                }

                if (min != null)
                    min.ValueChanged += minChanged;

                NotifyPropertyChanged("Min");
            }
        }

        /// <summary>
        /// Indicates whether an exceeding correction for the minimum should be applied.
        /// </summary>
        private bool ignoreMinExceeding = false;

        /// <summary>
        /// Indicates whether a change in the min should be ignored.
        /// </summary>
        private bool ignoreMinChange = false;

        /// <summary>
        /// Updates the database when a value of the minimum changes.
        /// </summary>
        private void min_ValueChanged(Vec4 vec)
        {
            if (!ignoreMinChange)
                this.Min = vec;
        }
        #endregion Property: Min

        #region Property: Max
        /// <summary>
        /// A handler for a change in the maximum.
        /// </summary>
        private Vec4.Vec4Handler maxChanged;

        /// <summary>
        /// The maximum value.
        /// </summary>
        private Vec4 max = null;

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public Vec4 Max
        {
            get
            {
                if (max == null)
                {
                    max = Database.Current.Select<Vec4>(this.ID, ValueTables.VectorValue, Columns.Max);

                    if (max == null)
                        max = new Vec4(SemanticsSettings.Values.MaxValue, SemanticsSettings.Values.MaxValue, SemanticsSettings.Values.MaxValue, SemanticsSettings.Values.MaxValue);

                    if (maxChanged == null)
                        maxChanged = new Vec4.Vec4Handler(max_ValueChanged);

                    max.ValueChanged += maxChanged;
                }

                return max;
            }
            set
            {
                if (maxChanged == null)
                    maxChanged = new Vec4.Vec4Handler(max_ValueChanged);

                if (max != null)
                    max.ValueChanged -= maxChanged;

                // Make sure the maximum does not exceed the minimum
                Vec4 min = this.Min;
                if (min != null && value != null)
                {
                    ignoreMaxChange = true;
                    value.X = Math.Max(value.X, min.X);
                    value.Y = Math.Max(value.Y, min.Y);
                    value.Z = Math.Max(value.Z, min.Z);
                    value.W = Math.Max(value.W, min.W);
                    ignoreMaxChange = false;
                }

                max = value;
                Database.Current.Update(this.ID, ValueTables.VectorValue, Columns.Max, max);

                // Make sure the value does not exceed the maximum
                if (value != null)
                {
                    Vec4 vec = this.Vector;
                    if (vec == null)
                        this.Vector = new Vec4(value);
                    else
                    {
                        float x = Math.Min(vec.X, value.X);
                        float y = Math.Min(vec.Y, value.Y);
                        float z = Math.Min(vec.Z, value.Z);
                        float w = Math.Min(vec.W, value.W);
                        Vec4 newVec = new Vec4(x, y, z, w);
                        if (vec.X != newVec.X || vec.Y != newVec.Y || vec.Z != newVec.Z || vec.W != newVec.W)
                            this.Vector = newVec;
                    }
                }

                if (max != null)
                    max.ValueChanged += maxChanged;

                NotifyPropertyChanged("Max");
            }
        }

        /// <summary>
        /// Indicates whether a change in the max should be ignored.
        /// </summary>
        private bool ignoreMaxChange = false;

        /// <summary>
        /// Updates the database when a value of the maximum changes.
        /// </summary>
        private void max_ValueChanged(Vec4 vec)
        {
            if (!ignoreMaxChange)
                this.Max = vec;
        }
        #endregion Property: Max

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: VectorValue()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static VectorValue()
        {
            // Variable
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(VectorVariable), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.VectorValue, typeof(VectorValue), dict);
        }
        #endregion Static Constructor: VectorValue()

        #region Constructor: VectorValue(VectorValue vectorValue)
        /// <summary>
        /// Clones the vector value.
        /// </summary>
        /// <param name="vectorValue">The vector value to clone.</param>
        public VectorValue(VectorValue vectorValue)
            : base(vectorValue)
        {
            if (vectorValue != null)
            {
                Database.Current.StartChange();

                if (vectorValue.Min != null)
                {
                    this.ignoreMinExceeding = true;
                    this.Min = new Vec4(vectorValue.Min);
                    this.ignoreMinExceeding = false;
                }
                if (vectorValue.Max != null)
                    this.Max = new Vec4(vectorValue.Max);
                if (vectorValue.Vector != null)
                    this.Vector = new Vec4(vectorValue.Vector);
                if (vectorValue.Variable != null)
                    this.Variable = vectorValue.Variable;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: VectorValue(VectorValue vectorValue)

        #region Constructor: VectorValue(uint id)
        /// <summary>
        /// Creates a vector value from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a value from.</param>
        private VectorValue(uint id)
            : base(id)
        {
        }
        #endregion Constructor: VectorValue(uint id)

        #region Constructor: VectorValue(Vec2 value)
        /// <summary>
        /// Creates a vector value from the given vector.
        /// </summary>
        /// <param name="value">The vector to create a vector value from.</param>
        public VectorValue(Vec2 value)
            : this(new Vec4(value))
        {
        }

        /// <summary>
        /// Creates a vector value from the given vector.
        /// </summary>
        /// <param name="value">The vector to create a vector value from.</param>
        /// <returns>A vector value from the given vector.</returns>
        public static implicit operator VectorValue(Vec2 value)
        {
            return new VectorValue(new Vec4(value));
        }
        #endregion Constructor: VectorValue(Vec2 value)

        #region Constructor: VectorValue(Vec3 value)
        /// <summary>
        /// Creates a vector value from the given vector.
        /// </summary>
        /// <param name="value">The vector to create a vector value from.</param>
        public VectorValue(Vec3 value)
            : this(new Vec4(value))
        {
        }

        /// <summary>
        /// Creates a vector value from the given vector.
        /// </summary>
        /// <param name="value">The vector to create a vector value from.</param>
        /// <returns>A vector value from the given vector.</returns>
        public static implicit operator VectorValue(Vec3 value)
        {
            return new VectorValue(new Vec4(value));
        }
        #endregion Constructor: VectorValue(Vec3 value)

        #region Constructor: VectorValue(Vec4 value)
        /// <summary>
        /// Creates a vector value from the given vector.
        /// </summary>
        /// <param name="value">The vector to create a vector value from.</param>
        public VectorValue(Vec4 value)
            : base()
        {
            Database.Current.StartChange();
            Database.Current.QueryBegin();

            this.ignoreMinExceeding = true;
            this.Min = new Vec4(SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MinValue);
            this.ignoreMinExceeding = false;
            this.Max = new Vec4(SemanticsSettings.Values.MaxValue, SemanticsSettings.Values.MaxValue, SemanticsSettings.Values.MaxValue, SemanticsSettings.Values.MaxValue);
            this.Vector = value;
            
            Database.Current.QueryCommit();
            Database.Current.StartChange();
        }

        /// <summary>
        /// Creates a vector value from the given vector.
        /// </summary>
        /// <param name="value">The vector to create a vector value from.</param>
        /// <returns>A vector value from the given vector.</returns>
        public static implicit operator VectorValue(Vec4 value)
        {
            return new VectorValue(value);
        }
        #endregion Constructor: VectorValue(Vec4 value)

        #region Constructor: VectorValue(VectorVariable vectorVariable)
        /// <summary>
        /// Creates a vector value from the given vector variable.
        /// </summary>
        /// <param name="vectorVariable">The vector variable to create a vector value from.</param>
        public VectorValue(VectorVariable vectorVariable)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = vectorVariable;

            Database.Current.StopChange();
        }
        #endregion Constructor: VectorValue(VectorVariable vectorVariable)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: ToString()
        /// <summary>
        /// Returns a vector representation.
        /// </summary>
        /// <returns>The vector representation.</returns>
        public override string ToString()
        {
            return this.Vector.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: VectorValue

    #region Class: TermValue
    /// <summary>
    /// A value that consists of a term.
    /// </summary>
    public sealed class TermValue : Value
    {

        #region Properties and Fields

        #region Property: Function1
        /// <summary>
        /// Gets or sets the (optional) function on the first value.
        /// </summary>
        public Function? Function1
        {
            get
            {
                return Database.Current.Select<Function?>(this.ID, ValueTables.TermValue, Columns.Function1);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TermValue, Columns.Function1, value);
                NotifyPropertyChanged("Function1");
            }
        }
        #endregion Property: Function1

        #region Property: Value1
        /// <summary>
        /// Gets or sets the (required) first value. Only valid for NumericalValue and TermValue!
        /// </summary>
        public Value Value1
        {
            get
            {
                return Database.Current.Select<Value>(this.ID, ValueTables.TermValue, Columns.Value1);
            }
            set
            {
                if (this.Value1 != null)
                    this.Value1.Remove();

                if (value is NumericalValue || value is TermValue)
                {
                    Database.Current.Update(this.ID, ValueTables.TermValue, Columns.Value1, value);
                    NotifyPropertyChanged("Value1");
                }
            }
        }
        #endregion Property: Value1

        #region Property: Operator
        /// <summary>
        /// Gets or sets the (optional) operator between both values.
        /// </summary>
        public Operator? Operator
        {
            get
            {
                return Database.Current.Select<Operator?>(this.ID, ValueTables.TermValue, Columns.Operator);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TermValue, Columns.Operator, value);
                NotifyPropertyChanged("Operator");
            }
        }
        #endregion Property: Operator

        #region Property: Function2
        /// <summary>
        /// Gets or sets the (optional) function on the second value.
        /// </summary>
        public Function? Function2
        {
            get
            {
                return Database.Current.Select<Function?>(this.ID, ValueTables.TermValue, Columns.Function2);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TermValue, Columns.Function2, value);
                NotifyPropertyChanged("Function2");
            }
        }
        #endregion Property: Function2

        #region Property: Value2
        /// <summary>
        /// Gets or sets the (optional) second value. Only valid for NumericalValue and TermValue!
        /// </summary>
        public Value Value2
        {
            get
            {
                return Database.Current.Select<Value>(this.ID, ValueTables.TermValue, Columns.Value2);
            }
            set
            {
                if (this.Value2 != null)
                    this.Value2.Remove();

                if (value is NumericalValue || value is TermValue)
                {
                    Database.Current.Update(this.ID, ValueTables.TermValue, Columns.Value2, value);
                    NotifyPropertyChanged("Value2");
                }
            }
        }
        #endregion Property: Value2

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TermValue(TermValue termValue)
        /// <summary>
        /// Clones the term value.
        /// </summary>
        /// <param name="termValue">The term value to clone.</param>
        public TermValue(TermValue termValue)
            : base(termValue)
        {
            if (termValue != null)
            {
                Database.Current.StartChange();

                this.Function1 = termValue.Function1;
                if (termValue.Value1 != null)
                    this.Value1 = termValue.Value1.Clone();
                this.Operator = termValue.Operator;
                this.Function2 = termValue.Function2;
                if (termValue.Value2 != null)
                    this.Value2 = termValue.Value2.Clone();

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: TermValue(TermValue termValue)

        #region Constructor: TermValue(uint id)
        /// <summary>
        /// Creates a term value from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a value from.</param>
        private TermValue(uint id)
            : base(id)
        {
        }
        #endregion Constructor: TermValue(uint id)

        #region Constructor: TermValue(Value value)
        /// <summary>
        /// Creates a new term value from the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public TermValue(Value value)
            : base()
        {
            Database.Current.StartChange();

            this.Value1 = value;

            Database.Current.StopChange();
        }
        #endregion Constructor: TermValue(Value value)

        #region Constructor: TermValue(Value value1, Operator op, Value value2)
        /// <summary>
        /// Creates a new term value from the given values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        public TermValue(Value value1, Operator op, Value value2)
            : this(value1)
        {
            Database.Current.StartChange();

            this.Operator = op;
            this.Value2 = value2;

            Database.Current.StopChange();
        }
        #endregion Constructor: TermValue(Value value1, Operator op, Value value2)

        #region Constructor: TermValue(Function function, Value value)
        /// <summary>
        /// Creates a new term value from the given function and value.
        /// </summary>
        /// <param name="function">The function to use on the value.</param>
        /// <param name="value">The value.</param>
        public TermValue(Function function, Value value)
            : base()
        {
            Database.Current.StartChange();

            this.Function1 = function;
            this.Value1 = value;

            Database.Current.StopChange();
        }
        #endregion Constructor: TermValue(Function function, Value value)

        #region Constructor: TermValue(Function function1, Value value1, Operator op, Function function2, Value value2)
        /// <summary>
        /// Creates a new term value from the given functions and values.
        /// </summary>
        /// <param name="function1">The function to use on the first value.</param>
        /// <param name="value1">The first value.</param>
        /// <param name="function2">The function to use on the second value.</param>
        /// <param name="value2">The second value.</param>
        public TermValue(Function function1, Value value1, Operator op, Function function2, Value value2)
            : this(function1, value1)
        {
            Database.Current.StartChange();

            this.Operator = op;
            this.Function2 = function2;
            this.Value2 = value2;

            Database.Current.StopChange();
        }
        #endregion Constructor: TermValue(Function function1, Value value1, Operator op, Function function2, Value value2)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            string str = "(";

            if (this.Function1 != null)
                str += this.Function1.ToString() + "(" + this.Value1.ToString() + ")";
            else
                str += this.Value1.ToString();

            if (this.Operator != null)
                str += " " + this.Operator.ToString() + " ";

            if (this.Function2 != null)
                str += this.Function2.ToString() + "(" + this.Value2.ToString() + ")";
            else
                str += this.Value2.ToString();

            str += ")";

            return str;
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: TermValue

    #region Class: ValueCondition
    /// <summary>
    /// A condition on a value.
    /// </summary>
    public abstract class ValueCondition : Condition
    {

        #region Method Group: Constructors

        #region Constructor: ValueCondition()
        /// <summary>
        /// Creates a new value condition.
        /// </summary>
        protected ValueCondition()
            : base()
        {
        }
        #endregion Constructor: ValueCondition()

        #region Constructor: ValueCondition(uint id)
        /// <summary>
        /// Creates a new value condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a value condition from.</param>
        protected ValueCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ValueCondition(uint id)

        #region Constructor: ValueCondition(ValueCondition valueCondition)
        /// <summary>
        /// Clones a value condition.
        /// </summary>
        /// <param name="valueCondition">The value condition to clone.</param>
        protected ValueCondition(ValueCondition valueCondition)
            : base(valueCondition)
        {
        }
        #endregion Constructor: ValueCondition(ValueCondition valueCondition)

        #region Method: Create(ValueType valueType)
        /// <summary>
        /// Creates a new value condition of the given value type.
        /// </summary>
        /// <param name="valueType">The value type to create a value condition from.</param>
        public static ValueCondition Create(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Numerical:
                    return new NumericalValueCondition(SemanticsSettings.Values.Value, EqualitySignExtended.Equal);
                case ValueType.Boolean:
                    return new BoolValueCondition(SemanticsSettings.Values.Boolean);
                case ValueType.String:
                    return new StringValueCondition(SemanticsSettings.Values.String, EqualitySign.Equal);
                case ValueType.Vector:
                    return new VectorValueCondition(new Vec4(SemanticsSettings.Values.Vector4X, SemanticsSettings.Values.Vector4Y, SemanticsSettings.Values.Vector4Y, SemanticsSettings.Values.Vector4Z), EqualitySignExtended.Equal);
                default:
                    break;
            }
            return null;
        }
        #endregion Method: Create(ValueType valueType)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the value condition.
        /// </summary>
        /// <returns>A clone of the value condition.</returns>
        public new ValueCondition Clone()
        {
            return base.Clone() as ValueCondition;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: ValueCondition

    #region Class: BoolValueCondition
    /// <summary>
    /// A condition on a bool value.
    /// </summary>
    public class BoolValueCondition : ValueCondition
    {

        #region Properties and Fields

        #region Property: Bool
        /// <summary>
        /// Gets or sets the required boolean value.
        /// </summary>
        public bool? Bool
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, ValueTables.BoolValueCondition, Columns.Bool);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.BoolValueCondition, Columns.Bool, value);
                NotifyPropertyChanged("Bool");
            }
        }
        #endregion Property: Bool

        #region Property: Variable
        /// <summary>
        /// Gets or sets the bool variable to represent the bool instead.
        /// </summary>
        public BoolVariable Variable
        {
            get
            {
                return Database.Current.Select<BoolVariable>(this.ID, ValueTables.BoolValueCondition, Columns.Variable);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.BoolValueCondition, Columns.Variable, value);
                NotifyPropertyChanged("Variable");
            }
        }
        #endregion Property: Variable

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: BoolValueCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static BoolValueCondition()
        {
            // Variable
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(BoolVariable), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.BoolValueCondition, typeof(BoolValueCondition), dict);
        }
        #endregion Static Constructor: BoolValueCondition()

        #region Constructor: BoolValueCondition()
        /// <summary>
        /// Creates a new bool value condition.
        /// </summary>
        public BoolValueCondition()
            : base()
        {
        }
        #endregion Constructor: BoolValueCondition()

        #region Constructor: BoolValueCondition(uint id)
        /// <summary>
        /// Creates a new bool value condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a bool value condition from.</param>
        internal BoolValueCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: BoolValueCondition(uint id)

        #region Constructor: BoolValueCondition(BoolValueCondition boolValueCondition)
        /// <summary>
        /// Clones a bool value condition.
        /// </summary>
        /// <param name="boolValueCondition">The bool value condition to clone.</param>
        public BoolValueCondition(BoolValueCondition boolValueCondition)
            : base(boolValueCondition)
        {
            if (boolValueCondition != null)
            {
                Database.Current.StartChange();

                this.Bool = boolValueCondition.Bool;
                if (boolValueCondition.Variable != null)
                    this.Variable = boolValueCondition.Variable;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: BoolValueCondition(BoolValueCondition boolValueCondition)

        #region Constructor: BoolValueCondition(bool boolean)
        /// <summary>
        /// Creates a bool value condition for the given bool.
        /// </summary>
        /// <param name="bool">The bool to create a bool value condition for.</param>
        public BoolValueCondition(bool boolean)
            : base()
        {
            Database.Current.StartChange();

            this.Bool = boolean;

            Database.Current.StopChange();
        }
        #endregion Constructor: BoolValueCondition(bool boolean)

        #region Constructor: BoolValueCondition(BoolVariable boolVariable)
        /// <summary>
        /// Creates a bool value condition from the given bool variable.
        /// </summary>
        /// <param name="boolVariable">The bool variable to create a bool value condition from.</param>
        public BoolValueCondition(BoolVariable boolVariable)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = boolVariable;

            Database.Current.StopChange();
        }
        #endregion Constructor: BoolValueCondition(BoolVariable boolVariable)

        #endregion Method Group: Constructors

    }
    #endregion Class: BoolValueCondition

    #region Class: NumericalValueCondition
    /// <summary>
    /// A condition on a numerical value.
    /// </summary>
    public class NumericalValueCondition : ValueCondition
    {

        #region Properties and Fields

        #region Property: Value
        /// <summary>
        /// Gets or sets the required numerical value.
        /// </summary>
        public float? Value
        {
            get
            {
                return Database.Current.Select<float?>(this.ID, ValueTables.NumericalValueCondition, Columns.Value);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueCondition, Columns.Value, value);
                NotifyPropertyChanged("Value");
            }
        }
        #endregion Property: Value

        #region Property: Variable
        /// <summary>
        /// Gets or sets the variable to represent the value instead. Only valid for numerical and term variables!
        /// </summary>
        public Variable Variable
        {
            get
            {
                return Database.Current.Select<Variable>(this.ID, ValueTables.NumericalValueCondition, Columns.Variable);
            }
            set
            {
                if (value is NumericalVariable || value is TermVariable)
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValueCondition, Columns.Variable, value);
                    NotifyPropertyChanged("Variable");
                }
            }
        }
        #endregion Property: Variable

        #region Property: ValueSign
        /// <summary>
        /// Gets or sets the sign for the value in the condition.
        /// </summary>
        public EqualitySignExtended? ValueSign
        {
            get
            {
                return Database.Current.Select<EqualitySignExtended?>(this.ID, ValueTables.NumericalValueCondition, Columns.ValueSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueCondition, Columns.ValueSign, value);
                NotifyPropertyChanged("ValueSign");
            }
        }
        #endregion Property: ValueSign

        #region Property: Prefix
        /// <summary>
        /// Gets or sets the prefix of the value.
        /// </summary>
        public Prefix Prefix
        {
            get
            {
                return Database.Current.Select<Prefix>(this.ID, ValueTables.NumericalValueCondition, Columns.Prefix);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueCondition, Columns.Prefix, value);
                NotifyPropertyChanged("Prefix");
            }
        }
        #endregion Property: Prefix

        #region Property: Unit
        /// <summary>
        /// Gets or sets the unit of the value.
        /// </summary>
        public Unit Unit
        {
            get
            {
                return Database.Current.Select<Unit>(this.ID, ValueTables.NumericalValueCondition, Columns.Unit);
            }
            set
            {
                if (value == null || this.UnitCategory == null || this.UnitCategory.HasUnit(value))
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValueCondition, Columns.Unit, value);
                    NotifyPropertyChanged("Unit");
                }
            }
        }
        #endregion Property: Unit

        #region Property: UnitCategory
        /// <summary>
        /// Gets or sets the optional unit category of this numerical value condition, to which the unit should be restricted.
        /// </summary>
        public UnitCategory UnitCategory
        {
            get
            {
                return Database.Current.Select<UnitCategory>(this.ID, ValueTables.NumericalValueCondition, Columns.UnitCategory);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueCondition, Columns.UnitCategory, value);
                NotifyPropertyChanged("UnitCategory");

                if (value != null && !value.HasUnit(this.Unit))
                    this.Unit = null;
            }
        }
        #endregion Property: UnitCategory

        #region Property: Min
        /// <summary>
        /// Gets or sets the required minimum value.
        /// </summary>
        public float? Min
        {
            get
            {
                return Database.Current.Select<float?>(this.ID, ValueTables.NumericalValueCondition, Columns.Min);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueCondition, Columns.Min, value);
                NotifyPropertyChanged("Min");
            }
        }
        #endregion Property: Min

        #region Property: MinVariable
        /// <summary>
        /// Gets or sets the variable to represent the min value instead. Only valid for numerical and term variables!
        /// </summary>
        public Variable MinVariable
        {
            get
            {
                return Database.Current.Select<Variable>(this.ID, ValueTables.NumericalValueCondition, Columns.MinVariable);
            }
            set
            {
                if (value is NumericalVariable || value is TermVariable)
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValueCondition, Columns.MinVariable, value);
                    NotifyPropertyChanged("MinVariable");
                }
            }
        }
        #endregion Property: MinVariable

        #region Property: MinSign
        /// <summary>
        /// Gets or sets the sign for the minimum value in the condition.
        /// </summary>
        public EqualitySignExtended? MinSign
        {
            get
            {
                return Database.Current.Select<EqualitySignExtended?>(this.ID, ValueTables.NumericalValueCondition, Columns.MinSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueCondition, Columns.MinSign, value);
                NotifyPropertyChanged("MinSign");
            }
        }
        #endregion Property: MinSign

        #region Property: Max
        /// <summary>
        /// Gets or sets the required maximum value.
        /// </summary>
        public float? Max
        {
            get
            {
                return Database.Current.Select<float?>(this.ID, ValueTables.NumericalValueCondition, Columns.Max);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueCondition, Columns.Max, value);
                NotifyPropertyChanged("Max");
            }
        }
        #endregion Property: Max

        #region Property: MaxVariable
        /// <summary>
        /// Gets or sets the variable to represent the max value instead. Only valid for numerical and term variables!
        /// </summary>
        public Variable MaxVariable
        {
            get
            {
                return Database.Current.Select<Variable>(this.ID, ValueTables.NumericalValueCondition, Columns.MaxVariable);
            }
            set
            {
                if (value is NumericalVariable || value is TermVariable)
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValueCondition, Columns.MaxVariable, value);
                    NotifyPropertyChanged("MaxVariable");
                }
            }
        }
        #endregion Property: MaxVariable

        #region Property: MaxSign
        /// <summary>
        /// Gets or sets the sign for the maximum value in the condition.
        /// </summary>
        public EqualitySignExtended? MaxSign
        {
            get
            {
                return Database.Current.Select<EqualitySignExtended?>(this.ID, ValueTables.NumericalValueCondition, Columns.MaxSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueCondition, Columns.MaxSign, value);
                NotifyPropertyChanged("MaxSign");
            }
        }
        #endregion Property: MaxSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: NumericalValueCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static NumericalValueCondition()
        {
            // Variable, unit, and unit category
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(NumericalVariable), EntryType.Nullable));
            dict.Add(Columns.Unit, new Tuple<Type, EntryType>(typeof(Unit), EntryType.Nullable));
            dict.Add(Columns.UnitCategory, new Tuple<Type, EntryType>(typeof(UnitCategory), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.NumericalValueCondition, typeof(NumericalValue), dict);
        }
        #endregion Static Constructor: NumericalValueCondition()

        #region Constructor: NumericalValueCondition()
        /// <summary>
        /// Creates a new numerical value condition.
        /// </summary>
        public NumericalValueCondition()
            : base()
        {
        }
        #endregion Constructor: NumericalValueCondition()

        #region Constructor: NumericalValueCondition(uint id)
        /// <summary>
        /// Creates a new numerical value condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a numerical value condition from.</param>
        internal NumericalValueCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: NumericalValueCondition(uint id)

        #region Constructor: NumericalValueCondition(NumericalValueCondition numericalValueCondition)
        /// <summary>
        /// Clones a numerical value condition.
        /// </summary>
        /// <param name="numericalValueCondition">The numerical value condition to clone.</param>
        public NumericalValueCondition(NumericalValueCondition numericalValueCondition)
            : base(numericalValueCondition)
        {
            if (numericalValueCondition != null)
            {
                Database.Current.StartChange();

                this.Value = numericalValueCondition.Value;
                if (numericalValueCondition.Variable != null)
                    this.Variable = numericalValueCondition.Variable;
                this.ValueSign = numericalValueCondition.ValueSign;
                this.Prefix = numericalValueCondition.Prefix;
                this.Unit = numericalValueCondition.Unit;
                this.UnitCategory = numericalValueCondition.UnitCategory;
                this.Min = numericalValueCondition.Min;
                if (numericalValueCondition.MinVariable != null)
                    this.MinVariable = numericalValueCondition.MinVariable;
                this.MinSign = numericalValueCondition.MinSign;
                this.Max = numericalValueCondition.Max;
                if (numericalValueCondition.MaxVariable != null)
                    this.MaxVariable = numericalValueCondition.MaxVariable;
                this.MaxSign = numericalValueCondition.MaxSign;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: NumericalValueCondition(NumericalValueCondition numericalValueCondition)

        #region Constructor: NumericalValueCondition(float val, EqualitySignExtended valueSign)
        /// <summary>
        /// Creates a numerical value condition for the given value.
        /// </summary>
        /// <param name="val">The value to create a numerical value condition for.</param>
        /// <param name="valueSign">The sign of the value.</param>
        public NumericalValueCondition(float val, EqualitySignExtended valueSign)
            : base()
        {
            Database.Current.StartChange();

            this.Value = val;
            this.ValueSign = valueSign;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValueCondition(float val, EqualitySignExtended valueSign)

        #region Constructor: NumericalValueCondition(NumericalVariable numericalVariable)
        /// <summary>
        /// Creates a numerical value condition from the given numerical variable.
        /// </summary>
        /// <param name="numericalVariable">The numerical variable to create a numerical value condition from.</param>
        public NumericalValueCondition(NumericalVariable numericalVariable)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = numericalVariable;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValueCondition(NumericalVariable numericalVariable)

        #endregion Method Group: Constructors

    }
    #endregion Class: NumericalValueCondition

    #region Class: StringValueCondition
    /// <summary>
    /// A condition on a string value.
    /// </summary>
    public class StringValueCondition : ValueCondition
    {

        #region Properties and Fields

        #region Property: String
        /// <summary>
        /// Gets or sets the required string value.
        /// </summary>
        public string String
        {
            get
            {
                return Database.Current.Select<string>(this.ID, ValueTables.StringValueCondition, Columns.String);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.StringValueCondition, Columns.String, value);
                NotifyPropertyChanged("String");
            }
        }
        #endregion Property: String

        #region Property: Variable
        /// <summary>
        /// Gets or sets the string variable to represent the string instead.
        /// </summary>
        public StringVariable Variable
        {
            get
            {
                return Database.Current.Select<StringVariable>(this.ID, ValueTables.StringValueCondition, Columns.Variable);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.StringValueCondition, Columns.Variable, value);
                NotifyPropertyChanged("Variable");
            }
        }
        #endregion Property: Variable

        #region Property: StringSign
        /// <summary>
        /// Gets or sets the sign for the string in the condition.
        /// </summary>
        public EqualitySign? StringSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.StringValueCondition, Columns.StringSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.StringValueCondition, Columns.StringSign, value);
                NotifyPropertyChanged("StringSign");
            }
        }
        #endregion Property: StringSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: StringValueCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static StringValueCondition()
        {
            // Variable
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(StringVariable), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.StringValueCondition, typeof(StringValueCondition), dict);
        }
        #endregion Static Constructor: StringValueCondition()

        #region Constructor: StringValueCondition()
        /// <summary>
        /// Creates a new string value condition.
        /// </summary>
        public StringValueCondition()
            : base()
        {
        }
        #endregion Constructor: StringValueCondition()

        #region Constructor: StringValueCondition(uint id)
        /// <summary>
        /// Creates a new string value condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a string value condition from.</param>
        internal StringValueCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: StringValueCondition(uint id)

        #region Constructor: StringValueCondition(StringValueCondition stringValueCondition)
        /// <summary>
        /// Clones a string value condition.
        /// </summary>
        /// <param name="stringValueCondition">The string value condition to clone.</param>
        public StringValueCondition(StringValueCondition stringValueCondition)
            : base(stringValueCondition)
        {
            if (stringValueCondition != null)
            {
                Database.Current.StartChange();

                this.String = stringValueCondition.String;
                if (stringValueCondition.Variable != null)
                    this.Variable = stringValueCondition.Variable;
                this.StringSign = stringValueCondition.StringSign;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: StringValueCondition(StringValueCondition stringValueCondition)

        #region Constructor: StringValueCondition(string str, EqualitySign stringSign)
        /// <summary>
        /// Creates a string value condition for the given string.
        /// </summary>
        /// <param name="str">The string to create a string value condition for.</param>
        /// <param name="stringSign">The sign of the string.</param>
        public StringValueCondition(string str, EqualitySign stringSign)
            : base()
        {
            Database.Current.StartChange();

            this.String = str;
            this.StringSign = stringSign;

            Database.Current.StopChange();
        }
        #endregion Constructor: StringValueCondition(string str, EqualitySign stringSign)

        #region Constructor: StringValueCondition(StringVariable stringVariable)
        /// <summary>
        /// Creates a string value condition from the given string variable.
        /// </summary>
        /// <param name="stringVariable">The string variable to create a string value condition from.</param>
        public StringValueCondition(StringVariable stringVariable)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = stringVariable;

            Database.Current.StopChange();
        }
        #endregion Constructor: StringValueCondition(StringVariable stringVariable)

        #endregion Method Group: Constructors

    }
    #endregion Class: StringValueCondition

    #region Class: VectorValueCondition
    /// <summary>
    /// A condition on a vector value.
    /// </summary>
    public class VectorValueCondition : ValueCondition
    {

        #region Properties and Fields

        #region Property: Vector
        /// <summary>
        /// A handler for a change in the vector.
        /// </summary>
        private Vec4.Vec4Handler vectorChanged;

        /// <summary>
        /// The required four-dimensional vector.
        /// </summary>
        private Vec4 vector = null;

        /// <summary>
        /// Gets or sets the required four-dimensional vector.
        /// </summary>
        public Vec4 Vector
        {
            get
            {
                if (vector == null)
                {
                    vector = Database.Current.Select<Vec4>(this.ID, ValueTables.VectorValueCondition, Columns.Vector);

                    if (vector != null)
                    {
                        if (vectorChanged == null)
                            vectorChanged = new Vec4.Vec4Handler(vector_ValueChanged);

                        vector.ValueChanged += vectorChanged;
                    }
                }

                return vector;
            }
            set
            {
                if (vectorChanged == null)
                    vectorChanged = new Vec4.Vec4Handler(vector_ValueChanged);

                if (vector != null)
                    vector.ValueChanged -= vectorChanged;

                vector = value;
                Database.Current.Update(this.ID, ValueTables.VectorValueCondition, Columns.Vector, vector);
                NotifyPropertyChanged("Vector");

                if (vector != null)
                    vector.ValueChanged += vectorChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the vector changes.
        /// </summary>
        private void vector_ValueChanged(Vec4 vec)
        {
            Database.Current.Update(this.ID, ValueTables.VectorValueCondition, Columns.Vector, vector);
            NotifyPropertyChanged("Vector");
        }
        #endregion Property: Vector

        #region Property: Variable
        /// <summary>
        /// Gets or sets the vector variable to represent the vector instead.
        /// </summary>
        public VectorVariable Variable
        {
            get
            {
                return Database.Current.Select<VectorVariable>(this.ID, ValueTables.VectorValueCondition, Columns.Variable);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.VectorValueCondition, Columns.Variable, value);
                NotifyPropertyChanged("Variable");
            }
        }
        #endregion Property: Variable

        #region Property: VectorSign
        /// <summary>
        /// Gets or sets the sign for the vector value in the condition.
        /// </summary>
        public EqualitySignExtended? VectorSign
        {
            get
            {
                return Database.Current.Select<EqualitySignExtended?>(this.ID, ValueTables.VectorValueCondition, Columns.VectorSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.VectorValueCondition, Columns.VectorSign, value);
                NotifyPropertyChanged("VectorSign");
            }
        }
        #endregion Property: VectorSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: VectorValueCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static VectorValueCondition()
        {
            // Variable
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(VectorVariable), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.VectorValueCondition, typeof(VectorValueCondition), dict);
        }
        #endregion Static Constructor: VectorValueCondition()

        #region Constructor: VectorValueCondition()
        /// <summary>
        /// Creates a new vector value condition.
        /// </summary>
        public VectorValueCondition()
            : base()
        {
        }
        #endregion Constructor: VectorValueCondition()

        #region Constructor: VectorValueCondition(uint id)
        /// <summary>
        /// Creates a new vector value condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a vector value condition from.</param>
        internal VectorValueCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: VectorValueCondition(uint id)

        #region Constructor: VectorValueCondition(VectorValueCondition vectorValueCondition)
        /// <summary>
        /// Clones a vector value condition.
        /// </summary>
        /// <param name="vectorValueCondition">The vector value condition to clone.</param>
        public VectorValueCondition(VectorValueCondition vectorValueCondition)
            : base(vectorValueCondition)
        {
            if (vectorValueCondition != null)
            {
                Database.Current.StartChange();

                if (vectorValueCondition.Vector != null)
                    this.Vector = new Vec4(vectorValueCondition.Vector);
                if (vectorValueCondition.Variable != null)
                    this.Variable = vectorValueCondition.Variable;
                this.VectorSign = vectorValueCondition.VectorSign;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: VectorValueCondition(VectorValueCondition vectorValueCondition)

        #region Constructor: VectorValueCondition(Vec4 vector, EqualitySignExtended vectorSign)
        /// <summary>
        /// Creates a vector value condition for the given vector.
        /// </summary>
        /// <param name="vector">The vector to create a vector value condition for.</param>
        /// <param name="vectorSign">The sign of the vector.</param>
        public VectorValueCondition(Vec4 vector, EqualitySignExtended vectorSign)
            : base()
        {
            Database.Current.StartChange();

            this.Vector = vector;
            this.VectorSign = vectorSign;

            Database.Current.StopChange();
        }
        #endregion Constructor: VectorValueCondition(Vec4 vector, EqualitySignExtended vectorSign)

        #region Constructor: VectorValueCondition(VectorVariable vectorVariable)
        /// <summary>
        /// Creates a vector value condition from the given vector variable.
        /// </summary>
        /// <param name="vectorVariable">The vector variable to create a vector value condition from.</param>
        public VectorValueCondition(VectorVariable vectorVariable)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = vectorVariable;

            Database.Current.StopChange();
        }
        #endregion Constructor: VectorValueCondition(VectorVariable vectorVariable)

        #endregion Method Group: Constructors

    }
    #endregion Class: VectorValueCondition

    #region Class: ValueChange
    /// <summary>
    /// A change on a value.
    /// </summary>
    public abstract class ValueChange : Change
    {

        #region Method Group: Constructors

        #region Constructor: ValueChange()
        /// <summary>
        /// Creates a new value change.
        /// </summary>
        protected ValueChange()
            : base()
        {
        }
        #endregion Constructor: ValueChange()

        #region Constructor: ValueChange(uint id)
        /// <summary>
        /// Creates a new value change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a value change from.</param>
        protected ValueChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ValueChange(uint id)

        #region Constructor: ValueChange(ValueChange valueChange)
        /// <summary>
        /// Clones a value change.
        /// </summary>
        /// <param name="valueChange">The value change to clone.</param>
        protected ValueChange(ValueChange valueChange)
            : base(valueChange)
        {
        }
        #endregion Constructor: ValueChange(ValueChange valueChange)

        #region Method: Create(ValueType valueType)
        /// <summary>
        /// Creates a new value change of the given value type.
        /// </summary>
        /// <param name="valueType">The value type to create a value change from.</param>
        public static ValueChange Create(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Numerical:
                    return new NumericalValueChange(SemanticsSettings.Values.Value, ValueChangeType.Increase);
                case ValueType.Boolean:
                    return new BoolValueChange(SemanticsSettings.Values.Boolean);
                case ValueType.String:
                    return new StringValueChange(SemanticsSettings.Values.String);
                case ValueType.Vector:
                    return new VectorValueChange(new Vec4(SemanticsSettings.Values.Vector4X, SemanticsSettings.Values.Vector4Y, SemanticsSettings.Values.Vector4Y, SemanticsSettings.Values.Vector4Z), ValueChangeType.Increase);
                default:
                    break;
            }
            return null;
        }
        #endregion Method: Create(ValueType valueType)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the value change.
        /// </summary>
        /// <returns>A clone of the value change.</returns>
        public new ValueChange Clone()
        {
            return base.Clone() as ValueChange;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: ValueChange

    #region Class: BoolValueChange
    /// <summary>
    /// A change on a bool value.
    /// </summary>
    public class BoolValueChange : ValueChange
    {

        #region Properties and Fields

        #region Property: Bool
        /// <summary>
        /// Gets or sets the boolean value to change to.
        /// </summary>
        public bool? Bool
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, ValueTables.BoolValueChange, Columns.Bool);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.BoolValueChange, Columns.Bool, value);
                NotifyPropertyChanged("Bool");
            }
        }
        #endregion Property: Bool

        #region Property: Variable
        /// <summary>
        /// Gets or sets the bool variable to represent the bool instead.
        /// </summary>
        public BoolVariable Variable
        {
            get
            {
                return Database.Current.Select<BoolVariable>(this.ID, ValueTables.BoolValueChange, Columns.Variable);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.BoolValueChange, Columns.Variable, value);
                NotifyPropertyChanged("Variable");
            }
        }
        #endregion Property: Variable

        #region Property: Reverse
        /// <summary>
        /// Gets or sets the value that indicates whether the boolean should be reversed. Only valid when set to True. If set to False, Bool will be used instead.
        /// </summary>
        public bool Reverse
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.BoolValueChange, Columns.Reverse);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.BoolValueChange, Columns.Reverse, value);
                NotifyPropertyChanged("Reverse");
            }
        }
        #endregion Property: Reverse

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: BoolValueChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static BoolValueChange()
        {
            // Variable
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(BoolVariable), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.BoolValueChange, typeof(BoolValueChange), dict);
        }
        #endregion Static Constructor: BoolValueChange()

        #region Constructor: BoolValueChange()
        /// <summary>
        /// Creates a new bool value change.
        /// </summary>
        public BoolValueChange()
            : base()
        {
        }
        #endregion Constructor: BoolValueChange()

        #region Constructor: BoolValueChange(uint id)
        /// <summary>
        /// Creates a new bool value change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a bool value change from.</param>
        internal BoolValueChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: BoolValueChange(uint id)

        #region Constructor: BoolValueChange(BoolValueChange boolValueChange)
        /// <summary>
        /// Clones a bool value change.
        /// </summary>
        /// <param name="boolValueChange">The bool value change to clone.</param>
        public BoolValueChange(BoolValueChange boolValueChange)
            : base(boolValueChange)
        {
            if (boolValueChange != null)
            {
                Database.Current.StartChange();

                this.Bool = boolValueChange.Bool;
                if (boolValueChange.Variable != null)
                    this.Variable = boolValueChange.Variable;
                this.Reverse = boolValueChange.Reverse;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: BoolValueChange(BoolValueChange boolValueChange)

        #region Constructor: BoolValueChange(bool boolean)
        /// <summary>
        /// Creates a bool value change for the given bool.
        /// </summary>
        /// <param name="bool">The bool to create a bool value change for.</param>
        public BoolValueChange(bool boolean)
            : base()
        {
            Database.Current.StartChange();

            this.Bool = boolean;

            Database.Current.StopChange();
        }
        #endregion Constructor: BoolValueChange(bool boolean)

        #region Constructor: BoolValueChange(BoolVariable boolVariable)
        /// <summary>
        /// Creates a bool value change from the given bool variable.
        /// </summary>
        /// <param name="boolVariable">The bool variable to create a bool value change from.</param>
        public BoolValueChange(BoolVariable boolVariable)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = boolVariable;

            Database.Current.StopChange();
        }
        #endregion Constructor: BoolValueChange(BoolVariable boolVariable)

        #endregion Method Group: Constructors

    }
    #endregion Class: BoolValueChange

    #region Class: NumericalValueChange
    /// <summary>
    /// A change on a numerical value.
    /// </summary>
    public class NumericalValueChange : ValueChange
    {

        #region Properties and Fields

        #region Property: Value
        /// <summary>
        /// Gets or sets the numerical value to change to.
        /// </summary>
        public float? Value
        {
            get
            {
                return Database.Current.Select<float?>(this.ID, ValueTables.NumericalValueChange, Columns.Value);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueChange, Columns.Value, value);
                NotifyPropertyChanged("Value");
            }
        }
        #endregion Property: Value

        #region Property: Variable
        /// <summary>
        /// Gets or sets the variable to represent the value instead. Only valid for numerical and term variables!
        /// </summary>
        public Variable Variable
        {
            get
            {
                return Database.Current.Select<Variable>(this.ID, ValueTables.NumericalValueChange, Columns.Variable);
            }
            set
            {
                if (value == null || value is NumericalVariable || value is TermVariable)
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValueChange, Columns.Variable, value);
                    NotifyPropertyChanged("Variable");
                }
            }
        }
        #endregion Property: Variable

        #region Property: ValueChangeType
        /// <summary>
        /// Gets or sets the type of change for the value.
        /// </summary>
        public ValueChangeType? ValueChangeType
        {
            get
            {
                return Database.Current.Select<ValueChangeType?>(this.ID, ValueTables.NumericalValueChange, Columns.ValueChange);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueChange, Columns.ValueChange, value);
                NotifyPropertyChanged("ValueChangeType");
            }
        }
        #endregion Property: ValueChangeType

        #region Property: Prefix
        /// <summary>
        /// Gets or sets the prefix of the value.
        /// </summary>
        public Prefix Prefix
        {
            get
            {
                return Database.Current.Select<Prefix>(this.ID, ValueTables.NumericalValueChange, Columns.Prefix);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueChange, Columns.Prefix, value);
                NotifyPropertyChanged("Prefix");
            }
        }
        #endregion Property: Prefix

        #region Property: Unit
        /// <summary>
        /// Gets or sets the unit of the value.
        /// </summary>
        public Unit Unit
        {
            get
            {
                return Database.Current.Select<Unit>(this.ID, ValueTables.NumericalValueChange, Columns.Unit);
            }
            set
            {
                if (value == null || this.UnitCategory == null || this.UnitCategory.HasUnit(value))
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValueChange, Columns.Unit, value);
                    NotifyPropertyChanged("Unit");
                }
            }
        }
        #endregion Property: Unit

        #region Property: UnitCategory
        /// <summary>
        /// Gets or sets the optional unit category of this numerical value change, to which the unit should be restricted.
        /// </summary>
        public UnitCategory UnitCategory
        {
            get
            {
                return Database.Current.Select<UnitCategory>(this.ID, ValueTables.NumericalValueChange, Columns.UnitCategory);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueChange, Columns.UnitCategory, value);
                NotifyPropertyChanged("UnitCategory");

                if (value != null && !value.HasUnit(this.Unit))
                    this.Unit = null;
            }
        }
        #endregion Property: UnitCategory

        #region Property: Min
        /// <summary>
        /// Gets or sets the minimum value to change to.
        /// </summary>
        public float? Min
        {
            get
            {
                return Database.Current.Select<float?>(this.ID, ValueTables.NumericalValueChange, Columns.Min);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueChange, Columns.Min, value);
                NotifyPropertyChanged("Min");
            }
        }
        #endregion Property: Min

        #region Property: MinVariable
        /// <summary>
        /// Gets or sets the variable to represent the min value instead. Only valid for numerical and term variables!
        /// </summary>
        public Variable MinVariable
        {
            get
            {
                return Database.Current.Select<Variable>(this.ID, ValueTables.NumericalValueChange, Columns.MinVariable);
            }
            set
            {
                if (value == null || value is NumericalVariable || value is TermVariable)
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValueChange, Columns.MinVariable, value);
                    NotifyPropertyChanged("MinVariable");
                }
            }
        }
        #endregion Property: MinVariable

        #region Property: MinChange
        /// <summary>
        /// Gets or sets the type of change for the minimum value.
        /// </summary>
        public ValueChangeType? MinChange
        {
            get
            {
                return Database.Current.Select<ValueChangeType?>(this.ID, ValueTables.NumericalValueChange, Columns.MinChange);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueChange, Columns.MinChange, value);
                NotifyPropertyChanged("MinChange");
            }
        }
        #endregion Property: MinChange

        #region Property: Max
        /// <summary>
        /// Gets or sets the maximum value to change to.
        /// </summary>
        public float? Max
        {
            get
            {
                return Database.Current.Select<float?>(this.ID, ValueTables.NumericalValueChange, Columns.Max);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueChange, Columns.Max, value);
                NotifyPropertyChanged("Max");
            }
        }
        #endregion Property: Max

        #region Property: MaxVariable
        /// <summary>
        /// Gets or sets the variable to represent the max value instead. Only valid for numerical and term variables!
        /// </summary>
        public Variable MaxVariable
        {
            get
            {
                return Database.Current.Select<Variable>(this.ID, ValueTables.NumericalValueChange, Columns.MaxVariable);
            }
            set
            {
                if (value == null || value is NumericalVariable || value is TermVariable)
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValueChange, Columns.MaxVariable, value);
                    NotifyPropertyChanged("MaxVariable");
                }
            }
        }
        #endregion Property: MaxVariable

        #region Property: MaxChange
        /// <summary>
        /// Gets or sets the type of change for the maximum value.
        /// </summary>
        public ValueChangeType? MaxChange
        {
            get
            {
                return Database.Current.Select<ValueChangeType?>(this.ID, ValueTables.NumericalValueChange, Columns.MaxChange);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueChange, Columns.MaxChange, value);
                NotifyPropertyChanged("MaxChange");
            }
        }
        #endregion Property: MaxChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: NumericalValueChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static NumericalValueChange()
        {
            // Variable, unit, and unit category
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(NumericalVariable), EntryType.Nullable));
            dict.Add(Columns.Unit, new Tuple<Type, EntryType>(typeof(Unit), EntryType.Nullable));
            dict.Add(Columns.UnitCategory, new Tuple<Type, EntryType>(typeof(UnitCategory), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.NumericalValueChange, typeof(NumericalValue), dict);
        }
        #endregion Static Constructor: NumericalValueChange()

        #region Constructor: NumericalValueChange()
        /// <summary>
        /// Creates a new numerical value change.
        /// </summary>
        public NumericalValueChange()
            : base()
        {
        }
        #endregion Constructor: NumericalValueChange()

        #region Constructor: NumericalValueChange(uint id)
        /// <summary>
        /// Creates a new numerical value change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a numerical value change from.</param>
        internal NumericalValueChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: NumericalValueChange(uint id)

        #region Constructor: NumericalValueChange(NumericalValueChange numericalValueChange)
        /// <summary>
        /// Clones a numerical value change.
        /// </summary>
        /// <param name="numericalValueChange">The numerical value change to clone.</param>
        public NumericalValueChange(NumericalValueChange numericalValueChange)
            : base(numericalValueChange)
        {
            if (numericalValueChange != null)
            {
                Database.Current.StartChange();

                this.Value = numericalValueChange.Value;
                if (numericalValueChange.Variable != null)
                    this.Variable = numericalValueChange.Variable;
                this.ValueChangeType = numericalValueChange.ValueChangeType;
                this.Prefix = numericalValueChange.Prefix;
                this.Unit = numericalValueChange.Unit;
                this.UnitCategory = numericalValueChange.UnitCategory;
                this.Min = numericalValueChange.Min;
                if (numericalValueChange.MinVariable != null)
                    this.MinVariable = numericalValueChange.MinVariable;
                this.MinChange = numericalValueChange.MinChange;
                this.Max = numericalValueChange.Max;
                if (numericalValueChange.MaxVariable != null)
                    this.MaxVariable = numericalValueChange.MaxVariable;
                this.MaxChange = numericalValueChange.MaxChange;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: NumericalValueChange(NumericalValueChange numericalValueChange)

        #region Constructor: NumericalValueChange(float val, ValueChangeType valueChangeType)
        /// <summary>
        /// Creates a numerical value change for the given value.
        /// </summary>
        /// <param name="val">The value to create a numerical value change for.</param>
        /// <param name="valueChangeType">The type of the numerical value change.</param>
        public NumericalValueChange(float val, ValueChangeType valueChangeType)
            : base()
        {
            Database.Current.StartChange();

            this.Value = val;
            this.ValueChangeType = valueChangeType;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValueChange(float val, ValueChangeType valueChangeType)

        #region Constructor: NumericalValueChange(NumericalVariable numericalVariable, ValueChangeType valueChangeType)
        /// <summary>
        /// Creates a numerical value change from the given numerical variable.
        /// </summary>
        /// <param name="numericalVariable">The numerical variable to create a numerical value change from.</param>
        /// <param name="valueChangeType">The type of the numerical value change.</param>
        public NumericalValueChange(NumericalVariable numericalVariable, ValueChangeType valueChangeType)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = numericalVariable;
            this.ValueChangeType = valueChangeType;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValueChange(NumericalVariable numericalVariable, ValueChangeType valueChangeType)

        #endregion Method Group: Constructors

    }
    #endregion Class: NumericalValueChange

    #region Class: StringValueChange
    /// <summary>
    /// A change on a string value.
    /// </summary>
    public class StringValueChange : ValueChange
    {

        #region Properties and Fields

        #region Property: String
        /// <summary>
        /// Gets or sets the string value to change to.
        /// </summary>
        public string String
        {
            get
            {
                return Database.Current.Select<string>(this.ID, ValueTables.StringValueChange, Columns.String);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.StringValueChange, Columns.String, value);
                NotifyPropertyChanged("String");
            }
        }
        #endregion Property: String

        #region Property: Variable
        /// <summary>
        /// Gets or sets the string variable to represent the string instead.
        /// </summary>
        public StringVariable Variable
        {
            get
            {
                return Database.Current.Select<StringVariable>(this.ID, ValueTables.StringValueChange, Columns.Variable);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.StringValueChange, Columns.Variable, value);
                NotifyPropertyChanged("Variable");
            }
        }
        #endregion Property: Variable

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: StringValueChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static StringValueChange()
        {
            // Variable
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(StringVariable), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.StringValueChange, typeof(StringValueChange), dict);
        }
        #endregion Static Constructor: StringValueChange()

        #region Constructor: StringValueChange()
        /// <summary>
        /// Creates a new string value change.
        /// </summary>
        public StringValueChange()
            : base()
        {
        }
        #endregion Constructor: StringValueChange()

        #region Constructor: StringValueChange(uint id)
        /// <summary>
        /// Creates a new string value change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a string value change from.</param>
        internal StringValueChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: StringValueChange(uint id)

        #region Constructor: StringValueChange(StringValueChange stringValueChange)
        /// <summary>
        /// Clones a string value change.
        /// </summary>
        /// <param name="stringValueChange">The string value change to clone.</param>
        public StringValueChange(StringValueChange stringValueChange)
            : base(stringValueChange)
        {
            if (stringValueChange != null)
            {
                Database.Current.StartChange();

                this.String = stringValueChange.String;
                if (stringValueChange.Variable != null)
                    this.Variable = stringValueChange.Variable;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: StringValueChange(StringValueChange stringValueChange)

        #region Constructor: StringValueChange(string str)
        /// <summary>
        /// Creates a string value change for the given string.
        /// </summary>
        /// <param name="str">The string to create a string value change for.</param>
        public StringValueChange(string str)
            : base()
        {
            Database.Current.StartChange();

            this.String = str;

            Database.Current.StopChange();
        }
        #endregion Constructor: StringValueChange(string str)

        #region Constructor: StringValueChange(StringVariable stringVariable)
        /// <summary>
        /// Creates a string value change from the given string variable.
        /// </summary>
        /// <param name="stringVariable">The string variable to create a string value change from.</param>
        public StringValueChange(StringVariable stringVariable)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = stringVariable;

            Database.Current.StopChange();
        }
        #endregion Constructor: StringValueChange(StringVariable stringVariable)

        #endregion Method Group: Constructors

    }
    #endregion Class: StringValueChange

    #region Class: VectorValueChange
    /// <summary>
    /// A change on a vector value.
    /// </summary>
    public class VectorValueChange : ValueChange
    {

        #region Properties and Fields

        #region Property: Vector
        /// <summary>
        /// A handler for a change in the vector.
        /// </summary>
        private Vec4.Vec4Handler vectorChanged;

        /// <summary>
        /// The four-dimensional vector to change to.
        /// </summary>
        private Vec4 vector = null;

        /// <summary>
        /// Gets or sets the four-dimensional vector to change to.
        /// </summary>
        public Vec4 Vector
        {
            get
            {
                if (vector == null)
                {
                    vector = Database.Current.Select<Vec4>(this.ID, ValueTables.VectorValueChange, Columns.Vector);

                    if (vector != null)
                    {
                        if (vectorChanged == null)
                            vectorChanged = new Vec4.Vec4Handler(value_ValueChanged);

                        vector.ValueChanged += vectorChanged;
                    }
                }

                return vector;
            }
            set
            {
                if (vectorChanged == null)
                    vectorChanged = new Vec4.Vec4Handler(value_ValueChanged);

                if (vector != null)
                    vector.ValueChanged -= vectorChanged;

                vector = value;
                Database.Current.Update(this.ID, ValueTables.VectorValueChange, Columns.Vector, vector);
                NotifyPropertyChanged("Vector");

                if (vector != null)
                    vector.ValueChanged += vectorChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the vector changes.
        /// </summary>
        private void value_ValueChanged(Vec4 vec)
        {
            Database.Current.Update(this.ID, ValueTables.VectorValueChange, Columns.Vector, vector);
            NotifyPropertyChanged("Vector");
        }
        #endregion Property: Vector

        #region Property: Variable
        /// <summary>
        /// Gets or sets the vector variable to represent the vector instead.
        /// </summary>
        public VectorVariable Variable
        {
            get
            {
                return Database.Current.Select<VectorVariable>(this.ID, ValueTables.VectorValueChange, Columns.Variable);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.VectorValueChange, Columns.Variable, value);
                NotifyPropertyChanged("Variable");
            }
        }
        #endregion Property: Variable

        #region Property: VectorChange
        /// <summary>
        /// Gets or sets the type of change for the vector value.
        /// </summary>
        public ValueChangeType? VectorChange
        {
            get
            {
                return Database.Current.Select<ValueChangeType?>(this.ID, ValueTables.VectorValueChange, Columns.VectorChange);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.VectorValueChange, Columns.VectorChange, value);
                NotifyPropertyChanged("VectorChange");
            }
        }
        #endregion Property: VectorChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: VectorValueChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static VectorValueChange()
        {
            // Variable
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(VectorVariable), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.VectorValueChange, typeof(VectorValueChange), dict);
        }
        #endregion Static Constructor: VectorValueChange()

        #region Constructor: VectorValueChange()
        /// <summary>
        /// Creates a new vector value change.
        /// </summary>
        public VectorValueChange()
            : base()
        {
        }
        #endregion Constructor: VectorValueChange()

        #region Constructor: VectorValueChange(uint id)
        /// <summary>
        /// Creates a new vector value change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a vector value change from.</param>
        internal VectorValueChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: VectorValueChange(uint id)

        #region Constructor: VectorValueChange(VectorValueChange vectorValueChange)
        /// <summary>
        /// Clones a vector value change.
        /// </summary>
        /// <param name="vectorValueChange">The vector value change to clone.</param>
        public VectorValueChange(VectorValueChange vectorValueChange)
            : base(vectorValueChange)
        {
            if (vectorValueChange != null)
            {
                Database.Current.StartChange();

                if (vectorValueChange.Vector != null)
                    this.Vector = new Vec4(vectorValueChange.Vector);
                if (vectorValueChange.Variable != null)
                    this.Variable = vectorValueChange.Variable;
                this.VectorChange = vectorValueChange.VectorChange;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: VectorValueChange(VectorValueChange vectorValueChange)

        #region Constructor: VectorValueChange(Vec4 vector, ValueChangeType vectorChange)
        /// <summary>
        /// Creates a vector value change for the given vector.
        /// </summary>
        /// <param name="vector">The vector to create a vector value change for.</param>
        /// <param name="vectorChange">The type of the vector change.</param>
        public VectorValueChange(Vec4 vector, ValueChangeType vectorChange)
            : base()
        {
            Database.Current.StartChange();

            this.Vector = vector;
            this.VectorChange = vectorChange;

            Database.Current.StopChange();
        }
        #endregion Constructor: VectorValueChange(Vec4 vector, ValueChangeType vectorChange)

        #region Constructor: VectorValueChange(VectorVariable vectorVariable, ValueChangeType vectorChange)
        /// <summary>
        /// Creates a vector value change from the given vector variable.
        /// </summary>
        /// <param name="vectorVariable">The vector variable to create a vector value change from.</param>
        /// <param name="vectorChange">The type of the vector change.</param>
        public VectorValueChange(VectorVariable vectorVariable, ValueChangeType vectorChange)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = vectorVariable;
            this.VectorChange = vectorChange;

            Database.Current.StopChange();
        }
        #endregion Constructor: VectorValueChange(VectorVariable vectorVariable, ValueChangeType vectorChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: VectorValueChange

    #region Class: NumericalValueRange
    /// <summary>
    /// A range on a numerical value.
    /// </summary>
    public class NumericalValueRange : IdHolder
    {

        #region Properties and Fields

        #region Property: ValueSign
        /// <summary>
        /// Gets or sets the sign for the value in the range.
        /// </summary>
        public EqualitySignExtendedDual ValueSign
        {
            get
            {
                return Database.Current.Select<EqualitySignExtendedDual>(this.ID, ValueTables.NumericalValueRange, Columns.ValueSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueRange, Columns.ValueSign, value);
                NotifyPropertyChanged("ValueSign");
            }
        }
        #endregion Property: ValueSign

        #region Property: Value
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public float Value
        {
            get
            {
                return Database.Current.Select<float>(this.ID, ValueTables.NumericalValueRange, Columns.Value);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueRange, Columns.Value, value);
                NotifyPropertyChanged("Value");
            }
        }
        #endregion Property: Value

        #region Property: Value2
        /// <summary>
        /// Gets or sets the second value. Only used for the 'Between' sign!
        /// </summary>
        public float Value2
        {
            get
            {
                return Database.Current.Select<float>(this.ID, ValueTables.NumericalValueRange, Columns.Value2);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueRange, Columns.Value2, value);
                NotifyPropertyChanged("Value2");
            }
        }
        #endregion Property: Value2

        #region Property: Variable
        /// <summary>
        /// Gets or sets the variable to represent the value instead. Only valid for numerical and term variables!
        /// </summary>
        public Variable Variable
        {
            get
            {
                return Database.Current.Select<Variable>(this.ID, ValueTables.NumericalValueRange, Columns.Variable);
            }
            set
            {
                if (value is NumericalVariable || value is TermVariable)
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValueRange, Columns.Variable, value);
                    NotifyPropertyChanged("Variable");
                }
            }
        }
        #endregion Property: Variable2

        #region Property: Variable2
        /// <summary>
        /// Gets or sets the variable to represent the second value instead. Only valid for numerical and term variables!
        /// </summary>
        public Variable Variable2
        {
            get
            {
                return Database.Current.Select<Variable>(this.ID, ValueTables.NumericalValueRange, Columns.Variable2);
            }
            set
            {
                if (value is NumericalVariable || value is TermVariable)
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValueRange, Columns.Variable2, value);
                    NotifyPropertyChanged("Variable2");
                }
            }
        }
        #endregion Property: Variable2

        #region Property: Prefix
        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        public Prefix Prefix
        {
            get
            {
                return Database.Current.Select<Prefix>(this.ID, ValueTables.NumericalValueRange, Columns.Prefix);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueRange, Columns.Prefix, value);
                NotifyPropertyChanged("Prefix");
            }
        }
        #endregion Property: Prefix

        #region Property: Unit
        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        public Unit Unit
        {
            get
            {
                return Database.Current.Select<Unit>(this.ID, ValueTables.NumericalValueRange, Columns.Unit);
            }
            set
            {
                if (value == null || this.UnitCategory == null || this.UnitCategory.HasUnit(value))
                {
                    Database.Current.Update(this.ID, ValueTables.NumericalValueRange, Columns.Unit, value);
                    NotifyPropertyChanged("Unit");
                }
            }
        }
        #endregion Property: Unit

        #region Property: UnitCategory
        /// <summary>
        /// Gets or sets the optional unit category of this numerical value range, to which the unit should be restricted.
        /// </summary>
        public UnitCategory UnitCategory
        {
            get
            {
                return Database.Current.Select<UnitCategory>(this.ID, ValueTables.NumericalValueRange, Columns.UnitCategory);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalValueRange, Columns.UnitCategory, value);
                NotifyPropertyChanged("UnitCategory");

                if (value != null && !value.HasUnit(this.Unit))
                    this.Unit = null;
            }
        }
        #endregion Property: UnitCategory

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: NumericalValueRange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static NumericalValueRange()
        {
            // Variables, unit, and unit category
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(NumericalVariable), EntryType.Nullable));
            dict.Add(Columns.Variable2, new Tuple<Type, EntryType>(typeof(NumericalVariable), EntryType.Nullable));
            dict.Add(Columns.Unit, new Tuple<Type, EntryType>(typeof(Unit), EntryType.Nullable));
            dict.Add(Columns.UnitCategory, new Tuple<Type, EntryType>(typeof(UnitCategory), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.NumericalValueRange, typeof(NumericalValueRange), dict);
        }
        #endregion Static Constructor: NumericalValueRange()

        #region Constructor: NumericalValueRange()
        /// <summary>
        /// Creates a new value range.
        /// </summary>
        public NumericalValueRange()
            : this(SemanticsSettings.Values.Value, EqualitySignExtendedDual.Equal)
        {
        }
        #endregion Constructor: NumericalValueRange()

        #region Constructor: NumericalValueRange(uint id)
        /// <summary>
        /// Creates a new value range from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a value range from.</param>
        internal NumericalValueRange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: NumericalValueRange(uint id)

        #region Constructor: NumericalValueRange(NumericalValueRange numericalValueRange)
        /// <summary>
        /// Clones a numerical value range.
        /// </summary>
        /// <param name="numericalValueRange">The numerical value range to clone.</param>
        public NumericalValueRange(NumericalValueRange numericalValueRange)
            : base()
        {
            if (numericalValueRange != null)
            {
                Database.Current.StartChange();

                this.ValueSign = numericalValueRange.ValueSign;
                this.Value = numericalValueRange.Value;
                this.Value2 = numericalValueRange.Value2;
                if (numericalValueRange.Variable != null)
                    this.Variable = numericalValueRange.Variable;
                if (numericalValueRange.Variable2 != null)
                    this.Variable2 = numericalValueRange.Variable2;
                this.Prefix = numericalValueRange.Prefix;
                this.Unit = numericalValueRange.Unit;
                this.UnitCategory = numericalValueRange.UnitCategory;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: NumericalValueRange(NumericalValueRange numericalValueRange)

        #region Constructor: NumericalValueRange(float value)
        /// <summary>
        /// Creates a new numerical value range with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public NumericalValueRange(float value)
            : this(value, Prefix.None, null, EqualitySignExtendedDual.Equal)
        {
        }

        /// <summary>
        /// Creates a numerical value range from the given float.
        /// </summary>
        /// <param name="val">The float to create a numerical value range from.</param>
        /// <returns>A numerical value range from the given float.</returns>
        public static implicit operator NumericalValueRange(float val)
        {
            return new NumericalValueRange(val);
        }
        #endregion Constructor: NumericalValueRange(float value)

        #region Constructor: NumericalValueRange(float value, float value2)
        /// <summary>
        /// Creates a new numerical value range between the given values.
        /// </summary>
        /// <param name="value">The first value.</param>
        /// <param name="value">The second value.</param>
        public NumericalValueRange(float value, float value2)
            : this(value, Prefix.None, null, EqualitySignExtendedDual.Between)
        {
            Database.Current.StartChange();

            this.Value2 = value2;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValueRange(float value, float value2)

        #region Constructor: NumericalValueRange(float value, Unit unit)
        /// <summary>
        /// Creates a new numerical value range with the given value and unit.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="unit">The unit for the value.</param>
        public NumericalValueRange(float value, Unit unit)
            : this(value, Prefix.None, unit, EqualitySignExtendedDual.Equal)
        {
        }
        #endregion Constructor: NumericalValueRange(float value, Unit unit)

        #region Constructor: NumericalValueRange(float value, UnitCategory unitCategory)
        /// <summary>
        /// Creates a new numerical value range with the given value and unit category.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="unitCategory">The unit category for the value.</param>
        public NumericalValueRange(float value, UnitCategory unitCategory)
            : this(value, Prefix.None, null, EqualitySignExtendedDual.Equal)
        {
            Database.Current.StartChange();

            this.UnitCategory = unitCategory;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValueRange(float value, UnitCategory unitCategory)

        #region Constructor: NumericalValueRange(float value, float value2, Unit unit)
        /// <summary>
        /// Creates a new numerical value range with the given value and unit.
        /// </summary>
        /// <param name="value">The first value.</param>
        /// <param name="value">The second value.</param>
        /// <param name="unit">The unit for the value.</param>
        public NumericalValueRange(float value, float value2, Unit unit)
            : this(value, Prefix.None, unit, EqualitySignExtendedDual.Between)
        {
            Database.Current.StartChange();

            this.Value2 = value2;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValueRange(float value, float value2, Unit unit)

        #region Constructor: NumericalValueRange(float value, Prefix prefix, Unit unit)
        /// <summary>
        /// Creates a new numerical value range with the given value, prefix, and unit.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="unit">The unit for the value.</param>
        public NumericalValueRange(float value, Prefix prefix, Unit unit)
            : this(value, prefix, unit, EqualitySignExtendedDual.Equal)
        {
        }
        #endregion Constructor: NumericalValueRange(float value, Prefix prefix, Unit unit)

        #region Constructor: NumericalValueRange(float value, float value2, Prefix prefix, Unit unit)
        /// <summary>
        /// Creates a new numerical value range with the given value, prefix, and unit.
        /// </summary>
        /// <param name="value">The first value.</param>
        /// <param name="value">The second value.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="unit">The unit for the value.</param>
        public NumericalValueRange(float value, float value2, Prefix prefix, Unit unit)
            : this(value, prefix, unit, EqualitySignExtendedDual.Between)
        {
            Database.Current.StartChange();

            this.Value2 = value2;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValueRange(float value, float value2, Prefix prefix, Unit unit)

        #region Constructor: NumericalValueRange(float value, EqualitySignExtendedDual valueSign)
        /// <summary>
        /// Creates a new numerical value range with the given value and sign.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueSign">The sign of the value.</param>
        public NumericalValueRange(float value, EqualitySignExtendedDual valueSign)
            : this(value, Prefix.None, null, valueSign)
        {
        }
        #endregion Constructor: NumericalValueRange(float value, EqualitySignExtendedDual valueSign)

        #region Constructor: NumericalValueRange(float value, EqualitySignExtendedDual valueSign, UnitCategory unitCategory)
        /// <summary>
        /// Creates a new numerical value range with the given value and sign.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueSign">The sign of the value.</param>
        /// <param name="unitCategory">The unit category of the value.</param>
        public NumericalValueRange(float value, EqualitySignExtendedDual valueSign, UnitCategory unitCategory)
            : this(value, Prefix.None, null, valueSign)
        {
            Database.Current.StartChange();

            this.UnitCategory = unitCategory;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValueRange(float value, EqualitySignExtendedDual valueSign, UnitCategory unitCategory)

        #region Constructor: NumericalValueRange(float value, Unit unit, EqualitySignExtendedDual valueSign)
        /// <summary>
        /// Creates a new numerical value range with the given value, unit, and sign.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="unit">The unit for the value.</param>
        /// <param name="distanceSign">The sign of the value.</param>
        public NumericalValueRange(float value, Unit unit, EqualitySignExtendedDual valueSign)
            : this(value, Prefix.None, unit, valueSign)
        {
        }
        #endregion Constructor: NumericalValueRange(float value, Unit unit, EqualitySignExtendedDual valueSign)

        #region Constructor: NumericalValueRange(float value, Prefix prefix, Unit unit, EqualitySignExtendedDual valueSign)
        /// <summary>
        /// Creates a new numerical value range with the given value, prefix, unit, and sign.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="unit">The unit for the value.</param>
        /// <param name="distanceSign">The sign of the value.</param>
        public NumericalValueRange(float value, Prefix prefix, Unit unit, EqualitySignExtendedDual valueSign)
            : base()
        {
            Database.Current.StartChange();
            Database.Current.QueryBegin();

            this.Value = value;
            this.Prefix = prefix;
            this.Unit = unit;
            this.ValueSign = valueSign;

            Database.Current.QueryCommit();
            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValueRange(float value, Prefix prefix, Unit unit, EqualitySignExtendedDual valueSign)

        #region Constructor: NumericalValueRange(NumericalVariable numericalVariable)
        /// <summary>
        /// Creates a numerical value range from the given numerical variable.
        /// </summary>
        /// <param name="numericalVariable">The numerical variable to create a numerical value range from.</param>
        public NumericalValueRange(NumericalVariable numericalVariable)
            : base()
        {
            Database.Current.StartChange();

            this.Variable = numericalVariable;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalValueRange(NumericalVariable numericalVariable)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            string str = string.Empty;

            EqualitySignExtendedDual sign = this.ValueSign;
            if (sign != EqualitySignExtendedDual.Equal && sign != EqualitySignExtendedDual.Between)
                str += sign.ToString() + " ";

            str += this.Value.ToString(CommonSettings.Culture);

            if (sign == EqualitySignExtendedDual.Between)
                str += "..." + this.Value2.ToString(CommonSettings.Culture);

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
    #endregion Class: NumericalValueRange

}