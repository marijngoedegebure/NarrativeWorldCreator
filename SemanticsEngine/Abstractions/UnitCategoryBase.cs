/**************************************************************************
 * 
 * UnitCategoryBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Semantics.Abstractions;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Abstractions
{

    #region Class: UnitCategoryBase
    /// <summary>
    /// A base of unit category.
    /// </summary>
    public class UnitCategoryBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: UnitCategory
        /// <summary>
        /// Gets the unit category of which this is a unit category base.
        /// </summary>
        protected internal UnitCategory UnitCategory
        {
            get
            {
                return this.IdHolder as UnitCategory;
            }
        }
        #endregion Property: UnitCategory

        #region Property: BaseUnit
        /// <summary>
        /// The base unit of this category.
        /// </summary>
        private UnitBase baseUnit = null;
        
        /// <summary>
        /// Gets the base unit of this category.
        /// </summary>
        public UnitBase BaseUnit
        {
            get
            {
                return baseUnit;
            }
        }
        #endregion Property: BaseUnit

        #region Property: Units
        /// <summary>
        /// Gets the units in this category.
        /// </summary>
        private UnitBase[] units = null;
        
        /// <summary>
        /// Gets the units in this category.
        /// </summary>
        public ReadOnlyCollection<UnitBase> Units
        {
            get
            {
                if (units == null)
                {
                    if (this.UnitCategory == null)
                        return new List<UnitBase>(0).AsReadOnly();

                    List<UnitBase> unitBases = new List<UnitBase>();
                    foreach (Unit unit in this.UnitCategory.Units)
                        unitBases.Add(BaseManager.Current.GetBase<UnitBase>(unit));
                    units = unitBases.ToArray();
                }
                return new ReadOnlyCollection<UnitBase>(units);
            }
        }
        #endregion Property: Units

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: UnitCategoryBase(UnitCategory unitCategory)
        /// <summary>
        /// Create a unit category base from the given unit category.
        /// </summary>
        /// <param name="unitCategory">The unit category to create a unit category base from.</param>
        protected internal UnitCategoryBase(UnitCategory unitCategory)
            : base(unitCategory)
        {
            if (unitCategory != null)
                this.baseUnit = BaseManager.Current.GetBase<UnitBase>(unitCategory.BaseUnit);
        }
        #endregion Constructor: UnitCategoryBase(UnitCategory unitCategory)

        #endregion Method Group: Constructors

    }
    #endregion Class: UnitCategoryBase

}