/**************************************************************************
 * 
 * Effect.cs
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
using System.Reflection;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: Effect
    /// <summary>
    /// An effect.
    /// </summary>
    public abstract class Effect : IdHolder
    {

        #region Properties and Fields

        #region Property: Priority
        /// <summary>
        /// Gets or sets the priority of the effect; effects with the highest priority will be handled first.
        /// </summary>
        public int Priority
        {
            get
            {
                return Database.Current.Select<int>(this.ID, ValueTables.Effect, Columns.Priority);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Effect, Columns.Priority, value);
                NotifyPropertyChanged("Priority");
            }
        }
        #endregion Property: Priority

        #region Property: Chance
        /// <summary>
        /// Gets the chance that this effect takes place.
        /// </summary>
        public Chance Chance
        {
            get
            {
                return Database.Current.Select<Chance>(this.ID, ValueTables.Effect, Columns.Chance);
            }
            private set
            {
                Database.Current.Update(this.ID, ValueTables.Effect, Columns.Chance, value);
                NotifyPropertyChanged("Chance");
            }
        }
        #endregion Property: Chance

        #region Property: OverrideTime
        /// <summary>
        /// Gets the value that indicates whether the time of the event should be overridden by the time of this effect.
        /// </summary>
        public bool OverrideTime
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Effect, Columns.OverrideTime);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Effect, Columns.OverrideTime, value);

                if (value)
                {
                    if (this.Time == null)
                        this.Time = new Time();
                }
                else
                    this.Time = null;

                NotifyPropertyChanged("OverrideTime");
            }
        }
        #endregion Property: OverrideTime

        #region Property: Time
        /// <summary>
        /// Gets or sets the time of the effect.
        /// </summary>
        public Time Time
        {
            get
            {
                return Database.Current.Select<Time>(this.ID, ValueTables.Effect, Columns.Time);
            }
            set
            {
                if (value == null && this.Time != null)
                    this.Time.Remove();

                Database.Current.Update(this.ID, ValueTables.Effect, Columns.Time, value);
                NotifyPropertyChanged("Time");
            }
        }
        #endregion Property: Time

        #region Property: Range
        /// <summary>
        /// Gets the range of the effect.
        /// </summary>
        public Range Range
        {
            get
            {
                return Database.Current.Select<Range>(this.ID, ValueTables.Effect, Columns.Range);
            }
            private set
            {
                Database.Current.Update(this.ID, ValueTables.Effect, Columns.Range, value);
                NotifyPropertyChanged("Range");
            }
        }
        #endregion Property: Range

        #region Property: StartTrigger
        /// <summary>
        /// Gets or sets the value that indicates when the effect should be triggered: when the event starts or stops.
        /// </summary>
        public EventState StartTrigger
        {
            get
            {
                return Database.Current.Select<EventState>(this.ID, ValueTables.Effect, Columns.StartTrigger);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Effect, Columns.StartTrigger, value);
                NotifyPropertyChanged("StartTrigger");
            }
        }
        #endregion Property: StartTrigger

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Effect()
        /// <summary>
        /// Creates a new effect.
        /// </summary>
        protected Effect()
            : base()
        {
            Database.Current.StartChange();

            this.Chance = new Chance();
            this.Range = new Range();

            Database.Current.StopChange();
        }
        #endregion Constructor: Effect()

        #region Constructor: Effect(uint id)
        /// <summary>
        /// Creates a new effect from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a effect from.</param>
        protected Effect(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Effect(uint id)

        #region Constructor: Effect(Effect effect)
        /// <summary>
        /// Clones a effect.
        /// </summary>
        /// <param name="effect">The effect to clone.</param>
        protected Effect(Effect effect)
            : base()
        {
            if (effect != null)
            {
                Database.Current.StartChange();

                this.Priority = effect.Priority;
                if (effect.Chance != null)
                    this.Chance = new Chance(effect.Chance);
                this.OverrideTime = effect.OverrideTime;
                if (effect.Time != null)
                    this.Time = new Time(effect.Time);
                if (effect.Range != null)
                    this.Range = new Range(effect.Range);
                this.StartTrigger = effect.StartTrigger;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Effect(Effect effect)
		
        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the effect.
        /// </summary>
        /// <returns>A clone of the effect.</returns>
        public Effect Clone()
        {
            try
            {
                Type type = this.GetType();
                return type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type }, null).Invoke(new object[] { this }) as Effect;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Method: Clone()

        #region Method: Remove()
        /// <summary>
        /// Remove the effect.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the chance
            if (this.Chance != null)
                this.Chance.Remove();

            // Remove the time
            if (this.Time != null)
                this.Time.Remove();

            // Remove the range
            if (this.Range != null)
                this.Range.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: Effect

}