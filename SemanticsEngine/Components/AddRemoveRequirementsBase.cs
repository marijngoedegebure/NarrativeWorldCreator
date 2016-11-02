/**************************************************************************
 * 
 * AddRemoveRequirementsBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Components;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region TODO Class: AddRemoveRequirementsBase
    /// <summary>
    /// A base for the add/remove requirements.
    /// </summary>
    public class AddRemoveRequirementsBase : Base
    {

        #region Properties and Fields

        #region Property: AddRemoveRequirements
        /// <summary>
        /// Gets the add/remove requirements of which this is an add/remove requirements base.
        /// </summary>
        internal AddRemoveRequirements AddRemoveRequirements
        {
            get
            {
                return this.IdHolder as AddRemoveRequirements;
            }
        }
        #endregion Property: AddRemoveRequirements

        #region Property: AbstractEntityAdded
        /// <summary>
        /// The abstract entity that should have been added.
        /// </summary>
        private AbstractEntityConditionBase abstractEntityAdded = null;

        /// <summary>
        /// Gets the abstract entity that should have been added.
        /// </summary>
        public AbstractEntityConditionBase AbstractEntityAdded
        {
            get
            {
                return abstractEntityAdded;
            }
        }
        #endregion Property: AbstractEntityAdded

        #region Property: AbstractEntityRemoved
        /// <summary>
        /// The abstract entity that should have been removed.
        /// </summary>
        private AbstractEntityConditionBase abstractEntityRemoved = null;

        /// <summary>
        /// Gets the abstract entity that should have been removed.
        /// </summary>
        public AbstractEntityConditionBase AbstractEntityRemoved
        {
            get
            {
                return abstractEntityRemoved;
            }
        }
        #endregion Property: AbstractEntityRemoved

        #region Property: ConnectionItemAdded
        /// <summary>
        /// The connection item that should have been added.
        /// </summary>
        private TangibleObjectConditionBase connectionItemAdded = null;

        /// <summary>
        /// Gets the connection item that should have been added.
        /// </summary>
        public TangibleObjectConditionBase ConnectionItemAdded
        {
            get
            {
                return connectionItemAdded;
            }
        }
        #endregion Property: ConnectionItemAdded

        #region Property: ConnectionItemRemoved
        /// <summary>
        /// The connection item that should have been removed.
        /// </summary>
        private TangibleObjectConditionBase connectionItemRemoved = null;

        /// <summary>
        /// Gets the connection item that should have been removed.
        /// </summary>
        public TangibleObjectConditionBase ConnectionItemRemoved
        {
            get
            {
                return connectionItemRemoved;
            }
        }
        #endregion Property: ConnectionItemRemoved

        #region Property: CoverAdded
        /// <summary>
        /// The cover that should have been added.
        /// </summary>
        private TangibleObjectConditionBase coverAdded = null;

        /// <summary>
        /// Gets the cover that should have been added.
        /// </summary>
        public TangibleObjectConditionBase CoverAdded
        {
            get
            {
                return coverAdded;
            }
        }
        #endregion Property: CoverAdded

        #region Property: CoverRemoved
        /// <summary>
        /// The cover that should have been removed.
        /// </summary>
        private TangibleObjectConditionBase coverRemoved = null;

        /// <summary>
        /// Gets the cover that should have been removed.
        /// </summary>
        public TangibleObjectConditionBase CoverRemoved
        {
            get
            {
                return coverRemoved;
            }
        }
        #endregion Property: CoverRemoved

        #region Property: ElementAdded
        /// <summary>
        /// The element that should have been added.
        /// </summary>
        private ElementConditionBase elementAdded = null;

        /// <summary>
        /// Gets the element that should have been added.
        /// </summary>
        public ElementConditionBase ElementAdded
        {
            get
            {
                return elementAdded;
            }
        }
        #endregion Property: ElementAdded

        #region Property: ElementRemoved
        /// <summary>
        /// The element that should have been removed.
        /// </summary>
        private ElementConditionBase elementRemoved = null;

        /// <summary>
        /// Gets the element that should have been removed.
        /// </summary>
        public ElementConditionBase ElementRemoved
        {
            get
            {
                return elementRemoved;
            }
        }
        #endregion Property: ElementRemoved

        #region Property: ItemAdded
        /// <summary>
        /// The item that should have been added.
        /// </summary>
        private TangibleObjectConditionBase itemAdded = null;
        
        /// <summary>
        /// Gets the item that should have been added.
        /// </summary>
        public TangibleObjectConditionBase ItemAdded
        {
            get
            {
                return itemAdded;
            }
        }
        #endregion Property: ItemAdded

        #region Property: ItemRemoved
        /// <summary>
        /// The item that should have been removed.
        /// </summary>
        private TangibleObjectConditionBase itemRemoved = null;
        
        /// <summary>
        /// Gets the item that should have been removed.
        /// </summary>
        public TangibleObjectConditionBase ItemRemoved
        {
            get
            {
                return itemRemoved;
            }
        }
        #endregion Property: ItemRemoved

        #region Property: LayerAdded
        /// <summary>
        /// The layer that should have been added.
        /// </summary>
        private LayerConditionBase layerAdded = null;

        /// <summary>
        /// Gets the layer that should have been added.
        /// </summary>
        public LayerConditionBase LayerAdded
        {
            get
            {
                return layerAdded;
            }
        }
        #endregion Property: LayerAdded

        #region Property: LayerRemoved
        /// <summary>
        /// The layer that should have been removed.
        /// </summary>
        private LayerConditionBase layerRemoved = null;

        /// <summary>
        /// Gets the layer that should have been removed.
        /// </summary>
        public LayerConditionBase LayerRemoved
        {
            get
            {
                return layerRemoved;
            }
        }
        #endregion Property: LayerRemoved

        #region Property: MatterAdded
        /// <summary>
        /// The matter that should have been added.
        /// </summary>
        private MatterConditionBase matterAdded = null;

        /// <summary>
        /// Gets the matter that should have been added.
        /// </summary>
        public MatterConditionBase MatterAdded
        {
            get
            {
                return matterAdded;
            }
        }
        #endregion Property: MatterAdded

        #region Property: MatterRemoved
        /// <summary>
        /// The matter that should have been removed.
        /// </summary>
        private MatterConditionBase matterRemoved = null;

        /// <summary>
        /// Gets the matter that should have been removed.
        /// </summary>
        public MatterConditionBase MatterRemoved
        {
            get
            {
                return matterRemoved;
            }
        }
        #endregion Property: MatterRemoved

        #region Property: PartAdded
        /// <summary>
        /// The part that should have been added.
        /// </summary>
        private TangibleObjectConditionBase partAdded = null;
        
        /// <summary>
        /// Gets the part that should have been added.
        /// </summary>
        public TangibleObjectConditionBase PartAdded
        {
            get
            {
                return partAdded;
            }
        }
        #endregion Property: PartAdded

        #region Property: PartRemoved
        /// <summary>
        /// The part that should have been removed.
        /// </summary>
        private TangibleObjectConditionBase partRemoved = null;
        
        /// <summary>
        /// Gets the part that should have been removed.
        /// </summary>
        public TangibleObjectConditionBase PartRemoved
        {
            get
            {
                return partRemoved;
            }
        }
        #endregion Property: PartRemoved

        #region Property: RelationshipAdded
        /// <summary>
        /// The relationship that should have been added.
        /// </summary>
        private RelationshipTypeBase relationshipAdded = null;

        /// <summary>
        /// Gets the relationship that should have been added.
        /// </summary>
        public RelationshipTypeBase RelationshipAdded
        {
            get
            {
                return relationshipAdded;
            }
        }
        #endregion Property: RelationshipAdded

        #region Property: RelationshipRemoved
        /// <summary>
        /// The relationship that should have been removed.
        /// </summary>
        private RelationshipTypeBase relationshipRemoved = null;

        /// <summary>
        /// Gets the relationship that should have been removed.
        /// </summary>
        public RelationshipTypeBase RelationshipRemoved
        {
            get
            {
                return relationshipRemoved;
            }
        }
        #endregion Property: RelationshipRemoved

        #region Property: SpaceAdded
        /// <summary>
        /// The space that should have been added.
        /// </summary>
        private SpaceConditionBase spaceAdded = null;
        
        /// <summary>
        /// Gets the space that should have been added.
        /// </summary>
        public SpaceConditionBase SpaceAdded
        {
            get
            {
                return spaceAdded;
            }
        }
        #endregion Property: SpaceAdded

        #region Property: SpaceRemoved
        /// <summary>
        /// The space that should have been removed.
        /// </summary>
        private SpaceConditionBase spaceRemoved = null;
        
        /// <summary>
        /// Gets the space that should have been removed.
        /// </summary>
        public SpaceConditionBase SpaceRemoved
        {
            get
            {
                return spaceRemoved;
            }
        }
        #endregion Property: SpaceRemoved

        #region Property: SubstanceAdded
        /// <summary>
        /// The substance that should have been added.
        /// </summary>
        private SubstanceConditionBase substanceAdded = null;
        
        /// <summary>
        /// Gets the substance that should have been added.
        /// </summary>
        public SubstanceConditionBase SubstanceAdded
        {
            get
            {
                return substanceAdded;
            }
        }
        #endregion Property: SubstanceAdded

        #region Property: SubstanceRemoved
        /// <summary>
        /// The substance that should have been removed.
        /// </summary>
        private SubstanceConditionBase substanceRemoved = null;
        
        /// <summary>
        /// Gets the substance that should have been removed.
        /// </summary>
        public SubstanceConditionBase SubstanceRemoved
        {
            get
            {
                return substanceRemoved;
            }
        }
        #endregion Property: SubstanceRemoved

        #region Property: TangibleMatterAdded
        /// <summary>
        /// The tangible matter that should have been added.
        /// </summary>
        private MatterConditionBase tangibleMatterAdded = null;

        /// <summary>
        /// Gets the tangible matter that should have been added.
        /// </summary>
        public MatterConditionBase TangibleMatterAdded
        {
            get
            {
                return tangibleMatterAdded;
            }
        }
        #endregion Property: TangibleMatterAdded

        #region Property: TangibleMatterRemoved
        /// <summary>
        /// The tangible matter that should have been removed.
        /// </summary>
        private MatterConditionBase tangibleMatterRemoved = null;

        /// <summary>
        /// Gets the tangible matter that should have been removed.
        /// </summary>
        public MatterConditionBase TangibleMatterRemoved
        {
            get
            {
                return tangibleMatterRemoved;
            }
        }
        #endregion Property: TangibleMatterRemoved

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AddRemoveRequirementsBase(AddRemoveRequirements addRemoveRequirements)
        /// <summary>
        /// Create an add/remove requirements base from the given add/remove requirements.
        /// </summary>
        /// <param name="addRemoveRequirements">The add/remove requirements to create an add/remove requirements base from.</param>
        internal AddRemoveRequirementsBase(AddRemoveRequirements addRemoveRequirements)
            : base(addRemoveRequirements)
        {
            if (addRemoveRequirements != null)
            {
                if (addRemoveRequirements.AbstractEntityAdded != null)
                    this.abstractEntityAdded = BaseManager.Current.GetBase<AbstractEntityConditionBase>(addRemoveRequirements.AbstractEntityAdded);
                if (addRemoveRequirements.AbstractEntityRemoved != null)
                    this.abstractEntityRemoved = BaseManager.Current.GetBase<AbstractEntityConditionBase>(addRemoveRequirements.AbstractEntityRemoved);
                if (addRemoveRequirements.ConnectionItemAdded != null)
                    this.connectionItemAdded = BaseManager.Current.GetBase<TangibleObjectConditionBase>(addRemoveRequirements.ConnectionItemAdded);
                if (addRemoveRequirements.ConnectionItemRemoved != null)
                    this.connectionItemRemoved = BaseManager.Current.GetBase<TangibleObjectConditionBase>(addRemoveRequirements.ConnectionItemRemoved);
                if (addRemoveRequirements.CoverAdded != null)
                    this.coverAdded = BaseManager.Current.GetBase<TangibleObjectConditionBase>(addRemoveRequirements.CoverAdded);
                if (addRemoveRequirements.CoverRemoved != null)
                    this.coverRemoved = BaseManager.Current.GetBase<TangibleObjectConditionBase>(addRemoveRequirements.CoverRemoved);
                if (addRemoveRequirements.ElementAdded != null)
                    this.elementAdded = BaseManager.Current.GetBase<ElementConditionBase>(addRemoveRequirements.ElementAdded);
                if (addRemoveRequirements.ElementRemoved != null)
                    this.elementRemoved = BaseManager.Current.GetBase<ElementConditionBase>(addRemoveRequirements.ElementRemoved);
                if (addRemoveRequirements.ItemAdded != null)
                    this.itemAdded = BaseManager.Current.GetBase<TangibleObjectConditionBase>(addRemoveRequirements.ItemAdded);
                if (addRemoveRequirements.ItemRemoved != null)
                    this.itemRemoved = BaseManager.Current.GetBase<TangibleObjectConditionBase>(addRemoveRequirements.ItemRemoved);
                if (addRemoveRequirements.LayerAdded != null)
                    this.layerAdded = BaseManager.Current.GetBase<LayerConditionBase>(addRemoveRequirements.LayerAdded);
                if (addRemoveRequirements.LayerRemoved != null)
                    this.layerRemoved = BaseManager.Current.GetBase<LayerConditionBase>(addRemoveRequirements.LayerRemoved);
                if (addRemoveRequirements.MatterAdded != null)
                    this.matterAdded = BaseManager.Current.GetBase<MatterConditionBase>(addRemoveRequirements.MatterAdded);
                if (addRemoveRequirements.MatterRemoved != null)
                    this.matterRemoved = BaseManager.Current.GetBase<MatterConditionBase>(addRemoveRequirements.MatterRemoved);
                if (addRemoveRequirements.PartAdded != null)
                    this.partAdded = BaseManager.Current.GetBase<TangibleObjectConditionBase>(addRemoveRequirements.PartAdded);
                if (addRemoveRequirements.PartRemoved != null)
                    this.partRemoved = BaseManager.Current.GetBase<TangibleObjectConditionBase>(addRemoveRequirements.PartRemoved);
                this.relationshipAdded = BaseManager.Current.GetBase<RelationshipTypeBase>(addRemoveRequirements.RelationshipAdded);
                this.relationshipRemoved = BaseManager.Current.GetBase<RelationshipTypeBase>(addRemoveRequirements.RelationshipRemoved);
                if (addRemoveRequirements.SpaceAdded != null)
                    this.spaceAdded = BaseManager.Current.GetBase<SpaceConditionBase>(addRemoveRequirements.SpaceAdded);
                if (addRemoveRequirements.SpaceRemoved != null)
                    this.spaceRemoved = BaseManager.Current.GetBase<SpaceConditionBase>(addRemoveRequirements.SpaceRemoved);
                if (addRemoveRequirements.SubstanceAdded != null)
                    this.substanceAdded = BaseManager.Current.GetBase<SubstanceConditionBase>(addRemoveRequirements.SubstanceAdded);
                if (addRemoveRequirements.SubstanceRemoved != null)
                    this.substanceRemoved = BaseManager.Current.GetBase<SubstanceConditionBase>(addRemoveRequirements.SubstanceRemoved);
                if (addRemoveRequirements.TangibleMatterAdded != null)
                    this.tangibleMatterAdded = BaseManager.Current.GetBase<MatterConditionBase>(addRemoveRequirements.TangibleMatterAdded);
                if (addRemoveRequirements.TangibleMatterRemoved != null)
                    this.tangibleMatterRemoved = BaseManager.Current.GetBase<MatterConditionBase>(addRemoveRequirements.TangibleMatterRemoved);
            }
        }
        #endregion Constructor: AddRemoveRequirementsBase(AddRemoveRequirements addRemoveRequirements)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region TODO Method: IsSatisfied(EntityInstance entityInstance, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the add/remove requirements are satisfied.
        /// </summary>
        /// <param name="entityInstance">The entity instance to check satisfaction for.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the add/remove requirements are satisfied.</returns>
        public bool IsSatisfied(EntityInstance entityInstance, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (entityInstance != null && SemanticsEngine.Current != null)
            {
                if (this.AbstractEntityAdded != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        int requiredQuantity = GetRequiredQuantity(this.AbstractEntityAdded.Quantity);
                        foreach (AbstractEntityInstance addedAbstractEntity in SemanticsEngine.Current.GetAddedAbstractEntities(entityInstance))
                        {
                            if (addedAbstractEntity.Satisfies(this.AbstractEntityAdded, iVariableInstanceHolder))
                            {
                                requiredQuantity--;
                                if (requiredQuantity <= 0)
                                {
                                    isSatisfied = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.AbstractEntityRemoved != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        int requiredQuantity = GetRequiredQuantity(this.AbstractEntityRemoved.Quantity);
                        foreach (AbstractEntityInstance removedAbstractEntity in SemanticsEngine.Current.GetRemovedAbstractEntities(entityInstance))
                        {
                            if (removedAbstractEntity.Satisfies(this.AbstractEntityRemoved, iVariableInstanceHolder))
                            {
                                requiredQuantity--;
                                if (requiredQuantity <= 0)
                                {
                                    isSatisfied = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.ConnectionItemAdded != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        TangibleObjectInstance tangibleObjectInstance = entityInstance as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.ConnectionItemAdded.Quantity);
                            foreach (TangibleObjectInstance addedConnectionItem in SemanticsEngine.Current.GetAddedConnectionItems(tangibleObjectInstance))
                            {
                                if (addedConnectionItem.Satisfies(this.ConnectionItemAdded, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }

                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.ConnectionItemRemoved != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        TangibleObjectInstance tangibleObjectInstance = entityInstance as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.ConnectionItemRemoved.Quantity);
                            foreach (TangibleObjectInstance removedConnectionItem in SemanticsEngine.Current.GetRemovedConnectionItems(tangibleObjectInstance))
                            {
                                if (removedConnectionItem.Satisfies(this.ConnectionItemRemoved, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.CoverAdded != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        TangibleObjectInstance tangibleObjectInstance = entityInstance as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.CoverAdded.Quantity);
                            foreach (TangibleObjectInstance addedCover in SemanticsEngine.Current.GetAddedCovers(tangibleObjectInstance))
                            {
                                if (addedCover.Satisfies(this.CoverAdded, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.CoverRemoved != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        TangibleObjectInstance tangibleObjectInstance = entityInstance as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.CoverRemoved.Quantity);
                            foreach (TangibleObjectInstance removedCover in SemanticsEngine.Current.GetRemovedCovers(tangibleObjectInstance))
                            {
                                if (removedCover.Satisfies(this.CoverRemoved, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.ElementAdded != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        MatterInstance matterInstance = entityInstance as MatterInstance;
                        if (matterInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.ElementAdded.Quantity);
                            foreach (ElementInstance addedElement in SemanticsEngine.Current.GetAddedElements(matterInstance))
                            {
                                if (addedElement.Satisfies(this.ElementAdded, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.ElementRemoved != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        MatterInstance matterInstance = entityInstance as MatterInstance;
                        if (matterInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.ElementRemoved.Quantity);
                            foreach (ElementInstance removedElement in SemanticsEngine.Current.GetRemovedElements(matterInstance))
                            {
                                if (removedElement.Satisfies(this.ElementRemoved, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.ItemAdded != null)
                {
                    bool isSatisfied = false;
                    SpaceInstance spaceInstance = entityInstance as SpaceInstance;
                    if (spaceInstance != null && spaceInstance.addedOrRemoved)
                    {
                        int requiredQuantity = GetRequiredQuantity(this.ItemAdded.Quantity);
                        foreach (TangibleObjectInstance addedItem in SemanticsEngine.Current.GetAddedItems(spaceInstance))
                        {
                            if (addedItem.Satisfies(this.ItemAdded, iVariableInstanceHolder))
                            {
                                requiredQuantity--;
                                if (requiredQuantity <= 0)
                                {
                                    isSatisfied = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        PhysicalObjectInstance physicalObjectInstance = entityInstance as PhysicalObjectInstance;
                        if (physicalObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.ItemAdded.Quantity);
                            foreach (SpaceInstance space in physicalObjectInstance.Spaces)
                            {
                                if (!isSatisfied && space.addedOrRemoved)
                                {
                                    foreach (TangibleObjectInstance addedItem in SemanticsEngine.Current.GetAddedItems(space))
                                    {
                                        if (addedItem.Satisfies(this.ItemAdded, iVariableInstanceHolder))
                                        {
                                            requiredQuantity--;
                                            if (requiredQuantity <= 0)
                                            {
                                                isSatisfied = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.ItemRemoved != null)
                {
                    bool isSatisfied = false;
                    SpaceInstance spaceInstance = entityInstance as SpaceInstance;
                    if (spaceInstance != null && spaceInstance.addedOrRemoved)
                    {
                        int requiredQuantity = GetRequiredQuantity(this.ItemRemoved.Quantity);
                        foreach (TangibleObjectInstance removedItem in SemanticsEngine.Current.GetRemovedItems(spaceInstance))
                        {
                            if (removedItem.Satisfies(this.ItemRemoved, iVariableInstanceHolder))
                            {
                                requiredQuantity--;
                                if (requiredQuantity <= 0)
                                {
                                    isSatisfied = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        PhysicalObjectInstance physicalObjectInstance = entityInstance as PhysicalObjectInstance;
                        if (physicalObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.ItemRemoved.Quantity);
                            foreach (SpaceInstance space in physicalObjectInstance.Spaces)
                            {
                                if (!isSatisfied && space.addedOrRemoved)
                                {
                                    foreach (TangibleObjectInstance removedItem in SemanticsEngine.Current.GetRemovedItems(space))
                                    {
                                        if (removedItem.Satisfies(this.ItemRemoved, iVariableInstanceHolder))
                                        {
                                            requiredQuantity--;
                                            if (requiredQuantity <= 0)
                                            {
                                                isSatisfied = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.LayerAdded != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        TangibleObjectInstance tangibleObjectInstance = entityInstance as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.LayerAdded.Quantity);
                            foreach (MatterInstance addedLayer in SemanticsEngine.Current.GetAddedLayers(tangibleObjectInstance))
                            {
                                if (addedLayer.Satisfies(this.LayerAdded, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.LayerRemoved != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        TangibleObjectInstance tangibleObjectInstance = entityInstance as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.LayerRemoved.Quantity);
                            foreach (MatterInstance removedLayer in SemanticsEngine.Current.GetRemovedLayers(tangibleObjectInstance))
                            {
                                if (removedLayer.Satisfies(this.LayerRemoved, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.MatterAdded != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        TangibleObjectInstance tangibleObjectInstance = entityInstance as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.MatterAdded.Quantity);
                            foreach (MatterInstance addedMatter in SemanticsEngine.Current.GetAddedMatter(tangibleObjectInstance))
                            {
                                requiredQuantity--;
                                if (requiredQuantity <= 0)
                                {
                                    isSatisfied = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.MatterRemoved != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        TangibleObjectInstance tangibleObjectInstance = entityInstance as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.MatterRemoved.Quantity);
                            foreach (MatterInstance removedMatter in SemanticsEngine.Current.GetRemovedMatter(tangibleObjectInstance))
                            {
                                requiredQuantity--;
                                if (requiredQuantity <= 0)
                                {
                                    isSatisfied = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.PartAdded != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        TangibleObjectInstance tangibleObjectInstance = entityInstance as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.PartAdded.Quantity);
                            foreach (TangibleObjectInstance addedPart in SemanticsEngine.Current.GetAddedParts(tangibleObjectInstance))
                            {
                                if (addedPart.Satisfies(this.PartAdded, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.PartRemoved != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        TangibleObjectInstance tangibleObjectInstance = entityInstance as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.PartRemoved.Quantity);
                            foreach (TangibleObjectInstance removedPart in SemanticsEngine.Current.GetRemovedParts(tangibleObjectInstance))
                            {
                                if (removedPart.Satisfies(this.PartRemoved, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.RelationshipAdded != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        foreach (RelationshipInstance addedRelationship in SemanticsEngine.Current.GetAddedRelationships(entityInstance))
                        {
                            if (this.RelationshipAdded.Equals(addedRelationship.RelationshipType))
                            {
                                isSatisfied = true;
                                break;
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.RelationshipRemoved != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        foreach (RelationshipInstance removedRelationship in SemanticsEngine.Current.GetRemovedRelationships(entityInstance))
                        {
                            if (this.RelationshipRemoved.Equals(removedRelationship.RelationshipType))
                            {
                                isSatisfied = true;
                                break;
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.SpaceAdded != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        PhysicalObjectInstance physicalObjectInstance = entityInstance as PhysicalObjectInstance;
                        if (physicalObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.SpaceAdded.Quantity);
                            foreach (SpaceInstance addedSpace in SemanticsEngine.Current.GetAddedSpaces(physicalObjectInstance))
                            {
                                if (addedSpace.Satisfies(this.SpaceAdded, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.SpaceRemoved != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        PhysicalObjectInstance physicalObjectInstance = entityInstance as PhysicalObjectInstance;
                        if (physicalObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.SpaceRemoved.Quantity);
                            foreach (SpaceInstance removedSpace in SemanticsEngine.Current.GetRemovedSpaces(physicalObjectInstance))
                            {
                                if (removedSpace.Satisfies(this.SpaceRemoved, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.SubstanceAdded != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        CompoundInstance compoundInstance = entityInstance as CompoundInstance;
                        if (compoundInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.SubstanceAdded.Quantity);
                            foreach (SubstanceInstance addedSubstance in SemanticsEngine.Current.GetAddedSubstances(compoundInstance))
                            {
                                if (addedSubstance.Satisfies(this.SubstanceAdded, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            MixtureInstance mixtureInstance = entityInstance as MixtureInstance;
                            if (mixtureInstance != null)
                            {
                                int requiredQuantity = GetRequiredQuantity(this.SubstanceAdded.Quantity);
                                foreach (SubstanceInstance addedSubstance in SemanticsEngine.Current.GetAddedSubstances(mixtureInstance))
                                {
                                    if (addedSubstance.Satisfies(this.SubstanceAdded, iVariableInstanceHolder))
                                    {
                                        requiredQuantity--;
                                        if (requiredQuantity <= 0)
                                        {
                                            isSatisfied = true;
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.SubstanceRemoved != null)
                {
                    bool isSatisfied = false;
                    if (entityInstance.addedOrRemoved)
                    {
                        CompoundInstance compoundInstance = entityInstance as CompoundInstance;
                        if (compoundInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.SubstanceRemoved.Quantity);
                            foreach (SubstanceInstance removedSubstance in SemanticsEngine.Current.GetRemovedSubstances(compoundInstance))
                            {
                                if (removedSubstance.Satisfies(this.SubstanceRemoved, iVariableInstanceHolder))
                                {
                                    requiredQuantity--;
                                    if (requiredQuantity <= 0)
                                    {
                                        isSatisfied = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            MixtureInstance mixtureInstance = entityInstance as MixtureInstance;
                            if (mixtureInstance != null)
                            {
                                int requiredQuantity = GetRequiredQuantity(this.SubstanceRemoved.Quantity);
                                foreach (SubstanceInstance removedSubstance in SemanticsEngine.Current.GetRemovedSubstances(mixtureInstance))
                                {
                                    if (removedSubstance.Satisfies(this.SubstanceRemoved, iVariableInstanceHolder))
                                    {
                                        requiredQuantity--;
                                        if (requiredQuantity <= 0)
                                        {
                                            isSatisfied = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.TangibleMatterAdded != null)
                {
                    bool isSatisfied = false;
                    SpaceInstance spaceInstance = entityInstance as SpaceInstance;
                    if (spaceInstance != null && spaceInstance.addedOrRemoved)
                    {
                        int requiredQuantity = GetRequiredQuantity(this.TangibleMatterAdded.Quantity);
                        foreach (MatterInstance addedTangibleMatter in SemanticsEngine.Current.GetAddedTangibleMatter(spaceInstance))
                        {
                            if (addedTangibleMatter.Satisfies(this.TangibleMatterAdded, iVariableInstanceHolder))
                            {
                                requiredQuantity--;
                                if (requiredQuantity <= 0)
                                {
                                    isSatisfied = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        PhysicalObjectInstance physicalObjectInstance = entityInstance as PhysicalObjectInstance;
                        if (physicalObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.TangibleMatterAdded.Quantity);
                            foreach (SpaceInstance space in physicalObjectInstance.Spaces)
                            {
                                if (!isSatisfied && space.addedOrRemoved)
                                {
                                    foreach (MatterInstance addedTangibleMatter in SemanticsEngine.Current.GetAddedTangibleMatter(space))
                                    {
                                        if (addedTangibleMatter.Satisfies(this.TangibleMatterAdded, iVariableInstanceHolder))
                                        {
                                            requiredQuantity--;
                                            if (requiredQuantity <= 0)
                                            {
                                                isSatisfied = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }

                if (this.TangibleMatterRemoved != null)
                {
                    bool isSatisfied = false;
                    SpaceInstance spaceInstance = entityInstance as SpaceInstance;
                    if (spaceInstance != null && spaceInstance.addedOrRemoved)
                    {
                        int requiredQuantity = GetRequiredQuantity(this.TangibleMatterRemoved.Quantity);
                        foreach (MatterInstance removedTangibleMatter in SemanticsEngine.Current.GetRemovedTangibleMatter(spaceInstance))
                        {
                            if (removedTangibleMatter.Satisfies(this.TangibleMatterRemoved, iVariableInstanceHolder))
                            {
                                requiredQuantity--;
                                if (requiredQuantity <= 0)
                                {
                                    isSatisfied = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        PhysicalObjectInstance physicalObjectInstance = entityInstance as PhysicalObjectInstance;
                        if (physicalObjectInstance != null)
                        {
                            int requiredQuantity = GetRequiredQuantity(this.TangibleMatterRemoved.Quantity);
                            foreach (SpaceInstance space in physicalObjectInstance.Spaces)
                            {
                                if (!isSatisfied && space.addedOrRemoved)
                                {
                                    foreach (MatterInstance removedTangibleMatter in SemanticsEngine.Current.GetRemovedTangibleMatter(space))
                                    {
                                        if (removedTangibleMatter.Satisfies(this.TangibleMatterRemoved, iVariableInstanceHolder))
                                        {
                                            requiredQuantity--;
                                            if (requiredQuantity <= 0)
                                            {
                                                isSatisfied = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }
            }
            return true;
        }
        #endregion Method: IsSatisfied(EntityInstance entityInstance, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetRequiredQuantity(NumericalValueConditionBase quantity)
        /// <summary>
        /// Gets the required quantity.
        /// </summary>
        /// <param name="quantity">The quantity condition.</param>
        /// <returns>Returns the required quantity.</returns>
        private int GetRequiredQuantity(NumericalValueConditionBase quantity)
        {
            int requiredQuantity = 0;
            if (quantity != null)
            {
                float? baseQuantity = quantity.BaseValue;
                if (baseQuantity != null)
                    requiredQuantity = (int)baseQuantity;
            }
            return requiredQuantity;
        }
        #endregion Method: GetRequiredQuantity(NumericalValueConditionBase quantity)
		
        #endregion Method Group: Other

    }
    #endregion Class: AddRemoveRequirementsBase

}