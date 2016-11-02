/**************************************************************************
 * 
 * TimeBase.cs
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
using SemanticsEngine.Abstractions;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: TimeBase
    /// <summary>
    /// A time with a delay, duration, and interval.
    /// </summary>  
    public sealed class TimeBase : Base
    {

        #region Properties and Fields

        #region Property: Time
        /// <summary>
        /// Gets the time this is a base of.
        /// </summary>
        internal Time Time
        {
            get
            {
                return this.IdHolder as Time;
            }
        }
        #endregion Property: Time

        #region Property: TimeType
        /// <summary>
        /// The time type.
        /// </summary>
        private TimeType timeType = default(TimeType);

        /// <summary>
        /// Gets the time type.
        /// </summary>
        public TimeType TimeType
        {
            get
            {
                return timeType;
            }
        }
        #endregion Property: TimeType

        #region Property: Delay
        /// <summary>
        /// The delay.
        /// </summary>
        private NumericalValueBase delay = null;

        /// <summary>
        /// Gets the delay.
        /// </summary>
        public NumericalValueBase Delay
        {
            get
            {
                if (delay == null)
                {
                    LoadDelay();
                    if (delay == null)
                        delay = new NumericalValueBase(SemanticsSettings.Values.Delay);
                    if (delay.Unit == null)
                    {
                        UnitCategoryBase timeUnitCategoryBase = Utils.GetSpecialUnitCategory(SpecialUnitCategories.Time);
                        if (timeUnitCategoryBase != null)
                            delay.Unit = timeUnitCategoryBase.BaseUnit;
                    }
                }
                return delay;
            }
        }

        /// <summary>
        /// Loads the delay.
        /// </summary>
        private void LoadDelay()
        {
            if (this.Time != null)
                delay = BaseManager.Current.GetBase<NumericalValueBase>(this.Time.Delay);
        }
        #endregion Property: Delay

        #region Property: Duration
        /// <summary>
        /// The duration. Only valid for a continuous time.
        /// </summary>
        private NumericalValueBase duration = null;

        /// <summary>
        /// Gets the duration. Only valid for a continuous time.
        /// </summary>
        public NumericalValueBase Duration
        {
            get
            {
                if (duration == null)
                {
                    LoadDuration();
                    if (duration == null)
                        duration = new NumericalValueBase(SemanticsSettings.Values.Duration);
                    if (duration.Unit == null)
                    {
                        UnitCategoryBase timeUnitCategoryBase = Utils.GetSpecialUnitCategory(SpecialUnitCategories.Time);
                        if (timeUnitCategoryBase != null)
                            duration.Unit = timeUnitCategoryBase.BaseUnit;
                    }
                }
                return duration;
            }
        }

        /// <summary>
        /// Loads the duration.
        /// </summary>
        private void LoadDuration()
        {
            if (this.Time != null)
                duration = BaseManager.Current.GetBase<NumericalValueBase>(this.Time.Duration);
        }
        #endregion Property: Duration

        #region Property: DurationType
        /// <summary>
        /// The duration type. Only valid for a continuous time.
        /// </summary>
        private DurationType durationType = default(DurationType);

        /// <summary>
        /// Gets the duration type. Only valid for a continuous time.
        /// </summary>
        public DurationType DurationType
        {
            get
            {
                return durationType;
            }
        }
        #endregion Property: DurationType

        #region Property: Interval
        /// <summary>
        /// The interval.
        /// </summary>
        private NumericalValueBase interval = null;

        /// <summary>
        /// Gets the interval.
        /// </summary>
        public NumericalValueBase Interval
        {
            get
            {
                if (interval == null)
                {
                    LoadInterval();
                    if (interval == null)
                        interval = new NumericalValueBase(SemanticsSettings.Values.Interval);
                    if (interval.Unit == null)
                    {
                        UnitCategoryBase timeUnitCategoryBase = Utils.GetSpecialUnitCategory(SpecialUnitCategories.Time);
                        if (timeUnitCategoryBase != null)
                            interval.Unit = timeUnitCategoryBase.BaseUnit;
                    }
                }
                return interval;
            }
        }

        /// <summary>
        /// Loads the interval.
        /// </summary>
        private void LoadInterval()
        {
            if (this.Time != null)
                interval = BaseManager.Current.GetBase<NumericalValueBase>(this.Time.Interval);
        }
        #endregion Property: Interval

        #region Property: Frequency
        /// <summary>
        /// The frequency, in case of a discrete time type. Only valid for a discrete time.
        /// </summary>
        private int frequency = SemanticsSettings.Values.Frequency;

        /// <summary>
        /// Gets the frequency, in case of a discrete time type. Only valid for a discrete time.
        /// </summary>
        public int Frequency
        {
            get
            {
                return frequency;
            }
        }
        #endregion Property: Frequency

        #region Property: FrequencyType
        /// <summary>
        /// The frequency type, in case of a discrete time type. Only valid for a discrete time.
        /// </summary>
        private FrequencyType frequencyType = default(FrequencyType);

        /// <summary>
        /// Gets the frequency type, in case of a discrete time type. Only valid for a discrete time.
        /// </summary>
        public FrequencyType FrequencyType
        {
            get
            {
                return frequencyType;
            }
        }
        #endregion Property: FrequencyType

        #region Property: FrequencyVariable
        /// <summary>
        /// The variable that represents the frequency instead. Only valid for a discrete time.
        /// </summary>
        private NumericalVariableBase frequencyVariable = null;

        /// <summary>
        /// Gets the variable that represents the frequency instead. Only valid for a discrete time.
        /// </summary>
        public NumericalVariableBase FrequencyVariable
        {
            get
            {
                return frequencyVariable;
            }
        }
        #endregion Property: FrequencyVariable

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TimeBase(Time time)
        /// <summary>
        /// Creates a new time base from the given time.
        /// </summary>
        /// <param name="time">The time to create a base from.</param>
        internal TimeBase(Time time)
            : base(time)
        {
            if (time != null)
            {
                this.timeType = time.TimeType;
                this.durationType = time.DurationType;
                this.frequency = time.Frequency;
                this.frequencyType = time.FrequencyType;
                this.frequencyVariable = BaseManager.Current.GetBase<NumericalVariableBase>(time.FrequencyVariable);

                if (BaseManager.PreloadProperties)
                {
                    LoadDelay();
                    LoadDuration();
                    LoadInterval();
                }
            }
        }
        #endregion Constructor: TimeBase(Time time)

        #endregion Method Group: Constructors

    }
    #endregion Class: TimeBase

}
