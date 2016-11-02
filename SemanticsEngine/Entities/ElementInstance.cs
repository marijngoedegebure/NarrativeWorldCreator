/**************************************************************************
 * 
 * ElementInstance.cs
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
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;

namespace SemanticsEngine.Entities
{

    #region Class: ElementInstance
    /// <summary>
    /// An instance of an element.
    /// </summary>
    public class ElementInstance : PhysicalEntityInstance
    {

        #region Properties and Fields

        #region Property: ElementBase
        /// <summary>
        /// Gets the element base of which this is an element instance.
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
        /// Gets the atomic number.
        /// </summary>
        public byte AtomicNumber
        {
            get
            {
                if (this.ElementBase != null)
                    return GetProperty<byte>("AtomicNumber", this.ElementBase.AtomicNumber);
                
                return SemanticsSettings.Values.AtomicNumber;
            }
            protected set
            {
                if (this.AtomicNumber != value)
                    SetProperty("AtomicNumber", value);
            }
        }
        #endregion Property: AtomicNumber

        #region Property: Symbol
        /// <summary>
        /// Gets the symbol.
        /// </summary>
        public String Symbol
        {
            get
            {
                if (this.ElementBase != null)
                    return GetProperty<String>("Symbol", this.ElementBase.Symbol);
                
                return SemanticsSettings.Values.Symbol;
            }
            protected set
            {
                if (this.Symbol != value)
                    SetProperty("Symbol", value);
            }
        }
        #endregion Property: Symbol

        #region Property: ElementType
        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        public ElementType ElementType
        {
            get
            {
                if (this.ElementBase != null)
                    return GetProperty<ElementType>("ElementType", this.ElementBase.ElementType);
                
                return default(ElementType);
            }
            protected set
            {
                if (this.ElementType != value)
                    SetProperty("ElementType", value);
            }
        }
        #endregion Property: ElementType

        #region Property: MetalType
        /// <summary>
        /// Gets the type of the metal.
        /// </summary>
        public MetalType MetalType
        {
            get
            {
                if (this.ElementBase != null)
                    return GetProperty<MetalType>("MetalType", this.ElementBase.MetalType);
                
                return default(MetalType);
            }
            protected set
            {
                if (this.MetalType != value)
                    SetProperty("MetalType", value);
            }
        }
        #endregion Property: MetalType

        #region Property: Group
        /// <summary>
        /// Gets the group (or family) of the element.
        /// </summary>
        public ElementGroup Group
        {
            get
            {
                if (this.ElementBase != null)
                    return GetProperty<ElementGroup>("Group", this.ElementBase.Group);
                
                return default(ElementGroup);
            }
            protected set
            {
                if (this.Group != value)
                    SetProperty("Group", value);
            }
        }
        #endregion Property: Group

        #region Property: Period
        /// <summary>
        /// Gets the period of the element.
        /// </summary>
        public ElementPeriod Period
        {
            get
            {
                if (this.ElementBase != null)
                    return GetProperty<ElementPeriod>("Period", this.ElementBase.Period);
                
                return default(ElementPeriod);
            }
            protected set
            {
                if (this.Period != value)
                    SetProperty("Period", value);
            }
        }
        #endregion Property: Period

        #region Property: Block
        /// <summary>
        /// Gets the block of the element.
        /// </summary>
        public ElementBlock Block
        {
            get
            {
                if (this.ElementBase != null)
                    return GetProperty<ElementBlock>("Block", this.ElementBase.Block);
                
                return default(ElementBlock);
            }
            protected set
            {
                if (this.Block != value)
                    SetProperty("Block", value);
            }
        }
        #endregion Property: Block

        #region Property: StandardState
        /// <summary>
        /// Gets the standard state of the element.
        /// </summary>
        public StateOfMatter StandardState
        {
            get
            {
                if (this.ElementBase != null)
                    return GetProperty<StateOfMatter>("StandardState", this.ElementBase.StandardState);
                
                return default(StateOfMatter);
            }
            protected set
            {
                if (this.StandardState != value)
                    SetProperty("StandardState", value);
            }
        }
        #endregion Property: StandardState

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ElementInstance(ElementBase elementBase)
        /// <summary>
        /// Creates a new element instance from the given element base.
        /// </summary>
        /// <param name="elementBase">The element base to create the element instance from.</param>
        internal ElementInstance(ElementBase elementBase)
            : base(elementBase)
        {
        }
        #endregion Constructor: ElementInstance(ElementBase elementBase)

        #region Constructor: ElementInstance(ElementInstance elementInstance)
        /// <summary>
        /// Clones an element instance.
        /// </summary>
        /// <param name="elementInstance">The element instance to clone.</param>
        protected internal ElementInstance(ElementInstance elementInstance)
            : base(elementInstance)
        {
            if (elementInstance != null)
            {
                this.AtomicNumber = elementInstance.AtomicNumber;
                this.Symbol = elementInstance.Symbol;
                this.ElementType = elementInstance.ElementType;
                this.MetalType = elementInstance.MetalType;
                this.Group = elementInstance.Group;
                this.Period = elementInstance.Period;
                this.Block = elementInstance.Block;
                this.StandardState = elementInstance.StandardState;
            }
        }
        #endregion Constructor: ElementInstance(ElementInstance elementInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the element instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the element instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the element instance is satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            // Check whether the base satisfies the condition
            if (conditionBase != null && base.Satisfies(conditionBase, iVariableInstanceHolder))
            {
                ElementConditionBase elementConditionBase = conditionBase as ElementConditionBase;
                if (elementConditionBase != null)
                {
                    // Check whether all the properties have the correct values
                    if ((elementConditionBase.AtomicNumberSign == null || elementConditionBase.AtomicNumber == null || Toolbox.Compare(this.AtomicNumber, (EqualitySignExtended)elementConditionBase.AtomicNumberSign, (uint)elementConditionBase.AtomicNumber)) &&
                        (elementConditionBase.SymbolSign == null || elementConditionBase.Symbol == null || Toolbox.Compare(this.Symbol, (EqualitySign)elementConditionBase.SymbolSign, elementConditionBase.Symbol)) &&
                        (elementConditionBase.ElementTypeSign == null || elementConditionBase.ElementType == null || Toolbox.Compare(this.ElementType, (EqualitySign)elementConditionBase.ElementTypeSign, (ElementType)elementConditionBase.ElementType)) &&
                        (elementConditionBase.MetalTypeSign == null || elementConditionBase.MetalType == null || Toolbox.Compare(this.MetalType, (EqualitySign)elementConditionBase.MetalTypeSign, (MetalType)elementConditionBase.MetalType)) &&
                        (elementConditionBase.GroupSign == null || elementConditionBase.Group == null || Toolbox.Compare(this.Group, (EqualitySign)elementConditionBase.GroupSign, (ElementGroup)elementConditionBase.Group)) &&
                        (elementConditionBase.PeriodSign == null || elementConditionBase.Period == null || Toolbox.Compare(this.Period, (EqualitySign)elementConditionBase.PeriodSign, (ElementPeriod)elementConditionBase.Period)) &&
                        (elementConditionBase.BlockSign == null || elementConditionBase.Block == null || Toolbox.Compare(this.Block, (EqualitySign)elementConditionBase.BlockSign, (ElementBlock)elementConditionBase.Block)) &&
                        (elementConditionBase.StandardStateSign == null || elementConditionBase.StandardState == null || Toolbox.Compare(this.StandardState, (EqualitySign)elementConditionBase.StandardStateSign, (StateOfMatter)elementConditionBase.StandardState)))
                        return true;
                }
                else
                    return true;
            }
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the change to the element instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the element instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null && base.Apply(changeBase, iVariableInstanceHolder))
            {
                ElementChangeBase elementChangeBase = changeBase as ElementChangeBase;
                if (elementChangeBase != null)
                {
                    // Apply all changes
                    if (elementChangeBase.AtomicNumber != null)
                        this.AtomicNumber = (byte)elementChangeBase.AtomicNumber;
                    if (!elementChangeBase.Symbol.Equals(string.Empty))
                        this.Symbol = elementChangeBase.Symbol;
                    if (elementChangeBase.ElementType != null)
                        this.ElementType = (ElementType)elementChangeBase.ElementType;
                    if (elementChangeBase.MetalType != null)
                        this.MetalType = (MetalType)elementChangeBase.MetalType;
                    if (elementChangeBase.Group != null)
                        this.Group = (ElementGroup)elementChangeBase.Group;
                    if (elementChangeBase.Period != null)
                        this.Period = (ElementPeriod)elementChangeBase.Period;
                    if (elementChangeBase.Block != null)
                        this.Block = (ElementBlock)elementChangeBase.Block;
                    if (elementChangeBase.StandardState != null)
                        this.StandardState = (StateOfMatter)elementChangeBase.StandardState;
                }
                return true;
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: ElementInstance

}