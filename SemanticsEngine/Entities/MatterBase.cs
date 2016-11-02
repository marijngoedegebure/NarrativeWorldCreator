/**************************************************************************
 * 
 * MatterBase.cs
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
using Semantics.Entities;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Entities
{

    #region Class: MatterBase
    /// <summary>
    /// A base of matter.
    /// </summary>
    public abstract class MatterBase : PhysicalEntityBase
    {

        #region Properties and Fields

        #region Property: Matter
        /// <summary>
        /// Gets the matter of which this is a matter base.
        /// </summary>
        protected internal Matter Matter
        {
            get
            {
                return this.IdHolder as Matter;
            }
        }
        #endregion Property: Matter

        #region Property: DefaultStateOfMatter
        /// <summary>
        /// The default state of matter.
        /// </summary>
        private StateOfMatter defaultStateOfMatter = default(StateOfMatter);

        /// <summary>
        /// Gets the default state of matter.
        /// </summary>
        public StateOfMatter DefaultStateOfMatter
        {
            get
            {
                return defaultStateOfMatter;
            }
        }
        #endregion Property: DefaultStateOfMatter

        #region Property: Elements
        /// <summary>
        /// The elements of the matter.
        /// </summary>
        private ElementValuedBase[] elements = null;

        /// <summary>
        /// Gets the elements of the matter.
        /// </summary>
        public ReadOnlyCollection<ElementValuedBase> Elements
        {
            get
            {
                if (elements == null)
                {
                    LoadElements();
                    if (elements == null)
                        return new List<ElementValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ElementValuedBase>(elements);
            }
        }

        /// <summary>
        /// Loads the elements.
        /// </summary>
        private void LoadElements()
        {
            if (this.Matter != null)
            {
                List<ElementValuedBase> elementValuedBases = new List<ElementValuedBase>();
                foreach (ElementValued elementValued in this.Matter.Elements)
                    elementValuedBases.Add(BaseManager.Current.GetBase<ElementValuedBase>(elementValued.Element));
                elements = elementValuedBases.ToArray();
            }
        }
        #endregion Property: Elements

        #region Property: ChemicalFormula
        /// <summary>
        /// The chemical formula.
        /// </summary>
        private String chemicalFormula = null;

        /// <summary>
        /// Gets the chemical formula.
        /// </summary>
        public String ChemicalFormula
        {
            get
            {
                if (chemicalFormula == null)
                {
                    LoadChemicalFormula();
                    if (chemicalFormula == null)
                        chemicalFormula = String.Empty;
                }
                return chemicalFormula;
            }
        }

        /// <summary>
        /// Loads the chemical formula.
        /// </summary>
        private void LoadChemicalFormula()
        {
            if (this.Matter != null)
                chemicalFormula = this.Matter.ChemicalFormula;
        }
        #endregion Property: ChemicalFormula

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MatterBase(Matter matter)
        /// <summary>
        /// Creates a new matter base from the given matter.
        /// </summary>
        /// <param name="matter">The matter to create the matter base from.</param>
        protected MatterBase(Matter matter)
            : base(matter)
        {
            if (matter != null)
            {
                this.defaultStateOfMatter = matter.DefaultStateOfMatter;

                if (BaseManager.PreloadProperties)
                {
                    LoadChemicalFormula();
                    LoadElements();
                }
            }
        }
        #endregion Constructor: MatterBase(Matter matter)

        #endregion Method Group: Constructors

    }
    #endregion Class: MatterBase

    #region Class: MatterValuedBase
    /// <summary>
    /// A base of valued matter.
    /// </summary>
    public abstract class MatterValuedBase : PhysicalEntityValuedBase
    {

        #region Properties and Fields

        #region Property: MatterValued
        /// <summary>
        /// Gets the valued matter of which this is a valued matter base.
        /// </summary>
        protected internal MatterValued MatterValued
        {
            get
            {
                return this.NodeValued as MatterValued;
            }
        }
        #endregion Property: MatterValued

        #region Property: MatterBase
        /// <summary>
        /// Gets the matter base of which this is a valued matter base.
        /// </summary>
        public MatterBase MatterBase
        {
            get
            {
                return this.NodeBase as MatterBase;
            }
        }
        #endregion Property: MatterBase

        #region Property: StateOfMatter
        /// <summary>
        /// The state of matter.
        /// </summary>
        private StateOfMatter stateOfMatter = default(StateOfMatter);

        /// <summary>
        /// Gets the state of matter.
        /// </summary>
        public StateOfMatter StateOfMatter
        {
            get
            {
                return stateOfMatter;
            }
        }
        #endregion Property: StateOfMatter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MatterValuedBase(MatterValued matterValued)
        /// <summary>
        /// Create a valued matter base from the given valued matter.
        /// </summary>
        /// <param name="matterValued">The valued matter to create a valued matter base from.</param>
        protected MatterValuedBase(MatterValued matterValued)
            : base(matterValued)
        {
            if (matterValued != null)
                this.stateOfMatter = matterValued.StateOfMatter;
        }
        #endregion Constructor: MatterValuedBase(MatterValued matterValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: MatterValuedBase

    #region Class: MatterConditionBase
    /// <summary>
    /// A condition on matter.
    /// </summary>
    public abstract class MatterConditionBase : PhysicalEntityConditionBase
    {

        #region Properties and Fields

        #region Property: MatterCondition
        /// <summary>
        /// Gets the matter condition of which this is a matter condition base.
        /// </summary>
        protected internal MatterCondition MatterCondition
        {
            get
            {
                return this.Condition as MatterCondition;
            }
        }
        #endregion Property: MatterCondition

        #region Property: MatterBase
        /// <summary>
        /// Gets the matter base of which this is a matter condition base.
        /// </summary>
        public MatterBase MatterBase
        {
            get
            {
                return this.NodeBase as MatterBase;
            }
        }
        #endregion Property: MatterBase

        #region Property: StateOfMatter
        /// <summary>
        /// The state of matter.
        /// </summary>
        private StateOfMatter? stateOfMatter = null;

        /// <summary>
        /// Gets the state of matter.
        /// </summary>
        public StateOfMatter? StateOfMatter
        {
            get
            {
                return stateOfMatter;
            }
        }
        #endregion Property: StateOfMatter

        #region Property: StateOfMatterSign
        /// <summary>
        /// The sign for the state of matter in the condition.
        /// </summary>
        private EqualitySign? stateOfMatterSign = null;

        /// <summary>
        /// Gets the sign for the state of matter in the condition.
        /// </summary>
        public EqualitySign? StateOfMatterSign
        {
            get
            {
                return stateOfMatterSign;
            }
        }
        #endregion Property: StateOfMatterSign

        #region Property: HasAllMandatoryElements
        /// <summary>
        /// The value that indicates whether all mandatory elements are required.
        /// </summary>
        private bool? hasAllMandatoryElements = null;

        /// <summary>
        /// Gets the value that indicates whether all mandatory elements are required.
        /// </summary>
        public bool? HasAllMandatoryElements
        {
            get
            {
                return hasAllMandatoryElements;
            }
        }
        #endregion Property: HasAllMandatoryElements

        #region Property: Elements
        /// <summary>
        /// The required elements.
        /// </summary>
        private ElementConditionBase[] elements = null;

        /// <summary>
        /// Gets the required elements.
        /// </summary>
        public ReadOnlyCollection<ElementConditionBase> Elements
        {
            get
            {
                if (elements == null)
                {
                    LoadElements();
                    if (elements == null)
                        return new List<ElementConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ElementConditionBase>(elements);
            }
        }

        /// <summary>
        /// Loads the elements.
        /// </summary>
        private void LoadElements()
        {
            if (this.MatterCondition != null)
            {
                List<ElementConditionBase> elementConditionBases = new List<ElementConditionBase>();
                foreach (ElementCondition elementCondition in this.MatterCondition.Elements)
                    elementConditionBases.Add(BaseManager.Current.GetBase<ElementConditionBase>(elementCondition));
                elements = elementConditionBases.ToArray();
            }
        }
        #endregion Property: Elements

        #region Property: ChemicalFormula
        /// <summary>
        /// The required chemical formula.
        /// </summary>
        private String chemicalFormula = null;

        /// <summary>
        /// Gets the required chemical formula.
        /// </summary>
        public String ChemicalFormula
        {
            get
            {
                return chemicalFormula;
            }
        }
        #endregion Property: ChemicalFormula

        #region Property: ChemicalFormulaSign
        /// <summary>
        /// The sign for the required chemical formula.
        /// </summary>
        private EqualitySign? chemicalFormulaSign = null;

        /// <summary>
        /// Gets the sign for the required chemical formula.
        /// </summary>
        public EqualitySign? ChemicalFormulaSign
        {
            get
            {
                return chemicalFormulaSign;
            }
        }
        #endregion Property: ChemicalFormulaSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MatterConditionBase(MatterCondition matterCondition)
        /// <summary>
        /// Creates a base of the given matter condition.
        /// </summary>
        /// <param name="matterCondition">The matter condition to create a base of.</param>
        protected MatterConditionBase(MatterCondition matterCondition)
            : base(matterCondition)
        {
            if (matterCondition != null)
            {
                this.stateOfMatter = matterCondition.StateOfMatter;
                this.stateOfMatterSign = matterCondition.StateOfMatterSign;
                this.hasAllMandatoryElements = matterCondition.HasAllMandatoryElements;
                this.chemicalFormula = matterCondition.ChemicalFormula;
                this.chemicalFormulaSign = matterCondition.ChemicalFormulaSign;

                if (BaseManager.PreloadProperties)
                    LoadElements();
            }
        }
        #endregion Constructor: MatterConditionBase(MatterCondition matterCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: MatterConditionBase

    #region Class: MatterChangeBase
    /// <summary>
    /// A change on matter.
    /// </summary>
    public abstract class MatterChangeBase : PhysicalEntityChangeBase
    {

        #region Properties and Fields

        #region Property: MatterChange
        /// <summary>
        /// Gets the matter change of which this is a matter change base.
        /// </summary>
        protected internal MatterChange MatterChange
        {
            get
            {
                return this.Change as MatterChange;
            }
        }
        #endregion Property: MatterChange

        #region Property: MatterBase
        /// <summary>
        /// Gets the affected matter base.
        /// </summary>
        public MatterBase MatterBase
        {
            get
            {
                return this.NodeBase as MatterBase;
            }
        }
        #endregion Property: MatterBase

        #region Property: StateOfMatter
        /// <summary>
        /// The state of matter.
        /// </summary>
        private StateOfMatter? stateOfMatter = null;

        /// <summary>
        /// Gets the state of matter.
        /// </summary>
        public StateOfMatter? StateOfMatter
        {
            get
            {
                return stateOfMatter;
            }
        }
        #endregion Property: StateOfMatter

        #region Property: Elements
        /// <summary>
        /// The elements to change.
        /// </summary>
        private ElementChangeBase[] elements = null;

        /// <summary>
        /// Gets the elements to change.
        /// </summary>
        public ReadOnlyCollection<ElementChangeBase> Elements
        {
            get
            {
                if (elements == null)
                {
                    LoadElements();
                    if (elements == null)
                        return new List<ElementChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ElementChangeBase>(elements);
            }
        }

        /// <summary>
        /// Loads the elements.
        /// </summary>
        private void LoadElements()
        {
            if (this.MatterChange != null)
            {
                List<ElementChangeBase> elementChangeBases = new List<ElementChangeBase>();
                foreach (ElementChange elementChange in this.MatterChange.Elements)
                    elementChangeBases.Add(BaseManager.Current.GetBase<ElementChangeBase>(elementChange));
                elements = elementChangeBases.ToArray();
            }
        }
        #endregion Property: Elements

        #region Property: ElementsToAdd
        /// <summary>
        /// The elements that should be added during the change.
        /// </summary>
        private ElementValuedBase[] elementsToAdd = null;

        /// <summary>
        /// Gets the elements that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<ElementValuedBase> ElementsToAdd
        {
            get
            {
                if (elementsToAdd == null)
                {
                    LoadElementsToAdd();
                    if (elementsToAdd == null)
                        return new List<ElementValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ElementValuedBase>(elementsToAdd);
            }
        }

        /// <summary>
        /// Loads the elements to add.
        /// </summary>
        private void LoadElementsToAdd()
        {
            if (this.MatterChange != null)
            {
                List<ElementValuedBase> elementValuedBases = new List<ElementValuedBase>();
                foreach (ElementValued elementValued in this.MatterChange.ElementsToAdd)
                    elementValuedBases.Add(BaseManager.Current.GetBase<ElementValuedBase>(elementValued));
                elementsToAdd = elementValuedBases.ToArray();
            }
        }
        #endregion Property: ElementsToAdd

        #region Property: ElementsToRemove
        /// <summary>
        /// The elements that should be removed during the change.
        /// </summary>
        private ElementConditionBase[] elementsToRemove = null;

        /// <summary>
        /// Gets the elements that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<ElementConditionBase> ElementsToRemove
        {
            get
            {
                if (elementsToRemove == null)
                {
                    LoadElementsToRemove();
                    if (elementsToRemove == null)
                        return new List<ElementConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ElementConditionBase>(elementsToRemove);
            }
        }

        /// <summary>
        /// Loads the elements to remove.
        /// </summary>
        private void LoadElementsToRemove()
        {
            if (this.MatterChange != null)
            {
                List<ElementConditionBase> elementConditionBases = new List<ElementConditionBase>();
                foreach (ElementCondition elementCondition in this.MatterChange.ElementsToRemove)
                    elementConditionBases.Add(BaseManager.Current.GetBase<ElementConditionBase>(elementCondition));
                elementsToRemove = elementConditionBases.ToArray();
            }
        }
        #endregion Property: ElementsToRemove

        #region Property: ChemicalFormula
        /// <summary>
        /// The chemical formula to change to.
        /// </summary>
        private String chemicalFormula = null;

        /// <summary>
        /// Gets the chemical formula to change to.
        /// </summary>
        public String ChemicalFormula
        {
            get
            {
                return chemicalFormula;
            }
        }
        #endregion Property: ChemicalFormula

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MatterChangeBase(MatterChange matterChange)
        /// <summary>
        /// Creates a base of the given matter change.
        /// </summary>
        /// <param name="materialChange">The matter change to create a base of.</param>
        protected MatterChangeBase(MatterChange matterChange)
            : base(matterChange)
        {
            if (matterChange != null)
            {
                this.stateOfMatter = matterChange.StateOfMatter;
                this.chemicalFormula = matterChange.ChemicalFormula;

                if (BaseManager.PreloadProperties)
                {
                    LoadElements();
                    LoadElementsToAdd();
                    LoadElementsToRemove();
                }
            }
        }
        #endregion Constructor: MatterChangeBase(MatterChange matterChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: MatterChangeBase

    #region Class: LayerBase
    /// <summary>
    /// A base of a layer.
    /// </summary>
    public sealed class LayerBase : MatterValuedBase
    {

        #region Properties and Fields

        #region Property: Layer
        /// <summary>
        /// Gets the layer of which this is a layer base.
        /// </summary>
        internal Layer Layer
        {
            get
            {
                return this.NodeValued as Layer;
            }
        }
        #endregion Property: Layer

        #region Property: Thickness
        /// <summary>
        /// The thickness.
        /// </summary>
        private NumericalValueBase thickness = null;

        /// <summary>
        /// Gets the thickness.
        /// </summary>
        public NumericalValueBase Thickness
        {
            get
            {
                if (thickness == null)
                {
                    LoadThickness();
                    if (thickness == null)
                        thickness = new NumericalValueBase(SemanticsSettings.Values.Thickness);
                }
                return thickness;
            }
        }

        /// <summary>
        /// Loads the thickness.
        /// </summary>
        private void LoadThickness()
        {
            if (this.Layer != null)
                thickness = BaseManager.Current.GetBase<NumericalValueBase>(this.Layer.Thickness);
        }
        #endregion Property: Thickness

        #region Property: Index
        /// <summary>
        /// The index of the layer when it is stacked on top of or underneath other layers: "the n-th layer".
        /// </summary>
        private int index = SemanticsSettings.Values.Index;

        /// <summary>
        /// Gets the index of the layer when it is stacked on top of or underneath other layers: "the n-th layer".
        /// </summary>
        public int Index
        {
            get
            {
                return index;
            }
        }
        #endregion Property: Index

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: LayerBase(Layer layer)
        /// <summary>
        /// Creates a layer base from the given layer.
        /// </summary>
        /// <param name="layer">The layer to create a layer base from.</param>
        internal LayerBase(Layer layer)
            : base(layer)
        {
            if (layer != null)
            {
                this.index = layer.Index;

                if (BaseManager.PreloadProperties)
                    LoadThickness();
            }
        }
        #endregion Constructor: LayerBase(Layer layer)

        #endregion Method Group: Constructors

    }
    #endregion Class: LayerBase

    #region Class: LayerConditionBase
    /// <summary>
    /// A condition on a layer.
    /// </summary>
    public sealed class LayerConditionBase : MatterConditionBase
    {

        #region Properties and Fields

        #region Property: LayerCondition
        /// <summary>
        /// Gets the layer condition of which this is a layer condition base.
        /// </summary>
        internal LayerCondition LayerCondition
        {
            get
            {
                return this.Condition as LayerCondition;
            }
        }
        #endregion Property: LayerCondition

        #region Property: Thickness
        /// <summary>
        /// The required thickness of the layer.
        /// </summary>
        private ValueConditionBase thickness = null;

        /// <summary>
        /// Gets the required thickness of the layer.
        /// </summary>
        public ValueConditionBase Thickness
        {
            get
            {
                if (thickness == null)
                    LoadThickness();
                return thickness;
            }
        }

        /// <summary>
        /// Loads the thickness.
        /// </summary>
        private void LoadThickness()
        {
            if (this.LayerCondition != null)
                thickness = BaseManager.Current.GetBase<ValueConditionBase>(this.LayerCondition.Thickness);
        }
        #endregion Property: Thickness

        #region Property: Index
        /// <summary>
        /// The required index of the layer.
        /// </summary>
        private int? index = null;

        /// <summary>
        /// Gets the required index of the layer.
        /// </summary>
        public int? Index
        {
            get
            {
                return index;
            }
        }
        #endregion Property: Index

        #region Property: IndexSign
        /// <summary>
        /// The sign for the index in the condition.
        /// </summary>
        private EqualitySignExtended? indexSign = null;

        /// <summary>
        /// Gets the sign for the index in the condition.
        /// </summary>
        public EqualitySignExtended? IndexSign
        {
            get
            {
                return indexSign;
            }
        }
        #endregion Property: IndexSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: LayerConditionBase(LayerCondition layerCondition)
        /// <summary>
        /// Creates a base of the given layer condition.
        /// </summary>
        /// <param name="layerCondition">The layer condition to create a base of.</param>
        internal LayerConditionBase(LayerCondition layerCondition)
            : base(layerCondition)
        {
            if (layerCondition != null)
            {
                this.index = layerCondition.Index;
                this.indexSign = layerCondition.IndexSign;

                if (BaseManager.PreloadProperties)
                    LoadThickness();
            }
        }
        #endregion Constructor: LayerConditionBase(LayerCondition layerCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: LayerConditionBase

    #region Class: LayerChangeBase
    /// <summary>
    /// A change on a layer.
    /// </summary>
    public sealed class LayerChangeBase : MatterChangeBase
    {

        #region Properties and Fields

        #region Property: LayerChange
        /// <summary>
        /// Gets the layer change of which this is a layer change base.
        /// </summary>
        internal LayerChange LayerChange
        {
            get
            {
                return this.Change as LayerChange;
            }
        }
        #endregion Property: LayerChange

        #region Property: Thickness
        /// <summary>
        /// The thickness.
        /// </summary>
        private ValueChangeBase thickness = null;

        /// <summary>
        /// Gets the thickness.
        /// </summary>
        public ValueChangeBase Thickness
        {
            get
            {
                if (thickness == null)
                    LoadThickness();
                return thickness;
            }
        }

        /// <summary>
        /// Loads the thickness.
        /// </summary>
        private void LoadThickness()
        {
            if (this.LayerChange != null)
                thickness = BaseManager.Current.GetBase<ValueChangeBase>(this.LayerChange.Thickness);
        }
        #endregion Property: Thickness

        #region Property: Index
        /// <summary>
        /// The index.
        /// </summary>
        private int? index = null;

        /// <summary>
        /// Gets the index.
        /// </summary>
        public int? Index
        {
            get
            {
                return index;
            }
        }
        #endregion Property: Index

        #region Property: IndexChange
        /// <summary>
        /// The type of change for the index.
        /// </summary>
        private ValueChangeType? indexChange = null;

        /// <summary>
        /// Gets the type of change for the index.
        /// </summary>
        public ValueChangeType? IndexChange
        {
            get
            {
                return indexChange;
            }
        }
        #endregion Property: IndexChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: LayerChangeBase(LayerChange layerChange)
        /// <summary>
        /// Creates a base of the given layer change.
        /// </summary>
        /// <param name="layerChange">The layer change to create a base of.</param>
        internal LayerChangeBase(LayerChange layerChange)
            : base(layerChange)
        {
            if (layerChange != null)
            {
                this.index = layerChange.Index;
                this.indexChange = layerChange.IndexChange;

                if (BaseManager.PreloadProperties)
                    LoadThickness();
            }
        }
        #endregion Constructor: LayerChangeBase(LayerChange layerChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: LayerChangeBase

}