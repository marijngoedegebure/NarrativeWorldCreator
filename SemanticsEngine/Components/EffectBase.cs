/**************************************************************************
 * 
 * EffectBase.cs
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
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: EffectBase
    /// <summary>
    /// A base of an effect.
    /// </summary>
    public abstract class EffectBase : Base
    {

        #region Properties and Fields

        #region Property: Effect
        /// <summary>
        /// The effect of which this is an effect base.
        /// </summary>
        private Effect effect = null;

        /// <summary>
        /// Gets the effect of which this is an effect base.
        /// </summary>
        protected internal Effect Effect
        {
            get
            {
                return effect;
            }
        }
        #endregion Property: Effect

        #region Property: Priority
        /// <summary>
        /// The priority of the effect; effects with the highest priority will be handled first.
        /// </summary>
        private int priority = 0;

        /// <summary>
        /// Gets the priority of the effect; effects with the highest priority will be handled first.
        /// </summary>
        public int Priority
        {
            get
            {
                return priority;
            }
        }
        #endregion Property: Priority

        #region Property: Chance
        /// <summary>
        /// The chance that this effect takes place.
        /// </summary>
        private ChanceBase chance = null;

        /// <summary>
        /// Gets the chance that this effect takes place.
        /// </summary>
        public ChanceBase Chance
        {
            get
            {
                if (chance == null)
                    LoadChance();
                return chance;
            }
        }

        /// <summary>
        /// Loads the chance.
        /// </summary>
        private void LoadChance()
        {
            if (this.Effect != null)
                chance = BaseManager.Current.GetBase<ChanceBase>(this.Effect.Chance);
        }
        #endregion Property: Chance

        #region Property: OverrideTime
        /// <summary>
        /// Indicates whether the time of the event should be overridden by the time of this effect.
        /// </summary>
        private bool overrideTime = false;

        /// <summary>
        /// Gets the value that indicates whether the time of the event should be overridden by the time of this effect.
        /// </summary>
        public bool OverrideTime
        {
            get
            {
                return overrideTime;
            }
        }
        #endregion Property: OverrideTime

        #region Property: Time
        /// <summary>
        /// The time of the effect.
        /// </summary>
        private TimeBase time = null;

        /// <summary>
        /// Gets the time of the effect.
        /// </summary>
        public TimeBase Time
        {
            get
            {
                if (time == null)
                    LoadTime();
                return time;
            }
        }

        /// <summary>
        /// Loads the time.
        /// </summary>
        private void LoadTime()
        {
            if (this.Effect != null)
                time = BaseManager.Current.GetBase<TimeBase>(this.Effect.Time);
        }
        #endregion Property: Time

        #region Property: Range
        /// <summary>
        /// The range of the effect.
        /// </summary>
        private RangeBase range = null;

        /// <summary>
        /// Gets the range of the effect.
        /// </summary>
        public RangeBase Range
        {
            get
            {
                if (range == null)
                    LoadRange();
                return range;
            }
        }

        /// <summary>
        /// Loads the range.
        /// </summary>
        private void LoadRange()
        {
            if (this.Effect != null)
                range = BaseManager.Current.GetBase<RangeBase>(this.Effect.Range);
        }
        #endregion Property: Range

        #region Property: StartTrigger
        /// <summary>
        /// The value that indicates when the effect should be triggered: when the event starts or stops.
        /// </summary>
        private EventState startTrigger = default(EventState);
        
        /// <summary>
        /// Gets the value that indicates when the effect should be triggered: when the event starts or stops.
        /// </summary>
        public EventState StartTrigger
        {
            get
            {
                return startTrigger;
            }
        }
        #endregion Property: StartTrigger

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EffectBase(Effect effect)
        /// <summary>
        /// Creates a new effect base from the given effect.
        /// </summary>
        /// <param name="effect">The effect to create a base from.</param>
        protected EffectBase(Effect effect)
            : base(effect)
        {
            if (effect != null)
            {
                this.effect = effect;

                this.priority = effect.Priority;
                this.overrideTime = effect.OverrideTime;
                this.startTrigger = effect.StartTrigger;

                if (BaseManager.PreloadProperties)
                {
                    LoadChance();
                    LoadTime();
                    LoadRange();
                }
            }
        }
        #endregion Constructor: EffectBase(Effect effect)

        #endregion Method Group: Constructors

    }
    #endregion Class: EffectBase

}