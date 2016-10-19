/**************************************************************************
 * 
 * Transfer.cs
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
using Common;
using Semantics.Data;
using Semantics.Entities;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: Transfer
    /// <summary>
    /// An effect that transfers items from the actor to a target, or vice versa.
    /// </summary>
    public sealed class Transfer : Effect
    {

        #region Properties and Fields

        #region Property: TransferType
        /// <summary>
        /// Gets or sets the type of the transfer, which decides whether items should be transferred, or the actor or target. 
        /// </summary>
        public TransferType TransferType
        {
            get
            {
                return Database.Current.Select<TransferType>(this.ID, ValueTables.Transfer, Columns.Type);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Transfer, Columns.Type, value);
                NotifyPropertyChanged("TransferType");
            }
        }
        #endregion Property: TransferType

        #region Property: Direction
        /// <summary>
        /// Gets or sets the direction of the items to transfer: actor to target, target to actor, actor to actor, or target to target(from one space to another).
        /// Only valid when Items has been chosen as TransferType!
        /// </summary>
        public TransferDirection Direction
        {
            get
            {
                return Database.Current.Select<TransferDirection>(this.ID, ValueTables.Transfer, Columns.Direction);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Transfer, Columns.Direction, value);
                NotifyPropertyChanged("Direction");
            }
        }
        #endregion Property: Direction

        #region Property: Items
        /// <summary>
        /// Gets the items to transfer. Only valid when Items has been chosen as TransferType!
        /// </summary>
        public ReadOnlyCollection<TangibleObjectValued> Items
        {
            get
            {
                return Database.Current.SelectAll<TangibleObjectValued>(this.ID, ValueTables.TransferItem, Columns.TangibleObjectValued).AsReadOnly();
            }
        }
        #endregion Property: Items

        #region Property: SourceSpace
        /// <summary>
        /// Gets or sets the (optional) source space of the items. If no space is selected, any space will do. Only valid when Items has been chosen as TransferType!
        /// </summary>
        public Space SourceSpace
        {
            get
            {
                return Database.Current.Select<Space>(this.ID, ValueTables.Transfer, Columns.SourceSpace);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Transfer, Columns.SourceSpace, value);
                NotifyPropertyChanged("SourceSpace");
            }
        }
        #endregion Property: SourceSpace

        #region Property: DestinationSpace
        /// <summary>
        /// Gets or sets the (optional) destination space of the items, actor, or target. If no space is selected, any space will do.
        /// </summary>
        public Space DestinationSpace
        {
            get
            {
                return Database.Current.Select<Space>(this.ID, ValueTables.Transfer, Columns.DestinationSpace);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Transfer, Columns.DestinationSpace, value);
                NotifyPropertyChanged("DestinationSpace");
            }
        }
        #endregion Property: DestinationSpace

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Transfer()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Transfer()
        {
            // Items
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.TangibleObjectValued, new Tuple<Type, EntryType>(typeof(TangibleObjectValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TransferItem, typeof(Transfer), dict);

            // Item source and destination
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SourceSpace, new Tuple<Type, EntryType>(typeof(Space), EntryType.Nullable));
            dict.Add(Columns.DestinationSpace, new Tuple<Type, EntryType>(typeof(Space), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.Transfer, typeof(Transfer), dict);
        }
        #endregion Static Constructor: Transfer()

        #region Constructor: Transfer()
        /// <summary>
        /// Creates a new transfer.
        /// </summary>
        public Transfer()
            : base()
        {
        }
        #endregion Constructor: Transfer()

        #region Constructor: Transfer(TransferType type)
        /// <summary>
        /// Creates a new transfer of the given type.
        /// </summary>
        /// <param name="type">The type of the transfer.</param>
        public Transfer(TransferType type)
            : base()
        {
            Database.Current.StartChange();

            this.TransferType = type;

            Database.Current.StopChange();
        }
        #endregion Constructor: Transfer(TransferType type)

        #region Constructor: Transfer(TransferDirection direction)
        /// <summary>
        /// Creates a new transfer in the given direction.
        /// </summary>
        /// <param name="direction">The direction of the transfer.</param>
        public Transfer(TransferDirection direction)
            : base()
        {
            Database.Current.StartChange();

            this.TransferType = TransferType.Items;
            this.Direction = direction;

            Database.Current.StopChange();
        }
        #endregion Constructor: Transfer(TransferDirection direction)

        #region Constructor: Transfer(uint id)
        /// <summary>
        /// Creates a new transfer from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a transfer from.</param>
        private Transfer(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Transfer(uint id)

        #region Constructor: Transfer(Transfer transfer)
        /// <summary>
        /// Clones a transfer.
        /// </summary>
        /// <param name="transfer">The transfer to clone.</param>
        public Transfer(Transfer transfer)
            : base(transfer)
        {
            if (transfer != null)
            {
                Database.Current.StartChange();

                this.TransferType = transfer.TransferType;
                this.Direction = transfer.Direction;
                this.SourceSpace = transfer.SourceSpace;
                this.DestinationSpace = transfer.DestinationSpace;

                foreach (TangibleObjectValued item in transfer.Items)
                    AddItem(new TangibleObjectValued(item));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Transfer(Transfer transfer)

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
                // If the item is already available in all items, there is no use to add it
                if (HasItem(item.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the item
                Database.Current.Insert(this.ID, ValueTables.TransferItem, new string[] { Columns.TangibleObject, Columns.TangibleObjectValued }, new object[] { item.TangibleObject, item });
                NotifyPropertyChanged("Items");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddItem(TangibleObjectValued item)

        #region Method: RemoveItem(TangibleObjectValued item)
        /// <summary>
        /// Removes an item.
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
                    Database.Current.Remove(this.ID, ValueTables.TransferItem, Columns.TangibleObjectValued, item);
                    NotifyPropertyChanged("Items");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveItem(TangibleObjectValued item)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasItem(TangibleObject tangibleObject)
        /// <summary>
        /// Checks if this transfer has an item of the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to check.</param>
        /// <returns>Returns true when this transfer has an item of the tangible object.</returns>
        public bool HasItem(TangibleObject tangibleObject)
        {
            if (tangibleObject != null)
            {
                foreach (TangibleObjectValued item in this.Items)
                {
                    if (tangibleObject.Equals(item.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasItem(TangibleObject tangibleObject)

        #region Method: Remove()
        /// <summary>
        /// Remove the event.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the items
            foreach (TangibleObjectValued item in this.Items)
                item.Remove();
            Database.Current.Remove(this.ID, ValueTables.TransferItem);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: Transfer

}