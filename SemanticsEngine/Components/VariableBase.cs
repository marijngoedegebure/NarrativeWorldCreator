/**************************************************************************
 * 
 * VariableBase.cs
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
using Common;
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: VariableBase
    /// <summary>
    /// A base for a variable.
    /// </summary>
    public abstract class VariableBase : Base
    {

        #region Properties and Fields

        #region Property: Variable
        /// <summary>
        /// Gets the variable this is a base of.
        /// </summary>
        internal Variable Variable
        {
            get
            {
                return this.IdHolder as Variable;
            }
        }
        #endregion Property: Variable

        #region Property: Name
        /// <summary>
        /// The name of the variable.
        /// </summary>
        private String name = null;
        
        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
        }
        #endregion Property: Name

        #region Property: VariableType
        /// <summary>
        /// The type of the variable. Only valid when VariableType has been set to 'Fixed' or 'RequiresManualInput'.
        /// </summary>
        private VariableType variableType = default(VariableType);
        
        /// <summary>
        /// Gets the type of the variable. Only valid when VariableType has been set to 'Fixed' or 'RequiresManualInput'.
        /// </summary>
        public VariableType VariableType
        {
            get
            {
                return variableType;
            }
        }
        #endregion Property: VariableType

        #region Property: Source
        /// <summary>
        /// The source, in case VariableType has been set to 'Attribute', 'Count', or 'Quantity'.
        /// </summary>
        private ActorTargetArtifactReference source = default(ActorTargetArtifactReference);

        /// <summary>
        /// Gets the source, in case VariableType has been set to 'Attribute', 'Count', or 'Quantity'.
        /// </summary>
        public ActorTargetArtifactReference Source
        {
            get
            {
                return source;
            }
        }
        #endregion Property: Source

        #region Property: Reference
        /// <summary>
        /// The reference of the variable, in case the Source has been set to 'Reference'.
        /// </summary>
        private ReferenceBase reference = null;

        /// <summary>
        /// Gets the reference of the variable, in case the Source has been set to 'Reference'.
        /// </summary>
        public ReferenceBase Reference
        {
            get
            {
                return reference;
            }
        }
        #endregion Property: Reference

        #region Property: Attribute
        /// <summary>
        /// The attribute of the variable, in case the VariableType has been set to 'Attribute' or 'Sum'.
        /// </summary>
        private AttributeBase attribute = null;
        
        /// <summary>
        /// Gets the attribute of the variable, in case the VariableType has been set to 'Attribute' or 'Sum'.
        /// </summary>
        public AttributeBase Attribute
        {
            get
            {
                return attribute;
            }
        }
        #endregion Property: Attribute

        #region Property: CountType
        /// <summary>
        /// Indicates where the search to the countable entities should be performed, in case VariableType has been set to 'Count'.
        /// </summary>
        private CountType countType = default(CountType);
        
        /// <summary>
        /// Gets where the search to the countable entities should be performed, in case VariableType has been set to 'Count'.
        /// </summary>
        public CountType CountType
        {
            get
            {
                return countType;
            }
        }
        #endregion Property: CountType

        #region Property: CountableEntity
        /// <summary>
        /// The countable entity, in case VariableType has been set to 'Count'.
        /// </summary>
        private EntityBase countableEntity = null;
        
        /// <summary>
        /// Gets the countable entity, in case VariableType has been set to 'Count'.
        /// </summary>
        public EntityBase CountableEntity
        {
            get
            {
                return countableEntity;
            }
        }
        #endregion Property: CountableEntity

        #region Property: Matter
        /// <summary>
        /// The matter of the variable, in case the VariableType has been set to 'Quantity'.
        /// </summary>
        private MatterBase matter = null;

        /// <summary>
        /// Gets the matter of the variable, in case the VariableType has been set to 'Quantity'.
        /// </summary>
        public MatterBase Matter
        {
            get
            {
                return matter;
            }
        }
        #endregion Property: Matter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: VariableBase(Variable variable)
        /// <summary>
        /// Create a variable base from the given variable.
        /// </summary>
        /// <param name="variable">The variable to create a variable base from.</param>
        protected VariableBase(Variable variable)
            : base(variable)
        {
            if (variable != null)
            {
                this.name = variable.Name;
                this.variableType = variable.VariableType;
                this.source = variable.Source;
                this.reference = BaseManager.Current.GetBase<ReferenceBase>(variable.Reference);
                this.attribute = BaseManager.Current.GetBase<AttributeBase>(variable.Attribute);
                this.countType = variable.CountType;
                this.countableEntity = BaseManager.Current.GetBase<EntityBase>(variable.CountableEntity);
                this.matter = BaseManager.Current.GetBase<MatterBase>(variable.Matter);
            }
        }
        #endregion Constructor: VariableBase(Variable variable)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation of the variable, which is its name.
        /// </summary>
        /// <returns>The string representation of the variable, which is its name.</returns>
        public override string ToString()
        {
            return this.Name;
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: VariableBase

    #region Class: BoolVariableBase
    /// <summary>
    /// A base of a bool variable.
    /// </summary>
    public sealed class BoolVariableBase : VariableBase
    {

        #region Properties and Fields

        #region Property: FixedValue
        /// <summary>
        /// The fixed value of the bool variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        private bool fixedValue = SemanticsSettings.Values.Boolean;
        
        /// <summary>
        /// Gets the fixed value of the bool variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        public bool FixedValue
        {
            get
            {
                return fixedValue;
            }
        }
        #endregion Property: FixedValue
        
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: BoolVariableBase(BoolVariable boolVariable)
        /// <summary>
        /// Create a bool variable base from the given bool variable.
        /// </summary>
        /// <param name="boolVariable">The bool variable to create a bool variable base from.</param>
        internal BoolVariableBase(BoolVariable boolVariable)
            : base(boolVariable)
        {
            if (boolVariable != null)
                this.fixedValue = boolVariable.FixedValue;
        }
        #endregion Constructor: BoolVariableBase(BoolVariable boolVariable)

        #endregion Method Group: Constructors

    }
    #endregion Class: BoolVariableBase

    #region Class: NumericalVariableBase
    /// <summary>
    /// A base of a numerical variable.
    /// </summary>
    public sealed class NumericalVariableBase : VariableBase
    {

        #region Properties and Fields

        #region Property: FixedValue
        /// <summary>
        /// The fixed value of the numerical variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        private float fixedValue = SemanticsSettings.Values.Value;

        /// <summary>
        /// Gets the fixed value of the numerical variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        public float FixedValue
        {
            get
            {
                return fixedValue;
            }
        }
        #endregion Property: FixedValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NumericalVariableBase(NumericalVariable numericalVariable)
        /// <summary>
        /// Create a numerical variable base from the given numerical variable.
        /// </summary>
        /// <param name="numericalVariable">The numerical variable to create a numerical variable base from.</param>
        internal NumericalVariableBase(NumericalVariable numericalVariable)
            : base(numericalVariable)
        {
            if (numericalVariable != null)
                this.fixedValue = numericalVariable.FixedValue;
        }
        #endregion Constructor: NumericalVariableBase(NumericalVariable numericalVariable)

        #endregion Method Group: Constructors

    }
    #endregion Class: NumericalVariableBase

    #region Class: StringVariableBase
    /// <summary>
    /// A base of a string variable.
    /// </summary>
    public sealed class StringVariableBase : VariableBase
    {

        #region Properties and Fields

        #region Property: FixedValue
        /// <summary>
        /// The fixed value of the string variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        private string fixedValue = SemanticsSettings.Values.String;

        /// <summary>
        /// Gets the fixed value of the string variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        public string FixedValue
        {
            get
            {
                return fixedValue;
            }
        }
        #endregion Property: FixedValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StringVariableBase(StringVariable stringVariable)
        /// <summary>
        /// Create a string variable base from the given string variable.
        /// </summary>
        /// <param name="stringVariable">The string variable to create a string variable base from.</param>
        internal StringVariableBase(StringVariable stringVariable)
            : base(stringVariable)
        {
            if (stringVariable != null)
                this.fixedValue = stringVariable.FixedValue;
        }
        #endregion Constructor: StringVariableBase(StringVariable stringVariable)

        #endregion Method Group: Constructors

    }
    #endregion Class: StringVariableBase

    #region Class: VectorVariableBase
    /// <summary>
    /// A base of a vector variable.
    /// </summary>
    public sealed class VectorVariableBase : VariableBase
    {

        #region Properties and Fields

        #region Property: FixedValue
        /// <summary>
        /// The fixed value of the vector variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        private Vec4 fixedValue = new Vec4(SemanticsSettings.Values.Vector4X, SemanticsSettings.Values.Vector4Y, SemanticsSettings.Values.Vector4Z, SemanticsSettings.Values.Vector4W);

        /// <summary>
        /// Gets the fixed value of the vector variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        public Vec4 FixedValue
        {
            get
            {
                return fixedValue;
            }
        }
        #endregion Property: FixedValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: VectorVariableBase(VectorVariable vectorVariable)
        /// <summary>
        /// Create a vector variable base from the given vector variable.
        /// </summary>
        /// <param name="vectorVariable">The vector variable to create a vector variable base from.</param>
        internal VectorVariableBase(VectorVariable vectorVariable)
            : base(vectorVariable)
        {
            if (vectorVariable != null)
                this.fixedValue = new Vec4(vectorVariable.FixedValue);
        }
        #endregion Constructor: VectorVariableBase(VectorVariable vectorVariable)

        #endregion Method Group: Constructors

    }
    #endregion Class: VectorVariableBase

    #region Class: RandomVariableBase
    /// <summary>
    /// A base of a random variable.
    /// </summary>
    public sealed class RandomVariableBase : VariableBase
    {

        #region Properties and Fields

        #region Property: FixedValue
        /// <summary>
        /// The fixed value of the random variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        private float fixedValue = SemanticsSettings.Values.Chance;

        /// <summary>
        /// Gets the fixed value of the random variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        public float FixedValue
        {
            get
            {
                return fixedValue;
            }
        }
        #endregion Property: FixedValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: RandomVariableBase(RandomVariable randomVariable)
        /// <summary>
        /// Create a random variable base from the given random variable.
        /// </summary>
        /// <param name="randomVariable">The random variable to create a random variable base from.</param>
        internal RandomVariableBase(RandomVariable randomVariable)
            : base(randomVariable)
        {
            if (randomVariable != null)
                this.fixedValue = randomVariable.FixedValue;
        }
        #endregion Constructor: RandomVariableBase(RandomVariable randomVariable)

        #endregion Method Group: Constructors

    }
    #endregion Class: RandomVariableBase

    #region Class: TermVariableBase
    /// <summary>
    /// A base of a term variable.
    /// </summary>
    public sealed class TermVariableBase : VariableBase
    {

        #region Properties and Fields

        #region Property: Function1
        /// <summary>
        /// The (optional) function on the first variable.
        /// </summary>
        private Function? function1 = null;

        /// <summary>
        /// Gets the (optional) function on the first variable.
        /// </summary>
        public Function? Function1
        {
            get
            {
                return function1;
            }
        }
        #endregion Property: Function1

        #region Property: Variable1
        /// <summary>
        /// The (required) first variable. Only works for NumericalVariableBase and TermVariableBase!
        /// </summary>
        private VariableBase variable1 = null;

        /// <summary>
        /// Gets the (required) first variable. Only works for NumericalVariableBase and TermVariableBase!
        /// </summary>
        public VariableBase Variable1
        {
            get
            {
                return variable1;
            }
        }
        #endregion Property: Variable1

        #region Property: Value1
        /// <summary>
        /// The first value of the term variable, in case the first variable should be a fixed value instead.
        /// </summary>
        public float? value1 = null;

        /// <summary>
        /// Gets the first value of the term variable, in case the first variable should be a fixed value instead.
        /// </summary>
        public float? Value1
        {
            get
            {
                return value1;
            }
        }
        #endregion Property: Value1

        #region Property: Operator
        /// <summary>
        /// The (optional) operator between both variables. Only works when Variable2 or Value2 are set!
        /// </summary>
        private Operator? op = null;

        /// <summary>
        /// Gets the (optional) operator between both variables. Only works when Variable2 or Value2 are set!
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
        /// The (optional) function on the second variable/value.
        /// </summary>
        private Function? function2 = null;

        /// <summary>
        /// Gets the (optional) function on the second variable/value.
        /// </summary>
        public Function? Function2
        {
            get
            {
                return function2;
            }
        }
        #endregion Property: Function2

        #region Property: Variable2
        /// <summary>
        /// The (optional) second variable. Only works for NumericalVariableBase and TermVariableBase and when an Operator has been set!
        /// </summary>
        private VariableBase variable2 = null;

        /// <summary>
        /// Gets the (optional) second variable. Only works for NumericalVariableBase and TermVariableBase and when an Operator has been set!
        /// </summary>
        public VariableBase Variable2
        {
            get
            {
                return variable2;
            }
        }
        #endregion Property: Variable2

        #region Property: Value2
        /// <summary>
        /// The second value of the term variable, in case the second variable should be a fixed value instead. Only works when an Operator has been set!
        /// </summary>
        public float? value2 = null;
        
        /// <summary>
        /// Gets the second value of the term variable, in case the second variable should be a fixed value instead. Only works when an Operator has been set!
        /// </summary>
        public float? Value2
        {
            get
            {
                return value2;
            }
        }
        #endregion Property: Value2

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TermVariableBase(TermVariable termVariable)
        /// <summary>
        /// Create a term variable base from the given term variable.
        /// </summary>
        /// <param name="termVariable">The term variable to create a term variable base from.</param>
        internal TermVariableBase(TermVariable termVariable)
            : base(termVariable)
        {
            if (termVariable != null)
            {
                this.function1 = termVariable.Function1;
                this.variable1 = BaseManager.Current.GetBase<VariableBase>(termVariable.Variable1);
                this.value1 = termVariable.Value1;
                this.op = termVariable.Operator;
                this.function2 = termVariable.Function2;
                this.variable2 = BaseManager.Current.GetBase<VariableBase>(termVariable.Variable2);
                this.value2 = termVariable.Value2;
            }
        }
        #endregion Constructor: TermVariableBase(TermVariable termVariable)

        #endregion Method Group: Constructors

    }
    #endregion Class: TermVariableBase

}