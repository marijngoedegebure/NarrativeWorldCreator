/**************************************************************************
 * 
 * Space.cs
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
using Common.Shapes;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Entities
{

    #region Class: Space
    /// <summary>
    /// A space that can contain items.
    /// </summary>
    public class Space : PhysicalObject, IComparable<Space>
    {

        #region Properties and Fields

        #region Property: Presence
        /// <summary>
        /// Gets or sets the value that indicates whether items in the space are attached to it (so move along), or just present.
        /// </summary>
        public Presence Presence
        {
            get
            {
                return Database.Current.Select<Presence>(this.ID, GenericTables.Space, Columns.Presence);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Space, Columns.Presence, value);
                NotifyPropertyChanged("Presence");
            }
        }
        #endregion Property: Presence

        #region Property: Items
        /// <summary>
        /// Gets all the items in the space.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectValued> Items
        {
            get
            {
                return Database.Current.SelectAll<TangibleObjectValued>(this.ID, GenericTables.SpaceItem, Columns.TangibleObjectValued).AsReadOnly();
            }
        }
        #endregion Property: Items

        #region Property: PhysicalObjectsSpaceOf
        /// <summary>
        /// Gets the physical objects of which this is a space.
        /// </summary>
        public ReadOnlyCollection<PhysicalObject> PhysicalObjectsSpaceOf
        {
            get
            {
                return Database.Current.SelectAll<PhysicalObject>(GenericTables.PhysicalObjectSpace, Columns.Space, this.ID).AsReadOnly();
            }
        }
        #endregion Property: PhysicalObjectsSpaceOf

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Space()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Space()
        {
            // Items
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.TangibleObjectValued, new Tuple<Type, EntryType>(typeof(TangibleObjectValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.SpaceItem, typeof(Space), dict);
        }
        #endregion Static Constructor: Space()

        #region Constructor: Space()
        /// <summary>
        /// Creates a new space.
        /// </summary>
        public Space()
            : base()
        {
        }
        #endregion Constructor: Space()

        #region Constructor: Space(uint id)
        /// <summary>
        /// Creates a new space from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new space from.</param>
        protected Space(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Space(uint id)

        #region Constructor: Space(string name)
        /// <summary>
        /// Creates a new space with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the space.</param>
        public Space(string name)
            : base(name)
        {
        }
        #endregion Constructor: Space(string name)

        #region Constructor: Space(Space space)
        /// <summary>
        /// Clones a space.
        /// </summary>
        /// <param name="space">The space to clone.</param>
        public Space(Space space)
            : base(space)
        {
            if (space != null)
            {
                Database.Current.StartChange();

                this.Presence = space.Presence;
                foreach (TangibleObjectValued item in space.Items)
                    AddItem(new TangibleObjectValued(item));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Space(Space space)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddItem(TangibleObjectValued item)
        /// <summary>
        /// Adds the given item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddItem(TangibleObjectValued item)
        {
            if (item != null)
            {
                // Check whether we already have a similar item
                if (HasItem(item.TangibleObject))
                    return Message.RelationExistsAlready;

                // Make sure to prevent looping
                if (HasPhysicalObject(item.TangibleObject))
                    return Message.RelationFail;

                // Add the item
                Database.Current.Insert(this.ID, GenericTables.SpaceItem, new string[] { Columns.TangibleObject, Columns.TangibleObjectValued }, new object[] { item.TangibleObject, item });
                NotifyPropertyChanged("Items");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddItem(TangibleObjectValued item)

        #region Method: RemoveItem(TangibleObjectValued item)
        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveItem(TangibleObjectValued item)
        {
            if (item != null)
            {
                if (this.Items.Contains(item))
                {
                    // Remove the item
                    Database.Current.Remove(this.ID, GenericTables.SpaceItem, Columns.TangibleObjectValued, item);
                    NotifyPropertyChanged("Items");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveItem(TangibleObjectValued item)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasItem(TangibleObject item)
        /// <summary>
        /// Checks whether the space has an item of the given tangible object.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true when the space has an item of the given tangible object.</returns>
        public bool HasItem(TangibleObject item)
        {
            if (item != null)
            {
                foreach (TangibleObjectValued tangibleObjectValued in this.Items)
                {
                    if (item.Equals(tangibleObjectValued.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasItem(TangibleObject item)

        #region Method: Remove()
        /// <summary>
        /// Remove the space.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the items
            foreach (TangibleObjectValued tangibleObjectValued in this.Items)
                tangibleObjectValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.SpaceItem);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Space other)
        /// <summary>
        /// Compares the space to the other space.
        /// </summary>
        /// <param name="other">The space to compare to this space.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Space other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Space other)

        #endregion Method Group: Other

    }
    #endregion Class: Space

    #region Class: SpaceValued
    /// <summary>
    /// A valued version of a space.
    /// </summary>
    public class SpaceValued : PhysicalObjectValued
    {

        #region Properties and Fields

        #region Property: Space
        /// <summary>
        /// Gets the space of which this is a valued space.
        /// </summary>
        public Space Space
        {
            get
            {
                return this.Node as Space;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Space

        #region Property: SpaceType
        /// <summary>
        /// Gets or sets the type of the space.
        /// </summary>
        public SpaceType SpaceType
        {
            get
            {
                return Database.Current.Select<SpaceType>(this.ID, ValueTables.SpaceValued, Columns.Type);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceValued, Columns.Type, value);
                NotifyPropertyChanged("SpaceType");
            }
        }
        #endregion Property: SpaceType

        #region Property: StorageType
        /// <summary>
        /// Gets or sets the storage type.
        /// </summary>
        public StorageType StorageType
        {
            get
            {
                return Database.Current.Select<StorageType>(this.ID, ValueTables.SpaceValued, Columns.StorageType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceValued, Columns.StorageType, value);
                NotifyPropertyChanged("StorageType");
            }
        }
        #endregion Property: StorageType

        #region Property: MaximumNumberOfItems
        /// <summary>
        /// Gets or sets the maximum number of items this valued space can hold.
        /// Only valid for the right Storage Type!
        /// </summary>
        public uint MaximumNumberOfItems
        {
            get
            {
                return Database.Current.Select<uint>(this.ID, ValueTables.SpaceValued, Columns.MaximumNumberOfItems);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceValued, Columns.MaximumNumberOfItems, value);
                NotifyPropertyChanged("MaximumNumberOfItems");
            }
        }
        #endregion Property: MaximumNumberOfItems

        #region Property: Capacity
        /// <summary>
        /// Gets or sets the capacity of the space.
        /// Only valid for the right Storage Type!
        /// </summary>
        public NumericalValue Capacity
        {
            get
            {
                return Database.Current.Select<NumericalValue>(this.ID, ValueTables.SpaceValued, Columns.Capacity);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceValued, Columns.Capacity, value);
                NotifyPropertyChanged("Capacity");
            }
        }
        #endregion Property: Capacity

        #region Property: ShapeDescription
        /// <summary>
        /// A handler for a change in the base shape description.
        /// </summary>
        private PropertyChangedEventHandler shapeDescriptionChanged;

        /// <summary>
        /// The shape description of the space.
        /// </summary>
        private BaseShapeDescription shapeDescription = null;

        /// <summary>
        /// Gets or sets the shape description of the space.
        /// </summary>
        public BaseShapeDescription ShapeDescription
        {
            get
            {
                if (shapeDescription == null)
                {
                    shapeDescription = Database.Current.Select<BaseShapeDescription>(this.ID, ValueTables.SpaceValued, Columns.ShapeDescription);

                    if (shapeDescription != null)
                    {
                        if (shapeDescriptionChanged == null)
                            shapeDescriptionChanged = new PropertyChangedEventHandler(shapeDescription_PropertyChanged);

                        shapeDescription.PropertyChanged += shapeDescriptionChanged;
                    }
                }

                return shapeDescription;
            }
            set
            {
                if (shapeDescriptionChanged == null)
                    shapeDescriptionChanged = new PropertyChangedEventHandler(shapeDescription_PropertyChanged);

                if (shapeDescription != null)
                    shapeDescription.PropertyChanged -= shapeDescriptionChanged;

                shapeDescription = value;
                Database.Current.Update(this.ID, ValueTables.SpaceValued, Columns.ShapeDescription, shapeDescription);
                NotifyPropertyChanged("ShapeDescription");

                if (shapeDescription != null)
                    shapeDescription.PropertyChanged += shapeDescriptionChanged;
            }
        }

        /// <summary>
        /// Updates the database when a property of the base shape description changes.
        /// </summary>
        private void shapeDescription_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Database.Current.Update(this.ID, ValueTables.SpaceValued, Columns.ShapeDescription, shapeDescription);
            NotifyPropertyChanged("ShapeDescription");
        }
        #endregion Property: ShapeDescription

        #region Property: Items
        /// <summary>
        /// Gets the items in the valued space.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectValued> Items
        {
            get
            {
                return Database.Current.SelectAll<TangibleObjectValued>(this.ID, ValueTables.SpaceValuedItem, Columns.TangibleObjectValued).AsReadOnly();
            }
        }
        #endregion Property: Items

        #region Property: AllItems
        /// <summary>
        /// Gets all items in the valued space and the space.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectValued> AllItems
        {
            get
            {
                List<TangibleObjectValued> allItems = new List<TangibleObjectValued>();
                allItems.AddRange(this.Items);
                if (this.Space != null)
                    allItems.AddRange(this.Space.Items);
                return allItems.AsReadOnly();
            }
        }
        #endregion Property: AllItems

        #region Property: TangibleMatter
        /// <summary>
        /// Gets all the tangible matter in the space.
        /// </summary>
        public ReadOnlyCollection<MatterValued> TangibleMatter
        {
            get
            {
                return Database.Current.SelectAll<MatterValued>(this.ID, ValueTables.SpaceValuedTangibleMatter, Columns.TangibleMatter).AsReadOnly();
            }
        }
        #endregion Property: TangibleMatter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: SpaceValued()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static SpaceValued()
        {
            // Items
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.TangibleObjectValued, new Tuple<Type, EntryType>(typeof(TangibleObjectValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.SpaceValuedItem, typeof(SpaceValued), dict);

            // Tangible matter
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.TangibleMatter, new Tuple<Type, EntryType>(typeof(MatterValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.SpaceValuedTangibleMatter, typeof(SpaceValued), dict);
        }
        #endregion Static Constructor: SpaceValued()

        #region Constructor: SpaceValued(uint id)
        /// <summary>
        /// Creates a new valued space from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new valued space from.</param>
        protected SpaceValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SpaceValued(uint id)

        #region Constructor: SpaceValued(SpaceValued spaceValued)
        /// <summary>
        /// Clones a valued space.
        /// </summary>
        /// <param name="spaceValued">The valued space to clone.</param>
        public SpaceValued(SpaceValued spaceValued)
            : base(spaceValued)
        {
            if (spaceValued != null)
            {
                Database.Current.StartChange();

                this.SpaceType = spaceValued.SpaceType;
                this.StorageType = spaceValued.StorageType;
                this.MaximumNumberOfItems = spaceValued.MaximumNumberOfItems;
                this.Capacity = new NumericalValue(spaceValued.Capacity);
                if (spaceValued.ShapeDescription != null)
                    this.ShapeDescription = new BaseShapeDescription(spaceValued.ShapeDescription);
                else
                    this.ShapeDescription = new BaseShapeDescription();
                foreach (TangibleObjectValued item in spaceValued.Items)
                    AddItem(new TangibleObjectValued(item));
                foreach (MatterValued tangibleMatter in spaceValued.TangibleMatter)
                    AddTangibleMatter(tangibleMatter.Clone());

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: SpaceValued(SpaceValued spaceValued)

        #region Constructor: SpaceValued(Space space)
        /// <summary>
        /// Creates a new valued space from the given space.
        /// </summary>
        /// <param name="space">The space to create a valued space from.</param>
        public SpaceValued(Space space)
            : base(space)
        {
            Database.Current.StartChange();

            this.ShapeDescription = new BaseShapeDescription();
            this.MaximumNumberOfItems = SemanticsSettings.Values.MaximumNumberOfItems;
            this.Capacity = new NumericalValue(SemanticsSettings.Values.Capacity, Prefix.None, null, 0, SemanticsSettings.Values.MaxValue);

            Database.Current.StopChange();
        }
        #endregion Constructor: SpaceValued(Space space)

        #region Constructor: SpaceValued(Space space, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new valued space from the given space in the given quantity.
        /// </summary>
        /// <param name="space">The space to create a valued space from.</param>
        /// <param name="quantity">The quantity of the valued space.</param>
        public SpaceValued(Space space, NumericalValueRange quantity)
            : base(space, quantity)
        {
            Database.Current.StartChange();

            this.ShapeDescription = new BaseShapeDescription();
            this.MaximumNumberOfItems = SemanticsSettings.Values.MaximumNumberOfItems;
            this.Capacity = new NumericalValue(SemanticsSettings.Values.Capacity, Prefix.None, null, 0, SemanticsSettings.Values.MaxValue);

            Database.Current.StopChange();
        }
        #endregion Constructor: SpaceValued(Space space, NumericalValueRange quantity)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddItem(TangibleObjectValued item)
        /// <summary>
        /// Adds the given item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddItem(TangibleObjectValued item)
        {
            if (item != null)
            {
                // Check whether we already have a similar item
                if (HasItem(item.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the item
                Database.Current.Insert(this.ID, ValueTables.SpaceValuedItem, new string[] { Columns.TangibleObject, Columns.TangibleObjectValued }, new object[] { item.TangibleObject, item });
                NotifyPropertyChanged("Items");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddItem(TangibleObjectValued item)

        #region Method: RemoveItem(TangibleObjectValued item)
        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveItem(TangibleObjectValued item)
        {
            if (item != null)
            {
                if (HasItem(item.TangibleObject))
                {
                    // Remove the item
                    Database.Current.Remove(this.ID, ValueTables.SpaceValuedItem, Columns.TangibleObjectValued, item);
                    NotifyPropertyChanged("Items");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveItem(TangibleObjectValued item)

        #region Method: AddTangibleMatter(MatterValued tangibleMatter)
        /// <summary>
        /// Adds the given tangible matter.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddTangibleMatter(MatterValued tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                // Check whether we already have similar tangible matter
                if (tangibleMatter.Matter != null && HasTangibleMatter(tangibleMatter.Matter))
                    return Message.RelationExistsAlready;

                // Add the tangible matter
                Database.Current.Insert(this.ID, ValueTables.SpaceValuedTangibleMatter, Columns.TangibleMatter, tangibleMatter);
                NotifyPropertyChanged("TangibleMatter");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddTangibleMatter(MatterValued tangibleMatter)

        #region Method: RemoveTangibleMatter(MatterValued tangibleMatter)
        /// <summary>
        /// Removes the given tangible matter.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveTangibleMatter(MatterValued tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                if (this.TangibleMatter.Contains(tangibleMatter))
                {
                    // Remove the tangible matter
                    Database.Current.Remove(this.ID, ValueTables.SpaceValuedTangibleMatter, Columns.TangibleMatter, tangibleMatter);
                    NotifyPropertyChanged("TangibleMatter");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveTangibleMatter(MatterValued tangibleMatter)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasItem(TangibleObject item)
        /// <summary>
        /// Checks whether the space has an item of the given tangible object.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true when the space has an item of the given tangible object.</returns>
        public bool HasItem(TangibleObject item)
        {
            if (item != null)
            {
                foreach (TangibleObjectValued tangibleObjectValued in this.Items)
                {
                    if (item.Equals(tangibleObjectValued.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasItem(TangibleObject item)

        #region Method: HasTangibleMatter(Matter matter)
        /// <summary>
        /// Checks whether the space has tangible matter of the given matter.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when the space has tangible matter of the matter.</returns>
        public bool HasTangibleMatter(Matter matter)
        {
            if (matter != null)
            {
                foreach (MatterValued tangibleMatter in this.TangibleMatter)
                {
                    if (tangibleMatter.Matter != null && matter.Equals(tangibleMatter.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasTangibleMatter(Matter matter)

        #region Method: Remove()
        /// <summary>
        /// Remove the valued space.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the capacity
            if (this.Capacity != null)
                this.Capacity.Remove();

            // Remove the items
            foreach (TangibleObjectValued tangibleObjectValued in this.Items)
                tangibleObjectValued.Remove();
            Database.Current.Remove(this.ID, ValueTables.SpaceValuedItem);

            // Remove the tangible matter
            foreach (MatterValued tangibleMatter in this.TangibleMatter)
                tangibleMatter.Remove();
            Database.Current.Remove(this.ID, ValueTables.SpaceValuedTangibleMatter);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: SpaceValued

    #region Class: SpaceCondition
    /// <summary>
    /// A condition on a space.
    /// </summary>
    public class SpaceCondition : PhysicalObjectCondition
    {

        #region Properties and Fields

        #region Property: Space
        /// <summary>
        /// Gets or sets the required space.
        /// </summary>
        public Space Space
        {
            get
            {
                return this.Node as Space;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Space

        #region Property: SpaceType
        /// <summary>
        /// Gets or sets the required space type.
        /// </summary>
        public SpaceType? SpaceType
        {
            get
            {
                return Database.Current.Select<SpaceType?>(this.ID, ValueTables.SpaceCondition, Columns.Type);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceCondition, Columns.Type, value);
                NotifyPropertyChanged("SpaceType");
            }
        }
        #endregion Property: SpaceType

        #region Property: SpaceTypeSign
        /// <summary>
        /// Gets or sets the sign for the space type.
        /// </summary>
        public EqualitySign? SpaceTypeSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.SpaceCondition, Columns.TypeSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceCondition, Columns.TypeSign, value);
                NotifyPropertyChanged("SpaceTypeSign");
            }
        }
        #endregion Property: SpaceTypeSign

        #region Property: StorageType
        /// <summary>
        /// Gets or sets the required storage type.
        /// </summary>
        public StorageType? StorageType
        {
            get
            {
                return Database.Current.Select<StorageType?>(this.ID, ValueTables.SpaceCondition, Columns.StorageType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceCondition, Columns.StorageType, value);
                NotifyPropertyChanged("StorageType");
            }
        }
        #endregion Property: StorageType

        #region Property: StorageTypeSign
        /// <summary>
        /// Gets or sets the sign for the storage type.
        /// </summary>
        public EqualitySign? StorageTypeSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.SpaceCondition, Columns.StorageTypeSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceCondition, Columns.StorageTypeSign, value);
                NotifyPropertyChanged("StorageTypeSign");
            }
        }
        #endregion Property: StorageTypeSign

        #region Property: MaximumNumberOfItems
        /// <summary>
        /// Gets or sets the maximum number of items this valued space should hold.
        /// </summary>
        public uint? MaximumNumberOfItems
        {
            get
            {
                return Database.Current.Select<uint?>(this.ID, ValueTables.SpaceCondition, Columns.MaximumNumberOfItems);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceCondition, Columns.MaximumNumberOfItems, value);
                NotifyPropertyChanged("MaximumNumberOfItems");
            }
        }
        #endregion Property: MaximumNumberOfItems

        #region Property: MaximumNumberOfItemsSign
        /// <summary>
        /// Gets or sets the sign for the maximum number of items.
        /// </summary>
        public EqualitySignExtended? MaximumNumberOfItemsSign
        {
            get
            {
                return Database.Current.Select<EqualitySignExtended?>(this.ID, ValueTables.SpaceCondition, Columns.MaximumNumberOfItemsSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceCondition, Columns.MaximumNumberOfItemsSign, value);
                NotifyPropertyChanged("MaximumNumberOfItemsSign");
            }
        }
        #endregion Property: MaximumNumberOfItemsSign

        #region Property: Capacity
        /// <summary>
        /// Gets or sets the required capacity.
        /// </summary>
        public NumericalValueCondition Capacity
        {
            get
            {
                return Database.Current.Select<NumericalValueCondition>(this.ID, ValueTables.SpaceCondition, Columns.Capacity);
            }
            set
            {
                if (this.Capacity != null)
                    this.Capacity.Remove();

                Database.Current.Update(this.ID, ValueTables.SpaceCondition, Columns.Capacity, value);
                NotifyPropertyChanged("Capacity");
            }
        }
        #endregion Property: Capacity

        #region Property: NumberOfItems
        /// <summary>
        /// Gets or sets the number of items the space should hold (equivalent to maximum number of items).
        /// </summary>
        public uint? NumberOfItems
        {
            get
            {
                return Database.Current.Select<uint?>(this.ID, ValueTables.SpaceCondition, Columns.NumberOfItems);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceCondition, Columns.NumberOfItems, value);
                NotifyPropertyChanged("NumberOfItems");
            }
        }
        #endregion Property: NumberOfItems

        #region Property: NumberOfItemsSign
        /// <summary>
        /// Gets or sets the sign for the number of items.
        /// </summary>
        public EqualitySignExtended? NumberOfItemsSign
        {
            get
            {
                return Database.Current.Select<EqualitySignExtended?>(this.ID, ValueTables.SpaceCondition, Columns.NumberOfItemsSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceCondition, Columns.NumberOfItemsSign, value);
                NotifyPropertyChanged("NumberOfItemsSign");
            }
        }
        #endregion Property: NumberOfItemsSign

        #region Property: Size
        /// <summary>
        /// Gets or sets the required size (equivalent to the capacity).
        /// </summary>
        public NumericalValueCondition Size
        {
            get
            {
                return Database.Current.Select<NumericalValueCondition>(this.ID, ValueTables.SpaceCondition, Columns.Size);
            }
            set
            {
                if (this.Size != null)
                    this.Size.Remove();

                Database.Current.Update(this.ID, ValueTables.SpaceCondition, Columns.Size, value);
                NotifyPropertyChanged("Size");
            }
        }
        #endregion Property: Size

        #region Property: IsFull
        /// <summary>
        /// Gets or sets the value that indicates whether the space has to be full.
        /// </summary>
        public bool? IsFull
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, ValueTables.SpaceCondition, Columns.IsFull);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceCondition, Columns.IsFull, value);
                NotifyPropertyChanged("Size");
            }
        }
        #endregion Property: IsFull

        #region Property: Items
        /// <summary>
        /// Gets all the required space items.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectCondition> Items
        {
            get
            {
                return Database.Current.SelectAll<TangibleObjectCondition>(this.ID, ValueTables.SpaceConditionItem, Columns.TangibleObjectCondition).AsReadOnly();
            }
        }
        #endregion Property: Items

        #region Property: TangibleMatter
        /// <summary>
        /// Gets all the required tangible matter.
        /// </summary>
        public ReadOnlyCollection<MatterCondition> TangibleMatter
        {
            get
            {
                return Database.Current.SelectAll<MatterCondition>(this.ID, ValueTables.SpaceConditionTangibleMatter, Columns.MatterCondition).AsReadOnly();
            }
        }
        #endregion Property: TangibleMatter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: SpaceCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static SpaceCondition()
        {
            // Items
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.TangibleObjectCondition, new Tuple<Type, EntryType>(typeof(TangibleObjectCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.SpaceConditionItem, typeof(SpaceCondition), dict);

            // Tangible matter
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MatterCondition, new Tuple<Type, EntryType>(typeof(MatterCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.SpaceConditionTangibleMatter, typeof(SpaceCondition), dict);
        }
        #endregion Static Constructor: SpaceCondition()

        #region Constructor: SpaceCondition()
        /// <summary>
        /// Creates a new space condition.
        /// </summary>
        public SpaceCondition()
            : base()
        {
        }
        #endregion Constructor: SpaceCondition()

        #region Constructor: SpaceCondition(uint id)
        /// <summary>
        /// Creates a new space condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a space condition from.</param>
        protected SpaceCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SpaceCondition(uint id)

        #region Constructor: SpaceCondition(SpaceCondition spaceCondition)
        /// <summary>
        /// Clones a space condition.
        /// </summary>
        /// <param name="spaceCondition">The space condition to clone.</param>
        public SpaceCondition(SpaceCondition spaceCondition)
            : base(spaceCondition)
        {
            if (spaceCondition != null)
            {
                Database.Current.StartChange();

                this.SpaceType = spaceCondition.SpaceType;
                this.SpaceTypeSign = spaceCondition.SpaceTypeSign;
                this.StorageType = spaceCondition.StorageType;
                this.StorageTypeSign = spaceCondition.StorageTypeSign;
                this.MaximumNumberOfItems = spaceCondition.MaximumNumberOfItems;
                this.MaximumNumberOfItemsSign = spaceCondition.MaximumNumberOfItemsSign;
                if (spaceCondition.Capacity != null)
                    this.Capacity = new NumericalValueCondition(spaceCondition.Capacity);
                this.NumberOfItems = spaceCondition.NumberOfItems;
                this.NumberOfItemsSign = spaceCondition.NumberOfItemsSign;
                if (spaceCondition.Size != null)
                    this.Size = new NumericalValueCondition(spaceCondition.Size);
                this.IsFull = spaceCondition.IsFull;
                foreach (TangibleObjectCondition item in spaceCondition.Items)
                    AddItem(new TangibleObjectCondition(item));
                foreach (MatterCondition tangibleMatter in spaceCondition.TangibleMatter)
                    AddTangibleMatter(tangibleMatter.Clone());

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: SpaceCondition(SpaceCondition spaceCondition)

        #region Constructor: SpaceCondition(Space space)
        /// <summary>
        /// Creates a condition for the given space.
        /// </summary>
        /// <param name="space">The space to create a condition for.</param>
        public SpaceCondition(Space space)
            : base(space)
        {
        }
        #endregion Constructor: SpaceCondition(Space space)

        #region Constructor: SpaceCondition(Space space, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given space in the given quantity.
        /// </summary>
        /// <param name="space">The space to create a condition for.</param>
        /// <param name="quantity">The quantity of the space condition.</param>
        public SpaceCondition(Space space, NumericalValueCondition quantity)
            : base(space, quantity)
        {
        }
        #endregion Constructor: SpaceCondition(Space space, NumericalValueCondition quantity)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddItem(TangibleObjectCondition item)
        /// <summary>
        /// Adds the given item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddItem(TangibleObjectCondition item)
        {
            if (item != null)
            {
                // Check whether we already have a similar item
                if (HasItem(item.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the item
                Database.Current.Insert(this.ID, ValueTables.SpaceConditionItem, Columns.TangibleObjectCondition, item);
                NotifyPropertyChanged("Items");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddItem(TangibleObjectCondition item)

        #region Method: RemoveItem(TangibleObjectCondition item)
        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveItem(TangibleObjectCondition item)
        {
            if (item != null)
            {
                if (this.Items.Contains(item))
                {
                    Database.Current.Remove(this.ID, ValueTables.SpaceConditionItem, Columns.TangibleObjectCondition, item);
                    NotifyPropertyChanged("Items");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveItem(TangibleObjectCondition item)

        #region Method: AddTangibleMatter(MatterCondition tangibleMatter)
        /// <summary>
        /// Adds the given tangible matter.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddTangibleMatter(MatterCondition tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                // Check whether we already have similar tangible matter
                if (HasTangibleMatter(tangibleMatter.Matter))
                    return Message.RelationExistsAlready;

                // Add the tangible matter
                Database.Current.Insert(this.ID, ValueTables.SpaceConditionTangibleMatter, Columns.MatterCondition, tangibleMatter);
                NotifyPropertyChanged("TangibleMatter");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddTangibleMatter(MatterCondition tangibleMatter)

        #region Method: RemoveTangibleMatter(MatterCondition tangibleMatter)
        /// <summary>
        /// Removes the given tangible matter.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveTangibleMatter(MatterCondition tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                if (this.TangibleMatter.Contains(tangibleMatter))
                {
                    Database.Current.Remove(this.ID, ValueTables.SpaceConditionTangibleMatter, Columns.MatterCondition, tangibleMatter);
                    NotifyPropertyChanged("TangibleMatter");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveTangibleMatter(MatterCondition item)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasItem(TangibleObject item)
        /// <summary>
        /// Checks whether the space condition has the given item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true when the space condition has the given item.</returns>
        public bool HasItem(TangibleObject item)
        {
            if (item != null)
            {
                foreach (TangibleObjectCondition tangibleObjectCondition in this.Items)
                {
                    if (item.Equals(tangibleObjectCondition.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasItem(TangibleObject item)

        #region Method: HasTangibleMatter(Matter tangibleMatter)
        /// <summary>
        /// Checks whether the space condition has the given tangible matter.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter to check.</param>
        /// <returns>Returns true when the space condition has the given tangible matter.</returns>
        public bool HasTangibleMatter(Matter tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                foreach (MatterCondition matterCondition in this.TangibleMatter)
                {
                    if (tangibleMatter.Equals(matterCondition.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasTangibleMatter(Matter tangibleMatter)

        #region Method: Remove()
        /// <summary>
        /// Remove the space condition.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the items
            foreach (TangibleObjectCondition item in this.Items)
                item.Remove();
            Database.Current.Remove(this.ID, ValueTables.SpaceConditionItem);

            // Remove the tangible matter
            foreach (MatterCondition tangibleMatter in this.TangibleMatter)
                tangibleMatter.Remove();
            Database.Current.Remove(this.ID, ValueTables.SpaceConditionTangibleMatter);

            // Remove the capacity
            if (this.Capacity != null)
                this.Capacity.Remove();

            // Remove the size
            if (this.Size != null)
                this.Size.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: SpaceCondition

    #region Class: SpaceChange
    /// <summary>
    /// A change on a space.
    /// </summary>
    public class SpaceChange : PhysicalObjectChange
    {

        #region Properties and Fields

        #region Property: Space
        /// <summary>
        /// Gets or sets the affected space.
        /// </summary>
        public Space Space
        {
            get
            {
                return this.Node as Space;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Space

        #region Property: SpaceType
        /// <summary>
        /// Gets or sets the space type to change to.
        /// </summary>
        public SpaceType? SpaceType
        {
            get
            {
                return Database.Current.Select<SpaceType?>(this.ID, ValueTables.SpaceChange, Columns.Type);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceChange, Columns.Type, value);
                NotifyPropertyChanged("SpaceType");
            }
        }
        #endregion Property: SpaceType

        #region Property: StorageType
        /// <summary>
        /// Gets or sets the storage type to change to.
        /// </summary>
        public StorageType? StorageType
        {
            get
            {
                return Database.Current.Select<StorageType?>(this.ID, ValueTables.SpaceChange, Columns.StorageType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceChange, Columns.StorageType, value);
                NotifyPropertyChanged("StorageType");
            }
        }
        #endregion Property: StorageType

        #region Property: MaximumNumberOfItems
        /// <summary>
        /// Gets or sets the maximum number of items to change to.
        /// </summary>
        public uint? MaximumNumberOfItems
        {
            get
            {
                return Database.Current.Select<uint?>(this.ID, ValueTables.SpaceChange, Columns.MaximumNumberOfItems);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceChange, Columns.MaximumNumberOfItems, value);
                NotifyPropertyChanged("MaximumNumberOfItems");
            }
        }
        #endregion Property: MaximumNumberOfItems

        #region Property: MaximumNumberOfItemsChangeType
        /// <summary>
        /// Gets or sets the type of change for the maximum number of items.
        /// </summary>
        public ValueChangeType? MaximumNumberOfItemsChangeType
        {
            get
            {
                return Database.Current.Select<ValueChangeType?>(this.ID, ValueTables.SpaceChange, Columns.MaximumNumberOfItemsChangeType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceChange, Columns.MaximumNumberOfItemsChangeType, value);
                NotifyPropertyChanged("MaximumNumberOfItemsChangeType");
            }
        }
        #endregion Property: MaximumNumberOfItemsChangeType

        #region Property: Capacity
        /// <summary>
        /// Gets or sets the capacity change.
        /// </summary>
        public NumericalValueChange Capacity
        {
            get
            {
                return Database.Current.Select<NumericalValueChange>(this.ID, ValueTables.SpaceChange, Columns.Capacity);
            }
            set
            {
                if (this.Capacity != null)
                    this.Capacity.Remove();

                Database.Current.Update(this.ID, ValueTables.SpaceChange, Columns.Capacity, value);
                NotifyPropertyChanged("Capacity");
            }
        }
        #endregion Property: Capacity

        #region Property: Items
        /// <summary>
        /// Gets the items that should be changed during the change.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectChange> Items
        {
            get
            {
                return Database.Current.SelectAll<TangibleObjectChange>(this.ID, ValueTables.SpaceChangeItem, Columns.TangibleObjectChange).AsReadOnly();
            }
        }
        #endregion Property: Items

        #region Property: ItemsToAdd
        /// <summary>
        /// Gets the items that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectValued> ItemsToAdd
        {
            get
            {
                return Database.Current.SelectAll<TangibleObjectValued>(this.ID, ValueTables.SpaceChangeItemToAdd, Columns.TangibleObjectValued).AsReadOnly();
            }
        }
        #endregion Property: ItemsToAdd

        #region Property: ItemsToRemove
        /// <summary>
        /// Gets the items that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectCondition> ItemsToRemove
        {
            get
            {
                return Database.Current.SelectAll<TangibleObjectCondition>(this.ID, ValueTables.SpaceChangeItemToRemove, Columns.TangibleObjectCondition).AsReadOnly();
            }
        }
        #endregion Property: ItemsToRemove

        #region Property: TangibleMatter
        /// <summary>
        /// Gets the tangible matter that should be changed during the change.
        /// </summary>
        public ReadOnlyCollection<MatterChange> TangibleMatter
        {
            get
            {
                return Database.Current.SelectAll<MatterChange>(this.ID, ValueTables.SpaceChangeTangibleMatter, Columns.MatterChange).AsReadOnly();
            }
        }
        #endregion Property: TangibleMatter

        #region Property: TangibleMatterToAdd
        /// <summary>
        /// Gets the tangible matter that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<MatterValued> TangibleMatterToAdd
        {
            get
            {
                return Database.Current.SelectAll<MatterValued>(this.ID, ValueTables.SpaceChangeTangibleMatterToAdd, Columns.MatterValued).AsReadOnly();
            }
        }
        #endregion Property: TangibleMatterToAdd

        #region Property: TangibleMatterToRemove
        /// <summary>
        /// Gets the tangible matter that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<MatterCondition> TangibleMatterToRemove
        {
            get
            {
                return Database.Current.SelectAll<MatterCondition>(this.ID, ValueTables.SpaceChangeTangibleMatterToRemove, Columns.MatterCondition).AsReadOnly();
            }
        }
        #endregion Property: TangibleMatterToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: SpaceChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static SpaceChange()
        {
            // Items
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.TangibleObjectChange, new Tuple<Type, EntryType>(typeof(TangibleObjectChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.SpaceChangeItem, typeof(SpaceChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.TangibleObjectValued, new Tuple<Type, EntryType>(typeof(TangibleObjectValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.SpaceChangeItemToAdd, typeof(SpaceChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.TangibleObjectCondition, new Tuple<Type, EntryType>(typeof(TangibleObjectCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.SpaceChangeItemToRemove, typeof(SpaceChange), dict);

            // Tangible matter
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MatterChange, new Tuple<Type, EntryType>(typeof(MatterChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.SpaceChangeTangibleMatter, typeof(SpaceChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MatterValued, new Tuple<Type, EntryType>(typeof(MatterValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.SpaceChangeTangibleMatterToAdd, typeof(SpaceChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MatterCondition, new Tuple<Type, EntryType>(typeof(MatterCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.SpaceChangeTangibleMatterToRemove, typeof(SpaceChange), dict);
        }
        #endregion Static Constructor: SpaceChange()

        #region Constructor: SpaceChange()
        /// <summary>
        /// Creates a new space change.
        /// </summary>
        public SpaceChange()
            : base()
        {
        }
        #endregion Constructor: SpaceChange()

        #region Constructor: SpaceChange(uint id)
        /// <summary>
        /// Creates a new space change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a space change from.</param>
        protected SpaceChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SpaceChange(uint id)

        #region Constructor: SpaceChange(SpaceChange spaceChange)
        /// <summary>
        /// Clones a space change.
        /// </summary>
        /// <param name="spaceChange">The space change to clone.</param>
        public SpaceChange(SpaceChange spaceChange)
            : base(spaceChange)
        {
            if (spaceChange != null)
            {
                Database.Current.StartChange();

                this.SpaceType = spaceChange.SpaceType;
                this.StorageType = spaceChange.StorageType;
                this.MaximumNumberOfItems = spaceChange.MaximumNumberOfItems;
                this.MaximumNumberOfItemsChangeType = spaceChange.MaximumNumberOfItemsChangeType;
                if (spaceChange.Capacity != null)
                    this.Capacity = new NumericalValueChange(spaceChange.Capacity);
                foreach (TangibleObjectChange item in spaceChange.Items)
                    AddItem(new TangibleObjectChange(item));
                foreach (TangibleObjectValued itemToAdd in spaceChange.ItemsToAdd)
                    AddItemToAdd(new TangibleObjectValued(itemToAdd));
                foreach (TangibleObjectCondition itemToRemove in spaceChange.ItemsToRemove)
                    AddItemToRemove(new TangibleObjectCondition(itemToRemove));

                foreach (MatterChange tangibleMatter in spaceChange.TangibleMatter)
                    AddTangibleMatter(tangibleMatter.Clone());
                foreach (MatterValued tangibleMatterToAdd in spaceChange.TangibleMatterToAdd)
                    AddTangibleMatterToAdd(tangibleMatterToAdd.Clone());
                foreach (MatterCondition tangibleMatterToRemove in spaceChange.TangibleMatterToRemove)
                    AddTangibleMatterToRemove(tangibleMatterToRemove.Clone());

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: SpaceChange(SpaceChange spaceChange)

        #region Constructor: SpaceChange(Space space)
        /// <summary>
        /// Creates a change for the given space.
        /// </summary>
        /// <param name="space">The space to create a change for.</param>
        public SpaceChange(Space space)
            : base(space)
        {
        }
        #endregion Constructor: SpaceChange(Space space)

        #region Constructor: SpaceChange(Space space, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given space in the form of the given quantity.
        /// </summary>
        /// <param name="space">The space to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        public SpaceChange(Space space, NumericalValueChange quantity)
            : base(space, quantity)
        {
        }
        #endregion Constructor: SpaceChange(Space space, NumericalValueChange quantity)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddItem(TangibleObjectChange item)
        /// <summary>
        /// Adds a item to the list of items to change.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddItem(TangibleObjectChange item)
        {
            if (item != null && item.TangibleObject != null)
            {
                // If the item is already available in all items, there is no use to add it
                if (HasItem(item.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the item
                Database.Current.Insert(this.ID, ValueTables.SpaceChangeItem, Columns.TangibleObjectChange, item);
                NotifyPropertyChanged("Items");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddItem(TangibleObjectChange item)

        #region Method: RemoveItem(TangibleObjectChange item)
        /// <summary>
        /// Removes a item from the list of items to change.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveItem(TangibleObjectChange item)
        {
            if (item != null)
            {
                if (this.Items.Contains(item))
                {
                    // Remove the item
                    Database.Current.Remove(this.ID, ValueTables.SpaceChangeItem, Columns.TangibleObjectChange, item);
                    NotifyPropertyChanged("Items");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveItem(TangibleObjectChange item)

        #region Method: AddItemToAdd(TangibleObjectValued item)
        /// <summary>
        /// Adds a item to the list of items to add.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddItemToAdd(TangibleObjectValued item)
        {
            if (item != null && item.TangibleObject != null)
            {
                // If the item is already available in all items, there is no use to add it
                if (HasItemToAdd(item.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the item
                Database.Current.Insert(this.ID, ValueTables.SpaceChangeItemToAdd, Columns.TangibleObjectValued, item);
                NotifyPropertyChanged("ItemsToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddItemToAdd(TangibleObjectValued item)

        #region Method: RemoveItemToAdd(TangibleObjectValued item)
        /// <summary>
        /// Removes a item from the list of items to add.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveItemToAdd(TangibleObjectValued item)
        {
            if (item != null)
            {
                if (this.ItemsToAdd.Contains(item))
                {
                    // Remove the item
                    Database.Current.Remove(this.ID, ValueTables.SpaceChangeItemToAdd, Columns.TangibleObjectValued, item);
                    NotifyPropertyChanged("ItemsToAdd");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveItemToAdd(TangibleObjectValued item)

        #region Method: AddItemToRemove(TangibleObjectCondition item)
        /// <summary>
        /// Adds a item to the list of items to remove.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddItemToRemove(TangibleObjectCondition item)
        {
            if (item != null)
            {
                // If the item is already available in all items, there is no use to add it
                if (HasItemToRemove(item.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the item
                Database.Current.Insert(this.ID, ValueTables.SpaceChangeItemToRemove, Columns.TangibleObjectCondition, item);
                NotifyPropertyChanged("ItemsToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddItemToRemove(TangibleObjectCondition item)

        #region Method: RemoveItemToRemove(TangibleObjectCondition item)
        /// <summary>
        /// Removes a item from the list of items to remove.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveItemToRemove(TangibleObjectCondition item)
        {
            if (item != null)
            {
                if (this.ItemsToRemove.Contains(item))
                {
                    // Remove the item
                    Database.Current.Remove(this.ID, ValueTables.SpaceChangeItemToRemove, Columns.TangibleObjectCondition, item);
                    NotifyPropertyChanged("ItemsToRemove");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveItemToRemove(TangibleObjectCondition item)

        #region Method: AddTangibleMatter(MatterChange tangibleMatter)
        /// <summary>
        /// Adds a tangible matter to the list of tangible matter to change.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddTangibleMatter(MatterChange tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                // If the tangible matter is already available in all tangible matter, there is no use to add it
                if (HasTangibleMatter(tangibleMatter.Matter))
                    return Message.RelationExistsAlready;

                // Add the tangible matter
                Database.Current.Insert(this.ID, ValueTables.SpaceChangeTangibleMatter, Columns.MatterChange, tangibleMatter);
                NotifyPropertyChanged("TangibleMatter");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddTangibleMatter(MatterChange tangibleMatter)

        #region Method: RemoveTangibleMatter(MatterChange tangibleMatter)
        /// <summary>
        /// Removes a tangible matter from the list of tangible matter to change.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveTangibleMatter(MatterChange tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                if (this.TangibleMatter.Contains(tangibleMatter))
                {
                    // Remove the tangible matter
                    Database.Current.Remove(this.ID, ValueTables.SpaceChangeTangibleMatter, Columns.MatterChange, tangibleMatter);
                    NotifyPropertyChanged("TangibleMatter");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveTangibleMatter(MatterChange tangibleMatter)

        #region Method: AddTangibleMatterToAdd(MatterValued tangibleMatter)
        /// <summary>
        /// Adds a tangible matter to the list of tangible matter to add.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddTangibleMatterToAdd(MatterValued tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                // If the tangible matter is already available in all tangible matter, there is no use to add it
                if (HasTangibleMatterToAdd(tangibleMatter.Matter))
                    return Message.RelationExistsAlready;

                // Add the tangible matter
                Database.Current.Insert(this.ID, ValueTables.SpaceChangeTangibleMatterToAdd, Columns.MatterValued, tangibleMatter);
                NotifyPropertyChanged("TangibleMatterToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddTangibleMatterToAdd(MatterValued tangibleMatter)

        #region Method: RemoveTangibleMatterToAdd(MatterValued tangibleMatter)
        /// <summary>
        /// Removes a tangible matter from the list of tangible matter to add.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveTangibleMatterToAdd(MatterValued tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                if (this.TangibleMatterToAdd.Contains(tangibleMatter))
                {
                    // Remove the tangible matter
                    Database.Current.Remove(this.ID, ValueTables.SpaceChangeTangibleMatterToAdd, Columns.MatterValued, tangibleMatter);
                    NotifyPropertyChanged("TangibleMatterToAdd");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveTangibleMatterToAdd(MatterValued tangibleMatter)

        #region Method: AddTangibleMatterToRemove(MatterCondition tangibleMatter)
        /// <summary>
        /// Adds a tangible matter to the list of tangible matter to remove.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddTangibleMatterToRemove(MatterCondition tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                // If the tangible matter is already available in all tangible matter, there is no use to add it
                if (HasTangibleMatterToRemove(tangibleMatter.Matter))
                    return Message.RelationExistsAlready;

                // Add the tangible matter
                Database.Current.Insert(this.ID, ValueTables.SpaceChangeTangibleMatterToRemove, Columns.MatterCondition, tangibleMatter);
                NotifyPropertyChanged("TangibleMatterToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddTangibleMatterToRemove(MatterCondition tangibleMatter)

        #region Method: RemoveTangibleMatterToRemove(MatterCondition tangibleMatter)
        /// <summary>
        /// Removes a tangible matter from the list of tangible matter to remove.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveTangibleMatterToRemove(MatterCondition tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                if (this.TangibleMatterToRemove.Contains(tangibleMatter))
                {
                    // Remove the tangible matter
                    Database.Current.Remove(this.ID, ValueTables.SpaceChangeTangibleMatterToRemove, Columns.MatterCondition, tangibleMatter);
                    NotifyPropertyChanged("TangibleMatterToRemove");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveTangibleMatterToRemove(MatterCondition tangibleMatter)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasItem(TangibleObject item)
        /// <summary>
        /// Checks whether the space change has the given item to change.
        /// </summary>
        /// <param name="item">The item that should be checked.</param>
        /// <returns>Returns true when the space change has the given item to change.</returns>
        public bool HasItem(TangibleObject item)
        {
            if (item != null)
            {
                foreach (TangibleObjectChange myItem in this.Items)
                {
                    if (item.Equals(myItem.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasItem(TangibleObject item)

        #region Method: HasItemToAdd(TangibleObject item)
        /// <summary>
        /// Checks whether the space change has the given item to add.
        /// </summary>
        /// <param name="item">The item that should be checked.</param>
        /// <returns>Returns true when the space change has the given item to add.</returns>
        public bool HasItemToAdd(TangibleObject item)
        {
            if (item != null)
            {
                foreach (TangibleObjectValued myItem in this.ItemsToAdd)
                {
                    if (item.Equals(myItem.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasItemToAdd(TangibleObject item)

        #region Method: HasItemToRemove(TangibleObject item)
        /// <summary>
        /// Checks whether the space change has the given item to remove.
        /// </summary>
        /// <param name="item">The item that should be checked.</param>
        /// <returns>Returns true when the space change has the given item to remove.</returns>
        public bool HasItemToRemove(TangibleObject item)
        {
            if (item != null)
            {
                foreach (TangibleObjectCondition myItem in this.ItemsToRemove)
                {
                    if (item.Equals(myItem.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasItemToRemove(TangibleObject item)

        #region Method: HasTangibleMatter(Matter tangibleMatter)
        /// <summary>
        /// Checks whether the space change has the given tangible matter to change.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter that should be checked.</param>
        /// <returns>Returns true when the space change has the given tangible matter to change.</returns>
        public bool HasTangibleMatter(Matter tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                foreach (MatterChange matterChange in this.TangibleMatter)
                {
                    if (tangibleMatter.Equals(matterChange.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasTangibleMatter(Matter tangibleMatter)

        #region Method: HasTangibleMatterToAdd(Matter tangibleMatter)
        /// <summary>
        /// Checks whether the space change has the given tangible matter to add.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter that should be checked.</param>
        /// <returns>Returns true when the space change has the given tangible matter to add.</returns>
        public bool HasTangibleMatterToAdd(Matter tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                foreach (MatterValued matterValued in this.TangibleMatterToAdd)
                {
                    if (tangibleMatter.Equals(matterValued.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasTangibleMatterToAdd(Matter tangibleMatter)

        #region Method: HasTangibleMatterToRemove(Matter tangibleMatter)
        /// <summary>
        /// Checks whether the space change has the given tangible matter to remove.
        /// </summary>
        /// <param name="tangibleMatter">The tangible matter that should be checked.</param>
        /// <returns>Returns true when the space change has the given tangible matter to remove.</returns>
        public bool HasTangibleMatterToRemove(Matter tangibleMatter)
        {
            if (tangibleMatter != null)
            {
                foreach (MatterCondition matterCondition in this.TangibleMatterToRemove)
                {
                    if (tangibleMatter.Equals(matterCondition.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasTangibleMatterToRemove(TangibleObject tangibleMatter)

        #region Method: Remove()
        /// <summary>
        /// Remove the space change.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the items
            foreach (TangibleObjectChange tangibleObjectChange in this.Items)
                tangibleObjectChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.SpaceChangeItem);

            foreach (TangibleObjectValued tangibleObjectValued in this.ItemsToAdd)
                tangibleObjectValued.Remove();
            Database.Current.Remove(this.ID, ValueTables.SpaceChangeItemToAdd);

            foreach (TangibleObjectCondition tangibleObjectCondition in this.ItemsToRemove)
                tangibleObjectCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.SpaceChangeItemToRemove);

            // Remove the tangible matter
            foreach (MatterChange matterChange in this.TangibleMatter)
                matterChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.SpaceChangeTangibleMatter);

            foreach (MatterValued matterValued in this.TangibleMatterToAdd)
                matterValued.Remove();
            Database.Current.Remove(this.ID, ValueTables.SpaceChangeTangibleMatterToAdd);

            foreach (MatterCondition matterCondition in this.TangibleMatterToRemove)
                matterCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.SpaceChangeTangibleMatterToRemove);

            // Remove the capacity
            if (this.Capacity != null)
                this.Capacity.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: SpaceChange

}