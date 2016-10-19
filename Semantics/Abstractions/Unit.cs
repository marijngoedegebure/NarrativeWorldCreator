/**************************************************************************
 * 
 * Unit.cs
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
using Common;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Abstractions
{

    #region Class: Unit
    /// <summary>
    /// The unit component is aware of its category, and has the equations to convert to and from the base unit of its category.
    /// </summary>
    public class Unit : Abstraction, IComparable<Unit>
    {

        #region Properties and Fields

        #region Property: Symbol
        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        public String Symbol
        {
            get
            {
                return Database.Current.Select<String>(this.ID, GenericTables.Unit, Columns.Symbol);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Unit, Columns.Symbol, value);
                NotifyPropertyChanged("Symbol");
            }
        }
        #endregion Property: Symbol

        #region Property: UnitCategory
        /// <summary>
        /// Gets or sets the category of this unit.
        /// </summary>
        public UnitCategory UnitCategory
        {
            get
            {
                return Database.Current.Select<UnitCategory>(this.ID, GenericTables.Unit, Columns.UnitCategory);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Unit, Columns.UnitCategory, value);
                NotifyPropertyChanged("UnitCategory");

                // Set this unit to be the base unit if none has been selected before
                if (value != null)
                {
                    if (value.BaseUnit == null)
                        value.BaseUnit = this;
                }
            }
        }
        #endregion Property: UnitCategory
        
        #region Property: ConversionEquation
        /// <summary>
        /// Gets or sets the equation to convert this unit to the base unit.
        /// </summary>
        public Equation ConversionEquation
        {
            get
            {
                return Database.Current.Select<Equation>(this.ID, GenericTables.Unit, Columns.ConversionEquation);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Unit, Columns.ConversionEquation, value);
                NotifyPropertyChanged("ConversionEquation");
            }
        }
        #endregion Property: ConversionEquation

        #region Property: ConversionBackEquation
        /// <summary>
        /// Gets the equation to convert the base unit to this unit.
        /// </summary>
        public Equation ConversionBackEquation
        {
            get
            {
                return Database.Current.Select<Equation>(this.ID, GenericTables.Unit, Columns.ConversionBackEquation);
            }
            internal set
            {
                Database.Current.Update(this.ID, GenericTables.Unit, Columns.ConversionBackEquation, value);
                NotifyPropertyChanged("ConversionBackEquation");
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

        #region Static Constructor: Unit()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Unit()
        {
            // Unit category
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.UnitCategory, new Tuple<Type, EntryType>(typeof(UnitCategory), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.Unit, typeof(Unit), dict);
        }
        #endregion Static Constructor: Unit()

        #region Constructor: Unit()
        /// <summary>
        /// Creates a new unit.
        /// </summary>
        public Unit()
            : base()
        {
            Database.Current.StartChange();
            Database.Current.QueryBegin();
            
            this.ConversionEquation = new Equation(SemanticsSettings.General.EquationVariable);
            this.ConversionBackEquation = new Equation(SemanticsSettings.General.EquationResultVariable);

            Database.Current.QueryCommit();
            Database.Current.StopChange();
        }
        #endregion Constructor: Unit()

        #region Constructor: Unit(uint id)
        /// <summary>
        /// Creates a new unit from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a unit from.</param>
        protected Unit(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Unit(uint id)

        #region Constructor: Unit(string name)
        /// <summary>
        /// Creates a new unit with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the unit.</param>
        public Unit(string name)
            : base(name)
        {
            Database.Current.StartChange();
            Database.Current.QueryBegin();

            this.ConversionEquation = new Equation(SemanticsSettings.General.EquationVariable);
            this.ConversionBackEquation = new Equation(SemanticsSettings.General.EquationResultVariable);

            Database.Current.QueryCommit();
            Database.Current.StopChange();
        }
        #endregion Constructor: Unit(string name)

        #region Constructor: Unit(Unit unit)
        /// <summary>
        /// Clones a unit.
        /// </summary>
        /// <param name="unit">The unit to clone.</param>
        public Unit(Unit unit)
            : base(unit)
        {
            if (unit != null)
            {
                Database.Current.StartChange();

                this.Symbol = unit.Symbol;
                this.UnitCategory = unit.UnitCategory;
                this.ConversionEquation = new Equation(unit.ConversionEquation);
                this.ConversionBackEquation = new Equation(unit.ConversionBackEquation);

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Unit(Unit unit)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the unit.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Unit other)
        /// <summary>
        /// Compares the unit to the other unit.
        /// </summary>
        /// <param name="other">The unit to compare to this unit.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Unit other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Unit other)

        #endregion Method Group: Other

    }
    #endregion Class: Unit

}
