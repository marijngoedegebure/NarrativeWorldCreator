/**************************************************************************
 * 
 * Time.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using Common;
using Semantics.Abstractions;
using Semantics.Data;
using Semantics.Utilities;
using Attribute = Semantics.Abstractions.Attribute;

namespace Semantics.Components
{

    #region Class: Time
    /// <summary>
    /// A time with a delay, duration, and interval.
    /// </summary>  
    public sealed class Time : IdHolder
    {

        #region Properties and Fields

        #region Property: TimeType
        /// <summary>
        /// Gets or sets the time type.
        /// </summary>
        public TimeType TimeType
        {
            get
            {
                return Database.Current.Select<TimeType>(this.ID, ValueTables.Time, Columns.Type);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Time, Columns.Type, value);
                NotifyPropertyChanged("TimeType");
            }
        }
        #endregion Property: TimeType

        #region Property: Delay
        /// <summary>
        /// Gets the delay.
        /// </summary>
        public NumericalValue Delay
        {
            get
            {
                return Database.Current.Select<NumericalValue>(this.ID, ValueTables.Time, Columns.Delay);
            }
            private set
            {
                Database.Current.Update(this.ID, ValueTables.Time, Columns.Delay, value);
                NotifyPropertyChanged("Delay");
            }
        }
        #endregion Property: Delay

        #region Property: Duration
        /// <summary>
        /// Gets the duration. Only valid for a continuous time.
        /// </summary>
        public NumericalValue Duration
        {
            get
            {
                return Database.Current.Select<NumericalValue>(this.ID, ValueTables.Time, Columns.Duration);
            }
            private set
            {
                Database.Current.Update(this.ID, ValueTables.Time, Columns.Duration, value);
                NotifyPropertyChanged("Duration");
            }
        }
        #endregion Property: Duration

        #region Property: DurationType
        /// <summary>
        /// Gets or sets the duration type. Only valid for a continuous time.
        /// </summary>
        public DurationType DurationType
        {
            get
            {
                return Database.Current.Select<DurationType>(this.ID, ValueTables.Time, Columns.DurationType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Time, Columns.DurationType, value);
                NotifyPropertyChanged("DurationType");
            }
        }
        #endregion Property: DurationType

        #region Property: Interval
        /// <summary>
        /// Gets the interval.
        /// </summary>
        public NumericalValue Interval
        {
            get
            {
                return Database.Current.Select<NumericalValue>(this.ID, ValueTables.Time, Columns.Interval);
            }
            private set
            {
                Database.Current.Update(this.ID, ValueTables.Time, Columns.Interval, value);
                NotifyPropertyChanged("Interval");
            }
        }
        #endregion Property: Interval

        #region Property: Frequency
        /// <summary>
        /// Gets or sets the frequency, in case of a discrete time type. Only valid for a discrete time.
        /// </summary>
        public int Frequency
        {
            get
            {
                return Database.Current.Select<int>(this.ID, ValueTables.Time, Columns.Frequency);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Time, Columns.Frequency, value);
                NotifyPropertyChanged("Frequency");
            }
        }
        #endregion Property: Frequency

        #region Property: FrequencyType
        /// <summary>
        /// Gets or sets the frequency type, in case of a discrete time type. Only valid for a discrete time.
        /// </summary>
        public FrequencyType FrequencyType
        {
            get
            {
                return Database.Current.Select<FrequencyType>(this.ID, ValueTables.Time, Columns.FrequencyType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Time, Columns.FrequencyType, value);
                NotifyPropertyChanged("FrequencyType");
            }
        }
        #endregion Property: FrequencyType

        #region Property: FrequencyVariable
        /// <summary>
        /// Gets or sets the variable that represents the frequency instead. Only valid for a discrete time.
        /// </summary>
        public NumericalVariable FrequencyVariable
        {
            get
            {
                return Database.Current.Select<NumericalVariable>(this.ID, ValueTables.Time, Columns.FrequencyVariable);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Time, Columns.FrequencyVariable, value);
                NotifyPropertyChanged("FrequencyVariable");
            }
        }
        #endregion Property: FrequencyVariable

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Time()
        /// <summary>
        /// Creates a new time.
        /// </summary>
        public Time()
            : base()
        {
            Database.Current.StartChange();

            // Get the special time unit category and subscribe to its changes
            UnitCategory timeUnitCategory = SemanticsManager.GetSpecialUnitCategory(SpecialUnitCategories.Time);
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);

            this.TimeType = default(TimeType);
            this.Delay = new NumericalValue(SemanticsSettings.Values.Delay, timeUnitCategory, 0, float.MaxValue);
            this.Duration = new NumericalValue(SemanticsSettings.Values.Duration, timeUnitCategory, 0, float.MaxValue);
            this.Interval = new NumericalValue(SemanticsSettings.Values.Interval, timeUnitCategory, 0, float.MaxValue);
            this.Frequency = SemanticsSettings.Values.Frequency;

            Database.Current.StopChange();
        }
        #endregion Constructor: Time()

        #region Constructor: Time(uint id)
        /// <summary>
        /// Creates a new time from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a time from.</param>
        private Time(uint id)
            : base(id)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: Time(uint id)

        #region Constructor: Time(Time time)
        /// <summary>
        /// Clones a time.
        /// </summary>
        /// <param name="time">The time to clone.</param>
        public Time(Time time)
            : base()
        {
            if (time != null)
            {
                Database.Current.StartChange();

                this.TimeType = time.TimeType;
                if (time.Delay != null)
                    this.Delay = new NumericalValue(time.Delay);
                if (time.Duration != null)
                    this.Duration = new NumericalValue(time.Duration);
                this.DurationType = time.DurationType;
                if (time.Interval != null)
                    this.Interval = new NumericalValue(time.Interval);
                this.Frequency = time.Frequency;
                this.FrequencyType = time.FrequencyType;
                this.FrequencyVariable = time.FrequencyVariable;

                Database.Current.StopChange();
            }

            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: Time(Time time)

        #region Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        /// <summary>
        /// Change the special time unit category.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <param name="unitCategory">The unit category.</param>
        private void SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        {
            if (specialUnitCategory == SpecialUnitCategories.Time)
            {
                if (this.Delay != null)
                    this.Delay.UnitCategory = unitCategory;
                if (this.Duration != null)
                    this.Duration.UnitCategory = unitCategory;
                if (this.Interval != null)
                    this.Interval.UnitCategory = unitCategory;
            }
        }
        #endregion Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the time.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the delay
            if (this.Delay != null)
                this.Delay.Remove();

            // Remove the duration
            if (this.Duration != null)
                this.Duration.Remove();

            // Remove the interval
            if (this.Interval != null)
                this.Interval.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: Time

}
