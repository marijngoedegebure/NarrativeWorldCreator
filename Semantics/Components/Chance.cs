/**************************************************************************
 * 
 * Chance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: Chance
    /// <summary>
    /// A chance with a value.
    /// </summary>
    public sealed class Chance : IdHolder
    {

        #region Properties and Fields

        #region Property: Value
        /// <summary>
        /// Gets or sets the value of the chance.
        /// </summary>
        public float Value
        {
            get
            {
                return Database.Current.Select<float>(this.ID, ValueTables.Chance, Columns.Value);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Chance, Columns.Value, value);
                NotifyPropertyChanged("Value");
            }
        }
        #endregion Property: Value

        #region Property: Variable
        /// <summary>
        /// Gets or sets the variable that represents the chance value instead. Only valid for numerical, random, and term variables!
        /// </summary>
        public Variable Variable
        {
            get
            {
                return Database.Current.Select<Variable>(this.ID, ValueTables.Chance, Columns.Variable);
            }
            set
            {
                if (value == null || value is NumericalVariable || value is RandomVariable || value is TermVariable)
                {
                    Database.Current.Update(this.ID, ValueTables.Chance, Columns.Variable, value);
                    NotifyPropertyChanged("Variable");
                }
            }
        }
        #endregion Property: Variable

        #region Property: IsRandom
        /// <summary>
        /// Gets or sets the value that indicates whether the chance should be random.
        /// </summary>
        public bool IsRandom
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Chance, Columns.IsRandom);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Chance, Columns.IsRandom, value);
                NotifyPropertyChanged("IsRandom");
            }
        }
        #endregion Property: IsRandom

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Chance()
        /// <summary>
        /// Creates a new chance.
        /// </summary>
        public Chance()
            : base()
        {
            Database.Current.StartChange();

            this.Value = SemanticsSettings.Values.Chance;

            Database.Current.StopChange();
        }
        #endregion Constructor: Chance() 

        #region Constructor: Chance(uint id)
        /// <summary>
        /// Creates a new chance from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a chance from.</param>
        internal Chance(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Chance(uint id)

        #region Constructor: Chance(Chance chance)
        /// <summary>
        /// Clones the given chance.
        /// </summary>
        /// <param name="chance">The chance to clone.</param>
        public Chance(Chance chance)
            : base()
        {
            if (chance != null)
            {
                Database.Current.StartChange();

                this.Value = chance.Value;
                this.Variable = chance.Variable;
                this.IsRandom = chance.IsRandom;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Chance(Chance chance)

        #region Constructor: Chance(float val)
        /// <summary>
        /// Creates a new chance with the given value.
        /// </summary>
        /// <param name="val">The chance value.</param>
        public Chance(float val)
            : base()
        {
            Database.Current.StartChange();

            this.Value = val;

            Database.Current.StopChange();
        }
        #endregion Constructor: Chance(float val)

        #endregion Method Group: Constructors

    }
    #endregion Class: Chance

}
