/**************************************************************************
 * 
 * ElementBase.cs
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
using Semantics.Entities;
using Semantics.Utilities;

namespace SemanticsEngine.Entities
{

    #region Class: ElementBase
    /// <summary>
    /// A base of an element.
    /// </summary>
    public class ElementBase : PhysicalEntityBase
    {

        #region Properties and Fields

        #region Property: Element
        /// <summary>
        /// Gets the element of which this is an element base.
        /// </summary>
        protected internal Element Element
        {
            get
            {
                return this.IdHolder as Element;
            }
        }
        #endregion Property: Element

        #region Property: AtomicNumber
        /// <summary>
        /// The atomic number.
        /// </summary>
        private byte atomicNumber = SemanticsSettings.Values.AtomicNumber;

        /// <summary>
        /// Gets the atomic number.
        /// </summary>
        public byte AtomicNumber
        {
            get
            {
                return atomicNumber;
            }
        }
        #endregion Property: AtomicNumber

        #region Property: Symbol
        /// <summary>
        /// The symbol.
        /// </summary>
        private String symbol = SemanticsSettings.Values.Symbol;

        /// <summary>
        /// Gets the symbol.
        /// </summary>
        public String Symbol
        {
            get
            {
                return symbol;
            }
        }
        #endregion Property: Symbol

        #region Property: ElementType
        /// <summary>
        /// The type of the element.
        /// </summary>
        private ElementType elementType = default(ElementType);

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        public ElementType ElementType
        {
            get
            {
                return elementType;
            }
        }
        #endregion Property: ElementType

        #region Property: MetalType
        /// <summary>
        /// The type of the metal.
        /// </summary>
        private MetalType metalType = default(MetalType);

        /// <summary>
        /// Gets the type of the metal.
        /// </summary>
        public MetalType MetalType
        {
            get
            {
                return metalType;
            }
        }
        #endregion Property: MetalType

        #region Property: Group
        /// <summary>
        /// The group (or family) of the element.
        /// </summary>
        private ElementGroup group = default(ElementGroup);

        /// <summary>
        /// Gets the group (or family) of the element.
        /// </summary>
        public ElementGroup Group
        {
            get
            {
                return group;
            }
        }
        #endregion Property: Group

        #region Property: Period
        /// <summary>
        /// The period of the element.
        /// </summary>
        private ElementPeriod period = default(ElementPeriod);

        /// <summary>
        /// Gets the period of the element.
        /// </summary>
        public ElementPeriod Period
        {
            get
            {
                return period;
            }
        }
        #endregion Property: Period

        #region Property: Block
        /// <summary>
        /// The block of the element.
        /// </summary>
        private ElementBlock block = default(ElementBlock);

        /// <summary>
        /// Gets the block of the element.
        /// </summary>
        public ElementBlock Block
        {
            get
            {
                return block;
            }
        }
        #endregion Property: Block

        #region Property: StandardState
        /// <summary>
        /// The standard state of the element.
        /// </summary>
        private StateOfMatter standardState = default(StateOfMatter);

        /// <summary>
        /// Gets the standard state of the element.
        /// </summary>
        public StateOfMatter StandardState
        {
            get
            {
                return standardState;
            }
        }
        #endregion Property: StandardState

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ElementBase(Element element)
        /// <summary>
        /// Creates a new element base from the given element.
        /// </summary>
        /// <param name="element">The element to create an element base from.</param>
        protected internal ElementBase(Element element)
            : base(element)
        {
            if (element != null)
            {
                this.atomicNumber = element.AtomicNumber;
                this.symbol = element.Symbol;
                this.elementType = element.ElementType;
                this.metalType = element.MetalType;
                this.group = element.Group;
                this.period = element.Period;
                this.block = element.Block;
                this.standardState = element.StandardState;
            }
        }
        #endregion Constructor: ElementBase(Element element)

        #region Constructor: ElementBase(ElementValued elementValued)
        /// <summary>
        /// Creates a new element base from the given valued element.
        /// </summary>
        /// <param name="elementValued">The valued element to create an element base from.</param>
        protected internal ElementBase(ElementValued elementValued)
            : base(elementValued)
        {
            if (elementValued != null && elementValued.Element != null)
            {
                this.atomicNumber = elementValued.Element.AtomicNumber;
                this.symbol = elementValued.Element.Symbol;
                this.elementType = elementValued.Element.ElementType;
                this.metalType = elementValued.Element.MetalType;
                this.group = elementValued.Element.Group;
                this.period = elementValued.Element.Period;
                this.block = elementValued.Element.Block;
                this.standardState = elementValued.Element.StandardState;
            }
        }
        #endregion Constructor: ElementBase(ElementValued elementValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: ElementBase

    #region Class: ElementValuedBase
    /// <summary>
    /// A base of a valued element.
    /// </summary>
    public class ElementValuedBase : PhysicalEntityValuedBase
    {

        #region Properties and Fields

        #region Property: ElementValued
        /// <summary>
        /// Gets the valued element of which this is a valued element base.
        /// </summary>
        protected internal ElementValued ElementValued
        {
            get
            {
                return this.NodeValued as ElementValued;
            }
        }
        #endregion Property: ElementValued

        #region Property: ElementBase
        /// <summary>
        /// Gets the element of which this is a valued element base.
        /// </summary>
        public ElementBase ElementBase
        {
            get
            {
                return this.NodeBase as ElementBase;
            }
        }
        #endregion Property: ElementBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ElementValuedBase(ElementValued elementValued)
        /// <summary>
        /// Create a valued element base from the given valued element.
        /// </summary>
        /// <param name="elementValued">The valued element to create a valued element base from.</param>
        protected internal ElementValuedBase(ElementValued elementValued)
            : base(elementValued)
        {
        }
        #endregion Constructor: ElementValuedBase(ElementValued elementValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: ElementValuedBase

    #region Class: ElementConditionBase
    /// <summary>
    /// A condition on an element.
    /// </summary>
    public class ElementConditionBase : PhysicalEntityConditionBase
    {

        #region Properties and Fields

        #region Property: ElementCondition
        /// <summary>
        /// Gets the element condition of which this is an element condition base.
        /// </summary>
        protected internal ElementCondition ElementCondition
        {
            get
            {
                return this.Condition as ElementCondition;
            }
        }
        #endregion Property: ElementCondition

        #region Property: ElementBase
        /// <summary>
        /// Gets the element base of which this is an element condition base.
        /// </summary>
        public ElementBase ElementBase
        {
            get
            {
                return this.NodeBase as ElementBase;
            }
        }
        #endregion Property: ElementBase

        #region Property: AtomicNumber
        /// <summary>
        /// The atomic number.
        /// </summary>
        private uint? atomicNumber = null;

        /// <summary>
        /// Gets the atomic number.
        /// </summary>
        public uint? AtomicNumber
        {
            get
            {
                return atomicNumber;
            }
        }
        #endregion Property: AtomicNumber

        #region Property: AtomicNumberSign
        /// <summary>
        /// The sign for the atomic number in the condition.
        /// </summary>
        private EqualitySignExtended? atomicNumberSign = null;

        /// <summary>
        /// Gets the sign for the atomic number in the condition.
        /// </summary>
        public EqualitySignExtended? AtomicNumberSign
        {
            get
            {
                return atomicNumberSign;
            }
        }
        #endregion Property: AtomicNumberSign

        #region Property: Symbol
        /// <summary>
        /// The symbol.
        /// </summary>
        private String symbol = null;

        /// <summary>
        /// Gets the symbol.
        /// </summary>
        public String Symbol
        {
            get
            {
                return symbol;
            }
        }
        #endregion Property: Symbol

        #region Property: SymbolSign
        /// <summary>
        /// The sign for the symbol in the condition.
        /// </summary>
        private EqualitySign? symbolSign = null;

        /// <summary>
        /// Gets the sign for the symbol in the condition.
        /// </summary>
        public EqualitySign? SymbolSign
        {
            get
            {
                return symbolSign;
            }
        }
        #endregion Property: SymbolSign

        #region Property: ElementType
        /// <summary>
        /// The type of the element.
        /// </summary>
        private ElementType? elementType = null;

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        public ElementType? ElementType
        {
            get
            {
                return elementType;
            }
        }
        #endregion Property: ElementType

        #region Property: ElementTypeSign
        /// <summary>
        /// The sign for the type of the element in the condition.
        /// </summary>
        private EqualitySign? elementTypeSign = null;

        /// <summary>
        /// Gets the sign for the type of the element in the condition.
        /// </summary>
        public EqualitySign? ElementTypeSign
        {
            get
            {
                return elementTypeSign;
            }
        }
        #endregion Property: ElementTypeSign

        #region Property: MetalType
        /// <summary>
        /// The type of the metal.
        /// </summary>
        private MetalType? metalType = null;

        /// <summary>
        /// Gets the type of the metal.
        /// </summary>
        public MetalType? MetalType
        {
            get
            {
                return metalType;
            }
        }
        #endregion Property: MetalType

        #region Property: MetalTypeSign
        /// <summary>
        /// The sign for the type of the metal in the condition.
        /// </summary>
        private EqualitySign? metalTypeSign = null;

        /// <summary>
        /// Gets the sign for the type of the metal in the condition.
        /// </summary>
        public EqualitySign? MetalTypeSign
        {
            get
            {
                return metalTypeSign;
            }
        }
        #endregion Property: MetalTypeSign

        #region Property: Group
        /// <summary>
        /// The group (or family) of the element.
        /// </summary>
        private ElementGroup? group = null;

        /// <summary>
        /// Gets the group (or family) of the element.
        /// </summary>
        public ElementGroup? Group
        {
            get
            {
                return group;
            }
        }
        #endregion Property: Group

        #region Property: GroupSign
        /// <summary>
        /// The sign for the group (or family) of the element in the condition.
        /// </summary>
        private EqualitySign? groupSign = null;

        /// <summary>
        /// Gets the sign for the group (or family) of the element in the condition.
        /// </summary>
        public EqualitySign? GroupSign
        {
            get
            {
                return groupSign;
            }
        }
        #endregion Property: GroupSign

        #region Property: Period
        /// <summary>
        /// The period of the element.
        /// </summary>
        private ElementPeriod? period = null;

        /// <summary>
        /// Gets the period of the element.
        /// </summary>
        public ElementPeriod? Period
        {
            get
            {
                return period;
            }
        }
        #endregion Property: Period

        #region Property: PeriodSign
        /// <summary>
        /// The sign for the period of the element in the condition.
        /// </summary>
        private EqualitySign? periodSign = null;

        /// <summary>
        /// Gets the sign for the period of the element in the condition.
        /// </summary>
        public EqualitySign? PeriodSign
        {
            get
            {
                return periodSign;
            }
        }
        #endregion Property: PeriodSign

        #region Property: Block
        /// <summary>
        /// The block of the element.
        /// </summary>
        private ElementBlock? block = null;

        /// <summary>
        /// Gets the block of the element.
        /// </summary>
        public ElementBlock? Block
        {
            get
            {
                return block;
            }
        }
        #endregion Property: Block

        #region Property: BlockSign
        /// <summary>
        /// The sign for the block of the element in the condition.
        /// </summary>
        private EqualitySign? blockSign = null;

        /// <summary>
        /// Gets the sign for the block of the element in the condition.
        /// </summary>
        public EqualitySign? BlockSign
        {
            get
            {
                return blockSign;
            }
        }
        #endregion Property: BlockSign

        #region Property: StandardState
        /// <summary>
        /// The standard state of the element.
        /// </summary>
        private StateOfMatter? standardState = null;

        /// <summary>
        /// Gets the standard state of the element.
        /// </summary>
        public StateOfMatter? StandardState
        {
            get
            {
                return standardState;
            }
        }
        #endregion Property: StandardState

        #region Property: StandardStateSign
        /// <summary>
        /// The sign for the standard state of the element in the condition.
        /// </summary>
        private EqualitySign? standardStateSign = null;

        /// <summary>
        /// Gets the sign for the standard state of the element in the condition.
        /// </summary>
        public EqualitySign? StandardStateSign
        {
            get
            {
                return standardStateSign;
            }
        }
        #endregion Property: StandardStateSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ElementConditionBase(ElementCondition elementCondition)
        /// <summary>
        /// Creates a base of the given element condition.
        /// </summary>
        /// <param name="elementCondition">The element condition to create a base of.</param>
        protected internal ElementConditionBase(ElementCondition elementCondition)
            : base(elementCondition)
        {
            if (elementCondition != null)
            {
                this.atomicNumber = elementCondition.AtomicNumber;
                this.atomicNumberSign = elementCondition.AtomicNumberSign;
                this.symbol = elementCondition.Symbol;
                this.symbolSign = elementCondition.SymbolSign;
                this.elementType = elementCondition.ElementType;
                this.elementTypeSign = elementCondition.ElementTypeSign;
                this.metalType = elementCondition.MetalType;
                this.metalTypeSign = elementCondition.MetalTypeSign;
                this.group = elementCondition.Group;
                this.groupSign = elementCondition.GroupSign;
                this.period = elementCondition.Period;
                this.periodSign = elementCondition.PeriodSign;
                this.block = elementCondition.Block;
                this.blockSign = elementCondition.BlockSign;
                this.standardState = elementCondition.StandardState;
                this.standardStateSign = elementCondition.StandardStateSign;
            }
        }
        #endregion Constructor: ElementConditionBase(ElementCondition elementCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: ElementConditionBase

    #region Class: ElementChangeBase
    /// <summary>
    /// A change on an element.
    /// </summary>
    public class ElementChangeBase : PhysicalEntityChangeBase
    {

        #region Properties and Fields

        #region Property: ElementChange
        /// <summary>
        /// Gets the element change of which this is an element change base.
        /// </summary>
        protected internal ElementChange ElementChange
        {
            get
            {
                return this.Change as ElementChange;
            }
        }
        #endregion Property: ElementChange

        #region Property: ElementBase
        /// <summary>
        /// Gets the affected element base.
        /// </summary>
        public ElementBase ElementBase
        {
            get
            {
                return this.NodeBase as ElementBase;
            }
        }
        #endregion Property: ElementBase

        #region Property: AtomicNumber
        /// <summary>
        /// The atomic number to change to.
        /// </summary>
        private byte? atomicNumber = null;

        /// <summary>
        /// Gets the atomic number to change to.
        /// </summary>
        public byte? AtomicNumber
        {
            get
            {
                return atomicNumber;
            }
        }
        #endregion Property: AtomicNumber

        #region Property: Symbol
        /// <summary>
        /// The symbol to change to.
        /// </summary>
        private String symbol = null;

        /// <summary>
        /// Gets the symbol to change to.
        /// </summary>
        public String Symbol
        {
            get
            {
                return symbol;
            }
        }
        #endregion Property: Symbol

        #region Property: ElementType
        /// <summary>
        /// The type of the element to change to.
        /// </summary>
        private ElementType? elementType = null;

        /// <summary>
        /// Gets the type of the element to change to.
        /// </summary>
        public ElementType? ElementType
        {
            get
            {
                return elementType;
            }
        }
        #endregion Property: ElementType

        #region Property: MetalType
        /// <summary>
        /// The type of the metal to change to.
        /// </summary>
        private MetalType? metalType = null;

        /// <summary>
        /// Gets the type of the metal to change to.
        /// </summary>
        public MetalType? MetalType
        {
            get
            {
                return metalType;
            }
        }
        #endregion Property: MetalType

        #region Property: Group
        /// <summary>
        /// The group (or family) of the element to change to.
        /// </summary>
        private ElementGroup? group = null;

        /// <summary>
        /// Gets the group (or family) of the element to change to.
        /// </summary>
        public ElementGroup? Group
        {
            get
            {
                return group;
            }
        }
        #endregion Property: Group

        #region Property: Period
        /// <summary>
        /// The period of the element to change to.
        /// </summary>
        private ElementPeriod? period = null;

        /// <summary>
        /// Gets the period of the element to change to.
        /// </summary>
        public ElementPeriod? Period
        {
            get
            {
                return period;
            }
        }
        #endregion Property: Period

        #region Property: Block
        /// <summary>
        /// The block of the element to change to.
        /// </summary>
        private ElementBlock? block = null;

        /// <summary>
        /// Gets the block of the element to change to.
        /// </summary>
        public ElementBlock? Block
        {
            get
            {
                return block;
            }
        }
        #endregion Property: Block

        #region Property: StandardState
        /// <summary>
        /// The standard state of the element to change to.
        /// </summary>
        private StateOfMatter? standardState = null;

        /// <summary>
        /// Gets the standard state of the element to change to.
        /// </summary>
        public StateOfMatter? StandardState
        {
            get
            {
                return standardState;
            }
        }
        #endregion Property: StandardState

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ElementChangeBase(ElementChange elementChange)
        /// <summary>
        /// Creates a base of the given element change.
        /// </summary>
        /// <param name="elementChange">The element change to create a base of.</param>
        protected internal ElementChangeBase(ElementChange elementChange)
            : base(elementChange)
        {
            if (elementChange != null)
            {
                this.atomicNumber = elementChange.AtomicNumber;
                this.symbol = elementChange.Symbol;
                this.elementType = elementChange.ElementType;
                this.metalType = elementChange.MetalType;
                this.group = elementChange.Group;
                this.period = elementChange.Period;
                this.block = elementChange.Block;
                this.standardState = elementChange.StandardState;
            }
        }
        #endregion Constructor: ElementChangeBase(ElementChange elementChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: ElementChangeBase

}