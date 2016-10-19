/**************************************************************************
 * 
 * Variable.cs
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
using System.Collections.Generic;
using System.Reflection;
using Common;
using Semantics.Data;
using Semantics.Entities;
using Semantics.Utilities;
using Attribute = Semantics.Abstractions.Attribute;

namespace Semantics.Components
{

    #region Class: Variable
    /// <summary>
    /// A variable.
    /// </summary>
    public abstract class Variable : IdHolder
    {

        #region Properties and Fields

        #region Property: Name
        /// <summary>
        /// Gets or sets the name of the variable.
        /// </summary>
        public String Name
        {
            get
            {
                return Database.Current.Select<String>(this.ID, LocalizationTables.VariableName, Columns.Name);
            }
            set
            {
                Database.Current.Update(this.ID, LocalizationTables.VariableName, Columns.Name, value);
                NotifyPropertyChanged("Name");
            }
        }
        #endregion Property: Name

        #region Property: VariableType
        /// <summary>
        /// Gets or sets the type of the variable.
        /// </summary>
        public VariableType VariableType
        {
            get
            {
                return Database.Current.Select<VariableType>(this.ID, ValueTables.Variable, Columns.Type);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Variable, Columns.Type, value);
                NotifyPropertyChanged("VariableType");
            }
        }
        #endregion Property: VariableType

        #region Property: Source
        /// <summary>
        /// Gets or sets the source, in case VariableType has been set to 'Attribute', 'Count', or 'Quantity'.
        /// </summary>
        public ActorTargetArtifactReference Source
        {
            get
            {
                return Database.Current.Select<ActorTargetArtifactReference>(this.ID, ValueTables.Variable, Columns.Source);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Variable, Columns.Source, value);
                NotifyPropertyChanged("Source");
            }
        }
        #endregion Property: Source

        #region Property: Reference
        /// <summary>
        /// Gets or sets the reference of the variable, in case the Source has been set to 'Reference'.
        /// </summary>
        public Reference Reference
        {
            get
            {
                return Database.Current.Select<Reference>(this.ID, ValueTables.Variable, Columns.Reference);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Variable, Columns.Reference, value);
                NotifyPropertyChanged("Reference");
            }
        }
        #endregion Property: Reference

        #region Property: Attribute
        /// <summary>
        /// Gets or sets the attribute of the variable, in case the VariableType has been set to 'Attribute' or 'Sum'.
        /// </summary>
        public Attribute Attribute
        {
            get
            {
                return Database.Current.Select<Attribute>(this.ID, ValueTables.Variable, Columns.Attribute);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Variable, Columns.Attribute, value);
                NotifyPropertyChanged("Attribute");
            }
        }
        #endregion Property: Attribute

        #region Property: CountType
        /// <summary>
        /// Gets or sets where the search to the countable entities should be performed, in case VariableType has been set to 'Count'.
        /// </summary>
        public CountType CountType
        {
            get
            {
                return Database.Current.Select<CountType>(this.ID, ValueTables.Variable, Columns.CountType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Variable, Columns.CountType, value);
                NotifyPropertyChanged("CountType");
            }
        }
        #endregion Property: CountType

        #region Property: CountableEntity
        /// <summary>
        /// Gets or sets the countable entity, in case VariableType has been set to 'Count'.
        /// </summary>
        public Entity CountableEntity
        {
            get
            {
                return Database.Current.Select<Entity>(this.ID, ValueTables.Variable, Columns.CountableEntity);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Variable, Columns.CountableEntity, value);
                NotifyPropertyChanged("CountableEntity");
            }
        }
        #endregion Property: CountableEntity

        #region Property: Matter
        /// <summary>
        /// Gets or sets the matter of the variable, in case the VariableType has been set to 'Quantity'.
        /// </summary>
        public Matter Matter
        {
            get
            {
                return Database.Current.Select<Matter>(this.ID, ValueTables.Variable, Columns.Matter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Variable, Columns.Matter, value);
                NotifyPropertyChanged("Matter");
            }
        }
        #endregion Property: Matter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Variable()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Variable()
        {
            // Attribute, reference, entity, and matter
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Attribute, new Tuple<Type, EntryType>(typeof(Attribute), EntryType.Nullable));
            dict.Add(Columns.Reference, new Tuple<Type, EntryType>(typeof(Reference), EntryType.Nullable));
            dict.Add(Columns.CountableEntity, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Nullable));
            dict.Add(Columns.Matter, new Tuple<Type, EntryType>(typeof(Matter), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.Variable, typeof(Variable), dict);
        }
        #endregion Static Constructor: Variable()

        #region Constructor: Variable()
        /// <summary>
        /// Creates a new variable.
        /// </summary>
        protected Variable()
            : base()
        {
            Database.Current.StartChange();

            // Insert the name
            Database.Current.Insert(this.ID, LocalizationTables.VariableName);

            Database.Current.StopChange();
        }
        #endregion Constructor: Variable()

        #region Constructor: Variable(uint id)
        /// <summary>
        /// Creates a new variable with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new variable from.</param>
        protected Variable(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Variable(uint id)

        #region Constructor: Variable(Variable variable)
        /// <summary>
        /// Clones the variable.
        /// </summary>
        /// <param name="variable">The variable to clone.</param>
        protected Variable(Variable variable)
            : base()
        {
            if (variable != null)
            {
                Database.Current.StartChange();

                Database.Current.Insert(this.ID, LocalizationTables.VariableName, Columns.Name, variable.Name);
                this.VariableType = variable.VariableType;
                this.Source = variable.Source;
                this.Reference = variable.Reference;
                this.Attribute = variable.Attribute;
                this.CountType = variable.CountType;
                this.CountableEntity = variable.CountableEntity;
                this.Matter = variable.Matter;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Variable(Variable variable)

        #region Constructor: Variable(String name)
        /// <summary>
        /// Creates a new variable with the given name.
        /// </summary>
        /// <param name="name">The name to create a new variable from.</param>
        public Variable(String name)
            : this()
        {
            Database.Current.StartChange();

            this.Name = name;

            Database.Current.StopChange();
        }
        #endregion Constructor: Variable(String name)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the variable.
        /// </summary>
        /// <returns>A clone of the variable.</returns>
        public Variable Clone()
        {
            try
            {
                Type type = this.GetType();
                return type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type }, null).Invoke(new object[] { this }) as Variable;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Method: Clone()

        #region Method: Remove()
        /// <summary>
        /// Remove the variable.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the name
            Database.Current.Remove(this.ID, LocalizationTables.VariableName);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

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
    #endregion Class: Variable

    #region Class: BoolVariable
    /// <summary>
    /// A bool variable.
    /// </summary>
    public sealed class BoolVariable : Variable
    {

        #region Properties and Fields

        #region Property: FixedValue
        /// <summary>
        /// Gets or sets the fixed value of the bool variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        public bool FixedValue
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.BoolVariable, Columns.Value);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.BoolVariable, Columns.Value, value);
                NotifyPropertyChanged("FixedValue");
            }
        }
        #endregion Property: FixedValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: BoolVariable()
        /// <summary>
        /// Creates a new bool variable.
        /// </summary>
        public BoolVariable()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.BoolVariable;

            // Set the the initial fixed value
            this.FixedValue = SemanticsSettings.Values.Boolean;

            Database.Current.StopChange();
        }
        #endregion Constructor: BoolVariable()

        #region Constructor: BoolVariable(uint id)
        /// <summary>
        /// Creates a new bool variable with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new bool variable from.</param>
        private BoolVariable(uint id)
            : base(id)
        {
        }
        #endregion Constructor: BoolVariable(uint id)

        #region Constructor: BoolVariable(BoolVariable boolVariable)
        /// <summary>
        /// Clones the bool variable.
        /// </summary>
        /// <param name="boolVariable">The bool variable to clone.</param>
        public BoolVariable(BoolVariable boolVariable)
            : base(boolVariable)
        {
            if (boolVariable != null)
            {
                Database.Current.StartChange();

                this.FixedValue = boolVariable.FixedValue;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: BoolVariable(BoolVariable boolVariable)

        #region Constructor: BoolVariable(bool value)
        /// <summary>
        /// Creates a new bool variable with the given value.
        /// </summary>
        /// <param name="value">The value to assign to the bool variable.</param>
        public BoolVariable(bool value)
            : base()
        {
            Database.Current.StartChange();

            this.FixedValue = value;
            this.VariableType = VariableType.Fixed;

            Database.Current.StopChange();
        }

        /// <summary>
        /// Creates a new bool variable with the given value.
        /// </summary>
        /// <param name="value">The value to assign to the bool variable.</param>
        /// <returns>A bool variable from the given bool.</returns>
        public static implicit operator BoolVariable(bool value)
        {
            return new BoolVariable(value);
        }
        #endregion Constructor: BoolVariable(bool value)

        #region Constructor: BoolVariable(String name)
        /// <summary>
        /// Creates a new bool variable with the given name.
        /// </summary>
        /// <param name="name">The name to create a new bool variable from.</param>
        public BoolVariable(String name)
            : base(name)
        {
        }
        #endregion Constructor: BoolVariable(String name)

        #region Constructor: BoolVariable(String name, bool value)
        /// <summary>
        /// Creates a new bool variable with the given name and value.
        /// </summary>
        /// <param name="name">The name to create a new bool variable from.</param>
        /// <param name="value">The value to assign to the bool variable.</param>
        public BoolVariable(String name, bool value)
            : base(name)
        {
            Database.Current.StartChange();

            this.FixedValue = value;
            this.VariableType = VariableType.Fixed;

            Database.Current.StopChange();
        }
        #endregion Constructor: BoolVariable(String name, bool value)

        #endregion Method Group: Constructors

    }
    #endregion Class: BoolVariable

    #region Class: NumericalVariable
    /// <summary>
    /// A numerical variable.
    /// </summary>
    public sealed class NumericalVariable : Variable
    {

        #region Properties and Fields

        #region Property: FixedValue
        /// <summary>
        /// Gets or sets the fixed value of the numerical variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        public float FixedValue
        {
            get
            {
                return Database.Current.Select<float>(this.ID, ValueTables.NumericalVariable, Columns.Value);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NumericalVariable, Columns.Value, value);
                NotifyPropertyChanged("FixedValue");
            }
        }
        #endregion Property: FixedValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NumericalVariable()
        /// <summary>
        /// Creates a new numerical variable.
        /// </summary>
        public NumericalVariable()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.NumericalVariable;

            // Set the the initial fixed value
            this.FixedValue = SemanticsSettings.Values.Value;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalVariable()

        #region Constructor: NumericalVariable(uint id)
        /// <summary>
        /// Creates a new numerical variable with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new numerical variable from.</param>
        private NumericalVariable(uint id)
            : base(id)
        {
        }
        #endregion Constructor: NumericalVariable(uint id)

        #region Constructor: NumericalVariable(NumericalVariable numericalVariable)
        /// <summary>
        /// Clones the numerical variable.
        /// </summary>
        /// <param name="numericalVariable">The numerical variable to clone.</param>
        public NumericalVariable(NumericalVariable numericalVariable)
            : base(numericalVariable)
        {
            if (numericalVariable != null)
            {
                Database.Current.StartChange();

                this.FixedValue = numericalVariable.FixedValue;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: NumericalVariable(NumericalVariable numericalVariable)

        #region Constructor: NumericalVariable(float value)
        /// <summary>
        /// Creates a new numerical variable with the given value.
        /// </summary>
        /// <param name="value">The value to assign to the numerical variable.</param>
        public NumericalVariable(float value)
            : base()
        {
            Database.Current.StartChange();

            this.FixedValue = value;
            this.VariableType = VariableType.Fixed;

            Database.Current.StopChange();
        }

        /// <summary>
        /// Creates a new numerical variable with the given value.
        /// </summary>
        /// <param name="value">The value to assign to the numerical variable.</param>
        /// <returns>A numerical variable from the given float.</returns>
        public static implicit operator NumericalVariable(float value)
        {
            return new NumericalVariable(value);
        }
        #endregion Constructor: NumericalVariable(float value)

        #region Constructor: NumericalVariable(String name)
        /// <summary>
        /// Creates a new numerical variable with the given name.
        /// </summary>
        /// <param name="name">The name to create a new numerical variable from.</param>
        public NumericalVariable(String name)
            : base(name)
        {
        }
        #endregion Constructor: NumericalVariable(String name)

        #region Constructor: NumericalVariable(String name, float value)
        /// <summary>
        /// Creates a new numerical variable with the given name and value.
        /// </summary>
        /// <param name="name">The name to create a new numerical variable from.</param>
        /// <param name="value">The value to assign to the numerical variable.</param>
        public NumericalVariable(String name, float value)
            : base(name)
        {
            Database.Current.StartChange();

            this.FixedValue = value;
            this.VariableType = VariableType.Fixed;

            Database.Current.StopChange();
        }
        #endregion Constructor: NumericalVariable(String name, float value)

        #endregion Method Group: Constructors

    }
    #endregion Class: NumericalVariable

    #region Class: StringVariable
    /// <summary>
    /// A string variable.
    /// </summary>
    public sealed class StringVariable : Variable
    {

        #region Properties and Fields

        #region Property: FixedValue
        /// <summary>
        /// Gets or sets the fixed value of the string variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        public string FixedValue
        {
            get
            {
                return Database.Current.Select<string>(this.ID, ValueTables.StringVariable, Columns.Value);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.StringVariable, Columns.Value, value);
                NotifyPropertyChanged("FixedValue");
            }
        }
        #endregion Property: FixedValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StringVariable()
        /// <summary>
        /// Creates a new string variable.
        /// </summary>
        public StringVariable()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.StringVariable;

            // Set the the initial fixed value
            this.FixedValue = SemanticsSettings.Values.String;

            Database.Current.StopChange();
        }
        #endregion Constructor: StringVariable()

        #region Constructor: StringVariable(uint id)
        /// <summary>
        /// Creates a new string variable with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new string variable from.</param>
        private StringVariable(uint id)
            : base(id)
        {
        }
        #endregion Constructor: StringVariable(uint id)

        #region Constructor: StringVariable(StringVariable stringVariable)
        /// <summary>
        /// Clones the string variable.
        /// </summary>
        /// <param name="stringVariable">The string variable to clone.</param>
        public StringVariable(StringVariable stringVariable)
            : base(stringVariable)
        {
            if (stringVariable != null)
            {
                Database.Current.StartChange();

                if (stringVariable.FixedValue != null)
                    this.FixedValue = stringVariable.FixedValue;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: StringVariable(StringVariable stringVariable)

        #region Constructor: StringVariable(string value)
        /// <summary>
        /// Creates a new string variable with the given value.
        /// </summary>
        /// <param name="value">The value to assign to the string variable.</param>
        public StringVariable(string value)
            : base()
        {
            Database.Current.StartChange();

            this.FixedValue = value;
            this.VariableType = VariableType.Fixed;

            Database.Current.StopChange();
        }

        /// <summary>
        /// Creates a new string variable with the given value.
        /// </summary>
        /// <param name="value">The value to assign to the string variable.</param>
        /// <returns>A string variable from the given string.</returns>
        public static implicit operator StringVariable(string value)
        {
            return new StringVariable(value);
        }
        #endregion Constructor: StringVariable(string value)

        #region Constructor: StringVariable(String name, string value)
        /// <summary>
        /// Creates a new string variable with the given name and value.
        /// </summary>
        /// <param name="name">The name to create a new string variable from.</param>
        /// <param name="value">The value to assign to the string variable.</param>
        public StringVariable(String name, string value)
            : base(name)
        {
            Database.Current.StartChange();

            this.FixedValue = value;
            this.VariableType = VariableType.Fixed;

            Database.Current.StopChange();
        }
        #endregion Constructor: StringVariable(String name, string value)

        #endregion Method Group: Constructors

    }
    #endregion Class: StringVariable

    #region Class: VectorVariable
    /// <summary>
    /// A vector variable.
    /// </summary>
    public sealed class VectorVariable : Variable
    {

        #region Properties and Fields

        #region Property: FixedValue
        /// <summary>
        /// A handler for a change in the vector.
        /// </summary>
        private Vec4.Vec4Handler vectorChanged;
        
        /// <summary>
        /// The vector value.
        /// </summary>
        private Vec4 vector = null;
        
        /// <summary>
        /// Gets or sets the fixed value of the vector variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        public Vec4 FixedValue
        {
            get
            {
                if (vector == null)
                {
                    vector = Database.Current.Select<Vec4>(this.ID, ValueTables.VectorVariable, Columns.Value);

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

                Database.Current.Update(this.ID, ValueTables.VectorVariable, Columns.Value, value);

                if (vector != null)
                    vector.ValueChanged += vectorChanged;

                NotifyPropertyChanged("FixedValue");
            }
        }

        /// <summary>
        /// Updates the database when a value of the vector changes.
        /// </summary>
        private void vector_ValueChanged(Vec4 vec)
        {
            this.FixedValue = vec;
        }
        #endregion Property: FixedValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: VectorVariable()
        /// <summary>
        /// Creates a new vector variable.
        /// </summary>
        public VectorVariable()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.VectorVariable;

            // Set the the initial fixed value
            this.FixedValue = new Vec4(SemanticsSettings.Values.Vector4X, SemanticsSettings.Values.Vector4Y, SemanticsSettings.Values.Vector4Z, SemanticsSettings.Values.Vector4W);

            Database.Current.StopChange();
        }
        #endregion Constructor: VectorVariable()

        #region Constructor: VectorVariable(uint id)
        /// <summary>
        /// Creates a new vector variable with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new vector variable from.</param>
        private VectorVariable(uint id)
            : base(id)
        {
        }
        #endregion Constructor: VectorVariable(uint id)

        #region Constructor: VectorVariable(VectorVariable vectorVariable)
        /// <summary>
        /// Clones the vector variable.
        /// </summary>
        /// <param name="vectorVariable">The vector variable to clone.</param>
        public VectorVariable(VectorVariable vectorVariable)
            : base(vectorVariable)
        {
            if (vectorVariable != null)
            {
                Database.Current.StartChange();

                if (vectorVariable.FixedValue != null)
                    this.FixedValue = new Vec4(vectorVariable.FixedValue);

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: VectorVariable(VectorVariable vectorVariable)

        #region Constructor: VectorVariable(Vec4 value)
        /// <summary>
        /// Creates a new vector variable with the given value.
        /// </summary>
        /// <param name="value">The value to assign to the vector variable.</param>
        public VectorVariable(Vec4 value)
            : base()
        {
            Database.Current.StartChange();

            this.FixedValue = value;

            Database.Current.StopChange();
        }

        /// <summary>
        /// Creates a new vector variable with the given value.
        /// </summary>
        /// <param name="value">The value to assign to the vector variable.</param>
        /// <returns>A vector variable from the given Vec4.</returns>
        public static implicit operator VectorVariable(Vec4 value)
        {
            return new VectorVariable(value);
        }
        #endregion Constructor: VectorVariable(Vec4 value)

        #region Constructor: VectorVariable(String name)
        /// <summary>
        /// Creates a new vector variable with the given name.
        /// </summary>
        /// <param name="name">The name to create a new vector variable from.</param>
        public VectorVariable(String name)
            : base(name)
        {
        }
        #endregion Constructor: VectorVariable(String name)

        #region Constructor: VectorVariable(String name, Vec4 value)
        /// <summary>
        /// Creates a new vector variable with the given name and value.
        /// </summary>
        /// <param name="name">The name to create a new vector variable from.</param>
        /// <param name="value">The value to assign to the vector variable.</param>
        public VectorVariable(String name, Vec4 value)
            : base(name)
        {
            Database.Current.StartChange();

            this.FixedValue = value;

            Database.Current.StopChange();
        }
        #endregion Constructor: VectorVariable(String name, Vec4 value)

        #endregion Method Group: Constructors

    }
    #endregion Class: VectorVariable

    #region Class: RandomVariable
    /// <summary>
    /// A random variable.
    /// </summary>
    public sealed class RandomVariable : Variable
    {

        #region Properties and Fields

        #region Property: FixedValue
        /// <summary>
        /// Gets or sets the fixed value of the random variable, in case the VariableType has been set to 'Fixed'.
        /// </summary>
        public float FixedValue
        {
            get
            {
                return Database.Current.Select<float>(this.ID, ValueTables.RandomVariable, Columns.Value);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RandomVariable, Columns.Value, value);
                NotifyPropertyChanged("FixedValue");
            }
        }
        #endregion Property: FixedValue

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: RandomVariable()
        /// <summary>
        /// Creates a new random variable.
        /// </summary>
        public RandomVariable()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.RandomVariable;

            // Set the the initial fixed value
            this.FixedValue = SemanticsSettings.Values.Chance;

            Database.Current.StopChange();
        }
        #endregion Constructor: RandomVariable()

        #region Constructor: RandomVariable(uint id)
        /// <summary>
        /// Creates a new random variable with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new random variable from.</param>
        private RandomVariable(uint id)
            : base(id)
        {
        }
        #endregion Constructor: RandomVariable(uint id)

        #region Constructor: RandomVariable(RandomVariable randomVariable)
        /// <summary>
        /// Clones the random variable.
        /// </summary>
        /// <param name="randomVariable">The random variable to clone.</param>
        public RandomVariable(RandomVariable randomVariable)
            : base(randomVariable)
        {
            if (randomVariable != null)
            {
                Database.Current.StartChange();

                this.FixedValue = randomVariable.FixedValue;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: RandomVariable(RandomVariable randomVariable)

        #region Constructor: RandomVariable(float value)
        /// <summary>
        /// Creates a new random variable with the given value.
        /// </summary>
        /// <param name="value">The value to assign to the random variable.</param>
        public RandomVariable(float value)
            : base()
        {
            Database.Current.StartChange();

            this.FixedValue = value;
            this.VariableType = VariableType.Fixed;

            Database.Current.StopChange();
        }

        /// <summary>
        /// Creates a new random variable with the given value.
        /// </summary>
        /// <param name="value">The value to assign to the random variable.</param>
        /// <returns>A random variable from the given float.</returns>
        public static implicit operator RandomVariable(float value)
        {
            return new RandomVariable(value);
        }
        #endregion Constructor: RandomVariable(float value)

        #region Constructor: RandomVariable(String name)
        /// <summary>
        /// Creates a new random variable with the given name.
        /// </summary>
        /// <param name="name">The name to create a new random variable from.</param>
        public RandomVariable(String name)
            : base(name)
        {
        }
        #endregion Constructor: RandomVariable(String name)

        #region Constructor: RandomVariable(String name, float value)
        /// <summary>
        /// Creates a new random variable with the given name and value.
        /// </summary>
        /// <param name="name">The name to create a new random variable from.</param>
        /// <param name="value">The value to assign to the random variable.</param>
        public RandomVariable(String name, float value)
            : base(name)
        {
            Database.Current.StartChange();

            this.FixedValue = value;
            this.VariableType = VariableType.Fixed;

            Database.Current.StopChange();
        }
        #endregion Constructor: RandomVariable(String name, float value)

        #endregion Method Group: Constructors

    }
    #endregion Class: RandomVariable

    #region Class: TermVariable
    /// <summary>
    /// A term variable.
    /// </summary>
    public sealed class TermVariable : Variable
    {

        #region Properties and Fields

        #region Property: Function1
        /// <summary>
        /// Gets or sets the (optional) function on the first variable.
        /// </summary>
        public Function? Function1
        {
            get
            {
                return Database.Current.Select<Function?>(this.ID, ValueTables.TermVariable, Columns.Function1);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TermVariable, Columns.Function1, value);
                NotifyPropertyChanged("Function1");
            }
        }
        #endregion Property: Function1

        #region Property: Variable1
        /// <summary>
        /// Gets or sets the (required) first variable. Only works for NumericalVariable and TermVariable!
        /// </summary>
        public Variable Variable1
        {
            get
            {
                return Database.Current.Select<Variable>(this.ID, ValueTables.TermVariable, Columns.Variable1);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TermVariable, Columns.Variable1, value);
                NotifyPropertyChanged("Variable1");
            }
        }
        #endregion Property: Variable1

        #region Property: Value1
        /// <summary>
        /// Gets or sets the first value of the term variable, in case the first variable should be a fixed value instead.
        /// </summary>
        public float? Value1
        {
            get
            {
                return Database.Current.Select<float?>(this.ID, ValueTables.TermVariable, Columns.Value1);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TermVariable, Columns.Value1, value);
                NotifyPropertyChanged("Value1");
            }
        }
        #endregion Property: Value1

        #region Property: Operator
        /// <summary>
        /// Gets or sets the (optional) operator between both variables. Only works when Variable2 or Value2 are set!
        /// </summary>
        public Operator? Operator
        {
            get
            {
                return Database.Current.Select<Operator?>(this.ID, ValueTables.TermVariable, Columns.Operator);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TermVariable, Columns.Operator, value);
                NotifyPropertyChanged("Operator");
            }
        }
        #endregion Property: Operator

        #region Property: Function2
        /// <summary>
        /// Gets or sets the (optional) function on the second variable/value.
        /// </summary>
        public Function? Function2
        {
            get
            {
                return Database.Current.Select<Function?>(this.ID, ValueTables.TermVariable, Columns.Function2);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TermVariable, Columns.Function2, value);
                NotifyPropertyChanged("Function2");
            }
        }
        #endregion Property: Function2

        #region Property: Variable2
        /// <summary>
        /// Gets or sets the (optional) second variable.  Only works for NumericalVariable and TermVariable and when an Operator has been set!
        /// </summary>
        public Variable Variable2
        {
            get
            {
                return Database.Current.Select<Variable>(this.ID, ValueTables.TermVariable, Columns.Variable2);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TermVariable, Columns.Variable2, value);
                NotifyPropertyChanged("Variable2");
            }
        }
        #endregion Property: Variable2

        #region Property: Value2
        /// <summary>
        /// Gets or sets the second value of the term variable, in case the second variable should be a fixed value instead. Only works when an Operator has been set!
        /// </summary>
        public float? Value2
        {
            get
            {
                return Database.Current.Select<float?>(this.ID, ValueTables.TermVariable, Columns.Value2);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TermVariable, Columns.Value2, value);
                NotifyPropertyChanged("Value2");
            }
        }
        #endregion Property: Value2

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TermVariable()
        /// <summary>
        /// Creates a new term variable.
        /// </summary>
        public TermVariable()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.TermVariable;

            Database.Current.StopChange();
        }
        #endregion Constructor: TermVariable()

        #region Constructor: TermVariable(uint id)
        /// <summary>
        /// Creates a new term variable with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new term variable from.</param>
        private TermVariable(uint id)
            : base(id)
        {
        }
        #endregion Constructor: TermVariable(uint id)

        #region Constructor: TermVariable(TermVariable termVariable)
        /// <summary>
        /// Clones the term variable.
        /// </summary>
        /// <param name="termVariable">The term variable to clone.</param>
        public TermVariable(TermVariable termVariable)
            : base(termVariable)
        {
            if (termVariable != null)
            {
                Database.Current.StartChange();

                this.Function1 = termVariable.Function1;
                if (termVariable.Variable1 != null)
                    this.Variable1 = termVariable.Variable1.Clone();
                this.Value1 = termVariable.Value1;
                this.Operator = termVariable.Operator;
                this.Function2 = termVariable.Function2;
                if (termVariable.Variable2 != null)
                    this.Variable2 = termVariable.Variable2.Clone();
                this.Value2 = termVariable.Value2;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: TermVariable(TermVariable termVariable)

        #region Constructor: TermVariable(String name)
        /// <summary>
        /// Creates a new term variable with the given name.
        /// </summary>
        /// <param name="name">The name to create a new term variable from.</param>
        public TermVariable(String name)
            : base(name)
        {
        }
        #endregion Constructor: TermVariable(String name)

        #endregion Method Group: Constructors

    }
    #endregion Class: TermVariable

}