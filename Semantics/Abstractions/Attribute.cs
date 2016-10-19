/**************************************************************************
 * 
 * Attribute.cs
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
using System.Collections.Generic;
using System.ComponentModel;
using Common;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;
using ValueType = Semantics.Utilities.ValueType;

namespace Semantics.Abstractions
{

    #region Class: Attribute
    /// <summary>
    /// An attribute component, having knowledge about the units or states in which its value can be expressed.
    /// </summary>
    public class Attribute : Abstraction, IComparable<Attribute>
    {

        #region Properties and Fields

        #region Property: AttributeType
        /// <summary>
        /// Gets or sets the type of the attribute.
        /// </summary>
        public AttributeType AttributeType
        {
            get
            {
                return Database.Current.Select<AttributeType>(this.ID, GenericTables.Attribute, Columns.AttributeType);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Attribute, Columns.AttributeType, value);
                NotifyPropertyChanged("AttributeType");
            }
        }
        #endregion Property: AttributeType

        #region Property: ValueType
        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        public ValueType ValueType
        {
            get
            {
                return Database.Current.Select<ValueType>(this.ID, GenericTables.Attribute, Columns.ValueType);
            }
            set
            {
                if (value != this.ValueType)
                {
                    Database.Current.Update(this.ID, GenericTables.Attribute, Columns.ValueType, value);
                    NotifyPropertyChanged("ValueType");

                    if (value != ValueType.Numerical)
                        this.UnitCategory = null;
                }
            }
        }
        #endregion Property: ValueType
		
        #region Property: UnitCategory
        /// <summary>
        /// Gets or sets the unit category of the attribute.
        /// </summary>
        public UnitCategory UnitCategory
        {
            get
            {
                return Database.Current.Select<UnitCategory>(this.ID, GenericTables.Attribute, Columns.UnitCategory);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Attribute, Columns.UnitCategory, value);
                NotifyPropertyChanged("UnitCategory");
            }
        }
        #endregion Property: UnitCategory
		
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Attribute()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Attribute()
        {
            // Unit category
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.UnitCategory, new Tuple<Type, EntryType>(typeof(UnitCategory), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.Attribute, typeof(Attribute), dict);
        }
        #endregion Static Constructor: Attribute()

        #region Constructor: Attribute()
        /// <summary>
        /// Creates a new attribute.
        /// </summary>
        public Attribute()
            : base()
        {
            this.AttributeType = default(AttributeType);
            this.ValueType = default(ValueType);
        }
        #endregion Constructor: Attribute()

        #region Constructor: Attribute(uint id)
        /// <summary>
        /// Creates a new attribute with the given ID.
        /// </summary>
        /// <param name="id">The ID to create the attribute from.</param>
        protected Attribute(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Attribute(uint id)

        #region Constructor: Attribute(string name)
        /// <summary>
        /// Creates a new attribute with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the attribute.</param>
        public Attribute(string name)
            : base(name)
        {
            Database.Current.StartChange();

            this.AttributeType = default(AttributeType);
            this.ValueType = default(ValueType);

            Database.Current.StopChange();
        }
        #endregion Constructor: Attribute(string name)

        #region Constructor: Attribute(string name, ValueType valueType)
        /// <summary>
        /// Creates a new attribute with the given name and value type.
        /// </summary>
        /// <param name="name">The name to assign to the attribute.</param>
        /// <param name="valueType">The type of the attribute's value.</param>
        public Attribute(string name, ValueType valueType)
            : base(name)
        {
            Database.Current.StartChange();

            this.AttributeType = default(AttributeType);
            this.ValueType = valueType;

            Database.Current.StopChange();
        }
        #endregion Constructor: Attribute(string name, ValueType valueType)

        #region Constructor: Attribute(Attribute attribute)
        /// <summary>
        /// Clones an attribute.
        /// </summary>
        /// <param name="attribute">The attribute to clone.</param>
        public Attribute(Attribute attribute)
            : base(attribute)
        {
            if (attribute != null)
            {
                Database.Current.StartChange();

                this.AttributeType = attribute.AttributeType;
                this.ValueType = attribute.ValueType;
                this.UnitCategory = attribute.UnitCategory;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Attribute(Attribute attribute)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the attribute.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Attribute other)
        /// <summary>
        /// Compares the attribute to the other attribute.
        /// </summary>
        /// <param name="other">The attribute to compare to this attribute.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Attribute other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Attribute other)

        #endregion Method Group: Other

    }
    #endregion Class: Attribute

    #region Class: AttributeValued
    /// <summary>
    /// A valued version of an attribute.
    /// </summary>
    public class AttributeValued : AbstractionValued
    {

        #region Properties and Fields

        #region Property: Attribute
        /// <summary>
        /// Gets the attribute of which this is an valued attribute.
        /// </summary>
        public Attribute Attribute
        {
            get
            {
                return this.Node as Attribute;
            }
            protected set
            {
                this.Node = value;
            }
        }

        /// <summary>
        /// Handles the change of a property change.
        /// </summary>
        private void attribute_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("ValueType"))
            {
                // Remove the old value
                this.Value.Remove();

                // Create a value of the new type
                this.Value = Value.Create(this.Attribute.ValueType);
            }
            else if (e.PropertyName.Equals("UnitCategory"))
            {
                NumericalValue numericalValue = this.Value as NumericalValue;
                if (numericalValue != null)
                    numericalValue.UnitCategory = this.Attribute.UnitCategory;
            }
        }
        #endregion Property: Attribute

        #region Property: Value
        /// <summary>
        /// Gets the value of the attribute.
        /// </summary>
        public Value Value
        {
            get
            {
                return Database.Current.Select<Value>(this.ID, ValueTables.AttributeValued, Columns.Value);
            }
            private set
            {
                Database.Current.Update(this.ID, ValueTables.AttributeValued, Columns.Value, value);
                NotifyPropertyChanged("Value");
            }
        }
        #endregion Property: Value

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AttributeValued(uint id)
        /// <summary>
        /// Creates a new valued attribute from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an valued attribute from.</param>
        protected AttributeValued(uint id)
            : base(id)
        {
            Init();
        }
        #endregion Constructor: AttributeValued(uint id)

        #region Constructor: AttributeValued(AttributeValued attributeValued)
        /// <summary>
        /// Clones the valued attribute.
        /// </summary>
        /// <param name="attributeValued">The valued attribute to clone.</param>
        public AttributeValued(AttributeValued attributeValued)
            : base(attributeValued)
        {
            if (attributeValued != null)
            {
                Database.Current.StartChange();

                if (attributeValued.Value != null)
                    this.Value = attributeValued.Value.Clone();

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: AttributeValued(AttributeValued attributeValued)

        #region Constructor: AttributeValued(Attribute attribute)
        /// <summary>
        /// Create a valued attribute from the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to create a valued attribute from.</param>
        public AttributeValued(Attribute attribute)
            : base(attribute)
        {
            Database.Current.StartChange();

            // Set the value, based on the attribute's value type
            ValueType valueType = default(ValueType);
            if (attribute != null)
                valueType = attribute.ValueType;
            this.Value = Value.Create(valueType);

            Init();

            Database.Current.StopChange();
        }
        #endregion Constructor: AttributeValued(Attribute attribute)

        #region Method: Init()
        /// <summary>
        /// Initializes the valued attribute.
        /// </summary>
        private void Init()
        {
            if (this.Attribute != null)
            {
                // Subscribe to changes
                this.Attribute.PropertyChanged += new PropertyChangedEventHandler(attribute_PropertyChanged);

                // Set the unit category
                NumericalValue numericalValue = this.Value as NumericalValue;
                if (numericalValue != null && this.Attribute.UnitCategory != null)
                    numericalValue.UnitCategory = this.Attribute.UnitCategory;
            }
        }
        #endregion Method: Init()

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            if (this.Node != null)
            {
                string returnString = this.Node.DefaultName + " {";
                returnString += this.Value.ToString() + "}";
                return returnString;
            }
            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: AttributeValued

    #region Class: AttributeCondition
    /// <summary>
    /// A condition on an attribute.
    /// </summary>
    public class AttributeCondition : AbstractionCondition
    {

        #region Properties and Fields

        #region Property: Attribute
        /// <summary>
        /// Gets or sets the attribute of which this is an attribute condition.
        /// </summary>
        public Attribute Attribute
        {
            get
            {
                return this.Node as Attribute;
            }
            set
            {
                if (attributeChangedHandler == null)
                    this.attributeChangedHandler = new PropertyChangedEventHandler(attribute_PropertyChanged);

                if (this.Attribute != null)
                    this.Attribute.PropertyChanged -= this.attributeChangedHandler;

                this.Node = value;

                if (value != null)
                {
                    value.PropertyChanged += this.attributeChangedHandler;

                    NumericalValueCondition numericalValueCondition = this.Value as NumericalValueCondition;
                    if (numericalValueCondition != null)
                        numericalValueCondition.UnitCategory = this.Attribute.UnitCategory;
                }
            }
        }

        /// <summary>
        /// A handler for a changed attribute.
        /// </summary>
        private PropertyChangedEventHandler attributeChangedHandler = null;

        /// <summary>
        /// Handles the change of a property change.
        /// </summary>
        private void attribute_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("UnitCategory"))
            {
                NumericalValueCondition numericalValueCondition = this.Value as NumericalValueCondition;
                if (numericalValueCondition != null)
                    numericalValueCondition.UnitCategory = this.Attribute.UnitCategory;
            }
        }
        #endregion Property: Attribute

        #region Property: Value
        /// <summary>
        /// Gets or sets the required value.
        /// </summary>
        public ValueCondition Value
        {
            get
            {
                return Database.Current.Select<ValueCondition>(this.ID, ValueTables.AttributeCondition, Columns.Value);
            }
            set
            {
                if (this.Value != null)
                    this.Value.Remove();

                Database.Current.Update(this.ID, ValueTables.AttributeCondition, Columns.Value, value);
                NotifyPropertyChanged("Value");

                if (value != null)
                {
                    NumericalValueCondition numericalValueCondition = value as NumericalValueCondition;
                    if (numericalValueCondition != null && this.Attribute != null)
                        numericalValueCondition.UnitCategory = this.Attribute.UnitCategory;
                }
            }
        }
        #endregion Property: Value

        #region Property: UnitCategory
        /// <summary>
        /// Gets or sets the required unit category.
        /// </summary>
        public UnitCategory UnitCategory
        {
            get
            {
                return Database.Current.Select<UnitCategory>(this.ID, ValueTables.AttributeCondition, Columns.UnitCategory);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.AttributeCondition, Columns.UnitCategory, value);
                NotifyPropertyChanged("UnitCategory");
            }
        }
        #endregion Property: UnitCategory

        #region Property: UnitCategorySign
        /// <summary>
        /// Gets or sets the sign for the unit category in the condition.
        /// </summary>
        public EqualitySign? UnitCategorySign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.AttributeCondition, Columns.UnitCategorySign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.AttributeCondition, Columns.UnitCategorySign, value);
                NotifyPropertyChanged("UnitCategorySign");
            }
        }
        #endregion Property: UnitCategorySign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: AttributeCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static AttributeCondition()
        {
            // Unit category
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.UnitCategory, new Tuple<Type, EntryType>(typeof(UnitCategory), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.AttributeCondition, typeof(AttributeCondition), dict);
        }
        #endregion Static Constructor: AttributeCondition()

        #region Constructor: AttributeCondition()
        /// <summary>
        /// Creates a new attribute condition.
        /// </summary>
        public AttributeCondition()
            : base()
        {
        }
        #endregion Constructor: AttributeCondition()

        #region Constructor: AttributeCondition(uint id)
        /// <summary>
        /// Creates a new attribute condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an attribute condition from.</param>
        protected AttributeCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AttributeCondition(uint id)

        #region Constructor: AttributeCondition(AttributeCondition attributeCondition)
        /// <summary>
        /// Clones an attribute condition.
        /// </summary>
        /// <param name="attributeCondition">The attribute condition to clone.</param>
        public AttributeCondition(AttributeCondition attributeCondition)
            : base(attributeCondition)
        {
            if (attributeCondition != null)
            {
                Database.Current.StartChange();

                if (attributeCondition.Value != null)
                    this.Value = attributeCondition.Value.Clone();
                this.UnitCategory = attributeCondition.UnitCategory;
                this.UnitCategorySign = attributeCondition.UnitCategorySign;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: AttributeCondition(AttributeCondition attributeCondition)

        #region Constructor: AttributeCondition(Attribute attribute)
        /// <summary>
        /// Creates a condition for the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to create a condition for.</param>
        public AttributeCondition(Attribute attribute)
            : base(attribute)
        {
            if (attribute != null)
            {
                Database.Current.StartChange();

                this.UnitCategory = attribute.UnitCategory;

                // Subscribe to changes
                this.attributeChangedHandler = new PropertyChangedEventHandler(attribute_PropertyChanged);
                attribute.PropertyChanged += this.attributeChangedHandler;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: AttributeCondition(Attribute attribute)

        #endregion Method Group: Constructors

    }
    #endregion Class: AttributeCondition

    #region Class: AttributeChange
    /// <summary>
    /// A change on an attribute.
    /// </summary>
    public class AttributeChange : AbstractionChange
    {

        #region Properties and Fields

        #region Property: Attribute
        /// <summary>
        /// Gets or sets the affected attribute.
        /// </summary>
        public Attribute Attribute
        {
            get
            {
                return this.Node as Attribute;
            }
            set
            {
                if (attributeChangedHandler == null)
                    this.attributeChangedHandler = new PropertyChangedEventHandler(attribute_PropertyChanged);

                if (this.Attribute != null)
                    this.Attribute.PropertyChanged -= this.attributeChangedHandler;

                this.Node = value;

                if (value != null)
                {
                    value.PropertyChanged += this.attributeChangedHandler;

                    NumericalValueChange numericalValueChange = this.Value as NumericalValueChange;
                    if (numericalValueChange != null)
                        numericalValueChange.UnitCategory = value.UnitCategory;
                }
            }
        }

        /// <summary>
        /// A handler for a changed attribute.
        /// </summary>
        private PropertyChangedEventHandler attributeChangedHandler = null;

        /// <summary>
        /// Handles the change of a property change.
        /// </summary>
        private void attribute_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("UnitCategory"))
            {
                NumericalValueChange numericalValueChange = this.Value as NumericalValueChange;
                if (numericalValueChange != null)
                    numericalValueChange.UnitCategory = this.Attribute.UnitCategory;
            }
        }
        #endregion Property: Attribute

        #region Property: Value
        /// <summary>
        /// Gets or sets the value to change to.
        /// </summary>
        public ValueChange Value
        {
            get
            {
                return Database.Current.Select<ValueChange>(this.ID, ValueTables.AttributeChange, Columns.Value);
            }
            set
            {
                if (this.Value != null)
                    this.Value.Remove();

                Database.Current.Update(this.ID, ValueTables.AttributeChange, Columns.Value, value);
                NotifyPropertyChanged("Value");

                if (value != null)
                {
                    NumericalValueChange numericalValueChange = value as NumericalValueChange;
                    if (numericalValueChange != null && this.Attribute != null)
                        numericalValueChange.UnitCategory = this.Attribute.UnitCategory;
                }
            }
        }
        #endregion Property: Value

        #region Property: UnitCategory
        /// <summary>
        /// Gets or sets the unit category to change to.
        /// </summary>
        public UnitCategory UnitCategory
        {
            get
            {
                return Database.Current.Select<UnitCategory>(this.ID, ValueTables.AttributeChange, Columns.UnitCategory);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.AttributeChange, Columns.UnitCategory, value);
                NotifyPropertyChanged("UnitCategory");
            }
        }
        #endregion Property: UnitCategory

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: AttributeChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static AttributeChange()
        {
            // Unit category
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.UnitCategory, new Tuple<Type, EntryType>(typeof(UnitCategory), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.AttributeChange, typeof(AttributeChange), dict);
        }
        #endregion Static Constructor: AttributeChange()

        #region Constructor: AttributeChange()
        /// <summary>
        /// Creates a new attribute change.
        /// </summary>
        public AttributeChange()
            : base()
        {
        }
        #endregion Constructor: AttributeChange()

        #region Constructor: AttributeChange(uint id)
        /// <summary>
        /// Creates a new attribute change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an attribute change from.</param>
        protected AttributeChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AttributeChange(uint id)

        #region Constructor: AttributeChange(AttributeChange attributeChange)
        /// <summary>
        /// Clones an attribute change.
        /// </summary>
        /// <param name="attributeChange">The attribute change to clone.</param>
        public AttributeChange(AttributeChange attributeChange)
            : base(attributeChange)
        {
            if (attributeChange != null)
            {
                Database.Current.StartChange();

                if (attributeChange.Value != null)
                    this.Value = attributeChange.Value.Clone();
                this.UnitCategory = attributeChange.UnitCategory;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: AttributeChange(AttributeChange attributeChange)

        #region Constructor: AttributeChange(Attribute attribute)
        /// <summary>
        /// Creates a change for the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to create a change for.</param>
        public AttributeChange(Attribute attribute)
            : base(attribute)
        {
            if (attribute != null)
            {
                Database.Current.StartChange();

                this.UnitCategory = attribute.UnitCategory;

                // Subscribe to changes
                this.attributeChangedHandler = new PropertyChangedEventHandler(attribute_PropertyChanged);
                attribute.PropertyChanged += this.attributeChangedHandler;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: AttributeChange(Attribute attribute)

        #region Constructor: AttributeChange(Attribute attribute, ValueChange valueChange)
        /// <summary>
        /// Creates a change for the given attribute in the form of the given value change.
        /// </summary>
        /// <param name="attribute">The attribute to create a change for.</param>
        /// <param name="attribute">The value change for the attribute.</param>
        public AttributeChange(Attribute attribute, ValueChange valueChange)
            : this(attribute)
        {
            if (attribute != null && valueChange != null)
            {
                Database.Current.StartChange();

                this.Value = valueChange;
                this.UnitCategory = attribute.UnitCategory;

                // Subscribe to changes
                this.attributeChangedHandler = new PropertyChangedEventHandler(attribute_PropertyChanged);
                attribute.PropertyChanged += this.attributeChangedHandler;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: AttributeChange(Attribute attribute, ValueChange valueChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: AttributeChange

}
