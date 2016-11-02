/**************************************************************************
 * 
 * TransferBase.cs
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
using Semantics.Components;
using Semantics.Entities;
using Semantics.Utilities;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: TransferBase
    /// <summary>
    /// A base of a transfer.
    /// </summary>
    public sealed class TransferBase : EffectBase
    {

        #region Properties and Fields

        #region Property: Transfer
        /// <summary>
        /// Gets the transfer this is a base of.
        /// </summary>
        internal Transfer Transfer
        {
            get
            {
                return this.IdHolder as Transfer;
            }
        }
        #endregion Property: Transfer

        #region Property: TransferType
        /// <summary>
        /// The type of the transfer, which decides whether items should be transferred, or the actor or target. 
        /// </summary>
        private TransferType transferType = default(TransferType);

        /// <summary>
        /// Gets the type of the transfer, which decides whether items should be transferred, or the actor or target. 
        /// </summary>
        public TransferType TransferType
        {
            get
            {
                return transferType;
            }
        }
        #endregion Property: TransferType

        #region Property: Direction
        /// <summary>
        /// The direction of the items to transfer: actor to target, target to actor, actor to actor, or target to target(from one space to another).
        /// Only valid when Items has been chosen as TransferType!
        /// </summary>
        private TransferDirection direction = default(TransferDirection);
        
        /// <summary>
        /// Gets the direction of the items to transfer: actor to target, target to actor, actor to actor, or target to target(from one space to another).
        /// Only valid when Items has been chosen as TransferType!
        /// </summary>
        public TransferDirection Direction
        {
            get
            {
                return direction;
            }
        }
        #endregion Property: Direction

        #region Property: Items
        /// <summary>
        /// The items of the transfer. Only valid when Items has been chosen as TransferType!
        /// </summary>
        private TangibleObjectValuedBase[] items = null;

        /// <summary>
        /// Gets the items of the transfer. Only valid when Items has been chosen as TransferType!
        /// </summary>
        public ReadOnlyCollection<TangibleObjectValuedBase> Items
        {
            get
            {
                if (items == null)
                {
                    LoadItems();
                    if (items == null)
                        return new List<TangibleObjectValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<TangibleObjectValuedBase>(items);
            }
        }

        /// <summary>
        /// Loads the items.
        /// </summary>
        private void LoadItems()
        {
            if (this.Transfer != null)
            {
                List<TangibleObjectValuedBase> tangibleObjectValuedBases = new List<TangibleObjectValuedBase>();
                foreach (TangibleObjectValued item in this.Transfer.Items)
                    tangibleObjectValuedBases.Add(BaseManager.Current.GetBase<TangibleObjectValuedBase>(item));
                items = tangibleObjectValuedBases.ToArray();
            }
        }
        #endregion Property: Items

        #region Property: SourceSpace
        /// <summary>
        /// The (optional) source space of the items. If no space is selected, any space will do. Only valid when Items has been chosen as TransferType!
        /// </summary>
        private SpaceBase sourceSpace = null;

        /// <summary>
        /// Gets the (optional) source space of the items. If no space is selected, any space will do. Only valid when Items has been chosen as TransferType!
        /// </summary>
        public SpaceBase SourceSpace
        {
            get
            {
                return sourceSpace;
            }
        }
        #endregion Property: SourceSpace

        #region Property: DestinationSpace
        /// <summary>
        /// The (optional) destination space of the items, actor, or target. If no space is selected, any space will do.
        /// </summary>
        private SpaceBase destinationSpace = null;
        
        /// <summary>
        /// Gets the (optional) destination space of the items, actor, or target. If no space is selected, any space will do.
        /// </summary>
        public SpaceBase DestinationSpace
        {
            get
            {
                return destinationSpace;
            }
        }
        #endregion Property: DestinationSpace

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TransferBase(Transfer transfer)
        /// <summary>
        /// Creates a base of the given transfer.
        /// </summary>
        /// <param name="transfer">The transfer to create a base of.</param>
        internal TransferBase(Transfer transfer)
            : base(transfer)
        {
            if (transfer != null)
            {
                this.transferType = transfer.TransferType;
                this.direction = transfer.Direction;
                this.sourceSpace = BaseManager.Current.GetBase<SpaceBase>(transfer.SourceSpace);
                this.destinationSpace = BaseManager.Current.GetBase<SpaceBase>(transfer.DestinationSpace);

                if (BaseManager.PreloadProperties)
                    LoadItems();
            }
        }
        #endregion Constructor: TransferBase(Transfer transfer)

        #endregion Method Group: Constructors

    }
    #endregion Class: TransferBase

}