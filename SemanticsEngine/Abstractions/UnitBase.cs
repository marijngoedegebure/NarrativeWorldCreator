/**************************************************************************
 * 
 * UnitBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using Semantics.Abstractions;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Abstractions
{

    #region Class: UnitBase
    /// <summary>
    /// A base of unit.
    /// </summary>
    public class UnitBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: Unit
        /// <summary>
        /// Gets the unit of which this is a unit base.
        /// </summary>
        protected internal Unit Unit
        {
            get
            {
                return this.IdHolder as Unit;
            }
        }
        #endregion Property: Unit

        #region Property: Symbol
        /// <summary>
        /// The symbol.
        /// </summary>
        private String symbol = SemanticsSettings.Values.Symbol;

        /// <summary>
        /// Gets the symbol.
        /// </summary>
        public String Symbol
        {
            get
            {
                return symbol;
            }
        }
        #endregion Property: Symbol

        #region Property: UnitCategory
        /// <summary>
        /// The category of this unit.
        /// </summary>
        private UnitCategoryBase unitCategory = null;
        
        /// <summary>
        /// Gets the category of this unit.
        /// </summary>
        public UnitCategoryBase UnitCategory
        {
            get
            {
                return unitCategory;
            }
        }
        #endregion Property: UnitCategory

        #region Property: ConversionEquation
        /// <summary>
        /// The equation to convert this unit to the base unit.
        /// </summary>
        private EquationBase conversionEquation = null;
        
        /// <summary>
        /// Gets the equation to convert this unit to the base unit.
        /// </summary>
        public EquationBase ConversionEquation
        {
            get
            {
                return conversionEquation;
            }
        }
        #endregion Property: ConversionEquation

        #region Property: ConversionBackEquation
        /// <summary>
        /// The equation to convert the base unit to this unit.
        /// </summary>
        private EquationBase conversionBackEquation = null;

        /// <summary>
        /// Gets the equation to convert the base unit to this unit.
        /// </summary>
        public EquationBase ConversionBackEquation
        {
            get
            {
                return conversionBackEquation;
            }
        }
        #endregion Property: ConversionBackEquation

        #region Property: IsBaseUnit
        /// <summary>
        /// Gets the value that indicates whether this unit is a base unit.
        /// </summary>
        public bool IsBaseUnit
        {
            get
            {
                if (this.UnitCategory != null)
                    return this.Equals(this.UnitCategory.BaseUnit);

                return false;
            }
        }
        #endregion Property: IsBaseUnit

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: UnitBase(Unit unit)
        /// <summary>
        /// Create a unit base from the given unit.
        /// </summary>
        /// <param name="unit">The unit to create a unit base from.</param>
        protected internal UnitBase(Unit unit)
            : base(unit)
        {
            if (unit != null)
            {
                this.symbol = unit.Symbol;
                this.unitCategory = BaseManager.Current.GetBase<UnitCategoryBase>(unit.UnitCategory);
                this.conversionEquation = BaseManager.Current.GetBase<EquationBase>(unit.ConversionEquation);
                this.conversionBackEquation = BaseManager.Current.GetBase<EquationBase>(unit.ConversionBackEquation);
            }
        }
        #endregion Constructor: UnitBase(Unit unit)

        #endregion Method Group: Constructors

    }
    #endregion Class: UnitBase

}