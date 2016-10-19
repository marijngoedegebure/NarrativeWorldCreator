/**************************************************************************
 * 
 * UnitCategory.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Abstractions
{

    #region Class: UnitCategory
    /// <summary>
    /// A unit category can have multiple units and a base unit.
    /// </summary>
    public class UnitCategory : Abstraction, IComparable<UnitCategory>
    {

        #region Properties and Fields

        #region Property: BaseUnit
        /// <summary>
        /// Gets or sets the base unit of this category.
        /// </summary>
        public Unit BaseUnit
        {
            get
            {
                Unit baseUnit = Database.Current.Select<Unit>(this.ID, GenericTables.UnitCategory, Columns.BaseUnit);

                // Try to set the base unit if this has not been done before
                if (baseUnit == null)
                {
                    ReadOnlyCollection<Unit> units = this.Units;
                    if (units.Count > 0)
                    {
                        skipEquationReset = true;
                        this.BaseUnit = units[0];
                        skipEquationReset = false;
                        baseUnit = units[0];
                    }
                }

                return baseUnit;
            }
            set
            {
                // Reset the conversion equations of the current base unit
                if (!skipEquationReset)
                {
                    Unit baseUnit = this.BaseUnit;
                    if (baseUnit != null)
                    {
                        if (baseUnit.ConversionEquation != null)
                            baseUnit.ConversionEquation.EquationString = SemanticsSettings.General.EquationVariable;
                        if (baseUnit.ConversionBackEquation != null)
                            baseUnit.ConversionBackEquation.EquationString = SemanticsSettings.General.EquationResultVariable;
                    }
                }

                Database.Current.Update(this.ID, GenericTables.UnitCategory, Columns.BaseUnit, value);
                NotifyPropertyChanged("BaseUnit");
            }
        }

        /// <summary>
        /// Indicates whether resetting of conversion equations should be skipped
        /// </summary>
        private bool skipEquationReset = false;
        #endregion Property: BaseUnit

        #region Property: Units
        /// <summary>
        /// Gets the units in this category.
        /// </summary>
        public ReadOnlyCollection<Unit> Units
        {
            get
            {
                return Database.Current.SelectAll<Unit>(GenericTables.Unit, Columns.UnitCategory, this.ID).AsReadOnly();
            }
        }
        #endregion Property: Units

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: UnitCategory()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static UnitCategory()
        {
            // Base unit
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.BaseUnit, new Tuple<Type, EntryType>(typeof(Unit), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.UnitCategory, typeof(UnitCategory), dict);
        }
        #endregion Static Constructor: UnitCategory()

        #region Constructor: UnitCategory()
        /// <summary>
        /// Creates a new unit category.
        /// </summary>
        public UnitCategory()
            : base()
        {
        }
        #endregion Constructor: UnitCategory()

        #region Constructor: UnitCategory(uint id)
        /// <summary>
        /// Creates a new unit category from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a unit category from.</param>
        protected UnitCategory(uint id)
            : base(id)
        {
        }
        #endregion Constructor: UnitCategory(uint id)

        #region Constructor: UnitCategory(string name)
        /// <summary>
        /// Creates a new unit category with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the unit category.</param>
        public UnitCategory(string name)
            : base(name)
        {
        }
        #endregion Constructor: UnitCategory(string name)

        #region Constructor: UnitCategory(UnitCategory unitCategory)
        /// <summary>
        /// Clones a unit category.
        /// </summary>
        /// <param name="unitCategory">The unit category to clone.</param>
        public UnitCategory(UnitCategory unitCategory)
            : base(unitCategory)
        {
            if (unitCategory != null)
            {
                Database.Current.StartChange();

                this.BaseUnit = unitCategory.BaseUnit;
                foreach (Unit unit in unitCategory.Units)
                    AddUnit(unit);

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: UnitCategory(UnitCategory unitCategory)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddUnit(Unit unit)
        /// <summary>
        /// Add a unit.
        /// </summary>
        /// <param name="unit">The unit to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddUnit(Unit unit)
        {
            if (unit != null)
            {
                // If the unit category of this unit is already this category, there is no reason to continue
                if (HasUnit(unit))
                    return Message.RelationExistsAlready;

                // Make this the unit category of the unit
                unit.UnitCategory = this;
                NotifyPropertyChanged("Units");

                // Set the base unit if none has been selected before
                if (this.BaseUnit == null)
                    this.BaseUnit = unit;

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddUnit(Unit unit)

        #region Method: RemoveUnit(Unit unit)
        /// <summary>
        /// Removes the given unit.
        /// </summary>
        /// <param name="unit">The unit to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveUnit(Unit unit)
        {
            if (unit != null)
            {
                // Reset the unit category of the unit
                unit.UnitCategory = null;
                NotifyPropertyChanged("Units");
                
                // Reset the base unit if it was the unit
                if (unit.Equals(this.BaseUnit))
                    this.BaseUnit = null;
                    
                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveUnit(Unit unit)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasUnit(Unit unit)
        /// <summary>
        /// Checks if this unit category has the given unit.
        /// </summary>
        /// <param name="unit">The unit to check.</param>
        /// <returns>Returns true when this unit category has the unit.</returns>
        public bool HasUnit(Unit unit)
        {
            if (unit != null)
                return this.Equals(unit.UnitCategory);
            return false;
        }
        #endregion Method: HasUnit(Unit unit)

        #region Method: Remove()
        /// <summary>
        /// Remove the unit category.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(UnitCategory other)
        /// <summary>
        /// Compares the unit category to the other unit category.
        /// </summary>
        /// <param name="other">The unit category to compare to this unit category.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(UnitCategory other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(UnitCategory other)

        #endregion Method Group: Other

    }
    #endregion Class: UnitCategory

}
