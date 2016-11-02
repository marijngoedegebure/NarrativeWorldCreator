/**************************************************************************
 * 
 * MatterInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Entities
{

    #region Class: MatterInstance
    /// <summary>
    /// An instance of matter.
    /// </summary>
    public abstract class MatterInstance : PhysicalEntityInstance
    {

        #region Events, Properties, and Fields

        #region Events: ElementHandler
        /// <summary>
        /// A handler for added or removed elements.
        /// </summary>
        /// <param name="sender">The matter instance the element was added to or removed from.</param>
        /// <param name="element">The added or removed element.</param>
        public delegate void ElementHandler(MatterInstance sender, ElementInstance element);

        /// <summary>
        /// An event to indicate an added element.
        /// </summary>
        public event ElementHandler ElementAdded;

        /// <summary>
        /// An event to indicate a removed element.
        /// </summary>
        public event ElementHandler ElementRemoved;
        #endregion Events: ElementHandler

        #region Property: MatterBase
        /// <summary>
        /// Gets the matter base of which this is a matter instance.
        /// </summary>
        public MatterBase MatterBase
        {
            get
            {
                return this.NodeBase as MatterBase;
            }
        }
        #endregion Property: MatterBase

        #region Property: MatterValuedBase
        /// <summary>
        /// Gets the valued matter base of which this is a matter instance.
        /// </summary>
        public MatterValuedBase MatterValuedBase
        {
            get
            {
                return this.Base as MatterValuedBase;
            }
        }
        #endregion Property: MatterValuedBase

        #region Property: Quantity
        /// <summary>
        /// The event handler for a change in the quantity.
        /// </summary>
        private PropertyChangedEventHandler quantityChanged = null;
        
        /// <summary>
        /// Gets the quantity.
        /// </summary>
        public NumericalValueInstance Quantity
        {
            get
            {
                // Return the locally modified value, or create a new instance from the base and subscribe to possible changes
                NumericalValueInstance quantity = GetProperty<NumericalValueInstance>("Quantity", null);
                if (quantity == null)
                {
                    if (this.MatterValuedBase != null)
                        quantity = new NumericalValueInstance(this.MatterValuedBase.Quantity.GetRandomValue(null));
                    
                    if (quantity == null)
                        quantity = new NumericalValueInstance(0);

                    quantity.Min = 0;

                    if (quantityChanged == null)
                        quantityChanged = new PropertyChangedEventHandler(quantity_PropertyChanged);

                    quantity.PropertyChanged += quantityChanged;
                }
                return quantity;
            }
            internal set
            {
                if (this.Quantity != value)
                {
                    if (quantityChanged != null && this.Quantity != null)
                        this.Quantity.PropertyChanged -= quantityChanged;

                    if (value == null)
                        value = new NumericalValueInstance(0);

                    if (quantityChanged == null)
                        quantityChanged = new PropertyChangedEventHandler(quantity_PropertyChanged);

                    value.PropertyChanged += quantityChanged;

                    SetProperty("Quantity", value);

                    // If the quantity is exhausted, remove the instance
                    if (value.BaseValue <= 0 && this.World != null)
                        this.World.RemoveInstance(this);
                }
            }
        }

        /// <summary>
        /// If the quantity changes, add the property and the modified quantity to the modifications table.
        /// </summary>
        private void quantity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != null)
            {
                SetProperty("Quantity", sender);

                if (e.PropertyName.Equals("Value"))
                {
                    // If the quantity is exhausted, remove the instance
                    if (this.Quantity.BaseValue <= 0 && this.World != null)
                        this.World.RemoveInstance(this);
                }
            }
        }
        #endregion Property: Quantity

        #region Property: StateOfMatter
        /// <summary>
        /// Gets the state of matter.
        /// </summary>
        public StateOfMatter StateOfMatter
        {
            get
            {
                if (this.MatterValuedBase != null)
                    return GetProperty<StateOfMatter>("StateOfMatter", this.MatterValuedBase.StateOfMatter);
                else if (this.MatterBase != null)
                    return GetProperty<StateOfMatter>("StateOfMatter", this.MatterBase.DefaultStateOfMatter);
                
                return default(StateOfMatter);
            }
            protected set
            {
                if (this.StateOfMatter != value)
                    SetProperty("StateOfMatter", value);
            }
        }
        #endregion Property: StateOfMatter

        #region Property: Elements
        /// <summary>
        /// The elements of the matter instance.
        /// </summary>
        private ElementInstance[] elements = null;
        
        /// <summary>
        /// Gets the elements of the matter instance.
        /// </summary>
        public ReadOnlyCollection<ElementInstance> Elements
        {
            get
            {
                if (elements != null)
                    return new ReadOnlyCollection<ElementInstance>(elements);

                return new List<ElementInstance>(0).AsReadOnly();
            }
        }
        #endregion Property: Elements

        #region Property: ChemicalFormula
        /// <summary>
        /// Gets the chemical formula.
        /// </summary>
        public String ChemicalFormula
        {
            get
            {
                if (this.MatterBase != null)
                    return GetProperty<String>("ChemicalFormula", this.MatterBase.ChemicalFormula);
                
                return String.Empty;
            }
            protected set
            {
                if (this.ChemicalFormula != value)
                    SetProperty("ChemicalFormula", value);
            }
        }
        #endregion Property: ChemicalFormula

        #region Property: TangibleObject
        /// <summary>
        /// The tangible object of which this may be the matter.
        /// </summary>
        private TangibleObjectInstance tangibleObject = null;

        /// <summary>
        /// Gets the tangible object of which this may be the matter.
        /// </summary>
        public TangibleObjectInstance TangibleObject
        {
            get
            {
                return tangibleObject;
            }
            internal set
            {
                if (tangibleObject != value)
                {
                    tangibleObject = value;
                    NotifyPropertyChanged("TangibleObject");
                }
            }
        }
        #endregion Property: TangibleObject

        #region Property: Space
        /// <summary>
        /// The space in which this may be tangible matter.
        /// </summary>
        private SpaceInstance space = null;

        /// <summary>
        /// Gets the space in which this may be tangible matter.
        /// </summary>
        public SpaceInstance Space
        {
            get
            {
                return space;
            }
            internal set
            {
                if (space != value)
                {
                    space = value;
                    NotifyPropertyChanged("Space");
                }
            }
        }
        #endregion Property: Space

        #region Property: Applicant
        /// <summary>
        /// The tangible object instance this matter is applied to as a layer.
        /// </summary>
        private TangibleObjectInstance applicant = null;

        /// <summary>
        /// Gets the tangible object instance this matter is applied to as a layer.
        /// </summary>
        public TangibleObjectInstance Applicant
        {
            get
            {
                return applicant;
            }
            internal set
            {
                if (applicant != value)
                {
                    applicant = value;
                    NotifyPropertyChanged("Applicant");
                }
            }
        }
        #endregion Property: Applicant

        #region Property: LayerIndex
        /// <summary>
        /// The index of the layer when it is stacked on top of or underneath other layers: "the n-th layer".
        /// </summary>
        private int layerIndex = -1;
        
        /// <summary>
        /// Gets the index of the layer when it is stacked on top of or underneath other layers: "the n-th layer".
        /// </summary>
        public int LayerIndex
        {
            get
            {
                return layerIndex;
            }
            set
            {
                if (layerIndex != value)
                {
                    layerIndex = value;
                    NotifyPropertyChanged("LayerIndex");
                }
            }
        }
        #endregion Property: LayerIndex

        #endregion Events, Properties, and Fields

        #region Method Group: Constructors

        #region Constructor: MatterInstance(MatterBase matterBase)
        /// <summary>
        /// Creates a new matter instance from the given matter base.
        /// </summary>
        /// <param name="matterValuedBase">The matter base to create the matter instance from.</param>
        protected MatterInstance(MatterBase matterBase)
            : this(matterBase, false)
        {
        }

        /// <summary>
        /// Creates a new matter instance from the given matter base.
        /// </summary>
        /// <param name="matterValuedBase">The matter base to create the matter instance from.</param>
        /// <param name="ignoreCreation">Indicates whether creation of elements should be ignored.</param>
        protected MatterInstance(MatterBase matterBase, bool ignoreCreation)
            : base(matterBase)
        {
            if (matterBase != null)
            {
                this.StateOfMatter = matterBase.DefaultStateOfMatter;

                if (!ignoreCreation)
                {
                    // Create instances from the mandatory base elements
                    foreach (ElementValuedBase elementValuedBase in this.MatterBase.Elements)
                    {
                        if ((InstanceManager.IgnoreNecessity || elementValuedBase.Necessity == Necessity.Mandatory) && elementValuedBase.Quantity != null)
                        {
                            for (int i = 0; i < elementValuedBase.Quantity.GetRandomInteger(this); i++)
                                AddElement(InstanceManager.Current.Create<ElementInstance>(elementValuedBase.ElementBase));
                        }
                    }
                }
            }
        }
        #endregion Constructor: MatterInstance(MatterBase matterBase)

        #region Constructor: MatterInstance(MatterValuedBase matterValuedBase)
        /// <summary>
        /// Creates a new matter instance from the given valued matter base.
        /// </summary>
        /// <param name="matterValuedBase">The valued matter base to create the matter instance from.</param>
        protected MatterInstance(MatterValuedBase matterValuedBase)
            : this(matterValuedBase, false)
        {
        }

        /// <summary>
        /// Creates a new matter instance from the given valued matter base.
        /// </summary>
        /// <param name="matterValuedBase">The valued matter base to create the matter instance from.</param>
        /// <param name="ignoreCreation">Indicates whether creation of elements should be ignored.</param>
        protected MatterInstance(MatterValuedBase matterValuedBase, bool ignoreCreation)
            : base(matterValuedBase)
        {
            if (!ignoreCreation && matterValuedBase != null)
            {
                // Create instances from the mandatory base elements
                foreach (ElementValuedBase elementValuedBase in this.MatterBase.Elements)
                {
                    if ((InstanceManager.IgnoreNecessity || elementValuedBase.Necessity == Necessity.Mandatory) && elementValuedBase.Quantity != null)
                    {
                        for (int i = 0; i < elementValuedBase.Quantity.GetRandomInteger(this); i++)
                            AddElement(InstanceManager.Current.Create<ElementInstance>(elementValuedBase.ElementBase));
                    }
                }
            }
        }
        #endregion Constructor: MatterInstance(MatterValuedBase matterValuedBase)

        #region Constructor: MatterInstance(LayerBase layerBase)
        /// <summary>
        /// Creates a new matter instance from the given layer base.
        /// </summary>
        /// <param name="layerBase">The layer base to create the matter instance from.</param>
        internal MatterInstance(LayerBase layerBase)
            : base(layerBase)
        {
            if (layerBase != null)
            {
                // Set the index of the layer
                this.LayerIndex = layerBase.Index;
            }
        }
        #endregion Constructor: MatterInstance(LayerBase layerBase)

        #region Constructor: MatterInstance(MatterInstance matterInstance)
        /// <summary>
        /// Clones a matter instance.
        /// </summary>
        /// <param name="matterInstance">The matter instance to clone.</param>
        protected MatterInstance(MatterInstance matterInstance)
            : base(matterInstance)
        {
            if (matterInstance != null)
            {
                if (matterInstance.Quantity != null)
                    this.Quantity = new NumericalValueInstance(matterInstance.Quantity);
                this.StateOfMatter = matterInstance.StateOfMatter;
                foreach (ElementInstance elementInstance in matterInstance.Elements)
                    AddElement(new ElementInstance(elementInstance));
                this.ChemicalFormula = matterInstance.ChemicalFormula;
                this.TangibleObject = matterInstance.TangibleObject;
                this.Space = matterInstance.Space;
                this.Applicant = matterInstance.Applicant;
                this.LayerIndex = matterInstance.LayerIndex;
            }
        }
        #endregion Constructor: MatterInstance(MatterInstance matterInstance)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddElement(ElementInstance elementInstance)
        /// <summary>
        /// Adds an element instance.
        /// </summary>
        /// <param name="elementInstance">The element instance to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        protected internal Message AddElement(ElementInstance elementInstance)
        {
            if (elementInstance != null)
            {
                // If the element instance is already available in all elements, there is no use to add it
                if (this.Elements.Contains(elementInstance))
                    return Message.RelationExistsAlready;

                // Add the element instance
                Utils.AddToArray<ElementInstance>(ref this.elements, elementInstance);
                NotifyPropertyChanged("Elements");

                // Add the symbol of the element to the chemical formula
                this.ChemicalFormula += elementInstance.Symbol;

                // Set the position
                elementInstance.Position = this.Position;

                // Set the world if this has not been done before
                if (elementInstance.World == null && this.World != null)
                    this.World.AddInstance(elementInstance);

                // Invoke an event
                if (ElementAdded != null)
                    ElementAdded.Invoke(this, elementInstance);

                // Notify the engine
                if (SemanticsEngine.Current != null)
                    SemanticsEngine.Current.HandleElementAdded(this, elementInstance);

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddElement(ElementInstance elementInstance)

        #region Method: RemoveElement(ElementInstance elementInstance)
        /// <summary>
        /// Removes an element instance.
        /// </summary>
        /// <param name="elementInstance">The element instance to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        protected internal Message RemoveElement(ElementInstance elementInstance)
        {
            if (elementInstance != null)
            {
                if (this.Elements.Contains(elementInstance))
                {
                    // Remove the symbol of the element of the chemical element
                    int index = this.ChemicalFormula.IndexOf(elementInstance.Symbol, StringComparison.Ordinal);
                    this.ChemicalFormula = this.ChemicalFormula.Remove(index, elementInstance.Symbol.Length);

                    // Remove the element instance
                    Utils.RemoveFromArray<ElementInstance>(ref this.elements, elementInstance);
                    NotifyPropertyChanged("Elements");

                    // Invoke an event
                    if (ElementRemoved != null)
                        ElementRemoved.Invoke(this, elementInstance);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleElementRemoved(this, elementInstance);

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveElement(ElementInstance elementInstance)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasElement(ElementBase elementBase)
        /// <summary>
        /// Checks whether the matter instance has the given element.
        /// </summary>
        /// <param name="elementBase">The element that should be checked.</param>
        /// <returns>Returns true when the matter instance has the element.</returns>
        public bool HasElement(ElementBase elementBase)
        {
            if (elementBase != null)
            {
                foreach (ElementInstance elementInstance in this.Elements)
                {
                    if (elementBase.Equals(elementInstance.ElementBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasElement(ElementBase elementBase)

        #region Method: MarkAsModified(bool modified, bool spread)
        /// <summary>
        /// Mark this instance as modified.
        /// </summary>
        /// <param name="modified">The value that indicates whether the instance has been modified.</param>
        /// <param name="spread">The value that indicates whether the marking should spread further.</param>
        internal override void MarkAsModified(bool modified, bool spread)
        {
            base.MarkAsModified(modified, spread);

            if (spread)
            {
                foreach (ElementInstance element in this.Elements)
                    element.MarkAsModified(modified, false);

                if (this.TangibleObject != null)
                    this.TangibleObject.MarkAsModified(modified, false);
                if (this.Space != null)
                    this.Space.MarkAsModified(modified, false);
            }
        }
        #endregion Method: MarkAsModified(bool modified, bool spread)

        #region Method: SetPosition(Vec3 newPosition)
        /// <summary>
        /// Set the new position.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        protected override void SetPosition(Vec3 newPosition)
        {
            Vec3 newPos = new Vec3(newPosition);
            Vec3 oldPosition = new Vec3(this.Position);
            Vec3 delta = newPos - oldPosition;

            // First set the position of the base
            base.SetPosition(newPos);

            // Then move all the elements
            foreach (ElementInstance element in this.Elements)
                element.Position += delta;
        }
        #endregion Method: SetPosition(Vec3 newPosition)

        #region Method: Clone()
        /// <summary>
        /// Clones the matter instance.
        /// </summary>
        /// <returns>A clone of the matter instance.</returns>
        public new MatterInstance Clone()
        {
            return base.Clone() as MatterInstance;
        }
        #endregion Method: Clone()

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the matter instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the matter instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the matter instance satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null)
            {
                // Check whether the base satisfies the condition
                if (base.Satisfies(conditionBase, iVariableInstanceHolder))
                {
                    // Matter condition
                    MatterConditionBase matterConditionBase = conditionBase as MatterConditionBase;
                    if (matterConditionBase != null)
                    {
                        // Layer condition
                        LayerConditionBase layerConditionBase = conditionBase as LayerConditionBase;
                        if (layerConditionBase != null)
                        {
                            // Check whether the index is correct
                            if (!(layerConditionBase.IndexSign == null || layerConditionBase.Index == null || Toolbox.Compare(this.LayerIndex, (EqualitySignExtended)layerConditionBase.IndexSign, (int)layerConditionBase.Index)))
                                return false;
                        }

                        // Check whether the required quantity is there, the state of matter is correct, and the chemical formula is correct
                        if ((matterConditionBase.Quantity == null || this.Quantity.Satisfies(matterConditionBase.Quantity, iVariableInstanceHolder)) &&
                            (matterConditionBase.StateOfMatterSign == null || matterConditionBase.StateOfMatter == null || Toolbox.Compare(this.StateOfMatter, (EqualitySign)matterConditionBase.StateOfMatterSign, (StateOfMatter)matterConditionBase.StateOfMatter)) &&
                            (matterConditionBase.ChemicalFormulaSign == null || matterConditionBase.ChemicalFormula == null || Toolbox.Compare(this.ChemicalFormula, (EqualitySign)matterConditionBase.ChemicalFormulaSign, matterConditionBase.ChemicalFormula)))
                        {
                            // Check whether the instance has all mandatory elements
                            if (matterConditionBase.HasAllMandatoryElements == true)
                            {
                                foreach (ElementValuedBase elementValuedBase in this.MatterBase.Elements)
                                {
                                    if (elementValuedBase.Necessity == Necessity.Mandatory)
                                    {
                                        int quantity = 0;
                                        foreach (ElementInstance elementInstance in this.Elements)
                                        {
                                            if (elementInstance.IsNodeOf(elementValuedBase.ElementBase))
                                                quantity++;
                                        }
                                        if (!elementValuedBase.Quantity.IsInRange(quantity))
                                            return false;
                                    }
                                }
                            }

                            // Check whether the instance has all required elements
                            foreach (ElementConditionBase elementConditionBase in matterConditionBase.Elements)
                            {
                                if (elementConditionBase.Quantity == null ||
                                    elementConditionBase.Quantity.BaseValue == null ||
                                    elementConditionBase.Quantity.ValueSign == null)
                                {
                                    bool satisfied = false;
                                    foreach (ElementInstance elementInstance in this.Elements)
                                    {
                                        if (elementInstance.Satisfies(elementConditionBase, iVariableInstanceHolder))
                                        {
                                            satisfied = true;
                                            break;
                                        }
                                    }
                                    if (!satisfied)
                                        return false;
                                }
                                else
                                {
                                    int quantity = 0;
                                    int requiredQuantity = (int)elementConditionBase.Quantity.BaseValue;
                                    foreach (ElementInstance elementInstance in this.Elements)
                                    {
                                        if (elementInstance.Satisfies(elementConditionBase, iVariableInstanceHolder))
                                            quantity++;
                                    }
                                    if (!Toolbox.Compare(quantity, (EqualitySignExtended)elementConditionBase.Quantity.ValueSign, requiredQuantity))
                                        return false;
                                }
                            }
                            return true;
                        }
                        return false;
                    }
                    else
                        return true;
                }
                else
                {
                    // Element condition
                    ElementConditionBase elementCondition = conditionBase as ElementConditionBase;
                    if (elementCondition != null)
                    {
                        foreach (ElementInstance elementInstance in this.Elements)
                        {
                            if (elementInstance.Satisfies(elementCondition, iVariableInstanceHolder))
                                return true;
                        }
                        return false;
                    }
                }
            }
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the matter instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the matter instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
            {
                if (base.Apply(changeBase, iVariableInstanceHolder))
                {
                    // Matter change
                    MatterChangeBase matterChangeBase = changeBase as MatterChangeBase;
                    if (matterChangeBase != null)
                    {
                        // Adjust the quantity
                        if (matterChangeBase.Quantity != null)
                            this.Quantity.Apply(matterChangeBase.Quantity, iVariableInstanceHolder);

                        // Adjust the state of matter
                        if (matterChangeBase.StateOfMatter != null)
                            this.StateOfMatter = (StateOfMatter)matterChangeBase.StateOfMatter;

                        // Apply the element changes
                        foreach (ElementChangeBase elementChangeBase in matterChangeBase.Elements)
                        {
                            foreach (ElementInstance elementInstance in this.Elements)
                                elementInstance.Apply(elementChangeBase, iVariableInstanceHolder);
                        }

                        // Add the elements
                        foreach (ElementValuedBase elementValuedBase in matterChangeBase.ElementsToAdd)
                        {
                            for (int i = 0; i < elementValuedBase.Quantity.BaseValue; i++)
                                AddElement(InstanceManager.Current.Create<ElementInstance>(elementValuedBase.ElementBase));
                        }

                        // Remove the elements
                        foreach (ElementConditionBase elementConditionBase in matterChangeBase.ElementsToRemove)
                        {
                            if (elementConditionBase.Quantity == null ||
                                elementConditionBase.Quantity.BaseValue == null ||
                                elementConditionBase.Quantity.ValueSign == null)
                            {
                                foreach (ElementInstance elementInstance in this.Elements)
                                {
                                    if (elementInstance.Satisfies(elementConditionBase, iVariableInstanceHolder))
                                    {
                                        RemoveElement(elementInstance);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                int quantity = (int)elementConditionBase.Quantity.BaseValue;
                                if (quantity > 0)
                                {
                                    ReadOnlyCollection<ElementInstance> elements = this.Elements;
                                    foreach (ElementInstance elementInstance in elements)
                                    {
                                        if (elementInstance.Satisfies(elementConditionBase, iVariableInstanceHolder))
                                        {
                                            RemoveElement(elementInstance);
                                            quantity--;
                                            if (quantity <= 0)
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                        // Change the chemical formula
                        if (matterChangeBase.ChemicalFormula != null)
                            this.ChemicalFormula = matterChangeBase.ChemicalFormula;

                        // Layer change
                        LayerChangeBase layerChangeBase = changeBase as LayerChangeBase;
                        if (layerChangeBase != null)
                        {
                            // Adjust the index
                            if (layerChangeBase.Index != null)
                                this.LayerIndex = (int)layerChangeBase.Index;
                        }
                    }
                    return true;
                }
                else
                {
                    // Element change
                    ElementChangeBase elementChange = changeBase as ElementChangeBase;
                    if (elementChange != null)
                    {
                        foreach (ElementInstance elementInstance in this.Elements)
                            elementInstance.Apply(elementChange, iVariableInstanceHolder);
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            if (this.Quantity != null)
                return this.DefaultName + " (" + this.Quantity.ToString() + ")";
            else
                return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: MatterInstance

}