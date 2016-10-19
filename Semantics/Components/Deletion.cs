/**************************************************************************
 * 
 * Deletion.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Abstractions;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: Deletion
    /// <summary>
    /// A deletion effect.
    /// </summary>
    public sealed class Deletion : Effect
    {

        #region Properties and Fields

        #region Property: DeletionType
        /// <summary>
        /// Gets or sets the type of the deletion.
        /// </summary>
        public DeletionType DeletionType
        {
            get
            {
                return Database.Current.Select<DeletionType>(this.ID, ValueTables.Deletion, Columns.DeletionType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Deletion, Columns.DeletionType, value);
                NotifyPropertyChanged("DeletionType");
            }
        }
        #endregion Property: DeletionType

        #region Property: Quantity
        /// <summary>
        /// Gets or sets the quantity to delete.
        /// </summary>
        public NumericalValue Quantity
        {
            get
            {
                return Database.Current.Select<NumericalValue>(this.ID, ValueTables.Deletion, Columns.Quantity);
            }
            private set
            {
                Database.Current.Update(this.ID, ValueTables.Deletion, Columns.Quantity, value);
                NotifyPropertyChanged("Quantity");
            }
        }
        #endregion Property: Quantity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Deletion()
        /// <summary>
        /// Creates a new deletion.
        /// </summary>
        public Deletion()
            : this(default(DeletionType), new NumericalValue(SemanticsSettings.Values.Quantity))
        {
        }
        #endregion Constructor: Deletion()

        #region Constructor: Deletion(uint id)
        /// <summary>
        /// Creates a new deletion from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a deletion from.</param>
        private Deletion(uint id)
            : base(id)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: Deletion(uint id)

        #region Constructor: Deletion(Deletion deletion)
        /// <summary>
        /// Clones a deletion.
        /// </summary>
        /// <param name="deletion">The deletion to clone.</param>
        public Deletion(Deletion deletion)
            : base(deletion)
        {
            if (deletion != null)
            {
                Database.Current.StartChange();

                this.DeletionType = deletion.DeletionType;
                if (deletion.Quantity != null)
                    this.Quantity = new NumericalValue(this.Quantity);

                Database.Current.StopChange();
            }

            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: Deletion(Deletion deletion)

        #region Constructor: Deletion(DeletionType deletionType)
        /// <summary>
        /// Creates a deletion for the given type.
        /// </summary>
        /// <param name="deletionType">The deletion type.</param>
        public Deletion(DeletionType deletionType)
            : this(deletionType, new NumericalValue(SemanticsSettings.Values.Quantity))
        {
        }
        #endregion Constructor: Deletion(DeletionType deletionType)

        #region Constructor: Deletion(DeletionType deletionType, NumericalValue quantity)
        /// <summary>
        /// Creates a deletion for the given type and with the given quantity.
        /// </summary>
        /// <param name="deletionType">The deletion type.</param>
        /// <param name="quantity">The quantity to remove.</param>
        public Deletion(DeletionType deletionType, NumericalValue quantity)
            : base()
        {
            Database.Current.StartChange();

            this.DeletionType = deletionType;

            // Get the special quantity unit category and subscribe to its changes
            UnitCategory quantityUnitCategory = SemanticsManager.GetSpecialUnitCategory(SpecialUnitCategories.Quantity);
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);

            if (quantity != null)
            {
                quantity.UnitCategory = quantityUnitCategory;
                this.Quantity = quantity;
            }
            else
                this.Quantity = new NumericalValue(SemanticsSettings.Values.Quantity, quantityUnitCategory);

            Database.Current.StopChange();
        }
        #endregion Constructor: Deletion(DeletionType deletionType, NumericalValue quantity)

        #region Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        /// <summary>
        /// Change the special quantity unit category.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <param name="unitCategory">The unit category.</param>
        private void SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        {
            if (specialUnitCategory == SpecialUnitCategories.Quantity && this.Quantity != null)
                this.Quantity.UnitCategory = unitCategory;
        }
        #endregion Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)

        #endregion Method Group: Constructors

    }
    #endregion Class: Deletion

}