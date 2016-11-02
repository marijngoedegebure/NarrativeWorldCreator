/**************************************************************************
 * 
 * ValueBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Common;
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: ValueBase
    /// <summary>
    /// A base of a value.
    /// </summary>
    public abstract class ValueBase : Base
    {

        #region Properties and Fields

        #region Property: IsRandom
        /// <summary>
        /// The value that indicates whether the value should be random.
        /// </summary>
        private bool isRandom = SemanticsSettings.Values.IsRandom;
        
        /// <summary>
        /// Gets the value that indicates whether the value should be random.
        /// </summary>
        public bool IsRandom
        {
            get
            {
                return isRandom;
            }
        }
        #endregion Property: IsRandom

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ValueBase(Value value)
        /// <summary>
        /// Creates a value base from the given value.
        /// </summary>
        /// <param name="value">The value to create a base for.</param>
        protected ValueBase(Value value)
            : base(value)
        {
            if (value != null)
                this.isRandom = value.IsRandom;
        }
        #endregion Constructor: ValueBase(Value value)

        #endregion Method Group: Constructors

    }
    #endregion Class: ValueBase

    #region Class: BoolValueBase
    /// <summary>
    /// A value that consists of a bool.
    /// </summary>
    public sealed class BoolValueBase : ValueBase
    {

        #region Properties and Fields

        #region Property: Bool
        /// <summary>
        /// The boolean value.
        /// </summary>
        private bool boolean = SemanticsSettings.Values.Boolean;

        /// <summary>
        /// Gets the boolean value.
        /// </summary>
        public bool Bool
        {
            get
            {
                return boolean;
            }
        }
        #endregion Property: Bool

        #region Property: Variable
        /// <summary>
        /// The bool variable to represent the bool instead.
        /// </summary>
        private BoolVariableBase variable = null;
        
        /// <summary>
        /// Gets the bool variable to represent the bool instead.
        /// </summary>
        public BoolVariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: BoolValueBase(BoolValue boolValue)
        /// <summary>
        /// Creates a base of the given bool value.
        /// </summary>
        /// <param name="boolValue">The bool value to create a base of.</param>
        internal BoolValueBase(BoolValue boolValue)
            : base(boolValue)
        {
            if (boolValue != null)
            {
                this.boolean = boolValue.Bool;
                this.variable = BaseManager.Current.GetBase<BoolVariableBase>(boolValue.Variable);
            }
        }
        #endregion Constructor: BoolValueBase(BoolValue boolValue)

        #region Constructor: BoolValueBase(bool boolean)
        /// <summary>
        /// Creates a base of the given boolean.
        /// </summary>
        /// <param name="boolean">The boolean to create a base of.</param>
        public BoolValueBase(bool boolean)
            : base(null)
        {
            this.boolean = boolean;
        }
        #endregion Constructor: BoolValueBase(bool boolean)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the value of the bool value base for the given variable holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The value of the bool value base.</returns>
        internal bool GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable != null)
                return new BoolVariableInstance(this.Variable, iVariableInstanceHolder).Bool;
            else
                return this.Bool;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

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
    #endregion Class: BoolValueBase

    #region Class: NumericalValueBase
    /// <summary>
    /// A numerical value, optionally accompanied by a prefix and a unit.
    /// </summary>
    public sealed class NumericalValueBase : ValueBase
    {

        #region Properties and Fields

        #region Property: Value
        /// <summary>
        /// The numerical value.
        /// </summary>
        private float val = SemanticsSettings.Values.Value;

        /// <summary>
        /// Gets the numerical value.
        /// </summary>
        public float Value
        {
            get
            {
                return val;
            }
            internal set
            {
                val = value;
            }
        }
        #endregion Property: Value

        #region Property: Variable
        /// <summary>
        /// The variable to represent the value instead. Only valid for numerical and term variables!
        /// </summary>
        private VariableBase variable = null;

        /// <summary>
        /// Gets the variable to represent the value instead. Only valid for numerical and term variables!
        /// </summary>
        public VariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #region Property: Prefix
        /// <summary>
        /// The prefix of the value.
        /// </summary>
        private Prefix prefix = default(Prefix);

        /// <summary>
        /// Gets the prefix of the value.
        /// </summary>
        public Prefix Prefix
        {
            get
            {
                return prefix;
            }
        }
        #endregion Property: Prefix

        #region Property: Unit
        /// <summary>
        /// The optional unit of this value.
        /// </summary>
        private UnitBase unit = null;

        /// <summary>
        /// Gets the optional unit of this value.
        /// </summary>
        public UnitBase Unit
        {
            get
            {
                return unit;
            }
            internal set
            {
                unit = value;
            }
        }
        #endregion Property: Unit

        #region Property: Min
        /// <summary>
        /// The minimum value.
        /// </summary>
        private float min = SemanticsSettings.Values.MinValue;

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        public float Min
        {
            get
            {
                return min;
            }
        }
        #endregion Property: Min

        #region Property: Max
        /// <summary>
        /// The maximum value.
        /// </summary>
        private float max = SemanticsSettings.Values.MaxValue;

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        public float Max
        {
            get
            {
                return max;
            }
        }
        #endregion Property: Max

        #region Property: RandomMin
        /// <summary>
        /// The minimum value of the random. Only valid when IsRandom has been set to true.
        /// If not set, the regular Min will be used when the random is applied.
        /// </summary>
        private float? randomMin = null;

        /// <summary>
        /// Gets the minimum value of the random. Only valid when IsRandom has been set to true.
        /// If not set, the regular Min will be used when the random is applied.
        /// </summary>
        public float? RandomMin
        {
            get
            {
                return randomMin;
            }
        }
        #endregion Property: RandomMin

        #region Property: RandomMax
        /// <summary>
        /// The maximum value of the random. Only valid when IsRandom has been set to true.
        /// If not set, the regular Max will be used when the random is applied.
        /// </summary>
        private float? randomMax = null;

        /// <summary>
        /// Gets the maximum value of the random. Only valid when IsRandom has been set to true.
        /// If not set, the regular Max will be used when the random is applied.
        /// </summary>
        public float? RandomMax
        {
            get
            {
                return randomMax;
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
                {
                    // Calculate the base value (without prefix and converted to the base unit)
                    baseValue = this.Value * Toolbox.PrefixToMultiplier(this.Prefix);
                    if (this.Unit != null && this.Unit.ConversionBackEquation != null && this.Unit.ConversionBackEquation.IsValid)
                        baseValue = this.Unit.ConversionBackEquation.Calculate(SemanticsSettings.General.EquationResultVariable, (float)baseValue);
                }
                return (float)baseValue;
            }
        }
        #endregion: Property: BaseValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NumericalValueBase(NumericalValue numericalValue)
        /// <summary>
        /// Creates a base of the given numerical value.
        /// </summary>
        /// <param name="numericalValue">The numerical value to create a base of.</param>
        internal NumericalValueBase(NumericalValue numericalValue)
            : base(numericalValue)
        {
            this.min = numericalValue.Min;
            this.max = numericalValue.Max;
            this.prefix = numericalValue.Prefix;
            this.unit = BaseManager.Current.GetBase<UnitBase>(numericalValue.Unit);
            this.val = numericalValue.Value;
            this.variable = BaseManager.Current.GetBase<VariableBase>(numericalValue.Variable);
            this.randomMin = numericalValue.RandomMin;
            this.randomMax = numericalValue.RandomMax;
        }
        #endregion Constructor: NumericalValueBase(NumericalValue numericalValue)

        #region Constructor: NumericalValueBase(float val)
        /// <summary>
        /// Creates a base of the given value.
        /// </summary>
        /// <param name="val">The value to create a base of.</param>
        public NumericalValueBase(float val)
            : base(null)
        {
            this.val = val;
        }
        #endregion Constructor: NumericalValueBase(float val)

        #region Constructor: NumericalValueBase(float val, UnitBase unitBase)
        /// <summary>
        /// Creates a base of the given value and unit.
        /// </summary>
        /// <param name="val">The value to create a base of.</param>
        /// <param name="unitBase">The unit.</param>
        public NumericalValueBase(float val, UnitBase unitBase)
            : base(null)
        {
            this.val = val;
            this.unit = unitBase;
        }
        #endregion Constructor: NumericalValueBase(float val, UnitBase unitBase)

        #region Constructor: NumericalValueBase(float val, Prefix prefix, UnitBase unitBase, float min, float max)
        /// <summary>
        /// Creates a base of the given value, unit, and min and max values.
        /// </summary>
        /// <param name="val">The value to create a base of.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="unitBase">The unit.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public NumericalValueBase(float val, Prefix prefix, UnitBase unitBase, float min, float max)
            : base(null)
        {
            this.val = val;
            this.prefix = prefix;
            this.unit = unitBase;
            this.min = min;
            this.max = max;
        }
        #endregion Constructor: NumericalValueBase(float val, Prefix prefix, UnitBase unitBase, float min, float max)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the value of the numerical value base for the given variable holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The value of the numerical value base.</returns>
        internal float GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable is NumericalVariableBase)
                return new NumericalVariableInstance((NumericalVariableBase)this.Variable, iVariableInstanceHolder).Value;
            else if (this.Variable is TermVariableBase)
                return new TermVariableInstance((TermVariableBase)this.Variable, iVariableInstanceHolder).Value;
            else
                return this.BaseValue;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

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
    #endregion Class: NumericalValueBase

    #region Class: StringValueBase
    /// <summary>
    /// A value that consists of a string.
    /// </summary>
    public sealed class StringValueBase : ValueBase
    {

        #region Properties and Fields

        #region Property: String
        /// <summary>
        /// The string value.
        /// </summary>
        private string str = SemanticsSettings.Values.String;

        /// <summary>
        /// Gets the string value.
        /// </summary>
        public string String
        {
            get
            {
                return str;
            }
        }
        #endregion Property: String

        #region Property: Variable
        /// <summary>
        /// The string variable to represent the string instead.
        /// </summary>
        private StringVariableBase variable = null;

        /// <summary>
        /// Gets the string variable to represent the string instead.
        /// </summary>
        public StringVariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #region Property: MaxLength
        /// <summary>
        /// The maximum length of the string.
        /// </summary>
        private uint maxLength = SemanticsSettings.Values.MaxLength;
        
        /// <summary>
        /// Gets the maximum length of the string.
        /// </summary>
        public uint MaxLength
        {
            get
            {
                return maxLength;
            }
        }
        #endregion Property: MaxLength

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StringValueBase(StringValue stringValue)
        /// <summary>
        /// Creates a base of the given string value.
        /// </summary>
        /// <param name="stringValue">The string value to create a base of.</param>
        internal StringValueBase(StringValue stringValue)
            : base(stringValue)
        {
            if (stringValue != null)
            {
                this.str = stringValue.String;
                this.variable = BaseManager.Current.GetBase<StringVariableBase>(stringValue.Variable);
                this.maxLength = stringValue.MaxLength;
            }
        }
        #endregion Constructor: StringValueBase(StringValue stringValue)

        #region Constructor: StringValueBase(string str)
        /// <summary>
        /// Creates a base of the given string.
        /// </summary>
        /// <param name="str">The string to create a base of.</param>
        public StringValueBase(string str)
            : base(null)
        {
            this.str = str;
        }
        #endregion Constructor: StringValueBase(string str)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the value of the string value base for the given variable holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The value of the string value base.</returns>
        internal string GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable != null)
                return new StringVariableInstance(this.Variable, iVariableInstanceHolder).String;
            else
                return this.String;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

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
    #endregion Class: StringValueBase

    #region Class: VectorValueBase
    /// <summary>
    /// A value that consists of a four-dimensional vector.
    /// </summary>
    public sealed class VectorValueBase : ValueBase
    {

        #region Properties and Fields

        #region Property: Vector
        /// <summary>
        /// The four-dimensional vector.
        /// </summary>
        private Vec4 vector = new Vec4(SemanticsSettings.Values.Vector4X, SemanticsSettings.Values.Vector4Y, SemanticsSettings.Values.Vector4Z, SemanticsSettings.Values.Vector4W);
        
        /// <summary>
        /// Gets the four-dimensional vector.
        /// </summary>
        public Vec4 Vector
        {
            get
            {
                return vector;
            }
        }
        #endregion Property: Vector

        #region Property: Variable
        /// <summary>
        /// The vector variable to represent the vector instead.
        /// </summary>
        private VectorVariableBase variable = null;

        /// <summary>
        /// Gets the vector variable to represent the vector instead.
        /// </summary>
        public VectorVariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #region Property: Min
        /// <summary>
        /// The minimum value.
        /// </summary>
        private Vec4 min = new Vec4(SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MinValue, SemanticsSettings.Values.MinValue);

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        public Vec4 Min
        {
            get
            {
                return min;
            }
        }
        #endregion Property: Min

        #region Property: Max
        /// <summary>
        /// The maximum value.
        /// </summary>
        private Vec4 max = new Vec4(SemanticsSettings.Values.MaxValue, SemanticsSettings.Values.MaxValue, SemanticsSettings.Values.MaxValue, SemanticsSettings.Values.MaxValue);

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        public Vec4 Max
        {
            get
            {
                return max;
            }
        }
        #endregion Property: Max

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: VectorValueBase(VectorValue vectorValue)
        /// <summary>
        /// Creates a base of the given vector value.
        /// </summary>
        /// <param name="vectorValue">The vector value to create a base of.</param>
        internal VectorValueBase(VectorValue vectorValue)
            : base(vectorValue)
        {
            if (vectorValue != null)
            {
                if (vectorValue.Min != null)
                    this.min = new Vec4(vectorValue.Min);
                if (vectorValue.Max != null)
                    this.max = new Vec4(vectorValue.Max);
                if (vectorValue.Vector != null)
                    this.vector = new Vec4(vectorValue.Vector);
                this.variable = BaseManager.Current.GetBase<VectorVariableBase>(vectorValue.Variable);
            }
        }
        #endregion Constructor: VectorValueBase(VectorValue vectorValue)

        #region Constructor: VectorValueBase(Vec4 vector)
        /// <summary>
        /// Creates a base of the given vector.
        /// </summary>
        /// <param name="vector">The vector to create a base of.</param>
        public VectorValueBase(Vec4 vector)
            : base(null)
        {
            if (vector != null)
                this.vector = new Vec4(vector);
        }
        #endregion Constructor: VectorValueBase(Vec4 vector)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the value of the vector value base for the given variable holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The value of the vector value base.</returns>
        internal Vec4 GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable != null)
            {
                Vec4 returnVec = new VectorVariableInstance(this.Variable, iVariableInstanceHolder).Vector;
                if (returnVec != null)
                    return returnVec;
            }
            
            return this.Vector;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

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
    #endregion Class: VectorValueBase

    #region Class: TermValueBase
    /// <summary>
    /// A value that consists of a term.
    /// </summary>
    public sealed class TermValueBase : ValueBase
    {

        #region Properties and Fields

        #region Property: Function1
        /// <summary>
        /// The (optional) function on the first value.
        /// </summary>
        private Function? function1 = null;
        
        /// <summary>
        /// Gets the (optional) function on the first value.
        /// </summary>
        public Function? Function1
        {
            get
            {
                return function1;
            }
        }
        #endregion Property: Function1

        #region Property: Value1
        /// <summary>
        /// The (required) first value. Only valid for NumericalValueBase and TermValueBase!
        /// </summary>
        private ValueBase value1 = null;
        
        /// <summary>
        /// Gets the (required) first value. Only valid for NumericalValueBase and TermValueBase!
        /// </summary>
        public ValueBase Value1
        {
            get
            {
                return value1;
            }
        }
        #endregion Property: Value1

        #region Property: Operator
        /// <summary>
        /// The (optional) operator between both values.
        /// </summary>
        private Operator? op = null;
        
        /// <summary>
        /// Gets the (optional) operator between both values.
        /// </summary>
        public Operator? Operator
        {
            get
            {
                return op;
            }
        }
        #endregion Property: Operator

        #region Property: Function2
        /// <summary>
        /// The (optional) function on the second value.
        /// </summary>
        private Function? function2 = null;
        
        /// <summary>
        /// Gets the (optional) function on the second value.
        /// </summary>
        public Function? Function2
        {
            get
            {
                return function2;
            }
        }
        #endregion Property: Function2

        #region Property: Value2
        /// <summary>
        /// The (optional) second value. Only valid for NumericalValueBase and TermValueBase!
        /// </summary>
        private ValueBase value2 = null;
        
        /// <summary>
        /// Gets the (optional) second value. Only valid for NumericalValueBase and TermValueBase!
        /// </summary>
        public ValueBase Value2
        {
            get
            {
                return value2;
            }
        }
        #endregion Property: Value2

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TermValueBase(TermValue termValue)
        /// <summary>
        /// Creates a new term value base for the given term value.
        /// </summary>
        /// <param name="termValue">The term value to create a base for.</param>
        internal TermValueBase(TermValue termValue)
            : base(termValue)
        {
            if (termValue != null)
            {
                this.function1 = termValue.Function1;
                this.value1 = BaseManager.Current.GetBase<ValueBase>(termValue.Value1);
                this.op = termValue.Operator;
                this.function2 = termValue.Function2;
                this.value2 = BaseManager.Current.GetBase<ValueBase>(termValue.Value2);
            }
        }
        #endregion Constructor: TermValueBase(TermValue termValue)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the value of the term value base for the given variable holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The value of the numerical value base.</returns>
        internal float GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Value1 != null)
            {
                // Calculate the first value
                float val = 0;
                if (this.Value1 is NumericalValueBase)
                    val = new NumericalValueInstance((NumericalValueBase)this.Value1, iVariableInstanceHolder).Value;
                else if (this.Value1 is TermValueBase)
                    val = ((TermValueBase)this.Value1).GetValue(iVariableInstanceHolder);
                if (this.Function1 != null)
                    val = Toolbox.Calculate((Function)this.Function1, val);

                // Possibly combine it with the second calculated value
                if (this.Operator != null && this.Value2 != null)
                {
                    float val2 = 0;
                    if (this.Value2 is NumericalValueBase)
                        val2 = new NumericalValueInstance((NumericalValueBase)this.Value2, iVariableInstanceHolder).Value;
                    else if (this.Value2 is TermValueBase)
                        val2 = ((TermValueBase)this.Value2).GetValue(iVariableInstanceHolder);

                    if (this.Function2 != null)
                        val2 = Toolbox.Calculate((Function)this.Function2, val2);

                    return Toolbox.Calculate(val, (Operator)this.Operator, val2);
                }

                return val;
            }

            return SemanticsSettings.Values.Value;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

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
    #endregion Class: TermValueBase

    #region Class: ValueConditionBase
    /// <summary>
    /// A condition on a value.
    /// </summary>
    public abstract class ValueConditionBase : ConditionBase
    {

        #region Properties and Fields

        #region Property: ValueCondition
        /// <summary>
        /// Gets the value condition of which this is a value condition base.
        /// </summary>
        protected internal ValueCondition ValueCondition
        {
            get
            {
                return this.Condition as ValueCondition;
            }
        }
        #endregion Property: ValueCondition

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ValueConditionBase(ValueCondition valueCondition)
        /// <summary>
        /// Creates a new value condition base from the given value condition.
        /// </summary>
        /// <param name="valueCondition">The value condition to create a new value condition base from.</param>
        protected ValueConditionBase(ValueCondition valueCondition)
            : base(valueCondition)
        {
        }
        #endregion Constructor: ValueConditionBase(ValueCondition valueCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: ValueConditionBase

    #region Class: BoolValueConditionBase
    /// <summary>
    /// A condition on a bool value.
    /// </summary>
    public sealed class BoolValueConditionBase : ValueConditionBase
    {

        #region Properties and Fields

        #region Property: BoolValueCondition
        /// <summary>
        /// Gets the bool value condition of which this is a bool value condition base.
        /// </summary>
        internal BoolValueCondition BoolValueCondition
        {
            get
            {
                return this.Condition as BoolValueCondition;
            }
        }
        #endregion Property: BoolValueCondition

        #region Property: Bool
        /// <summary>
        /// The required boolean value.
        /// </summary>
        private bool? boolean = null;

        /// <summary>
        /// Gets the required boolean value.
        /// </summary>
        public bool? Bool
        {
            get
            {
                return boolean;
            }
        }
        #endregion Property: Bool

        #region Property: Variable
        /// <summary>
        /// The bool variable to represent the bool instead.
        /// </summary>
        private BoolVariableBase variable = null;

        /// <summary>
        /// Gets the bool variable to represent the bool instead.
        /// </summary>
        public BoolVariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: BoolValueConditionBase(BoolValueCondition boolValueCondition)
        /// <summary>
        /// Creates a new bool value condition from the given bool value condition.
        /// </summary>
        /// <param name="boolValueCondition">The bool value condition to create a new bool value condition from.</param>
        internal BoolValueConditionBase(BoolValueCondition boolValueCondition)
            : base(boolValueCondition)
        {
            if (boolValueCondition != null)
            {
                this.boolean = boolValueCondition.Bool;
                this.variable = BaseManager.Current.GetBase<BoolVariableBase>(boolValueCondition.Variable);
            }
        }
        #endregion Constructor: BoolValueConditionBase(BoolValueCondition boolValueCondition)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Gets the value of the bool value condition base, possibly from a variable of the given holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns the value of the bool value condition base.</returns>
        public bool? GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable != null)
            {
                BoolVariableInstance boolVariableInstance = iVariableInstanceHolder.GetVariable(this.Variable) as BoolVariableInstance;
                if (boolVariableInstance != null)
                    return boolVariableInstance.Bool;
            }

            return this.Bool;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: BoolValueConditionBase

    #region Class: NumericalValueConditionBase
    /// <summary>
    /// A condition on a numerical value.
    /// </summary>
    public sealed class NumericalValueConditionBase : ValueConditionBase
    {

        #region Properties and Fields

        #region Property: NumericalValueCondition
        /// <summary>
        /// Gets the numerical value condition of which this is a numerical value condition base.
        /// </summary>
        internal NumericalValueCondition NumericalValueCondition
        {
            get
            {
                return this.Condition as NumericalValueCondition;
            }
        }
        #endregion Property: NumericalValueCondition

        #region Property: Value
        /// <summary>
        /// The required numerical value.
        /// </summary>
        private float? val = null;

        /// <summary>
        /// Gets the required numerical value.
        /// </summary>
        public float? Value
        {
            get
            {
                return val;
            }
        }
        #endregion Property: Value

        #region Property: Variable
        /// <summary>
        /// The variable to represent the value instead. Only valid for numerical and term variables!
        /// </summary>
        private VariableBase variable = null;

        /// <summary>
        /// Gets the variable to represent the value instead. Only valid for numerical and term variables!
        /// </summary>
        public VariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #region Property: ValueSign
        /// <summary>
        /// The sign for the value in the condition.
        /// </summary>
        private EqualitySignExtended? valueSign = null;

        /// <summary>
        /// Gets the sign for the value in the condition.
        /// </summary>
        public EqualitySignExtended? ValueSign
        {
            get
            {
                return valueSign;
            }
        }
        #endregion Property: ValueSign

        #region Property: Prefix
        /// <summary>
        /// The prefix of the value.
        /// </summary>
        private Prefix prefix = default(Prefix);

        /// <summary>
        /// Gets the prefix of the value.
        /// </summary>
        public Prefix Prefix
        {
            get
            {
                return prefix;
            }
        }
        #endregion Property: Prefix

        #region Property: Unit
        /// <summary>
        /// The unit of the value.
        /// </summary>
        private UnitBase unit = null;

        /// <summary>
        /// Gets the unit of the value.
        /// </summary>
        public UnitBase Unit
        {
            get
            {
                return unit;
            }
        }
        #endregion Property: Unit

        #region Property: Min
        /// <summary>
        /// The required minimum value.
        /// </summary>
        private float? min = null;

        /// <summary>
        /// Gets the required minimum value.
        /// </summary>
        public float? Min
        {
            get
            {
                return min;
            }
        }
        #endregion Property: Min

        #region Property: MinVariable
        /// <summary>
        /// The variable to represent the min value instead. Only valid for numerical and term variables!
        /// </summary>
        private VariableBase minVariable = null;

        /// <summary>
        /// Gets the variable to represent the min value instead. Only valid for numerical and term variables!
        /// </summary>
        public VariableBase MinVariable
        {
            get
            {
                return minVariable;
            }
        }
        #endregion Property: MinVariable

        #region Property: MinSign
        /// <summary>
        /// The sign for the minimum value in the condition.
        /// </summary>
        private EqualitySignExtended? minSign = null;

        /// <summary>
        /// Gets the sign for the minimum value in the condition.
        /// </summary>
        public EqualitySignExtended? MinSign
        {
            get
            {
                return minSign;
            }
        }
        #endregion Property: MinSign

        #region Property: Max
        /// <summary>
        /// The required maximum value.
        /// </summary>
        private float? max = null;

        /// <summary>
        /// Gets the required maximum value.
        /// </summary>
        public float? Max
        {
            get
            {
                return max;
            }
        }
        #endregion Property: Max

        #region Property: MaxVariable
        /// <summary>
        /// The variable to represent the max value instead. Only valid for numerical and term variables!
        /// </summary>
        private VariableBase maxVariable = null;

        /// <summary>
        /// Gets the variable to represent the max value instead. Only valid for numerical and term variables!
        /// </summary>
        public VariableBase MaxVariable
        {
            get
            {
                return maxVariable;
            }
        }
        #endregion Property: MaxVariable

        #region Property: MaxSign
        /// <summary>
        /// The sign for the maximum value in the condition.
        /// </summary>
        private EqualitySignExtended? maxSign = null;

        /// <summary>
        /// Gets the sign for the maximum value in the condition.
        /// </summary>
        public EqualitySignExtended? MaxSign
        {
            get
            {
                return maxSign;
            }
        }
        #endregion Property: MaxSign

        #region Property: BaseValue
        /// <summary>
        /// The value that is converted to the base unit, without a prefix.
        /// </summary>
        private float? baseValue = null;

        /// <summary>
        /// Gets the value that is converted to the base unit, without a prefix.
        /// </summary>
        public float? BaseValue
        {
            get
            {
                if (baseValue == null && this.Value != null)
                    baseValue = Utils.GetBaseValue((float)this.Value, this.Prefix, this.Unit);
                return baseValue;
            }
        }
        #endregion: Property: BaseValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NumericalValueConditionBase(NumericalValueCondition numericalValueCondition)
        /// <summary>
        /// Creates a new numerical value condition from the given numerical value condition.
        /// </summary>
        /// <param name="numericalValueCondition">The numerical value condition to create a new numerical value condition from.</param>
        internal NumericalValueConditionBase(NumericalValueCondition numericalValueCondition)
            : base(numericalValueCondition)
        {
            if (numericalValueCondition != null)
            {
                this.val = numericalValueCondition.Value;
                this.variable = BaseManager.Current.GetBase<VariableBase>(numericalValueCondition.Variable);
                this.valueSign = numericalValueCondition.ValueSign;
                this.prefix = numericalValueCondition.Prefix;
                this.unit = BaseManager.Current.GetBase<UnitBase>(numericalValueCondition.Unit);
                this.min = numericalValueCondition.Min;
                this.minVariable = BaseManager.Current.GetBase<VariableBase>(numericalValueCondition.MinVariable);
                this.minSign = numericalValueCondition.MinSign;
                this.max = numericalValueCondition.Max;
                this.maxVariable = BaseManager.Current.GetBase<VariableBase>(numericalValueCondition.MaxVariable);
                this.maxSign = numericalValueCondition.MaxSign;
            }
        }
        #endregion Constructor: NumericalValueConditionBase(NumericalValueCondition numericalValueCondition)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Gets the value of the numerical value condition base, possibly from a variable of the given holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns the value of the numerical value condition base.</returns>
        public float? GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable != null)
            {
                VariableInstance variableInstance = iVariableInstanceHolder.GetVariable(this.Variable);

                NumericalVariableInstance numericalVariableInstance = variableInstance as NumericalVariableInstance;
                if (numericalVariableInstance != null)
                    return Utils.GetBaseValue(numericalVariableInstance.Value, this.Prefix, this.Unit);

                TermVariableInstance termVariableInstance = variableInstance as TermVariableInstance;
                if (termVariableInstance != null)
                    return Utils.GetBaseValue(termVariableInstance.Value, this.Prefix, this.Unit);
            }

            return this.BaseValue;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetMin(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Gets the min value of the numerical value condition base, possibly from a variable of the given holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns the min value of the numerical value condition base.</returns>
        public float? GetMin(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.MinVariable != null)
            {
                VariableInstance variableInstance = iVariableInstanceHolder.GetVariable(this.MinVariable);

                NumericalVariableInstance numericalVariableInstance = variableInstance as NumericalVariableInstance;
                if (numericalVariableInstance != null)
                    return Utils.GetBaseValue(numericalVariableInstance.Value, this.Prefix, this.Unit);

                TermVariableInstance termVariableInstance = variableInstance as TermVariableInstance;
                if (termVariableInstance != null)
                    return Utils.GetBaseValue(termVariableInstance.Value, this.Prefix, this.Unit);
            }

            return this.Min;
        }
        #endregion Method: GetMin(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetMax(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Gets the max value of the numerical value condition base, possibly from a variable of the given holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns the max value of the numerical value condition base.</returns>
        public float? GetMax(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.MaxVariable != null)
            {
                VariableInstance variableInstance = iVariableInstanceHolder.GetVariable(this.MaxVariable);

                NumericalVariableInstance numericalVariableInstance = variableInstance as NumericalVariableInstance;
                if (numericalVariableInstance != null)
                    return Utils.GetBaseValue(numericalVariableInstance.Value, this.Prefix, this.Unit);

                TermVariableInstance termVariableInstance = variableInstance as TermVariableInstance;
                if (termVariableInstance != null)
                    return Utils.GetBaseValue(termVariableInstance.Value, this.Prefix, this.Unit);
            }

            return this.Max;
        }
        #endregion Method: GetMax(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: ValueConditionBase

    #region Class: StringValueConditionBase
    /// <summary>
    /// A condition on a string value.
    /// </summary>
    public sealed class StringValueConditionBase : ValueConditionBase
    {

        #region Properties and Fields

        #region Property: StringValueCondition
        /// <summary>
        /// Gets the string value condition of which this is a string value condition base.
        /// </summary>
        internal StringValueCondition StringValueCondition
        {
            get
            {
                return this.Condition as StringValueCondition;
            }
        }
        #endregion Property: StringValueCondition

        #region Property: String
        /// <summary>
        /// The required string value.
        /// </summary>
        private string str = null;

        /// <summary>
        /// Gets the required string value.
        /// </summary>
        public string String
        {
            get
            {
                return str;
            }
        }
        #endregion Property: String

        #region Property: Variable
        /// <summary>
        /// The string variable to represent the string instead.
        /// </summary>
        private StringVariableBase variable = null;

        /// <summary>
        /// Gets the string variable to represent the string instead.
        /// </summary>
        public StringVariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #region Property: StringSign
        /// <summary>
        /// The sign for the string in the condition.
        /// </summary>
        private EqualitySign? stringSign = null;

        /// <summary>
        /// Gets the sign for the string in the condition.
        /// </summary>
        public EqualitySign? StringSign
        {
            get
            {
                return stringSign;
            }
        }
        #endregion Property: StringSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StringValueConditionBase(StringValueCondition stringValueCondition)
        /// <summary>
        /// Creates a new string value condition from the given string value condition.
        /// </summary>
        /// <param name="stringValueCondition">The string value condition to create a new string value condition from.</param>
        internal StringValueConditionBase(StringValueCondition stringValueCondition)
            : base(stringValueCondition)
        {
            if (stringValueCondition != null)
            {
                this.str = stringValueCondition.String;
                this.variable = BaseManager.Current.GetBase<StringVariableBase>(stringValueCondition.Variable);
                this.stringSign = stringValueCondition.StringSign;
            }
        }
        #endregion Constructor: StringValueConditionBase(StringValueCondition stringValueCondition)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Gets the value of the string value condition base, possibly from a variable of the given holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns the value of the string value condition base.</returns>
        public string GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable != null)
            {
                StringVariableInstance stringVariableInstance = iVariableInstanceHolder.GetVariable(this.Variable) as StringVariableInstance;
                if (stringVariableInstance != null)
                    return stringVariableInstance.String;
            }

            return this.String;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: ValueConditionBase

    #region Class: VectorValueConditionBase
    /// <summary>
    /// A condition on a vector value.
    /// </summary>
    public sealed class VectorValueConditionBase : ValueConditionBase
    {

        #region Properties and Fields

        #region Property: VectorValueCondition
        /// <summary>
        /// Gets the vector value condition of which this is a vector value condition base.
        /// </summary>
        internal VectorValueCondition VectorValueCondition
        {
            get
            {
                return this.Condition as VectorValueCondition;
            }
        }
        #endregion Property: VectorValueCondition

        #region Property: Vector
        /// <summary>
        /// The required four-dimensional vector.
        /// </summary>
        private Vec4 vector = null;

        /// <summary>
        /// Gets the required four-dimensional vector.
        /// </summary>
        public Vec4 Vector
        {
            get
            {
                return vector;
            }
        }
        #endregion Property: Vector

        #region Property: Variable
        /// <summary>
        /// The vector variable to represent the vector instead.
        /// </summary>
        private VectorVariableBase variable = null;

        /// <summary>
        /// Gets the vector variable to represent the vector instead.
        /// </summary>
        public VectorVariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #region Property: VectorSign
        /// <summary>
        /// The sign for the vector value in the condition.
        /// </summary>
        private EqualitySignExtended? vectorSign = null;

        /// <summary>
        /// Gets the sign for the vector value in the condition.
        /// </summary>
        public EqualitySignExtended? VectorSign
        {
            get
            {
                return vectorSign;
            }
        }
        #endregion Property: VectorSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: VectorValueConditionBase(VectorValueCondition vectorValueCondition)
        /// <summary>
        /// Creates a new vector value condition from the given vector value condition.
        /// </summary>
        /// <param name="vectorValueCondition">The vector value condition to create a new vector value condition from.</param>
        internal VectorValueConditionBase(VectorValueCondition vectorValueCondition)
            : base(vectorValueCondition)
        {
            if (vectorValueCondition != null)
            {
                if (vectorValueCondition.Vector != null)
                    this.vector = new Vec4(vectorValueCondition.Vector);
                this.variable = BaseManager.Current.GetBase<VectorVariableBase>(vectorValueCondition.Variable);
                this.vectorSign = vectorValueCondition.VectorSign;
            }
        }
        #endregion Constructor: VectorValueConditionBase(VectorValueCondition vectorValueCondition)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Gets the value of the vector value condition base, possibly from a variable of the given holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns the value of the vector value condition base.</returns>
        public Vec4 GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable != null)
            {
                VectorVariableInstance vectorVariableInstance = iVariableInstanceHolder.GetVariable(this.Variable) as VectorVariableInstance;
                if (vectorVariableInstance != null)
                    return vectorVariableInstance.Vector;
            }

            return this.Vector;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: ValueConditionBase

    #region Class: ValueChangeBase
    /// <summary>
    /// A change on a value.
    /// </summary>
    public abstract class ValueChangeBase : ChangeBase
    {

        #region Properties and Fields

        #region Property: ValueChange
        /// <summary>
        /// Gets the value change of which this is a value change base.
        /// </summary>
        protected internal ValueChange ValueChange
        {
            get
            {
                return this.Change as ValueChange;
            }
        }
        #endregion Property: ValueChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ValueChangeBase(ValueChange valueChange)
        /// <summary>
        /// Creates a value change base from the given value change.
        /// </summary>
        /// <param name="valueChange">The value change to create a value change base from.</param>
        protected ValueChangeBase(ValueChange valueChange)
            : base(valueChange)
        {
        }
        #endregion Constructor: ValueChangeBase(ValueChange valueChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: ValueChangeBase

    #region Class: BoolValueChangeBase
    /// <summary>
    /// A change on a bool value.
    /// </summary>
    public sealed class BoolValueChangeBase : ValueChangeBase
    {

        #region Properties and Fields

        #region Property: BoolValueChange
        /// <summary>
        /// Gets the bool value change of which this is a bool value change base.
        /// </summary>
        internal BoolValueChange BoolValueChange
        {
            get
            {
                return this.Change as BoolValueChange;
            }
        }
        #endregion Property: BoolValueChange

        #region Property: Bool
        /// <summary>
        /// The boolean value to change to.
        /// </summary>
        private bool? boolean = null;

        /// <summary>
        /// Gets or sets the boolean value to change to.
        /// </summary>
        public bool? Bool
        {
            get
            {
                return boolean;
            }
        }
        #endregion Property: Bool

        #region Property: Variable
        /// <summary>
        /// The bool variable to represent the bool instead.
        /// </summary>
        private BoolVariableBase variable = null;

        /// <summary>
        /// Gets the bool variable to represent the bool instead.
        /// </summary>
        public BoolVariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #region Property: Reverse
        /// <summary>
        /// The value that indicates whether the boolean should be reversed. Only valid when set to True. If set to False, Bool will be used instead.
        /// </summary>
        private bool reverse = false;
        
        /// <summary>
        /// Gets the value that indicates whether the boolean should be reversed. Only valid when set to True. If set to False, Bool will be used instead.
        /// </summary>
        public bool Reverse
        {
            get
            {
                return reverse;
            }
        }
        #endregion Property: Reverse

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: BoolValueChangeBase(BoolValueChange boolValueChange)
        /// <summary>
        /// Creates a bool value change base from the given bool value change.
        /// </summary>
        /// <param name="boolValueChange">The bool value change to create a bool value change base from.</param>
        internal BoolValueChangeBase(BoolValueChange boolValueChange)
            : base(boolValueChange)
        {
            if (boolValueChange != null)
            {
                this.boolean = boolValueChange.Bool;
                this.variable = BaseManager.Current.GetBase<BoolVariableBase>(boolValueChange.Variable);
                this.reverse = boolValueChange.Reverse;
            }
        }
        #endregion Constructor: BoolValueChangeBase(BoolValueChange boolValueChange)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Gets the value of the bool value change base, possibly from a variable of the given holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns the value of the bool value change base.</returns>
        public bool? GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable != null)
            {
                BoolVariableInstance boolVariableInstance = iVariableInstanceHolder.GetVariable(this.Variable) as BoolVariableInstance;
                if (boolVariableInstance != null)
                    return boolVariableInstance.Bool;
            }

            return this.Bool;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: BoolValueChangeBase

    #region Class: NumericalValueChangeBase
    /// <summary>
    /// A change on a numerical value.
    /// </summary>
    public sealed class NumericalValueChangeBase : ValueChangeBase
    {

        #region Properties and Fields

        #region Property: NumericalValueChange
        /// <summary>
        /// Gets the numerical value change of which this is a numerical value change base.
        /// </summary>
        internal NumericalValueChange NumericalValueChange
        {
            get
            {
                return this.Change as NumericalValueChange;
            }
        }
        #endregion Property: NumericalValueChange

        #region Property: Value
        /// <summary>
        /// The numerical value to change to.
        /// </summary>
        private float? val = null;

        /// <summary>
        /// Gets or sets the numerical value to change to.
        /// </summary>
        public float? Value
        {
            get
            {
                return val;
            }
        }
        #endregion Property: Value

        #region Property: Variable
        /// <summary>
        /// The variable to represent the value instead. Only valid for numerical and term variables!
        /// </summary>
        private VariableBase variable = null;

        /// <summary>
        /// Gets the variable to represent the value instead. Only valid for numerical and term variables!
        /// </summary>
        public VariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #region Property: ValueChangeType
        /// <summary>
        /// The type of change for the value.
        /// </summary>
        private ValueChangeType? valueChangeType = null;

        /// <summary>
        /// Gets or sets the type of change for the value.
        /// </summary>
        public ValueChangeType? ValueChangeType
        {
            get
            {
                return valueChangeType;
            }
        }
        #endregion Property: ValueChangeType

        #region Property: Prefix
        /// <summary>
        /// The prefix of the value.
        /// </summary>
        private Prefix prefix = default(Prefix);

        /// <summary>
        /// Gets or sets the prefix of the value.
        /// </summary>
        public Prefix Prefix
        {
            get
            {
                return prefix;
            }
            internal set
            {
                if (prefix != value)
                {
                    prefix = value;
                    NotifyPropertyChanged("Prefix");
                }
            }
        }
        #endregion Property: Prefix

        #region Property: Unit
        /// <summary>
        /// The unit of the value.
        /// </summary>
        private UnitBase unit = null;

        /// <summary>
        /// Gets or sets the unit of the value.
        /// </summary>
        public UnitBase Unit
        {
            get
            {
                return unit;
            }
        }
        #endregion Property: Unit

        #region Property: Min
        /// <summary>
        /// The minimum value to change to.
        /// </summary>
        private float? min = null;

        /// <summary>
        /// Gets or sets the minimum value to change to.
        /// </summary>
        public float? Min
        {
            get
            {
                return min;
            }
        }
        #endregion Property: Min

        #region Property: MinVariable
        /// <summary>
        /// The variable to represent the min value instead. Only valid for numerical and term variables!
        /// </summary>
        private VariableBase minVariable = null;

        /// <summary>
        /// Gets the variable to represent the min value instead. Only valid for numerical and term variables!
        /// </summary>
        public VariableBase MinVariable
        {
            get
            {
                return minVariable;
            }
        }
        #endregion Property: MinVariable

        #region Property: MinChange
        /// <summary>
        /// The type of change for the minimum value.
        /// </summary>
        private ValueChangeType? minChange = null;

        /// <summary>
        /// Gets or sets the type of change for the minimum value.
        /// </summary>
        public ValueChangeType? MinChange
        {
            get
            {
                return minChange;
            }
        }
        #endregion Property: MinChange

        #region Property: Max
        /// <summary>
        /// The maximum value to change to.
        /// </summary>
        private float? max = null;

        /// <summary>
        /// Gets or sets the maximum value to change to.
        /// </summary>
        public float? Max
        {
            get
            {
                return max;
            }
        }
        #endregion Property: Max

        #region Property: MaxVariable
        /// <summary>
        /// The variable to represent the max value instead. Only valid for numerical and term variables!
        /// </summary>
        private VariableBase maxVariable = null;

        /// <summary>
        /// Gets the variable to represent the max value instead. Only valid for numerical and term variables!
        /// </summary>
        public VariableBase MaxVariable
        {
            get
            {
                return maxVariable;
            }
        }
        #endregion Property: MaxVariable

        #region Property: MaxChange
        /// <summary>
        /// The type of change for the maximum value.
        /// </summary>
        private ValueChangeType? maxChange = null;

        /// <summary>
        /// Gets or sets the type of change for the maximum value.
        /// </summary>
        public ValueChangeType? MaxChange
        {
            get
            {
                return maxChange;
            }
        }
        #endregion Property: MaxChange

        #region Property: BaseValue
        /// <summary>
        /// The value that is converted to the base unit, without a prefix.
        /// </summary>
        private float? baseValue = null;

        /// <summary>
        /// Gets the value that is converted to the base unit, without a prefix.
        /// </summary>
        public float? BaseValue
        {
            get
            {
                if (baseValue == null && this.Value != null)
                    baseValue = Utils.GetBaseValue((float)this.Value, this.Prefix, this.Unit);
                return baseValue;
            }
        }
        #endregion: Property: BaseValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NumericalValueChangeBase(NumericalValueChange numericalValueChange)
        /// <summary>
        /// Creates a numerical value change base from the given numerical value change.
        /// </summary>
        /// <param name="numericalValueChange">The numerical value change to create a numerical value change base from.</param>
        internal NumericalValueChangeBase(NumericalValueChange numericalValueChange)
            : base(numericalValueChange)
        {
            if (numericalValueChange != null)
            {
                this.val = numericalValueChange.Value;
                this.variable = BaseManager.Current.GetBase<VariableBase>(numericalValueChange.Variable);
                this.valueChangeType = numericalValueChange.ValueChangeType;
                this.prefix = numericalValueChange.Prefix;
                this.unit = BaseManager.Current.GetBase<UnitBase>(numericalValueChange.Unit);
                this.min = numericalValueChange.Min;
                this.minVariable = BaseManager.Current.GetBase<VariableBase>(numericalValueChange.MinVariable);
                this.minChange = numericalValueChange.MinChange;
                this.max = numericalValueChange.Max;
                this.maxVariable = BaseManager.Current.GetBase<VariableBase>(numericalValueChange.MaxVariable);
                this.maxChange = numericalValueChange.MaxChange;
            }
        }
        #endregion Constructor: NumericalValueChangeBase(NumericalValueChange numericalValueChange)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Gets the value of the numerical value change base, possibly from a variable of the given holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns the value of the numerical value change base.</returns>
        public float? GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable != null)
            {
                VariableInstance variableInstance = iVariableInstanceHolder.GetVariable(this.Variable);

                NumericalVariableInstance numericalVariableInstance = variableInstance as NumericalVariableInstance;
                if (numericalVariableInstance != null)
                    return Utils.GetBaseValue(numericalVariableInstance.Value, this.Prefix, this.Unit);

                TermVariableInstance termVariableInstance = variableInstance as TermVariableInstance;
                if (termVariableInstance != null)
                    return Utils.GetBaseValue(termVariableInstance.Value, this.Prefix, this.Unit);
            }

            return this.BaseValue;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetMin(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Gets the min value of the numerical value change base, possibly from a variable of the given holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns the min value of the numerical value change base.</returns>
        public float? GetMin(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.MinVariable != null)
            {
                VariableInstance variableInstance = iVariableInstanceHolder.GetVariable(this.MinVariable);

                NumericalVariableInstance numericalVariableInstance = variableInstance as NumericalVariableInstance;
                if (numericalVariableInstance != null)
                    return Utils.GetBaseValue(numericalVariableInstance.Value, this.Prefix, this.Unit);

                TermVariableInstance termVariableInstance = variableInstance as TermVariableInstance;
                if (termVariableInstance != null)
                    return Utils.GetBaseValue(termVariableInstance.Value, this.Prefix, this.Unit);
            }

            return this.Min;
        }
        #endregion Method: GetMin(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetMax(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Gets the max value of the numerical value change base, possibly from a variable of the given holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns the max value of the numerical value change base.</returns>
        public float? GetMax(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.MaxVariable != null)
            {
                VariableInstance variableInstance = iVariableInstanceHolder.GetVariable(this.MaxVariable);

                NumericalVariableInstance numericalVariableInstance = variableInstance as NumericalVariableInstance;
                if (numericalVariableInstance != null)
                    return Utils.GetBaseValue(numericalVariableInstance.Value, this.Prefix, this.Unit);

                TermVariableInstance termVariableInstance = variableInstance as TermVariableInstance;
                if (termVariableInstance != null)
                    return Utils.GetBaseValue(termVariableInstance.Value, this.Prefix, this.Unit);
            }

            return this.Max;
        }
        #endregion Method: GetMax(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: NumericalValueChangeBase

    #region Class: StringValueChangeBase
    /// <summary>
    /// A change on a string value.
    /// </summary>
    public sealed class StringValueChangeBase : ValueChangeBase
    {

        #region Properties and Fields

        #region Property: StringValueChange
        /// <summary>
        /// Gets the string value change of which this is a string value change base.
        /// </summary>
        internal StringValueChange StringValueChange
        {
            get
            {
                return this.Change as StringValueChange;
            }
        }
        #endregion Property: StringValueChange

        #region Property: String
        /// <summary>
        /// The string value to change to.
        /// </summary>
        private string str = null;

        /// <summary>
        /// Gets or sets the string value to change to.
        /// </summary>
        public string String
        {
            get
            {
                return str;
            }
        }
        #endregion Property: String

        #region Property: Variable
        /// <summary>
        /// The string variable to represent the string instead.
        /// </summary>
        private StringVariableBase variable = null;

        /// <summary>
        /// Gets the string variable to represent the string instead.
        /// </summary>
        public StringVariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StringValueChangeBase(StringValueChange stringValueChange)
        /// <summary>
        /// Creates a string value change base from the given string value change.
        /// </summary>
        /// <param name="stringValueChange">The string value change to create a string value change base from.</param>
        internal StringValueChangeBase(StringValueChange stringValueChange)
            : base(stringValueChange)
        {
            if (stringValueChange != null)
            {
                this.str = stringValueChange.String;
                this.variable = BaseManager.Current.GetBase<StringVariableBase>(stringValueChange.Variable);
            }
        }
        #endregion Constructor: StringValueChangeBase(StringValueChange stringValueChange)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Gets the value of the string value change base, possibly from a variable of the given holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns the value of the string value change base.</returns>
        public string GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable != null)
            {
                StringVariableInstance stringVariableInstance = iVariableInstanceHolder.GetVariable(this.Variable) as StringVariableInstance;
                if (stringVariableInstance != null)
                    return stringVariableInstance.String;
            }

            return this.String;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: StringValueChangeBase

    #region Class: VectorValueChangeBase
    /// <summary>
    /// A change on a vector value.
    /// </summary>
    public sealed class VectorValueChangeBase : ValueChangeBase
    {

        #region Properties and Fields

        #region Property: VectorValueChange
        /// <summary>
        /// Gets the vector value change of which this is a vector value change base.
        /// </summary>
        internal VectorValueChange VectorValueChange
        {
            get
            {
                return this.Change as VectorValueChange;
            }
        }
        #endregion Property: VectorValueChange

        #region Property: Vector
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
                return vector;
            }
        }
        #endregion Property: Vector

        #region Property: Variable
        /// <summary>
        /// The vector variable to represent the vector instead.
        /// </summary>
        private VectorVariableBase variable = null;

        /// <summary>
        /// Gets the vector variable to represent the vector instead.
        /// </summary>
        public VectorVariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #region Property: VectorChange
        /// <summary>
        /// The type of change for the vector value.
        /// </summary>
        private ValueChangeType? vectorChange = null;

        /// <summary>
        /// Gets or sets the type of change for the vector value.
        /// </summary>
        public ValueChangeType? VectorChange
        {
            get
            {
                return vectorChange;
            }
        }
        #endregion Property: VectorChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: VectorValueChangeBase(VectorValueChange vectorValueChange)
        /// <summary>
        /// Creates a vector value change base from the given vector value change.
        /// </summary>
        /// <param name="vectorValueChange">The vector value change to create a vector value change base from.</param>
        internal VectorValueChangeBase(VectorValueChange vectorValueChange)
            : base(vectorValueChange)
        {
            if (vectorValueChange != null)
            {
                if (vectorValueChange.Vector != null)
                    this.vector = new Vec4(vectorValueChange.Vector);
                this.variable = BaseManager.Current.GetBase<VectorVariableBase>(vectorValueChange.Variable);
                this.vectorChange = vectorValueChange.VectorChange;
            }
        }
        #endregion Constructor: VectorValueChangeBase(VectorValueChange vectorValueChange)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Gets the value of the vector value change base, possibly from a variable of the given holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns the value of the vector value change base.</returns>
        public Vec4 GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable != null)
            {
                VectorVariableInstance vectorVariableInstance = iVariableInstanceHolder.GetVariable(this.Variable) as VectorVariableInstance;
                if (vectorVariableInstance != null)
                    return vectorVariableInstance.Vector;
            }

            return this.Vector;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: VectorValueChangeBase

    #region Class: NumericalValueRangeBase
    /// <summary>
    /// A range on a numerical value.
    /// </summary>
    public class NumericalValueRangeBase : Base
    {

        #region Properties and Fields

        #region Property: NumericalValueRange
        /// <summary>
        /// Gets the numerical value range of which this is a numerical value range base.
        /// </summary>
        internal NumericalValueRange NumericalValueRange
        {
            get
            {
                return this.IdHolder as NumericalValueRange;
            }
        }
        #endregion Property: NumericalValueRange

        #region Property: ValueSign
        /// <summary>
        /// The sign for the value in the range.
        /// </summary>
        private EqualitySignExtendedDual valueSign = EqualitySignExtendedDual.Equal;

        /// <summary>
        /// Gets the sign for the value in the range.
        /// </summary>
        public EqualitySignExtendedDual ValueSign
        {
            get
            {
                return valueSign;
            }
        }
        #endregion Property: ValueSign

        #region Property: Value
        /// <summary>
        /// The value of this numerical value range.
        /// </summary>
        private float value = SemanticsSettings.Values.Value;

        /// <summary>
        /// Gets the value of this numerical value range.
        /// </summary>
        public float Value
        {
            get
            {
                return value;
            }
        }
        #endregion Property: Value

        #region Property: Value2
        /// <summary>
        /// The second value of this numerical value range. Only used for the 'Between' sign!
        /// </summary>
        private float value2 = SemanticsSettings.Values.Value;

        /// <summary>
        /// Gets the second value of this numerical value range. Only used for the 'Between' sign!
        /// </summary>
        public float Value2
        {
            get
            {
                return value2;
            }
        }
        #endregion Property: Value2

        #region Property: Variable
        /// <summary>
        /// The variable to represent the value instead. Only valid for numerical and term variables!
        /// </summary>
        private VariableBase variable = null;

        /// <summary>
        /// Gets the variable to represent the value instead. Only valid for numerical and term variables!
        /// </summary>
        public VariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #region Property: Variable2
        /// <summary>
        /// The variable to represent the second value instead. Only valid for numerical and term variables!
        /// </summary>
        private VariableBase variable2 = null;

        /// <summary>
        /// Gets the variable to represent the second value instead. Only valid for numerical and term variables!
        /// </summary>
        public VariableBase Variable2
        {
            get
            {
                return variable2;
            }
        }
        #endregion Property: Variable2

        #region Property: Prefix
        /// <summary>
        /// The prefix.
        /// </summary>
        private Prefix prefix = default(Prefix);

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        public Prefix Prefix
        {
            get
            {
                return prefix;
            }
        }
        #endregion Property: Prefix

        #region Property: Unit
        /// <summary>
        /// The unit for the value.
        /// </summary>
        private UnitBase unit = null;

        /// <summary>
        /// Gets the unit for the value.
        /// </summary>
        public UnitBase Unit
        {
            get
            {
                return unit;
            }
            internal set
            {
                unit = value;
            }
        }
        #endregion Property: Unit

        #region Property: BaseValue
        /// <summary>
        /// Get the value that is converted to the base unit, without a prefix.
        /// </summary>
        private float? baseValue = null;

        /// <summary>
        /// Get the value that is converted to the base unit, without a prefix.
        /// </summary>
        public float BaseValue
        {
            get
            {
                if (baseValue == null)
                    baseValue = Utils.GetBaseValue(this.value, this.prefix, this.unit);
                return (float)baseValue;
            }
        }
        #endregion Property: BaseValue

        #region Property: BaseValue2
        /// <summary>
        /// Get the second value that is converted to the base unit, without a prefix.
        /// </summary>
        private float? baseValue2 = null;

        /// <summary>
        /// Get the second value that is converted to the base unit, without a prefix.
        /// </summary>
        public float BaseValue2
        {
            get
            {
                if (baseValue2 == null)
                    baseValue2 = Utils.GetBaseValue(this.value2, this.prefix, this.unit);
                return (float)baseValue2;
            }
        }
        #endregion Property: BaseValue2

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NumericalValueRangeBase(NumericalValueRange numericalValueRange)
        /// <summary>
        /// Creates a base of the given numerical value range.
        /// </summary>
        /// <param name="numericalValueRange">The numerical value range to create a base of.</param>
        internal NumericalValueRangeBase(NumericalValueRange numericalValueRange)
            : base(numericalValueRange)
        {
            if (numericalValueRange != null)
            {
                this.valueSign = numericalValueRange.ValueSign;
                this.value = numericalValueRange.Value;
                this.value2 = numericalValueRange.Value2;
                this.variable = BaseManager.Current.GetBase<VariableBase>(numericalValueRange.Variable);
                this.variable2 = BaseManager.Current.GetBase<VariableBase>(numericalValueRange.Variable2);
                this.prefix = numericalValueRange.Prefix;
                this.unit = BaseManager.Current.GetBase<UnitBase>(numericalValueRange.Unit);
            }
        }
        #endregion Constructor: NumericalValueRangeBase(NumericalValueRange numericalValueRange)

        #region Constructor: NumericalValueRangeBase(float val)
        /// <summary>
        /// Creates a numerical value range base of the given value.
        /// </summary>
        /// <param name="val">The value to create a numerical range base of.</param>
        internal NumericalValueRangeBase(float val)
            : base(null)
        {
            this.value = val;
        }
        #endregion Constructor: NumericalValueRangeBase(float val)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the value of the numerical value range base for the given variable holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The value of the numerical value range base.</returns>
        internal float GetValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable != null)
            {
                if (this.Variable is NumericalVariableBase)
                    return new NumericalVariableInstance((NumericalVariableBase)this.Variable, iVariableInstanceHolder).Value;
                else if (this.Variable is TermVariableBase)
                    return new TermVariableInstance((TermVariableBase)this.Variable, iVariableInstanceHolder).Value;
            }
            return this.BaseValue;
        }
        #endregion Method: GetValue(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetValue2(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the second value of the numerical value range base for the given variable holder.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The second value of the numerical value range base.</returns>
        internal float GetValue2(IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (this.Variable2 != null)
            {
                if (this.Variable2 is NumericalVariableBase)
                    return new NumericalVariableInstance((NumericalVariableBase)this.Variable2, iVariableInstanceHolder).Value;
                else if (this.Variable2 is TermVariableBase)
                    return new TermVariableInstance((TermVariableBase)this.Variable2, iVariableInstanceHolder).Value;
            }
            return this.BaseValue2;
        }
        #endregion Method: GetValue2(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetMinimumHighestInteger(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the minimum highest integer in the range; that is, the highest integer excluding all values exceeding the 'greater than' sign.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The minimum highest integer in the range.</returns>
        public int GetMinimumHighestInteger(IVariableInstanceHolder iVariableInstanceHolder)
        {
            int highestInteger = 0;

            switch (this.ValueSign)
            {
                case EqualitySignExtendedDual.Equal:
                case EqualitySignExtendedDual.GreaterOrEqual:
                case EqualitySignExtendedDual.LowerOrEqual:
                    highestInteger = (int)GetValue(iVariableInstanceHolder);
                    break;
                case EqualitySignExtendedDual.NotEqual:
                case EqualitySignExtendedDual.Greater:
                    highestInteger = (int)GetValue(iVariableInstanceHolder) + 1;
                    break;
                case EqualitySignExtendedDual.Lower:
                    highestInteger = (int)GetValue(iVariableInstanceHolder) - 1;
                    break;
                case EqualitySignExtendedDual.Between:
                    highestInteger = (int)GetValue2(iVariableInstanceHolder);
                    break;
                case EqualitySignExtendedDual.NotBetween:
                    if (GetValue(iVariableInstanceHolder) == 2)
                        highestInteger = 1;
                    else
                        highestInteger = (int)GetValue2(iVariableInstanceHolder) + 1;
                    break;
                default:
                    break;
            }

            return highestInteger;
        }
        #endregion Method: GetMinimumHighestInteger(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetMinimumHighestValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the minimum highest value in the range; that is, the highest value excluding all values exceeding the 'greater than' sign.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The minimum highest value in the range.</returns>
        public float GetMinimumHighestValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            float highestValue = 0;

            switch (this.ValueSign)
            {
                case EqualitySignExtendedDual.Equal:
                case EqualitySignExtendedDual.GreaterOrEqual:
                case EqualitySignExtendedDual.LowerOrEqual:
                    highestValue = GetValue(iVariableInstanceHolder);
                    break;
                case EqualitySignExtendedDual.NotEqual:
                case EqualitySignExtendedDual.Greater:
                    highestValue = GetValue(iVariableInstanceHolder) + 0.1f;
                    break;
                case EqualitySignExtendedDual.Lower:
                    highestValue = GetValue(iVariableInstanceHolder) - 0.1f;
                    break;
                case EqualitySignExtendedDual.Between:
                    highestValue = GetValue2(iVariableInstanceHolder);
                    break;
                case EqualitySignExtendedDual.NotBetween:
                    float value1 = GetValue(iVariableInstanceHolder) - 0.1f;
                    if (value1 >= 0)
                        highestValue = value1;
                    else
                        highestValue = GetValue2(iVariableInstanceHolder) + 0.1f;
                    break;
                default:
                    break;
            }

            return highestValue;
        }
        #endregion Method: GetMinimumHighestValue(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetRandomInteger(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get a random integer in the range.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>A random integer in the range.</returns>
        public int GetRandomInteger(IVariableInstanceHolder iVariableInstanceHolder)
        {
            int randomInteger = 0;

            int baseValue = (int)GetValue(iVariableInstanceHolder);
            switch (this.ValueSign)
            {
                case EqualitySignExtendedDual.Equal:
                    randomInteger = baseValue;
                    break;
                case EqualitySignExtendedDual.GreaterOrEqual:
                    randomInteger = (int)RandomNumber.Next(baseValue, baseValue * SemanticsEngineSettings.GreaterMultiplier);
                    break;
                case EqualitySignExtendedDual.Greater:
                    randomInteger = (int)RandomNumber.Next(baseValue + 1, (baseValue + 1) * SemanticsEngineSettings.GreaterMultiplier);
                    break;
                case EqualitySignExtendedDual.LowerOrEqual:
                    randomInteger = (int)RandomNumber.Next(0, baseValue);
                    break;
                case EqualitySignExtendedDual.NotEqual:
                    if (baseValue >= 1 && RandomNumber.RandomF() > 0.5f)
                        randomInteger = (int)RandomNumber.Next(0, baseValue);
                    else
                        randomInteger = (int)RandomNumber.Next(baseValue + 1, (baseValue + 1) * SemanticsEngineSettings.GreaterMultiplier);
                    break;
                case EqualitySignExtendedDual.Lower:
                    randomInteger = (int)RandomNumber.Next(0, baseValue);
                    break;
                case EqualitySignExtendedDual.Between:
                    randomInteger = RandomNumber.Next(baseValue, (int)GetValue2(iVariableInstanceHolder) + 1);
                    break;
                case EqualitySignExtendedDual.NotBetween:
                    if (baseValue >= 1 && RandomNumber.RandomF() > 0.5f)
                        randomInteger = (int)RandomNumber.Next(0, (int)baseValue);
                    else
                    {
                        int baseValue2 = (int)GetValue2(iVariableInstanceHolder) + 1;
                        randomInteger = (int)RandomNumber.Next(baseValue2, baseValue2 * SemanticsEngineSettings.GreaterMultiplier);
                    }
                    break;
                default:
                    break;
            }

            return randomInteger;
        }
        #endregion Method: GetRandomInteger(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetRandomValue(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get a random value in the range.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>A random value in the range.</returns>
        public float GetRandomValue(IVariableInstanceHolder iVariableInstanceHolder)
        {
            float randomValue = 0;

            float baseValue = GetValue(iVariableInstanceHolder);
            switch (this.ValueSign)
            {
                case EqualitySignExtendedDual.Equal:
                    randomValue = baseValue;
                    break;
                case EqualitySignExtendedDual.GreaterOrEqual:
                    randomValue = RandomNumber.RandomF(baseValue, baseValue * SemanticsEngineSettings.GreaterMultiplier);
                    break;
                case EqualitySignExtendedDual.Greater:
                    randomValue = RandomNumber.RandomF(baseValue + 0.1f, (baseValue + 0.1f) * SemanticsEngineSettings.GreaterMultiplier);
                    break;
                case EqualitySignExtendedDual.LowerOrEqual:
                    randomValue = RandomNumber.RandomF(0, baseValue);
                    break;
                case EqualitySignExtendedDual.NotEqual:
                    if (baseValue >= 1 && RandomNumber.RandomF() > 0.5f)
                        randomValue = RandomNumber.RandomF(0, baseValue);
                    else
                        randomValue = RandomNumber.RandomF(baseValue + 0.1f, (baseValue + 0.1f) * SemanticsEngineSettings.GreaterMultiplier);
                    break;
                case EqualitySignExtendedDual.Lower:
                    randomValue = (int)RandomNumber.RandomF(0, baseValue);
                    break;
                case EqualitySignExtendedDual.Between:
                    randomValue = RandomNumber.RandomF(baseValue, GetValue2(iVariableInstanceHolder) + 0.1f);
                    break;
                case EqualitySignExtendedDual.NotBetween:
                    if (baseValue >= 1 && RandomNumber.RandomF() > 0.5f)
                        randomValue = RandomNumber.RandomF(0, baseValue);
                    else
                    {
                        float baseValue2 = GetValue2(iVariableInstanceHolder) + 0.1f;
                        randomValue = RandomNumber.RandomF(baseValue2, baseValue2 * SemanticsEngineSettings.GreaterMultiplier);
                    }
                    break;
                default:
                    break;
            }

            return randomValue;
        }
        #endregion Method: GetRandomValue(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: IsInRange(NumericalValueInstance numericalValueInstance)
        /// <summary>
        /// Check whether the given numerical value instance is in the range.
        /// </summary>
        /// <param name="numericalValueInstance">The numerical value instance to check.</param>
        /// <returns>Returns whether the numerical value instance is in the range.</returns>
        public bool IsInRange(NumericalValueInstance numericalValueInstance)
        {
            if (numericalValueInstance != null)
                return Toolbox.Compare(this.BaseValue, this.ValueSign, numericalValueInstance.BaseValue, this.BaseValue2);
            return false;
        }
        #endregion Method: IsInRange(NumericalValueInstance numericalValueInstance)

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
    #endregion Class: NumericalValueRangeBase

}