/**************************************************************************
 * 
 * SpaceBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common.Shapes;
using Semantics.Entities;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Entities
{

    #region Class: SpaceBase
    /// <summary>
    /// A base of a space.
    /// </summary>
    public class SpaceBase : PhysicalObjectBase
    {

        #region Properties and Fields

        #region Property: Space
        /// <summary>
        /// Gets the space of which this is a space base.
        /// </summary>
        protected internal Space Space
        {
            get
            {
                return this.IdHolder as Space;
            }
        }
        #endregion Property: Space

        #region Property: Presence
        /// <summary>
        /// Indicates whether items in the space are attached to it (so move along), or just present.
        /// </summary>
        private Presence presence = default(Presence);

        /// <summary>
        /// Gets the presence, which indicates whether items in the space are attached to it (so move along), or just present.
        /// </summary>
        public Presence Presence
        {
            get
            {
                return presence;
            }
        }
        #endregion Property: Presence

        #region Property: Items
        /// <summary>
        /// All the items in the space.
        /// </summary>
        private TangibleObjectBase[] items = null;

        /// <summary>
        /// Gets all the items in the space.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectBase> Items
        {
            get
            {
                if (items == null)
                {
                    LoadItems();
                    if (items == null)
                        return new List<TangibleObjectBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<TangibleObjectBase>(items);
            }
        }

        /// <summary>
        /// Loads the items.
        /// </summary>
        private void LoadItems()
        {
            if (this.Space != null)
            {
                List<TangibleObjectBase> spaceItems = new List<TangibleObjectBase>();
                foreach (TangibleObjectValued tangibleObjectValued in this.Space.Items)
                {
                    for (int i = 0; i < tangibleObjectValued.Quantity.Value; i++)
                        spaceItems.Add(BaseManager.Current.GetBase<TangibleObjectBase>(tangibleObjectValued.TangibleObject));
                }
                items = spaceItems.ToArray();
            }
        }
        #endregion Property: Items

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpaceBase(Space space)
        /// <summary>
        /// Creates a new space base from the given space.
        /// </summary>
        /// <param name="space">The space to create a base from.</param>    
        protected internal SpaceBase(Space space)
            : base(space)
        {
            if (space != null)
            {
                this.presence = space.Presence;

                if (BaseManager.PreloadProperties)
                    LoadItems();
            }
        }
        #endregion Constructor: SpaceBase(Space space)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpaceBase

    #region Class: SpaceValuedBase
    /// <summary>
    /// A base of a valued space.
    /// </summary>
    public class SpaceValuedBase : PhysicalObjectValuedBase
    {

        #region Properties and Fields

        #region Property: SpaceValued
        /// <summary>
        /// Gets the valued space of which this is a valued space base.
        /// </summary>
        protected internal SpaceValued SpaceValued
        {
            get
            {
                return this.NodeValued as SpaceValued;
            }
        }
        #endregion Property: SpaceValued

        #region Property: SpaceBase
        /// <summary>
        /// Gets the space base.
        /// </summary>
        public SpaceBase SpaceBase
        {
            get
            {
                return this.NodeBase as SpaceBase;
            }
        }
        #endregion Property: SpaceBase

        #region Property: SpaceType
        /// <summary>
        /// The type of the space.
        /// </summary>
        private SpaceType spaceType = default(SpaceType);

        /// <summary>
        /// Gets the type of the space.
        /// </summary>
        public SpaceType SpaceType
        {
            get
            {
                return spaceType;
            }
        }
        #endregion Property: SpaceType

        #region Property: StorageType
        /// <summary>
        /// The storage type.
        /// </summary>
        private StorageType storageType = default(StorageType);
        
        /// <summary>
        /// Gets the storage type.
        /// </summary>
        public StorageType StorageType
        {
            get
            {
                return storageType;
            }
        }
        #endregion Property: StorageType

        #region Property: MaximumNumberOfItems
        /// <summary>
        /// The maximum number of items this valued space can hold.
        /// </summary>
        private uint maximumNumberOfItems = SemanticsSettings.Values.MaximumNumberOfItems;
        
        /// <summary>
        /// Gets the maximum number of items this valued space can hold.
        /// </summary>
        public uint MaximumNumberOfItems
        {
            get
            {
                return maximumNumberOfItems;
            }
        }
        #endregion Property: MaximumNumberOfItems

        #region Property: Capacity
        /// <summary>
        /// The capacity of the space.
        /// </summary>
        private NumericalValueBase capacity = null;

        /// <summary>
        /// Gets the capacity of the space.
        /// </summary>
        public NumericalValueBase Capacity
        {
            get
            {
                if (capacity == null)
                {
                    LoadCapacity();
                    if (capacity == null)
                        capacity = new NumericalValueBase(SemanticsSettings.Values.Capacity, Prefix.None, null, 0, SemanticsSettings.Values.MaxValue);
                }
                return capacity;
            }
        }

        /// <summary>
        /// Loads the capacity.
        /// </summary>
        private void LoadCapacity()
        {
            if (this.SpaceValued != null)
                capacity = BaseManager.Current.GetBase<NumericalValueBase>(this.SpaceValued.Capacity);
        }
        #endregion Property: Capacity

        #region Property: ShapeDescription
        /// <summary>
        /// The shape description of the space.
        /// </summary>
        private BaseShapeDescription shapeDescription = null;

        /// <summary>
        /// Gets the shape description of the space.
        /// </summary>
        public BaseShapeDescription ShapeDescription
        {
            get
            {
                if (shapeDescription == null)
                {
                    LoadShapeDescription();
                    if (shapeDescription == null)
                        shapeDescription = new BaseShapeDescription();
                }
                return shapeDescription;
            }
        }

        /// <summary>
        /// Loads the shape description.
        /// </summary>
        private void LoadShapeDescription()
        {
            if (this.SpaceValued != null)
                shapeDescription = this.SpaceValued.ShapeDescription;
        }
        #endregion Property: ShapeDescription

        #region Property: Items
        /// <summary>
        /// All the items in the space.
        /// </summary>
        private TangibleObjectBase[] items = null;

        /// <summary>
        /// Gets all the items in the space.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectBase> Items
        {
            get
            {
                if (items == null)
                {
                    LoadItems();
                    if (items == null)
                        return new List<TangibleObjectBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<TangibleObjectBase>(items);
            }
        }

        /// <summary>
        /// Loads the items.
        /// </summary>
        private void LoadItems()
        {
            if (this.SpaceValued != null)
            {
                List<TangibleObjectBase> spaceItems = new List<TangibleObjectBase>();
                foreach (TangibleObjectValued tangibleObjectValued in this.SpaceValued.Items)
                {
                    for (int i = 0; i < tangibleObjectValued.Quantity.Value; i++)
                        spaceItems.Add(BaseManager.Current.GetBase<TangibleObjectBase>(tangibleObjectValued.TangibleObject));
                }
                items = spaceItems.ToArray();
            }
        }
        #endregion Property: Items

        #region Property: TangibleMatter
        /// <summary>
        /// All the tangible matter in the space.
        /// </summary>
        private MatterValuedBase[] tangibleMatter = null;

        /// <summary>
        /// Gets all the tangible matter in the space.
        /// </summary>
        public ReadOnlyCollection<MatterValuedBase> TangibleMatter
        {
            get
            {
                if (tangibleMatter == null)
                {
                    LoadTangibleMatter();
                    if (tangibleMatter == null)
                        return new List<MatterValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<MatterValuedBase>(tangibleMatter);
            }
        }

        /// <summary>
        /// Loads the tangible matter.
        /// </summary>
        private void LoadTangibleMatter()
        {
            if (this.SpaceValued != null)
            {
                List<MatterValuedBase> tangibleMatterBases = new List<MatterValuedBase>();
                foreach (MatterValued matterValued in this.SpaceValued.TangibleMatter)
                    tangibleMatterBases.Add(BaseManager.Current.GetBase<MatterValuedBase>(matterValued));
                tangibleMatter = tangibleMatterBases.ToArray();
            }
        }
        #endregion Property: TangibleMatter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpaceValuedBase(SpaceValued spaceValued)
        /// <summary>
        /// Create a valued space base from the given valued space.
        /// </summary>
        /// <param name="spaceValued">The valued space to create a valued space base from.</param>
        protected internal SpaceValuedBase(SpaceValued spaceValued)
            : base(spaceValued)
        {
            if (spaceValued != null)
            {
                this.spaceType = spaceValued.SpaceType;
                this.storageType = spaceValued.StorageType;
                this.maximumNumberOfItems = spaceValued.MaximumNumberOfItems;

                if (BaseManager.PreloadProperties)
                {
                    LoadCapacity();
                    LoadShapeDescription();
                    LoadItems();
                    LoadTangibleMatter();
                }
            }
        }
        #endregion Constructor: SpaceValuedBase(SpaceValued spaceValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpaceValuedBase

    #region Class: SpaceConditionBase
    /// <summary>
    /// A condition on a space.
    /// </summary>
    public class SpaceConditionBase : PhysicalObjectConditionBase
    {

        #region Properties and Fields

        #region Property: SpaceCondition
        /// <summary>
        /// Gets the space condition of which this is a space condition base.
        /// </summary>
        protected internal SpaceCondition SpaceCondition
        {
            get
            {
                return this.Condition as SpaceCondition;
            }
        }
        #endregion Property: SpaceCondition

        #region Property: SpaceBase
        /// <summary>
        /// Gets the space base of which this is a space condition base.
        /// </summary>
        public SpaceBase SpaceBase
        {
            get
            {
                return this.NodeBase as SpaceBase;
            }
        }
        #endregion Property: SpaceBase

        #region Property: SpaceType
        /// <summary>
        /// The required space type.
        /// </summary>
        private SpaceType? spaceType = null;

        /// <summary>
        /// Gets the required space type.
        /// </summary>
        protected internal SpaceType? SpaceType
        {
            get
            {
                return spaceType;
            }
        }
        #endregion Property: SpaceType

        #region Property: SpaceTypeSign
        /// <summary>
        /// The sign for the space type.
        /// </summary>
        private EqualitySign? spaceTypeSign = null;

        /// <summary>
        /// Gets the sign for the space type.
        /// </summary>
        public EqualitySign? SpaceTypeSign
        {
            get
            {
                return spaceTypeSign;
            }
        }
        #endregion Property: SpaceTypeSign

        #region Property: StorageType
        /// <summary>
        /// The required storage type.
        /// </summary>
        private StorageType? storageType = null;
        
        /// <summary>
        /// Gets the required storage type.
        /// </summary>
        public StorageType? StorageType
        {
            get
            {
                return storageType;
            }
        }
        #endregion Property: StorageType

        #region Property: StorageTypeSign
        /// <summary>
        /// The sign for the storage type.
        /// </summary>
        private EqualitySign? storageTypeSign = null;
        
        /// <summary>
        /// Gets the sign for the storage type.
        /// </summary>
        public EqualitySign? StorageTypeSign
        {
            get
            {
                return storageTypeSign;
            }
        }
        #endregion Property: StorageTypeSign

        #region Property: MaximumNumberOfItems
        /// <summary>
        /// The maximum number of items this valued space should hold.
        /// </summary>
        private uint? maximumNumberOfItems = null;

        /// <summary>
        /// Gets the maximum number of items this valued space should hold.
        /// </summary>
        public uint? MaximumNumberOfItems
        {
            get
            {
                return maximumNumberOfItems;
            }
        }
        #endregion Property: MaximumNumberOfItems

        #region Property: MaximumNumberOfItemsSign
        /// <summary>
        /// The sign for the maximum number of items.
        /// </summary>
        private EqualitySignExtended? maximumNumberOfItemsSign = null;
        
        /// <summary>
        /// Gets the sign for the maximum number of items.
        /// </summary>
        public EqualitySignExtended? MaximumNumberOfItemsSign
        {
            get
            {
                return maximumNumberOfItemsSign;
            }
        }
        #endregion Property: MaximumNumberOfItemsSign

        #region Property: Capacity
        /// <summary>
        /// The required capacity.
        /// </summary>
        private ValueConditionBase capacity = null;

        /// <summary>
        /// Gets the required capacity.
        /// </summary>
        public ValueConditionBase Capacity
        {
            get
            {
                if (capacity == null)
                    LoadCapacity();
                return capacity;
            }
        }

        /// <summary>
        /// Loads the capacity.
        /// </summary>
        private void LoadCapacity()
        {
            if (this.SpaceCondition != null)
                capacity = BaseManager.Current.GetBase<ValueConditionBase>(this.SpaceCondition.Capacity);
        }
        #endregion Property: Capacity

        #region Property: NumberOfItems
        /// <summary>
        /// The number of items the space should hold (equivalent to maximum number of items).
        /// </summary>
        private uint? numberOfItems = null;
        
        /// <summary>
        /// Gets the number of items the space should hold (equivalent to maximum number of items).
        /// </summary>
        public uint? NumberOfItems
        {
            get
            {
                return numberOfItems;
            }
        }
        #endregion Property: NumberOfItems

        #region Property: NumberOfItemsSign
        /// <summary>
        /// The sign for the number of items.
        /// </summary>
        private EqualitySignExtended? numberOfItemsSign = null;
        
        /// <summary>
        /// Gets the sign for the number of items.
        /// </summary>
        public EqualitySignExtended? NumberOfItemsSign
        {
            get
            {
                return numberOfItemsSign;
            }
        }
        #endregion Property: NumberOfItemsSign

        #region Property: Size
        /// <summary>
        /// The required size.
        /// </summary>
        private ValueConditionBase size = null;

        /// <summary>
        /// Gets the required size.
        /// </summary>
        public ValueConditionBase Size
        {
            get
            {
                if (size == null)
                    LoadSize();
                return size;
            }
        }

        /// <summary>
        /// Loads the size.
        /// </summary>
        private void LoadSize()
        {
            if (this.SpaceCondition != null)
                size = BaseManager.Current.GetBase<ValueConditionBase>(this.SpaceCondition.Size);
        }
        #endregion Property: Size

        #region Property: IsFull
        /// <summary>
        /// Requires the space to be full.
        /// </summary>
        private bool? isFull = null;

        /// <summary>
        /// Gets the value that indicates whether the space has to be full.
        /// </summary>
        public bool? IsFull
        {
            get
            {
                return isFull;
            }
        }
        #endregion Property: IsFull

        #region Property: Items
        /// <summary>
        /// All the required space items.
        /// </summary>
        private TangibleObjectConditionBase[] items = null;

        /// <summary>
        /// Gets all the required space items.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectConditionBase> Items
        {
            get
            {
                if (items == null)
                {
                    LoadItems();
                    if (items == null)
                        return new List<TangibleObjectConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<TangibleObjectConditionBase>(items);
            }
        }

        /// <summary>
        /// Loads the items.
        /// </summary>
        private void LoadItems()
        {
            if (this.SpaceCondition != null)
            {
                List<TangibleObjectConditionBase> tangibleObjectConditionBases = new List<TangibleObjectConditionBase>();
                foreach (TangibleObjectCondition tangibleObjectCondition in this.SpaceCondition.Items)
                    tangibleObjectConditionBases.Add(BaseManager.Current.GetBase<TangibleObjectConditionBase>(tangibleObjectCondition));
                items = tangibleObjectConditionBases.ToArray();
            }
        }
        #endregion Property: Items

        #region Property: TangibleMatter
        /// <summary>
        /// All the required tangible matter.
        /// </summary>
        private MatterConditionBase[] tangibleMatter = null;

        /// <summary>
        /// Gets all the required tangible matter.
        /// </summary>
        public ReadOnlyCollection<MatterConditionBase> TangibleMatter
        {
            get
            {
                if (tangibleMatter == null)
                {
                    LoadTangibleMatter();
                    if (tangibleMatter == null)
                        return new List<MatterConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<MatterConditionBase>(tangibleMatter);
            }
        }

        /// <summary>
        /// Loads the tangible matter.
        /// </summary>
        private void LoadTangibleMatter()
        {
            if (this.SpaceCondition != null)
            {
                List<MatterConditionBase> matterConditionBases = new List<MatterConditionBase>();
                foreach (MatterCondition matterCondition in this.SpaceCondition.TangibleMatter)
                    matterConditionBases.Add(BaseManager.Current.GetBase<MatterConditionBase>(matterCondition));
                tangibleMatter = matterConditionBases.ToArray();
            }
        }
        #endregion Property: TangibleMatter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpaceConditionBase(SpaceCondition spaceCondition)
        /// <summary>
        /// Creates a base of the given space condition.
        /// </summary>
        /// <param name="spaceCondition">The space condition to create a base of.</param>
        protected internal SpaceConditionBase(SpaceCondition spaceCondition)
            : base(spaceCondition)
        {
            if (spaceCondition != null)
            {
                this.spaceType = spaceCondition.SpaceType;
                this.spaceTypeSign = spaceCondition.SpaceTypeSign;
                this.storageType = spaceCondition.StorageType;
                this.storageTypeSign = spaceCondition.StorageTypeSign;
                this.maximumNumberOfItems = spaceCondition.MaximumNumberOfItems;
                this.maximumNumberOfItemsSign = spaceCondition.MaximumNumberOfItemsSign;
                this.numberOfItems = spaceCondition.NumberOfItems;
                this.numberOfItemsSign = spaceCondition.NumberOfItemsSign;
                this.isFull = spaceCondition.IsFull;

                if (BaseManager.PreloadProperties)
                {
                    LoadCapacity();
                    LoadSize();
                    LoadItems();
                    LoadTangibleMatter();
                }
            }
        }
        #endregion Constructor: SpaceConditionBase(SpaceCondition spaceCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpaceConditionBase

    #region Class: SpaceChangeBase
    /// <summary>
    /// A change on a space.
    /// </summary>
    public class SpaceChangeBase : PhysicalObjectChangeBase
    {

        #region Properties and Fields

        #region Property: SpaceChange
        /// <summary>
        /// Gets the space change of which this is a space change base.
        /// </summary>
        protected internal SpaceChange SpaceChange
        {
            get
            {
                return this.Change as SpaceChange;
            }
        }
        #endregion Property: SpaceChange

        #region Property: SpaceBase
        /// <summary>
        /// Gets the affected space base.
        /// </summary>
        public SpaceBase SpaceBase
        {
            get
            {
                return this.NodeBase as SpaceBase;
            }
        }
        #endregion Property: SpaceBase

        #region Property: SpaceType
        /// <summary>
        /// The space type to change to.
        /// </summary>
        private SpaceType? spaceType = null;

        /// <summary>
        /// Gets the space type to change to.
        /// </summary>
        public SpaceType? SpaceType
        {
            get
            {
                return spaceType;
            }
        }
        #endregion Property: SpaceType

        #region Property: StorageType
        /// <summary>
        /// The storage type to change to.
        /// </summary>
        private StorageType? storageType = null;

        /// <summary>
        /// Gets the storage type to change to.
        /// </summary>
        public StorageType? StorageType
        {
            get
            {
                return storageType;
            }
        }
        #endregion Property: StorageType

        #region Property: MaximumNumberOfItems
        /// <summary>
        /// The maximum number of items to change to.
        /// </summary>
        private uint? maximumNumberOfItems = null;
        
        /// <summary>
        /// Gets the maximum number of items to change to.
        /// </summary>
        public uint? MaximumNumberOfItems
        {
            get
            {
                return maximumNumberOfItems;
            }
        }
        #endregion Property: MaximumNumberOfItems

        #region Property: MaximumNumberOfItemsChangeType
        /// <summary>
        /// The type of change for the maximum number of items.
        /// </summary>
        private ValueChangeType? maximumNumberOfItemsChangeType = null;

        /// <summary>
        /// Gets the type of change for the maximum number of items.
        /// </summary>
        public ValueChangeType? MaximumNumberOfItemsChangeType
        {
            get
            {
                return maximumNumberOfItemsChangeType;
            }
        }
        #endregion Property: MaximumNumberOfItemsChangeType

        #region Property: Capacity
        /// <summary>
        /// The capacity to change to.
        /// </summary>
        private ValueChangeBase capacity = null;

        /// <summary>
        /// Gets or sets the capacity to change to.
        /// </summary>
        public ValueChangeBase Capacity
        {
            get
            {
                if (capacity == null)
                    LoadCapacity();
                return capacity;
            }
        }

        /// <summary>
        /// Loads the capacity.
        /// </summary>
        private void LoadCapacity()
        {
            if (this.SpaceChange != null)
                capacity = BaseManager.Current.GetBase<ValueChangeBase>(this.SpaceChange.Capacity);
        }
        #endregion Property: Capacity

        #region Property: Items
        /// <summary>
        /// The items that should be changed.
        /// </summary>
        private TangibleObjectChangeBase[] items = null;

        /// <summary>
        /// Gets the items that should be changed.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectChangeBase> Items
        {
            get
            {
                if (items == null)
                {
                    LoadItems();
                    if (items == null)
                        return new List<TangibleObjectChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<TangibleObjectChangeBase>(items);
            }
        }

        /// <summary>
        /// Loads the items.
        /// </summary>
        private void LoadItems()
        {
            if (this.SpaceChange != null)
            {
                List<TangibleObjectChangeBase> tangibleObjectChangeBases = new List<TangibleObjectChangeBase>();
                foreach (TangibleObjectChange tangibleObjectChange in this.SpaceChange.Items)
                    tangibleObjectChangeBases.Add(BaseManager.Current.GetBase<TangibleObjectChangeBase>(tangibleObjectChange));
                items = tangibleObjectChangeBases.ToArray();
            }
        }
        #endregion Property: Items

        #region Property: ItemsToAdd
        /// <summary>
        /// The items that should be added during the change.
        /// </summary>
        private TangibleObjectValuedBase[] itemsToAdd = null;

        /// <summary>
        /// Gets the items that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectValuedBase> ItemsToAdd
        {
            get
            {
                if (itemsToAdd == null)
                {
                    LoadItemsToAdd();
                    if (itemsToAdd == null)
                        return new List<TangibleObjectValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<TangibleObjectValuedBase>(itemsToAdd);
            }
        }

        /// <summary>
        /// Loads the items to add.
        /// </summary>
        private void LoadItemsToAdd()
        {
            if (this.SpaceChange != null)
            {
                List<TangibleObjectValuedBase> tangibleObjectValuedBases = new List<TangibleObjectValuedBase>();
                foreach (TangibleObjectValued tangibleObjectValued in this.SpaceChange.ItemsToAdd)
                    tangibleObjectValuedBases.Add(BaseManager.Current.GetBase<TangibleObjectValuedBase>(tangibleObjectValued));
                itemsToAdd = tangibleObjectValuedBases.ToArray();
            }
        }
        #endregion Property: ItemsToAdd

        #region Property: ItemsToRemove
        /// <summary>
        /// The items that should be removed during the change.
        /// </summary>
        private TangibleObjectConditionBase[] itemsToRemove = null;

        /// <summary>
        /// Gets the items that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectConditionBase> ItemsToRemove
        {
            get
            {
                if (itemsToRemove == null)
                {
                    LoadItemsToRemove();
                    if (itemsToRemove == null)
                        return new List<TangibleObjectConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<TangibleObjectConditionBase>(itemsToRemove);
            }
        }

        /// <summary>
        /// Loads the items to remove.
        /// </summary>
        private void LoadItemsToRemove()
        {
            if (this.SpaceChange != null)
            {
                List<TangibleObjectConditionBase> tangibleObjectConditionBases = new List<TangibleObjectConditionBase>();
                foreach (TangibleObjectCondition tangibleObjectCondition in this.SpaceChange.ItemsToRemove)
                    tangibleObjectConditionBases.Add(BaseManager.Current.GetBase<TangibleObjectConditionBase>(tangibleObjectCondition));
                itemsToRemove = tangibleObjectConditionBases.ToArray();
            }
        }
        #endregion Property: ItemsToRemove

        #region Property: TangibleMatter
        /// <summary>
        /// The tangible matter that should be changed.
        /// </summary>
        private MatterChangeBase[] tangibleMatter = null;

        /// <summary>
        /// Gets the tangible matter that should be changed.
        /// </summary>
        public ReadOnlyCollection<MatterChangeBase> TangibleMatter
        {
            get
            {
                if (tangibleMatter == null)
                {
                    LoadTangibleMatter();
                    if (tangibleMatter == null)
                        return new List<MatterChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<MatterChangeBase>(tangibleMatter);
            }
        }

        /// <summary>
        /// Loads the tangible matter.
        /// </summary>
        private void LoadTangibleMatter()
        {
            if (this.SpaceChange != null)
            {
                List<MatterChangeBase> matterChangeBases = new List<MatterChangeBase>();
                foreach (MatterChange matterChange in this.SpaceChange.TangibleMatter)
                    matterChangeBases.Add(BaseManager.Current.GetBase<MatterChangeBase>(matterChange));
                tangibleMatter = matterChangeBases.ToArray();
            }
        }
        #endregion Property: TangibleMatter

        #region Property: TangibleMatterToAdd
        /// <summary>
        /// The tangible matter that should be added during the change.
        /// </summary>
        private MatterValuedBase[] tangibleMatterToAdd = null;

        /// <summary>
        /// Gets the tangible matter that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<MatterValuedBase> TangibleMatterToAdd
        {
            get
            {
                if (tangibleMatterToAdd == null)
                {
                    LoadTangibleMatterToAdd();
                    if (tangibleMatterToAdd == null)
                        return new List<MatterValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<MatterValuedBase>(tangibleMatterToAdd);
            }
        }

        /// <summary>
        /// Loads the tangible matter to add.
        /// </summary>
        private void LoadTangibleMatterToAdd()
        {
            if (this.SpaceChange != null)
            {
                List<MatterValuedBase> matterValuedBases = new List<MatterValuedBase>();
                foreach (MatterValued matterValued in this.SpaceChange.TangibleMatterToAdd)
                    matterValuedBases.Add(BaseManager.Current.GetBase<MatterValuedBase>(matterValued));
                tangibleMatterToAdd = matterValuedBases.ToArray();
            }
        }
        #endregion Property: TangibleMatterToAdd

        #region Property: TangibleMatterToRemove
        /// <summary>
        /// The tangible matter that should be removed during the change.
        /// </summary>
        private MatterConditionBase[] tangibleMatterToRemove = null;

        /// <summary>
        /// Gets the tangible matter that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<MatterConditionBase> TangibleMatterToRemove
        {
            get
            {
                if (tangibleMatterToRemove == null)
                {
                    LoadTangibleMatterToRemove();
                    if (tangibleMatterToRemove == null)
                        return new List<MatterConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<MatterConditionBase>(tangibleMatterToRemove);
            }
        }

        /// <summary>
        /// Loads the tangible matter to remove.
        /// </summary>
        private void LoadTangibleMatterToRemove()
        {
            if (this.SpaceChange != null)
            {
                List<MatterConditionBase> matterConditionBases = new List<MatterConditionBase>();
                foreach (MatterCondition matterCondition in this.SpaceChange.TangibleMatterToRemove)
                    matterConditionBases.Add(BaseManager.Current.GetBase<MatterConditionBase>(matterCondition));
                tangibleMatterToRemove = matterConditionBases.ToArray();
            }
        }
        #endregion Property: TangibleMattersToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpaceChangeBase(SpaceChange spaceChange)
        /// <summary>
        /// Creates a base of the given space change.
        /// </summary>
        /// <param name="spaceChange">The space change to create a base of.</param>
        protected internal SpaceChangeBase(SpaceChange spaceChange)
            : base(spaceChange)
        {
            if (spaceChange != null)
            {
                this.spaceType = spaceChange.SpaceType;
                this.storageType = spaceChange.StorageType;
                this.maximumNumberOfItems = spaceChange.MaximumNumberOfItems;
                this.maximumNumberOfItemsChangeType = spaceChange.MaximumNumberOfItemsChangeType;

                if (BaseManager.PreloadProperties)
                {
                    LoadCapacity();
                    LoadItems();
                    LoadItemsToAdd();
                    LoadItemsToRemove();
                    LoadTangibleMatter();
                    LoadTangibleMatterToAdd();
                    LoadTangibleMatterToRemove();
                }
            }
        }
        #endregion Constructor: SpaceChangeBase(SpaceChange spaceChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpaceChangeBase

}