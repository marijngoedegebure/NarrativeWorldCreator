/**************************************************************************
 * 
 * Matter.cs
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
using System.Collections.ObjectModel;
using Common;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Entities
{

    #region Class: Matter
    /// <summary>
    /// The matter component indicates of what a physical object consists.
    /// </summary>
    public abstract class Matter : PhysicalEntity, IComparable<Matter>
    {

        #region Properties and Fields

        #region Property: DefaultStateOfMatter
        /// <summary>
        /// Gets or sets the default state of matter.
        /// </summary>
        public StateOfMatter DefaultStateOfMatter
        {
            get
            {
                return Database.Current.Select<StateOfMatter>(this.ID, GenericTables.Matter, Columns.DefaultStateOfMatter);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Matter, Columns.DefaultStateOfMatter, value);
                NotifyPropertyChanged("DefaultStateOfMatter");
            }
        }
        #endregion Property: DefaultStateOfMatter

        #region Property: Elements
        /// <summary>
        /// Gets all elements of the matter.
        /// </summary>
        public ReadOnlyCollection<ElementValued> Elements
        {
            get
            {
                List<ElementValued> elements = new List<ElementValued>();
                elements.AddRange(this.PersonalElements);
                elements.AddRange(this.InheritedElements);
                elements.AddRange(this.OverriddenElements);
                return elements.AsReadOnly();
            }
        }
        #endregion Property: Elements

        #region Property: PersonalElements
        /// <summary>
        /// Gets the personal elements of the matter.
        /// </summary>
        public ReadOnlyCollection<ElementValued> PersonalElements
        {
            get
            {
                return Database.Current.SelectAll<ElementValued>(this.ID, GenericTables.MatterElement, Columns.ElementValued).AsReadOnly();
            }
        }
        #endregion Property: PersonalElements

        #region Property: InheritedElements
        /// <summary>
        /// Gets the inherited elements of the matter.
        /// </summary>
        public ReadOnlyCollection<ElementValued> InheritedElements
        {
            get
            {
                List<ElementValued> inheritedElements = new List<ElementValued>();
                foreach (Node parent in this.PersonalParents)
                {
                    foreach (ElementValued inheritedElement in ((Matter)parent).Elements)
                    {
                        if (!HasOverriddenElement(inheritedElement.Element))
                            inheritedElements.Add(inheritedElement);
                    }
                }
                return inheritedElements.AsReadOnly();
            }
        }
        #endregion Property: InheritedElements

        #region Property: OverriddenElements
        /// <summary>
        /// Gets the overridden elements.
        /// </summary>
        public ReadOnlyCollection<ElementValued> OverriddenElements
        {
            get
            {
                return Database.Current.SelectAll<ElementValued>(this.ID, GenericTables.MatterOverriddenElement, Columns.ElementValued).AsReadOnly();
            }
        }
        #endregion Property: OverriddenElements

        #region Property: ChemicalFormula
        /// <summary>
        /// Gets or sets the chemical formula.
        /// </summary>
        public String ChemicalFormula
        {
            get
            {
                return Database.Current.Select<String>(this.ID, GenericTables.Matter, Columns.Formula);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Matter, Columns.Formula, value);
                NotifyPropertyChanged("ChemicalFormula");
            }
        }
        #endregion Property: ChemicalFormula

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Matter()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Matter()
        {
            // Elements
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ElementValued, new Tuple<Type, EntryType>(typeof(ElementValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.MatterElement, typeof(Matter), dict);

            // Overridden elements
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ElementValued, new Tuple<Type, EntryType>(typeof(ElementValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.MatterOverriddenElement, typeof(Matter), dict);
        }
        #endregion Static Constructor: Matter()

        #region Constructor: Matter()
        /// <summary>
        /// Creates new matter.
        /// </summary>
        protected Matter()
            : base()
        {
        }
        #endregion Constructor: Matter()

        #region Constructor: Matter(uint id)
        /// <summary>
        /// Creates new matter from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the matter from.</param>
        protected Matter(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Matter(uint id)

        #region Constructor: Matter(string name)
        /// <summary>
        /// Creates a new matter with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the matter.</param>
        protected Matter(string name)
            : base(name)
        {
        }
        #endregion Constructor: Matter(string name)

        #region Constructor: Matter(Matter matter)
        /// <summary>
        /// Clones matter.
        /// </summary>
        /// <param name="material">The matter to clone.</param>
        protected Matter(Matter matter)
            : base(matter)
        {
            if (matter != null)
            {
                Database.Current.StartChange();

                this.DefaultStateOfMatter = matter.DefaultStateOfMatter;
                foreach (ElementValued elementValued in matter.PersonalElements)
                    AddElement(new ElementValued(elementValued));
                foreach (ElementValued elementValued in matter.OverriddenElements)
                    AddOverriddenElement(new ElementValued(elementValued));
                this.ChemicalFormula = matter.ChemicalFormula;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Matter(Matter matter)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddElement(ElementValued elementValued)
        /// <summary>
        /// Adds a valued element.
        /// </summary>
        /// <param name="elementValued">The valued element to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddElement(ElementValued elementValued)
        {
            if (elementValued != null && elementValued.Element != null)
            {
                // If the valued element is already available in all elements, there is no use to add it
                if (HasElement(elementValued.Element))
                    return Message.RelationExistsAlready;

                // Add the valued element
                Database.Current.Insert(this.ID, GenericTables.MatterElement, new string[] { Columns.Element, Columns.ElementValued }, new object[] { elementValued.Element, elementValued });
                NotifyPropertyChanged("PersonalElements");
                NotifyPropertyChanged("Elements");

                // Add the symbol of the element to the chemical formula
                this.ChemicalFormula += elementValued.Element.Symbol;

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddElement(ElementValued elementValued)

        #region Method: RemoveElement(ElementValued elementValued)
        /// <summary>
        /// Removes a valued element.
        /// </summary>
        /// <param name="elementValued">The valued element to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveElement(ElementValued elementValued)
        {
            if (elementValued != null)
            {
                if (this.Elements.Contains(elementValued))
                {
                    // Remove the symbol of the element of the chemical element
                    int index = this.ChemicalFormula.IndexOf(elementValued.Element.Symbol, StringComparison.Ordinal);
                    this.ChemicalFormula = this.ChemicalFormula.Remove(index, elementValued.Element.Symbol.Length);

                    // Remove the valued element
                    Database.Current.Remove(this.ID, GenericTables.MatterElement, Columns.ElementValued, elementValued);
                    NotifyPropertyChanged("PersonalElements");
                    NotifyPropertyChanged("Elements");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveElement(ElementValued elementValued)

        #region Method: OverrideElement(ElementValued inheritedElement)
        /// <summary>
        /// Override the given inherited element.
        /// </summary>
        /// <param name="inheritedElement">The inherited element that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverrideElement(ElementValued inheritedElement)
        {
            if (inheritedElement != null && inheritedElement.Element != null && this.InheritedElements.Contains(inheritedElement))
            {
                // If the element is already available, there is no use to add it
                foreach (ElementValued personalElement in this.PersonalElements)
                {
                    if (inheritedElement.Element.Equals(personalElement.Element))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenElement(inheritedElement.Element))
                    return Message.RelationExistsAlready;

                // Copy the valued element and add it
                return AddOverriddenElement(new ElementValued(inheritedElement));
            }
            return Message.RelationFail;
        }
        #endregion Method: OverrideElement(ElementValued inheritedElement)

        #region Method: AddOverriddenElement(ElementValued inheritedElement)
        /// <summary>
        /// Add the given overridden element.
        /// </summary>
        /// <param name="overriddenElement">The overridden element to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenElement(ElementValued overriddenElement)
        {
            if (overriddenElement != null)
            {
                Database.Current.Insert(this.ID, GenericTables.MatterOverriddenElement, Columns.ElementValued, overriddenElement);
                NotifyPropertyChanged("OverriddenElements");
                NotifyPropertyChanged("InheritedElements");
                NotifyPropertyChanged("Elements");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenElement(ElementValued inheritedElement)

        #region Method: RemoveOverriddenElement(ElementValued overriddenElement)
        /// <summary>
        /// Removes an overridden element.
        /// </summary>
        /// <param name="overriddenElement">The overridden element to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenElement(ElementValued overriddenElement)
        {
            if (overriddenElement != null)
            {
                if (this.OverriddenElements.Contains(overriddenElement))
                {
                    // Remove the overridden element
                    Database.Current.Remove(this.ID, GenericTables.MatterOverriddenElement, Columns.ElementValued, overriddenElement);
                    NotifyPropertyChanged("OverriddenElements");
                    NotifyPropertyChanged("InheritedElements");
                    NotifyPropertyChanged("Elements");
                    overriddenElement.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenElement(ElementValued overriddenElement)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasElement(Element element)
        /// <summary>
        /// Checks whether the matter has the given element.
        /// </summary>
        /// <param name="element">The element that should be checked.</param>
        /// <returns>Returns true when the matter has the element.</returns>
        public bool HasElement(Element element)
        {
            if (element != null)
            {
                foreach (ElementValued elementValued in this.Elements)
                {
                    if (element.Equals(elementValued.Element))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasElement(Element element)

        #region Method: HasOverriddenElement(Element element)
        /// <summary>
        /// Checks whether the matter has the given overridden element.
        /// </summary>
        /// <param name="element">The element that should be checked.</param>
        /// <returns>Returns true when the matter has the overridden element.</returns>
        private bool HasOverriddenElement(Element element)
        {
            if (element != null)
            {
                foreach (ElementValued elementValued in this.OverriddenElements)
                {
                    if (element.Equals(elementValued.Element))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasOverriddenElement(Element element)

        #region Method: Remove()
        /// <summary>
        /// Remove the matter.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the elements
            foreach (ElementValued elementValued in this.PersonalElements)
                elementValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.MatterElement);

            // Remove the overridden elements
            foreach (ElementValued elementValued in this.OverriddenElements)
                elementValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.MatterOverriddenElement);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the matter.
        /// </summary>
        /// <returns>A clone of the matter.</returns>
        public new Matter Clone()
        {
            return base.Clone() as Matter;
        }
        #endregion Method: Clone()

        #region Method: CompareTo(Matter other)
        /// <summary>
        /// Compares the matter to the other matter.
        /// </summary>
        /// <param name="other">The matter to compare to this matter.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Matter other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Matter other)

        #endregion Method Group: Other

    }
    #endregion Class: Matter

    #region Class: MatterValued
    /// <summary>
    /// A valued version of matter.
    /// </summary>
    public abstract class MatterValued : PhysicalEntityValued
    {

        #region Properties and Fields

        #region Property: Matter
        /// <summary>
        /// Gets the matter of which this is valued matter.
        /// </summary>
        public Matter Matter
        {
            get
            {
                return this.Node as Matter;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Matter

        #region Property: StateOfMatter
        /// <summary>
        /// Gets or sets the state of matter.
        /// </summary>
        public StateOfMatter StateOfMatter
        {
            get
            {
                return Database.Current.Select<StateOfMatter>(this.ID, ValueTables.MatterValued, Columns.StateOfMatter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MatterValued, Columns.StateOfMatter, value);
                NotifyPropertyChanged("StateOfMatter");
            }
        }
        #endregion Property: StateOfMatter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MatterValued(uint id)
        /// <summary>
        /// Creates new valued matter from the given ID.
        /// </summary>
        /// <param name="id">The ID to create valued matter from.</param>
        protected MatterValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: MatterValued(uint id)

        #region Constructor: MatterValued(MatterValued matterValued)
        /// <summary>
        /// Clones valued matter.
        /// </summary>
        /// <param name="matterValued">The valued matter to clone.</param>
        protected MatterValued(MatterValued matterValued)
            : base(matterValued)
        {
            if (matterValued != null)
            {
                Database.Current.StartChange();

                this.StateOfMatter = matterValued.StateOfMatter;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: MatterValued(MatterValued matterValued)

        #region Constructor: MatterValued(Matter matter)
        /// <summary>
        /// Creates valued matter from the given matter.
        /// </summary>
        /// <param name="matter">The matter to create valued matter from.</param>
        protected MatterValued(Matter matter)
            : base(matter)
        {
            Database.Current.StartChange();

            if (matter != null)
                this.StateOfMatter = matter.DefaultStateOfMatter;
            else
                this.StateOfMatter = default(StateOfMatter);

            Database.Current.StopChange();
        }
        #endregion Constructor: MatterValued(Matter matter)

        #region Constructor: MatterValued(MatterValued matter, NumericalValueRange quantity)
        /// <summary>
        /// Creates valued matter from the given matter in the given quantity.
        /// </summary>
        /// <param name="matter">The matter to create valued matter from.</param>
        /// <param name="quantity">The quantity of the valued matter.</param>
        protected MatterValued(Matter matter, NumericalValueRange quantity)
            : base(matter, quantity)
        {
            Database.Current.StartChange();

            if (matter != null)
                this.StateOfMatter = matter.DefaultStateOfMatter;
            else
                this.StateOfMatter = default(StateOfMatter);

            Database.Current.StopChange();
        }
        #endregion Constructor: MatterValued(MatterValued matter, NumericalValueRange quantity)

        #region Method: Create(Matter matter)
        /// <summary>
        /// Create a valued matter of the given matter.
        /// </summary>
        /// <param name="matter">The matter to create a valued matter of.</param>
        /// <returns>A valued matter of the given matter.</returns>
        public static MatterValued Create(Matter matter)
        {
            Compound compound = matter as Compound;
            if (compound != null)
                return new CompoundValued(compound);

            Material material = matter as Material;
            if (material != null)
                return new MaterialValued(material);

            Mixture mixture = matter as Mixture;
            if (mixture != null)
                return new MixtureValued(mixture);

            Substance substance = matter as Substance;
            if (substance != null)
                return new SubstanceValued(substance);

            return null;
        }
        #endregion Method: Create(Matter matter)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the valued matter.
        /// </summary>
        /// <returns>A clone of the valued matter.</returns>
        public new MatterValued Clone()
        {
            return base.Clone() as MatterValued;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: MatterValued

    #region Class: MatterCondition
    /// <summary>
    /// A condition on matter.
    /// </summary>
    public abstract class MatterCondition : PhysicalEntityCondition
    {

        #region Properties and Fields

        #region Property: Matter
        /// <summary>
        /// Gets or sets the required matter.
        /// </summary>
        public Matter Matter
        {
            get
            {
                return this.Node as Matter;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Matter

        #region Property: StateOfMatter
        /// <summary>
        /// Gets or sets the state of matter.
        /// </summary>
        public StateOfMatter? StateOfMatter
        {
            get
            {
                return Database.Current.Select<StateOfMatter?>(this.ID, ValueTables.MatterCondition, Columns.StateOfMatter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MatterCondition, Columns.StateOfMatter, value);
                NotifyPropertyChanged("StateOfMatter");
            }
        }
        #endregion Property: StateOfMatter

        #region Property: StateOfMatterSign
        /// <summary>
        /// Gets or sets the sign for the state of matter in the condition.
        /// </summary>
        public EqualitySign? StateOfMatterSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.MatterCondition, Columns.StateOfMatterSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MatterCondition, Columns.StateOfMatterSign, value);
                NotifyPropertyChanged("StateOfMatterSign");
            }
        }
        #endregion Property: StateOfMatterSign

        #region Property: HasAllMandatoryElements
        /// <summary>
        /// Gets or sets the value that indicates whether all mandatory elements are required.
        /// </summary>
        public bool? HasAllMandatoryElements
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, ValueTables.MatterCondition, Columns.HasAllMandatoryElements);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MatterCondition, Columns.HasAllMandatoryElements, value);
                NotifyPropertyChanged("HasAllMandatoryElements");
            }
        }
        #endregion Property: HasAllMandatoryElements

        #region Property: Elements
        /// <summary>
        /// Gets the required elements.
        /// </summary>
        public ReadOnlyCollection<ElementCondition> Elements
        {
            get
            {
                return Database.Current.SelectAll<ElementCondition>(this.ID, ValueTables.MatterConditionElementCondition, Columns.ElementCondition).AsReadOnly();
            }
        }
        #endregion Property: Elements

        #region Property: ChemicalFormula
        /// <summary>
        /// Gets or sets the required chemical formula.
        /// </summary>
        public String ChemicalFormula
        {
            get
            {
                return Database.Current.Select<String>(this.ID, ValueTables.MatterCondition, Columns.Formula);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MatterCondition, Columns.Formula, value);
                NotifyPropertyChanged("ChemicalFormula");
            }
        }
        #endregion Property: ChemicalFormula

        #region Property: ChemicalFormulaSign
        /// <summary>
        /// Gets or sets the sign for the required chemical formula.
        /// </summary>
        public EqualitySign? ChemicalFormulaSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.MatterCondition, Columns.FormulaSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MatterCondition, Columns.FormulaSign, value);
                NotifyPropertyChanged("ChemicalFormulaSign");
            }
        }
        #endregion Property: ChemicalFormulaSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: MatterCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static MatterCondition()
        {
            // Elements
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ElementCondition, new Tuple<Type, EntryType>(typeof(ElementCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.MatterConditionElementCondition, typeof(MatterCondition), dict);
        }
        #endregion Static Constructor: MatterCondition()

        #region Constructor: MatterCondition()
        /// <summary>
        /// Creates a new matter condition.
        /// </summary>
        protected MatterCondition()
            : base()
        {
        }
        #endregion Constructor: MatterCondition()

        #region Constructor: MatterCondition(uint id)
        /// <summary>
        /// Creates a new matter condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a matter condition from.</param>
        protected MatterCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: MatterCondition(uint id)

        #region Constructor: MatterCondition(MatterCondition matterCondition)
        /// <summary>
        /// Clones a matter condition.
        /// </summary>
        /// <param name="matterCondition">The matter condition to clone.</param>
        protected MatterCondition(MatterCondition matterCondition)
            : base(matterCondition)
        {
            if (matterCondition != null)
            {
                Database.Current.StartChange();

                this.StateOfMatter = matterCondition.StateOfMatter;
                this.StateOfMatterSign = matterCondition.StateOfMatterSign;
                this.HasAllMandatoryElements = matterCondition.HasAllMandatoryElements;
                foreach (ElementCondition elementCondition in matterCondition.Elements)
                    AddElement(new ElementCondition(elementCondition));
                this.ChemicalFormula = matterCondition.ChemicalFormula;
                this.ChemicalFormulaSign = matterCondition.ChemicalFormulaSign;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: MatterCondition(MatterCondition matterCondition)

        #region Constructor: MatterCondition(Matter matter)
        /// <summary>
        /// Creates a condition for the given matter.
        /// </summary>
        /// <param name="matter">The matter to create a condition for.</param>
        protected MatterCondition(Matter matter)
            : base(matter)
        {
        }
        #endregion Constructor: MatterCondition(Matter matter)

        #region Constructor: MatterCondition(Matter matter, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given matter in the given quantity.
        /// </summary>
        /// <param name="matter">The matter to create a condition for.</param>
        /// <param name="quantity">The quantity of the matter condition.</param>
        protected MatterCondition(Matter matter, NumericalValueCondition quantity)
            : base(matter, quantity)
        {
        }
        #endregion Constructor: MatterCondition(Matter matter, NumericalValueCondition quantity)

        #region Method: Create(Matter matter)
        /// <summary>
        /// Create a matter condition of the given matter.
        /// </summary>
        /// <param name="matter">The matter to create a matter condition of.</param>
        /// <returns>A matter condition of the given matter.</returns>
        public static MatterCondition Create(Matter matter)
        {
            Compound compound = matter as Compound;
            if (compound != null)
                return new CompoundCondition(compound);

            Material material = matter as Material;
            if (material != null)
                return new MaterialCondition(material);

            Mixture mixture = matter as Mixture;
            if (mixture != null)
                return new MixtureCondition(mixture);

            Substance substance = matter as Substance;
            if (substance != null)
                return new SubstanceCondition(substance);

            return null;
        }
        #endregion Method: Create(Matter matter)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddElement(ElementCondition elementCondition)
        /// <summary>
        /// Adds an element condition.
        /// </summary>
        /// <param name="elementCondition">The element condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddElement(ElementCondition elementCondition)
        {
            if (elementCondition != null)
            {
                // If the element condition is already available in all elements, there is no use to add it
                if (HasElement(elementCondition.Element))
                    return Message.RelationExistsAlready;

                // Add the element condition
                Database.Current.Insert(this.ID, ValueTables.MatterConditionElementCondition, Columns.ElementCondition, elementCondition);
                NotifyPropertyChanged("Elements");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddElement(ElementCondition elementCondition)

        #region Method: RemoveElement(ElementCondition elementCondition)
        /// <summary>
        /// Removes an element condition.
        /// </summary>
        /// <param name="elementCondition">The element condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveElement(ElementCondition elementCondition)
        {
            if (elementCondition != null)
            {
                if (this.Elements.Contains(elementCondition))
                {
                    // Remove the element condition
                    Database.Current.Remove(this.ID, ValueTables.MatterConditionElementCondition, Columns.ElementCondition, elementCondition);
                    NotifyPropertyChanged("Elements");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveElement(ElementCondition elementCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasElement(Element element)
        /// <summary>
        /// Checks whether the matter condition has the given element.
        /// </summary>
        /// <param name="element">The element that should be checked.</param>
        /// <returns>Returns true when the matter condition has the element.</returns>
        public bool HasElement(Element element)
        {
            if (element != null)
            {
                foreach (ElementCondition elementCondition in this.Elements)
                {
                    if (element.Equals(elementCondition.Element))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasElement(Element element)

        #region Method: Remove()
        /// <summary>
        /// Remove the chemical substance condition.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the elements
            foreach (ElementCondition elementCondition in this.Elements)
                elementCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.MatterConditionElementCondition);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the matter condition.
        /// </summary>
        /// <returns>A clone of the matter condition.</returns>
        public new MatterCondition Clone()
        {
            return base.Clone() as MatterCondition;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: MatterCondition

    #region Class: MatterChange
    /// <summary>
    /// A change on matter.
    /// </summary>
    public abstract class MatterChange : PhysicalEntityChange
    {

        #region Properties and Fields

        #region Property: Matter
        /// <summary>
        /// Gets or sets the affected matter.
        /// </summary>
        public Matter Matter
        {
            get
            {
                return this.Node as Matter;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Matter

        #region Property: StateOfMatter
        /// <summary>
        /// Gets or sets the state of matter.
        /// </summary>
        public StateOfMatter? StateOfMatter
        {
            get
            {
                return Database.Current.Select<StateOfMatter?>(this.ID, ValueTables.MatterChange, Columns.StateOfMatter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MatterChange, Columns.StateOfMatter, value);
                NotifyPropertyChanged("StateOfMatter");
            }
        }
        #endregion Property: StateOfMatter

        #region Property: Elements
        /// <summary>
        /// Gets the elements to change.
        /// </summary>
        public ReadOnlyCollection<ElementChange> Elements
        {
            get
            {
                return Database.Current.SelectAll<ElementChange>(this.ID, ValueTables.MatterChangeElementChange, Columns.ElementChange).AsReadOnly();
            }
        }
        #endregion Property: Elements

        #region Property: ElementsToAdd
        /// <summary>
        /// Gets the elements that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<ElementValued> ElementsToAdd
        {
            get
            {
                return Database.Current.SelectAll<ElementValued>(this.ID, ValueTables.MatterChangeElementToAdd, Columns.ElementValued).AsReadOnly();
            }
        }
        #endregion Property: ElementsToAdd

        #region Property: ElementsToRemove
        /// <summary>
        /// Gets the elements that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<ElementCondition> ElementsToRemove
        {
            get
            {
                return Database.Current.SelectAll<ElementCondition>(this.ID, ValueTables.MatterChangeElementToRemove, Columns.ElementCondition).AsReadOnly();
            }
        }
        #endregion Property: ElementsToRemove

        #region Property: ChemicalFormula
        /// <summary>
        /// Gets or sets the chemical formula to change to.
        /// </summary>
        public String ChemicalFormula
        {
            get
            {
                return Database.Current.Select<String>(this.ID, ValueTables.MatterChange, Columns.Formula);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MatterChange, Columns.Formula, value);
                NotifyPropertyChanged("ChemicalFormula");
            }
        }
        #endregion Property: ChemicalFormula

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: MatterChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static MatterChange()
        {
            // Elements
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ElementChange, new Tuple<Type, EntryType>(typeof(ElementChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.MatterChangeElementChange, typeof(MatterChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ElementValued, new Tuple<Type, EntryType>(typeof(ElementValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.MatterChangeElementToAdd, typeof(MatterChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ElementCondition, new Tuple<Type, EntryType>(typeof(ElementCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.MatterChangeElementToRemove, typeof(MatterChange), dict);
        }
        #endregion Static Constructor: MatterChange()

        #region Constructor: MatterChange()
        /// <summary>
        /// Creates a new matter change.
        /// </summary>
        protected MatterChange()
            : base()
        {
        }
        #endregion Constructor: MatterChange()

        #region Constructor: MatterChange(uint id)
        /// <summary>
        /// Creates a new matter change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a matter change from.</param>
        protected MatterChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: MatterChange(uint id)

        #region Constructor: MatterChange(MatterChange matterChange)
        /// <summary>
        /// Clones a matter change.
        /// </summary>
        /// <param name="materialChange">The matter change to clone.</param>
        protected MatterChange(MatterChange matterChange)
            : base(matterChange)
        {
            if (matterChange != null)
            {
                Database.Current.StartChange();

                this.StateOfMatter = matterChange.StateOfMatter;
                foreach (ElementChange elementChange in matterChange.Elements)
                    AddElement(new ElementChange(elementChange));
                foreach (ElementValued elementValued in matterChange.ElementsToAdd)
                    AddElementToAdd(new ElementValued(elementValued));
                foreach (ElementCondition elementCondition in matterChange.ElementsToRemove)
                    AddElementToRemove(new ElementCondition(elementCondition));
                this.ChemicalFormula = matterChange.ChemicalFormula;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: MatterChange(MatterChange matterChange)

        #region Constructor: MatterChange(Matter matter)
        /// <summary>
        /// Creates a change for the given matter.
        /// </summary>
        /// <param name="matter">The matter to create a change for.</param>
        protected MatterChange(Matter matter)
            : base(matter)
        {
        }
        #endregion Constructor: MatterChange(Matter matter)

        #region Constructor: MatterChange(Matter matter, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given matter in the form of the given quantity.
        /// </summary>
        /// <param name="matter">The matter to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        protected MatterChange(Matter matter, NumericalValueChange quantity)
            : base(matter, quantity)
        {
        }
        #endregion Constructor: MatterChange(Matter matter, NumericalValueChange quantity)

        #region Method: Create(Matter matter)
        /// <summary>
        /// Create a matter change of the given matter.
        /// </summary>
        /// <param name="matter">The matter to create a matter change of.</param>
        /// <returns>A matter change of the given matter.</returns>
        public static MatterChange Create(Matter matter)
        {
            Compound compound = matter as Compound;
            if (compound != null)
                return new CompoundChange(compound);

            Material material = matter as Material;
            if (material != null)
                return new MaterialChange(material);

            Mixture mixture = matter as Mixture;
            if (mixture != null)
                return new MixtureChange(mixture);

            Substance substance = matter as Substance;
            if (substance != null)
                return new SubstanceChange(substance);

            return null;
        }
        #endregion Method: Create(Matter matter)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddElement(ElementChange elementChange)
        /// <summary>
        /// Adds an element change.
        /// </summary>
        /// <param name="elementChange">The element change to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddElement(ElementChange elementChange)
        {
            if (elementChange != null)
            {
                // If the element change is already available in all elements, there is no use to add it
                if (HasElement(elementChange.Element))
                    return Message.RelationExistsAlready;

                // Add the element change
                Database.Current.Insert(this.ID, ValueTables.MatterChangeElementChange, Columns.ElementChange, elementChange);
                NotifyPropertyChanged("Elements");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddElement(ElementChange elementChange)

        #region Method: RemoveElement(ElementChange elementChange)
        /// <summary>
        /// Removes an element change.
        /// </summary>
        /// <param name="elementChange">The element change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveElement(ElementChange elementChange)
        {
            if (elementChange != null)
            {
                if (this.Elements.Contains(elementChange))
                {
                    // Remove the element change
                    Database.Current.Remove(this.ID, ValueTables.MatterChangeElementChange, Columns.ElementChange, elementChange);
                    NotifyPropertyChanged("Elements");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveElement(ElementChange elementChange)

        #region Method: AddElementToAdd(ElementValued elementValued)
        /// <summary>
        /// Adds a valued element to the list of elements to add.
        /// </summary>
        /// <param name="elementValued">The valued element to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddElementToAdd(ElementValued elementValued)
        {
            if (elementValued != null && elementValued.Element != null)
            {
                // If the valued element is already available in all elements, there is no use to add it
                if (HasElementToAdd(elementValued.Element))
                    return Message.RelationExistsAlready;

                // Add the valued element
                Database.Current.Insert(this.ID, ValueTables.MatterChangeElementToAdd, Columns.ElementValued, elementValued);
                NotifyPropertyChanged("ElementsToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddElementToAdd(ElementValued elementValued)

        #region Method: RemoveElementToAdd(ElementValued elementValued)
        /// <summary>
        /// Removes a valued element from the list of elements to add.
        /// </summary>
        /// <param name="elementValued">The valued element to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveElementToAdd(ElementValued elementValued)
        {
            if (elementValued != null)
            {
                if (this.ElementsToAdd.Contains(elementValued))
                {
                    // Remove the valued element
                    Database.Current.Remove(this.ID, ValueTables.MatterChangeElementToAdd, Columns.ElementValued, elementValued);
                    NotifyPropertyChanged("ElementsToAdd");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveElementToAdd(ElementValued elementValued)

        #region Method: AddElementToRemove(ElementCondition elementCondition)
        /// <summary>
        /// Adds an element condition to the list of elements to remove.
        /// </summary>
        /// <param name="elementCondition">The element condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddElementToRemove(ElementCondition elementCondition)
        {
            if (elementCondition != null)
            {
                // If the element condition is already available in all elements, there is no use to add it
                if (HasElementToRemove(elementCondition.Element))
                    return Message.RelationExistsAlready;

                // Add the element condition
                Database.Current.Insert(this.ID, ValueTables.MatterChangeElementToRemove, Columns.ElementCondition, elementCondition);
                NotifyPropertyChanged("ElementsToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddElementToRemove(ElementCondition elementCondition)

        #region Method: RemoveElementToRemove(ElementCondition elementCondition)
        /// <summary>
        /// Removes an element condition from the list of elements to remove.
        /// </summary>
        /// <param name="elementCondition">The element condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveElementToRemove(ElementCondition elementCondition)
        {
            if (elementCondition != null)
            {
                if (this.ElementsToRemove.Contains(elementCondition))
                {
                    // Remove the element condition
                    Database.Current.Remove(this.ID, ValueTables.MatterChangeElementToRemove, Columns.ElementCondition, elementCondition);
                    NotifyPropertyChanged("ElementsToRemove");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveElementToRemove(ElementCondition elementCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasElement(Element element)
        /// <summary>
        /// Checks whether the matter change has the given element.
        /// </summary>
        /// <param name="element">The element that should be checked.</param>
        /// <returns>Returns true when the matter change has the given element.</returns>
        public bool HasElement(Element element)
        {
            if (element != null)
            {
                foreach (ElementChange elementChange in this.Elements)
                {
                    if (element.Equals(elementChange.Element))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasElement(Element element)

        #region Method: HasElementToAdd(Element element)
        /// <summary>
        /// Checks whether the matter change has the given element to add.
        /// </summary>
        /// <param name="element">The element that should be checked.</param>
        /// <returns>Returns true when the matter change has the given element to add.</returns>
        public bool HasElementToAdd(Element element)
        {
            if (element != null)
            {
                foreach (ElementValued elementValued in this.ElementsToAdd)
                {
                    if (element.Equals(elementValued.Element))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasElementToAdd(Element element)

        #region Method: HasElementToRemove(Element element)
        /// <summary>
        /// Checks whether the matter change has the given element to remove.
        /// </summary>
        /// <param name="element">The element that should be checked.</param>
        /// <returns>Returns true when the matter change has the given element to remove.</returns>
        public bool HasElementToRemove(Element element)
        {
            if (element != null)
            {
                foreach (ElementCondition elementCondition in this.ElementsToRemove)
                {
                    if (element.Equals(elementCondition.Element))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasElementToRemove(Element element)

        #region Method: Remove()
        /// <summary>
        /// Remove the matter change.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the elements
            foreach (ElementChange elementChange in this.Elements)
                elementChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.MatterChangeElementChange);

            foreach (ElementValued elementValued in this.ElementsToAdd)
                elementValued.Remove();
            Database.Current.Remove(this.ID, ValueTables.MatterChangeElementToAdd);

            foreach (ElementCondition elementCondition in this.ElementsToRemove)
                elementCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.MatterChangeElementToRemove);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the matter change.
        /// </summary>
        /// <returns>A clone of the matter change.</returns>
        public new MatterChange Clone()
        {
            return base.Clone() as MatterChange;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: MatterChange

    #region Class: Layer
    /// <summary>
    /// A valued version of matter that acts as a layer.
    /// </summary>
    public sealed class Layer : MatterValued
    {

        #region Properties and Fields

        #region Property: Thickness
        /// <summary>
        /// Gets or sets the thickness of the layer.
        /// </summary>
        public NumericalValue Thickness
        {
            get
            {
                return Database.Current.Select<NumericalValue>(this.ID, ValueTables.Layer, Columns.Thickness);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Layer, Columns.Thickness, value);
                NotifyPropertyChanged("Thickness");
            }
        }
        #endregion Property: Thickness

        #region Property: Index
        /// <summary>
        /// Gets or sets the index of the layer when it is stacked on top of or underneath other layers: "the n-th layer".
        /// </summary>
        public int Index
        {
            get
            {
                return Database.Current.Select<int>(this.ID, ValueTables.Layer, Columns.Index);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Layer, Columns.Index, value);
                NotifyPropertyChanged("Index");
            }
        }
        #endregion Property: Index

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Layer(uint id)
        /// <summary>
        /// Creates a new layer from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a layer from.</param>
        private Layer(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Layer(uint id)

        #region Constructor: Layer(Layer layer)
        /// <summary>
        /// Clones a layer.
        /// </summary>
        /// <param name="layer">The layer to clone.</param>
        public Layer(Layer layer)
            : base(layer)
        {
            if (layer != null)
            {
                Database.Current.StartChange();

                this.Thickness = new NumericalValue(layer.Thickness);
                this.Index = layer.Index;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Layer(Layer layer)

        #region Constructor: Layer(Matter matter)
        /// <summary>
        /// Creates a layer from the given matter.
        /// </summary>
        /// <param name="matter">The matter to create a layer from.</param>
        public Layer(Matter matter)
            : base(matter)
        {
            Database.Current.StartChange();

            this.Thickness = new NumericalValue(SemanticsSettings.Values.Thickness);
            this.Index = SemanticsSettings.Values.Index;

            Database.Current.StopChange();
        }
        #endregion Constructor: Layer(Matter matter)

        #region Constructor: Layer(Matter matter, NumericalValueRange quantity)
        /// <summary>
        /// Creates a layer from the given matter in the given quantity.
        /// </summary>
        /// <param name="matter">The matter to create a layer from.</param>
        /// <param name="quantity">The quantity of the layer.</param>
        public Layer(Matter matter, NumericalValueRange quantity)
            : base(matter, quantity)
        {
            Database.Current.StartChange();

            this.Thickness = new NumericalValue(SemanticsSettings.Values.Thickness);
            this.Index = SemanticsSettings.Values.Index;

            Database.Current.StopChange();
        }
        #endregion Constructor: Layer(Matter matter, NumericalValueRange quantity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the layer.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the thickness
            if (this.Thickness != null)
                this.Thickness.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: MaterialValued

    #region Class: LayerCondition
    /// <summary>
    /// A condition on a layer.
    /// </summary>
    public sealed class LayerCondition : MatterCondition
    {

        #region Properties and Fields

        #region Property: Thickness
        /// <summary>
        /// Gets or sets the required thickness of the layer.
        /// </summary>
        public NumericalValueCondition Thickness
        {
            get
            {
                return Database.Current.Select<NumericalValueCondition>(this.ID, ValueTables.LayerCondition, Columns.Thickness);
            }
            set
            {
                if (this.Thickness != null)
                    this.Thickness.Remove();

                Database.Current.Update(this.ID, ValueTables.LayerCondition, Columns.Thickness, value);
                NotifyPropertyChanged("Thickness");
            }
        }
        #endregion Property: Thickness

        #region Property: Index
        /// <summary>
        /// Gets or sets the required index of the layer.
        /// </summary>
        public int? Index
        {
            get
            {
                return Database.Current.Select<int?>(this.ID, ValueTables.LayerCondition, Columns.Index);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.LayerCondition, Columns.Index, value);
                NotifyPropertyChanged("Index");
            }
        }
        #endregion Property: Index

        #region Property: IndexSign
        /// <summary>
        /// Gets or sets the sign for the index in the condition.
        /// </summary>
        public EqualitySignExtended? IndexSign
        {
            get
            {
                return Database.Current.Select<EqualitySignExtended?>(this.ID, ValueTables.LayerCondition, Columns.IndexSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.LayerCondition, Columns.IndexSign, value);
                NotifyPropertyChanged("IndexSign");
            }
        }
        #endregion Property: IndexSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: LayerCondition()
        /// <summary>
        /// Creates a new layer condition.
        /// </summary>
        public LayerCondition()
            : base()
        {
        }
        #endregion Constructor: LayerCondition()

        #region Constructor: LayerCondition(uint id)
        /// <summary>
        /// Creates a new layer condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a layer condition from.</param>
        private LayerCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: LayerCondition(uint id)

        #region Constructor: LayerCondition(LayerCondition layerCondition)
        /// <summary>
        /// Clones a layer condition.
        /// </summary>
        /// <param name="layerCondition">The layer condition to clone.</param>
        public LayerCondition(LayerCondition layerCondition)
            : base(layerCondition)
        {
            if (layerCondition != null)
            {
                Database.Current.StartChange();

                if (layerCondition.Thickness != null)
                    this.Thickness = new NumericalValueCondition(layerCondition.Thickness);
                this.Index = layerCondition.Index;
                this.IndexSign = layerCondition.IndexSign;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: LayerCondition(LayerCondition layerCondition)

        #region Constructor: LayerCondition(Matter matter)
        /// <summary>
        /// Creates a condition for the given matter.
        /// </summary>
        /// <param name="matter">The matter to create a condition for.</param>
        public LayerCondition(Matter matter)
            : base(matter)
        {
        }
        #endregion Constructor: LayerCondition(Matter matter)

        #region Constructor: LayerCondition(Matter matter, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given matter in the given quantity.
        /// </summary>
        /// <param name="matter">The matter to create a condition for.</param>
        /// <param name="quantity">The quantity of the layer condition.</param>
        public LayerCondition(Matter matter, NumericalValueCondition quantity)
            : base(matter, quantity)
        {
        }
        #endregion Constructor: LayerCondition(Matter matter, NumericalValueCondition quantity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the layer condition.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the thickness
            if (this.Thickness != null)
                this.Thickness.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: LayerCondition

    #region Class: LayerChange
    /// <summary>
    /// A change on a layer.
    /// </summary>
    public sealed class LayerChange : MatterChange
    {

        #region Properties and Fields

        #region Property: Thickness
        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        public NumericalValueChange Thickness
        {
            get
            {
                return Database.Current.Select<NumericalValueChange>(this.ID, ValueTables.LayerChange, Columns.Thickness);
            }
            set
            {
                if (this.Thickness != null)
                    this.Thickness.Remove();

                Database.Current.Update(this.ID, ValueTables.LayerChange, Columns.Thickness, value);
                NotifyPropertyChanged("Thickness");
            }
        }
        #endregion Property: Thickness

        #region Property: Index
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int? Index
        {
            get
            {
                return Database.Current.Select<int>(this.ID, ValueTables.LayerChange, Columns.Index);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.LayerChange, Columns.Index, value);
                NotifyPropertyChanged("Index");
            }
        }
        #endregion Property: Index

        #region Property: IndexChange
        /// <summary>
        /// Gets or sets the type of change for the index.
        /// </summary>
        public ValueChangeType? IndexChange
        {
            get
            {
                return Database.Current.Select<ValueChangeType?>(this.ID, ValueTables.LayerChange, Columns.IndexChange);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.LayerChange, Columns.IndexChange, value);
                NotifyPropertyChanged("IndexChange");
            }
        }
        #endregion Property: IndexChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: LayerChange()
        /// <summary>
        /// Creates a new layer change.
        /// </summary>
        public LayerChange()
            : base()
        {
        }
        #endregion Constructor: LayerChange()

        #region Constructor: LayerChange(uint id)
        /// <summary>
        /// Creates a new layer change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a layer change from.</param>
        private LayerChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: LayerChange(uint id)

        #region Constructor: LayerChange(LayerChange layerChange)
        /// <summary>
        /// Clones a layer change.
        /// </summary>
        /// <param name="layerChange">The layer change to clone.</param>
        public LayerChange(LayerChange layerChange)
            : base(layerChange)
        {
            if (layerChange != null)
            {
                Database.Current.StartChange();

                if (layerChange.Thickness != null)
                    this.Thickness = new NumericalValueChange(layerChange.Thickness);
                this.Index = layerChange.Index;
                this.IndexChange = layerChange.IndexChange;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: LayerChange(LayerChange layerChange)

        #region Constructor: LayerChange(Matter matter)
        /// <summary>
        /// Creates a change for the given matter.
        /// </summary>
        /// <param name="matter">The matter to create a change for.</param>
        public LayerChange(Matter matter)
            : base(matter)
        {
            Database.Current.StartChange();

            this.Thickness = new NumericalValueChange();
            this.IndexChange = ValueChangeType.Increase;

            Database.Current.StopChange();
        }
        #endregion Constructor: LayerChange(Matter matter)

        #region Constructor: LayerChange(Matter matter, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given matter in the form of the given quantity.
        /// </summary>
        /// <param name="matter">The matter to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        public LayerChange(Matter matter, NumericalValueChange quantity)
            : base(matter, quantity)
        {
            Database.Current.StartChange();

            this.Thickness = new NumericalValueChange();
            this.IndexChange = ValueChangeType.Increase;

            Database.Current.StopChange();
        }
        #endregion Constructor: LayerChange(Matter matter, NumericalValueChange quantity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the layer change.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the thickness
            if (this.Thickness != null)
                this.Thickness.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: LayerChange

}