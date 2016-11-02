/**************************************************************************
 * 
 * SpaceInstance.cs
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
using System.ComponentModel;
using Common;
using Common.Shapes;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Entities
{

    #region Class: SpaceInstance
    /// <summary>
    /// An instance of a space.
    /// </summary>
    public class SpaceInstance : PhysicalObjectInstance
    {

        #region Events, Properties, and Fields

        #region Events: ItemHandler
        /// <summary>
        /// A handler for added or removed items.
        /// </summary>
        /// <param name="sender">The space instance the item was added to or removed from.</param>
        /// <param name="item">The added or removed item.</param>
        public delegate void ItemHandler(SpaceInstance sender, TangibleObjectInstance item);
        
        /// <summary>
        /// An event to indicate an added item.
        /// </summary>
        public event ItemHandler ItemAdded;

        /// <summary>
        /// An event to indicate a removed item.
        /// </summary>
        public event ItemHandler ItemRemoved;
        #endregion Events: ItemHandler

        #region Events: TangibleMatterHandler
        /// <summary>
        /// A handler for added or removed tangible matter.
        /// </summary>
        /// <param name="sender">The space instance the tangible matter was added to or removed from.</param>
        /// <param name="tangibleMatter">The added or removed tangible matter.</param>
        public delegate void TangibleMatterHandler(SpaceInstance sender, MatterInstance tangibleMatter);

        /// <summary>
        /// An event to indicate added tangible matter.
        /// </summary>
        public event TangibleMatterHandler TangibleMatterAdded;

        /// <summary>
        /// An event to indicate removed tangible matter.
        /// </summary>
        public event TangibleMatterHandler TangibleMatterRemoved;
        #endregion Events: TangibleMatterHandler

        #region Property: SpaceBase
        /// <summary>
        /// Gets the space base of which this is a space instance.
        /// </summary>
        public SpaceBase SpaceBase
        {
            get
            {
                return this.NodeBase as SpaceBase;
            }
        }
        #endregion Property: SpaceBase

        #region Property: SpaceValuedBase
        /// <summary>
        /// Gets the valued space base of which this is a space instance.
        /// </summary>
        public SpaceValuedBase SpaceValuedBase
        {
            get
            {
                return this.Base as SpaceValuedBase;
            }
        }
        #endregion Property: SpaceValuedBase

        #region Property: Items
        /// <summary>
        /// All the items in the space.
        /// </summary>
        private TangibleObjectInstance[] items = null;

        /// <summary>
        /// Gets all the items in the space.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectInstance> Items
        {
            get
            {
                if (items != null)
                    return new ReadOnlyCollection<TangibleObjectInstance>(items);

                return new List<TangibleObjectInstance>(0).AsReadOnly();
            }
        }
        #endregion Property: Items

        #region Property: Presence
        /// <summary>
        /// Gets the presence, which indicates whether items in the space are attached to it (so move along), or just present.
        /// </summary>
        public Presence Presence
        {
            get
            {
                if (this.SpaceBase != null)
                    return GetProperty<Presence>("Presence", this.SpaceBase.Presence);
                else
                    return GetProperty<Presence>("Presence", default(Presence));
            }
            protected set
            {
                if (this.Presence != value)
                    SetProperty("Presence", value);
            }
        }
        #endregion Property: Presence

        #region Property: SpaceType
        /// <summary>
        /// Gets the type of the space.
        /// </summary>
        public SpaceType SpaceType
        {
            get
            {
                if (this.SpaceValuedBase != null)
                    return GetProperty<SpaceType>("SpaceType", this.SpaceValuedBase.SpaceType);
                else
                    return GetProperty<SpaceType>("SpaceType", default(SpaceType));
            }
            protected set
            {
                if (this.SpaceType != value)
                    SetProperty("SpaceType", value);
            }
        }
        #endregion Property: SpaceType

        #region Property: StorageType
        /// <summary>
        /// Gets the storage type.
        /// </summary>
        public StorageType StorageType
        {
            get
            {
                if (this.SpaceValuedBase != null)
                    return GetProperty<StorageType>("StorageType", this.SpaceValuedBase.StorageType);
                else
                    return GetProperty<StorageType>("StorageType", default(StorageType));
            }
            protected set
            {
                if (this.StorageType != value)
                    SetProperty("StorageType", value);
            }
        }
        #endregion Property: StorageType

        #region Property: MaximumNumberOfItems
        /// <summary>
        /// Gets the maximum number of items this space can hold.
        /// </summary>
        protected internal uint MaximumNumberOfItems
        {
            get
            {
                if (this.SpaceValuedBase != null)
                    return GetProperty<uint>("MaximumNumberOfItems", this.SpaceValuedBase.MaximumNumberOfItems);
                else
                    return GetProperty<uint>("MaximumNumberOfItems", SemanticsSettings.Values.MaximumNumberOfItems);
            }
            protected set
            {
                if (this.MaximumNumberOfItems != value)
                    SetProperty("MaximumNumberOfItems", value);
            }
        }
        #endregion Property: MaximumNumberOfItems

        #region Property: NumberOfItems
        /// <summary>
        /// Gets the number of items in the space.
        /// </summary>
        public uint NumberOfItems
        {
            get
            {
                return (uint)this.Items.Count;
            }
        }
        #endregion Property: NumberOfItems

        #region Property: Capacity
        /// <summary>
        /// Gets the capacity of the space.
        /// </summary>
        public NumericalValueInstance Capacity
        {
            get
            {
                // Return the locally modified value, or create a new instance and subscribe to possible changes
                NumericalValueInstance capacity = GetProperty<NumericalValueInstance>("Capacity", null);
                if (capacity == null)
                {
                    if (this.SpaceValuedBase != null)
                        capacity = InstanceManager.Current.Create<NumericalValueInstance>(this.SpaceValuedBase.Capacity);
                    else
                        capacity = new NumericalValueInstance(SemanticsSettings.Values.Capacity);
                    capacity.Min = 0;
                    if (capacity != null)
                        capacity.PropertyChanged += new PropertyChangedEventHandler(capacity_PropertyChanged);
                }
                return capacity;
            }
            protected set
            {
                if (this.Capacity != value)
                    SetProperty("Capacity", value);
            }
        }

        /// <summary>
        /// If the capacity changes, add the property and the modified capacity to the modifications table.
        /// </summary>
        private void capacity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != null)
                SetProperty("Capacity", sender);
        }
        #endregion Property: Capacity

        #region Property: Size
        /// <summary>
        /// Gets the size of the space.
        /// </summary>
        public NumericalValueInstance Size
        {
            get
            {
                return PhysicsManager.Current.GetSize(this);
            }
        }
        #endregion Property: Size

        #region Property: IsFull
        /// <summary>
        /// Gets the value that indicates whether the space is full.
        /// </summary>
        public bool IsFull
        {
            get
            {
                switch (this.StorageType)
                {
                    case StorageType.Unlimited:
                        return false;
                    case StorageType.MaximumNumberOfItems:
                        return this.NumberOfItems >= this.MaximumNumberOfItems;
                    case StorageType.Capacity:
                        return this.Size >= this.Capacity;
                    case StorageType.MaximumNumberOfItemsAndCapacity:
                        return this.NumberOfItems >= this.MaximumNumberOfItems || this.Size >= this.Capacity;
                    default:
                        break;
                }
                return false;
            }
        }
        #endregion Property: IsFull

        #region Property: Shape
        /// <summary>
        /// Gets the shape of the space.
        /// </summary>
        public BaseShape Shape
        {
            get
            {
                return GetProperty<BaseShape>("Shape");
            }
            protected set
            {
                if (this.Shape != value)
                    SetProperty("Shape", value);
            }
        }
        #endregion Property: Shape

        #region Property: OriginalShape
        /// <summary>
        /// Gets the original, untransformed shape of the space: this can be used to create transformed shape.
        /// </summary>
        public BaseShape OriginalShape
        {
            get
            {
                return GetProperty<BaseShape>("OriginalShape");
            }
            private set
            {
                if (this.OriginalShape != value)
                    SetProperty("OriginalShape", value);
            }
        }
        #endregion Property: OriginalShape

        #region Property: PhysicalObject
        /// <summary>
        /// The physical object for which this is a space.
        /// </summary>
        private PhysicalObjectInstance physicalObject = null;

        /// <summary>
        /// Gets the physical object for which this is a space.
        /// </summary>
        public PhysicalObjectInstance PhysicalObject
        {
            get
            {
                return physicalObject;
            }
            internal set
            {
                if (physicalObject != value)
                {
                    physicalObject = value;
                    NotifyPropertyChanged("PhysicalObject");
                }
            }
        }
        #endregion Property: PhysicalObject

        #region Property: TangibleMatter
        /// <summary>
        /// All the tangible matter in the space.
        /// </summary>
        private MatterInstance[] tangibleMatter = null;

        /// <summary>
        /// Gets all the tangible matter in the space.
        /// </summary>
        public ReadOnlyCollection<MatterInstance> TangibleMatter
        {
            get
            {
                if (tangibleMatter != null)
                    return new ReadOnlyCollection<MatterInstance>(tangibleMatter);

                return new List<MatterInstance>(0).AsReadOnly();
            }
        }
        #endregion Property: TangibleMatter

        #endregion Events, Properties, and Fields

        #region Method Group: Constructors

        #region Constructor: SpaceInstance(SpaceBase spaceBase)
        /// <summary>
        /// Creates a new space instance from the given valued space base.
        /// </summary>
        /// <param name="spaceBase">The space base to create the space instance from.</param>
        internal SpaceInstance(SpaceBase spaceBase)
            : this(spaceBase, SemanticsEngineSettings.DefaultCreateOptions)
        {
        }
        #endregion Constructor: SpaceInstance(SpaceBase spaceBase)

        #region Constructor: SpaceInstance(SpaceBase spaceBase, CreateOptions createOptions)
        /// <summary>
        /// Creates a new space instance from the given space base.
        /// </summary>
        /// <param name="spaceBase">The space base to create the space instance from.</param>
        /// <param name="createOptions">The create options.</param>
        internal SpaceInstance(SpaceBase spaceBase, CreateOptions createOptions)
            : base(spaceBase, createOptions)
        {
            if (spaceBase != null)
                Create(createOptions);
        }
        #endregion Constructor: SpaceInstance(SpaceBase spaceBase, CreateOptions createOptions)

        #region Constructor: SpaceInstance(SpaceValuedBase spaceValuedBase)
        /// <summary>
        /// Creates a new space instance from the given valued space base.
        /// </summary>
        /// <param name="spaceValuedBase">The valued space base to create the space instance from.</param>
        internal SpaceInstance(SpaceValuedBase spaceValuedBase)
            : this(spaceValuedBase, SemanticsEngineSettings.DefaultCreateOptions)
        {
        }
        #endregion Constructor: SpaceInstance(SpaceValuedBase spaceValuedBase)

        #region Constructor: SpaceInstance(SpaceValuedBase spaceValuedBase, CreateOptions createOptions)
        /// <summary>
        /// Creates a new space instance from the given valued space base.
        /// </summary>
        /// <param name="spaceValuedBase">The valued space base to create the space instance from.</param>
        /// <param name="createOptions">The create options.</param>
        internal SpaceInstance(SpaceValuedBase spaceValuedBase, CreateOptions createOptions)
            : base(spaceValuedBase, createOptions)
        {
            if (spaceValuedBase != null && spaceValuedBase.SpaceBase != null)
                Create(createOptions);
        }
        #endregion Constructor: SpaceInstance(SpaceValuedBase spaceValuedBase, CreateOptions createOptions)

        #region Constructor: SpaceInstance(SpaceInstance spaceInstance)
        /// <summary>
        /// Clones a space instance.
        /// </summary>
        /// <param name="space">The space to clone.</param>
        protected internal SpaceInstance(SpaceInstance spaceInstance)
            : base(spaceInstance)
        {
            if (spaceInstance != null)
            {
                this.Presence = spaceInstance.Presence;
                this.SpaceType = spaceInstance.SpaceType;
                this.StorageType = spaceInstance.StorageType;
                this.MaximumNumberOfItems = spaceInstance.MaximumNumberOfItems;
                this.Capacity = new NumericalValueInstance(spaceInstance.Capacity);

                foreach (TangibleObjectInstance item in spaceInstance.Items)
                    AddItem(new TangibleObjectInstance(item));

                if (spaceInstance.Shape != null)
                    this.Shape = spaceInstance.Shape.Clone();
                if (spaceInstance.OriginalShape != null)
                    this.OriginalShape = spaceInstance.OriginalShape.Clone();

                foreach (MatterInstance tangibleMatterInstance in spaceInstance.TangibleMatter)
                    AddTangibleMatter(tangibleMatterInstance.Clone());
            }
        }
        #endregion Constructor: SpaceInstance(SpaceInstance spaceInstance)

        #region Method: Create(CreateOptions createOptions)
        /// <summary>
        /// Create the items.
        /// </summary>
        /// <param name="createOptions">The create options.</param>
        private void Create(CreateOptions createOptions)
        {
            if (this.SpaceBase != null)
            {
                if (createOptions.Has(CreateOptions.Items))
                {
                    // Create instances from the mandatory base items
                    foreach (TangibleObjectBase tangibleObjectBase in this.SpaceBase.Items)
                    {
                        if (InstanceManager.IgnoreNecessity || tangibleObjectBase.Necessity == Necessity.Mandatory)
                            AddItem(InstanceManager.Current.Create(tangibleObjectBase, createOptions));
                    }
                }
            }

            if (this.SpaceValuedBase != null)
            {
                if (createOptions.Has(CreateOptions.Items))
                {
                    // Create instances from the mandatory base items
                    foreach (TangibleObjectBase tangibleObjectBase in this.SpaceValuedBase.Items)
                    {
                        if (InstanceManager.IgnoreNecessity || tangibleObjectBase.Necessity == Necessity.Mandatory)
                            AddItem(InstanceManager.Current.Create(tangibleObjectBase, createOptions));
                    }
                }

                if (createOptions.Has(CreateOptions.TangibleMatter))
                {
                    // Create the tangible matter
                    foreach (MatterValuedBase tangibleMatterBase in this.SpaceValuedBase.TangibleMatter)
                    {
                        if (InstanceManager.IgnoreNecessity || tangibleMatterBase.Necessity == Necessity.Mandatory)
                            AddTangibleMatter(InstanceManager.Current.Create<MatterInstance>(tangibleMatterBase));
                    }
                }
            }
        }
        #endregion Method: Create(CreateOptions createOptions)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddItem(TangibleObjectInstance item)
        /// <summary>
        /// Adds the given item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddItem(TangibleObjectInstance item)
        {
            return AddItem(item, false);
        }
        #endregion Method: AddItem(TangibleObjectInstance item)

        #region Method: AddItem(TangibleObjectInstance item, bool forceAdd)
        /// <summary>
        /// Adds the given item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="forceAdd">If true, the item is added, regardless of any placement managers.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddItem(TangibleObjectInstance item, bool forceAdd)
        {
            if (item != null)
            {
                // Check whether we already have the item
                if (this.Items.Contains(item))
                    return Message.RelationExistsAlready;

                // Check whether the item still fits in the space
                if (!forceAdd)
                {
                    switch (this.StorageType)
                    {
                        case StorageType.Unlimited:
                            break;
                        case StorageType.MaximumNumberOfItems:
                            if (this.NumberOfItems + 1 > this.MaximumNumberOfItems)
                                return Message.RelationFail;
                            break;
                        case StorageType.Capacity:
                            if (this.Size + item.Volume >= this.Capacity)
                                return Message.RelationFail;
                            break;
                        case StorageType.MaximumNumberOfItemsAndCapacity:
                            if ((this.NumberOfItems + 1 > this.MaximumNumberOfItems) || (this.Size + item.Volume >= this.Capacity))
                                return Message.RelationFail;
                            break;
                        default:
                            break;
                    }
                }

                // Remove the item from its current space
                if (item.Space != null)
                    item.Space.RemoveItem(item);

                // Try to find out whether the item fits, and if so, get its position
                Vec3 position = Vec3.Zero;
                if (forceAdd || PhysicsManager.Current.WillItFit(item, this, out position))
                {
                    // Add the item to this space
                    Utils.AddToArray<TangibleObjectInstance>(ref this.items, item);
                    NotifyPropertyChanged("Items");
                    item.Space = this;
                    
                    // Set the position
                    if (!forceAdd)
                        item.Position = position;

                    // Set the world if this has not been done before
                    if (item.World == null && this.World != null)
                        this.World.AddInstance(item);

                    // Invoke an event
                    if (ItemAdded != null)
                        ItemAdded.Invoke(this, item);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleItemAdded(this, item);

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: AddItem(TangibleObjectInstance item, bool forceAdd)

        #region Method: RemoveItem(TangibleObjectInstance item)
        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveItem(TangibleObjectInstance item)
        {
            if (item != null)
            {
                if (this.Items.Contains(item))
                {
                    // Remove the item
                    Utils.RemoveFromArray<TangibleObjectInstance>(ref this.items, item);
                    NotifyPropertyChanged("Items");
                    item.Space = null;

                    // Invoke an event
                    if (ItemRemoved != null)
                        ItemRemoved.Invoke(this, item);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleItemRemoved(this, item);

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveItem(TangibleObjectInstance item)

        #region Method: AddTangibleMatter(MatterInstance tangibleMatterInstance)
        /// <summary>
        /// Adds the given tangible matter.
        /// </summary>
        /// <param name="tangibleMatterInstance">The tangible matter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddTangibleMatter(MatterInstance tangibleMatterInstance)
        {
            SubstanceInstance substanceInstance = tangibleMatterInstance as SubstanceInstance;
            if (substanceInstance != null && (substanceInstance.Compound != null || substanceInstance.Mixture != null))
                return AddTangibleMatter(tangibleMatterInstance, substanceInstance.Position);

            return AddTangibleMatter(tangibleMatterInstance, this.Position);
        }

        /// <summary>
        /// Adds the given tangible matter.
        /// </summary>
        /// <param name="tangibleMatterInstance">The tangible matter to add.</param>
        /// <param name="position">The position the tangible matter should get.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddTangibleMatter(MatterInstance tangibleMatterInstance, Vec3 position)
        {
            if (tangibleMatterInstance != null)
            {
                bool ignoreMixing = false;
                SubstanceInstance substanceInstance = tangibleMatterInstance as SubstanceInstance;
                if (substanceInstance != null)
                    ignoreMixing = substanceInstance.Compound != null || substanceInstance.Mixture != null;

                if (position == null)
                    position = this.Position;

                // Check whether we already have the item
                if (this.TangibleMatter.Contains(tangibleMatterInstance))
                    return Message.RelationExistsAlready;

                // Check whether the tangible matter still fits in the space
                bool checkCapacity = false;
                switch (this.StorageType)
                {
                    case StorageType.Unlimited:
                        break;
                    case StorageType.MaximumNumberOfItems:
                        if (this.NumberOfItems + 1 > this.MaximumNumberOfItems)
                            return Message.RelationFail;
                        break;
                    case StorageType.Capacity:
                        checkCapacity = true;
                        break;
                    case StorageType.MaximumNumberOfItemsAndCapacity:
                        if (this.NumberOfItems + 1 > this.MaximumNumberOfItems)
                            return Message.RelationFail;
                        checkCapacity = true;
                        break;
                    default:
                        break;
                }
                if (checkCapacity)
                {
                    // Don't continue if the size is already greater than the capacity
                    NumericalValueInstance size = this.Size;
                    NumericalValueInstance capacity = this.Capacity;
                    if (size > capacity)
                        return Message.RelationFail;

                    // Check whether there will be an overflow when the matter is added
                    if ((size + tangibleMatterInstance.Quantity) - capacity > 0)
                    {
                        NumericalValueInstance usefulQuantity = capacity - size;

                        // In case of an overflow, only add what is possible; therefore, make a copy of the matter
                        MatterInstance copy = tangibleMatterInstance.Clone();
                        copy.Space = null;

                        // Reduce the quantity of the original matter, and set the quantity of the copied matter to the useful quantity
                        tangibleMatterInstance.Quantity -= usefulQuantity;
                        copy.Quantity.Value = 0;
                        copy.Quantity += usefulQuantity;

                        // Continue working with the copy
                        tangibleMatterInstance = copy;
                    }
                }

                // Remove the tangible matter from its current space
                if (tangibleMatterInstance.Space != null)
                    tangibleMatterInstance.Space.RemoveTangibleMatter(tangibleMatterInstance);

                // Set the position
                tangibleMatterInstance.Position = position;

                // Check whether the matter can mix with other matter it collides with; if there is a mixture or compound, add that one instead
                List<MatterInstance> allTangibleMatterToAdd = new List<MatterInstance>();
                List<MatterInstance> matterInstances = new List<MatterInstance>();
                matterInstances.Add(tangibleMatterInstance);
                if (!ignoreMixing)
                {
                    foreach (MatterInstance matterInstance in this.TangibleMatter)
                    {
                        SubstanceInstance substance = matterInstance as SubstanceInstance;
                        if ((substance == null || (substance.Compound == null && substance.Mixture == null)) && PhysicsManager.Current.Collide(matterInstance, tangibleMatterInstance))
                            matterInstances.Add(matterInstance);
                    }
                    MixtureInstance mixtureInstance = null;
                    if (MixtureInstance.TryCreate(matterInstances, out mixtureInstance))
                        allTangibleMatterToAdd.Add(mixtureInstance);
                    else
                    {
                        CompoundInstance compoundInstance = null;
                        if (CompoundInstance.TryCreate(matterInstances, out compoundInstance))
                            allTangibleMatterToAdd.Add(compoundInstance);
                    }
                }

                // If there is still anything left of the mixed tangible matter that was originally added, add the remaining quantity
                if (allTangibleMatterToAdd.Count == 0 || (tangibleMatterInstance != null && tangibleMatterInstance.Quantity != null && tangibleMatterInstance.Quantity.BaseValue > 0))
                    allTangibleMatterToAdd.Add(tangibleMatterInstance);

                foreach (MatterInstance tangibleMatterToAdd in allTangibleMatterToAdd)
                {
                    // If there is an instance that is based on the same matter, merge them if they collide
                    if (!ignoreMixing && tangibleMatterToAdd.MatterBase != null)
                    {
                        foreach (MatterInstance myMatterInstance in this.TangibleMatter)
                        {
                            if (tangibleMatterToAdd.MatterBase.Equals(myMatterInstance.MatterBase) && PhysicsManager.Current.Collide(tangibleMatterToAdd, myMatterInstance))
                            {
                                // Increase the quantity of our substance with the quantity of the other one
                                myMatterInstance.Quantity += tangibleMatterToAdd.Quantity;

                                // Deplete the quantity of the other, making it useless and to be removed
                                tangibleMatterToAdd.Quantity -= tangibleMatterToAdd.Quantity;

                                return Message.RelationSuccess;
                            }
                        }
                    }

                    // Add the tangible matter to this space
                    Utils.AddToArray<MatterInstance>(ref this.tangibleMatter, tangibleMatterToAdd);
                    NotifyPropertyChanged("TangibleMatter");
                    tangibleMatterToAdd.Space = this;

                    // Set the position
                    tangibleMatterToAdd.Position = position;

                    // Set the world if this has not been done before
                    if (tangibleMatterToAdd.World == null && this.World != null)
                        this.World.AddInstance(tangibleMatterToAdd);

                    // Invoke an event
                    if (TangibleMatterAdded != null)
                        TangibleMatterAdded.Invoke(this, tangibleMatterToAdd);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleTangibleMatterAdded(this, tangibleMatterToAdd);
                }
                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddTangibleMatter(MatterInstance tangibleMatterInstance)

        #region Method: RemoveTangibleMatter(MatterInstance tangibleMatterInstance)
        /// <summary>
        /// Removes the given tangible matter.
        /// </summary>
        /// <param name="tangibleMatterInstance">The tangible matter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveTangibleMatter(MatterInstance tangibleMatterInstance)
        {
            if (tangibleMatterInstance != null)
            {
                if (this.TangibleMatter.Contains(tangibleMatterInstance))
                {
                    // Remove the tangible matter
                    Utils.RemoveFromArray<MatterInstance>(ref this.tangibleMatter, tangibleMatterInstance);
                    NotifyPropertyChanged("TangibleMatter");
                    tangibleMatterInstance.Space = null;

                    // Invoke an event
                    if (TangibleMatterRemoved != null)
                        TangibleMatterRemoved.Invoke(this, tangibleMatterInstance);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleTangibleMatterRemoved(this, tangibleMatterInstance);

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveTangibleMatter(MatterInstance tangibleMatterInstance)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasItem(TangibleObjectBase item)
        /// <summary>
        /// Checks whether the space has an item of the given tangible object.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true when the space has an item of the given tangible object.</returns>
        public bool HasItem(TangibleObjectBase item)
        {
            if (item != null)
            {
                foreach (TangibleObjectInstance tangibleObjectInstance in this.Items)
                {
                    if (item.Equals(tangibleObjectInstance.TangibleObjectBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasItem(TangibleObjectBase item)

        #region Method: HasTangibleMatter(MatterBase matter)
        /// <summary>
        /// Checks whether the space has tangible matter of the given matter.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when the space has tangible matter of the matter.</returns>
        public bool HasTangibleMatter(MatterBase matter)
        {
            if (matter != null)
            {
                foreach (MatterInstance tangibleMatterInstance in this.TangibleMatter)
                {
                    if (matter.Equals(tangibleMatterInstance.MatterBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasTangibleMatter(MatterBase matter)

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
                foreach (TangibleObjectInstance item in this.Items)
                    item.MarkAsModified(modified, false);
                foreach (MatterInstance tangibleMatter in this.TangibleMatter)
                    tangibleMatter.MarkAsModified(modified, false);

                if (this.PhysicalObject != null)
                    this.PhysicalObject.MarkAsModified(modified, false);
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
            Vec3 oldPosition = new Vec3(this.Position);
            Vec3 delta = newPosition - oldPosition;

            // First set the position of the base
            base.SetPosition(newPosition);

            // Then move all the space items and tangible matter
            if (this.Presence == Presence.Attached)
            {
                foreach (TangibleObjectInstance item in this.Items)
                    item.Position += delta;
                foreach (MatterInstance tangibleMatter in this.TangibleMatter)
                    tangibleMatter.Position += delta;
            }

            TransformToPositionAndRotation();
        }
        #endregion Method: SetPosition(Vec3 newPosition)

        #region Method: SetRotation(Quaternion newRotation)
        /// <summary>
        /// Set the new rotation.
        /// </summary>
        /// <param name="newRotation">The new rotation.</param>
        protected override void SetRotation(Quaternion newRotation)
        {
            // First set the position of the base
            base.SetRotation(newRotation);

            // Then move all the parts and covers
            if (this.Presence == Presence.Attached)
            {
                foreach (TangibleObjectInstance item in this.Items)
                {
                    //--- When we assume an offset -> this should be different
                    if ((item.Position - this.Position).length() > float.Epsilon)
                        throw new System.NotImplementedException();
                    item.Rotation = newRotation;
                }
            }
            foreach (SpaceInstance space in this.Spaces)
            {
                //--- When we assume an offset -> this should be different
                if ((space.Position - this.Position).length() > float.Epsilon)
                    throw new System.NotImplementedException();
                space.Rotation = newRotation;
            }

            TransformToPositionAndRotation();
        }
        #endregion Method: SetPosition(Vec3 newPosition)

        #region Method: SetIsLocked(bool isLockedValue)
        /// <summary>
        /// Set the new value for IsLocked.
        /// </summary>
        /// <param name="isLockedValue">The new value for IsLocked.</param>
        protected override void SetIsLocked(bool isLockedValue)
        {
            // First set the base
            base.SetIsLocked(isLockedValue);

            // Then lock all items
            foreach (TangibleObjectInstance item in this.Items)
                item.IsLocked = isLockedValue;
        }
        #endregion Method: SetIsLocked(bool isLockedValue)

        #region Method: SetBaseShape(BaseShape baseShape)
        /// <summary>
        /// Set the base shape.
        /// </summary>
        /// <param name="baseShape">The base shape to set.</param>
        public void SetBaseShape(BaseShape baseShape)
        {
            if (baseShape != null)
            {
                Box boundingBox = baseShape.GetBoundingBox();
                SetBoundingBox(boundingBox);
                this.Shape = baseShape;
                this.OriginalShape = baseShape.Clone();

                //--- Now the child spaces of this space can be generated based on the bounding box of the parent space
                Common.MathParser.MathFunctionSolverWithCustomConstants mfswcc = new Common.MathParser.MathFunctionSolverWithCustomConstants(null);
                mfswcc.AddConstant(SemanticsSettings.General.BoundingBoxName, new FunctionalBox(boundingBox));

                foreach (SpaceInstance subSpace in this.Spaces)
                {
                    if (subSpace.SpaceValuedBase.ShapeDescription.Type != BaseShapeType.Undefined)
                        subSpace.SetBaseShape(subSpace.SpaceValuedBase.ShapeDescription.Instantiate(mfswcc));
                }

                TransformToPositionAndRotation();
            }
        }
        #endregion Method: SetBaseShape(BaseShape baseShape)

        #region Method: Transform(Matrix4 transform)
        /// <summary>
        /// Transform the shape.
        /// </summary>
        /// <param name="transform">The transformation matrix.</param>
        public void Transform(Matrix4 transform)
        {
            if (this.OriginalShape != null)
                this.Shape = this.OriginalShape.Transform(transform);
            //foreach (SpaceInstance s in this.Spaces)
            //    s.Transform(transform);
        }
        #endregion Method: Transform(Matrix4 transform)

        #region Method: TransformToPositionAndRotation()
        /// <summary>
        /// Transform the shape.
        /// </summary>
        private void TransformToPositionAndRotation()
        {
            Transform(Matrix4.RotationY(this.Rotation.Yaw) * Matrix4.Translation(this.Position));
        }
        #endregion Method: TransformToPositionAndRotation()

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the space satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the space.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the space satisfies the condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null)
            {
                // Check whether the base satisfies the condition
                ignoreSpaceCheck = true;
                if (base.Satisfies(conditionBase, iVariableInstanceHolder))
                {
                    // Space condition
                    SpaceConditionBase spaceConditionBase = conditionBase as SpaceConditionBase;
                    if (spaceConditionBase != null)
                    {
                        // Check whether properties are satisfied
                        if ((spaceConditionBase.SpaceTypeSign == null || spaceConditionBase.SpaceType == null || Toolbox.Compare(this.SpaceType, (EqualitySign)spaceConditionBase.SpaceTypeSign, (SpaceType)spaceConditionBase.SpaceType)) &&
                            (spaceConditionBase.StorageTypeSign == null || spaceConditionBase.StorageType == null || Toolbox.Compare(this.StorageType, (EqualitySign)spaceConditionBase.StorageTypeSign, (StorageType)spaceConditionBase.StorageType)) &&
                            (spaceConditionBase.MaximumNumberOfItemsSign == null || spaceConditionBase.MaximumNumberOfItems == null || Toolbox.Compare(this.MaximumNumberOfItems, (EqualitySignExtended)spaceConditionBase.MaximumNumberOfItemsSign, (uint)spaceConditionBase.MaximumNumberOfItems)) &&
                            (spaceConditionBase.Capacity == null || (this.Capacity != null && this.Capacity.Satisfies(spaceConditionBase.Capacity, iVariableInstanceHolder))) &&
                            (spaceConditionBase.Size == null || (this.Size != null && this.Size.Satisfies(spaceConditionBase.Size, iVariableInstanceHolder))) &&
                            (spaceConditionBase.IsFull == null || Toolbox.Compare(this.IsFull, EqualitySign.Equal, (bool)spaceConditionBase.IsFull)))
                        {
                            // Check whether the required items are there
                            foreach (TangibleObjectConditionBase tangibleObjectConditionBase in spaceConditionBase.Items)
                            {
                                if (tangibleObjectConditionBase.Quantity == null ||
                                    tangibleObjectConditionBase.Quantity.BaseValue == null ||
                                    tangibleObjectConditionBase.Quantity.ValueSign == null)
                                {
                                    bool satisfied = false;
                                    foreach (TangibleObjectInstance item in this.Items)
                                    {
                                        if (item.Satisfies(tangibleObjectConditionBase, iVariableInstanceHolder))
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
                                    int requiredQuantity = (int)tangibleObjectConditionBase.Quantity.BaseValue;
                                    foreach (PhysicalObjectInstance physicalObjectInstance in this.Items)
                                    {
                                        if (physicalObjectInstance.Satisfies(tangibleObjectConditionBase, iVariableInstanceHolder))
                                            quantity++;
                                    }
                                    if (!Toolbox.Compare(quantity, (EqualitySignExtended)tangibleObjectConditionBase.Quantity.ValueSign, requiredQuantity))
                                        return false;
                                }
                            }

                            // Check whether the instance has the required tangible matter
                            foreach (MatterConditionBase matterConditionBase in spaceConditionBase.TangibleMatter)
                            {
                                bool satisfied = false;
                                foreach (MatterInstance tangibleMatterInstance in this.TangibleMatter)
                                {
                                    if (tangibleMatterInstance.Satisfies(matterConditionBase, iVariableInstanceHolder))
                                    {
                                        satisfied = true;
                                        break;
                                    }
                                }
                                if (!satisfied)
                                    return false;
                            }

                            return true;
                        }
                        return false;
                    }
                    return true;
                }
                else
                {
                    // Tangible object condition
                    TangibleObjectConditionBase tangibleObjectCondition = conditionBase as TangibleObjectConditionBase;
                    if (tangibleObjectCondition != null)
                    {
                        foreach (TangibleObjectInstance tangibleObjectInstance in this.Items)
                        {
                            if (tangibleObjectInstance.Satisfies(tangibleObjectCondition, iVariableInstanceHolder))
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
        /// Apply the given change to the space.
        /// </summary>
        /// <param name="changeBase">The change to apply to the space.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
            {
                if (base.Apply(changeBase, iVariableInstanceHolder))
                {
                    // Space change
                    SpaceChangeBase spaceChangeBase = changeBase as SpaceChangeBase;
                    if (spaceChangeBase != null)
                    {
                        // Adjust the space type
                        if (spaceChangeBase.SpaceType != null)
                            this.SpaceType = (SpaceType)spaceChangeBase.SpaceType;

                        // Adjust the storage type
                        if (spaceChangeBase.StorageType != null)
                            this.StorageType = (StorageType)spaceChangeBase.StorageType;

                        // Adjust the maximum number of items
                        if (spaceChangeBase.MaximumNumberOfItems != null)
                        {
                            if (spaceChangeBase.MaximumNumberOfItemsChangeType == null || spaceChangeBase.MaximumNumberOfItemsChangeType == ValueChangeType.Equate)
                                this.MaximumNumberOfItems = (uint)spaceChangeBase.MaximumNumberOfItems;
                            else if (spaceChangeBase.MaximumNumberOfItemsChangeType == ValueChangeType.Increase)
                                this.MaximumNumberOfItems += (uint)spaceChangeBase.MaximumNumberOfItems;
                            else if (spaceChangeBase.MaximumNumberOfItemsChangeType == ValueChangeType.Decrease)
                                this.MaximumNumberOfItems -= (uint)spaceChangeBase.MaximumNumberOfItems;
                        }

                        // Adjust the capacity
                        if (spaceChangeBase.Capacity != null)
                            this.Capacity.Apply(spaceChangeBase.Capacity, iVariableInstanceHolder);

                        // Change the items
                        foreach (TangibleObjectChangeBase tangibleObjectChangeBase in spaceChangeBase.Items)
                        {
                            foreach (TangibleObjectInstance tangibleObjectInstance in this.Items)
                                tangibleObjectInstance.Apply(tangibleObjectChangeBase, iVariableInstanceHolder);
                        }

                        // Add the items
                        foreach (TangibleObjectValuedBase tangibleObjectValuedBase in spaceChangeBase.ItemsToAdd)
                        {
                            for (int i = 0; i < tangibleObjectValuedBase.Quantity.BaseValue; i++)
                                AddItem(InstanceManager.Current.Create<TangibleObjectInstance>(tangibleObjectValuedBase.TangibleObjectBase));
                        }
                        
                        // Remove the items
                        foreach (TangibleObjectConditionBase tangibleObjectConditionBase in spaceChangeBase.ItemsToRemove)
                        {
                            if (tangibleObjectConditionBase.Quantity == null ||
                                tangibleObjectConditionBase.Quantity.BaseValue == null ||
                                tangibleObjectConditionBase.Quantity.ValueSign == null)
                            {
                                foreach (TangibleObjectInstance tangibleObjectInstance in this.Items)
                                {
                                    if (tangibleObjectInstance.Satisfies(tangibleObjectConditionBase, iVariableInstanceHolder))
                                    {
                                        RemoveItem(tangibleObjectInstance);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                int quantity = (int)tangibleObjectConditionBase.Quantity.BaseValue;
                                if (quantity > 0)
                                {
                                    ReadOnlyCollection<TangibleObjectInstance> items = this.Items;
                                    foreach (TangibleObjectInstance tangibleObjectInstance in items)
                                    {
                                        if (tangibleObjectInstance.Satisfies(tangibleObjectConditionBase, iVariableInstanceHolder))
                                        {
                                            RemoveItem(tangibleObjectInstance);
                                            quantity--;
                                            if (quantity <= 0)
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                        // Change the tangible matter
                        foreach (MatterChangeBase matterChangeBase in spaceChangeBase.TangibleMatter)
                        {
                            foreach (MatterInstance tangibleMatterInstance in this.TangibleMatter)
                                tangibleMatterInstance.Apply(matterChangeBase, iVariableInstanceHolder);
                        }

                        // Add the tangible matter
                        foreach (MatterValuedBase matterValuedBase in spaceChangeBase.TangibleMatterToAdd)
                            AddTangibleMatter(InstanceManager.Current.Create<MatterInstance>(matterValuedBase));

                        // Remove the tangible matter
                        foreach (MatterConditionBase matterConditionBase in spaceChangeBase.TangibleMatterToRemove)
                        {
                            if (matterConditionBase.Quantity == null ||
                                matterConditionBase.Quantity.BaseValue == null ||
                                matterConditionBase.Quantity.ValueSign == null)
                            {
                                foreach (MatterInstance tangibleMatterInstance in this.TangibleMatter)
                                {
                                    if (tangibleMatterInstance.Satisfies(matterConditionBase, iVariableInstanceHolder))
                                    {
                                        RemoveTangibleMatter(tangibleMatterInstance);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                float quantity = (float)matterConditionBase.Quantity.BaseValue;
                                if (quantity > 0)
                                {
                                    ReadOnlyCollection<MatterInstance> tangibleMatter = this.TangibleMatter;
                                    foreach (MatterInstance tangibleMatterInstance in tangibleMatter)
                                    {
                                        if (tangibleMatterInstance.Satisfies(matterConditionBase, iVariableInstanceHolder))
                                        {
                                            if (tangibleMatterInstance.Quantity.BaseValue > quantity)
                                            {
                                                tangibleMatterInstance.Quantity -= quantity;
                                                break;
                                            }
                                            else
                                            {
                                                quantity -= tangibleMatterInstance.Quantity.BaseValue;
                                                RemoveTangibleMatter(tangibleMatterInstance);
                                                if (quantity <= 0)
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    // Tangible object change
                    TangibleObjectChangeBase tangibleObjectChange = changeBase as TangibleObjectChangeBase;
                    if (tangibleObjectChange != null)
                    {
                        foreach (TangibleObjectInstance tangibleObjectInstance in this.Items)
                            tangibleObjectInstance.Apply(tangibleObjectChange, iVariableInstanceHolder);
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
            string returnString = this.DefaultName + " {";

            ReadOnlyCollection<TangibleObjectInstance> items = this.Items;
            for (int i = 0; i < items.Count; i++)
            {
                if (i != 0)
                    returnString += ", ";
                returnString += items[i].DefaultName;
            }
            returnString += " ; ";
            ReadOnlyCollection<MatterInstance> tangibleMatter = this.TangibleMatter;
            for (int i = 0; i < tangibleMatter.Count; i++)
            {
                if (i != 0)
                    returnString += ", ";
                returnString += tangibleMatter[i].DefaultName;
            }

            returnString += "}";

            return returnString;
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: SpaceInstance

}