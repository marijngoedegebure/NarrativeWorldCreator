/**************************************************************************
 * 
 * Element.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2010-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Entities
{

    #region Class: Element
    /// <summary>
    /// A chemical element is a pure chemical substance consisting of one type of atom distinguished by its atomic number, which is the number of protons in its nucleus.
    /// </summary>
    public class Element : PhysicalEntity, IComparable<Element>
    {

        #region Properties and Fields

        #region Property: AtomicNumber
        /// <summary>
        /// Gets or sets the atomic number.
        /// </summary>
        public byte AtomicNumber
        {
            get
            {
                return Database.Current.Select<byte>(this.ID, GenericTables.Element, Columns.AtomicNumber);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Element, Columns.AtomicNumber, value);
                NotifyPropertyChanged("AtomicNumber");
            }
        }
        #endregion Property: AtomicNumber		

        #region Property: Symbol
        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        public String Symbol
        {
            get
            {
                return Database.Current.Select<String>(this.ID, GenericTables.Element, Columns.Symbol);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Element, Columns.Symbol, value);
                NotifyPropertyChanged("Symbol");
            }
        }
        #endregion Property: Symbol

        #region Property: ElementType
        /// <summary>
        /// Gets or sets the type of the element.
        /// </summary>
        public ElementType ElementType
        {
            get
            {
                return Database.Current.Select<ElementType>(this.ID, GenericTables.Element, Columns.Type);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Element, Columns.Type, value);
                NotifyPropertyChanged("ElementType");
            }
        }
        #endregion Property: ElementType

        #region Property: MetalType
        /// <summary>
        /// Gets or sets the type of the metal.
        /// </summary>
        public MetalType MetalType
        {
            get
            {
                return Database.Current.Select<MetalType>(this.ID, GenericTables.Element, Columns.MetalType);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Element, Columns.MetalType, value);
                NotifyPropertyChanged("MetalType");
            }
        }
        #endregion Property: MetalType

        #region Property: Group
        /// <summary>
        /// Gets or sets the group (or family) of the element.
        /// </summary>
        public ElementGroup Group
        {
            get
            {
                return Database.Current.Select<ElementGroup>(this.ID, GenericTables.Element, Columns.Group);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Element, Columns.Group, value);
                NotifyPropertyChanged("Group");
            }
        }
        #endregion Property: Group

        #region Property: Period
        /// <summary>
        /// Gets or sets the period of the element.
        /// </summary>
        public ElementPeriod Period
        {
            get
            {
                return Database.Current.Select<ElementPeriod>(this.ID, GenericTables.Element, Columns.Period);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Element, Columns.Period, value);
                NotifyPropertyChanged("Period");
            }
        }
        #endregion Property: Period

        #region Property: Block
        /// <summary>
        /// Gets or sets the block of the element.
        /// </summary>
        public ElementBlock Block
        {
            get
            {
                return Database.Current.Select<ElementBlock>(this.ID, GenericTables.Element, Columns.Block);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Element, Columns.Block, value);
                NotifyPropertyChanged("Block");
            }
        }
        #endregion Property: Block

        #region Property: StandardState
        /// <summary>
        /// Gets or sets the standard state of the element.
        /// </summary>
        public StateOfMatter StandardState
        {
            get
            {
                return Database.Current.Select<StateOfMatter>(this.ID, GenericTables.Element, Columns.StandardState);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Element, Columns.StandardState, value);
                NotifyPropertyChanged("StandardState");
            }
        }
        #endregion Property: StandardState

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Element()
        /// <summary>
        /// Creates a new element.
        /// </summary>
        public Element()
            : base()
        {
            Database.Current.StartChange();
            Database.Current.QueryBegin();

            this.AtomicNumber = SemanticsSettings.Values.AtomicNumber;
            this.Symbol = SemanticsSettings.Values.Symbol;
            this.ElementType = default(ElementType);
            this.MetalType = default(MetalType);
            this.Group = default(ElementGroup);
            this.Period = default(ElementPeriod);
            this.Block = default(ElementBlock);
            this.StandardState = default(StateOfMatter);

            Database.Current.QueryCommit();
            Database.Current.StopChange();
        }
        #endregion Constructor: Element()

        #region Constructor: Element(uint id)
        /// <summary>
        /// Creates a new element with the given ID.
        /// </summary>
        /// <param name="id">The ID to create the new element from.</param>
        protected Element(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Element(uint id)

        #region Constructor: Element(string name)
        /// <summary>
        /// Creates a new element with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the element.</param>
        public Element(string name)
            : base(name)
        {
        }
        #endregion Constructor: Element(string name)

        #region Constructor: Element(string name, byte atomicNumber, string symbol)
        /// <summary>
        /// Creates a new element with the given name, atomic number, and symbol.
        /// </summary>
        /// <param name="name">The name to assign to the element.</param>
        /// <param name="atomicNumber">The atomic number of the element.</param>
        /// <param name="symbol">The symbol of the element.</param>
        public Element(string name, byte atomicNumber, string symbol)
            : base(name)
        {
            Database.Current.StartChange();

            this.AtomicNumber = atomicNumber;
            if (symbol != null)
                this.Symbol = symbol;

            Database.Current.StopChange();
        }
        #endregion Constructor: Element(string name, byte atomicNumber, string symbol)

        #region Constructor: Element(Element element)
        /// <summary>
        /// Clones an element.
        /// </summary>
        /// <param name="element">The element to be copied.</param>
        public Element(Element element)
            : base(element)
        {
            if (element != null)
            {
                Database.Current.StartChange();

                this.AtomicNumber = element.AtomicNumber;
                this.Symbol = element.Symbol;
                this.ElementType = element.ElementType;
                this.MetalType = element.MetalType;
                this.Group = element.Group;
                this.Period = element.Period;
                this.Block = element.Block;
                this.StandardState = element.StandardState;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Element(Element element)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the element.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Element other)
        /// <summary>
        /// Compares the element to the other element.
        /// </summary>
        /// <param name="other">The element to compare to this element.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Element other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Element other)

        #endregion Method Group: Other

    }
    #endregion Class: Element

    #region Class: ElementValued
    /// <summary>
    /// A valued version of an element.
    /// </summary>
    public class ElementValued : PhysicalEntityValued
    {

        #region Properties and Fields

        #region Property: Element
        /// <summary>
        /// Gets the element of which this is a valued element.
        /// </summary>
        public Element Element
        {
            get
            {
                return this.Node as Element;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Element
		
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ElementValued(uint id)
        /// <summary>
        /// Creates a new valued element from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued element from.</param>
        protected ElementValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ElementValued(uint id)

        #region Constructor: ElementValued(ElementValued elementValued)
        /// <summary>
        /// Clones a valued element.
        /// </summary>
        /// <param name="elementValued">The valued element to clone.</param>
        public ElementValued(ElementValued elementValued)
            : base(elementValued)
        {
        }
        #endregion Constructor: ElementValued(ElementValued elementValued)

        #region Constructor: ElementValued(Element element)
        /// <summary>
        /// Creates a new valued element from the given element.
        /// </summary>
        /// <param name="element">The element to create a valued element from.</param>
        public ElementValued(Element element)
            : base(element)
        {
        }
        #endregion Constructor: ElementValued(Element element)

        #region Constructor: ElementValued(Element element, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new valued element from the given element in the given quantity.
        /// </summary>
        /// <param name="element">The element to create a valued element from.</param>
        /// <param name="quantity">The quantity of the valued element.</param>
        public ElementValued(Element element, NumericalValueRange quantity)
            : base(element, quantity)
        {
        }
        #endregion Constructor: ElementValued(Element element, NumericalValueRange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: ElementValued

    #region Class: ElementCondition
    /// <summary>
    /// A condition on an element.
    /// </summary>
    public class ElementCondition : PhysicalEntityCondition
    {

        #region Properties and Fields

        #region Property: Element
        /// <summary>
        /// Gets or sets the required element.
        /// </summary>
        public Element Element
        {
            get
            {
                return this.Node as Element;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Element

        #region Property: AtomicNumber
        /// <summary>
        /// Gets or sets the atomic number.
        /// </summary>
        public uint? AtomicNumber
        {
            get
            {
                return Database.Current.Select<uint?>(this.ID, ValueTables.ElementCondition, Columns.AtomicNumber);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.AtomicNumber, value);
                NotifyPropertyChanged("AtomicNumber");
            }
        }
        #endregion Property: AtomicNumber

        #region Property: AtomicNumberSign
        /// <summary>
        /// Gets or sets the sign for the atomic number in the condition.
        /// </summary>
        public EqualitySignExtended? AtomicNumberSign
        {
            get
            {
                return Database.Current.Select<EqualitySignExtended?>(this.ID, ValueTables.ElementCondition, Columns.AtomicNumberSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.AtomicNumberSign, value);
                NotifyPropertyChanged("AtomicNumberSign");
            }
        }
        #endregion Property: AtomicNumberSign

        #region Property: Symbol
        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        public String Symbol
        {
            get
            {
                return Database.Current.Select<String>(this.ID, ValueTables.ElementCondition, Columns.Symbol);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.Symbol, value);
                NotifyPropertyChanged("Symbol");
            }
        }
        #endregion Property: Symbol

        #region Property: SymbolSign
        /// <summary>
        /// Gets or sets the sign for the symbol in the condition.
        /// </summary>
        public EqualitySign? SymbolSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.ElementCondition, Columns.SymbolSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.SymbolSign, value);
                NotifyPropertyChanged("SymbolSign");
            }
        }
        #endregion Property: SymbolSign

        #region Property: ElementType
        /// <summary>
        /// Gets or sets the type of the element.
        /// </summary>
        public ElementType? ElementType
        {
            get
            {
                return Database.Current.Select<ElementType?>(this.ID, ValueTables.ElementCondition, Columns.Type);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.Type, value);
                NotifyPropertyChanged("ElementType");
            }
        }
        #endregion Property: ElementType

        #region Property: ElementTypeSign
        /// <summary>
        /// Gets or sets the sign for the type of the element in the condition.
        /// </summary>
        public EqualitySign? ElementTypeSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.ElementCondition, Columns.TypeSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.TypeSign, value);
                NotifyPropertyChanged("ElementTypeSign");
            }
        }
        #endregion Property: ElementTypeSign

        #region Property: MetalType
        /// <summary>
        /// Gets or sets the type of the metal.
        /// </summary>
        public MetalType? MetalType
        {
            get
            {
                return Database.Current.Select<MetalType?>(this.ID, ValueTables.ElementCondition, Columns.MetalType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.MetalType, value);
                NotifyPropertyChanged("MetalType");
            }
        }
        #endregion Property: MetalType

        #region Property: MetalTypeSign
        /// <summary>
        /// Gets or sets the sign for the type of the metal in the condition.
        /// </summary>
        public EqualitySign? MetalTypeSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.ElementCondition, Columns.MetalTypeSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.MetalTypeSign, value);
                NotifyPropertyChanged("MetalTypeSign");
            }
        }
        #endregion Property: MetalTypeSign

        #region Property: Group
        /// <summary>
        /// Gets or sets the group (or family) of the element.
        /// </summary>
        public ElementGroup? Group
        {
            get
            {
                return Database.Current.Select<ElementGroup?>(this.ID, ValueTables.ElementCondition, Columns.Group);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.Group, value);
                NotifyPropertyChanged("Group");
            }
        }
        #endregion Property: Group

        #region Property: GroupSign
        /// <summary>
        /// Gets or sets the sign for the group (or family) of the element in the condition.
        /// </summary>
        public EqualitySign? GroupSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.ElementCondition, Columns.GroupSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.GroupSign, value);
                NotifyPropertyChanged("GroupSign");
            }
        }
        #endregion Property: GroupSign

        #region Property: Period
        /// <summary>
        /// Gets or sets the period of the element.
        /// </summary>
        public ElementPeriod? Period
        {
            get
            {
                return Database.Current.Select<ElementPeriod?>(this.ID, ValueTables.ElementCondition, Columns.Period);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.Period, value);
                NotifyPropertyChanged("Period");
            }
        }
        #endregion Property: Period

        #region Property: PeriodSign
        /// <summary>
        /// Gets or sets the sign for the period of the element in the condition.
        /// </summary>
        public EqualitySign? PeriodSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.ElementCondition, Columns.PeriodSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.PeriodSign, value);
                NotifyPropertyChanged("PeriodSign");
            }
        }
        #endregion Property: PeriodSign

        #region Property: Block
        /// <summary>
        /// Gets or sets the block of the element.
        /// </summary>
        public ElementBlock? Block
        {
            get
            {
                return Database.Current.Select<ElementBlock?>(this.ID, ValueTables.ElementCondition, Columns.Block);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.Block, value);
                NotifyPropertyChanged("Block");
            }
        }
        #endregion Property: Block

        #region Property: BlockSign
        /// <summary>
        /// Gets or sets the sign for the block of the element in the condition.
        /// </summary>
        public EqualitySign? BlockSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.ElementCondition, Columns.BlockSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.BlockSign, value);
                NotifyPropertyChanged("BlockSign");
            }
        }
        #endregion Property: BlockSign

        #region Property: StandardState
        /// <summary>
        /// Gets or sets the standard state of the element.
        /// </summary>
        public StateOfMatter? StandardState
        {
            get
            {
                return Database.Current.Select<StateOfMatter?>(this.ID, ValueTables.ElementCondition, Columns.StandardState);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.StandardState, value);
                NotifyPropertyChanged("StandardState");
            }
        }
        #endregion Property: StandardState

        #region Property: StandardStateSign
        /// <summary>
        /// Gets or sets the sign for the standard state of the element in the condition.
        /// </summary>
        public EqualitySign? StandardStateSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.ElementCondition, Columns.StandardStateSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementCondition, Columns.StandardStateSign, value);
                NotifyPropertyChanged("StandardStateSign");
            }
        }
        #endregion Property: StandardStateSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ElementCondition()
        /// <summary>
        /// Creates a new element condition.
        /// </summary>
        public ElementCondition()
            : base()
        {
            Database.Current.StartChange();

            this.AtomicNumberSign = EqualitySignExtended.Equal;
            this.SymbolSign = EqualitySign.Equal;
            this.ElementTypeSign = EqualitySign.Equal;
            this.MetalTypeSign = EqualitySign.Equal;
            this.GroupSign = EqualitySign.Equal;
            this.PeriodSign = EqualitySign.Equal;
            this.BlockSign = EqualitySign.Equal;
            this.StandardStateSign = EqualitySign.Equal;

            Database.Current.StopChange();
        }
        #endregion Constructor: ElementCondition()

        #region Constructor: ElementCondition(uint id)
        /// <summary>
        /// Creates a new element condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the element condition from.</param>
        protected ElementCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ElementCondition(uint id)

        #region Constructor: ElementCondition(ElementCondition elementCondition)
        /// <summary>
        /// Clones an element condition.
        /// </summary>
        /// <param name="elementCondition">The element condition to clone.</param>
        public ElementCondition(ElementCondition elementCondition)
            : base(elementCondition)
        {
            if (elementCondition != null)
            {
                Database.Current.StartChange();

                this.AtomicNumber = elementCondition.AtomicNumber;
                this.AtomicNumberSign = elementCondition.AtomicNumberSign;
                this.Symbol = elementCondition.Symbol;
                this.SymbolSign = elementCondition.SymbolSign;
                this.ElementType = elementCondition.ElementType;
                this.ElementTypeSign = elementCondition.ElementTypeSign;
                this.MetalType = elementCondition.MetalType;
                this.MetalTypeSign = elementCondition.MetalTypeSign;
                this.Group = elementCondition.Group;
                this.GroupSign = elementCondition.GroupSign;
                this.Period = elementCondition.Period;
                this.PeriodSign = elementCondition.PeriodSign;
                this.Block = elementCondition.Block;
                this.BlockSign = elementCondition.BlockSign;
                this.StandardState = elementCondition.StandardState;
                this.StandardStateSign = elementCondition.StandardStateSign;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: ElementCondition(ElementCondition elementCondition)

        #region Constructor: ElementCondition(Element element)
        /// <summary>
        /// Creates a condition for the given element.
        /// </summary>
        /// <param name="element">The element to create a condition for.</param>
        public ElementCondition(Element element)
            : base(element)
        {
        }
        #endregion Constructor: ElementCondition(Element element)

        #region Constructor: ElementCondition(Element element, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given element in the given quantity.
        /// </summary>
        /// <param name="element">The element to create a condition for.</param>
        /// <param name="quantity">The quantity of the element condition.</param>
        public ElementCondition(Element element, NumericalValueCondition quantity)
            : base(element, quantity)
        {
        }
        #endregion Constructor: ElementCondition(Element element, NumericalValueCondition quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: ElementCondition

    #region Class: ElementChange
    /// <summary>
    /// A change on an element.
    /// </summary>
    public class ElementChange : PhysicalEntityChange
    {

        #region Properties and Fields

        #region Property: Element
        /// <summary>
        /// Gets or sets the affected element.
        /// </summary>
        public Element Element
        {
            get
            {
                return this.Node as Element;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Element

        #region Property: AtomicNumber
        /// <summary>
        /// Gets or sets the atomic number to change to.
        /// </summary>
        public byte? AtomicNumber
        {
            get
            {
                return Database.Current.Select<byte?>(this.ID, ValueTables.ElementChange, Columns.AtomicNumber);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementChange, Columns.AtomicNumber, value);
                NotifyPropertyChanged("AtomicNumber");
            }
        }
        #endregion Property: AtomicNumber

        #region Property: Symbol
        /// <summary>
        /// Gets or sets the symbol to change to.
        /// </summary>
        public String Symbol
        {
            get
            {
                return Database.Current.Select<String>(this.ID, ValueTables.ElementChange, Columns.Symbol);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementChange, Columns.Symbol, value);
                NotifyPropertyChanged("Symbol");
            }
        }
        #endregion Property: Symbol

        #region Property: ElementType
        /// <summary>
        /// Gets or sets the type of the element to change to.
        /// </summary>
        public ElementType? ElementType
        {
            get
            {
                return Database.Current.Select<ElementType?>(this.ID, ValueTables.ElementChange, Columns.Type);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementChange, Columns.Type, value);
                NotifyPropertyChanged("ElementType");
            }
        }
        #endregion Property: ElementType

        #region Property: MetalType
        /// <summary>
        /// Gets or sets the type of the metal to change to.
        /// </summary>
        public MetalType? MetalType
        {
            get
            {
                return Database.Current.Select<MetalType?>(this.ID, ValueTables.ElementChange, Columns.MetalType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementChange, Columns.MetalType, value);
                NotifyPropertyChanged("MetalType");
            }
        }
        #endregion Property: MetalType

        #region Property: Group
        /// <summary>
        /// Gets or sets the group (or family) of the element to change to.
        /// </summary>
        public ElementGroup? Group
        {
            get
            {
                return Database.Current.Select<ElementGroup?>(this.ID, ValueTables.ElementChange, Columns.Group);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementChange, Columns.Group, value);
                NotifyPropertyChanged("Group");
            }
        }
        #endregion Property: Group

        #region Property: Period
        /// <summary>
        /// Gets or sets the period of the element to change to.
        /// </summary>
        public ElementPeriod? Period
        {
            get
            {
                return Database.Current.Select<ElementPeriod?>(this.ID, ValueTables.ElementChange, Columns.Period);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementChange, Columns.Period, value);
                NotifyPropertyChanged("Period");
            }
        }
        #endregion Property: Period

        #region Property: Block
        /// <summary>
        /// Gets or sets the block of the element to change to.
        /// </summary>
        public ElementBlock? Block
        {
            get
            {
                return Database.Current.Select<ElementBlock?>(this.ID, ValueTables.ElementChange, Columns.Block);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementChange, Columns.Block, value);
                NotifyPropertyChanged("Block");
            }
        }
        #endregion Property: Block

        #region Property: StandardState
        /// <summary>
        /// Gets or sets the standard state of the element to change to.
        /// </summary>
        public StateOfMatter? StandardState
        {
            get
            {
                return Database.Current.Select<StateOfMatter?>(this.ID, ValueTables.ElementChange, Columns.StandardState);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ElementChange, Columns.StandardState, value);
                NotifyPropertyChanged("StandardState");
            }
        }
        #endregion Property: StandardState

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ElementChange()
        /// <summary>
        /// Creates an element change.
        /// </summary>
        public ElementChange()
            : base()
        {
        }
        #endregion Constructor: ElementChange()

        #region Constructor: ElementChange(uint id)
        /// <summary>
        /// Creates a new element change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an element change from.</param>
        protected ElementChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ElementChange(uint id)

        #region Constructor: ElementChange(ElementChange elementChange)
        /// <summary>
        /// Clones an element change.
        /// </summary>
        /// <param name="elementChange">The element change to clone.</param>
        public ElementChange(ElementChange elementChange)
            : base(elementChange)
        {
            if (elementChange != null)
            {
                Database.Current.StartChange();

                this.AtomicNumber = elementChange.AtomicNumber;
                this.Symbol = elementChange.Symbol;
                this.ElementType = elementChange.ElementType;
                this.MetalType = elementChange.MetalType;
                this.Group = elementChange.Group;
                this.Period = elementChange.Period;
                this.Block = elementChange.Block;
                this.StandardState = elementChange.StandardState;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: ElementChange(ElementChange elementChange)

        #region Constructor: ElementChange(Element element)
        /// <summary>
        /// Creates a change for the given element.
        /// </summary>
        /// <param name="element">The element to create a change for.</param>
        public ElementChange(Element element)
            : base(element)
        {
        }
        #endregion Constructor: ElementChange(Element element)

        #region Constructor: ElementChange(Element element, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given element in the form of the given quantity.
        /// </summary>
        /// <param name="element">The element to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        public ElementChange(Element element, NumericalValueChange quantity)
            : base(element, quantity)
        {
        }
        #endregion Constructor: ElementChange(Element element, NumericalValueChange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: ElementChange

}