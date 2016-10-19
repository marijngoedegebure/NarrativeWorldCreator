/**************************************************************************
 * 
 * SpatialRequirement.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Abstractions;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: SpatialRequirement
    /// <summary>
    /// A spatial requirement states to be within, or be away from a certain distance.
    /// </summary>
    public sealed class SpatialRequirement : NumericalValueRange
    {

        #region Method Group: Constructors

        #region Constructor: SpatialRequirement()
        /// <summary>
        /// Creates a new spatial requirement.
        /// </summary>
        public SpatialRequirement()
            : this(SemanticsSettings.Values.Distance, EqualitySignExtendedDual.LowerOrEqual)
        {
        }
        #endregion Constructor: SpatialRequirement()

        #region Constructor: SpatialRequirement(uint id)
        /// <summary>
        /// Creates a new spatial requirement from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a spatial requirement from.</param>
        internal SpatialRequirement(uint id)
            : base(id)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: SpatialRequirement(uint id)

        #region Constructor: SpatialRequirement(SpatialRequirement spatialRequirement)
        /// <summary>
        /// Clones a spatial requirement.
        /// </summary>
        /// <param name="spatialRequirement">The spatial requirement to clone.</param>
        public SpatialRequirement(SpatialRequirement spatialRequirement)
            : base(spatialRequirement)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: SpatialRequirement(SpatialRequirement spatialRequirement)

        #region Constructor: SpatialRequirement(float distance)
        /// <summary>
        /// Creates a new spatial requirement with the given distance.
        /// </summary>
        /// <param name="distance">The maximum distance.</param>
        public SpatialRequirement(float distance)
            : this(distance, Prefix.None, null, EqualitySignExtendedDual.LowerOrEqual)
        {
        }
        #endregion Constructor: SpatialRequirement(float distance)

        #region Constructor: SpatialRequirement(float distance, Unit unit)
        /// <summary>
        /// Creates a new spatial requirement with the given distance and unit.
        /// </summary>
        /// <param name="distance">The maximum distance.</param>
        /// <param name="unit">The unit for the distance.</param>
        public SpatialRequirement(float distance, Unit unit)
            : this(distance, Prefix.None, unit, EqualitySignExtendedDual.LowerOrEqual)
        {
        }
        #endregion Constructor: SpatialRequirement(float distance, Unit unit)

        #region Constructor: SpatialRequirement(float distance, Prefix prefix, Unit unit)
        /// <summary>
        /// Creates a new spatial requirement with the given distance, prefix, and unit.
        /// </summary>
        /// <param name="distance">The maximum distance.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="unit">The unit for the distance.</param>
        public SpatialRequirement(float distance, Prefix prefix, Unit unit)
            : this(distance, prefix, unit, EqualitySignExtendedDual.LowerOrEqual)
        {
        }
        #endregion Constructor: SpatialRequirement(float distance, Prefix prefix, Unit unit)

        #region Constructor: SpatialRequirement(float distance, EqualitySignExtendedDual distanceSign)
        /// <summary>
        /// Creates a new spatial requirement with the given distance and sign.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <param name="distanceSign">The sign of the distance.</param>
        public SpatialRequirement(float distance, EqualitySignExtendedDual distanceSign)
            : this(distance, Prefix.None, null, distanceSign)
        {
        }
        #endregion Constructor: SpatialRequirement(float distance, EqualitySignExtendedDual distanceSign)

        #region Constructor: SpatialRequirement(float distance, Unit unit, EqualitySignExtendedDual distanceSign)
        /// <summary>
        /// Creates a new spatial requirement with the given distance, unit, and sign.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <param name="unit">The unit for the distance.</param>
        /// <param name="distanceSign">The sign of the distance.</param>
        public SpatialRequirement(float distance, Unit unit, EqualitySignExtendedDual distanceSign)
            : this(distance, Prefix.None, unit, distanceSign)
        {
        }
        #endregion Constructor: SpatialRequirement(float distance, Unit unit, EqualitySignExtendedDual distanceSign)

        #region Constructor: SpatialRequirement(float distance, Prefix prefix, Unit unit, EqualitySignExtendedDual distanceSign)
        /// <summary>
        /// Creates a new spatial requirement with the given distance, prefix, unit, and sign.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="unit">The unit for the distance.</param>
        /// <param name="distanceSign">The sign of the distance.</param>
        public SpatialRequirement(float distance, Prefix prefix, Unit unit, EqualitySignExtendedDual distanceSign)
            : base(distance, prefix, unit, distanceSign)
        {
            Database.Current.StartChange();

            // Set the unit category to the special distance unit category
            this.UnitCategory = SemanticsManager.GetSpecialUnitCategory(SpecialUnitCategories.Distance);
            
            // Subscribe to changes of the special distance unit category
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);

            Database.Current.StopChange();
        }
        #endregion Constructor: SpatialRequirement(float distance, Prefix prefix, Unit unit, EqualitySignExtendedDual distanceSign)

        #region Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        /// <summary>
        /// Change the special distance unit category.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <param name="unitCategory">The unit category.</param>
        private void SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        {
            if (specialUnitCategory == SpecialUnitCategories.Distance)
                this.UnitCategory = unitCategory;
        }
        #endregion Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpatialRequirement

}