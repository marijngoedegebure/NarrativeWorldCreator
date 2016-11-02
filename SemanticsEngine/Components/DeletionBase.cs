/**************************************************************************
 * 
 * DeletionBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: DeletionBase
    /// <summary>
    /// A base of a deletion.
    /// </summary>
    public sealed class DeletionBase : EffectBase
    {

        #region Properties and Fields

        #region Property: Deletion
        /// <summary>
        /// Gets the deletion of which this is a deletion base.
        /// </summary>
        internal Deletion Deletion
        {
            get
            {
                return this.Effect as Deletion;
            }
        }
        #endregion Property: Deletion

        #region Property: DeletionType
        /// <summary>
        /// The type of the deletion.
        /// </summary>
        private DeletionType deletionType = default(DeletionType);

        /// <summary>
        /// Gets the type of the deletion.
        /// </summary>
        public DeletionType DeletionType
        {
            get
            {
                return deletionType;
            }
        }
        #endregion Property: DeletionType

        #region Property: Quantity
        /// <summary>
        /// The quantity to delete.
        /// </summary>
        private NumericalValueBase quantity = null;

        /// <summary>
        /// Gets the quantity to delete.
        /// </summary>
        public NumericalValueBase Quantity
        {
            get
            {
                if (quantity == null)
                {
                    LoadQuantity();
                    if (quantity == null)
                        quantity = new NumericalValueBase(SemanticsSettings.Values.Quantity);
                }
                return quantity;
            }
        }

        /// <summary>
        /// Loads the quantity to delete.
        /// </summary>
        private void LoadQuantity()
        {
            if (this.Deletion != null)
                quantity = BaseManager.Current.GetBase<NumericalValueBase>(this.Deletion.Quantity);
        }
        #endregion Property: Quantity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: DeletionBase(Deletion deletion)
        /// <summary>
        /// Creates a base of the given deletion.
        /// </summary>
        /// <param name="deletion">The deletion to create a base of.</param>
        internal DeletionBase(Deletion deletion)
            : base(deletion)
        {
            if (deletion != null)
            {
                this.deletionType = deletion.DeletionType;

                if (BaseManager.PreloadProperties)
                    LoadQuantity();
            }
        }
        #endregion Constructor: DeletionBase(Deletion deletion)

        #endregion Method Group: Constructors

    }
    #endregion Class: DeletionBase

}